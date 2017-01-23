using System;

namespace Unity.BindingsGenerator.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	internal class NativeIncludeAttribute : Attribute
	{
		public string Header
		{
			get;
			set;
		}
	}
}
