using System;

namespace UnityEditor
{
	internal enum DownloadResolution
	{
		Unresolved,
		SkipAsset,
		TrashMyChanges,
		TrashServerChanges,
		Merge
	}
}
