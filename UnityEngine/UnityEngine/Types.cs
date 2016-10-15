using System;
using System.Reflection;

namespace UnityEngine
{
	public static class Types
	{
		public static Type GetType(string typeName, string assemblyName)
		{
			Type result;
			try
			{
				result = Assembly.Load(assemblyName).GetType(typeName);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}
	}
}
