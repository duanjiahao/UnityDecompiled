using System;
using UnityEngine;

namespace UnityEditor.Scripting.ScriptCompilation
{
	[Serializable]
	internal class CustomScriptAssemblyData
	{
		public string name;

		public string[] references;

		public string[] includePlatforms;

		public string[] excludePlatforms;

		public static CustomScriptAssemblyData FromJson(string json)
		{
			CustomScriptAssemblyData customScriptAssemblyData = JsonUtility.FromJson<CustomScriptAssemblyData>(json);
			if (customScriptAssemblyData == null)
			{
				throw new Exception("Json file does not contain an assembly definition");
			}
			if (string.IsNullOrEmpty(customScriptAssemblyData.name))
			{
				throw new Exception("Required property 'name' not set");
			}
			if (customScriptAssemblyData.excludePlatforms != null && customScriptAssemblyData.excludePlatforms.Length > 0 && customScriptAssemblyData.includePlatforms != null && customScriptAssemblyData.includePlatforms.Length > 0)
			{
				throw new Exception("Both 'excludePlatforms' and 'includePlatforms' are set.");
			}
			return customScriptAssemblyData;
		}
	}
}
