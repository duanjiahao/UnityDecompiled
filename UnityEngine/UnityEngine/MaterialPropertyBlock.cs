using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class MaterialPropertyBlock
	{
		internal IntPtr m_Ptr;

		public extern bool isEmpty
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public MaterialPropertyBlock()
		{
			this.InitBlock();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InitBlock();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void DestroyBlock();

		~MaterialPropertyBlock()
		{
			this.DestroyBlock();
		}

		public void SetFloat(string name, float value)
		{
			this.SetFloat(Shader.PropertyToID(name), value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetFloat(int nameID, float value);

		public void SetVector(string name, Vector4 value)
		{
			this.SetVector(Shader.PropertyToID(name), value);
		}

		public void SetVector(int nameID, Vector4 value)
		{
			MaterialPropertyBlock.INTERNAL_CALL_SetVector(this, nameID, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetVector(MaterialPropertyBlock self, int nameID, ref Vector4 value);

		public void SetColor(string name, Color value)
		{
			this.SetColor(Shader.PropertyToID(name), value);
		}

		public void SetColor(int nameID, Color value)
		{
			MaterialPropertyBlock.INTERNAL_CALL_SetColor(this, nameID, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetColor(MaterialPropertyBlock self, int nameID, ref Color value);

		public void SetMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrix(Shader.PropertyToID(name), value);
		}

		public void SetMatrix(int nameID, Matrix4x4 value)
		{
			MaterialPropertyBlock.INTERNAL_CALL_SetMatrix(this, nameID, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetMatrix(MaterialPropertyBlock self, int nameID, ref Matrix4x4 value);

		public void SetTexture(string name, Texture value)
		{
			this.SetTexture(Shader.PropertyToID(name), value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTexture(int nameID, Texture value);

		public void SetBuffer(string name, ComputeBuffer value)
		{
			this.SetBuffer(Shader.PropertyToID(name), value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBuffer(int nameID, ComputeBuffer value);

		public float GetFloat(string name)
		{
			return this.GetFloat(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetFloat(int nameID);

		public Vector4 GetVector(string name)
		{
			return this.GetVector(Shader.PropertyToID(name));
		}

		public Vector4 GetVector(int nameID)
		{
			Vector4 result;
			MaterialPropertyBlock.INTERNAL_CALL_GetVector(this, nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetVector(MaterialPropertyBlock self, int nameID, out Vector4 value);

		public Matrix4x4 GetMatrix(string name)
		{
			return this.GetMatrix(Shader.PropertyToID(name));
		}

		public Matrix4x4 GetMatrix(int nameID)
		{
			Matrix4x4 result;
			MaterialPropertyBlock.INTERNAL_CALL_GetMatrix(this, nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetMatrix(MaterialPropertyBlock self, int nameID, out Matrix4x4 value);

		public void GetFloatArray(string name, List<float> values)
		{
			this.GetFloatArray(Shader.PropertyToID(name), values);
		}

		public void GetFloatArray(int nameID, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.GetFloatArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetFloatArrayImplList(int nameID, object list);

		public float[] GetFloatArray(string name)
		{
			return this.GetFloatArray(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float[] GetFloatArray(int nameID);

		public void GetVectorArray(string name, List<Vector4> values)
		{
			this.GetVectorArray(Shader.PropertyToID(name), values);
		}

		public void GetVectorArray(int nameID, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.GetVectorArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVectorArrayImplList(int nameID, object list);

		public Vector4[] GetVectorArray(string name)
		{
			return this.GetVectorArray(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector4[] GetVectorArray(int nameID);

		public void GetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.GetMatrixArray(Shader.PropertyToID(name), values);
		}

		public void GetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.GetMatrixArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetMatrixArrayImplList(int nameID, object list);

		public Matrix4x4[] GetMatrixArray(string name)
		{
			return this.GetMatrixArray(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Matrix4x4[] GetMatrixArray(int nameID);

		public Texture GetTexture(string name)
		{
			return this.GetTexture(Shader.PropertyToID(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture GetTexture(int nameID);

		public void SetFloatArray(string name, List<float> values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values);
		}

		public void SetFloatArray(int nameID, List<float> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetFloatArrayImplList(nameID, values);
		}

		public void SetFloatArray(string name, float[] values)
		{
			this.SetFloatArray(Shader.PropertyToID(name), values);
		}

		public void SetFloatArray(int nameID, float[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetFloatArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatArrayImpl(int nameID, float[] values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatArrayImplList(int nameID, object list);

		public void SetVectorArray(string name, List<Vector4> values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values);
		}

		public void SetVectorArray(int nameID, List<Vector4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetVectorArrayImplList(nameID, values);
		}

		public void SetVectorArray(string name, Vector4[] values)
		{
			this.SetVectorArray(Shader.PropertyToID(name), values);
		}

		public void SetVectorArray(int nameID, Vector4[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetVectorArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVectorArrayImpl(int nameID, Vector4[] values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVectorArrayImplList(int nameID, object list);

		public void SetMatrixArray(string name, List<Matrix4x4> values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values);
		}

		public void SetMatrixArray(int nameID, List<Matrix4x4> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetMatrixArrayImplList(nameID, values);
		}

		public void SetMatrixArray(string name, Matrix4x4[] values)
		{
			this.SetMatrixArray(Shader.PropertyToID(name), values);
		}

		public void SetMatrixArray(int nameID, Matrix4x4[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetMatrixArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrixArrayImpl(int nameID, Matrix4x4[] values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrixArrayImplList(int nameID, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
		public void AddFloat(string name, float value)
		{
			this.SetFloat(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", false)]
		public void AddFloat(int nameID, float value)
		{
			this.SetFloat(nameID, value);
		}

		[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
		public void AddVector(string name, Vector4 value)
		{
			this.SetVector(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", false)]
		public void AddVector(int nameID, Vector4 value)
		{
			this.SetVector(nameID, value);
		}

		[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
		public void AddColor(string name, Color value)
		{
			this.SetColor(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", false)]
		public void AddColor(int nameID, Color value)
		{
			this.SetColor(nameID, value);
		}

		[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
		public void AddMatrix(string name, Matrix4x4 value)
		{
			this.SetMatrix(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", false)]
		public void AddMatrix(int nameID, Matrix4x4 value)
		{
			this.SetMatrix(nameID, value);
		}

		[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
		public void AddTexture(string name, Texture value)
		{
			this.SetTexture(Shader.PropertyToID(name), value);
		}

		[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", false)]
		public void AddTexture(int nameID, Texture value)
		{
			this.SetTexture(nameID, value);
		}
	}
}
