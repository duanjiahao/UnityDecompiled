using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class LocalizationDatabase
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SystemLanguage GetDefaultEditorLanguage();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetCurrentEditorLanguage(SystemLanguage lang);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SystemLanguage GetCurrentEditorLanguage();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReadEditorLocalizationResources();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SystemLanguage[] GetAvailableEditorLanguages();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetLocalizedString(string original);

		public static string MarkForTranslation(string value)
		{
			return value;
		}
	}
}
