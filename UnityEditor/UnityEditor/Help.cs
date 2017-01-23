using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Help
	{
		public static bool HasHelpForObject(UnityEngine.Object obj)
		{
			return Help.HasHelpForObject(obj, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasHelpForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);

		internal static string GetNiceHelpNameForObject(UnityEngine.Object obj)
		{
			return Help.GetNiceHelpNameForObject(obj, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetNiceHelpNameForObject(UnityEngine.Object obj, bool defaultToMonoBehaviour);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetHelpURLForObject(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ShowHelpForObject(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ShowHelpPage(string page);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BrowseURL(string url);
	}
}
