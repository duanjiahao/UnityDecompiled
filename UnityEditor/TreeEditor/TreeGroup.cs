using System;
using System.Collections.Generic;
using UnityEngine;
namespace TreeEditor
{
	[Serializable]
	public class TreeGroup
	{
		public enum LockFlag
		{
			LockPosition = 1,
			LockAlignment,
			LockShape = 4
		}
		public enum BuildFlag
		{
			BuildAmbientOcclusion = 1,
			BuildWeldParts
		}
		public enum DistributionMode
		{
			Random,
			Alternate,
			Opposite,
			Whorled
		}
		protected static readonly bool GenerateDoubleSidedGeometry = true;
		protected static readonly float GenerateBendNormalFactor = 0.4f;
		protected static readonly float GenerateBendBillboardNormalFactor = 0.8f;
		[SerializeField]
		private int _uniqueID = -1;
		public int seed = 1234;
		[SerializeField]
		private int _internalSeed = 1234;
		[SerializeField]
		internal string m_Hash;
		public int distributionFrequency = 1;
		public TreeGroup.DistributionMode distributionMode;
		public AnimationCurve distributionCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 1f)
		});
		public int distributionNodes = 5;
		public float distributionTwirl;
		public float distributionPitch;
		public AnimationCurve distributionPitchCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 1f)
		});
		public float distributionScale = 1f;
		public AnimationCurve distributionScaleCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 0.3f)
		});
		public bool showAnimationProps = true;
		public float animationPrimary = 0.5f;
		public float animationSecondary = 0.5f;
		public float animationEdge = 1f;
		public bool visible = true;
		public int lockFlags;
		public int[] nodeIDs = new int[0];
		public int parentGroupID = -1;
		public int[] childGroupIDs = new int[0];
		[NonSerialized]
		internal List<TreeNode> nodes = new List<TreeNode>();
		[NonSerialized]
		internal TreeGroup parentGroup;
		[NonSerialized]
		internal List<TreeGroup> childGroups = new List<TreeGroup>();
		public int uniqueID
		{
			get
			{
				return this._uniqueID;
			}
			set
			{
				if (this._uniqueID == -1)
				{
					this._uniqueID = value;
				}
			}
		}
		public virtual float GetRadiusAtTime(TreeNode node, float t, bool includeModifications)
		{
			return 0f;
		}
		public virtual bool CanHaveSubGroups()
		{
			return true;
		}
		public void Lock()
		{
			if (this.lockFlags == 0)
			{
				for (int i = 0; i < this.nodes.Count; i++)
				{
					this.nodes[i].baseAngle = this.nodes[i].angle;
				}
			}
			this.lockFlags = 7;
		}
		public void Unlock()
		{
			this.lockFlags = 0;
		}
		internal virtual bool HasExternalChanges()
		{
			return false;
		}
		public bool CheckExternalChanges()
		{
			bool flag = this.HasExternalChanges();
			for (int i = 0; i < this.childGroups.Count; i++)
			{
				flag |= this.childGroups[i].CheckExternalChanges();
			}
			return flag;
		}
		public void UpdateFrequency(TreeData owner)
		{
			Profiler.BeginSample("UpdateFrequency");
			if (this.distributionFrequency < 1)
			{
				this.distributionFrequency = 1;
			}
			if (this.parentGroup == null)
			{
				this.distributionFrequency = 1;
				if (this.nodes.Count < 1)
				{
					owner.AddNode(this, null, false);
				}
			}
			else
			{
				if (this.lockFlags == 0 && this.parentGroup != null)
				{
					int num = 0;
					for (int i = 0; i < this.parentGroup.nodes.Count; i++)
					{
						int num2 = Mathf.RoundToInt((float)this.distributionFrequency * this.parentGroup.nodes[i].GetScale());
						if (num2 < 1)
						{
							num2 = 1;
						}
						for (int j = 0; j < num2; j++)
						{
							if (num < this.nodes.Count)
							{
								owner.SetNodeParent(this.nodes[num], this.parentGroup.nodes[i]);
							}
							else
							{
								owner.AddNode(this, this.parentGroup.nodes[i], false);
							}
							num++;
						}
					}
					if (num < this.nodes.Count)
					{
						List<TreeNode> list = new List<TreeNode>();
						for (int k = num; k < this.nodes.Count; k++)
						{
							list.Add(this.nodes[k]);
						}
						for (int l = 0; l < list.Count; l++)
						{
							owner.DeleteNode(list[l], false);
						}
					}
					this.UpdateSeed();
					this.UpdateDistribution(true, false);
				}
			}
			for (int m = 0; m < this.childGroups.Count; m++)
			{
				this.childGroups[m].UpdateFrequency(owner);
			}
			Profiler.EndSample();
		}
		public void UpdateSeed()
		{
			TreeGroup treeGroup = this;
			while (treeGroup.parentGroup != null)
			{
				treeGroup = treeGroup.parentGroup;
			}
			int num = treeGroup.seed;
			this._internalSeed = num + (int)((float)this.seed * 1.21f);
			for (int i = 0; i < this.nodes.Count; i++)
			{
				this.nodes[i].seed = num + this._internalSeed + (int)((float)i * 3.7482f);
			}
			for (int j = 0; j < this.childGroups.Count; j++)
			{
				this.childGroups[j].UpdateSeed();
			}
		}
		public Vector2 ComputeWindFactor(TreeNode node, float offset)
		{
			Vector2 result;
			if (node.group.parentGroup.GetType() == typeof(TreeGroupRoot))
			{
				result = new Vector2(0f, 0f);
			}
			else
			{
				result = node.parent.group.ComputeWindFactor(node.parent, node.offset);
			}
			float scale = node.GetScale();
			result.x += offset * offset * offset * scale * this.animationPrimary;
			result.y += offset * offset * scale * this.animationSecondary;
			return result;
		}
		private float ComputeOffset(int index, int count, float distributionSum, float distributionStep)
		{
			float num = 0f;
			float num2 = ((float)index + 1f) / ((float)count + 1f) * distributionSum;
			for (float num3 = 0f; num3 <= 1f; num3 += distributionStep)
			{
				num += Mathf.Clamp01(this.distributionCurve.Evaluate(num3));
				if (num >= num2)
				{
					return num3;
				}
			}
			return num2 / distributionSum;
		}
		public float GetRootSpread()
		{
			TreeGroup treeGroup = this;
			while (treeGroup.parentGroup != null)
			{
				treeGroup = treeGroup.parentGroup;
			}
			return treeGroup.nodes[0].size;
		}
		public Matrix4x4 GetRootMatrix()
		{
			TreeGroup treeGroup = this;
			while (treeGroup.parentGroup != null)
			{
				treeGroup = treeGroup.parentGroup;
			}
			return treeGroup.nodes[0].matrix;
		}
		public void UpdateDistribution(bool completeUpdate, bool updateSubGroups)
		{
			Profiler.BeginSample("UpdateDistribution");
			UnityEngine.Random.seed = this._internalSeed;
			if (completeUpdate)
			{
				float num = 0f;
				float[] array = new float[100];
				float distributionStep = 1f / (float)array.Length;
				float num2 = (float)array.Length - 1f;
				for (int i = 0; i < array.Length; i++)
				{
					float time = (float)i / num2;
					array[i] = Mathf.Clamp01(this.distributionCurve.Evaluate(time));
					num += array[i];
				}
				for (int j = 0; j < this.nodes.Count; j++)
				{
					TreeNode treeNode = this.nodes[j];
					if (this.lockFlags == 0)
					{
						if (j == 0 && this.nodes.Count == 1 && (this.parentGroup == null || this.parentGroup.GetType() == typeof(TreeGroupRoot)))
						{
							treeNode.offset = 0f;
							treeNode.baseAngle = 0f;
							treeNode.pitch = 0f;
							treeNode.scale = Mathf.Clamp01(this.distributionScaleCurve.Evaluate(treeNode.offset)) * this.distributionScale + (1f - this.distributionScale);
						}
						else
						{
							int num3 = 0;
							int num4 = 0;
							for (int k = 0; k < this.nodes.Count; k++)
							{
								if (this.nodes[k].parentID == treeNode.parentID)
								{
									if (j == k)
									{
										num3 = num4;
									}
									num4++;
								}
							}
							switch (this.distributionMode)
							{
							case TreeGroup.DistributionMode.Random:
							{
								float offset = 0f;
								float num5 = 0f;
								for (int l = 0; l < 5; l++)
								{
									num5 = UnityEngine.Random.value * num;
								}
								for (int m = 0; m < array.Length; m++)
								{
									offset = (float)m / num2;
									num5 -= array[m];
									if (num5 <= 0f)
									{
										break;
									}
								}
								treeNode.baseAngle = UnityEngine.Random.value * 360f;
								treeNode.offset = offset;
								break;
							}
							case TreeGroup.DistributionMode.Alternate:
							{
								float num6 = this.ComputeOffset(num3, num4, num, distributionStep);
								float num7 = 180f * (float)num3;
								treeNode.baseAngle = num7 + num6 * this.distributionTwirl * 360f;
								treeNode.offset = num6;
								break;
							}
							case TreeGroup.DistributionMode.Opposite:
							{
								float num8 = this.ComputeOffset(num3 / 2, num4 / 2, num, distributionStep);
								float num9 = 90f * (float)(num3 / 2) + (float)(num3 % 2) * 180f;
								treeNode.baseAngle = num9 + num8 * this.distributionTwirl * 360f;
								treeNode.offset = num8;
								break;
							}
							case TreeGroup.DistributionMode.Whorled:
							{
								int num10 = this.distributionNodes;
								int num11 = num3 % num10;
								int num12 = num3 / num10;
								float num13 = this.ComputeOffset(num3 / num10, num4 / num10, num, distributionStep);
								float num14 = 360f / (float)num10 * (float)num11 + 180f / (float)num10 * (float)num12;
								treeNode.baseAngle = num14 + num13 * this.distributionTwirl * 360f;
								treeNode.offset = num13;
								break;
							}
							}
						}
					}
				}
			}
			for (int n = 0; n < this.nodes.Count; n++)
			{
				TreeNode treeNode2 = this.nodes[n];
				if (treeNode2.parent == null)
				{
					treeNode2.visible = true;
				}
				else
				{
					treeNode2.visible = treeNode2.parent.visible;
					if (treeNode2.offset > treeNode2.parent.breakOffset)
					{
						treeNode2.visible = false;
					}
				}
				if (this.lockFlags == 0)
				{
					treeNode2.angle = treeNode2.baseAngle;
					treeNode2.pitch = Mathf.Clamp(this.distributionPitchCurve.Evaluate(treeNode2.offset), -1f, 1f) * -75f * this.distributionPitch;
				}
				else
				{
					treeNode2.angle = treeNode2.baseAngle;
				}
				treeNode2.scale = Mathf.Clamp01(this.distributionScaleCurve.Evaluate(treeNode2.offset)) * this.distributionScale + (1f - this.distributionScale);
			}
			if (this.parentGroup == null || this.parentGroup.GetType() == typeof(TreeGroupRoot))
			{
				for (int num15 = 0; num15 < this.nodes.Count; num15++)
				{
					this.nodes[num15].animSeed = 0f;
				}
			}
			else
			{
				for (int num16 = 0; num16 < this.nodes.Count; num16++)
				{
					if (this.nodes[num16].parent == null)
					{
						this.nodes[num16].animSeed = 0f;
					}
					else
					{
						if (this.nodes[num16].parent.animSeed == 0f)
						{
							this.nodes[num16].animSeed = (float)this.nodes[num16].seed / 9.78f % 1f + 0.001f;
						}
						else
						{
							this.nodes[num16].animSeed = this.nodes[num16].parent.animSeed;
						}
					}
				}
			}
			if (updateSubGroups)
			{
				for (int num17 = 0; num17 < this.childGroups.Count; num17++)
				{
					this.childGroups[num17].UpdateDistribution(completeUpdate, updateSubGroups);
				}
			}
			Profiler.EndSample();
		}
		public virtual void UpdateParameters()
		{
			for (int i = 0; i < this.childGroups.Count; i++)
			{
				this.childGroups[i].UpdateParameters();
			}
		}
		public virtual void BuildAOSpheres(List<TreeAOSphere> aoSpheres)
		{
			Profiler.BeginSample("BuildAOSpheres");
			for (int i = 0; i < this.childGroups.Count; i++)
			{
				this.childGroups[i].BuildAOSpheres(aoSpheres);
			}
			Profiler.EndSample();
		}
		public virtual void UpdateMesh(List<TreeMaterial> materials, List<TreeVertex> verts, List<TreeTriangle> tris, List<TreeAOSphere> aoSpheres, int buildFlags, float adaptiveQuality, float aoDensity)
		{
			for (int i = 0; i < this.childGroups.Count; i++)
			{
				this.childGroups[i].UpdateMesh(materials, verts, tris, aoSpheres, buildFlags, adaptiveQuality, aoDensity);
			}
		}
		public virtual void UpdateMatrix()
		{
		}
		protected static int GetMaterialIndex(Material m, List<TreeMaterial> materials, bool tileV)
		{
			for (int i = 0; i < materials.Count; i++)
			{
				if (materials[i].material == m)
				{
					materials[i].tileV |= tileV;
					return i;
				}
			}
			materials.Add(new TreeMaterial
			{
				material = m,
				tileV = tileV
			});
			return materials.Count - 1;
		}
		protected static Vector4 CreateTangent(TreeNode node, Quaternion rot, Vector3 normal)
		{
			Vector3 vector = node.matrix.MultiplyVector(rot * new Vector3(1f, 0f, 0f));
			vector -= normal * Vector3.Dot(vector, normal);
			vector.Normalize();
			return new Vector4(vector.x, vector.y, vector.z, 1f);
		}
		protected static float ComputeAmbientOcclusion(Vector3 pos, Vector3 nor, List<TreeAOSphere> aoSpheres, float aoDensity)
		{
			if (aoSpheres.Count == 0)
			{
				return 1f;
			}
			float num = 0f;
			for (int i = 0; i < aoSpheres.Count; i++)
			{
				num += aoSpheres[i].PointOcclusion(pos, nor) * aoSpheres[i].density * 0.25f;
			}
			return 1f - Mathf.Clamp01(num) * aoDensity;
		}
	}
}
