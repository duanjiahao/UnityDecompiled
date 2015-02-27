using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor
{
	internal sealed class UploadingBuildsUtility
	{
		internal static void UploadBuild(string buildPath, bool autoRun)
		{
			UploadingBuildsUtility.UploadBuild(buildPath, 0, autoRun);
		}
		internal static void UploadBuild(string buildPath, int overwriteHandling, bool autoRun)
		{
			UploadingBuildsUtility.UploadBuild(buildPath, PlayerSettings.productName, overwriteHandling, autoRun);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UploadBuild(string buildPath, string displayName, int overwriteHandling, bool autoRun);
		internal static bool ResumeBuildUpload(string displayName)
		{
			return UploadingBuildsUtility.ResumeBuildUpload(displayName, true);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool ResumeBuildUpload(string displayName, bool replace);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern UploadingBuild[] GetUploadingBuilds();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RemoveUploadingBuild(string displayName);
	}
}
