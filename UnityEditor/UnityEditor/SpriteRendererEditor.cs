using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SpriteRenderer))]
	internal class SpriteRendererEditor : RendererEditorBase
	{
		private SerializedProperty m_Sprite;
		private SerializedProperty m_Color;
		private SerializedProperty m_Material;
		private static Texture2D s_WarningIcon;
		private GUIContent m_MaterialStyle = EditorGUIUtility.TextContent("SpriteRenderer.Material");
		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Sprite = base.serializedObject.FindProperty("m_Sprite");
			this.m_Color = base.serializedObject.FindProperty("m_Color");
			this.m_Material = base.serializedObject.FindProperty("m_Materials.Array");
			EditorUtility.SetSelectedWireframeHidden(this.target as SpriteRenderer, true);
		}
		public void OnDisable()
		{
			EditorUtility.SetSelectedWireframeHidden(this.target as SpriteRenderer, false);
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Sprite, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Color, true, new GUILayoutOption[0]);
			if (this.m_Material.arraySize == 0)
			{
				this.m_Material.InsertArrayElementAtIndex(0);
			}
			EditorGUILayout.PropertyField(this.m_Material.GetArrayElementAtIndex(0), this.m_MaterialStyle, true, new GUILayoutOption[0]);
			base.RenderSortingLayerFields();
			this.CheckForErrors();
			base.serializedObject.ApplyModifiedProperties();
		}
		private void CheckForErrors()
		{
			if (this.IsMaterialUsingFixedFunction())
			{
				SpriteRendererEditor.ShowError("Material uses fixed function shader. It is not compatible with SpriteRenderer.");
			}
			if (this.IsMaterialTextureAtlasConflict())
			{
				SpriteRendererEditor.ShowError("Material has CanUseSpriteAtlas=False tag. Sprite texture has atlasHint set. Rendering artifacts possible.");
			}
			bool flag;
			if (!this.DoesMaterialHaveSpriteTexture(out flag))
			{
				SpriteRendererEditor.ShowError("Material does not have a _MainTex texture property. It is required for SpriteRenderer.");
			}
			else
			{
				if (flag)
				{
					SpriteRendererEditor.ShowError("Material texture property _MainTex has offset/scale set. It is incompatible with SpriteRenderer.");
				}
			}
		}
		private bool IsMaterialUsingFixedFunction()
		{
			Material sharedMaterial = (this.target as SpriteRenderer).sharedMaterial;
			return !(sharedMaterial == null) && ShaderUtil.DoesShaderContainFixedFunctionPasses(sharedMaterial.shader);
		}
		private bool IsMaterialTextureAtlasConflict()
		{
			Material sharedMaterial = (this.target as SpriteRenderer).sharedMaterial;
			if (sharedMaterial == null)
			{
				return false;
			}
			string tag = sharedMaterial.GetTag("CanUseSpriteAtlas", false);
			if (tag.ToLower() == "false")
			{
				Sprite assetObject = this.m_Sprite.objectReferenceValue as Sprite;
				TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(assetObject)) as TextureImporter;
				if (textureImporter.spritePackingTag != null && textureImporter.spritePackingTag.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
		private bool DoesMaterialHaveSpriteTexture(out bool tiled)
		{
			tiled = false;
			Material sharedMaterial = (this.target as SpriteRenderer).sharedMaterial;
			if (sharedMaterial == null)
			{
				return true;
			}
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
			return sharedMaterial.HasProperty("_MainTex");
		}
		private static void ShowError(string error)
		{
			if (SpriteRendererEditor.s_WarningIcon == null)
			{
				SpriteRendererEditor.s_WarningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
			}
			GUIContent content = new GUIContent(error)
			{
				image = SpriteRendererEditor.s_WarningIcon
			};
			GUILayout.Space(5f);
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}
	}
}
