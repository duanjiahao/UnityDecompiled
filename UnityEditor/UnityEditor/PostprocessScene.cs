using System;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityEditor
{
	internal class PostprocessScene
	{
		internal class UnityBuildPostprocessor
		{
			[PostProcessScene(0)]
			public static void OnPostprocessScene()
			{
				int num;
				int num2;
				PlayerSettings.GetBatchingForPlatform(EditorUserBuildSettings.activeBuildTarget, out num, out num2);
				if (num != 0)
				{
					InternalStaticBatchingUtility.Combine(null, true, true);
				}
			}
		}
	}
}
