using Microsoft.Win32;
using System;
using System.IO;
using UnityEngine;
namespace UnityEditor
{
	internal class AndroidSdkRoot
	{
		private static string GuessPerPlatform()
		{
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.OSXEditor)
			{
				if (platform != RuntimePlatform.WindowsEditor)
				{
					return string.Empty;
				}
				string text = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Android SDK Tools", "Path", string.Empty).ToString();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Android SDK Tools", "Path", string.Empty).ToString();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				text = Environment.GetEnvironmentVariable("ProgramFiles");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			else
			{
				string environmentVariable = Environment.GetEnvironmentVariable("HOME");
				if (!string.IsNullOrEmpty(environmentVariable))
				{
					return environmentVariable;
				}
			}
			return string.Empty;
		}
		internal static string Browse(string sdkPath)
		{
			string defaultName = string.Empty;
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				defaultName = "android-sdk-mac_x86";
			}
			string title = "Select Android SDK root folder";
			string text = sdkPath;
			if (string.IsNullOrEmpty(text))
			{
				try
				{
					text = AndroidSdkRoot.GuessPerPlatform();
				}
				catch (Exception value)
				{
					Console.WriteLine("Exception while trying to guess Android SDK location");
					Console.WriteLine(value);
				}
			}
			while (true)
			{
				sdkPath = EditorUtility.OpenFolderPanel(title, text, defaultName);
				if (sdkPath.Length == 0)
				{
					break;
				}
				if (AndroidSdkRoot.IsSdkDir(sdkPath))
				{
					return sdkPath;
				}
			}
			return string.Empty;
		}
		internal static bool IsSdkDir(string path)
		{
			return Directory.Exists(Path.Combine(path, "platform-tools")) || File.Exists(Path.Combine(Path.Combine(path, "tools"), "android")) || File.Exists(Path.Combine(Path.Combine(path, "tools"), "android.bat"));
		}
	}
}
