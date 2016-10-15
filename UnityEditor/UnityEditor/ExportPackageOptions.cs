using System;

namespace UnityEditor
{
	[Flags]
	public enum ExportPackageOptions
	{
		Default = 0,
		Interactive = 1,
		Recurse = 2,
		IncludeDependencies = 4,
		IncludeLibraryAssets = 8
	}
}
