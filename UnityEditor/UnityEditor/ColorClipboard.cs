using System;
using UnityEngine;

namespace UnityEditor
{
	internal static class ColorClipboard
	{
		public static void SetColor(Color color)
		{
			EditorGUIUtility.systemCopyBuffer = "";
			EditorGUIUtility.SetPasteboardColor(color);
		}

		public static bool HasColor()
		{
			Color color;
			return ColorClipboard.TryGetColor(false, out color);
		}

		public static bool TryGetColor(bool allowHDR, out Color color)
		{
			bool flag = false;
			if (ColorUtility.TryParseHtmlString(EditorGUIUtility.systemCopyBuffer, out color))
			{
				flag = true;
			}
			else if (EditorGUIUtility.HasPasteboardColor())
			{
				color = EditorGUIUtility.GetPasteboardColor();
				flag = true;
			}
			bool result;
			if (flag)
			{
				if (!allowHDR && color.maxColorComponent > 1f)
				{
					color = color.RGBMultiplied(1f / color.maxColorComponent);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
