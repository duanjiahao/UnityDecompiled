using System;
using System.Runtime.CompilerServices;
using UnityEngine;
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
		public enum ShaderPropertyTexDim
		{
			TexDimUnknown = -1,
			TexDimNone,
			TexDimDeprecated1D,
			TexDim2D,
			TexDim3D,
			TexDimCUBE,
			TexDimAny,
			TexDimRECT = 5
		}
		internal enum ShaderModel
		{
			None,
			SM1,
			SM2,
			SM3,
			SM4,
			SM5
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal static extern bool hardwareSupportsFullNPOT
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal static extern bool wireframeMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void FetchCachedErrors(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetErrorCount(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetShaderErrorMessage(Shader s, int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetShaderErrorPlatform(Shader s, int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetShaderErrorWarning(Shader s, int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetShaderErrorFile(Shader s, int index, bool fileNameOnly);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetShaderErrorLine(Shader s, int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSnippetCount(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSnippetSize(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetComboCount(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasSurfaceShaders(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetPropertyCount(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetPropertyDescription(Shader s, int propertyIdx);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetPropertyName(Shader s, int propertyIdx);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ShaderUtil.ShaderPropertyType GetPropertyType(Shader s, int propertyIdx);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetRangeLimits(Shader s, int propertyIdx, int defminmax);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ShaderUtil.ShaderPropertyTexDim GetTexDim(Shader s, int propertyIdx);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetTextureDimension(Texture t);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsShaderPropertyHidden(Shader s, int propertyIdx);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetShaderPropertyAttribute(Shader s, string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasShadowCasterPass(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasShadowCollectorPass(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasTangentChannel(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetSourceChannels(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoesIgnoreProjector(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetRenderQueue(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetLOD(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetFallback(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSubShaderCount(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetDependency(Shader s, string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ShaderUtil.ShaderModel GetVertexModel(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern ShaderUtil.ShaderModel GetFragmentModel(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoesShaderContainFixedFunctionPasses(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenCompiledShader(Shader shader, bool allPlatforms);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenParsedSurfaceShader(Shader shader);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenShaderSnippets(Shader shader);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void OpenShaderCombinations(Shader shader);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rawViewportRect(out Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_rawViewportRect(ref Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_rawScissorRect(out Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_set_rawScissorRect(ref Rect value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasClip(Shader s);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RecreateGfxDevice();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RecreateSkinnedMeshResources();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader CreateShaderAsset(string source);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateShaderAsset(Shader shader, string source);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MaterialProperty[] GetMaterialProperties(UnityEngine.Object[] mats);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, string name);
		internal static MaterialProperty GetMaterialProperty(UnityEngine.Object[] mats, int propertyIndex)
		{
			return ShaderUtil.GetMaterialProperty_Index(mats, propertyIndex);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MaterialProperty GetMaterialProperty_Index(UnityEngine.Object[] mats, int propertyIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ApplyProperty(MaterialProperty prop, int propertyMask, string undoName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ApplyMaterialPropertyBlockToMaterialProperty(MaterialPropertyBlock propertyBlock, MaterialProperty materialProperty);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ApplyMaterialPropertyToMaterialPropertyBlock(MaterialProperty materialProperty, int propertyMask, MaterialPropertyBlock propertyBlock);
	}
}
