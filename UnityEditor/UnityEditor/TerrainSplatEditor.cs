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
		private void ShowNormalMapShaderWarning()
		{
			if (this.m_NormalMap != null && this.m_Terrain != null && (this.m_Terrain.materialTemplate == null || !this.m_Terrain.materialTemplate.HasProperty("_Normal0")))
			{
				EditorGUILayout.HelpBox("Note: in order for normal map to have effect, a custom material with normal mapped terrain shader needs to be used.", MessageType.Info);
			}
		}
		private static void TextureFieldGUI(string label, ref Texture2D texture)
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
		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = (float)Screen.width - 64f - 20f;
			bool flag = true;
			this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, false, GUI.skin.verticalScrollbar, GUI.skin.scrollView, new GUILayoutOption[0]);
			flag &= this.ValidateTerrain();
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			TerrainSplatEditor.TextureFieldGUI("Texture", ref this.m_Texture);
			TerrainSplatEditor.TextureFieldGUI("Normal Map", ref this.m_NormalMap);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			flag &= this.ValidateMainTexture(this.m_Texture);
			this.ShowNormalMapShaderWarning();
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
