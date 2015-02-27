using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEditor;
namespace UnityEditorInternal
{
	internal class PostProcessNaclPlayer
	{
		private static bool s_ChromeFailedToLaunch;
		private static string ProgramFilesx86
		{
			get
			{
				if (IntPtr.Size == 8 || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))
				{
					return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
				}
				return Environment.GetEnvironmentVariable("ProgramFiles");
			}
		}
		private static bool IsWindows
		{
			get
			{
				return Environment.OSVersion.Platform != PlatformID.Unix;
			}
		}
		private static string BrowserPath
		{
			get
			{
				if (PostProcessNaclPlayer.IsWindows)
				{
					string text = Path.Combine(PostProcessNaclPlayer.ProgramFilesx86, Path.Combine("Google", Path.Combine("Chrome", Path.Combine("Application", "chrome.exe"))));
					if (File.Exists(text))
					{
						return text;
					}
					string text2 = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe";
					if (File.Exists(text2))
					{
						return text2;
					}
					string text3 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Path.Combine("Google", Path.Combine("Chrome", Path.Combine("Application", "chrome.exe"))));
					if (File.Exists(text3))
					{
						return text3;
					}
					throw new Exception("Cannot find Chrome");
				}
				else
				{
					string text4 = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
					if (File.Exists(text4))
					{
						return text4;
					}
					throw new Exception("Cannot find Chrome");
				}
			}
		}
		internal static void Launch(BuildTarget target, string targetfile)
		{
			targetfile = Path.GetFullPath(targetfile);
			byte[] buffer = (!PostProcessNaclPlayer.IsWindows) ? Encoding.UTF8.GetBytes(targetfile) : Encoding.Unicode.GetBytes(targetfile);
			byte[] array = new SHA256Managed().ComputeHash(buffer);
			string text = string.Empty;
			for (int i = 0; i < 16; i++)
			{
				text += (char)(97 + (array[i] >> 4));
				text += (char)(97 + (array[i] & 15));
			}
			string text2 = string.Concat(new string[]
			{
				"chrome-extension://",
				text,
				"/",
				Path.GetFileName(targetfile),
				"_nacl.html"
			});
			do
			{
				PostProcessNaclPlayer.s_ChromeFailedToLaunch = false;
				Process process = new Process();
				process.StartInfo.FileName = PostProcessNaclPlayer.BrowserPath;
				process.StartInfo.Arguments = string.Concat(new string[]
				{
					" --load-extension=\"",
					targetfile,
					"\" \"",
					text2,
					"\""
				});
				process.StartInfo.UseShellExecute = false;
				process.Start();
				Thread.Sleep(5000);
				PostProcessNaclPlayer.s_ChromeFailedToLaunch = process.HasExited;
				if (PostProcessNaclPlayer.s_ChromeFailedToLaunch)
				{
					if (PostProcessNaclPlayer.IsWindows)
					{
						PostProcessNaclPlayer.s_ChromeFailedToLaunch = EditorUtility.DisplayDialog("Is Google Chrome already running?", "If Chrome is already running, you will only be able to test content which has been installed into Chrome before. If your content is not showing in Chrome, please quit Chrome and try again.", "Try again", "Cancel");
					}
					else
					{
						PostProcessNaclPlayer.s_ChromeFailedToLaunch = EditorUtility.DisplayDialog("Could not start Google Chrome. Is it already running?", "Please quit any running instances of Chrome and try again.", "Try again", "Cancel");
					}
				}
			}
			while (PostProcessNaclPlayer.s_ChromeFailedToLaunch);
		}
		private static void StripPhysicsFiles(string path, bool stripPhysics)
		{
			string[] files = Directory.GetFiles(path);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Contains("_nophysx"))
				{
					if (stripPhysics)
					{
						string text2 = text.Replace("_nophysx", string.Empty);
						File.Delete(text2);
						File.Move(text, text2);
					}
					else
					{
						File.Delete(text);
					}
				}
			}
			string[] directories = Directory.GetDirectories(path);
			string[] array2 = directories;
			for (int j = 0; j < array2.Length; j++)
			{
				string path2 = array2[j];
				PostProcessNaclPlayer.StripPhysicsFiles(path2, stripPhysics);
			}
		}
		internal static void PostProcess(BuildOptions options, string installPath, string downloadWebplayerUrl, int width, int height)
		{
			PostProcessWebPlayer.PostProcess(options, installPath, downloadWebplayerUrl, width, height);
			string fileName = Path.GetFileName(installPath);
			string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.NaCl, BuildOptions.None);
			string text = "unity_nacl_files_3.x.x";
			string text2 = Path.Combine(installPath, text);
			string from = Path.Combine(playbackEngineDirectory, "unity_nacl_files");
			if (Directory.Exists(text2))
			{
				FileUtil.DeleteFileOrDirectory(text2);
			}
			FileUtil.CopyFileOrDirectory(from, text2);
			PostProcessNaclPlayer.StripPhysicsFiles(text2, PlayerSettings.stripPhysics);
			string from2 = Path.Combine(playbackEngineDirectory, "template.html");
			string text3 = Path.Combine(installPath, fileName + "_nacl.html");
			if (File.Exists(text3))
			{
				FileUtil.DeleteFileOrDirectory(text3);
			}
			FileUtil.CopyFileOrDirectory(from2, text3);
			string from3 = Path.Combine(playbackEngineDirectory, "template.json");
			string text4 = Path.Combine(installPath, "manifest.json");
			if (File.Exists(text4))
			{
				FileUtil.DeleteFileOrDirectory(text4);
			}
			FileUtil.CopyFileOrDirectory(from3, text4);
			List<string> list = new List<string>();
			list.Add("%UNITY_WIDTH%");
			list.Add(width.ToString());
			list.Add("%UNITY_HEIGHT%");
			list.Add(height.ToString());
			list.Add("%UNITY_WEB_NAME%");
			list.Add(PlayerSettings.productName);
			list.Add("%UNITY_WEB_PATH%");
			list.Add(fileName + ".unity3d");
			list.Add("%UNITY_WEB_HTML_PATH%");
			list.Add(fileName + "_nacl.html");
			list.Add("%UNITY_NACL_PARAMETERS%");
			list.Add(((options & BuildOptions.Development) == BuildOptions.None) ? string.Empty : "softexceptions=\"1\"");
			list.Add("%UNITY_NACL_ENGINE_PATH%");
			list.Add(text);
			list.Add("%UNITY_BETA_WARNING%");
			list.Add((!InternalEditorUtility.IsUnityBeta()) ? string.Empty : "\r\n\t\t<p style=\"color: #c00; font-size: small; font-style: italic;\">Built with beta version of Unity.</p>");
			string[] templateCustomKeys = PlayerSettings.templateCustomKeys;
			for (int i = 0; i < templateCustomKeys.Length; i++)
			{
				string text5 = templateCustomKeys[i];
				list.Add("%UNITY_CUSTOM_" + text5.ToUpper() + "%");
				list.Add(PlayerSettings.GetTemplateCustomValue(text5));
			}
			FileUtil.ReplaceText(text3, list.ToArray());
			FileUtil.ReplaceText(text4, list.ToArray());
		}
	}
}
