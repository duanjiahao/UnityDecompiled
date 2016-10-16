using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class CanvasRenderer : Component
	{
		public delegate void OnRequestRebuild();

		public static event CanvasRenderer.OnRequestRebuild onRequestRebuild
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				CanvasRenderer.onRequestRebuild = (CanvasRenderer.OnRequestRebuild)Delegate.Combine(CanvasRenderer.onRequestRebuild, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				CanvasRenderer.onRequestRebuild = (CanvasRenderer.OnRequestRebuild)Delegate.Remove(CanvasRenderer.onRequestRebuild, value);
			}
		}

		[Obsolete("isMask is no longer supported. See EnableClipping for vertex clipping configuration")]
		public extern bool isMask
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasRectClipping
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasPopInstruction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int materialCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int popMaterialCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int relativeDepth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool cull
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int absoluteDepth
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasMoved
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public void SetColor(Color color)
		{
			CanvasRenderer.INTERNAL_CALL_SetColor(this, ref color);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetColor(CanvasRenderer self, ref Color color);

		public Color GetColor()
		{
			Color result;
			CanvasRenderer.INTERNAL_CALL_GetColor(this, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetColor(CanvasRenderer self, out Color value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetAlpha();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAlpha(float alpha);

		[Obsolete("UI System now uses meshes. Generate a mesh and use 'SetMesh' instead")]
		public void SetVertices(List<UIVertex> vertices)
		{
			this.SetVertices(vertices.ToArray(), vertices.Count);
		}

		[Obsolete("UI System now uses meshes. Generate a mesh and use 'SetMesh' instead")]
		public void SetVertices(UIVertex[] vertices, int size)
		{
			Mesh mesh = new Mesh();
			List<Vector3> list = new List<Vector3>();
			List<Color32> list2 = new List<Color32>();
			List<Vector2> list3 = new List<Vector2>();
			List<Vector2> list4 = new List<Vector2>();
			List<Vector3> list5 = new List<Vector3>();
			List<Vector4> list6 = new List<Vector4>();
			List<int> list7 = new List<int>();
			for (int i = 0; i < size; i += 4)
			{
				for (int j = 0; j < 4; j++)
				{
					list.Add(vertices[i + j].position);
					list2.Add(vertices[i + j].color);
					list3.Add(vertices[i + j].uv0);
					list4.Add(vertices[i + j].uv1);
					list5.Add(vertices[i + j].normal);
					list6.Add(vertices[i + j].tangent);
				}
				list7.Add(i);
				list7.Add(i + 1);
				list7.Add(i + 2);
				list7.Add(i + 2);
				list7.Add(i + 3);
				list7.Add(i);
			}
			mesh.SetVertices(list);
			mesh.SetColors(list2);
			mesh.SetNormals(list5);
			mesh.SetTangents(list6);
			mesh.SetUVs(0, list3);
			mesh.SetUVs(1, list4);
			mesh.SetIndices(list7.ToArray(), MeshTopology.Triangles, 0);
			this.SetMesh(mesh);
			Object.DestroyImmediate(mesh);
		}

		public void EnableRectClipping(Rect rect)
		{
			CanvasRenderer.INTERNAL_CALL_EnableRectClipping(this, ref rect);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_EnableRectClipping(CanvasRenderer self, ref Rect rect);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DisableRectClipping();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMaterial(Material material, int index);

		public void SetMaterial(Material material, Texture texture)
		{
			this.materialCount = Math.Max(1, this.materialCount);
			this.SetMaterial(material, 0);
			this.SetTexture(texture);
		}

		public Material GetMaterial()
		{
			return this.GetMaterial(0);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Material GetMaterial(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPopMaterial(Material material, int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Material GetPopMaterial(int index);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTexture(Texture texture);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAlphaTexture(Texture texture);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMesh(Mesh mesh);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Clear();

		public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indicies)
		{
			CanvasRenderer.SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, normals, tangents);
			CanvasRenderer.SplitIndiciesStreamsInternal(verts, indicies);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SplitUIVertexStreamsInternal(object verts, object positions, object colors, object uv0S, object uv1S, object normals, object tangents);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SplitIndiciesStreamsInternal(object verts, object indicies);

		public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indicies)
		{
			CanvasRenderer.CreateUIVertexStreamInternal(verts, positions, colors, uv0S, uv1S, normals, tangents, indicies);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateUIVertexStreamInternal(object verts, object positions, object colors, object uv0S, object uv1S, object normals, object tangents, object indicies);

		public static void AddUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector2> uv0S, List<Vector2> uv1S, List<Vector3> normals, List<Vector4> tangents)
		{
			CanvasRenderer.SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, normals, tangents);
		}

		private static void RequestRefresh()
		{
			if (CanvasRenderer.onRequestRebuild != null)
			{
				CanvasRenderer.onRequestRebuild();
			}
		}
	}
}
