using System;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.Input
{
	[RequiredByNativeCode]
	public struct InteractionSource
	{
		internal uint m_id;

		internal InteractionSourceKind m_kind;

		public uint id
		{
			get
			{
				return this.m_id;
			}
		}

		public InteractionSourceKind kind
		{
			get
			{
				return this.m_kind;
			}
		}
	}
}
