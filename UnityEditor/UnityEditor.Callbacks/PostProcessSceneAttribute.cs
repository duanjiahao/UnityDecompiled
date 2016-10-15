using System;
using UnityEngine.Scripting;

namespace UnityEditor.Callbacks
{
	[RequiredByNativeCode]
	public sealed class PostProcessSceneAttribute : CallbackOrderAttribute
	{
		private int m_version;

		internal int version
		{
			get
			{
				return this.m_version;
			}
		}

		public PostProcessSceneAttribute()
		{
			this.m_CallbackOrder = 1;
			this.m_version = 0;
		}

		public PostProcessSceneAttribute(int callbackOrder)
		{
			this.m_CallbackOrder = callbackOrder;
			this.m_version = 0;
		}

		public PostProcessSceneAttribute(int callbackOrder, int version)
		{
			this.m_CallbackOrder = callbackOrder;
			this.m_version = version;
		}
	}
}
