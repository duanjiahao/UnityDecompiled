using System;

namespace UnityEditor
{
	public struct BuildPlayerOptions
	{
		public string[] scenes
		{
			get;
			set;
		}

		public string locationPathName
		{
			get;
			set;
		}

		public string assetBundleManifestPath
		{
			get;
			set;
		}

		public BuildTargetGroup targetGroup
		{
			get;
			set;
		}

		public BuildTarget target
		{
			get;
			set;
		}

		public BuildOptions options
		{
			get;
			set;
		}
	}
}
