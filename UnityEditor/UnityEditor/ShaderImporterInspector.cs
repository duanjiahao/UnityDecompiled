using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(ShaderImporter))]
	internal class ShaderImporterInspector : AssetImporterInspector
	{
		private List<string> names = new List<string>();
		private List<Texture> textures = new List<Texture>();
		private List<ShaderUtil.ShaderPropertyTexDim> dimensions = new List<ShaderUtil.ShaderPropertyTexDim>();
		internal override void OnHeaderControlsGUI()
		{
			Shader target = base.assetEditor.target as Shader;
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
		private void ShowDefaultTextures(Shader shader, ShaderImporter importer)
		{
			for (int i = 0; i < this.names.Count; i++)
			{
				string name = this.names[i];
				string label = ObjectNames.NicifyVariableName(name);
				Texture obj = this.textures[i];
				Texture value = null;
				EditorGUI.BeginChangeCheck();
				switch (this.dimensions[i])
				{
				case ShaderUtil.ShaderPropertyTexDim.TexDim2D:
					value = (EditorGUILayout.ObjectField(label, obj, typeof(Texture2D), false, new GUILayoutOption[0]) as Texture2D);
					break;
				case ShaderUtil.ShaderPropertyTexDim.TexDimCUBE:
					value = (EditorGUILayout.ObjectField(label, obj, typeof(Cubemap), false, new GUILayoutOption[0]) as Cubemap);
					break;
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
				for (int j = 0; j < this.names.Count; j++)
				{
					if (this.names[j] == propertyName && this.textures[j] != shaderImporter.GetDefaultTexture(propertyName))
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
			this.names = new List<string>();
			this.textures = new List<Texture>();
			this.dimensions = new List<ShaderUtil.ShaderPropertyTexDim>();
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
					Texture defaultTexture = shaderImporter.GetDefaultTexture(propertyName);
					this.names.Add(propertyName);
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
			shaderImporter.SetDefaultTextures(this.names.ToArray(), this.textures.ToArray());
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
			if (ShaderImporterInspector.GetNumberOfTextures(shader) != this.names.Count)
			{
				this.ResetValues();
			}
			this.ShowDefaultTextures(shader, shaderImporter);
			base.ApplyRevertGUI();
		}
	}
}
