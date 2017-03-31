using System;
using System.Collections.Generic;

namespace UnityEditor.Modules
{
	internal static class ModuleUtils
	{
		internal static string[] GetAdditionalReferencesForUserScripts()
		{
			List<string> list = new List<string>();
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModulesDontRegister)
			{
				list.AddRange(current.AssemblyReferencesForUserScripts);
			}
			return list.ToArray();
		}

		internal static string[] GetAdditionalReferencesForEditorCsharpProject()
		{
			List<string> list = new List<string>();
			foreach (IPlatformSupportModule current in ModuleManager.platformSupportModulesDontRegister)
			{
				list.AddRange(current.AssemblyReferencesForEditorCsharpProject);
			}
			return list.ToArray();
		}
	}
}
