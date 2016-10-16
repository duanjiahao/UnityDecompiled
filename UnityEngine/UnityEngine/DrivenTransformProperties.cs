using System;

namespace UnityEngine
{
	[Flags]
	public enum DrivenTransformProperties
	{
		None = 0,
		All = -1,
		AnchoredPositionX = 2,
		AnchoredPositionY = 4,
		AnchoredPositionZ = 8,
		Rotation = 16,
		ScaleX = 32,
		ScaleY = 64,
		ScaleZ = 128,
		AnchorMinX = 256,
		AnchorMinY = 512,
		AnchorMaxX = 1024,
		AnchorMaxY = 2048,
		SizeDeltaX = 4096,
		SizeDeltaY = 8192,
		PivotX = 16384,
		PivotY = 32768,
		AnchoredPosition = 6,
		AnchoredPosition3D = 14,
		Scale = 224,
		AnchorMin = 768,
		AnchorMax = 3072,
		Anchors = 3840,
		SizeDelta = 12288,
		Pivot = 49152
	}
}
