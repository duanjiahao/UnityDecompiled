using System;

namespace UnityEngine.CSSLayout
{
	internal class MeasureOutput
	{
		public static long Make(double width, double height)
		{
			return MeasureOutput.Make((int)width, (int)height);
		}

		public static long Make(int width, int height)
		{
			return (long)width << 32 | (long)((ulong)height);
		}

		public static int GetWidth(long measureOutput)
		{
			return (int)((ulong)-1 & (ulong)(measureOutput >> 32));
		}

		public static int GetHeight(long measureOutput)
		{
			return (int)((ulong)-1 & (ulong)measureOutput);
		}
	}
}
