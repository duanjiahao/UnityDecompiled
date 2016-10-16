using System;
using UnityEngine.Scripting;

namespace UnityEditor.Callbacks
{
	[RequiredByNativeCode]
	public sealed class DidReloadScripts : CallbackOrderAttribute
	{
		public DidReloadScripts()
		{
			this.m_CallbackOrder = 1;
		}

		public DidReloadScripts(int callbackOrder)
		{
			this.m_CallbackOrder = callbackOrder;
		}
	}
}
