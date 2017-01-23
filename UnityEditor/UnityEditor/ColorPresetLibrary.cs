using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ColorPresetLibrary : PresetLibrary
	{
		[Serializable]
		private class ColorPreset
		{
			[SerializeField]
			private string m_Name;

			[SerializeField]
			private Color m_Color;

			public Color color
			{
				get
				{
					return this.m_Color;
				}
				set
				{
					this.m_Color = value;
				}
			}

			public string name
			{
				get
				{
					return this.m_Name;
				}
				set
				{
					this.m_Name = value;
				}
			}

			public ColorPreset(Color preset, string presetName)
			{
				this.color = preset;
				this.name = presetName;
			}

			public ColorPreset(Color preset, Color preset2, string presetName)
			{
				this.color = preset;
				this.name = presetName;
			}
		}

		[SerializeField]
		private List<ColorPresetLibrary.ColorPreset> m_Presets = new List<ColorPresetLibrary.ColorPreset>();

		private Texture2D m_ColorSwatch;

		private Texture2D m_ColorSwatchTriangular;

		private Texture2D m_MiniColorSwatchTriangular;

		private Texture2D m_CheckerBoard;

		public const int kSwatchSize = 14;

		public const int kMiniSwatchSize = 8;

		private void OnDestroy()
		{
			if (this.m_ColorSwatch != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ColorSwatch);
			}
			if (this.m_ColorSwatchTriangular != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_ColorSwatchTriangular);
			}
			if (this.m_MiniColorSwatchTriangular != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_MiniColorSwatchTriangular);
			}
			if (this.m_CheckerBoard != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_CheckerBoard);
			}
		}

		public override int Count()
		{
			return this.m_Presets.Count;
		}

		public override object GetPreset(int index)
		{
			return this.m_Presets[index].color;
		}

		public override void Add(object presetObject, string presetName)
		{
			Color preset = (Color)presetObject;
			this.m_Presets.Add(new ColorPresetLibrary.ColorPreset(preset, presetName));
		}

		public override void Replace(int index, object newPresetObject)
		{
			Color color = (Color)newPresetObject;
			this.m_Presets[index].color = color;
		}

		public override void Remove(int index)
		{
			this.m_Presets.RemoveAt(index);
		}

		public override void Move(int index, int destIndex, bool insertAfterDestIndex)
		{
			PresetLibraryHelpers.MoveListItem<ColorPresetLibrary.ColorPreset>(this.m_Presets, index, destIndex, insertAfterDestIndex);
		}

		public override void Draw(Rect rect, int index)
		{
			this.DrawInternal(rect, this.m_Presets[index].color);
		}

		public override void Draw(Rect rect, object presetObject)
		{
			this.DrawInternal(rect, (Color)presetObject);
		}

		private void Init()
		{
			if (this.m_ColorSwatch == null)
			{
				this.m_ColorSwatch = ColorPresetLibrary.CreateColorSwatchWithBorder(14, 14, false);
			}
			if (this.m_ColorSwatchTriangular == null)
			{
				this.m_ColorSwatchTriangular = ColorPresetLibrary.CreateColorSwatchWithBorder(14, 14, true);
			}
			if (this.m_MiniColorSwatchTriangular == null)
			{
				this.m_MiniColorSwatchTriangular = ColorPresetLibrary.CreateColorSwatchWithBorder(8, 8, true);
			}
			if (this.m_CheckerBoard == null)
			{
				this.m_CheckerBoard = GradientEditor.CreateCheckerTexture(2, 2, 3, new Color(0.8f, 0.8f, 0.8f), new Color(0.5f, 0.5f, 0.5f));
			}
		}

		private void DrawInternal(Rect rect, Color preset)
		{
			this.Init();
			bool flag = preset.maxColorComponent > 1f;
			if (flag)
			{
				preset = preset.RGBMultiplied(1f / preset.maxColorComponent);
			}
			Color color = GUI.color;
			if ((int)rect.height == 14)
			{
				if (preset.a > 0.97f)
				{
					this.RenderSolidSwatch(rect, preset);
				}
				else
				{
					this.RenderSwatchWithAlpha(rect, preset, this.m_ColorSwatchTriangular);
				}
				if (flag)
				{
					GUI.Label(rect, "h");
				}
			}
			else
			{
				this.RenderSwatchWithAlpha(rect, preset, this.m_MiniColorSwatchTriangular);
			}
			GUI.color = color;
		}

		private void RenderSolidSwatch(Rect rect, Color preset)
		{
			GUI.color = preset;
			GUI.DrawTexture(rect, this.m_ColorSwatch);
		}

		private void RenderSwatchWithAlpha(Rect rect, Color preset, Texture2D swatchTexture)
		{
			Rect position = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
			GUI.color = Color.white;
			Rect texCoords = new Rect(0f, 0f, position.width / (float)this.m_CheckerBoard.width, position.height / (float)this.m_CheckerBoard.height);
			GUI.DrawTextureWithTexCoords(position, this.m_CheckerBoard, texCoords, false);
			GUI.color = preset;
			GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
			GUI.color = new Color(preset.r, preset.g, preset.b, 1f);
			GUI.DrawTexture(rect, swatchTexture);
		}

		public override string GetName(int index)
		{
			return this.m_Presets[index].name;
		}

		public override void SetName(int index, string presetName)
		{
			this.m_Presets[index].name = presetName;
		}

		public void CreateDebugColors()
		{
			for (int i = 0; i < 2000; i++)
			{
				this.m_Presets.Add(new ColorPresetLibrary.ColorPreset(new Color(UnityEngine.Random.Range(0.2f, 1f), UnityEngine.Random.Range(0.2f, 1f), UnityEngine.Random.Range(0.2f, 1f), 1f), "Preset Color " + i));
			}
		}

		private static Texture2D CreateColorSwatchWithBorder(int width, int height, bool triangular)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color[] array = new Color[width * height];
			Color color = new Color(1f, 1f, 1f, 0f);
			if (triangular)
			{
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						if (i < width - j)
						{
							array[j + i * width] = Color.white;
						}
						else
						{
							array[j + i * width] = color;
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < height * width; k++)
				{
					array[k] = Color.white;
				}
			}
			for (int l = 0; l < width; l++)
			{
				array[l] = Color.black;
			}
			for (int m = 0; m < width; m++)
			{
				array[(height - 1) * width + m] = Color.black;
			}
			for (int n = 0; n < height; n++)
			{
				array[n * width] = Color.black;
			}
			for (int num = 0; num < height; num++)
			{
				array[num * width + width - 1] = Color.black;
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}
	}
}
