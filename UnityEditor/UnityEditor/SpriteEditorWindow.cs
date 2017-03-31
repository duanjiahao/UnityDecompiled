using System;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEditor.U2D;
using UnityEditor.U2D.Interface;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Interface;

namespace UnityEditor
{
	internal class SpriteEditorWindow : SpriteUtilityWindow, ISpriteEditor
	{
		private class SpriteEditorWindowStyles
		{
			public static readonly GUIContent editingDisableMessageLabel = EditorGUIUtility.TextContent("Editing is disabled during play mode");

			public static readonly GUIContent revertButtonLabel = EditorGUIUtility.TextContent("Revert");

			public static readonly GUIContent applyButtonLabel = EditorGUIUtility.TextContent("Apply");

			public static readonly GUIContent spriteEditorWindowTitle = EditorGUIUtility.TextContent("Sprite Editor");

			public static readonly GUIContent pendingChangesDialogContent = EditorGUIUtility.TextContent("You have pending changes in the Sprite Editor Window.\nDo you want to apply these changes?");

			public static readonly GUIContent yesButtonLabel = EditorGUIUtility.TextContent("Yes");

			public static readonly GUIContent noButtonLabel = EditorGUIUtility.TextContent("No");

			public static readonly GUIContent applyRevertDialogTitle = EditorGUIUtility.TextContent("Unapplied import settings");

			public static readonly GUIContent applyRevertDialogContent = EditorGUIUtility.TextContent("Unapplied import settings for '{0}'");

			public static readonly GUIContent noSelectionWarning = EditorGUIUtility.TextContent("No texture or sprite selected");

			public static readonly GUIContent applyRevertModuleDialogTitle = EditorGUIUtility.TextContent("Unapplied module changes");

			public static readonly GUIContent applyRevertModuleDialogContent = EditorGUIUtility.TextContent("You have unapplied changes from the current module");

			public static readonly GUIContent saveProgressTitle = EditorGUIUtility.TextContent("Saving");

			public static readonly GUIContent saveContentText = EditorGUIUtility.TextContent("Saving Sprites {0}/{1}");

			public static readonly GUIContent loadProgressTitle = EditorGUIUtility.TextContent("Loading");

			public static readonly GUIContent loadContentText = EditorGUIUtility.TextContent("Loading Sprites {0}/{1}");
		}

		internal class PreviewTexture2D : UnityEngine.U2D.Interface.Texture2D
		{
			private int m_ActualWidth = 0;

			private int m_ActualHeight = 0;

			public override int width
			{
				get
				{
					return this.m_ActualWidth;
				}
			}

			public override int height
			{
				get
				{
					return this.m_ActualHeight;
				}
			}

			public PreviewTexture2D(UnityEngine.Texture2D t) : base(t)
			{
				if (t != null)
				{
					(AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(t)) as TextureImporter).GetWidthAndHeight(ref this.m_ActualWidth, ref this.m_ActualHeight);
				}
			}
		}

		private SpriteEditorWindow.SpriteEditorWindowStyles m_SpriteEditorWindowStyles;

		private const float k_MarginForFraming = 0.05f;

		private const float k_WarningMessageWidth = 250f;

		private const float k_WarningMessageHeight = 40f;

		private const float k_ModuleListWidth = 90f;

		public static SpriteEditorWindow s_Instance;

		public bool m_ResetOnNextRepaint;

		public bool m_IgnoreNextPostprocessEvent;

		public ITexture2D m_OriginalTexture;

		private SpriteRectCache m_RectsCache;

		private SerializedObject m_TextureImporterSO;

		private TextureImporter m_TextureImporter;

		private SerializedProperty m_TextureImporterSprites;

		private bool m_RequestRepaint = false;

		public static bool s_OneClickDragStarted = false;

		public string m_SelectedAssetPath;

		private IEventSystem m_EventSystem;

		private IUndoSystem m_UndoSystem;

		private IAssetDatabase m_AssetDatabase;

