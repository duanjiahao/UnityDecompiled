using System;
using System.IO;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditorInternal;
internal abstract class DesktopStandalonePostProcessor
{
	protected BuildPostProcessArgs m_PostProcessArgs;
	protected abstract string StagingAreaPluginsFolder
	{
		get;
	}
	protected abstract string DestinationFolderForInstallingIntoBuildsFolder
	{
		get;
	}
	protected bool InstallingIntoBuildsFolder
	{
		get
		{
			return (this.m_PostProcessArgs.options & BuildOptions.InstallInBuildFolder) != BuildOptions.None;
		}
	}
	protected bool UseIl2Cpp
	{
		get
		{
			return (this.m_PostProcessArgs.options & BuildOptions.Il2CPP) != BuildOptions.None;
		}
	}
	protected string StagingArea
	{
		get
		{
			return this.m_PostProcessArgs.stagingArea;
		}
	}
	protected string InstallPath
	{
		get
		{
			return this.m_PostProcessArgs.installPath;
		}
	}
	protected string DataFolder
	{
		get
		{
			return this.StagingArea + "/Data";
		}
	}
	protected BuildTarget Target
	{
		get
		{
			return this.m_PostProcessArgs.target;
		}
	}
	protected string DestinationFolder
	{
		get
		{
			return FileUtil.UnityGetDirectoryName(this.m_PostProcessArgs.installPath);
		}
	}
	protected bool Development
	{
		get
		{
			return (this.m_PostProcessArgs.options & BuildOptions.Development) != BuildOptions.None;
		}
	}
	protected DesktopStandalonePostProcessor(BuildPostProcessArgs postProcessArgs)
	{
		this.m_PostProcessArgs = postProcessArgs;
	}
	public void PostProcess()
	{
		this.SetupStagingArea();
		this.CopyStagingAreaIntoDestination();
	}
	protected virtual void SetupStagingArea()
	{
		Directory.CreateDirectory(this.DataFolder);
		PostprocessBuildPlayer.InstallPlugins(this.StagingAreaPluginsFolder, this.m_PostProcessArgs.target);
		PostprocessBuildPlayer.InstallStreamingAssets(this.DataFolder);
		if (this.UseIl2Cpp)
		{
			this.CopyVariationFolderIntoStagingArea();
			IL2CPPUtils.RunIl2Cpp(this.StagingArea + "/Data", this.GetPlatformProvider(this.m_PostProcessArgs.target), delegate(string s)
			{
			}, this.m_PostProcessArgs.usedClassRegistry);
		}
		if (this.InstallingIntoBuildsFolder)
		{
			return;
		}
		if (!this.UseIl2Cpp)
		{
			this.CopyVariationFolderIntoStagingArea();
		}
		this.RenameFilesInStagingArea();
	}
	protected virtual void CopyVariationFolderIntoStagingArea()
	{
		string source = this.m_PostProcessArgs.playerPackage + "/Variations/" + this.GetVariationName();
		FileUtil.CopyDirectoryFiltered(source, this.StagingArea, true, (string f) => !f.Contains("UnityEngine.mdb"), true);
	}
	protected void CopyStagingAreaIntoDestination()
	{
		if (this.InstallingIntoBuildsFolder)
		{
			FileUtil.CopyDirectoryFiltered(this.DataFolder, Unsupported.GetBaseUnityDeveloperFolder() + "/" + this.DestinationFolderForInstallingIntoBuildsFolder, true, (string f) => true, true);
			return;
		}
		this.DeleteDestination();
		FileUtil.CopyDirectoryFiltered(this.StagingArea, this.DestinationFolder, true, (string f) => true, true);
	}
	protected abstract void DeleteDestination();
	protected virtual string GetVariationName()
	{
		return this.PlatformStringFor(this.m_PostProcessArgs.target) + "_" + (((this.m_PostProcessArgs.options & BuildOptions.Development) == BuildOptions.None) ? "nondevelopment" : "development");
	}
	protected abstract string PlatformStringFor(BuildTarget target);
	protected abstract void RenameFilesInStagingArea();
	protected abstract IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target);
}
