using System;

namespace UnityEngine
{
	[Flags]
	public enum RigidbodyConstraints2D
	{
		None = 0,
		FreezePositionX = 1,
		FreezePositionY = 2,
		FreezeRotation = 4,
		FreezePosition = 3,
		FreezeAll = 7
	}
}
