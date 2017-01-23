using System;

namespace UnityEngine
{
	[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.")]
	public enum ParticleSystemEmissionType
	{
		Time,
		Distance
	}
}
