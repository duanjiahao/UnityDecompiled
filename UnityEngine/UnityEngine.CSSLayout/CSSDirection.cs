using System;

namespace UnityEngine.CSSLayout
{
	internal enum CSSDirection
	{
		Inherit,
		LTR,
		RTL,
		[Obsolete("Use LTR instead")]
		LeftToRight = 1,
		[Obsolete("Use RTL instead")]
		RightToLeft
	}
}
