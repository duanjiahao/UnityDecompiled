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
		WebPlayerOfflineDeployment = 128,
		ConnectWithProfiler = 256,
		AllowDebugging = 512,
		SymlinkLibraries = 1024,
		UncompressedAssetBundle = 2048,
		StripDebugSymbols = 0,
		CompressTextures = 0,
		ConnectToHost = 4096,
		DeployOnline = 8192,
		EnableHeadlessMode = 16384,
		BuildScriptsOnly = 32768,
		Il2CPP = 65536
	}
}
