using System;

namespace UnityEngine.Bindings
{
	internal interface IBindingsIsFreeFunctionProviderAttribute : IBindingsAttribute
	{
		bool IsFreeFunction
		{
			get;
			set;
		}

		bool HasExplicitThis
		{
			get;
			set;
		}
	}
}
