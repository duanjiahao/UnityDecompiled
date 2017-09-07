using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal struct RuleMatcher
	{
		public StyleSheet sheet;

		public StyleComplexSelector complexSelector;

		public int simpleSelectorIndex;

		public int depth;
	}
}
