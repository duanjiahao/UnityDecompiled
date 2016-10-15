using System;

namespace UnityEngine.Networking
{
	public enum QosType
	{
		Unreliable,
		UnreliableFragmented,
		UnreliableSequenced,
		Reliable,
		ReliableFragmented,
		ReliableSequenced,
		StateUpdate,
		ReliableStateUpdate,
		AllCostDelivery
	}
}
