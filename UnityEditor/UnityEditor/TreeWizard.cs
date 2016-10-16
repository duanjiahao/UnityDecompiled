using System;
using UnityEngine;

namespace UnityEditor
{
	internal class TreeWizard : TerrainWizard
	{
		public GameObject m_Tree;

		public float m_BendFactor;

		private int m_PrototypeIndex = -1;

		private bool m_IsValidTree;

		public void OnEnable()
		{
			base.minSize = new Vector2(400f, 150f);
		}

		private static bool IsValidTree(GameObject tree, int prototypeIndex, Terrain terrain)
		{
			if (tree == null)
			{
				return false;
			}
			TreePrototype[] treePrototypes = terrain.terrainData.treePrototypes;
			for (int i = 0; i < treePrototypes.Length; i++)
			{
				if (i != prototypeIndex && treePrototypes[i].m_Prefab == tree)
				{
					return false;
				}
			}
			return true;
		}

		internal void InitializeDefaults(Terrain terrain, int index)
		{
			this.m_Terrain = terrain;
			this.m_PrototypeIndex = index;
			if (this.m_PrototypeIndex == -1)
			{
				this.m_Tree = null;
				this.m_BendFactor = 0f;
			}
			else
			{
				this.m_Tree = this.m_Terrain.terrainData.treePrototypes[this.m_PrototypeIndex].prefab;
				this.m_BendFactor = this.m_Terrain.terrainData.treePrototypes[this.m_PrototypeIndex].bendFactor;
			}
			this.m_IsValidTree = TreeWizard.IsValidTree(this.m_Tree, this.m_PrototypeIndex, terrain);
			this.OnWizardUpdate();
		}

		private void DoApply()
		{
			if (base.terrainData == null)
			{
				return;
			}
			TreePrototype[] treePrototypes = this.m_Terrain.terrainData.treePrototypes;
			if (this.m_PrototypeIndex == -1)
			{
				TreePrototype[] array = new TreePrototype[treePrototypes.Length + 1];
				for (int i = 0; i < treePrototypes.Length; i++)
				{
					array[i] = treePrototypes[i];
				}
				array[treePrototypes.Length] = new TreePrototype();
				array[treePrototypes.Length].prefab = this.m_Tree;
				array[treePrototypes.Length].bendFactor = this.m_BendFactor;
				this.m_PrototypeIndex = treePrototypes.Length;
				this.m_Terrain.terrainData.treePrototypes = array;
				TreePainter.selectedTree = this.m_PrototypeIndex;
			}
			else
			{
				treePrototypes[this.m_PrototypeIndex].prefab = this.m_Tree;
				treePrototypes[this.m_PrototypeIndex].bendFactor = this.m_BendFactor;
				this.m_Terrain.terrainData.treePrototypes = treePrototypes;
			}
			this.m_Terrain.Flush();
			EditorUtility.SetDirty(this.m_Terrain);
		}

		private void OnWizardCreate()
		{
			this.DoApply();
		}

		private void OnWizardOtherButton()
		{
			this.DoApply();
		}

		protected override bool DrawWizardGUI()
		{
			EditorGUI.BeginChangeCheck();
			bool allowSceneObjects = !EditorUtility.IsPersistent(this.m_Terrain.terrainData);
			this.m_Tree = (GameObject)EditorGUILayout.ObjectField("Tree Prefab", this.m_Tree, typeof(GameObject), allowSceneObjects, new GUILayoutOption[0]);
			if (!TerrainEditorUtility.IsLODTreePrototype(this.m_Tree))
			{
				this.m_BendFactor = EditorGUILayout.FloatField("Bend Factor", this.m_BendFactor, new GUILayoutOption[0]);
			}
			bool flag = EditorGUI.EndChangeCheck();
			if (flag)
			{
				this.m_IsValidTree = TreeWizard.IsValidTree(this.m_Tree, this.m_PrototypeIndex, this.m_Terrain);
			}
			return flag;
		}

		internal override void OnWizardUpdate()
		{
			base.OnWizardUpdate();
			if (this.m_Tree == null)
			{
				base.errorString = "Please assign a tree";
				base.isValid = false;
			}
			else if (!this.m_IsValidTree)
			{
				base.errorString = "Tree has already been selected as a prototype";
				base.isValid = false;
			}
			else if (this.m_PrototypeIndex != -1)
			{
				this.DoApply();
			}
		}
	}
}
