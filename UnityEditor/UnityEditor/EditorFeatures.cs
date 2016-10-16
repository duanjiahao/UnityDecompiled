using System;

namespace UnityEditor
{
	[Flags]
	internal enum EditorFeatures
	{
		None = 0,
		PreviewGUI = 1,
		OnSceneDrag = 4
	}
}
