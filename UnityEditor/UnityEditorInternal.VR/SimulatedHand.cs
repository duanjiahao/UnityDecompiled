using System;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	public class SimulatedHand
	{
		private GestureHand m_Hand;

		public Vector3 position
		{
			get
			{
				return HolographicEmulation.GetHandPosition_Internal(this.m_Hand);
			}
			set
			{
				HolographicEmulation.SetHandPosition_Internal(this.m_Hand, value);
			}
		}

		public bool activated
		{
			get
			{
				return HolographicEmulation.GetHandActivated_Internal(this.m_Hand);
			}
			set
			{
				HolographicEmulation.SetHandActivated_Internal(this.m_Hand, value);
			}
		}

		public bool visible
		{
			get
			{
				return HolographicEmulation.GetHandVisible_Internal(this.m_Hand);
			}
		}

		internal SimulatedHand(GestureHand hand)
		{
			this.m_Hand = hand;
		}

		public void EnsureVisible()
		{
			HolographicEmulation.EnsureHandVisible_Internal(this.m_Hand);
		}

		public void PerformGesture(SimulatedGesture gesture)
		{
			HolographicEmulation.PerformGesture_Internal(this.m_Hand, gesture);
		}
	}
}
