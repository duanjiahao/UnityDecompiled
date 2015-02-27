using System;
namespace UnityEditor.Callbacks
{
	public sealed class PostProcessSceneAttribute : CallbackOrderAttribute
	{
		public PostProcessSceneAttribute()
		{
			this.m_CallbackOrder = 1;
		}
		public PostProcessSceneAttribute(int callbackOrder)
		{
			this.m_CallbackOrder = callbackOrder;
		}
	}
}
