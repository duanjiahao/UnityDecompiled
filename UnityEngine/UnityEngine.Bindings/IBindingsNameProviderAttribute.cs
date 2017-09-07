using System;

namespace UnityEngine.Bindings
{
	internal interface IBindingsNameProviderAttribute : IBindingsAttribute
	{
		string Name
		{
			get;
			set;
		}
	}
}
