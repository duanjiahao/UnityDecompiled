using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class PresetLibraryEditor<T> where T : PresetLibrary
	{
		private class Styles
		{
			public GUIStyle innerShadowBg = PresetLibraryEditor<T>.Styles.GetStyle("InnerShadowBg");

			public GUIStyle optionsButton = PresetLibraryEditor<T>.Styles.GetStyle("PaneOptions");

			public GUIStyle newPresetStyle = new GUIStyle(EditorStyles.boldLabel);

			public GUIContent plusButtonText = new GUIContent("", "Add new preset");

			public GUIContent plusButtonTextNotCheckedOut = new GUIContent("", "To add presets you need to press the 'Check out' button below");

			public GUIContent header = new GUIContent("Presets");

			public GUIContent newPreset = new GUIContent("New");

			public Styles()
			{
				this.newPresetStyle.alignment = TextAnchor.MiddleCenter;
				this.newPresetStyle.normal.textColor = Color.white;
			}

			private static GUIStyle GetStyle(string styleName)
			{
				return styleName;
			}
		}

		private class DragState
		{
			public int dragUponIndex
			{
				get;
				set;
			}

			public int draggingIndex
			{
				get;
				set;
			}

			public bool insertAfterIndex
			{
				get;
				set;
			}

			public Rect dragUponRect
			{
				get;
				set;
			}

			public DragState()
			{
				this.dragUponIndex = -1;
				this.draggingIndex = -1;
			}

			public bool IsDragging()
			{
				return this.draggingIndex != -1;
			}
		}

		internal class PresetContextMenu
		{
			private static PresetLibraryEditor<T> s_Caller;

			private static int s_PresetIndex;

			internal static void Show(bool isOpenForEdit, int presetIndex, object newPresetObject, PresetLibraryEditor<T> caller)
			{
				PresetLibraryEditor<T>.PresetContextMenu.s_Caller = caller;
				PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex = presetIndex;
				GUIContent content = new GUIContent("Replace");
				GUIContent content2 = new GUIContent("Delete");
				GUIContent content3 = new GUIContent("Rename");
				GUIContent content4 = new GUIContent("Move To First");
				GenericMenu genericMenu = new GenericMenu();
				if (isOpenForEdit)
				{
					genericMenu.AddItem(content, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().Replace), newPresetObject);
					genericMenu.AddItem(content2, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().Delete), 0);
					if (caller.drawLabels)
					{
						genericMenu.AddItem(content3, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().Rename), 0);
					}
					genericMenu.AddItem(content4, false, new GenericMenu.MenuFunction2(new PresetLibraryEditor<T>.PresetContextMenu().MoveToTop), 0);
				}
				else
				{
					genericMenu.AddDisabledItem(content);
					genericMenu.AddDisabledItem(content2);
					if (caller.drawLabels)
					{
						genericMenu.AddDisabledItem(content3);
					}
					genericMenu.AddDisabledItem(content4);
				}
				genericMenu.ShowAsContext();
			}

			private void Delete(object userData)
			{
				PresetLibraryEditor<T>.PresetContextMenu.s_Caller.DeletePreset(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex);
			}

			private void Replace(object userData)
			{
				PresetLibraryEditor<T>.PresetContextMenu.s_Caller.ReplacePreset(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex, userData);
			}

			private void Rename(object userData)
			{
				T currentLib = PresetLibraryEditor<T>.PresetContextMenu.s_Caller.GetCurrentLib();
				string name = currentLib.GetName(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex);
				PresetLibraryEditor<T>.PresetContextMenu.s_Caller.BeginRenaming(name, PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex, 0f);
			}

			private void MoveToTop(object userData)
			{
				PresetLibraryEditor<T>.PresetContextMenu.s_Caller.MovePreset(PresetLibraryEditor<T>.PresetContextMenu.s_PresetIndex, 0, false);
			}
		}

		private class SettingsMenu
		{
			private class ViewModeData
			{
				public GUIContent text;

				public int itemHeight;

				public PresetLibraryEditorState.ItemViewMode viewmode;
			}

			private static PresetLibraryEditor<T> s_Owner;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache0;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache1;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache2;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache3;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache4;

			[CompilerGenerated]
			private static GenericMenu.MenuFunction2 <>f__mg$cache5;

			public static void Show(Rect activatorRect, PresetLibraryEditor<T> owner)
			{
				PresetLibraryEditor<T>.SettingsMenu.s_Owner = owner;
				GenericMenu genericMenu = new GenericMenu();
				int num = (int)PresetLibraryEditor<T>.SettingsMenu.s_Owner.minMaxPreviewHeight.x;
				int num2 = (int)PresetLibraryEditor<T>.SettingsMenu.s_Owner.minMaxPreviewHeight.y;
				List<PresetLibraryEditor<T>.SettingsMenu.ViewModeData> list;
				if (num == num2)
				{
					list = new List<PresetLibraryEditor<T>.SettingsMenu.ViewModeData>
					{
						new PresetLibraryEditor<T>.SettingsMenu.ViewModeData
						{
							text = new GUIContent("Grid"),
							itemHeight = num,
							viewmode = PresetLibraryEditorState.ItemViewMode.Grid
						},
						new PresetLibraryEditor<T>.SettingsMenu.ViewModeData
						{
							text = new GUIContent("List"),
							itemHeight = num,
							viewmode = PresetLibraryEditorState.ItemViewMode.List
						}
					};
				}
				else
				{
					list = new List<PresetLibraryEditor<T>.SettingsMenu.ViewModeData>
					{
						new PresetLibraryEditor<T>.SettingsMenu.ViewModeData
						{
							text = new GUIContent("Small Grid"),
							itemHeight = num,
							viewmode = PresetLibraryEditorState.ItemViewMode.Grid
						},
						new PresetLibraryEditor<T>.SettingsMenu.ViewModeData
						{
							text = new GUIContent("Large Grid"),
							itemHeight = num2,
							viewmode = PresetLibraryEditorState.ItemViewMode.Grid
						},
						new PresetLibraryEditor<T>.SettingsMenu.ViewModeData
						{
							text = new GUIContent("Small List"),
							itemHeight = num,
							viewmode = PresetLibraryEditorState.ItemViewMode.List
						},
						new PresetLibraryEditor<T>.SettingsMenu.ViewModeData
						{
							text = new GUIContent("Large List"),
							itemHeight = num2,
							viewmode = PresetLibraryEditorState.ItemViewMode.List
						}
					};
				}
				for (int i = 0; i < list.Count; i++)
				{
					bool flag = PresetLibraryEditor<T>.SettingsMenu.s_Owner.itemViewMode == list[i].viewmode && (int)PresetLibraryEditor<T>.SettingsMenu.s_Owner.previewHeight == list[i].itemHeight;
					GenericMenu arg_1FF_0 = genericMenu;
					GUIContent arg_1FF_1 = list[i].text;
					bool arg_1FF_2 = flag;
					if (PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache0 == null)
					{
						PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache0 = new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.ViewModeChange);
					}
					arg_1FF_0.AddItem(arg_1FF_1, arg_1FF_2, PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache0, list[i]);
				}
				genericMenu.AddSeparator("");
				List<string> list2;
				List<string> list3;
				ScriptableSingleton<PresetLibraryManager>.instance.GetAvailableLibraries<T>(PresetLibraryEditor<T>.SettingsMenu.s_Owner.m_SaveLoadHelper, out list2, out list3);
				list2.Sort();
				list3.Sort();
				string a = PresetLibraryEditor<T>.SettingsMenu.s_Owner.currentLibraryWithoutExtension + "." + PresetLibraryEditor<T>.SettingsMenu.s_Owner.m_SaveLoadHelper.fileExtensionWithoutDot;
				string str = " (Project)";
				foreach (string current in list2)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(current);
					GenericMenu arg_2C8_0 = genericMenu;
					GUIContent arg_2C8_1 = new GUIContent(fileNameWithoutExtension);
					bool arg_2C8_2 = a == current;
					if (PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache1 == null)
					{
						PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache1 = new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.LibraryModeChange);
					}
					arg_2C8_0.AddItem(arg_2C8_1, arg_2C8_2, PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache1, current);
				}
				foreach (string current2 in list3)
				{
					string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(current2);
					GenericMenu arg_346_0 = genericMenu;
					GUIContent arg_346_1 = new GUIContent(fileNameWithoutExtension2 + str);
					bool arg_346_2 = a == current2;
					if (PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache2 == null)
					{
						PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache2 = new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.LibraryModeChange);
					}
					arg_346_0.AddItem(arg_346_1, arg_346_2, PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache2, current2);
				}
				genericMenu.AddSeparator("");
				GenericMenu arg_3A5_0 = genericMenu;
				GUIContent arg_3A5_1 = new GUIContent("Create New Library...");
				bool arg_3A5_2 = false;
				if (PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache3 == null)
				{
					PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache3 = new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.CreateLibrary);
				}
				arg_3A5_0.AddItem(arg_3A5_1, arg_3A5_2, PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache3, 0);
				if (PresetLibraryEditor<T>.SettingsMenu.HasDefaultPresets())
				{
					genericMenu.AddSeparator("");
					GenericMenu arg_3EF_0 = genericMenu;
					GUIContent arg_3EF_1 = new GUIContent("Add Factory Presets To Current Library");
					bool arg_3EF_2 = false;
					if (PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache4 == null)
					{
						PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache4 = new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.AddDefaultPresetsToCurrentLibrary);
					}
					arg_3EF_0.AddItem(arg_3EF_1, arg_3EF_2, PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache4, 0);
				}
				genericMenu.AddSeparator("");
				GenericMenu arg_42F_0 = genericMenu;
				GUIContent arg_42F_1 = new GUIContent("Reveal Current Library Location");
				bool arg_42F_2 = false;
				if (PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache5 == null)
				{
					PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache5 = new GenericMenu.MenuFunction2(PresetLibraryEditor<T>.SettingsMenu.RevealCurrentLibrary);
				}
				arg_42F_0.AddItem(arg_42F_1, arg_42F_2, PresetLibraryEditor<T>.SettingsMenu.<>f__mg$cache5, 0);
				genericMenu.DropDown(activatorRect);
			}

			private static void ViewModeChange(object userData)
			{
				PresetLibraryEditor<T>.SettingsMenu.ViewModeData viewModeData = (PresetLibraryEditor<T>.SettingsMenu.ViewModeData)userData;
				PresetLibraryEditor<T>.SettingsMenu.s_Owner.itemViewMode = viewModeData.viewmode;
				PresetLibraryEditor<T>.SettingsMenu.s_Owner.previewHeight = (float)viewModeData.itemHeight;
			}

			private static void LibraryModeChange(object userData)
			{
				string currentLibraryWithoutExtension = (string)userData;
				PresetLibraryEditor<T>.SettingsMenu.s_Owner.currentLibraryWithoutExtension = currentLibraryWithoutExtension;
			}

			private static void CreateLibrary(object userData)
			{
				PresetLibraryEditor<T>.SettingsMenu.s_Owner.wantsToCreateLibrary = true;
			}

			private static void RevealCurrentLibrary(object userData)
			{
				PresetLibraryEditor<T>.SettingsMenu.s_Owner.RevealCurrentLibrary();
			}

			private static bool HasDefaultPresets()
			{
				return PresetLibraryEditor<T>.SettingsMenu.s_Owner.addDefaultPresets != null;
			}

			private static void AddDefaultPresetsToCurrentLibrary(object userData)
			{
				if (PresetLibraryEditor<T>.SettingsMenu.s_Owner.addDefaultPresets != null)
				{
					PresetLibraryEditor<T>.SettingsMenu.s_Owner.addDefaultPresets(PresetLibraryEditor<T>.SettingsMenu.s_Owner.GetCurrentLib());
				}
				PresetLibraryEditor<T>.SettingsMenu.s_Owner.SaveCurrentLib();
			}
		}

		private static PresetLibraryEditor<T>.Styles s_Styles;

		private PresetLibraryEditor<T>.DragState m_DragState = new PresetLibraryEditor<T>.DragState();

		private readonly VerticalGrid m_Grid = new VerticalGrid();

		private readonly PresetLibraryEditorState m_State;

		private readonly ScriptableObjectSaveLoadHelper<T> m_SaveLoadHelper;

		private readonly Action<int, object> m_ItemClickedCallback;

		public Action<PresetLibrary> addDefaultPresets;

		public Action presetsWasReordered;

		private const float kGridLabelHeight = 16f;

		private const float kCheckoutButtonMaxWidth = 100f;

		private const float kCheckoutButtonMargin = 2f;

		private Vector2 m_MinMaxPreviewHeight = new Vector2(14f, 64f);

		private float m_PreviewAspect = 8f;

		private bool m_ShowAddNewPresetItem = true;

		private bool m_ShowedScrollBarLastFrame = false;

		private bool m_IsOpenForEdit = true;

		private PresetFileLocation m_PresetLibraryFileLocation;

		public float contentHeight
		{
			get;
			private set;
		}

		private float topAreaHeight
		{
			get
			{
				return 20f;
			}
		}

		private float versionControlAreaHeight
		{
			get
			{
				return 20f;
			}
		}

		private float gridWidth
		{
			get;
			set;
		}

		public bool wantsToCreateLibrary
		{
			get;
			set;
		}

		public bool showHeader
		{
			get;
			set;
		}

		public float settingsMenuRightMargin
		{
			get;
			set;
		}

		public bool alwaysShowScrollAreaHorizontalLines
		{
			get;
			set;
		}

		public bool useOnePixelOverlappedGrid
		{
			get;
			set;
		}

		public RectOffset marginsForList
		{
			get;
			set;
		}

		public RectOffset marginsForGrid
		{
			get;
			set;
		}

		public string currentLibraryWithoutExtension
		{
			get
			{
				return this.m_State.m_CurrrentLibrary;
			}
			set
			{
				this.m_State.m_CurrrentLibrary = Path.ChangeExtension(value, null);
				this.m_PresetLibraryFileLocation = PresetLibraryLocations.GetFileLocationFromPath(this.m_State.m_CurrrentLibrary);
				this.OnLayoutChanged();
				this.Repaint();
			}
		}

		public float previewAspect
		{
			get
			{
				return this.m_PreviewAspect;
			}
			set
			{
				this.m_PreviewAspect = value;
			}
		}

		public Vector2 minMaxPreviewHeight
		{
			get
			{
				return this.m_MinMaxPreviewHeight;
			}
			set
			{
				this.m_MinMaxPreviewHeight = value;
				this.previewHeight = this.previewHeight;
			}
		}

		public float previewHeight
		{
			get
			{
				return this.m_State.m_PreviewHeight;
			}
			set
			{
				this.m_State.m_PreviewHeight = Mathf.Clamp(value, this.minMaxPreviewHeight.x, this.minMaxPreviewHeight.y);
				this.Repaint();
			}
		}

		public PresetLibraryEditorState.ItemViewMode itemViewMode
		{
			get
			{
				return this.m_State.itemViewMode;
			}
			set
			{
				this.m_State.itemViewMode = value;
				this.OnLayoutChanged();
				this.Repaint();
			}
		}

		private bool drawLabels
		{
			get
			{
				return this.m_State.itemViewMode == PresetLibraryEditorState.ItemViewMode.List;
			}
		}

		private string pathWithExtension
		{
			get
			{
				return this.currentLibraryWithoutExtension + "." + this.m_SaveLoadHelper.fileExtensionWithoutDot;
			}
		}

		public PresetLibraryEditor(ScriptableObjectSaveLoadHelper<T> helper, PresetLibraryEditorState state, Action<int, object> itemClickedCallback)
		{
			this.m_SaveLoadHelper = helper;
			this.m_State = state;
			this.m_ItemClickedCallback = itemClickedCallback;
			this.settingsMenuRightMargin = 10f;
			this.useOnePixelOverlappedGrid = false;
			this.alwaysShowScrollAreaHorizontalLines = true;
			this.marginsForList = new RectOffset(10, 10, 5, 5);
			this.marginsForGrid = new RectOffset(5, 5, 5, 5);
			this.m_PresetLibraryFileLocation = PresetLibraryLocations.GetFileLocationFromPath(this.currentLibraryWithoutExtension);
		}

		public void InitializeGrid(float availableWidth)
		{
			T currentLib = this.GetCurrentLib();
			if (currentLib != null)
			{
				if (availableWidth > 0f)
				{
					this.SetupGrid(availableWidth, currentLib.Count());
				}
			}
			else
			{
				Debug.LogError("Could not load preset library " + this.currentLibraryWithoutExtension);
			}
		}

		private void Repaint()
		{
			HandleUtility.Repaint();
		}

		private void ValidateNoExtension(string value)
		{
			if (Path.HasExtension(value))
			{
				Debug.LogError("currentLibraryWithoutExtension should not have an extension: " + value);
			}
		}

		private string CreateNewLibraryCallback(string libraryName, PresetFileLocation fileLocation)
		{
			string defaultFilePathForFileLocation = PresetLibraryLocations.GetDefaultFilePathForFileLocation(fileLocation);
			string text = Path.Combine(defaultFilePathForFileLocation, libraryName);
			if (this.CreateNewLibrary(text) != null)
			{
				this.currentLibraryWithoutExtension = text;
			}
			return ScriptableSingleton<PresetLibraryManager>.instance.GetLastError();
		}

		private static bool IsItemVisible(float scrollHeight, float itemYMin, float itemYMax, float scrollPos)
		{
			float num = itemYMin - scrollPos;
			float num2 = itemYMax - scrollPos;
			return num2 >= 0f && num <= scrollHeight;
		}

		private void OnLayoutChanged()
		{
			T currentLib = this.GetCurrentLib();
			if (!(currentLib == null) && this.gridWidth > 0f)
			{
				this.SetupGrid(this.gridWidth, currentLib.Count());
			}
		}

		private void SetupGrid(float width, int itemCount)
		{
			if (width < 1f)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Invalid width ",
					width,
					", ",
					Event.current.type
				}));
			}
			else
			{
				if (this.m_ShowAddNewPresetItem)
				{
					itemCount++;
				}
				this.m_Grid.useFixedHorizontalSpacing = this.useOnePixelOverlappedGrid;
				this.m_Grid.fixedHorizontalSpacing = (float)((!this.useOnePixelOverlappedGrid) ? 0 : -1);
				PresetLibraryEditorState.ItemViewMode itemViewMode = this.m_State.itemViewMode;
				if (itemViewMode != PresetLibraryEditorState.ItemViewMode.Grid)
				{
					if (itemViewMode == PresetLibraryEditorState.ItemViewMode.List)
					{
						this.m_Grid.fixedWidth = width;
						this.m_Grid.topMargin = (float)this.marginsForList.top;
						this.m_Grid.bottomMargin = (float)this.marginsForList.bottom;
						this.m_Grid.leftMargin = (float)this.marginsForList.left;
						this.m_Grid.rightMargin = (float)this.marginsForList.right;
						this.m_Grid.verticalSpacing = 2f;
						this.m_Grid.minHorizontalSpacing = 0f;
						this.m_Grid.itemSize = new Vector2(width - this.m_Grid.leftMargin, this.m_State.m_PreviewHeight);
						this.m_Grid.InitNumRowsAndColumns(itemCount, 2147483647);
					}
				}
				else
				{
					this.m_Grid.fixedWidth = width;
					this.m_Grid.topMargin = (float)this.marginsForGrid.top;
					this.m_Grid.bottomMargin = (float)this.marginsForGrid.bottom;
					this.m_Grid.leftMargin = (float)this.marginsForGrid.left;
					this.m_Grid.rightMargin = (float)this.marginsForGrid.right;
					this.m_Grid.verticalSpacing = (float)((!this.useOnePixelOverlappedGrid) ? 2 : -1);
					this.m_Grid.minHorizontalSpacing = 1f;
					this.m_Grid.itemSize = new Vector2(this.m_State.m_PreviewHeight * this.m_PreviewAspect, this.m_State.m_PreviewHeight);
					this.m_Grid.InitNumRowsAndColumns(itemCount, 2147483647);
				}
				float num = this.m_Grid.CalcRect(itemCount - 1, 0f).yMax + this.m_Grid.bottomMargin;
				this.contentHeight = this.topAreaHeight + num + ((!this.m_IsOpenForEdit) ? this.versionControlAreaHeight : 0f);
			}
		}

		public void OnGUI(Rect rect, object presetObject)
		{
			if (rect.width >= 2f)
			{
				this.m_State.m_RenameOverlay.OnEvent();
				T currentLib = this.GetCurrentLib();
				if (PresetLibraryEditor<T>.s_Styles == null)
				{
					PresetLibraryEditor<T>.s_Styles = new PresetLibraryEditor<T>.Styles();
				}
				Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.topAreaHeight);
				Rect rect3 = new Rect(rect.x, rect2.yMax, rect.width, rect.height - this.topAreaHeight);
				this.TopArea(rect2);
				this.ListArea(rect3, currentLib, presetObject);
			}
		}

		private void TopArea(Rect rect)
		{
			GUI.BeginGroup(rect);
			if (this.showHeader)
			{
				GUI.Label(new Rect(10f, 2f, rect.width - 20f, rect.height), PresetLibraryEditor<T>.s_Styles.header);
			}
			Rect rect2 = new Rect(rect.width - 16f - this.settingsMenuRightMargin, (rect.height - 6f) * 0.5f, 16f, rect.height);
			if (Event.current.type == EventType.Repaint)
			{
				PresetLibraryEditor<T>.s_Styles.optionsButton.Draw(rect2, false, false, false, false);
			}
			rect2.y = 0f;
			rect2.height = rect.height;
			rect2.width = 24f;
			if (GUI.Button(rect2, GUIContent.none, GUIStyle.none))
			{
				PresetLibraryEditor<T>.SettingsMenu.Show(rect2, this);
			}
			if (this.wantsToCreateLibrary)
			{
				this.wantsToCreateLibrary = false;
				PopupWindow.Show(rect2, new PopupWindowContentForNewLibrary(new Func<string, PresetFileLocation, string>(this.CreateNewLibraryCallback)));
				GUIUtility.ExitGUI();
			}
			GUI.EndGroup();
		}

		private Rect GetDragRect(Rect itemRect)
		{
			int num = Mathf.FloorToInt(this.m_Grid.horizontalSpacing * 0.5f + 0.5f);
			int num2 = Mathf.FloorToInt(this.m_Grid.verticalSpacing * 0.5f + 0.5f);
			return new RectOffset(num, num, num2, num2).Add(itemRect);
		}

		private void ClearDragState()
		{
			this.m_DragState.dragUponIndex = -1;
			this.m_DragState.draggingIndex = -1;
		}

		private void DrawHoverEffect(Rect itemRect, bool drawAsSelection)
		{
			Color color = GUI.color;
			GUI.color = new Color(0f, 0f, 0.4f, (!drawAsSelection) ? 0.3f : 0.8f);
			Rect position = new RectOffset(3, 3, 3, 3).Add(itemRect);
			GUI.Label(position, GUIContent.none, EditorStyles.helpBox);
			GUI.color = color;
		}

		private void VersionControlArea(Rect rect)
		{
			if (rect.width > 100f)
			{
				rect = new Rect(rect.xMax - 100f - 2f, rect.y + 2f, 100f, rect.height - 4f);
			}
			if (GUI.Button(rect, "Check out", EditorStyles.miniButton))
			{
				Provider.Checkout(new string[]
				{
					this.pathWithExtension
				}, CheckoutMode.Asset);
			}
		}

		private void ListArea(Rect rect, PresetLibrary lib, object newPresetObject)
		{
			if (!(lib == null))
			{
				Event current = Event.current;
				if (this.m_PresetLibraryFileLocation == PresetFileLocation.ProjectFolder && current.type == EventType.Repaint)
				{
					this.m_IsOpenForEdit = AssetDatabase.IsOpenForEdit(this.pathWithExtension);
				}
				else if (this.m_PresetLibraryFileLocation == PresetFileLocation.PreferencesFolder)
				{
					this.m_IsOpenForEdit = true;
				}
				if (!this.m_IsOpenForEdit)
				{
					Rect rect2 = new Rect(rect.x, rect.yMax - this.versionControlAreaHeight, rect.width, this.versionControlAreaHeight);
					this.VersionControlArea(rect2);
					rect.height -= this.versionControlAreaHeight;
				}
				for (int i = 0; i < 2; i++)
				{
					this.gridWidth = ((!this.m_ShowedScrollBarLastFrame) ? rect.width : (rect.width - 17f));
					this.SetupGrid(this.gridWidth, lib.Count());
					bool flag = this.m_Grid.height > rect.height;
					if (flag == this.m_ShowedScrollBarLastFrame)
					{
						break;
					}
					this.m_ShowedScrollBarLastFrame = flag;
				}
				if ((this.m_ShowedScrollBarLastFrame || this.alwaysShowScrollAreaHorizontalLines) && Event.current.type == EventType.Repaint)
				{
					Rect rect3 = new RectOffset(1, 1, 1, 1).Add(rect);
					rect3.height = 1f;
					EditorGUI.DrawRect(rect3, new Color(0f, 0f, 0f, 0.3f));
					rect3.y += rect.height + 1f;
					EditorGUI.DrawRect(rect3, new Color(0f, 0f, 0f, 0.3f));
				}
				Rect viewRect = new Rect(0f, 0f, 1f, this.m_Grid.height);
				this.m_State.m_ScrollPosition = GUI.BeginScrollView(rect, this.m_State.m_ScrollPosition, viewRect);
				float num = 0f;
				int maxIndex = (!this.m_ShowAddNewPresetItem) ? (lib.Count() - 1) : lib.Count();
				int num2;
				int num3;
				bool flag2 = this.m_Grid.IsVisibleInScrollView(rect.height, this.m_State.m_ScrollPosition.y, num, maxIndex, out num2, out num3);
				bool flag3 = false;
				if (flag2)
				{
					if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().isWaitingForDelay)
					{
						if (!this.m_State.m_RenameOverlay.OnGUI())
						{
							this.EndRename();
							current.Use();
						}
						this.Repaint();
					}
					for (int j = num2; j <= num3; j++)
					{
						int num4 = j + 1000000;
						Rect rect4 = this.m_Grid.CalcRect(j, num);
						Rect rect5 = rect4;
						Rect rect6 = rect4;
						PresetLibraryEditorState.ItemViewMode itemViewMode = this.m_State.itemViewMode;
						if (itemViewMode != PresetLibraryEditorState.ItemViewMode.List)
						{
							if (itemViewMode != PresetLibraryEditorState.ItemViewMode.Grid)
							{
							}
						}
						else
						{
							rect5.width = this.m_State.m_PreviewHeight * this.m_PreviewAspect;
							rect6.x += rect5.width + 8f;
							rect6.width -= rect5.width + 10f;
							rect6.height = 16f;
							rect6.y = rect4.yMin + (rect4.height - 16f) * 0.5f;
						}
						if (this.m_ShowAddNewPresetItem && j == lib.Count())
						{
							this.CreateNewPresetButton(rect5, newPresetObject, lib, this.m_IsOpenForEdit);
						}
						else
						{
							bool flag4 = this.IsRenaming(j);
							if (flag4)
							{
								Rect editFieldRect = rect6;
								editFieldRect.y -= 1f;
								editFieldRect.x -= 1f;
								this.m_State.m_RenameOverlay.editFieldRect = editFieldRect;
							}
							switch (current.type)
							{
							case EventType.MouseDown:
								if (current.button == 0 && rect4.Contains(current.mousePosition))
								{
									GUIUtility.hotControl = num4;
									if (current.clickCount == 1)
									{
										this.m_ItemClickedCallback(current.clickCount, lib.GetPreset(j));
										current.Use();
									}
								}
								break;
							case EventType.MouseUp:
								if (GUIUtility.hotControl == num4)
								{
									GUIUtility.hotControl = 0;
									if (current.button == 0 && rect4.Contains(current.mousePosition))
									{
										if (Event.current.alt && this.m_IsOpenForEdit)
										{
											this.DeletePreset(j);
											current.Use();
										}
									}
								}
								break;
							case EventType.MouseMove:
								if (rect4.Contains(current.mousePosition))
								{
									if (this.m_State.m_HoverIndex != j)
									{
										this.m_State.m_HoverIndex = j;
										this.Repaint();
									}
								}
								else if (this.m_State.m_HoverIndex == j)
								{
									this.m_State.m_HoverIndex = -1;
									this.Repaint();
								}
								break;
							case EventType.MouseDrag:
								if (GUIUtility.hotControl == num4 && this.m_IsOpenForEdit)
								{
									DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), num4);
									if (dragAndDropDelay.CanStartDrag())
									{
										DragAndDrop.PrepareStartDrag();
										DragAndDrop.SetGenericData("DraggingPreset", j);
										DragAndDrop.objectReferences = new UnityEngine.Object[0];
										DragAndDrop.StartDrag("");
										this.m_DragState.draggingIndex = j;
										this.m_DragState.dragUponIndex = j;
										GUIUtility.hotControl = 0;
									}
									current.Use();
								}
								break;
							case EventType.Repaint:
								if (this.m_State.m_HoverIndex == j)
								{
									if (!rect4.Contains(current.mousePosition))
									{
										this.m_State.m_HoverIndex = -1;
									}
								}
								if (this.m_DragState.draggingIndex == j || GUIUtility.hotControl == num4)
								{
									this.DrawHoverEffect(rect4, false);
								}
								lib.Draw(rect5, j);
								if (!flag4 && this.drawLabels)
								{
									GUI.Label(rect6, GUIContent.Temp(lib.GetName(j)));
								}
								if (this.m_DragState.dragUponIndex == j && this.m_DragState.draggingIndex != this.m_DragState.dragUponIndex)
								{
									flag3 = true;
								}
								if (GUIUtility.hotControl == 0 && Event.current.alt && this.m_IsOpenForEdit)
								{
									EditorGUIUtility.AddCursorRect(rect4, MouseCursor.ArrowMinus);
								}
								break;
							case EventType.DragUpdated:
							case EventType.DragPerform:
							{
								Rect dragRect = this.GetDragRect(rect4);
								if (dragRect.Contains(current.mousePosition))
								{
									this.m_DragState.dragUponIndex = j;
									this.m_DragState.dragUponRect = rect4;
									if (this.m_State.itemViewMode == PresetLibraryEditorState.ItemViewMode.List)
									{
										this.m_DragState.insertAfterIndex = ((current.mousePosition.y - dragRect.y) / dragRect.height > 0.5f);
									}
									else
									{
										this.m_DragState.insertAfterIndex = ((current.mousePosition.x - dragRect.x) / dragRect.width > 0.5f);
									}
									bool flag5 = current.type == EventType.DragPerform;
									if (flag5)
									{
										if (this.m_DragState.draggingIndex >= 0)
										{
											this.MovePreset(this.m_DragState.draggingIndex, this.m_DragState.dragUponIndex, this.m_DragState.insertAfterIndex);
											DragAndDrop.AcceptDrag();
										}
										this.ClearDragState();
									}
									DragAndDrop.visualMode = DragAndDropVisualMode.Move;
									current.Use();
								}
								break;
							}
							case EventType.DragExited:
								if (this.m_DragState.IsDragging())
								{
									this.ClearDragState();
									current.Use();
								}
								break;
							case EventType.ContextClick:
								if (rect4.Contains(current.mousePosition))
								{
									PresetLibraryEditor<T>.PresetContextMenu.Show(this.m_IsOpenForEdit, j, newPresetObject, this);
									current.Use();
								}
								break;
							}
						}
					}
					if (flag3)
					{
						this.DrawDragInsertionMarker();
					}
				}
				GUI.EndScrollView();
			}
		}

		private void DrawDragInsertionMarker()
		{
			if (this.m_DragState.IsDragging())
			{
				Rect dragRect = this.GetDragRect(this.m_DragState.dragUponRect);
				Rect rect;
				if (this.m_State.itemViewMode == PresetLibraryEditorState.ItemViewMode.List)
				{
					if (this.m_DragState.insertAfterIndex)
					{
						rect = new Rect(dragRect.xMin, dragRect.yMax - 1f, dragRect.width, 2f);
					}
					else
					{
						rect = new Rect(dragRect.xMin, dragRect.yMin - 1f, dragRect.width, 2f);
					}
				}
				else if (this.m_DragState.insertAfterIndex)
				{
					rect = new Rect(dragRect.xMax - 1f, dragRect.yMin, 2f, dragRect.height);
				}
				else
				{
					rect = new Rect(dragRect.xMin - 1f, dragRect.yMin, 2f, dragRect.height);
				}
				EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 1f));
			}
		}

		private void CreateNewPresetButton(Rect buttonRect, object newPresetObject, PresetLibrary lib, bool isOpenForEdit)
		{
			using (new EditorGUI.DisabledScope(!isOpenForEdit))
			{
				if (GUI.Button(buttonRect, (!isOpenForEdit) ? PresetLibraryEditor<T>.s_Styles.plusButtonTextNotCheckedOut : PresetLibraryEditor<T>.s_Styles.plusButtonText))
				{
					int itemIndex = this.CreateNewPreset(newPresetObject, "");
					if (this.drawLabels)
					{
						this.BeginRenaming("", itemIndex, 0f);
					}
					InspectorWindow.RepaintAllInspectors();
				}
				if (Event.current.type == EventType.Repaint)
				{
					Rect rect = new RectOffset(-3, -3, -3, -3).Add(buttonRect);
					lib.Draw(rect, newPresetObject);
					if (buttonRect.width > 30f)
					{
						PresetLibraryEditor<T>.LabelWithOutline(buttonRect, PresetLibraryEditor<T>.s_Styles.newPreset, new Color(0.1f, 0.1f, 0.1f), PresetLibraryEditor<T>.s_Styles.newPresetStyle);
					}
					else if (lib.Count() == 0 && isOpenForEdit)
					{
						buttonRect.x = buttonRect.xMax + 5f;
						buttonRect.width = 200f;
						buttonRect.height = 16f;
						using (new EditorGUI.DisabledScope(true))
						{
							GUI.Label(buttonRect, "Click to add new preset");
						}
					}
				}
			}
		}

		private static void LabelWithOutline(Rect rect, GUIContent content, Color outlineColor, GUIStyle style)
		{
			Color color = GUI.color;
			GUI.color = outlineColor;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Rect position = rect;
						position.x += (float)j;
						position.y += (float)i;
						GUI.Label(position, content, style);
					}
				}
			}
			GUI.color = color;
			GUI.Label(rect, content, style);
		}

		private bool IsRenaming(int itemID)
		{
			return this.GetRenameOverlay().IsRenaming() && this.GetRenameOverlay().userData == itemID && !this.GetRenameOverlay().isWaitingForDelay;
		}

		private RenameOverlay GetRenameOverlay()
		{
			return this.m_State.m_RenameOverlay;
		}

		private void BeginRenaming(string name, int itemIndex, float delay)
		{
			this.GetRenameOverlay().BeginRename(name, itemIndex, delay);
		}

		private void EndRename()
		{
			if (this.GetRenameOverlay().userAcceptedRename)
			{
				string name = (!string.IsNullOrEmpty(this.GetRenameOverlay().name)) ? this.GetRenameOverlay().name : this.GetRenameOverlay().originalName;
				int userData = this.GetRenameOverlay().userData;
				T currentLib = this.GetCurrentLib();
				if (userData >= 0 && userData < currentLib.Count())
				{
					currentLib.SetName(userData, name);
					this.SaveCurrentLib();
				}
				this.GetRenameOverlay().EndRename(true);
			}
		}

		public T GetCurrentLib()
		{
			T t = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, this.currentLibraryWithoutExtension);
			if (t == null)
			{
				t = ScriptableSingleton<PresetLibraryManager>.instance.GetLibrary<T>(this.m_SaveLoadHelper, PresetLibraryLocations.defaultPresetLibraryPath);
				if (t == null)
				{
					t = this.CreateNewLibrary(PresetLibraryLocations.defaultPresetLibraryPath);
					if (t != null)
					{
						if (this.addDefaultPresets != null)
						{
							this.addDefaultPresets(t);
							ScriptableSingleton<PresetLibraryManager>.instance.SaveLibrary<T>(this.m_SaveLoadHelper, t, PresetLibraryLocations.defaultPresetLibraryPath);
						}
					}
					else
					{
						Debug.LogError("Could not create Default preset library " + ScriptableSingleton<PresetLibraryManager>.instance.GetLastError());
					}
				}
				this.currentLibraryWithoutExtension = PresetLibraryLocations.defaultPresetLibraryPath;
			}
			return t;
		}

		public void UnloadUsedLibraries()
		{
			ScriptableSingleton<PresetLibraryManager>.instance.UnloadAllLibrariesFor<T>(this.m_SaveLoadHelper);
		}

		public void DeletePreset(int presetIndex)
		{
			T currentLib = this.GetCurrentLib();
			if (!(currentLib == null))
			{
				if (presetIndex < 0 || presetIndex >= currentLib.Count())
				{
					Debug.LogError("DeletePreset: Invalid index: out of bounds");
				}
				else
				{
					currentLib.Remove(presetIndex);
					this.SaveCurrentLib();
					if (this.presetsWasReordered != null)
					{
						this.presetsWasReordered();
					}
					this.OnLayoutChanged();
				}
			}
		}

		public void ReplacePreset(int presetIndex, object presetObject)
		{
			T currentLib = this.GetCurrentLib();
			if (!(currentLib == null))
			{
				if (presetIndex < 0 || presetIndex >= currentLib.Count())
				{
					Debug.LogError("ReplacePreset: Invalid index: out of bounds");
				}
				else
				{
					currentLib.Replace(presetIndex, presetObject);
					this.SaveCurrentLib();
					if (this.presetsWasReordered != null)
					{
						this.presetsWasReordered();
					}
				}
			}
		}

		public void MovePreset(int presetIndex, int destPresetIndex, bool insertAfterDestIndex)
		{
			T currentLib = this.GetCurrentLib();
			if (!(currentLib == null))
			{
				if (presetIndex < 0 || presetIndex >= currentLib.Count())
				{
					Debug.LogError("ReplacePreset: Invalid index: out of bounds");
				}
				else
				{
					currentLib.Move(presetIndex, destPresetIndex, insertAfterDestIndex);
					this.SaveCurrentLib();
					if (this.presetsWasReordered != null)
					{
						this.presetsWasReordered();
					}
				}
			}
		}

		public int CreateNewPreset(object presetObject, string presetName)
		{
			T currentLib = this.GetCurrentLib();
			int result;
			if (currentLib == null)
			{
				Debug.Log("No current library selected!");
				result = -1;
			}
			else
			{
				currentLib.Add(presetObject, presetName);
				this.SaveCurrentLib();
				if (this.presetsWasReordered != null)
				{
					this.presetsWasReordered();
				}
				this.Repaint();
				this.OnLayoutChanged();
				result = currentLib.Count() - 1;
			}
			return result;
		}

		public void SaveCurrentLib()
		{
			T currentLib = this.GetCurrentLib();
			if (currentLib == null)
			{
				Debug.Log("No current library selected!");
			}
			else
			{
				ScriptableSingleton<PresetLibraryManager>.instance.SaveLibrary<T>(this.m_SaveLoadHelper, currentLib, this.currentLibraryWithoutExtension);
				InternalEditorUtility.RepaintAllViews();
			}
		}

		public T CreateNewLibrary(string presetLibraryPathWithoutExtension)
		{
			T t = ScriptableSingleton<PresetLibraryManager>.instance.CreateLibrary<T>(this.m_SaveLoadHelper, presetLibraryPathWithoutExtension);
			if (t != null)
			{
				ScriptableSingleton<PresetLibraryManager>.instance.SaveLibrary<T>(this.m_SaveLoadHelper, t, presetLibraryPathWithoutExtension);
				InternalEditorUtility.RepaintAllViews();
			}
			return t;
		}

		public void RevealCurrentLibrary()
		{
			if (this.m_PresetLibraryFileLocation == PresetFileLocation.PreferencesFolder)
			{
				EditorUtility.RevealInFinder(Path.GetFullPath(this.pathWithExtension));
			}
			else
			{
				EditorGUIUtility.PingObject(AssetDatabase.GetMainAssetInstanceID(this.pathWithExtension));
			}
		}
	}
}