		private IGUIUtility m_GUIUtility;

		private UnityEngine.Texture2D m_OutlineTexture;

		private UnityEngine.Texture2D m_ReadableTexture;

		[SerializeField]
		private SpriteRect m_Selected;

		private GUIContent[] m_RegisteredModuleNames;

		private List<ISpriteEditorModule> m_AllRegisteredModules;

		private List<ISpriteEditorModule> m_RegisteredModules;

		private ISpriteEditorModule m_CurrentModule = null;

		private int m_CurrentModuleIndex = 0;

		private Rect warningMessageRect
		{
			get
			{
				return new Rect(base.position.width - 250f - 8f - 16f, 24f, 250f, 40f);
			}
		}

		private bool multipleSprites
		{
			get
			{
				return this.m_TextureImporter != null && this.m_TextureImporter.spriteImportMode == SpriteImportMode.Multiple;
			}
		}

		private bool validSprite
		{
			get
			{
				return this.m_TextureImporter != null && this.m_TextureImporter.spriteImportMode != SpriteImportMode.None;
			}
		}

		private bool activeTextureSelected
		{
			get
			{
				return this.m_TextureImporter != null && this.m_Texture != null && this.m_OriginalTexture != null;
			}
		}

		public bool textureIsDirty
		{
			get;
			set;
		}

		public bool selectedTextureChanged
		{
			get
			{
				ITexture2D selectedTexture2D = this.GetSelectedTexture2D();
				return selectedTexture2D != null && this.m_OriginalTexture != selectedTexture2D;
			}
		}

		public ISpriteRectCache spriteRects
		{
			get
			{
				return this.m_RectsCache;
			}
		}

		public SpriteRect selectedSpriteRect
		{
			get
			{
				SpriteRect result;
				if (this.editingDisabled)
				{
					result = null;
				}
				else
				{
					result = this.m_Selected;
				}
				return result;
			}
			set
			{
				this.m_Selected = value;
			}
		}

		public bool enableMouseMoveEvent
		{
			set
			{
				base.wantsMouseMove = value;
			}
		}

		public Rect windowDimension
		{
			get
			{
				return base.position;
			}
		}

		public ITexture2D selectedTexture
		{
			get
			{
				return this.m_OriginalTexture;
			}
		}

		public ITexture2D previewTexture
		{
			get
			{
				return this.m_Texture;
			}
		}

		public bool editingDisabled
		{
			get
			{
				return EditorApplication.isPlayingOrWillChangePlaymode;
			}
		}

		public static void GetWindow()
		{
			EditorWindow.GetWindow<SpriteEditorWindow>();
		}

		private void ModifierKeysChanged()
		{
			if (EditorWindow.focusedWindow == this)
			{
				base.Repaint();
			}
		}

		private void OnFocus()
		{
			if (this.selectedTextureChanged)
			{
				this.OnSelectionChange();
			}
		}

		public static void TextureImporterApply(SerializedObject so)
		{
			if (!(SpriteEditorWindow.s_Instance == null))
			{
				SpriteEditorWindow.s_Instance.ApplyCacheSettingsToInspector(so);
			}
		}

		private void ApplyCacheSettingsToInspector(SerializedObject so)
		{
			if (this.m_TextureImporterSO != null && this.m_TextureImporterSO.targetObject == so.targetObject)
			{
				if (so.FindProperty("m_SpriteMode").intValue == this.m_TextureImporterSO.FindProperty("m_SpriteMode").intValue)
				{
					SpriteEditorWindow.s_Instance.m_IgnoreNextPostprocessEvent = true;
				}
				else if (this.textureIsDirty)
				{
					bool flag = EditorUtility.DisplayDialog(SpriteEditorWindow.SpriteEditorWindowStyles.spriteEditorWindowTitle.text, SpriteEditorWindow.SpriteEditorWindowStyles.pendingChangesDialogContent.text, SpriteEditorWindow.SpriteEditorWindowStyles.yesButtonLabel.text, SpriteEditorWindow.SpriteEditorWindowStyles.noButtonLabel.text);
					if (flag)
					{
						this.DoApply(so);
					}
				}
			}
		}

