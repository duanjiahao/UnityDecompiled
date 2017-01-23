using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Modules;
using UnityEngine;

internal class DesktopStandaloneBuildWindowExtension : DefaultBuildWindowExtension
{
	private GUIContent m_StandaloneTarget = EditorGUIUtility.TextContent("Target Platform|Destination platform for standalone build");

	private GUIContent m_Architecture = EditorGUIUtility.TextContent("Architecture|Build m_Architecture for standalone");

	private BuildTarget[] m_StandaloneSubtargets;

	private GUIContent[] m_StandaloneSubtargetStrings;

	public DesktopStandaloneBuildWindowExtension()
	{
		this.SetupStandaloneSubtargets();
	}

	private void SetupStandaloneSubtargets()
	{
		List<BuildTarget> list = new List<BuildTarget>();
		List<GUIContent> list2 = new List<GUIContent>();
		if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)))
		{
			list.Add(BuildTarget.StandaloneWindows);
			list2.Add(EditorGUIUtility.TextContent("Windows"));
		}
		if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
		{
			list.Add(BuildTarget.StandaloneOSXIntel);
			list2.Add(EditorGUIUtility.TextContent("Mac OS X"));
		}
		if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)))
		{
			list.Add(BuildTarget.StandaloneLinux);
			list2.Add(EditorGUIUtility.TextContent("Linux"));
		}
		this.m_StandaloneSubtargets = list.ToArray();
		this.m_StandaloneSubtargetStrings = list2.ToArray();
	}

	private static BuildTarget GetBestStandaloneTarget(BuildTarget selectedTarget)
	{
		BuildTarget result;
		if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(selectedTarget)))
		{
			result = selectedTarget;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor && ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneWindows)))
		{
			result = BuildTarget.StandaloneWindows;
		}
		else if (Application.platform == RuntimePlatform.OSXEditor && ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
		{
			result = BuildTarget.StandaloneOSXIntel;
		}
		else if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneOSXIntel)))
		{
			result = BuildTarget.StandaloneOSXIntel;
		}
		else if (ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(BuildTarget.StandaloneLinux)))
		{
			result = BuildTarget.StandaloneLinux;
		}
		else
		{
			result = BuildTarget.StandaloneWindows;
		}
		return result;
	}

	private static Dictionary<GUIContent, BuildTarget> GetArchitecturesForPlatform(BuildTarget target)
	{
		Dictionary<GUIContent, BuildTarget> result;
		switch (target)
		{
		case BuildTarget.StandaloneOSXUniversal:
		case BuildTarget.StandaloneOSXIntel:
			goto IL_BF;
		case (BuildTarget)3:
			IL_19:
			switch (target)
			{
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				goto IL_7C;
			case BuildTarget.WP8Player:
				IL_32:
				switch (target)
				{
				case BuildTarget.StandaloneLinux:
					goto IL_7C;
				case BuildTarget.StandaloneWindows64:
					goto IL_4C;
				}
				result = null;
				return result;
			case BuildTarget.StandaloneOSXIntel64:
				goto IL_BF;
			}
			goto IL_32;
			IL_7C:
			result = new Dictionary<GUIContent, BuildTarget>
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
			return result;
		case BuildTarget.StandaloneWindows:
			goto IL_4C;
		}
		goto IL_19;
		IL_4C:
		result = new Dictionary<GUIContent, BuildTarget>
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
		return result;
		IL_BF:
		result = new Dictionary<GUIContent, BuildTarget>
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
		return result;
	}

	private static BuildTarget DefaultTargetForPlatform(BuildTarget target)
	{
		BuildTarget result;
		switch (target)
		{
		case BuildTarget.StandaloneOSXUniversal:
		case BuildTarget.StandaloneOSXIntel:
			goto IL_5B;
		case (BuildTarget)3:
			IL_19:
			switch (target)
			{
			case BuildTarget.StandaloneLinux64:
			case BuildTarget.StandaloneLinuxUniversal:
				goto IL_53;
			case BuildTarget.WP8Player:
				IL_32:
				switch (target)
				{
				case BuildTarget.StandaloneLinux:
					goto IL_53;
				case BuildTarget.StandaloneWindows64:
					goto IL_4C;
				}
				result = target;
				return result;
			case BuildTarget.StandaloneOSXIntel64:
				goto IL_5B;
			}
			goto IL_32;
			IL_53:
			result = BuildTarget.StandaloneLinux;
			return result;
		case BuildTarget.StandaloneWindows:
			goto IL_4C;
		}
		goto IL_19;
		IL_4C:
		result = BuildTarget.StandaloneWindows;
		return result;
		IL_5B:
		result = BuildTarget.StandaloneOSXIntel;
		return result;
	}

	public override void ShowPlatformBuildOptions()
	{
		BuildTarget bestStandaloneTarget = DesktopStandaloneBuildWindowExtension.GetBestStandaloneTarget(EditorUserBuildSettings.selectedStandaloneTarget);
		BuildTarget buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
		int num = Math.Max(0, Array.IndexOf<BuildTarget>(this.m_StandaloneSubtargets, DesktopStandaloneBuildWindowExtension.DefaultTargetForPlatform(bestStandaloneTarget)));
		int num2 = EditorGUILayout.Popup(this.m_StandaloneTarget, num, this.m_StandaloneSubtargetStrings, new GUILayoutOption[0]);
		if (num2 == num)
		{
			Dictionary<GUIContent, BuildTarget> architecturesForPlatform = DesktopStandaloneBuildWindowExtension.GetArchitecturesForPlatform(bestStandaloneTarget);
			if (architecturesForPlatform != null)
			{
				GUIContent[] array = new List<GUIContent>(architecturesForPlatform.Keys).ToArray();
				int num3 = 0;
				if (num2 == num)
				{
					foreach (KeyValuePair<GUIContent, BuildTarget> current in architecturesForPlatform)
					{
						if (current.Value == bestStandaloneTarget)
						{
							num3 = Math.Max(0, Array.IndexOf<GUIContent>(array, current.Key));
							break;
						}
					}
				}
				num3 = EditorGUILayout.Popup(this.m_Architecture, num3, array, new GUILayoutOption[0]);
				buildTarget = architecturesForPlatform[array[num3]];
			}
		}
		else
		{
			buildTarget = this.m_StandaloneSubtargets[num2];
		}
		if (buildTarget != EditorUserBuildSettings.selectedStandaloneTarget)
		{
			EditorUserBuildSettings.selectedStandaloneTarget = buildTarget;
			GUIUtility.ExitGUI();
		}
	}

	public override bool EnabledBuildButton()
	{
		return true;
	}

	public override bool EnabledBuildAndRunButton()
	{
		return true;
	}
}
