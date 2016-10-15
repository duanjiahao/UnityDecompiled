using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class SpaceAttribute : PropertyAttribute
	{
		public readonly float height;

		public SpaceAttribute()
		{
			this.height = 8f;
		}

		public SpaceAttribute(float height)
		{
			this.height = height;
		}
	}
}