		public void RefreshPropertiesCache()
		{
			this.m_OriginalTexture = this.GetSelectedTexture2D();
			if (!(this.m_OriginalTexture == null))
			{
				this.m_TextureImporter = (AssetImporter.GetAtPath(this.m_SelectedAssetPath) as TextureImporter);
				if (!(this.m_TextureImporter == null))
				{
					this.m_TextureImporterSO = new SerializedObject(this.m_TextureImporter);
					this.m_TextureImporterSprites = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Sprites");
					this.m_Texture = ((!(this.m_OriginalTexture == null)) ? new SpriteEditorWindow.PreviewTexture2D(this.m_OriginalTexture) : null);
				}
			}
		}

		public void InvalidatePropertiesCache()
		{
			if (this.m_RectsCache)
			{
				this.m_RectsCache.ClearAll();
				UnityEngine.Object.DestroyImmediate(this.m_RectsCache);
			}
			if (this.m_ReadableTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ReadableTexture);
				this.m_ReadableTexture = null;
			}
			this.m_OriginalTexture = null;
			this.m_TextureImporter = null;
			this.m_TextureImporterSO = null;
			this.m_TextureImporterSprites = null;
		}

		public bool IsEditingDisabled()
		{
			return EditorApplication.isPlayingOrWillChangePlaymode;
		}

		private void OnSelectionChange()
		{
			if (this.GetSelectedTexture2D() == null || this.selectedTextureChanged)
			{
				this.HandleApplyRevertDialog();
				this.ResetWindow();
				this.RefreshPropertiesCache();
				this.RefreshRects();
			}
			if (this.m_RectsCache != null)
			{
				if (Selection.activeObject is Sprite)
				{
					this.UpdateSelectedSpriteRect(Selection.activeObject as Sprite);
				}
				else if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SpriteRenderer>())
				{
					Sprite sprite = Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite;
					this.UpdateSelectedSpriteRect(sprite);
				}
			}
			this.UpdateAvailableModules();
			base.Repaint();
		}

		public void ResetWindow()
		{
			this.InvalidatePropertiesCache();
			this.selectedSpriteRect = null;
			this.textureIsDirty = false;
			this.m_Zoom = -1f;
		}

		private void OnEnable()
		{
			this.m_EventSystem = new EventSystem();
			this.m_UndoSystem = new UndoSystem();
			this.m_AssetDatabase = new AssetDatabaseSystem();
			this.m_GUIUtility = new GUIUtilitySystem();
			base.minSize = new Vector2(360f, 200f);
			base.titleContent = SpriteEditorWindow.SpriteEditorWindowStyles.spriteEditorWindowTitle;
			SpriteEditorWindow.s_Instance = this;
			this.m_UndoSystem.RegisterUndoCallback(new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
			this.ResetWindow();
			this.RefreshPropertiesCache();
			this.RefreshRects();
			this.InitModules();
		}

		private void UndoRedoPerformed()
		{
			ITexture2D selectedTexture2D = this.GetSelectedTexture2D();
			if (selectedTexture2D != null && this.m_OriginalTexture != selectedTexture2D)
			{
				this.OnSelectionChange();
			}
			if (this.m_RectsCache != null && !this.m_RectsCache.Contains(this.selectedSpriteRect))
			{
				this.selectedSpriteRect = null;
			}
			base.Repaint();
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			if (this.m_RectsCache != null)
			{
				Undo.ClearUndo(this.m_RectsCache);
			}
			this.HandleApplyRevertDialog();
			this.InvalidatePropertiesCache();
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
			SpriteEditorWindow.s_Instance = null;
			if (this.m_OutlineTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_OutlineTexture);
				this.m_OutlineTexture = null;
			}
			if (this.m_ReadableTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ReadableTexture);
				this.m_ReadableTexture = null;
			}
		}

		private void HandleApplyRevertDialog()
		{
			if (this.textureIsDirty && this.m_TextureImporter != null)
			{
				if (EditorUtility.DisplayDialog(SpriteEditorWindow.SpriteEditorWindowStyles.applyRevertDialogTitle.text, string.Format(SpriteEditorWindow.SpriteEditorWindowStyles.applyRevertDialogContent.text, this.m_TextureImporter.assetPath), SpriteEditorWindow.SpriteEditorWindowStyles.applyButtonLabel.text, SpriteEditorWindow.SpriteEditorWindowStyles.revertButtonLabel.text))
				{
					this.DoApply();
				}
				else
				{
					this.DoRevert();
				}
				this.SetupModule(this.m_CurrentModuleIndex);
			}
		}

		private void RefreshRects()
		{
			if (this.m_TextureImporterSprites != null)
			{
				if (this.m_RectsCache)
				{
					this.m_RectsCache.ClearAll();
					Undo.ClearUndo(this.m_RectsCache);
					UnityEngine.Object.DestroyImmediate(this.m_RectsCache);
				}
				this.m_RectsCache = ScriptableObject.CreateInstance<SpriteRectCache>();
				if (this.multipleSprites)
				{
					for (int i = 0; i < this.m_TextureImporterSprites.arraySize; i++)
					{
						SpriteRect spriteRect = new SpriteRect();
						spriteRect.LoadFromSerializedProperty(this.m_TextureImporterSprites.GetArrayElementAtIndex(i));
						this.m_RectsCache.AddRect(spriteRect);
						EditorUtility.DisplayProgressBar(SpriteEditorWindow.SpriteEditorWindowStyles.loadProgressTitle.text, string.Format(SpriteEditorWindow.SpriteEditorWindowStyles.loadContentText.text, i, this.m_TextureImporterSprites.arraySize), (float)i / (float)this.m_TextureImporterSprites.arraySize);
					}
				}
				else if (this.validSprite)
				{
					SpriteRect spriteRect2 = new SpriteRect();
					spriteRect2.rect = new Rect(0f, 0f, (float)this.m_Texture.width, (float)this.m_Texture.height);
					spriteRect2.name = this.m_OriginalTexture.name;
					spriteRect2.alignment = (SpriteAlignment)this.m_TextureImporterSO.FindProperty("m_Alignment").intValue;
					spriteRect2.border = this.m_TextureImporter.spriteBorder;
					spriteRect2.pivot = SpriteEditorUtility.GetPivotValue(spriteRect2.alignment, this.m_TextureImporter.spritePivot);
					spriteRect2.tessellationDetail = this.m_TextureImporterSO.FindProperty("m_SpriteTessellationDetail").floatValue;
					SerializedProperty outlineSP = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Outline");
					spriteRect2.outline = SpriteRect.AcquireOutline(outlineSP);
					this.m_RectsCache.AddRect(spriteRect2);
				}
				EditorUtility.ClearProgressBar();
				if (this.m_RectsCache.Count > 0)
				{
					this.selectedSpriteRect = this.m_RectsCache.RectAt(0);
				}
			}
		}

		private void OnGUI()
		{
			base.InitStyles();
			if (this.m_ResetOnNextRepaint || this.selectedTextureChanged)
			{
				this.ResetWindow();
				this.RefreshPropertiesCache();
				this.RefreshRects();
				this.UpdateAvailableModules();
				this.SetupModule(this.m_CurrentModuleIndex);
				this.m_ResetOnNextRepaint = false;
			}
			Matrix4x4 matrix = Handles.matrix;
			if (!this.activeTextureSelected)
			{
				using (new EditorGUI.DisabledScope(true))
				{
					GUILayout.Label(SpriteEditorWindow.SpriteEditorWindowStyles.noSelectionWarning, new GUILayoutOption[0]);
				}
			}
			else
			{
				this.DoToolbarGUI();
				base.DoTextureGUI();
				this.DoEditingDisabledMessage();
				this.m_CurrentModule.OnPostGUI();
				Handles.matrix = matrix;
				if (this.m_RequestRepaint)
				{
					base.Repaint();
				}
			}
		}

		protected override void DoTextureGUIExtras()
		{
			this.HandleFrameSelected();
			if (this.m_EventSystem.current.type == EventType.Repaint)
			{
				SpriteEditorUtility.BeginLines(new Color(1f, 1f, 1f, 0.5f));
				for (int i = 0; i < this.m_RectsCache.Count; i++)
				{
					if (this.m_RectsCache.RectAt(i) != this.selectedSpriteRect)
					{
						SpriteEditorUtility.DrawBox(this.m_RectsCache.RectAt(i).rect);
					}
				}
				SpriteEditorUtility.EndLines();
			}
			this.m_CurrentModule.DoTextureGUI();
		}

		private void DoToolbarGUI()
		{
			GUIStyle toolbar = EditorStyles.toolbar;
			Rect rect = new Rect(0f, 0f, base.position.width, 17f);
			if (this.m_EventSystem.current.type == EventType.Repaint)
			{
				toolbar.Draw(rect, false, false, false, false);
			}
			this.m_TextureViewRect = new Rect(0f, 17f, base.position.width - 16f, base.position.height - 16f - 17f);
			if (this.m_RegisteredModules.Count > 1)
			{
				int num = EditorGUI.Popup(new Rect(0f, 0f, 90f, 17f), this.m_CurrentModuleIndex, this.m_RegisteredModuleNames, EditorStyles.toolbarPopup);
				if (num != this.m_CurrentModuleIndex)
				{
					if (this.textureIsDirty)
					{
						if (EditorUtility.DisplayDialog(SpriteEditorWindow.SpriteEditorWindowStyles.applyRevertModuleDialogTitle.text, SpriteEditorWindow.SpriteEditorWindowStyles.applyRevertModuleDialogContent.text, SpriteEditorWindow.SpriteEditorWindowStyles.applyButtonLabel.text, SpriteEditorWindow.SpriteEditorWindowStyles.revertButtonLabel.text))
						{
							this.DoApply();
						}
						else
						{
							this.DoRevert();
						}
					}
					this.SetupModule(num);
				}
				rect.x = 90f;
			}
			rect = base.DoAlphaZoomToolbarGUI(rect);
			Rect position = rect;
			position.x = position.width;
			using (new EditorGUI.DisabledScope(!this.textureIsDirty))
			{
				position.width = EditorStyles.toolbarButton.CalcSize(SpriteEditorWindow.SpriteEditorWindowStyles.applyButtonLabel).x;
				position.x -= position.width;
				if (GUI.Button(position, SpriteEditorWindow.SpriteEditorWindowStyles.applyButtonLabel, EditorStyles.toolbarButton))
				{
					this.DoApply();
					this.SetupModule(this.m_CurrentModuleIndex);
				}
				position.width = EditorStyles.toolbarButton.CalcSize(SpriteEditorWindow.SpriteEditorWindowStyles.revertButtonLabel).x;
				position.x -= position.width;
				if (GUI.Button(position, SpriteEditorWindow.SpriteEditorWindowStyles.revertButtonLabel, EditorStyles.toolbarButton))
				{
					this.DoRevert();
					this.SetupModule(this.m_CurrentModuleIndex);
				}
			}
			rect.width = position.x - rect.x;
			this.m_CurrentModule.DrawToolbarGUI(rect);
		}

		private void DoEditingDisabledMessage()
		{
			if (this.IsEditingDisabled())
			{
				GUILayout.BeginArea(this.warningMessageRect);
				EditorGUILayout.HelpBox(SpriteEditorWindow.SpriteEditorWindowStyles.editingDisableMessageLabel.text, MessageType.Warning);
				GUILayout.EndArea();
			}
		}

		private void DoApply(SerializedObject so)
		{
			if (this.multipleSprites)
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				SerializedProperty serializedProperty = so.FindProperty("m_SpriteSheet.m_Sprites");
				for (int i = 0; i < this.m_RectsCache.Count; i++)
				{
					SpriteRect spriteRect = this.m_RectsCache.RectAt(i);
					if (string.IsNullOrEmpty(spriteRect.name))
					{
						spriteRect.name = "Empty";
					}
					if (!string.IsNullOrEmpty(spriteRect.originalName))
					{
						list.Add(spriteRect.originalName);
						list2.Add(spriteRect.name);
					}
					if (serializedProperty.arraySize < this.m_RectsCache.Count)
					{
						serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
					}
					SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(i);
					spriteRect.ApplyToSerializedProperty(arrayElementAtIndex);
					EditorUtility.DisplayProgressBar(SpriteEditorWindow.SpriteEditorWindowStyles.saveProgressTitle.text, string.Format(SpriteEditorWindow.SpriteEditorWindowStyles.saveContentText.text, i, this.m_RectsCache.Count), (float)i / (float)this.m_RectsCache.Count);
				}
				while (this.m_RectsCache.Count < serializedProperty.arraySize)
				{
					serializedProperty.DeleteArrayElementAtIndex(this.m_RectsCache.Count);
				}
				if (list.Count > 0)
				{
					PatchImportSettingRecycleID.PatchMultiple(so, 213, list.ToArray(), list2.ToArray());
				}
			}
			else if (this.m_RectsCache.Count > 0)
			{
				SpriteRect spriteRect2 = this.m_RectsCache.RectAt(0);
				so.FindProperty("m_Alignment").intValue = (int)spriteRect2.alignment;
				so.FindProperty("m_SpriteBorder").vector4Value = spriteRect2.border;
				so.FindProperty("m_SpritePivot").vector2Value = spriteRect2.pivot;
				so.FindProperty("m_SpriteTessellationDetail").floatValue = spriteRect2.tessellationDetail;
				SerializedProperty serializedProperty2 = so.FindProperty("m_SpriteSheet.m_Outline");
				if (spriteRect2.outline != null)
				{
					SpriteRect.ApplyOutlineChanges(serializedProperty2, spriteRect2.outline);
				}
				else
				{
					serializedProperty2.ClearArray();
				}
			}
			EditorUtility.ClearProgressBar();
		}

		private void DoApply()
		{
			this.m_UndoSystem.ClearUndo(this.m_RectsCache);
			this.DoApply(this.m_TextureImporterSO);
			this.m_TextureImporterSO.ApplyModifiedPropertiesWithoutUndo();
			this.m_IgnoreNextPostprocessEvent = true;
			this.DoTextureReimport(this.m_TextureImporter.assetPath);
			this.textureIsDirty = false;
			this.selectedSpriteRect = null;
		}

		private void DoRevert()
		{
			this.textureIsDirty = false;
			this.selectedSpriteRect = null;
			this.RefreshRects();
			GUI.FocusControl("");
		}

		public void HandleSpriteSelection()
		{
			if (this.m_EventSystem.current.type == EventType.MouseDown && this.m_EventSystem.current.button == 0 && GUIUtility.hotControl == 0 && !this.m_EventSystem.current.alt)
			{
				SpriteRect selectedSpriteRect = this.selectedSpriteRect;
				this.selectedSpriteRect = this.TrySelect(this.m_EventSystem.current.mousePosition);
				if (this.selectedSpriteRect != null)
				{
					SpriteEditorWindow.s_OneClickDragStarted = true;
				}
				else
				{
					this.RequestRepaint();
				}
				if (selectedSpriteRect != this.selectedSpriteRect && this.selectedSpriteRect != null)
				{
					this.m_EventSystem.current.Use();
				}
			}
		}

		private void HandleFrameSelected()
		{
			IEvent current = this.m_EventSystem.current;
			if ((current.type == EventType.ValidateCommand || current.type == EventType.ExecuteCommand) && current.commandName == "FrameSelected")
			{
				if (current.type == EventType.ExecuteCommand)
				{
					if (this.selectedSpriteRect == null)
					{
						return;
					}
					Rect rect = this.selectedSpriteRect.rect;
					float zoom = this.m_Zoom;
					if (rect.width < rect.height)
					{
						zoom = this.m_TextureViewRect.height / (rect.height + this.m_TextureViewRect.height * 0.05f);
					}
					else
					{
						zoom = this.m_TextureViewRect.width / (rect.width + this.m_TextureViewRect.width * 0.05f);
					}
					this.m_Zoom = zoom;
					this.m_ScrollPosition.x = (rect.center.x - (float)this.m_Texture.width * 0.5f) * this.m_Zoom;
					this.m_ScrollPosition.y = (rect.center.y - (float)this.m_Texture.height * 0.5f) * this.m_Zoom * -1f;
					base.Repaint();
				}
				current.Use();
			}
		}

		private void UpdateSelectedSpriteRect(Sprite sprite)
		{
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				if (sprite.rect == this.m_RectsCache.RectAt(i).rect)
				{
					this.selectedSpriteRect = this.m_RectsCache.RectAt(i);
					return;
				}
			}
			this.selectedSpriteRect = null;
		}

		private ITexture2D GetSelectedTexture2D()
		{
			UnityEngine.Texture2D texture2D = null;
			if (Selection.activeObject is UnityEngine.Texture2D)
			{
				texture2D = (Selection.activeObject as UnityEngine.Texture2D);
			}
			else if (Selection.activeObject is Sprite)
			{
				texture2D = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(Selection.activeObject as Sprite, false);
			}
			else if (Selection.activeGameObject)
			{
				if (Selection.activeGameObject.GetComponent<SpriteRenderer>())
				{
					if (Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite)
					{
						texture2D = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite, false);
					}
				}
			}
			if (texture2D != null)
			{
				this.m_SelectedAssetPath = this.m_AssetDatabase.GetAssetPath(texture2D);
			}
			return new UnityEngine.U2D.Interface.Texture2D(texture2D);
		}

		private SpriteRect TrySelect(Vector2 mousePosition)
		{
			float num = 3.40282347E+38f;
			SpriteRect spriteRect = null;
			mousePosition = Handles.inverseMatrix.MultiplyPoint(mousePosition);
			SpriteRect result;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				SpriteRect spriteRect2 = this.m_RectsCache.RectAt(i);
				if (spriteRect2.rect.Contains(mousePosition))
				{
					if (spriteRect2 == this.selectedSpriteRect)
					{
						result = spriteRect2;
						return result;
					}
					float width = spriteRect2.rect.width;
					float height = spriteRect2.rect.height;
					float num2 = width * height;
					if (width > 0f && height > 0f && num2 < num)
					{
						spriteRect = spriteRect2;
						num = num2;
					}
				}
			}
			result = spriteRect;
			return result;
		}

		public void DoTextureReimport(string path)
		{
			if (this.m_TextureImporterSO != null)
			{
				try
				{
					AssetDatabase.StartAssetEditing();
					AssetDatabase.ImportAsset(path);
				}
				finally
				{
					AssetDatabase.StopAssetEditing();
				}
				this.textureIsDirty = false;
			}
		}

		private void SetupModule(int newModuleIndex)
		{
			if (!(SpriteEditorWindow.s_Instance == null))
			{
				if (this.m_CurrentModule != null)
				{
					this.m_CurrentModule.OnModuleDeactivate();
				}
				if (this.m_RegisteredModules.Count > newModuleIndex)
				{
					this.m_CurrentModule = this.m_RegisteredModules[newModuleIndex];
					this.m_CurrentModule.OnModuleActivate();
					this.m_CurrentModuleIndex = newModuleIndex;
				}
			}
		}

		private void UpdateAvailableModules()
		{
			if (this.m_AllRegisteredModules != null)
			{
				this.m_RegisteredModules = new List<ISpriteEditorModule>();
				foreach (ISpriteEditorModule current in this.m_AllRegisteredModules)
				{
					if (current.CanBeActivated())
					{
						this.m_RegisteredModules.Add(current);
					}
				}
				this.m_RegisteredModuleNames = new GUIContent[this.m_RegisteredModules.Count];
				for (int i = 0; i < this.m_RegisteredModules.Count; i++)
				{
					this.m_RegisteredModuleNames[i] = new GUIContent(this.m_RegisteredModules[i].moduleName);
				}
				if (!this.m_RegisteredModules.Contains(this.m_CurrentModule))
				{
					this.SetupModule(0);
				}
				else
				{
					this.SetupModule(this.m_CurrentModuleIndex);
				}
			}
		}

		private void InitModules()
		{
			this.m_AllRegisteredModules = new List<ISpriteEditorModule>();
			if (this.m_OutlineTexture == null)
			{
				this.m_OutlineTexture = new UnityEngine.Texture2D(1, 16, TextureFormat.RGBA32, false);
				this.m_OutlineTexture.SetPixels(new Color[]
				{
					new Color(0.5f, 0.5f, 0.5f, 0.5f),
					new Color(0.5f, 0.5f, 0.5f, 0.5f),
					new Color(0.8f, 0.8f, 0.8f, 0.8f),
					new Color(0.8f, 0.8f, 0.8f, 0.8f),
					Color.white,
					Color.white,
					Color.white,
					Color.white,
					new Color(0.8f, 0.8f, 0.8f, 1f),
					new Color(0.5f, 0.5f, 0.5f, 0.8f),
					new Color(0.3f, 0.3f, 0.3f, 0.5f),
					new Color(0.3f, 0.3f, 0.3f, 0.5f),
					new Color(0.3f, 0.3f, 0.3f, 0.3f),
					new Color(0.3f, 0.3f, 0.3f, 0.3f),
					new Color(0.1f, 0.1f, 0.1f, 0.1f),
					new Color(0.1f, 0.1f, 0.1f, 0.1f)
				});
				this.m_OutlineTexture.Apply();
				this.m_OutlineTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			UnityEngine.U2D.Interface.Texture2D outlineTexture = new UnityEngine.U2D.Interface.Texture2D(this.m_OutlineTexture);
			this.m_AllRegisteredModules.Add(new SpriteFrameModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase));
			this.m_AllRegisteredModules.Add(new SpritePolygonModeModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase));
			this.m_AllRegisteredModules.Add(new SpriteOutlineModule(this, this.m_EventSystem, this.m_UndoSystem, this.m_AssetDatabase, this.m_GUIUtility, new ShapeEditorFactory(), outlineTexture));
			this.UpdateAvailableModules();
		}

		public void RequestRepaint()
		{
			if (EditorWindow.focusedWindow != this)
			{
				base.Repaint();
			}
			else
			{
				this.m_RequestRepaint = true;
			}
		}

		public void SetDataModified()
		{
			this.textureIsDirty = true;
		}

		public void DisplayProgressBar(string title, string content, float progress)
		{
			EditorUtility.DisplayProgressBar(title, content, progress);
		}

		public void ClearProgressBar()
		{
			EditorUtility.ClearProgressBar();
		}

		public ITexture2D GetReadableTexture2D()
		{
			if (this.m_ReadableTexture == null)
			{
				ITextureImporter assetImporterFromPath = this.m_AssetDatabase.GetAssetImporterFromPath(this.m_SelectedAssetPath);
				int width = 0;
				int height = 0;
				assetImporterFromPath.GetWidthAndHeight(ref width, ref height);
				this.m_ReadableTexture = SpriteUtility.CreateTemporaryDuplicate(this.m_OriginalTexture, width, height);
				if (this.m_ReadableTexture != null)
				{
					this.m_ReadableTexture.filterMode = FilterMode.Point;
				}
			}
			return new UnityEngine.U2D.Interface.Texture2D(this.m_ReadableTexture);
		}
	}
}
