using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.Build;
using UnityEditor.BuildReporting;
using UnityEditor.Connect;
using UnityEditor.Modules;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace UnityEditor
{
	public class BuildPlayerWindow : EditorWindow
	{
		private class Styles
		{
			public static readonly GUIContent invalidColorSpaceMessage = EditorGUIUtility.TextContent("In order to build a player go to 'Player Settings...' to resolve the incompatibility between the Color Space and the current settings.");

			public GUIStyle selected = "OL SelectedRow";

			public GUIStyle box = "OL Box";

			public GUIStyle title = EditorStyles.boldLabel;

			public GUIStyle evenRow = "CN EntryBackEven";

			public GUIStyle oddRow = "CN EntryBackOdd";

			public GUIStyle platformSelector = "PlayerSettingsPlatform";

			public GUIStyle toggle = "Toggle";

			public GUIStyle levelString = "PlayerSettingsLevel";

			public GUIStyle levelStringCounter = new GUIStyle("Label");

			public Vector2 toggleSize;

			public GUIContent noSessionDialogText = EditorGUIUtility.TextContent("In order to publish your build to UDN, you need to sign in via the AssetStore and tick the 'Stay signed in' checkbox.");

			public GUIContent platformTitle = EditorGUIUtility.TextContent("Platform|Which platform to build for");

			public GUIContent switchPlatform = EditorGUIUtility.TextContent("Switch Platform");

			public GUIContent build = EditorGUIUtility.TextContent("Build");

			public GUIContent export = EditorGUIUtility.TextContent("Export");

			public GUIContent buildAndRun = EditorGUIUtility.TextContent("Build And Run");

			public GUIContent scenesInBuild = EditorGUIUtility.TextContent("Scenes In Build|Which scenes to include in the build");

			public Texture2D activePlatformIcon = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image as Texture2D;

			public const float kButtonWidth = 110f;

			private const string kShopURL = "https://store.unity3d.com/shop/";

			private const string kDownloadURL = "http://unity3d.com/unity/download/";

			private const string kMailURL = "http://unity3d.com/company/sales?type=sales";

			public GUIContent[,] notLicensedMessages;

			private GUIContent[,] buildTargetNotInstalled;

			public GUIContent debugBuild;

			public GUIContent profileBuild;

			public GUIContent allowDebugging;

			public GUIContent symlinkiOSLibraries;

			public GUIContent explicitNullChecks;

			public GUIContent explicitDivideByZeroChecks;

			public GUIContent enableHeadlessMode;

			public GUIContent buildScriptsOnly;

			public GUIContent learnAboutUnityCloudBuild;

			public Styles()
			{
				GUIContent[,] expr_123 = new GUIContent[15, 3];
				expr_123[0, 0] = EditorGUIUtility.TextContent("Your license does not cover Standalone Publishing.");
				expr_123[0, 1] = new GUIContent("");
				expr_123[0, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[1, 0] = EditorGUIUtility.TextContent("Your license does not cover iOS Publishing.");
				expr_123[1, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[1, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[2, 0] = EditorGUIUtility.TextContent("Your license does not cover Apple TV Publishing.");
				expr_123[2, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[2, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[3, 0] = EditorGUIUtility.TextContent("Your license does not cover Android Publishing.");
				expr_123[3, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[3, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[4, 0] = EditorGUIUtility.TextContent("Your license does not cover Tizen Publishing.");
				expr_123[4, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[4, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[5, 0] = EditorGUIUtility.TextContent("Your license does not cover Xbox One Publishing.");
				expr_123[5, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_123[5, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_123[6, 0] = EditorGUIUtility.TextContent("Your license does not cover PS Vita Publishing.");
				expr_123[6, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_123[6, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_123[7, 0] = EditorGUIUtility.TextContent("Your license does not cover PS4 Publishing.");
				expr_123[7, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_123[7, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_123[8, 0] = EditorGUIUtility.TextContent("Your license does not cover Wii U Publishing.");
				expr_123[8, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_123[8, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_123[9, 0] = EditorGUIUtility.TextContent("Your license does not cover Universal Windows Platform Publishing.");
				expr_123[9, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[9, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[10, 0] = EditorGUIUtility.TextContent("Your license does not cover Windows Phone 8 Publishing.");
				expr_123[10, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[10, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[11, 0] = EditorGUIUtility.TextContent("Your license does not cover SamsungTV Publishing");
				expr_123[11, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[11, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[12, 0] = EditorGUIUtility.TextContent("Your license does not cover Nintendo 3DS Publishing");
				expr_123[12, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_123[12, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_123[13, 0] = EditorGUIUtility.TextContent("Your license does not cover Facebook Publishing");
				expr_123[13, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_123[13, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_123[14, 0] = EditorGUIUtility.TextContent("Your license does not cover Nintendo Switch Publishing");
				expr_123[14, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_123[14, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				this.notLicensedMessages = expr_123;
				GUIContent[,] expr_46D = new GUIContent[15, 3];
				expr_46D[0, 0] = EditorGUIUtility.TextContent("Standalone Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[0, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[1, 0] = EditorGUIUtility.TextContent("iOS Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[1, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[2, 0] = EditorGUIUtility.TextContent("Apple TV Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[2, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[3, 0] = EditorGUIUtility.TextContent("Android Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[3, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[4, 0] = EditorGUIUtility.TextContent("Tizen is not supported in this build.\nDownload a build that supports it.");
				expr_46D[4, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[5, 0] = EditorGUIUtility.TextContent("Xbox One Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[5, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[6, 0] = EditorGUIUtility.TextContent("PS Vita Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[6, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[7, 0] = EditorGUIUtility.TextContent("PS4 Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[7, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[8, 0] = EditorGUIUtility.TextContent("Wii U Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[8, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[9, 0] = EditorGUIUtility.TextContent("Universal Windows Platform Player is not supported in\nthis build.\n\nDownload a build that supports it.");
				expr_46D[9, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[10, 0] = EditorGUIUtility.TextContent("Windows Phone 8 Player is not supported\nin this build.\n\nDownload a build that supports it.");
				expr_46D[10, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[11, 0] = EditorGUIUtility.TextContent("SamsungTV Player is not supported in this build.\nDownload a build that supports it.");
				expr_46D[11, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[12, 0] = EditorGUIUtility.TextContent("Nintendo 3DS is not supported in this build.\nDownload a build that supports it.");
				expr_46D[12, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[13, 0] = EditorGUIUtility.TextContent("Facebook is not supported in this build.\nDownload a build that supports it.");
				expr_46D[13, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_46D[14, 0] = EditorGUIUtility.TextContent("Nintendo Switch is not supported in this build.\nDownload a build that supports it.");
				expr_46D[14, 2] = new GUIContent("http://unity3d.com/unity/download/");
				this.buildTargetNotInstalled = expr_46D;
				this.debugBuild = EditorGUIUtility.TextContent("Development Build");
				this.profileBuild = EditorGUIUtility.TextContent("Autoconnect Profiler");
				this.allowDebugging = EditorGUIUtility.TextContent("Script Debugging");
				this.symlinkiOSLibraries = EditorGUIUtility.TextContent("Symlink Unity libraries");
				this.explicitNullChecks = EditorGUIUtility.TextContent("Explicit Null Checks");
				this.explicitDivideByZeroChecks = EditorGUIUtility.TextContent("Divide By Zero Checks");
				this.enableHeadlessMode = EditorGUIUtility.TextContent("Headless Mode");
				this.buildScriptsOnly = EditorGUIUtility.TextContent("Scripts Only Build");
				this.learnAboutUnityCloudBuild = EditorGUIUtility.TextContent("Learn about Unity Cloud Build");
				base..ctor();
				this.levelStringCounter.alignment = TextAnchor.MiddleRight;
				if (Unsupported.IsDeveloperBuild() && (this.buildTargetNotInstalled.GetLength(0) != this.notLicensedMessages.GetLength(0) || this.buildTargetNotInstalled.GetLength(0) != BuildPlatforms.instance.buildPlatforms.Length))
				{
					Debug.LogErrorFormat("Build platforms and messages are desynced in BuildPlayerWindow! ({0} vs. {1} vs. {2}) DON'T SHIP THIS!", new object[]
					{
						this.buildTargetNotInstalled.GetLength(0),
						this.notLicensedMessages.GetLength(0),
						BuildPlatforms.instance.buildPlatforms.Length
					});
				}
			}

			public GUIContent GetTargetNotInstalled(int index, int item)
			{
				if (index >= this.buildTargetNotInstalled.GetLength(0))
				{
					index = 0;
				}
				return this.buildTargetNotInstalled[index, item];
			}

			public GUIContent GetDownloadErrorForTarget(BuildTarget target)
			{
				return null;
			}
		}

		public class BuildMethodException : Exception
		{
			public BuildMethodException() : base("")
			{
			}

			public BuildMethodException(string message) : base(message)
			{
			}
		}

		public static class DefaultBuildMethods
		{
			public static void BuildPlayer(BuildPlayerOptions options)
			{
				if (!UnityConnect.instance.canBuildWithUPID)
				{
					if (!EditorUtility.DisplayDialog("Missing Project ID", "Because you are not a member of this project this build will not access Unity services.\nDo you want to continue?", "Yes", "No"))
					{
						throw new BuildPlayerWindow.BuildMethodException();
					}
				}
				if (!BuildPipeline.IsBuildTargetSupported(options.targetGroup, options.target))
				{
					throw new BuildPlayerWindow.BuildMethodException("Build target is not supported.");
				}
				string targetStringFrom = ModuleManager.GetTargetStringFrom(EditorUserBuildSettings.selectedBuildTargetGroup, options.target);
				IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(targetStringFrom);
				if (buildWindowExtension != null && (options.options & BuildOptions.AutoRunPlayer) != BuildOptions.None && !buildWindowExtension.EnabledBuildAndRunButton())
				{
					throw new BuildPlayerWindow.BuildMethodException();
				}
				if (Unsupported.IsBleedingEdgeBuild())
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine("This version of Unity is a BleedingEdge build that has not seen any manual testing.");
					stringBuilder.AppendLine("You should consider this build unstable.");
					stringBuilder.AppendLine("We strongly recommend that you use a normal version of Unity instead.");
					if (EditorUtility.DisplayDialog("BleedingEdge Build", stringBuilder.ToString(), "Cancel", "OK"))
					{
						throw new BuildPlayerWindow.BuildMethodException();
					}
				}
				bool delayToAfterScriptReload = false;
				if (EditorUserBuildSettings.activeBuildTarget != options.target || EditorUserBuildSettings.activeBuildTargetGroup != options.targetGroup)
				{
					if (!EditorUserBuildSettings.SwitchActiveBuildTargetAsync(options.targetGroup, options.target))
					{
						string message = string.Format("Could not switch to build target '{0}', '{1}'.", BuildPipeline.GetBuildTargetGroupDisplayName(options.targetGroup), BuildPlatforms.instance.GetBuildTargetDisplayName(options.target));
						throw new BuildPlayerWindow.BuildMethodException(message);
					}
					if (EditorApplication.isCompiling)
					{
						delayToAfterScriptReload = true;
					}
				}
				BuildReport buildReport = BuildPipeline.BuildPlayerInternalNoCheck(options.scenes, options.locationPathName, null, options.targetGroup, options.target, options.options, delayToAfterScriptReload);
				if (buildReport != null && buildReport.totalErrors > 0)
				{
					throw new BuildPlayerWindow.BuildMethodException("Build failed with errors.");
				}
			}

			public static BuildPlayerOptions GetBuildPlayerOptions(BuildPlayerOptions defaultBuildPlayerOptions)
			{
				return BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptionsInternal(true, defaultBuildPlayerOptions);
			}

			internal static BuildPlayerOptions GetBuildPlayerOptionsInternal(bool askForBuildLocation, BuildPlayerOptions defaultBuildPlayerOptions)
			{
				BuildPlayerOptions result = defaultBuildPlayerOptions;
				bool flag = false;
				BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
				BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
				bool flag2 = EditorUserBuildSettings.installInBuildFolder && PostprocessBuildPlayer.SupportsInstallInBuildFolder(selectedBuildTargetGroup, buildTarget) && (Unsupported.IsDeveloperBuild() || BuildPlayerWindow.DefaultBuildMethods.IsMetroPlayer(buildTarget));
				if (buildTarget == BuildTarget.Android)
				{
					result.options |= BuildOptions.CompressWithLz4;
				}
				bool development = EditorUserBuildSettings.development;
				if (development)
				{
					result.options |= BuildOptions.Development;
				}
				if (EditorUserBuildSettings.allowDebugging && development)
				{
					result.options |= BuildOptions.AllowDebugging;
				}
				if (EditorUserBuildSettings.symlinkLibraries)
				{
					result.options |= BuildOptions.SymlinkLibraries;
				}
				if (buildTarget == BuildTarget.Android)
				{
					if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
					{
						result.options |= BuildOptions.AcceptExternalModificationsToPlayer;
					}
				}
				if (EditorUserBuildSettings.enableHeadlessMode)
				{
					result.options |= BuildOptions.EnableHeadlessMode;
				}
				if (EditorUserBuildSettings.connectProfiler && (development || buildTarget == BuildTarget.WSAPlayer))
				{
					result.options |= BuildOptions.ConnectWithProfiler;
				}
				if (EditorUserBuildSettings.buildScriptsOnly)
				{
					result.options |= BuildOptions.BuildScriptsOnly;
				}
				if (flag2)
				{
					result.options |= BuildOptions.InstallInBuildFolder;
				}
				if (!flag2)
				{
					if (askForBuildLocation && !BuildPlayerWindow.DefaultBuildMethods.PickBuildLocation(selectedBuildTargetGroup, buildTarget, result.options, out flag))
					{
						throw new BuildPlayerWindow.BuildMethodException();
					}
					string buildLocation = EditorUserBuildSettings.GetBuildLocation(buildTarget);
					if (buildLocation.Length == 0)
					{
						throw new BuildPlayerWindow.BuildMethodException("Build location for buildTarget " + buildTarget.ToString() + "is not valid.");
					}
					if (!askForBuildLocation)
					{
						CanAppendBuild canAppendBuild = InternalEditorUtility.BuildCanBeAppended(buildTarget, buildLocation);
						if (canAppendBuild != CanAppendBuild.Unsupported)
						{
							if (canAppendBuild != CanAppendBuild.Yes)
							{
								if (canAppendBuild == CanAppendBuild.No)
								{
									if (!BuildPlayerWindow.DefaultBuildMethods.PickBuildLocation(selectedBuildTargetGroup, buildTarget, result.options, out flag))
									{
										throw new BuildPlayerWindow.BuildMethodException();
									}
									buildLocation = EditorUserBuildSettings.GetBuildLocation(buildTarget);
									if (!BuildPlayerWindow.BuildLocationIsValid(buildLocation))
									{
										throw new BuildPlayerWindow.BuildMethodException("Build location for buildTarget " + buildTarget.ToString() + "is not valid.");
									}
								}
							}
							else
							{
								flag = true;
							}
						}
					}
				}
				if (flag)
				{
					result.options |= BuildOptions.AcceptExternalModificationsToPlayer;
				}
				result.target = buildTarget;
				result.targetGroup = selectedBuildTargetGroup;
				result.locationPathName = EditorUserBuildSettings.GetBuildLocation(buildTarget);
				result.assetBundleManifestPath = null;
				ArrayList arrayList = new ArrayList();
				EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
				EditorBuildSettingsScene[] array = scenes;
				for (int i = 0; i < array.Length; i++)
				{
					EditorBuildSettingsScene editorBuildSettingsScene = array[i];
					if (editorBuildSettingsScene.enabled)
					{
						arrayList.Add(editorBuildSettingsScene.path);
					}
				}
				result.scenes = (arrayList.ToArray(typeof(string)) as string[]);
				return result;
			}

			private static bool PickBuildLocation(BuildTargetGroup targetGroup, BuildTarget target, BuildOptions options, out bool updateExistingBuild)
			{
				updateExistingBuild = false;
				string buildLocation = EditorUserBuildSettings.GetBuildLocation(target);
				bool result;
				if (target == BuildTarget.Android && EditorUserBuildSettings.exportAsGoogleAndroidProject)
				{
					string title = "Export Google Android Project";
					string location = EditorUtility.SaveFolderPanel(title, buildLocation, "");
					EditorUserBuildSettings.SetBuildLocation(target, location);
					result = true;
				}
				else
				{
					string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(targetGroup, target, options);
					string directory = FileUtil.DeleteLastPathNameComponent(buildLocation);
					string lastPathNameComponent = FileUtil.GetLastPathNameComponent(buildLocation);
					string title2 = "Build " + BuildPlatforms.instance.GetBuildTargetDisplayName(target);
					string text = EditorUtility.SaveBuildPanel(target, title2, directory, lastPathNameComponent, extensionForBuildTarget, out updateExistingBuild);
					if (text == string.Empty)
					{
						result = false;
					}
					else
					{
						if (extensionForBuildTarget != string.Empty && FileUtil.GetPathExtension(text).ToLower() != extensionForBuildTarget)
						{
							text = text + '.' + extensionForBuildTarget;
						}
						string lastPathNameComponent2 = FileUtil.GetLastPathNameComponent(text);
						if (lastPathNameComponent2 == string.Empty)
						{
							result = false;
						}
						else
						{
							string path = (!(extensionForBuildTarget != string.Empty)) ? text : FileUtil.DeleteLastPathNameComponent(text);
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
							if (target == BuildTarget.iOS && Application.platform != RuntimePlatform.OSXEditor && !BuildPlayerWindow.DefaultBuildMethods.FolderIsEmpty(text) && !BuildPlayerWindow.DefaultBuildMethods.UserWantsToDeleteFiles(text))
							{
								result = false;
							}
							else
							{
								EditorUserBuildSettings.SetBuildLocation(target, text);
								result = true;
							}
						}
					}
				}
				return result;
			}

			private static bool FolderIsEmpty(string path)
			{
				return !Directory.Exists(path) || (Directory.GetDirectories(path).Length == 0 && Directory.GetFiles(path).Length == 0);
			}

			private static bool UserWantsToDeleteFiles(string path)
			{
				string message = "WARNING: all files and folders located in target folder: '" + path + "' will be deleted by build process.";
				return EditorUtility.DisplayDialog("Deleting existing files", message, "OK", "Cancel");
			}

			private static bool IsMetroPlayer(BuildTarget target)
			{
				return target == BuildTarget.WSAPlayer;
			}
		}

		private class PublishStyles
		{
			public const int kIconSize = 32;

			public const int kRowHeight = 36;

			public GUIContent xiaomiIcon = EditorGUIUtility.IconContent("BuildSettings.Xiaomi");

			public GUIContent learnAboutXiaomiInstallation = EditorGUIUtility.TextContent("Installation and Setup");

			public GUIContent publishTitle = EditorGUIUtility.TextContent("SDKs for App Stores|Integrations with 3rd party app stores");
		}

		private ListViewState lv = new ListViewState();

		private bool[] selectedLVItems = new bool[0];

		private bool[] selectedBeforeDrag;

		private int initialSelectedLVItem = -1;

		private Vector2 scrollPosition = new Vector2(0f, 0f);

		private const string kAssetsFolder = "Assets/";

		private const string kEditorBuildSettingsPath = "ProjectSettings/EditorBuildSettings.asset";

		private static BuildPlayerWindow.Styles styles = null;

		private static Regex s_VersionPattern = new Regex("(?<shortVersion>\\d+\\.\\d+\\.\\d+(?<suffix>((?<alphabeta>[abx])|[fp])[^\\s]*))( \\((?<revision>[a-fA-F\\d]+)\\))?", RegexOptions.Compiled);

		private static Dictionary<string, string> s_ModuleNames = new Dictionary<string, string>
		{
			{
				"SamsungTV",
				"Samsung-TV"
			},
			{
				"tvOS",
				"AppleTV"
			},
			{
				"OSXStandalone",
				"Mac"
			},
			{
				"WindowsStandalone",
				"Windows"
			},
			{
				"LinuxStandalone",
				"Linux"
			},
			{
				"Facebook",
				"Facebook-Games"
			}
		};

		private static Func<BuildPlayerOptions, BuildPlayerOptions> getBuildPlayerOptionsHandler;

		private static Action<BuildPlayerOptions> buildPlayerHandler;

		private BuildPlayerWindow.PublishStyles publishStyles = null;

		public BuildPlayerWindow()
		{
			base.position = new Rect(50f, 50f, 540f, 530f);
			base.minSize = new Vector2(630f, 580f);
			base.titleContent = new GUIContent("Build Settings");
		}

		public static void ShowBuildPlayerWindow()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = EditorUserBuildSettings.activeBuildTargetGroup;
			EditorWindow.GetWindow<BuildPlayerWindow>(true, "Build Settings");
		}

		private static bool BuildLocationIsValid(string path)
		{
			return path.Length > 0 && Directory.Exists(FileUtil.DeleteLastPathNameComponent(path));
		}

		private static void BuildPlayerAndRun()
		{
			BuildTarget target = BuildPlayerWindow.CalculateSelectedBuildTarget();
			string buildLocation = EditorUserBuildSettings.GetBuildLocation(target);
			BuildPlayerWindow.BuildPlayerAndRun(!BuildPlayerWindow.BuildLocationIsValid(buildLocation));
		}

		private static void BuildPlayerAndRun(bool askForBuildLocation)
		{
			BuildPlayerWindow.CallBuildMethods(askForBuildLocation, BuildOptions.AutoRunPlayer | BuildOptions.StrictMode);
		}

		private void ActiveScenesGUI()
		{
			int num = 0;
			int row = this.lv.row;
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			Event current = Event.current;
			Rect rect = GUILayoutUtility.GetRect(BuildPlayerWindow.styles.scenesInBuild, BuildPlayerWindow.styles.title);
			List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			this.lv.totalRows = list.Count;
			if (this.selectedLVItems.Length != list.Count)
			{
				Array.Resize<bool>(ref this.selectedLVItems, list.Count);
			}
			int[] array = new int[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				EditorBuildSettingsScene editorBuildSettingsScene = list[i];
				array[i] = num;
				if (editorBuildSettingsScene.enabled)
				{
					num++;
				}
			}
			IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, (ListViewOptions)3, BuildPlayerWindow.styles.box, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					EditorBuildSettingsScene editorBuildSettingsScene2 = list[listViewElement.row];
					bool flag = File.Exists(editorBuildSettingsScene2.path);
					using (new EditorGUI.DisabledScope(!flag))
					{
						bool flag2 = this.selectedLVItems[listViewElement.row];
						if (flag2 && current.type == EventType.Repaint)
						{
							BuildPlayerWindow.styles.selected.Draw(listViewElement.position, false, false, false, false);
						}
						if (!flag)
						{
							editorBuildSettingsScene2.enabled = false;
						}
						Rect position = new Rect(listViewElement.position.x + 4f, listViewElement.position.y, BuildPlayerWindow.styles.toggleSize.x, BuildPlayerWindow.styles.toggleSize.y);
						EditorGUI.BeginChangeCheck();
						editorBuildSettingsScene2.enabled = GUI.Toggle(position, editorBuildSettingsScene2.enabled, "");
						if (EditorGUI.EndChangeCheck() && flag2)
						{
							for (int j = 0; j < list.Count; j++)
							{
								if (this.selectedLVItems[j])
								{
									list[j].enabled = editorBuildSettingsScene2.enabled;
								}
							}
						}
						GUILayout.Space(BuildPlayerWindow.styles.toggleSize.x);
						string text = editorBuildSettingsScene2.path;
						if (text.StartsWith("Assets/"))
						{
							text = text.Substring("Assets/".Length);
						}
						if (text.EndsWith(".unity", StringComparison.InvariantCultureIgnoreCase))
						{
							text = text.Substring(0, text.Length - ".unity".Length);
						}
						Rect rect2 = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(text), BuildPlayerWindow.styles.levelString);
						if (Event.current.type == EventType.Repaint)
						{
							BuildPlayerWindow.styles.levelString.Draw(rect2, EditorGUIUtility.TempContent(text), false, false, flag2, false);
						}
						GUILayout.Label((!editorBuildSettingsScene2.enabled) ? "" : array[listViewElement.row].ToString(), BuildPlayerWindow.styles.levelStringCounter, new GUILayoutOption[]
						{
							GUILayout.MaxWidth(36f)
						});
					}
					if (ListViewGUILayout.HasMouseUp(listViewElement.position) && !shift && !actionKey)
					{
						if (!shift && !actionKey)
						{
							ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
						}
					}
					else if (ListViewGUILayout.HasMouseDown(listViewElement.position))
					{
						if (!this.selectedLVItems[listViewElement.row] || shift || actionKey)
						{
							ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
						}
						this.lv.row = listViewElement.row;
						this.selectedBeforeDrag = new bool[this.selectedLVItems.Length];
						this.selectedLVItems.CopyTo(this.selectedBeforeDrag, 0);
						this.selectedBeforeDrag[this.lv.row] = true;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			GUI.Label(rect, BuildPlayerWindow.styles.scenesInBuild, BuildPlayerWindow.styles.title);
			if (GUIUtility.keyboardControl == this.lv.ID)
			{
				if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
				{
					Event.current.Use();
				}
				else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
				{
					for (int i = 0; i < this.selectedLVItems.Length; i++)
					{
						this.selectedLVItems[i] = true;
					}
					this.lv.selectionChanged = true;
					Event.current.Use();
					GUIUtility.ExitGUI();
				}
			}
			if (this.lv.selectionChanged)
			{
				ListViewGUILayout.MultiSelection(row, this.lv.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
			}
			if (this.lv.fileNames != null)
			{
				Array.Sort<string>(this.lv.fileNames);
				int num2 = 0;
				int i = 0;
				while (i < this.lv.fileNames.Length)
				{
					if (this.lv.fileNames[i].EndsWith("unity", StringComparison.InvariantCultureIgnoreCase))
					{
						string scenePath = FileUtil.GetProjectRelativePath(this.lv.fileNames[i]);
						if (scenePath == string.Empty)
						{
							scenePath = this.lv.fileNames[i];
						}
						if (!list.Any((EditorBuildSettingsScene s) => s.path == scenePath))
						{
							GUID gUID;
							GUID.TryParse(AssetDatabase.AssetPathToGUID(scenePath), out gUID);
							EditorBuildSettingsScene item = (!(gUID == default(GUID))) ? new EditorBuildSettingsScene(gUID, true) : new EditorBuildSettingsScene(scenePath, true);
							list.Insert(this.lv.draggedTo + num2++, item);
						}
					}
					IL_676:
					i++;
					continue;
					goto IL_676;
				}
				if (num2 != 0)
				{
					Array.Resize<bool>(ref this.selectedLVItems, list.Count);
					for (i = 0; i < this.selectedLVItems.Length; i++)
					{
						this.selectedLVItems[i] = (i >= this.lv.draggedTo && i < this.lv.draggedTo + num2);
					}
				}
				this.lv.draggedTo = -1;
			}
			if (this.lv.draggedTo != -1)
			{
				List<EditorBuildSettingsScene> list2 = new List<EditorBuildSettingsScene>();
				int num3 = 0;
				int i = 0;
				while (i < this.selectedLVItems.Length)
				{
					if (this.selectedBeforeDrag[i])
					{
						list2.Add(list[num3]);
						list.RemoveAt(num3);
						num3--;
						if (this.lv.draggedTo >= i)
						{
							this.lv.draggedTo--;
						}
					}
					i++;
					num3++;
				}
				this.lv.draggedTo = ((this.lv.draggedTo <= list.Count && this.lv.draggedTo >= 0) ? this.lv.draggedTo : list.Count);
				list.InsertRange(this.lv.draggedTo, list2);
				for (i = 0; i < this.selectedLVItems.Length; i++)
				{
					this.selectedLVItems[i] = (i >= this.lv.draggedTo && i < this.lv.draggedTo + list2.Count);
				}
			}
			if (current.type == EventType.KeyDown && (current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete) && GUIUtility.keyboardControl == this.lv.ID)
			{
				int num3 = 0;
				int i = 0;
				while (i < this.selectedLVItems.Length)
				{
					if (this.selectedLVItems[i])
					{
						list.RemoveAt(num3);
						num3--;
					}
					this.selectedLVItems[i] = false;
					i++;
					num3++;
				}
				this.lv.row = 0;
				current.Use();
			}
			EditorBuildSettings.scenes = list.ToArray();
		}

		private void AddOpenScenes()
		{
			List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
			bool flag = false;
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene scene = SceneManager.GetSceneAt(i);
				if (scene.path.Length != 0 || EditorSceneManager.SaveScene(scene, "", false))
				{
					if (!list.Any((EditorBuildSettingsScene s) => s.path == scene.path))
					{
						GUID gUID;
						GUID.TryParse(scene.guid, out gUID);
						EditorBuildSettingsScene item = (!(gUID == default(GUID))) ? new EditorBuildSettingsScene(gUID, true) : new EditorBuildSettingsScene(scene.path, true);
						list.Add(item);
						flag = true;
					}
				}
			}
			if (flag)
			{
				EditorBuildSettings.scenes = list.ToArray();
				base.Repaint();
				GUIUtility.ExitGUI();
			}
		}

		private static BuildTarget CalculateSelectedBuildTarget()
		{
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			BuildTarget result;
			if (selectedBuildTargetGroup != BuildTargetGroup.Standalone)
			{
				if (selectedBuildTargetGroup != BuildTargetGroup.Facebook)
				{
					if (BuildPlatforms.instance == null)
					{
						throw new Exception("Build platforms are not initialized.");
					}
					BuildPlatform buildPlatform = BuildPlatforms.instance.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
					if (buildPlatform == null)
					{
						throw new Exception("Could not find build platform for target group " + selectedBuildTargetGroup);
					}
					result = buildPlatform.defaultTarget;
				}
				else
				{
					result = EditorUserBuildSettings.selectedFacebookTarget;
				}
			}
			else
			{
				result = EditorUserBuildSettings.selectedStandaloneTarget;
			}
			return result;
		}

		private void ActiveBuildTargetsGUI()
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.Width(255f)
			});
			GUILayout.Label(BuildPlayerWindow.styles.platformTitle, BuildPlayerWindow.styles.title, new GUILayoutOption[0]);
			this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, "OL Box");
			for (int i = 0; i < 2; i++)
			{
				bool flag = i == 0;
				bool flag2 = false;
				BuildPlatform[] buildPlatforms = BuildPlatforms.instance.buildPlatforms;
				for (int j = 0; j < buildPlatforms.Length; j++)
				{
					BuildPlatform buildPlatform = buildPlatforms[j];
					if (BuildPlayerWindow.IsBuildTargetGroupSupported(buildPlatform.targetGroup, buildPlatform.defaultTarget) == flag)
					{
						if (BuildPlayerWindow.IsBuildTargetGroupSupported(buildPlatform.targetGroup, buildPlatform.defaultTarget) || buildPlatform.forceShowTarget)
						{
							if (BuildPipeline.IsBuildTargetCompatibleWithOS(buildPlatform.defaultTarget))
							{
								this.ShowOption(buildPlatform, buildPlatform.title, (!flag2) ? BuildPlayerWindow.styles.oddRow : BuildPlayerWindow.styles.evenRow);
								flag2 = !flag2;
							}
						}
					}
				}
				GUI.contentColor = Color.white;
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			BuildTarget target = BuildPlayerWindow.CalculateSelectedBuildTarget();
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = (BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, target) && EditorUserBuildSettings.activeBuildTargetGroup != selectedBuildTargetGroup);
			if (GUILayout.Button(BuildPlayerWindow.styles.switchPlatform, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				EditorUserBuildSettings.SwitchActiveBuildTargetAsync(selectedBuildTargetGroup, target);
				GUIUtility.ExitGUI();
			}
			GUI.enabled = BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, target);
			if (GUILayout.Button(new GUIContent("Player Settings..."), new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
				EditorWindow.GetWindow<InspectorWindow>();
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			GUILayout.EndVertical();
		}

		private void ShowAlert()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			EditorGUILayout.HelpBox("Because you are not a member of this project this build will not access Unity services.", MessageType.Warning);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
		}

		private void ShowOption(BuildPlatform bp, GUIContent title, GUIStyle background)
		{
			Rect rect = GUILayoutUtility.GetRect(50f, 36f);
			rect.x += 1f;
			rect.y += 1f;
			bool flag = BuildPipeline.LicenseCheck(bp.defaultTarget);
			GUI.contentColor = new Color(1f, 1f, 1f, (!flag) ? 0.7f : 1f);
			bool flag2 = EditorUserBuildSettings.selectedBuildTargetGroup == bp.targetGroup;
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(rect, GUIContent.none, false, false, flag2, false);
				GUI.Label(new Rect(rect.x + 3f, rect.y + 3f, 32f, 32f), title.image, GUIStyle.none);
				if (EditorUserBuildSettings.activeBuildTargetGroup == bp.targetGroup)
				{
					GUI.Label(new Rect(rect.xMax - (float)BuildPlayerWindow.styles.activePlatformIcon.width - 8f, rect.y + 3f + (float)((32 - BuildPlayerWindow.styles.activePlatformIcon.height) / 2), (float)BuildPlayerWindow.styles.activePlatformIcon.width, (float)BuildPlayerWindow.styles.activePlatformIcon.height), BuildPlayerWindow.styles.activePlatformIcon, GUIStyle.none);
				}
			}
			if (GUI.Toggle(rect, flag2, title.text, BuildPlayerWindow.styles.platformSelector))
			{
				if (EditorUserBuildSettings.selectedBuildTargetGroup != bp.targetGroup)
				{
					EditorUserBuildSettings.selectedBuildTargetGroup = bp.targetGroup;
					UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(InspectorWindow));
					for (int i = 0; i < array.Length; i++)
					{
						InspectorWindow inspectorWindow = array[i] as InspectorWindow;
						if (inspectorWindow != null)
						{
							inspectorWindow.Repaint();
						}
					}
				}
			}
		}

		private void OnGUI()
		{
			if (BuildPlayerWindow.styles == null)
			{
				BuildPlayerWindow.styles = new BuildPlayerWindow.Styles();
				BuildPlayerWindow.styles.toggleSize = BuildPlayerWindow.styles.toggle.CalcSize(new GUIContent("X"));
				this.lv.rowHeight = (int)BuildPlayerWindow.styles.levelString.CalcHeight(new GUIContent("X"), 100f);
			}
			if (!UnityConnect.instance.canBuildWithUPID)
			{
				this.ShowAlert();
			}
			GUILayout.Space(5f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			string text = "";
			bool flag = !AssetDatabase.IsOpenForEdit("ProjectSettings/EditorBuildSettings.asset", out text, StatusQueryOptions.UseCachedIfPossible);
			using (new EditorGUI.DisabledScope(flag))
			{
				this.ActiveScenesGUI();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (flag)
				{
					GUI.enabled = true;
					if (Provider.enabled && GUILayout.Button("Check out", new GUILayoutOption[0]))
					{
						Asset assetByPath = Provider.GetAssetByPath("ProjectSettings/EditorBuildSettings.asset");
						Provider.Checkout(new AssetList
						{
							assetByPath
						}, CheckoutMode.Asset);
					}
					GUILayout.Label(text, new GUILayoutOption[0]);
					GUI.enabled = false;
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add Open Scenes", new GUILayoutOption[0]))
				{
					this.AddOpenScenes();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(351f)
			});
			this.ActiveBuildTargetsGUI();
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			this.ShowBuildTargetSettings();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.EndVertical();
			GUILayout.Space(10f);
			GUILayout.EndHorizontal();
		}

		internal static bool IsBuildTargetGroupSupported(BuildTargetGroup targetGroup, BuildTarget target)
		{
			return targetGroup == BuildTargetGroup.Standalone || BuildPipeline.IsBuildTargetSupported(targetGroup, target);
		}

		private static void RepairSelectedBuildTargetGroup()
		{
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			if (selectedBuildTargetGroup == BuildTargetGroup.Unknown || BuildPlatforms.instance.BuildPlatformIndexFromTargetGroup(selectedBuildTargetGroup) < 0)
			{
				EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
			}
		}

		private static bool IsAnyStandaloneModuleLoaded()
		{
			return ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows));
		}

		private static bool IsColorSpaceValid(BuildPlatform platform)
		{
			bool result;
			if (PlayerSettings.colorSpace == ColorSpace.Linear)
			{
				bool flag = true;
				bool flag2 = true;
				if (platform.targetGroup == BuildTargetGroup.iPhone)
				{
					GraphicsDeviceType[] graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
					flag = (!graphicsAPIs.Contains(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs.Contains(GraphicsDeviceType.OpenGLES2));
					Version v = new Version(8, 0);
					Version version = new Version(6, 0);
					Version v2 = (!string.IsNullOrEmpty(PlayerSettings.iOS.targetOSVersionString)) ? new Version(PlayerSettings.iOS.targetOSVersionString) : version;
					flag2 = (v2 >= v);
				}
				else if (platform.targetGroup == BuildTargetGroup.tvOS)
				{
					GraphicsDeviceType[] graphicsAPIs2 = PlayerSettings.GetGraphicsAPIs(BuildTarget.tvOS);
					flag = (!graphicsAPIs2.Contains(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs2.Contains(GraphicsDeviceType.OpenGLES2));
				}
				else if (platform.targetGroup == BuildTargetGroup.Android)
				{
					GraphicsDeviceType[] graphicsAPIs3 = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
					flag = ((graphicsAPIs3.Contains(GraphicsDeviceType.Vulkan) || graphicsAPIs3.Contains(GraphicsDeviceType.OpenGLES3)) && !graphicsAPIs3.Contains(GraphicsDeviceType.OpenGLES2));
					flag2 = (PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel18);
				}
				result = (flag && flag2);
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static string GetPlaybackEngineDownloadURL(string moduleName)
		{
			string result;
			if (moduleName == "PS4" || moduleName == "PSP2" || moduleName == "XboxOne")
			{
				result = "https://unity3d.com/platform-installation";
			}
			else
			{
				string fullUnityVersion = InternalEditorUtility.GetFullUnityVersion();
				string text = "";
				string text2 = "";
				Match match = BuildPlayerWindow.s_VersionPattern.Match(fullUnityVersion);
				if (!match.Success || !match.Groups["shortVersion"].Success || !match.Groups["suffix"].Success)
				{
					Debug.LogWarningFormat("Error parsing version '{0}'", new object[]
					{
						fullUnityVersion
					});
				}
				if (match.Groups["shortVersion"].Success)
				{
					text2 = match.Groups["shortVersion"].Value;
				}
				if (match.Groups["revision"].Success)
				{
					text = match.Groups["revision"].Value;
				}
				if (BuildPlayerWindow.s_ModuleNames.ContainsKey(moduleName))
				{
					moduleName = BuildPlayerWindow.s_ModuleNames[moduleName];
				}
				string text3 = "download";
				string text4 = "download_unity";
				string text5 = "Unknown";
				string text6 = string.Empty;
				if (match.Groups["alphabeta"].Success)
				{
					text3 = "beta";
					text4 = "download";
				}
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					text5 = "TargetSupportInstaller";
					text6 = ".exe";
				}
				else if (Application.platform == RuntimePlatform.OSXEditor)
				{
					text5 = "MacEditorTargetInstaller";
					text6 = ".pkg";
				}
				result = string.Format("http://{0}.unity3d.com/{1}/{2}/{3}/UnitySetup-{4}-Support-for-Editor-{5}{6}", new object[]
				{
					text3,
					text4,
					text,
					text5,
					moduleName,
					text2,
					text6
				});
			}
			return result;
		}

		private bool IsModuleInstalled(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
		{
			bool flag = BuildPipeline.LicenseCheck(buildTarget);
			string targetStringFrom = ModuleManager.GetTargetStringFrom(buildTargetGroup, buildTarget);
			return flag && !string.IsNullOrEmpty(targetStringFrom) && ModuleManager.GetBuildPostProcessor(targetStringFrom) == null && (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone || !BuildPlayerWindow.IsAnyStandaloneModuleLoaded());
		}

		private void ShowBuildTargetSettings()
		{
			EditorGUIUtility.labelWidth = Mathf.Min(180f, (base.position.width - 265f) * 0.47f);
			BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			BuildPlatform buildPlatform = BuildPlatforms.instance.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
			bool flag = BuildPipeline.LicenseCheck(buildTarget);
			GUILayout.Space(18f);
			Rect rect = GUILayoutUtility.GetRect(50f, 36f);
			rect.x += 1f;
			GUI.Label(new Rect(rect.x + 3f, rect.y + 3f, 32f, 32f), buildPlatform.title.image, GUIStyle.none);
			GUI.Toggle(rect, false, buildPlatform.title.text, BuildPlayerWindow.styles.platformSelector);
			GUILayout.Space(10f);
			if (buildPlatform.targetGroup == BuildTargetGroup.WebGL && !BuildPipeline.IsBuildTargetSupported(buildPlatform.targetGroup, buildTarget))
			{
				if (IntPtr.Size == 4)
				{
					GUILayout.Label("Building for WebGL requires a 64-bit Unity editor.", new GUILayoutOption[0]);
					BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
					return;
				}
			}
			string targetStringFrom = ModuleManager.GetTargetStringFrom(selectedBuildTargetGroup, buildTarget);
			if (this.IsModuleInstalled(selectedBuildTargetGroup, buildTarget))
			{
				GUILayout.Label("No " + BuildPlatforms.instance.GetModuleDisplayName(selectedBuildTargetGroup, buildTarget) + " module loaded.", new GUILayoutOption[0]);
				if (GUILayout.Button("Open Download Page", EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					string playbackEngineDownloadURL = BuildPlayerWindow.GetPlaybackEngineDownloadURL(targetStringFrom);
					Help.BrowseURL(playbackEngineDownloadURL);
				}
				BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
			}
			else
			{
				if (Application.HasProLicense() && !InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(buildTarget))
				{
					string text = string.Format("{0} is not included in your Unity Pro license. Your {0} build will include a Unity Personal Edition splash screen.\n\nYou must be eligible to use Unity Personal Edition to use this build option. Please refer to our EULA for further information.", BuildPlatforms.instance.GetBuildTargetDisplayName(buildTarget));
					GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
					GUILayout.Label(text, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button("EULA", EditorStyles.miniButton, new GUILayoutOption[0]))
					{
						Application.OpenURL("http://unity3d.com/legal/eula");
					}
					if (GUILayout.Button(string.Format("Add {0} to your Unity Pro license", BuildPlatforms.instance.GetBuildTargetDisplayName(buildTarget)), EditorStyles.miniButton, new GUILayoutOption[0]))
					{
						Application.OpenURL("http://unity3d.com/get-unity");
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				}
				GUIContent downloadErrorForTarget = BuildPlayerWindow.styles.GetDownloadErrorForTarget(buildTarget);
				if (downloadErrorForTarget != null)
				{
					GUILayout.Label(downloadErrorForTarget, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
					BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
				}
				else if (!flag)
				{
					int num = BuildPlatforms.instance.BuildPlatformIndexFromTargetGroup(buildPlatform.targetGroup);
					GUILayout.Label(BuildPlayerWindow.styles.notLicensedMessages[num, 0], EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
					GUILayout.Space(5f);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					if (BuildPlayerWindow.styles.notLicensedMessages[num, 1].text.Length != 0)
					{
						if (GUILayout.Button(BuildPlayerWindow.styles.notLicensedMessages[num, 1], new GUILayoutOption[0]))
						{
							Application.OpenURL(BuildPlayerWindow.styles.notLicensedMessages[num, 2].text);
						}
					}
					GUILayout.EndHorizontal();
					BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
				}
				else
				{
					string targetStringFrom2 = ModuleManager.GetTargetStringFrom(buildPlatform.targetGroup, buildTarget);
					IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(targetStringFrom2);
					if (buildWindowExtension != null)
					{
						buildWindowExtension.ShowPlatformBuildOptions();
					}
					GUI.changed = false;
					BuildTargetGroup targetGroup = buildPlatform.targetGroup;
					if (targetGroup == BuildTargetGroup.iPhone || targetGroup == BuildTargetGroup.tvOS)
					{
						if (Application.platform == RuntimePlatform.OSXEditor)
						{
							EditorUserBuildSettings.symlinkLibraries = EditorGUILayout.Toggle(BuildPlayerWindow.styles.symlinkiOSLibraries, EditorUserBuildSettings.symlinkLibraries, new GUILayoutOption[0]);
						}
					}
					GUI.enabled = true;
					bool flag2 = buildWindowExtension == null || buildWindowExtension.EnabledBuildButton();
					bool enableBuildAndRunButton = false;
					bool flag3 = buildWindowExtension == null || buildWindowExtension.ShouldDrawScriptDebuggingCheckbox();
					bool flag4 = buildWindowExtension != null && buildWindowExtension.ShouldDrawExplicitNullCheckbox();
					bool flag5 = buildWindowExtension != null && buildWindowExtension.ShouldDrawExplicitDivideByZeroCheckbox();
					bool flag6 = buildWindowExtension == null || buildWindowExtension.ShouldDrawDevelopmentPlayerCheckbox();
					bool flag7 = buildTarget == BuildTarget.StandaloneLinux || buildTarget == BuildTarget.StandaloneLinux64 || buildTarget == BuildTarget.StandaloneLinuxUniversal;
					IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(selectedBuildTargetGroup, buildTarget);
					bool flag8 = buildPostProcessor != null && buildPostProcessor.SupportsScriptsOnlyBuild();
					bool canInstallInBuildFolder = false;
					if (BuildPipeline.IsBuildTargetSupported(selectedBuildTargetGroup, buildTarget))
					{
						bool flag9 = buildWindowExtension == null || buildWindowExtension.ShouldDrawProfilerCheckbox();
						GUI.enabled = flag6;
						if (flag6)
						{
							EditorUserBuildSettings.development = EditorGUILayout.Toggle(BuildPlayerWindow.styles.debugBuild, EditorUserBuildSettings.development, new GUILayoutOption[0]);
						}
						bool development = EditorUserBuildSettings.development;
						GUI.enabled = development;
						if (flag9)
						{
							if (!GUI.enabled)
							{
								if (!development)
								{
									BuildPlayerWindow.styles.profileBuild.tooltip = "Profiling only enabled in Development Player";
								}
							}
							else
							{
								BuildPlayerWindow.styles.profileBuild.tooltip = "";
							}
							EditorUserBuildSettings.connectProfiler = EditorGUILayout.Toggle(BuildPlayerWindow.styles.profileBuild, EditorUserBuildSettings.connectProfiler, new GUILayoutOption[0]);
						}
						GUI.enabled = development;
						if (flag3)
						{
							EditorUserBuildSettings.allowDebugging = EditorGUILayout.Toggle(BuildPlayerWindow.styles.allowDebugging, EditorUserBuildSettings.allowDebugging, new GUILayoutOption[0]);
						}
						if (flag4)
						{
							GUI.enabled = !development;
							if (!GUI.enabled)
							{
								EditorUserBuildSettings.explicitNullChecks = true;
							}
							EditorUserBuildSettings.explicitNullChecks = EditorGUILayout.Toggle(BuildPlayerWindow.styles.explicitNullChecks, EditorUserBuildSettings.explicitNullChecks, new GUILayoutOption[0]);
							GUI.enabled = development;
						}
						if (flag5)
						{
							GUI.enabled = !development;
							if (!GUI.enabled)
							{
								EditorUserBuildSettings.explicitDivideByZeroChecks = true;
							}
							EditorUserBuildSettings.explicitDivideByZeroChecks = EditorGUILayout.Toggle(BuildPlayerWindow.styles.explicitDivideByZeroChecks, EditorUserBuildSettings.explicitDivideByZeroChecks, new GUILayoutOption[0]);
							GUI.enabled = development;
						}
						if (flag8)
						{
							EditorUserBuildSettings.buildScriptsOnly = EditorGUILayout.Toggle(BuildPlayerWindow.styles.buildScriptsOnly, EditorUserBuildSettings.buildScriptsOnly, new GUILayoutOption[0]);
						}
						GUI.enabled = !development;
						if (flag7)
						{
							EditorUserBuildSettings.enableHeadlessMode = EditorGUILayout.Toggle(BuildPlayerWindow.styles.enableHeadlessMode, EditorUserBuildSettings.enableHeadlessMode && !development, new GUILayoutOption[0]);
						}
						GUI.enabled = true;
						GUILayout.FlexibleSpace();
						canInstallInBuildFolder = (Unsupported.IsDeveloperBuild() && PostprocessBuildPlayer.SupportsInstallInBuildFolder(selectedBuildTargetGroup, buildTarget));
						if (flag2)
						{
							enableBuildAndRunButton = ((buildWindowExtension == null) ? (!EditorUserBuildSettings.installInBuildFolder) : (buildWindowExtension.EnabledBuildAndRunButton() && !EditorUserBuildSettings.installInBuildFolder));
						}
					}
					else
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(true)
						});
						GUILayout.BeginVertical(new GUILayoutOption[]
						{
							GUILayout.ExpandWidth(true)
						});
						int index = BuildPlatforms.instance.BuildPlatformIndexFromTargetGroup(buildPlatform.targetGroup);
						GUILayout.Label(BuildPlayerWindow.styles.GetTargetNotInstalled(index, 0), new GUILayoutOption[0]);
						if (BuildPlayerWindow.styles.GetTargetNotInstalled(index, 1) != null && GUILayout.Button(BuildPlayerWindow.styles.GetTargetNotInstalled(index, 1), new GUILayoutOption[0]))
						{
							Application.OpenURL(BuildPlayerWindow.styles.GetTargetNotInstalled(index, 2).text);
						}
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
					}
					if (buildTarget == BuildTarget.Android)
					{
						this.AndroidPublishGUI();
					}
					BuildPlayerWindow.GUIBuildButtons(buildWindowExtension, flag2, enableBuildAndRunButton, canInstallInBuildFolder, buildPlatform);
				}
			}
		}

		private static void GUIBuildButtons(bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlatform platform)
		{
			BuildPlayerWindow.GUIBuildButtons(null, enableBuildButton, enableBuildAndRunButton, canInstallInBuildFolder, platform);
		}

		private static void GUIBuildButtons(IBuildWindowExtension buildWindowExtension, bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlatform platform)
		{
			GUILayout.FlexibleSpace();
			if (canInstallInBuildFolder)
			{
				EditorUserBuildSettings.installInBuildFolder = GUILayout.Toggle(EditorUserBuildSettings.installInBuildFolder, "Install in Builds folder\n(for debugging with source code)", new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				});
			}
			else
			{
				EditorUserBuildSettings.installInBuildFolder = false;
			}
			if (buildWindowExtension != null && Unsupported.IsDeveloperBuild())
			{
				buildWindowExtension.ShowInternalPlatformBuildOptions();
			}
			if (!BuildPlayerWindow.IsColorSpaceValid(platform) && enableBuildButton && enableBuildAndRunButton)
			{
				enableBuildAndRunButton = false;
				enableBuildButton = false;
				EditorGUILayout.HelpBox(BuildPlayerWindow.Styles.invalidColorSpaceMessage.text, MessageType.Warning);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (EditorGUILayout.LinkLabel(BuildPlayerWindow.styles.learnAboutUnityCloudBuild, new GUILayoutOption[0]))
			{
				Application.OpenURL(string.Format("{0}/from/editor/buildsettings?upid={1}&pid={2}&currentplatform={3}&selectedplatform={4}&unityversion={5}", new object[]
				{
					WebURLs.cloudBuildPage,
					PlayerSettings.cloudProjectId,
					PlayerSettings.productGUID,
					EditorUserBuildSettings.activeBuildTarget,
					BuildPlayerWindow.CalculateSelectedBuildTarget(),
					Application.unityVersion
				}));
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(6f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUIContent content = BuildPlayerWindow.styles.build;
			if (platform.targetGroup == BuildTargetGroup.Android && EditorUserBuildSettings.exportAsGoogleAndroidProject)
			{
				content = BuildPlayerWindow.styles.export;
			}
			if (platform.targetGroup == BuildTargetGroup.iPhone && Application.platform != RuntimePlatform.OSXEditor)
			{
				enableBuildAndRunButton = false;
			}
			GUI.enabled = enableBuildButton;
			if (GUILayout.Button(content, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				BuildPlayerWindow.CallBuildMethods(true, BuildOptions.ShowBuiltPlayer);
				GUIUtility.ExitGUI();
			}
			GUI.enabled = enableBuildAndRunButton;
			if (GUILayout.Button(BuildPlayerWindow.styles.buildAndRun, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				BuildPlayerWindow.BuildPlayerAndRun(true);
				GUIUtility.ExitGUI();
			}
			GUILayout.EndHorizontal();
		}

		public static void RegisterGetBuildPlayerOptionsHandler(Func<BuildPlayerOptions, BuildPlayerOptions> func)
		{
			if (func != null && BuildPlayerWindow.getBuildPlayerOptionsHandler != null)
			{
				Debug.LogWarning("The get build player options handler in BuildPlayerWindow is being reassigned!");
			}
			BuildPlayerWindow.getBuildPlayerOptionsHandler = func;
		}

		public static void RegisterBuildPlayerHandler(Action<BuildPlayerOptions> func)
		{
			if (func != null && BuildPlayerWindow.buildPlayerHandler != null)
			{
				Debug.LogWarning("The build player handler in BuildPlayerWindow is being reassigned!");
			}
			BuildPlayerWindow.buildPlayerHandler = func;
		}

		private static void CallBuildMethods(bool askForBuildLocation, BuildOptions defaultBuildOptions)
		{
			try
			{
				BuildPlayerOptions buildPlayerOptions = default(BuildPlayerOptions);
				buildPlayerOptions.options = defaultBuildOptions;
				if (BuildPlayerWindow.getBuildPlayerOptionsHandler != null)
				{
					buildPlayerOptions = BuildPlayerWindow.getBuildPlayerOptionsHandler(buildPlayerOptions);
				}
				else
				{
					buildPlayerOptions = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptionsInternal(askForBuildLocation, buildPlayerOptions);
				}
				if (BuildPlayerWindow.buildPlayerHandler != null)
				{
					BuildPlayerWindow.buildPlayerHandler(buildPlayerOptions);
				}
				else
				{
					BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
				}
			}
			catch (BuildPlayerWindow.BuildMethodException ex)
			{
				if (!string.IsNullOrEmpty(ex.Message))
				{
					Debug.LogError(ex);
				}
			}
		}

		private void AndroidPublishGUI()
		{
			if (this.publishStyles == null)
			{
				this.publishStyles = new BuildPlayerWindow.PublishStyles();
			}
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label(this.publishStyles.publishTitle, BuildPlayerWindow.styles.title, new GUILayoutOption[0]);
			using (new EditorGUILayout.HorizontalScope(BuildPlayerWindow.styles.box, new GUILayoutOption[]
			{
				GUILayout.Height(36f)
			}))
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(3f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Space(4f);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(2f);
				GUILayout.Label(this.publishStyles.xiaomiIcon, new GUILayoutOption[]
				{
					GUILayout.Width(32f),
					GUILayout.Height(32f)
				});
				GUILayout.EndVertical();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label("Xiaomi Mi Game Center", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (EditorGUILayout.LinkLabel(this.publishStyles.learnAboutXiaomiInstallation, new GUILayoutOption[0]))
				{
					Application.OpenURL("http://unity3d.com/partners/xiaomi/guide");
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.Space(4f);
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			GUILayout.EndVertical();
		}
	}
}
