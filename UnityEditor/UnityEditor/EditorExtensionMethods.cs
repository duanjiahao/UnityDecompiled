using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal static class EditorExtensionMethods
	{
		internal static bool MainActionKeyForControl(this Event evt, int controlId)
		{
			if (GUIUtility.keyboardControl != controlId)
			{
				return false;
			}
			bool flag = evt.alt || evt.shift || evt.command || evt.control;
			if (evt.type == EventType.KeyDown && evt.character == ' ' && !flag)
			{
				evt.Use();
				return false;
			}
			return evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) && !flag;
		}

		internal static bool IsArrayOrList(this Type listType)
		{
			return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
		}

		internal static Type GetArrayOrListElementType(this Type listType)
		{
			if (listType.IsArray)
			{
				return listType.GetElementType();
			}
			if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return listType.GetGenericArguments()[0];
			}
			return null;
		}

		internal static List<Enum> EnumGetNonObsoleteValues(this Type type)
		{
			string[] names = Enum.GetNames(type);
			Enum[] array = Enum.GetValues(type).Cast<Enum>().ToArray<Enum>();
			List<Enum> list = new List<Enum>();
			for (int i = 0; i < names.Length; i++)
			{
				MemberInfo[] member = type.GetMember(names[i]);
				object[] customAttributes = member[0].GetCustomAttributes(typeof(ObsoleteAttribute), false);
				bool flag = false;
				object[] array2 = customAttributes;
				for (int j = 0; j < array2.Length; j++)
				{
					object obj = array2[j];
					if (obj is ObsoleteAttribute)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					list.Add(array[i]);
				}
			}
			return list;
		}
	}
}
