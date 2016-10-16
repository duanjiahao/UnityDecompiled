using System;

namespace UnityEditor.Callbacks
{
	internal sealed class RegisterPluginsAttribute : CallbackOrderAttribute
	{
		public RegisterPluginsAttribute()
		{
			this.m_CallbackOrder = 1;
		}

		public RegisterPluginsAttribute(int callbackOrder)
		{
			this.m_CallbackOrder = callbackOrder;
		}
	}
}
