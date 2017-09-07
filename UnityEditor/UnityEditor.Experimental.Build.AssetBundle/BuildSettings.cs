using System;
using UnityEngine.Scripting;

namespace UnityEditor.Experimental.Build.AssetBundle
{
	[UsedByNativeCode]
	[Serializable]
	public struct BuildSettings
	{
		public string outputFolder;

		public string scriptsFolder;

		public BuildTarget target;

		public BuildTargetGroup group;

		public bool editorBundles;
	}
}
