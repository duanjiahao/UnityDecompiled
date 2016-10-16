using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct GradientColorKey
	{
		public Color color;

		public float time;

		public GradientColorKey(Color col, float time)
		{
			this.color = col;
			this.time = time;
		}
	}
}
