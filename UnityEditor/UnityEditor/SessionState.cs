using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	internal sealed class SessionState
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetBool(string key, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetBool(string key, bool defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EraseBool(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetFloat(string key, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetFloat(string key, float defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EraseFloat(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetInt(string key, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetInt(string key, int defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EraseInt(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetString(string key, string value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetString(string key, string defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EraseString(string key);

		internal static void SetVector3(string key, Vector3 value)
		{
			SessionState.INTERNAL_CALL_SetVector3(key, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetVector3(string key, ref Vector3 value);

		internal static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			Vector3 result;
			SessionState.INTERNAL_CALL_GetVector3(key, ref defaultValue, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetVector3(string key, ref Vector3 defaultValue, out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EraseVector3(string key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIntArray(string key, int[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int[] GetIntArray(string key, int[] defaultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EraseIntArray(string key);
	}
}
