using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	internal static class SetPropertyUtility
	{
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			bool result;
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				result = false;
			}
			else
			{
				currentValue = newValue;
				result = true;
			}
			return result;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			bool result;
			if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				result = false;
			}
			else
			{
				currentValue = newValue;
				result = true;
			}
			return result;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			bool result;
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				result = false;
			}
			else
			{
				currentValue = newValue;
				result = true;
			}
			return result;
		}
	}
}
