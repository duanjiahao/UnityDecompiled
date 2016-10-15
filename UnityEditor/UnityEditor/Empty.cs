using System;

namespace UnityEditor
{
	internal class Empty
	{
		internal static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
		}

		internal static bool OnOpenAsset(int instanceID, int line)
		{
			return false;
		}
	}
}
