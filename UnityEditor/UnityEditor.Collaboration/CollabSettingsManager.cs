using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor.Collaboration
{
	internal sealed class CollabSettingsManager
	{
		public delegate void SettingStatusChanged(CollabSettingType type, CollabSettingStatus status);

		public static Dictionary<CollabSettingType, CollabSettingsManager.SettingStatusChanged> statusNotifier;

		public static extern bool inProgressEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		static CollabSettingsManager()
		{
			CollabSettingsManager.statusNotifier = new Dictionary<CollabSettingType, CollabSettingsManager.SettingStatusChanged>();
			IEnumerator enumerator = Enum.GetValues(typeof(CollabSettingType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CollabSettingType key = (CollabSettingType)enumerator.Current;
					CollabSettingsManager.statusNotifier[key] = null;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		private static void NotifyStatusListeners(CollabSettingType type, CollabSettingStatus status)
		{
			if (CollabSettingsManager.statusNotifier[type] != null)
			{
				CollabSettingsManager.statusNotifier[type](type, status);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsAvailable(CollabSettingType type);
	}
}
