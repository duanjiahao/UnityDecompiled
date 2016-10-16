using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;

namespace UnityEditor
{
	public sealed class EditorPrefs
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetInt(string key, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);

		[ExcludeFromDocs]
		public static int GetInt(string key)
		{
			int defaultValue = 0;
			return EditorPrefs.GetInt(key, defaultValue);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetFloat(string key, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);

		[ExcludeFromDocs]
		public static float GetFloat(string key)
		{
			float defaultValue = 0f;
			return EditorPrefs.GetFloat(key, defaultValue);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetString(string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public static string GetString(string key)
		{
			string empty = string.Empty;
			return EditorPrefs.GetString(key, empty);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBool(string key, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);

		[ExcludeFromDocs]
		public static bool GetBool(string key)
		{
			bool defaultValue = false;
			return EditorPrefs.GetBool(key, defaultValue);
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
	}
}
