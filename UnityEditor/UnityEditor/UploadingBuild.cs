using System;
namespace UnityEditor
{
	internal struct UploadingBuild
	{
		public string displayName;
		public string url;
		public UploadingBuildStatus status;
		public float progress;
	}
}
