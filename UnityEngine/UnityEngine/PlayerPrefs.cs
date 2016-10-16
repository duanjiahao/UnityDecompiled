using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class PlayerPrefs
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetInt(string key, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetFloat(string key, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TrySetSetString(string key, string value);

		public static void SetInt(string key, int value)
		{
			if (!PlayerPrefs.TrySetInt(key, value))
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);

		[ExcludeFromDocs]
		public static int GetInt(string key)
		{
			int defaultValue = 0;
			return PlayerPrefs.GetInt(key, defaultValue);
		}

		public static void SetFloat(string key, float value)
		{
			if (!PlayerPrefs.TrySetFloat(key, value))
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);

		[ExcludeFromDocs]
		public static float GetFloat(string key)
		{
			float defaultValue = 0f;
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		public static void SetString(string key, string value)
		{
			if (!PlayerPrefs.TrySetSetString(key, value))
			{
				throw new PlayerPrefsException("Could not store preference value");
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public static string GetString(string key)
		{
			string empty = string.Empty;
			return PlayerPrefs.GetString(key, empty);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasKey(string key);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteKey(string key);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteAll();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Save();
	}
}
