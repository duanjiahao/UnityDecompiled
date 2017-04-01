using System;

namespace UnityEngine
{
	[Obsolete("This component is part of the legacy particle system, which is deprecated and will be removed in a future release. Use the ParticleSystem component instead.", false), RequireComponent(typeof(Transform))]
	public sealed class EllipsoidParticleEmitter : ParticleEmitter
	{
		internal EllipsoidParticleEmitter()
		{
		}
	}
}
