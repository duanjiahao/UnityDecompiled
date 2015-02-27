using System;
namespace UnityEditor.Callbacks
{
	public sealed class OnOpenAssetAttribute : CallbackOrderAttribute
	{
		public OnOpenAssetAttribute()
		{
			this.m_CallbackOrder = 1;
		}
		public OnOpenAssetAttribute(int callbackOrder)
		{
			this.m_CallbackOrder = callbackOrder;
		}
	}
}
