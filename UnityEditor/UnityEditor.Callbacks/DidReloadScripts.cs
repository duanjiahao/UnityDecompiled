using System;
namespace UnityEditor.Callbacks
{
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
