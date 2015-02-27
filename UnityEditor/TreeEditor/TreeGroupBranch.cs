using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class TreeGroupBranch : TreeGroup
	{
		public enum GeometryMode
		{
			Branch,
			BranchFrond,
			Frond
		}
		private static float spreadMul = 5f;
		public float lodQualityMultiplier = 1f;
		public TreeGroupBranch.GeometryMode geometryMode;
		public Material materialBranch;
		public Material materialBreak;
		public Material materialFrond;
		public Vector2 height = new Vector2(10f, 15f);
		public float radius = 0.5f;
		public AnimationCurve radiusCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f, -1f, -1f),
			new Keyframe(1f, 0f, -1f, -1f)
		});
		public bool radiusMode = true;
		public float capSmoothing;
		public float crinklyness = 0.1f;
		public AnimationCurve crinkCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 1f)
		});
		public float seekBlend;
		public AnimationCurve seekCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 1f)
		});
		public float noise = 0.1f;
		public AnimationCurve noiseCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 1f)
		});
		public float noiseScaleU = 0.2f;
		public float noiseScaleV = 0.1f;
		public float flareSize;
		public float flareHeight = 0.1f;
		public float flareNoise = 0.3f;
		public float weldHeight = 0.1f;
		public float weldSpreadTop;
		public float weldSpreadBottom;
		public float breakingChance;
		public Vector2 breakingSpot = new Vector2(0.4f, 0.6f);
		public int frondCount = 1;
		public float frondWidth = 1f;
		public AnimationCurve frondCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 1f)
		});
		public Vector2 frondRange = new Vector2(0.1f, 1f);
		public float frondRotation;
		public float frondCrease;
		internal override bool HasExternalChanges()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			if (this.geometryMode == TreeGroupBranch.GeometryMode.Branch)
			{
				list.Add(this.materialBranch);
				if (this.breakingChance > 0f)
				{
					list.Add(this.materialBreak);
				}
			}
			else
			{
				if (this.geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
				{
					list.Add(this.materialBranch);
					list.Add(this.materialFrond);
					if (this.breakingChance > 0f)
					{
						list.Add(this.materialBreak);
					}
				}
				else
				{
					if (this.geometryMode == TreeGroupBranch.GeometryMode.Frond)
					{
						list.Add(this.materialFrond);
					}
				}
			}
			string text = InternalEditorUtility.CalculateHashForObjectsAndDependencies(list.ToArray());
			if (text != this.m_Hash)
			{
				this.m_Hash = text;
				return true;
			}
			return false;
		}
		private Vector3 GetFlareWeldAtTime(TreeNode node, float time)
		{
			float scale = node.GetScale();
			float num = 0f;
			float num2 = 0f;
			if (this.flareHeight > 0.001f)
			{
				float value = (1f - time - (1f - this.flareHeight)) / this.flareHeight;
				num = Mathf.Pow(Mathf.Clamp01(value), 1.5f) * scale;
			}
			if (this.weldHeight > 0.001f)
			{
				float value2 = (1f - time - (1f - this.weldHeight)) / this.weldHeight;
				num2 = Mathf.Pow(Mathf.Clamp01(value2), 1.5f) * scale;
			}
			return new Vector3(num * this.flareSize, num2 * this.weldSpreadTop * TreeGroupBranch.spreadMul, num2 * this.weldSpreadBottom * TreeGroupBranch.spreadMul);
		}
		public override float GetRadiusAtTime(TreeNode node, float time, bool includeModifications)
		{
			if (this.geometryMode == TreeGroupBranch.GeometryMode.Frond)
			{
				return 0f;
			}
			float num = Mathf.Clamp01(this.radiusCurve.Evaluate(time)) * node.size;
			float num2 = 1f - node.capRange;
			if (time > num2)
			{
				float f = Mathf.Acos(Mathf.Clamp01((time - num2) / node.capRange));
				float num3 = Mathf.Sin(f);
				num *= num3;
			}
			if (includeModifications)
			{
				Vector3 flareWeldAtTime = this.GetFlareWeldAtTime(node, time);
				num += Mathf.Max(flareWeldAtTime.x, Mathf.Max(flareWeldAtTime.y, flareWeldAtTime.z) * 0.25f) * 0.1f;
			}
			return num;
		}
		public override void UpdateParameters()
		{
			if (this.parentGroup == null || this.parentGroup.GetType() == typeof(TreeGroupRoot))
			{
				this.weldSpreadTop = 0f;
				this.weldSpreadBottom = 0f;
				this.animationSecondary = 0f;
			}
			else
			{
				this.flareSize = 0f;
			}
			float num = (this.height.y - this.height.x) / this.height.y;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				TreeNode treeNode = this.nodes[i];
				UnityEngine.Random.seed = treeNode.seed;
				float num2 = 1f;
				for (int j = 0; j < 5; j++)
				{
					num2 = 1f - UnityEngine.Random.value * num;
				}
				if (this.lockFlags == 0)
				{
					treeNode.scale *= num2;
				}
				float num3 = 0f;
				for (int k = 0; k < 5; k++)
				{
					num3 = UnityEngine.Random.value;
				}
				for (int l = 0; l < 5; l++)
				{
					treeNode.breakOffset = 1f;
					if (UnityEngine.Random.value <= this.breakingChance && this.breakingChance > 0f)
					{
						treeNode.breakOffset = this.breakingSpot.x + (this.breakingSpot.y - this.breakingSpot.x) * num3;
						if (treeNode.breakOffset < 0.01f)
						{
							treeNode.breakOffset = 0.01f;
						}
					}
				}
				if (!(this.parentGroup is TreeGroupRoot))
				{
					treeNode.size = this.radius;
					if (this.radiusMode)
					{
						treeNode.size *= treeNode.scale;
					}
					if (treeNode.parent != null && treeNode.parent.spline != null)
					{
						treeNode.size = Mathf.Min(treeNode.parent.group.GetRadiusAtTime(treeNode.parent, treeNode.offset, false) * 0.75f, treeNode.size);
					}
				}
				else
				{
					if (this.lockFlags == 0)
					{
						treeNode.size = this.radius;
						if (this.radiusMode)
						{
							treeNode.size *= treeNode.scale;
						}
					}
				}
			}
			this.UpdateMatrix();
			this.UpdateSplines();
			base.UpdateParameters();
		}
		public void UpdateSplines()
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				TreeNode treeNode = this.nodes[i];
				this.UpdateSpline(treeNode);
				if (this.capSmoothing < 0.01f)
				{
					treeNode.capRange = 0f;
				}
				else
				{
					float num = Mathf.Clamp(this.radiusCurve.Evaluate(1f), 0f, 1f) * treeNode.size;
					float approximateLength = treeNode.spline.GetApproximateLength();
					treeNode.capRange = num / approximateLength * this.capSmoothing * 2f;
				}
			}
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
				if (this.parentGroup is TreeGroupRoot)
				{
					TreeGroupRoot treeGroupRoot = this.parentGroup as TreeGroupRoot;
					for (int j = 0; j < this.nodes.Count; j++)
					{
						TreeNode treeNode2 = this.nodes[j];
						float num = treeNode2.offset * base.GetRootSpread();
						float f = treeNode2.angle * 0.0174532924f;
						Vector3 pos = new Vector3(Mathf.Cos(f) * num, -treeGroupRoot.groundOffset, Mathf.Sin(f) * num);
						Quaternion q = Quaternion.Euler(treeNode2.pitch * -Mathf.Sin(f), 0f, treeNode2.pitch * Mathf.Cos(f)) * Quaternion.Euler(0f, treeNode2.angle, 0f);
						treeNode2.matrix = Matrix4x4.TRS(pos, q, Vector3.one);
					}
				}
				else
				{
					for (int k = 0; k < this.nodes.Count; k++)
					{
						TreeNode treeNode3 = this.nodes[k];
						Vector3 pos2 = default(Vector3);
						Quaternion q2 = default(Quaternion);
						float y = 0f;
						treeNode3.parent.GetPropertiesAtTime(treeNode3.offset, out pos2, out q2, out y);
						Quaternion q3 = Quaternion.Euler(90f, treeNode3.angle, 0f);
						Matrix4x4 rhs = Matrix4x4.TRS(Vector3.zero, q3, Vector3.one);
						Matrix4x4 rhs2 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(treeNode3.pitch, 0f, 0f), Vector3.one);
						treeNode3.matrix = treeNode3.parent.matrix * Matrix4x4.TRS(pos2, q2, Vector3.one) * rhs * rhs2;
						Vector3 vector = treeNode3.matrix.MultiplyPoint(new Vector3(0f, y, 0f));
						treeNode3.matrix.m03 = vector.x;
						treeNode3.matrix.m13 = vector.y;
						treeNode3.matrix.m23 = vector.z;
					}
				}
			}
		}
		public override void BuildAOSpheres(List<TreeAOSphere> aoSpheres)
		{
			float num = 0.5f;
			if (this.visible)
			{
				bool flag = this.geometryMode != TreeGroupBranch.GeometryMode.Branch;
				bool flag2 = this.geometryMode != TreeGroupBranch.GeometryMode.Frond;
				for (int i = 0; i < this.nodes.Count; i++)
				{
					TreeNode treeNode = this.nodes[i];
					if (treeNode.visible)
					{
						float scale = treeNode.GetScale();
						float num2 = treeNode.spline.GetApproximateLength();
						if (num2 < num)
						{
							num2 = num;
						}
						float a = num / num2;
						float num4;
						for (float num3 = 0f; num3 < 1f; num3 += Mathf.Max(a, num4 / num2))
						{
							Vector3 pos = treeNode.matrix.MultiplyPoint(treeNode.spline.GetPositionAtTime(num3));
							num4 = 0f;
							if (flag2)
							{
								num4 = this.GetRadiusAtTime(treeNode, num3, false) * 0.95f;
							}
							if (flag)
							{
								num4 = Mathf.Max(num4, 0.95f * (Mathf.Clamp01(this.frondCurve.Evaluate(num3)) * this.frondWidth * scale));
								pos.y -= num4;
							}
							if (num4 > 0.01f)
							{
								aoSpheres.Add(new TreeAOSphere(pos, num4, 1f));
							}
						}
					}
				}
			}
			base.BuildAOSpheres(aoSpheres);
		}
		public override void UpdateMesh(List<TreeMaterial> materials, List<TreeVertex> verts, List<TreeTriangle> tris, List<TreeAOSphere> aoSpheres, int buildFlags, float adaptiveQuality, float aoDensity)
		{
			if (this.geometryMode == TreeGroupBranch.GeometryMode.Branch || this.geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
			{
				TreeGroup.GetMaterialIndex(this.materialBranch, materials, true);
				if (this.breakingChance > 0f)
				{
					TreeGroup.GetMaterialIndex(this.materialBreak, materials, false);
				}
			}
			if (this.geometryMode == TreeGroupBranch.GeometryMode.Frond || this.geometryMode == TreeGroupBranch.GeometryMode.BranchFrond)
			{
				TreeGroup.GetMaterialIndex(this.materialFrond, materials, false);
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				this.UpdateNodeMesh(this.nodes[i], materials, verts, tris, aoSpheres, buildFlags, adaptiveQuality, aoDensity);
			}
			base.UpdateMesh(materials, verts, tris, aoSpheres, buildFlags, adaptiveQuality, aoDensity);
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
			Profiler.BeginSample("TreeGroupBranch.UpdateNodeMesh");
			int count = verts.Count;
			float approximateLength = node.spline.GetApproximateLength();
			List<RingLoop> list = new List<RingLoop>();
			float adaptiveQuality2 = Mathf.Clamp01(adaptiveQuality * this.lodQualityMultiplier);
			List<float> adaptiveSamples = TreeData.GetAdaptiveSamples(this, node, adaptiveQuality2);
			int adaptiveRadialSegments = TreeData.GetAdaptiveRadialSegments(this.radius, adaptiveQuality2);
			TreeGroupBranch treeGroupBranch = null;
			if (this.parentGroup != null && this.parentGroup.GetType() == typeof(TreeGroupBranch))
			{
				treeGroupBranch = (TreeGroupBranch)this.parentGroup;
			}
			if (this.geometryMode == TreeGroupBranch.GeometryMode.BranchFrond || this.geometryMode == TreeGroupBranch.GeometryMode.Branch)
			{
				int materialIndex = TreeGroup.GetMaterialIndex(this.materialBranch, materials, true);
				float num = 0f;
				float num2 = 0f;
				float num3 = approximateLength / (this.GetRadiusAtTime(node, 0f, false) * 3.14159274f * 2f);
				bool flag = true;
				if (node.parent != null && treeGroupBranch != null)
				{
					num2 = node.offset * node.parent.spline.GetApproximateLength();
				}
				float num4 = 1f - node.capRange;
				for (int i = 0; i < adaptiveSamples.Count; i++)
				{
					float num5 = adaptiveSamples[i];
					Vector3 positionAtTime = node.spline.GetPositionAtTime(num5);
					Quaternion rotationAtTime = node.spline.GetRotationAtTime(num5);
					float radiusAtTime = this.GetRadiusAtTime(node, num5, false);
					Matrix4x4 m = node.matrix * Matrix4x4.TRS(positionAtTime, rotationAtTime, new Vector3(1f, 1f, 1f));
					Vector3 flareWeldAtTime = this.GetFlareWeldAtTime(node, num5);
					float num6 = Mathf.Max(flareWeldAtTime.x, Mathf.Max(flareWeldAtTime.y, flareWeldAtTime.z) * 0.25f);
					if (num5 <= num4)
					{
						adaptiveRadialSegments = TreeData.GetAdaptiveRadialSegments(radiusAtTime + num6, adaptiveQuality2);
					}
					if (flag)
					{
						if (i > 0)
						{
							float num7 = adaptiveSamples[i - 1];
							float num8 = num5 - num7;
							float num9 = (radiusAtTime + this.GetRadiusAtTime(node, num7, false)) * 0.5f;
							num += num8 * approximateLength / (num9 * 3.14159274f * 2f);
						}
					}
					else
					{
						num = num2 + num5 * num3;
					}
					Vector2 vector = base.ComputeWindFactor(node, num5);
					RingLoop ringLoop = new RingLoop();
					ringLoop.Reset(radiusAtTime, m, num, adaptiveRadialSegments);
					ringLoop.SetSurfaceAngle(node.GetSurfaceAngleAtTime(num5));
					ringLoop.SetAnimationProperties(vector.x, vector.y, 0f, node.animSeed);
					ringLoop.SetSpread(flareWeldAtTime.y, flareWeldAtTime.z);
					ringLoop.SetNoise(this.noise * Mathf.Clamp01(this.noiseCurve.Evaluate(num5)), this.noiseScaleU * 10f, this.noiseScaleV * 10f);
					ringLoop.SetFlares(flareWeldAtTime.x, this.flareNoise * 10f);
					int count2 = verts.Count;
					ringLoop.BuildVertices(verts);
					int count3 = verts.Count;
					if ((buildFlags & 2) != 0)
					{
						float num10 = this.weldHeight;
						float num11 = Mathf.Pow(Mathf.Clamp01((1f - num5 - (1f - this.weldHeight)) / this.weldHeight), 1.5f);
						float d = 1f - num11;
						if (num5 < num10 && node.parent != null && node.parent.spline != null)
						{
							Ray ray = default(Ray);
							for (int j = count2; j < count3; j++)
							{
								ray.origin = verts[j].pos;
								ray.direction = m.MultiplyVector(-Vector3.up);
								Vector3 pos = verts[j].pos;
								Vector3 nor = verts[j].nor;
								float num12 = -10000f;
								float num13 = 100000f;
								for (int k = node.parent.triStart; k < node.parent.triEnd; k++)
								{
									object obj = MathUtils.IntersectRayTriangle(ray, verts[tris[k].v[0]].pos, verts[tris[k].v[1]].pos, verts[tris[k].v[2]].pos, true);
									if (obj != null)
									{
										RaycastHit raycastHit = (RaycastHit)obj;
										if (Mathf.Abs(raycastHit.distance) < num13 && raycastHit.distance > num12)
										{
											num13 = Mathf.Abs(raycastHit.distance);
											verts[j].nor = verts[tris[k].v[0]].nor * raycastHit.barycentricCoordinate.x + verts[tris[k].v[1]].nor * raycastHit.barycentricCoordinate.y + verts[tris[k].v[2]].nor * raycastHit.barycentricCoordinate.z;
											verts[j].nor = verts[j].nor * num11 + nor * d;
											verts[j].pos = raycastHit.point * num11 + pos * d;
										}
									}
								}
							}
						}
					}
					list.Add(ringLoop);
					if (num5 == 1f && ringLoop.radius > 0.005f)
					{
						RingLoop ringLoop2 = ringLoop.Clone();
						ringLoop2.radius = 0f;
						ringLoop2.baseOffset += radiusAtTime / 6.28318548f;
						ringLoop2.BuildVertices(verts);
						list.Add(ringLoop2);
					}
				}
				if (list.Count > 0 && list[list.Count - 1].radius > 0.025f && node.breakOffset < 1f)
				{
					float mappingScale = 1f / (this.radius * 3.14159274f * 2f);
					float sphereFactor = 0f;
					float num14 = 1f;
					int mappingMode = 0;
					Material m2 = this.materialBranch;
					if (this.materialBreak != null)
					{
						m2 = this.materialBreak;
					}
					int materialIndex2 = TreeGroup.GetMaterialIndex(m2, materials, false);
					list[list.Count - 1].Cap(sphereFactor, num14, mappingMode, mappingScale, verts, tris, materialIndex2);
				}
				node.triStart = tris.Count;
				for (int l = 0; l < list.Count - 1; l++)
				{
					list[l].Connect(list[l + 1], tris, materialIndex, false, false);
				}
				node.triEnd = tris.Count;
				list.Clear();
			}
			float num15 = Mathf.Min(this.frondRange.x, this.frondRange.y);
			float num16 = Mathf.Max(this.frondRange.x, this.frondRange.y);
			float num17 = num15;
			float num18 = num16;
			num15 = Mathf.Clamp(num15, 0f, node.breakOffset);
			num16 = Mathf.Clamp(num16, 0f, node.breakOffset);
			if ((this.geometryMode == TreeGroupBranch.GeometryMode.BranchFrond || this.geometryMode == TreeGroupBranch.GeometryMode.Frond) && this.frondCount > 0 && num15 != num16)
			{
				bool flag2 = true;
				bool flag3 = true;
				for (int n = 0; n < adaptiveSamples.Count; n++)
				{
					float num19 = adaptiveSamples[n];
					if (num19 < num15)
					{
						adaptiveSamples.RemoveAt(n);
						n--;
					}
					else
					{
						if (num19 == num15)
						{
							flag2 = false;
						}
						else
						{
							if (num19 == num16)
							{
								flag3 = false;
							}
							else
							{
								if (num19 > num16)
								{
									adaptiveSamples.RemoveAt(n);
									n--;
								}
							}
						}
					}
				}
				if (flag2)
				{
					adaptiveSamples.Insert(0, num15);
				}
				if (flag3)
				{
					adaptiveSamples.Add(num16);
				}
				int materialIndex3 = TreeGroup.GetMaterialIndex(this.materialFrond, materials, false);
				float num20 = 1f - node.capRange;
				for (int num21 = 0; num21 < this.frondCount; num21++)
				{
					float num22 = this.frondCrease * 90f * 0.0174532924f;
					float num23 = (this.frondRotation * 360f + (float)num21 * 180f / (float)this.frondCount - 90f) * 0.0174532924f;
					float f = -num23 - num22;
					float f2 = num23 - num22;
					Vector3 a = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
					Vector3 vector2 = new Vector3(a.z, 0f, -a.x);
					Vector3 a2 = new Vector3(Mathf.Sin(f2), 0f, -Mathf.Cos(f2));
					Vector3 vector3 = new Vector3(-a2.z, 0f, a2.x);
					for (int num24 = 0; num24 < adaptiveSamples.Count; num24++)
					{
						float num25 = adaptiveSamples[num24];
						float y = (num25 - num17) / (num18 - num17);
						float timeParam = num25;
						if (num25 > num20)
						{
							timeParam = num20;
							float f3 = Mathf.Acos(Mathf.Clamp01((num25 - num20) / node.capRange));
							float num26 = Mathf.Sin(f3);
							float y2 = Mathf.Cos(f3) * this.capSmoothing;
							a = new Vector3(Mathf.Sin(f) * num26, y2, Mathf.Cos(f) * num26);
							vector2 = new Vector3(a.z, a.y, -a.x);
							a2 = new Vector3(Mathf.Sin(f2) * num26, y2, -Mathf.Cos(f2) * num26);
							vector3 = new Vector3(-a2.z, a2.y, a2.x);
						}
						Vector3 a3 = new Vector3(0f, 0f, -1f);
						Vector3 positionAtTime2 = node.spline.GetPositionAtTime(timeParam);
						Quaternion rotationAtTime2 = node.spline.GetRotationAtTime(num25);
						float d2 = Mathf.Clamp01(this.frondCurve.Evaluate(num25)) * this.frondWidth * node.GetScale();
						Matrix4x4 matrix4x = node.matrix * Matrix4x4.TRS(positionAtTime2, rotationAtTime2, new Vector3(1f, 1f, 1f));
						if (TreeGroup.GenerateDoubleSidedGeometry)
						{
							for (float num27 = -1f; num27 < 2f; num27 += 2f)
							{
								TreeVertex treeVertex = new TreeVertex();
								treeVertex.pos = matrix4x.MultiplyPoint(a * d2);
								treeVertex.nor = matrix4x.MultiplyVector(vector2 * num27).normalized;
								treeVertex.tangent = TreeGroup.CreateTangent(node, rotationAtTime2, treeVertex.nor);
								treeVertex.tangent.w = -num27;
								treeVertex.uv0 = new Vector2(1f, y);
								TreeVertex treeVertex2 = new TreeVertex();
								treeVertex2.pos = matrix4x.MultiplyPoint(Vector3.zero);
								treeVertex2.nor = matrix4x.MultiplyVector(a3 * num27).normalized;
								treeVertex2.tangent = TreeGroup.CreateTangent(node, rotationAtTime2, treeVertex2.nor);
								treeVertex2.tangent.w = -num27;
								treeVertex2.uv0 = new Vector2(0.5f, y);
								TreeVertex treeVertex3 = new TreeVertex();
								treeVertex3.pos = matrix4x.MultiplyPoint(Vector3.zero);
								treeVertex3.nor = matrix4x.MultiplyVector(a3 * num27).normalized;
								treeVertex3.tangent = TreeGroup.CreateTangent(node, rotationAtTime2, treeVertex3.nor);
								treeVertex3.tangent.w = -num27;
								treeVertex3.uv0 = new Vector2(0.5f, y);
								TreeVertex treeVertex4 = new TreeVertex();
								treeVertex4.pos = matrix4x.MultiplyPoint(a2 * d2);
								treeVertex4.nor = matrix4x.MultiplyVector(vector3 * num27).normalized;
								treeVertex4.tangent = TreeGroup.CreateTangent(node, rotationAtTime2, treeVertex4.nor);
								treeVertex4.tangent.w = -num27;
								treeVertex4.uv0 = new Vector2(0f, y);
								Vector2 vector4 = base.ComputeWindFactor(node, num25);
								treeVertex.SetAnimationProperties(vector4.x, vector4.y, this.animationEdge, node.animSeed);
								treeVertex2.SetAnimationProperties(vector4.x, vector4.y, 0f, node.animSeed);
								treeVertex3.SetAnimationProperties(vector4.x, vector4.y, 0f, node.animSeed);
								treeVertex4.SetAnimationProperties(vector4.x, vector4.y, this.animationEdge, node.animSeed);
								verts.Add(treeVertex);
								verts.Add(treeVertex2);
								verts.Add(treeVertex3);
								verts.Add(treeVertex4);
							}
							if (num24 > 0)
							{
								int count4 = verts.Count;
								TreeTriangle treeTriangle = new TreeTriangle(materialIndex3, count4 - 4, count4 - 3, count4 - 11);
								TreeTriangle treeTriangle2 = new TreeTriangle(materialIndex3, count4 - 4, count4 - 11, count4 - 12);
								treeTriangle.flip();
								treeTriangle2.flip();
								TreeTriangle item = new TreeTriangle(materialIndex3, count4 - 8, count4 - 7, count4 - 15);
								TreeTriangle item2 = new TreeTriangle(materialIndex3, count4 - 8, count4 - 15, count4 - 16);
								tris.Add(treeTriangle);
								tris.Add(treeTriangle2);
								tris.Add(item);
								tris.Add(item2);
								TreeTriangle item3 = new TreeTriangle(materialIndex3, count4 - 2, count4 - 9, count4 - 1);
								TreeTriangle item4 = new TreeTriangle(materialIndex3, count4 - 2, count4 - 10, count4 - 9);
								TreeTriangle treeTriangle3 = new TreeTriangle(materialIndex3, count4 - 6, count4 - 13, count4 - 5);
								TreeTriangle treeTriangle4 = new TreeTriangle(materialIndex3, count4 - 6, count4 - 14, count4 - 13);
								treeTriangle3.flip();
								treeTriangle4.flip();
								tris.Add(item3);
								tris.Add(item4);
								tris.Add(treeTriangle3);
								tris.Add(treeTriangle4);
							}
						}
						else
						{
							TreeVertex treeVertex5 = new TreeVertex();
							treeVertex5.pos = matrix4x.MultiplyPoint(a * d2);
							treeVertex5.nor = matrix4x.MultiplyVector(vector2).normalized;
							treeVertex5.uv0 = new Vector2(0f, y);
							TreeVertex treeVertex6 = new TreeVertex();
							treeVertex6.pos = matrix4x.MultiplyPoint(Vector3.zero);
							treeVertex6.nor = matrix4x.MultiplyVector(Vector3.back).normalized;
							treeVertex6.uv0 = new Vector2(0.5f, y);
							TreeVertex treeVertex7 = new TreeVertex();
							treeVertex7.pos = matrix4x.MultiplyPoint(a2 * d2);
							treeVertex7.nor = matrix4x.MultiplyVector(vector3).normalized;
							treeVertex7.uv0 = new Vector2(1f, y);
							Vector2 vector5 = base.ComputeWindFactor(node, num25);
							treeVertex5.SetAnimationProperties(vector5.x, vector5.y, this.animationEdge, node.animSeed);
							treeVertex6.SetAnimationProperties(vector5.x, vector5.y, 0f, node.animSeed);
							treeVertex7.SetAnimationProperties(vector5.x, vector5.y, this.animationEdge, node.animSeed);
							verts.Add(treeVertex5);
							verts.Add(treeVertex6);
							verts.Add(treeVertex7);
							if (num24 > 0)
							{
								int count5 = verts.Count;
								TreeTriangle item5 = new TreeTriangle(materialIndex3, count5 - 2, count5 - 3, count5 - 6);
								TreeTriangle item6 = new TreeTriangle(materialIndex3, count5 - 2, count5 - 6, count5 - 5);
								tris.Add(item5);
								tris.Add(item6);
								TreeTriangle item7 = new TreeTriangle(materialIndex3, count5 - 2, count5 - 4, count5 - 1);
								TreeTriangle item8 = new TreeTriangle(materialIndex3, count5 - 2, count5 - 5, count5 - 4);
								tris.Add(item7);
								tris.Add(item8);
							}
						}
					}
				}
			}
			if ((buildFlags & 1) != 0)
			{
				for (int num28 = count; num28 < verts.Count; num28++)
				{
					verts[num28].SetAmbientOcclusion(TreeGroup.ComputeAmbientOcclusion(verts[num28].pos, verts[num28].nor, aoSpheres, aoDensity));
				}
			}
			node.vertEnd = verts.Count;
			Profiler.EndSample();
		}
		public void UpdateSpline(TreeNode node)
		{
			if (this.lockFlags != 0)
			{
				return;
			}
			UnityEngine.Random.seed = node.seed;
			if (node.spline == null)
			{
				TreeSpline spline = new TreeSpline();
				node.spline = spline;
			}
			float num = this.height.y * node.GetScale();
			float num2 = 1f;
			int num3 = (int)Mathf.Round(num / num2);
			float num4 = 0f;
			Quaternion quaternion = Quaternion.identity;
			Vector3 vector = new Vector3(0f, 0f, 0f);
			Matrix4x4 inverse = (node.matrix * base.GetRootMatrix()).inverse;
			Quaternion to = MathUtils.QuaternionFromMatrix(inverse) * Quaternion.Euler(0f, node.angle, 0f);
			Quaternion to2 = MathUtils.QuaternionFromMatrix(inverse) * Quaternion.Euler(-180f, node.angle, 0f);
			node.spline.Reset();
			node.spline.AddPoint(vector, 0f);
			for (int i = 0; i < num3; i++)
			{
				float num5 = num2;
				if (i == num3 - 1)
				{
					num5 = num - num4;
				}
				num4 += num5;
				float num6 = num4 / num;
				float num7 = Mathf.Clamp(this.seekCurve.Evaluate(num6), -1f, 1f);
				float t = Mathf.Clamp01(num7) * this.seekBlend;
				float t2 = Mathf.Clamp01(-num7) * this.seekBlend;
				quaternion = Quaternion.Slerp(quaternion, to, t);
				quaternion = Quaternion.Slerp(quaternion, to2, t2);
				float t3 = this.crinklyness * Mathf.Clamp01(this.crinkCurve.Evaluate(num6));
				Quaternion to3 = Quaternion.Euler(new Vector3(180f * (UnityEngine.Random.value - 0.5f), node.angle, 180f * (UnityEngine.Random.value - 0.5f)));
				quaternion = Quaternion.Slerp(quaternion, to3, t3);
				vector += quaternion * new Vector3(0f, num5, 0f);
				node.spline.AddPoint(vector, num6);
			}
			node.spline.UpdateTime();
			node.spline.UpdateRotations();
		}
	}
}
