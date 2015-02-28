using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	internal class GameObjectTreeViewDataSource : LazyTreeViewDataSource
	{
		public class SortingState
		{
			private BaseHierarchySort m_HierarchySort;
			private bool m_ImplementsCompare;
			public BaseHierarchySort sortingObject
			{
				get
				{
					return this.m_HierarchySort;
				}
				set
				{
					this.m_HierarchySort = value;
					if (this.m_HierarchySort != null)
					{
						this.m_ImplementsCompare = (this.m_HierarchySort.GetType().GetMethod("Compare").DeclaringType != typeof(BaseHierarchySort));
					}
				}
			}
			public bool implementsCompare
			{
				get
				{
					return this.m_ImplementsCompare;
				}
			}
		}
		private const double k_LongFetchTime = 0.05;
		private const double k_FetchDelta = 0.1;
		private const int k_MaxDelayedFetch = 5;
		private const HierarchyType k_HierarchyType = HierarchyType.GameObjects;
		private readonly int m_RootInstanceID;
		private string m_SearchString = string.Empty;
		private int m_SearchMode;
		private double m_LastFetchTime;
		private int m_DelayedFetches;
		private bool m_NeedsChildParentReferenceSetup;
		public GameObjectTreeViewDataSource.SortingState sortingState = new GameObjectTreeViewDataSource.SortingState();
		public string searchString
		{
			get
			{
				return this.m_SearchString;
			}
			set
			{
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
		public GameObjectTreeViewDataSource(TreeView treeView, int rootInstanceID, bool showRootNode, bool rootNodeIsCollapsable) : base(treeView)
		{
			this.m_RootInstanceID = rootInstanceID;
			this.showRootNode = showRootNode;
			base.rootIsCollapsable = rootNodeIsCollapsable;
		}
		internal void SetupChildParentReferencesIfNeeded()
		{
			if (this.m_NeedsChildParentReferenceSetup)
			{
				this.m_NeedsChildParentReferenceSetup = false;
				TreeViewUtility.SetChildParentReferences(this.GetVisibleRows(), this.m_RootItem);
			}
		}
		public override TreeViewItem FindItem(int id)
		{
			this.RevealItem(id);
			this.SetupChildParentReferencesIfNeeded();
			return base.FindItem(id);
		}
		public override void FetchData()
		{
			Profiler.BeginSample("SceneHierarchyWindow.FetchData");
			int depth = 0;
			double timeSinceStartup = EditorApplication.timeSinceStartup;
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
			hierarchyProperty.Reset();
			hierarchyProperty.alphaSorted = this.IsUsingAlphaSort();
			if (this.m_RootInstanceID != 0)
			{
				bool flag = hierarchyProperty.Find(this.m_RootInstanceID, null);
				string displayName = (!flag) ? "RootOfSceneHierarchy" : hierarchyProperty.name;
				this.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, displayName);
				if (!flag)
				{
					Debug.LogError("Root gameobject with id " + this.m_RootInstanceID + " not found!!");
				}
			}
			else
			{
				this.m_RootItem = new GameObjectTreeViewItem(this.m_RootInstanceID, depth, null, "RootOfSceneHierarchy");
			}
			if (!base.showRootNode)
			{
				this.SetExpanded(this.m_RootItem, true);
			}
			bool flag2 = !string.IsNullOrEmpty(this.m_SearchString);
			if (flag2)
			{
				hierarchyProperty.SetSearchFilter(this.m_SearchString, this.m_SearchMode);
			}
			this.m_VisibleRows = this.CalcVisibleItems(hierarchyProperty, flag2);
			this.m_NeedsChildParentReferenceSetup = true;
			this.m_NeedRefreshVisibleFolders = false;
			if (this.sortingState.sortingObject != null && this.sortingState.implementsCompare)
			{
				this.SortVisibleRows();
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
		private bool IsUsingAlphaSort()
		{
			return this.sortingState.sortingObject.GetType() == typeof(AlphabeticalSort);
		}
		private List<TreeViewItem> CalcVisibleItems(HierarchyProperty property, bool hasSearchString)
		{
			int depth = property.depth;
			int[] expanded = base.expandedIDs.ToArray();
			List<TreeViewItem> list = new List<TreeViewItem>();
			while (property.NextWithDepthCheck(expanded, depth))
			{
				int adjustedItemDepth = this.GetAdjustedItemDepth(hasSearchString, depth, property.depth);
				GameObjectTreeViewItem item = this.CreateTreeViewItem(property, hasSearchString, adjustedItemDepth, true);
				list.Add(item);
			}
			return list;
		}
		private GameObjectTreeViewItem CreateTreeViewItem(HierarchyProperty property, bool hasSearchString, int depth, bool shouldDisplay)
		{
			GameObjectTreeViewItem gameObjectTreeViewItem = new GameObjectTreeViewItem(property.instanceID, depth, null, string.Empty);
			gameObjectTreeViewItem.colorCode = property.colorCode;
			gameObjectTreeViewItem.objectPPTR = property.pptrValue;
			gameObjectTreeViewItem.shouldDisplay = shouldDisplay;
			if (!hasSearchString && property.hasChildren)
			{
				gameObjectTreeViewItem.children = LazyTreeViewDataSource.CreateChildListForCollapsedParent();
			}
			return gameObjectTreeViewItem;
		}
		private int GetAdjustedItemDepth(bool hasSearchString, int minDepth, int adjPropertyDepth)
		{
			return (!hasSearchString) ? (adjPropertyDepth - minDepth) : 0;
		}
		protected override HashSet<int> GetParentsAbove(int id)
		{
			HashSet<int> hashSet = new HashSet<int>();
			IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
			if (hierarchyProperty.Find(id, null))
			{
				while (hierarchyProperty.Parent())
				{
					hashSet.Add(hierarchyProperty.instanceID);
				}
			}
			return hashSet;
		}
		protected override HashSet<int> GetParentsBelow(int id)
		{
			HashSet<int> hashSet = new HashSet<int>();
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
			return hashSet;
		}
		private void SortVisibleRows()
		{
			this.SetupChildParentReferencesIfNeeded();
			this.SortChildrenRecursively(this.m_RootItem, this.sortingState.sortingObject);
			this.m_VisibleRows.Clear();
			this.RebuildVisibilityTree(this.m_RootItem, this.m_VisibleRows);
		}
		private void SortChildrenRecursively(TreeViewItem item, BaseHierarchySort comparer)
		{
			if (item == null || !item.hasChildren)
			{
				return;
			}
			item.children = item.children.OrderBy((TreeViewItem x) => (x as GameObjectTreeViewItem).objectPPTR as GameObject, comparer).ToList<TreeViewItem>();
			for (int i = 0; i < item.children.Count; i++)
			{
				this.SortChildrenRecursively(item.children[i], comparer);
			}
		}
		private void RebuildVisibilityTree(TreeViewItem item, List<TreeViewItem> visibleItems)
		{
			if (item == null || !item.hasChildren)
			{
				return;
			}
			for (int i = 0; i < item.children.Count; i++)
			{
				if (item.children[i] != null)
				{
					visibleItems.Add(item.children[i]);
					this.RebuildVisibilityTree(item.children[i], visibleItems);
				}
			}
		}
	}
}
