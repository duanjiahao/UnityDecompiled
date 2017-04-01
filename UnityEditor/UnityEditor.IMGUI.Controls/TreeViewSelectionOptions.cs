using System;

namespace UnityEditor.IMGUI.Controls
{
	[Flags]
	public enum TreeViewSelectionOptions
	{
		None = 0,
		FireSelectionChanged = 1,
		RevealAndFrame = 2
	}
}
