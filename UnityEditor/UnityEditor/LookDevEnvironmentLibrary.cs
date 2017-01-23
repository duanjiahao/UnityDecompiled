using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class LookDevEnvironmentLibrary : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<CubemapInfo> m_HDRIList = new List<CubemapInfo>();

		[SerializeField]
		private List<CubemapInfo> m_SerialShadowMapHDRIList = new List<CubemapInfo>();

		private LookDevView m_LookDevView = null;

		private bool m_Dirty = false;

		public bool dirty
		{
			get
			{
				return this.m_Dirty;
			}
			set
			{
				this.m_Dirty = value;
			}
		}

		public List<CubemapInfo> hdriList
		{
			get
			{
				return this.m_HDRIList;
			}
		}

		public int hdriCount
		{
			get
			{
				return this.hdriList.Count;
			}
		}

		public void InsertHDRI(Cubemap cubemap)
		{
			this.InsertHDRI(cubemap, -1);
		}

		public void InsertHDRI(Cubemap cubemap, int insertionIndex)
		{
			Undo.RecordObject(this.m_LookDevView.envLibrary, "Insert HDRI");
			Undo.RecordObject(this.m_LookDevView.config, "Insert HDRI");
			Cubemap cubemap0 = null;
			Cubemap cubemap1 = null;
			if (cubemap == LookDevResources.m_DefaultHDRI)
			{
				cubemap0 = LookDevResources.m_DefaultHDRI;
				cubemap1 = LookDevResources.m_DefaultHDRI;
			}
			else
			{
				cubemap0 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[0].currentHDRIIndex].cubemap;
				cubemap1 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[1].currentHDRIIndex].cubemap;
			}
			int num = this.m_HDRIList.FindIndex((CubemapInfo x) => x.cubemap == cubemap);
			if (num == -1)
			{
				this.m_Dirty = true;
				CubemapInfo cubemapInfo = null;
				for (int i = 0; i < this.m_HDRIList.Count; i++)
				{
					if (this.m_HDRIList[i].cubemapShadowInfo.cubemap == cubemap)
					{
						cubemapInfo = this.m_HDRIList[i].cubemapShadowInfo;
						cubemapInfo.SetCubemapShadowInfo(cubemapInfo);
						break;
					}
				}
				if (cubemapInfo == null)
				{
					cubemapInfo = new CubemapInfo();
					cubemapInfo.cubemap = cubemap;
					cubemapInfo.ambientProbe.Clear();
					cubemapInfo.alreadyComputed = false;
					cubemapInfo.SetCubemapShadowInfo(cubemapInfo);
				}
				int count = this.m_HDRIList.Count;
				this.m_HDRIList.Insert((insertionIndex != -1) ? insertionIndex : count, cubemapInfo);
				if (cubemapInfo.cubemap != LookDevResources.m_DefaultHDRI)
				{
					LookDevResources.UpdateShadowInfoWithBrightestSpot(cubemapInfo);
				}
			}
			if (num != insertionIndex && num != -1 && insertionIndex != -1)
			{
				CubemapInfo item = this.m_HDRIList[num];
				this.m_HDRIList.RemoveAt(num);
				this.m_HDRIList.Insert((num <= insertionIndex) ? (insertionIndex - 1) : insertionIndex, item);
			}
			this.m_LookDevView.config.lookDevContexts[0].UpdateProperty(LookDevProperty.HDRI, this.m_HDRIList.FindIndex((CubemapInfo x) => x.cubemap == cubemap0));
			this.m_LookDevView.config.lookDevContexts[1].UpdateProperty(LookDevProperty.HDRI, this.m_HDRIList.FindIndex((CubemapInfo x) => x.cubemap == cubemap1));
			this.m_LookDevView.Repaint();
		}

		public bool RemoveHDRI(Cubemap cubemap)
		{
			if (cubemap != null)
			{
				Undo.RecordObject(this.m_LookDevView.envLibrary, "Remove HDRI");
				Undo.RecordObject(this.m_LookDevView.config, "Remove HDRI");
			}
			bool result;
			if (cubemap == LookDevResources.m_DefaultHDRI)
			{
				Debug.LogWarning("Cannot remove default HDRI from the library");
				result = false;
			}
			else
			{
				int num = this.m_HDRIList.FindIndex((CubemapInfo x) => x.cubemap == cubemap);
				if (num != -1)
				{
					Cubemap cubemap0 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[0].currentHDRIIndex].cubemap;
					Cubemap cubemap1 = this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[1].currentHDRIIndex].cubemap;
					this.m_HDRIList.RemoveAt(num);
					int num2 = (this.m_HDRIList.Count != 0) ? 0 : -1;
					this.m_LookDevView.config.lookDevContexts[0].UpdateProperty(LookDevProperty.HDRI, (!(cubemap0 == cubemap)) ? this.m_HDRIList.FindIndex((CubemapInfo x) => x.cubemap == cubemap0) : num2);
					this.m_LookDevView.config.lookDevContexts[1].UpdateProperty(LookDevProperty.HDRI, (!(cubemap1 == cubemap)) ? this.m_HDRIList.FindIndex((CubemapInfo x) => x.cubemap == cubemap1) : num2);
					this.m_LookDevView.Repaint();
					this.m_Dirty = true;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void CleanupDeletedHDRI()
		{
			while (this.RemoveHDRI(null))
			{
			}
		}

		private ShadowInfo GetCurrentShadowInfo()
		{
			return this.m_HDRIList[this.m_LookDevView.config.lookDevContexts[(int)this.m_LookDevView.config.currentEditionContext].currentHDRIIndex].shadowInfo;
		}

		public void SetLookDevView(LookDevView lookDevView)
		{
			this.m_LookDevView = lookDevView;
		}

		public void OnBeforeSerialize()
		{
			this.m_SerialShadowMapHDRIList.Clear();
			for (int i = 0; i < this.m_HDRIList.Count; i++)
			{
				CubemapInfo shadowCubemapInfo = this.m_HDRIList[i].cubemapShadowInfo;
				this.m_HDRIList[i].serialIndexMain = this.m_HDRIList.FindIndex((CubemapInfo x) => x == shadowCubemapInfo);
				if (this.m_HDRIList[i].serialIndexMain == -1)
				{
					this.m_HDRIList[i].serialIndexShadow = this.m_SerialShadowMapHDRIList.FindIndex((CubemapInfo x) => x == shadowCubemapInfo);
					if (this.m_HDRIList[i].serialIndexShadow == -1)
					{
						this.m_SerialShadowMapHDRIList.Add(shadowCubemapInfo);
						this.m_HDRIList[i].serialIndexShadow = this.m_SerialShadowMapHDRIList.Count - 1;
					}
				}
			}
		}

		public void OnAfterDeserialize()
		{
			for (int i = 0; i < this.m_HDRIList.Count; i++)
			{
				if (this.m_HDRIList[i].serialIndexMain != -1)
				{
					this.m_HDRIList[i].cubemapShadowInfo = this.m_HDRIList[this.hdriList[i].serialIndexMain];
				}
				else
				{
					this.m_HDRIList[i].cubemapShadowInfo = this.m_SerialShadowMapHDRIList[this.m_HDRIList[i].serialIndexShadow];
				}
			}
		}
	}
}
