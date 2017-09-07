using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property)]
	internal class StaticAccessorAttribute : Attribute, IBindingsAttribute
	{
		public string Name
		{
			get;
			set;
		}

		public bool Pointer
		{
			get;
			set;
		}

		public StaticAccessorAttribute()
		{
		}

		internal StaticAccessorAttribute(string name)
		{
			this.Name = name;
		}

		public StaticAccessorAttribute(bool pointer)
		{
			this.Pointer = pointer;
		}

		public StaticAccessorAttribute(string name, bool pointer)
		{
			this.Name = name;
			this.Pointer = pointer;
		}
	}
}
