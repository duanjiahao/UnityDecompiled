using System;
using UnityEngine.Scripting;

namespace UnityEditor.Callbacks
{
	[RequiredByNativeCode]
	public sealed class PostProcessBuildAttribute : CallbackOrderAttribute
	{
		public PostProcessBuildAttribute()
		{
			this.m_CallbackOrder = 1;
		}

		public PostProcessBuildAttribute(int callbackOrder)
		{
			this.m_CallbackOrder = callbackOrder;
		}
	}
}
