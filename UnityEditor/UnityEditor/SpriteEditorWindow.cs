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
		public enum AutoSlicingMethod
		{
			DeleteAll,
			Smart,
			Safe
		}
		private const float maxSnapDistance = 14f;
		internal static PrefKey k_SpriteEditorTrim = new PrefKey("Sprite Editor/Trim", "#t");
		public static SpriteEditorWindow s_Instance;
		public bool m_ResetOnNextRepaint;
		public bool m_IgnoreNextPostprocessEvent;
		public Texture2D m_OriginalTexture;
		private SpriteRectCache m_RectsCache;
		private SerializedObject m_TextureImporterSO;
		private TextureImporter m_TextureImporter;
		private SerializedProperty m_TextureImporterSprites;
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
		private SpriteRect selected
		{
			get
			{
				return this.m_Selected;
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
				return new Rect(base.position.width - 330f - 8f - 16f, base.position.height - 148f - 8f - 16f, 330f, 148f);
			}
		}
		private bool multipleSprites
		{
			get
			{
				return this.m_TextureImporter != null && this.m_TextureImporter.spriteImportMode == SpriteImportMode.Multiple;
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
		public static void GetWindow()
		{
			EditorWindow.GetWindow<SpriteEditorWindow>();
		}
		public void RefreshPropertiesCache()
		{
			this.m_OriginalTexture = this.GetSelectedTexture2D();
			if (this.m_OriginalTexture == null)
			{
				return;
			}
			this.m_TextureImporter = (AssetImporter.GetAtPath(this.m_SelectedAssetPath) as TextureImporter);
			if (this.m_TextureImporter == null)
			{
				return;
			}
			this.m_TextureImporterSO = new SerializedObject(this.m_TextureImporter);
			this.m_TextureImporterSprites = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Sprites");
			if (this.m_RectsCache != null)
			{
				this.selected = ((this.m_TextureImporterSprites.arraySize <= 0) ? null : this.m_RectsCache.RectAt(0));
			}
			int width = 0;
			int height = 0;
			this.m_TextureImporter.GetWidthAndHeight(ref width, ref height);
			this.m_Texture = this.CreateTemporaryDuplicate(AssetDatabase.LoadMainAssetAtPath(this.m_TextureImporter.assetPath) as Texture2D, width, height);
			if (this.m_Texture == null)
			{
				return;
			}
			this.m_Texture.filterMode = FilterMode.Point;
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
		private void OnSelectionChange()
		{
			if (this.selectedTextureChanged)
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
			this.m_Zoom = -1f;
			this.RefreshPropertiesCache();
			this.RefreshRects();
			base.Repaint();
		}
		private void OnEnable()
		{
			base.minSize = new Vector2(360f, 200f);
			base.title = EditorGUIUtility.TextContent("SpriteEditorWindow.WindowTitle").text;
			SpriteEditorWindow.s_Instance = this;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
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
			this.HandleApplyRevertDialog();
			this.InvalidatePropertiesCache();
			SpriteEditorWindow.s_Instance = null;
		}
		private void HandleApplyRevertDialog()
		{
			if (this.textureIsDirty && this.m_TextureImporter != null)
			{
				if (EditorUtility.DisplayDialog("Unapplied import settings", "Unapplied import settings for '" + this.m_TextureImporter.assetPath + "'", "Apply", "Revert"))
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
			if (this.m_TextureImporterSprites == null)
			{
				return;
			}
			if (this.m_RectsCache)
			{
				this.m_RectsCache.ClearAll();
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
					this.m_RectsCache.AddRect(spriteRect);
				}
			}
			else
			{
				SpriteRect spriteRect2 = new SpriteRect();
				spriteRect2.m_Rect = new Rect(0f, 0f, (float)this.m_Texture.width, (float)this.m_Texture.height);
				spriteRect2.m_Name = this.m_OriginalTexture.name;
				spriteRect2.m_Alignment = (SpriteAlignment)this.m_TextureImporterSO.FindProperty("m_Alignment").intValue;
				spriteRect2.m_Border = this.m_TextureImporter.spriteBorder;
				spriteRect2.m_Pivot = SpriteEditorUtility.GetPivotValue(spriteRect2.m_Alignment, this.m_TextureImporter.spritePivot);
				this.m_RectsCache.AddRect(spriteRect2);
			}
			if (this.m_RectsCache.Count > 0)
			{
				this.selected = this.m_RectsCache.RectAt(0);
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
				EditorGUI.BeginDisabledGroup(true);
				GUILayout.Label(SpriteUtilityWindow.Styles.s_NoSelectionWarning, new GUILayoutOption[0]);
				EditorGUI.EndDisabledGroup();
				return;
			}
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
			this.DoSelectedFrameInspector();
			Handles.matrix = matrix;
		}
		protected override void DoTextureGUIExtras()
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
			if (this.multipleSprites)
			{
				this.HandleCreate();
				this.HandleDelete();
				this.HandleDuplicate();
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
			EditorGUI.BeginDisabledGroup(!this.multipleSprites);
			Rect buttonRect = EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button("Slice", "toolbarPopup", new GUILayoutOption[0]))
			{
				SpriteEditorMenu.s_SpriteEditor = this;
				if (SpriteEditorMenu.ShowAtPosition(buttonRect))
				{
					GUIUtility.ExitGUI();
				}
			}
			EditorGUI.BeginDisabledGroup(this.selected == null);
			if (GUILayout.Button(new GUIContent("Trim", "Trims selected rectangle (T)"), EditorStyles.toolbarButton, new GUILayoutOption[0]) || SpriteEditorWindow.k_SpriteEditorTrim.activated)
			{
				Rect rect = this.TrimAlpha(this.selected.m_Rect);
				rect = this.ClampSpriteRect(rect);
				this.selected.m_Rect = rect;
				base.Repaint();
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
		private void DoSelectedFrameInspector()
		{
			if (this.selected != null)
			{
				EditorGUIUtility.wideMode = true;
				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 135f;
				GUILayout.BeginArea(this.inspectorRect);
				GUILayout.BeginVertical(new GUIContent("Sprite"), GUI.skin.window, new GUILayoutOption[0]);
				EditorGUI.BeginDisabledGroup(!this.multipleSprites);
				this.DoNameField();
				this.DoPositionField();
				EditorGUI.EndDisabledGroup();
				Rect fieldLocation;
				this.DoBorderFields(out fieldLocation);
				this.DoPivotFields(fieldLocation);
				GUILayout.EndVertical();
				GUILayout.EndArea();
				EditorGUIUtility.labelWidth = labelWidth;
			}
		}
		private void DoPivotFields(Rect fieldLocation)
		{
			EditorGUI.BeginChangeCheck();
			this.selected.m_Alignment = (SpriteAlignment)EditorGUILayout.Popup(SpriteUtilityWindow.Styles.s_PivotLabel, (int)this.selected.m_Alignment, SpriteUtilityWindow.Styles.spriteAlignmentOptions, new GUILayoutOption[0]);
			Vector2 pivot = this.selected.m_Pivot;
			EditorGUI.BeginDisabledGroup(this.selected.m_Alignment != SpriteAlignment.Custom);
			fieldLocation.x = 5f;
			fieldLocation.y += 36f;
			fieldLocation.width = 414f;
			Vector2 customOffset = EditorGUI.Vector2Field(fieldLocation, "Custom Pivot", pivot);
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Pivot");
				this.textureIsDirty = true;
				this.selected.m_Pivot = SpriteEditorUtility.GetPivotValue(this.selected.m_Alignment, customOffset);
			}
		}
		private void DoBorderFields(out Rect fieldLocation)
		{
			EditorGUI.BeginChangeCheck();
			Rect rect = GUILayoutUtility.GetRect(322f, 322f, 32f, 32f);
			Vector4 vector = this.ClampSpriteBorder(this.selected.m_Border);
			Vector4 border = vector;
			Rect position = rect;
			position.width = EditorGUIUtility.labelWidth;
			position.height = 16f;
			GUI.Label(position, "Border");
			fieldLocation = rect;
			fieldLocation.width -= EditorGUIUtility.labelWidth;
			fieldLocation.height = 16f;
			fieldLocation.x += EditorGUIUtility.labelWidth;
			fieldLocation.width /= 2f;
			fieldLocation.width -= 2f;
			EditorGUIUtility.labelWidth = 12f;
			border.x = (float)EditorGUI.IntField(fieldLocation, "L", Mathf.RoundToInt(border.x));
			fieldLocation.x += fieldLocation.width + 3f;
			border.w = (float)EditorGUI.IntField(fieldLocation, "T", Mathf.RoundToInt(border.w));
			fieldLocation.y += 16f;
			fieldLocation.x -= fieldLocation.width + 3f;
			border.z = (float)EditorGUI.IntField(fieldLocation, "R", Mathf.RoundToInt(border.z));
			fieldLocation.x += fieldLocation.width + 3f;
			border.y = (float)EditorGUI.IntField(fieldLocation, "B", Mathf.RoundToInt(border.y));
			EditorGUIUtility.labelWidth = 135f;
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
			Rect rect2 = EditorGUILayout.RectField("Position", this.FlipNegativeRect(rect), new GUILayoutOption[0]);
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
			string text = EditorGUILayout.TextField("Name", name, new GUILayoutOption[0]);
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
			EditorGUI.BeginDisabledGroup(!this.textureIsDirty);
			if (GUILayout.Button("Revert", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.DoRevert();
			}
			if (GUILayout.Button("Apply", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.DoApply();
			}
			EditorGUI.EndDisabledGroup();
		}
		private void DoApply()
		{
			if (this.multipleSprites)
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				this.m_TextureImporterSprites.ClearArray();
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
					this.m_TextureImporterSprites.InsertArrayElementAtIndex(i);
					SerializedProperty arrayElementAtIndex = this.m_TextureImporterSprites.GetArrayElementAtIndex(i);
					arrayElementAtIndex.FindPropertyRelative("m_Rect").rectValue = spriteRect.m_Rect;
					arrayElementAtIndex.FindPropertyRelative("m_Border").vector4Value = spriteRect.m_Border;
					arrayElementAtIndex.FindPropertyRelative("m_Name").stringValue = spriteRect.m_Name;
					arrayElementAtIndex.FindPropertyRelative("m_Alignment").intValue = (int)spriteRect.m_Alignment;
					arrayElementAtIndex.FindPropertyRelative("m_Pivot").vector2Value = spriteRect.m_Pivot;
				}
				if (list.Count > 0)
				{
					PatchImportSettingRecycleID.PatchMultiple(this.m_TextureImporterSO, 213, list.ToArray(), list2.ToArray());
				}
				this.m_TextureImporterSO.ApplyModifiedProperties();
			}
			else
			{
				if (this.m_RectsCache.Count > 0)
				{
					SpriteRect spriteRect2 = this.m_RectsCache.RectAt(0);
					this.m_TextureImporterSO.FindProperty("m_Alignment").intValue = (int)spriteRect2.m_Alignment;
					this.m_TextureImporterSO.ApplyModifiedProperties();
					this.m_TextureImporter.spriteBorder = spriteRect2.m_Border;
					this.m_TextureImporter.spritePivot = spriteRect2.m_Pivot;
				}
			}
			this.m_IgnoreNextPostprocessEvent = true;
			this.DoTextureReimport(this.m_TextureImporter.assetPath);
			this.textureIsDirty = false;
		}
		private void DoRevert()
		{
			this.m_TextureIsDirty = false;
			this.selected = null;
			this.RefreshRects();
		}
		private void HandleDuplicate()
		{
			if (Event.current.commandName == "Duplicate" && (Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand))
			{
				if (Event.current.type == EventType.ExecuteCommand)
				{
					Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Duplicate sprite");
					this.selected = this.AddSprite(this.selected.m_Rect, (int)this.selected.m_Alignment, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
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
					this.selected = this.AddSprite(rect, 0, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
					GUIUtility.keyboardControl = 0;
				}
			}
		}
		private void HandleDelete()
		{
			if ((Event.current.commandName == "SoftDelete" || Event.current.commandName == "Delete") && (Event.current.type == EventType.ValidateCommand || Event.current.type == EventType.ExecuteCommand))
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
					this.selected.m_Pivot = this.SnapPivot(pivot);
					this.textureIsDirty = true;
				}
			}
		}
		private Rect ClampSpriteRect(Rect rect)
		{
			return SpriteEditorUtility.RoundedRect(new Rect
			{
				xMin = Mathf.Clamp(rect.xMin, 0f, (float)(this.m_Texture.width - 1)),
				yMin = Mathf.Clamp(rect.yMin, 0f, (float)(this.m_Texture.height - 1)),
				xMax = Mathf.Clamp(rect.xMax, 1f, (float)this.m_Texture.width),
				yMax = Mathf.Clamp(rect.yMax, 1f, (float)this.m_Texture.height)
			});
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
			bool flag = false;
			for (int i = 0; i < snapPointsArray.Length; i++)
			{
				if ((vector - snapPointsArray[i]).magnitude * this.m_Zoom < 14f)
				{
					vector = snapPointsArray[i];
					this.selected.m_Alignment = (SpriteAlignment)i;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.selected.m_Alignment = SpriteAlignment.Custom;
			}
			return this.ConvertFromTextureToNormalizedSpace(vector, rect);
		}
		public Vector2 ApplySpriteAlignmentToPivot(Vector2 pivot, Rect rect, SpriteAlignment alignment)
		{
			Vector2[] snapPointsArray = this.GetSnapPointsArray(rect);
			if (alignment != SpriteAlignment.Custom)
			{
				Vector2 texturePos = snapPointsArray[(int)alignment];
				return this.ConvertFromTextureToNormalizedSpace(texturePos, rect);
			}
			return pivot;
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
			else
			{
				if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SpriteRenderer>())
				{
					Sprite sprite = Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite;
					this.SelectSpriteIndex(sprite);
				}
			}
		}
		private void SelectSpriteIndex(Sprite sprite)
		{
			if (sprite == null)
			{
				return;
			}
			this.selected = null;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				if (sprite.rect == this.m_RectsCache.RectAt(i).m_Rect)
				{
					this.selected = this.m_RectsCache.RectAt(i);
					return;
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
			else
			{
				if (Selection.activeObject is Sprite)
				{
					texture2D = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(Selection.activeObject as Sprite, false);
				}
				else
				{
					if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<SpriteRenderer>() && Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite)
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
			SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
			for (int k = 0; k < this.m_RectsCache.Count; k++)
			{
				SpriteRect spriteRect = this.m_RectsCache.RectAt(k);
				if (this.ShouldDrawBorders(spriteRect))
				{
					Vector4 border = spriteRect.m_Border;
					Rect rect2 = spriteRect.m_Rect;
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
			SpriteRect result = null;
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				if (this.m_RectsCache.RectAt(i).m_Rect.Contains(Handles.s_InverseMatrix.MultiplyPoint(mousePosition)))
				{
					if (this.m_RectsCache.RectAt(i) == this.selected)
					{
						return this.m_RectsCache.RectAt(i);
					}
					float width = this.m_RectsCache.RectAt(i).m_Rect.width;
					float height = this.m_RectsCache.RectAt(i).m_Rect.height;
					float num2 = width * height;
					if (width > 0f && height > 0f && num2 < num)
					{
						result = this.m_RectsCache.RectAt(i);
						num = num2;
					}
				}
			}
			return result;
		}
		public SpriteRect AddSprite(Rect rect, int alignment, int colliderAlphaCutoff, float colliderDetail)
		{
			SpriteRect spriteRect = new SpriteRect();
			spriteRect.m_Rect = rect;
			spriteRect.m_Alignment = (SpriteAlignment)alignment;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.m_TextureImporter.assetPath);
			spriteRect.m_Name = this.GetUniqueName(fileNameWithoutExtension);
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
			return new Rect((float)num, (float)num3, (float)(num2 - num + 1), (float)(num4 - num3 + 1));
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
			if (this.selected == null)
			{
				return;
			}
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
		private void HandleRectSideScalingHandles()
		{
			if (this.selected == null)
			{
				return;
			}
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
		private void HandleBorderSidePointScalingSliders()
		{
			if (this.selected == null)
			{
				return;
			}
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
		private void HandleBorderCornerScalingHandles()
		{
			if (this.selected == null)
			{
				return;
			}
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
		private void HandleBorderSideScalingHandles()
		{
			if (this.selected == null)
			{
				return;
			}
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
			float result;
			if (isHorizontal)
			{
				Rect cursorRect = new Rect(pos.x - fixedWidth * 0.5f, pos.y, fixedWidth, height);
				result = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeHorizontal, cursorRect).x;
			}
			else
			{
				Rect cursorRect2 = new Rect(pos.x, pos.y - fixedWidth * 0.5f, width, fixedWidth);
				result = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeVertical, cursorRect2).y;
			}
			if (EditorGUI.EndChangeCheck())
			{
				return result;
			}
			return (!isHorizontal) ? y : x;
		}
		public void DoAutomaticSlicing(int minimumSpriteSize, int alignment, SpriteEditorWindow.AutoSlicingMethod slicingMethod)
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
				this.AddSprite(current, alignment, slicingMethod);
			}
			this.selected = null;
			this.textureIsDirty = true;
			base.Repaint();
		}
		public void DoGridSlicing(Vector2 size, Vector2 offset, Vector2 padding, int alignment)
		{
			Rect[] array = InternalSpriteUtility.GenerateGridSpriteRectangles(this.m_Texture, offset, size, padding);
			bool flag = true;
			if (array.Length > 1000 && !EditorUtility.DisplayDialog("Creating multiple sprites", "Creating " + array.Length + " sprites. \nThis can take up to several minutes.", "Ok", "Cancel"))
			{
				flag = false;
			}
			if (flag)
			{
				Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Grid Slicing");
				this.m_RectsCache.ClearAll();
				Rect[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Rect rect = array2[i];
					this.AddSprite(rect, alignment, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
				}
				this.selected = null;
				this.textureIsDirty = true;
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
			if (rects == null || rects.Count == 0)
			{
				return new List<Rect>();
			}
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
			return list;
		}
		private void AddSprite(Rect frame, int alignment, SpriteEditorWindow.AutoSlicingMethod slicingMethod)
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
					}
				}
				else
				{
					this.AddSprite(frame, alignment, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
				}
			}
			else
			{
				this.AddSprite(frame, alignment, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
			}
		}
		private SpriteRect GetExistingOverlappingSprite(Rect rect)
		{
			for (int i = 0; i < this.m_RectsCache.Count; i++)
			{
				Rect rect2 = this.m_RectsCache.RectAt(i).m_Rect;
				if (this.Overlap(rect2, rect))
				{
					return this.m_RectsCache.RectAt(i);
				}
			}
			return null;
		}
		private bool Overlap(Rect a, Rect b)
		{
			return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
		}
		private bool MouseOnTopOfInspector()
		{
			if (this.selected == null)
			{
				return false;
			}
			Vector2 vector = GUIClip.Unclip(Event.current.mousePosition);
			vector += new Vector2(0f, -22f);
			return this.inspectorRect.Contains(vector);
		}
		private bool PixelHasAlpha(int x, int y)
		{
			if (this.m_Texture == null)
			{
				return false;
			}
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
			return SpriteEditorWindow.s_AlphaPixelCache[num];
		}
		private Texture2D CreateTemporaryDuplicate(Texture2D original, int width, int height)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture || !original)
			{
				return null;
			}
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
			RenderTexture active = RenderTexture.active;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
			Graphics.Blit(original, temporary);
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, original.mipmapCount > 1);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
			texture2D.Apply();
			RenderTexture.ReleaseTemporary(temporary);
			EditorGUIUtility.SetRenderTextureNoViewport(active);
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(false);
			texture2D.alphaIsTransparency = original.alphaIsTransparency;
			return texture2D;
		}
	}
}
