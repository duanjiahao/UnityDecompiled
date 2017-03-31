using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Utils
{
	internal class MonoInstallationFinder
	{
		public const string MonoInstallation = "Mono";

		public const string MonoBleedingEdgeInstallation = "MonoBleedingEdge";

		public static string GetFrameWorksFolder()
		{
			string text = FileUtil.NiceWinPath(EditorApplication.applicationPath);
			string result;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				result = Path.Combine(Path.GetDirectoryName(text), "Data");
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				result = Path.Combine(text, "Contents");
			}
			else
			{
				result = Path.Combine(Path.GetDirectoryName(text), "Data");
			}
			return result;
		}

		public static string GetProfileDirectory(string profile)
		{
			string monoInstallation = MonoInstallationFinder.GetMonoInstallation();
			return Path.Combine(monoInstallation, Path.Combine("lib", Path.Combine("mono", profile)));
		}

		public static string GetProfileDirectory(string profile, string monoInstallation)
		{
			string monoInstallation2 = MonoInstallationFinder.GetMonoInstallation(monoInstallation);
			return Path.Combine(monoInstallation2, Path.Combine("lib", Path.Combine("mono", profile)));
		}

		public static string GetProfilesDirectory(string monoInstallation)
		{
			string monoInstallation2 = MonoInstallationFinder.GetMonoInstallation(monoInstallation);
			return Path.Combine(monoInstallation2, Path.Combine("lib", "mono"));
		}

		public static string GetEtcDirectory(string monoInstallation)
		{
			string monoInstallation2 = MonoInstallationFinder.GetMonoInstallation(monoInstallation);
			return Path.Combine(monoInstallation2, Path.Combine("etc", "mono"));
		}

		public static string GetMonoInstallation()
		{
			return MonoInstallationFinder.GetMonoInstallation("Mono");
		}

		public static string GetMonoBleedingEdgeInstallation()
		{
			return MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge");
		}

		public static string GetMonoInstallation(string monoName)
		{
			return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), monoName);
		}
	}
}
