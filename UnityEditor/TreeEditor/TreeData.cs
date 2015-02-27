using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace TreeEditor
{
	public class TreeData : ScriptableObject
	{
		[SerializeField]
		private int _uniqueID;
		public string materialHash;
		public TreeGroupRoot root;
		public TreeGroupBranch[] branchGroups;
		public TreeGroupLeaf[] leafGroups;
		public TreeNode[] nodes;
		public Mesh mesh;
		public Material optimizedSolidMaterial;
		public Material optimizedCutoutMaterial;
		public bool isInPreviewMode;
		public TreeGroup GetGroup(int id)
		{
			if (id == this.root.uniqueID)
			{
				return this.root;
			}
			for (int i = 0; i < this.branchGroups.Length; i++)
			{
				if (this.branchGroups[i].uniqueID == id)
				{
					return this.branchGroups[i];
				}
			}
			for (int j = 0; j < this.leafGroups.Length; j++)
			{
				if (this.leafGroups[j].uniqueID == id)
				{
					return this.leafGroups[j];
				}
			}
			return null;
		}
		public TreeNode GetNode(int id)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i].uniqueID == id)
				{
					return this.nodes[i];
				}
			}
			return null;
		}
		private int GetNodeCount()
		{
			return this.nodes.Length;
		}
		private TreeNode GetNodeAt(int i)
		{
			if (i >= 0 && i < this.nodes.Length)
			{
				return this.nodes[i];
			}
			return null;
		}
		private int GetGroupCount()
		{
			return 1 + this.branchGroups.Length + this.leafGroups.Length;
		}
		private TreeGroup GetGroupAt(int i)
		{
			if (i == 0)
			{
				return this.root;
			}
			i--;
			if (i >= 0 && i < this.branchGroups.Length)
			{
				return this.branchGroups[i];
			}
			i -= this.branchGroups.Length;
			if (i >= 0 && i < this.leafGroups.Length)
			{
				return this.leafGroups[i];
			}
			return null;
		}
		public void ValidateReferences()
		{
			Profiler.BeginSample("ValidateReferences");
			int groupCount = this.GetGroupCount();
			for (int i = 0; i < groupCount; i++)
			{
				TreeGroup groupAt = this.GetGroupAt(i);
				groupAt.parentGroup = this.GetGroup(groupAt.parentGroupID);
				groupAt.childGroups.Clear();
				groupAt.nodes.Clear();
				for (int j = 0; j < groupAt.childGroupIDs.Length; j++)
				{
					TreeGroup group = this.GetGroup(groupAt.childGroupIDs[j]);
					groupAt.childGroups.Add(group);
				}
				for (int k = 0; k < groupAt.nodeIDs.Length; k++)
				{
					TreeNode node = this.GetNode(groupAt.nodeIDs[k]);
					groupAt.nodes.Add(node);
				}
			}
			int nodeCount = this.GetNodeCount();
			for (int l = 0; l < nodeCount; l++)
			{
				TreeNode nodeAt = this.GetNodeAt(l);
				nodeAt.parent = this.GetNode(nodeAt.parentID);
				nodeAt.group = this.GetGroup(nodeAt.groupID);
			}
			Profiler.EndSample();
		}
		public void ClearReferences()
		{
			for (int i = 0; i < this.branchGroups.Length; i++)
			{
				this.branchGroups[i].parentGroup = null;
				this.branchGroups[i].childGroups.Clear();
				this.branchGroups[i].nodes.Clear();
			}
			for (int j = 0; j < this.leafGroups.Length; j++)
			{
				this.leafGroups[j].parentGroup = null;
				this.leafGroups[j].childGroups.Clear();
				this.leafGroups[j].nodes.Clear();
			}
			for (int k = 0; k < this.nodes.Length; k++)
			{
				this.nodes[k].parent = null;
				this.nodes[k].group = null;
			}
		}
		public TreeGroup AddGroup(TreeGroup parent, Type type)
		{
			TreeGroup treeGroup;
			if (type == typeof(TreeGroupBranch))
			{
				treeGroup = new TreeGroupBranch();
				this.branchGroups = this.ArrayAdd(this.branchGroups, treeGroup as TreeGroupBranch);
			}
			else
			{
				if (type != typeof(TreeGroupLeaf))
				{
					return null;
				}
				treeGroup = new TreeGroupLeaf();
				this.leafGroups = this.ArrayAdd(this.leafGroups, treeGroup as TreeGroupLeaf);
			}
			treeGroup.uniqueID = this._uniqueID;
			this._uniqueID++;
			treeGroup.parentGroupID = 0;
			treeGroup.distributionFrequency = 1;
			this.SetGroupParent(treeGroup, parent);
			return treeGroup;
		}
		private void CopyFields(object n, object n2)
		{
			if (n.GetType() != n2.GetType())
			{
				return;
			}
			FieldInfo[] fields = n.GetType().GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].IsPublic)
				{
					if (fields[i].FieldType == typeof(TreeSpline))
					{
						TreeSpline o = fields[i].GetValue(n) as TreeSpline;
						fields[i].SetValue(n2, new TreeSpline(o));
					}
					else
					{
						if (fields[i].FieldType == typeof(AnimationCurve))
						{
							AnimationCurve animationCurve = fields[i].GetValue(n) as AnimationCurve;
							AnimationCurve animationCurve2 = new AnimationCurve(animationCurve.keys);
							animationCurve2.postWrapMode = animationCurve.postWrapMode;
							animationCurve2.preWrapMode = animationCurve.preWrapMode;
							fields[i].SetValue(n2, animationCurve2);
						}
						else
						{
							fields[i].SetValue(n2, fields[i].GetValue(n));
						}
					}
				}
			}
		}
		public TreeGroup DuplicateGroup(TreeGroup g)
		{
			TreeGroup treeGroup = this.AddGroup(this.GetGroup(g.parentGroupID), g.GetType());
			this.CopyFields(g, treeGroup);
			treeGroup.childGroupIDs = new int[0];
			treeGroup.nodeIDs = new int[0];
			for (int i = 0; i < g.nodeIDs.Length; i++)
			{
				TreeNode node = this.GetNode(g.nodeIDs[i]);
				TreeNode treeNode = this.AddNode(treeGroup, this.GetNode(node.parentID));
				this.CopyFields(node, treeNode);
				treeNode.groupID = treeGroup.uniqueID;
			}
			return treeGroup;
		}
		public void DeleteGroup(TreeGroup g)
		{
			for (int i = g.nodes.Count - 1; i >= 0; i--)
			{
				this.DeleteNode(g.nodes[i], false);
			}
			if (g.GetType() == typeof(TreeGroupBranch))
			{
				this.branchGroups = this.ArrayRemove(this.branchGroups, g as TreeGroupBranch);
			}
			else
			{
				if (g.GetType() == typeof(TreeGroupLeaf))
				{
					this.leafGroups = this.ArrayRemove(this.leafGroups, g as TreeGroupLeaf);
				}
			}
			this.SetGroupParent(g, null);
		}
		public void SetGroupParent(TreeGroup g, TreeGroup parent)
		{
			TreeGroup group = this.GetGroup(g.parentGroupID);
			if (group != null)
			{
				group.childGroupIDs = this.ArrayRemove(group.childGroupIDs, g.uniqueID);
				group.childGroups.Remove(g);
			}
			if (parent != null)
			{
				g.parentGroup = parent;
				g.parentGroupID = parent.uniqueID;
				parent.childGroups.Add(g);
				parent.childGroupIDs = this.ArrayAdd(parent.childGroupIDs, g.uniqueID);
			}
			else
			{
				g.parentGroup = null;
				g.parentGroupID = 0;
			}
			this.ValidateReferences();
			this.UpdateFrequency(g.uniqueID);
		}
		public void LockGroup(TreeGroup g)
		{
			g.Lock();
		}
		public void UnlockGroup(TreeGroup g)
		{
			g.Unlock();
			this.UpdateFrequency(g.uniqueID);
		}
		public bool IsAncestor(TreeGroup ancestor, TreeGroup g)
		{
			if (g == null)
			{
				return false;
			}
			for (TreeGroup group = this.GetGroup(g.parentGroupID); group != null; group = this.GetGroup(group.parentGroupID))
			{
				if (group == ancestor)
				{
					return true;
				}
			}
			return false;
		}
		public TreeNode AddNode(TreeGroup g, TreeNode parent)
		{
			return this.AddNode(g, parent, true);
		}
		public TreeNode AddNode(TreeGroup g, TreeNode parent, bool validate)
		{
			if (g == null)
			{
				return null;
			}
			TreeNode treeNode = new TreeNode();
			treeNode.uniqueID = this._uniqueID;
			this._uniqueID++;
			this.SetNodeParent(treeNode, parent);
			treeNode.groupID = g.uniqueID;
			treeNode.group = g;
			g.nodeIDs = this.ArrayAdd(g.nodeIDs, treeNode.uniqueID);
			g.nodes.Add(treeNode);
			this.nodes = this.ArrayAdd(this.nodes, treeNode);
			if (validate)
			{
				this.ValidateReferences();
			}
			return treeNode;
		}
		public void SetNodeParent(TreeNode n, TreeNode parent)
		{
			if (parent != null)
			{
				n.parentID = parent.uniqueID;
				n.parent = parent;
			}
			else
			{
				n.parentID = 0;
				n.parent = null;
			}
		}
		public void DeleteNode(TreeNode n)
		{
			this.DeleteNode(n, true);
		}
		public void DeleteNode(TreeNode n, bool validate)
		{
			TreeGroup group = this.GetGroup(n.groupID);
			if (group != null)
			{
				group.nodeIDs = this.ArrayRemove(group.nodeIDs, n.uniqueID);
				group.nodes.Remove(n);
				for (int i = 0; i < group.childGroups.Count; i++)
				{
					TreeGroup treeGroup = group.childGroups[i];
					for (int j = treeGroup.nodes.Count - 1; j >= 0; j--)
					{
						if (treeGroup.nodes[j] != null && treeGroup.nodes[j].parentID == n.uniqueID)
						{
							this.DeleteNode(treeGroup.nodes[j], false);
						}
					}
				}
			}
			n.group = null;
			n.groupID = 0;
			n.parent = null;
			n.parentID = 0;
			this.nodes = this.ArrayRemove(this.nodes, n);
			if (validate)
			{
				this.ValidateReferences();
			}
		}
		public TreeNode DuplicateNode(TreeNode n)
		{
			TreeGroup group = this.GetGroup(n.groupID);
			if (group == null)
			{
				return null;
			}
			TreeNode treeNode = this.AddNode(group, this.GetNode(n.parentID));
			this.CopyFields(n, treeNode);
			return treeNode;
		}
		public void Initialize()
		{
			if (this.root == null)
			{
				this.branchGroups = new TreeGroupBranch[0];
				this.leafGroups = new TreeGroupLeaf[0];
				this.nodes = new TreeNode[0];
				this._uniqueID = 1;
				this.root = new TreeGroupRoot();
				this.root.uniqueID = this._uniqueID;
				this.root.distributionFrequency = 1;
				this._uniqueID++;
				this.UpdateFrequency(this.root.uniqueID);
				this.AddGroup(this.root, typeof(TreeGroupBranch));
			}
		}
		public void UpdateSeed(int id)
		{
			TreeGroup group = this.GetGroup(id);
			if (group == null)
			{
				return;
			}
			int seed = UnityEngine.Random.seed;
			this.ClearReferences();
			this.ValidateReferences();
			group.UpdateSeed();
			group.UpdateDistribution(true, true);
			this.ClearReferences();
			UnityEngine.Random.seed = seed;
		}
		public void UpdateFrequency(int id)
		{
			TreeGroup group = this.GetGroup(id);
			if (group == null)
			{
				return;
			}
			int seed = UnityEngine.Random.seed;
			this.ClearReferences();
			this.ValidateReferences();
			group.UpdateFrequency(this);
			this.ClearReferences();
			UnityEngine.Random.seed = seed;
		}
		public void UpdateDistribution(int id)
		{
			TreeGroup group = this.GetGroup(id);
			if (group == null)
			{
				return;
			}
			int seed = UnityEngine.Random.seed;
			this.ClearReferences();
			this.ValidateReferences();
			group.UpdateDistribution(true, true);
			this.ClearReferences();
			UnityEngine.Random.seed = seed;
		}
		public static int GetAdaptiveHeightSegments(float h, float adaptiveQuality)
		{
			return (int)Mathf.Max(h * adaptiveQuality, 2f);
		}
		public static int GetAdaptiveRadialSegments(float r, float adaptiveQuality)
		{
			int value = (int)(r * 24f * adaptiveQuality) / 2 * 2;
			return Mathf.Clamp(value, 4, 32);
		}
		public static List<float> GetAdaptiveSamples(TreeGroup group, TreeNode node, float adaptiveQuality)
		{
			List<float> list = new List<float>();
			if (node.spline == null)
			{
				return list;
			}
			float num = 1f - node.capRange;
			SplineNode[] array = node.spline.GetNodes();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= node.breakOffset)
				{
					list.Add(node.breakOffset);
					break;
				}
				if (array[i].time > num)
				{
					list.Add(num);
					break;
				}
				list.Add(array[i].time);
			}
			list.Sort();
			if (list.Count < 2)
			{
				return list;
			}
			float num2 = 1f;
			if (group.GetType() == typeof(TreeGroupBranch))
			{
				num2 = ((TreeGroupBranch)group).radius;
			}
			float num3 = Mathf.Lerp(0.999f, 0.99999f, adaptiveQuality);
			float num4 = Mathf.Lerp(0.5f, 0.985f, adaptiveQuality);
			float num5 = Mathf.Lerp(0.3f * num2, 0.1f * num2, adaptiveQuality);
			int num6 = 200;
			int j = 0;
			while (j < list.Count - 1)
			{
				for (int k = j; k < list.Count - 1; k++)
				{
					Quaternion rotationAtTime = node.spline.GetRotationAtTime(list[k]);
					Quaternion rotationAtTime2 = node.spline.GetRotationAtTime(list[k + 1]);
					Vector3 lhs = rotationAtTime * Vector3.up;
					Vector3 rhs = rotationAtTime2 * Vector3.up;
					Vector3 lhs2 = rotationAtTime * Vector3.right;
					Vector3 rhs2 = rotationAtTime2 * Vector3.right;
					Vector3 lhs3 = rotationAtTime * Vector3.forward;
					Vector3 rhs3 = rotationAtTime2 * Vector3.forward;
					float radiusAtTime = group.GetRadiusAtTime(node, list[k], true);
					float radiusAtTime2 = group.GetRadiusAtTime(node, list[k + 1], true);
					bool flag = false;
					if (Vector3.Dot(lhs, rhs) < num4)
					{
						flag = true;
					}
					if (Vector3.Dot(lhs2, rhs2) < num4)
					{
						flag = true;
					}
					if (Vector3.Dot(lhs3, rhs3) < num4)
					{
						flag = true;
					}
					if (Mathf.Abs(radiusAtTime - radiusAtTime2) > num5)
					{
						flag = true;
					}
					if (flag)
					{
						num6--;
						if (num6 > 0)
						{
							float item = (list[k] + list[k + 1]) * 0.5f;
							list.Insert(k + 1, item);
							break;
						}
					}
					j = k + 1;
				}
			}
			for (int l = 0; l < list.Count - 2; l++)
			{
				Vector3 positionAtTime = node.spline.GetPositionAtTime(list[l]);
				Vector3 positionAtTime2 = node.spline.GetPositionAtTime(list[l + 1]);
				Vector3 positionAtTime3 = node.spline.GetPositionAtTime(list[l + 2]);
				float radiusAtTime3 = group.GetRadiusAtTime(node, list[l], true);
				float radiusAtTime4 = group.GetRadiusAtTime(node, list[l + 1], true);
				float radiusAtTime5 = group.GetRadiusAtTime(node, list[l + 2], true);
				Vector3 normalized = (positionAtTime2 - positionAtTime).normalized;
				Vector3 normalized2 = (positionAtTime3 - positionAtTime).normalized;
				bool flag2 = false;
				if (Vector3.Dot(normalized, normalized2) >= num3)
				{
					flag2 = true;
				}
				if (Mathf.Abs(radiusAtTime3 - radiusAtTime4) > num5)
				{
					flag2 = false;
				}
				if (Mathf.Abs(radiusAtTime4 - radiusAtTime5) > num5)
				{
					flag2 = false;
				}
				if (flag2)
				{
					list.RemoveAt(l + 1);
					l--;
				}
			}
			if (node.capRange > 0f)
			{
				int num7 = 1 + Mathf.CeilToInt(node.capRange * 16f * adaptiveQuality);
				for (int m = 0; m < num7; m++)
				{
					float f = (float)(m + 1) / (float)num7 * 3.14159274f * 0.5f;
					float num8 = Mathf.Sin(f);
					float num9 = num + node.capRange * num8;
					if (num9 < node.breakOffset)
					{
						list.Add(num9);
					}
				}
				list.Sort();
			}
			if (1f <= node.breakOffset)
			{
				if (list[list.Count - 1] < 1f)
				{
					list.Add(1f);
				}
				else
				{
					list[list.Count - 1] = 1f;
				}
			}
			return list;
		}
		public void PreviewMesh(Matrix4x4 worldToLocalMatrix, out Material[] outMaterials)
		{
			outMaterials = null;
			if (!this.mesh)
			{
				Debug.LogError("TreeData must have mesh  assigned");
				return;
			}
			bool enableAmbientOcclusion = this.root.enableAmbientOcclusion;
			float adaptiveLODQuality = this.root.adaptiveLODQuality;
			this.root.enableMaterialOptimize = false;
			this.root.enableWelding = false;
			this.root.enableAmbientOcclusion = false;
			this.root.adaptiveLODQuality = 0f;
			this.UpdateMesh(worldToLocalMatrix, out outMaterials);
			this.root.enableWelding = true;
			this.root.enableMaterialOptimize = true;
			this.root.enableAmbientOcclusion = enableAmbientOcclusion;
			this.root.adaptiveLODQuality = adaptiveLODQuality;
			this.isInPreviewMode = true;
		}
		public void UpdateMesh(Matrix4x4 worldToLocalMatrix, out Material[] outMaterials)
		{
			outMaterials = null;
			if (!this.mesh)
			{
				Debug.LogError("TreeData must have mesh  assigned");
				return;
			}
			this.isInPreviewMode = false;
			List<TreeMaterial> materials = new List<TreeMaterial>();
			List<TreeVertex> list = new List<TreeVertex>();
			List<TreeTriangle> list2 = new List<TreeTriangle>();
			List<TreeAOSphere> aoSpheres = new List<TreeAOSphere>();
			int num = 0;
			if (this.root.enableAmbientOcclusion)
			{
				num |= 1;
			}
			if (this.root.enableWelding)
			{
				num |= 2;
			}
			this.UpdateMesh(worldToLocalMatrix, materials, list, list2, aoSpheres, num, this.root.adaptiveLODQuality, this.root.aoDensity);
			if (list.Count > 65000)
			{
				Debug.LogWarning("Tree mesh would exceed maximum vertex limit .. aborting");
				return;
			}
			this.mesh.Clear();
			if (list.Count == 0 || list2.Count == 0)
			{
				return;
			}
			this.OptimizeMaterial(materials, list, list2);
			Profiler.BeginSample("CopyMeshData");
			Vector3[] array = new Vector3[list.Count];
			Vector3[] array2 = new Vector3[list.Count];
			Vector2[] array3 = new Vector2[list.Count];
			Vector2[] array4 = new Vector2[list.Count];
			Vector4[] array5 = new Vector4[list.Count];
			Color[] array6 = new Color[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = list[i].pos;
				array2[i] = list[i].nor;
				array3[i] = list[i].uv0;
				array4[i] = list[i].uv1;
				array5[i] = list[i].tangent;
				array6[i] = list[i].color;
			}
			this.mesh.vertices = array;
			this.mesh.normals = array2;
			this.mesh.uv = array3;
			this.mesh.uv1 = array4;
			this.mesh.tangents = array5;
			this.mesh.colors = array6;
			int[] array7 = new int[list2.Count * 3];
			List<Material> list3 = new List<Material>(2);
			this.mesh.subMeshCount = 2;
			for (int j = 0; j < 2; j++)
			{
				int num2 = 0;
				for (int k = 0; k < list2.Count; k++)
				{
					if (list2[k].materialIndex == j)
					{
						array7[num2] = list2[k].v[0];
						array7[num2 + 1] = list2[k].v[1];
						array7[num2 + 2] = list2[k].v[2];
						num2 += 3;
					}
				}
				if (num2 > 0)
				{
					int[] array8 = new int[num2];
					for (int l = 0; l < num2; l++)
					{
						array8[l] = array7[l];
					}
					this.mesh.SetTriangles(array8, list3.Count);
					if (j == 0)
					{
						list3.Add(this.optimizedSolidMaterial);
					}
					else
					{
						list3.Add(this.optimizedCutoutMaterial);
					}
				}
			}
			outMaterials = list3.ToArray();
			this.mesh.subMeshCount = list3.Count;
			Profiler.EndSample();
			this.mesh.RecalculateBounds();
		}
		private static void ExtractOptimizedShaders(List<TreeMaterial> materials, out Shader optimizedSolidShader, out Shader optimizedCutoutShader)
		{
			List<Shader> list = new List<Shader>();
			List<Shader> list2 = new List<Shader>();
			foreach (TreeMaterial current in materials)
			{
				Material material = current.material;
				if (material && material.shader)
				{
					if (TreeEditorHelper.IsTreeBarkShader(material.shader))
					{
						list.Add(material.shader);
					}
					else
					{
						if (TreeEditorHelper.IsTreeLeafShader(material.shader))
						{
							list2.Add(material.shader);
						}
					}
				}
			}
			optimizedSolidShader = null;
			optimizedCutoutShader = null;
			if (list.Count > 0)
			{
				optimizedSolidShader = Shader.Find(TreeEditorHelper.GetOptimizedShaderName(list[0]));
			}
			if (list2.Count > 0)
			{
				optimizedCutoutShader = Shader.Find(TreeEditorHelper.GetOptimizedShaderName(list2[0]));
			}
			if (!optimizedSolidShader)
			{
				optimizedSolidShader = TreeEditorHelper.DefaultOptimizedBarkShader;
			}
			if (!optimizedCutoutShader)
			{
				optimizedCutoutShader = TreeEditorHelper.DefaultOptimizedLeafShader;
			}
		}
		public bool OptimizeMaterial(List<TreeMaterial> materials, List<TreeVertex> vertices, List<TreeTriangle> triangles)
		{
			if (!this.optimizedSolidMaterial || !this.optimizedCutoutMaterial)
			{
				Debug.LogError("Optimized materials haven't been assigned");
				return false;
			}
			Shader shader;
			Shader shader2;
			TreeData.ExtractOptimizedShaders(materials, out shader, out shader2);
			this.optimizedSolidMaterial.shader = shader;
			this.optimizedCutoutMaterial.shader = shader2;
			int num = 1024;
			int num2 = 1024;
			int padding = 32;
			Profiler.BeginSample("OptimizeMaterial");
			float[] array = new float[materials.Count];
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < materials.Count; i++)
			{
				if (!materials[i].tileV)
				{
					num4 += 1f;
				}
				else
				{
					num3 += 1f;
				}
			}
			for (int j = 0; j < materials.Count; j++)
			{
				if (materials[j].tileV)
				{
					array[j] = 1f;
				}
				else
				{
					array[j] = 1f / num4;
				}
			}
			TextureAtlas textureAtlas = new TextureAtlas();
			for (int k = 0; k < materials.Count; k++)
			{
				Texture2D texture2D = null;
				Texture2D normal = null;
				Texture2D gloss = null;
				Texture2D transtex = null;
				Texture2D shadowOffsetTex = null;
				Color color = new Color(1f, 1f, 1f, 1f);
				float num5 = 0.03f;
				Vector2 textureScale = new Vector2(1f, 1f);
				Material material = materials[k].material;
				if (material)
				{
					if (material.HasProperty("_Color"))
					{
						color = material.GetColor("_Color");
					}
					if (material.HasProperty("_MainTex"))
					{
						texture2D = (material.mainTexture as Texture2D);
						textureScale = material.GetTextureScale("_MainTex");
					}
					if (material.HasProperty("_BumpMap"))
					{
						normal = (material.GetTexture("_BumpMap") as Texture2D);
					}
					if (material.HasProperty("_GlossMap"))
					{
						gloss = (material.GetTexture("_GlossMap") as Texture2D);
					}
					if (material.HasProperty("_TranslucencyMap"))
					{
						transtex = (material.GetTexture("_TranslucencyMap") as Texture2D);
					}
					if (material.HasProperty("_Shininess"))
					{
						num5 = material.GetFloat("_Shininess");
					}
					if (material.HasProperty("_ShadowOffset"))
					{
						shadowOffsetTex = (material.GetTexture("_ShadowOffset") as Texture2D);
					}
				}
				num5 = Mathf.Clamp(num5, 0.03f, 1f);
				Vector2 scale = new Vector2(array[k], array[k]);
				if (texture2D)
				{
					scale.x *= (float)num / (float)texture2D.width;
					scale.y *= (float)num2 / (float)texture2D.height;
				}
				bool tileV = materials[k].tileV;
				if (!tileV)
				{
					textureScale = new Vector2(1f, 1f);
				}
				textureAtlas.AddTexture("tex" + k, texture2D, color, normal, gloss, transtex, shadowOffsetTex, num5, scale, tileV, textureScale);
			}
			textureAtlas.Pack(ref num, num2, padding, true);
			this.UpdateTextures(textureAtlas, materials);
			Rect rect = default(Rect);
			Vector2 texTiling = new Vector2(1f, 1f);
			int num6 = -1;
			for (int l = 0; l < triangles.Count; l++)
			{
				TreeTriangle treeTriangle = triangles[l];
				if (treeTriangle.materialIndex != num6)
				{
					num6 = treeTriangle.materialIndex;
					rect = textureAtlas.GetUVRect("tex" + treeTriangle.materialIndex);
					texTiling = textureAtlas.GetTexTiling("tex" + treeTriangle.materialIndex);
				}
				for (int m = 0; m < 3; m++)
				{
					TreeVertex treeVertex = vertices[treeTriangle.v[m]];
					if (!treeVertex.flag)
					{
						treeVertex.uv0.x = rect.x + treeVertex.uv0.x * rect.width;
						treeVertex.uv0.y = (rect.y + treeVertex.uv0.y * rect.height) * texTiling.y;
						treeVertex.flag = true;
					}
				}
				if (treeTriangle.isCutout)
				{
					treeTriangle.materialIndex = 1;
				}
				else
				{
					treeTriangle.materialIndex = 0;
				}
			}
			Profiler.EndSample();
			return true;
		}
		private static Texture2D[] WriteOptimizedTextures(string treeAssetPath, Texture2D[] textures)
		{
			string[] array = new string[textures.Length];
			string text = Path.Combine(Path.GetDirectoryName(treeAssetPath), Path.GetFileNameWithoutExtension(treeAssetPath) + "_Textures");
			Directory.CreateDirectory(text);
			for (int i = 0; i < textures.Length; i++)
			{
				byte[] bytes = textures[i].EncodeToPNG();
				array[i] = Path.Combine(text, textures[i].name + ".png");
				File.WriteAllBytes(array[i], bytes);
			}
			AssetDatabase.Refresh();
			for (int j = 0; j < textures.Length; j++)
			{
				textures[j] = (AssetDatabase.LoadMainAssetAtPath(array[j]) as Texture2D);
			}
			return textures;
		}
		public bool CheckExternalChanges()
		{
			this.ValidateReferences();
			return this.root.CheckExternalChanges();
		}
		private void UpdateShadowTexture(Texture2D shadowTexture, int texWidth, int texHeight)
		{
			if (!shadowTexture)
			{
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(shadowTexture);
			TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			int[] array = new int[]
			{
				1,
				2,
				4,
				8,
				16
			};
			int num = Mathf.Max(8, Mathf.ClosestPowerOfTwo(Mathf.Min(texWidth, texHeight) / array[this.root.shadowTextureQuality]));
			if (num != textureImporter.maxTextureSize)
			{
				textureImporter.maxTextureSize = num;
				textureImporter.mipmapEnabled = true;
				AssetDatabase.ImportAsset(assetPath);
			}
		}
		private bool UpdateTextures(TextureAtlas atlas, List<TreeMaterial> materials)
		{
			if (!this.root.enableMaterialOptimize)
			{
				return false;
			}
			bool flag = this.optimizedSolidMaterial.GetTexture("_MainTex") != null && this.optimizedSolidMaterial.GetTexture("_BumpSpecMap") != null && this.optimizedSolidMaterial.GetTexture("_TranslucencyMap") != null && this.optimizedCutoutMaterial.GetTexture("_MainTex") != null && this.optimizedCutoutMaterial.GetTexture("_ShadowTex") != null && this.optimizedCutoutMaterial.GetTexture("_BumpSpecMap") != null && this.optimizedCutoutMaterial.GetTexture("_TranslucencyMap");
			UnityEngine.Object[] array = new UnityEngine.Object[materials.Count];
			for (int i = 0; i < materials.Count; i++)
			{
				array[i] = materials[i].material;
			}
			string text = InternalEditorUtility.CalculateHashForObjectsAndDependencies(array);
			text += atlas.GetHashCode();
			if (this.materialHash == text && flag)
			{
				this.UpdateShadowTexture(this.optimizedCutoutMaterial.GetTexture("_ShadowTex") as Texture2D, atlas.atlasWidth, atlas.atlasHeight);
				return false;
			}
			this.materialHash = text;
			int atlasWidth = atlas.atlasWidth;
			int atlasHeight = atlas.atlasHeight;
			int atlasPadding = atlas.atlasPadding;
			Texture2D texture2D = new Texture2D(atlasWidth, atlasHeight, TextureFormat.ARGB32, true);
			Texture2D texture2D2 = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGB24, true);
			Texture2D texture2D3 = new Texture2D(atlasWidth, atlasHeight, TextureFormat.ARGB32, true);
			Texture2D texture2D4 = new Texture2D(atlasWidth, atlasHeight, TextureFormat.ARGB32, true);
			texture2D.name = "diffuse";
			texture2D2.name = "shadow";
			texture2D3.name = "normal_specular";
			texture2D4.name = "translucency_gloss";
			SavedRenderTargetState savedRenderTargetState = new SavedRenderTargetState();
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
			RenderTexture temporary = RenderTexture.GetTemporary(atlasWidth, atlasHeight, 0, RenderTextureFormat.ARGB32);
			Color white = Color.white;
			Color color = new Color(0.03f, 0.5f, 0f, 0.5f);
			Color color2 = new Color(0f, 0f, 0f, 0f);
			Texture2D texture2D5 = new Texture2D(1, 1);
			texture2D5.SetPixel(0, 0, white);
			texture2D5.Apply();
			Texture2D texture2D6 = new Texture2D(1, 1);
			texture2D6.SetPixel(0, 0, white);
			texture2D6.Apply();
			Texture2D texture2D7 = new Texture2D(1, 1);
			texture2D7.SetPixel(0, 0, color);
			texture2D7.Apply();
			Texture2D texture2D8 = new Texture2D(1, 1);
			texture2D8.SetPixel(0, 0, color2);
			texture2D8.Apply();
			Texture2D texture2D9 = texture2D8;
			Texture2D texture2D10 = new Texture2D(1, 1);
			texture2D10.SetPixel(0, 0, Color.white);
			texture2D10.Apply();
			Material material = EditorGUIUtility.LoadRequired("Inspectors/TreeCreator/TreeTextureCombinerMaterial.mat") as Material;
			for (int j = 0; j < 4; j++)
			{
				RenderTexture.active = temporary;
				GL.LoadPixelMatrix(0f, (float)atlasWidth, 0f, (float)atlasHeight);
				material.SetVector("_TexSize", new Vector4((float)atlasWidth, (float)atlasHeight, 0f, 0f));
				switch (j)
				{
				case 0:
					GL.Clear(false, true, color);
					break;
				case 1:
					GL.Clear(false, true, color2);
					break;
				case 2:
					GL.Clear(false, true, color2);
					break;
				case 3:
					GL.Clear(false, true, color2);
					break;
				}
				for (int k = 0; k < atlas.nodes.Count; k++)
				{
					TextureAtlas.TextureNode textureNode = atlas.nodes[k];
					Rect packedRect = textureNode.packedRect;
					Texture texture = null;
					Texture texture2 = null;
					Color color3 = default(Color);
					switch (j)
					{
					case 0:
						texture = textureNode.normalTexture;
						texture2 = textureNode.shadowOffsetTexture;
						color3 = new Color(textureNode.shininess, 0f, 0f, 0f);
						if (texture == null)
						{
							texture = texture2D7;
						}
						if (texture2 == null)
						{
							texture2 = texture2D9;
						}
						break;
					case 1:
						texture = textureNode.diffuseTexture;
						color3 = textureNode.diffuseColor;
						if (texture == null)
						{
							texture = texture2D5;
						}
						break;
					case 2:
						texture = textureNode.translucencyTexture;
						texture2 = textureNode.glossTexture;
						if (texture == null)
						{
							texture = texture2D10;
						}
						if (texture2 == null)
						{
							texture2 = texture2D8;
						}
						break;
					case 3:
						texture2 = textureNode.diffuseTexture;
						if (texture2 == null)
						{
							texture2 = texture2D5;
						}
						break;
					}
					if (textureNode.tileV)
					{
						float x = packedRect.x;
						float num = (float)atlasPadding / 2f;
						for (float num2 = num; num2 > 0f; num2 -= 1f)
						{
							Rect rect = new Rect(packedRect);
							Rect rect2 = new Rect(packedRect);
							rect.x = x - num2;
							rect2.x = x + num2;
							this.DrawTexture(rect, texture, texture2, material, color3, j);
							this.DrawTexture(rect2, texture, texture2, material, color3, j);
						}
					}
					this.DrawTexture(packedRect, texture, texture2, material, color3, j);
				}
				switch (j)
				{
				case 0:
					texture2D3.ReadPixels(new Rect(0f, 0f, (float)atlasWidth, (float)atlasHeight), 0, 0);
					texture2D3.Apply(true);
					break;
				case 1:
					texture2D.ReadPixels(new Rect(0f, 0f, (float)atlasWidth, (float)atlasHeight), 0, 0);
					texture2D.Apply(true);
					break;
				case 2:
					texture2D4.ReadPixels(new Rect(0f, 0f, (float)atlasWidth, (float)atlasHeight), 0, 0);
					texture2D4.Apply(true);
					break;
				case 3:
					texture2D2.ReadPixels(new Rect(0f, 0f, (float)atlasWidth, (float)atlasHeight), 0, 0);
					texture2D2.Apply(true);
					break;
				}
			}
			savedRenderTargetState.Restore();
			this.optimizedSolidMaterial.SetPass(0);
			RenderTexture.ReleaseTemporary(temporary);
			UnityEngine.Object.DestroyImmediate(texture2D5);
			UnityEngine.Object.DestroyImmediate(texture2D6);
			UnityEngine.Object.DestroyImmediate(texture2D10);
			UnityEngine.Object.DestroyImmediate(texture2D8);
			UnityEngine.Object.DestroyImmediate(texture2D7);
			EditorUtility.SetTemporarilyAllowIndieRenderTexture(false);
			Texture2D[] array2 = new Texture2D[]
			{
				texture2D,
				texture2D3,
				texture2D4,
				texture2D2
			};
			array2 = TreeData.WriteOptimizedTextures(AssetDatabase.GetAssetPath(this), array2);
			UnityEngine.Object.DestroyImmediate(texture2D);
			UnityEngine.Object.DestroyImmediate(texture2D3);
			UnityEngine.Object.DestroyImmediate(texture2D4);
			UnityEngine.Object.DestroyImmediate(texture2D2);
			this.optimizedSolidMaterial.SetTexture("_MainTex", array2[0]);
			this.optimizedSolidMaterial.SetTexture("_BumpSpecMap", array2[1]);
			this.optimizedSolidMaterial.SetTexture("_TranslucencyMap", array2[2]);
			this.optimizedCutoutMaterial.SetTexture("_MainTex", array2[0]);
			this.optimizedCutoutMaterial.SetTexture("_BumpSpecMap", array2[1]);
			this.optimizedCutoutMaterial.SetTexture("_TranslucencyMap", array2[2]);
			this.optimizedCutoutMaterial.SetTexture("_ShadowTex", array2[3]);
			this.UpdateShadowTexture(array2[3], atlas.atlasWidth, atlas.atlasHeight);
			return true;
		}
		private void DrawTexture(Rect rect, Texture rgbTexture, Texture alphaTexture, Material material, Color color, int pass)
		{
			material.SetColor("_Color", color);
			material.SetTexture("_RGBSource", rgbTexture);
			material.SetTexture("_AlphaSource", alphaTexture);
			material.SetPass(pass);
			RenderTexture active = RenderTexture.active;
			Vector2 vector = Vector2.Scale(active.GetTexelOffset(), new Vector2((float)active.width, (float)active.height)) * -1f;
			rect.x += vector.x;
			rect.y += vector.y;
			GL.Begin(7);
			GL.TexCoord(new Vector3(0f, 0f, 0f));
			GL.Vertex3(rect.x, rect.y, 0f);
			GL.TexCoord(new Vector3(1f, 0f, 0f));
			GL.Vertex3(rect.x + rect.width, rect.y, 0f);
			GL.TexCoord(new Vector3(1f, 1f, 0f));
			GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0f);
			GL.TexCoord(new Vector3(0f, 1f, 0f));
			GL.Vertex3(rect.x, rect.y + rect.height, 0f);
			GL.End();
		}
		public void UpdateMesh(Matrix4x4 matrix, List<TreeMaterial> materials, List<TreeVertex> verts, List<TreeTriangle> tris, List<TreeAOSphere> aoSpheres, int buildFlags, float adaptiveQuality, float aoDensity)
		{
			int seed = UnityEngine.Random.seed;
			RingLoop.SetNoiseSeed(this.root.seed);
			this.ClearReferences();
			this.ValidateReferences();
			this.root.UpdateSeed();
			this.root.SetRootMatrix(matrix);
			this.root.UpdateDistribution(false, true);
			this.root.UpdateParameters();
			if ((buildFlags & 1) != 0)
			{
				this.root.BuildAOSpheres(aoSpheres);
			}
			this.root.UpdateMesh(materials, verts, tris, aoSpheres, buildFlags, this.root.adaptiveLODQuality, this.root.aoDensity);
			this.ClearReferences();
			UnityEngine.Random.seed = seed;
		}
		private int[] ArrayAdd(int[] array, int value)
		{
			return new List<int>(array)
			{
				value
			}.ToArray();
		}
		private TreeGroup[] ArrayAdd(TreeGroup[] array, TreeGroup value)
		{
			return new List<TreeGroup>(array)
			{
				value
			}.ToArray();
		}
		private TreeGroupBranch[] ArrayAdd(TreeGroupBranch[] array, TreeGroupBranch value)
		{
			return new List<TreeGroupBranch>(array)
			{
				value
			}.ToArray();
		}
		private TreeGroupLeaf[] ArrayAdd(TreeGroupLeaf[] array, TreeGroupLeaf value)
		{
			return new List<TreeGroupLeaf>(array)
			{
				value
			}.ToArray();
		}
		private TreeNode[] ArrayAdd(TreeNode[] array, TreeNode value)
		{
			return new List<TreeNode>(array)
			{
				value
			}.ToArray();
		}
		private int[] ArrayRemove(int[] array, int value)
		{
			List<int> list = new List<int>(array);
			list.Remove(value);
			return list.ToArray();
		}
		private TreeGroup[] ArrayRemove(TreeGroup[] array, TreeGroup value)
		{
			List<TreeGroup> list = new List<TreeGroup>(array);
			list.Remove(value);
			return list.ToArray();
		}
		private TreeGroupBranch[] ArrayRemove(TreeGroupBranch[] array, TreeGroupBranch value)
		{
			List<TreeGroupBranch> list = new List<TreeGroupBranch>(array);
			list.Remove(value);
			return list.ToArray();
		}
		private TreeGroupLeaf[] ArrayRemove(TreeGroupLeaf[] array, TreeGroupLeaf value)
		{
			List<TreeGroupLeaf> list = new List<TreeGroupLeaf>(array);
			list.Remove(value);
			return list.ToArray();
		}
		private TreeNode[] ArrayRemove(TreeNode[] array, TreeNode value)
		{
			List<TreeNode> list = new List<TreeNode>(array);
			list.Remove(value);
			return list.ToArray();
		}
	}
}
