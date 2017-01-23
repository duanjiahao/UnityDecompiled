using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
	internal class BuildPlayerWindow : EditorWindow
	{
		public class SceneSorter : IComparer
		{
			int IComparer.Compare(object x, object y)
			{
				return new CaseInsensitiveComparer().Compare(y, x);
			}
		}

		public class BuildPlatform
		{
			public string name;

			public GUIContent title;

			public Texture2D smallIcon;

			public BuildTargetGroup targetGroup;

			public bool forceShowTarget;

			public string tooltip;

			public BuildTarget DefaultTarget
			{
				get
				{
					BuildTargetGroup buildTargetGroup = this.targetGroup;
					switch (buildTargetGroup)
					{
					case BuildTargetGroup.WebGL:
					{
						BuildTarget result = BuildTarget.WebGL;
						return result;
					}
					case BuildTargetGroup.WSA:
					{
						BuildTarget result = BuildTarget.WSAPlayer;
						return result;
					}
					case BuildTargetGroup.WP8:
					case BuildTargetGroup.BlackBerry:
					case BuildTargetGroup.PSM:
						IL_45:
						switch (buildTargetGroup)
						{
						case BuildTargetGroup.Standalone:
						{
							BuildTarget result = BuildTarget.StandaloneWindows;
							return result;
						}
						case BuildTargetGroup.WebPlayer:
						case (BuildTargetGroup)3:
						{
							IL_5D:
							BuildTarget result;
							if (buildTargetGroup != BuildTargetGroup.Android)
							{
								result = BuildTarget.iPhone;
								return result;
							}
							result = BuildTarget.Android;
							return result;
						}
						case BuildTargetGroup.iPhone:
						{
							BuildTarget result = BuildTarget.iOS;
							return result;
						}
						}
						goto IL_5D;
					case BuildTargetGroup.Tizen:
					{
						BuildTarget result = BuildTarget.Tizen;
						return result;
					}
					case BuildTargetGroup.PSP2:
					{
						BuildTarget result = BuildTarget.PSP2;
						return result;
					}
					case BuildTargetGroup.PS4:
					{
						BuildTarget result = BuildTarget.PS4;
						return result;
					}
					case BuildTargetGroup.XboxOne:
					{
						BuildTarget result = BuildTarget.XboxOne;
						return result;
					}
					case BuildTargetGroup.SamsungTV:
					{
						BuildTarget result = BuildTarget.SamsungTV;
						return result;
					}
					case BuildTargetGroup.N3DS:
					{
						BuildTarget result = BuildTarget.N3DS;
						return result;
					}
					case BuildTargetGroup.WiiU:
					{
						BuildTarget result = BuildTarget.WiiU;
						return result;
					}
					case BuildTargetGroup.tvOS:
					{
						BuildTarget result = BuildTarget.tvOS;
						return result;
					}
					}
					goto IL_45;
				}
			}

			public BuildPlatform(string locTitle, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget) : this(locTitle, "", iconId, targetGroup, forceShowTarget)
			{
			}

			public BuildPlatform(string locTitle, string tooltip, string iconId, BuildTargetGroup targetGroup, bool forceShowTarget)
			{
				this.targetGroup = targetGroup;
				this.name = ((targetGroup == BuildTargetGroup.Unknown) ? "" : BuildPipeline.GetBuildTargetGroupName(this.DefaultTarget));
				this.title = EditorGUIUtility.TextContentWithIcon(locTitle, iconId);
				this.smallIcon = (EditorGUIUtility.IconContent(iconId + ".Small").image as Texture2D);
				this.tooltip = tooltip;
				this.forceShowTarget = forceShowTarget;
			}
		}

		private class BuildPlatforms
		{
			public BuildPlayerWindow.BuildPlatform[] buildPlatforms;

			internal BuildPlatforms()
			{
				List<BuildPlayerWindow.BuildPlatform> list = new List<BuildPlayerWindow.BuildPlatform>();
				list.Add(new BuildPlayerWindow.BuildPlatform("PC, Mac & Linux Standalone", "BuildSettings.Standalone", BuildTargetGroup.Standalone, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("iOS", "BuildSettings.iPhone", BuildTargetGroup.iPhone, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("tvOS", "BuildSettings.tvOS", BuildTargetGroup.tvOS, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("Android", "BuildSettings.Android", BuildTargetGroup.Android, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("Tizen", "BuildSettings.Tizen", BuildTargetGroup.Tizen, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("Xbox One", "BuildSettings.XboxOne", BuildTargetGroup.XboxOne, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("PS Vita", "BuildSettings.PSP2", BuildTargetGroup.PSP2, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("PS4", "BuildSettings.PS4", BuildTargetGroup.PS4, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("Wii U", "BuildSettings.WiiU", BuildTargetGroup.WiiU, false));
				list.Add(new BuildPlayerWindow.BuildPlatform("Windows Store", "BuildSettings.Metro", BuildTargetGroup.WSA, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("WebGL", "BuildSettings.WebGL", BuildTargetGroup.WebGL, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("Samsung TV", "BuildSettings.SamsungTV", BuildTargetGroup.SamsungTV, true));
				list.Add(new BuildPlayerWindow.BuildPlatform("Nintendo 3DS", "BuildSettings.N3DS", BuildTargetGroup.N3DS, false));
				foreach (BuildPlayerWindow.BuildPlatform current in list)
				{
					current.tooltip = BuildPipeline.GetBuildTargetGroupDisplayName(current.targetGroup) + " settings";
				}
				this.buildPlatforms = list.ToArray();
			}

			public string GetBuildTargetDisplayName(BuildTarget target)
			{
				BuildPlayerWindow.BuildPlatform[] array = this.buildPlatforms;
				string result;
				for (int i = 0; i < array.Length; i++)
				{
					BuildPlayerWindow.BuildPlatform buildPlatform = array[i];
					if (buildPlatform.DefaultTarget == target)
					{
						result = buildPlatform.title.text;
						return result;
					}
				}
				switch (target)
				{
				case BuildTarget.StandaloneOSXUniversal:
				case BuildTarget.StandaloneOSXIntel:
					goto IL_96;
				case (BuildTarget)3:
					IL_58:
					switch (target)
					{
					case BuildTarget.StandaloneLinux64:
					case BuildTarget.StandaloneLinuxUniversal:
						goto IL_A1;
					case BuildTarget.WP8Player:
						IL_71:
						switch (target)
						{
						case BuildTarget.StandaloneLinux:
							goto IL_A1;
						case BuildTarget.StandaloneWindows64:
							goto IL_8B;
						}
						result = "Unsupported Target";
						return result;
					case BuildTarget.StandaloneOSXIntel64:
						goto IL_96;
					}
					goto IL_71;
					IL_A1:
					result = "Linux";
					return result;
				case BuildTarget.StandaloneWindows:
					goto IL_8B;
				}
				goto IL_58;
				IL_8B:
				result = "Windows";
				return result;
				IL_96:
				result = "Mac OS X";
				return result;
			}

			public int BuildPlatformIndexFromTargetGroup(BuildTargetGroup group)
			{
				int result;
				for (int i = 0; i < this.buildPlatforms.Length; i++)
				{
					if (group == this.buildPlatforms[i].targetGroup)
					{
						result = i;
						return result;
					}
				}
				result = -1;
				return result;
			}

			public BuildPlayerWindow.BuildPlatform BuildPlatformFromTargetGroup(BuildTargetGroup group)
			{
				int num = this.BuildPlatformIndexFromTargetGroup(group);
				return (num == -1) ? null : this.buildPlatforms[num];
			}
		}

		private class Styles
		{
			public static readonly GUIContent invalidColorSpaceMessage = EditorGUIUtility.TextContent("In order to build a player go to 'Player Settings...' to resolve the incompatibility between the Color Space and the current settings.");

			public GUIStyle selected = "ServerUpdateChangesetOn";

			public GUIStyle box = "OL Box";

			public GUIStyle title = "OL title";

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

			public GUIContent forceOptimizeScriptCompilation;

			public GUIContent learnAboutUnityCloudBuild;

			public Styles()
			{
				GUIContent[,] expr_128 = new GUIContent[13, 3];
				expr_128[0, 0] = EditorGUIUtility.TextContent("Your license does not cover Standalone Publishing.");
				expr_128[0, 1] = new GUIContent("");
				expr_128[0, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[1, 0] = EditorGUIUtility.TextContent("Your license does not cover iOS Publishing.");
				expr_128[1, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[1, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[2, 0] = EditorGUIUtility.TextContent("Your license does not cover Apple TV Publishing.");
				expr_128[2, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[2, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[3, 0] = EditorGUIUtility.TextContent("Your license does not cover Android Publishing.");
				expr_128[3, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[3, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[4, 0] = EditorGUIUtility.TextContent("Your license does not cover Tizen Publishing.");
				expr_128[4, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[4, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[5, 0] = EditorGUIUtility.TextContent("Your license does not cover Xbox One Publishing.");
				expr_128[5, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_128[5, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_128[6, 0] = EditorGUIUtility.TextContent("Your license does not cover PS Vita Publishing.");
				expr_128[6, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_128[6, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_128[7, 0] = EditorGUIUtility.TextContent("Your license does not cover PS4 Publishing.");
				expr_128[7, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_128[7, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_128[8, 0] = EditorGUIUtility.TextContent("Your license does not cover Wii U Publishing.");
				expr_128[8, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_128[8, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				expr_128[9, 0] = EditorGUIUtility.TextContent("Your license does not cover Windows Store Publishing.");
				expr_128[9, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[9, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[10, 0] = EditorGUIUtility.TextContent("Your license does not cover Windows Phone 8 Publishing.");
				expr_128[10, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[10, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[11, 0] = EditorGUIUtility.TextContent("Your license does not cover SamsungTV Publishing");
				expr_128[11, 1] = EditorGUIUtility.TextContent("Go to Our Online Store");
				expr_128[11, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[12, 0] = EditorGUIUtility.TextContent("Your license does not cover Nintendo 3DS Publishing");
				expr_128[12, 1] = EditorGUIUtility.TextContent("Contact sales");
				expr_128[12, 2] = new GUIContent("http://unity3d.com/company/sales?type=sales");
				this.notLicensedMessages = expr_128;
				GUIContent[,] expr_400 = new GUIContent[13, 3];
				expr_400[0, 0] = EditorGUIUtility.TextContent("Standalone Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[0, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[1, 0] = EditorGUIUtility.TextContent("iOS Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[1, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[2, 0] = EditorGUIUtility.TextContent("Apple TV Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[2, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[3, 0] = EditorGUIUtility.TextContent("Android Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[3, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[4, 0] = EditorGUIUtility.TextContent("Tizen is not supported in this build.\nDownload a build that supports it.");
				expr_400[4, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[5, 0] = EditorGUIUtility.TextContent("Xbox One Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[5, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[6, 0] = EditorGUIUtility.TextContent("PS Vita Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[6, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[7, 0] = EditorGUIUtility.TextContent("PS4 Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[7, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[8, 0] = EditorGUIUtility.TextContent("Wii U Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[8, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[9, 0] = EditorGUIUtility.TextContent("Windows Store Player is not supported in\nthis build.\n\nDownload a build that supports it.");
				expr_400[9, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[10, 0] = EditorGUIUtility.TextContent("Windows Phone 8 Player is not supported\nin this build.\n\nDownload a build that supports it.");
				expr_400[10, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[11, 0] = EditorGUIUtility.TextContent("SamsungTV Player is not supported in this build.\nDownload a build that supports it.");
				expr_400[11, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_400[12, 0] = EditorGUIUtility.TextContent("Nintendo 3DS is not supported in this build.\nDownload a build that supports it.");
				expr_400[12, 2] = new GUIContent("http://unity3d.com/unity/download/");
				this.buildTargetNotInstalled = expr_400;
				this.debugBuild = EditorGUIUtility.TextContent("Development Build");
				this.profileBuild = EditorGUIUtility.TextContent("Autoconnect Profiler");
				this.allowDebugging = EditorGUIUtility.TextContent("Script Debugging");
				this.symlinkiOSLibraries = EditorGUIUtility.TextContent("Symlink Unity libraries");
				this.explicitNullChecks = EditorGUIUtility.TextContent("Explicit Null Checks");
				this.explicitDivideByZeroChecks = EditorGUIUtility.TextContent("Divide By Zero Checks");
				this.enableHeadlessMode = EditorGUIUtility.TextContent("Headless Mode");
				this.buildScriptsOnly = EditorGUIUtility.TextContent("Scripts Only Build");
				this.forceOptimizeScriptCompilation = EditorGUIUtility.TextContent("Build Optimized Scripts|Compile IL2CPP using full compiler optimizations. Note this will obfuscate callstack output.");
				this.learnAboutUnityCloudBuild = EditorGUIUtility.TextContent("Learn about Unity Cloud Build");
				base..ctor();
				this.levelStringCounter.alignment = TextAnchor.MiddleRight;
				if (Unsupported.IsDeveloperBuild() && (this.buildTargetNotInstalled.GetLength(0) != this.notLicensedMessages.GetLength(0) || this.buildTargetNotInstalled.GetLength(0) != BuildPlayerWindow.s_BuildPlatforms.buildPlatforms.Length))
				{
					Debug.LogErrorFormat("Build platforms and messages are desynced in BuildPlayerWindow! ({0} vs. {1} vs. {2}) DON'T SHIP THIS!", new object[]
					{
						this.buildTargetNotInstalled.GetLength(0),
						this.notLicensedMessages.GetLength(0),
						BuildPlayerWindow.s_BuildPlatforms.buildPlatforms.Length
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

		private static BuildPlayerWindow.BuildPlatforms s_BuildPlatforms;

		private ListViewState lv = new ListViewState();

		private bool[] selectedLVItems = new bool[0];

		private bool[] selectedBeforeDrag;

		private int initialSelectedLVItem = -1;

		private Vector2 scrollPosition = new Vector2(0f, 0f);

		private const string kAssetsFolder = "Assets/";

		private const string kEditorBuildSettingsPath = "ProjectSettings/EditorBuildSettings.asset";

		private static BuildPlayerWindow.Styles styles = null;

		public BuildPlayerWindow()
		{
			base.position = new Rect(50f, 50f, 540f, 530f);
			base.minSize = new Vector2(630f, 580f);
			base.titleContent = new GUIContent("Build Settings");
		}

		private static void ShowBuildPlayerWindow()
		{
			EditorUserBuildSettings.selectedBuildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			EditorWindow.GetWindow<BuildPlayerWindow>(true, "Build Settings");
		}

		private static void BuildPlayerAndRun()
		{
			if (!BuildPlayerWindow.BuildPlayerWithDefaultSettings(false, BuildOptions.AutoRunPlayer))
			{
				BuildPlayerWindow.ShowBuildPlayerWindow();
			}
		}

		private static void BuildPlayerAndSelect()
		{
			if (!BuildPlayerWindow.BuildPlayerWithDefaultSettings(false, BuildOptions.ShowBuiltPlayer))
			{
				BuildPlayerWindow.ShowBuildPlayerWindow();
			}
		}

		private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions)
		{
			return BuildPlayerWindow.BuildPlayerWithDefaultSettings(askForBuildLocation, forceOptions, true);
		}

		private static bool IsMetroPlayer(BuildTarget target)
		{
			return target == BuildTarget.WSAPlayer;
		}

		private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions, bool first)
		{
			bool flag = false;
			BuildPlayerWindow.InitBuildPlatforms();
			bool result;
			if (!UnityConnect.instance.canBuildWithUPID)
			{
				if (!EditorUtility.DisplayDialog("Missing Project ID", "Because you are not a member of this project this build will not access Unity services.\nDo you want to continue?", "Yes", "No"))
				{
					result = false;
					return result;
				}
			}
			BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
			if (!BuildPipeline.IsBuildTargetSupported(buildTarget))
			{
				result = false;
			}
			else
			{
				string targetStringFromBuildTargetGroup = ModuleManager.GetTargetStringFromBuildTargetGroup(BuildPlayerWindow.s_BuildPlatforms.BuildPlatformFromTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup).targetGroup);
				IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(targetStringFromBuildTargetGroup);
				if (buildWindowExtension != null && (forceOptions & BuildOptions.AutoRunPlayer) != BuildOptions.None && !buildWindowExtension.EnabledBuildAndRunButton())
				{
					result = false;
				}
				else
				{
					if (Unsupported.IsBleedingEdgeBuild())
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("This version of Unity is a BleedingEdge build that has not seen any manual testing.");
						stringBuilder.AppendLine("You should consider this build unstable.");
						stringBuilder.AppendLine("We strongly recommend that you use a normal version of Unity instead.");
						if (EditorUtility.DisplayDialog("BleedingEdge Build", stringBuilder.ToString(), "Cancel", "OK"))
						{
							result = false;
							return result;
						}
					}
					string text = "";
					bool flag2 = EditorUserBuildSettings.installInBuildFolder && PostprocessBuildPlayer.SupportsInstallInBuildFolder(buildTarget) && (Unsupported.IsDeveloperBuild() || BuildPlayerWindow.IsMetroPlayer(buildTarget));
					BuildOptions buildOptions = forceOptions;
					bool development = EditorUserBuildSettings.development;
					if (development)
					{
						buildOptions |= BuildOptions.Development;
					}
					if (EditorUserBuildSettings.allowDebugging && development)
					{
						buildOptions |= BuildOptions.AllowDebugging;
					}
					if (EditorUserBuildSettings.symlinkLibraries)
					{
						buildOptions |= BuildOptions.SymlinkLibraries;
					}
					if (buildTarget == BuildTarget.Android)
					{
						if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
						{
							buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
						}
					}
					if (EditorUserBuildSettings.enableHeadlessMode)
					{
						buildOptions |= BuildOptions.EnableHeadlessMode;
					}
					if (EditorUserBuildSettings.connectProfiler && (development || buildTarget == BuildTarget.WSAPlayer))
					{
						buildOptions |= BuildOptions.ConnectWithProfiler;
					}
					if (EditorUserBuildSettings.buildScriptsOnly)
					{
						buildOptions |= BuildOptions.BuildScriptsOnly;
					}
					if (EditorUserBuildSettings.forceOptimizeScriptCompilation)
					{
						buildOptions |= BuildOptions.ForceOptimizeScriptCompilation;
					}
					if (flag2)
					{
						buildOptions |= BuildOptions.InstallInBuildFolder;
					}
					if (!flag2)
					{
						if (askForBuildLocation && !BuildPlayerWindow.PickBuildLocation(buildTarget, buildOptions, out flag))
						{
							result = false;
							return result;
						}
						text = EditorUserBuildSettings.GetBuildLocation(buildTarget);
						if (text.Length == 0)
						{
							result = false;
							return result;
						}
						if (!askForBuildLocation)
						{
							CanAppendBuild canAppendBuild = InternalEditorUtility.BuildCanBeAppended(buildTarget, text);
							if (canAppendBuild != CanAppendBuild.Unsupported)
							{
								if (canAppendBuild != CanAppendBuild.Yes)
								{
									if (canAppendBuild == CanAppendBuild.No)
									{
										if (!BuildPlayerWindow.PickBuildLocation(buildTarget, buildOptions, out flag))
										{
											result = false;
											return result;
										}
										text = EditorUserBuildSettings.GetBuildLocation(buildTarget);
										if (text.Length == 0 || !Directory.Exists(FileUtil.DeleteLastPathNameComponent(text)))
										{
											result = false;
											return result;
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
						buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
					}
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
					string[] levels = arrayList.ToArray(typeof(string)) as string[];
					bool delayToAfterScriptReload = false;
					if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
					{
						if (!EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget))
						{
							Debug.LogErrorFormat("Could not switch to build target '{0}'.", new object[]
							{
								BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(buildTarget)
							});
							result = false;
							return result;
						}
						if (EditorApplication.isCompiling)
						{
							delayToAfterScriptReload = true;
						}
					}
					BuildReport buildReport = BuildPipeline.BuildPlayerInternalNoCheck(levels, text, null, buildTarget, buildOptions, delayToAfterScriptReload);
					result = (buildReport == null || buildReport.totalErrors == 0);
				}
			}
			return result;
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
							EditorBuildSettingsScene editorBuildSettingsScene3 = new EditorBuildSettingsScene();
							editorBuildSettingsScene3.path = scenePath;
							editorBuildSettingsScene3.enabled = true;
							list.Insert(this.lv.draggedTo + num2++, editorBuildSettingsScene3);
						}
					}
					IL_64D:
					i++;
					continue;
					goto IL_64D;
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
						list.Add(new EditorBuildSettingsScene
						{
							path = scene.path,
							enabled = true
						});
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
				if (BuildPlayerWindow.s_BuildPlatforms == null)
				{
					throw new Exception("Build platforms are not initialized.");
				}
				BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
				if (buildPlatform == null)
				{
					throw new Exception("Could not find build platform for target group " + selectedBuildTargetGroup);
				}
				result = buildPlatform.DefaultTarget;
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
				BuildPlayerWindow.BuildPlatform[] buildPlatforms = BuildPlayerWindow.s_BuildPlatforms.buildPlatforms;
				for (int j = 0; j < buildPlatforms.Length; j++)
				{
					BuildPlayerWindow.BuildPlatform buildPlatform = buildPlatforms[j];
					if (BuildPlayerWindow.IsBuildTargetGroupSupported(buildPlatform.DefaultTarget) == flag)
					{
						if (BuildPlayerWindow.IsBuildTargetGroupSupported(buildPlatform.DefaultTarget) || buildPlatform.forceShowTarget)
						{
							if (BuildPipeline.IsBuildTargetCompatibleWithOS(buildPlatform.DefaultTarget))
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
			BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = (BuildPipeline.IsBuildTargetSupported(buildTarget) && BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) != BuildPipeline.GetBuildTargetGroup(buildTarget));
			if (GUILayout.Button(BuildPlayerWindow.styles.switchPlatform, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
				GUIUtility.ExitGUI();
			}
			GUI.enabled = BuildPipeline.IsBuildTargetSupported(buildTarget);
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

		private void ShowOption(BuildPlayerWindow.BuildPlatform bp, GUIContent title, GUIStyle background)
		{
			Rect rect = GUILayoutUtility.GetRect(50f, 36f);
			rect.x += 1f;
			rect.y += 1f;
			bool flag = BuildPipeline.LicenseCheck(bp.DefaultTarget);
			GUI.contentColor = new Color(1f, 1f, 1f, (!flag) ? 0.7f : 1f);
			bool flag2 = EditorUserBuildSettings.selectedBuildTargetGroup == bp.targetGroup;
			if (Event.current.type == EventType.Repaint)
			{
				background.Draw(rect, GUIContent.none, false, false, flag2, false);
				GUI.Label(new Rect(rect.x + 3f, rect.y + 3f, 32f, 32f), title.image, GUIStyle.none);
				if (BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == bp.targetGroup)
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
			BuildPlayerWindow.InitBuildPlatforms();
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
			bool flag = !AssetDatabase.IsOpenForEdit("ProjectSettings/EditorBuildSettings.asset", out text);
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
				GUILayout.Height(301f)
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

		private static void InitBuildPlatforms()
		{
			if (BuildPlayerWindow.s_BuildPlatforms == null)
			{
				BuildPlayerWindow.s_BuildPlatforms = new BuildPlayerWindow.BuildPlatforms();
				BuildPlayerWindow.RepairSelectedBuildTargetGroup();
			}
		}

		internal static List<BuildPlayerWindow.BuildPlatform> GetValidPlatforms()
		{
			BuildPlayerWindow.InitBuildPlatforms();
			List<BuildPlayerWindow.BuildPlatform> list = new List<BuildPlayerWindow.BuildPlatform>();
			BuildPlayerWindow.BuildPlatform[] buildPlatforms = BuildPlayerWindow.s_BuildPlatforms.buildPlatforms;
			for (int i = 0; i < buildPlatforms.Length; i++)
			{
				BuildPlayerWindow.BuildPlatform buildPlatform = buildPlatforms[i];
				if (buildPlatform.targetGroup == BuildTargetGroup.Standalone || BuildPipeline.IsBuildTargetSupported(buildPlatform.DefaultTarget))
				{
					list.Add(buildPlatform);
				}
			}
			return list;
		}

		internal static bool IsBuildTargetGroupSupported(BuildTarget target)
		{
			return target == BuildTarget.StandaloneWindows || BuildPipeline.IsBuildTargetSupported(target);
		}

		private static void RepairSelectedBuildTargetGroup()
		{
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			if (selectedBuildTargetGroup == BuildTargetGroup.Unknown || BuildPlayerWindow.s_BuildPlatforms == null || BuildPlayerWindow.s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(selectedBuildTargetGroup) < 0)
			{
				EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Standalone;
			}
		}

		private static bool IsAnyStandaloneModuleLoaded()
		{
			return ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows));
		}

		private static bool IsColorSpaceValid(BuildPlayerWindow.BuildPlatform platform)
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
					flag = (graphicsAPIs3.Contains(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs3.Contains(GraphicsDeviceType.OpenGLES2));
					flag2 = (PlayerSettings.Android.minSdkVersion >= AndroidSdkVersions.AndroidApiLevel18);
				}
				else if (platform.targetGroup == BuildTargetGroup.Standalone)
				{
					GraphicsDeviceType[] graphicsAPIs4 = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
					flag = (!graphicsAPIs4.Contains(GraphicsDeviceType.OpenGLES3) && !graphicsAPIs4.Contains(GraphicsDeviceType.OpenGLES2));
				}
				result = (flag && flag2);
			}
			else
			{
				result = true;
			}
			return result;
		}

		private static string GetPlaybackEngineDownloadURL(string moduleName)
		{
			string unityVersionFull = InternalEditorUtility.GetUnityVersionFull();
			string text = "";
			string text2 = "";
			int num = unityVersionFull.LastIndexOf('_');
			if (num != -1)
			{
				text = unityVersionFull.Substring(num + 1);
				text2 = unityVersionFull.Substring(0, num);
			}
			string result;
			if (moduleName == "XboxOne")
			{
				result = "http://blogs.unity3d.com/2014/08/11/unity-for-xbox-one-is-here/";
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
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
					}
				};
				if (dictionary.ContainsKey(moduleName))
				{
					moduleName = dictionary[moduleName];
				}
				string text3 = "Unknown";
				string text4;
				string text5;
				if (text2.IndexOf('a') != -1 || text2.IndexOf('b') != -1)
				{
					text4 = "beta";
					text5 = "download";
				}
				else
				{
					text4 = "download";
					text5 = "download_unity";
				}
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					text3 = "TargetSupportInstaller";
				}
				else if (Application.platform == RuntimePlatform.OSXEditor)
				{
					text3 = "MacEditorTargetInstaller";
				}
				string text6 = string.Concat(new string[]
				{
					"http://",
					text4,
					".unity3d.com/",
					text5,
					"/",
					text,
					"/",
					text3,
					"/UnitySetup-",
					moduleName,
					"-Support-for-Editor-",
					text2
				});
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					text6 += ".exe";
				}
				else if (Application.platform == RuntimePlatform.OSXEditor)
				{
					text6 += ".pkg";
				}
				result = text6;
			}
			return result;
		}

		private bool IsModuleInstalled(BuildTarget buildTarget)
		{
			bool flag = BuildPipeline.LicenseCheck(buildTarget);
			string targetStringFromBuildTarget = ModuleManager.GetTargetStringFromBuildTarget(buildTarget);
			return flag && !string.IsNullOrEmpty(targetStringFromBuildTarget) && ModuleManager.GetBuildPostProcessor(buildTarget) == null && (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone || !BuildPlayerWindow.IsAnyStandaloneModuleLoaded());
		}

		private void ShowBuildTargetSettings()
		{
			EditorGUIUtility.labelWidth = Mathf.Min(180f, (base.position.width - 265f) * 0.47f);
			BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformFromTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			bool flag = BuildPipeline.LicenseCheck(buildTarget);
			GUILayout.Space(18f);
			Rect rect = GUILayoutUtility.GetRect(50f, 36f);
			rect.x += 1f;
			GUI.Label(new Rect(rect.x + 3f, rect.y + 3f, 32f, 32f), buildPlatform.title.image, GUIStyle.none);
			GUI.Toggle(rect, false, buildPlatform.title.text, BuildPlayerWindow.styles.platformSelector);
			GUILayout.Space(10f);
			if (buildPlatform.targetGroup == BuildTargetGroup.WebGL && !BuildPipeline.IsBuildTargetSupported(buildTarget))
			{
				if (IntPtr.Size == 4)
				{
					GUILayout.Label("Building for WebGL requires a 64-bit Unity editor.", new GUILayoutOption[0]);
					BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
					return;
				}
			}
			string targetStringFromBuildTarget = ModuleManager.GetTargetStringFromBuildTarget(buildTarget);
			if (this.IsModuleInstalled(buildTarget))
			{
				GUILayout.Label("No " + BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(buildTarget) + " module loaded.", new GUILayoutOption[0]);
				if (GUILayout.Button("Open Download Page", EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					string playbackEngineDownloadURL = BuildPlayerWindow.GetPlaybackEngineDownloadURL(targetStringFromBuildTarget);
					Help.BrowseURL(playbackEngineDownloadURL);
				}
				BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
			}
			else
			{
				if (Application.HasProLicense() && !InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(buildTarget))
				{
					string text = string.Format("{0} is not included in your Unity Pro license. Your {0} build will include a Unity Personal Edition splash screen.\n\nYou must be eligible to use Unity Personal Edition to use this build option. Please refer to our EULA for further information.", BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(buildTarget));
					GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
					GUILayout.Label(text, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button("EULA", EditorStyles.miniButton, new GUILayoutOption[0]))
					{
						Application.OpenURL("http://unity3d.com/legal/eula");
					}
					if (GUILayout.Button(string.Format("Add {0} to your Unity Pro license", BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(buildTarget)), EditorStyles.miniButton, new GUILayoutOption[0]))
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
					int num = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(buildPlatform.targetGroup);
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
					string targetStringFrom = ModuleManager.GetTargetStringFrom(buildPlatform.targetGroup, buildTarget);
					IBuildWindowExtension buildWindowExtension = ModuleManager.GetBuildWindowExtension(targetStringFrom);
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
					IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(buildTarget);
					bool flag8 = buildPostProcessor != null && buildPostProcessor.SupportsScriptsOnlyBuild();
					bool canInstallInBuildFolder = false;
					if (BuildPipeline.IsBuildTargetSupported(buildTarget))
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
						bool flag10 = PlayerSettings.GetScriptingBackend(buildPlatform.targetGroup) == ScriptingImplementation.IL2CPP;
						bool flag11 = buildWindowExtension != null && development && flag10 && buildWindowExtension.ShouldDrawForceOptimizeScriptsCheckbox();
						if (flag11)
						{
							EditorUserBuildSettings.forceOptimizeScriptCompilation = EditorGUILayout.Toggle(BuildPlayerWindow.styles.forceOptimizeScriptCompilation, EditorUserBuildSettings.forceOptimizeScriptCompilation, new GUILayoutOption[0]);
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
						canInstallInBuildFolder = (Unsupported.IsDeveloperBuild() && PostprocessBuildPlayer.SupportsInstallInBuildFolder(buildTarget));
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
						int index = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(buildPlatform.targetGroup);
						GUILayout.Label(BuildPlayerWindow.styles.GetTargetNotInstalled(index, 0), new GUILayoutOption[0]);
						if (BuildPlayerWindow.styles.GetTargetNotInstalled(index, 1) != null && GUILayout.Button(BuildPlayerWindow.styles.GetTargetNotInstalled(index, 1), new GUILayoutOption[0]))
						{
							Application.OpenURL(BuildPlayerWindow.styles.GetTargetNotInstalled(index, 2).text);
						}
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
					}
					BuildPlayerWindow.GUIBuildButtons(buildWindowExtension, flag2, enableBuildAndRunButton, canInstallInBuildFolder, buildPlatform);
				}
			}
		}

		private static void GUIBuildButtons(bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlayerWindow.BuildPlatform platform)
		{
			BuildPlayerWindow.GUIBuildButtons(null, enableBuildButton, enableBuildAndRunButton, canInstallInBuildFolder, platform);
		}

		private static void GUIBuildButtons(IBuildWindowExtension buildWindowExtension, bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlayerWindow.BuildPlatform platform)
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
				BuildPlayerWindow.BuildPlayerWithDefaultSettings(true, BuildOptions.ShowBuiltPlayer);
				GUIUtility.ExitGUI();
			}
			GUI.enabled = enableBuildAndRunButton;
			if (GUILayout.Button(BuildPlayerWindow.styles.buildAndRun, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				BuildPlayerWindow.BuildPlayerWithDefaultSettings(true, BuildOptions.AutoRunPlayer);
				GUIUtility.ExitGUI();
			}
			GUILayout.EndHorizontal();
		}

		private static bool PickBuildLocation(BuildTarget target, BuildOptions options, out bool updateExistingBuild)
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
				string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(target, options);
				string directory = FileUtil.DeleteLastPathNameComponent(buildLocation);
				string lastPathNameComponent = FileUtil.GetLastPathNameComponent(buildLocation);
				string title2 = "Build " + BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(target);
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
						if (target == BuildTarget.iOS && Application.platform != RuntimePlatform.OSXEditor && !BuildPlayerWindow.FolderIsEmpty(text) && !BuildPlayerWindow.UserWantsToDeleteFiles(text))
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
	}
}
