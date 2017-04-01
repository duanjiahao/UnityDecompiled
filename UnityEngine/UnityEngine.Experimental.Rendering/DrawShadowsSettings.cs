using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	[UsedByNativeCode]
	public struct DrawShadowsSettings
	{
		private IntPtr _cullResults;

		public int lightIndex;

		public ShadowSplitData splitData;

		public CullResults cullResults
		{
			set
			{
				this._cullResults = value.cullResults;
			}
		}

		public DrawShadowsSettings(CullResults cullResults, int lightIndex)
		{
			this._cullResults = cullResults.cullResults;
			this.lightIndex = lightIndex;
			this.splitData.cullingPlaneCount = 0;
			this.splitData.cullingSphere = Vector4.zero;
		}
	}
}
