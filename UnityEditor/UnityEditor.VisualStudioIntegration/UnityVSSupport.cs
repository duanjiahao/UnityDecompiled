using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				string text = editorPath ?? EditorPrefs.GetString("kScriptsDefaultApp");
				if (text.EndsWith("UnityVS.OpenFile.exe"))
				{
					text = SyncVS.FindBestVisualStudio();
					if (text != null)
					{
						EditorPrefs.SetString("kScriptsDefaultApp", text);
					}
				}
				VisualStudioVersion version;
				if (UnityVSSupport.IsVisualStudio(text, out version))
				{
					UnityVSSupport.m_ShouldUnityVSBeActive = true;
					string vstuBridgeAssembly = UnityVSSupport.GetVstuBridgeAssembly(version);
					if (vstuBridgeAssembly == null)
					{
						Console.WriteLine("Unable to find bridge dll in registry for Microsoft Visual Studio Tools for Unity for " + text);
					}
					else if (!File.Exists(vstuBridgeAssembly))
					{
						Console.WriteLine("Unable to find bridge dll on disk for Microsoft Visual Studio Tools for Unity for " + vstuBridgeAssembly);
					}
					else
					{
						UnityVSSupport.s_UnityVSBridgeToLoad = vstuBridgeAssembly;
						InternalEditorUtility.SetupCustomDll(Path.GetFileNameWithoutExtension(vstuBridgeAssembly), vstuBridgeAssembly);
					}
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
				KeyValuePair<VisualStudioVersion, string>[] array = (from kvp in SyncVS.InstalledVisualStudios
				where Paths.AreEqual(kvp.Value, externalEditor, true)
				select kvp).ToArray<KeyValuePair<VisualStudioVersion, string>>();
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
				vsVersion = VisualStudioVersion.VisualStudio15;
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
				case VisualStudioVersion.VisualStudio15:
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
			if (Application.platform == RuntimePlatform.WindowsEditor)
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
