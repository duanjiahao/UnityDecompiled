using System;

namespace UnityEditor
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class PreferenceItem : Attribute
	{
		public string name;

		public PreferenceItem(string name)
		{
			this.name = name;
		}
	}
}
