using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class SubstanceImporter : AssetImporter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetPrototypeNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetMaterialCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralMaterial[] GetMaterials();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string CloneMaterial(ProceduralMaterial material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string InstantiateMaterial(string prototypeName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DestroyMaterial(ProceduralMaterial material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetMaterial(ProceduralMaterial material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool RenameMaterial(ProceduralMaterial material, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnShaderModified(ProceduralMaterial material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralOutputType GetTextureAlphaSource(ProceduralMaterial material, string textureName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTextureAlphaSource(ProceduralMaterial material, string textureName, ProceduralOutputType alphaSource);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetPlatformTextureSettings(string materialName, string platform, out int maxTextureWidth, out int maxTextureHeight, out int textureFormat, out int loadBehavior);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformTextureSettings(ProceduralMaterial material, string platform, int maxTextureWidth, int maxTextureHeight, int textureFormat, int loadBehavior);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ScriptingProceduralMaterialInformation GetMaterialInformation(ProceduralMaterial material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMaterialInformation(ProceduralMaterial material, ScriptingProceduralMaterialInformation scriptingProcMatInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CanShaderPropertyHostProceduralOutput(string name, ProceduralOutputType substanceType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ClearPlatformTextureSettings(string materialName, string platform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void OnTextureInformationsChanged(ProceduralTexture texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ExportBitmapsInternal(ProceduralMaterial material, string exportPath, bool alphaRemap);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsSubstanceParented(ProceduralTexture texture, ProceduralMaterial material);

		public Vector2 GetMaterialOffset(ProceduralMaterial material)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			return this.GetMaterialInformation(material).offset;
		}

		public void SetMaterialOffset(ProceduralMaterial material, Vector2 offset)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			ScriptingProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.offset = offset;
			this.SetMaterialInformation(material, materialInformation);
		}

		public Vector2 GetMaterialScale(ProceduralMaterial material)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			return this.GetMaterialInformation(material).scale;
		}

		public void SetMaterialScale(ProceduralMaterial material, Vector2 scale)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			ScriptingProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.scale = scale;
			this.SetMaterialInformation(material, materialInformation);
		}

		public bool GetGenerateAllOutputs(ProceduralMaterial material)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			return this.GetMaterialInformation(material).generateAllOutputs;
		}

		public void SetGenerateAllOutputs(ProceduralMaterial material, bool generated)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			ScriptingProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.generateAllOutputs = generated;
			this.SetMaterialInformation(material, materialInformation);
		}

		public int GetAnimationUpdateRate(ProceduralMaterial material)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			return this.GetMaterialInformation(material).animationUpdateRate;
		}

		public void SetAnimationUpdateRate(ProceduralMaterial material, int animation_update_rate)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			ScriptingProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.animationUpdateRate = animation_update_rate;
			this.SetMaterialInformation(material, materialInformation);
		}

		public bool GetGenerateMipMaps(ProceduralMaterial material)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			return this.GetMaterialInformation(material).generateMipMaps;
		}

		public void SetGenerateMipMaps(ProceduralMaterial material, bool mode)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			ScriptingProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.generateMipMaps = mode;
			this.SetMaterialInformation(material, materialInformation);
		}

		internal static bool IsProceduralTextureSlot(Material material, Texture tex, string name)
		{
			return material is ProceduralMaterial && tex is ProceduralTexture && SubstanceImporter.CanShaderPropertyHostProceduralOutput(name, (tex as ProceduralTexture).GetProceduralOutputType()) && SubstanceImporter.IsSubstanceParented(tex as ProceduralTexture, material as ProceduralMaterial);
		}

		public void ExportBitmaps(ProceduralMaterial material, string exportPath, bool alphaRemap)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			if (exportPath == "")
			{
				throw new ArgumentException("Invalid export path specified");
			}
			DirectoryInfo directoryInfo = Directory.CreateDirectory(exportPath);
			if (!directoryInfo.Exists)
			{
				throw new ArgumentException("Export folder " + exportPath + " doesn't exist and cannot be created.");
			}
			this.ExportBitmapsInternal(material, exportPath, alphaRemap);
		}

		public void ExportPreset(ProceduralMaterial material, string exportPath)
		{
			if (material == null)
			{
				throw new ArgumentException("Invalid ProceduralMaterial");
			}
			if (exportPath == "")
			{
				throw new ArgumentException("Invalid export path specified");
			}
			DirectoryInfo directoryInfo = Directory.CreateDirectory(exportPath);
			if (!directoryInfo.Exists)
			{
				throw new ArgumentException("Export folder " + exportPath + " doesn't exist and cannot be created.");
			}
			File.WriteAllText(Path.Combine(exportPath, material.name + ".sbsprs"), material.preset);
		}
	}
}
