using System;
using UnityEngine;

namespace UnityEditor
{
	internal class PreviewHelpers
	{
		internal static void AdjustWidthAndHeightForStaticPreview(int textureWidth, int textureHeight, ref int width, ref int height)
		{
			int max = width;
			int max2 = height;
			if (textureWidth <= width && textureHeight <= height)
			{
				width = textureWidth;
				height = textureHeight;
			}
			else
			{
				float b = (float)height / (float)textureWidth;
				float a = (float)width / (float)textureHeight;
				float num = Mathf.Min(a, b);
				width = Mathf.RoundToInt((float)textureWidth * num);
				height = Mathf.RoundToInt((float)textureHeight * num);
			}
			width = Mathf.Clamp(width, 2, max);
			height = Mathf.Clamp(height, 2, max2);
		}
	}
}
