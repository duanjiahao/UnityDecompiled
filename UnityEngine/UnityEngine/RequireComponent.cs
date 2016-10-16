using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class RequireComponent : Attribute
	{
		public Type m_Type0;

		public Type m_Type1;

		public Type m_Type2;

		public RequireComponent(Type requiredComponent)
		{
			this.m_Type0 = requiredComponent;
		}

		public RequireComponent(Type requiredComponent, Type requiredComponent2)
		{
			this.m_Type0 = requiredComponent;
			this.m_Type1 = requiredComponent2;
		}

		public RequireComponent(Type requiredComponent, Type requiredComponent2, Type requiredComponent3)
		{
			this.m_Type0 = requiredComponent;
			this.m_Type1 = requiredComponent2;
			this.m_Type2 = requiredComponent3;
		}
	}
}
