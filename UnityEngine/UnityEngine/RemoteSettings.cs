using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class RemoteSettings
	{
		public delegate void UpdatedEventHandler();

		public static event RemoteSettings.UpdatedEventHandler Updated
		{
			add
			{
				RemoteSettings.UpdatedEventHandler updatedEventHandler = RemoteSettings.Updated;
				RemoteSettings.UpdatedEventHandler updatedEventHandler2;
				do
				{
					updatedEventHandler2 = updatedEventHandler;
					updatedEventHandler = Interlocked.CompareExchange<RemoteSettings.UpdatedEventHandler>(ref RemoteSettings.Updated, (RemoteSettings.UpdatedEventHandler)Delegate.Combine(updatedEventHandler2, value), updatedEventHandler);
				}
				while (updatedEventHandler != updatedEventHandler2);
			}
			remove
			{
				RemoteSettings.UpdatedEventHandler updatedEventHandler = RemoteSettings.Updated;
				RemoteSettings.UpdatedEventHandler updatedEventHandler2;
				do
				{
					updatedEventHandler2 = updatedEventHandler;
					updatedEventHandler = Interlocked.CompareExchange<RemoteSettings.UpdatedEventHandler>(ref RemoteSettings.Updated, (RemoteSettings.UpdatedEventHandler)Delegate.Remove(updatedEventHandler2, value), updatedEventHandler);
				}
				while (updatedEventHandler != updatedEventHandler2);
			}
		}

		[RequiredByNativeCode]
		public static void CallOnUpdate()
		{
			RemoteSettings.UpdatedEventHandler updated = RemoteSettings.Updated;
			if (updated != null)
			{
				updated();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetInt(string key, [DefaultValue("0")] int defaultValue);

		[ExcludeFromDocs]
		public static int GetInt(string key)
		{
			int defaultValue = 0;
			return RemoteSettings.GetInt(key, defaultValue);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloat(string key, [DefaultValue("0.0F")] float defaultValue);

		[ExcludeFromDocs]
		public static float GetFloat(string key)
		{
			float defaultValue = 0f;
			return RemoteSettings.GetFloat(key, defaultValue);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetString(string key, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public static string GetString(string key)
		{
			string defaultValue = "";
			return RemoteSettings.GetString(key, defaultValue);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetBool(string key, [DefaultValue("false")] bool defaultValue);

		[ExcludeFromDocs]
		public static bool GetBool(string key)
		{
			bool defaultValue = false;
			return RemoteSettings.GetBool(key, defaultValue);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasKey(string key);
	}
}
