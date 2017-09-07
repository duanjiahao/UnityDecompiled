using System;

namespace UnityEngine.Bindings
{
	internal interface IBindingsHeaderProviderAttribute : IBindingsAttribute
	{
		string Header
		{
			get;
			set;
		}
	}
}
