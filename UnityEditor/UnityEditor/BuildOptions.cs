using System;

namespace UnityEditor
{
	[Flags]
	public enum BuildOptions
	{
		None = 0,
		Development = 1,
		AutoRunPlayer = 4,
		ShowBuiltPlayer = 8,
		BuildAdditionalStreamedScenes = 16,
		AcceptExternalModificationsToPlayer = 32,
		InstallInBuildFolder = 64,
		[Obsolete("WebPlayer has been removed in 5.4")]
		WebPlayerOfflineDeployment = 128,
		ConnectWithProfiler = 256,
		AllowDebugging = 512,
		SymlinkLibraries = 1024,
		UncompressedAssetBundle = 2048,
		[Obsolete("Use BuildOptions.Development instead")]
		StripDebugSymbols = 0,
		[Obsolete("Texture Compression is now always enabled")]
		CompressTextures = 0,
		ConnectToHost = 4096,
		EnableHeadlessMode = 16384,
		BuildScriptsOnly = 32768,
		Il2CPP = 65536,
		ForceEnableAssertions = 131072,
		ForceOptimizeScriptCompilation = 524288,
		ComputeCRC = 1048576,
		StrictMode = 2097152
	}
}
