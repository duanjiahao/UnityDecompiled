using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.TreeViewExamples;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Project", icon = "Project")]
	internal class ProjectBrowser : EditorWindow, IHasCustomMenu
	{
		internal enum ItemType
		{
			Asset,
			SavedFilter
		}

		private enum ViewMode
		{
			OneColumn,
			TwoColumns
		}

		public enum SearchViewState
		{
			NotSearching,
			AllAssets,
			SubFolders,
			AssetStore
		}

		private class Styles
		{
			public GUIStyle smallStatus = "ObjectPickerSmallStatus";

			public GUIStyle largeStatus = "ObjectPickerLargeStatus";

			public GUIStyle toolbarBack = "ObjectPickerToolbar";

			public GUIStyle tab = "ObjectPickerTab";

			public GUIStyle bottomResize = "WindowBottomResize";

			public GUIStyle background = "ObjectPickerBackground";

			public GUIStyle previewBackground = "PopupCurveSwatchBackground";

			public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";

			public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";

			public GUIStyle topBarBg = "ProjectBrowserTopBarBg";

			public GUIStyle selectedPathLabel = "Label";

			public GUIStyle exposablePopup = ProjectBrowser.Styles.GetStyle("ExposablePopupMenu");

			public GUIStyle lockButton = "IN LockButton";

			public GUIStyle foldout = "AC RightArrow";

			public GUIStyle exposablePopupItem = ProjectBrowser.Styles.GetStyle("ExposablePopupItem");

			public GUIContent m_FilterByLabel = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"), "Search by Label");

			public GUIContent m_FilterByType = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Search by Type");

			public GUIContent m_ShowChildAssetsContent = new GUIContent("", EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow"), "Toggle visibility of child assets in folders");

			public GUIContent m_CreateDropdownContent = new GUIContent("Create");

			public GUIContent m_SaveFilterContent = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Save search");

			public GUIContent m_EmptyFolderText = new GUIContent("This folder is empty");

			public GUIContent m_SearchIn = new GUIContent("Search:");

			private static GUIStyle GetStyle(string styleName)
			{
				return styleName;
			}
		}

		internal class SavedFiltersContextMenu
		{
			private int m_SavedFilterInstanceID;

			private SavedFiltersContextMenu(int savedFilterInstanceID)
			{
				this.m_SavedFilterInstanceID = savedFilterInstanceID;
			}

			internal static void Show(int savedFilterInstanceID)
			{
				GUIContent content = new GUIContent("Delete");
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(content, false, new GenericMenu.MenuFunction(new ProjectBrowser.SavedFiltersContextMenu(savedFilterInstanceID).Delete));
				genericMenu.ShowAsContext();
			}

			private void Delete()
			{
				ProjectBrowser.DeleteFilter(this.m_SavedFilterInstanceID);
			}
		}

		internal class BreadCrumbListMenu
		{
			private static ProjectBrowser m_Caller;

			private string m_SubFolder;

			private BreadCrumbListMenu(string subFolder)
			{
				this.m_SubFolder = subFolder;
			}

			internal static void Show(string folder, string currentSubFolder, Rect activatorRect, ProjectBrowser caller)
			{
				ProjectBrowser.BreadCrumbListMenu.m_Caller = caller;
				string[] subFolders = AssetDatabase.GetSubFolders(folder);
				GenericMenu genericMenu = new GenericMenu();
				if (subFolders.Length >= 0)
				{
					currentSubFolder = Path.GetFileName(currentSubFolder);
					string[] array = subFolders;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						string fileName = Path.GetFileName(text);
						genericMenu.AddItem(new GUIContent(fileName), fileName == currentSubFolder, new GenericMenu.MenuFunction(new ProjectBrowser.BreadCrumbListMenu(text).SelectSubFolder));
						genericMenu.ShowAsContext();
					}
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent("No sub folders..."));
				}
				genericMenu.DropDown(activatorRect);
			}

			private void SelectSubFolder()
			{
				int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(this.m_SubFolder);
				if (mainAssetInstanceID != 0)
				{
					ProjectBrowser.BreadCrumbListMenu.m_Caller.ShowFolderContents(mainAssetInstanceID, false);
				}
			}
		}

		internal class AssetStoreItemContextMenu
		{
			private AssetStoreItemContextMenu()
			{
			}

			internal static void Show()
			{
				GenericMenu genericMenu = new GenericMenu();
				GUIContent content = new GUIContent("Show in Asset Store window");
				AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
				if (firstAsset != null && firstAsset.id != 0)
				{
					genericMenu.AddItem(content, false, new GenericMenu.MenuFunction(new ProjectBrowser.AssetStoreItemContextMenu().OpenAssetStoreWindow));
				}
				else
				{
					genericMenu.AddDisabledItem(content);
				}
				genericMenu.ShowAsContext();
			}

			private void OpenAssetStoreWindow()
			{
				AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
				if (firstAsset != null)
				{
					AssetStoreAssetInspector.OpenItemInAssetStore(firstAsset);
				}
			}
		}

		private static List<ProjectBrowser> s_ProjectBrowsers = new List<ProjectBrowser>();

		public static ProjectBrowser s_LastInteractedProjectBrowser;

		private static ProjectBrowser.Styles s_Styles;

		private static int s_HashForSearchField = "ProjectBrowserSearchField".GetHashCode();

		[SerializeField]
		private SearchFilter m_SearchFilter;

		[NonSerialized]
		private string m_SearchFieldText = "";

		[SerializeField]
		private ProjectBrowser.ViewMode m_ViewMode = ProjectBrowser.ViewMode.TwoColumns;

		[SerializeField]
		private int m_StartGridSize = 64;

		[SerializeField]
		private string[] m_LastFolders = new string[0];

		[SerializeField]
		private float m_LastFoldersGridSize = -1f;

		[SerializeField]
		private string m_LastProjectPath;

		[SerializeField]
		private bool m_IsLocked;

		private bool m_EnableOldAssetTree = true;

		private bool m_FocusSearchField;

		private string m_SelectedPath;

		private List<GUIContent> m_SelectedPathSplitted = new List<GUIContent>();

		private float m_LastListWidth;

		private int m_CurrentNumItems;

		private bool m_DidSelectSearchResult = false;

		private bool m_ItemSelectedByRightClickThisEvent = false;

		private bool m_InternalSelectionChange = false;

		private SearchFilter.SearchArea m_LastLocalAssetsSearchArea = SearchFilter.SearchArea.AllAssets;

		private PopupList.InputData m_AssetLabels;

		private PopupList.InputData m_ObjectTypes;

		private bool m_UseTreeViewSelectionInsteadOfMainSelection;

		[SerializeField]
		private TreeViewState m_FolderTreeState;

		private TreeViewController m_FolderTree;

		private int m_TreeViewKeyboardControlID;

		[SerializeField]
		private TreeViewState m_AssetTreeState;

		private TreeViewController m_AssetTree;

		[SerializeField]
		private ObjectListAreaState m_ListAreaState;

		private ObjectListArea m_ListArea;

		private int m_ListKeyboardControlID;

		private bool m_GrabKeyboardFocusForListArea = false;

		private List<KeyValuePair<GUIContent, string>> m_BreadCrumbs = new List<KeyValuePair<GUIContent, string>>();

		private bool m_BreadCrumbLastFolderHasSubFolders = false;

		private ExposablePopupMenu m_SearchAreaMenu;

		private const float k_MinHeight = 250f;

		private const float k_MinWidthOneColumn = 230f;

		private const float k_MinWidthTwoColumns = 230f;

		private float m_ToolbarHeight;

		private const float k_BottomBarHeight = 17f;

		private float k_MinDirectoriesAreaWidth = 110f;

		[SerializeField]
		private float m_DirectoriesAreaWidth = 115f;

		private const float k_ResizerWidth = 5f;

		private const float k_SliderWidth = 55f;

		[NonSerialized]
		private float m_SearchAreaMenuOffset = -1f;

		[NonSerialized]
		private Rect m_ListAreaRect;

		[NonSerialized]
		private Rect m_TreeViewRect;

		[NonSerialized]
		private Rect m_BottomBarRect;

		[NonSerialized]
		private Rect m_ListHeaderRect;

		[NonSerialized]
		private int m_LastFramedID = -1;

		[NonSerialized]
		public GUIContent m_SearchAllAssets = new GUIContent("Assets");

		[NonSerialized]
		public GUIContent m_SearchInFolders = new GUIContent("");

		[NonSerialized]
		public GUIContent m_SearchAssetStore = new GUIContent("Asset Store");

		private bool useTreeViewSelectionInsteadOfMainSelection
		{
			get
			{
				return this.m_UseTreeViewSelectionInsteadOfMainSelection;
			}
			set
			{
				this.m_UseTreeViewSelectionInsteadOfMainSelection = value;
			}
		}

		public float listAreaGridSize
		{
			get
			{
				return (float)this.m_ListArea.gridSize;
			}
		}

		private ProjectBrowser()
		{
		}

		public static List<ProjectBrowser> GetAllProjectBrowsers()
		{
			return ProjectBrowser.s_ProjectBrowsers;
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			ProjectBrowser.s_ProjectBrowsers.Add(this);
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
			EditorApplication.assetLabelsChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.assetLabelsChanged, new EditorApplication.CallbackFunction(this.OnAssetLabelsChanged));
			EditorApplication.assetBundleNameChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.assetBundleNameChanged, new EditorApplication.CallbackFunction(this.OnAssetBundleNameChanged));
			ProjectBrowser.s_LastInteractedProjectBrowser = this;
		}

		private void OnDisable()
		{
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
			EditorApplication.assetLabelsChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.assetLabelsChanged, new EditorApplication.CallbackFunction(this.OnAssetLabelsChanged));
			EditorApplication.assetBundleNameChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.assetBundleNameChanged, new EditorApplication.CallbackFunction(this.OnAssetBundleNameChanged));
			ProjectBrowser.s_ProjectBrowsers.Remove(this);
		}

		private void OnPlayModeStateChanged()
		{
			this.EndRenaming();
		}

		private void OnAssetLabelsChanged()
		{
			if (this.Initialized())
			{
				this.SetupAssetLabelList();
				if (this.m_SearchFilter.IsSearching())
				{
					this.InitListArea();
				}
			}
		}

		private void OnAssetBundleNameChanged()
		{
			if (this.m_ListArea != null)
			{
				this.InitListArea();
			}
		}

		private void Awake()
		{
			if (this.m_ListAreaState != null)
			{
				this.m_ListAreaState.OnAwake();
			}
			if (this.m_FolderTreeState != null)
			{
				this.m_FolderTreeState.OnAwake();
				this.m_FolderTreeState.expandedIDs = new List<int>(InternalEditorUtility.expandedProjectWindowItems);
			}
			if (this.m_AssetTreeState != null)
			{
				this.m_AssetTreeState.OnAwake();
				this.m_AssetTreeState.expandedIDs = new List<int>(InternalEditorUtility.expandedProjectWindowItems);
			}
			if (this.m_SearchFilter != null)
			{
				this.EnsureValidFolders();
			}
		}

		private string GetAnalyticsSizeLabel(float size)
		{
			string result;
			if (size > 600f)
			{
				result = "Larger than 600 pix";
			}
			else if (size < 240f)
			{
				result = "Less than 240 pix";
			}
			else
			{
				result = "240 - 600 pix";
			}
			return result;
		}

		internal static ProjectBrowser.ItemType GetItemType(int instanceID)
		{
			ProjectBrowser.ItemType result;
			if (SavedSearchFilters.IsSavedFilter(instanceID))
			{
				result = ProjectBrowser.ItemType.SavedFilter;
			}
			else
			{
				result = ProjectBrowser.ItemType.Asset;
			}
			return result;
		}

		internal string GetActiveFolderPath()
		{
			string result;
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing && this.m_SearchFilter.folders.Length > 0)
			{
				result = this.m_SearchFilter.folders[0];
			}
			else
			{
				result = "Assets";
			}
			return result;
		}

		private void EnsureValidFolders()
		{
			HashSet<string> hashSet = new HashSet<string>();
			string[] folders = this.m_SearchFilter.folders;
			for (int i = 0; i < folders.Length; i++)
			{
				string text = folders[i];
				if (AssetDatabase.IsValidFolder(text))
				{
					hashSet.Add(text);
				}
				else
				{
					string text2 = text;
					for (int j = 0; j < 30; j++)
					{
						if (string.IsNullOrEmpty(text2))
						{
							break;
						}
						text2 = ProjectWindowUtil.GetContainingFolder(text2);
						if (!string.IsNullOrEmpty(text2) && AssetDatabase.IsValidFolder(text2))
						{
							hashSet.Add(text2);
							break;
						}
					}
				}
			}
			this.m_SearchFilter.folders = hashSet.ToArray<string>();
		}

		private void OnProjectChanged()
		{
			if (this.m_AssetTree != null)
			{
				this.m_AssetTree.ReloadData();
				this.SetSearchFoldersFromCurrentSelection();
			}
			if (this.m_FolderTree != null)
			{
				this.m_FolderTree.ReloadData();
				this.SetSearchFolderFromFolderTreeSelection();
			}
			this.EnsureValidFolders();
			if (this.m_ListArea != null)
			{
				this.InitListArea();
			}
			this.RefreshSelectedPath();
			this.m_BreadCrumbs.Clear();
		}

		public bool Initialized()
		{
			return this.m_ListArea != null;
		}

		public void Init()
		{
			if (!this.Initialized())
			{
				this.m_FocusSearchField = false;
				bool flag = this.m_SearchFilter == null;
				if (flag)
				{
					this.m_DirectoriesAreaWidth = Mathf.Min(base.position.width / 2f, 200f);
				}
				if (this.m_SearchFilter == null)
				{
					this.m_SearchFilter = new SearchFilter();
				}
				this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
				this.CalculateRects();
				this.RefreshSelectedPath();
				this.SetupDroplists();
				if (this.m_ListAreaState == null)
				{
					this.m_ListAreaState = new ObjectListAreaState();
				}
				this.m_ListAreaState.m_RenameOverlay.isRenamingFilename = true;
				this.m_ListArea = new ObjectListArea(this.m_ListAreaState, this, false);
				this.m_ListArea.allowDeselection = true;
				this.m_ListArea.allowDragging = true;
				this.m_ListArea.allowFocusRendering = true;
				this.m_ListArea.allowMultiSelect = true;
				this.m_ListArea.allowRenaming = true;
				this.m_ListArea.allowBuiltinResources = false;
				this.m_ListArea.allowUserRenderingHook = true;
				this.m_ListArea.allowFindNextShortcut = true;
				this.m_ListArea.foldersFirst = this.GetShouldShowFoldersFirst();
				ObjectListArea expr_136 = this.m_ListArea;
				expr_136.repaintCallback = (Action)Delegate.Combine(expr_136.repaintCallback, new Action(base.Repaint));
				ObjectListArea expr_15D = this.m_ListArea;
				expr_15D.itemSelectedCallback = (Action<bool>)Delegate.Combine(expr_15D.itemSelectedCallback, new Action<bool>(this.ListAreaItemSelectedCallback));
				ObjectListArea expr_184 = this.m_ListArea;
				expr_184.keyboardCallback = (Action)Delegate.Combine(expr_184.keyboardCallback, new Action(this.ListAreaKeyboardCallback));
				ObjectListArea expr_1AB = this.m_ListArea;
				expr_1AB.gotKeyboardFocus = (Action)Delegate.Combine(expr_1AB.gotKeyboardFocus, new Action(this.ListGotKeyboardFocus));
				ObjectListArea expr_1D2 = this.m_ListArea;
				expr_1D2.drawLocalAssetHeader = (Func<Rect, float>)Delegate.Combine(expr_1D2.drawLocalAssetHeader, new Func<Rect, float>(this.DrawLocalAssetHeader));
				ObjectListArea expr_1F9 = this.m_ListArea;
				expr_1F9.assetStoreSearchEnded = (Action)Delegate.Combine(expr_1F9.assetStoreSearchEnded, new Action(this.AssetStoreSearchEndedCallback));
				this.m_ListArea.gridSize = this.m_StartGridSize;
				this.m_StartGridSize = Mathf.Clamp(this.m_StartGridSize, this.m_ListArea.minGridSize, this.m_ListArea.maxGridSize);
				this.m_LastFoldersGridSize = Mathf.Min(this.m_LastFoldersGridSize, (float)this.m_ListArea.maxGridSize);
				this.m_SearchAreaMenu = new ExposablePopupMenu();
				if (this.m_FolderTreeState == null)
				{
					this.m_FolderTreeState = new TreeViewState();
				}
				this.m_FolderTreeState.renameOverlay.isRenamingFilename = true;
				if (this.m_AssetTreeState == null)
				{
					this.m_AssetTreeState = new TreeViewState();
				}
				this.m_AssetTreeState.renameOverlay.isRenamingFilename = true;
				this.InitViewMode(this.m_ViewMode);
				this.EnsureValidSetup();
				this.RefreshSearchText();
				this.SyncFilterGUI();
				this.InitListArea();
			}
		}

		public void SetSearch(string searchString)
		{
			this.SetSearch(SearchFilter.CreateSearchFilterFromString(searchString));
		}

		public void SetSearch(SearchFilter searchFilter)
		{
			this.m_SearchFilter = searchFilter;
			this.m_SearchFieldText = searchFilter.FilterToSearchFieldString();
			this.TopBarSearchSettingsChanged();
		}

		private void SetSearchViewState(ProjectBrowser.SearchViewState state)
		{
			switch (state)
			{
			case ProjectBrowser.SearchViewState.NotSearching:
				Debug.LogError("Invalid search mode as setter");
				break;
			case ProjectBrowser.SearchViewState.AllAssets:
				this.m_SearchFilter.searchArea = SearchFilter.SearchArea.AllAssets;
				break;
			case ProjectBrowser.SearchViewState.SubFolders:
				this.m_SearchFilter.searchArea = SearchFilter.SearchArea.SelectedFolders;
				break;
			case ProjectBrowser.SearchViewState.AssetStore:
				this.m_SearchFilter.searchArea = SearchFilter.SearchArea.AssetStore;
				break;
			}
			this.InitSearchMenu();
			this.InitListArea();
		}

		private ProjectBrowser.SearchViewState GetSearchViewState()
		{
			SearchFilter.State state = this.m_SearchFilter.GetState();
			ProjectBrowser.SearchViewState result;
			if (state != SearchFilter.State.SearchingInAllAssets)
			{
				if (state != SearchFilter.State.SearchingInFolders)
				{
					if (state != SearchFilter.State.SearchingInAssetStore)
					{
						result = ProjectBrowser.SearchViewState.NotSearching;
					}
					else
					{
						result = ProjectBrowser.SearchViewState.AssetStore;
					}
				}
				else
				{
					result = ProjectBrowser.SearchViewState.SubFolders;
				}
			}
			else
			{
				result = ProjectBrowser.SearchViewState.AllAssets;
			}
			return result;
		}

		private void SearchButtonClickedCallback(ExposablePopupMenu.ItemData itemClicked)
		{
			if (!itemClicked.m_On)
			{
				this.SetSearchViewState((ProjectBrowser.SearchViewState)itemClicked.m_UserData);
				if (this.m_SearchFilter.searchArea == SearchFilter.SearchArea.AllAssets || this.m_SearchFilter.searchArea == SearchFilter.SearchArea.SelectedFolders)
				{
					this.m_LastLocalAssetsSearchArea = this.m_SearchFilter.searchArea;
				}
			}
		}

		private void InitSearchMenu()
		{
			ProjectBrowser.SearchViewState searchViewState = this.GetSearchViewState();
			if (searchViewState != ProjectBrowser.SearchViewState.NotSearching)
			{
				List<ExposablePopupMenu.ItemData> list = new List<ExposablePopupMenu.ItemData>();
				GUIStyle gUIStyle = "ExposablePopupItem";
				GUIStyle gUIStyle2 = "ExposablePopupItem";
				bool enabled = this.m_SearchFilter.folders.Length > 0;
				this.m_SearchAssetStore.text = this.m_ListArea.GetAssetStoreButtonText();
				bool flag = searchViewState == ProjectBrowser.SearchViewState.AllAssets;
				list.Add(new ExposablePopupMenu.ItemData(this.m_SearchAllAssets, (!flag) ? gUIStyle2 : gUIStyle, flag, true, 1));
				flag = (searchViewState == ProjectBrowser.SearchViewState.SubFolders);
				list.Add(new ExposablePopupMenu.ItemData(this.m_SearchInFolders, (!flag) ? gUIStyle2 : gUIStyle, flag, enabled, 2));
				flag = (searchViewState == ProjectBrowser.SearchViewState.AssetStore);
				list.Add(new ExposablePopupMenu.ItemData(this.m_SearchAssetStore, (!flag) ? gUIStyle2 : gUIStyle, flag, true, 3));
				GUIContent content = this.m_SearchAllAssets;
				switch (searchViewState)
				{
				case ProjectBrowser.SearchViewState.NotSearching:
					content = this.m_SearchAssetStore;
					break;
				case ProjectBrowser.SearchViewState.AllAssets:
					content = this.m_SearchAllAssets;
					break;
				case ProjectBrowser.SearchViewState.SubFolders:
					content = this.m_SearchInFolders;
					break;
				case ProjectBrowser.SearchViewState.AssetStore:
					content = this.m_SearchAssetStore;
					break;
				default:
					Debug.LogError("Unhandled enum");
					break;
				}
				ExposablePopupMenu.PopupButtonData popupButtonData = new ExposablePopupMenu.PopupButtonData(content, ProjectBrowser.s_Styles.exposablePopup);
				this.m_SearchAreaMenu.Init(list, 10f, 450f, popupButtonData, new Action<ExposablePopupMenu.ItemData>(this.SearchButtonClickedCallback));
			}
		}

		private void AssetStoreSearchEndedCallback()
		{
			this.InitSearchMenu();
		}

		public static void ShowAssetStoreHitsWhileSearchingLocalAssetsChanged()
		{
			foreach (ProjectBrowser current in ProjectBrowser.s_ProjectBrowsers)
			{
				current.m_ListArea.ShowAssetStoreHitCountWhileSearchingLocalAssetsChanged();
				current.InitSearchMenu();
			}
		}

		private void RefreshSearchText()
		{
			if (this.m_SearchFilter.folders.Length > 0)
			{
				string[] baseFolders = ProjectWindowUtil.GetBaseFolders(this.m_SearchFilter.folders);
				string text = "";
				string tooltip = "";
				int num = 3;
				int num2 = 0;
				while (num2 < baseFolders.Length && num2 < num)
				{
					if (num2 > 0)
					{
						text += ", ";
					}
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFolders[num2]);
					text = text + "'" + fileNameWithoutExtension + "'";
					if (num2 == 0 && fileNameWithoutExtension != "Assets")
					{
						tooltip = baseFolders[num2];
					}
					num2++;
				}
				if (baseFolders.Length > num)
				{
					text += " +";
				}
				this.m_SearchInFolders.text = text;
				this.m_SearchInFolders.tooltip = tooltip;
			}
			else
			{
				this.m_SearchInFolders.text = "Selected folder";
				this.m_SearchInFolders.tooltip = "";
			}
			this.m_BreadCrumbs.Clear();
			this.InitSearchMenu();
		}

		private void EnsureValidSetup()
		{
			if (this.m_LastProjectPath != Directory.GetCurrentDirectory())
			{
				this.m_SearchFilter = new SearchFilter();
				this.m_LastFolders = new string[0];
				this.SyncFilterGUI();
				if (Selection.activeInstanceID != 0)
				{
					this.FrameObjectPrivate(Selection.activeInstanceID, !this.m_IsLocked, false);
				}
				if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && !this.IsShowingFolderContents())
				{
					this.SelectAssetsFolder();
				}
				this.m_LastProjectPath = Directory.GetCurrentDirectory();
			}
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && this.m_SearchFilter.GetState() == SearchFilter.State.EmptySearchFilter)
			{
				this.SelectAssetsFolder();
			}
		}

		private void OnGUIAssetCallback(int instanceID, Rect rect)
		{
			if (EditorApplication.projectWindowItemOnGUI != null)
			{
				string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID));
				EditorApplication.projectWindowItemOnGUI(guid, rect);
			}
		}

		private void InitViewMode(ProjectBrowser.ViewMode viewMode)
		{
			this.m_ViewMode = viewMode;
			this.m_FolderTree = null;
			this.m_AssetTree = null;
			this.useTreeViewSelectionInsteadOfMainSelection = false;
			if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				this.m_AssetTree = new TreeViewController(this, this.m_AssetTreeState);
				this.m_AssetTree.deselectOnUnhandledMouseDown = true;
				TreeViewController expr_4D = this.m_AssetTree;
				expr_4D.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_4D.selectionChangedCallback, new Action<int[]>(this.AssetTreeSelectionCallback));
				TreeViewController expr_74 = this.m_AssetTree;
				expr_74.keyboardInputCallback = (Action)Delegate.Combine(expr_74.keyboardInputCallback, new Action(this.AssetTreeKeyboardInputCallback));
				TreeViewController expr_9B = this.m_AssetTree;
				expr_9B.contextClickItemCallback = (Action<int>)Delegate.Combine(expr_9B.contextClickItemCallback, new Action<int>(this.AssetTreeViewContextClick));
				TreeViewController expr_C2 = this.m_AssetTree;
				expr_C2.contextClickOutsideItemsCallback = (Action)Delegate.Combine(expr_C2.contextClickOutsideItemsCallback, new Action(this.AssetTreeViewContextClickOutsideItems));
				TreeViewController expr_E9 = this.m_AssetTree;
				expr_E9.itemDoubleClickedCallback = (Action<int>)Delegate.Combine(expr_E9.itemDoubleClickedCallback, new Action<int>(this.AssetTreeItemDoubleClickedCallback));
				TreeViewController expr_110 = this.m_AssetTree;
				expr_110.onGUIRowCallback = (Action<int, Rect>)Delegate.Combine(expr_110.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
				TreeViewController expr_137 = this.m_AssetTree;
				expr_137.dragEndedCallback = (Action<int[], bool>)Delegate.Combine(expr_137.dragEndedCallback, new Action<int[], bool>(this.AssetTreeDragEnded));
				string guid = AssetDatabase.AssetPathToGUID("Assets");
				AssetsTreeViewDataSource assetsTreeViewDataSource = new AssetsTreeViewDataSource(this.m_AssetTree, AssetDatabase.GetInstanceIDFromGUID(guid), false, false);
				assetsTreeViewDataSource.foldersFirst = this.GetShouldShowFoldersFirst();
				this.m_AssetTree.Init(this.m_TreeViewRect, assetsTreeViewDataSource, new AssetsTreeViewGUI(this.m_AssetTree), new AssetsTreeViewDragging(this.m_AssetTree));
				this.m_AssetTree.ReloadData();
			}
			else if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
			{
				this.m_FolderTree = new TreeViewController(this, this.m_FolderTreeState);
				this.m_FolderTree.deselectOnUnhandledMouseDown = false;
				TreeViewController expr_1ED = this.m_FolderTree;
				expr_1ED.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_1ED.selectionChangedCallback, new Action<int[]>(this.FolderTreeSelectionCallback));
				TreeViewController expr_214 = this.m_FolderTree;
				expr_214.contextClickItemCallback = (Action<int>)Delegate.Combine(expr_214.contextClickItemCallback, new Action<int>(this.FolderTreeViewContextClick));
				TreeViewController expr_23B = this.m_FolderTree;
				expr_23B.onGUIRowCallback = (Action<int, Rect>)Delegate.Combine(expr_23B.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
				TreeViewController expr_262 = this.m_FolderTree;
				expr_262.dragEndedCallback = (Action<int[], bool>)Delegate.Combine(expr_262.dragEndedCallback, new Action<int[], bool>(this.FolderTreeDragEnded));
				this.m_FolderTree.Init(this.m_TreeViewRect, new ProjectBrowserColumnOneTreeViewDataSource(this.m_FolderTree), new ProjectBrowserColumnOneTreeViewGUI(this.m_FolderTree), new ProjectBrowserColumnOneTreeViewDragging(this.m_FolderTree));
				this.m_FolderTree.ReloadData();
			}
			float x = (this.m_ViewMode != ProjectBrowser.ViewMode.OneColumn) ? 230f : 230f;
			base.minSize = new Vector2(x, 250f);
			base.maxSize = new Vector2(10000f, 10000f);
		}

		private bool GetShouldShowFoldersFirst()
		{
			return Application.platform != RuntimePlatform.OSXEditor;
		}

		private void SetViewMode(ProjectBrowser.ViewMode newViewMode)
		{
			if (this.m_ViewMode != newViewMode)
			{
				this.EndRenaming();
				this.InitViewMode((this.m_ViewMode != ProjectBrowser.ViewMode.OneColumn) ? ProjectBrowser.ViewMode.OneColumn : ProjectBrowser.ViewMode.TwoColumns);
				if (Selection.activeInstanceID != 0)
				{
					this.FrameObjectPrivate(Selection.activeInstanceID, !this.m_IsLocked, false);
				}
				base.RepaintImmediately();
			}
		}

		private void EndRenaming()
		{
			if (this.m_AssetTree != null)
			{
				this.m_AssetTree.EndNameEditing(true);
			}
			if (this.m_FolderTree != null)
			{
				this.m_FolderTree.EndNameEditing(true);
			}
			if (this.m_ListArea != null)
			{
				this.m_ListArea.EndRename(true);
			}
		}

		private string[] GetTypesDisplayNames()
		{
			return new string[]
			{
				"AnimationClip",
				"AudioClip",
				"AudioMixer",
				"Font",
				"GUISkin",
				"Material",
				"Mesh",
				"Model",
				"PhysicMaterial",
				"Prefab",
				"Scene",
				"Script",
				"Shader",
				"Sprite",
				"Texture"
			};
		}

		public void TypeListCallback(PopupList.ListElement element)
		{
			if (!Event.current.control)
			{
				foreach (PopupList.ListElement current in this.m_ObjectTypes.m_ListElements)
				{
					if (current != element)
					{
						current.selected = false;
					}
				}
			}
			element.selected = !element.selected;
			string[] array = (from item in this.m_ObjectTypes.m_ListElements
			where item.selected
			select item.text).ToArray<string>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i];
			}
			this.m_SearchFilter.classNames = array;
			this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
			this.TopBarSearchSettingsChanged();
			base.Repaint();
		}

		public void AssetLabelListCallback(PopupList.ListElement element)
		{
			if (!Event.current.control)
			{
				foreach (PopupList.ListElement current in this.m_AssetLabels.m_ListElements)
				{
					if (current != element)
					{
						current.selected = false;
					}
				}
			}
			element.selected = !element.selected;
			this.m_SearchFilter.assetLabels = (from item in this.m_AssetLabels.m_ListElements
			where item.selected
			select item.text).ToArray<string>();
			this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
			this.TopBarSearchSettingsChanged();
			base.Repaint();
		}

		private void SetupDroplists()
		{
			this.SetupAssetLabelList();
			this.m_ObjectTypes = new PopupList.InputData();
			this.m_ObjectTypes.m_CloseOnSelection = false;
			this.m_ObjectTypes.m_AllowCustom = false;
			this.m_ObjectTypes.m_OnSelectCallback = new PopupList.OnSelectCallback(this.TypeListCallback);
			this.m_ObjectTypes.m_SortAlphabetically = false;
			this.m_ObjectTypes.m_MaxCount = 0;
			string[] typesDisplayNames = this.GetTypesDisplayNames();
			for (int i = 0; i < typesDisplayNames.Length; i++)
			{
				PopupList.ListElement listElement = this.m_ObjectTypes.NewOrMatchingElement(typesDisplayNames[i]);
				if (i == 0)
				{
					listElement.selected = true;
				}
			}
		}

		private void SetupAssetLabelList()
		{
			Dictionary<string, float> allLabels = AssetDatabase.GetAllLabels();
			this.m_AssetLabels = new PopupList.InputData();
			this.m_AssetLabels.m_CloseOnSelection = false;
			this.m_AssetLabels.m_AllowCustom = true;
			this.m_AssetLabels.m_OnSelectCallback = new PopupList.OnSelectCallback(this.AssetLabelListCallback);
			this.m_AssetLabels.m_MaxCount = 15;
			this.m_AssetLabels.m_SortAlphabetically = true;
			foreach (KeyValuePair<string, float> current in allLabels)
			{
				PopupList.ListElement listElement = this.m_AssetLabels.NewOrMatchingElement(current.Key);
				if (listElement.filterScore < current.Value)
				{
					listElement.filterScore = current.Value;
				}
			}
		}

		private void SyncFilterGUI()
		{
			List<string> list = new List<string>(this.m_SearchFilter.assetLabels);
			foreach (PopupList.ListElement current in this.m_AssetLabels.m_ListElements)
			{
				current.selected = list.Contains(current.text);
			}
			List<string> list2 = new List<string>(this.m_SearchFilter.classNames);
			foreach (PopupList.ListElement current2 in this.m_ObjectTypes.m_ListElements)
			{
				current2.selected = list2.Contains(current2.text);
			}
			this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
		}

		private static int GetParentInstanceID(int objectInstanceID)
		{
			string assetPath = AssetDatabase.GetAssetPath(objectInstanceID);
			int num = assetPath.LastIndexOf("/");
			int result;
			if (num >= 0)
			{
				string assetPath2 = assetPath.Substring(0, num);
				UnityEngine.Object @object = AssetDatabase.LoadAssetAtPath(assetPath2, typeof(UnityEngine.Object));
				if (@object != null)
				{
					result = @object.GetInstanceID();
					return result;
				}
			}
			else
			{
				Debug.LogError("Invalid path: " + assetPath);
			}
			result = -1;
			return result;
		}

		private bool IsShowingFolder(int folderInstanceID)
		{
			string assetPath = AssetDatabase.GetAssetPath(folderInstanceID);
			return new List<string>(this.m_SearchFilter.folders).Contains(assetPath);
		}

		private void ShowFolderContents(int folderInstanceID, bool revealAndFrameInFolderTree)
		{
			if (this.m_ViewMode != ProjectBrowser.ViewMode.TwoColumns)
			{
				Debug.LogError("ShowFolderContents should only be called in two column mode");
			}
			if (folderInstanceID != 0)
			{
				string assetPath = AssetDatabase.GetAssetPath(folderInstanceID);
				this.m_SearchFilter.ClearSearch();
				this.m_SearchFilter.folders = new string[]
				{
					assetPath
				};
				this.m_FolderTree.SetSelection(new int[]
				{
					folderInstanceID
				}, revealAndFrameInFolderTree);
				this.FolderTreeSelectionChanged(true);
			}
		}

		private bool IsShowingFolderContents()
		{
			return this.m_SearchFilter.folders.Length > 0;
		}

		private void ListGotKeyboardFocus()
		{
		}

		private void ListAreaKeyboardCallback()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				KeyCode keyCode = Event.current.keyCode;
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
					goto IL_58;
				case KeyCode.KeypadEquals:
					IL_39:
					if (keyCode == KeyCode.Backspace)
					{
						if (Application.platform != RuntimePlatform.OSXEditor && this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
						{
							this.ShowParentFolderOfCurrentlySelected();
							Event.current.Use();
						}
						goto IL_168;
					}
					if (keyCode == KeyCode.Return)
					{
						goto IL_58;
					}
					if (keyCode != KeyCode.F2)
					{
						goto IL_168;
					}
					if (Application.platform != RuntimePlatform.OSXEditor)
					{
						if (this.m_ListArea.BeginRename(0f))
						{
							Event.current.Use();
						}
					}
					goto IL_168;
				case KeyCode.UpArrow:
					if (Application.platform == RuntimePlatform.OSXEditor && Event.current.command && this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
					{
						this.ShowParentFolderOfCurrentlySelected();
						Event.current.Use();
					}
					goto IL_168;
				case KeyCode.DownArrow:
					if (Application.platform == RuntimePlatform.OSXEditor && Event.current.command)
					{
						Event.current.Use();
						this.OpenListAreaSelection();
					}
					goto IL_168;
				}
				goto IL_39;
				IL_58:
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					if (this.m_ListArea.BeginRename(0f))
					{
						Event.current.Use();
					}
				}
				else
				{
					Event.current.Use();
					this.OpenListAreaSelection();
				}
				IL_168:;
			}
		}

		private void ShowParentFolderOfCurrentlySelected()
		{
			if (this.IsShowingFolderContents())
			{
				int[] selection = this.m_FolderTree.GetSelection();
				if (selection.Length == 1)
				{
					TreeViewItem treeViewItem = this.m_FolderTree.FindItem(selection[0]);
					if (treeViewItem != null && treeViewItem.parent != null && treeViewItem.id != ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID())
					{
						this.SetFolderSelection(new int[]
						{
							treeViewItem.parent.id
						}, true);
						this.m_ListArea.Frame(treeViewItem.id, true, false);
						Selection.activeInstanceID = treeViewItem.id;
					}
				}
			}
		}

		private void OpenListAreaSelection()
		{
			int[] selection = this.m_ListArea.GetSelection();
			int num = selection.Length;
			if (num > 0)
			{
				int num2 = 0;
				int[] array = selection;
				for (int i = 0; i < array.Length; i++)
				{
					int instanceID = array[i];
					if (ProjectWindowUtil.IsFolder(instanceID))
					{
						num2++;
					}
				}
				bool flag = num2 == num;
				if (flag)
				{
					if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
					{
						this.SetFolderSelection(selection, false);
					}
					else if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
					{
						this.ClearSearch();
						this.m_AssetTree.Frame(selection[0], true, false);
					}
					base.Repaint();
					GUIUtility.ExitGUI();
				}
				else
				{
					ProjectBrowser.OpenAssetSelection(selection);
					base.Repaint();
					GUIUtility.ExitGUI();
				}
			}
		}

		private static void OpenAssetSelection(int[] selectedInstanceIDs)
		{
			for (int i = 0; i < selectedInstanceIDs.Length; i++)
			{
				int instanceID = selectedInstanceIDs[i];
				if (AssetDatabase.Contains(instanceID))
				{
					AssetDatabase.OpenAsset(instanceID);
				}
			}
			GUIUtility.ExitGUI();
		}

		private void SetAsLastInteractedProjectBrowser()
		{
			ProjectBrowser.s_LastInteractedProjectBrowser = this;
		}

		private void RefreshSelectedPath()
		{
			if (Selection.activeObject != null)
			{
				this.m_SelectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			}
			else
			{
				this.m_SelectedPath = "";
			}
			this.m_SelectedPathSplitted.Clear();
		}

		private void ListAreaItemSelectedCallback(bool doubleClicked)
		{
			this.SetAsLastInteractedProjectBrowser();
			Selection.activeObject = null;
			int[] selection = this.m_ListArea.GetSelection();
			if (selection.Length > 0)
			{
				Selection.instanceIDs = selection;
				this.m_SearchFilter.searchArea = this.m_LastLocalAssetsSearchArea;
				this.m_InternalSelectionChange = true;
			}
			else if (AssetStoreAssetSelection.Count > 0)
			{
				Selection.activeObject = AssetStoreAssetInspector.Instance;
			}
			this.m_FocusSearchField = false;
			if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
			{
				this.m_ItemSelectedByRightClickThisEvent = true;
			}
			this.RefreshSelectedPath();
			this.m_DidSelectSearchResult = this.m_SearchFilter.IsSearching();
			if (doubleClicked)
			{
				this.OpenListAreaSelection();
			}
		}

		private void OnGotFocus()
		{
		}

		private void OnLostFocus()
		{
			this.EndRenaming();
			EditorGUI.EndEditingActiveTextField();
		}

		private void OnSelectionChange()
		{
			if (this.m_ListArea != null)
			{
				this.m_ListArea.InitSelection(Selection.instanceIDs);
				if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
				{
					bool revealSelectionAndFrameLastSelected = !this.m_IsLocked;
					this.m_AssetTree.SetSelection(Selection.instanceIDs, revealSelectionAndFrameLastSelected);
				}
				else if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
				{
					if (!this.m_InternalSelectionChange)
					{
						bool flag = !this.m_IsLocked && Selection.instanceIDs.Length > 0;
						if (flag)
						{
							int instanceID = Selection.instanceIDs[Selection.instanceIDs.Length - 1];
							if (this.m_SearchFilter.IsSearching())
							{
								this.m_ListArea.Frame(instanceID, true, false);
							}
							else
							{
								this.FrameObjectInTwoColumnMode(instanceID, true, false);
							}
						}
					}
				}
				this.m_InternalSelectionChange = false;
				if (Selection.activeObject != null && Selection.activeObject.GetType() != typeof(AssetStoreAssetInspector))
				{
					this.m_ListArea.selectedAssetStoreAsset = false;
					AssetStoreAssetSelection.Clear();
				}
				this.RefreshSelectedPath();
				base.Repaint();
			}
		}

		private void SetFoldersInSearchFilter(int[] selectedInstanceIDs)
		{
			this.m_SearchFilter.folders = ProjectBrowser.GetFolderPathsFromInstanceIDs(selectedInstanceIDs);
			this.EnsureValidFolders();
			if (selectedInstanceIDs.Length > 0)
			{
				if (this.m_LastFoldersGridSize > 0f)
				{
					this.m_ListArea.gridSize = (int)this.m_LastFoldersGridSize;
				}
			}
		}

		private void SetFolderSelection(int[] selectedInstanceIDs, bool revealSelectionAndFrameLastSelected)
		{
			this.m_FolderTree.SetSelection(selectedInstanceIDs, revealSelectionAndFrameLastSelected);
			this.SetFoldersInSearchFilter(selectedInstanceIDs);
			this.FolderTreeSelectionChanged(true);
		}

		private void AssetTreeItemDoubleClickedCallback(int instanceID)
		{
			ProjectBrowser.OpenAssetSelection(Selection.instanceIDs);
		}

		private void AssetTreeKeyboardInputCallback()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode != KeyCode.Return && keyCode != KeyCode.KeypadEnter)
				{
					if (keyCode == KeyCode.DownArrow)
					{
						if (Application.platform == RuntimePlatform.OSXEditor && Event.current.command)
						{
							Event.current.Use();
							ProjectBrowser.OpenAssetSelection(Selection.instanceIDs);
						}
					}
				}
				else if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					Event.current.Use();
					ProjectBrowser.OpenAssetSelection(Selection.instanceIDs);
				}
			}
		}

		private void AssetTreeSelectionCallback(int[] selectedTreeViewInstanceIDs)
		{
			this.SetAsLastInteractedProjectBrowser();
			Selection.activeObject = null;
			if (selectedTreeViewInstanceIDs.Length > 0)
			{
				Selection.instanceIDs = selectedTreeViewInstanceIDs;
			}
			this.RefreshSelectedPath();
			this.SetSearchFoldersFromCurrentSelection();
			this.RefreshSearchText();
		}

		private void SetSearchFoldersFromCurrentSelection()
		{
			HashSet<string> hashSet = new HashSet<string>();
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				if (AssetDatabase.Contains(instanceID))
				{
					string assetPath = AssetDatabase.GetAssetPath(instanceID);
					if (AssetDatabase.IsValidFolder(assetPath))
					{
						hashSet.Add(assetPath);
					}
					else
					{
						string containingFolder = ProjectWindowUtil.GetContainingFolder(assetPath);
						if (!string.IsNullOrEmpty(containingFolder))
						{
							hashSet.Add(containingFolder);
						}
					}
				}
			}
			this.m_SearchFilter.folders = ProjectWindowUtil.GetBaseFolders(hashSet.ToArray<string>());
		}

		private void SetSearchFolderFromFolderTreeSelection()
		{
			if (this.m_FolderTree != null)
			{
				this.m_SearchFilter.folders = ProjectBrowser.GetFolderPathsFromInstanceIDs(this.m_FolderTree.GetSelection());
			}
		}

		private void FolderTreeSelectionCallback(int[] selectedTreeViewInstanceIDs)
		{
			this.SetAsLastInteractedProjectBrowser();
			int num = 0;
			if (selectedTreeViewInstanceIDs.Length > 0)
			{
				num = selectedTreeViewInstanceIDs[0];
			}
			bool folderWasSelected = false;
			if (num != 0)
			{
				ProjectBrowser.ItemType itemType = ProjectBrowser.GetItemType(num);
				if (itemType == ProjectBrowser.ItemType.Asset)
				{
					this.SetFoldersInSearchFilter(selectedTreeViewInstanceIDs);
					folderWasSelected = true;
				}
				if (itemType == ProjectBrowser.ItemType.SavedFilter)
				{
					SearchFilter filter = SavedSearchFilters.GetFilter(num);
					if (this.ValidateFilter(num, filter))
					{
						this.m_SearchFilter = filter;
						this.EnsureValidFolders();
						float previewSize = SavedSearchFilters.GetPreviewSize(num);
						if (previewSize > 0f)
						{
							this.m_ListArea.gridSize = Mathf.Clamp((int)previewSize, this.m_ListArea.minGridSize, this.m_ListArea.maxGridSize);
						}
						this.SyncFilterGUI();
					}
				}
			}
			this.FolderTreeSelectionChanged(folderWasSelected);
		}

		private bool ValidateFilter(int savedFilterID, SearchFilter filter)
		{
			bool result;
			if (filter == null)
			{
				result = false;
			}
			else
			{
				SearchFilter.State state = filter.GetState();
				if (state == SearchFilter.State.FolderBrowsing || state == SearchFilter.State.SearchingInFolders)
				{
					string[] folders = filter.folders;
					for (int i = 0; i < folders.Length; i++)
					{
						string text = folders[i];
						if (AssetDatabase.GetMainAssetInstanceID(text) == 0)
						{
							if (EditorUtility.DisplayDialog("Folder not found", "The folder '" + text + "' might have been deleted or belong to another project. Do you want to delete the favorite?", "Delete", "Cancel"))
							{
								SavedSearchFilters.RemoveSavedFilter(savedFilterID);
							}
							result = false;
							return result;
						}
					}
				}
				result = true;
			}
			return result;
		}

		private void ShowAndHideFolderTreeSelectionAsNeeded()
		{
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && this.m_FolderTree != null)
			{
				bool flag = false;
				int[] selection = this.m_FolderTree.GetSelection();
				if (selection.Length > 0)
				{
					flag = (ProjectBrowser.GetItemType(selection[0]) == ProjectBrowser.ItemType.SavedFilter);
				}
				switch (this.GetSearchViewState())
				{
				case ProjectBrowser.SearchViewState.NotSearching:
				case ProjectBrowser.SearchViewState.SubFolders:
					if (!flag)
					{
						this.m_FolderTree.SetSelection(ProjectBrowser.GetFolderInstanceIDs(this.m_SearchFilter.folders), true);
					}
					break;
				case ProjectBrowser.SearchViewState.AllAssets:
				case ProjectBrowser.SearchViewState.AssetStore:
					if (!flag)
					{
						this.m_FolderTree.SetSelection(new int[0], false);
					}
					break;
				}
			}
		}

		private void InitListArea()
		{
			this.ShowAndHideFolderTreeSelectionAsNeeded();
			this.m_ListArea.Init(this.m_ListAreaRect, HierarchyType.Assets, this.m_SearchFilter, false);
			this.m_ListArea.InitSelection(Selection.instanceIDs);
		}

		private void OnInspectorUpdate()
		{
			if (this.m_ListArea != null)
			{
				this.m_ListArea.OnInspectorUpdate();
			}
		}

		private void OnDestroy()
		{
			if (this.m_ListArea != null)
			{
				this.m_ListArea.OnDestroy();
			}
			if (this == ProjectBrowser.s_LastInteractedProjectBrowser)
			{
				ProjectBrowser.s_LastInteractedProjectBrowser = null;
			}
		}

		private static List<string> GetMainPaths(List<int> instanceIDs)
		{
			List<string> list = new List<string>();
			foreach (int current in instanceIDs)
			{
				if (AssetDatabase.IsMainAsset(current))
				{
					string assetPath = AssetDatabase.GetAssetPath(current);
					list.Add(assetPath);
				}
			}
			return list;
		}

		internal static int[] DuplicateFolders(int[] instanceIDs)
		{
			AssetDatabase.Refresh();
			List<string> list = new List<string>();
			bool flag = false;
			int assetsFolderInstanceID = ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID();
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int num = instanceIDs[i];
				if (num != assetsFolderInstanceID)
				{
					string assetPath = AssetDatabase.GetAssetPath(InternalEditorUtility.GetObjectFromInstanceID(num));
					string text = AssetDatabase.GenerateUniqueAssetPath(assetPath);
					if (text.Length != 0)
					{
						flag |= !AssetDatabase.CopyAsset(assetPath, text);
					}
					else
					{
						flag |= true;
					}
					if (!flag)
					{
						list.Add(text);
					}
				}
			}
			AssetDatabase.Refresh();
			int[] array = new int[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				array[j] = AssetDatabase.LoadMainAssetAtPath(list[j]).GetInstanceID();
			}
			return array;
		}

		private static void DeleteFilter(int filterInstanceID)
		{
			if (SavedSearchFilters.GetRootInstanceID() == filterInstanceID)
			{
				string title = "Cannot Delete";
				EditorUtility.DisplayDialog(title, "Deleting the 'Filters' root is not allowed", "Ok");
			}
			else
			{
				string title2 = "Delete selected favorite?";
				if (EditorUtility.DisplayDialog(title2, "You cannot undo this action.", "Delete", "Cancel"))
				{
					SavedSearchFilters.RemoveSavedFilter(filterInstanceID);
				}
			}
		}

		private bool HandleCommandEventsForTreeView()
		{
			EventType type = Event.current.type;
			bool result;
			if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
			{
				bool flag = type == EventType.ExecuteCommand;
				int[] selection = this.m_FolderTree.GetSelection();
				if (selection.Length == 0)
				{
					result = false;
					return result;
				}
				ProjectBrowser.ItemType itemType = ProjectBrowser.GetItemType(selection[0]);
				if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
				{
					Event.current.Use();
					if (flag)
					{
						if (itemType == ProjectBrowser.ItemType.SavedFilter)
						{
							ProjectBrowser.DeleteFilter(selection[0]);
							GUIUtility.ExitGUI();
						}
						else if (itemType == ProjectBrowser.ItemType.Asset)
						{
							bool flag2 = Event.current.commandName == "SoftDelete";
							ProjectBrowser.DeleteSelectedAssets(flag2);
							if (flag2)
							{
								base.Focus();
							}
						}
					}
					GUIUtility.ExitGUI();
				}
				else if (Event.current.commandName == "Duplicate")
				{
					if (flag)
					{
						if (itemType != ProjectBrowser.ItemType.SavedFilter)
						{
							if (itemType == ProjectBrowser.ItemType.Asset)
							{
								Event.current.Use();
								int[] selectedIDs = ProjectBrowser.DuplicateFolders(selection);
								this.m_FolderTree.SetSelection(selectedIDs, true);
								GUIUtility.ExitGUI();
							}
						}
					}
					else
					{
						Event.current.Use();
					}
				}
			}
			result = false;
			return result;
		}

		private bool HandleCommandEvents()
		{
			EventType type = Event.current.type;
			if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
			{
				bool flag = type == EventType.ExecuteCommand;
				if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
				{
					Event.current.Use();
					if (flag)
					{
						bool flag2 = Event.current.commandName == "SoftDelete";
						ProjectBrowser.DeleteSelectedAssets(flag2);
						if (flag2)
						{
							base.Focus();
						}
					}
					GUIUtility.ExitGUI();
				}
				else if (Event.current.commandName == "Duplicate")
				{
					if (flag)
					{
						Event.current.Use();
						ProjectWindowUtil.DuplicateSelectedAssets();
						GUIUtility.ExitGUI();
					}
					else
					{
						UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
						if (filtered.Length != 0)
						{
							Event.current.Use();
						}
					}
				}
				else if (Event.current.commandName == "FocusProjectWindow")
				{
					if (flag)
					{
						this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
						Event.current.Use();
						base.Focus();
						GUIUtility.ExitGUI();
					}
					else
					{
						Event.current.Use();
					}
				}
				else if (Event.current.commandName == "SelectAll")
				{
					if (flag)
					{
						this.SelectAll();
					}
					Event.current.Use();
				}
				else if (Event.current.commandName == "FrameSelected")
				{
					if (flag)
					{
						this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
						Event.current.Use();
						GUIUtility.ExitGUI();
					}
					Event.current.Use();
				}
				else if (Event.current.commandName == "Find")
				{
					if (flag)
					{
						this.m_FocusSearchField = true;
					}
					Event.current.Use();
				}
			}
			return false;
		}

		private void SelectAll()
		{
			if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				if (this.m_SearchFilter.IsSearching())
				{
					this.m_ListArea.SelectAll();
				}
				else
				{
					int[] rowIDs = this.m_AssetTree.GetRowIDs();
					this.m_AssetTree.SetSelection(rowIDs, false);
					this.AssetTreeSelectionCallback(rowIDs);
				}
			}
			else if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
			{
				this.m_ListArea.SelectAll();
			}
			else
			{
				Debug.LogError("Missing implementation for ViewMode " + this.m_ViewMode);
			}
		}

		private void RefreshSplittedSelectedPath()
		{
			if (ProjectBrowser.s_Styles == null)
			{
				ProjectBrowser.s_Styles = new ProjectBrowser.Styles();
			}
			this.m_SelectedPathSplitted.Clear();
			if (string.IsNullOrEmpty(this.m_SelectedPath))
			{
				this.m_SelectedPathSplitted.Add(new GUIContent());
			}
			else
			{
				string text = this.m_SelectedPath;
				if (this.m_SelectedPath.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
				{
					text = this.m_SelectedPath.Substring("assets/".Length);
				}
				if (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)
				{
					this.m_SelectedPathSplitted.Add(new GUIContent(Path.GetFileName(this.m_SelectedPath), AssetDatabase.GetCachedIcon(this.m_SelectedPath)));
				}
				else
				{
					float num = base.position.width - this.m_DirectoriesAreaWidth - 55f - 16f;
					if (ProjectBrowser.s_Styles.selectedPathLabel.CalcSize(GUIContent.Temp(text)).x + 25f > num)
					{
						string[] array = text.Split(new char[]
						{
							'/'
						});
						string text2 = "Assets/";
						for (int i = 0; i < array.Length; i++)
						{
							text2 += array[i];
							Texture cachedIcon = AssetDatabase.GetCachedIcon(text2);
							this.m_SelectedPathSplitted.Add(new GUIContent(array[i], cachedIcon));
							text2 += "/";
						}
					}
					else
					{
						this.m_SelectedPathSplitted.Add(new GUIContent(text, AssetDatabase.GetCachedIcon(this.m_SelectedPath)));
					}
				}
			}
		}

		private float GetBottomBarHeight()
		{
			if (this.m_SelectedPathSplitted.Count == 0)
			{
				this.RefreshSplittedSelectedPath();
			}
			float result;
			if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn && !this.m_SearchFilter.IsSearching())
			{
				result = 0f;
			}
			else
			{
				result = 17f * (float)this.m_SelectedPathSplitted.Count;
			}
			return result;
		}

		private float GetListHeaderHeight()
		{
			return (this.m_SearchFilter.GetState() != SearchFilter.State.EmptySearchFilter) ? 18f : 0f;
		}

		private void CalculateRects()
		{
			float bottomBarHeight = this.GetBottomBarHeight();
			float listHeaderHeight = this.GetListHeaderHeight();
			if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				this.m_ListAreaRect = new Rect(0f, this.m_ToolbarHeight + listHeaderHeight, base.position.width, base.position.height - this.m_ToolbarHeight - listHeaderHeight - bottomBarHeight);
				this.m_TreeViewRect = new Rect(0f, this.m_ToolbarHeight, base.position.width, base.position.height - this.m_ToolbarHeight - bottomBarHeight);
				this.m_BottomBarRect = new Rect(0f, base.position.height - bottomBarHeight, base.position.width, bottomBarHeight);
				this.m_ListHeaderRect = new Rect(0f, this.m_ToolbarHeight, base.position.width, listHeaderHeight);
			}
			else
			{
				float width = base.position.width - this.m_DirectoriesAreaWidth;
				this.m_ListAreaRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight + listHeaderHeight, width, base.position.height - this.m_ToolbarHeight - listHeaderHeight - bottomBarHeight);
				this.m_TreeViewRect = new Rect(0f, this.m_ToolbarHeight, this.m_DirectoriesAreaWidth, base.position.height - this.m_ToolbarHeight);
				this.m_BottomBarRect = new Rect(this.m_DirectoriesAreaWidth, base.position.height - bottomBarHeight, width, bottomBarHeight);
				this.m_ListHeaderRect = new Rect(this.m_ListAreaRect.x, this.m_ToolbarHeight, this.m_ListAreaRect.width, listHeaderHeight);
			}
		}

		private void EndPing()
		{
			if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				this.m_AssetTree.EndPing();
			}
			else
			{
				this.m_FolderTree.EndPing();
				this.m_ListArea.EndPing();
			}
		}

		private void OnEvent()
		{
			if (this.m_AssetTree != null)
			{
				this.m_AssetTree.OnEvent();
			}
			if (this.m_FolderTree != null)
			{
				this.m_FolderTree.OnEvent();
			}
			if (this.m_ListArea != null)
			{
				this.m_ListArea.OnEvent();
			}
		}

		private void OnGUI()
		{
			if (ProjectBrowser.s_Styles == null)
			{
				ProjectBrowser.s_Styles = new ProjectBrowser.Styles();
			}
			if (!this.Initialized())
			{
				this.Init();
			}
			this.m_ListKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.m_TreeViewKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.OnEvent();
			this.m_ToolbarHeight = 18f;
			this.m_ItemSelectedByRightClickThisEvent = false;
			this.ResizeHandling(base.position.width, base.position.height - this.m_ToolbarHeight);
			this.CalculateRects();
			Event current = Event.current;
			Rect position = new Rect(0f, 0f, base.position.width, base.position.height);
			if (current.type == EventType.MouseDown && position.Contains(current.mousePosition))
			{
				this.EndPing();
				this.SetAsLastInteractedProjectBrowser();
			}
			if (this.m_GrabKeyboardFocusForListArea)
			{
				this.m_GrabKeyboardFocusForListArea = false;
				GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
			}
			GUI.BeginGroup(position, GUIContent.none);
			this.TopToolbar();
			this.BottomBar();
			if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				if (this.m_SearchFilter.IsSearching())
				{
					this.SearchAreaBar();
					if (GUIUtility.keyboardControl == this.m_TreeViewKeyboardControlID)
					{
						GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
					}
					this.m_ListArea.OnGUI(this.m_ListAreaRect, this.m_ListKeyboardControlID);
				}
				else
				{
					if (GUIUtility.keyboardControl == this.m_ListKeyboardControlID)
					{
						GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
					}
					this.m_AssetTree.OnGUI(this.m_TreeViewRect, this.m_TreeViewKeyboardControlID);
				}
			}
			else
			{
				if (this.m_SearchFilter.IsSearching())
				{
					this.SearchAreaBar();
				}
				else
				{
					this.BreadCrumbBar();
				}
				this.m_FolderTree.OnGUI(this.m_TreeViewRect, this.m_TreeViewKeyboardControlID);
				EditorGUIUtility.DrawHorizontalSplitter(new Rect(this.m_ListAreaRect.x, this.m_ToolbarHeight, 1f, this.m_TreeViewRect.height));
				this.m_ListArea.OnGUI(this.m_ListAreaRect, this.m_ListKeyboardControlID);
				if (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing && this.m_ListArea.numItemsDisplayed == 0)
				{
					Vector2 vector = EditorStyles.label.CalcSize(ProjectBrowser.s_Styles.m_EmptyFolderText);
					Rect position2 = new Rect(this.m_ListAreaRect.x + 2f + Mathf.Max(0f, (this.m_ListAreaRect.width - vector.x) * 0.5f), this.m_ListAreaRect.y + 10f, vector.x, 20f);
					using (new EditorGUI.DisabledScope(true))
					{
						GUI.Label(position2, ProjectBrowser.s_Styles.m_EmptyFolderText, EditorStyles.label);
					}
				}
			}
			this.HandleContextClickInListArea(this.m_ListAreaRect);
			if (this.m_ListArea.gridSize != this.m_StartGridSize)
			{
				this.m_StartGridSize = this.m_ListArea.gridSize;
				if (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)
				{
					this.m_LastFoldersGridSize = (float)this.m_ListArea.gridSize;
				}
			}
			GUI.EndGroup();
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
			{
				this.useTreeViewSelectionInsteadOfMainSelection = (GUIUtility.keyboardControl == this.m_TreeViewKeyboardControlID);
			}
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && GUIUtility.keyboardControl == this.m_TreeViewKeyboardControlID)
			{
				this.HandleCommandEventsForTreeView();
			}
			this.HandleCommandEvents();
		}

		private void HandleContextClickInListArea(Rect listRect)
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.ContextClick)
				{
					if (listRect.Contains(current.mousePosition))
					{
						GUIUtility.hotControl = 0;
						if (AssetStoreAssetSelection.GetFirstAsset() != null)
						{
							ProjectBrowser.AssetStoreItemContextMenu.Show();
						}
						else
						{
							EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
						}
						current.Use();
					}
				}
			}
			else if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing && current.button == 1 && !this.m_ItemSelectedByRightClickThisEvent)
			{
				if (this.m_SearchFilter.folders.Length > 0 && listRect.Contains(current.mousePosition))
				{
					this.m_InternalSelectionChange = true;
					Selection.instanceIDs = ProjectBrowser.GetFolderInstanceIDs(this.m_SearchFilter.folders);
				}
			}
		}

		private void AssetTreeViewContextClick(int clickedItemID)
		{
			Event current = Event.current;
			EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
			current.Use();
		}

		private void AssetTreeViewContextClickOutsideItems()
		{
			Event current = Event.current;
			if (this.m_AssetTree.GetSelection().Length > 0)
			{
				int[] array = new int[0];
				this.m_AssetTree.SetSelection(array, false);
				this.AssetTreeSelectionCallback(array);
			}
			EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
			current.Use();
		}

		private void FolderTreeViewContextClick(int clickedItemID)
		{
			Event current = Event.current;
			if (SavedSearchFilters.IsSavedFilter(clickedItemID))
			{
				if (clickedItemID != SavedSearchFilters.GetRootInstanceID())
				{
					ProjectBrowser.SavedFiltersContextMenu.Show(clickedItemID);
				}
			}
			else
			{
				EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
			}
			current.Use();
		}

		private void AssetTreeDragEnded(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
		{
			if (draggedInstanceIds != null && draggedItemsFromOwnTreeView)
			{
				this.m_AssetTree.SetSelection(draggedInstanceIds, true);
				this.m_AssetTree.NotifyListenersThatSelectionChanged();
				base.Repaint();
				GUIUtility.ExitGUI();
			}
		}

		private void FolderTreeDragEnded(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
		{
		}

		private void TopToolbar()
		{
			GUILayout.BeginArea(new Rect(0f, 0f, base.position.width, this.m_ToolbarHeight));
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			float num = base.position.width - this.m_DirectoriesAreaWidth;
			float num2 = 4f;
			if (num >= 500f)
			{
				num2 = 10f;
			}
			this.CreateDropdown();
			GUILayout.FlexibleSpace();
			GUILayout.Space(num2 * 2f);
			this.SearchField();
			GUILayout.Space(num2);
			this.TypeDropDown();
			this.AssetLabelsDropDown();
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
			{
				this.ButtonSaveFilter();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void SetOneColumn()
		{
			this.SetViewMode(ProjectBrowser.ViewMode.OneColumn);
		}

		private void SetTwoColumns()
		{
			this.SetViewMode(ProjectBrowser.ViewMode.TwoColumns);
		}

		private void OpenTreeViewTestWindow()
		{
			EditorWindow.GetWindow<TreeViewTestWindow>();
		}

		private void ToggleExpansionAnimationPreference()
		{
			bool @bool = EditorPrefs.GetBool("TreeViewExpansionAnimation", false);
			EditorPrefs.SetBool("TreeViewExpansionAnimation", !@bool);
			InternalEditorUtility.RequestScriptReload();
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			if (this.m_EnableOldAssetTree)
			{
				GUIContent content = new GUIContent("One Column Layout");
				GUIContent content2 = new GUIContent("Two Column Layout");
				menu.AddItem(content, this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn, new GenericMenu.MenuFunction(this.SetOneColumn));
				if (base.position.width >= 230f)
				{
					menu.AddItem(content2, this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns, new GenericMenu.MenuFunction(this.SetTwoColumns));
				}
				else
				{
					menu.AddDisabledItem(content2);
				}
				if (Unsupported.IsDeveloperBuild())
				{
					menu.AddItem(new GUIContent("DEVELOPER/Open TreeView Test Window..."), false, new GenericMenu.MenuFunction(this.OpenTreeViewTestWindow));
					menu.AddItem(new GUIContent("DEVELOPER/Use TreeView Expansion Animation"), EditorPrefs.GetBool("TreeViewExpansionAnimation", false), new GenericMenu.MenuFunction(this.ToggleExpansionAnimationPreference));
				}
			}
		}

		private float DrawLocalAssetHeader(Rect r)
		{
			return 0f;
		}

		private void ResizeHandling(float width, float height)
		{
			if (this.m_ViewMode != ProjectBrowser.ViewMode.OneColumn)
			{
				Rect dragRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight, 5f, height);
				dragRect = EditorGUIUtility.HandleHorizontalSplitter(dragRect, base.position.width, this.k_MinDirectoriesAreaWidth, 230f - this.k_MinDirectoriesAreaWidth);
				this.m_DirectoriesAreaWidth = dragRect.x;
				float num = base.position.width - this.m_DirectoriesAreaWidth;
				if (num != this.m_LastListWidth)
				{
					this.RefreshSplittedSelectedPath();
				}
				this.m_LastListWidth = num;
			}
		}

		private void ButtonSaveFilter()
		{
			using (new EditorGUI.DisabledScope(!this.m_SearchFilter.IsSearching()))
			{
				if (GUILayout.Button(ProjectBrowser.s_Styles.m_SaveFilterContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					ProjectBrowserColumnOneTreeViewGUI projectBrowserColumnOneTreeViewGUI = this.m_FolderTree.gui as ProjectBrowserColumnOneTreeViewGUI;
					if (projectBrowserColumnOneTreeViewGUI != null)
					{
						bool flag = true;
						int[] selection = this.m_FolderTree.GetSelection();
						if (selection.Length == 1)
						{
							int num = selection[0];
							bool flag2 = SavedSearchFilters.GetRootInstanceID() == num;
							if (SavedSearchFilters.IsSavedFilter(num) && !flag2)
							{
								flag = false;
								string title = "Overwrite Filter?";
								string message = "Do you want to overwrite '" + SavedSearchFilters.GetName(num) + "' or create a new filter?";
								int num2 = EditorUtility.DisplayDialogComplex(title, message, "Overwrite", "Create", "Cancel");
								if (num2 == 0)
								{
									SavedSearchFilters.UpdateExistingSavedFilter(num, this.m_SearchFilter, this.listAreaGridSize);
								}
								else if (num2 == 1)
								{
									flag = true;
								}
							}
						}
						if (flag)
						{
							base.Focus();
							projectBrowserColumnOneTreeViewGUI.BeginCreateSavedFilter(this.m_SearchFilter);
						}
					}
				}
			}
		}

		private void CreateDropdown()
		{
			Rect rect = GUILayoutUtility.GetRect(ProjectBrowser.s_Styles.m_CreateDropdownContent, EditorStyles.toolbarDropDown);
			if (EditorGUI.ButtonMouseDown(rect, ProjectBrowser.s_Styles.m_CreateDropdownContent, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				GUIUtility.hotControl = 0;
				EditorUtility.DisplayPopupMenu(rect, "Assets/Create", null);
			}
		}

		private void AssetLabelsDropDown()
		{
			Rect rect = GUILayoutUtility.GetRect(ProjectBrowser.s_Styles.m_FilterByLabel, EditorStyles.toolbarButton);
			if (EditorGUI.ButtonMouseDown(rect, ProjectBrowser.s_Styles.m_FilterByLabel, FocusType.Passive, EditorStyles.toolbarButton))
			{
				PopupWindow.Show(rect, new PopupList(this.m_AssetLabels), null, ShowMode.PopupMenuWithKeyboardFocus);
			}
		}

		private void TypeDropDown()
		{
			Rect rect = GUILayoutUtility.GetRect(ProjectBrowser.s_Styles.m_FilterByType, EditorStyles.toolbarButton);
			if (EditorGUI.ButtonMouseDown(rect, ProjectBrowser.s_Styles.m_FilterByType, FocusType.Passive, EditorStyles.toolbarButton))
			{
				PopupWindow.Show(rect, new PopupList(this.m_ObjectTypes));
			}
		}

		private void SearchField()
		{
			Rect rect = GUILayoutUtility.GetRect(0f, EditorGUILayout.kLabelFloatMaxW * 1.5f, 16f, 16f, EditorStyles.toolbarSearchField, new GUILayoutOption[]
			{
				GUILayout.MinWidth(65f),
				GUILayout.MaxWidth(300f)
			});
			int controlID = GUIUtility.GetControlID(ProjectBrowser.s_HashForSearchField, FocusType.Passive, rect);
			if (this.m_FocusSearchField)
			{
				GUIUtility.keyboardControl = controlID;
				EditorGUIUtility.editingTextField = true;
				if (Event.current.type == EventType.Repaint)
				{
					this.m_FocusSearchField = false;
				}
			}
			Event current = Event.current;
			if (current.type == EventType.KeyDown && (current.keyCode == KeyCode.DownArrow || current.keyCode == KeyCode.UpArrow))
			{
				if (GUIUtility.keyboardControl == controlID)
				{
					if (!this.m_ListArea.IsLastClickedItemVisible())
					{
						this.m_ListArea.SelectFirst();
					}
					GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
					current.Use();
				}
			}
			string text = EditorGUI.ToolbarSearchField(controlID, rect, this.m_SearchFieldText, false);
			if (text != this.m_SearchFieldText || this.m_FocusSearchField)
			{
				this.m_SearchFieldText = text;
				this.m_SearchFilter.SearchFieldStringToFilter(this.m_SearchFieldText);
				this.SyncFilterGUI();
				this.TopBarSearchSettingsChanged();
				base.Repaint();
			}
		}

		private void TopBarSearchSettingsChanged()
		{
			if (!this.m_SearchFilter.IsSearching())
			{
				if (this.m_DidSelectSearchResult)
				{
					this.m_DidSelectSearchResult = false;
					this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
					if (GUIUtility.keyboardControl == 0)
					{
						if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
						{
							GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
						}
						else if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
						{
							GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
						}
					}
				}
				else if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
				{
					if (GUIUtility.keyboardControl == 0 && this.m_LastFolders != null && this.m_LastFolders.Length > 0)
					{
						this.m_SearchFilter.folders = this.m_LastFolders;
						this.SetFolderSelection(ProjectBrowser.GetFolderInstanceIDs(this.m_LastFolders), true);
					}
				}
			}
			else
			{
				this.InitSearchMenu();
			}
			this.InitListArea();
		}

		private static int[] GetFolderInstanceIDs(string[] folders)
		{
			int[] array = new int[folders.Length];
			for (int i = 0; i < folders.Length; i++)
			{
				array[i] = AssetDatabase.GetMainAssetInstanceID(folders[i]);
			}
			return array;
		}

		private static string[] GetFolderPathsFromInstanceIDs(int[] instanceIDs)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				string assetPath = AssetDatabase.GetAssetPath(instanceID);
				if (!string.IsNullOrEmpty(assetPath))
				{
					list.Add(assetPath);
				}
			}
			return list.ToArray();
		}

		private void ClearSearch()
		{
			this.m_SearchFilter.ClearSearch();
			this.m_SearchFieldText = "";
			this.m_AssetLabels.DeselectAll();
			this.m_ObjectTypes.DeselectAll();
			this.m_DidSelectSearchResult = false;
		}

		private void FolderTreeSelectionChanged(bool folderWasSelected)
		{
			if (folderWasSelected)
			{
				ProjectBrowser.SearchViewState searchViewState = this.GetSearchViewState();
				if (searchViewState == ProjectBrowser.SearchViewState.AllAssets || searchViewState == ProjectBrowser.SearchViewState.AssetStore)
				{
					string[] folders = this.m_SearchFilter.folders;
					this.ClearSearch();
					this.m_SearchFilter.folders = folders;
					this.m_SearchFilter.searchArea = this.m_LastLocalAssetsSearchArea;
				}
				this.m_LastFolders = this.m_SearchFilter.folders;
			}
			this.RefreshSearchText();
			this.InitListArea();
		}

		private void IconSizeSlider(Rect r)
		{
			EditorGUI.BeginChangeCheck();
			int gridSize = (int)GUI.HorizontalSlider(r, (float)this.m_ListArea.gridSize, (float)this.m_ListArea.minGridSize, (float)this.m_ListArea.maxGridSize);
			if (EditorGUI.EndChangeCheck())
			{
				AssetStorePreviewManager.AbortSize(this.m_ListArea.gridSize);
				this.m_ListArea.gridSize = gridSize;
			}
		}

		private void SearchAreaBar()
		{
			GUI.Label(this.m_ListHeaderRect, GUIContent.none, ProjectBrowser.s_Styles.topBarBg);
			Rect listHeaderRect = this.m_ListHeaderRect;
			listHeaderRect.x += 5f;
			listHeaderRect.width -= 10f;
			listHeaderRect.y += 1f;
			GUIStyle boldLabel = EditorStyles.boldLabel;
			GUI.Label(listHeaderRect, ProjectBrowser.s_Styles.m_SearchIn, boldLabel);
			if (this.m_SearchAreaMenuOffset < 0f)
			{
				this.m_SearchAreaMenuOffset = boldLabel.CalcSize(ProjectBrowser.s_Styles.m_SearchIn).x;
			}
			listHeaderRect.x += this.m_SearchAreaMenuOffset + 7f;
			listHeaderRect.width -= this.m_SearchAreaMenuOffset + 7f;
			listHeaderRect.width = this.m_SearchAreaMenu.OnGUI(listHeaderRect);
		}

		private void BreadCrumbBar()
		{
			if (this.m_ListHeaderRect.height > 0f)
			{
				if (this.m_SearchFilter.folders.Length != 0)
				{
					Event current = Event.current;
					if (current.type == EventType.MouseDown && this.m_ListHeaderRect.Contains(current.mousePosition))
					{
						GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
						base.Repaint();
					}
					if (this.m_BreadCrumbs.Count == 0)
					{
						string text = this.m_SearchFilter.folders[0];
						string[] array = text.Split(new char[]
						{
							'/'
						});
						string text2 = "";
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string text3 = array2[i];
							if (!string.IsNullOrEmpty(text2))
							{
								text2 += "/";
							}
							text2 += text3;
							this.m_BreadCrumbs.Add(new KeyValuePair<GUIContent, string>(new GUIContent(text3), text2));
						}
						this.m_BreadCrumbLastFolderHasSubFolders = (AssetDatabase.GetSubFolders(text).Length > 0);
					}
					GUI.Label(this.m_ListHeaderRect, GUIContent.none, ProjectBrowser.s_Styles.topBarBg);
					Rect listHeaderRect = this.m_ListHeaderRect;
					listHeaderRect.y += 1f;
					listHeaderRect.x += 4f;
					if (this.m_SearchFilter.folders.Length == 1)
					{
						for (int j = 0; j < this.m_BreadCrumbs.Count; j++)
						{
							bool flag = j == this.m_BreadCrumbs.Count - 1;
							GUIStyle gUIStyle = (!flag) ? EditorStyles.label : EditorStyles.boldLabel;
							GUIContent key = this.m_BreadCrumbs[j].Key;
							string value = this.m_BreadCrumbs[j].Value;
							Vector2 vector = gUIStyle.CalcSize(key);
							listHeaderRect.width = vector.x;
							if (GUI.Button(listHeaderRect, key, gUIStyle))
							{
								this.ShowFolderContents(AssetDatabase.GetMainAssetInstanceID(value), false);
							}
							listHeaderRect.x += vector.x + 3f;
							if (!flag || this.m_BreadCrumbLastFolderHasSubFolders)
							{
								Rect rect = new Rect(listHeaderRect.x, listHeaderRect.y + 2f, 13f, 13f);
								if (EditorGUI.ButtonMouseDown(rect, GUIContent.none, FocusType.Passive, ProjectBrowser.s_Styles.foldout))
								{
									string currentSubFolder = "";
									if (!flag)
									{
										currentSubFolder = this.m_BreadCrumbs[j + 1].Value;
									}
									ProjectBrowser.BreadCrumbListMenu.Show(value, currentSubFolder, rect, this);
								}
							}
							listHeaderRect.x += 11f;
						}
					}
					else if (this.m_SearchFilter.folders.Length > 1)
					{
						GUI.Label(listHeaderRect, GUIContent.Temp("Showing multiple folders..."), EditorStyles.miniLabel);
					}
				}
			}
		}

		private void BottomBar()
		{
			if (this.m_BottomBarRect.height != 0f)
			{
				Rect bottomBarRect = this.m_BottomBarRect;
				GUI.Label(bottomBarRect, GUIContent.none, ProjectBrowser.s_Styles.bottomBarBg);
				Rect r = new Rect(bottomBarRect.x + bottomBarRect.width - 55f - 16f, bottomBarRect.y + bottomBarRect.height - 17f, 55f, 17f);
				this.IconSizeSlider(r);
				EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
				bottomBarRect.width -= 4f;
				bottomBarRect.x += 2f;
				bottomBarRect.height = 17f;
				for (int i = this.m_SelectedPathSplitted.Count - 1; i >= 0; i--)
				{
					if (i == 0)
					{
						bottomBarRect.width = bottomBarRect.width - 55f - 14f;
					}
					GUI.Label(bottomBarRect, this.m_SelectedPathSplitted[i], ProjectBrowser.s_Styles.selectedPathLabel);
					bottomBarRect.y += 17f;
				}
				EditorGUIUtility.SetIconSize(new Vector2(0f, 0f));
			}
		}

		private void SelectAssetsFolder()
		{
			this.ShowFolderContents(ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID(), true);
		}

		private string ValidateCreateNewAssetPath(string pathName)
		{
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns && this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing && this.m_SearchFilter.folders.Length > 0)
			{
				if (!pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
				{
					if (Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets).Length == 0)
					{
						pathName = Path.Combine(this.m_SearchFilter.folders[0], pathName);
						pathName = pathName.Replace("\\", "/");
					}
				}
			}
			return pathName;
		}

		internal void BeginPreimportedNameEditing(int instanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
		{
			if (!this.Initialized())
			{
				this.Init();
			}
			this.EndRenaming();
			bool isCreatingNewFolder = endAction is DoCreateFolder;
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
			{
				if (this.m_SearchFilter.GetState() != SearchFilter.State.FolderBrowsing)
				{
					this.SelectAssetsFolder();
				}
				pathName = this.ValidateCreateNewAssetPath(pathName);
				if (this.m_ListAreaState.m_CreateAssetUtility.BeginNewAssetCreation(instanceID, endAction, pathName, icon, resourceFile))
				{
					this.ShowFolderContents(AssetDatabase.GetMainAssetInstanceID(this.m_ListAreaState.m_CreateAssetUtility.folder), true);
					this.m_ListArea.BeginNamingNewAsset(this.m_ListAreaState.m_CreateAssetUtility.originalName, instanceID, isCreatingNewFolder);
				}
			}
			else if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				if (this.m_SearchFilter.IsSearching())
				{
					this.ClearSearch();
				}
				AssetsTreeViewGUI assetsTreeViewGUI = this.m_AssetTree.gui as AssetsTreeViewGUI;
				if (assetsTreeViewGUI != null)
				{
					assetsTreeViewGUI.BeginCreateNewAsset(instanceID, endAction, pathName, icon, resourceFile);
				}
				else
				{
					Debug.LogError("Not valid defaultTreeViewGUI!");
				}
			}
		}

		public void FrameObject(int instanceID, bool ping)
		{
			this.FrameObjectPrivate(instanceID, !this.m_IsLocked, ping);
			if (ProjectBrowser.s_LastInteractedProjectBrowser == this)
			{
				this.m_GrabKeyboardFocusForListArea = true;
			}
		}

		private void FrameObjectPrivate(int instanceID, bool frame, bool ping)
		{
			if (instanceID != 0 && this.m_ListArea != null)
			{
				if (this.m_LastFramedID != instanceID)
				{
					this.EndPing();
				}
				this.m_LastFramedID = instanceID;
				this.ClearSearch();
				if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
				{
					this.FrameObjectInTwoColumnMode(instanceID, frame, ping);
				}
				else if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
				{
					this.m_AssetTree.Frame(instanceID, frame, ping);
				}
			}
		}

		private void FrameObjectInTwoColumnMode(int instanceID, bool frame, bool ping)
		{
			int num = 0;
			string assetPath = AssetDatabase.GetAssetPath(instanceID);
			if (!string.IsNullOrEmpty(assetPath))
			{
				string containingFolder = ProjectWindowUtil.GetContainingFolder(assetPath);
				if (!string.IsNullOrEmpty(containingFolder))
				{
					num = AssetDatabase.GetMainAssetInstanceID(containingFolder);
				}
				if (num == 0)
				{
					num = ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID();
				}
			}
			if (num != 0)
			{
				this.m_FolderTree.Frame(num, frame, ping);
				if (frame)
				{
					this.ShowFolderContents(num, true);
				}
				this.m_ListArea.Frame(instanceID, frame, ping);
			}
		}

		private static int[] GetTreeViewFolderSelection()
		{
			ProjectBrowser projectBrowser = ProjectBrowser.s_LastInteractedProjectBrowser;
			int[] result;
			if (projectBrowser != null && projectBrowser.useTreeViewSelectionInsteadOfMainSelection && projectBrowser.m_FolderTree != null)
			{
				result = ProjectBrowser.s_LastInteractedProjectBrowser.m_FolderTree.GetSelection();
			}
			else
			{
				result = new int[0];
			}
			return result;
		}

		private int GetProjectBrowserDebugID()
		{
			int result;
			for (int i = 0; i < ProjectBrowser.s_ProjectBrowsers.Count; i++)
			{
				if (ProjectBrowser.s_ProjectBrowsers[i] == this)
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		internal static void DeleteSelectedAssets(bool askIfSure)
		{
			int[] treeViewFolderSelection = ProjectBrowser.GetTreeViewFolderSelection();
			List<int> list;
			if (treeViewFolderSelection.Length > 0)
			{
				list = new List<int>(treeViewFolderSelection);
			}
			else
			{
				list = new List<int>(Selection.instanceIDs);
			}
			if (list.Count != 0)
			{
				bool flag = list.IndexOf(ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID()) >= 0;
				if (flag)
				{
					string title = "Cannot Delete";
					EditorUtility.DisplayDialog(title, "Deleting the 'Assets' folder is not allowed", "Ok");
				}
				else
				{
					List<string> mainPaths = ProjectBrowser.GetMainPaths(list);
					if (mainPaths.Count != 0)
					{
						if (askIfSure)
						{
							string text = "Delete selected asset";
							if (mainPaths.Count > 1)
							{
								text += "s";
							}
							text += "?";
							int num = 3;
							string text2 = "";
							int num2 = 0;
							while (num2 < mainPaths.Count && num2 < num)
							{
								text2 = text2 + "   " + mainPaths[num2] + "\n";
								num2++;
							}
							if (mainPaths.Count > num)
							{
								text2 += "   ...\n";
							}
							text2 += "\nYou cannot undo this action.";
							if (!EditorUtility.DisplayDialog(text, text2, "Delete", "Cancel"))
							{
								return;
							}
						}
						AssetDatabase.StartAssetEditing();
						foreach (string current in mainPaths)
						{
							AssetDatabase.MoveAssetToTrash(current);
						}
						AssetDatabase.StopAssetEditing();
						Selection.instanceIDs = new int[0];
					}
				}
			}
		}

		internal IHierarchyProperty GetHierarchyPropertyUsingFilter(string textFilter)
		{
			return FilteredHierarchyProperty.CreateHierarchyPropertyForFilter(new FilteredHierarchy(HierarchyType.Assets)
			{
				searchFilter = SearchFilter.CreateSearchFilterFromString(textFilter)
			});
		}

		internal void ShowObjectsInList(int[] instanceIDs)
		{
			if (!this.Initialized())
			{
				this.Init();
			}
			if (this.m_ViewMode == ProjectBrowser.ViewMode.TwoColumns)
			{
				this.m_ListArea.ShowObjectsInList(instanceIDs);
				this.m_FolderTree.SetSelection(new int[0], false);
			}
			else if (this.m_ViewMode == ProjectBrowser.ViewMode.OneColumn)
			{
				int[] instanceIDs2 = Selection.instanceIDs;
				for (int i = 0; i < instanceIDs2.Length; i++)
				{
					int id = instanceIDs2[i];
					this.m_AssetTree.Frame(id, true, false);
				}
			}
		}

		private static void ShowSelectedObjectsInLastInteractedProjectBrowser()
		{
			if (ProjectBrowser.s_LastInteractedProjectBrowser != null)
			{
				int[] instanceIDs = Selection.instanceIDs;
				ProjectBrowser.s_LastInteractedProjectBrowser.ShowObjectsInList(instanceIDs);
			}
		}

		protected virtual void ShowButton(Rect r)
		{
			if (ProjectBrowser.s_Styles == null)
			{
				ProjectBrowser.s_Styles = new ProjectBrowser.Styles();
			}
			this.m_IsLocked = GUI.Toggle(r, this.m_IsLocked, GUIContent.none, ProjectBrowser.s_Styles.lockButton);
		}
	}
}
