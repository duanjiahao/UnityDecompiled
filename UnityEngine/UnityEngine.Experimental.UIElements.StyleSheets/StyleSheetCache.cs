using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.StyleSheets;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	internal static class StyleSheetCache
	{
		private struct SheetHandleKey
		{
			public readonly int sheetInstanceID;

			public readonly int index;

			public SheetHandleKey(StyleSheet sheet, int index)
			{
				this.sheetInstanceID = sheet.GetInstanceID();
				this.index = index;
			}
		}

		private class SheetHandleKeyComparer : IEqualityComparer<StyleSheetCache.SheetHandleKey>
		{
			public bool Equals(StyleSheetCache.SheetHandleKey x, StyleSheetCache.SheetHandleKey y)
			{
				return x.sheetInstanceID == y.sheetInstanceID && x.index == y.index;
			}

			public int GetHashCode(StyleSheetCache.SheetHandleKey key)
			{
				return key.sheetInstanceID.GetHashCode() ^ key.index.GetHashCode();
			}
		}

		private static StyleSheetCache.SheetHandleKeyComparer s_Comparer;

		private static Dictionary<StyleSheetCache.SheetHandleKey, int> s_EnumToIntCache;

		private static Dictionary<StyleSheetCache.SheetHandleKey, StylePropertyID[]> s_RulePropertyIDsCache;

		private static Dictionary<string, StylePropertyID> s_NameToIDCache;

		static StyleSheetCache()
		{
			StyleSheetCache.s_Comparer = new StyleSheetCache.SheetHandleKeyComparer();
			StyleSheetCache.s_EnumToIntCache = new Dictionary<StyleSheetCache.SheetHandleKey, int>(StyleSheetCache.s_Comparer);
			StyleSheetCache.s_RulePropertyIDsCache = new Dictionary<StyleSheetCache.SheetHandleKey, StylePropertyID[]>(StyleSheetCache.s_Comparer);
			StyleSheetCache.s_NameToIDCache = new Dictionary<string, StylePropertyID>();
			FieldInfo[] fields = typeof(VisualElementStyles).GetFields();
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo element = array[i];
				StylePropertyAttribute stylePropertyAttribute = (StylePropertyAttribute)Attribute.GetCustomAttribute(element, typeof(StylePropertyAttribute));
				if (stylePropertyAttribute != null)
				{
					StyleSheetCache.s_NameToIDCache.Add(stylePropertyAttribute.propertyName, stylePropertyAttribute.propertyID);
				}
			}
		}

		internal static void ClearCaches()
		{
			StyleSheetCache.s_EnumToIntCache.Clear();
			StyleSheetCache.s_RulePropertyIDsCache.Clear();
		}

		internal static int GetEnumValue<T>(StyleSheet sheet, StyleValueHandle handle)
		{
			Debug.Assert(handle.valueType == StyleValueType.Enum);
			StyleSheetCache.SheetHandleKey key = new StyleSheetCache.SheetHandleKey(sheet, handle.valueIndex);
			int num;
			if (!StyleSheetCache.s_EnumToIntCache.TryGetValue(key, out num))
			{
				string value = sheet.ReadEnum(handle).Replace("-", string.Empty);
				object obj = Enum.Parse(typeof(T), value, true);
				num = (int)obj;
				StyleSheetCache.s_EnumToIntCache.Add(key, num);
			}
			Debug.Assert(Enum.GetName(typeof(T), num) != null);
			return num;
		}

		internal static StylePropertyID[] GetPropertyIDs(StyleSheet sheet, int ruleIndex)
		{
			StyleSheetCache.SheetHandleKey key = new StyleSheetCache.SheetHandleKey(sheet, ruleIndex);
			StylePropertyID[] array;
			if (!StyleSheetCache.s_RulePropertyIDsCache.TryGetValue(key, out array))
			{
				StyleRule styleRule = sheet.rules[ruleIndex];
				array = new StylePropertyID[styleRule.properties.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = StyleSheetCache.GetPropertyID(styleRule.properties[i].name);
				}
				StyleSheetCache.s_RulePropertyIDsCache.Add(key, array);
			}
			return array;
		}

		private static StylePropertyID GetPropertyID(string name)
		{
			StylePropertyID result;
			if (!StyleSheetCache.s_NameToIDCache.TryGetValue(name, out result))
			{
				result = StylePropertyID.Custom;
			}
			return result;
		}
	}
}
