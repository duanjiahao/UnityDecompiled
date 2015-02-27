using System;
using System.IO;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.WindowsStandalone;
using UnityEditorInternal;
internal class WindowsDesktopStandalonePostProcessor : DesktopStandalonePostProcessor
{
	protected override string DestinationFolderForInstallingIntoBuildsFolder
	{
		get
		{
			string str = (!base.Development) ? "win32_nondevelopment" : "win32_development";
			return "build/WindowsStandaloneSupport/Variations/" + str + "/Data";
		}
	}
	private string FullDataFolderPath
	{
		get
		{
			return Path.Combine(base.DestinationFolder, this.DestinationFileWithoutExtension + "_Data");
		}
	}
	private string DestinationFileWithoutExtension
	{
		get
		{
			return FileUtil.UnityGetFileNameWithoutExtension(this.FullPathToExe);
		}
	}
	private string FullPathToExe
	{
		get
		{
			return this.m_PostProcessArgs.installPath;
		}
	}
	protected override string StagingAreaPluginsFolder
	{
		get
		{
			return base.StagingArea + "/Data/Plugins";
		}
	}
	public WindowsDesktopStandalonePostProcessor(BuildPostProcessArgs postProcessArgs) : base(postProcessArgs)
	{
	}
	protected override void RenameFilesInStagingArea()
	{
		string fileName = Path.GetFileName(this.m_PostProcessArgs.installPath);
		FileUtil.MoveFileOrDirectory(base.StagingArea + "/player_win.exe", base.StagingArea + "/" + fileName);
		FileUtil.MoveFileOrDirectory(base.StagingArea + "/Data", base.StagingArea + "/" + Path.GetFileNameWithoutExtension(fileName) + "_Data");
	}
	protected override void DeleteDestination()
	{
		FileUtil.DeleteFileOrDirectory(this.FullPathToExe);
		FileUtil.DeleteFileOrDirectory(this.FullDataFolderPath);
		FileUtil.DeleteFileOrDirectory(Path.Combine(base.DestinationFolder, "UnityPlayer_Symbols.pdb"));
	}
	protected override string GetVariationName()
	{
		if (this.m_PostProcessArgs.target == BuildTarget.StandaloneGLESEmu)
		{
			return "glesemulator";
		}
		return base.GetVariationName();
	}
	protected override string PlatformStringFor(BuildTarget target)
	{
		if (target == BuildTarget.StandaloneWindows)
		{
			return "win32";
		}
		if (target != BuildTarget.StandaloneWindows64)
		{
			throw new ArgumentException("Unexpected target: " + target);
		}
		return "win64";
	}
	protected override IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target)
	{
		if (target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64)
		{
			throw new Exception("Build target not supported.");
		}
		return new WindowsStandaloneIl2CppPlatformProvider(target, base.DataFolder, base.Development);
	}
}
