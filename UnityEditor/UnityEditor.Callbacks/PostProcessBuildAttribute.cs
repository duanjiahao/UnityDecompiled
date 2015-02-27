using System;
namespace UnityEditor.Callbacks
{
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
