using System;
using System.Collections.Generic;
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
	}
}
