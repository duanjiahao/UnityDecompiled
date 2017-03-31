using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	internal class GameObjectTreeViewDataSource : LazyTreeViewDataSource
	{
		private const double k_LongFetchTime = 0.05;

		private const double k_FetchDelta = 0.1;

		private const int k_MaxDelayedFetch = 5;

		private const HierarchyType k_HierarchyType = HierarchyType.GameObjects;

		private readonly int kGameObjectClassID = UnityType.FindTypeByName("GameObject").persistentTypeID;

		private const int k_DefaultStartCapacity = 1000;

		private int m_RootInstanceID;

		private string m_SearchString = "";

		private int m_SearchMode = 0;

		private double m_LastFetchTime = 0.0;

		private int m_DelayedFetches = 0;

		private bool m_NeedsChildParentReferenceSetup;

		private bool m_RowsPartiallyInitialized;

		private int m_RowCount;

		private List<TreeViewItem> m_ListOfRows;

		private List<GameObjectTreeViewItem> m_StickySceneHeaderItems = new List<GameObjectTreeViewItem>();

		public HierarchySorting sortingState = new TransformSorting();

		public List<GameObjectTreeViewItem> sceneHeaderItems
		{
			get
			{
				return this.m_StickySceneHeaderItems;
			}
		}

		public string searchString
		{
			get
			{
				return this.m_SearchString;
			}
			set
			{
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(this.m_SearchString))
				{
					this.ClearSearchFilter();
				}
				this.m_SearchString = value;
			}
		}

		public int searchMode
		{
			get
			{
				return this.m_SearchMode;
			}
			set
			{
				this.m_SearchMode = value;
			}
		}

		public bool isFetchAIssue
		{
			get
			{
				return this.m_DelayedFetches >= 5;
			}
		}

		public override int rowCount
		{
			get
			{
				return this.m_RowCount;
			}
		}

		public GameObjectTreeViewDataSource(TreeViewController treeView, int rootInstanceID, bool showRoot, bool rootItemIsCollapsable) : base(treeView)
		{
			this.m_RootInstanceID = rootInstanceID;
			base.showRootItem = showRoot;
			base.rootIsCollapsable = rootItemIsCollapsable;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			GameObjectTreeViewGUI gameObjectTreeViewGUI = (GameObjectTreeViewGUI)this.m_TreeView.gui;
			gameObjectTreeViewGUI.scrollHeightChanged += new Action(this.EnsureFullyInitialized);
			gameObjectTreeViewGUI.scrollPositionChanged += new Action(this.EnsureFullyInitialized);
			gameObjectTreeViewGUI.mouseAndKeyboardInput += new Action(this.EnsureFullyInitialized);
		}

		internal void SetupChildParentReferencesIfNeeded()
		{
			this.EnsureFullyInitialized();
			if (this.m_NeedsChildParentReferenceSetup)
			{
				this.m_NeedsChildParentReferenceSetup = false;
				TreeViewUtility.SetChildParentReferences(this.GetRows(), this.m_RootItem);
			}
		}

		public void EnsureFullyInitialized()
		{
			if (this.m_RowsPartiallyInitialized)
			{
				this.InitializeFull();
				this.m_RowsPartiallyInitialized = false;
			}
		}

		public override void RevealItem(int itemID)
		{
			if (this.IsValidHierarchyInstanceID(itemID))
			{
				base.RevealItem(itemID);
			}
		}

		public override bool IsRevealed(int id)
		{
			return this.GetRow(id) != -1;
		}

		private bool IsValidHierarchyInstanceID(int instanceID)
		{
			bool flag = SceneHierarchyWindow.IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(instanceID));
			bool flag2 = InternalEditorUtility.GetClassIDWithoutLoadingObject(instanceID) == this.kGameObjectClassID;
			return flag || flag2;
		}

		private HierarchyProperty FindHierarchyProperty(int instanceID)
		{
			HierarchyProperty result;
			if (!this.IsValidHierarchyInstanceID(instanceID))
			{
				result = null;
			}
			else
			{
				HierarchyProperty hierarchyProperty = this.CreateHierarchyProperty();
				if (hierarchyProperty.Find(instanceID, this.m_TreeView.state.expandedIDs.ToArray()))
				{
					result = hierarchyProperty;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public override int GetRow(int id)
		{
			bool flag = !string.IsNullOrEmpty(this.m_SearchString);
			int result;
			if (flag)
			{
				result = base.GetRow(id);
			}
			else
			{
				HierarchyProperty hierarchyProperty = this.FindHierarchyProperty(id);
				if (hierarchyProperty != null)
				{
					result = hierarchyProperty.row;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		public override TreeViewItem GetItem(int row)
		{
			return this.m_Rows[row];
		}

		public override IList<TreeViewItem> GetRows()
		{
			this.InitIfNeeded();
			this.EnsureFullyInitialized();
			return this.m_Rows;
		}

		public override TreeViewItem FindItem(int id)
		{
			this.RevealItem(id);
			this.SetupChildParentReferencesIfNeeded();
			return base.FindItem(id);
		}

		private HierarchyProperty CreateHierarchyProperty()
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
			hierarchyProperty.Reset();
			hierarchyProperty.alphaSorted = this.IsUsingAlphaSort();
			return hierarchyProperty;
		}

		private void CreateRootItem(HierarchyProperty property)
		{
			int depth = 0;
			if (property.isValid)
			{
				this.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, property.name);
			}
			else
			{
				this.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, "RootOfAll");
			}
			if (!base.showRootItem)
			{
				this.SetExpanded(this.m_RootItem, true);
			}
		}

		private void ClearSearchFilter()
		{
			HierarchyProperty hierarchyProperty = this.CreateHierarchyProperty();
			hierarchyProperty.SetSearchFilter("", 0);
		}

		public override void FetchData()
		{
			Profiler.BeginSample("SceneHierarchyWindow.FetchData");
			this.m_RowsPartiallyInitialized = false;
			double timeSinceStartup = EditorApplication.timeSinceStartup;
			HierarchyProperty hierarchyProperty = this.CreateHierarchyProperty();
			if (this.m_RootInstanceID != 0)
			{
				if (!hierarchyProperty.Find(this.m_RootInstanceID, null))
				{
					Debug.LogError("Root gameobject with id " + this.m_RootInstanceID + " not found!!");
					this.m_RootInstanceID = 0;
					hierarchyProperty.Reset();
				}
			}
			this.CreateRootItem(hierarchyProperty);
			this.m_NeedRefreshRows = false;
			this.m_NeedsChildParentReferenceSetup = true;
			bool flag = this.m_RootInstanceID != 0;
			bool flag2 = !string.IsNullOrEmpty(this.m_SearchString);
			if (flag2 || flag)
			{
				if (flag2)
				{
					hierarchyProperty.SetSearchFilter(this.m_SearchString, this.m_SearchMode);
				}
				this.InitializeProgressivly(hierarchyProperty, flag, flag2);
			}
			else
			{
				this.InitializeMinimal();
			}
			double timeSinceStartup2 = EditorApplication.timeSinceStartup;
			double num = timeSinceStartup2 - timeSinceStartup;
			double num2 = timeSinceStartup2 - this.m_LastFetchTime;
			if (num2 > 0.1 && num > 0.05)
			{
				this.m_DelayedFetches++;
			}
			else
			{
				this.m_DelayedFetches = 0;
			}
			this.m_LastFetchTime = timeSinceStartup;
			this.m_TreeView.SetSelection(Selection.instanceIDs, false);
			this.CreateSceneHeaderItems();
			if (SceneHierarchyWindow.s_Debug)
			{
				Debug.Log(string.Concat(new object[]
				{
					"Fetch time: ",
					num * 1000.0,
					" ms, alphaSort = ",
					this.IsUsingAlphaSort()
				}));
			}
			Profiler.EndSample();
		}

		public override bool CanBeParent(TreeViewItem item)
		{
			this.SetupChildParentReferencesIfNeeded();
			return base.CanBeParent(item);
		}

		private bool IsUsingAlphaSort()
		{
			return this.sortingState.GetType() == typeof(AlphabeticalSorting);
		}

		private static void Resize(List<TreeViewItem> list, int count)
		{
			int count2 = list.Count;
			if (count < count2)
			{
				list.RemoveRange(count, count2 - count);
			}
			else if (count > count2)
			{
				if (count > list.Capacity)
				{
					list.Capacity = count + 20;
				}
				list.AddRange(Enumerable.Repeat<TreeViewItem>(null, count - count2));
			}
		}

		private void ResizeItemList(int count)
		{
			this.AllocateBackingArrayIfNeeded();
			if (this.m_ListOfRows.Count != count)
			{
				GameObjectTreeViewDataSource.Resize(this.m_ListOfRows, count);
			}
		}

		private void AllocateBackingArrayIfNeeded()
		{
			if (this.m_Rows == null)
			{
				int capacity = (this.m_RowCount <= 1000) ? 1000 : this.m_RowCount;
				this.m_ListOfRows = new List<TreeViewItem>(capacity);
				this.m_Rows = this.m_ListOfRows;
			}
		}

		private void InitializeMinimal()
		{
			int[] expanded = this.m_TreeView.state.expandedIDs.ToArray();
			HierarchyProperty hierarchyProperty = this.CreateHierarchyProperty();
			this.m_RowCount = hierarchyProperty.CountRemaining(expanded);
			this.ResizeItemList(this.m_RowCount);
			hierarchyProperty.Reset();
			if (SceneHierarchyWindow.debug)
			{
				GameObjectTreeViewDataSource.Log("Init minimal (" + this.m_RowCount + ")");
			}
			int firstRow;
			int lastRow;
			this.m_TreeView.gui.GetFirstAndLastRowVisible(out firstRow, out lastRow);
			this.InitializeRows(hierarchyProperty, firstRow, lastRow);
			this.m_RowsPartiallyInitialized = true;
		}

		private void InitializeFull()
		{
			if (SceneHierarchyWindow.debug)
			{
				GameObjectTreeViewDataSource.Log("Init full (" + this.m_RowCount + ")");
			}
			HierarchyProperty property = this.CreateHierarchyProperty();
			this.InitializeRows(property, 0, this.m_RowCount - 1);
		}

		private void InitializeProgressivly(HierarchyProperty property, bool subTreeWanted, bool isSearching)
		{
			this.AllocateBackingArrayIfNeeded();
			int num = (!subTreeWanted) ? 0 : (property.depth + 1);
			if (!isSearching)
			{
				int num2 = 0;
				int[] expanded = base.expandedIDs.ToArray();
				int num3 = (!subTreeWanted) ? 0 : (property.depth + 1);
				while (property.NextWithDepthCheck(expanded, num))
				{
					GameObjectTreeViewItem item = this.EnsureCreatedItem(num2);
					this.InitTreeViewItem(item, property, property.hasChildren, property.depth - num3);
					num2++;
				}
				this.m_RowCount = num2;
			}
			else
			{
				this.m_RowCount = this.InitializeSearchResults(property, num);
			}
			this.ResizeItemList(this.m_RowCount);
		}

		private int InitializeSearchResults(HierarchyProperty property, int minAllowedDepth)
		{
			int num = -1;
			int num2 = 0;
			List<int> list = new List<int>();
			while (property.NextWithDepthCheck(null, minAllowedDepth))
			{
				GameObjectTreeViewItem item = this.EnsureCreatedItem(num2);
				if (this.AddSceneHeaderToSearchIfNeeded(item, property, ref num))
				{
					num2++;
					list.Add(num2);
					if (this.IsSceneHeader(property))
					{
						continue;
					}
					item = this.EnsureCreatedItem(num2);
				}
				this.InitTreeViewItem(item, property, false, 0);
				num2++;
			}
			int num3 = num2;
			if (list.Count > 0)
			{
				int num4 = list[0];
				for (int i = 1; i < list.Count; i++)
				{
					int count = list[i] - num4 - 1;
					this.m_ListOfRows.Sort(num4, count, new TreeViewItemAlphaNumericSort());
					num4 = list[i];
				}
				this.m_ListOfRows.Sort(num4, num3 - num4, new TreeViewItemAlphaNumericSort());
			}
			return num3;
		}

		private bool AddSceneHeaderToSearchIfNeeded(GameObjectTreeViewItem item, HierarchyProperty property, ref int currentSceneHandle)
		{
			Scene scene = property.GetScene();
			bool result;
			if (currentSceneHandle != scene.handle)
			{
				currentSceneHandle = scene.handle;
				this.InitTreeViewItem(item, scene.handle, scene, true, 0, null, false, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private GameObjectTreeViewItem EnsureCreatedItem(int row)
		{
			if (row >= this.m_Rows.Count)
			{
				this.m_Rows.Add(null);
			}
			GameObjectTreeViewItem gameObjectTreeViewItem = (GameObjectTreeViewItem)this.m_Rows[row];
			if (gameObjectTreeViewItem == null)
			{
				gameObjectTreeViewItem = new GameObjectTreeViewItem(0, 0, null, null);
				this.m_Rows[row] = gameObjectTreeViewItem;
			}
			return gameObjectTreeViewItem;
		}

		private void InitializeRows(HierarchyProperty property, int firstRow, int lastRow)
		{
			property.Reset();
			int[] expanded = base.expandedIDs.ToArray();
			if (firstRow > 0)
			{
				if (!property.Skip(firstRow, expanded))
				{
					Debug.LogError("Failed to skip " + firstRow);
				}
			}
			int num = firstRow;
			while (property.Next(expanded) && num <= lastRow)
			{
				GameObjectTreeViewItem item = this.EnsureCreatedItem(num);
				this.InitTreeViewItem(item, property, property.hasChildren, property.depth);
				num++;
			}
		}

		private void InitTreeViewItem(GameObjectTreeViewItem item, HierarchyProperty property, bool itemHasChildren, int itemDepth)
		{
			this.InitTreeViewItem(item, property.instanceID, property.GetScene(), this.IsSceneHeader(property), property.colorCode, property.pptrValue, itemHasChildren, itemDepth);
		}

		private void InitTreeViewItem(GameObjectTreeViewItem item, int itemID, Scene scene, bool isSceneHeader, int colorCode, UnityEngine.Object pptrObject, bool hasChildren, int depth)
		{
			item.children = null;
			item.id = itemID;
			item.depth = depth;
			item.parent = null;
			if (isSceneHeader)
			{
				item.displayName = ((!string.IsNullOrEmpty(scene.name)) ? scene.name : "Untitled");
			}
			else
			{
				item.displayName = null;
			}
			item.colorCode = colorCode;
			item.objectPPTR = pptrObject;
			item.shouldDisplay = true;
			item.isSceneHeader = isSceneHeader;
			item.scene = scene;
			item.icon = ((!isSceneHeader) ? null : EditorGUIUtility.FindTexture("SceneAsset Icon"));
			if (hasChildren)
			{
				item.children = LazyTreeViewDataSource.CreateChildListForCollapsedParent();
			}
		}

		private bool IsSceneHeader(HierarchyProperty property)
		{
			return property.pptrValue == null;
		}

		protected override HashSet<int> GetParentsAbove(int id)
		{
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> result;
			if (!this.IsValidHierarchyInstanceID(id))
			{
				result = hashSet;
			}
			else
			{
				IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
				if (hierarchyProperty.Find(id, null))
				{
					while (hierarchyProperty.Parent())
					{
						hashSet.Add(hierarchyProperty.instanceID);
					}
				}
				result = hashSet;
			}
			return result;
		}

		protected override HashSet<int> GetParentsBelow(int id)
		{
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<int> result;
			if (!this.IsValidHierarchyInstanceID(id))
			{
				result = hashSet;
			}
			else
			{
				IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
				if (hierarchyProperty.Find(id, null))
				{
					hashSet.Add(id);
					int depth = hierarchyProperty.depth;
					while (hierarchyProperty.Next(null) && hierarchyProperty.depth > depth)
					{
						if (hierarchyProperty.hasChildren)
						{
							hashSet.Add(hierarchyProperty.instanceID);
						}
					}
				}
				result = hashSet;
			}
			return result;
		}

		private static void Log(string text)
		{
			Debug.Log(text);
		}

		private void CreateSceneHeaderItems()
		{
			this.m_StickySceneHeaderItems.Clear();
			int sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				GameObjectTreeViewItem item = new GameObjectTreeViewItem(0, 0, null, null);
				this.InitTreeViewItem(item, sceneAt.handle, sceneAt, true, 0, null, false, 0);
				this.m_StickySceneHeaderItems.Add(item);
			}
		}
	}
}
