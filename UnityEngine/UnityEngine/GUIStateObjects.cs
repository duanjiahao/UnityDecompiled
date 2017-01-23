using System;
using System.Collections.Generic;
using System.Security;

namespace UnityEngine
{
	internal class GUIStateObjects
	{
		private static Dictionary<int, object> s_StateCache = new Dictionary<int, object>();

		[SecuritySafeCritical]
		internal static object GetStateObject(Type t, int controlID)
		{
			object obj;
			if (!GUIStateObjects.s_StateCache.TryGetValue(controlID, out obj) || obj.GetType() != t)
			{
				obj = Activator.CreateInstance(t);
				GUIStateObjects.s_StateCache[controlID] = obj;
			}
			return obj;
		}

		internal static object QueryStateObject(Type t, int controlID)
		{
			object obj = GUIStateObjects.s_StateCache[controlID];
			object result;
			if (t.IsInstanceOfType(obj))
			{
				result = obj;
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static void Tests_ClearObjects()
		{
			GUIStateObjects.s_StateCache.Clear();
		}
	}
}
