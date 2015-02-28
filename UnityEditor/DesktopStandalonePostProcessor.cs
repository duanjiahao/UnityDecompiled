using System;
using System.IO;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditorInternal;
using UnityEngine;
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
	protected DesktopStandalonePostProcessor()
	{
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
	private void CopyNativePlugins()
	{
		string buildTargetName = BuildPipeline.GetBuildTargetName(this.m_PostProcessArgs.target);
		IPluginImporterExtension pluginImporterExtension = new DesktopPluginImporterExtension();
		string stagingAreaPluginsFolder = this.StagingAreaPluginsFolder;
		string path = Path.Combine(stagingAreaPluginsFolder, "x86");
		string path2 = Path.Combine(stagingAreaPluginsFolder, "x86_64");
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		PluginImporter[] importers = PluginImporter.GetImporters(this.m_PostProcessArgs.target);
		for (int i = 0; i < importers.Length; i++)
		{
			PluginImporter pluginImporter = importers[i];
			BuildTarget target = this.m_PostProcessArgs.target;
			if (pluginImporter.isNativePlugin)
			{
				if (string.IsNullOrEmpty(pluginImporter.assetPath))
				{
					Debug.LogWarning("Got empty plugin importer path for " + this.m_PostProcessArgs.target.ToString());
				}
				else
				{
					if (!flag)
					{
						Directory.CreateDirectory(stagingAreaPluginsFolder);
						flag = true;
					}
					bool flag4 = Directory.Exists(pluginImporter.assetPath);
					string platformData = pluginImporter.GetPlatformData(target, "CPU");
					string text = platformData;
					switch (text)
					{
					case "x86":
						if (target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneLinux64)
						{
							goto IL_21A;
						}
						if (!flag2)
						{
							Directory.CreateDirectory(path);
							flag2 = true;
						}
						break;
					case "x86_64":
						if (target != BuildTarget.StandaloneOSXIntel64 && target != BuildTarget.StandaloneOSXUniversal && target != BuildTarget.StandaloneWindows64 && target != BuildTarget.StandaloneLinux64 && target != BuildTarget.StandaloneLinuxUniversal)
						{
							goto IL_21A;
						}
						if (!flag3)
						{
							Directory.CreateDirectory(path2);
							flag3 = true;
						}
						break;
					case "None":
						goto IL_21A;
					}
					string text2 = Path.Combine(stagingAreaPluginsFolder, pluginImporterExtension.CalculateFinalPluginPath(buildTargetName, pluginImporter));
					if (flag4)
					{
						FileUtil.CopyDirectoryRecursive(pluginImporter.assetPath, text2);
					}
					else
					{
						FileUtil.UnityFileCopy(pluginImporter.assetPath, text2);
					}
				}
			}
			IL_21A:;
		}
	}
	protected virtual void SetupStagingArea()
	{
		Directory.CreateDirectory(this.DataFolder);
		this.CopyNativePlugins();
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
		if (!this.InstallingIntoBuildsFolder)
		{
			this.DeleteDestination();
			FileUtil.CopyDirectoryFiltered(this.StagingArea, this.DestinationFolder, true, (string f) => true, true);
			return;
		}
		string text = Unsupported.GetBaseUnityDeveloperFolder() + "/" + this.DestinationFolderForInstallingIntoBuildsFolder;
		if (!Directory.Exists(Path.GetDirectoryName(text)))
		{
			throw new Exception("Installing in builds folder failed because the player has not been built (You most likely want to enable 'Development build').");
		}
		FileUtil.CopyDirectoryFiltered(this.DataFolder, text, true, (string f) => true, true);
	}
	protected abstract void DeleteDestination();
	protected virtual string GetVariationName()
	{
		return string.Format("{0}_{1}", this.PlatformStringFor(this.m_PostProcessArgs.target), (!this.Development) ? "nondevelopment" : "development");
	}
	protected abstract string PlatformStringFor(BuildTarget target);
	protected abstract void RenameFilesInStagingArea();
	protected abstract IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target);
}
