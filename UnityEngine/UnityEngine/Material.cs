using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class Material : Object
	{
		public extern Shader shader
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Color color
		{
			get
			{
				return this.GetColor("_Color");
			}
			set
			{
				this.SetColor("_Color", value);
			}
		}

		public Texture mainTexture
		{
			get
			{
				return this.GetTexture("_MainTex");
			}
			set
			{
				this.SetTexture("_MainTex", value);
			}
		}

		public Vector2 mainTextureOffset
		{
			get
			{
				return this.GetTextureOffset("_MainTex");
			}
			set
			{
				this.SetTextureOffset("_MainTex", value);
			}
		}

		public Vector2 mainTextureScale
		{
			get
			{
				return this.GetTextureScale("_MainTex");
			}
			set
			{
				this.SetTextureScale("_MainTex", value);
			}
		}

		public extern int passCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int renderQueue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] shaderKeywords
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern MaterialGlobalIlluminationFlags globalIlluminationFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Creating materials from shader source string is no longer supported. Use Shader assets instead.")]
		public Material(string contents)
		{
			Material.Internal_CreateWithString(this, contents);
		}

		public Material(Shader shader)
		{
			Material.Internal_CreateWithShader(this, shader);
		}

		public Material(Material source)
		{
			Material.Internal_CreateWithMaterial(this, source);
		}

		public void SetColor(string propertyName, Color color)
		{
			this.SetColor(Shader.PropertyToID(propertyName), color);
		}

		public void SetColor(int nameID, Color color)
		{
			Material.INTERNAL_CALL_SetColor(this, nameID, ref color);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetColor(Material self, int nameID, ref Color color);

		public Color GetColor(string propertyName)
		{
			return this.GetColor(Shader.PropertyToID(propertyName));
		}

		public Color GetColor(int nameID)
		{
			Color result;
			Material.INTERNAL_CALL_GetColor(this, nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetColor(Material self, int nameID, out Color value);

		public void SetVector(string propertyName, Vector4 vector)
		{
			this.SetColor(propertyName, new Color(vector.x, vector.y, vector.z, vector.w));
		}

		public void SetVector(int nameID, Vector4 vector)
		{
			this.SetColor(nameID, new Color(vector.x, vector.y, vector.z, vector.w));
		}

		public Vector4 GetVector(string propertyName)
		{
			Color color = this.GetColor(propertyName);
			return new Vector4(color.r, color.g, color.b, color.a);
		}

		public Vector4 GetVector(int nameID)
		{
			Color color = this.GetColor(nameID);
			return new Vector4(color.r, color.g, color.b, color.a);
		}

		public void SetTexture(string propertyName, Texture texture)
		{
			this.SetTexture(Shader.PropertyToID(propertyName), texture);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTexture(int nameID, Texture texture);

		public Texture GetTexture(string propertyName)
		{
			return this.GetTexture(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Texture GetTexture(int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTextureScaleAndOffset(Material mat, string name, out Vector4 output);

		public void SetTextureOffset(string propertyName, Vector2 offset)
		{
			Material.INTERNAL_CALL_SetTextureOffset(this, propertyName, ref offset);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTextureOffset(Material self, string propertyName, ref Vector2 offset);

		public Vector2 GetTextureOffset(string propertyName)
		{
			Vector4 vector;
			Material.Internal_GetTextureScaleAndOffset(this, propertyName, out vector);
			return new Vector2(vector.z, vector.w);
		}

		public void SetTextureScale(string propertyName, Vector2 scale)
		{
			Material.INTERNAL_CALL_SetTextureScale(this, propertyName, ref scale);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTextureScale(Material self, string propertyName, ref Vector2 scale);

		public Vector2 GetTextureScale(string propertyName)
		{
			Vector4 vector;
			Material.Internal_GetTextureScaleAndOffset(this, propertyName, out vector);
			return new Vector2(vector.x, vector.y);
		}

		public void SetMatrix(string propertyName, Matrix4x4 matrix)
		{
			this.SetMatrix(Shader.PropertyToID(propertyName), matrix);
		}

		public void SetMatrix(int nameID, Matrix4x4 matrix)
		{
			Material.INTERNAL_CALL_SetMatrix(this, nameID, ref matrix);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetMatrix(Material self, int nameID, ref Matrix4x4 matrix);

		public Matrix4x4 GetMatrix(string propertyName)
		{
			return this.GetMatrix(Shader.PropertyToID(propertyName));
		}

		public Matrix4x4 GetMatrix(int nameID)
		{
			Matrix4x4 result;
			Material.INTERNAL_CALL_GetMatrix(this, nameID, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetMatrix(Material self, int nameID, out Matrix4x4 value);

		public void SetFloat(string propertyName, float value)
		{
			this.SetFloat(Shader.PropertyToID(propertyName), value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetFloat(int nameID, float value);

		public float GetFloat(string propertyName)
		{
			return this.GetFloat(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetFloat(int nameID);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatArrayImplList(int nameID, object values);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetVectorArrayImplList(int nameID, object values);

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

		public void SetColorArray(string name, List<Color> values)
		{
			this.SetColorArray(Shader.PropertyToID(name), values);
		}

		public void SetColorArray(int nameID, List<Color> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Count == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetColorArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColorArrayImplList(int nameID, object values);

		public void SetColorArray(string name, Color[] values)
		{
			this.SetColorArray(Shader.PropertyToID(name), values);
		}

		public void SetColorArray(int nameID, Color[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length == 0)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetColorArrayImpl(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetColorArrayImpl(int nameID, Color[] values);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetMatrixArrayImplList(int nameID, object values);

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

		public float[] GetFloatArray(int nameID)
		{
			return this.GetFloatArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float[] GetFloatArrayImpl(int nameID);

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

		public Vector4[] GetVectorArray(int nameID)
		{
			return this.GetVectorArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Vector4[] GetVectorArrayImpl(int nameID);

		public void GetColorArray(string name, List<Color> values)
		{
			this.GetColorArray(Shader.PropertyToID(name), values);
		}

		public void GetColorArray(int nameID, List<Color> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.GetColorArrayImplList(nameID, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetColorArrayImplList(int nameID, object list);

		public Color[] GetColorArray(string name)
		{
			return this.GetColorArray(Shader.PropertyToID(name));
		}

		public Color[] GetColorArray(int nameID)
		{
			return this.GetColorArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Color[] GetColorArrayImpl(int nameID);

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

		public Matrix4x4[] GetMatrixArray(int nameID)
		{
			return this.GetMatrixArrayImpl(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Matrix4x4[] GetMatrixArrayImpl(int nameID);

		public void SetInt(string propertyName, int value)
		{
			this.SetFloat(propertyName, (float)value);
		}

		public void SetInt(int nameID, int value)
		{
			this.SetFloat(nameID, (float)value);
		}

		public int GetInt(string propertyName)
		{
			return (int)this.GetFloat(propertyName);
		}

		public int GetInt(int nameID)
		{
			return (int)this.GetFloat(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBuffer(string propertyName, ComputeBuffer buffer);

		public bool HasProperty(string propertyName)
		{
			return this.HasProperty(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasProperty(int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetTag(string tag, bool searchFallbacks, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public string GetTag(string tag, bool searchFallbacks)
		{
			string defaultValue = "";
			return this.GetTag(tag, searchFallbacks, defaultValue);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetOverrideTag(string tag, string val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Lerp(Material start, Material end, float t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetPass(int pass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetPassName(int pass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int FindPass(string passName);

		[Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.")]
		public static Material Create(string scriptContents)
		{
			return new Material(scriptContents);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateWithString([Writable] Material mono, string contents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateWithShader([Writable] Material mono, Shader shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateWithMaterial([Writable] Material mono, Material source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CopyPropertiesFromMaterial(Material mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EnableKeyword(string keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisableKeyword(string keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsKeywordEnabled(string keyword);
	}
}
