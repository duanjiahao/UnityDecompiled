using System;
using UnityEditor;

internal static class DesktopStandaloneSettings
{
	private static readonly string kSettingCopyPDBFiles = "CopyPDBFiles";

	internal static string PlatformName
	{
		get
		{
			return "Standalone";
		}
	}

	internal static bool CopyPDBFiles
	{
		get
		{
			return EditorUserBuildSettings.GetPlatformSettings(DesktopStandaloneSettings.PlatformName, DesktopStandaloneSettings.kSettingCopyPDBFiles).ToLower() == "true";
		}
		set
		{
			EditorUserBuildSettings.SetPlatformSettings(DesktopStandaloneSettings.PlatformName, DesktopStandaloneSettings.kSettingCopyPDBFiles, value.ToString().ToLower());
		}
	}
}
