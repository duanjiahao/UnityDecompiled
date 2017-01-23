using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[InitializeOnLoad]
	internal class PrefabLayoutRebuilder
	{
		[CompilerGenerated]
		private static PrefabUtility.PrefabInstanceUpdated <>f__mg$cache0;

		static PrefabLayoutRebuilder()
		{
			Delegate arg_23_0 = PrefabUtility.prefabInstanceUpdated;
			if (PrefabLayoutRebuilder.<>f__mg$cache0 == null)
			{
				PrefabLayoutRebuilder.<>f__mg$cache0 = new PrefabUtility.PrefabInstanceUpdated(PrefabLayoutRebuilder.OnPrefabInstanceUpdates);
			}
			PrefabUtility.prefabInstanceUpdated = (PrefabUtility.PrefabInstanceUpdated)Delegate.Combine(arg_23_0, PrefabLayoutRebuilder.<>f__mg$cache0);
		}

		private static void OnPrefabInstanceUpdates(GameObject instance)
		{
			if (instance)
			{
				RectTransform rectTransform = instance.transform as RectTransform;
				if (rectTransform)
				{
					LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				}
			}
		}
	}
}
