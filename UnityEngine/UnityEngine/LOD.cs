using System;
namespace UnityEngine
{
	public struct LOD
	{
		public float screenRelativeTransitionHeight;
		public Renderer[] renderers;
		public LOD(float screenRelativeTransitionHeight, Renderer[] renderers)
		{
			this.screenRelativeTransitionHeight = screenRelativeTransitionHeight;
			this.renderers = renderers;
		}
	}
}
