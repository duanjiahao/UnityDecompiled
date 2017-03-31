using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct NetworkMessageInfo
	{
		private double m_TimeStamp;

		private NetworkPlayer m_Sender;

		private NetworkViewID m_ViewID;

		public double timestamp
		{
			get
			{
				return this.m_TimeStamp;
			}
		}

		public NetworkPlayer sender
		{
			get
			{
				return this.m_Sender;
			}
		}

		public NetworkView networkView
		{
			get
			{
				NetworkView result;
				if (this.m_ViewID == NetworkViewID.unassigned)
				{
					Debug.LogError("No NetworkView is assigned to this NetworkMessageInfo object. Note that this is expected in OnNetworkInstantiate().");
					result = this.NullNetworkView();
				}
				else
				{
					result = NetworkView.Find(this.m_ViewID);
				}
				return result;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern NetworkView NullNetworkView();
	}
}
