using System;

namespace UnityEngine.StyleSheets
{
	internal enum StyleSelectorType
	{
		Unknown,
		Wildcard,
		Type,
		Class,
		PseudoClass,
		RecursivePseudoClass,
		ID
	}
}
