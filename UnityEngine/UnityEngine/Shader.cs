using System;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class Shader : Object
	{
		public extern bool isSupported
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern string customEditor
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern ShaderHardwareTier globalShaderHardwareTier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int maximumLOD
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int globalMaximumLOD
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int renderQueue
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern DisableBatchingType disableBatching
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader Find(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Shader FindBuiltin(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableKeyword(string keyword);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableKeyword(string keyword);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsKeywordEnabled(string keyword);

		public static void SetGlobalColor(string propertyName, Color color)
		{
			Shader.SetGlobalColor(Shader.PropertyToID(propertyName), color);
		}

		public static void SetGlobalColor(int nameID, Color color)
		{
			Shader.INTERNAL_CALL_SetGlobalColor(nameID, ref color);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalColor(int nameID, ref Color color);

		public static void SetGlobalVector(string propertyName, Vector4 vec)
		{
			Shader.SetGlobalColor(propertyName, vec);
		}

		public static void SetGlobalVector(int nameID, Vector4 vec)
		{
			Shader.SetGlobalColor(nameID, vec);
		}

		public static void SetGlobalFloat(string propertyName, float value)
		{
			Shader.SetGlobalFloat(Shader.PropertyToID(propertyName), value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalFloat(int nameID, float value);

		public static void SetGlobalInt(string propertyName, int value)
		{
			Shader.SetGlobalFloat(propertyName, (float)value);
		}

		public static void SetGlobalInt(int nameID, int value)
		{
			Shader.SetGlobalFloat(nameID, (float)value);
		}

		public static void SetGlobalTexture(string propertyName, Texture tex)
		{
			Shader.SetGlobalTexture(Shader.PropertyToID(propertyName), tex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalTexture(int nameID, Texture tex);

		public static void SetGlobalMatrix(string propertyName, Matrix4x4 mat)
		{
			Shader.SetGlobalMatrix(Shader.PropertyToID(propertyName), mat);
		}

		public static void SetGlobalMatrix(int nameID, Matrix4x4 mat)
		{
			Shader.INTERNAL_CALL_SetGlobalMatrix(nameID, ref mat);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalMatrix(int nameID, ref Matrix4x4 mat);

		public static void SetGlobalFloatArray(string propertyName, float[] values)
		{
			Shader.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
		}

		public static void SetGlobalFloatArray(int nameID, float[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			Shader.SetGlobalFloatArrayInternal(nameID, values);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArrayInternal(int nameID, float[] values);

		public static void SetGlobalVectorArray(string propertyName, Vector4[] values)
		{
			Shader.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
		}

		public static void SetGlobalVectorArray(int nameID, Vector4[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			Shader.SetGlobalVectorArrayInternal(nameID, values);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArrayInternal(int nameID, Vector4[] values);

		public static void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
		{
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
		}

		public static void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			Shader.SetGlobalMatrixArrayInternal(nameID, values);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArrayInternal(int nameID, Matrix4x4[] values);

		[Obsolete("SetGlobalTexGenMode is not supported anymore. Use programmable shaders to achieve the same effect.", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalTexGenMode(string propertyName, TexGenMode mode);

		[Obsolete("SetGlobalTextureMatrixName is not supported anymore. Use programmable shaders to achieve the same effect.", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalTextureMatrixName(string propertyName, string matrixName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalBuffer(string propertyName, ComputeBuffer buffer);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int PropertyToID(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WarmupAllShaders();
	}
}
