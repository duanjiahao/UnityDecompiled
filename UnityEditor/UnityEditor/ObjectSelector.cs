using System;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	internal class ObjectSelector : EditorWindow
	{
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
		}

		private const float kMinTopSize = 250f;

		private const float kMinWidth = 200f;

		private const float kPreviewMargin = 5f;

		private const float kPreviewExpandedAreaHeight = 75f;

		private ObjectSelector.Styles m_Styles;

		private string m_RequiredType;

		private string m_SearchFilter;

		private bool m_FocusSearchFilter;

		private bool m_AllowSceneObjects;

		private bool m_IsShowingAssets;

		private SavedInt m_StartGridSize = new SavedInt("ObjectSelector.GridSize", 64);

		internal int objectSelectorID;

		private int m_ModalUndoGroup = -1;

		private UnityEngine.Object m_OriginalSelection;

		private EditorCache m_EditorCache;

		private GUIView m_DelegateView;

		private PreviewResizer m_PreviewResizer = new PreviewResizer();

		private List<int> m_AllowedIDs;

		private ObjectListAreaState m_ListAreaState;

		private ObjectListArea m_ListArea;

		private ObjectTreeForSelector m_ObjectTreeWithSearch = new ObjectTreeForSelector();

		private UnityEngine.Object m_ObjectBeingEdited;

		private float m_ToolbarHeight = 44f;

		private float m_PreviewSize;

		private float m_TopSize;

		private AnimBool m_ShowWidePreview = new AnimBool();

		private AnimBool m_ShowOverlapPreview = new AnimBool();

		private static ObjectSelector s_SharedObjectSelector;

		private Rect listPosition
		{
			get
			{
				return new Rect(0f, this.m_ToolbarHeight, base.position.width, Mathf.Max(0f, this.m_TopSize - this.m_ToolbarHeight));
			}
		}

		public List<int> allowedInstanceIDs
		{
			get
			{
				return this.m_AllowedIDs;
			}
		}

		public static ObjectSelector get
		{
			get
			{
				if (ObjectSelector.s_SharedObjectSelector == null)
				{
					UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(ObjectSelector));
					if (array != null && array.Length > 0)
					{
						ObjectSelector.s_SharedObjectSelector = (ObjectSelector)array[0];
					}
					if (ObjectSelector.s_SharedObjectSelector == null)
					{
						ObjectSelector.s_SharedObjectSelector = ScriptableObject.CreateInstance<ObjectSelector>();
					}
				}
				return ObjectSelector.s_SharedObjectSelector;
			}
		}

		public static bool isVisible
		{
			get
			{
				return ObjectSelector.s_SharedObjectSelector != null;
			}
		}

		internal string searchFilter
		{
			get
			{
				return this.m_SearchFilter;
			}
			set
			{
				this.m_SearchFilter = value;
				this.FilterSettingsChanged();
			}
		}

		private bool IsUsingTreeView()
		{
			return this.m_ObjectTreeWithSearch.IsInitialized();
		}

		private int GetSelectedInstanceID()
		{
			int[] array = (!this.IsUsingTreeView()) ? this.m_ListArea.GetSelection() : this.m_ObjectTreeWithSearch.GetSelection();
			if (array.Length >= 1)
			{
				return array[0];
			}
			return 0;
		}

		private void OnEnable()
		{
			base.hideFlags = HideFlags.DontSave;
			this.m_ShowOverlapPreview.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowOverlapPreview.speed = 1.5f;
			this.m_ShowWidePreview.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowWidePreview.speed = 1.5f;
			this.m_PreviewResizer.Init("ObjectPickerPreview");
			this.m_PreviewSize = this.m_PreviewResizer.GetPreviewSize();
			AssetPreview.ClearTemporaryAssetPreviews();
			this.SetupPreview();
		}

		private void OnDisable()
		{
			this.SendEvent("ObjectSelectorClosed", false);
			if (this.m_ListArea != null)
			{
				this.m_StartGridSize.value = this.m_ListArea.gridSize;
			}
			Undo.CollapseUndoOperations(this.m_ModalUndoGroup);
			if (ObjectSelector.s_SharedObjectSelector == this)
			{
				ObjectSelector.s_SharedObjectSelector = null;
			}
			if (this.m_EditorCache != null)
			{
				this.m_EditorCache.Dispose();
			}
			AssetPreview.ClearTemporaryAssetPreviews();
		}

		public void SetupPreview()
		{
			bool flag = this.PreviewIsOpen();
			bool flag2 = this.PreviewIsWide();
			BaseAnimValue<bool> arg_2F_0 = this.m_ShowOverlapPreview;
			bool flag3 = flag && !flag2;
			this.m_ShowOverlapPreview.value = flag3;
			arg_2F_0.target = flag3;
			BaseAnimValue<bool> arg_52_0 = this.m_ShowWidePreview;
			flag3 = (flag && flag2);
			this.m_ShowWidePreview.value = flag3;
			arg_52_0.target = flag3;
		}

		private void ListAreaItemSelectedCallback(bool doubleClicked)
		{
			if (doubleClicked)
			{
				this.ItemWasDoubleClicked();
			}
			else
			{
				this.m_FocusSearchFilter = false;
				this.SendEvent("ObjectSelectorUpdated", true);
			}
		}

		private static bool GuessIfUserIsLookingForAnAsset(string requiredClassName, bool checkGameObject)
		{
			string[] array = new string[]
			{
				"AnimationClip",
				"AnimatorController",
				"AnimatorOverrideController",
				"AudioClip",
				"Avatar",
				"Flare",
				"Font",
				"Material",
				"ProceduralMaterial",
				"Mesh",
				"PhysicMaterial",
				"GUISkin",
				"Shader",
				"TerrainData",
				"Texture",
				"Cubemap",
				"MovieTexture",
				"RenderTexture",
				"Texture2D",
				"ProceduralTexture",
				"Sprite",
				"AudioMixerGroup",
				"AudioMixerSnapshot"
			};
			if (checkGameObject && requiredClassName == "GameObject")
			{
				return true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == requiredClassName)
				{
					return true;
				}
			}
			return false;
		}

		private Scene GetSceneFromObject(UnityEngine.Object obj)
		{
			GameObject gameObject = obj as GameObject;
			if (gameObject != null)
			{
				return gameObject.scene;
			}
			Component component = obj as Component;
			if (component != null)
			{
				return component.gameObject.scene;
			}
			return default(Scene);
		}

		private void FilterSettingsChanged()
		{
			SearchFilter searchFilter = new SearchFilter();
			searchFilter.SearchFieldStringToFilter(this.m_SearchFilter);
			if (!string.IsNullOrEmpty(this.m_RequiredType))
			{
				searchFilter.classNames = new string[]
				{
					this.m_RequiredType
				};
			}
			HierarchyType hierarchyType = (!this.m_IsShowingAssets) ? HierarchyType.GameObjects : HierarchyType.Assets;
			if (EditorSceneManager.preventCrossSceneReferences && hierarchyType == HierarchyType.GameObjects && this.m_ObjectBeingEdited != null)
			{
				Scene sceneFromObject = this.GetSceneFromObject(this.m_ObjectBeingEdited);
				if (sceneFromObject.IsValid())
				{
					searchFilter.scenePaths = new string[]
					{
						sceneFromObject.path
					};
				}
			}
			this.m_ListArea.Init(this.listPosition, hierarchyType, searchFilter, true);
		}

		private static bool ShouldTreeViewBeUsed(string className)
		{
			return className == "AudioMixerGroup";
		}

		public void Show(UnityEngine.Object obj, Type requiredType, SerializedProperty property, bool allowSceneObjects)
		{
			this.Show(obj, requiredType, property, allowSceneObjects, null);
		}

		internal void Show(UnityEngine.Object obj, Type requiredType, SerializedProperty property, bool allowSceneObjects, List<int> allowedInstanceIDs)
		{
			this.m_AllowSceneObjects = allowSceneObjects;
			this.m_IsShowingAssets = true;
			this.m_AllowedIDs = allowedInstanceIDs;
			string text = string.Empty;
			if (property != null)
			{
				text = property.objectReferenceTypeString;
				obj = property.objectReferenceValue;
				this.m_ObjectBeingEdited = property.serializedObject.targetObject;
				if (this.m_ObjectBeingEdited != null && EditorUtility.IsPersistent(this.m_ObjectBeingEdited))
				{
					this.m_AllowSceneObjects = false;
				}
			}
			else if (requiredType != null)
			{
				text = requiredType.Name;
			}
			if (this.m_AllowSceneObjects)
			{
				if (obj != null)
				{
					if (typeof(Component).IsAssignableFrom(obj.GetType()))
					{
						obj = ((Component)obj).gameObject;
					}
					this.m_IsShowingAssets = (EditorUtility.IsPersistent(obj) || ObjectSelector.GuessIfUserIsLookingForAnAsset(text, false));
				}
				else
				{
					this.m_IsShowingAssets = ObjectSelector.GuessIfUserIsLookingForAnAsset(text, true);
				}
			}
			else
			{
				this.m_IsShowingAssets = true;
			}
			this.m_DelegateView = GUIView.current;
			this.m_RequiredType = text;
			this.m_SearchFilter = string.Empty;
			this.m_OriginalSelection = obj;
			this.m_ModalUndoGroup = Undo.GetCurrentGroup();
			ContainerWindow.SetFreezeDisplay(true);
			base.ShowWithMode(ShowMode.AuxWindow);
			base.titleContent = new GUIContent("Select " + text);
			Rect position = this.m_Parent.window.position;
			position.width = EditorPrefs.GetFloat("ObjectSelectorWidth", 200f);
			position.height = EditorPrefs.GetFloat("ObjectSelectorHeight", 390f);
			base.position = position;
			base.minSize = new Vector2(200f, 335f);
			base.maxSize = new Vector2(10000f, 10000f);
			this.SetupPreview();
			base.Focus();
			ContainerWindow.SetFreezeDisplay(false);
			this.m_FocusSearchFilter = true;
			this.m_Parent.AddToAuxWindowList();
			int num = (!(obj != null)) ? 0 : obj.GetInstanceID();
			if (property != null && property.hasMultipleDifferentValues)
			{
				num = 0;
			}
			if (ObjectSelector.ShouldTreeViewBeUsed(text))
			{
				this.m_ObjectTreeWithSearch.Init(base.position, this, new UnityAction<ObjectTreeForSelector.TreeSelectorData>(this.CreateAndSetTreeView), new UnityAction<TreeViewItem>(this.TreeViewSelection), new UnityAction(this.ItemWasDoubleClicked), num, 0);
			}
			else
			{
				this.InitIfNeeded();
				this.m_ListArea.InitSelection(new int[]
				{
					num
				});
				if (num != 0)
				{
					this.m_ListArea.Frame(num, true, false);
				}
			}
		}

		private void ItemWasDoubleClicked()
		{
			base.Close();
			GUIUtility.ExitGUI();
		}

		private void CreateAndSetTreeView(ObjectTreeForSelector.TreeSelectorData data)
		{
			TreeViewForAudioMixerGroup.CreateAndSetTreeView(data);
		}

		private void TreeViewSelection(TreeViewItem item)
		{
			this.SendEvent("ObjectSelectorUpdated", true);
		}

		private void InitIfNeeded()
		{
			if (this.m_ListAreaState == null)
			{
				this.m_ListAreaState = new ObjectListAreaState();
			}
			if (this.m_ListArea == null)
			{
				this.m_ListArea = new ObjectListArea(this.m_ListAreaState, this, true);
				this.m_ListArea.allowDeselection = false;
				this.m_ListArea.allowDragging = false;
				this.m_ListArea.allowFocusRendering = false;
				this.m_ListArea.allowMultiSelect = false;
				this.m_ListArea.allowRenaming = false;
				this.m_ListArea.allowBuiltinResources = true;
				ObjectListArea expr_82 = this.m_ListArea;
				expr_82.repaintCallback = (Action)Delegate.Combine(expr_82.repaintCallback, new Action(base.Repaint));
				ObjectListArea expr_A9 = this.m_ListArea;
				expr_A9.itemSelectedCallback = (Action<bool>)Delegate.Combine(expr_A9.itemSelectedCallback, new Action<bool>(this.ListAreaItemSelectedCallback));
				this.m_ListArea.gridSize = this.m_StartGridSize.value;
				this.FilterSettingsChanged();
			}
		}

		public static UnityEngine.Object GetCurrentObject()
		{
			return EditorUtility.InstanceIDToObject(ObjectSelector.get.GetSelectedInstanceID());
		}

		public static UnityEngine.Object GetInitialObject()
		{
			return ObjectSelector.get.m_OriginalSelection;
		}

		private void SearchArea()
		{
			GUI.Label(new Rect(0f, 0f, base.position.width, this.m_ToolbarHeight), GUIContent.none, this.m_Styles.toolbarBack);
			bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			GUI.SetNextControlName("SearchFilter");
			string text = EditorGUI.SearchField(new Rect(5f, 5f, base.position.width - 10f, 15f), this.m_SearchFilter);
			if (flag && Event.current.type == EventType.Used)
			{
				if (this.m_SearchFilter == string.Empty)
				{
					this.Cancel();
				}
				this.m_FocusSearchFilter = true;
			}
			if (text != this.m_SearchFilter || this.m_FocusSearchFilter)
			{
				this.m_SearchFilter = text;
				this.FilterSettingsChanged();
				base.Repaint();
			}
			if (this.m_FocusSearchFilter)
			{
				EditorGUI.FocusTextInControl("SearchFilter");
				this.m_FocusSearchFilter = false;
			}
			GUI.changed = false;
			GUILayout.BeginArea(new Rect(0f, 26f, base.position.width, this.m_ToolbarHeight - 26f));
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool flag2 = GUILayout.Toggle(this.m_IsShowingAssets, "Assets", this.m_Styles.tab, new GUILayoutOption[0]);
			if (!this.m_IsShowingAssets && flag2)
			{
				this.m_IsShowingAssets = true;
			}
			if (!this.m_AllowSceneObjects)
			{
				GUI.enabled = false;
				GUI.color = new Color(1f, 1f, 1f, 0f);
			}
			bool flag3 = !this.m_IsShowingAssets;
			flag3 = GUILayout.Toggle(flag3, "Scene", this.m_Styles.tab, new GUILayoutOption[0]);
			if (this.m_IsShowingAssets && flag3)
			{
				this.m_IsShowingAssets = false;
			}
			if (!this.m_AllowSceneObjects)
			{
				GUI.color = new Color(1f, 1f, 1f, 1f);
				GUI.enabled = true;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			if (GUI.changed)
			{
				this.FilterSettingsChanged();
			}
			if (this.m_ListArea.CanShowThumbnails())
			{
				this.m_ListArea.gridSize = (int)GUI.HorizontalSlider(new Rect(base.position.width - 60f, 26f, 55f, this.m_ToolbarHeight - 28f), (float)this.m_ListArea.gridSize, (float)this.m_ListArea.minGridSize, (float)this.m_ListArea.maxGridSize);
			}
		}

		private void OnInspectorUpdate()
		{
			if (this.m_ListArea != null && AssetPreview.HasAnyNewPreviewTexturesAvailable(this.m_ListArea.GetAssetPreviewManagerID()))
			{
				base.Repaint();
			}
		}

		private void PreviewArea()
		{
			GUI.Box(new Rect(0f, this.m_TopSize, base.position.width, this.m_PreviewSize), string.Empty, this.m_Styles.previewBackground);
			if (this.m_ListArea.GetSelection().Length == 0)
			{
				return;
			}
			EditorWrapper editorWrapper = null;
			UnityEngine.Object currentObject = ObjectSelector.GetCurrentObject();
			if (this.m_PreviewSize < 75f)
			{
				string text;
				if (currentObject != null)
				{
					editorWrapper = this.m_EditorCache[currentObject];
					string str = ObjectNames.NicifyVariableName(currentObject.GetType().Name);
					if (editorWrapper != null)
					{
						text = editorWrapper.name + " (" + str + ")";
					}
					else
					{
						text = currentObject.name + " (" + str + ")";
					}
					text = text + "      " + AssetDatabase.GetAssetPath(currentObject);
				}
				else
				{
					text = "None";
				}
				this.LinePreview(text, currentObject, editorWrapper);
			}
			else
			{
				if (this.m_EditorCache == null)
				{
					this.m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);
				}
				string text3;
				if (currentObject != null)
				{
					editorWrapper = this.m_EditorCache[currentObject];
					string text2 = ObjectNames.NicifyVariableName(currentObject.GetType().Name);
					if (editorWrapper != null)
					{
						text3 = editorWrapper.GetInfoString();
						if (text3 != string.Empty)
						{
							text3 = string.Concat(new string[]
							{
								editorWrapper.name,
								"\n",
								text2,
								"\n",
								text3
							});
						}
						else
						{
							text3 = editorWrapper.name + "\n" + text2;
						}
					}
					else
					{
						text3 = currentObject.name + "\n" + text2;
					}
					text3 = text3 + "\n" + AssetDatabase.GetAssetPath(currentObject);
				}
				else
				{
					text3 = "None";
				}
				if (this.m_ShowWidePreview.faded != 0f)
				{
					GUI.color = new Color(1f, 1f, 1f, this.m_ShowWidePreview.faded);
					this.WidePreview(this.m_PreviewSize, text3, currentObject, editorWrapper);
				}
				if (this.m_ShowOverlapPreview.faded != 0f)
				{
					GUI.color = new Color(1f, 1f, 1f, this.m_ShowOverlapPreview.faded);
					this.OverlapPreview(this.m_PreviewSize, text3, currentObject, editorWrapper);
				}
				GUI.color = Color.white;
				this.m_EditorCache.CleanupUntouchedEditors();
			}
		}

		private void WidePreview(float actualSize, string s, UnityEngine.Object o, EditorWrapper p)
		{
			float num = 5f;
			Rect position = new Rect(num, this.m_TopSize + num, actualSize - num * 2f, actualSize - num * 2f);
			Rect position2 = new Rect(this.m_PreviewSize + 3f, this.m_TopSize + (this.m_PreviewSize - 75f) * 0.5f, this.m_Parent.window.position.width - this.m_PreviewSize - 3f - num, 75f);
			if (p != null && p.HasPreviewGUI())
			{
				p.OnPreviewGUI(position, this.m_Styles.previewTextureBackground);
			}
			else if (o != null)
			{
				this.DrawObjectIcon(position, this.m_ListArea.m_SelectedObjectIcon);
			}
			if (EditorGUIUtility.isProSkin)
			{
				EditorGUI.DropShadowLabel(position2, s, this.m_Styles.smallStatus);
			}
			else
			{
				GUI.Label(position2, s, this.m_Styles.smallStatus);
			}
		}

		private void OverlapPreview(float actualSize, string s, UnityEngine.Object o, EditorWrapper p)
		{
			float num = 5f;
			Rect position = new Rect(num, this.m_TopSize + num, base.position.width - num * 2f, actualSize - num * 2f);
			if (p != null && p.HasPreviewGUI())
			{
				p.OnPreviewGUI(position, this.m_Styles.previewTextureBackground);
			}
			else if (o != null)
			{
				this.DrawObjectIcon(position, this.m_ListArea.m_SelectedObjectIcon);
			}
			if (EditorGUIUtility.isProSkin)
			{
				EditorGUI.DropShadowLabel(position, s, this.m_Styles.largeStatus);
			}
			else
			{
				EditorGUI.DoDropShadowLabel(position, EditorGUIUtility.TempContent(s), this.m_Styles.largeStatus, 0.3f);
			}
		}

		private void LinePreview(string s, UnityEngine.Object o, EditorWrapper p)
		{
			if (this.m_ListArea.m_SelectedObjectIcon != null)
			{
				GUI.DrawTexture(new Rect(2f, (float)((int)(this.m_TopSize + 2f)), 16f, 16f), this.m_ListArea.m_SelectedObjectIcon, ScaleMode.StretchToFill);
			}
			Rect position = new Rect(20f, this.m_TopSize + 1f, base.position.width - 22f, 18f);
			if (EditorGUIUtility.isProSkin)
			{
				EditorGUI.DropShadowLabel(position, s, this.m_Styles.smallStatus);
			}
			else
			{
				GUI.Label(position, s, this.m_Styles.smallStatus);
			}
		}

		private void DrawObjectIcon(Rect position, Texture icon)
		{
			if (icon == null)
			{
				return;
			}
			int num = Mathf.Min((int)position.width, (int)position.height);
			if (num >= icon.width * 2)
			{
				num = icon.width * 2;
			}
			FilterMode filterMode = icon.filterMode;
			icon.filterMode = FilterMode.Point;
			GUI.DrawTexture(new Rect(position.x + (float)(((int)position.width - num) / 2), position.y + (float)(((int)position.height - num) / 2), (float)num, (float)num), icon, ScaleMode.ScaleToFit);
			icon.filterMode = filterMode;
		}

		private void ResizeBottomPartOfWindow()
		{
			GUI.changed = false;
			this.m_PreviewSize = this.m_PreviewResizer.ResizeHandle(base.position, 65f, 270f, 20f) + 20f;
			this.m_TopSize = base.position.height - this.m_PreviewSize;
			bool flag = this.PreviewIsOpen();
			bool flag2 = this.PreviewIsWide();
			this.m_ShowOverlapPreview.target = (flag && !flag2);
			this.m_ShowWidePreview.target = (flag && flag2);
		}

		private bool PreviewIsOpen()
		{
			return this.m_PreviewSize >= 37f;
		}

		private bool PreviewIsWide()
		{
			return base.position.width - this.m_PreviewSize - 5f > Mathf.Min(this.m_PreviewSize * 2f - 20f, 256f);
		}

		private void SendEvent(string eventName, bool exitGUI)
		{
			if (this.m_DelegateView)
			{
				Event e = EditorGUIUtility.CommandEvent(eventName);
				try
				{
					this.m_DelegateView.SendEvent(e);
				}
				finally
				{
				}
				if (exitGUI)
				{
					GUIUtility.ExitGUI();
				}
			}
		}

		private void HandleKeyboard()
		{
			if (Event.current.type != EventType.KeyDown)
			{
				return;
			}
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode != KeyCode.Return && keyCode != KeyCode.KeypadEnter)
			{
				return;
			}
			base.Close();
			GUI.changed = true;
			GUIUtility.ExitGUI();
			Event.current.Use();
			GUI.changed = true;
		}

		private void Cancel()
		{
			Undo.RevertAllDownToGroup(this.m_ModalUndoGroup);
			base.Close();
			GUI.changed = true;
			GUIUtility.ExitGUI();
		}

		private void OnDestroy()
		{
			if (this.m_ListArea != null)
			{
				this.m_ListArea.OnDestroy();
			}
			this.m_ObjectTreeWithSearch.Clear();
		}

		private void OnGUI()
		{
			this.HandleKeyboard();
			if (this.m_ObjectTreeWithSearch.IsInitialized())
			{
				this.OnObjectTreeGUI();
			}
			else
			{
				this.OnObjectGridGUI();
			}
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				this.Cancel();
			}
		}

		private void OnObjectTreeGUI()
		{
			this.m_ObjectTreeWithSearch.OnGUI(new Rect(0f, 0f, base.position.width, base.position.height));
		}

		private void OnObjectGridGUI()
		{
			this.InitIfNeeded();
			if (this.m_Styles == null)
			{
				this.m_Styles = new ObjectSelector.Styles();
			}
			if (this.m_EditorCache == null)
			{
				this.m_EditorCache = new EditorCache(EditorFeatures.PreviewGUI);
			}
			this.ResizeBottomPartOfWindow();
			Rect position = base.position;
			EditorPrefs.SetFloat("ObjectSelectorWidth", position.width);
			EditorPrefs.SetFloat("ObjectSelectorHeight", position.height);
			GUI.BeginGroup(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none);
			this.m_ListArea.HandleKeyboard(false);
			this.SearchArea();
			this.GridListArea();
			this.PreviewArea();
			GUI.EndGroup();
			GUI.Label(new Rect(base.position.width * 0.5f - 16f, base.position.height - this.m_PreviewSize + 2f, 32f, this.m_Styles.bottomResize.fixedHeight), GUIContent.none, this.m_Styles.bottomResize);
		}

		private void GridListArea()
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.m_ListArea.OnGUI(this.listPosition, controlID);
		}
	}
}
