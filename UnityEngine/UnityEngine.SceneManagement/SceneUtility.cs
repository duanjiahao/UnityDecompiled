using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.SceneManagement
{
	public static class SceneUtility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetScenePathByBuildIndex(int buildIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetBuildIndexByScenePath(string scenePath);
	}
}
