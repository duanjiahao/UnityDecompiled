using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Sprites;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class SpriteEditorWindow : SpriteUtilityWindow
	{
		private enum GizmoMode
		{
			BorderEditing,
			RectEditing
		}

		private class SpriteEditorWindowStyles
		{
			public static readonly GUIContent changeShapeLabel = EditorGUIUtility.TextContent("Change Shape");

			public static readonly GUIContent sliceButtonLabel = EditorGUIUtility.TextContent("Slice");

			public static readonly GUIContent trimButtonLabel = EditorGUIUtility.TextContent("Trim|Trims selected rectangle (T)");

			public static readonly GUIContent sidesLabel = EditorGUIUtility.TextContent("Sides");

			public static readonly GUIContent polygonChangeShapeHelpBoxContent = EditorGUIUtility.TextContent("Sides can only be either 0 or anything between 3 and 128");

			public static readonly GUIContent changeButtonLabel = EditorGUIUtility.TextContent("Change|Change to the new number of sides");

			public static readonly GUIContent editingDiableMessageLabel = EditorGUIUtility.TextContent("Editing is disabled during play mode");

			public static readonly GUIContent spriteLabel = EditorGUIUtility.TextContent("Sprite");

			public static readonly GUIContent customPivotLabel = EditorGUIUtility.TextContent("Custom Pivot");

			public static readonly GUIContent borderLabel = EditorGUIUtility.TextContent("Border");

			public static readonly GUIContent lLabel = EditorGUIUtility.TextContent("L");

			public static readonly GUIContent tLabel = EditorGUIUtility.TextContent("T");

			public static readonly GUIContent rLabel = EditorGUIUtility.TextContent("R");

			public static readonly GUIContent bLabel = EditorGUIUtility.TextContent("B");

			public static readonly GUIContent positionLabel = EditorGUIUtility.TextContent("Position");

			public static readonly GUIContent xLabel = EditorGUIUtility.TextContent("X");

			public static readonly GUIContent yLabel = EditorGUIUtility.TextContent("Y");

			public static readonly GUIContent wLabel = EditorGUIUtility.TextContent("W");

			public static readonly GUIContent hLabel = EditorGUIUtility.TextContent("H");

			public static readonly GUIContent nameLabel = EditorGUIUtility.TextContent("Name");

			public static readonly GUIContent revertButtonLabel = EditorGUIUtility.TextContent("Revert");

			public static readonly GUIContent applyButtonLabel = EditorGUIUtility.TextContent("Apply");

			public static readonly GUIContent spriteEditorWindowTitle = EditorGUIUtility.TextContent("Sprite Editor");

			public static readonly GUIContent pendingChangesDialogContent = EditorGUIUtility.TextContent("You have pending changes in the Sprite Editor Window.\nDo you want to apply these changes?");

			public static readonly GUIContent yesButtonLabel = EditorGUIUtility.TextContent("Yes");

			public static readonly GUIContent noButtonLabel = EditorGUIUtility.TextContent("No");

			public static readonly GUIContent applyRevertDialogTitle = EditorGUIUtility.TextContent("Unapplied import settings");

			public static readonly GUIContent applyRevertDialogContent = EditorGUIUtility.TextContent("Unapplied import settings for '{0}'");

			public static readonly GUIContent creatingMultipleSpriteDialogTitle = EditorGUIUtility.TextContent("Creating multiple sprites");

			public static readonly GUIContent creatingMultipleSpriteDialogContent = EditorGUIUtility.TextContent("Creating {0} sprites. \nThis can take up to several minutes");

			public static readonly GUIContent okButtonLabel = EditorGUIUtility.TextContent("Ok");

			public static readonly GUIContent cancelButtonLabel = EditorGUIUtility.TextContent("Cancel");
		}

		public enum AutoSlicingMethod
		{
			DeleteAll,
			Smart,
			Safe
		}

		internal static PrefKey k_SpriteEditorTrim = new PrefKey("Sprite Editor/Trim", "#t");

		private const float maxSnapDistance = 14f;

		private const float marginForFraming = 0.05f;

		private const float k_WarningMessageWidth = 250f;

		private const float k_WarningMessageHeight = 40f;

		public static SpriteEditorWindow s_Instance;

		public bool m_ResetOnNextRepaint;

		public bool m_IgnoreNextPostprocessEvent;

		public Texture2D m_OriginalTexture;

		private const int k_PolygonChangeShapeWindowMargin = 17;

		private const int k_PolygonChangeShapeWindowWidth = 150;

		private const int k_PolygonChangeShapeWindowHeight = 45;

		private const int k_PolygonChangeShapeWindowWarningHeight = 65;

		private int m_PolygonSides = 0;

		private bool m_ShowPolygonChangeShapeWindow = false;

		private Rect m_PolygonChangeShapeWindowRect = new Rect(0f, 17f, 150f, 45f);

		protected const float k_InspectorHeight = 160f;

		private SpriteRectCache m_RectsCache;

		private SerializedObject m_TextureImporterSO;

		private TextureImporter m_TextureImporter;

		private SerializedProperty m_TextureImporterSprites;

		private SerializedProperty m_SpriteSheetOutline;

		public static bool s_OneClickDragStarted = false;

		private bool m_TextureIsDirty;

		private static bool[] s_AlphaPixelCache;

		public string m_SelectedAssetPath;

		private SpriteEditorWindow.GizmoMode m_GizmoMode;

		[SerializeField]
		private SpriteRect m_Selected;

		public Texture2D originalTexture
		{
			get
			{
				return this.m_OriginalTexture;
			}
		}

		internal Texture2D previewTexture
		{
			get
			{
				return this.m_Texture;
			}
		}

		internal SpriteRect selected
		{
			get
			{
				SpriteRect result;
				if (this.IsEditingDisabled())
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
				if (value != this.m_Selected)
				{
					this.m_Selected = value;
				}
			}
		}

		private int defaultColliderAlphaCutoff
		{
			get
			{
				return 254;
			}
		}

		private float defaultColliderDetail
		{
			get
			{
				return 0.25f;
			}
		}

		private Rect inspectorRect
		{
			get
			{
				return new Rect(base.position.width - 330f - 8f - 16f, base.position.height - 160f - 8f - 16f, 330f, 160f);
			}
		}

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
			get
			{
				return this.m_TextureIsDirty;
			}
			set
			{
				this.m_TextureIsDirty = value;
			}
		}

		public bool selectedTextureChanged
		{
			get
			{
				Texture2D selectedTexture2D = this.GetSelectedTexture2D();
				return selectedTexture2D != null && this.m_OriginalTexture != selectedTexture2D;
			}
		}

		private bool polygonSprite
		{
			get
			{
				return this.m_TextureImporter != null && this.m_TextureImporter.spriteImportMode == SpriteImportMode.Polygon;
			}
		}

		private bool isSidesValid
		{
			get
			{
				return this.m_PolygonSides == 0 || (this.m_PolygonSides >= 3 && this.m_PolygonSides <= 128);
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

		private void OnPlayModeChanged()
		{
			this.OnSelectionChange();
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
					this.m_SpriteSheetOutline = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Outline");
					if (this.m_RectsCache != null)
					{
						this.selected = ((this.m_TextureImporterSprites.arraySize <= 0) ? null : this.m_RectsCache.RectAt(0));
					}
					int width = 0;
					int height = 0;
					this.m_TextureImporter.GetWidthAndHeight(ref width, ref height);
					this.m_Texture = this.CreateTemporaryDuplicate(AssetDatabase.LoadMainAssetAtPath(this.m_TextureImporter.assetPath) as Texture2D, width, height);
					if (!(this.m_Texture == null))
					{
						this.m_Texture.filterMode = FilterMode.Point;
					}
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
			if (this.m_Texture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Texture);
			}
			this.m_OriginalTexture = null;
			this.m_TextureImporter = null;
			this.m_TextureImporterSO = null;
			this.m_TextureImporterSprites = null;
			SpriteEditorWindow.s_AlphaPixelCache = null;
		}

		private void InitializeAnimVariables()
		{
		}

		private void DeterminePolygonSides()
		{
			if (this.selected != null && this.selected.m_Outline != null && this.selected.m_Outline.Count == 1)
			{
				this.m_PolygonSides = this.selected.m_Outline[0].Count;
			}
			else
			{
				this.m_PolygonSides = 0;
			}
		}

		private static void AcquireOutline(SerializedProperty outlineSP, SpriteRect spriteRect)
		{
			for (int i = 0; i < outlineSP.arraySize; i++)
			{
				List<Vector2> list = new List<Vector2>();
				SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
				for (int j = 0; j < arrayElementAtIndex.arraySize; j++)
				{
					Vector2 vector2Value = arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value;
					list.Add(vector2Value);
				}
				spriteRect.m_Outline.Add(list);
			}
		}

		private static void ApplyOutlineChanges(SerializedProperty outlineSP, SpriteRect spriteRect)
		{
			outlineSP.ClearArray();
			for (int i = 0; i < spriteRect.m_Outline.Count; i++)
			{
				outlineSP.InsertArrayElementAtIndex(i);
				SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
				arrayElementAtIndex.ClearArray();
				List<Vector2> list = spriteRect.m_Outline[i];
				for (int j = 0; j < list.Count; j++)
				{
					arrayElementAtIndex.InsertArrayElementAtIndex(j);
					arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value = list[j];
				}
			}
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
			}
			this.InvalidatePropertiesCache();
			this.Reset();
			this.UpdateSelectedSprite();
			base.Repaint();
		}

		public void Reset()
		{
			this.InvalidatePropertiesCache();
			this.selected = null;
			this.textureIsDirty = false;
			this.m_Zoom = -1f;
			this.RefreshPropertiesCache();
			this.RefreshRects();
			this.m_ShowPolygonChangeShapeWindow = this.polygonSprite;
			if (this.m_ShowPolygonChangeShapeWindow)
			{
				this.DeterminePolygonSides();
			}
			base.Repaint();
		}

		private void OnEnable()
		{
			base.minSize = new Vector2(360f, 200f);
			base.titleContent = SpriteEditorWindow.SpriteEditorWindowStyles.spriteEditorWindowTitle;
			SpriteEditorWindow.s_Instance = this;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeChanged));
			this.Reset();
		}

		private void UndoRedoPerformed()
		{
			Texture2D selectedTexture2D = this.GetSelectedTexture2D();
			if (selectedTexture2D != null && this.m_OriginalTexture != selectedTexture2D)
			{
				this.OnSelectionChange();
			}
			if (this.m_RectsCache != null && !this.m_RectsCache.Contains(this.selected))
			{
				this.selected = null;
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
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeChanged));
			SpriteEditorWindow.s_Instance = null;
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
						spriteRect.m_Rect = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Rect").rectValue;
						spriteRect.m_Name = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue;
						spriteRect.m_Alignment = (SpriteAlignment)this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Alignment").intValue;
						spriteRect.m_Border = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Border").vector4Value;
						spriteRect.m_Pivot = SpriteEditorUtility.GetPivotValue(spriteRect.m_Alignment, this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Pivot").vector2Value);
						spriteRect.m_TessellationDetail = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_TessellationDetail").floatValue;
						SerializedProperty outlineSP = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Outline");
						SpriteEditorWindow.AcquireOutline(outlineSP, spriteRect);
						this.m_RectsCache.AddRect(spriteRect);
					}
				}
				else if (this.validSprite)
				{
					SpriteRect spriteRect2 = new SpriteRect();
					spriteRect2.m_Rect = new Rect(0f, 0f, (float)this.m_Texture.width, (float)this.m_Texture.height);
					spriteRect2.m_Name = this.m_OriginalTexture.name;
					spriteRect2.m_Alignment = (SpriteAlignment)this.m_TextureImporterSO.FindProperty("m_Alignment").intValue;
					spriteRect2.m_Border = this.m_TextureImporter.spriteBorder;
					spriteRect2.m_Pivot = SpriteEditorUtility.GetPivotValue(spriteRect2.m_Alignment, this.m_TextureImporter.spritePivot);
					spriteRect2.m_TessellationDetail = this.m_TextureImporterSO.FindProperty("m_SpriteTessellationDetail").floatValue;
					SpriteEditorWindow.AcquireOutline(this.m_SpriteSheetOutline, spriteRect2);
					this.m_RectsCache.AddRect(spriteRect2);
				}
				if (this.m_RectsCache.Count > 0)
				{
					this.selected = this.m_RectsCache.RectAt(0);
				}
			}
		}

		private void OnGUI()
		{
			if (this.m_ResetOnNextRepaint || this.selectedTextureChanged)
			{
				this.Reset();
				this.m_ResetOnNextRepaint = false;
			}
			Matrix4x4 matrix = Handles.matrix;
			if (!this.activeTextureSelected)
			{
				using (new EditorGUI.DisabledScope(true))
				{
					GUILayout.Label(SpriteUtilityWindow.Styles.s_NoSelectionWarning, new GUILayoutOption[0]);
				}
			}
			else
			{
				base.InitStyles();
				Rect rect = EditorGUILayout.BeginHorizontal(GUIContent.none, "Toolbar", new GUILayoutOption[0]);
				this.DoToolbarGUI();
				GUILayout.FlexibleSpace();
				this.DoApplyRevertGUI();
				base.DoAlphaZoomToolbarGUI();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				this.m_TextureViewRect = new Rect(0f, rect.yMax, base.position.width - 16f, base.position.height - 16f - rect.height);
				GUILayout.FlexibleSpace();
				base.DoTextureGUI();
				EditorGUILayout.EndHorizontal();
				this.DoPolygonChangeShapeWindow();
				this.DoEditingDisabledMessage();
				this.DoSelectedFrameInspector();
				Handles.matrix = matrix;
			}
		}

		protected override void DoTextureGUIExtras()
		{
			if (!this.IsEditingDisabled())
			{
				this.HandleGizmoMode();
				if (this.multipleSprites)
				{
					this.HandleRectCornerScalingHandles();
				}
				this.HandleBorderCornerScalingHandles();
				this.HandleBorderSidePointScalingSliders();
				if (this.multipleSprites)
				{
					this.HandleRectSideScalingHandles();
				}
				this.HandleBorderSideScalingHandles();
				this.HandlePivotHandle();
				if (this.multipleSprites)
				{
					this.HandleDragging();
				}
				this.HandleSelection();
				this.HandleFrameSelected();
				if (this.multipleSprites)
				{
					this.HandleCreate();
					this.HandleDelete();
					this.HandleDuplicate();
				}
			}
		}

		private void HandleGizmoMode()
		{
			if (Event.current.control)
			{
				this.m_GizmoMode = SpriteEditorWindow.GizmoMode.BorderEditing;
			}
			else
			{
				this.m_GizmoMode = SpriteEditorWindow.GizmoMode.RectEditing;
			}
			Event current = Event.current;
			if ((current.type == EventType.KeyDown || current.type == EventType.KeyUp) && (current.keyCode == KeyCode.LeftControl || current.keyCode == KeyCode.RightControl || current.keyCode == KeyCode.LeftAlt || current.keyCode == KeyCode.RightAlt))
			{
				base.Repaint();
			}
		}

		private void DoToolbarGUI()
		{
			if (this.polygonSprite)
			{
				using (new EditorGUI.DisabledScope(this.IsEditingDisabled()))
				{
					this.m_ShowPolygonChangeShapeWindow = GUILayout.Toggle(this.m_ShowPolygonChangeShapeWindow, SpriteEditorWindow.SpriteEditorWindowStyles.changeShapeLabel, EditorStyles.toolbarButton, new GUILayoutOption[0]);
				}
			}
			else
			{
				using (new EditorGUI.DisabledScope(!this.multipleSprites || this.IsEditingDisabled()))
				{
					Rect buttonRect = EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button(SpriteEditorWindow.SpriteEditorWindowStyles.sliceButtonLabel, EditorStyles.toolbarPopup, new GUILayoutOption[0]))
					{
						SpriteEditorMenu.s_SpriteEditor = this;
						if (SpriteEditorMenu.ShowAtPosition(buttonRect))
						{
							GUIUtility.ExitGUI();
						}
					}
					using (new EditorGUI.DisabledScope(this.selected == null))
					{
						if (GUILayout.Button(SpriteEditorWindow.SpriteEditorWindowStyles.trimButtonLabel, EditorStyles.toolbarButton, new GUILayoutOption[0]) || (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()) && SpriteEditorWindow.k_SpriteEditorTrim.activated))
						{
							Rect rect = this.TrimAlpha(this.selected.m_Rect);
							if (rect.width <= 0f && rect.height <= 0f)
							{
								this.m_RectsCache.RemoveRect(this.selected);
								this.selected = null;
							}
							else
							{
								rect = this.ClampSpriteRect(rect);
								if (this.selected.m_Rect != rect)
								{
									this.textureIsDirty = true;
								}
								this.selected.m_Rect = rect;
							}
							base.Repaint();
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void DoPolygonChangeShapeWindow()
		{
			if (this.m_ShowPolygonChangeShapeWindow && !this.IsEditingDisabled())
			{
				bool flag = false;
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 45f;
				GUILayout.BeginArea(this.m_PolygonChangeShapeWindowRect);
				GUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
				Event current = Event.current;
				if (this.isSidesValid && current.type == EventType.KeyDown && current.keyCode == KeyCode.Return)
				{
					flag = true;
					current.Use();
				}
				EditorGUI.BeginChangeCheck();
				this.m_PolygonSides = EditorGUILayout.IntField(SpriteEditorWindow.SpriteEditorWindowStyles.sidesLabel, this.m_PolygonSides, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					if (!this.isSidesValid)
					{
						this.m_PolygonChangeShapeWindowRect.height = 65f;
					}
					else
					{
						this.m_PolygonChangeShapeWindowRect.height = 45f;
					}
				}
				GUILayout.FlexibleSpace();
				if (!this.isSidesValid)
				{
					EditorGUILayout.HelpBox(SpriteEditorWindow.SpriteEditorWindowStyles.polygonChangeShapeHelpBoxContent.text, MessageType.Warning, true);
				}
				else
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					using (new EditorGUI.DisabledScope(!this.isSidesValid))
					{
						if (GUILayout.Button(SpriteEditorWindow.SpriteEditorWindowStyles.changeButtonLabel, new GUILayoutOption[0]))
						{
							flag = true;
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				if (flag)
				{
					if (this.isSidesValid)
					{
						this.GeneratePolygonOutline(this.m_PolygonSides);
					}
					this.m_ShowPolygonChangeShapeWindow = false;
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}
				EditorGUIUtility.labelWidth = labelWidth;
				GUILayout.EndArea();
			}
		}

		private void FourIntFields(GUIContent label, GUIContent labelX, GUIContent labelY, GUIContent labelZ, GUIContent labelW, ref int x, ref int y, ref int z, ref int w)
		{
			Rect rect = GUILayoutUtility.GetRect(322f, 32f);
			Rect position = rect;
			position.width = EditorGUIUtility.labelWidth;
			position.height = 16f;
			GUI.Label(position, label);
			Rect position2 = rect;
			position2.width -= EditorGUIUtility.labelWidth;
			position2.height = 16f;
			position2.x += EditorGUIUtility.labelWidth;
			position2.width /= 2f;
			position2.width -= 2f;
			EditorGUIUtility.labelWidth = 12f;
			GUI.SetNextControlName("FourIntFields_x");
			x = EditorGUI.IntField(position2, labelX, x);
			position2.x += position2.width + 3f;
			GUI.SetNextControlName("FourIntFields_y");
			y = EditorGUI.IntField(position2, labelY, y);
			position2.y += 16f;
			position2.x -= position2.width + 3f;
			GUI.SetNextControlName("FourIntFields_z");
			z = EditorGUI.IntField(position2, labelZ, z);
			position2.x += position2.width + 3f;
			GUI.SetNextControlName("FourIntFields_w");
			w = EditorGUI.IntField(position2, labelW, w);
			EditorGUIUtility.labelWidth = 135f;
		}

		private void DoEditingDisabledMessage()
		{
			if (this.IsEditingDisabled())
			{
				GUILayout.BeginArea(this.warningMessageRect);
				EditorGUILayout.HelpBox(SpriteEditorWindow.SpriteEditorWindowStyles.editingDiableMessageLabel.text, MessageType.Warning);
				GUILayout.EndArea();
			}
		}

		private void DoSelectedFrameInspector()
		{
			if (this.selected != null)
			{
				EditorGUIUtility.wideMode = true;
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 135f;
				GUILayout.BeginArea(this.inspectorRect);
				GUILayout.BeginVertical(SpriteEditorWindow.SpriteEditorWindowStyles.spriteLabel, GUI.skin.window, new GUILayoutOption[0]);
				using (new EditorGUI.DisabledScope(!this.multipleSprites))
				{
					this.DoNameField();
					this.DoPositionField();
				}
				this.DoBorderFields();
				this.DoPivotFields();
				GUILayout.EndVertical();
				GUILayout.EndArea();
				EditorGUIUtility.labelWidth = labelWidth;
			}
		}

		private void DoPivotFields()
		{
			EditorGUI.BeginChangeCheck();
			this.selected.m_Alignment = (SpriteAlignment)EditorGUILayout.Popup(SpriteUtilityWindow.Styles.s_PivotLabel, (int)this.selected.m_Alignment, SpriteUtilityWindow.Styles.spriteAlignmentOptions, new GUILayoutOption[0]);
			Vector2 pivot = this.selected.m_Pivot;
			Vector2 customOffset = pivot;
			using (new EditorGUI.DisabledScope(this.selected.m_Alignment != SpriteAlignment.Custom))
			{
				Rect rect = GUILayoutUtility.GetRect(322f, EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, SpriteEditorWindow.SpriteEditorWindowStyles.customPivotLabel));
				GUI.SetNextControlName("PivotField");
				customOffset = EditorGUI.Vector2Field(rect, SpriteEditorWindow.SpriteEditorWindowStyles.customPivotLabel, pivot);
			}
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Pivot");
				this.textureIsDirty = true;
				this.selected.m_Pivot = SpriteEditorUtility.GetPivotValue(this.selected.m_Alignment, customOffset);
			}
		}

		private void DoBorderFields()
		{
			EditorGUI.BeginChangeCheck();
			Vector4 vector = this.ClampSpriteBorder(this.selected.m_Border);
			int num = Mathf.RoundToInt(vector.x);
			int num2 = Mathf.RoundToInt(vector.y);
			int num3 = Mathf.RoundToInt(vector.z);
			int num4 = Mathf.RoundToInt(vector.w);
			this.FourIntFields(SpriteEditorWindow.SpriteEditorWindowStyles.borderLabel, SpriteEditorWindow.SpriteEditorWindowStyles.lLabel, SpriteEditorWindow.SpriteEditorWindowStyles.tLabel, SpriteEditorWindow.SpriteEditorWindowStyles.rLabel, SpriteEditorWindow.SpriteEditorWindowStyles.bLabel, ref num, ref num4, ref num3, ref num2);
			Vector4 border = new Vector4((float)num, (float)num2, (float)num3, (float)num4);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Border");
				this.textureIsDirty = true;
				this.selected.m_Border = this.ClampSpriteBorder(border);
			}
		}

		private void DoPositionField()
		{
			EditorGUI.BeginChangeCheck();
			Rect rect = this.selected.m_Rect;
			int num = Mathf.RoundToInt(rect.x);
			int num2 = Mathf.RoundToInt(rect.y);
			int num3 = Mathf.RoundToInt(rect.width);
			int num4 = Mathf.RoundToInt(rect.height);
			this.FourIntFields(SpriteEditorWindow.SpriteEditorWindowStyles.positionLabel, SpriteEditorWindow.SpriteEditorWindowStyles.xLabel, SpriteEditorWindow.SpriteEditorWindowStyles.yLabel, SpriteEditorWindow.SpriteEditorWindowStyles.wLabel, SpriteEditorWindow.SpriteEditorWindowStyles.hLabel, ref num, ref num2, ref num3, ref num4);
			Rect rect2 = new Rect((float)num, (float)num2, (float)num3, (float)num4);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Position");
				this.textureIsDirty = true;
				this.selected.m_Rect = this.ClampSpriteRect(rect2);
			}
		}

		private void DoNameField()
		{
			EditorGUI.BeginChangeCheck();
			string name = this.selected.m_Name;
			GUI.SetNextControlName("SpriteName");
			string text = EditorGUILayout.TextField(SpriteEditorWindow.SpriteEditorWindowStyles.nameLabel, name, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Name");
				this.textureIsDirty = true;
				text = InternalEditorUtility.RemoveInvalidCharsFromFileName(text, true);
				if (string.IsNullOrEmpty(this.selected.m_OriginalName) && text != name)
				{
					this.selected.m_OriginalName = name;
				}
				if (string.IsNullOrEmpty(text))
				{
					text = name;
				}
				foreach (SpriteRect current in this.m_RectsCache.m_Rects)
				{
					if (current.m_Name == text)
					{
						text = this.selected.m_OriginalName;
						break;
					}
				}
				this.selected.m_Name = text;
			}
		}

		private void DoApplyRevertGUI()
		{
			using (new EditorGUI.DisabledScope(!this.textureIsDirty))
			{
				if (GUILayout.Button(SpriteEditorWindow.SpriteEditorWindowStyles.revertButtonLabel, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					this.DoRevert();
				}
				if (GUILayout.Button(SpriteEditorWindow.SpriteEditorWindowStyles.applyButtonLabel, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					this.DoApply();
				}
			}
		}

		private void DoApply(SerializedObject so)
		{
			if (this.multipleSprites)
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				SerializedProperty serializedProperty = so.FindProperty("m_SpriteSheet.m_Sprites");
				serializedProperty.ClearArray();
				for (int i = 0; i < this.m_RectsCache.Count; i++)
				{
					SpriteRect spriteRect = this.m_RectsCache.RectAt(i);
					if (string.IsNullOrEmpty(spriteRect.m_Name))
					{
						spriteRect.m_Name = "Empty";
					}
					if (!string.IsNullOrEmpty(spriteRect.m_OriginalName))
					{
						list.Add(spriteRect.m_OriginalName);
						list2.Add(spriteRect.m_Name);
					}
					serializedProperty.InsertArrayElementAtIndex(i);
					SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(i);
					arrayElementAtIndex.FindPropertyRelative("m_Rect").rectValue = spriteRect.m_Rect;
					arrayElementAtIndex.FindPropertyRelative("m_Border").vector4Value = spriteRect.m_Border;
					arrayElementAtIndex.FindPropertyRelative("m_Name").stringValue = spriteRect.m_Name;
					arrayElementAtIndex.FindPropertyRelative("m_Alignment").intValue = (int)spriteRect.m_Alignment;
					arrayElementAtIndex.FindPropertyRelative("m_Pivot").vector2Value = spriteRect.m_Pivot;
					arrayElementAtIndex.FindPropertyRelative("m_TessellationDetail").floatValue = spriteRect.m_TessellationDetail;
					SerializedProperty outlineSP = arrayElementAtIndex.FindPropertyRelative("m_Outline");
					SpriteEditorWindow.ApplyOutlineChanges(outlineSP, spriteRect);
				}
				if (list.Count > 0)
				{
					PatchImportSettingRecycleID.PatchMultiple(so, 213, list.ToArray(), list2.ToArray());
				}
			}
			else if (this.m_RectsCache.Count > 0)
			{
				SpriteRect spriteRect2 = this.m_RectsCache.RectAt(0);
				so.FindProperty("m_Alignment").intValue = (int)spriteRect2.m_Alignment;
				so.FindProperty("m_SpriteBorder").vector4Value = spriteRect2.m_Border;
				so.FindProperty("m_SpritePivot").vector2Value = spriteRect2.m_Pivot;
				so.FindProperty("m_SpriteTessellationDetail").floatValue = spriteRect2.m_TessellationDetail;
				this.m_SpriteSheetOutline.ClearArray();
				SpriteEditorWindow.ApplyOutlineChanges(this.m_SpriteSheetOutline, spriteRect2);
			}
		}

		private void DoApply()
		{
			Undo.ClearUndo(this.m_RectsCache);
			this.DoApply(this.m_TextureImporterSO);
			this.m_TextureImporterSO.ApplyModifiedPropertiesWithoutUndo();
			this.m_IgnoreNextPostprocessEvent = true;
			this.DoTextureReimport(this.m_TextureImporter.assetPath);
			this.textureIsDirty = false;
			this.selected = null;
		}

		private void DoRevert()
		{
			this.m_TextureIsDirty = false;
			this.selected = null;
			this.RefreshRects();
			GUI.FocusControl("");
		}

		private void HandleDuplicate()
		{
			if ((Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand) && Event.current.commandName == "Duplicate")
			{
				if (Event.current.type == EventType.ExecuteCommand)
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Duplicate sprite");
					this.selected = this.AddSprite(this.selected.m_Rect, (int)this.selected.m_Alignment, this.selected.m_Pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
				}
				Event.current.Use();
			}
		}

		private void HandleCreate()
		{
			if (!this.MouseOnTopOfInspector() && !Event.current.alt)
			{
				EditorGUI.BeginChangeCheck();
				Rect rect = SpriteEditorHandles.RectCreator((float)this.m_Texture.width, (float)this.m_Texture.height, SpriteUtilityWindow.s_Styles.createRect);
				if (EditorGUI.EndChangeCheck() && rect.width > 0f && rect.height > 0f)
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Create sprite");
					this.selected = this.AddSprite(rect, 0, Vector2.zero, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
					GUIUtility.keyboardControl = 0;
				}
			}
		}

		private void HandleDelete()
		{
			if ((Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete"))
			{
				if (Event.current.type == EventType.ExecuteCommand)
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Delete sprite");
					this.m_RectsCache.RemoveRect(this.selected);
					this.selected = null;
					this.textureIsDirty = true;
				}
				Event.current.Use();
			}
		}

		private void HandleDragging()
		{
			if (this.selected != null && !this.MouseOnTopOfInspector())
			{
				Rect clamp = new Rect(0f, 0f, (float)this.m_Texture.width, (float)this.m_Texture.height);
				EditorGUI.BeginChangeCheck();
				SpriteRect selected = this.selected;
				Rect rect = this.selected.m_Rect;
				Rect rect2 = SpriteEditorUtility.ClampedRect(SpriteEditorUtility.RoundedRect(SpriteEditorHandles.SliderRect(rect)), clamp, true);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Move sprite");
					selected.m_Rect = rect2;
					this.textureIsDirty = true;
				}
			}
		}

		private void HandleSelection()
		{
			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && GUIUtility.hotControl == 0 && !Event.current.alt && !this.MouseOnTopOfInspector())
			{
				SpriteRect selected = this.selected;
				this.selected = this.TrySelect(Event.current.mousePosition);
				if (this.selected != null)
				{
					SpriteEditorWindow.s_OneClickDragStarted = true;
				}
				else
				{
					base.Repaint();
				}
				if (selected != this.selected && this.selected != null)
				{
					Event.current.Use();
				}
			}
		}

		private void HandleFrameSelected()
		{
			if ((Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand) && Event.current.commandName == "FrameSelected")
			{
				if (Event.current.type == EventType.ExecuteCommand)
				{
					if (this.selected == null)
					{
						return;
					}
					Rect rect = this.selected.m_Rect;
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
				Event.current.Use();
			}
		}

		private bool ShouldShowRectScaling()
		{
			return this.selected != null && this.m_GizmoMode == SpriteEditorWindow.GizmoMode.RectEditing;
		}

		private void HandlePivotHandle()
		{
			if (this.selected != null)
			{
				EditorGUI.BeginChangeCheck();
				SpriteRect selected = this.selected;
				selected.m_Pivot = this.ApplySpriteAlignmentToPivot(selected.m_Pivot, selected.m_Rect, selected.m_Alignment);
				Vector2 pivot = SpriteEditorHandles.PivotSlider(selected.m_Rect, selected.m_Pivot, SpriteUtilityWindow.s_Styles.pivotdot, SpriteUtilityWindow.s_Styles.pivotdotactive);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Move sprite pivot");
					if (Event.current.control)
					{
						this.selected.m_Pivot = this.SnapPivot(pivot);
					}
					else
					{
						this.selected.m_Pivot = pivot;
						this.selected.m_Alignment = SpriteAlignment.Custom;
					}
					this.textureIsDirty = true;
				}
			}
		}

		private Rect ClampSpriteRect(Rect rect)
		{
			Rect rect2 = default(Rect);
			rect2.xMin = Mathf.Clamp(rect.xMin, 0f, (float)(this.m_Texture.width - 1));
			rect2.yMin = Mathf.Clamp(rect.yMin, 0f, (float)(this.m_Texture.height - 1));
			rect2.xMax = Mathf.Clamp(rect.xMax, 1f, (float)this.m_Texture.width);
			rect2.yMax = Mathf.Clamp(rect.yMax, 1f, (float)this.m_Texture.height);
			if (Mathf.RoundToInt(rect2.width) == 0)
			{
				rect2.width = 1f;
			}
			if (Mathf.RoundToInt(rect2.height) == 0)
			{
				rect2.height = 1f;
			}
			return SpriteEditorUtility.RoundedRect(rect2);
		}

		private Rect FlipNegativeRect(Rect rect)
		{
			return new Rect
			{
				xMin = Mathf.Min(rect.xMin, rect.xMax),
				yMin = Mathf.Min(rect.yMin, rect.yMax),
				xMax = Mathf.Max(rect.xMin, rect.xMax),
				yMax = Mathf.Max(rect.yMin, rect.yMax)
			};
		}

		private Vector4 ClampSpriteBorder(Vector4 border)
		{
			Rect rect = this.FlipNegativeRect(this.selected.m_Rect);
			float width = rect.width;
			float height = rect.height;
			return new Vector4
			{
				x = (float)Mathf.RoundToInt(Mathf.Clamp(border.x, 0f, Mathf.Min(width - border.z, width))),
				z = (float)Mathf.RoundToInt(Mathf.Clamp(border.z, 0f, Mathf.Min(width - border.x, width))),
				y = (float)Mathf.RoundToInt(Mathf.Clamp(border.y, 0f, Mathf.Min(height - border.w, height))),
				w = (float)Mathf.RoundToInt(Mathf.Clamp(border.w, 0f, Mathf.Min(height - border.y, height)))
			};
		}

		private Vector2 SnapPivot(Vector2 pivot)
		{
			SpriteRect selected = this.selected;
			Rect rect = selected.m_Rect;
			Vector2 vector = new Vector2(rect.xMin + rect.width * pivot.x, rect.yMin + rect.height * pivot.y);
			Vector2[] snapPointsArray = this.GetSnapPointsArray(rect);
			SpriteAlignment alignment = SpriteAlignment.Custom;
			float num = 3.40282347E+38f;
			for (int i = 0; i < snapPointsArray.Length; i++)
			{
				float num2 = (vector - snapPointsArray[i]).magnitude * this.m_Zoom;
				if (num2 < num)
				{
					alignment = (SpriteAlignment)i;
					num = num2;
				}
			}
			this.selected.m_Alignment = alignment;
			return this.ConvertFromTextureToNormalizedSpace(vector, rect);
		}

		public Vector2 ApplySpriteAlignmentToPivot(Vector2 pivot, Rect rect, SpriteAlignment alignment)
		{
			Vector2[] snapPointsArray = this.GetSnapPointsArray(rect);
			Vector2 result;
			if (alignment != SpriteAlignment.Custom)
			{
				Vector2 texturePos = snapPointsArray[(int)alignment];
				result = this.ConvertFromTextureToNormalizedSpace(texturePos, rect);
			}
			else
			{
				result = pivot;
			}
			return result;
		}

		private Vector2 ConvertFromTextureToNormalizedSpace(Vector2 texturePos, Rect rect)
		{
			return new Vector2((texturePos.x - rect.xMin) / rect.width, (texturePos.y - rect.yMin) / rect.height);
		}

		private Vector2[] GetSnapPointsArray(Rect rect)
		{
			Vector2[] array = new Vector2[9];
			array[1] = new Vector2(rect.xMin, rect.yMax);
			array[2] = new Vector2(rect.center.x, rect.yMax);
			array[3] = new Vector2(rect.xMax, rect.yMax);
			array[4] = new Vector2(rect.xMin, rect.center.y);
			array[0] = new Vector2(rect.center.x, rect.center.y);
			array[5] = new Vector2(rect.xMax, rect.center.y);
			array[6] = new Vector2(rect.xMin, rect.yMin);
			array[7] = new Vector2(rect.center.x, rect.yMin);
			array[8] = new Vector2(rect.xMax, rect.yMin);
			return array;
		}

		private void UpdateSelectedSprite()
		{
			if (Selection.activeObject is Sprite)
			{
				this.SelectSpriteIndex(Selection.activeObject as Sprite);
			}
			else if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SpriteRenderer>())
			{
				Sprite sprite = Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite;
				this.SelectSpriteIndex(sprite);
			}
		}

		private void SelectSpriteIndex(Sprite sprite)
		{
			if (!(sprite == null))
			{
				this.selected = null;
				for (int i = 0; i < this.m_RectsCache.Count; i++)
				{
					if (sprite.rect == this.m_RectsCache.RectAt(i).m_Rect)
					{
						this.selected = this.m_RectsCache.RectAt(i);
						break;
					}
				}
			}
		}

		private Texture2D GetSelectedTexture2D()
		{
			Texture2D texture2D = null;
			if (Selection.activeObject is Texture2D)
			{
				texture2D = (Selection.activeObject as Texture2D);
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
				this.m_SelectedAssetPath = AssetDatabase.GetAssetPath(texture2D);
			}
			return texture2D;
		}

		protected override void DrawGizmos()
		{
			SpriteEditorUtility.BeginLines(new Color(0f, 0f, 0f, 0.25f));
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				Rect rect = this.m_RectsCache.RectAt(i).m_Rect;
				if (this.m_RectsCache.RectAt(i) != this.selected)
				{
					SpriteEditorUtility.DrawBox(new Rect(rect.xMin + 1f / this.m_Zoom, rect.yMin + 1f / this.m_Zoom, rect.width, rect.height));
				}
			}
			SpriteEditorUtility.EndLines();
			SpriteEditorUtility.BeginLines(new Color(1f, 1f, 1f, 0.5f));
			for (int j = 0; j < this.m_RectsCache.Count; j++)
			{
				if (this.m_RectsCache.RectAt(j) != this.selected)
				{
					SpriteEditorUtility.DrawBox(this.m_RectsCache.RectAt(j).m_Rect);
				}
			}
			SpriteEditorUtility.EndLines();
			if (this.polygonSprite)
			{
				for (int k = 0; k < this.m_RectsCache.Count; k++)
				{
					SpriteRect spriteRect = this.m_RectsCache.RectAt(k);
					Vector2 b = spriteRect.m_Rect.size * 0.5f;
					if (spriteRect.m_Outline.Count > 0)
					{
						SpriteEditorUtility.BeginLines(new Color(0.75f, 0.75f, 0.75f, 0.75f));
						for (int l = 0; l < spriteRect.m_Outline.Count; l++)
						{
							int m;
							for (m = 0; m < spriteRect.m_Outline[l].Count - 1; m++)
							{
								SpriteEditorUtility.DrawLine(spriteRect.m_Outline[l][m] + b, spriteRect.m_Outline[l][m + 1] + b);
							}
							SpriteEditorUtility.DrawLine(spriteRect.m_Outline[l][m] + b, spriteRect.m_Outline[l][0] + b);
						}
						SpriteEditorUtility.EndLines();
					}
				}
			}
			SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
			for (int n = 0; n < this.m_RectsCache.Count; n++)
			{
				SpriteRect spriteRect2 = this.m_RectsCache.RectAt(n);
				if (this.ShouldDrawBorders(spriteRect2))
				{
					Vector4 border = spriteRect2.m_Border;
					Rect rect2 = spriteRect2.m_Rect;
					SpriteEditorUtility.DrawLine(new Vector3(rect2.xMin + border.x, rect2.yMin), new Vector3(rect2.xMin + border.x, rect2.yMax));
					SpriteEditorUtility.DrawLine(new Vector3(rect2.xMax - border.z, rect2.yMin), new Vector3(rect2.xMax - border.z, rect2.yMax));
					SpriteEditorUtility.DrawLine(new Vector3(rect2.xMin, rect2.yMin + border.y), new Vector3(rect2.xMax, rect2.yMin + border.y));
					SpriteEditorUtility.DrawLine(new Vector3(rect2.xMin, rect2.yMax - border.w), new Vector3(rect2.xMax, rect2.yMax - border.w));
				}
			}
			SpriteEditorUtility.EndLines();
			if (this.ShouldShowRectScaling())
			{
				Rect rect3 = this.selected.m_Rect;
				SpriteEditorUtility.BeginLines(new Color(0f, 0.1f, 0.3f, 0.25f));
				SpriteEditorUtility.DrawBox(new Rect(rect3.xMin + 1f / this.m_Zoom, rect3.yMin + 1f / this.m_Zoom, rect3.width, rect3.height));
				SpriteEditorUtility.EndLines();
				SpriteEditorUtility.BeginLines(new Color(0.25f, 0.5f, 1f, 0.75f));
				SpriteEditorUtility.DrawBox(rect3);
				SpriteEditorUtility.EndLines();
			}
		}

		private bool ShouldDrawBorders(SpriteRect currentRect)
		{
			Vector4 border = currentRect.m_Border;
			return !Mathf.Approximately(border.sqrMagnitude, 0f) || (currentRect == this.selected && this.m_GizmoMode == SpriteEditorWindow.GizmoMode.BorderEditing);
		}

		private SpriteRect TrySelect(Vector2 mousePosition)
		{
			float num = 1E+07f;
			SpriteRect spriteRect = null;
			SpriteRect result;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				if (this.m_RectsCache.RectAt(i).m_Rect.Contains(Handles.s_InverseMatrix.MultiplyPoint(mousePosition)))
				{
					if (this.m_RectsCache.RectAt(i) == this.selected)
					{
						result = this.m_RectsCache.RectAt(i);
						return result;
					}
					float width = this.m_RectsCache.RectAt(i).m_Rect.width;
					float height = this.m_RectsCache.RectAt(i).m_Rect.height;
					float num2 = width * height;
					if (width > 0f && height > 0f && num2 < num)
					{
						spriteRect = this.m_RectsCache.RectAt(i);
						num = num2;
					}
				}
			}
			result = spriteRect;
			return result;
		}

		public SpriteRect AddSprite(Rect rect, int alignment, Vector2 pivot, int colliderAlphaCutoff, float colliderDetail)
		{
			SpriteRect spriteRect = new SpriteRect();
			spriteRect.m_Rect = rect;
			spriteRect.m_Alignment = (SpriteAlignment)alignment;
			spriteRect.m_Pivot = pivot;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.m_TextureImporter.assetPath);
			spriteRect.m_Name = this.GetUniqueName(fileNameWithoutExtension);
			spriteRect.m_OriginalName = spriteRect.m_Name;
			this.textureIsDirty = true;
			this.m_RectsCache.AddRect(spriteRect);
			return spriteRect;
		}

		private string GetUniqueName(string prefix)
		{
			int num = 0;
			string text;
			bool flag;
			do
			{
				text = prefix + "_" + num++;
				flag = false;
				foreach (SpriteRect current in this.m_RectsCache.m_Rects)
				{
					if (current.m_Name == text)
					{
						flag = true;
					}
				}
			}
			while (flag);
			return text;
		}

		private Rect TrimAlpha(Rect rect)
		{
			int num = (int)rect.xMax;
			int num2 = (int)rect.xMin;
			int num3 = (int)rect.yMax;
			int num4 = (int)rect.yMin;
			for (int i = (int)rect.yMin; i < (int)rect.yMax; i++)
			{
				for (int j = (int)rect.xMin; j < (int)rect.xMax; j++)
				{
					if (this.PixelHasAlpha(j, i))
					{
						num = Mathf.Min(num, j);
						num2 = Mathf.Max(num2, j);
						num3 = Mathf.Min(num3, i);
						num4 = Mathf.Max(num4, i);
					}
				}
			}
			Rect result;
			if (num > num2 || num3 > num4)
			{
				result = new Rect(0f, 0f, 0f, 0f);
			}
			else
			{
				result = new Rect((float)num, (float)num3, (float)(num2 - num + 1), (float)(num4 - num3 + 1));
			}
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

		private void HandleRectCornerScalingHandles()
		{
			if (this.selected != null)
			{
				GUIStyle dragdot = SpriteUtilityWindow.s_Styles.dragdot;
				GUIStyle dragdotactive = SpriteUtilityWindow.s_Styles.dragdotactive;
				Color white = Color.white;
				Rect rect = new Rect(this.selected.m_Rect);
				float xMin = rect.xMin;
				float xMax = rect.xMax;
				float yMax = rect.yMax;
				float yMin = rect.yMin;
				EditorGUI.BeginChangeCheck();
				this.HandleBorderPointSlider(ref xMin, ref yMax, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
				this.HandleBorderPointSlider(ref xMax, ref yMax, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
				this.HandleBorderPointSlider(ref xMin, ref yMin, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
				this.HandleBorderPointSlider(ref xMax, ref yMin, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite");
					rect.xMin = xMin;
					rect.xMax = xMax;
					rect.yMax = yMax;
					rect.yMin = yMin;
					this.selected.m_Rect = this.ClampSpriteRect(rect);
					this.selected.m_Border = this.ClampSpriteBorder(this.selected.m_Border);
					this.textureIsDirty = true;
				}
				if (GUIUtility.hotControl == 0)
				{
					this.selected.m_Rect = this.FlipNegativeRect(this.selected.m_Rect);
					this.selected.m_Border = this.ClampSpriteBorder(this.selected.m_Border);
				}
			}
		}

		private void HandleRectSideScalingHandles()
		{
			if (this.selected != null)
			{
				Rect rect = new Rect(this.selected.m_Rect);
				float num = rect.xMin;
				float num2 = rect.xMax;
				float num3 = rect.yMax;
				float num4 = rect.yMin;
				Vector2 vector = Handles.matrix.MultiplyPoint(new Vector3(rect.xMin, rect.yMin));
				Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
				float width = Mathf.Abs(vector2.x - vector.x);
				float height = Mathf.Abs(vector2.y - vector.y);
				EditorGUI.BeginChangeCheck();
				num = this.HandleBorderScaleSlider(num, rect.yMax, width, height, true);
				num2 = this.HandleBorderScaleSlider(num2, rect.yMax, width, height, true);
				num3 = this.HandleBorderScaleSlider(rect.xMin, num3, width, height, false);
				num4 = this.HandleBorderScaleSlider(rect.xMin, num4, width, height, false);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite");
					rect.xMin = num;
					rect.xMax = num2;
					rect.yMax = num3;
					rect.yMin = num4;
					this.selected.m_Rect = this.ClampSpriteRect(rect);
					this.selected.m_Border = this.ClampSpriteBorder(this.selected.m_Border);
					this.textureIsDirty = true;
				}
			}
		}

		private void HandleBorderSidePointScalingSliders()
		{
			if (this.selected != null)
			{
				GUIStyle dragBorderdot = SpriteUtilityWindow.s_Styles.dragBorderdot;
				GUIStyle dragBorderDotActive = SpriteUtilityWindow.s_Styles.dragBorderDotActive;
				Color color = new Color(0f, 1f, 0f);
				Rect rect = this.selected.m_Rect;
				Vector4 border = this.selected.m_Border;
				float num = rect.xMin + border.x;
				float num2 = rect.xMax - border.z;
				float num3 = rect.yMax - border.w;
				float num4 = rect.yMin + border.y;
				EditorGUI.BeginChangeCheck();
				float num5 = num4 - (num4 - num3) / 2f;
				float num6 = num - (num - num2) / 2f;
				float num7 = num5;
				this.HandleBorderPointSlider(ref num, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
				num7 = num5;
				this.HandleBorderPointSlider(ref num2, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
				num7 = num6;
				this.HandleBorderPointSlider(ref num7, ref num3, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
				num7 = num6;
				this.HandleBorderPointSlider(ref num7, ref num4, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite border");
					border.x = num - rect.xMin;
					border.z = rect.xMax - num2;
					border.w = rect.yMax - num3;
					border.y = num4 - rect.yMin;
					this.textureIsDirty = true;
				}
				this.selected.m_Border = this.ClampSpriteBorder(border);
			}
		}

		private void HandleBorderCornerScalingHandles()
		{
			if (this.selected != null)
			{
				GUIStyle dragBorderdot = SpriteUtilityWindow.s_Styles.dragBorderdot;
				GUIStyle dragBorderDotActive = SpriteUtilityWindow.s_Styles.dragBorderDotActive;
				Color color = new Color(0f, 1f, 0f);
				Rect rect = new Rect(this.selected.m_Rect);
				Vector4 border = this.selected.m_Border;
				float num = rect.xMin + border.x;
				float num2 = rect.xMax - border.z;
				float num3 = rect.yMax - border.w;
				float num4 = rect.yMin + border.y;
				EditorGUI.BeginChangeCheck();
				this.HandleBorderPointSlider(ref num, ref num3, MouseCursor.ResizeUpLeft, border.x < 1f && border.w < 1f, dragBorderdot, dragBorderDotActive, color);
				this.HandleBorderPointSlider(ref num2, ref num3, MouseCursor.ResizeUpRight, border.z < 1f && border.w < 1f, dragBorderdot, dragBorderDotActive, color);
				this.HandleBorderPointSlider(ref num, ref num4, MouseCursor.ResizeUpRight, border.x < 1f && border.y < 1f, dragBorderdot, dragBorderDotActive, color);
				this.HandleBorderPointSlider(ref num2, ref num4, MouseCursor.ResizeUpLeft, border.z < 1f && border.y < 1f, dragBorderdot, dragBorderDotActive, color);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite border");
					border.x = num - rect.xMin;
					border.z = rect.xMax - num2;
					border.w = rect.yMax - num3;
					border.y = num4 - rect.yMin;
					this.textureIsDirty = true;
				}
				this.selected.m_Border = this.ClampSpriteBorder(border);
			}
		}

		private void HandleBorderSideScalingHandles()
		{
			if (this.selected != null)
			{
				Rect rect = new Rect(this.selected.m_Rect);
				Vector4 border = this.selected.m_Border;
				float num = rect.xMin + border.x;
				float num2 = rect.xMax - border.z;
				float num3 = rect.yMax - border.w;
				float num4 = rect.yMin + border.y;
				Vector2 vector = Handles.matrix.MultiplyPoint(new Vector3(rect.xMin, rect.yMin));
				Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
				float width = Mathf.Abs(vector2.x - vector.x);
				float height = Mathf.Abs(vector2.y - vector.y);
				EditorGUI.BeginChangeCheck();
				num = this.HandleBorderScaleSlider(num, rect.yMax, width, height, true);
				num2 = this.HandleBorderScaleSlider(num2, rect.yMax, width, height, true);
				num3 = this.HandleBorderScaleSlider(rect.xMin, num3, width, height, false);
				num4 = this.HandleBorderScaleSlider(rect.xMin, num4, width, height, false);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite border");
					border.x = num - rect.xMin;
					border.z = rect.xMax - num2;
					border.w = rect.yMax - num3;
					border.y = num4 - rect.yMin;
					this.selected.m_Border = this.ClampSpriteBorder(border);
					this.textureIsDirty = true;
				}
			}
		}

		private void HandleBorderPointSlider(ref float x, ref float y, MouseCursor mouseCursor, bool isHidden, GUIStyle dragDot, GUIStyle dragDotActive, Color color)
		{
			Color color2 = GUI.color;
			if (isHidden)
			{
				GUI.color = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				GUI.color = color;
			}
			Vector2 vector = SpriteEditorHandles.PointSlider(new Vector2(x, y), mouseCursor, dragDot, dragDotActive);
			x = vector.x;
			y = vector.y;
			GUI.color = color2;
		}

		private float HandleBorderScaleSlider(float x, float y, float width, float height, bool isHorizontal)
		{
			float fixedWidth = SpriteUtilityWindow.s_Styles.dragBorderdot.fixedWidth;
			Vector2 pos = Handles.matrix.MultiplyPoint(new Vector2(x, y));
			EditorGUI.BeginChangeCheck();
			float num;
			if (isHorizontal)
			{
				Rect cursorRect = new Rect(pos.x - fixedWidth * 0.5f, pos.y, fixedWidth, height);
				num = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeHorizontal, cursorRect).x;
			}
			else
			{
				Rect cursorRect2 = new Rect(pos.x, pos.y - fixedWidth * 0.5f, width, fixedWidth);
				num = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeVertical, cursorRect2).y;
			}
			float result;
			if (EditorGUI.EndChangeCheck())
			{
				result = num;
			}
			else
			{
				result = ((!isHorizontal) ? y : x);
			}
			return result;
		}

		public void DoAutomaticSlicing(int minimumSpriteSize, int alignment, Vector2 pivot, SpriteEditorWindow.AutoSlicingMethod slicingMethod)
		{
			Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Automatic Slicing");
			if (slicingMethod == SpriteEditorWindow.AutoSlicingMethod.DeleteAll)
			{
				this.m_RectsCache.ClearAll();
			}
			List<Rect> list = new List<Rect>(InternalSpriteUtility.GenerateAutomaticSpriteRectangles(this.m_Texture, minimumSpriteSize, 0));
			list = this.SortRects(list);
			foreach (Rect current in list)
			{
				this.AddSprite(current, alignment, pivot, slicingMethod);
			}
			this.selected = null;
			this.textureIsDirty = true;
			base.Repaint();
		}

		public void DoGridSlicing(Vector2 size, Vector2 offset, Vector2 padding, int alignment, Vector2 pivot)
		{
			Rect[] array = InternalSpriteUtility.GenerateGridSpriteRectangles(this.m_Texture, offset, size, padding);
			bool flag = true;
			if (array.Length > 1000)
			{
				if (!EditorUtility.DisplayDialog(SpriteEditorWindow.SpriteEditorWindowStyles.creatingMultipleSpriteDialogTitle.text, string.Format(SpriteEditorWindow.SpriteEditorWindowStyles.creatingMultipleSpriteDialogContent.text, array.Length), SpriteEditorWindow.SpriteEditorWindowStyles.okButtonLabel.text, SpriteEditorWindow.SpriteEditorWindowStyles.cancelButtonLabel.text))
				{
					flag = false;
				}
			}
			if (flag)
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Grid Slicing");
				this.m_RectsCache.ClearAll();
				Rect[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Rect rect = array2[i];
					this.AddSprite(rect, alignment, pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
				}
				this.selected = null;
				this.textureIsDirty = true;
			}
			base.Repaint();
		}

		public void GeneratePolygonOutline(int sides)
		{
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				SpriteRect spriteRect = this.m_RectsCache.RectAt(i);
				List<Vector2> list = new List<Vector2>();
				list.AddRange(UnityEditor.Sprites.SpriteUtility.GeneratePolygonOutlineVerticesOfSize(sides, (int)spriteRect.m_Rect.width, (int)spriteRect.m_Rect.height));
				spriteRect.m_Outline.Clear();
				spriteRect.m_Outline.Add(list);
				this.m_TextureIsDirty = true;
			}
			base.Repaint();
		}

		private List<Rect> SortRects(List<Rect> rects)
		{
			List<Rect> list = new List<Rect>();
			while (rects.Count > 0)
			{
				Rect rect = rects[rects.Count - 1];
				Rect sweepRect = new Rect(0f, rect.yMin, (float)this.m_Texture.width, rect.height);
				List<Rect> list2 = this.RectSweep(rects, sweepRect);
				if (list2.Count <= 0)
				{
					list.AddRange(rects);
					break;
				}
				list.AddRange(list2);
			}
			return list;
		}

		private List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
		{
			List<Rect> result;
			if (rects == null || rects.Count == 0)
			{
				result = new List<Rect>();
			}
			else
			{
				List<Rect> list = new List<Rect>();
				foreach (Rect current in rects)
				{
					if (this.Overlap(current, sweepRect))
					{
						list.Add(current);
					}
				}
				foreach (Rect current2 in list)
				{
					rects.Remove(current2);
				}
				list.Sort((Rect a, Rect b) => a.x.CompareTo(b.x));
				result = list;
			}
			return result;
		}

		private void AddSprite(Rect frame, int alignment, Vector2 pivot, SpriteEditorWindow.AutoSlicingMethod slicingMethod)
		{
			if (slicingMethod != SpriteEditorWindow.AutoSlicingMethod.DeleteAll)
			{
				SpriteRect existingOverlappingSprite = this.GetExistingOverlappingSprite(frame);
				if (existingOverlappingSprite != null)
				{
					if (slicingMethod == SpriteEditorWindow.AutoSlicingMethod.Smart)
					{
						existingOverlappingSprite.m_Rect = frame;
						existingOverlappingSprite.m_Alignment = (SpriteAlignment)alignment;
						existingOverlappingSprite.m_Pivot = pivot;
					}
				}
				else
				{
					this.AddSprite(frame, alignment, pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
				}
			}
			else
			{
				this.AddSprite(frame, alignment, pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
			}
		}

		private SpriteRect GetExistingOverlappingSprite(Rect rect)
		{
			SpriteRect result;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				Rect rect2 = this.m_RectsCache.RectAt(i).m_Rect;
				if (this.Overlap(rect2, rect))
				{
					result = this.m_RectsCache.RectAt(i);
					return result;
				}
			}
			result = null;
			return result;
		}

		private bool Overlap(Rect a, Rect b)
		{
			return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
		}

		private bool MouseOnTopOfInspector()
		{
			bool result;
			if (this.selected == null)
			{
				result = false;
			}
			else
			{
				Vector2 vector = GUIClip.Unclip(Event.current.mousePosition);
				vector += new Vector2(0f, -22f);
				result = this.inspectorRect.Contains(vector);
			}
			return result;
		}

		private bool PixelHasAlpha(int x, int y)
		{
			bool result;
			if (this.m_Texture == null)
			{
				result = false;
			}
			else
			{
				if (SpriteEditorWindow.s_AlphaPixelCache == null)
				{
					SpriteEditorWindow.s_AlphaPixelCache = new bool[this.m_Texture.width * this.m_Texture.height];
					Color32[] pixels = this.m_Texture.GetPixels32();
					for (int i = 0; i < pixels.Length; i++)
					{
						SpriteEditorWindow.s_AlphaPixelCache[i] = (pixels[i].a != 0);
					}
				}
				int num = y * this.m_Texture.width + x;
				result = SpriteEditorWindow.s_AlphaPixelCache[num];
			}
			return result;
		}

		private Texture2D CreateTemporaryDuplicate(Texture2D original, int width, int height)
		{
			Texture2D result;
			if (!ShaderUtil.hardwareSupportsRectRenderTexture || !original)
			{
				result = null;
			}
			else
			{
				RenderTexture active = RenderTexture.active;
				bool flag = !TextureUtil.GetLinearSampled(original);
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, (!flag) ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
				GL.sRGBWrite = (flag && QualitySettings.activeColorSpace == ColorSpace.Linear);
				Graphics.Blit(original, temporary);
				GL.sRGBWrite = false;
				RenderTexture.active = temporary;
				bool flag2 = width >= SystemInfo.maxTextureSize || height >= SystemInfo.maxTextureSize;
				Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, original.mipmapCount > 1 || flag2);
				texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
				texture2D.Apply();
				RenderTexture.ReleaseTemporary(temporary);
				EditorGUIUtility.SetRenderTextureNoViewport(active);
				texture2D.alphaIsTransparency = original.alphaIsTransparency;
				result = texture2D;
			}
			return result;
		}
	}
}
