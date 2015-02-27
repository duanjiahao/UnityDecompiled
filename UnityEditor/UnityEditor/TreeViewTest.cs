using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class TreeViewTest
	{
		public class FooTreeViewItem : TreeViewItem
		{
			public TreeViewTest.BackendData.Foo foo
			{
				get;
				private set;
			}
			public FooTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, TreeViewTest.BackendData.Foo foo) : base(id, depth, parent, displayName)
			{
				this.foo = foo;
			}
		}
		private class TestDataSource : TreeViewDataSource
		{
			private TreeViewTest.BackendData m_Backend;
			public int itemCounter
			{
				get;
				private set;
			}
			public TestDataSource(TreeView treeView, TreeViewTest.BackendData data) : base(treeView)
			{
				this.m_Backend = data;
				this.FetchData();
			}
			public override void FetchData()
			{
				this.itemCounter = 1;
				this.m_RootItem = new TreeViewTest.FooTreeViewItem(this.m_Backend.root.id, 0, null, this.m_Backend.root.name, this.m_Backend.root);
				this.AddChildrenRecursive(this.m_Backend.root, this.m_RootItem);
				this.m_NeedRefreshVisibleFolders = true;
			}
			private void AddChildrenRecursive(TreeViewTest.BackendData.Foo source, TreeViewItem dest)
			{
				if (source.hasChildren)
				{
					dest.children = new List<TreeViewItem>(source.children.Count);
					for (int i = 0; i < source.children.Count; i++)
					{
						TreeViewTest.BackendData.Foo foo = source.children[i];
						dest.children[i] = new TreeViewTest.FooTreeViewItem(foo.id, dest.depth + 1, dest, foo.name, foo);
						this.itemCounter++;
						this.AddChildrenRecursive(foo, dest.children[i]);
					}
				}
			}
			public override bool CanBeParent(TreeViewItem item)
			{
				return item.hasChildren;
			}
		}
		private class LazyTestDataSource : LazyTreeViewDataSource
		{
			private TreeViewTest.BackendData m_Backend;
			public int itemCounter
			{
				get;
				private set;
			}
			public LazyTestDataSource(TreeView treeView, TreeViewTest.BackendData data) : base(treeView)
			{
				this.m_Backend = data;
				this.FetchData();
			}
			public override void FetchData()
			{
				this.itemCounter = 1;
				this.m_RootItem = new TreeViewTest.FooTreeViewItem(this.m_Backend.root.id, 0, null, this.m_Backend.root.name, this.m_Backend.root);
				this.AddVisibleChildrenRecursive(this.m_Backend.root, this.m_RootItem);
				this.m_VisibleRows = new List<TreeViewItem>();
				base.GetVisibleItemsRecursive(this.m_RootItem, this.m_VisibleRows);
				this.m_NeedRefreshVisibleFolders = false;
			}
			private void AddVisibleChildrenRecursive(TreeViewTest.BackendData.Foo source, TreeViewItem dest)
			{
				if (this.IsExpanded(source.id))
				{
					if (source.children != null && source.children.Count > 0)
					{
						dest.children = new List<TreeViewItem>(source.children.Count);
						for (int i = 0; i < source.children.Count; i++)
						{
							TreeViewTest.BackendData.Foo foo = source.children[i];
							dest.children[i] = new TreeViewTest.FooTreeViewItem(foo.id, dest.depth + 1, dest, foo.name, foo);
							this.itemCounter++;
							this.AddVisibleChildrenRecursive(foo, dest.children[i]);
						}
					}
				}
				else
				{
					if (source.hasChildren)
					{
						dest.children = new List<TreeViewItem>
						{
							new TreeViewItem(-1, -1, null, string.Empty)
						};
					}
				}
			}
			public override bool CanBeParent(TreeViewItem item)
			{
				return item.hasChildren;
			}
			protected override HashSet<int> GetParentsAbove(int id)
			{
				HashSet<int> hashSet = new HashSet<int>();
				for (TreeViewTest.BackendData.Foo foo = TreeViewTest.BackendData.FindNodeRecursive(this.m_Backend.root, id); foo != null; foo = foo.parent)
				{
					if (foo.parent != null)
					{
						hashSet.Add(foo.parent.id);
					}
				}
				return hashSet;
			}
			protected override HashSet<int> GetParentsBelow(int id)
			{
				return this.m_Backend.GetParentsBelow(id);
			}
		}
		private class TestGUI : TreeViewGUI
		{
			private Texture2D m_FolderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			private Texture2D m_Icon = EditorGUIUtility.FindTexture("boo Script Icon");
			private GUIStyle m_LabelStyle;
			private GUIStyle m_LabelStyleRightAlign;
			private float[] columnWidths
			{
				get
				{
					return this.m_TreeView.state.columnWidths;
				}
			}
			public TestGUI(TreeView treeView) : base(treeView)
			{
			}
			protected override Texture GetIconForNode(TreeViewItem item)
			{
				return (!item.hasChildren) ? this.m_Icon : this.m_FolderIcon;
			}
			protected override void RenameEnded()
			{
			}
			protected override void SyncFakeItem()
			{
			}
			protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
			{
				if (this.m_LabelStyle == null)
				{
					this.m_LabelStyle = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
					RectOffset arg_3F_0 = this.m_LabelStyle.padding;
					int num = 6;
					this.m_LabelStyle.padding.right = num;
					arg_3F_0.left = num;
					this.m_LabelStyleRightAlign = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
					RectOffset arg_78_0 = this.m_LabelStyleRightAlign.padding;
					num = 6;
					this.m_LabelStyleRightAlign.padding.left = num;
					arg_78_0.right = num;
					this.m_LabelStyleRightAlign.alignment = TextAnchor.MiddleRight;
				}
				if (isPinging || this.columnWidths == null || this.columnWidths.Length == 0)
				{
					base.DrawIconAndLabel(rect, item, label, selected, focused, useBoldFont, isPinging);
					return;
				}
				Rect rect2 = rect;
				for (int i = 0; i < this.columnWidths.Length; i++)
				{
					rect2.width = this.columnWidths[i];
					if (i == 0)
					{
						base.DrawIconAndLabel(rect2, item, label, selected, focused, useBoldFont, isPinging);
					}
					else
					{
						GUI.Label(rect2, "Zksdf SDFS DFASDF ", (i % 2 != 0) ? this.m_LabelStyleRightAlign : this.m_LabelStyle);
					}
					rect2.x += rect2.width;
				}
			}
		}
		public class TestDragging : TreeViewDragging
		{
			private class FooDragData
			{
				public List<TreeViewItem> m_DraggedItems;
				public FooDragData(List<TreeViewItem> draggedItems)
				{
					this.m_DraggedItems = draggedItems;
				}
			}
			private const string k_GenericDragID = "FooDragging";
			private TreeViewTest.BackendData m_BackendData;
			public TestDragging(TreeView treeView, TreeViewTest.BackendData data) : base(treeView)
			{
				this.m_BackendData = data;
			}
			public override void StartDrag(TreeViewItem draggedNode, List<int> draggedItemIDs)
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.SetGenericData("FooDragging", new TreeViewTest.TestDragging.FooDragData(this.GetItemsFromIDs(draggedItemIDs)));
				DragAndDrop.objectReferences = new UnityEngine.Object[0];
				string title = draggedItemIDs.Count + " Foo" + ((draggedItemIDs.Count <= 1) ? string.Empty : "s");
				DragAndDrop.StartDrag(title);
			}
			public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
			{
				TreeViewTest.TestDragging.FooDragData fooDragData = DragAndDrop.GetGenericData("FooDragging") as TreeViewTest.TestDragging.FooDragData;
				TreeViewTest.FooTreeViewItem fooTreeViewItem = targetItem as TreeViewTest.FooTreeViewItem;
				TreeViewTest.FooTreeViewItem fooTreeViewItem2 = parentItem as TreeViewTest.FooTreeViewItem;
				if (fooTreeViewItem2 != null && fooDragData != null)
				{
					bool flag = this.ValidDrag(parentItem, fooDragData.m_DraggedItems);
					if (perform && flag)
					{
						List<TreeViewTest.BackendData.Foo> draggedItems = (
							from x in fooDragData.m_DraggedItems
							where x is TreeViewTest.FooTreeViewItem
							select ((TreeViewTest.FooTreeViewItem)x).foo).ToList<TreeViewTest.BackendData.Foo>();
						this.m_BackendData.ReparentSelection(fooTreeViewItem2.foo, fooTreeViewItem.foo, draggedItems);
						this.m_TreeView.ReloadData();
					}
					return (!flag) ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move;
				}
				return DragAndDropVisualMode.None;
			}
			private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
			{
				if (!parent.hasChildren)
				{
					return false;
				}
				for (TreeViewItem treeViewItem = parent; treeViewItem != null; treeViewItem = treeViewItem.parent)
				{
					if (draggedItems.Contains(treeViewItem))
					{
						return false;
					}
				}
				return true;
			}
			private List<TreeViewItem> GetItemsFromIDs(IEnumerable<int> draggedItemIDs)
			{
				return TreeViewUtility.FindItemsInList(draggedItemIDs, this.m_TreeView.data.GetVisibleRows());
			}
		}
		public class BackendData
		{
			public class Foo
			{
				public string name
				{
					get;
					set;
				}
				public int id
				{
					get;
					set;
				}
				public int depth
				{
					get;
					set;
				}
				public TreeViewTest.BackendData.Foo parent
				{
					get;
					set;
				}
				public List<TreeViewTest.BackendData.Foo> children
				{
					get;
					set;
				}
				public bool hasChildren
				{
					get
					{
						return this.children != null && this.children.Count > 0;
					}
				}
				public Foo(string name, int depth, int id)
				{
					this.name = name;
					this.depth = depth;
					this.id = id;
				}
			}
			private const int k_MinChildren = 3;
			private const int k_MaxChildren = 15;
			private const float k_ProbOfLastDescendent = 0.5f;
			private const int k_MaxDepth = 12;
			private TreeViewTest.BackendData.Foo m_Root;
			public bool m_RecursiveFindParentsBelow = true;
			private int m_MaxItems = 10000;
			public TreeViewTest.BackendData.Foo root
			{
				get
				{
					return this.m_Root;
				}
			}
			public int IDCounter
			{
				get;
				private set;
			}
			public void GenerateData(int maxNumItems)
			{
				this.m_MaxItems = maxNumItems;
				this.IDCounter = 1;
				this.m_Root = new TreeViewTest.BackendData.Foo("Root", 0, 0);
				while (this.IDCounter < this.m_MaxItems)
				{
					this.AddChildrenRecursive(this.m_Root, UnityEngine.Random.Range(3, 15), true);
				}
			}
			public HashSet<int> GetParentsBelow(int id)
			{
				TreeViewTest.BackendData.Foo foo = TreeViewTest.BackendData.FindNodeRecursive(this.root, id);
				if (foo == null)
				{
					return new HashSet<int>();
				}
				if (this.m_RecursiveFindParentsBelow)
				{
					return this.GetParentsBelowRecursive(foo);
				}
				return this.GetParentsBelowStackBased(foo);
			}
			private HashSet<int> GetParentsBelowStackBased(TreeViewTest.BackendData.Foo searchFromThis)
			{
				Stack<TreeViewTest.BackendData.Foo> stack = new Stack<TreeViewTest.BackendData.Foo>();
				stack.Push(searchFromThis);
				HashSet<int> hashSet = new HashSet<int>();
				while (stack.Count > 0)
				{
					TreeViewTest.BackendData.Foo foo = stack.Pop();
					if (foo.hasChildren)
					{
						hashSet.Add(foo.id);
						foreach (TreeViewTest.BackendData.Foo current in foo.children)
						{
							stack.Push(current);
						}
					}
				}
				return hashSet;
			}
			private HashSet<int> GetParentsBelowRecursive(TreeViewTest.BackendData.Foo searchFromThis)
			{
				HashSet<int> hashSet = new HashSet<int>();
				TreeViewTest.BackendData.GetParentsBelowRecursive(searchFromThis, hashSet);
				return hashSet;
			}
			private static void GetParentsBelowRecursive(TreeViewTest.BackendData.Foo item, HashSet<int> parentIDs)
			{
				if (!item.hasChildren)
				{
					return;
				}
				parentIDs.Add(item.id);
				foreach (TreeViewTest.BackendData.Foo current in item.children)
				{
					TreeViewTest.BackendData.GetParentsBelowRecursive(current, parentIDs);
				}
			}
			public void ReparentSelection(TreeViewTest.BackendData.Foo parentItem, TreeViewTest.BackendData.Foo insertAfterItem, List<TreeViewTest.BackendData.Foo> draggedItems)
			{
				foreach (TreeViewTest.BackendData.Foo current in draggedItems)
				{
					current.parent.children.Remove(current);
					current.parent = parentItem;
				}
				if (!parentItem.hasChildren)
				{
					parentItem.children = new List<TreeViewTest.BackendData.Foo>();
				}
				List<TreeViewTest.BackendData.Foo> list = new List<TreeViewTest.BackendData.Foo>(parentItem.children);
				int index = 0;
				if (parentItem == insertAfterItem)
				{
					index = 0;
				}
				else
				{
					int num = parentItem.children.IndexOf(insertAfterItem);
					if (num >= 0)
					{
						index = num + 1;
					}
					else
					{
						Debug.LogError("Did not find insertAfterItem, should be a child of parentItem!!");
					}
				}
				list.InsertRange(index, draggedItems);
				parentItem.children = list;
			}
			private void AddChildrenRecursive(TreeViewTest.BackendData.Foo foo, int numChildren, bool force)
			{
				if (this.IDCounter > this.m_MaxItems)
				{
					return;
				}
				if (foo.depth >= 12)
				{
					return;
				}
				if (!force && UnityEngine.Random.value < 0.5f)
				{
					return;
				}
				if (foo.children == null)
				{
					foo.children = new List<TreeViewTest.BackendData.Foo>(numChildren);
				}
				for (int i = 0; i < numChildren; i++)
				{
					TreeViewTest.BackendData.Foo foo2 = new TreeViewTest.BackendData.Foo("Tud" + this.IDCounter, foo.depth + 1, ++this.IDCounter);
					foo2.parent = foo;
					foo.children.Add(foo2);
				}
				if (this.IDCounter > this.m_MaxItems)
				{
					return;
				}
				foreach (TreeViewTest.BackendData.Foo current in foo.children)
				{
					this.AddChildrenRecursive(current, UnityEngine.Random.Range(3, 15), false);
				}
			}
			public static TreeViewTest.BackendData.Foo FindNodeRecursive(TreeViewTest.BackendData.Foo item, int id)
			{
				if (item == null)
				{
					return null;
				}
				if (item.id == id)
				{
					return item;
				}
				if (item.children == null)
				{
					return null;
				}
				foreach (TreeViewTest.BackendData.Foo current in item.children)
				{
					TreeViewTest.BackendData.Foo foo = TreeViewTest.BackendData.FindNodeRecursive(current, id);
					if (foo != null)
					{
						return foo;
					}
				}
				return null;
			}
		}
		internal class TreeViewColumnHeader
		{
			public float[] columnWidths
			{
				get;
				set;
			}
			public float minColumnWidth
			{
				get;
				set;
			}
			public float dragWidth
			{
				get;
				set;
			}
			public Action<int, Rect> columnRenderer
			{
				get;
				set;
			}
			public TreeViewColumnHeader()
			{
				this.minColumnWidth = 10f;
				this.dragWidth = 6f;
			}
			public void OnGUI(Rect rect)
			{
				float num = rect.x;
				for (int i = 0; i < this.columnWidths.Length; i++)
				{
					Rect arg = new Rect(num, rect.y, this.columnWidths[i], rect.height);
					num += this.columnWidths[i];
					Rect position = new Rect(num - this.dragWidth / 2f, rect.y, 3f, rect.height);
					float x = EditorGUI.MouseDeltaReader(position, true).x;
					if (x != 0f)
					{
						this.columnWidths[i] += x;
						this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
					}
					if (this.columnRenderer != null)
					{
						this.columnRenderer(i, arg);
					}
					if (Event.current.type == EventType.Repaint)
					{
						EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeLeftRight);
					}
				}
			}
		}
		private TreeViewTest.BackendData m_BackendData;
		private TreeView m_TreeView;
		private EditorWindow m_EditorWindow;
		private bool m_Lazy;
		private TreeViewTest.TreeViewColumnHeader m_ColumnHeader;
		private GUIStyle m_HeaderStyle;
		private GUIStyle m_HeaderStyleRightAligned;
		public TreeViewTest(EditorWindow editorWindow, bool lazy)
		{
			this.m_EditorWindow = editorWindow;
			this.m_Lazy = lazy;
		}
		public int GetNumItemsInData()
		{
			return this.m_BackendData.IDCounter;
		}
		public int GetNumItemsInTree()
		{
			TreeViewTest.LazyTestDataSource lazyTestDataSource = this.m_TreeView.data as TreeViewTest.LazyTestDataSource;
			if (lazyTestDataSource != null)
			{
				return lazyTestDataSource.itemCounter;
			}
			TreeViewTest.TestDataSource testDataSource = this.m_TreeView.data as TreeViewTest.TestDataSource;
			if (testDataSource != null)
			{
				return testDataSource.itemCounter;
			}
			return -1;
		}
		public void Init(Rect rect, TreeViewTest.BackendData backendData)
		{
			if (this.m_TreeView != null)
			{
				return;
			}
			this.m_BackendData = backendData;
			TreeViewState treeViewState = new TreeViewState();
			treeViewState.columnWidths = new float[]
			{
				250f,
				90f,
				93f,
				98f,
				74f,
				78f
			};
			this.m_TreeView = new TreeView(this.m_EditorWindow, treeViewState);
			ITreeViewGUI gui = new TreeViewTest.TestGUI(this.m_TreeView);
			ITreeViewDragging dragging = new TreeViewTest.TestDragging(this.m_TreeView, this.m_BackendData);
			ITreeViewDataSource data;
			if (this.m_Lazy)
			{
				data = new TreeViewTest.LazyTestDataSource(this.m_TreeView, this.m_BackendData);
			}
			else
			{
				data = new TreeViewTest.TestDataSource(this.m_TreeView, this.m_BackendData);
			}
			this.m_TreeView.Init(rect, data, gui, dragging);
			this.m_ColumnHeader = new TreeViewTest.TreeViewColumnHeader();
			this.m_ColumnHeader.columnWidths = treeViewState.columnWidths;
			this.m_ColumnHeader.minColumnWidth = 30f;
			TreeViewTest.TreeViewColumnHeader expr_D5 = this.m_ColumnHeader;
			expr_D5.columnRenderer = (Action<int, Rect>)Delegate.Combine(expr_D5.columnRenderer, new Action<int, Rect>(this.OnColumnRenderer));
		}
		private void OnColumnRenderer(int column, Rect rect)
		{
			if (this.m_HeaderStyle == null)
			{
				this.m_HeaderStyle = new GUIStyle(EditorStyles.toolbarButton);
				this.m_HeaderStyle.padding.left = 4;
				this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
				this.m_HeaderStyleRightAligned = new GUIStyle(EditorStyles.toolbarButton);
				this.m_HeaderStyleRightAligned.padding.right = 4;
				this.m_HeaderStyleRightAligned.alignment = TextAnchor.MiddleRight;
			}
			string[] array = new string[]
			{
				"Name",
				"Date Modified",
				"Size",
				"Kind",
				"Author",
				"Platform",
				"Faster",
				"Slower"
			};
			GUI.Label(rect, array[column], (column % 2 != 0) ? this.m_HeaderStyleRightAligned : this.m_HeaderStyle);
		}
		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, 17f);
			Rect screenRect = new Rect(rect.x, rect.yMax - 20f, rect.width, 20f);
			GUI.Label(rect2, string.Empty, EditorStyles.toolbar);
			this.m_ColumnHeader.OnGUI(rect2);
			Profiler.BeginSample("TREEVIEW");
			rect.y += rect2.height;
			rect.height -= rect2.height + screenRect.height;
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(rect, controlID);
			Profiler.EndSample();
			GUILayout.BeginArea(screenRect, this.GetHeader(), EditorStyles.helpBox);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			this.m_BackendData.m_RecursiveFindParentsBelow = GUILayout.Toggle(this.m_BackendData.m_RecursiveFindParentsBelow, GUIContent.Temp("Recursive"), new GUILayoutOption[0]);
			if (GUILayout.Button("Ping", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				int num = this.GetNumItemsInData() / 2;
				this.m_TreeView.Frame(num, true, true);
				this.m_TreeView.SetSelection(new int[]
				{
					num
				}, false);
			}
			if (GUILayout.Button("Frame", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				int num2 = this.GetNumItemsInData() / 10;
				this.m_TreeView.Frame(num2, true, false);
				this.m_TreeView.SetSelection(new int[]
				{
					num2
				}, false);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		private string GetHeader()
		{
			return string.Concat(new object[]
			{
				(!this.m_Lazy) ? "FULL: " : "LAZY: ",
				"GUI items: ",
				this.GetNumItemsInTree(),
				"  (data items: ",
				this.GetNumItemsInData(),
				")"
			});
		}
	}
}
