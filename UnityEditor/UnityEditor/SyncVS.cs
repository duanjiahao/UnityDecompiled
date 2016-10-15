using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEditor.VisualStudioIntegration;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[InitializeOnLoad]
	internal class SyncVS : AssetPostprocessor
	{
		private class SolutionSynchronizationSettings : DefaultSolutionSynchronizationSettings
		{
			public override int VisualStudioVersion
			{
				get
				{
					string externalScriptEditor = InternalEditorUtility.GetExternalScriptEditor();
					if (SyncVS.InstalledVisualStudios.ContainsKey(UnityEditor.VisualStudioVersion.VisualStudio2008) && externalScriptEditor != string.Empty && SyncVS.PathsAreEquivalent(SyncVS.InstalledVisualStudios[UnityEditor.VisualStudioVersion.VisualStudio2008], externalScriptEditor))
					{
						return 9;
					}
					return 10;
				}
			}

			public override string SolutionTemplate
			{
				get
				{
					return EditorPrefs.GetString("VSSolutionText", base.SolutionTemplate);
				}
			}

			public override string EditorAssemblyPath
			{
				get
				{
					return InternalEditorUtility.GetEditorAssemblyPath();
				}
			}

			public override string EngineAssemblyPath
			{
				get
				{
					return InternalEditorUtility.GetEngineAssemblyPath();
				}
			}

			public override string[] Defines
			{
				get
				{
					return EditorUserBuildSettings.activeScriptCompilationDefines;
				}
			}

			internal static bool IsOSX
			{
				get
				{
					return Environment.OSVersion.Platform == PlatformID.Unix;
				}
			}

			internal static bool IsWindows
			{
				get
				{
					return !SyncVS.SolutionSynchronizationSettings.IsOSX && Path.DirectorySeparatorChar == '\\' && Environment.NewLine == "\r\n";
				}
			}

			public override string GetProjectHeaderTemplate(ScriptingLanguage language)
			{
				return EditorPrefs.GetString("VSProjectHeader", base.GetProjectHeaderTemplate(language));
			}

			public override string GetProjectFooterTemplate(ScriptingLanguage language)
			{
				return EditorPrefs.GetString("VSProjectFooter", base.GetProjectFooterTemplate(language));
			}

			protected override string FrameworksPath()
			{
				return EditorApplication.applicationContentsPath;
			}
		}

		private static bool s_AlreadySyncedThisDomainReload;

		private static readonly SolutionSynchronizer Synchronizer;

		internal static Dictionary<VisualStudioVersion, string> InstalledVisualStudios
		{
			get;
			private set;
		}

		static SyncVS()
		{
			SyncVS.Synchronizer = new SolutionSynchronizer(Directory.GetParent(Application.dataPath).FullName, new SyncVS.SolutionSynchronizationSettings());
			EditorUserBuildSettings.activeBuildTargetChanged = (Action)Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, new Action(SyncVS.SyncVisualStudioProjectIfItAlreadyExists));
			try
			{
				SyncVS.InstalledVisualStudios = (SyncVS.GetInstalledVisualStudios() as Dictionary<VisualStudioVersion, string>);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error detecting Visual Studio installations: {0}{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
				SyncVS.InstalledVisualStudios = new Dictionary<VisualStudioVersion, string>();
			}
			SyncVS.SetVisualStudioAsEditorIfNoEditorWasSet();
			UnityVSSupport.Initialize();
		}

		private static void SetVisualStudioAsEditorIfNoEditorWasSet()
		{
			string @string = EditorPrefs.GetString("kScriptsDefaultApp");
			string text = SyncVS.FindBestVisualStudio();
			if (@string == string.Empty && text != null)
			{
				EditorPrefs.SetString("kScriptsDefaultApp", text);
			}
		}

		public static string FindBestVisualStudio()
		{
			return (from kvp in SyncVS.InstalledVisualStudios
			orderby kvp.Key descending
			select kvp into kvp2
			select kvp2.Value).FirstOrDefault<string>();
		}

		public static bool ProjectExists()
		{
			return SyncVS.Synchronizer.SolutionExists();
		}

		public static void CreateIfDoesntExist()
		{
			if (!SyncVS.Synchronizer.SolutionExists())
			{
				SyncVS.Synchronizer.Sync();
			}
		}

		public static void SyncVisualStudioProjectIfItAlreadyExists()
		{
			if (SyncVS.Synchronizer.SolutionExists())
			{
				SyncVS.Synchronizer.Sync();
			}
		}

		public static void PostprocessSyncProject(string[] importedAssets, string[] addedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			SyncVS.Synchronizer.SyncIfNeeded(addedAssets.Union(deletedAssets.Union(movedAssets.Union(movedFromAssetPaths))));
		}

		[MenuItem("Assets/Open C# Project")]
		private static void SyncAndOpenSolution()
		{
			SyncVS.SyncSolution();
			SyncVS.OpenProjectFileUnlessInBatchMode();
		}

		public static void SyncSolution()
		{
			AssetDatabase.Refresh();
			SyncVS.Synchronizer.Sync();
		}

		public static void SyncIfFirstFileOpenSinceDomainLoad()
		{
			if (SyncVS.s_AlreadySyncedThisDomainReload)
			{
				return;
			}
			SyncVS.s_AlreadySyncedThisDomainReload = true;
			SyncVS.Synchronizer.Sync();
		}

		private static void OpenProjectFileUnlessInBatchMode()
		{
			if (InternalEditorUtility.inBatchMode)
			{
				return;
			}
			InternalEditorUtility.OpenFileAtLineExternal(string.Empty, -1);
		}

		private static IDictionary<VisualStudioVersion, string> GetInstalledVisualStudios()
		{
			Dictionary<VisualStudioVersion, string> dictionary = new Dictionary<VisualStudioVersion, string>();
			if (SyncVS.SolutionSynchronizationSettings.IsWindows)
			{
				using (IEnumerator enumerator = Enum.GetValues(typeof(VisualStudioVersion)).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						VisualStudioVersion visualStudioVersion = (VisualStudioVersion)((int)enumerator.Current);
						try
						{
							string text = Environment.GetEnvironmentVariable(string.Format("VS{0}0COMNTOOLS", (int)visualStudioVersion));
							if (!string.IsNullOrEmpty(text))
							{
								string text2 = Paths.Combine(new string[]
								{
									text,
									"..",
									"IDE",
									"devenv.exe"
								});
								if (File.Exists(text2))
								{
									dictionary[visualStudioVersion] = text2;
									continue;
								}
							}
							text = SyncVS.GetRegistryValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}.0", (int)visualStudioVersion), "InstallDir");
							if (string.IsNullOrEmpty(text))
							{
								text = SyncVS.GetRegistryValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Microsoft\\VisualStudio\\{0}.0", (int)visualStudioVersion), "InstallDir");
							}
							if (!string.IsNullOrEmpty(text))
							{
								string text3 = Paths.Combine(new string[]
								{
									text,
									"devenv.exe"
								});
								if (File.Exists(text3))
								{
									dictionary[visualStudioVersion] = text3;
									continue;
								}
							}
							text = SyncVS.GetRegistryValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}.0\\Debugger", (int)visualStudioVersion), "FEQARuntimeImplDll");
							if (!string.IsNullOrEmpty(text))
							{
								string text4 = SyncVS.DeriveVisualStudioPath(text);
								if (!string.IsNullOrEmpty(text4) && File.Exists(text4))
								{
									dictionary[visualStudioVersion] = SyncVS.DeriveVisualStudioPath(text);
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			return dictionary;
		}

		private static string GetRegistryValue(string path, string key)
		{
			string result;
			try
			{
				result = (Registry.GetValue(path, key, null) as string);
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}

		private static string DeriveVisualStudioPath(string debuggerPath)
		{
			string a = SyncVS.DeriveProgramFilesSentinel();
			string a2 = "Common7";
			bool flag = false;
			string[] array = debuggerPath.Split(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			}, StringSplitOptions.RemoveEmptyEntries);
			string text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				if (!flag && string.Equals(a, text2, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
				}
				else if (flag)
				{
					text = Path.Combine(text, text2);
					if (string.Equals(a2, text2, StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
				}
			}
			return Paths.Combine(new string[]
			{
				text,
				"IDE",
				"devenv.exe"
			});
		}

		private static string DeriveProgramFilesSentinel()
		{
			string text = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Split(new char[]
			{
				Path.DirectorySeparatorChar,
				Path.AltDirectorySeparatorChar
			}).LastOrDefault<string>();
			if (!string.IsNullOrEmpty(text))
			{
				int num = text.LastIndexOf("(x86)");
				if (0 <= num)
				{
					text = text.Remove(num);
				}
				return text.TrimEnd(new char[0]);
			}
			return "Program Files";
		}

		private static bool PathsAreEquivalent(string aPath, string zPath)
		{
			if (aPath == null && zPath == null)
			{
				return true;
			}
			if (string.IsNullOrEmpty(aPath) || string.IsNullOrEmpty(zPath))
			{
				return false;
			}
			aPath = Path.GetFullPath(aPath);
			zPath = Path.GetFullPath(zPath);
			StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
			if (!SyncVS.SolutionSynchronizationSettings.IsOSX && !SyncVS.SolutionSynchronizationSettings.IsWindows)
			{
				comparisonType = StringComparison.Ordinal;
			}
			aPath = aPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			zPath = zPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return string.Equals(aPath, zPath, comparisonType);
		}

		internal static bool CheckVisualStudioVersion(int major, int minor, int build)
		{
			int num = -1;
			int num2 = -1;
			if (major != 11)
			{
				return false;
			}
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\DevDiv\\vc\\Servicing");
			if (registryKey == null)
			{
				return false;
			}
			string[] subKeyNames = registryKey.GetSubKeyNames();
			for (int i = 0; i < subKeyNames.Length; i++)
			{
				string text = subKeyNames[i];
				if (text.StartsWith("11.") && text.Length > 3)
				{
					try
					{
						int num3 = Convert.ToInt32(text.Substring(3));
						if (num3 > num)
						{
							num = num3;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			if (num < 0)
			{
				return false;
			}
			RegistryKey registryKey2 = registryKey.OpenSubKey(string.Format("11.{0}\\RuntimeDebug", num));
			if (registryKey2 == null)
			{
				return false;
			}
			string text2 = registryKey2.GetValue("Version", null) as string;
			if (text2 == null)
			{
				return false;
			}
			string[] array = text2.Split(new char[]
			{
				'.'
			});
			if (array == null || array.Length < 3)
			{
				return false;
			}
			try
			{
				num2 = Convert.ToInt32(array[2]);
			}
			catch (Exception)
			{
				return false;
			}
			return num > minor || (num == minor && num2 >= build);
		}
	}
}
