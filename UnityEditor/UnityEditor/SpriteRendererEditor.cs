using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SpriteRenderer))]
	internal class SpriteRendererEditor : RendererEditorBase
	{
		private static class Contents
		{
			public static readonly GUIContent flipLabel = EditorGUIUtility.TextContent("Flip|Sprite flipping");

			public static readonly int flipToggleHash = "FlipToggleHash".GetHashCode();

			public static readonly GUIContent fullTileLabel = EditorGUIUtility.TextContent("Tile Mode|Specify the 9 slice tiling behaviour");

			public static readonly GUIContent fullTileThresholdLabel = EditorGUIUtility.TextContent("Stretch Value|This value defines how much the center portion will stretch before it tiles.");

			public static readonly GUIContent drawModeLabel = EditorGUIUtility.TextContent("Draw Mode|Specify the draw mode for the sprite");

			public static readonly GUIContent widthLabel = EditorGUIUtility.TextContent("Width|The width dimension value for the sprite");

			public static readonly GUIContent heightLabel = EditorGUIUtility.TextContent("Height|The height dimension value for the sprite");

			public static readonly GUIContent sizeLabel = EditorGUIUtility.TextContent("Size|The rendering dimension for the sprite");

			public static readonly GUIContent notFullRectWarningLabel = EditorGUIUtility.TextContent("Sprite Tiling might not appear correctly because the Sprite used is not generated with Full Rect. To fix this, change the Mesh Type in the Sprite's import setting to Full Rect");

			public static readonly GUIContent notFullRectMultiEditWarningLabel = EditorGUIUtility.TextContent("Sprite Tiling might not appear correctly because some of the Sprites used are not generated with Full Rect. To fix this, change the Mesh Type in the Sprite's import setting to Full Rect");

			public static readonly int sizeFieldHash = "SpriteRendererSizeField".GetHashCode();

			public static readonly GUIContent materialLabel = EditorGUIUtility.TextContent("Material|Material to be used by SpriteRenderer");

			public static readonly GUIContent spriteLabel = EditorGUIUtility.TextContent("Sprite|The Sprite to render");

			public static readonly GUIContent colorLabel = EditorGUIUtility.TextContent("Color|Rendering color for the Sprite graphic");

			public static readonly Texture2D warningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
		}

		private SerializedProperty m_Sprite;

		private SerializedProperty m_Color;

		private SerializedProperty m_Material;

		private SerializedProperty m_FlipX;

		private SerializedProperty m_FlipY;

		private SerializedProperty m_DrawMode;

		private SerializedProperty m_SpriteTileMode;

		private SerializedProperty m_AdaptiveModeThreshold;

		private SerializedProperty m_Size;

		private AnimBool m_ShowDrawMode;

		private AnimBool m_ShowTileMode;

		private AnimBool m_ShowAdaptiveThreshold;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Sprite = base.serializedObject.FindProperty("m_Sprite");
			this.m_Color = base.serializedObject.FindProperty("m_Color");
			this.m_FlipX = base.serializedObject.FindProperty("m_FlipX");
			this.m_FlipY = base.serializedObject.FindProperty("m_FlipY");
			this.m_Material = base.serializedObject.FindProperty("m_Materials.Array");
			this.m_DrawMode = base.serializedObject.FindProperty("m_DrawMode");
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_SpriteTileMode = base.serializedObject.FindProperty("m_SpriteTileMode");
			this.m_AdaptiveModeThreshold = base.serializedObject.FindProperty("m_AdaptiveModeThreshold");
			this.m_ShowDrawMode = new AnimBool(this.ShouldShowDrawMode());
			this.m_ShowTileMode = new AnimBool(this.ShouldShowTileMode());
			this.m_ShowAdaptiveThreshold = new AnimBool(this.ShouldShowAdaptiveThreshold());
			this.m_ShowDrawMode.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowTileMode.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowAdaptiveThreshold.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Sprite, SpriteRendererEditor.Contents.spriteLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Color, SpriteRendererEditor.Contents.colorLabel, true, new GUILayoutOption[0]);
			this.FlipToggles();
			if (this.m_Material.arraySize == 0)
			{
				this.m_Material.InsertArrayElementAtIndex(0);
			}
			Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, 16f, 16f);
			EditorGUI.showMixedValue = this.m_Material.hasMultipleDifferentValues;
			UnityEngine.Object objectReferenceValue = this.m_Material.GetArrayElementAtIndex(0).objectReferenceValue;
			UnityEngine.Object @object = EditorGUI.ObjectField(rect, SpriteRendererEditor.Contents.materialLabel, objectReferenceValue, typeof(Material), false);
			if (@object != objectReferenceValue)
			{
				this.m_Material.GetArrayElementAtIndex(0).objectReferenceValue = @object;
			}
			EditorGUI.showMixedValue = false;
			EditorGUILayout.PropertyField(this.m_DrawMode, SpriteRendererEditor.Contents.drawModeLabel, new GUILayoutOption[0]);
			this.m_ShowDrawMode.target = this.ShouldShowDrawMode();
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowDrawMode.faded))
			{
				string spriteNotFullRectWarning = this.GetSpriteNotFullRectWarning();
				if (spriteNotFullRectWarning != null)
				{
					EditorGUILayout.HelpBox(spriteNotFullRectWarning, MessageType.Warning);
				}
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUILayout.PrefixLabel(SpriteRendererEditor.Contents.sizeLabel);
				EditorGUI.showMixedValue = this.m_Size.hasMultipleDifferentValues;
				this.FloatFieldLabelAbove(SpriteRendererEditor.Contents.widthLabel, this.m_Size.FindPropertyRelative("x"));
				this.FloatFieldLabelAbove(SpriteRendererEditor.Contents.heightLabel, this.m_Size.FindPropertyRelative("y"));
				EditorGUI.showMixedValue = false;
				EditorGUILayout.EndHorizontal();
				this.m_ShowTileMode.target = this.ShouldShowTileMode();
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowTileMode.faded))
				{
					EditorGUILayout.PropertyField(this.m_SpriteTileMode, SpriteRendererEditor.Contents.fullTileLabel, new GUILayoutOption[0]);
					this.m_ShowAdaptiveThreshold.target = this.ShouldShowAdaptiveThreshold();
					if (EditorGUILayout.BeginFadeGroup(this.m_ShowAdaptiveThreshold.faded))
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.Slider(this.m_AdaptiveModeThreshold, 0f, 1f, SpriteRendererEditor.Contents.fullTileThresholdLabel, new GUILayoutOption[0]);
						EditorGUI.indentLevel--;
					}
					EditorGUILayout.EndFadeGroup();
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			base.RenderSortingLayerFields();
			this.CheckForErrors();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void FloatFieldLabelAbove(GUIContent contentLabel, SerializedProperty sp)
		{
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(contentLabel, EditorStyles.label);
			GUIContent label = EditorGUI.BeginProperty(rect, contentLabel, sp);
			int controlID = GUIUtility.GetControlID(SpriteRendererEditor.Contents.sizeFieldHash, FocusType.Keyboard, rect);
			EditorGUI.HandlePrefixLabel(rect, rect, label, controlID);
			Rect rect2 = GUILayoutUtility.GetRect(contentLabel, EditorStyles.textField);
			EditorGUI.BeginChangeCheck();
			float floatValue = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect2, rect, controlID, sp.floatValue, EditorGUI.kFloatFieldFormatString, EditorStyles.textField, true);
			if (EditorGUI.EndChangeCheck())
			{
				sp.floatValue = floatValue;
			}
			EditorGUI.EndProperty();
			EditorGUILayout.EndVertical();
		}

		private string GetSpriteNotFullRectWarning()
		{
			UnityEngine.Object[] targets = base.targets;
			string result;
			for (int i = 0; i < targets.Length; i++)
			{
				UnityEngine.Object @object = targets[i];
				if (!(@object as SpriteRenderer).shouldSupportTiling)
				{
					result = ((base.targets.Length != 1) ? SpriteRendererEditor.Contents.notFullRectMultiEditWarningLabel.text : SpriteRendererEditor.Contents.notFullRectWarningLabel.text);
					return result;
				}
			}
			result = null;
			return result;
		}

		private bool ShouldShowDrawMode()
		{
			return this.m_DrawMode.intValue != 0 && !this.m_DrawMode.hasMultipleDifferentValues;
		}

		private bool ShouldShowAdaptiveThreshold()
		{
			return this.m_SpriteTileMode.intValue == 1 && !this.m_SpriteTileMode.hasMultipleDifferentValues;
		}

		private bool ShouldShowTileMode()
		{
			return this.m_DrawMode.intValue == 2 && !this.m_DrawMode.hasMultipleDifferentValues;
		}

		private void FlipToggles()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
			int controlID = GUIUtility.GetControlID(SpriteRendererEditor.Contents.flipToggleHash, FocusType.Keyboard, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, SpriteRendererEditor.Contents.flipLabel);
			rect.width = 30f;
			this.FlipToggle(rect, "X", this.m_FlipX);
			rect.x += 30f;
			this.FlipToggle(rect, "Y", this.m_FlipY);
			GUILayout.EndHorizontal();
		}

		private void FlipToggle(Rect r, string label, SerializedProperty property)
		{
			bool flag = property.boolValue;
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			EditorGUI.BeginChangeCheck();
			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			flag = EditorGUI.ToggleLeft(r, label, flag);
			EditorGUI.indentLevel = indentLevel;
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(base.targets, "Edit Constraints");
				property.boolValue = flag;
			}
			EditorGUI.showMixedValue = false;
		}

		private void CheckForErrors()
		{
			if (this.IsMaterialTextureAtlasConflict())
			{
				SpriteRendererEditor.ShowError("Material has CanUseSpriteAtlas=False tag. Sprite texture has atlasHint set. Rendering artifacts possible.");
			}
			bool flag;
			if (!this.DoesMaterialHaveSpriteTexture(out flag))
			{
				SpriteRendererEditor.ShowError("Material does not have a _MainTex texture property. It is required for SpriteRenderer.");
			}
			else if (flag)
			{
				SpriteRendererEditor.ShowError("Material texture property _MainTex has offset/scale set. It is incompatible with SpriteRenderer.");
			}
		}

		private bool IsMaterialTextureAtlasConflict()
		{
			Material sharedMaterial = (base.target as SpriteRenderer).sharedMaterial;
			bool result;
			if (sharedMaterial == null)
			{
				result = false;
			}
			else
			{
				string tag = sharedMaterial.GetTag("CanUseSpriteAtlas", false);
				if (tag.ToLower() == "false")
				{
					Sprite assetObject = this.m_Sprite.objectReferenceValue as Sprite;
					TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(assetObject)) as TextureImporter;
					if (textureImporter != null && textureImporter.spritePackingTag != null && textureImporter.spritePackingTag.Length > 0)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private bool DoesMaterialHaveSpriteTexture(out bool tiled)
		{
			tiled = false;
			Material sharedMaterial = (base.target as SpriteRenderer).sharedMaterial;
			bool result;
			if (sharedMaterial == null)
			{
				result = true;
			}
			else
			{
				bool flag = sharedMaterial.HasProperty("_MainTex");
				if (flag)
				{
					Vector2 textureOffset = sharedMaterial.GetTextureOffset("_MainTex");
					Vector2 textureScale = sharedMaterial.GetTextureScale("_MainTex");
					if (textureOffset.x != 0f || textureOffset.y != 0f || textureScale.x != 1f || textureScale.y != 1f)
					{
						tiled = true;
					}
				}
				result = sharedMaterial.HasProperty("_MainTex");
			}
			return result;
		}

		private static void ShowError(string error)
		{
			GUIContent content = new GUIContent(error)
			{
				image = SpriteRendererEditor.Contents.warningIcon
			};
			GUILayout.Space(5f);
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}
	}
}
