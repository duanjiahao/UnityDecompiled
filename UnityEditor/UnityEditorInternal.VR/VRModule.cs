using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal.VR
{
	public class VRModule
	{
		private static bool IsTargetingCardboardOnIOS(BuildTarget target)
		{
			return PlayerSettings.virtualRealitySupported && target == BuildTarget.iOS && VREditor.IsVRDeviceEnabledForBuildTarget(target, "cardboard");
		}

		public static void SetupBuildSettings(BuildTarget target, int osVerMajor)
		{
			if (VRModule.IsTargetingCardboardOnIOS(target) && osVerMajor < 8)
			{
				Debug.LogWarning(string.Format("Deployment target version is set to {0}, but Cardboard supports only versions starting from 8.0.", osVerMajor));
			}
		}

		public static bool ShouldInjectVRDependenciesForBuildTarget(BuildTarget target)
		{
			bool result;
			if (!PlayerSettings.virtualRealitySupported)
			{
				result = false;
			}
			else
			{
				BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
				bool flag = buildTargetGroup == BuildTargetGroup.iPhone && VREditor.IsVRDeviceEnabledForBuildTarget(target, "cardboard");
				result = flag;
			}
			return result;
		}
	}
}
