using System;

namespace UnityEngine
{
	public enum CollisionFlags
	{
		None,
		Sides,
		Above,
		Below = 4,
		CollidedSides = 1,
		CollidedAbove,
		CollidedBelow = 4
	}
}
