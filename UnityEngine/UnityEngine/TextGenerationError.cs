using System;

namespace UnityEngine
{
	[Flags]
	internal enum TextGenerationError
	{
		None = 0,
		CustomSizeOnNonDynamicFont = 1,
		CustomStyleOnNonDynamicFont = 2,
		NoFont = 4
	}
}
