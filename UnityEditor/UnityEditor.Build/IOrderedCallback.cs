using System;

namespace UnityEditor.Build
{
	public interface IOrderedCallback
	{
		int callbackOrder
		{
			get;
		}
	}
}
