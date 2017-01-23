using System;

namespace UnityEngine.VR.WSA.Input
{
	public enum GestureSettings
	{
		None,
		Tap,
		DoubleTap,
		Hold = 4,
		ManipulationTranslate = 8,
		NavigationX = 16,
		NavigationY = 32,
		NavigationZ = 64,
		NavigationRailsX = 128,
		NavigationRailsY = 256,
		NavigationRailsZ = 512
	}
}
