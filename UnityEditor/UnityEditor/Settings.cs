using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace UnityEditor
{
	internal class Settings
	{
		private static SortedList<string, object> m_Prefs = new SortedList<string, object>();
		internal static T Get<T>(string name, T defaultValue) where T : IPrefType, new()
		{
			if (defaultValue == null)
			{
				throw new ArgumentException("default can not be null", "defaultValue");
			}
			if (Settings.m_Prefs.ContainsKey(name))
			{
				return (T)((object)Settings.m_Prefs[name]);
			}
			string @string = EditorPrefs.GetString(name, string.Empty);
			if (@string == string.Empty)
			{
				Settings.Set<T>(name, defaultValue);
				return defaultValue;
			}
			defaultValue.FromUniqueString(@string);
			Settings.Set<T>(name, defaultValue);
			return defaultValue;
		}
		internal static void Set<T>(string name, T value) where T : IPrefType
		{
			EditorPrefs.SetString(name, value.ToUniqueString());
			Settings.m_Prefs[name] = value;
		}
		[DebuggerHidden]
		internal static IEnumerable<KeyValuePair<string, T>> Prefs<T>() where T : IPrefType
		{
			Settings.<Prefs>c__Iterator1<T> <Prefs>c__Iterator = new Settings.<Prefs>c__Iterator1<T>();
			Settings.<Prefs>c__Iterator1<T> expr_07 = <Prefs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}
