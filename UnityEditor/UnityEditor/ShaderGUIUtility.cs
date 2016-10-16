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
							return Activator.CreateInstance(type) as ShaderGUI;
						}
						return null;
					}
					else
					{
						j++;
					}
				}
			}
			return null;
		}
	}
}
