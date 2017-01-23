using System;
using UnityEngine.UI.Collections;

namespace UnityEngine.UI
{
	public class ClipperRegistry
	{
		private static ClipperRegistry s_Instance;

		private readonly IndexedSet<IClipper> m_Clippers = new IndexedSet<IClipper>();

		public static ClipperRegistry instance
		{
			get
			{
				if (ClipperRegistry.s_Instance == null)
				{
					ClipperRegistry.s_Instance = new ClipperRegistry();
				}
				return ClipperRegistry.s_Instance;
			}
		}

		protected ClipperRegistry()
		{
		}

		public void Cull()
		{
			for (int i = 0; i < this.m_Clippers.Count; i++)
			{
				this.m_Clippers[i].PerformClipping();
			}
		}

		public static void Register(IClipper c)
		{
			if (c != null)
			{
				ClipperRegistry.instance.m_Clippers.AddUnique(c);
			}
		}

		public static void Unregister(IClipper c)
		{
			ClipperRegistry.instance.m_Clippers.Remove(c);
		}
	}
}
