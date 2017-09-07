using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	public class VertexHelper : IDisposable
	{
		private List<Vector3> m_Positions = ListPool<Vector3>.Get();

		private List<Color32> m_Colors = ListPool<Color32>.Get();

		private List<Vector2> m_Uv0S = ListPool<Vector2>.Get();

		private List<Vector2> m_Uv1S = ListPool<Vector2>.Get();

		private List<Vector2> m_Uv2S = ListPool<Vector2>.Get();

		private List<Vector2> m_Uv3S = ListPool<Vector2>.Get();

		private List<Vector3> m_Normals = ListPool<Vector3>.Get();

		private List<Vector4> m_Tangents = ListPool<Vector4>.Get();

		private List<int> m_Indices = ListPool<int>.Get();

		private static readonly Vector4 s_DefaultTangent = new Vector4(1f, 0f, 0f, -1f);

		private static readonly Vector3 s_DefaultNormal = Vector3.back;

		public int currentVertCount
		{
			get
			{
				return this.m_Positions.Count;
			}
		}

		public int currentIndexCount
		{
			get
			{
				return this.m_Indices.Count;
			}
		}

		public VertexHelper()
		{
		}

		public VertexHelper(Mesh m)
		{
			this.m_Positions.AddRange(m.vertices);
			this.m_Colors.AddRange(m.colors32);
			this.m_Uv0S.AddRange(m.uv);
			this.m_Uv1S.AddRange(m.uv2);
			this.m_Uv2S.AddRange(m.uv3);
			this.m_Uv3S.AddRange(m.uv4);
			this.m_Normals.AddRange(m.normals);
			this.m_Tangents.AddRange(m.tangents);
			this.m_Indices.AddRange(m.GetIndices(0));
		}

		public void Clear()
		{
			this.m_Positions.Clear();
			this.m_Colors.Clear();
			this.m_Uv0S.Clear();
			this.m_Uv1S.Clear();
			this.m_Uv2S.Clear();
			this.m_Uv3S.Clear();
			this.m_Normals.Clear();
			this.m_Tangents.Clear();
			this.m_Indices.Clear();
		}

		public void PopulateUIVertex(ref UIVertex vertex, int i)
		{
			vertex.position = this.m_Positions[i];
			vertex.color = this.m_Colors[i];
			vertex.uv0 = this.m_Uv0S[i];
			vertex.uv1 = this.m_Uv1S[i];
			vertex.uv2 = this.m_Uv2S[i];
			vertex.uv3 = this.m_Uv3S[i];
			vertex.normal = this.m_Normals[i];
			vertex.tangent = this.m_Tangents[i];
		}

		public void SetUIVertex(UIVertex vertex, int i)
		{
			this.m_Positions[i] = vertex.position;
			this.m_Colors[i] = vertex.color;
			this.m_Uv0S[i] = vertex.uv0;
			this.m_Uv1S[i] = vertex.uv1;
			this.m_Uv2S[i] = vertex.uv2;
			this.m_Uv3S[i] = vertex.uv3;
			this.m_Normals[i] = vertex.normal;
			this.m_Tangents[i] = vertex.tangent;
		}

		public void FillMesh(Mesh mesh)
		{
			mesh.Clear();
			if (this.m_Positions.Count >= 65000)
			{
				throw new ArgumentException("Mesh can not have more than 65000 vertices");
			}
			mesh.SetVertices(this.m_Positions);
			mesh.SetColors(this.m_Colors);
			mesh.SetUVs(0, this.m_Uv0S);
			mesh.SetUVs(1, this.m_Uv1S);
			mesh.SetUVs(2, this.m_Uv2S);
			mesh.SetUVs(3, this.m_Uv3S);
			mesh.SetNormals(this.m_Normals);
			mesh.SetTangents(this.m_Tangents);
			mesh.SetTriangles(this.m_Indices, 0);
			mesh.RecalculateBounds();
		}

		public void Dispose()
		{
			ListPool<Vector3>.Release(this.m_Positions);
			ListPool<Color32>.Release(this.m_Colors);
			ListPool<Vector2>.Release(this.m_Uv0S);
			ListPool<Vector2>.Release(this.m_Uv1S);
			ListPool<Vector2>.Release(this.m_Uv2S);
			ListPool<Vector2>.Release(this.m_Uv3S);
			ListPool<Vector3>.Release(this.m_Normals);
			ListPool<Vector4>.Release(this.m_Tangents);
			ListPool<int>.Release(this.m_Indices);
			this.m_Positions = null;
			this.m_Colors = null;
			this.m_Uv0S = null;
			this.m_Uv1S = null;
			this.m_Uv2S = null;
			this.m_Uv3S = null;
			this.m_Normals = null;
			this.m_Tangents = null;
			this.m_Indices = null;
		}

		public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1, Vector3 normal, Vector4 tangent)
		{
			this.m_Positions.Add(position);
			this.m_Colors.Add(color);
			this.m_Uv0S.Add(uv0);
			this.m_Uv1S.Add(uv1);
			this.m_Uv2S.Add(Vector2.zero);
			this.m_Uv3S.Add(Vector2.zero);
			this.m_Normals.Add(normal);
			this.m_Tangents.Add(tangent);
		}

		public void AddVert(Vector3 position, Color32 color, Vector2 uv0)
		{
			this.AddVert(position, color, uv0, Vector2.zero, VertexHelper.s_DefaultNormal, VertexHelper.s_DefaultTangent);
		}

		public void AddVert(UIVertex v)
		{
			this.AddVert(v.position, v.color, v.uv0, v.uv1, v.normal, v.tangent);
		}

		public void AddTriangle(int idx0, int idx1, int idx2)
		{
			this.m_Indices.Add(idx0);
			this.m_Indices.Add(idx1);
			this.m_Indices.Add(idx2);
		}

		public void AddUIVertexQuad(UIVertex[] verts)
		{
			int currentVertCount = this.currentVertCount;
			for (int i = 0; i < 4; i++)
			{
				this.AddVert(verts[i].position, verts[i].color, verts[i].uv0, verts[i].uv1, verts[i].normal, verts[i].tangent);
			}
			this.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			this.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		public void AddUIVertexStream(List<UIVertex> verts, List<int> indices)
		{
			if (verts != null)
			{
				CanvasRenderer.AddUIVertexStream(verts, this.m_Positions, this.m_Colors, this.m_Uv0S, this.m_Uv1S, this.m_Uv2S, this.m_Uv3S, this.m_Normals, this.m_Tangents);
			}
			if (indices != null)
			{
				this.m_Indices.AddRange(indices);
			}
		}

		public void AddUIVertexTriangleStream(List<UIVertex> verts)
		{
			if (verts != null)
			{
				CanvasRenderer.SplitUIVertexStreams(verts, this.m_Positions, this.m_Colors, this.m_Uv0S, this.m_Uv1S, this.m_Uv2S, this.m_Uv3S, this.m_Normals, this.m_Tangents, this.m_Indices);
			}
		}

		public void GetUIVertexStream(List<UIVertex> stream)
		{
			if (stream != null)
			{
				CanvasRenderer.CreateUIVertexStream(stream, this.m_Positions, this.m_Colors, this.m_Uv0S, this.m_Uv1S, this.m_Uv2S, this.m_Uv3S, this.m_Normals, this.m_Tangents, this.m_Indices);
			}
		}
	}
}
