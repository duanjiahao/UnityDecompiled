using System;

namespace UnityEditor
{
	[Obsolete("Setting and getting import channels is not used anymore (use forceToMono instead)", true)]
	public enum AudioImporterChannels
	{
		Automatic,
		Mono,
		Stereo
	}
}
