using System;

namespace UnityEngine
{
	public enum RigidbodyConstraints
	{
		None,
		FreezePositionX = 2,
		FreezePositionY = 4,
		FreezePositionZ = 8,
		FreezeRotationX = 16,
		FreezeRotationY = 32,
		FreezeRotationZ = 64,
		FreezePosition = 14,
		FreezeRotation = 112,
		FreezeAll = 126
	}
}
