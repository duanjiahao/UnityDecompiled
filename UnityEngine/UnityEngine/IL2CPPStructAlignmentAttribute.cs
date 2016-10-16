using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Struct)]
	internal class IL2CPPStructAlignmentAttribute : Attribute
	{
		public int Align;

		public IL2CPPStructAlignmentAttribute()
		{
			this.Align = 1;
		}
	}
}
