using System;
using System.Reflection;

namespace UnityEditor
{
	internal static class ShaderGUIUtility
	{
		internal static ShaderGUI CreateShaderGUI(string customEditorName)
		{
			string value = "UnityEditor." + customEditorName;
			Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
			ShaderGUI result;
			for (int i = loadedAssemblies.Length - 1; i >= 0; i--)
			{
				Assembly assembly = loadedAssemblies[i];
				Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
				Type[] array = typesFromAssembly;
				int j = 0;
				while (j < array.Length)
				{
					Type type = array[j];
					if (type.FullName.Equals(customEditorName, StringComparison.Ordinal) || type.FullName.Equals(value, StringComparison.Ordinal))
					{
						if (typeof(ShaderGUI).IsAssignableFrom(type))
						{
							ShaderGUI shaderGUI = Activator.CreateInstance(type) as ShaderGUI;
							result = shaderGUI;
							return result;
						}
						result = null;
						return result;
					}
					else
					{
						j++;
					}
				}
			}
			result = null;
			return result;
		}
	}
}
