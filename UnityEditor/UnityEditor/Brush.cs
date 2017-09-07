using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor
{
	internal class Brush
	{
		private float[] m_Strength;

		private int m_Size;

		private Texture2D m_Brush;

		private Texture2D m_Preview;

		private Projector m_BrushProjector;

		internal const int kMinBrushSize = 3;

		public bool Load(Texture2D brushTex, int size)
		{
			if (this.m_BrushProjector != null && this.m_Preview != null)
			{
				this.m_BrushProjector.material.mainTexture = this.m_Preview;
			}
			bool result;
			if (this.m_Brush == brushTex && size == this.m_Size && this.m_Strength != null)
			{
				result = true;
			}
			else if (brushTex != null)
			{
				float num = (float)size;
				this.m_Size = size;
				this.m_Strength = new float[this.m_Size * this.m_Size];
				if (this.m_Size > 3)
				{
					for (int i = 0; i < this.m_Size; i++)
					{
						for (int j = 0; j < this.m_Size; j++)
						{
							this.m_Strength[i * this.m_Size + j] = brushTex.GetPixelBilinear(((float)j + 0.5f) / num, (float)i / num).a;
						}
					}
				}
				else
				{
					for (int k = 0; k < this.m_Strength.Length; k++)
					{
						this.m_Strength[k] = 1f;
					}
				}
				UnityEngine.Object.DestroyImmediate(this.m_Preview);
				this.m_Preview = new Texture2D(this.m_Size, this.m_Size, TextureFormat.RGBA32, false);
				this.m_Preview.hideFlags = HideFlags.HideAndDontSave;
				this.m_Preview.wrapMode = TextureWrapMode.Repeat;
				this.m_Preview.filterMode = FilterMode.Point;
				Color[] array = new Color[this.m_Size * this.m_Size];
				for (int l = 0; l < array.Length; l++)
				{
					array[l] = new Color(1f, 1f, 1f, this.m_Strength[l]);
				}
				this.m_Preview.SetPixels(0, 0, this.m_Size, this.m_Size, array, 0);
				this.m_Preview.Apply();
				if (this.m_BrushProjector == null)
				{
					this.CreatePreviewBrush();
				}
				this.m_BrushProjector.material.mainTexture = this.m_Preview;
				if (GraphicsSettings.renderPipelineAsset != null)
				{
					this.m_BrushProjector.material.renderQueue = GraphicsSettings.renderPipelineAsset.GetTerrainBrushPassIndex();
				}
				this.m_Brush = brushTex;
				result = true;
			}
			else
			{
				this.m_Strength = new float[1];
				this.m_Strength[0] = 1f;
				this.m_Size = 1;
				result = false;
			}
			return result;
		}

		public float GetStrengthInt(int ix, int iy)
		{
			ix = Mathf.Clamp(ix, 0, this.m_Size - 1);
			iy = Mathf.Clamp(iy, 0, this.m_Size - 1);
			return this.m_Strength[iy * this.m_Size + ix];
		}

		public void Dispose()
		{
			if (this.m_BrushProjector)
			{
				UnityEngine.Object.DestroyImmediate(this.m_BrushProjector.gameObject);
				this.m_BrushProjector = null;
			}
			UnityEngine.Object.DestroyImmediate(this.m_Preview);
			this.m_Preview = null;
		}

		public Projector GetPreviewProjector()
		{
			return this.m_BrushProjector;
		}

		private void CreatePreviewBrush()
		{
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("TerrainInspectorBrushPreview", HideFlags.HideAndDontSave, new Type[]
			{
				typeof(Projector)
			});
			this.m_BrushProjector = (gameObject.GetComponent(typeof(Projector)) as Projector);
			this.m_BrushProjector.enabled = false;
			this.m_BrushProjector.nearClipPlane = -1000f;
			this.m_BrushProjector.farClipPlane = 1000f;
			this.m_BrushProjector.orthographic = true;
			this.m_BrushProjector.orthographicSize = 10f;
			this.m_BrushProjector.transform.Rotate(90f, 0f, 0f);
			Material material = EditorGUIUtility.LoadRequired("SceneView/TerrainBrushMaterial.mat") as Material;
			material.SetTexture("_CutoutTex", (Texture2D)EditorGUIUtility.Load(EditorResourcesUtility.brushesPath + "brush_cutout.png"));
			this.m_BrushProjector.material = material;
		}
	}
}
