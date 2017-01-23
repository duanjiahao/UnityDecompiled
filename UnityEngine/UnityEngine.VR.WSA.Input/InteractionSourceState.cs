using System;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Input
{
	[RequiredByNativeCode]
	public struct InteractionSourceState
	{
		internal byte m_pressed;

		internal InteractionSourceProperties m_properties;

		internal InteractionSource m_source;

		internal Ray m_headRay;

		public bool pressed
		{
			get
			{
				return this.m_pressed != 0;
			}
		}

		public InteractionSourceProperties properties
		{
			get
			{
				return this.m_properties;
			}
		}

		public InteractionSource source
		{
			get
			{
				return this.m_source;
			}
		}

		public Ray headRay
		{
			get
			{
				return this.m_headRay;
			}
		}
	}
}
