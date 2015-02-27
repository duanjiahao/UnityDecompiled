using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Modules;
using UnityEditor.VersionControl;
using UnityEditorInternal;
using UnityEngine;
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
			public BuildTarget DefaultTarget
			{
				get
				{
					switch (this.targetGroup)
					{
					case BuildTargetGroup.Standalone:
						return BuildTarget.StandaloneWindows;
					case BuildTargetGroup.WebPlayer:
						return BuildTarget.WebPlayer;
					case BuildTargetGroup.iPhone:
						return BuildTarget.iPhone;
					case BuildTargetGroup.PS3:
						return BuildTarget.PS3;
					case BuildTargetGroup.XBOX360:
						return BuildTarget.XBOX360;
					case BuildTargetGroup.Android:
						return BuildTarget.Android;
					case BuildTargetGroup.GLESEmu:
						return BuildTarget.StandaloneGLESEmu;
					case BuildTargetGroup.NaCl:
						return BuildTarget.NaCl;
					case BuildTargetGroup.FlashPlayer:
						return BuildTarget.FlashPlayer;
					case BuildTargetGroup.Metro:
						return BuildTarget.MetroPlayer;
					case BuildTargetGroup.WP8:
						return BuildTarget.WP8Player;
					case BuildTargetGroup.BB10:
						return BuildTarget.BB10;
					case BuildTargetGroup.Tizen:
						return BuildTarget.Tizen;
					case BuildTargetGroup.PSP2:
						return BuildTarget.PSP2;
					case BuildTargetGroup.PS4:
						return BuildTarget.PS4;
					case BuildTargetGroup.PSM:
						return BuildTarget.PSM;
					case BuildTargetGroup.XboxOne:
						return BuildTarget.XboxOne;
					case BuildTargetGroup.SamsungTV:
						return BuildTarget.SamsungTV;
					}
					return (BuildTarget)(-1);
				}
			}
			public BuildPlatform(string locTitle, BuildTargetGroup targetGroup, bool forceShowTarget)
			{
				this.targetGroup = targetGroup;
				this.name = BuildPipeline.GetBuildTargetGroupName(this.DefaultTarget);
				this.title = EditorGUIUtility.TextContent(locTitle);
				this.smallIcon = (EditorGUIUtility.IconContent(locTitle + ".Small").image as Texture2D);
				this.forceShowTarget = forceShowTarget;
			}
		}
		private class BuildPlatforms
		{
			public BuildPlayerWindow.BuildPlatform[] buildPlatforms;
			public BuildTarget[] standaloneSubtargets;
			public GUIContent[] standaloneSubtargetStrings;
			public SCEBuildSubtarget[] sceSubtargets = new SCEBuildSubtarget[]
			{
				SCEBuildSubtarget.PCHosted,
				SCEBuildSubtarget.HddTitle,
				SCEBuildSubtarget.BluRayTitle
			};
			public GUIContent[] sceSubtargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.SCEBuildSubtargetPCHosted"),
				EditorGUIUtility.TextContent("BuildSettings.SCEBuildSubtargetHddTitle"),
				EditorGUIUtility.TextContent("BuildSettings.SCEBuildSubtargetBluRayTitle")
			};
			public PSP2BuildSubtarget[] psp2Subtargets = new PSP2BuildSubtarget[]
			{
				PSP2BuildSubtarget.PCHosted,
				PSP2BuildSubtarget.Package
			};
			public GUIContent[] psp2SubtargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.PSP2BuildSubtargetPCHosted"),
				EditorGUIUtility.TextContent("BuildSettings.PSP2BuildSubtargetPackage")
			};
			public FlashBuildSubtarget[] flashBuildSubtargets = new FlashBuildSubtarget[]
			{
				FlashBuildSubtarget.Flash11dot2,
				FlashBuildSubtarget.Flash11dot3,
				FlashBuildSubtarget.Flash11dot4,
				FlashBuildSubtarget.Flash11dot5,
				FlashBuildSubtarget.Flash11dot6,
				FlashBuildSubtarget.Flash11dot7,
				FlashBuildSubtarget.Flash11dot8
			};
			public GUIContent[] flashBuildSubtargetString = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot2"),
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot3"),
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot4"),
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot5"),
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot6"),
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot7"),
				EditorGUIUtility.TextContent("BuildSettings.FlashSubtarget11dot8")
			};
			public MetroBuildType[] metroBuildTypes = new MetroBuildType[]
			{
				MetroBuildType.VisualStudioCpp,
				MetroBuildType.VisualStudioCSharp
			};
			public GUIContent[] metroBuildTypeStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.MetroBuildTypeVisualStudioCpp"),
				EditorGUIUtility.TextContent("BuildSettings.MetroBuildTypeVisualStudioCSharp")
			};
			public MetroSDK[] metroSDKs = new MetroSDK[]
			{
				MetroSDK.SDK80,
				MetroSDK.SDK81,
				MetroSDK.PhoneSDK81,
				MetroSDK.UniversalSDK81
			};
			public GUIContent[] metroSDKStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.MetroSDK80"),
				EditorGUIUtility.TextContent("BuildSettings.MetroSDK81"),
				EditorGUIUtility.TextContent("BuildSettings.MetroPhoneSDK81"),
				EditorGUIUtility.TextContent("BuildSettings.MetroUniversalSDK81")
			};
			public MetroBuildAndRunDeployTarget[] metroBuildAndRunDeployTargets = new MetroBuildAndRunDeployTarget[]
			{
				MetroBuildAndRunDeployTarget.LocalMachine,
				MetroBuildAndRunDeployTarget.WindowsPhone,
				MetroBuildAndRunDeployTarget.LocalMachineAndWindowsPhone
			};
			public GUIContent[] metroBuildAndRunDeployTargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.DeployTargetLocalMachine"),
				EditorGUIUtility.TextContent("BuildSettings.DeployTargetWindowsPhone"),
				EditorGUIUtility.TextContent("BuildSettings.DeployTargetBoth")
			};
			public XboxBuildSubtarget[] xboxBuildSubtargets = new XboxBuildSubtarget[]
			{
				XboxBuildSubtarget.Development,
				XboxBuildSubtarget.Master,
				XboxBuildSubtarget.Debug
			};
			public GUIContent[] xboxBuildSubtargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.XboxBuildSubtargetDevelopment"),
				EditorGUIUtility.TextContent("BuildSettings.XboxBuildSubtargetMaster"),
				EditorGUIUtility.TextContent("BuildSettings.XboxBuildSubtargetDebug")
			};
			public XboxRunMethod[] xboxRunMethods = new XboxRunMethod[]
			{
				XboxRunMethod.HDD,
				XboxRunMethod.DiscEmuFast,
				XboxRunMethod.DiscEmuAccurate
			};
			public GUIContent[] xboxRunMethodStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.XboxRunMethodHDD"),
				EditorGUIUtility.TextContent("BuildSettings.XboxRunMethodDiscEmuFast"),
				EditorGUIUtility.TextContent("BuildSettings.XboxRunMethodDiscEmuAccurate")
			};
			public AndroidBuildSubtarget[] androidBuildSubtargets = new AndroidBuildSubtarget[]
			{
				AndroidBuildSubtarget.Generic,
				AndroidBuildSubtarget.DXT,
				AndroidBuildSubtarget.PVRTC,
				AndroidBuildSubtarget.ATC,
				AndroidBuildSubtarget.ETC,
				AndroidBuildSubtarget.ETC2,
				AndroidBuildSubtarget.ASTC
			};
			public GUIContent[] androidBuildSubtargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetGeneric"),
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetDXT"),
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetPVRTC"),
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetATC"),
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetETC"),
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetETC2"),
				EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtargetASTC")
			};
			public BlackBerryBuildSubtarget[] blackberryBuildSubtargets = new BlackBerryBuildSubtarget[]
			{
				BlackBerryBuildSubtarget.Generic,
				BlackBerryBuildSubtarget.PVRTC,
				BlackBerryBuildSubtarget.ATC,
				BlackBerryBuildSubtarget.ETC
			};
			public GUIContent[] blackberryBuildSubtargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildSubtargetGeneric"),
				EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildSubtargetPVRTC"),
				EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildSubtargetATC"),
				EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildSubtargetETC")
			};
			public BlackBerryBuildType[] blackberryBuildTypes = new BlackBerryBuildType[]
			{
				BlackBerryBuildType.Debug,
				BlackBerryBuildType.Submission
			};
			public GUIContent[] blackberryBuildTypeStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildTypeDebug"),
				EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildTypeSubmission")
			};
			public TizenBuildSubtarget[] tizenBuildSubtargets = new TizenBuildSubtarget[]
			{
				TizenBuildSubtarget.Generic,
				TizenBuildSubtarget.ATC,
				TizenBuildSubtarget.ETC
			};
			public GUIContent[] tizenBuildSubtargetStrings = new GUIContent[]
			{
				EditorGUIUtility.TextContent("BuildSettings.TizenBuildSubtargetGeneric"),
				EditorGUIUtility.TextContent("BuildSettings.TizenBuildSubtargetATC"),
				EditorGUIUtility.TextContent("BuildSettings.TizenBuildSubtargetETC")
			};
			internal BuildPlatforms()
			{
				this.buildPlatforms = new List<BuildPlayerWindow.BuildPlatform>
				{
					new BuildPlayerWindow.BuildPlatform("BuildSettings.Web", BuildTargetGroup.WebPlayer, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.Standalone", BuildTargetGroup.Standalone, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.iPhone", BuildTargetGroup.iPhone, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.Android", BuildTargetGroup.Android, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.BlackBerry", BuildTargetGroup.BB10, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.Tizen", BuildTargetGroup.Tizen, false),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.XBox360", BuildTargetGroup.XBOX360, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.XboxOne", BuildTargetGroup.XboxOne, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.PS3", BuildTargetGroup.PS3, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.PSP2", BuildTargetGroup.PSP2, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.PS4", BuildTargetGroup.PS4, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.PSM", BuildTargetGroup.PSM, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.StandaloneGLESEmu", BuildTargetGroup.GLESEmu, false),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.FlashPlayer", BuildTargetGroup.FlashPlayer, false),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.Metro", BuildTargetGroup.Metro, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.WP8", BuildTargetGroup.WP8, true),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.NaCl", BuildTargetGroup.NaCl, false),
					new BuildPlayerWindow.BuildPlatform("BuildSettings.SamsungTV", BuildTargetGroup.SamsungTV, false)
				}.ToArray();
				List<BuildTarget> list = new List<BuildTarget>();
				list.Add(BuildTarget.StandaloneWindows);
				list.Add(BuildTarget.StandaloneOSXIntel);
				list.Add(BuildTarget.StandaloneLinux);
				List<GUIContent> list2 = new List<GUIContent>();
				list2.Add(EditorGUIUtility.TextContent("BuildSettings.StandaloneWindows"));
				list2.Add(EditorGUIUtility.TextContent("BuildSettings.StandaloneOSXIntel"));
				list2.Add(EditorGUIUtility.TextContent("BuildSettings.StandaloneLinux"));
				this.standaloneSubtargets = list.ToArray();
				this.standaloneSubtargetStrings = list2.ToArray();
			}
			public string GetBuildTargetDisplayName(BuildTarget target)
			{
				BuildPlayerWindow.BuildPlatform[] array = this.buildPlatforms;
				for (int i = 0; i < array.Length; i++)
				{
					BuildPlayerWindow.BuildPlatform buildPlatform = array[i];
					if (buildPlatform.DefaultTarget == target)
					{
						return buildPlatform.title.text;
					}
				}
				if (target == BuildTarget.WebPlayerStreamed)
				{
					return this.BuildPlatformFromTargetGroup(BuildTargetGroup.WebPlayer).title.text;
				}
				for (int j = 0; j < this.standaloneSubtargets.Length; j++)
				{
					if (this.standaloneSubtargets[j] == BuildPlayerWindow.BuildPlatforms.DefaultTargetForPlatform(target))
					{
						return this.standaloneSubtargetStrings[j].text;
					}
				}
				return "Unsupported Target";
			}
			public static Dictionary<GUIContent, BuildTarget> GetArchitecturesForPlatform(BuildTarget target)
			{
				switch (target)
				{
				case BuildTarget.StandaloneOSXUniversal:
				case BuildTarget.StandaloneOSXIntel:
					goto IL_B6;
				case (BuildTarget)3:
					IL_1A:
					switch (target)
					{
					case BuildTarget.StandaloneLinux64:
					case BuildTarget.StandaloneLinuxUniversal:
						goto IL_78;
					case BuildTarget.WP8Player:
						IL_33:
						switch (target)
						{
						case BuildTarget.StandaloneLinux:
							goto IL_78;
						case BuildTarget.StandaloneWindows64:
							goto IL_4D;
						}
						return null;
					case BuildTarget.StandaloneOSXIntel64:
						goto IL_B6;
					}
					goto IL_33;
					IL_78:
					return new Dictionary<GUIContent, BuildTarget>
					{

						{
							EditorGUIUtility.TextContent("x86"),
							BuildTarget.StandaloneLinux
						},

						{
							EditorGUIUtility.TextContent("x86_64"),
							BuildTarget.StandaloneLinux64
						},

						{
							EditorGUIUtility.TextContent("x86 + x86_64 (Universal)"),
							BuildTarget.StandaloneLinuxUniversal
						}
					};
				case BuildTarget.StandaloneWindows:
					goto IL_4D;
				}
				goto IL_1A;
				IL_4D:
				return new Dictionary<GUIContent, BuildTarget>
				{

					{
						EditorGUIUtility.TextContent("x86"),
						BuildTarget.StandaloneWindows
					},

					{
						EditorGUIUtility.TextContent("x86_64"),
						BuildTarget.StandaloneWindows64
					}
				};
				IL_B6:
				return new Dictionary<GUIContent, BuildTarget>
				{

					{
						EditorGUIUtility.TextContent("x86"),
						BuildTarget.StandaloneOSXIntel
					},

					{
						EditorGUIUtility.TextContent("x86_64"),
						BuildTarget.StandaloneOSXIntel64
					},

					{
						EditorGUIUtility.TextContent("Universal"),
						BuildTarget.StandaloneOSXUniversal
					}
				};
			}
			public static BuildTarget DefaultTargetForPlatform(BuildTarget target)
			{
				switch (target)
				{
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
					return BuildTarget.StandaloneLinux;
				case BuildTarget.FlashPlayer:
				case (BuildTarget)20:
				case (BuildTarget)22:
				case (BuildTarget)23:
					IL_37:
					switch (target)
					{
					case BuildTarget.StandaloneOSXUniversal:
					case BuildTarget.StandaloneOSXIntel:
						return BuildTarget.StandaloneOSXIntel;
					case BuildTarget.StandaloneWindows:
						return BuildTarget.StandaloneWindows;
					}
					return target;
				case BuildTarget.StandaloneWindows64:
					return BuildTarget.StandaloneWindows;
				case BuildTarget.MetroPlayer:
					return BuildTarget.MetroPlayer;
				case BuildTarget.WP8Player:
					return BuildTarget.WP8Player;
				case BuildTarget.StandaloneOSXIntel64:
					return BuildTarget.StandaloneOSXIntel;
				}
				goto IL_37;
			}
			public int BuildPlatformIndexFromTargetGroup(BuildTargetGroup group)
			{
				for (int i = 0; i < this.buildPlatforms.Length; i++)
				{
					if (group == this.buildPlatforms[i].targetGroup)
					{
						return i;
					}
				}
				return -1;
			}
			public BuildPlayerWindow.BuildPlatform BuildPlatformFromTargetGroup(BuildTargetGroup group)
			{
				int num = this.BuildPlatformIndexFromTargetGroup(group);
				return (num == -1) ? null : this.buildPlatforms[num];
			}
		}
		private class Styles
		{
			public const float kButtonWidth = 110f;
			private const string kShopURL = "https://store.unity3d.com/shop/";
			private const string kDownloadURL = "http://unity3d.com/unity/download/";
			private const string kMailURL = "mailto:sales@unity3d.com";
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
			public GUIContent noSessionDialogText = EditorGUIUtility.TextContent("UploadingBuildsMonitor.NoSessionDialogText");
			public GUIContent platformTitle = EditorGUIUtility.TextContent("BuildSettings.PlatformTitle");
			public GUIContent switchPlatform = EditorGUIUtility.TextContent("BuildSettings.SwitchPlatform");
			public GUIContent build = EditorGUIUtility.TextContent("BuildSettings.Build");
			public GUIContent export = EditorGUIUtility.TextContent("BuildSettings.Export");
			public GUIContent buildAndRun = EditorGUIUtility.TextContent("BuildSettings.BuildAndRun");
			public GUIContent scenesInBuild = EditorGUIUtility.TextContent("BuildSettings.ScenesInBuild");
			public Texture2D activePlatformIcon = EditorGUIUtility.IconContent("BuildSettings.SelectedIcon").image as Texture2D;
			public GUIContent[,] notLicensedMessages;
			private GUIContent[,] buildTargetNotInstalled;
			public GUIContent sceTarget;
			public GUIContent psp2Target;
			public GUIContent flashTarget;
			public GUIContent metroBuildType;
			public GUIContent metroSDK;
			public GUIContent metroBuildAndRunDeployTarget;
			public GUIContent standaloneTarget;
			public GUIContent architecture;
			public GUIContent webPlayerStreamed;
			public GUIContent webPlayerOfflineDeployment;
			public GUIContent debugBuild;
			public GUIContent profileBuild;
			public GUIContent allowDebugging;
			public GUIContent createProject;
			public GUIContent symlinkiOSLibraries;
			public GUIContent xboxBuildSubtarget;
			public GUIContent xboxRunMethod;
			public GUIContent androidBuildSubtarget;
			public GUIContent explicitNullChecks;
			public GUIContent enableHeadlessMode;
			public GUIContent blackberryBuildSubtarget;
			public GUIContent blackberryBuildType;
			public GUIContent tizenBuildSubtarget;
			public Styles()
			{
				GUIContent[,] expr_128 = new GUIContent[17, 3];
				expr_128[0, 0] = EditorGUIUtility.TextContent("BuildSettings.NoWeb");
				expr_128[0, 1] = EditorGUIUtility.TextContent("BuildSettings.NoWebButton");
				expr_128[0, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[1, 0] = EditorGUIUtility.TextContent("BuildSettings.NoStandalone");
				expr_128[1, 1] = EditorGUIUtility.TextContent("BuildSettings.NoWebButton");
				expr_128[1, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[2, 0] = EditorGUIUtility.TextContent("BuildSettings.NoiPhone");
				expr_128[2, 1] = EditorGUIUtility.TextContent("BuildSettings.NoiPhoneButton");
				expr_128[2, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[3, 0] = EditorGUIUtility.TextContent("BuildSettings.NoAndroid");
				expr_128[3, 1] = EditorGUIUtility.TextContent("BuildSettings.NoAndroidButton");
				expr_128[3, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[4, 0] = EditorGUIUtility.TextContent("BuildSettings.NoBB10");
				expr_128[4, 1] = EditorGUIUtility.TextContent("BuildSettings.NoBB10Button");
				expr_128[4, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[5, 0] = EditorGUIUtility.TextContent("BuildSettings.NoTizen");
				expr_128[5, 1] = EditorGUIUtility.TextContent("BuildSettings.NoTizenButton");
				expr_128[5, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[6, 0] = EditorGUIUtility.TextContent("BuildSettings.NoXBox360");
				expr_128[6, 1] = EditorGUIUtility.TextContent("BuildSettings.NoXBox360Button");
				expr_128[6, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[7, 0] = EditorGUIUtility.TextContent("BuildSettings.NoXboxOne");
				expr_128[7, 1] = EditorGUIUtility.TextContent("BuildSettings.NoXboxOneButton");
				expr_128[7, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[8, 0] = EditorGUIUtility.TextContent("BuildSettings.NoPS3");
				expr_128[8, 1] = EditorGUIUtility.TextContent("BuildSettings.NoPS3Button");
				expr_128[8, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[9, 0] = EditorGUIUtility.TextContent("BuildSettings.NoPSP2");
				expr_128[9, 1] = EditorGUIUtility.TextContent("BuildSettings.NoPSP2Button");
				expr_128[9, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[10, 0] = EditorGUIUtility.TextContent("BuildSettings.NoPS4");
				expr_128[10, 1] = EditorGUIUtility.TextContent("BuildSettings.NoPS4Button");
				expr_128[10, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[11, 0] = EditorGUIUtility.TextContent("BuildSettings.NoPSM");
				expr_128[11, 1] = EditorGUIUtility.TextContent("BuildSettings.NoPSMButton");
				expr_128[11, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[12, 0] = EditorGUIUtility.TextContent("BuildSettings.NoGLESEmu");
				expr_128[12, 1] = EditorGUIUtility.TextContent("BuildSettings.NoGLESEmuButton");
				expr_128[12, 2] = new GUIContent("mailto:sales@unity3d.com");
				expr_128[13, 0] = EditorGUIUtility.TextContent("BuildSettings.NoFlash");
				expr_128[13, 1] = EditorGUIUtility.TextContent("BuildSettings.NoFlashButton");
				expr_128[13, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[14, 0] = EditorGUIUtility.TextContent("BuildSettings.NoMetro");
				expr_128[14, 1] = EditorGUIUtility.TextContent("BuildSettings.NoMetroButton");
				expr_128[14, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[15, 0] = EditorGUIUtility.TextContent("BuildSettings.NoWP8");
				expr_128[15, 1] = EditorGUIUtility.TextContent("BuildSettings.NoWP8Button");
				expr_128[15, 2] = new GUIContent("https://store.unity3d.com/shop/");
				expr_128[16, 0] = EditorGUIUtility.TextContent("BuildSettings.NoSamsungTV");
				expr_128[16, 1] = EditorGUIUtility.TextContent("BuildSettings.NoSamsungTVButton");
				expr_128[16, 2] = new GUIContent("https://store.unity3d.com/shop/");
				this.notLicensedMessages = expr_128;
				GUIContent[,] expr_4E4 = new GUIContent[17, 3];
				expr_4E4[0, 0] = EditorGUIUtility.TextContent("BuildSettings.WebNotInstalled");
				expr_4E4[0, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[1, 0] = EditorGUIUtility.TextContent("BuildSettings.StandaloneNotInstalled");
				expr_4E4[1, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[2, 0] = EditorGUIUtility.TextContent("BuildSettings.iPhoneNotInstalled");
				expr_4E4[2, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[3, 0] = EditorGUIUtility.TextContent("BuildSettings.AndroidNotInstalled");
				expr_4E4[3, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[4, 0] = EditorGUIUtility.TextContent("BuildSettings.BlackBerryNotInstalled");
				expr_4E4[4, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[5, 0] = EditorGUIUtility.TextContent("BuildSettings.TizenNotInstalled");
				expr_4E4[5, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[6, 0] = EditorGUIUtility.TextContent("BuildSettings.XBOX360NotInstalled");
				expr_4E4[6, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[7, 0] = EditorGUIUtility.TextContent("BuildSettings.XboxOneNotInstalled");
				expr_4E4[7, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[8, 0] = EditorGUIUtility.TextContent("BuildSettings.PS3NotInstalled");
				expr_4E4[8, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[9, 0] = EditorGUIUtility.TextContent("BuildSettings.PSP2NotInstalled");
				expr_4E4[9, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[10, 0] = EditorGUIUtility.TextContent("BuildSettings.PS4NotInstalled");
				expr_4E4[10, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[11, 0] = EditorGUIUtility.TextContent("BuildSettings.PSMNotInstalled");
				expr_4E4[11, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[12, 0] = EditorGUIUtility.TextContent("BuildSettings.GLESEmuNotInstalled");
				expr_4E4[12, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[13, 0] = EditorGUIUtility.TextContent("BuildSettings.FlashNotInstalled");
				expr_4E4[13, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[14, 0] = EditorGUIUtility.TextContent("BuildSettings.MetroNotInstalled");
				expr_4E4[14, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[15, 0] = EditorGUIUtility.TextContent("BuildSettings.WP8NotInstalled");
				expr_4E4[15, 2] = new GUIContent("http://unity3d.com/unity/download/");
				expr_4E4[16, 0] = EditorGUIUtility.TextContent("BuildSettings.SamsungTVNotInstalled");
				expr_4E4[16, 2] = new GUIContent("http://unity3d.com/unity/download/");
				this.buildTargetNotInstalled = expr_4E4;
				this.sceTarget = EditorGUIUtility.TextContent("BuildSettings.SCEBuildSubtarget");
				this.psp2Target = EditorGUIUtility.TextContent("BuildSettings.PSP2BuildSubtarget");
				this.flashTarget = EditorGUIUtility.TextContent("BuildSettings.FlashBuildSubtarget");
				this.metroBuildType = EditorGUIUtility.TextContent("BuildSettings.MetroBuildType");
				this.metroSDK = EditorGUIUtility.TextContent("BuildSettings.MetroSDK");
				this.metroBuildAndRunDeployTarget = EditorGUIUtility.TextContent("BuildSettings.MetroBuildAndRunDeployTarget");
				this.standaloneTarget = EditorGUIUtility.TextContent("BuildSettings.StandaloneTarget");
				this.architecture = EditorGUIUtility.TextContent("BuildSettings.Architecture");
				this.webPlayerStreamed = EditorGUIUtility.TextContent("BuildSettings.WebPlayerStreamed");
				this.webPlayerOfflineDeployment = EditorGUIUtility.TextContent("BuildSettings.WebPlayerOfflineDeployment");
				this.debugBuild = EditorGUIUtility.TextContent("BuildSettings.DebugBuild");
				this.profileBuild = EditorGUIUtility.TextContent("BuildSettings.ConnectProfiler");
				this.allowDebugging = EditorGUIUtility.TextContent("BuildSettings.AllowDebugging");
				this.createProject = EditorGUIUtility.TextContent("BuildSettings.ExportAndroidProject");
				this.symlinkiOSLibraries = EditorGUIUtility.TextContent("BuildSettings.SymlinkiOSLibraries");
				this.xboxBuildSubtarget = EditorGUIUtility.TextContent("BuildSettings.XboxBuildSubtarget");
				this.xboxRunMethod = EditorGUIUtility.TextContent("BuildSettings.XboxRunMethod");
				this.androidBuildSubtarget = EditorGUIUtility.TextContent("BuildSettings.AndroidBuildSubtarget");
				this.explicitNullChecks = EditorGUIUtility.TextContent("BuildSettings.ExplicitNullChecks");
				this.enableHeadlessMode = EditorGUIUtility.TextContent("BuildSettings.EnableHeadlessMode");
				this.blackberryBuildSubtarget = EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildSubtarget");
				this.blackberryBuildType = EditorGUIUtility.TextContent("BuildSettings.BlackBerryBuildType");
				this.tizenBuildSubtarget = EditorGUIUtility.TextContent("BuildSettings.TizenBuildSubtarget");
				base..ctor();
				this.levelStringCounter.alignment = TextAnchor.MiddleRight;
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
		private class ScenePostprocessor : AssetPostprocessor
		{
			private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
			{
				EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
				for (int i = 0; i < movedAssets.Length; i++)
				{
					string path = movedAssets[i];
					if (Path.GetExtension(path) == ".unity")
					{
						EditorBuildSettingsScene[] array = scenes;
						for (int j = 0; j < array.Length; j++)
						{
							EditorBuildSettingsScene editorBuildSettingsScene = array[j];
							if (editorBuildSettingsScene.path.ToLower() == movedFromPath[i].ToLower())
							{
								editorBuildSettingsScene.path = path;
							}
						}
					}
				}
				EditorBuildSettings.scenes = scenes;
			}
		}
		private const string kAssetsFolder = "Assets/";
		private const string kEditorBuildSettingsPath = "ProjectSettings/EditorBuildSettings.asset";
		private static BuildPlayerWindow.BuildPlatforms s_BuildPlatforms;
		private ListViewState lv = new ListViewState();
		private bool[] selectedLVItems = new bool[0];
		private bool[] selectedBeforeDrag;
		private int initialSelectedLVItem = -1;
		private Vector2 scrollPosition = new Vector2(0f, 0f);
		private static BuildPlayerWindow.Styles styles;
		public BuildPlayerWindow()
		{
			base.position = new Rect(50f, 50f, 540f, 530f);
			base.minSize = new Vector2(550f, 580f);
			base.title = "Build Settings";
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
		private static bool EnsureLoggedInAndBuild(bool askForBuildLocation, BuildOptions forceOptions)
		{
			if (string.IsNullOrEmpty(UploadingBuildsMonitor.GetActiveSessionID()))
			{
				AssetStoreLoginWindow.Login(BuildPlayerWindow.styles.noSessionDialogText.text, delegate(string errorMessage)
				{
					if (string.IsNullOrEmpty(errorMessage))
					{
						BuildPlayerWindow.BuildPlayerWithDefaultSettings(askForBuildLocation, forceOptions, false);
					}
					else
					{
						Debug.LogError(errorMessage);
					}
				});
				return false;
			}
			return true;
		}
		private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions)
		{
			return BuildPlayerWindow.BuildPlayerWithDefaultSettings(askForBuildLocation, forceOptions, true);
		}
		private static bool IsMetroPlayer(BuildTarget target)
		{
			return target == BuildTarget.MetroPlayer;
		}
		private static bool IsWP8Player(BuildTarget target)
		{
			return target == BuildTarget.WP8Player;
		}
		private static bool IsIl2cppBuild()
		{
			int num = 0;
			PlayerSettings.GetPropertyOptionalInt("ScriptingBackend", ref num, EditorUserBuildSettings.selectedBuildTargetGroup);
			return num == 1;
		}
		private static ScriptingImplementation GetDefaultNonIl2cppBackend()
		{
			return ScriptingImplementation.Mono2x;
		}
		private static bool BuildPlayerWithDefaultSettings(bool askForBuildLocation, BuildOptions forceOptions, bool first)
		{
			if (first && EditorUserBuildSettings.webPlayerDeployOnline && !BuildPlayerWindow.EnsureLoggedInAndBuild(askForBuildLocation, forceOptions))
			{
				return false;
			}
			BuildPlayerWindow.InitBuildPlatforms();
			BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
			if (!BuildPipeline.IsBuildTargetSupported(buildTarget))
			{
				return false;
			}
			if (Unsupported.IsBleedingEdgeBuild())
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("This version of Unity is a BleedingEdge build that has not seen any manual testing.");
				stringBuilder.AppendLine("You should consider this build unstable.");
				stringBuilder.AppendLine("We strongly recommend that you use a normal version of Unity instead.");
				if (EditorUtility.DisplayDialog("BleedingEdge Build", stringBuilder.ToString(), "Cancel", "OK"))
				{
					return false;
				}
			}
			if (buildTarget == BuildTarget.BB10 && (forceOptions & BuildOptions.AutoRunPlayer) != BuildOptions.None && (string.IsNullOrEmpty(PlayerSettings.BlackBerry.authorId) || string.IsNullOrEmpty(PlayerSettings.BlackBerry.deviceAddress) || string.IsNullOrEmpty(PlayerSettings.BlackBerry.devicePassword)))
			{
				Debug.LogError(EditorGUIUtility.TextContent("BuildSettings.BlackBerryValidationFailed").text);
				return false;
			}
			string text = string.Empty;
			bool flag = EditorUserBuildSettings.installInBuildFolder && PostprocessBuildPlayer.SupportsInstallInBuildFolder(buildTarget) && (Unsupported.IsDeveloperBuild() || BuildPlayerWindow.IsMetroPlayer(buildTarget) || BuildPlayerWindow.IsWP8Player(buildTarget));
			if (!flag)
			{
				if (EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.WebPlayer && EditorUserBuildSettings.webPlayerDeployOnline)
				{
					text = Path.Combine(Path.Combine(Path.GetTempPath(), "Unity"), "UDNBuild");
					try
					{
						Directory.CreateDirectory(text);
					}
					catch (Exception)
					{
					}
					if (!Directory.Exists(text))
					{
						Debug.LogError(string.Format("Failed to create temporary build directory at {0}", text));
						return false;
					}
					text = text.Replace('\\', '/');
				}
				else
				{
					if (askForBuildLocation && !BuildPlayerWindow.PickBuildLocation(buildTarget))
					{
						return false;
					}
					text = EditorUserBuildSettings.GetBuildLocation(buildTarget);
				}
				if (text.Length == 0)
				{
					return false;
				}
				if (!askForBuildLocation)
				{
					switch (InternalEditorUtility.BuildCanBeAppended(buildTarget, text))
					{
					case CanAppendBuild.Yes:
						EditorUserBuildSettings.appendProject = true;
						break;
					case CanAppendBuild.No:
						if (!BuildPlayerWindow.PickBuildLocation(buildTarget))
						{
							return false;
						}
						text = EditorUserBuildSettings.GetBuildLocation(buildTarget);
						if (text.Length == 0 || !Directory.Exists(FileUtil.DeleteLastPathNameComponent(text)))
						{
							return false;
						}
						break;
					}
				}
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
			if (EditorUserBuildSettings.appendProject)
			{
				buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
			}
			if (EditorUserBuildSettings.webPlayerDeployOnline)
			{
				buildOptions |= BuildOptions.DeployOnline;
			}
			if (EditorUserBuildSettings.webPlayerOfflineDeployment)
			{
				buildOptions |= BuildOptions.WebPlayerOfflineDeployment;
			}
			if (EditorUserBuildSettings.enableHeadlessMode)
			{
				buildOptions |= BuildOptions.EnableHeadlessMode;
			}
			if (EditorUserBuildSettings.connectProfiler && (development || buildTarget == BuildTarget.MetroPlayer || buildTarget == BuildTarget.WP8Player))
			{
				buildOptions |= BuildOptions.ConnectWithProfiler;
			}
			if (flag)
			{
				buildOptions |= BuildOptions.InstallInBuildFolder;
			}
			bool delayToAfterScriptReload = false;
			if (EditorUserBuildSettings.activeBuildTarget != buildTarget)
			{
				if (!EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget))
				{
					Debug.LogError(string.Format("Could not switch to build target '{0}'.", BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(buildTarget)));
					return false;
				}
				if (EditorApplication.isCompiling)
				{
					delayToAfterScriptReload = true;
				}
			}
			uint num = 0u;
			string text2 = BuildPipeline.BuildPlayerInternalNoCheck(levels, text, buildTarget, buildOptions, delayToAfterScriptReload, out num);
			return text2.Length == 0;
		}
		private void ActiveScenesGUI()
		{
			int num = 0;
			int row = this.lv.row;
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			Event current = Event.current;
			Rect rect = GUILayoutUtility.GetRect(BuildPlayerWindow.styles.scenesInBuild, BuildPlayerWindow.styles.title);
			ArrayList arrayList = new ArrayList(EditorBuildSettings.scenes);
			this.lv.totalRows = arrayList.Count;
			if (this.selectedLVItems.Length != arrayList.Count)
			{
				Array.Resize<bool>(ref this.selectedLVItems, arrayList.Count);
			}
			int[] array = new int[arrayList.Count];
			for (int i = 0; i < array.Length; i++)
			{
				EditorBuildSettingsScene editorBuildSettingsScene = (EditorBuildSettingsScene)arrayList[i];
				array[i] = num;
				if (editorBuildSettingsScene.enabled)
				{
					num++;
				}
			}
			foreach (ListViewElement listViewElement in ListViewGUILayout.ListView(this.lv, (ListViewOptions)3, BuildPlayerWindow.styles.box, new GUILayoutOption[0]))
			{
				EditorBuildSettingsScene editorBuildSettingsScene2 = (EditorBuildSettingsScene)arrayList[listViewElement.row];
				bool flag = File.Exists(editorBuildSettingsScene2.path);
				EditorGUI.BeginDisabledGroup(!flag);
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
				editorBuildSettingsScene2.enabled = GUI.Toggle(position, editorBuildSettingsScene2.enabled, string.Empty);
				if (EditorGUI.EndChangeCheck() && flag2)
				{
					for (int j = 0; j < arrayList.Count; j++)
					{
						if (this.selectedLVItems[j])
						{
							((EditorBuildSettingsScene)arrayList[j]).enabled = editorBuildSettingsScene2.enabled;
						}
					}
				}
				GUILayout.Space(BuildPlayerWindow.styles.toggleSize.x);
				string text = editorBuildSettingsScene2.path;
				if (text.StartsWith("Assets/"))
				{
					text = text.Substring("Assets/".Length);
				}
				Rect rect2 = GUILayoutUtility.GetRect(EditorGUIUtility.TempContent(text), BuildPlayerWindow.styles.levelString);
				if (Event.current.type == EventType.Repaint)
				{
					BuildPlayerWindow.styles.levelString.Draw(rect2, EditorGUIUtility.TempContent(text), false, false, flag2, false);
				}
				GUILayout.Label((!editorBuildSettingsScene2.enabled) ? string.Empty : array[listViewElement.row].ToString(), BuildPlayerWindow.styles.levelStringCounter, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(36f)
				});
				EditorGUI.EndDisabledGroup();
				if (ListViewGUILayout.HasMouseUp(listViewElement.position) && !shift && !actionKey)
				{
					if (!shift && !actionKey)
					{
						ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
					}
				}
				else
				{
					if (ListViewGUILayout.HasMouseDown(listViewElement.position))
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
			GUI.Label(rect, BuildPlayerWindow.styles.scenesInBuild, BuildPlayerWindow.styles.title);
			if (GUIUtility.keyboardControl == this.lv.ID)
			{
				if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
				{
					Event.current.Use();
				}
				else
				{
					if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
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
			}
			if (this.lv.selectionChanged)
			{
				ListViewGUILayout.MultiSelection(row, this.lv.row, ref this.initialSelectedLVItem, ref this.selectedLVItems);
			}
			if (this.lv.fileNames != null)
			{
				Array.Sort<string>(this.lv.fileNames);
				int num2 = 0;
				for (int i = 0; i < this.lv.fileNames.Length; i++)
				{
					if (this.lv.fileNames[i].EndsWith("unity"))
					{
						EditorBuildSettingsScene editorBuildSettingsScene3 = new EditorBuildSettingsScene();
						editorBuildSettingsScene3.path = FileUtil.GetProjectRelativePath(this.lv.fileNames[i]);
						if (editorBuildSettingsScene3.path == string.Empty)
						{
							editorBuildSettingsScene3.path = this.lv.fileNames[i];
						}
						editorBuildSettingsScene3.enabled = true;
						arrayList.Insert(this.lv.draggedTo + num2++, editorBuildSettingsScene3);
					}
				}
				if (num2 != 0)
				{
					Array.Resize<bool>(ref this.selectedLVItems, arrayList.Count);
					for (int i = 0; i < this.selectedLVItems.Length; i++)
					{
						this.selectedLVItems[i] = (i >= this.lv.draggedTo && i < this.lv.draggedTo + num2);
					}
				}
				this.lv.draggedTo = -1;
			}
			if (this.lv.draggedTo != -1)
			{
				ArrayList arrayList2 = new ArrayList();
				int num3 = 0;
				int i = 0;
				while (i < this.selectedLVItems.Length)
				{
					if (this.selectedBeforeDrag[i])
					{
						arrayList2.Add(arrayList[num3]);
						arrayList.RemoveAt(num3);
						num3--;
						if (this.lv.draggedTo >= i)
						{
							this.lv.draggedTo--;
						}
					}
					i++;
					num3++;
				}
				this.lv.draggedTo = ((this.lv.draggedTo <= arrayList.Count && this.lv.draggedTo >= 0) ? this.lv.draggedTo : arrayList.Count);
				arrayList.InsertRange(this.lv.draggedTo, arrayList2);
				for (i = 0; i < this.selectedLVItems.Length; i++)
				{
					this.selectedLVItems[i] = (i >= this.lv.draggedTo && i < this.lv.draggedTo + arrayList2.Count);
				}
			}
			if (current.type == EventType.KeyDown && (current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete))
			{
				int num3 = 0;
				int i = 0;
				while (i < this.selectedLVItems.Length)
				{
					if (this.selectedLVItems[i])
					{
						arrayList.RemoveAt(num3);
						num3--;
					}
					this.selectedLVItems[i] = false;
					i++;
					num3++;
				}
				this.lv.row = -1;
				current.Use();
			}
			EditorBuildSettings.scenes = (arrayList.ToArray(typeof(EditorBuildSettingsScene)) as EditorBuildSettingsScene[]);
		}
		private void AddCurrentScene()
		{
			string currentScene = EditorApplication.currentScene;
			if (currentScene.Length == 0)
			{
				EditorApplication.SaveCurrentSceneIfUserWantsToForce();
				currentScene = EditorApplication.currentScene;
			}
			if (currentScene.Length != 0)
			{
				EditorBuildSettings.scenes = (new ArrayList(EditorBuildSettings.scenes)
				{
					new EditorBuildSettingsScene
					{
						path = currentScene,
						enabled = true
					}
				}.ToArray(typeof(EditorBuildSettingsScene)) as EditorBuildSettingsScene[]);
			}
			base.Repaint();
			GUIUtility.ExitGUI();
		}
		private static BuildTarget CalculateSelectedBuildTarget()
		{
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			BuildTargetGroup buildTargetGroup = selectedBuildTargetGroup;
			if (buildTargetGroup == BuildTargetGroup.Standalone)
			{
				return EditorUserBuildSettings.selectedStandaloneTarget;
			}
			if (buildTargetGroup == BuildTargetGroup.WebPlayer)
			{
				return (!EditorUserBuildSettings.webPlayerStreamed) ? BuildTarget.WebPlayer : BuildTarget.WebPlayerStreamed;
			}
			if (BuildPlayerWindow.s_BuildPlatforms == null)
			{
				throw new Exception("Build platforms are not initialized.");
			}
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformFromTargetGroup(selectedBuildTargetGroup);
			if (buildPlatform == null)
			{
				throw new Exception("Could not find build platform for target group " + selectedBuildTargetGroup);
			}
			return buildPlatform.DefaultTarget;
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
							this.ShowOption(buildPlatform, buildPlatform.title, (!flag2) ? BuildPlayerWindow.styles.oddRow : BuildPlayerWindow.styles.evenRow);
							flag2 = !flag2;
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
			}
			GUI.enabled = BuildPipeline.IsBuildTargetSupported(buildTarget);
			if (GUILayout.Button(new GUIContent("Player Settings..."), new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			GUILayout.EndVertical();
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
			if (GUI.Toggle(rect, flag2, title.text, BuildPlayerWindow.styles.platformSelector) && EditorUserBuildSettings.selectedBuildTargetGroup != bp.targetGroup)
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
		private void OnGUI()
		{
			if (BuildPlayerWindow.styles == null)
			{
				BuildPlayerWindow.styles = new BuildPlayerWindow.Styles();
				BuildPlayerWindow.styles.toggleSize = BuildPlayerWindow.styles.toggle.CalcSize(new GUIContent("X"));
				this.lv.rowHeight = (int)BuildPlayerWindow.styles.levelString.CalcHeight(new GUIContent("X"), 100f);
			}
			BuildPlayerWindow.InitBuildPlatforms();
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			string empty = string.Empty;
			bool flag = !AssetDatabase.IsOpenForEdit("ProjectSettings/EditorBuildSettings.asset", out empty);
			EditorGUI.BeginDisabledGroup(flag);
			this.ActiveScenesGUI();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (flag)
			{
				GUI.enabled = true;
				if (Provider.enabled && GUILayout.Button("Checkout", new GUILayoutOption[0]))
				{
					Asset assetByPath = Provider.GetAssetByPath("ProjectSettings/EditorBuildSettings.asset");
					Task task = Provider.Checkout(new AssetList
					{
						assetByPath
					}, CheckoutMode.Both);
					task.SetCompletionAction(CompletionAction.UpdatePendingWindow);
				}
				GUILayout.Label(empty, new GUILayoutOption[0]);
				GUI.enabled = false;
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add Current", new GUILayoutOption[0]))
			{
				this.AddCurrentScene();
			}
			GUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
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
		private static BuildTarget RestoreLastKnownPlatformsBuildTarget(BuildPlayerWindow.BuildPlatform bp)
		{
			BuildTargetGroup targetGroup = bp.targetGroup;
			if (targetGroup == BuildTargetGroup.Standalone)
			{
				return EditorUserBuildSettings.selectedStandaloneTarget;
			}
			if (targetGroup != BuildTargetGroup.WebPlayer)
			{
				return bp.DefaultTarget;
			}
			return (!EditorUserBuildSettings.webPlayerStreamed) ? BuildTarget.WebPlayer : BuildTarget.WebPlayerStreamed;
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
		internal static bool IsXboxBuildSubtargetDevelopment(XboxBuildSubtarget target)
		{
			return target != XboxBuildSubtarget.Master;
		}
		private static void RepairSelectedBuildTargetGroup()
		{
			BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			if (selectedBuildTargetGroup == BuildTargetGroup.Unknown || BuildPlayerWindow.s_BuildPlatforms == null || BuildPlayerWindow.s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(selectedBuildTargetGroup) < 0)
			{
				EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.WebPlayer;
			}
		}
		private void ShowBuildTargetSettings()
		{
			EditorGUIUtility.labelWidth = Mathf.Min(180f, (base.position.width - 265f) * 0.47f);
			BuildTarget buildTarget = BuildPlayerWindow.CalculateSelectedBuildTarget();
			BuildPlayerWindow.BuildPlatform buildPlatform = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformFromTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			GUILayout.Space(18f);
			Rect rect = GUILayoutUtility.GetRect(50f, 36f);
			rect.x += 1f;
			GUI.Label(new Rect(rect.x + 3f, rect.y + 3f, 32f, 32f), buildPlatform.title.image, GUIStyle.none);
			GUI.Toggle(rect, false, buildPlatform.title.text, BuildPlayerWindow.styles.platformSelector);
			GUILayout.Space(10f);
			if (!string.IsNullOrEmpty(ModuleManager.GetTargetStringFromBuildTarget(buildTarget)) && ModuleManager.GetBuildPostProcessor(buildTarget) == null)
			{
				GUILayout.Label("No " + BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(buildTarget) + " module loaded.", new GUILayoutOption[0]);
				if (GUILayout.Button("Module Manager", EditorStyles.miniButton, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					InternalEditorUtility.ShowPackageManagerWindow();
				}
				BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
				return;
			}
			GUIContent downloadErrorForTarget = BuildPlayerWindow.styles.GetDownloadErrorForTarget(buildTarget);
			if (downloadErrorForTarget != null)
			{
				GUILayout.Label(downloadErrorForTarget, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
				return;
			}
			if (!BuildPipeline.LicenseCheck(buildTarget))
			{
				int num = BuildPlayerWindow.s_BuildPlatforms.BuildPlatformIndexFromTargetGroup(buildPlatform.targetGroup);
				GUILayout.Label(BuildPlayerWindow.styles.notLicensedMessages[num, 0], EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (BuildPlayerWindow.styles.notLicensedMessages[num, 1].text.Length != 0 && GUILayout.Button(BuildPlayerWindow.styles.notLicensedMessages[num, 1], new GUILayoutOption[0]))
				{
					Application.OpenURL(BuildPlayerWindow.styles.notLicensedMessages[num, 2].text);
				}
				GUILayout.EndHorizontal();
				BuildPlayerWindow.GUIBuildButtons(false, false, false, buildPlatform);
				return;
			}
			GUI.changed = false;
			switch (buildPlatform.targetGroup)
			{
			case BuildTargetGroup.Standalone:
			{
				BuildTarget selectedStandaloneTarget = EditorUserBuildSettings.selectedStandaloneTarget;
				int num2 = Math.Max(0, Array.IndexOf<BuildTarget>(BuildPlayerWindow.s_BuildPlatforms.standaloneSubtargets, BuildPlayerWindow.BuildPlatforms.DefaultTargetForPlatform(selectedStandaloneTarget)));
				int num3 = EditorGUILayout.Popup(BuildPlayerWindow.styles.standaloneTarget, num2, BuildPlayerWindow.s_BuildPlatforms.standaloneSubtargetStrings, new GUILayoutOption[0]);
				Dictionary<GUIContent, BuildTarget> architecturesForPlatform = BuildPlayerWindow.BuildPlatforms.GetArchitecturesForPlatform(selectedStandaloneTarget);
				BuildTarget buildTarget2;
				if (num3 == num2 && architecturesForPlatform != null)
				{
					GUIContent[] array = new List<GUIContent>(architecturesForPlatform.Keys).ToArray();
					int num4 = 0;
					if (num3 == num2)
					{
						foreach (KeyValuePair<GUIContent, BuildTarget> current in architecturesForPlatform)
						{
							if (current.Value == selectedStandaloneTarget)
							{
								num4 = Math.Max(0, Array.IndexOf<GUIContent>(array, current.Key));
								break;
							}
						}
					}
					num4 = EditorGUILayout.Popup(BuildPlayerWindow.styles.architecture, num4, array, new GUILayoutOption[0]);
					buildTarget2 = architecturesForPlatform[array[num4]];
				}
				else
				{
					buildTarget2 = BuildPlayerWindow.s_BuildPlatforms.standaloneSubtargets[num3];
					if (BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone)
					{
						EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget2);
					}
				}
				EditorUserBuildSettings.selectedStandaloneTarget = buildTarget2;
				break;
			}
			case BuildTargetGroup.WebPlayer:
			{
				EditorUserBuildSettings.webPlayerDeployOnline = false;
				GUI.enabled = BuildPipeline.LicenseCheck(BuildTarget.WebPlayerStreamed);
				bool webPlayerStreamed = EditorGUILayout.Toggle(BuildPlayerWindow.styles.webPlayerStreamed, EditorUserBuildSettings.webPlayerStreamed, new GUILayoutOption[0]);
				if (GUI.changed)
				{
					EditorUserBuildSettings.webPlayerStreamed = webPlayerStreamed;
				}
				bool enabled = GUI.enabled;
				if (EditorUserBuildSettings.webPlayerDeployOnline)
				{
					GUI.enabled = false;
				}
				EditorUserBuildSettings.webPlayerOfflineDeployment = EditorGUILayout.Toggle(BuildPlayerWindow.styles.webPlayerOfflineDeployment, EditorUserBuildSettings.webPlayerOfflineDeployment, new GUILayoutOption[0]);
				GUI.enabled = enabled;
				break;
			}
			case BuildTargetGroup.iPhone:
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					EditorUserBuildSettings.symlinkLibraries = EditorGUILayout.Toggle(BuildPlayerWindow.styles.symlinkiOSLibraries, EditorUserBuildSettings.symlinkLibraries, new GUILayoutOption[0]);
				}
				EditorUserBuildSettings.appendProject = false;
				break;
			case BuildTargetGroup.PS3:
			{
				int num5 = Array.IndexOf<SCEBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.sceSubtargets, EditorUserBuildSettings.sceBuildSubtarget);
				if (num5 == -1)
				{
					num5 = 0;
				}
				num5 = EditorGUILayout.Popup(BuildPlayerWindow.styles.sceTarget, num5, BuildPlayerWindow.s_BuildPlatforms.sceSubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.sceBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.sceSubtargets[num5];
				break;
			}
			case BuildTargetGroup.XBOX360:
			{
				int num6 = Array.IndexOf<XboxBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.xboxBuildSubtargets, EditorUserBuildSettings.xboxBuildSubtarget);
				if (num6 == -1)
				{
					num6 = 0;
				}
				num6 = EditorGUILayout.Popup(BuildPlayerWindow.styles.xboxBuildSubtarget, num6, BuildPlayerWindow.s_BuildPlatforms.xboxBuildSubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.xboxBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.xboxBuildSubtargets[num6];
				num6 = Array.IndexOf<XboxRunMethod>(BuildPlayerWindow.s_BuildPlatforms.xboxRunMethods, EditorUserBuildSettings.xboxRunMethod);
				if (num6 == -1)
				{
					num6 = 0;
				}
				num6 = EditorGUILayout.Popup(BuildPlayerWindow.styles.xboxRunMethod, num6, BuildPlayerWindow.s_BuildPlatforms.xboxRunMethodStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.xboxRunMethod = BuildPlayerWindow.s_BuildPlatforms.xboxRunMethods[num6];
				break;
			}
			case BuildTargetGroup.Android:
			{
				int num7 = Array.IndexOf<AndroidBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.androidBuildSubtargets, EditorUserBuildSettings.androidBuildSubtarget);
				if (num7 == -1)
				{
					num7 = 0;
				}
				num7 = EditorGUILayout.Popup(BuildPlayerWindow.styles.androidBuildSubtarget, num7, BuildPlayerWindow.s_BuildPlatforms.androidBuildSubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.androidBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.androidBuildSubtargets[num7];
				bool flag = EditorUserBuildSettings.installInBuildFolder && PostprocessBuildPlayer.SupportsInstallInBuildFolder(buildTarget);
				bool arg_632_0 = EditorUserBuildSettings.appendProject;
				bool flag2 = !flag;
				GUI.enabled = flag2;
				EditorUserBuildSettings.appendProject = (arg_632_0 & flag2);
				EditorUserBuildSettings.appendProject = EditorGUILayout.Toggle(BuildPlayerWindow.styles.createProject, EditorUserBuildSettings.appendProject, new GUILayoutOption[0]);
				GUI.enabled = true;
				break;
			}
			case BuildTargetGroup.FlashPlayer:
			{
				int num8 = Array.IndexOf<FlashBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.flashBuildSubtargets, EditorUserBuildSettings.flashBuildSubtarget);
				if (num8 == -1)
				{
					num8 = 0;
				}
				num8 = EditorGUILayout.Popup(BuildPlayerWindow.styles.flashTarget, num8, BuildPlayerWindow.s_BuildPlatforms.flashBuildSubtargetString, new GUILayoutOption[0]);
				EditorUserBuildSettings.flashBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.flashBuildSubtargets[num8];
				break;
			}
			case BuildTargetGroup.Metro:
			{
				int num9 = Math.Max(0, Array.IndexOf<MetroBuildType>(BuildPlayerWindow.s_BuildPlatforms.metroBuildTypes, EditorUserBuildSettings.metroBuildType));
				num9 = EditorGUILayout.Popup(BuildPlayerWindow.styles.metroBuildType, num9, BuildPlayerWindow.s_BuildPlatforms.metroBuildTypeStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.metroBuildType = BuildPlayerWindow.s_BuildPlatforms.metroBuildTypes[num9];
				num9 = Math.Max(0, Array.IndexOf<MetroSDK>(BuildPlayerWindow.s_BuildPlatforms.metroSDKs, EditorUserBuildSettings.metroSDK));
				num9 = EditorGUILayout.Popup(BuildPlayerWindow.styles.metroSDK, num9, BuildPlayerWindow.s_BuildPlatforms.metroSDKStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.metroSDK = BuildPlayerWindow.s_BuildPlatforms.metroSDKs[num9];
				GUI.enabled = (EditorUserBuildSettings.metroSDK == MetroSDK.UniversalSDK81);
				num9 = Math.Max(0, Array.IndexOf<MetroBuildAndRunDeployTarget>(BuildPlayerWindow.s_BuildPlatforms.metroBuildAndRunDeployTargets, EditorUserBuildSettings.metroBuildAndRunDeployTarget));
				num9 = EditorGUILayout.Popup(BuildPlayerWindow.styles.metroBuildAndRunDeployTarget, num9, BuildPlayerWindow.s_BuildPlatforms.metroBuildAndRunDeployTargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.metroBuildAndRunDeployTarget = BuildPlayerWindow.s_BuildPlatforms.metroBuildAndRunDeployTargets[num9];
				GUI.enabled = true;
				EditorGUILayout.LabelField(EditorGUIUtility.TextContent("BuildSettings.MetroDebugging"), new GUILayoutOption[0]);
				GUI.enabled = (EditorUserBuildSettings.metroBuildType == MetroBuildType.VisualStudioCSharp || EditorUserBuildSettings.metroBuildType == MetroBuildType.VisualStudioCSharpDX);
				EditorUserBuildSettings.metroGenerateReferenceProjects = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("BuildSettings.MetroGenerateReferenceProjects"), EditorUserBuildSettings.metroGenerateReferenceProjects, new GUILayoutOption[0]);
				GUI.enabled = true;
				break;
			}
			case BuildTargetGroup.BB10:
			{
				int num10 = Array.IndexOf<BlackBerryBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.blackberryBuildSubtargets, EditorUserBuildSettings.blackberryBuildSubtarget);
				if (num10 == -1)
				{
					num10 = 0;
				}
				num10 = EditorGUILayout.Popup(BuildPlayerWindow.styles.blackberryBuildSubtarget, num10, BuildPlayerWindow.s_BuildPlatforms.blackberryBuildSubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.blackberryBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.blackberryBuildSubtargets[num10];
				num10 = Array.IndexOf<BlackBerryBuildType>(BuildPlayerWindow.s_BuildPlatforms.blackberryBuildTypes, EditorUserBuildSettings.blackberryBuildType);
				if (num10 == -1)
				{
					num10 = 0;
				}
				num10 = EditorGUILayout.Popup(BuildPlayerWindow.styles.blackberryBuildType, num10, BuildPlayerWindow.s_BuildPlatforms.blackberryBuildTypeStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.blackberryBuildType = BuildPlayerWindow.s_BuildPlatforms.blackberryBuildTypes[num10];
				GUI.enabled = true;
				break;
			}
			case BuildTargetGroup.Tizen:
			{
				int num11 = Array.IndexOf<TizenBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.tizenBuildSubtargets, EditorUserBuildSettings.tizenBuildSubtarget);
				if (num11 == -1)
				{
					num11 = 0;
				}
				num11 = EditorGUILayout.Popup(BuildPlayerWindow.styles.tizenBuildSubtarget, num11, BuildPlayerWindow.s_BuildPlatforms.tizenBuildSubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.tizenBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.tizenBuildSubtargets[num11];
				GUI.enabled = true;
				break;
			}
			case BuildTargetGroup.PSP2:
			{
				int num12 = Array.IndexOf<PSP2BuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.psp2Subtargets, EditorUserBuildSettings.psp2BuildSubtarget);
				if (num12 == -1)
				{
					num12 = 0;
				}
				num12 = EditorGUILayout.Popup(BuildPlayerWindow.styles.psp2Target, num12, BuildPlayerWindow.s_BuildPlatforms.psp2SubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.psp2BuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.psp2Subtargets[num12];
				break;
			}
			case BuildTargetGroup.PS4:
			{
				int num13 = Array.IndexOf<SCEBuildSubtarget>(BuildPlayerWindow.s_BuildPlatforms.sceSubtargets, EditorUserBuildSettings.sceBuildSubtarget);
				if (num13 == -1)
				{
					num13 = 0;
				}
				num13 = EditorGUILayout.Popup(BuildPlayerWindow.styles.sceTarget, num13, BuildPlayerWindow.s_BuildPlatforms.sceSubtargetStrings, new GUILayoutOption[0]);
				EditorUserBuildSettings.sceBuildSubtarget = BuildPlayerWindow.s_BuildPlatforms.sceSubtargets[num13];
				break;
			}
			}
			GUI.enabled = true;
			bool flag3 = true;
			bool enableBuildAndRunButton = false;
			bool flag4 = buildTarget != BuildTarget.PS3 && buildTarget != BuildTarget.PSP2 && buildTarget != BuildTarget.PS4 && buildTarget != BuildTarget.PSM && buildTarget != BuildTarget.FlashPlayer && buildTarget != BuildTarget.NaCl && !BuildPlayerWindow.IsMetroPlayer(buildTarget) && !BuildPlayerWindow.IsWP8Player(buildTarget);
			bool flag5 = buildTarget == BuildTarget.XBOX360 || buildTarget == BuildTarget.PS3 || buildTarget == BuildTarget.PSP2 || buildTarget == BuildTarget.PS4 || buildTarget == BuildTarget.PSM;
			bool flag6 = (buildTarget == BuildTarget.StandaloneLinux || buildTarget == BuildTarget.StandaloneLinux64 || buildTarget == BuildTarget.StandaloneLinuxUniversal) && Application.HasProLicense();
			bool canInstallInBuildFolder = false;
			if ((BuildPlayerWindow.IsMetroPlayer(buildTarget) || BuildPlayerWindow.IsWP8Player(buildTarget)) && !InternalEditorUtility.RunningUnderWindows8())
			{
				flag3 = false;
				GUILayout.Label(EditorGUIUtility.TextContent("BuildSettings.NoWindows8"), EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			}
			if (BuildPipeline.IsBuildTargetSupported(buildTarget))
			{
				bool flag7 = buildTarget == BuildTarget.PS3 && EditorUserBuildSettings.sceBuildSubtarget != SCEBuildSubtarget.PCHosted;
				bool flag8 = buildTarget == BuildTarget.PS4 && EditorUserBuildSettings.sceBuildSubtarget != SCEBuildSubtarget.PCHosted;
				bool flag9 = buildTarget == BuildTarget.PSP2 && EditorUserBuildSettings.psp2BuildSubtarget != PSP2BuildSubtarget.PCHosted;
				bool flag10 = flag7 || flag8 || flag9;
				bool flag11 = buildTarget == BuildTarget.PSP2 || BuildTarget.PS4 == buildTarget;
				bool flag12 = buildTarget != BuildTarget.FlashPlayer && buildTarget != BuildTarget.NaCl && !BuildPlayerWindow.IsWP8Player(buildTarget);
				bool flag13 = !flag10;
				GUI.enabled = flag13;
				BuildTargetGroup targetGroup = buildPlatform.targetGroup;
				if (targetGroup != BuildTargetGroup.XBOX360)
				{
					EditorUserBuildSettings.development = EditorGUILayout.Toggle(BuildPlayerWindow.styles.debugBuild, EditorUserBuildSettings.development, new GUILayoutOption[0]);
				}
				else
				{
					EditorUserBuildSettings.development = BuildPlayerWindow.IsXboxBuildSubtargetDevelopment(EditorUserBuildSettings.xboxBuildSubtarget);
				}
				bool development = EditorUserBuildSettings.development;
				GUI.enabled = (flag13 && development);
				GUI.enabled = (GUI.enabled && InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(buildTarget));
				GUI.enabled = (GUI.enabled && buildPlatform.targetGroup != BuildTargetGroup.WebPlayer);
				if (flag12)
				{
					if (!GUI.enabled)
					{
						if (!InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(buildTarget))
						{
							BuildPlayerWindow.styles.profileBuild.tooltip = "Pro Version for selected platform is needed for profiling";
						}
						else
						{
							if (!development)
							{
								BuildPlayerWindow.styles.profileBuild.tooltip = "Profiling only enabled in Development Player";
							}
							else
							{
								if (buildPlatform.targetGroup == BuildTargetGroup.WebPlayer)
								{
									BuildPlayerWindow.styles.profileBuild.tooltip = "Autoconnect not available from webplayer. Manually connect in Profiler";
								}
							}
						}
					}
					else
					{
						BuildPlayerWindow.styles.profileBuild.tooltip = string.Empty;
					}
					EditorUserBuildSettings.connectProfiler = EditorGUILayout.Toggle(BuildPlayerWindow.styles.profileBuild, EditorUserBuildSettings.connectProfiler, new GUILayoutOption[0]);
				}
				GUI.enabled = (flag13 && development);
				if (flag4)
				{
					bool enabled2 = GUI.enabled;
					GUI.enabled = (buildTarget != BuildTarget.iPhone || !BuildPlayerWindow.IsIl2cppBuild());
					BuildPlayerWindow.styles.allowDebugging.tooltip = string.Empty;
					if (!GUI.enabled)
					{
						BuildPlayerWindow.styles.allowDebugging.tooltip = "Debugging projects that target the IL2CPP scripting backend is not supported";
					}
					EditorUserBuildSettings.allowDebugging = EditorGUILayout.Toggle(BuildPlayerWindow.styles.allowDebugging, EditorUserBuildSettings.allowDebugging, new GUILayoutOption[0]);
					GUI.enabled = enabled2;
				}
				if (flag5)
				{
					GUI.enabled = (!flag13 || !development);
					if (!GUI.enabled)
					{
						EditorUserBuildSettings.explicitNullChecks = true;
					}
					EditorUserBuildSettings.explicitNullChecks = EditorGUILayout.Toggle(BuildPlayerWindow.styles.explicitNullChecks, EditorUserBuildSettings.explicitNullChecks, new GUILayoutOption[0]);
				}
				GUI.enabled = !development;
				if (flag6)
				{
					EditorUserBuildSettings.enableHeadlessMode = EditorGUILayout.Toggle(BuildPlayerWindow.styles.enableHeadlessMode, EditorUserBuildSettings.enableHeadlessMode && !development, new GUILayoutOption[0]);
				}
				GUI.enabled = true;
				GUILayout.FlexibleSpace();
				canInstallInBuildFolder = (!flag10 && Unsupported.IsDeveloperBuild() && PostprocessBuildPlayer.SupportsInstallInBuildFolder(buildTarget));
				if (flag3)
				{
					enableBuildAndRunButton = ((!flag10 && !EditorUserBuildSettings.installInBuildFolder) || flag11);
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
			BuildPlayerWindow.GUIBuildButtons(flag3, enableBuildAndRunButton, canInstallInBuildFolder, buildPlatform);
		}
		private static void GUIBuildButtons(bool enableBuildButton, bool enableBuildAndRunButton, bool canInstallInBuildFolder, BuildPlayerWindow.BuildPlatform platform)
		{
			GUILayout.FlexibleSpace();
			if (canInstallInBuildFolder)
			{
				EditorUserBuildSettings.installInBuildFolder = GUILayout.Toggle(EditorUserBuildSettings.installInBuildFolder, "Install in Builds folder\n(for debugging with source code)", new GUILayoutOption[0]);
			}
			else
			{
				EditorUserBuildSettings.installInBuildFolder = false;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (platform.targetGroup == BuildTargetGroup.Android && EditorUserBuildSettings.appendProject)
			{
				enableBuildAndRunButton = false;
			}
			GUIContent content = BuildPlayerWindow.styles.build;
			if (platform.targetGroup == BuildTargetGroup.Android && EditorUserBuildSettings.appendProject)
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
		private static bool PickBuildLocation(BuildTarget target)
		{
			string buildLocation = EditorUserBuildSettings.GetBuildLocation(target);
			if (target == BuildTarget.Android && EditorUserBuildSettings.appendProject)
			{
				string title = "Export Google Android Project";
				string location = EditorUtility.SaveFolderPanel(title, buildLocation, string.Empty);
				EditorUserBuildSettings.SetBuildLocation(target, location);
				return true;
			}
			string extensionForBuildTarget = PostprocessBuildPlayer.GetExtensionForBuildTarget(target);
			string directory = FileUtil.DeleteLastPathNameComponent(buildLocation);
			string lastPathNameComponent = FileUtil.GetLastPathNameComponent(buildLocation);
			string title2 = "Build " + BuildPlayerWindow.s_BuildPlatforms.GetBuildTargetDisplayName(target);
			string text = EditorUtility.SaveBuildPanel(target, title2, directory, lastPathNameComponent, extensionForBuildTarget);
			if (text == string.Empty)
			{
				return false;
			}
			if (extensionForBuildTarget != string.Empty && FileUtil.GetPathExtension(text).ToLower() != extensionForBuildTarget)
			{
				text = text + '.' + extensionForBuildTarget;
			}
			string lastPathNameComponent2 = FileUtil.GetLastPathNameComponent(text);
			if (lastPathNameComponent2 == string.Empty)
			{
				return false;
			}
			string path = (!(extensionForBuildTarget != string.Empty)) ? text : FileUtil.DeleteLastPathNameComponent(text);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (target == BuildTarget.iPhone && Application.platform != RuntimePlatform.OSXEditor && !BuildPlayerWindow.FolderIsEmpty(text) && !BuildPlayerWindow.UserWantsToDeleteFiles(text))
			{
				return false;
			}
			EditorUserBuildSettings.SetBuildLocation(target, text);
			return true;
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
		private static bool MetroAndWP8BuildRequirementsMet(out GUIContent msg)
		{
			if (!InternalEditorUtility.RunningUnderWindows8())
			{
				msg = EditorGUIUtility.TextContent("BuildSettings.NoWindows8");
				return false;
			}
			msg = null;
			return true;
		}
	}
}
