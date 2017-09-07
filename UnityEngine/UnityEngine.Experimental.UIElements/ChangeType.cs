using System;

namespace UnityEngine.Experimental.UIElements
{
	[Flags]
	public enum ChangeType
	{
		Layout = 16,
		Styles = 8,
		Transform = 4,
		StylesPath = 2,
		Repaint = 1
	}
}
