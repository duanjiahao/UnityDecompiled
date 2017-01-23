using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class BillboardAsset : Object
	{
		public extern float width
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float bottom
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int imageCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int vertexCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int indexCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public BillboardAsset()
		{
			BillboardAsset.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] BillboardAsset obj);

		public void GetImageTexCoords(List<Vector4> imageTexCoords)
		{
			if (imageTexCoords == null)
			{
				throw new ArgumentNullException("imageTexCoords");
			}
			this.GetImageTexCoordsInternal(imageTexCoords);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector4[] GetImageTexCoords();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetImageTexCoordsInternal(object list);

		public void SetImageTexCoords(List<Vector4> imageTexCoords)
		{
			if (imageTexCoords == null)
			{
				throw new ArgumentNullException("imageTexCoords");
			}
			this.SetImageTexCoordsInternalList(imageTexCoords);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetImageTexCoords(Vector4[] imageTexCoords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetImageTexCoordsInternalList(object list);

		public void GetVertices(List<Vector2> vertices)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			this.GetVerticesInternal(vertices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector2[] GetVertices();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetVerticesInternal(object list);

		public void SetVertices(List<Vector2> vertices)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("vertices");
			}
			this.SetVerticesInternalList(vertices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetVertices(Vector2[] vertices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetVerticesInternalList(object list);

		public void GetIndices(List<ushort> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.GetIndicesInternal(indices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ushort[] GetIndices();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetIndicesInternal(object list);

		public void SetIndices(List<ushort> indices)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			this.SetIndicesInternalList(indices);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetIndices(ushort[] indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIndicesInternalList(object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MakeMaterialProperties(MaterialPropertyBlock properties, Camera camera);
	}
}
