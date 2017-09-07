using System;
using System.Reflection;

namespace UnityEditor
{
	internal static class ShaderGUIUtility
	{
		private static Type ExtractCustomEditorType(string customEditorName)
		{
			Type result;
			if (string.IsNullOrEmpty(customEditorName))
			{
				result = null;
			}
			else
			{
				string value = "UnityEditor." + customEditorName;
				Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
				for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
				{
					Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(loadedAssemblies[i]);
					for (int j = 0; j < typesFromAssembly.Length; j++)
					{
						Type type = typesFromAssembly[j];
						if (type.FullName.Equals(customEditorName, StringComparison.Ordinal) || type.FullName.Equals(value, StringComparison.Ordinal))
						{
							result = ((!typeof(ShaderGUI).IsAssignableFrom(type)) ? null : type);
							return result;
						}
					}
				}
				result = null;
			}
			return result;
		}

		internal static ShaderGUI CreateShaderGUI(string customEditorName)
		{
			Type type = ShaderGUIUtility.ExtractCustomEditorType(customEditorName);
			return (type == null) ? null : (Activator.CreateInstance(type) as ShaderGUI);
		}
	}
}
