using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[Serializable]
	internal class ObjectTreeForSelector
	{
		internal class TreeSelectorData
		{
			public ObjectTreeForSelector objectTreeForSelector;

			public EditorWindow editorWindow;

			public TreeViewState state;

			public Rect treeViewRect;

			public int userData;
		}

		[Serializable]
		public class SelectionEvent : UnityEvent<TreeViewItem>
		{
		}

		[Serializable]
		public class TreeViewNeededEvent : UnityEvent<ObjectTreeForSelector.TreeSelectorData>
		{
		}

		[Serializable]
		public class DoubleClickedEvent : UnityEvent
		{
		}

		private class Styles
		{
			public GUIStyle searchBg = new GUIStyle("ProjectBrowserTopBarBg");

			public GUIStyle bottomBarBg = new GUIStyle("ProjectBrowserBottomBarBg");

			public Styles()
			{
				this.searchBg.border = new RectOffset(0, 0, 2, 2);
				this.searchBg.fixedHeight = 0f;
				this.bottomBarBg.alignment = TextAnchor.MiddleLeft;
				this.bottomBarBg.fontSize = EditorStyles.label.fontSize;
				this.bottomBarBg.padding = new RectOffset(5, 5, 0, 0);
			}
		}

		private const string kSearchFieldTag = "TreeSearchField";

		private const float kBottomBarHeight = 17f;

		private const float kTopBarHeight = 27f;

		private EditorWindow m_Owner;

		private TreeView m_TreeView;

		private TreeViewState m_TreeViewState;

		private bool m_FocusSearchFilter;

		private int m_ErrorCounter;

		private int m_OriginalSelectedID;

		private int m_UserData;

		private int m_LastSelectedID = -1;

		private string m_SelectedPath = string.Empty;

		private ObjectTreeForSelector.SelectionEvent m_SelectionEvent;

		private ObjectTreeForSelector.TreeViewNeededEvent m_TreeViewNeededEvent;

		private ObjectTreeForSelector.DoubleClickedEvent m_DoubleClickedEvent;

		private static ObjectTreeForSelector.Styles s_Styles;

		public bool IsInitialized()
		{
			return this.m_Owner != null;
		}

		public void Init(Rect position, EditorWindow owner, UnityAction<ObjectTreeForSelector.TreeSelectorData> treeViewNeededCallback, UnityAction<TreeViewItem> selectionCallback, UnityAction doubleClickedCallback, int initialSelectedTreeViewItemID, int userData)
		{
			this.Clear();
			this.m_Owner = owner;
			this.m_TreeViewNeededEvent = new ObjectTreeForSelector.TreeViewNeededEvent();
			this.m_TreeViewNeededEvent.AddPersistentListener(treeViewNeededCallback, UnityEventCallState.EditorAndRuntime);
			this.m_SelectionEvent = new ObjectTreeForSelector.SelectionEvent();
			this.m_SelectionEvent.AddPersistentListener(selectionCallback, UnityEventCallState.EditorAndRuntime);
			this.m_DoubleClickedEvent = new ObjectTreeForSelector.DoubleClickedEvent();
			this.m_DoubleClickedEvent.AddPersistentListener(doubleClickedCallback, UnityEventCallState.EditorAndRuntime);
			this.m_OriginalSelectedID = initialSelectedTreeViewItemID;
			this.m_UserData = userData;
			this.m_FocusSearchFilter = true;
			this.EnsureTreeViewIsValid(this.GetTreeViewRect(position));
			if (this.m_TreeView != null)
			{
				this.m_TreeView.SetSelection(new int[]
				{
					this.m_OriginalSelectedID
				}, true);
				if (this.m_OriginalSelectedID == 0)
				{
					this.m_TreeView.data.SetExpandedWithChildren(this.m_TreeView.data.root, true);
				}
			}
		}

		public void Clear()
		{
			this.m_Owner = null;
			this.m_TreeViewNeededEvent = null;
			this.m_SelectionEvent = null;
			this.m_DoubleClickedEvent = null;
			this.m_OriginalSelectedID = 0;
			this.m_UserData = 0;
			this.m_TreeView = null;
			this.m_TreeViewState = null;
			this.m_ErrorCounter = 0;
			this.m_FocusSearchFilter = false;
		}

		public int[] GetSelection()
		{
			if (this.m_TreeView != null)
			{
				return this.m_TreeView.GetSelection();
			}
			return new int[0];
		}

		public void SetTreeView(TreeView treeView)
		{
			this.m_TreeView = treeView;
			TreeView expr_0D = this.m_TreeView;
			expr_0D.selectionChangedCallback = (Action<int[]>)Delegate.Remove(expr_0D.selectionChangedCallback, new Action<int[]>(this.OnItemSelectionChanged));
			TreeView expr_34 = this.m_TreeView;
			expr_34.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_34.selectionChangedCallback, new Action<int[]>(this.OnItemSelectionChanged));
			TreeView expr_5B = this.m_TreeView;
			expr_5B.itemDoubleClickedCallback = (Action<int>)Delegate.Remove(expr_5B.itemDoubleClickedCallback, new Action<int>(this.OnItemDoubleClicked));
			TreeView expr_82 = this.m_TreeView;
			expr_82.itemDoubleClickedCallback = (Action<int>)Delegate.Combine(expr_82.itemDoubleClickedCallback, new Action<int>(this.OnItemDoubleClicked));
		}

		private bool EnsureTreeViewIsValid(Rect treeViewRect)
		{
			if (this.m_TreeViewState == null)
			{
				this.m_TreeViewState = new TreeViewState();
			}
			if (this.m_TreeView == null)
			{
				ObjectTreeForSelector.TreeSelectorData arg = new ObjectTreeForSelector.TreeSelectorData
				{
					state = this.m_TreeViewState,
					treeViewRect = treeViewRect,
					userData = this.m_UserData,
					objectTreeForSelector = this,
					editorWindow = this.m_Owner
				};
				this.m_TreeViewNeededEvent.Invoke(arg);
				if (this.m_TreeView != null && this.m_TreeView.data.root == null)
				{
					this.m_TreeView.ReloadData();
				}
				if (this.m_TreeView == null)
				{
					if (this.m_ErrorCounter == 0)
					{
						Debug.LogError("ObjectTreeSelector is missing its tree view. Ensure to call 'SetTreeView()' when the treeViewNeededCallback is invoked!");
						this.m_ErrorCounter++;
					}
					return false;
				}
			}
			return true;
		}

		private Rect GetTreeViewRect(Rect position)
		{
			return new Rect(0f, 27f, position.width, position.height - 17f - 27f);
		}

		public void OnGUI(Rect position)
		{
			if (ObjectTreeForSelector.s_Styles == null)
			{
				ObjectTreeForSelector.s_Styles = new ObjectTreeForSelector.Styles();
			}
			Rect rect = new Rect(0f, 0f, position.width, position.height);
			Rect toolbarRect = new Rect(rect.x, rect.y, rect.width, 27f);
			Rect bottomRect = new Rect(rect.x, rect.yMax - 17f, rect.width, 17f);
			Rect treeViewRect = this.GetTreeViewRect(position);
			if (!this.EnsureTreeViewIsValid(treeViewRect))
			{
				return;
			}
			int controlID = GUIUtility.GetControlID("Tree".GetHashCode(), FocusType.Keyboard);
			this.HandleCommandEvents();
			this.HandleKeyboard(controlID);
			this.SearchArea(toolbarRect);
			this.TreeViewArea(treeViewRect, controlID);
			this.BottomBar(bottomRect);
		}

		private void BottomBar(Rect bottomRect)
		{
			int num = this.m_TreeView.GetSelection().FirstOrDefault<int>();
			if (num != this.m_LastSelectedID)
			{
				this.m_LastSelectedID = num;
				this.m_SelectedPath = string.Empty;
				TreeViewItem treeViewItem = this.m_TreeView.FindItem(num);
				if (treeViewItem != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					TreeViewItem treeViewItem2 = treeViewItem;
					while (treeViewItem2 != null && treeViewItem2 != this.m_TreeView.data.root)
					{
						if (treeViewItem2 != treeViewItem)
						{
							stringBuilder.Insert(0, "/");
						}
						stringBuilder.Insert(0, treeViewItem2.displayName);
						treeViewItem2 = treeViewItem2.parent;
					}
					this.m_SelectedPath = stringBuilder.ToString();
				}
			}
			GUI.Label(bottomRect, GUIContent.none, ObjectTreeForSelector.s_Styles.bottomBarBg);
			if (!string.IsNullOrEmpty(this.m_SelectedPath))
			{
				GUI.Label(bottomRect, GUIContent.Temp(this.m_SelectedPath), EditorStyles.miniLabel);
			}
		}

		private void OnItemDoubleClicked(int id)
		{
			if (this.m_DoubleClickedEvent != null)
			{
				this.m_DoubleClickedEvent.Invoke();
			}
		}

		private void OnItemSelectionChanged(int[] selection)
		{
			if (this.m_SelectionEvent != null)
			{
				TreeViewItem item = null;
				if (selection.Length > 0)
				{
					item = this.m_TreeView.FindItem(selection[0]);
				}
				this.FireSelectionEvent(item);
			}
		}

		private void HandleKeyboard(int treeViewControlID)
		{
			if (Event.current.type != EventType.KeyDown)
			{
				return;
			}
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode != KeyCode.UpArrow && keyCode != KeyCode.DownArrow)
			{
				return;
			}
			bool flag = GUI.GetNameOfFocusedControl() == "TreeSearchField";
			if (flag)
			{
				GUIUtility.keyboardControl = treeViewControlID;
				if (this.m_TreeView.IsLastClickedPartOfRows())
				{
					this.FrameSelectedTreeViewItem();
				}
				else
				{
					this.m_TreeView.OffsetSelection(1);
				}
				Event.current.Use();
			}
		}

		private void FrameSelectedTreeViewItem()
		{
			this.m_TreeView.Frame(this.m_TreeView.state.lastClickedID, true, false);
		}

		private void HandleCommandEvents()
		{
			Event current = Event.current;
			if (current.type != EventType.ExecuteCommand && current.type != EventType.ValidateCommand)
			{
				return;
			}
			if (current.commandName == "FrameSelected")
			{
				if (current.type == EventType.ExecuteCommand && this.m_TreeView.HasSelection())
				{
					this.m_TreeView.searchString = string.Empty;
					this.FrameSelectedTreeViewItem();
				}
				current.Use();
				GUIUtility.ExitGUI();
			}
			if (current.commandName == "Find")
			{
				if (current.type == EventType.ExecuteCommand)
				{
					this.FocusSearchField();
				}
				current.Use();
			}
		}

		private void FireSelectionEvent(TreeViewItem item)
		{
			if (this.m_SelectionEvent != null)
			{
				this.m_SelectionEvent.Invoke(item);
			}
		}

		private void TreeViewArea(Rect treeViewRect, int treeViewControlID)
		{
			bool flag = this.m_TreeView.data.rowCount > 0;
			if (flag)
			{
				this.m_TreeView.OnGUI(treeViewRect, treeViewControlID);
			}
		}

		private void SearchArea(Rect toolbarRect)
		{
			GUI.Label(toolbarRect, GUIContent.none, ObjectTreeForSelector.s_Styles.searchBg);
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			GUI.SetNextControlName("TreeSearchField");
			string text = EditorGUI.SearchField(new Rect(5f, 5f, toolbarRect.width - 10f, 15f), this.m_TreeView.searchString);
			if (flag && Event.current.type == EventType.Used && this.m_TreeView.searchString != string.Empty)
			{
				this.m_FocusSearchFilter = true;
			}
			if (text != this.m_TreeView.searchString || this.m_FocusSearchFilter)
			{
				this.m_TreeView.searchString = text;
				HandleUtility.Repaint();
			}
			if (this.m_FocusSearchFilter)
			{
				EditorGUI.FocusTextInControl("TreeSearchField");
				if (Event.current.type == EventType.Repaint)
				{
					this.m_FocusSearchFilter = false;
				}
			}
		}

		internal void FocusSearchField()
		{
			this.m_FocusSearchFilter = true;
		}
	}
}
