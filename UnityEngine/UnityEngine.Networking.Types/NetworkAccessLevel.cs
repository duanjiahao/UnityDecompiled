using System;
using System.ComponentModel;

namespace UnityEngine.Networking.Types
{
	[DefaultValue(NetworkAccessLevel.Invalid)]
	public enum NetworkAccessLevel : ulong
	{
		Invalid,
		User,
		Owner,
		Admin = 4uL
	}
}
