using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
	internal class NativeConditionalAttribute : Attribute
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

		public NativeConditionalAttribute(string condition)
		{
			this.Condition = condition;
			this.Enabled = true;
		}

		public NativeConditionalAttribute(string condition, bool enabled)
		{
			this.Condition = condition;
			this.Enabled = enabled;
		}

		public NativeConditionalAttribute(bool enabled)
		{
			this.Condition = null;
			this.Enabled = enabled;
		}
	}
}
