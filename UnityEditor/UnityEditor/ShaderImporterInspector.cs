using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CustomEditor(typeof(ShaderImporter))]
	internal class ShaderImporterInspector : AssetImporterInspector
	{
		private List<string> propertyNames = new List<string>();

		private List<string> displayNames = new List<string>();

		private List<Texture> textures = new List<Texture>();

		private List<TextureDimension> dimensions = new List<TextureDimension>();

		internal override void OnHeaderControlsGUI()
		{
			Shader target = this.assetEditor.target as Shader;
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open...", EditorStyles.miniButton, new GUILayoutOption[0]))
			{
				AssetDatabase.OpenAsset(target);
				GUIUtility.ExitGUI();
			}
		}

		public void OnEnable()
		{
			this.ResetValues();
		}

		private void ShowDefaultTextures()
		{
			if (this.propertyNames.Count == 0)
			{
				return;
			}
			EditorGUILayout.LabelField("Default Maps", EditorStyles.boldLabel, new GUILayoutOption[0]);
			for (int i = 0; i < this.propertyNames.Count; i++)
			{
				Texture obj = this.textures[i];
				Texture value = null;
				EditorGUI.BeginChangeCheck();
				Type textureTypeFromDimension = MaterialEditor.GetTextureTypeFromDimension(this.dimensions[i]);
				if (textureTypeFromDimension != null)
				{
					string t = (!string.IsNullOrEmpty(this.displayNames[i])) ? this.displayNames[i] : ObjectNames.NicifyVariableName(this.propertyNames[i]);
					value = (EditorGUILayout.MiniThumbnailObjectField(GUIContent.Temp(t), obj, textureTypeFromDimension, null, new GUILayoutOption[0]) as Texture);
				}
				if (EditorGUI.EndChangeCheck())
				{
					this.textures[i] = value;
				}
			}
		}

		internal override bool HasModified()
		{
			if (base.HasModified())
			{
				return true;
			}
			ShaderImporter shaderImporter = this.target as ShaderImporter;
			if (shaderImporter == null)
			{
				return false;
			}
			Shader shader = shaderImporter.GetShader();
			if (shader == null)
			{
				return false;
			}
			int propertyCount = ShaderUtil.GetPropertyCount(shader);
			for (int i = 0; i < propertyCount; i++)
			{
				string propertyName = ShaderUtil.GetPropertyName(shader, i);
				for (int j = 0; j < this.propertyNames.Count; j++)
				{
					if (this.propertyNames[j] == propertyName && this.textures[j] != shaderImporter.GetDefaultTexture(propertyName))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal override void ResetValues()
		{
			base.ResetValues();
			this.propertyNames = new List<string>();
			this.displayNames = new List<string>();
			this.textures = new List<Texture>();
			this.dimensions = new List<TextureDimension>();
			ShaderImporter shaderImporter = this.target as ShaderImporter;
			if (shaderImporter == null)
			{
				return;
			}
			Shader shader = shaderImporter.GetShader();
			if (shader == null)
			{
				return;
			}
			int propertyCount = ShaderUtil.GetPropertyCount(shader);
			for (int i = 0; i < propertyCount; i++)
			{
				if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
				{
					string propertyName = ShaderUtil.GetPropertyName(shader, i);
					string propertyDescription = ShaderUtil.GetPropertyDescription(shader, i);
					Texture defaultTexture = shaderImporter.GetDefaultTexture(propertyName);
					this.propertyNames.Add(propertyName);
					this.displayNames.Add(propertyDescription);
					this.textures.Add(defaultTexture);
					this.dimensions.Add(ShaderUtil.GetTexDim(shader, i));
				}
			}
		}

		internal override void Apply()
		{
			base.Apply();
			ShaderImporter shaderImporter = this.target as ShaderImporter;
			if (shaderImporter == null)
			{
				return;
			}
			shaderImporter.SetDefaultTextures(this.propertyNames.ToArray(), this.textures.ToArray());
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(shaderImporter));
		}

		private static int GetNumberOfTextures(Shader shader)
		{
			int num = 0;
			int propertyCount = ShaderUtil.GetPropertyCount(shader);
			for (int i = 0; i < propertyCount; i++)
			{
				if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
				{
					num++;
				}
			}
			return num;
		}

		public override void OnInspectorGUI()
		{
			ShaderImporter shaderImporter = this.target as ShaderImporter;
			if (shaderImporter == null)
			{
				return;
			}
			Shader shader = shaderImporter.GetShader();
			if (shader == null)
			{
				return;
			}
			if (ShaderImporterInspector.GetNumberOfTextures(shader) != this.propertyNames.Count)
			{
				this.ResetValues();
			}
			this.ShowDefaultTextures();
			base.ApplyRevertGUI();
		}
	}
}
