using System;
namespace UnityEditor
{
	public abstract class CallbackOrderAttribute : Attribute
	{
		protected int m_CallbackOrder;
		internal int callbackOrder
		{
			get
			{
				return this.m_CallbackOrder;
			}
		}
	}
}
