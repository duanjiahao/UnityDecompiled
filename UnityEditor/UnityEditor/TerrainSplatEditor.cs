using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TerrainSplatEditor : EditorWindow
	{
		private string m_ButtonTitle = string.Empty;

		private Vector2 m_ScrollPosition;

		private Terrain m_Terrain;

		private int m_Index = -1;

		public Texture2D m_Texture;

		public Texture2D m_NormalMap;

		private Vector2 m_TileSize;

		private Vector2 m_TileOffset;

		private Color m_Specular;

		private float m_Metallic;

		private float m_Smoothness;

		public TerrainSplatEditor()
		{
			base.position = new Rect(50f, 50f, 200f, 300f);
			base.minSize = new Vector2(200f, 300f);
		}

		internal static void ShowTerrainSplatEditor(string title, string button, Terrain terrain, int index)
		{
			TerrainSplatEditor window = EditorWindow.GetWindow<TerrainSplatEditor>(true, title);
			window.m_ButtonTitle = button;
			window.InitializeData(terrain, index);
		}

		private void InitializeData(Terrain terrain, int index)
		{
			this.m_Terrain = terrain;
			this.m_Index = index;
			SplatPrototype splatPrototype;
			if (index == -1)
			{
				splatPrototype = new SplatPrototype();
			}
			else
			{
				splatPrototype = this.m_Terrain.terrainData.splatPrototypes[index];
			}
			this.m_Texture = splatPrototype.texture;
			this.m_NormalMap = splatPrototype.normalMap;
			this.m_TileSize = splatPrototype.tileSize;
			this.m_TileOffset = splatPrototype.tileOffset;
			this.m_Specular = splatPrototype.specular;
			this.m_Metallic = splatPrototype.metallic;
			this.m_Smoothness = splatPrototype.smoothness;
		}

		private void ApplyTerrainSplat()
		{
			if (this.m_Terrain == null || this.m_Terrain.terrainData == null)
			{
				return;
			}
			SplatPrototype[] array = this.m_Terrain.terrainData.splatPrototypes;
			if (this.m_Index == -1)
			{
				SplatPrototype[] array2 = new SplatPrototype[array.Length + 1];
				Array.Copy(array, 0, array2, 0, array.Length);
				this.m_Index = array.Length;
				array = array2;
				array[this.m_Index] = new SplatPrototype();
			}
			array[this.m_Index].texture = this.m_Texture;
			array[this.m_Index].normalMap = this.m_NormalMap;
			array[this.m_Index].tileSize = this.m_TileSize;
			array[this.m_Index].tileOffset = this.m_TileOffset;
			array[this.m_Index].specular = this.m_Specular;
			array[this.m_Index].metallic = this.m_Metallic;
			array[this.m_Index].smoothness = this.m_Smoothness;
			this.m_Terrain.terrainData.splatPrototypes = array;
			EditorUtility.SetDirty(this.m_Terrain);
		}

		private bool ValidateTerrain()
		{
			if (this.m_Terrain == null || this.m_Terrain.terrainData == null)
			{
				EditorGUILayout.HelpBox("Terrain does not exist", MessageType.Error);
				return false;
			}
			return true;
		}

		private bool ValidateMainTexture(Texture2D tex)
		{
			if (tex == null)
			{
				EditorGUILayout.HelpBox("Assign a tiling texture", MessageType.Warning);
				return false;
			}
			if (tex.wrapMode != TextureWrapMode.Repeat)
			{
				EditorGUILayout.HelpBox("Texture wrap mode must be set to Repeat", MessageType.Warning);
				return false;
			}
			if (tex.width != Mathf.ClosestPowerOfTwo(tex.width) || tex.height != Mathf.ClosestPowerOfTwo(tex.height))
			{
				EditorGUILayout.HelpBox("Texture size must be power of two", MessageType.Warning);
				return false;
			}
			if (tex.mipmapCount <= 1)
			{
				EditorGUILayout.HelpBox("Texture must have mip maps", MessageType.Warning);
				return false;
			}
			return true;
		}

		private static void TextureFieldGUI(string label, ref Texture2D texture, float alignmentOffset)
		{
			GUILayout.Space(6f);
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			GUILayout.Label(label, new GUILayoutOption[0]);
			Type typeFromHandle = typeof(Texture2D);
			Rect rect = GUILayoutUtility.GetRect(64f, 64f, 64f, 64f, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(64f)
			});
			rect.x += alignmentOffset;
			texture = (EditorGUI.DoObjectField(rect, rect, GUIUtility.GetControlID(12354, EditorGUIUtility.native, rect), texture, typeFromHandle, null, null, false) as Texture2D);
			GUILayout.EndVertical();
		}

		private static void SplatSizeGUI(ref Vector2 scale, ref Vector2 offset)
		{
			GUILayoutOption gUILayoutOption = GUILayout.Width(10f);
			GUILayoutOption gUILayoutOption2 = GUILayout.MinWidth(32f);
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(string.Empty, EditorStyles.miniLabel, new GUILayoutOption[]
			{
				gUILayoutOption
			});
			GUILayout.Label("x", EditorStyles.miniLabel, new GUILayoutOption[]
			{
				gUILayoutOption
			});
			GUILayout.Label("y", EditorStyles.miniLabel, new GUILayoutOption[]
			{
				gUILayoutOption
			});
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Size", EditorStyles.miniLabel, new GUILayoutOption[0]);
			scale.x = EditorGUILayout.FloatField(scale.x, EditorStyles.miniTextField, new GUILayoutOption[]
			{
				gUILayoutOption2
			});
			scale.y = EditorGUILayout.FloatField(scale.y, EditorStyles.miniTextField, new GUILayoutOption[]
			{
				gUILayoutOption2
			});
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Offset", EditorStyles.miniLabel, new GUILayoutOption[0]);
			offset.x = EditorGUILayout.FloatField(offset.x, EditorStyles.miniTextField, new GUILayoutOption[]
			{
				gUILayoutOption2
			});
			offset.y = EditorGUILayout.FloatField(offset.y, EditorStyles.miniTextField, new GUILayoutOption[]
			{
				gUILayoutOption2
			});
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}

		private static bool IsUsingMetallic(Terrain.MaterialType materialType, Material materialTemplate)
		{
			return materialType == Terrain.MaterialType.BuiltInStandard || (materialType == Terrain.MaterialType.Custom && materialTemplate != null && materialTemplate.HasProperty("_Metallic0"));
		}

		private static bool IsUsingSpecular(Terrain.MaterialType materialType, Material materialTemplate)
		{
			return materialType == Terrain.MaterialType.BuiltInStandard || (materialType == Terrain.MaterialType.Custom && materialTemplate != null && materialTemplate.HasProperty("_Specular0"));
		}

		private static bool IsUsingSmoothness(Terrain.MaterialType materialType, Material materialTemplate)
		{
			return materialType == Terrain.MaterialType.BuiltInStandard || (materialType == Terrain.MaterialType.Custom && materialTemplate != null && materialTemplate.HasProperty("_Smoothness0"));
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = base.position.width - 64f - 20f;
			bool flag = true;
			this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, false, GUI.skin.verticalScrollbar, GUI.skin.scrollView, new GUILayoutOption[0]);
			flag &= this.ValidateTerrain();
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			string label = string.Empty;
			float alignmentOffset = 0f;
			switch (this.m_Terrain.materialType)
			{
			case Terrain.MaterialType.BuiltInStandard:
				label = " Albedo (RGB)\nSmoothness (A)";
				alignmentOffset = 15f;
				break;
			case Terrain.MaterialType.BuiltInLegacyDiffuse:
				label = "\n Diffuse (RGB)";
				alignmentOffset = 15f;
				break;
			case Terrain.MaterialType.BuiltInLegacySpecular:
				label = "Diffuse (RGB)\n   Gloss (A)";
				alignmentOffset = 12f;
				break;
			case Terrain.MaterialType.Custom:
				label = " \n  Splat";
				alignmentOffset = 0f;
				break;
			}
			TerrainSplatEditor.TextureFieldGUI(label, ref this.m_Texture, alignmentOffset);
			TerrainSplatEditor.TextureFieldGUI("\nNormal", ref this.m_NormalMap, -4f);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			flag &= this.ValidateMainTexture(this.m_Texture);
			if (flag)
			{
				if (TerrainSplatEditor.IsUsingMetallic(this.m_Terrain.materialType, this.m_Terrain.materialTemplate))
				{
					EditorGUILayout.Space();
					float labelWidth = EditorGUIUtility.labelWidth;
					EditorGUIUtility.labelWidth = 75f;
					this.m_Metallic = EditorGUILayout.Slider("Metallic", this.m_Metallic, 0f, 1f, new GUILayoutOption[0]);
					EditorGUIUtility.labelWidth = labelWidth;
				}
				else if (TerrainSplatEditor.IsUsingSpecular(this.m_Terrain.materialType, this.m_Terrain.materialTemplate))
				{
					this.m_Specular = EditorGUILayout.ColorField("Specular", this.m_Specular, new GUILayoutOption[0]);
				}
				if (TerrainSplatEditor.IsUsingSmoothness(this.m_Terrain.materialType, this.m_Terrain.materialTemplate) && !TextureUtil.HasAlphaTextureFormat(this.m_Texture.format))
				{
					EditorGUILayout.Space();
					float labelWidth2 = EditorGUIUtility.labelWidth;
					EditorGUIUtility.labelWidth = 75f;
					this.m_Smoothness = EditorGUILayout.Slider("Smoothness", this.m_Smoothness, 0f, 1f, new GUILayoutOption[0]);
					EditorGUIUtility.labelWidth = labelWidth2;
				}
			}
			TerrainSplatEditor.SplatSizeGUI(ref this.m_TileSize, ref this.m_TileOffset);
			bool flag2 = EditorGUI.EndChangeCheck();
			EditorGUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = flag;
			if (GUILayout.Button(this.m_ButtonTitle, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}))
			{
				this.ApplyTerrainSplat();
				base.Close();
				GUIUtility.ExitGUI();
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			if (flag2 && flag && this.m_Index != -1)
			{
				this.ApplyTerrainSplat();
			}
		}
	}
}
