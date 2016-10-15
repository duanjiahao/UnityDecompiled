using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssemblyReloadEvents
	{
		public static void OnBeforeAssemblyReload()
		{
			Security.ClearVerifiedAssemblies();
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
