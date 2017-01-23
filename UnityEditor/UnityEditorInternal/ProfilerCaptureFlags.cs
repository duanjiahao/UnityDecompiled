using System;

namespace UnityEditorInternal
{
	[Flags]
	public enum ProfilerCaptureFlags
	{
		None = 0,
		Channels = 1,
		DSPNodes = 2,
		Clips = 4,
		All = 7
	}
}
