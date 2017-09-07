using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	internal sealed class EditorAnalytics
	{
		public static bool SendEventServiceInfo(object parameters)
		{
			return EditorAnalytics.SendEvent("serviceInfo", parameters);
		}

		public static bool SendEventShowService(object parameters)
		{
			return EditorAnalytics.SendEvent("showService", parameters);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendEvent(string eventName, object parameters);
	}
}
