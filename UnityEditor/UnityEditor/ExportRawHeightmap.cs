using System;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
	internal class ExportRawHeightmap : TerrainWizard
	{
		internal enum Depth
		{
			Bit8 = 1,
			Bit16
		}

		internal enum ByteOrder
		{
			Mac = 1,
			Windows
		}

		public ExportRawHeightmap.Depth m_Depth = ExportRawHeightmap.Depth.Bit16;

		public ExportRawHeightmap.ByteOrder m_ByteOrder = ExportRawHeightmap.ByteOrder.Windows;

		public bool m_FlipVertically;

		public void OnEnable()
		{
			base.minSize = new Vector2(400f, 200f);
		}

		internal void OnWizardCreate()
		{
			if (this.m_Terrain == null)
			{
				base.isValid = false;
				base.errorString = "Terrain does not exist";
			}
			string text = EditorUtility.SaveFilePanel("Save Raw Heightmap", string.Empty, "terrain", "raw");
			if (text != string.Empty)
			{
				this.WriteRaw(text);
			}
		}

		internal override void OnWizardUpdate()
		{
			base.OnWizardUpdate();
			if (base.terrainData)
			{
				base.helpString = string.Concat(new object[]
				{
					"Width ",
					base.terrainData.heightmapWidth,
					"\nHeight ",
					base.terrainData.heightmapHeight
				});
			}
		}

		private void WriteRaw(string path)
		{
			int heightmapWidth = base.terrainData.heightmapWidth;
			int heightmapHeight = base.terrainData.heightmapHeight;
			float[,] heights = base.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
			byte[] array = new byte[heightmapWidth * heightmapHeight * (int)this.m_Depth];
			if (this.m_Depth == ExportRawHeightmap.Depth.Bit16)
			{
				float num = 65536f;
				for (int i = 0; i < heightmapHeight; i++)
				{
					for (int j = 0; j < heightmapWidth; j++)
					{
						int num2 = j + i * heightmapWidth;
						int num3 = (!this.m_FlipVertically) ? i : (heightmapHeight - 1 - i);
						int value = Mathf.RoundToInt(heights[num3, j] * num);
						ushort value2 = (ushort)Mathf.Clamp(value, 0, 65535);
						byte[] bytes = BitConverter.GetBytes(value2);
						if (this.m_ByteOrder == ExportRawHeightmap.ByteOrder.Mac == BitConverter.IsLittleEndian)
						{
							array[num2 * 2] = bytes[1];
							array[num2 * 2 + 1] = bytes[0];
						}
						else
						{
							array[num2 * 2] = bytes[0];
							array[num2 * 2 + 1] = bytes[1];
						}
					}
				}
			}
			else
			{
				float num4 = 256f;
				for (int k = 0; k < heightmapHeight; k++)
				{
					for (int l = 0; l < heightmapWidth; l++)
					{
						int num5 = l + k * heightmapWidth;
						int num6 = (!this.m_FlipVertically) ? k : (heightmapHeight - 1 - k);
						int value3 = Mathf.RoundToInt(heights[num6, l] * num4);
						byte b = (byte)Mathf.Clamp(value3, 0, 255);
						array[num5] = b;
					}
				}
			}
			FileStream fileStream = new FileStream(path, FileMode.Create);
			fileStream.Write(array, 0, array.Length);
			fileStream.Close();
		}

		private new void InitializeDefaults(Terrain terrain)
		{
			this.m_Terrain = terrain;
			base.helpString = string.Concat(new object[]
			{
				"Width ",
				terrain.terrainData.heightmapWidth,
				" Height ",
				terrain.terrainData.heightmapHeight
			});
			this.OnWizardUpdate();
		}
	}
}
