using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
	internal static class RaycasterManager
	{
		private static readonly List<BaseRaycaster> s_Raycasters = new List<BaseRaycaster>();

		public static void AddRaycaster(BaseRaycaster baseRaycaster)
		{
			if (!RaycasterManager.s_Raycasters.Contains(baseRaycaster))
			{
				RaycasterManager.s_Raycasters.Add(baseRaycaster);
			}
		}

		public static List<BaseRaycaster> GetRaycasters()
		{
			return RaycasterManager.s_Raycasters;
		}

		public static void RemoveRaycasters(BaseRaycaster baseRaycaster)
		{
			if (RaycasterManager.s_Raycasters.Contains(baseRaycaster))
			{
				RaycasterManager.s_Raycasters.Remove(baseRaycaster);
			}
		}
	}
}
