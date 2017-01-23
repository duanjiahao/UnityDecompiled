using System;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class AssemblyReloadEvents
	{
		public static void OnBeforeAssemblyReload()
		{
			InternalEditorUtility.AuxWindowManager_OnAssemblyReload();
		}

		public static void OnAfterAssemblyReload()
		{
			foreach (ProjectBrowser current in ProjectBrowser.GetAllProjectBrowsers())
			{
				current.Repaint();
			}
		}
	}
}
