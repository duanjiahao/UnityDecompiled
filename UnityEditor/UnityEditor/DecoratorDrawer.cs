using System;
using UnityEngine;

namespace UnityEditor
{
	public abstract class DecoratorDrawer : GUIDrawer
	{
		internal PropertyAttribute m_Attribute;

		public PropertyAttribute attribute
		{
			get
			{
				return this.m_Attribute;
			}
		}

		public virtual void OnGUI(Rect position)
		{
		}

		public virtual float GetHeight()
		{
			return 16f;
		}
	}
}
