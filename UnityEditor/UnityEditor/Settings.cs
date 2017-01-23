using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityEditor
{
	internal class Settings
	{
		private static List<IPrefType> m_AddedPrefs = new List<IPrefType>();

		private static SortedList<string, object> m_Prefs = new SortedList<string, object>();

		internal static void Add(IPrefType value)
		{
			Settings.m_AddedPrefs.Add(value);
		}

		internal static T Get<T>(string name, T defaultValue) where T : IPrefType, new()
		{
			Settings.Load();
			if (defaultValue == null)
			{
				throw new ArgumentException("default can not be null", "defaultValue");
			}
			T result;
			if (Settings.m_Prefs.ContainsKey(name))
			{
				result = (T)((object)Settings.m_Prefs[name]);
			}
			else
			{
				string @string = EditorPrefs.GetString(name, "");
				if (@string == "")
				{
					Settings.Set<T>(name, defaultValue);
					result = defaultValue;
				}
				else
				{
					defaultValue.FromUniqueString(@string);
					Settings.Set<T>(name, defaultValue);
					result = defaultValue;
				}
			}
			return result;
		}

		internal static void Set<T>(string name, T value) where T : IPrefType
		{
			Settings.Load();
			EditorPrefs.SetString(name, value.ToUniqueString());
			Settings.m_Prefs[name] = value;
		}

		[DebuggerHidden]
		internal static IEnumerable<KeyValuePair<string, T>> Prefs<T>() where T : IPrefType
		{
			Settings.<Prefs>c__Iterator0<T> <Prefs>c__Iterator = new Settings.<Prefs>c__Iterator0<T>();
			Settings.<Prefs>c__Iterator0<T> expr_07 = <Prefs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}

		private static void Load()
		{
			if (Settings.m_AddedPrefs.Any<IPrefType>())
			{
				List<IPrefType> list = new List<IPrefType>(Settings.m_AddedPrefs);
				Settings.m_AddedPrefs.Clear();
				foreach (IPrefType current in list)
				{
					current.Load();
				}
			}
		}
	}
}
