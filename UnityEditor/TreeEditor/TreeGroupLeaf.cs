using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class TreeGroupLeaf : TreeGroup
	{
		public enum GeometryMode
		{
			PLANE,
			CROSS,
			TRI_CROSS,
			BILLBOARD,
			MESH
		}
		internal static Dictionary<Texture, Vector2[]> s_TextureHulls;
		internal static bool s_TextureHullsDirty;
		public int geometryMode;
		public Material materialLeaf;
		public GameObject instanceMesh;
		public Vector2 size = Vector2.one;
		public float perpendicularAlign;
		public float horizontalAlign;
		private static Mesh cloneMesh;
		private static MeshFilter cloneMeshFilter;
		private static Renderer cloneRenderer;
		private static Vector3[] cloneVerts;
		private static Vector3[] cloneNormals;
		private static Vector2[] cloneUVs;
		private static Vector4[] cloneTangents;
		public override bool CanHaveSubGroups()
		{
			return false;
		}
		internal override bool HasExternalChanges()
		{
			string text;
			if (this.geometryMode == 4)
			{
				text = InternalEditorUtility.CalculateHashForObjectsAndDependencies(new UnityEngine.Object[]
				{
					this.instanceMesh
				});
			}
			else
			{
				text = InternalEditorUtility.CalculateHashForObjectsAndDependencies(new UnityEngine.Object[]
				{
					this.materialLeaf
				});
			}
			if (text != this.m_Hash)
			{
				this.m_Hash = text;
				return true;
			}
			return false;
		}
		public override void UpdateParameters()
		{
			if (this.lockFlags == 0)
			{
				for (int i = 0; i < this.nodes.Count; i++)
				{
					TreeNode treeNode = this.nodes[i];
					UnityEngine.Random.seed = treeNode.seed;
					for (int j = 0; j < 5; j++)
					{
						treeNode.scale *= this.size.x + (this.size.y - this.size.x) * UnityEngine.Random.value;
					}
					for (int k = 0; k < 5; k++)
					{
						float x = (UnityEngine.Random.value - 0.5f) * 180f * (1f - this.perpendicularAlign);
						float y = (UnityEngine.Random.value - 0.5f) * 180f * (1f - this.perpendicularAlign);
						float z = 0f;
						treeNode.rotation = Quaternion.Euler(x, y, z);
					}
				}
			}
			this.UpdateMatrix();
			base.UpdateParameters();
		}
		public override void UpdateMatrix()
		{
			if (this.parentGroup == null)
			{
				for (int i = 0; i < this.nodes.Count; i++)
				{
					TreeNode treeNode = this.nodes[i];
					treeNode.matrix = Matrix4x4.identity;
				}
			}
			else
			{
				TreeGroupRoot treeGroupRoot = this.parentGroup as TreeGroupRoot;
				for (int j = 0; j < this.nodes.Count; j++)
				{
					TreeNode treeNode2 = this.nodes[j];
					Vector3 pos = default(Vector3);
					Quaternion q = default(Quaternion);
					float num = 0f;
					float num2 = 0f;
					if (treeGroupRoot != null)
					{
						float num3 = treeNode2.offset * base.GetRootSpread();
						float f = treeNode2.angle * 0.0174532924f;
						pos = new Vector3(Mathf.Cos(f) * num3, -treeGroupRoot.groundOffset, Mathf.Sin(f) * num3);
						q = Quaternion.Euler(treeNode2.pitch * -Mathf.Sin(f), 0f, treeNode2.pitch * Mathf.Cos(f)) * Quaternion.Euler(0f, treeNode2.angle, 0f);
					}
					else
					{
						treeNode2.parent.GetPropertiesAtTime(treeNode2.offset, out pos, out q, out num);
						num2 = treeNode2.parent.GetSurfaceAngleAtTime(treeNode2.offset);
					}
					Quaternion q2 = Quaternion.Euler(90f, treeNode2.angle, 0f);
					Matrix4x4 rhs = Matrix4x4.TRS(Vector3.zero, q2, Vector3.one);
					Matrix4x4 rhs2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(treeNode2.pitch + num2, 0f, 0f), Vector3.one);
					treeNode2.matrix = treeNode2.parent.matrix * Matrix4x4.TRS(pos, q, Vector3.one) * rhs * rhs2;
					treeNode2.matrix *= Matrix4x4.TRS(new Vector3(0f, 0f, 0f), treeNode2.rotation, new Vector3(1f, 1f, 1f));
					if (this.horizontalAlign > 0f)
					{
						Vector4 column = treeNode2.matrix.GetColumn(3);
						Quaternion to = Quaternion.Euler(90f, treeNode2.angle, 0f);
						Quaternion q3 = Quaternion.Slerp(MathUtils.QuaternionFromMatrix(treeNode2.matrix), to, this.horizontalAlign);
						treeNode2.matrix = Matrix4x4.TRS(Vector3.zero, q3, Vector3.one);
						treeNode2.matrix.SetColumn(3, column);
					}
					Vector3 vector;
					if (this.geometryMode == 4)
					{
						vector = treeNode2.matrix.MultiplyPoint(new Vector3(0f, num, 0f));
						treeNode2.matrix *= Matrix4x4.Scale(new Vector3(treeNode2.scale, treeNode2.scale, treeNode2.scale));
					}
					else
					{
						vector = treeNode2.matrix.MultiplyPoint(new Vector3(0f, num + treeNode2.scale, 0f));
					}
					treeNode2.matrix.m03 = vector.x;
					treeNode2.matrix.m13 = vector.y;
					treeNode2.matrix.m23 = vector.z;
				}
			}
			base.UpdateMatrix();
		}
		public override void BuildAOSpheres(List<TreeAOSphere> aoSpheres)
		{
			if (this.visible)
			{
				float num = 0.75f;
				for (int i = 0; i < this.nodes.Count; i++)
				{
					TreeNode treeNode = this.nodes[i];
					if (treeNode.visible)
					{
						Vector3 pos = treeNode.matrix.MultiplyPoint(new Vector3(0f, 0f, 0f));
						float radius = treeNode.scale * num;
						aoSpheres.Add(new TreeAOSphere(pos, radius, 0.5f));
					}
				}
			}
			base.BuildAOSpheres(aoSpheres);
		}
		public override void UpdateMesh(List<TreeMaterial> materials, List<TreeVertex> verts, List<TreeTriangle> tris, List<TreeAOSphere> aoSpheres, int buildFlags, float adaptiveQuality, float aoDensity)
		{
			if (this.geometryMode == 4)
			{
				if (this.instanceMesh != null)
				{
					TreeGroupLeaf.cloneMeshFilter = this.instanceMesh.GetComponent<MeshFilter>();
					if (TreeGroupLeaf.cloneMeshFilter != null)
					{
						TreeGroupLeaf.cloneMesh = TreeGroupLeaf.cloneMeshFilter.sharedMesh;
						if (TreeGroupLeaf.cloneMesh != null)
						{
							Vector3 extents = TreeGroupLeaf.cloneMesh.bounds.extents;
							float num = Mathf.Max(extents.x, extents.z) * 0.5f;
							TreeGroupLeaf.cloneVerts = TreeGroupLeaf.cloneMesh.vertices;
							TreeGroupLeaf.cloneNormals = TreeGroupLeaf.cloneMesh.normals;
							TreeGroupLeaf.cloneUVs = TreeGroupLeaf.cloneMesh.uv;
							TreeGroupLeaf.cloneTangents = TreeGroupLeaf.cloneMesh.tangents;
							for (int i = 0; i < TreeGroupLeaf.cloneVerts.Length; i++)
							{
								Vector3[] expr_D8_cp_0 = TreeGroupLeaf.cloneVerts;
								int expr_D8_cp_1 = i;
								expr_D8_cp_0[expr_D8_cp_1].x = expr_D8_cp_0[expr_D8_cp_1].x / num;
								Vector3[] expr_F0_cp_0 = TreeGroupLeaf.cloneVerts;
								int expr_F0_cp_1 = i;
								expr_F0_cp_0[expr_F0_cp_1].y = expr_F0_cp_0[expr_F0_cp_1].y / num;
								Vector3[] expr_108_cp_0 = TreeGroupLeaf.cloneVerts;
								int expr_108_cp_1 = i;
								expr_108_cp_0[expr_108_cp_1].z = expr_108_cp_0[expr_108_cp_1].z / num;
							}
						}
					}
					if (this.instanceMesh.GetComponent<Renderer>() != null)
					{
						Material[] sharedMaterials = this.instanceMesh.GetComponent<Renderer>().sharedMaterials;
						for (int j = 0; j < sharedMaterials.Length; j++)
						{
							TreeGroup.GetMaterialIndex(sharedMaterials[j], materials, false);
						}
					}
				}
			}
			else
			{
				TreeGroup.GetMaterialIndex(this.materialLeaf, materials, false);
			}
			for (int k = 0; k < this.nodes.Count; k++)
			{
				this.UpdateNodeMesh(this.nodes[k], materials, verts, tris, aoSpheres, buildFlags, adaptiveQuality, aoDensity);
			}
			TreeGroupLeaf.cloneMesh = null;
			TreeGroupLeaf.cloneMeshFilter = null;
			TreeGroupLeaf.cloneVerts = null;
			TreeGroupLeaf.cloneNormals = null;
			TreeGroupLeaf.cloneUVs = null;
			TreeGroupLeaf.cloneTangents = null;
			base.UpdateMesh(materials, verts, tris, aoSpheres, buildFlags, adaptiveQuality, aoDensity);
		}
		private static TreeVertex CreateBillboardVertex(TreeNode node, Quaternion billboardRotation, Vector3 normalBase, float normalFix, Vector3 tangentBase, Vector2 uv)
		{
			TreeVertex treeVertex = new TreeVertex();
			treeVertex.pos = node.matrix.MultiplyPoint(Vector3.zero);
			treeVertex.uv0 = uv;
			uv = 2f * uv - Vector2.one;
			treeVertex.nor = billboardRotation * new Vector3(uv.x * node.scale, uv.y * node.scale, 0f);
			treeVertex.nor.z = normalFix;
			Vector3 normalized = (billboardRotation * new Vector3(uv.x * normalBase.x, uv.y * normalBase.y, normalBase.z)).normalized;
			treeVertex.tangent = (tangentBase - normalized * Vector3.Dot(tangentBase, normalized)).normalized;
			treeVertex.tangent.w = 0f;
			return treeVertex;
		}
		private Vector2[] GetPlaneHullVertices(Material mat)
		{
			if (mat == null)
			{
				return null;
			}
			if (!mat.HasProperty("_MainTex"))
			{
				return null;
			}
			Texture mainTexture = mat.mainTexture;
			if (!mainTexture)
			{
				return null;
			}
			if (TreeGroupLeaf.s_TextureHulls == null || TreeGroupLeaf.s_TextureHullsDirty)
			{
				TreeGroupLeaf.s_TextureHulls = new Dictionary<Texture, Vector2[]>();
				TreeGroupLeaf.s_TextureHullsDirty = false;
			}
			if (TreeGroupLeaf.s_TextureHulls.ContainsKey(mainTexture))
			{
				return TreeGroupLeaf.s_TextureHulls[mainTexture];
			}
			Vector2[] array = MeshUtility.ComputeTextureBoundingHull(mainTexture, 4);
			Vector2 vector = array[1];
			array[1] = array[3];
			array[3] = vector;
			TreeGroupLeaf.s_TextureHulls.Add(mainTexture, array);
			return array;
		}
		private void UpdateNodeMesh(TreeNode node, List<TreeMaterial> materials, List<TreeVertex> verts, List<TreeTriangle> tris, List<TreeAOSphere> aoSpheres, int buildFlags, float adaptiveQuality, float aoDensity)
		{
			node.triStart = tris.Count;
			node.triEnd = tris.Count;
			node.vertStart = verts.Count;
			node.vertEnd = verts.Count;
			if (!node.visible || !this.visible)
			{
				return;
			}
			Profiler.BeginSample("TreeGroupLeaf.UpdateNodeMesh");
			Vector2 vector = base.ComputeWindFactor(node, node.offset);
			if (this.geometryMode == 4)
			{
				if (TreeGroupLeaf.cloneMesh == null)
				{
					return;
				}
				if (TreeGroupLeaf.cloneVerts == null)
				{
					return;
				}
				if (TreeGroupLeaf.cloneNormals == null)
				{
					return;
				}
				if (TreeGroupLeaf.cloneTangents == null)
				{
					return;
				}
				if (TreeGroupLeaf.cloneUVs == null)
				{
					return;
				}
				Matrix4x4 localToWorldMatrix = this.instanceMesh.transform.localToWorldMatrix;
				Matrix4x4 matrix4x = node.matrix * localToWorldMatrix;
				int count = verts.Count;
				float num = 5f;
				for (int i = 0; i < TreeGroupLeaf.cloneVerts.Length; i++)
				{
					TreeVertex treeVertex = new TreeVertex();
					treeVertex.pos = matrix4x.MultiplyPoint(TreeGroupLeaf.cloneVerts[i]);
					treeVertex.nor = matrix4x.MultiplyVector(TreeGroupLeaf.cloneNormals[i]).normalized;
					treeVertex.uv0 = new Vector2(TreeGroupLeaf.cloneUVs[i].x, TreeGroupLeaf.cloneUVs[i].y);
					Vector3 normalized = matrix4x.MultiplyVector(new Vector3(TreeGroupLeaf.cloneTangents[i].x, TreeGroupLeaf.cloneTangents[i].y, TreeGroupLeaf.cloneTangents[i].z)).normalized;
					treeVertex.tangent = new Vector4(normalized.x, normalized.y, normalized.z, TreeGroupLeaf.cloneTangents[i].w);
					float edgeFactor = TreeGroupLeaf.cloneVerts[i].magnitude / num * this.animationEdge;
					treeVertex.SetAnimationProperties(vector.x, vector.y, edgeFactor, node.animSeed);
					if ((buildFlags & 1) != 0)
					{
						treeVertex.SetAmbientOcclusion(TreeGroup.ComputeAmbientOcclusion(treeVertex.pos, treeVertex.nor, aoSpheres, aoDensity));
					}
					verts.Add(treeVertex);
				}
				for (int j = 0; j < TreeGroupLeaf.cloneMesh.subMeshCount; j++)
				{
					int[] triangles = TreeGroupLeaf.cloneMesh.GetTriangles(j);
					int materialIndex;
					if (this.instanceMesh.GetComponent<Renderer>() != null && j < this.instanceMesh.GetComponent<Renderer>().sharedMaterials.Length)
					{
						materialIndex = TreeGroup.GetMaterialIndex(this.instanceMesh.GetComponent<Renderer>().sharedMaterials[j], materials, false);
					}
					else
					{
						materialIndex = TreeGroup.GetMaterialIndex(null, materials, false);
					}
					for (int k = 0; k < triangles.Length; k += 3)
					{
						TreeTriangle item = new TreeTriangle(materialIndex, triangles[k] + count, triangles[k + 1] + count, triangles[k + 2] + count);
						tris.Add(item);
					}
				}
			}
			else
			{
				if (this.geometryMode == 3)
				{
					Vector3 eulerAngles = node.rotation.eulerAngles;
					eulerAngles.z = eulerAngles.x * 2f;
					eulerAngles.x = 0f;
					eulerAngles.y = 0f;
					Quaternion quaternion = Quaternion.Euler(eulerAngles);
					Vector3 normalBase = new Vector3(TreeGroup.GenerateBendBillboardNormalFactor, TreeGroup.GenerateBendBillboardNormalFactor, 1f);
					Vector3 tangentBase = quaternion * new Vector3(1f, 0f, 0f);
					float normalFix = node.scale / (TreeGroup.GenerateBendBillboardNormalFactor * TreeGroup.GenerateBendBillboardNormalFactor);
					TreeVertex treeVertex2 = TreeGroupLeaf.CreateBillboardVertex(node, quaternion, normalBase, normalFix, tangentBase, new Vector2(0f, 1f));
					TreeVertex treeVertex3 = TreeGroupLeaf.CreateBillboardVertex(node, quaternion, normalBase, normalFix, tangentBase, new Vector2(0f, 0f));
					TreeVertex treeVertex4 = TreeGroupLeaf.CreateBillboardVertex(node, quaternion, normalBase, normalFix, tangentBase, new Vector2(1f, 0f));
					TreeVertex treeVertex5 = TreeGroupLeaf.CreateBillboardVertex(node, quaternion, normalBase, normalFix, tangentBase, new Vector2(1f, 1f));
					treeVertex2.SetAnimationProperties(vector.x, vector.y, this.animationEdge, node.animSeed);
					treeVertex3.SetAnimationProperties(vector.x, vector.y, this.animationEdge, node.animSeed);
					treeVertex4.SetAnimationProperties(vector.x, vector.y, this.animationEdge, node.animSeed);
					treeVertex5.SetAnimationProperties(vector.x, vector.y, this.animationEdge, node.animSeed);
					if ((buildFlags & 1) != 0)
					{
						Vector3 b = Vector3.right * node.scale;
						Vector3 b2 = Vector3.forward * node.scale;
						float num2 = TreeGroup.ComputeAmbientOcclusion(treeVertex2.pos + b, Vector3.right, aoSpheres, aoDensity);
						num2 += TreeGroup.ComputeAmbientOcclusion(treeVertex2.pos - b, -Vector3.right, aoSpheres, aoDensity);
						num2 += TreeGroup.ComputeAmbientOcclusion(treeVertex2.pos + b2, Vector3.forward, aoSpheres, aoDensity);
						num2 += TreeGroup.ComputeAmbientOcclusion(treeVertex2.pos - b2, -Vector3.forward, aoSpheres, aoDensity);
						num2 /= 4f;
						treeVertex2.SetAmbientOcclusion(num2);
						treeVertex3.SetAmbientOcclusion(num2);
						treeVertex4.SetAmbientOcclusion(num2);
						treeVertex5.SetAmbientOcclusion(num2);
					}
					int count2 = verts.Count;
					verts.Add(treeVertex2);
					verts.Add(treeVertex3);
					verts.Add(treeVertex4);
					verts.Add(treeVertex5);
					int materialIndex2 = TreeGroup.GetMaterialIndex(this.materialLeaf, materials, false);
					tris.Add(new TreeTriangle(materialIndex2, count2, count2 + 2, count2 + 1, true));
					tris.Add(new TreeTriangle(materialIndex2, count2, count2 + 3, count2 + 2, true));
				}
				else
				{
					int num3 = 0;
					switch (this.geometryMode)
					{
					case 0:
						num3 = 1;
						break;
					case 1:
						num3 = 2;
						break;
					case 2:
						num3 = 3;
						break;
					}
					int materialIndex3 = TreeGroup.GetMaterialIndex(this.materialLeaf, materials, false);
					Vector2[] array = new Vector2[]
					{
						new Vector2(0f, 1f),
						new Vector2(0f, 0f),
						new Vector2(1f, 0f),
						new Vector2(1f, 1f)
					};
					Vector2[] array2 = this.GetPlaneHullVertices(this.materialLeaf);
					if (array2 == null)
					{
						array2 = array;
					}
					float scale = node.scale;
					Vector3[] array3 = new Vector3[]
					{
						new Vector3(-scale, 0f, -scale),
						new Vector3(-scale, 0f, scale),
						new Vector3(scale, 0f, scale),
						new Vector3(scale, 0f, -scale)
					};
					Vector3 vector2 = new Vector3(TreeGroup.GenerateBendNormalFactor, 1f - TreeGroup.GenerateBendNormalFactor, TreeGroup.GenerateBendNormalFactor);
					Vector3[] expr_788 = new Vector3[4];
					int arg_7B4_0_cp_1 = 0;
					Vector3 vector3 = new Vector3(-vector2.x, vector2.y, -vector2.z);
					expr_788[arg_7B4_0_cp_1] = vector3.normalized;
					int arg_7E2_0_cp_1 = 1;
					Vector3 vector4 = new Vector3(-vector2.x, vector2.y, 0f);
					expr_788[arg_7E2_0_cp_1] = vector4.normalized;
					int arg_80F_0_cp_1 = 2;
					Vector3 vector5 = new Vector3(vector2.x, vector2.y, 0f);
					expr_788[arg_80F_0_cp_1] = vector5.normalized;
					int arg_83F_0_cp_1 = 3;
					Vector3 vector6 = new Vector3(vector2.x, vector2.y, -vector2.z);
					expr_788[arg_83F_0_cp_1] = vector6.normalized;
					Vector3[] array4 = expr_788;
					for (int l = 0; l < num3; l++)
					{
						Quaternion quaternion2 = Quaternion.Euler(new Vector3(90f, 0f, 0f));
						int num4 = l;
						if (num4 != 1)
						{
							if (num4 == 2)
							{
								quaternion2 = Quaternion.Euler(new Vector3(0f, 90f, 0f));
							}
						}
						else
						{
							quaternion2 = Quaternion.Euler(new Vector3(90f, 90f, 0f));
						}
						TreeVertex[] array5 = new TreeVertex[]
						{
							new TreeVertex(),
							new TreeVertex(),
							new TreeVertex(),
							new TreeVertex(),
							new TreeVertex(),
							new TreeVertex(),
							new TreeVertex(),
							new TreeVertex()
						};
						for (int m = 0; m < 4; m++)
						{
							array5[m].pos = node.matrix.MultiplyPoint(quaternion2 * array3[m]);
							array5[m].nor = node.matrix.MultiplyVector(quaternion2 * array4[m]);
							array5[m].tangent = TreeGroup.CreateTangent(node, quaternion2, array5[m].nor);
							array5[m].uv0 = array2[m];
							array5[m].SetAnimationProperties(vector.x, vector.y, this.animationEdge, node.animSeed);
							if ((buildFlags & 1) != 0)
							{
								array5[m].SetAmbientOcclusion(TreeGroup.ComputeAmbientOcclusion(array5[m].pos, array5[m].nor, aoSpheres, aoDensity));
							}
						}
						for (int n = 0; n < 4; n++)
						{
							array5[n + 4].Lerp4(array5, array2[n]);
							array5[n + 4].uv0 = array5[n].uv0;
							array5[n + 4].uv1 = array5[n].uv1;
							array5[n + 4].flag = array5[n].flag;
						}
						int count3 = verts.Count;
						for (int num5 = 0; num5 < 4; num5++)
						{
							verts.Add(array5[num5 + 4]);
						}
						tris.Add(new TreeTriangle(materialIndex3, count3, count3 + 1, count3 + 2));
						tris.Add(new TreeTriangle(materialIndex3, count3, count3 + 2, count3 + 3));
						Vector3 inNormal = node.matrix.MultiplyVector(quaternion2 * new Vector3(0f, 1f, 0f));
						if (TreeGroup.GenerateDoubleSidedGeometry)
						{
							TreeVertex[] array6 = new TreeVertex[]
							{
								new TreeVertex(),
								new TreeVertex(),
								new TreeVertex(),
								new TreeVertex(),
								new TreeVertex(),
								new TreeVertex(),
								new TreeVertex(),
								new TreeVertex()
							};
							for (int num6 = 0; num6 < 4; num6++)
							{
								array6[num6].pos = array5[num6].pos;
								array6[num6].nor = Vector3.Reflect(array5[num6].nor, inNormal);
								array6[num6].tangent = Vector3.Reflect(array5[num6].tangent, inNormal);
								array6[num6].tangent.w = -1f;
								array6[num6].uv0 = array5[num6].uv0;
								array6[num6].SetAnimationProperties(vector.x, vector.y, this.animationEdge, node.animSeed);
								if ((buildFlags & 1) != 0)
								{
									array6[num6].SetAmbientOcclusion(TreeGroup.ComputeAmbientOcclusion(array6[num6].pos, array6[num6].nor, aoSpheres, aoDensity));
								}
							}
							for (int num7 = 0; num7 < 4; num7++)
							{
								array6[num7 + 4].Lerp4(array6, array2[num7]);
								array6[num7 + 4].uv0 = array6[num7].uv0;
								array6[num7 + 4].uv1 = array6[num7].uv1;
								array6[num7 + 4].flag = array6[num7].flag;
							}
							int count4 = verts.Count;
							for (int num8 = 0; num8 < 4; num8++)
							{
								verts.Add(array6[num8 + 4]);
							}
							tris.Add(new TreeTriangle(materialIndex3, count4, count4 + 2, count4 + 1));
							tris.Add(new TreeTriangle(materialIndex3, count4, count4 + 3, count4 + 2));
						}
					}
				}
			}
			node.triEnd = tris.Count;
			node.vertEnd = verts.Count;
			Profiler.EndSample();
		}
	}
}
