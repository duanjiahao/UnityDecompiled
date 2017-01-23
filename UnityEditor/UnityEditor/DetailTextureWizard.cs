using System;
using UnityEngine;

namespace UnityEditor
{
	internal class DetailTextureWizard : TerrainWizard
	{
		public Texture2D m_DetailTexture;

		public float m_MinWidth;

		public float m_MaxWidth;

		public float m_MinHeight;

		public float m_MaxHeight;

		public float m_NoiseSpread;

		public Color m_HealthyColor;

		public Color m_DryColor;

		public bool m_Billboard;

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
				detailPrototype.renderMode = DetailRenderMode.GrassBillboard;
			}
			else
			{
				detailPrototype = this.m_Terrain.terrainData.detailPrototypes[this.m_PrototypeIndex];
			}
			this.m_DetailTexture = detailPrototype.prototypeTexture;
			this.m_MinWidth = detailPrototype.minWidth;
			this.m_MaxWidth = detailPrototype.maxWidth;
			this.m_MinHeight = detailPrototype.minHeight;
			this.m_MaxHeight = detailPrototype.maxHeight;
			this.m_NoiseSpread = detailPrototype.noiseSpread;
			this.m_HealthyColor = detailPrototype.healthyColor;
			this.m_DryColor = detailPrototype.dryColor;
			this.m_Billboard = (detailPrototype.renderMode == DetailRenderMode.GrassBillboard);
			this.OnWizardUpdate();
		}

		private void DoApply()
		{
			if (!(base.terrainData == null))
			{
				DetailPrototype[] array = this.m_Terrain.terrainData.detailPrototypes;
				if (this.m_PrototypeIndex == -1)
				{
					DetailPrototype[] array2 = new DetailPrototype[array.Length + 1];
					Array.Copy(array, 0, array2, 0, array.Length);
					this.m_PrototypeIndex = array.Length;
					array = array2;
					array[this.m_PrototypeIndex] = new DetailPrototype();
				}
				array[this.m_PrototypeIndex].prototype = null;
				array[this.m_PrototypeIndex].prototypeTexture = this.m_DetailTexture;
				array[this.m_PrototypeIndex].minWidth = this.m_MinWidth;
				array[this.m_PrototypeIndex].maxWidth = this.m_MaxWidth;
				array[this.m_PrototypeIndex].minHeight = this.m_MinHeight;
				array[this.m_PrototypeIndex].maxHeight = this.m_MaxHeight;
				array[this.m_PrototypeIndex].noiseSpread = this.m_NoiseSpread;
				array[this.m_PrototypeIndex].healthyColor = this.m_HealthyColor;
				array[this.m_PrototypeIndex].dryColor = this.m_DryColor;
				array[this.m_PrototypeIndex].renderMode = ((!this.m_Billboard) ? DetailRenderMode.Grass : DetailRenderMode.GrassBillboard);
				array[this.m_PrototypeIndex].usePrototypeMesh = false;
				this.m_Terrain.terrainData.detailPrototypes = array;
				EditorUtility.SetDirty(this.m_Terrain);
			}
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
			this.m_MinHeight = Mathf.Max(0f, this.m_MinHeight);
			this.m_MaxHeight = Mathf.Max(this.m_MinHeight, this.m_MaxHeight);
			this.m_MinWidth = Mathf.Max(0f, this.m_MinWidth);
			this.m_MaxWidth = Mathf.Max(this.m_MinWidth, this.m_MaxWidth);
			base.OnWizardUpdate();
			if (this.m_DetailTexture == null)
			{
				base.errorString = "Please assign a detail texture";
				base.isValid = false;
			}
			else if (this.m_PrototypeIndex != -1)
			{
				this.DoApply();
			}
		}
	}
}
