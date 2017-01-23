using System;

namespace Unity.BindingsGenerator.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Enum)]
	internal class NativeEnumAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public string Header
		{
			get;
			set;
		}

		public bool GenerateNativeType
		{
			get;
			set;
		}
	}
}
