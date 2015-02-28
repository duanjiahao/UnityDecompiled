using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class SceneHierarchyWindow : SearchableEditorWindow
	{
		private class Styles
		{
			private const string kCustomSorting = "CustomSorting";
			private const string kWarningSymbol = "console.warnicon.sml";
			private const string kWarningMessage = "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.";
			public GUIContent defaultSortingContent = new GUIContent(EditorGUIUtility.FindTexture("CustomSorting"));
			public GUIContent createContent = new GUIContent("Create");
			public GUIContent fetchWarning = new GUIContent(string.Empty, EditorGUIUtility.FindTexture("console.warnicon.sml"), "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.");
			public GUIStyle MiniButton;
			public Styles()
			{
				this.MiniButton = "ToolbarButton";
			}
		}
		private const float toolbarHeight = 17f;
		private static SceneHierarchyWindow.Styles s_Styles;
		private static List<SceneHierarchyWindow> s_SceneHierarchyWindow = new List<SceneHierarchyWindow>();
		private TreeView m_TreeView;
		[SerializeField]
		private TreeViewState m_TreeViewState;
		private int m_TreeViewKeyboardControlID;
		[SerializeField]
		private string m_CurrentSortMethod = string.Empty;
		[NonSerialized]
		private int m_LastFramedID = -1;
		private Dictionary<string, BaseHierarchySort> m_SortingObjects;
		private bool m_AllowAlphaNumericalSort;
		[NonSerialized]
		private bool m_DidSelectSearchResult;
		public static bool s_Debug = false;
		private string currentSortMethod
		{
			get
			{
				return this.m_CurrentSortMethod;
			}
			set
			{
				this.m_CurrentSortMethod = value;
				if (!this.m_SortingObjects.ContainsKey(this.m_CurrentSortMethod))
				{
					this.m_CurrentSortMethod = this.GetNameForType(typeof(TransformSort));
				}
				GameObjectTreeViewDataSource gameObjectTreeViewDataSource = (GameObjectTreeViewDataSource)this.treeView.data;
				gameObjectTreeViewDataSource.sortingState.sortingObject = this.m_SortingObjects[this.m_CurrentSortMethod];
				GameObjectsTreeViewDragging gameObjectsTreeViewDragging = (GameObjectsTreeViewDragging)this.treeView.dragging;
				gameObjectsTreeViewDragging.allowDragBetween = !gameObjectTreeViewDataSource.sortingState.implementsCompare;
			}
		}
		private bool hasSortMethods
		{
			get
			{
				return this.m_SortingObjects.Count > 1;
			}
		}
		private Rect treeViewRect
		{
			get
			{
				return new Rect(0f, 17f, base.position.width, base.position.height - 17f);
			}
		}
		private TreeView treeView
		{
			get
			{
				if (this.m_TreeView == null)
				{
					this.Init();
				}
				return this.m_TreeView;
			}
		}
		public static List<SceneHierarchyWindow> GetAllSceneHierarchyWindows()
		{
			return SceneHierarchyWindow.s_SceneHierarchyWindow;
		}
		private void Init()
		{
			if (this.m_TreeView != null)
			{
				return;
			}
			if (this.m_TreeViewState == null)
			{
				this.m_TreeViewState = new TreeViewState();
			}
			this.m_TreeView = new TreeView(this, this.m_TreeViewState);
			TreeView expr_3A = this.m_TreeView;
			expr_3A.itemDoubleClickedCallback = (Action<int>)Delegate.Combine(expr_3A.itemDoubleClickedCallback, new Action<int>(this.TreeViewItemDoubleClicked));
			TreeView expr_61 = this.m_TreeView;
			expr_61.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_61.selectionChangedCallback, new Action<int[]>(this.TreeViewSelectionChanged));
			TreeView expr_88 = this.m_TreeView;
			expr_88.onGUIRowCallback = (Action<int, Rect>)Delegate.Combine(expr_88.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
			TreeView expr_AF = this.m_TreeView;
			expr_AF.dragEndedCallback = (Action<int[], bool>)Delegate.Combine(expr_AF.dragEndedCallback, new Action<int[], bool>(this.OnDragEndedCallback));
			this.m_TreeView.deselectOnUnhandledMouseDown = true;
			GameObjectTreeViewDataSource gameObjectTreeViewDataSource = new GameObjectTreeViewDataSource(this.m_TreeView, 0, false, false);
			GameObjectsTreeViewDragging dragging = new GameObjectsTreeViewDragging(this.m_TreeView);
			GameObjectTreeViewGUI gui = new GameObjectTreeViewGUI(this.m_TreeView, false);
			this.m_TreeView.Init(this.treeViewRect, gameObjectTreeViewDataSource, gui, dragging);
			gameObjectTreeViewDataSource.searchMode = (int)this.m_SearchMode;
			gameObjectTreeViewDataSource.searchString = this.m_SearchFilter;
			this.m_AllowAlphaNumericalSort = (EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false) || InternalEditorUtility.inBatchMode);
			this.SetUpSortMethodLists();
			this.m_TreeView.ReloadData();
		}
		internal void SelectPrevious()
		{
			this.m_TreeView.OffsetSelection(-1);
		}
		internal void SelectNext()
		{
			this.m_TreeView.OffsetSelection(1);
		}
		public UnityEngine.Object[] GetCurrentVisibleObjects()
		{
			List<TreeViewItem> visibleRows = this.m_TreeView.data.GetVisibleRows();
			UnityEngine.Object[] array = new UnityEngine.Object[visibleRows.Count];
			for (int i = 0; i < visibleRows.Count; i++)
			{
				array[i] = ((GameObjectTreeViewItem)visibleRows[i]).objectPPTR;
			}
			return array;
		}
		private void Awake()
		{
			this.m_HierarchyType = HierarchyType.GameObjects;
		}
		private void OnBecameVisible()
		{
			this.ReloadData();
		}
		public override void OnEnable()
		{
			base.OnEnable();
			SceneHierarchyWindow.s_SceneHierarchyWindow.Add(this);
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
		}
		public override void OnDisable()
		{
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
			SceneHierarchyWindow.s_SceneHierarchyWindow.Remove(this);
		}
		private void OnGUI()
		{
			if (SceneHierarchyWindow.s_Styles == null)
			{
				SceneHierarchyWindow.s_Styles = new SceneHierarchyWindow.Styles();
			}
			this.m_TreeViewKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.OnEvent();
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
			Event current = Event.current;
			if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
			{
				this.treeView.EndPing();
			}
			this.DoToolbar();
			float searchPathHeight = this.DoSearchResultPathGUI();
			this.DoTreeView(searchPathHeight);
			this.ExecuteCommands();
			this.HandleContextClick();
		}
		private void OnLostFocus()
		{
			this.treeView.EndNameEditing(true);
			EditorGUI.EndEditingActiveTextField();
		}
		private void TreeViewItemDoubleClicked(int instanceID)
		{
			SceneView.FrameLastActiveSceneView();
		}
		private void TreeViewSelectionChanged(int[] ids)
		{
			Selection.instanceIDs = ids;
			this.m_DidSelectSearchResult = !string.IsNullOrEmpty(this.m_SearchFilter);
		}
		public void SetExpandedRecursive(int id, bool expand)
		{
			TreeViewItem treeViewItem = this.treeView.data.FindItem(id);
			if (treeViewItem == null)
			{
				this.ReloadData();
				treeViewItem = this.treeView.data.FindItem(id);
			}
			if (treeViewItem != null)
			{
				this.treeView.data.SetExpandedWithChildren(treeViewItem, expand);
			}
		}
		private void OnGUIAssetCallback(int instanceID, Rect rect)
		{
			if (EditorApplication.hierarchyWindowItemOnGUI != null)
			{
				EditorApplication.hierarchyWindowItemOnGUI(instanceID, rect);
			}
		}
		private void OnDragEndedCallback(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
		{
			if (draggedInstanceIds != null && draggedItemsFromOwnTreeView)
			{
				this.ReloadData();
				this.treeView.SetSelection(draggedInstanceIds, true);
				this.treeView.NotifyListenersThatSelectionChanged();
				base.Repaint();
				GUIUtility.ExitGUI();
			}
		}
		public void ReloadData()
		{
			this.treeView.ReloadData();
		}
		public void SearchChanged()
		{
			GameObjectTreeViewDataSource gameObjectTreeViewDataSource = (GameObjectTreeViewDataSource)this.treeView.data;
			if (gameObjectTreeViewDataSource.searchMode == (int)base.searchMode && gameObjectTreeViewDataSource.searchString == this.m_SearchFilter)
			{
				return;
			}
			gameObjectTreeViewDataSource.searchMode = (int)base.searchMode;
			gameObjectTreeViewDataSource.searchString = this.m_SearchFilter;
			if (this.m_SearchFilter == string.Empty)
			{
				this.treeView.Frame(Selection.activeInstanceID, true, false);
			}
			this.ReloadData();
		}
		private void OnSelectionChange()
		{
			this.treeView.SetSelection(Selection.instanceIDs, true);
			base.Repaint();
		}
		private void OnHierarchyChange()
		{
			this.treeView.EndNameEditing(false);
			this.ReloadData();
		}
		private float DoSearchResultPathGUI()
		{
			if (!base.hasSearchFilter)
			{
				return 0f;
			}
			GUILayout.FlexibleSpace();
			Rect rect = EditorGUILayout.BeginVertical(EditorStyles.inspectorBig, new GUILayoutOption[0]);
			GUILayout.Label("Path:", new GUILayoutOption[0]);
			if (this.m_TreeView.HasSelection())
			{
				int instanceID = this.m_TreeView.GetSelection()[0];
				IHierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.GameObjects);
				hierarchyProperty.Find(instanceID, null);
				if (hierarchyProperty.isValid)
				{
					do
					{
						EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
						GUILayout.Label(hierarchyProperty.icon, new GUILayoutOption[0]);
						GUILayout.Label(hierarchyProperty.name, new GUILayoutOption[0]);
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
					}
					while (hierarchyProperty.Parent());
				}
			}
			EditorGUILayout.EndVertical();
			GUILayout.Space(0f);
			return rect.height;
		}
		private void OnEvent()
		{
			this.treeView.OnEvent();
		}
		private void DoTreeView(float searchPathHeight)
		{
			Rect treeViewRect = this.treeViewRect;
			treeViewRect.height -= searchPathHeight;
			this.treeView.OnGUI(treeViewRect, this.m_TreeViewKeyboardControlID);
		}
		private void DoToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			this.CreateGameObjectPopup();
			GUILayout.Space(6f);
			if (SceneHierarchyWindow.s_Debug)
			{
				GUILayout.Label(string.Empty + this.m_TreeView.data.GetVisibleRows().Count, EditorStyles.miniLabel, new GUILayoutOption[0]);
				GUILayout.Space(6f);
			}
			GUILayout.FlexibleSpace();
			Event current = Event.current;
			if (base.hasSearchFilterFocus && current.type == EventType.KeyDown && (current.keyCode == KeyCode.DownArrow || current.keyCode == KeyCode.UpArrow))
			{
				GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
				if (this.treeView.IsLastClickedPartOfVisibleRows())
				{
					this.treeView.Frame(this.treeView.state.lastClickedID, true, false);
					this.m_DidSelectSearchResult = !string.IsNullOrEmpty(this.m_SearchFilter);
				}
				else
				{
					this.treeView.OffsetSelection(1);
				}
				current.Use();
			}
			base.SearchFieldGUI();
			GUILayout.Space(6f);
			if (this.hasSortMethods)
			{
				if (Application.isPlaying && ((GameObjectTreeViewDataSource)this.treeView.data).isFetchAIssue)
				{
					GUILayout.Toggle(false, SceneHierarchyWindow.s_Styles.fetchWarning, SceneHierarchyWindow.s_Styles.MiniButton, new GUILayoutOption[0]);
				}
				this.SortMethodsDropDown();
			}
			GUILayout.EndHorizontal();
		}
		internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode, bool setAll)
		{
			base.SetSearchFilter(searchFilter, searchMode, setAll);
			if (this.m_DidSelectSearchResult && string.IsNullOrEmpty(searchFilter))
			{
				this.m_DidSelectSearchResult = false;
				this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
				if (GUIUtility.keyboardControl == 0)
				{
					GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
				}
			}
		}
		private void AddCreateGameObjectItemsToMenu(GenericMenu menu, UnityEngine.Object[] context, bool includeCreateEmptyChild)
		{
			string[] submenus = Unsupported.GetSubmenus("GameObject");
			string[] array = submenus;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				UnityEngine.Object[] temporaryContext = context;
				if (includeCreateEmptyChild || !(text.ToLower() == "GameObject/Create Empty Child".ToLower()))
				{
					if (text.EndsWith("..."))
					{
						temporaryContext = null;
					}
					if (text.ToLower() == "GameObject/Center On Children".ToLower())
					{
						return;
					}
					MenuUtils.ExtractMenuItemWithPath(text, menu, text.Substring(11), temporaryContext);
				}
			}
		}
		private void CreateGameObjectPopup()
		{
			Rect rect = GUILayoutUtility.GetRect(SceneHierarchyWindow.s_Styles.createContent, EditorStyles.toolbarDropDown, null);
			if (Event.current.type == EventType.Repaint)
			{
				EditorStyles.toolbarDropDown.Draw(rect, SceneHierarchyWindow.s_Styles.createContent, false, false, false, false);
			}
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
			{
				GUIUtility.hotControl = 0;
				GenericMenu genericMenu = new GenericMenu();
				this.AddCreateGameObjectItemsToMenu(genericMenu, null, true);
				genericMenu.DropDown(rect);
				Event.current.Use();
			}
		}
		private void SortMethodsDropDown()
		{
			if (this.hasSortMethods)
			{
				GUIContent gUIContent = this.m_SortingObjects[this.currentSortMethod].content;
				if (gUIContent == null)
				{
					gUIContent = SceneHierarchyWindow.s_Styles.defaultSortingContent;
					gUIContent.tooltip = this.currentSortMethod;
				}
				Rect rect = GUILayoutUtility.GetRect(gUIContent, EditorStyles.toolbarButton);
				if (EditorGUI.ButtonMouseDown(rect, gUIContent, FocusType.Passive, EditorStyles.toolbarButton))
				{
					List<SceneHierarchySortingWindow.InputData> list = new List<SceneHierarchySortingWindow.InputData>();
					foreach (KeyValuePair<string, BaseHierarchySort> current in this.m_SortingObjects)
					{
						list.Add(new SceneHierarchySortingWindow.InputData
						{
							m_TypeName = current.Key,
							m_Name = ObjectNames.NicifyVariableName(current.Key),
							m_Selected = current.Key == this.m_CurrentSortMethod
						});
					}
					if (SceneHierarchySortingWindow.ShowAtPosition(new Vector2(rect.x, rect.y + rect.height), list, new SceneHierarchySortingWindow.OnSelectCallback(this.SortFunctionCallback)))
					{
						GUIUtility.ExitGUI();
					}
				}
			}
		}
		private void SetUpSortMethodLists()
		{
			this.m_SortingObjects = new Dictionary<string, BaseHierarchySort>();
			Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
			for (int i = 0; i < loadedAssemblies.Length; i++)
			{
				Assembly assembly = loadedAssemblies[i];
				foreach (BaseHierarchySort current in AssemblyHelper.FindImplementors<BaseHierarchySort>(assembly))
				{
					if (current.GetType() != typeof(AlphabeticalSort) || this.m_AllowAlphaNumericalSort)
					{
						string nameForType = this.GetNameForType(current.GetType());
						this.m_SortingObjects.Add(nameForType, current);
					}
				}
			}
			this.currentSortMethod = this.m_CurrentSortMethod;
		}
		private string GetNameForType(Type type)
		{
			return type.Name;
		}
		private void SortFunctionCallback(SceneHierarchySortingWindow.InputData data)
		{
			this.SetSortFunction(data.m_TypeName);
		}
		public void SetSortFunction(Type sortType)
		{
			this.SetSortFunction(this.GetNameForType(sortType));
		}
		private void SetSortFunction(string sortTypeName)
		{
			if (!this.m_SortingObjects.ContainsKey(sortTypeName))
			{
				Debug.LogError("Invalid search type name: " + sortTypeName);
				return;
			}
			this.currentSortMethod = sortTypeName;
			if (this.treeView.GetSelection().Any<int>())
			{
				this.treeView.Frame(this.treeView.GetSelection().First<int>(), true, false);
			}
			this.treeView.ReloadData();
		}
		public void DirtySortingMethods()
		{
			this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
			this.SetUpSortMethodLists();
			this.treeView.SetSelection(this.treeView.GetSelection(), true);
			this.treeView.ReloadData();
		}
		private void ExecuteCommands()
		{
			Event current = Event.current;
			if (current.type != EventType.ExecuteCommand && current.type != EventType.ValidateCommand)
			{
				return;
			}
			bool flag = current.type == EventType.ExecuteCommand;
			if (current.commandName == "Delete" || current.commandName == "SoftDelete")
			{
				if (flag)
				{
					this.DeleteGO();
				}
				current.Use();
				GUIUtility.ExitGUI();
			}
			else
			{
				if (current.commandName == "Duplicate")
				{
					if (flag)
					{
						this.DuplicateGO();
					}
					current.Use();
					GUIUtility.ExitGUI();
				}
				else
				{
					if (current.commandName == "Copy")
					{
						if (flag)
						{
							this.CopyGO();
						}
						current.Use();
						GUIUtility.ExitGUI();
					}
					else
					{
						if (current.commandName == "Paste")
						{
							if (flag)
							{
								this.PasteGO();
							}
							current.Use();
							GUIUtility.ExitGUI();
						}
						else
						{
							if (current.commandName == "SelectAll")
							{
								if (flag)
								{
									this.SelectAll();
								}
								current.Use();
								GUIUtility.ExitGUI();
							}
							else
							{
								if (current.commandName == "FrameSelected")
								{
									if (current.type == EventType.ExecuteCommand)
									{
										this.FrameObjectPrivate(Selection.activeInstanceID, true, true);
									}
									current.Use();
									GUIUtility.ExitGUI();
								}
								else
								{
									if (current.commandName == "Find")
									{
										if (current.type == EventType.ExecuteCommand)
										{
											base.FocusSearchField();
										}
										current.Use();
									}
								}
							}
						}
					}
				}
			}
		}
		private void HandleContextClick()
		{
			Event current = Event.current;
			if (current.type != EventType.ContextClick)
			{
				return;
			}
			current.Use();
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupCopy"), false, new GenericMenu.MenuFunction(this.CopyGO));
			genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupPaste"), false, new GenericMenu.MenuFunction(this.PasteGO));
			genericMenu.AddSeparator(string.Empty);
			if (!base.hasSearchFilter && this.m_TreeViewState.selectedIDs.Count == 1)
			{
				genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupRename"), false, new GenericMenu.MenuFunction(this.RenameGO));
			}
			else
			{
				genericMenu.AddDisabledItem(EditorGUIUtility.TextContent("HierarchyPopupRename"));
			}
			genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupDuplicate"), false, new GenericMenu.MenuFunction(this.DuplicateGO));
			genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupDelete"), false, new GenericMenu.MenuFunction(this.DeleteGO));
			genericMenu.AddSeparator(string.Empty);
			bool flag = false;
			if (this.m_TreeViewState.selectedIDs.Count == 1)
			{
				GameObjectTreeViewItem gameObjectTreeViewItem = this.treeView.FindNode(this.m_TreeViewState.selectedIDs[0]) as GameObjectTreeViewItem;
				if (gameObjectTreeViewItem != null)
				{
					UnityEngine.Object prefab = PrefabUtility.GetPrefabParent(gameObjectTreeViewItem.objectPPTR);
					if (prefab != null)
					{
						genericMenu.AddItem(EditorGUIUtility.TextContent("HierarchyPopupSelectPrefab"), false, delegate
						{
							Selection.activeObject = prefab;
							EditorGUIUtility.PingObject(prefab.GetInstanceID());
						});
						flag = true;
					}
				}
			}
			if (!flag)
			{
				genericMenu.AddDisabledItem(EditorGUIUtility.TextContent("HierarchyPopupSelectPrefab"));
			}
			genericMenu.AddSeparator(string.Empty);
			this.AddCreateGameObjectItemsToMenu(genericMenu, Selection.objects, false);
			genericMenu.ShowAsContext();
		}
		private void CopyGO()
		{
			Unsupported.CopyGameObjectsToPasteboard();
		}
		private void PasteGO()
		{
			Unsupported.PasteGameObjectsFromPasteboard();
		}
		private void DuplicateGO()
		{
			Unsupported.DuplicateGameObjectsUsingPasteboard();
		}
		private void RenameGO()
		{
			this.treeView.BeginNameEditing(0f);
		}
		private void DeleteGO()
		{
			Unsupported.DeleteGameObjectSelection();
		}
		private void SelectAll()
		{
			int[] visibleRowIDs = this.treeView.GetVisibleRowIDs();
			this.treeView.SetSelection(visibleRowIDs, false);
			this.TreeViewSelectionChanged(visibleRowIDs);
		}
		public void FrameObject(int instanceID, bool ping)
		{
			this.FrameObjectPrivate(instanceID, true, ping);
		}
		private void FrameObjectPrivate(int instanceID, bool frame, bool ping)
		{
			if (instanceID == 0)
			{
				return;
			}
			if (this.m_LastFramedID != instanceID)
			{
				this.treeView.EndPing();
			}
			this.SetSearchFilter(string.Empty, SearchableEditorWindow.SearchMode.All, true);
			this.m_LastFramedID = instanceID;
			this.treeView.Frame(instanceID, frame, ping);
			this.FrameObjectPrivate(InternalEditorUtility.GetGameObjectInstanceIDFromComponent(instanceID), frame, ping);
		}
	}
}
