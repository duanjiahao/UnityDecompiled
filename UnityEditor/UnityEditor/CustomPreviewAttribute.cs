using System;

namespace UnityEditor
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class CustomPreviewAttribute : Attribute
	{
		internal Type m_Type;

		public CustomPreviewAttribute(Type type)
		{
			this.m_Type = type;
		}
	}
}
