using System;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	public class SimulatedHead
	{
		public float diameter
		{
			get
			{
				return HolographicEmulation.GetHeadDiameter_Internal();
			}
			set
			{
				HolographicEmulation.SetHeadDiameter_Internal(value);
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return HolographicEmulation.GetHeadRotation_Internal();
			}
			set
			{
				HolographicEmulation.SetHeadRotation_Internal(value);
			}
		}

		internal SimulatedHead()
		{
		}
	}
}
