using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public sealed class Shader : Object
	{
		public extern bool isSupported
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern string customEditor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int maximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int globalMaximumLOD
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int renderQueue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern DisableBatchingType disableBatching
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", false)]
		public static ShaderHardwareTier globalShaderHardwareTier
		{
			get
			{
				return (ShaderHardwareTier)Graphics.activeTier;
			}
			set
			{
				Graphics.activeTier = (GraphicsTier)value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Shader Find(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Shader FindBuiltin(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EnableKeyword(string keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableKeyword(string keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsKeywordEnabled(string keyword);

		public static void SetGlobalVector(string propertyName, Vector4 vec)
		{
			Shader.SetGlobalVector(Shader.PropertyToID(propertyName), vec);
		}

		public static void SetGlobalVector(int nameID, Vector4 vec)
		{
			Shader.INTERNAL_CALL_SetGlobalVector(nameID, ref vec);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalVector(int nameID, ref Vector4 vec);

		public static void SetGlobalColor(string propertyName, Color color)
		{
			Shader.SetGlobalColor(Shader.PropertyToID(propertyName), color);
		}

		public static void SetGlobalColor(int nameID, Color color)
		{
			Shader.SetGlobalVector(nameID, color);
		}

		public static void SetGlobalFloat(string propertyName, float value)
		{
			Shader.SetGlobalFloat(Shader.PropertyToID(propertyName), value);
		}

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

		public static void SetGlobalMatrix(string propertyName, Matrix4x4 mat)
		{
			Shader.SetGlobalMatrix(Shader.PropertyToID(propertyName), mat);
		}

		public static void SetGlobalMatrix(int nameID, Matrix4x4 mat)
		{
			Shader.INTERNAL_CALL_SetGlobalMatrix(nameID, ref mat);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetGlobalMatrix(int nameID, ref Matrix4x4 mat);

		public static void SetGlobalTexture(string propertyName, Texture tex)
		{
			Shader.SetGlobalTexture(Shader.PropertyToID(propertyName), tex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalTexture(int nameID, Texture tex);

		public static void SetGlobalFloatArray(string name, List<float> values)
		{
			Shader.SetGlobalFloatArray(Shader.PropertyToID(name), values);
		}

		public static void SetGlobalFloatArray(int nameID, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			Shader.SetGlobalFloatArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArrayImplList(int nameID, object values);

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
			Shader.SetGlobalFloatArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArrayImpl(int nameID, float[] values);

		public static void SetGlobalVectorArray(string name, List<Vector4> values)
		{
			Shader.SetGlobalVectorArray(Shader.PropertyToID(name), values);
		}

		public static void SetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			Shader.SetGlobalVectorArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArrayImplList(int nameID, object values);

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
			Shader.SetGlobalVectorArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArrayImpl(int nameID, Vector4[] values);

		public static void SetGlobalMatrixArray(string name, List<Matrix4x4> values)
		{
			Shader.SetGlobalMatrixArray(Shader.PropertyToID(name), values);
		}

		public static void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			Shader.SetGlobalMatrixArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArrayImplList(int nameID, object values);

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
			Shader.SetGlobalMatrixArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArrayImpl(int nameID, Matrix4x4[] values);

		public static void SetGlobalBuffer(string name, ComputeBuffer buffer)
		{
			Shader.SetGlobalBuffer(Shader.PropertyToID(name), buffer);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetGlobalBuffer(int nameID, ComputeBuffer buffer);

		public static float GetGlobalFloat(string name)
		{
			return Shader.GetGlobalFloat(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetGlobalFloat(int nameID);

		public static int GetGlobalInt(string name)
		{
			return Shader.GetGlobalInt(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetGlobalInt(int nameID);

		public static Vector4 GetGlobalVector(string name)
		{
			return Shader.GetGlobalVector(Shader.PropertyToID(name));
		}

		public static Vector4 GetGlobalVector(int nameID)
		{
			Vector4 result;
			Shader.INTERNAL_CALL_GetGlobalVector(nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetGlobalVector(int nameID, out Vector4 value);

		public static Color GetGlobalColor(string name)
		{
			return Shader.GetGlobalColor(Shader.PropertyToID(name));
		}

		public static Color GetGlobalColor(int nameID)
		{
			Color result;
			Shader.INTERNAL_CALL_GetGlobalColor(nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetGlobalColor(int nameID, out Color value);

		public static Matrix4x4 GetGlobalMatrix(string name)
		{
			return Shader.GetGlobalMatrix(Shader.PropertyToID(name));
		}

		public static Matrix4x4 GetGlobalMatrix(int nameID)
		{
			Matrix4x4 result;
			Shader.INTERNAL_CALL_GetGlobalMatrix(nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetGlobalMatrix(int nameID, out Matrix4x4 value);

		public static Texture GetGlobalTexture(string name)
		{
			return Shader.GetGlobalTexture(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture GetGlobalTexture(int nameID);

		public static void GetGlobalFloatArray(string name, List<float> values)
		{
			Shader.GetGlobalFloatArray(Shader.PropertyToID(name), values);
		}

		public static void GetGlobalFloatArray(int nameID, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			Shader.GetGlobalFloatArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalFloatArrayImplList(int nameID, object list);

		public static float[] GetGlobalFloatArray(string name)
		{
			return Shader.GetGlobalFloatArray(Shader.PropertyToID(name));
		}

		public static float[] GetGlobalFloatArray(int nameID)
		{
			return Shader.GetGlobalFloatArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float[] GetGlobalFloatArrayImpl(int nameID);

		public static void GetGlobalVectorArray(string name, List<Vector4> values)
		{
			Shader.GetGlobalVectorArray(Shader.PropertyToID(name), values);
		}

		public static void GetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			Shader.GetGlobalVectorArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalVectorArrayImplList(int nameID, object list);

		public static Vector4[] GetGlobalVectorArray(string name)
		{
			return Shader.GetGlobalVectorArray(Shader.PropertyToID(name));
		}

		public static Vector4[] GetGlobalVectorArray(int nameID)
		{
			return Shader.GetGlobalVectorArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector4[] GetGlobalVectorArrayImpl(int nameID);

		public static void GetGlobalMatrixArray(string name, List<Matrix4x4> values)
		{
			Shader.GetGlobalMatrixArray(Shader.PropertyToID(name), values);
		}

		public static void GetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			Shader.GetGlobalMatrixArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalMatrixArrayImplList(int nameID, object list);

		public static Matrix4x4[] GetGlobalMatrixArray(string name)
		{
			return Shader.GetGlobalMatrixArray(Shader.PropertyToID(name));
		}

		public static Matrix4x4[] GetGlobalMatrixArray(int nameID)
		{
			return Shader.GetGlobalMatrixArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Matrix4x4[] GetGlobalMatrixArrayImpl(int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int PropertyToID(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WarmupAllShaders();

		[Obsolete("SetGlobalTexGenMode is not supported anymore. Use programmable shaders to achieve the same effect.", true)]
		public static void SetGlobalTexGenMode(string propertyName, TexGenMode mode)
		{
		}

		[Obsolete("SetGlobalTextureMatrixName is not supported anymore. Use programmable shaders to achieve the same effect.", true)]
		public static void SetGlobalTextureMatrixName(string propertyName, string matrixName)
		{
		}
	}
}
