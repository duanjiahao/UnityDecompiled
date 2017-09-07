using System;

namespace UnityEngine.Bindings
{
	internal interface IBindingsThrowsProviderAttribute : IBindingsAttribute
	{
		bool ThrowsException
		{
			get;
			set;
		}
	}
}
