using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property)]
	internal class NativeConditionalAttribute : Attribute, IBindingsAttribute
	{
		public string Condition
		{
			get;
			set;
		}

		public bool Enabled
		{
			get;
			set;
		}

		public NativeConditionalAttribute()
		{
		}

		public NativeConditionalAttribute(string condition)
		{
			this.Condition = condition;
			this.Enabled = true;
		}

		public NativeConditionalAttribute(bool enabled)
		{
			this.Enabled = enabled;
		}

		public NativeConditionalAttribute(string condition, bool enabled) : this(condition)
		{
			this.Enabled = enabled;
		}
	}
}
