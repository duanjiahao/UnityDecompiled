using System;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Input
{
	[RequiredByNativeCode]
	public struct InteractionSourceLocation
	{
		internal byte m_hasPosition;

		internal Vector3 m_position;

		internal byte m_hasVelocity;

		internal Vector3 m_velocity;

		public bool TryGetPosition(out Vector3 position)
		{
			position = this.m_position;
			return this.m_hasPosition != 0;
		}

		public bool TryGetVelocity(out Vector3 velocity)
		{
			velocity = this.m_velocity;
			return this.m_hasVelocity != 0;
		}
	}
}
