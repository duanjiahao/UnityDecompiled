using System;
using System.Collections.Generic;

namespace UnityEditor
{
	internal class DebugUtils
	{
		internal static string ListToString<T>(IEnumerable<T> list)
		{
			if (list == null)
			{
				return "[null list]";
			}
			string text = "[";
			int num = 0;
			foreach (T current in list)
			{
				if (num != 0)
				{
					text += ", ";
				}
				if (current != null)
				{
					text += current.ToString();
				}
				else
				{
					text += "'null'";
				}
				num++;
			}
			text += "]";
			if (num == 0)
			{
				return "[empty list]";
			}
			return string.Concat(new object[]
			{
				"(",
				num,
				") ",
				text
			});
		}
	}
}
