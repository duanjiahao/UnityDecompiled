using System;

namespace UnityEditor.Audio
{
	internal enum ParameterTransitionType
	{
		Lerp,
		Smoothstep,
		Squared,
		SquareRoot,
		BrickwallStart,
		BrickwallEnd
	}
}
