using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor
{
	public sealed class SubstanceImporter : AssetImporter
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string[] GetPrototypeNames();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetMaterialCount();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralMaterial[] GetMaterials();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string CloneMaterial(ProceduralMaterial material);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string InstantiateMaterial(string prototypeName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DestroyMaterial(ProceduralMaterial material);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetMaterial(ProceduralMaterial material);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool RenameMaterial(ProceduralMaterial material, string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnShaderModified(ProceduralMaterial material);
		public Vector2 GetMaterialOffset(ProceduralMaterial material)
		{
			return this.GetMaterialInformation(material).offset;
		}
		public void SetMaterialOffset(ProceduralMaterial material, Vector2 offset)
		{
			ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.offset = offset;
			this.SetMaterialInformation(material, materialInformation);
		}
		public Vector2 GetMaterialScale(ProceduralMaterial material)
		{
			return this.GetMaterialInformation(material).scale;
		}
		public void SetMaterialScale(ProceduralMaterial material, Vector2 scale)
		{
			ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.scale = scale;
			this.SetMaterialInformation(material, materialInformation);
		}
		public bool GetGenerateAllOutputs(ProceduralMaterial material)
		{
			return this.GetMaterialInformation(material).generateAllOutputs;
		}
		public void SetGenerateAllOutputs(ProceduralMaterial material, bool generated)
		{
			ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.generateAllOutputs = generated;
			this.SetMaterialInformation(material, materialInformation);
		}
		public int GetAnimationUpdateRate(ProceduralMaterial material)
		{
			return this.GetMaterialInformation(material).animationUpdateRate;
		}
		public void SetAnimationUpdateRate(ProceduralMaterial material, int animation_update_rate)
		{
			ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.animationUpdateRate = animation_update_rate;
			this.SetMaterialInformation(material, materialInformation);
		}
		public bool GetGenerateMipMaps(ProceduralMaterial material)
		{
			return this.GetMaterialInformation(material).generateMipMaps;
		}
		public void SetGenerateMipMaps(ProceduralMaterial material, bool mode)
		{
			ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
			materialInformation.generateMipMaps = mode;
			this.SetMaterialInformation(material, materialInformation);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProceduralOutputType GetTextureAlphaSource(ProceduralMaterial material, string textureName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTextureAlphaSource(ProceduralMaterial material, string textureName, ProceduralOutputType alphaSource);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetPlatformTextureSettings(string materialName, string platform, out int maxTextureWidth, out int maxTextureHeight, out int textureFormat, out int loadBehavior);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPlatformTextureSettings(string materialName, string platform, int maxTextureWidth, int maxTextureHeight, int textureFormat, int loadBehavior);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ProceduralMaterialInformation GetMaterialInformation(ProceduralMaterial material);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMaterialInformation(ProceduralMaterial material, ProceduralMaterialInformation information);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CanShaderPropertyHostProceduralOutput(string name, ProceduralOutputType substanceType);
		internal static bool IsProceduralTextureSlot(Material material, Texture tex, string name)
		{
			return material is ProceduralMaterial && tex is ProceduralTexture && SubstanceImporter.CanShaderPropertyHostProceduralOutput(name, (tex as ProceduralTexture).GetProceduralOutputType()) && SubstanceImporter.IsSubstanceParented(tex as ProceduralTexture, material as ProceduralMaterial);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ClearPlatformTextureSettings(string materialName, string platform);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void OnTextureInformationsChanged(ProceduralTexture texture);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void ExportBitmaps(ProceduralMaterial material);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsSubstanceParented(ProceduralTexture texture, ProceduralMaterial material);
	}
}
