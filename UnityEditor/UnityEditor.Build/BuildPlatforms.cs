using System;
using System.Collections.Generic;

namespace UnityEditor.Build
{
	internal class BuildPlatforms
	{
		private static readonly BuildPlatforms s_Instance = new BuildPlatforms();

		public BuildPlatform[] buildPlatforms;

		public static BuildPlatforms instance
		{
			get
			{
				return BuildPlatforms.s_Instance;
			}
		}

		internal BuildPlatforms()
		{
			List<BuildPlatform> list = new List<BuildPlatform>();
			list.Add(new BuildPlatform("PC, Mac & Linux Standalone", "BuildSettings.Standalone", BuildTargetGroup.Standalone, true));
			list.Add(new BuildPlatform("iOS", "BuildSettings.iPhone", BuildTargetGroup.iPhone, true));
			list.Add(new BuildPlatform("tvOS", "BuildSettings.tvOS", BuildTargetGroup.tvOS, true));
			list.Add(new BuildPlatform("Android", "BuildSettings.Android", BuildTargetGroup.Android, true));
			list.Add(new BuildPlatform("Tizen", "BuildSettings.Tizen", BuildTargetGroup.Tizen, true));
			list.Add(new BuildPlatform("Xbox One", "BuildSettings.XboxOne", BuildTargetGroup.XboxOne, true));
			list.Add(new BuildPlatform("PS Vita", "BuildSettings.PSP2", BuildTargetGroup.PSP2, true));
			list.Add(new BuildPlatform("PS4", "BuildSettings.PS4", BuildTargetGroup.PS4, true));
			list.Add(new BuildPlatform("Wii U", "BuildSettings.WiiU", BuildTargetGroup.WiiU, false));
			list.Add(new BuildPlatform("Universal Windows Platform", "BuildSettings.Metro", BuildTargetGroup.WSA, true));
			list.Add(new BuildPlatform("WebGL", "BuildSettings.WebGL", BuildTargetGroup.WebGL, true));
			list.Add(new BuildPlatform("Samsung TV", "BuildSettings.SamsungTV", BuildTargetGroup.SamsungTV, true));
			list.Add(new BuildPlatform("Nintendo 3DS", "BuildSettings.N3DS", BuildTargetGroup.N3DS, false));
			list.Add(new BuildPlatform("Facebook", "BuildSettings.Facebook", BuildTargetGroup.Facebook, true));
			list.Add(new BuildPlatform("Nintendo Switch", "BuildSettings.Switch", BuildTargetGroup.Switch, false));
			foreach (BuildPlatform current in list)
			{
				current.tooltip = BuildPipeline.GetBuildTargetGroupDisplayName(current.targetGroup) + " settings";
			}
			this.buildPlatforms = list.ToArray();
		}

		public string GetBuildTargetDisplayName(BuildTarget target)
		{
			BuildPlatform[] array = this.buildPlatforms;
			string result;
			for (int i = 0; i < array.Length; i++)
			{
				BuildPlatform buildPlatform = array[i];
				if (buildPlatform.defaultTarget == target)
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

		public string GetModuleDisplayName(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
		{
			string result;
			if (buildTargetGroup != BuildTargetGroup.Facebook)
			{
				result = this.GetBuildTargetDisplayName(buildTarget);
			}
			else
			{
				result = BuildPipeline.GetBuildTargetGroupDisplayName(buildTargetGroup);
			}
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

		public BuildPlatform BuildPlatformFromTargetGroup(BuildTargetGroup group)
		{
			int num = this.BuildPlatformIndexFromTargetGroup(group);
			return (num == -1) ? null : this.buildPlatforms[num];
		}

		public List<BuildPlatform> GetValidPlatforms(bool includeMetaPlatforms)
		{
			List<BuildPlatform> list = new List<BuildPlatform>();
			BuildPlatform[] array = this.buildPlatforms;
			for (int i = 0; i < array.Length; i++)
			{
				BuildPlatform buildPlatform = array[i];
				if ((buildPlatform.targetGroup == BuildTargetGroup.Standalone || BuildPipeline.IsBuildTargetSupported(buildPlatform.targetGroup, buildPlatform.defaultTarget)) && (buildPlatform.targetGroup != BuildTargetGroup.Facebook || includeMetaPlatforms))
				{
					list.Add(buildPlatform);
				}
			}
			return list;
		}

		public List<BuildPlatform> GetValidPlatforms()
		{
			return this.GetValidPlatforms(false);
		}
	}
}
