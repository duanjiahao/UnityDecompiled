using System;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleSheetExtensions
	{
		public static void Apply(this StyleSheet sheet, StyleValueHandle handle, int specificity, ref Style<float> property)
		{
			if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 2)
			{
				StyleSheetExtensions.Apply<float>(0f, specificity, ref property);
			}
			else
			{
				StyleSheetExtensions.Apply<float>(sheet.ReadFloat(handle), specificity, ref property);
			}
		}

		public static void Apply(this StyleSheet sheet, StyleValueHandle handle, int specificity, ref Style<Color> property)
		{
			if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 2)
			{
				StyleSheetExtensions.Apply<Color>(default(Color), specificity, ref property);
			}
			else
			{
				StyleSheetExtensions.Apply<Color>(sheet.ReadColor(handle), specificity, ref property);
			}
		}

		public static void Apply(this StyleSheet sheet, StyleValueHandle handle, int specificity, ref Style<int> property)
		{
			if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 2)
			{
				StyleSheetExtensions.Apply<int>(0, specificity, ref property);
			}
			else
			{
				StyleSheetExtensions.Apply<int>((int)sheet.ReadFloat(handle), specificity, ref property);
			}
		}

		public static void Apply(this StyleSheet sheet, StyleValueHandle handle, int specificity, ref Style<bool> property)
		{
			bool val = sheet.ReadKeyword(handle) == StyleValueKeyword.True;
			if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 2)
			{
				StyleSheetExtensions.Apply<bool>(false, specificity, ref property);
			}
			else
			{
				StyleSheetExtensions.Apply<bool>(val, specificity, ref property);
			}
		}

		public static void Apply<T>(this StyleSheet sheet, StyleValueHandle handle, int specificity, ref Style<int> property) where T : struct
		{
			if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 2)
			{
				StyleSheetExtensions.Apply<int>(0, specificity, ref property);
			}
			else
			{
				StyleSheetExtensions.Apply<int>(StyleSheetCache.GetEnumValue<T>(sheet, handle), specificity, ref property);
			}
		}

		public static void Apply<T>(this StyleSheet sheet, StyleValueHandle handle, int specificity, LoadResourceFunction loadResourceFunc, ref Style<T> property) where T : UnityEngine.Object
		{
			if (handle.valueType == StyleValueType.Keyword && handle.valueIndex == 5)
			{
				StyleSheetExtensions.Apply<T>((T)((object)null), specificity, ref property);
			}
			else
			{
				string text = sheet.ReadResourcePath(handle);
				if (!string.IsNullOrEmpty(text))
				{
					T t = loadResourceFunc(text, typeof(T)) as T;
					if (t != null)
					{
						StyleSheetExtensions.Apply<T>(t, specificity, ref property);
					}
					else
					{
						Debug.LogWarning(string.Format("{0} resource not found for path: {1}", typeof(T).Name, text));
					}
				}
			}
		}

		private static void Apply<T>(T val, int specificity, ref Style<T> property)
		{
			property.Apply(new Style<T>(val, specificity), StylePropertyApplyMode.CopyIfMoreSpecific);
		}

		public static string ReadAsString(this StyleSheet sheet, StyleValueHandle handle)
		{
			string result = string.Empty;
			switch (handle.valueType)
			{
			case StyleValueType.Keyword:
				result = sheet.ReadKeyword(handle).ToString();
				break;
			case StyleValueType.Float:
				result = sheet.ReadFloat(handle).ToString();
				break;
			case StyleValueType.Color:
				result = sheet.ReadColor(handle).ToString();
				break;
			case StyleValueType.ResourcePath:
				result = sheet.ReadResourcePath(handle);
				break;
			case StyleValueType.Enum:
				result = sheet.ReadEnum(handle);
				break;
			case StyleValueType.String:
				result = sheet.ReadString(handle);
				break;
			default:
				throw new ArgumentException("Unhandled type " + handle.valueType);
			}
			return result;
		}
	}
}
