using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.VisualStudioIntegration
{
	internal class UnityVSSupport
	{
		private static bool m_ShouldUnityVSBeActive;

		public static string s_UnityVSBridgeToLoad;

		private static bool? s_IsUnityVSEnabled;

		private static string s_AboutLabel;

		public static void Initialize()
		{
			UnityVSSupport.Initialize(null);
		}

		public static void Initialize(string editorPath)
		{
			string externalEditor = editorPath ?? ScriptEditorUtility.GetExternalScriptEditor();
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				UnityVSSupport.InitializeVSForMac(externalEditor);
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				UnityVSSupport.InitializeVisualStudio(externalEditor);
			}
		}

		private static void InitializeVSForMac(string externalEditor)
		{
			Version vsfmVersion;
			if (UnityVSSupport.IsVSForMac(externalEditor, out vsfmVersion))
			{
				UnityVSSupport.m_ShouldUnityVSBeActive = true;
				string vSForMacBridgeAssembly = UnityVSSupport.GetVSForMacBridgeAssembly(externalEditor, vsfmVersion);
				if (string.IsNullOrEmpty(vSForMacBridgeAssembly) || !File.Exists(vSForMacBridgeAssembly))
				{
					Console.WriteLine("Unable to find Tools for Unity bridge dll for Visual Studio for Mac " + externalEditor);
				}
				else
				{
					UnityVSSupport.s_UnityVSBridgeToLoad = vSForMacBridgeAssembly;
					InternalEditorUtility.RegisterPrecompiledAssembly(Path.GetFileNameWithoutExtension(vSForMacBridgeAssembly), vSForMacBridgeAssembly);
				}
			}
		}

		private static bool IsVSForMac(string externalEditor, out Version vsfmVersion)
		{
			vsfmVersion = null;
			bool result;
			if (!externalEditor.ToLower().EndsWith("visual studio.app"))
			{
				result = false;
			}
			else
			{
				try
				{
					result = UnityVSSupport.GetVSForMacVersion(externalEditor, out vsfmVersion);
				}
				catch (Exception arg)
				{
					Console.WriteLine("Failed to read Visual Studio for Mac information: {0}", arg);
					result = false;
				}
			}
			return result;
		}

		private static bool GetVSForMacVersion(string externalEditor, out Version vsfmVersion)
		{
			vsfmVersion = null;
			string path = Path.Combine(externalEditor, "Contents/Info.plist");
			bool result;
			if (!File.Exists(path))
			{
				result = false;
			}
			else
			{
				string input = File.ReadAllText(path);
				Match match = Regex.Match(input, "\\<key\\>CFBundleShortVersionString\\</key\\>\\s+\\<string\\>(?<version>\\d+\\.\\d+\\.\\d+\\.\\d+?)\\</string\\>");
				Group group = match.Groups["version"];
				if (!group.Success)
				{
					result = false;
				}
				else
				{
					vsfmVersion = new Version(group.Value);
					result = true;
				}
			}
			return result;
		}

		private static string GetVSForMacBridgeAssembly(string externalEditor, Version vsfmVersion)
		{
			string text = Environment.GetEnvironmentVariable("VSTUM_BRIDGE");
			string result;
			if (!string.IsNullOrEmpty(text) && File.Exists(text))
			{
				result = text;
			}
			else
			{
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Application Support/VisualStudio/" + vsfmVersion.Major + ".0/LocalInstall/Addins");
				if (Directory.Exists(path))
				{
					string[] directories = Directory.GetDirectories(path, "MonoDevelop.Unity*", SearchOption.TopDirectoryOnly);
					for (int i = 0; i < directories.Length; i++)
					{
						string path2 = directories[i];
						text = Path.Combine(path2, "Editor/SyntaxTree.VisualStudio.Unity.Bridge.dll");
						if (File.Exists(text))
						{
							result = text;
							return result;
						}
					}
				}
				text = Path.Combine(externalEditor, "Contents/Resources/lib/monodevelop/AddIns/MonoDevelop.Unity/Editor/SyntaxTree.VisualStudio.Unity.Bridge.dll");
				if (File.Exists(text))
				{
					result = text;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static void InitializeVisualStudio(string externalEditor)
		{
			if (externalEditor.EndsWith("UnityVS.OpenFile.exe"))
			{
				externalEditor = SyncVS.FindBestVisualStudio();
				if (externalEditor != null)
				{
					ScriptEditorUtility.SetExternalScriptEditor(externalEditor);
				}
			}
			VisualStudioVersion version;
			if (UnityVSSupport.IsVisualStudio(externalEditor, out version))
			{
				UnityVSSupport.m_ShouldUnityVSBeActive = true;
				string vstuBridgeAssembly = UnityVSSupport.GetVstuBridgeAssembly(version);
				if (vstuBridgeAssembly == null)
				{
					Console.WriteLine("Unable to find bridge dll in registry for Microsoft Visual Studio Tools for Unity for " + externalEditor);
				}
				else if (!File.Exists(vstuBridgeAssembly))
				{
					Console.WriteLine("Unable to find bridge dll on disk for Microsoft Visual Studio Tools for Unity for " + vstuBridgeAssembly);
				}
				else
				{
					UnityVSSupport.s_UnityVSBridgeToLoad = vstuBridgeAssembly;
					InternalEditorUtility.RegisterPrecompiledAssembly(Path.GetFileNameWithoutExtension(vstuBridgeAssembly), vstuBridgeAssembly);
				}
			}
		}

		private static bool IsVisualStudio(string externalEditor, out VisualStudioVersion vsVersion)
		{
			bool result;
			if (string.IsNullOrEmpty(externalEditor))
			{
				vsVersion = VisualStudioVersion.Invalid;
				result = false;
			}
			else
			{
				KeyValuePair<VisualStudioVersion, VisualStudioPath[]>[] array = (from kvp in SyncVS.InstalledVisualStudios
				where kvp.Value.Any((VisualStudioPath v) => Paths.AreEqual(v.Path, externalEditor, true))
				select kvp).ToArray<KeyValuePair<VisualStudioVersion, VisualStudioPath[]>>();
				if (array.Length > 0)
				{
					vsVersion = array[0].Key;
					result = true;
				}
				else
				{
					if (externalEditor.EndsWith("devenv.exe", StringComparison.OrdinalIgnoreCase))
					{
						if (UnityVSSupport.TryGetVisualStudioVersion(externalEditor, out vsVersion))
						{
							result = true;
							return result;
						}
					}
					vsVersion = VisualStudioVersion.Invalid;
					result = false;
				}
			}
			return result;
		}

		private static bool TryGetVisualStudioVersion(string externalEditor, out VisualStudioVersion vsVersion)
		{
			bool result;
			switch (UnityVSSupport.ProductVersion(externalEditor).Major)
			{
			case 9:
				vsVersion = VisualStudioVersion.VisualStudio2008;
				result = true;
				return result;
			case 10:
				vsVersion = VisualStudioVersion.VisualStudio2010;
				result = true;
				return result;
			case 11:
				vsVersion = VisualStudioVersion.VisualStudio2012;
				result = true;
				return result;
			case 12:
				vsVersion = VisualStudioVersion.VisualStudio2013;
				result = true;
				return result;
			case 14:
				vsVersion = VisualStudioVersion.VisualStudio2015;
				result = true;
				return result;
			case 15:
				vsVersion = VisualStudioVersion.VisualStudio2017;
				result = true;
				return result;
			}
			vsVersion = VisualStudioVersion.Invalid;
			result = false;
			return result;
		}

		private static Version ProductVersion(string externalEditor)
		{
			Version result;
			try
			{
				result = new Version(FileVersionInfo.GetVersionInfo(externalEditor).ProductVersion);
			}
			catch (Exception)
			{
				result = new Version(0, 0);
			}
			return result;
		}

		public static bool ShouldUnityVSBeActive()
		{
			return UnityVSSupport.m_ShouldUnityVSBeActive;
		}

		private static string GetAssemblyLocation(Assembly a)
		{
			string result;
			try
			{
				result = a.Location;
			}
			catch (NotSupportedException)
			{
				result = null;
			}
			return result;
		}

		public static bool IsUnityVSEnabled()
		{
			if (!UnityVSSupport.s_IsUnityVSEnabled.HasValue)
			{
				bool arg_49_0;
				if (UnityVSSupport.m_ShouldUnityVSBeActive)
				{
					arg_49_0 = AppDomain.CurrentDomain.GetAssemblies().Any((Assembly a) => UnityVSSupport.GetAssemblyLocation(a) == UnityVSSupport.s_UnityVSBridgeToLoad);
				}
				else
				{
					arg_49_0 = false;
				}
				UnityVSSupport.s_IsUnityVSEnabled = new bool?(arg_49_0);
			}
			return UnityVSSupport.s_IsUnityVSEnabled.Value;
		}

		private static string GetVstuBridgeAssembly(VisualStudioVersion version)
		{
			string result;
			try
			{
				string vsVersion = string.Empty;
				switch (version)
				{
				case VisualStudioVersion.VisualStudio2010:
					vsVersion = "2010";
					break;
				case VisualStudioVersion.VisualStudio2012:
					vsVersion = "2012";
					break;
				case VisualStudioVersion.VisualStudio2013:
					vsVersion = "2013";
					break;
				case VisualStudioVersion.VisualStudio2015:
					vsVersion = "2015";
					break;
				case VisualStudioVersion.VisualStudio2017:
					vsVersion = "15.0";
					break;
				}
				result = (UnityVSSupport.GetVstuBridgePathFromRegistry(vsVersion, true) ?? UnityVSSupport.GetVstuBridgePathFromRegistry(vsVersion, false));
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		private static string GetVstuBridgePathFromRegistry(string vsVersion, bool currentUser)
		{
			string keyName = string.Format("{0}\\Software\\Microsoft\\Microsoft Visual Studio {1} Tools for Unity", (!currentUser) ? "HKEY_LOCAL_MACHINE" : "HKEY_CURRENT_USER", vsVersion);
			return (string)Registry.GetValue(keyName, "UnityExtensionPath", null);
		}

		public static void ScriptEditorChanged(string editorPath)
		{
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
			{
				UnityVSSupport.Initialize(editorPath);
				InternalEditorUtility.RequestScriptReload();
			}
		}

		public static string GetAboutWindowLabel()
		{
			string result;
			if (UnityVSSupport.s_AboutLabel != null)
			{
				result = UnityVSSupport.s_AboutLabel;
			}
			else
			{
				UnityVSSupport.s_AboutLabel = UnityVSSupport.CalculateAboutWindowLabel();
				result = UnityVSSupport.s_AboutLabel;
			}
			return result;
		}

		private static string CalculateAboutWindowLabel()
		{
			string result;
			if (!UnityVSSupport.IsUnityVSEnabled())
			{
				result = "";
			}
			else
			{
				Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly a) => a.Location == UnityVSSupport.s_UnityVSBridgeToLoad);
				if (assembly == null)
				{
					result = "";
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder("Microsoft Visual Studio Tools for Unity ");
					stringBuilder.Append(assembly.GetName().Version);
					stringBuilder.Append(" enabled");
					result = stringBuilder.ToString();
				}
			}
			return result;
		}
	}
}
