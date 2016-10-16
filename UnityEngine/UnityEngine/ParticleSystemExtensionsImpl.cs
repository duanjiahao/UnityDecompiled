using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal sealed class ParticleSystemExtensionsImpl
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSafeCollisionEventSize(ParticleSystem ps);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCollisionEventsDeprecated(ParticleSystem ps, GameObject go, ParticleCollisionEvent[] collisionEvents);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetSafeTriggerParticlesSize(ParticleSystem ps, int type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCollisionEvents(ParticleSystem ps, GameObject go, object collisionEvents);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetTriggerParticles(ParticleSystem ps, int type, object particles);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetTriggerParticles(ParticleSystem ps, int type, object particles, int offset, int count);
	}
}
