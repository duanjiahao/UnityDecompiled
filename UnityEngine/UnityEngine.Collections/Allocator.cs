using System;
using UnityEngine.Scripting;

namespace UnityEngine.Collections
{
	[UsedByNativeCode]
	public enum Allocator
	{
		Invalid,
		None,
		Temp,
		TempJob,
		Persistent
	}
}
