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
		internal static extern int GetCollisionEvents(ParticleSystem ps, GameObject go, ParticleCollisionEvent[] collisionEvents);
	}
}
