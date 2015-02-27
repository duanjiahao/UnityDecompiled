using System;
using System.IO;
using UnityEngine;
namespace UnityEditor.Utils
{
	internal class MonoInstallationFinder
	{
		public static string GetFrameWorksFolder()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return Path.GetDirectoryName(EditorApplication.applicationPath) + "/Data/";
			}
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				return EditorApplication.applicationPath + "/Contents/Frameworks/";
			}
			return Path.GetDirectoryName(EditorApplication.applicationPath) + "/Data/";
		}
		public static string GetProfileDirectory(BuildTarget target, string profile)
		{
			string monoInstallation = MonoInstallationFinder.GetMonoInstallation();
			return Path.Combine(monoInstallation, "lib/mono/" + profile);
		}
		public static string GetMonoInstallation()
		{
			return MonoInstallationFinder.GetMonoInstallation("Mono");
		}
		public static string GetMonoInstallation(string monoName)
		{
			return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), monoName);
		}
	}
}
