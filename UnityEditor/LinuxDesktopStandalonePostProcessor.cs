using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.LinuxStandalone;
using UnityEditor.Modules;
using UnityEditorInternal;
internal class LinuxDesktopStandalonePostProcessor : DesktopStandalonePostProcessor
{
	protected override string StagingAreaPluginsFolder
	{
		get
		{
			string text = FileUtil.CombinePaths(new string[]
			{
				base.StagingArea,
				"Data",
				"Plugins"
			});
			return (base.Target != BuildTarget.StandaloneLinuxUniversal) ? Path.Combine(text, this.ArchitecturePostFixFor(base.Target)) : text;
		}
	}
	private bool Headless
	{
		get
		{
			return (this.m_PostProcessArgs.options & BuildOptions.EnableHeadlessMode) != BuildOptions.None;
		}
	}
	private string FullDataFolderPath
	{
		get
		{
			return Path.Combine(base.DestinationFolder, Path.GetFileNameWithoutExtension(base.InstallPath) + "_Data");
		}
	}
	protected override string DestinationFolderForInstallingIntoBuildsFolder
	{
		get
		{
			throw new NotSupportedException();
		}
	}
	public LinuxDesktopStandalonePostProcessor(BuildPostProcessArgs postProcessArgs) : base(postProcessArgs)
	{
	}
	private string ArchitecturePostFixFor(BuildTarget buildTarget)
	{
		return (buildTarget != BuildTarget.StandaloneLinux64) ? "x86" : "x86_64";
	}
	protected override void CopyVariationFolderIntoStagingArea()
	{
		if (base.Target != BuildTarget.StandaloneLinuxUniversal)
		{
			base.CopyVariationFolderIntoStagingArea();
			return;
		}
		BuildTarget[] array = new BuildTarget[]
		{
			BuildTarget.StandaloneLinux,
			BuildTarget.StandaloneLinux64
		};
		for (int i = 0; i < array.Length; i++)
		{
			BuildTarget buildTarget = array[i];
			string source = FileUtil.CombinePaths(new string[]
			{
				this.m_PostProcessArgs.playerPackage,
				"Variations",
				this.VariationNameFor(buildTarget)
			});
			FileUtil.CopyDirectoryFiltered(source, base.StagingArea, true, (string f) => true, true);
			this.RenameStagingAreaFile("LinuxPlayer", Path.ChangeExtension("LinuxPlayer", this.ArchitecturePostFixFor(buildTarget)));
		}
	}
	protected override void RenameFilesInStagingArea()
	{
		this.RenameStagingAreaFile("Data", FileUtil.UnityGetFileNameWithoutExtension(base.InstallPath) + "_Data");
		if (base.Target != BuildTarget.StandaloneLinuxUniversal)
		{
			this.RenameStagingAreaFile("LinuxPlayer", FileUtil.UnityGetFileName(base.InstallPath));
			return;
		}
		foreach (string current in new BuildTarget[]
		{
			BuildTarget.StandaloneLinux,
			BuildTarget.StandaloneLinux64
		}.Select(new Func<BuildTarget, string>(this.ArchitecturePostFixFor)))
		{
			this.RenameStagingAreaFile(Path.ChangeExtension("LinuxPlayer", current), Path.ChangeExtension(FileUtil.UnityGetFileName(base.InstallPath), current));
		}
	}
	private void RenameStagingAreaFile(string from, string to)
	{
		FileUtil.MoveFileOrDirectory(Path.Combine(base.StagingArea, from), Path.Combine(base.StagingArea, to));
	}
	protected override string GetVariationName()
	{
		return this.VariationNameFor(base.Target);
	}
	private string VariationNameFor(BuildTarget target)
	{
		return string.Format("{0}_{1}_{2}", this.PlatformStringFor(target), (!this.Headless) ? "withgfx" : "headless", (!base.Development) ? "nondevelopment" : "development");
	}
	protected override void DeleteDestination()
	{
		FileUtil.DeleteFileOrDirectory(Path.ChangeExtension(base.InstallPath, this.ArchitecturePostFixFor(BuildTarget.StandaloneLinux)));
		FileUtil.DeleteFileOrDirectory(Path.ChangeExtension(base.InstallPath, this.ArchitecturePostFixFor(BuildTarget.StandaloneLinux64)));
		FileUtil.DeleteFileOrDirectory(this.FullDataFolderPath);
	}
	protected override string PlatformStringFor(BuildTarget target)
	{
		if (target == BuildTarget.StandaloneLinux64)
		{
			return "linux64";
		}
		if (target == BuildTarget.StandaloneLinuxUniversal)
		{
			return "universal";
		}
		if (target != BuildTarget.StandaloneLinux)
		{
			throw new ArgumentException("Unexpected target: " + target);
		}
		return "linux32";
	}
	protected override IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target)
	{
		return new LinuxStandaloneIl2CppPlatformProvider(target, base.DataFolder);
	}
}
