using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Struct)]
	internal class NativeStructAttribute : Attribute
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

		public NativeStructGenerateOption GenerateMarshallingType
		{
			get;
			set;
		}
	}
}
