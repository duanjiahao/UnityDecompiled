using System;
namespace UnityEngine
{
	public struct GradientAlphaKey
	{
		public float alpha;
		public float time;
		public GradientAlphaKey(float alpha, float time)
		{
			this.alpha = alpha;
			this.time = time;
		}
	}
}
