using System;
using System.IO;
using UnityEngine;

namespace UnityEditor
{
	internal class ImportRawHeightmap : TerrainWizard
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

		public ImportRawHeightmap.Depth m_Depth = ImportRawHeightmap.Depth.Bit16;

		public int m_Width = 1;

		public int m_Height = 1;

		public ImportRawHeightmap.ByteOrder m_ByteOrder = ImportRawHeightmap.ByteOrder.Windows;

		public bool m_FlipVertically = false;

		public Vector3 m_TerrainSize = new Vector3(2000f, 600f, 2000f);

		private string m_Path;

		private void PickRawDefaults(string path)
		{
			FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
			int num = (int)fileStream.Length;
			fileStream.Close();
			this.m_TerrainSize = base.terrainData.size;
			if (base.terrainData.heightmapWidth * base.terrainData.heightmapHeight == num)
			{
				this.m_Width = base.terrainData.heightmapWidth;
				this.m_Height = base.terrainData.heightmapHeight;
				this.m_Depth = ImportRawHeightmap.Depth.Bit8;
			}
			else if (base.terrainData.heightmapWidth * base.terrainData.heightmapHeight * 2 == num)
			{
				this.m_Width = base.terrainData.heightmapWidth;
				this.m_Height = base.terrainData.heightmapHeight;
				this.m_Depth = ImportRawHeightmap.Depth.Bit16;
			}
			else
			{
				this.m_Depth = ImportRawHeightmap.Depth.Bit16;
				int num2 = num / (int)this.m_Depth;
				int num3 = Mathf.RoundToInt(Mathf.Sqrt((float)num2));
				int num4 = Mathf.RoundToInt(Mathf.Sqrt((float)num2));
				if (num3 * num4 * (int)this.m_Depth == num)
				{
					this.m_Width = num3;
					this.m_Height = num4;
				}
				else
				{
					this.m_Depth = ImportRawHeightmap.Depth.Bit8;
					num2 = num / (int)this.m_Depth;
					num3 = Mathf.RoundToInt(Mathf.Sqrt((float)num2));
					num4 = Mathf.RoundToInt(Mathf.Sqrt((float)num2));
					if (num3 * num4 * (int)this.m_Depth == num)
					{
						this.m_Width = num3;
						this.m_Height = num4;
					}
					else
					{
						this.m_Depth = ImportRawHeightmap.Depth.Bit16;
					}
				}
			}
		}

		internal void OnWizardCreate()
		{
			if (this.m_Terrain == null)
			{
				base.isValid = false;
				base.errorString = "Terrain does not exist";
			}
			if (this.m_Width > 4097 || this.m_Height > 4097)
			{
				base.isValid = false;
				base.errorString = "Heightmaps above 4097x4097 in resolution are not supported";
				Debug.LogError(base.errorString);
			}
			if (File.Exists(this.m_Path) && base.isValid)
			{
				Undo.RegisterCompleteObjectUndo(base.terrainData, "Import Raw heightmap");
				base.terrainData.heightmapResolution = Mathf.Max(this.m_Width, this.m_Height);
				base.terrainData.size = this.m_TerrainSize;
				this.ReadRaw(this.m_Path);
				base.FlushHeightmapModification();
			}
		}

		private void ReadRaw(string path)
		{
			byte[] array;
			using (BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
			{
				array = binaryReader.ReadBytes(this.m_Width * this.m_Height * (int)this.m_Depth);
				binaryReader.Close();
			}
			int heightmapWidth = base.terrainData.heightmapWidth;
			int heightmapHeight = base.terrainData.heightmapHeight;
			float[,] array2 = new float[heightmapHeight, heightmapWidth];
			if (this.m_Depth == ImportRawHeightmap.Depth.Bit16)
			{
				float num = 1.52587891E-05f;
				for (int i = 0; i < heightmapHeight; i++)
				{
					for (int j = 0; j < heightmapWidth; j++)
					{
						int num2 = Mathf.Clamp(j, 0, this.m_Width - 1) + Mathf.Clamp(i, 0, this.m_Height - 1) * this.m_Width;
						if (this.m_ByteOrder == ImportRawHeightmap.ByteOrder.Mac == BitConverter.IsLittleEndian)
						{
							byte b = array[num2 * 2];
							array[num2 * 2] = array[num2 * 2 + 1];
							array[num2 * 2 + 1] = b;
						}
						ushort num3 = BitConverter.ToUInt16(array, num2 * 2);
						float num4 = (float)num3 * num;
						int num5 = (!this.m_FlipVertically) ? i : (heightmapHeight - 1 - i);
						array2[num5, j] = num4;
					}
				}
			}
			else
			{
				float num6 = 0.00390625f;
				for (int k = 0; k < heightmapHeight; k++)
				{
					for (int l = 0; l < heightmapWidth; l++)
					{
						int num7 = Mathf.Clamp(l, 0, this.m_Width - 1) + Mathf.Clamp(k, 0, this.m_Height - 1) * this.m_Width;
						byte b2 = array[num7];
						float num8 = (float)b2 * num6;
						int num9 = (!this.m_FlipVertically) ? k : (heightmapHeight - 1 - k);
						array2[num9, l] = num8;
					}
				}
			}
			base.terrainData.SetHeights(0, 0, array2);
		}

		internal void InitializeImportRaw(Terrain terrain, string path)
		{
			this.m_Terrain = terrain;
			this.m_Path = path;
			this.PickRawDefaults(this.m_Path);
			base.helpString = "Raw files must use a single channel and be either 8 or 16 bit.";
			this.OnWizardUpdate();
		}
	}
}
