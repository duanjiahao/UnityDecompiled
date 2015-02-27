using System;
namespace UnityEditor
{
	internal enum UploadingBuildStatus
	{
		Authorizing,
		Authorized,
		Uploading,
		Uploaded,
		UploadFailed
	}
}
