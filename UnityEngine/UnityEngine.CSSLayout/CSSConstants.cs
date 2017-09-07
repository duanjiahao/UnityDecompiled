using System;

namespace UnityEngine.CSSLayout
{
	internal static class CSSConstants
	{
		public const float Undefined = float.NaN;

		public static bool IsUndefined(float value)
		{
			return float.IsNaN(value);
		}
	}
}
