using System;

namespace UnityEngine.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	internal class NativeHeaderAttribute : Attribute, IBindingsHeaderProviderAttribute, IBindingsAttribute
	{
		public string Header
		{
			get;
			set;
		}

		public NativeHeaderAttribute()
		{
		}

		public NativeHeaderAttribute(string header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			if (header == "")
			{
				throw new ArgumentException("header cannot be empty", "header");
			}
			this.Header = header;
		}
	}
}
