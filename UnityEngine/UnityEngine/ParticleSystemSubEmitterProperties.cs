using System;

namespace UnityEngine
{
	[Flags]
	public enum ParticleSystemSubEmitterProperties
	{
		InheritNothing = 0,
		InheritEverything = 7,
		InheritColor = 1,
		InheritSize = 2,
		InheritRotation = 4
	}
}
