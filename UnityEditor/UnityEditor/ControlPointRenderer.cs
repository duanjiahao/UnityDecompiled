using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class ControlPointRenderer
	{
		private class RenderChunk
		{
			public Mesh mesh;

			public List<Vector3> vertices;

			public List<Color32> colors;

			public List<Vector2> uvs;

			public List<int> indices;

			public bool isDirty = true;
		}

		private List<ControlPointRenderer.RenderChunk> m_RenderChunks = new List<ControlPointRenderer.RenderChunk>();

		private Texture2D m_Icon;

		private const int kMaxVertices = 65000;

		private const string kControlPointRendererMeshName = "ControlPointRendererMesh";

		private static Material s_Material;

		public static Material material
		{
			get
			{
				if (!ControlPointRenderer.s_Material)
				{
					Shader shader = (Shader)EditorGUIUtility.LoadRequired("Editors/AnimationWindow/ControlPoint.shader");
					ControlPointRenderer.s_Material = new Material(shader);
				}
				return ControlPointRenderer.s_Material;
			}
		}

		public ControlPointRenderer(Texture2D icon)
		{
			this.m_Icon = icon;
		}

		public void FlushCache()
		{
			for (int i = 0; i < this.m_RenderChunks.Count; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.m_RenderChunks[i].mesh);
			}
			this.m_RenderChunks.Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_RenderChunks.Count; i++)
			{
				ControlPointRenderer.RenderChunk renderChunk = this.m_RenderChunks[i];
				renderChunk.mesh.Clear();
				renderChunk.vertices.Clear();
				renderChunk.colors.Clear();
				renderChunk.uvs.Clear();
				renderChunk.indices.Clear();
				renderChunk.isDirty = true;
			}
		}

		public void Render()
		{
			Material material = ControlPointRenderer.material;
			material.SetTexture("_MainTex", this.m_Icon);
			material.SetPass(0);
			for (int i = 0; i < this.m_RenderChunks.Count; i++)
			{
				ControlPointRenderer.RenderChunk renderChunk = this.m_RenderChunks[i];
				if (renderChunk.isDirty)
				{
					renderChunk.mesh.vertices = renderChunk.vertices.ToArray();
					renderChunk.mesh.colors32 = renderChunk.colors.ToArray();
					renderChunk.mesh.uv = renderChunk.uvs.ToArray();
					renderChunk.mesh.SetIndices(renderChunk.indices.ToArray(), MeshTopology.Triangles, 0);
					renderChunk.isDirty = false;
				}
				Graphics.DrawMeshNow(renderChunk.mesh, Handles.matrix);
			}
		}

		public void AddPoint(Rect rect, Color color)
		{
			ControlPointRenderer.RenderChunk renderChunk = this.GetRenderChunk();
			int count = renderChunk.vertices.Count;
			renderChunk.vertices.Add(new Vector3(rect.xMin, rect.yMin, 0f));
			renderChunk.vertices.Add(new Vector3(rect.xMax, rect.yMin, 0f));
			renderChunk.vertices.Add(new Vector3(rect.xMax, rect.yMax, 0f));
			renderChunk.vertices.Add(new Vector3(rect.xMin, rect.yMax, 0f));
			renderChunk.colors.Add(color);
			renderChunk.colors.Add(color);
			renderChunk.colors.Add(color);
			renderChunk.colors.Add(color);
			renderChunk.uvs.Add(new Vector2(0f, 0f));
			renderChunk.uvs.Add(new Vector2(1f, 0f));
			renderChunk.uvs.Add(new Vector2(1f, 1f));
			renderChunk.uvs.Add(new Vector2(0f, 1f));
			renderChunk.indices.Add(count);
			renderChunk.indices.Add(count + 1);
			renderChunk.indices.Add(count + 2);
			renderChunk.indices.Add(count);
			renderChunk.indices.Add(count + 2);
			renderChunk.indices.Add(count + 3);
			renderChunk.isDirty = true;
		}

		private ControlPointRenderer.RenderChunk GetRenderChunk()
		{
			ControlPointRenderer.RenderChunk renderChunk;
			if (this.m_RenderChunks.Count > 0)
			{
				renderChunk = this.m_RenderChunks.Last<ControlPointRenderer.RenderChunk>();
				if (renderChunk.vertices.Count + 4 > 65000)
				{
					renderChunk = this.CreateRenderChunk();
				}
			}
			else
			{
				renderChunk = this.CreateRenderChunk();
			}
			return renderChunk;
		}

		private ControlPointRenderer.RenderChunk CreateRenderChunk()
		{
			ControlPointRenderer.RenderChunk renderChunk = new ControlPointRenderer.RenderChunk();
			renderChunk.mesh = new Mesh();
			renderChunk.mesh.name = "ControlPointRendererMesh";
			renderChunk.mesh.hideFlags |= HideFlags.DontSave;
			renderChunk.vertices = new List<Vector3>();
			renderChunk.colors = new List<Color32>();
			renderChunk.uvs = new List<Vector2>();
			renderChunk.indices = new List<int>();
			this.m_RenderChunks.Add(renderChunk);
			return renderChunk;
		}
	}
}
