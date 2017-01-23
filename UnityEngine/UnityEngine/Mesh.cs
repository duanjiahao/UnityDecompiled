using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Mesh : Object
	{
		internal enum InternalShaderChannel
		{
			Vertex,
			Normal,
			Color,
			TexCoord0,
			TexCoord1,
			TexCoord2,
			TexCoord3,
			Tangent
		}

		internal enum InternalVertexChannelType
		{
			Float,
			Color = 2
		}

		public extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool canAccess
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int blendShapeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int vertexBufferCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Bounds bounds
		{
			get
			{
				Bounds result;
				this.INTERNAL_get_bounds(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_bounds(ref value);
			}
		}

		public extern int vertexCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int subMeshCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern BoneWeight[] boneWeights
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Matrix4x4[] bindposes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property Mesh.uv1 has been deprecated. Use Mesh.uv2 instead (UnityUpgradable) -> uv2", true)]
		public Vector2[] uv1
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public Vector3[] vertices
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector3>(Mesh.InternalShaderChannel.Vertex);
			}
			set
			{
				this.SetArrayForChannel<Vector3>(Mesh.InternalShaderChannel.Vertex, value);
			}
		}

		public Vector3[] normals
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector3>(Mesh.InternalShaderChannel.Normal);
			}
			set
			{
				this.SetArrayForChannel<Vector3>(Mesh.InternalShaderChannel.Normal, value);
			}
		}

		public Vector4[] tangents
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector4>(Mesh.InternalShaderChannel.Tangent);
			}
			set
			{
				this.SetArrayForChannel<Vector4>(Mesh.InternalShaderChannel.Tangent, value);
			}
		}

		public Vector2[] uv
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord0);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord0, value);
			}
		}

		public Vector2[] uv2
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord1);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord1, value);
			}
		}

		public Vector2[] uv3
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord2);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord2, value);
			}
		}

		public Vector2[] uv4
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord3);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord3, value);
			}
		}

		public Color[] colors
		{
			get
			{
				return this.GetAllocArrayFromChannel<Color>(Mesh.InternalShaderChannel.Color);
			}
			set
			{
				this.SetArrayForChannel<Color>(Mesh.InternalShaderChannel.Color, value);
			}
		}

		public Color32[] colors32
		{
			get
			{
				return this.GetAllocArrayFromChannel<Color32>(Mesh.InternalShaderChannel.Color, Mesh.InternalVertexChannelType.Color, 1);
			}
			set
			{
				this.SetArrayForChannel<Color32>(Mesh.InternalShaderChannel.Color, Mesh.InternalVertexChannelType.Color, 1, value);
			}
		}

		public int[] triangles
		{
			get
			{
				int[] result;
				if (this.canAccess)
				{
					result = this.GetTrianglesImpl(-1);
				}
				else
				{
					this.PrintErrorCantAccessMeshForIndices();
					result = new int[0];
				}
				return result;
			}
			set
			{
				if (this.canAccess)
				{
					this.SetTrianglesImpl(-1, value, this.SafeLength(value));
				}
				else
				{
					this.PrintErrorCantAccessMeshForIndices();
				}
			}
		}

		public Mesh()
		{
			Mesh.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Mesh mono);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear([UnityEngine.Internal.DefaultValue("true")] bool keepVertexLayout);

		[ExcludeFromDocs]
		public void Clear()
		{
			bool keepVertexLayout = true;
			this.Clear(keepVertexLayout);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void PrintErrorCantAccessMesh(Mesh.InternalShaderChannel channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void PrintErrorCantAccessMeshForIndices();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void PrintErrorBadSubmeshIndexTriangles();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void PrintErrorBadSubmeshIndexIndices();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetArrayForChannelImpl(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, Array values, int arraySize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Array GetAllocArrayFromChannelImpl(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetArrayFromChannelImpl(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, Array values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasChannel(Mesh.InternalShaderChannel channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResizeList(object list, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Array ExtractArrayFromList(object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int[] GetTrianglesImpl(int submesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int[] GetIndicesImpl(int submesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTrianglesImpl(int submesh, Array triangles, int arraySize, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds);

		[ExcludeFromDocs]
		private void SetTrianglesImpl(int submesh, Array triangles, int arraySize)
		{
			bool calculateBounds = true;
			this.SetTrianglesImpl(submesh, triangles, arraySize, calculateBounds);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds);

		[ExcludeFromDocs]
		private void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize)
		{
			bool calculateBounds = true;
			this.SetIndicesImpl(submesh, topology, indices, arraySize, calculateBounds);
		}

		[ExcludeFromDocs]
		public void SetTriangles(int[] triangles, int submesh)
		{
			bool calculateBounds = true;
			this.SetTriangles(triangles, submesh, calculateBounds);
		}

		public void SetTriangles(int[] triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
		{
			if (this.CheckCanAccessSubmeshTriangles(submesh))
			{
				this.SetTrianglesImpl(submesh, triangles, this.SafeLength(triangles), calculateBounds);
			}
		}

		[ExcludeFromDocs]
		public void SetTriangles(List<int> triangles, int submesh)
		{
			bool calculateBounds = true;
			this.SetTriangles(triangles, submesh, calculateBounds);
		}

		public void SetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
		{
			if (this.CheckCanAccessSubmeshTriangles(submesh))
			{
				this.SetTrianglesImpl(submesh, Mesh.ExtractArrayFromList(triangles), this.SafeLength<int>(triangles), calculateBounds);
			}
		}

		[ExcludeFromDocs]
		public void SetIndices(int[] indices, MeshTopology topology, int submesh)
		{
			bool calculateBounds = true;
			this.SetIndices(indices, topology, submesh, calculateBounds);
		}

		public void SetIndices(int[] indices, MeshTopology topology, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds)
		{
			if (this.CheckCanAccessSubmeshIndices(submesh))
			{
				this.SetIndicesImpl(submesh, topology, indices, this.SafeLength(indices), calculateBounds);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearBlendShapes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetBlendShapeName(int shapeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetBlendShapeFrameCount(int shapeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

		public IntPtr GetNativeVertexBufferPtr(int bufferIndex)
		{
			IntPtr result;
			Mesh.INTERNAL_CALL_GetNativeVertexBufferPtr(this, bufferIndex, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeVertexBufferPtr(Mesh self, int bufferIndex, out IntPtr value);

		public IntPtr GetNativeIndexBufferPtr()
		{
			IntPtr result;
			Mesh.INTERNAL_CALL_GetNativeIndexBufferPtr(this, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeIndexBufferPtr(Mesh self, out IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bounds(out Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_bounds(ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateBounds();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateNormals();

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("This method is no longer supported (UnityUpgradable)", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Optimize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MeshTopology GetTopology(int submesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CombineMeshes(CombineInstance[] combine, [UnityEngine.Internal.DefaultValue("true")] bool mergeSubMeshes, [UnityEngine.Internal.DefaultValue("true")] bool useMatrices);

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
		{
			bool useMatrices = true;
			this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
		}

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine)
		{
			bool useMatrices = true;
			bool mergeSubMeshes = true;
			this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void MarkDynamic();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void UploadMeshData(bool markNoLogerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetBlendShapeIndex(string blendShapeName);

		internal Mesh.InternalShaderChannel GetUVChannel(int uvIndex)
		{
			if (uvIndex < 0 || uvIndex > 3)
			{
				throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
			}
			return Mesh.InternalShaderChannel.TexCoord0 + uvIndex;
		}

		internal static int DefaultDimensionForChannel(Mesh.InternalShaderChannel channel)
		{
			int result;
			if (channel == Mesh.InternalShaderChannel.Vertex || channel == Mesh.InternalShaderChannel.Normal)
			{
				result = 3;
			}
			else if (channel >= Mesh.InternalShaderChannel.TexCoord0 && channel <= Mesh.InternalShaderChannel.TexCoord3)
			{
				result = 2;
			}
			else
			{
				if (channel != Mesh.InternalShaderChannel.Tangent && channel != Mesh.InternalShaderChannel.Color)
				{
					throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
				}
				result = 4;
			}
			return result;
		}

		private T[] GetAllocArrayFromChannel<T>(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim)
		{
			T[] result;
			if (this.canAccess)
			{
				if (this.HasChannel(channel))
				{
					result = (T[])this.GetAllocArrayFromChannelImpl(channel, format, dim);
					return result;
				}
			}
			else
			{
				this.PrintErrorCantAccessMesh(channel);
			}
			result = new T[0];
			return result;
		}

		private T[] GetAllocArrayFromChannel<T>(Mesh.InternalShaderChannel channel)
		{
			return this.GetAllocArrayFromChannel<T>(channel, Mesh.InternalVertexChannelType.Float, Mesh.DefaultDimensionForChannel(channel));
		}

		private int SafeLength(Array values)
		{
			return (values == null) ? 0 : values.Length;
		}

		private int SafeLength<T>(List<T> values)
		{
			return (values == null) ? 0 : values.Count;
		}

		private void SetSizedArrayForChannel(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, Array values, int valuesCount)
		{
			if (this.canAccess)
			{
				this.SetArrayForChannelImpl(channel, format, dim, values, valuesCount);
			}
			else
			{
				this.PrintErrorCantAccessMesh(channel);
			}
		}

		private void SetArrayForChannel<T>(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, T[] values)
		{
			this.SetSizedArrayForChannel(channel, format, dim, values, this.SafeLength(values));
		}

		private void SetArrayForChannel<T>(Mesh.InternalShaderChannel channel, T[] values)
		{
			this.SetSizedArrayForChannel(channel, Mesh.InternalVertexChannelType.Float, Mesh.DefaultDimensionForChannel(channel), values, this.SafeLength(values));
		}

		private void SetListForChannel<T>(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, List<T> values)
		{
			this.SetSizedArrayForChannel(channel, format, dim, Mesh.ExtractArrayFromList(values), this.SafeLength<T>(values));
		}

		private void SetListForChannel<T>(Mesh.InternalShaderChannel channel, List<T> values)
		{
			this.SetSizedArrayForChannel(channel, Mesh.InternalVertexChannelType.Float, Mesh.DefaultDimensionForChannel(channel), Mesh.ExtractArrayFromList(values), this.SafeLength<T>(values));
		}

		public void SetVertices(List<Vector3> inVertices)
		{
			this.SetListForChannel<Vector3>(Mesh.InternalShaderChannel.Vertex, inVertices);
		}

		public void SetNormals(List<Vector3> inNormals)
		{
			this.SetListForChannel<Vector3>(Mesh.InternalShaderChannel.Normal, inNormals);
		}

		public void SetTangents(List<Vector4> inTangents)
		{
			this.SetListForChannel<Vector4>(Mesh.InternalShaderChannel.Tangent, inTangents);
		}

		public void SetColors(List<Color> inColors)
		{
			this.SetListForChannel<Color>(Mesh.InternalShaderChannel.Color, inColors);
		}

		public void SetColors(List<Color32> inColors)
		{
			this.SetListForChannel<Color32>(Mesh.InternalShaderChannel.Color, Mesh.InternalVertexChannelType.Color, 1, inColors);
		}

		private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs)
		{
			if (uvIndex < 0 || uvIndex > 3)
			{
				Debug.LogError("The uv index is invalid (must be in [0..3]");
			}
			else
			{
				this.SetListForChannel<T>(this.GetUVChannel(uvIndex), Mesh.InternalVertexChannelType.Float, dim, uvs);
			}
		}

		public void SetUVs(int channel, List<Vector2> uvs)
		{
			this.SetUvsImpl<Vector2>(channel, 2, uvs);
		}

		public void SetUVs(int channel, List<Vector3> uvs)
		{
			this.SetUvsImpl<Vector3>(channel, 3, uvs);
		}

		public void SetUVs(int channel, List<Vector4> uvs)
		{
			this.SetUvsImpl<Vector4>(channel, 4, uvs);
		}

		private void GetUVsInternal<T>(List<T> uvs, int uvIndex, int dim)
		{
			Mesh.InternalShaderChannel uVChannel = this.GetUVChannel(uvIndex);
			Mesh.ResizeList(uvs, this.vertexCount);
			this.GetArrayFromChannelImpl(uVChannel, Mesh.InternalVertexChannelType.Float, dim, Mesh.ExtractArrayFromList(uvs));
		}

		private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
		{
			if (uvs == null)
			{
				throw new ArgumentException("The result uvs list cannot be null", "uvs");
			}
			if (uvIndex < 0 || uvIndex > 3)
			{
				throw new ArgumentException("The uv index is invalid (must be in [0..3]", "uvIndex");
			}
			uvs.Clear();
			Mesh.InternalShaderChannel uVChannel = this.GetUVChannel(uvIndex);
			if (!this.canAccess)
			{
				this.PrintErrorCantAccessMesh(uVChannel);
			}
			else if (this.HasChannel(uVChannel))
			{
				if (this.vertexCount > uvs.Capacity)
				{
					uvs.Capacity = this.vertexCount;
				}
				this.GetUVsInternal<T>(uvs, uvIndex, dim);
			}
		}

		public void GetUVs(int channel, List<Vector2> uvs)
		{
			this.GetUVsImpl<Vector2>(channel, uvs, 2);
		}

		public void GetUVs(int channel, List<Vector3> uvs)
		{
			this.GetUVsImpl<Vector3>(channel, uvs, 3);
		}

		public void GetUVs(int channel, List<Vector4> uvs)
		{
			this.GetUVsImpl<Vector4>(channel, uvs, 4);
		}

		private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
		{
			bool result;
			if (!this.canAccess)
			{
				this.PrintErrorCantAccessMeshForIndices();
				result = false;
			}
			else if (submesh < 0 || submesh >= this.subMeshCount)
			{
				if (errorAboutTriangles)
				{
					this.PrintErrorBadSubmeshIndexTriangles();
				}
				else
				{
					this.PrintErrorBadSubmeshIndexIndices();
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		private bool CheckCanAccessSubmeshTriangles(int submesh)
		{
			return this.CheckCanAccessSubmesh(submesh, true);
		}

		private bool CheckCanAccessSubmeshIndices(int submesh)
		{
			return this.CheckCanAccessSubmesh(submesh, false);
		}

		public int[] GetTriangles(int submesh)
		{
			return (!this.CheckCanAccessSubmeshTriangles(submesh)) ? new int[0] : this.GetTrianglesImpl(submesh);
		}

		public int[] GetIndices(int submesh)
		{
			return (!this.CheckCanAccessSubmeshIndices(submesh)) ? new int[0] : this.GetIndicesImpl(submesh);
		}
	}
}
