using System;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Input
{
	[RequiredByNativeCode]
	public struct InteractionSourceProperties
	{
		internal double m_sourceLossRisk;

		internal Vector3 m_sourceLossMitigationDirection;

		internal InteractionSourceLocation m_location;

		public double sourceLossRisk
		{
			get
			{
				return this.m_sourceLossRisk;
			}
		}

		public Vector3 sourceLossMitigationDirection
		{
			get
			{
				return this.m_sourceLossMitigationDirection;
			}
		}

		public InteractionSourceLocation location
		{
			get
			{
				return this.m_location;
			}
		}
	}
}
