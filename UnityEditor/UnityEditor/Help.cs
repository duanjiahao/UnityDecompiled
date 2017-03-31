using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class Help
	{
		public static bool HasHelpForObject(UnityEngine.Object obj)
		{
			return Help.HasHelpForObject(obj, true);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasHelpForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);

		internal static string GetNiceHelpNameForObject(UnityEngine.Object obj)
		{
			return Help.GetNiceHelpNameForObject(obj, true);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetNiceHelpNameForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetHelpURLForObject(UnityEngine.Object obj);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ShowHelpForObject(UnityEngine.Object obj);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ShowHelpPage(string page);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BrowseURL(string url);
	}
}
