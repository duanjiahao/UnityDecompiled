using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	internal class NativeIncludeAttribute : Attribute
	{
		public string Header
		{
			get;
			set;
		}
	}
}
