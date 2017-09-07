using System;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	public class SimulatedBody
	{
		public Vector3 position
		{
			get
			{
				return HolographicEmulation.GetBodyPosition_Internal();
			}
			set
			{
				HolographicEmulation.SetBodyPosition_Internal(value);
			}
		}

		public float rotation
		{
			get
			{
				return HolographicEmulation.GetBodyRotation_Internal();
			}
			set
			{
				HolographicEmulation.SetBodyRotation_Internal(value);
			}
		}

		public float height
		{
			get
			{
				return HolographicEmulation.GetBodyHeight_Internal();
			}
			set
			{
				HolographicEmulation.SetBodyHeight_Internal(value);
			}
		}

		internal SimulatedBody()
		{
		}
	}
}
