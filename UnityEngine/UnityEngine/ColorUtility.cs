using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ColorUtility
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoTryParseHtmlColor(string htmlString, out Color32 color);

		public static bool TryParseHtmlString(string htmlString, out Color color)
		{
			Color32 c;
			bool result = ColorUtility.DoTryParseHtmlColor(htmlString, out c);
			color = c;
			return result;
		}

		public static string ToHtmlStringRGB(Color color)
		{
			Color32 color2 = color;
			return string.Format("{0:X2}{1:X2}{2:X2}", color2.r, color2.g, color2.b);
		}

		public static string ToHtmlStringRGBA(Color color)
		{
			Color32 color2 = color;
			return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
			{
				color2.r,
				color2.g,
				color2.b,
				color2.a
			});
		}
	}
}
