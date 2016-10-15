using System;

namespace UnityEngine
{
	public sealed class AddComponentMenu : Attribute
	{
		private string m_AddComponentMenu;

		private int m_Ordering;

		public string componentMenu
		{
			get
			{
				return this.m_AddComponentMenu;
			}
		}

		public int componentOrder
		{
			get
			{
				return this.m_Ordering;
			}
		}

		public AddComponentMenu(string menuName)
		{
			this.m_AddComponentMenu = menuName;
			this.m_Ordering = 0;
		}

		public AddComponentMenu(string menuName, int order)
		{
			this.m_AddComponentMenu = menuName;
			this.m_Ordering = order;
		}
	}
}
