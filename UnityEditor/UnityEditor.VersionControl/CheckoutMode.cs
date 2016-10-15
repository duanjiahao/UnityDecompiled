using System;

namespace UnityEditor.VersionControl
{
	[Flags]
	public enum CheckoutMode
	{
		Asset = 1,
		Meta = 2,
		Both = 3,
		Exact = 4
	}
}
