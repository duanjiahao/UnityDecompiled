using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	internal static class VRPostProcess
	{
		private static readonly string VR_FOLDER = Path.Combine("VR", "Unity");

		private static string GetPluginExtension(BuildTarget target, bool isSpatializer)
		{
			switch (target)
			{
			case BuildTarget.StandaloneOSXUniversal:
			case BuildTarget.StandaloneOSXIntel:
				goto IL_4A;
			case (BuildTarget)3:
				IL_1A:
				switch (target)
				{
				case BuildTarget.StandaloneWindows64:
				case BuildTarget.WSAPlayer:
					goto IL_44;
				case BuildTarget.WebGL:
					IL_2F:
					if (target != BuildTarget.Android)
					{
						if (target != BuildTarget.StandaloneOSXIntel64)
						{
							return string.Empty;
						}
						goto IL_4A;
					}
					else
					{
						if (isSpatializer)
						{
							return ".so";
						}
						return ".aar";
					}
					break;
				}
				goto IL_2F;
			case BuildTarget.StandaloneWindows:
				goto IL_44;
			}
			goto IL_1A;
			IL_44:
			return ".dll";
			IL_4A:
			return ".bundle";
		}

		private static string GetPluginFolder(BuildTarget target)
		{
			string buildTargetName = BuildPipeline.GetBuildTargetName(target);
			string[] paths = new string[]
			{
				EditorApplication.applicationContentsPath,
				VRPostProcess.VR_FOLDER,
				buildTargetName
			};
			string[] paths2 = new string[]
			{
				EditorApplication.applicationContentsPath,
				VRPostProcess.VR_FOLDER,
				buildTargetName.ToLower()
			};
			string text = FileUtil.CombinePaths(paths);
			if (!Directory.Exists(text))
			{
				text = FileUtil.CombinePaths(paths2);
				if (!Directory.Exists(text))
				{
					text = null;
				}
			}
			return text;
		}

		[RegisterPlugins]
		private static IEnumerable<PluginDesc> RegisterSpatializerPlugins(BuildTarget target)
		{
			List<PluginDesc> list = new List<PluginDesc>();
			if (AudioUtil.canUseSpatializerEffect)
			{
				string pluginFolder = VRPostProcess.GetPluginFolder(target);
				if (pluginFolder != null)
				{
					VRDeviceInfoEditor[] allVRDeviceInfo = VREditor.GetAllVRDeviceInfo(BuildPipeline.GetBuildTargetGroup(target));
					string currentSpatializerEffectName = AudioUtil.GetCurrentSpatializerEffectName();
					for (int i = 0; i < allVRDeviceInfo.Length; i++)
					{
						if (currentSpatializerEffectName == allVRDeviceInfo[i].spatializerEffectName)
						{
							string[] paths = new string[]
							{
								pluginFolder,
								allVRDeviceInfo[i].spatializerPluginName
							};
							string text = FileUtil.CombinePaths(paths) + VRPostProcess.GetPluginExtension(target, true);
							if (File.Exists(text) || Directory.Exists(text))
							{
								list.Add(new PluginDesc
								{
									pluginPath = text
								});
								Debug.LogWarning("Native Spatializer Plugin: " + allVRDeviceInfo[i].spatializerPluginName + " was included in build.");
								break;
							}
						}
					}
					if (!list.Any<PluginDesc>())
					{
						Debug.LogWarning("Spatializer Effect: " + currentSpatializerEffectName + ", is not natively supported for the current build target.");
					}
				}
			}
			return list;
		}

		[RegisterPlugins]
		private static IEnumerable<PluginDesc> RegisterVRPlugins(BuildTarget target)
		{
			List<PluginDesc> list = new List<PluginDesc>();
			if (PlayerSettings.virtualRealitySupported)
			{
				string pluginFolder = VRPostProcess.GetPluginFolder(target);
				if (pluginFolder != null)
				{
					string pluginExtension = VRPostProcess.GetPluginExtension(target, false);
					VRDeviceInfoEditor[] enabledVRDeviceInfo = VREditor.GetEnabledVRDeviceInfo(BuildPipeline.GetBuildTargetGroup(target));
					string[] array = (from d in enabledVRDeviceInfo
					where !string.IsNullOrEmpty(d.externalPluginName)
					select d.externalPluginName).ToArray<string>();
					for (int i = 0; i < array.Length; i++)
					{
						string[] paths = new string[]
						{
							pluginFolder,
							array[i]
						};
						list.Add(new PluginDesc
						{
							pluginPath = FileUtil.CombinePaths(paths) + pluginExtension
						});
					}
				}
				if (!list.Any<PluginDesc>())
				{
					Debug.LogWarning("Unable to find plugins folder " + target + ". Native VR plugins will not be loaded.");
				}
			}
			return list;
		}
	}
}
