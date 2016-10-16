using System;

namespace UnityEngine
{
	[Obsolete("This is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false)]
	public enum ParticleRenderMode
	{
		Billboard,
		Stretch = 3,
		SortedBillboard = 2,
		HorizontalBillboard = 4,
		VerticalBillboard
	}
}
