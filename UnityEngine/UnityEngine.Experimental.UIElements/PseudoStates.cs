using System;

namespace UnityEngine.Experimental.UIElements
{
	[Flags]
	internal enum PseudoStates
	{
		Active = 1,
		Hover = 2,
		Checked = 8,
		Selected = 16,
		Disabled = 32,
		Focus = 64,
		Invisible = -2147483648
	}
}
