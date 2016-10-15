using System;

namespace UnityEngine
{
	public interface ICanvasRaycastFilter
	{
		bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera);
	}
}
