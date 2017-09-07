using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal class PropertyNameUtils
	{
		public static PropertyName PropertyNameFromString([Unmarshalled] string name)
		{
			PropertyName result;
			PropertyNameUtils.PropertyNameFromString_Injected(name, out result);
			return result;
		}

		public static string StringFromPropertyName(PropertyName propertyName)
		{
			return PropertyNameUtils.StringFromPropertyName_Injected(ref propertyName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ConflictCountForID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PropertyNameFromString_Injected(string name, out PropertyName ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string StringFromPropertyName_Injected(ref PropertyName propertyName);
	}
}
