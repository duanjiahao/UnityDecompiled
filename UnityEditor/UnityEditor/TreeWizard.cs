using System;
using UnityEngine;
namespace UnityEditor
{
	internal class TreeWizard : TerrainWizard
	{
		public GameObject m_Tree;
		public float m_BendFactor;
		private int m_PrototypeIndex = -1;
		public void OnEnable()
		{
			base.minSize = new Vector2(400f, 150f);
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
		internal override void OnWizardUpdate()
		{
			base.OnWizardUpdate();
			if (this.m_Tree == null)
			{
				base.errorString = "Please assign a tree";
				base.isValid = false;
			}
			else
			{
				if (this.m_PrototypeIndex != -1)
				{
					this.DoApply();
				}
			}
		}
	}
}
