using System;

namespace UnityEngine.Experimental.Rendering
{
	[Flags]
	public enum VisibleLightFlags
	{
		None = 0,
		IntersectsNearPlane = 1,
		IntersectsFarPlane = 2
	}
}
