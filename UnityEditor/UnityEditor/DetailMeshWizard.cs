using System;
using UnityEngine;

namespace UnityEditor
{
	internal class DetailMeshWizard : TerrainWizard
	{
		public GameObject m_Detail;

		public float m_NoiseSpread;

		public float m_MinWidth;

		public float m_MaxWidth;

		public float m_MinHeight;

		public float m_MaxHeight;

		public Color m_HealthyColor;

		public Color m_DryColor;

		public DetailMeshRenderMode m_RenderMode;

		private int m_PrototypeIndex = -1;

		public void OnEnable()
		{
			base.minSize = new Vector2(400f, 400f);
		}

		internal void InitializeDefaults(Terrain terrain, int index)
		{
			this.m_Terrain = terrain;
			this.m_PrototypeIndex = index;
			DetailPrototype detailPrototype;
			if (this.m_PrototypeIndex == -1)
			{
				detailPrototype = new DetailPrototype();
			}
			else
			{
				detailPrototype = this.m_Terrain.terrainData.detailPrototypes[this.m_PrototypeIndex];
			}
			this.m_Detail = detailPrototype.prototype;
			this.m_NoiseSpread = detailPrototype.noiseSpread;
			this.m_MinWidth = detailPrototype.minWidth;
			this.m_MaxWidth = detailPrototype.maxWidth;
			this.m_MinHeight = detailPrototype.minHeight;
			this.m_MaxHeight = detailPrototype.maxHeight;
			this.m_HealthyColor = detailPrototype.healthyColor;
			this.m_DryColor = detailPrototype.dryColor;
			switch (detailPrototype.renderMode)
			{
			case DetailRenderMode.GrassBillboard:
				Debug.LogError("Detail meshes can't be rendered as billboards");
				this.m_RenderMode = DetailMeshRenderMode.Grass;
				break;
			case DetailRenderMode.VertexLit:
				this.m_RenderMode = DetailMeshRenderMode.VertexLit;
				break;
			case DetailRenderMode.Grass:
				this.m_RenderMode = DetailMeshRenderMode.Grass;
				break;
			}
			this.OnWizardUpdate();
		}

		private void DoApply()
		{
			if (base.terrainData == null)
			{
				return;
			}
			DetailPrototype[] array = this.m_Terrain.terrainData.detailPrototypes;
			if (this.m_PrototypeIndex == -1)
			{
				DetailPrototype[] array2 = new DetailPrototype[array.Length + 1];
				Array.Copy(array, 0, array2, 0, array.Length);
				this.m_PrototypeIndex = array.Length;
				array = array2;
				array[this.m_PrototypeIndex] = new DetailPrototype();
			}
			array[this.m_PrototypeIndex].renderMode = DetailRenderMode.VertexLit;
			array[this.m_PrototypeIndex].usePrototypeMesh = true;
			array[this.m_PrototypeIndex].prototype = this.m_Detail;
			array[this.m_PrototypeIndex].prototypeTexture = null;
			array[this.m_PrototypeIndex].noiseSpread = this.m_NoiseSpread;
			array[this.m_PrototypeIndex].minWidth = this.m_MinWidth;
			array[this.m_PrototypeIndex].maxWidth = this.m_MaxWidth;
			array[this.m_PrototypeIndex].minHeight = this.m_MinHeight;
			array[this.m_PrototypeIndex].maxHeight = this.m_MaxHeight;
			array[this.m_PrototypeIndex].healthyColor = this.m_HealthyColor;
			array[this.m_PrototypeIndex].dryColor = this.m_DryColor;
			if (this.m_RenderMode == DetailMeshRenderMode.Grass)
			{
				array[this.m_PrototypeIndex].renderMode = DetailRenderMode.Grass;
			}
			else
			{
				array[this.m_PrototypeIndex].renderMode = DetailRenderMode.VertexLit;
			}
			this.m_Terrain.terrainData.detailPrototypes = array;
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
			if (this.m_Detail == null)
			{
				base.errorString = "Please assign a detail prefab";
				base.isValid = false;
			}
			else if (this.m_PrototypeIndex != -1)
			{
				this.DoApply();
			}
		}
	}
}
