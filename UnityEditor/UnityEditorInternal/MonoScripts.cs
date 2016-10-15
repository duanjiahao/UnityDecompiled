using System;
using UnityEditor;

namespace UnityEditorInternal
{
	public static class MonoScripts
	{
		public static MonoScript CreateMonoScript(string scriptContents, string className, string nameSpace, string assemblyName, bool isEditorScript)
		{
			MonoScript monoScript = new MonoScript();
			monoScript.Init(scriptContents, className, nameSpace, assemblyName, isEditorScript);
			return monoScript;
		}
	}
}
