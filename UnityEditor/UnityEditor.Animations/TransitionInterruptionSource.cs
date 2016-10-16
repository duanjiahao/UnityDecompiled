using System;

namespace UnityEditor.Animations
{
	public enum TransitionInterruptionSource
	{
		None,
		Source,
		Destination,
		SourceThenDestination,
		DestinationThenSource
	}
}
