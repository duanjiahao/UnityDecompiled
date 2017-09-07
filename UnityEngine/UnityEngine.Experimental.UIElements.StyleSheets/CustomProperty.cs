using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal struct CustomProperty
	{
		public int specificity;

		public StyleValueHandle handle;

		public StyleSheet data;
	}
}
