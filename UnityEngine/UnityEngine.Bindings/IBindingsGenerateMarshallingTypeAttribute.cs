using System;

namespace UnityEngine.Bindings
{
	internal interface IBindingsGenerateMarshallingTypeAttribute : IBindingsAttribute
	{
		CodegenOptions CodegenOptions
		{
			get;
			set;
		}
	}
}
