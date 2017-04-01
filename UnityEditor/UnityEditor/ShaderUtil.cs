using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class ShaderUtil
	{
		public enum ShaderPropertyType
		{
			Color,
			Vector,
			Float,
			Range,
			TexEnv
		}

		internal enum ShaderCompilerPlatformType
		{
			OpenGL,
			D3D9,
			Xbox360,
			PS3,
			D3D11,
			OpenGLES20,
			OpenGLES20Desktop,
			Flash,
			D3D11_9x,
			OpenGLES30,
			PSVita,
			PS4,
			XboxOne,
			PSM,
			Metal,
			OpenGLCore,
			N3DS,
			WiiU,
			Vulkan,
			Switch,
			Count
		}

		[Obsolete("Use UnityEngine.Rendering.TextureDimension instead.")]
		public enum ShaderPropertyTexDim
		{
			TexDimNone,
			TexDim2D = 2,
			TexDim3D,
			TexDimCUBE,
			TexDimAny = 6
		}

		internal static Rect rawViewportRect
		{
			get
			{
				Rect result;
				ShaderUtil.INTERNAL_get_rawViewportRect(out result);
				return result;
			}
			set
			{
				ShaderUtil.INTERNAL_set_rawViewportRect(ref value);
			}
		}

		internal static Rect rawScissorRect
		{
			get
			{
				Rect result;
				ShaderUtil.INTERNAL_get_rawScissorRect(out result);
				return result;
			}
			set
			{
				ShaderUtil.INTERNAL_set_rawScissorRect(ref value);
			}
		}

		public static extern bool hardwareSupportsRectRenderTexture
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern bool hardwareSupportsFullNPOT
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetAvailableShaderCompilerPlatforms();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void FetchCachedErrors(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetShaderErrorCount(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ShaderError[] GetShaderErrors(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetComputeShaderPlatformCount(ComputeShader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern GraphicsDeviceType GetComputeShaderPlatformType(ComputeShader s, int platformIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetComputeShaderPlatformKernelCount(ComputeShader s, int platformIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetComputeShaderPlatformKernelName(ComputeShader s, int platformIndex, int kernelIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetComputeShaderErrorCount(ComputeShader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ShaderError[] GetComputeShaderErrors(ComputeShader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetComboCount(Shader s, bool usedBySceneOnly);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasSurfaceShaders(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasFixedFunctionShaders(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasShaderSnippets(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPropertyCount(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetPropertyDescription(Shader s, int propertyIdx);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetPropertyName(Shader s, int propertyIdx);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ShaderUtil.ShaderPropertyType GetPropertyType(Shader s, int propertyIdx);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetRangeLimits(Shader s, int propertyIdx, int defminmax);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern TextureDimension GetTexDim(Shader s, int propertyIdx);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsShaderPropertyHidden(Shader s, int propertyIdx);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string[] GetShaderPropertyAttributes(Shader s, string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasInstancing(Shader s);

		internal static bool MaterialsUseInstancingShader(SerializedProperty materialsArray)
		{
			bool result;
			if (materialsArray.hasMultipleDifferentValues)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < materialsArray.arraySize; i++)
				{
					Material material = materialsArray.GetArrayElementAtIndex(i).objectReferenceValue as Material;
					if (material != null && material.enableInstancing && material.shader != null && ShaderUtil.HasInstancing(material.shader))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasShadowCasterPass(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasTangentChannel(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoesIgnoreProjector(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetRenderQueue(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLOD(Shader s);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetDependency(Shader s, string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetTextureBindingIndex(Shader s, int texturePropertyID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenCompiledShader(Shader shader, int mode, int customPlatformsMask, bool includeAllVariants);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenCompiledComputeShader(ComputeShader shader, bool allVariantsAndPlatforms);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenParsedSurfaceShader(Shader shader);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenGeneratedFixedFunctionShader(Shader shader);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenShaderSnippets(Shader shader);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenShaderCombinations(Shader shader, bool usedBySceneOnly);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenSystemShaderIncludeError(string includeName, int line);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateLightmapStrippingFromCurrentScene();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateFogStrippingFromCurrentScene();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SaveCurrentShaderVariantCollection(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearCurrentShaderVariantCollection();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReloadAllShaders();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCurrentShaderVariantCollectionShaderCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCurrentShaderVariantCollectionVariantCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetShaderVariantEntries(Shader shader, ShaderVariantCollection skipAlreadyInCollection, out int[] types, out string[] keywords);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AddNewShaderToCollection(Shader shader, ShaderVariantCollection collection);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rawViewportRect(out Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_rawViewportRect(ref Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rawScissorRect(out Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_rawScissorRect(ref Rect value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RecreateGfxDevice();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RecreateSkinnedMeshResources();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader CreateShaderAsset(string source);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateShaderAsset(Shader shader, string source);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMaterialRawRenderQueue(Material mat);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MaterialProperty[] GetMaterialProperties(UnityEngine.Object[] mats);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, string name);

		internal static MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, int propertyIndex)
		{
			return ShaderUtil.GetMaterialProperty_Index(mats, propertyIndex);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MaterialProperty GetMaterialProperty_Index(UnityEngine.Object[] mats, int propertyIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ApplyProperty(MaterialProperty prop, int propertyMask, string undoName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ApplyMaterialPropertyBlockToMaterialProperty(MaterialPropertyBlock propertyBlock, MaterialProperty materialProperty);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ApplyMaterialPropertyToMaterialPropertyBlock(MaterialProperty materialProperty, int propertyMask, MaterialPropertyBlock propertyBlock);
	}
}
