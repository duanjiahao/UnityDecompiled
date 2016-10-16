using System;

namespace UnityEditor
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public sealed class CustomPropertyDrawer : Attribute
	{
		internal Type m_Type;

		internal bool m_UseForChildren;

		public CustomPropertyDrawer(Type type)
		{
			this.m_Type = type;
		}

		public CustomPropertyDrawer(Type type, bool useForChildren)
		{
			this.m_Type = type;
			this.m_UseForChildren = useForChildren;
		}
	}
}
