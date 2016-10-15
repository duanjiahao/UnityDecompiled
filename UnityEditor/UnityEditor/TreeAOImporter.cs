using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TreeAOImporter : AssetPostprocessor
	{
		private void OnPostprocessModel(GameObject root)
		{
			string text = base.assetPath.ToLower();
			if (text.IndexOf("ambient-occlusion") != -1)
			{
				Component[] componentsInChildren = root.GetComponentsInChildren(typeof(MeshFilter));
				Component[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					MeshFilter meshFilter = (MeshFilter)array[i];
					if (meshFilter.sharedMesh != null)
					{
						Mesh sharedMesh = meshFilter.sharedMesh;
						TreeAO.CalcSoftOcclusion(sharedMesh);
						Bounds bounds = sharedMesh.bounds;
						Color[] array2 = sharedMesh.colors;
						Vector3[] vertices = sharedMesh.vertices;
						Vector4[] tangents = sharedMesh.tangents;
						if (array2.Length == 0)
						{
							array2 = new Color[sharedMesh.vertexCount];
							for (int j = 0; j < array2.Length; j++)
							{
								array2[j] = Color.white;
							}
						}
						float b = 0f;
						for (int k = 0; k < tangents.Length; k++)
						{
							b = Mathf.Max(tangents[k].w, b);
						}
						float num = 0f;
						for (int l = 0; l < array2.Length; l++)
						{
							Vector2 vector = new Vector2(vertices[l].x, vertices[l].z);
							float magnitude = vector.magnitude;
							num = Mathf.Max(magnitude, num);
						}
						for (int m = 0; m < array2.Length; m++)
						{
							Vector2 vector2 = new Vector2(vertices[m].x, vertices[m].z);
							float num2 = vector2.magnitude / num;
							float num3 = (vertices[m].y - bounds.min.y) / bounds.size.y;
							array2[m].a = num3 * num2 * 0.6f + num3 * 0.5f;
						}
						sharedMesh.colors = array2;
					}
				}
			}
		}
	}
}
