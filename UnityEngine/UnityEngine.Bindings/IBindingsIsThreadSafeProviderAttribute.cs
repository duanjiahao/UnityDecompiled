using System;

namespace UnityEngine.Bindings
{
	internal interface IBindingsIsThreadSafeProviderAttribute : IBindingsAttribute
	{
		bool IsThreadSafe
		{
			get;
			set;
		}
	}
}
