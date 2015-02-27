using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
namespace TreeEditor
{
	public class TreeEditorHelper
	{
		public enum NodeType
		{
			BarkNode,
			LeafNode
		}
		private const string kDefaultBarkShaderName = "Nature/Tree Creator Bark";
		private const string kDefaultLeafShaderName = "Nature/Tree Creator Leaves Fast";
		private const string kDefaultOptimizedBarkShaderName = "Hidden/Nature/Tree Creator Bark Optimized";
		private const string kDefaultOptimizedLeafShaderName = "Hidden/Nature/Tree Creator Leaves Optimized";
		private const string kOptimizedShaderDependency = "OptimizedShader";
		private readonly Dictionary<string, AnimBool> m_AnimBools = new Dictionary<string, AnimBool>();
		private readonly List<string> m_BarkShaders = new List<string>();
		private readonly List<string> m_LeafShaders = new List<string>();
		private readonly HashSet<string> m_WrongShaders = new HashSet<string>();
		private readonly Dictionary<string, int> m_SelectedShader = new Dictionary<string, int>();
		private TreeData m_TreeData;
		private static string s_UIText;
		private static readonly Dictionary<string, GUIContent> s_Dictionary = new Dictionary<string, GUIContent>();
		internal static Shader DefaultOptimizedBarkShader
		{
			get
			{
				return Shader.Find("Hidden/Nature/Tree Creator Bark Optimized");
			}
		}
		internal static Shader DefaultOptimizedLeafShader
		{
			get
			{
				return Shader.Find("Hidden/Nature/Tree Creator Leaves Optimized");
			}
		}
		private static string uiText
		{
			get
			{
				if (TreeEditorHelper.s_UIText == null)
				{
					TreeEditorHelper.s_UIText = File.ReadAllText(EditorApplication.applicationContentsPath + "/Resources/TreeEditor_UI_Strings_EN.txt");
				}
				if (TreeEditorHelper.s_UIText == null)
				{
					Debug.LogError("Missing asset");
				}
				return TreeEditorHelper.s_UIText;
			}
		}
		internal void SetAnimsCallback(UnityAction callback)
		{
			foreach (KeyValuePair<string, AnimBool> current in this.m_AnimBools)
			{
				current.Value.valueChanged.AddListener(callback);
			}
		}
		public void OnEnable(TreeData treeData)
		{
			this.m_TreeData = treeData;
		}
		public bool AreShadersCorrect()
		{
			bool flag = this.m_BarkShaders.Count + this.m_LeafShaders.Count > 2;
			return this.m_WrongShaders.Count == 0 && !flag;
		}
		public static string GetOptimizedShaderName(Shader shader)
		{
			if (shader)
			{
				return ShaderUtil.GetDependency(shader, "OptimizedShader");
			}
			return null;
		}
		private static bool IsTreeShader(Shader shader)
		{
			return TreeEditorHelper.IsTreeBarkShader(shader) || TreeEditorHelper.IsTreeLeafShader(shader);
		}
		public static bool IsTreeLeafShader(Shader shader)
		{
			return TreeEditorHelper.HasOptimizedShaderAndNameContains(shader, "leaves");
		}
		public static bool IsTreeBarkShader(Shader shader)
		{
			return TreeEditorHelper.HasOptimizedShaderAndNameContains(shader, "bark");
		}
		public bool GUITooManyShaders()
		{
			bool flag = this.GUITooManyShaders(TreeEditorHelper.NodeType.BarkNode);
			flag |= this.GUITooManyShaders(TreeEditorHelper.NodeType.LeafNode);
			if (flag)
			{
				this.RefreshAllTreeShaders();
			}
			return flag;
		}
		private bool GUITooManyShaders(TreeEditorHelper.NodeType nodeType)
		{
			string text = nodeType.ToString();
			if (this.CheckForTooManyShaders(nodeType))
			{
				this.SetAnimBool(text, true, true);
			}
			List<string> shadersListForNodeType = this.GetShadersListForNodeType(nodeType);
			GUIContent gUIContent = TreeEditorHelper.GetGUIContent((nodeType != TreeEditorHelper.NodeType.BarkNode) ? "TreeEditor.TreeGroup.TooManyLeafShaders" : "TreeEditor.TreeGroup.TooManyBarkShaders");
			GUIContent gUIContent2 = TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroup.ChangeAllShadersOnNodesButton");
			int num = this.GUIShowError(text, shadersListForNodeType, gUIContent, gUIContent2, ConsoleWindow.iconError);
			if (num >= 0)
			{
				Shader shader = Shader.Find(shadersListForNodeType[num]);
				TreeEditorHelper.ChangeShaderOnMaterials(this.m_TreeData, shader, this.m_TreeData.root, nodeType);
				this.DisableAnimBool(text);
				this.RemoveSelectedIndex(text);
				return true;
			}
			return false;
		}
		public bool GUIWrongShader(string uniqueID, Material value, TreeEditorHelper.NodeType nodeType)
		{
			GUIContent gUIContent = TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroup.NotATreeShader");
			GUIContent gUIContent2 = TreeEditorHelper.GetGUIContent("TreeEditor.TreeGroup.ChangeShaderButton");
			if (TreeEditorHelper.IsMaterialCorrect(value))
			{
				return false;
			}
			List<string> recommendedShaders = this.GetRecommendedShaders(nodeType);
			this.m_WrongShaders.Add(uniqueID);
			this.SetAnimBool(uniqueID, true, true);
			int num = this.GUIShowError(uniqueID, recommendedShaders, gUIContent, gUIContent2, ConsoleWindow.iconError);
			if (num >= 0)
			{
				value.shader = Shader.Find(recommendedShaders[num]);
				this.m_WrongShaders.Remove(uniqueID);
				this.DisableAnimBool(uniqueID);
				this.RemoveSelectedIndex(uniqueID);
				return true;
			}
			return false;
		}
		private int GUIShowError(string uniqueID, List<string> list, GUIContent message, GUIContent button, Texture2D icon)
		{
			int result = -1;
			if (this.m_AnimBools.ContainsKey(uniqueID))
			{
				if (EditorGUILayout.BeginFadeGroup(this.m_AnimBools[uniqueID].faded))
				{
					GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
					message.image = icon;
					GUILayout.Label(message, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					int num = EditorGUILayout.Popup(this.GetSelectedIndex(uniqueID), list.ToArray(), new GUILayoutOption[0]);
					this.SetSelectedIndex(uniqueID, num);
					if (GUILayout.Button(button, EditorStyles.miniButton, new GUILayoutOption[0]))
					{
						result = num;
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
				EditorGUILayout.EndFadeGroup();
			}
			return result;
		}
		public void RefreshAllTreeShaders()
		{
			this.m_BarkShaders.Clear();
			this.m_LeafShaders.Clear();
			TreeEditorHelper.GetAllTreeShaders(this.m_TreeData, this.m_BarkShaders, this.m_LeafShaders, this.m_TreeData.root);
		}
		private static void GetAllTreeShaders(TreeData treeData, List<string> barkShaders, List<string> leafShaders, TreeGroup group)
		{
			if (group is TreeGroupBranch)
			{
				TreeGroupBranch treeGroupBranch = group as TreeGroupBranch;
				TreeEditorHelper.AddShaderFromMaterial(treeGroupBranch.materialBranch, barkShaders, leafShaders);
				TreeEditorHelper.AddShaderFromMaterial(treeGroupBranch.materialBreak, barkShaders, leafShaders);
				TreeEditorHelper.AddShaderFromMaterial(treeGroupBranch.materialFrond, barkShaders, leafShaders);
			}
			else
			{
				if (group is TreeGroupLeaf)
				{
					TreeGroupLeaf treeGroupLeaf = group as TreeGroupLeaf;
					TreeEditorHelper.AddShaderFromMaterial(treeGroupLeaf.materialLeaf, barkShaders, leafShaders);
				}
			}
			int[] childGroupIDs = group.childGroupIDs;
			for (int i = 0; i < childGroupIDs.Length; i++)
			{
				int id = childGroupIDs[i];
				TreeGroup group2 = treeData.GetGroup(id);
				TreeEditorHelper.GetAllTreeShaders(treeData, barkShaders, leafShaders, group2);
			}
		}
		public bool NodeHasWrongMaterial(TreeGroup group)
		{
			bool flag = false;
			if (group is TreeGroupBranch)
			{
				TreeGroupBranch treeGroupBranch = group as TreeGroupBranch;
				flag |= !TreeEditorHelper.IsMaterialCorrect(treeGroupBranch.materialBranch);
				flag |= !TreeEditorHelper.IsMaterialCorrect(treeGroupBranch.materialBreak);
				flag |= !TreeEditorHelper.IsMaterialCorrect(treeGroupBranch.materialFrond);
			}
			else
			{
				if (group is TreeGroupLeaf)
				{
					TreeGroupLeaf treeGroupLeaf = group as TreeGroupLeaf;
					flag |= !TreeEditorHelper.IsMaterialCorrect(treeGroupLeaf.materialLeaf);
				}
			}
			return flag;
		}
		private static bool IsMaterialCorrect(Material material)
		{
			return !material || TreeEditorHelper.IsTreeShader(material.shader);
		}
		private List<string> GetShadersListForNodeType(TreeEditorHelper.NodeType nodeType)
		{
			if (nodeType == TreeEditorHelper.NodeType.BarkNode)
			{
				return this.m_BarkShaders;
			}
			return this.m_LeafShaders;
		}
		private List<string> GetShadersListOppositeToNodeType(TreeEditorHelper.NodeType nodeType)
		{
			if (nodeType == TreeEditorHelper.NodeType.BarkNode)
			{
				return this.GetShadersListForNodeType(TreeEditorHelper.NodeType.LeafNode);
			}
			return this.GetShadersListForNodeType(TreeEditorHelper.NodeType.BarkNode);
		}
		private static string GetDefaultShader(TreeEditorHelper.NodeType nodeType)
		{
			if (nodeType == TreeEditorHelper.NodeType.BarkNode)
			{
				return "Nature/Tree Creator Bark";
			}
			return "Nature/Tree Creator Leaves Fast";
		}
		private List<string> GetRecommendedShaders(TreeEditorHelper.NodeType nodeType)
		{
			List<string> list = new List<string>(3);
			List<string> shadersListForNodeType = this.GetShadersListForNodeType(nodeType);
			List<string> shadersListOppositeToNodeType = this.GetShadersListOppositeToNodeType(nodeType);
			if (shadersListForNodeType.Count == 1 || (shadersListForNodeType.Count == 2 && shadersListOppositeToNodeType.Count == 0))
			{
				foreach (string current in shadersListForNodeType)
				{
					list.Add(current);
				}
			}
			if (shadersListForNodeType.Count == 0)
			{
				list.Add(TreeEditorHelper.GetDefaultShader(nodeType));
			}
			return list;
		}
		private bool CheckForTooManyShaders(TreeEditorHelper.NodeType nodeType)
		{
			List<string> shadersListForNodeType = this.GetShadersListForNodeType(nodeType);
			List<string> shadersListOppositeToNodeType = this.GetShadersListOppositeToNodeType(nodeType);
			return shadersListForNodeType.Count > 2 || (shadersListForNodeType.Count == 2 && shadersListOppositeToNodeType.Count > 0);
		}
		private static bool HasOptimizedShaderAndNameContains(Shader shader, string name)
		{
			return TreeEditorHelper.GetOptimizedShaderName(shader) != null && shader.name.ToLower().Contains(name);
		}
		private static void AddShaderFromMaterial(Material material, List<string> barkShaders, List<string> leafShaders)
		{
			if (material && material.shader)
			{
				Shader shader = material.shader;
				if (TreeEditorHelper.IsTreeBarkShader(shader) && !barkShaders.Contains(shader.name))
				{
					barkShaders.Add(shader.name);
				}
				else
				{
					if (TreeEditorHelper.IsTreeLeafShader(material.shader) && !leafShaders.Contains(shader.name))
					{
						leafShaders.Add(shader.name);
					}
				}
			}
		}
		private static void ChangeShaderOnMaterial(Material material, Shader shader)
		{
			if (material && shader)
			{
				material.shader = shader;
			}
		}
		private static void ChangeShaderOnMaterials(TreeData treeData, Shader shader, TreeGroup group, TreeEditorHelper.NodeType nodeType)
		{
			if (group is TreeGroupBranch && nodeType == TreeEditorHelper.NodeType.BarkNode)
			{
				TreeGroupBranch treeGroupBranch = group as TreeGroupBranch;
				TreeEditorHelper.ChangeShaderOnMaterial(treeGroupBranch.materialBranch, shader);
				TreeEditorHelper.ChangeShaderOnMaterial(treeGroupBranch.materialBreak, shader);
				TreeEditorHelper.ChangeShaderOnMaterial(treeGroupBranch.materialFrond, shader);
			}
			else
			{
				if (group is TreeGroupLeaf && nodeType == TreeEditorHelper.NodeType.LeafNode)
				{
					TreeGroupLeaf treeGroupLeaf = group as TreeGroupLeaf;
					TreeEditorHelper.ChangeShaderOnMaterial(treeGroupLeaf.materialLeaf, shader);
				}
			}
			int[] childGroupIDs = group.childGroupIDs;
			for (int i = 0; i < childGroupIDs.Length; i++)
			{
				int id = childGroupIDs[i];
				TreeGroup group2 = treeData.GetGroup(id);
				TreeEditorHelper.ChangeShaderOnMaterials(treeData, shader, group2, nodeType);
			}
		}
		private void RemoveSelectedIndex(string contentID)
		{
			this.m_SelectedShader.Remove(contentID);
		}
		private void SetSelectedIndex(string contentID, int value)
		{
			if (this.m_SelectedShader.ContainsKey(contentID))
			{
				this.m_SelectedShader[contentID] = value;
			}
			else
			{
				this.m_SelectedShader.Add(contentID, value);
			}
		}
		private int GetSelectedIndex(string contentID)
		{
			if (!this.m_SelectedShader.ContainsKey(contentID))
			{
				this.m_SelectedShader.Add(contentID, 0);
			}
			return this.m_SelectedShader[contentID];
		}
		private void SetAnimBool(string contentID, bool target, bool value)
		{
			this.SetAnimBool(contentID, target);
			this.m_AnimBools[contentID].value = value;
		}
		private void SetAnimBool(string contentID, bool target)
		{
			AnimBool animBool;
			if (!this.m_AnimBools.ContainsKey(contentID))
			{
				animBool = new AnimBool();
				this.m_AnimBools.Add(contentID, animBool);
			}
			else
			{
				animBool = this.m_AnimBools[contentID];
			}
			animBool.target = target;
		}
		private void DisableAnimBool(string contentID)
		{
			if (this.m_AnimBools.ContainsKey(contentID))
			{
				this.m_AnimBools[contentID].target = false;
			}
		}
		public static GUIContent GetGUIContent(string id)
		{
			if (TreeEditorHelper.s_Dictionary.ContainsKey(id))
			{
				return TreeEditorHelper.s_Dictionary[id];
			}
			string uIString = TreeEditorHelper.GetUIString(id);
			if (uIString == null)
			{
				return new GUIContent(id, string.Empty);
			}
			GUIContent gUIContent = new GUIContent(TreeEditorHelper.ExtractLabel(uIString), TreeEditorHelper.ExtractTooltip(uIString));
			TreeEditorHelper.s_Dictionary.Add(id, gUIContent);
			return gUIContent;
		}
		public static string GetUIString(string id)
		{
			string text = id + ":";
			int num = TreeEditorHelper.uiText.IndexOf(text);
			if (num < 0)
			{
				Debug.LogError("Missing definition for " + id + "!");
				return null;
			}
			int num2 = TreeEditorHelper.uiText.IndexOf("\n", num);
			num += text.Length;
			return TreeEditorHelper.uiText.Substring(num, num2 - num);
		}
		public static string ExtractLabel(string uiString)
		{
			string[] array = uiString.Split(new char[]
			{
				'|'
			});
			return array[0].Trim();
		}
		public static string ExtractTooltip(string uiString)
		{
			string[] array = uiString.Split(new char[]
			{
				'|'
			});
			return array[1].Trim();
		}
	}
}
