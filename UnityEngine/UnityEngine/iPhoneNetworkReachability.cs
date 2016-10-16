using System;

namespace UnityEngine
{
	[Obsolete("iPhoneNetworkReachability enumeration is deprecated. Please use NetworkReachability instead (UnityUpgradable) -> NetworkReachability", true)]
	public enum iPhoneNetworkReachability
	{
		NotReachable,
		ReachableViaCarrierDataNetwork,
		[Obsolete]
		ReachableViaWiFiNetwork
	}
}
