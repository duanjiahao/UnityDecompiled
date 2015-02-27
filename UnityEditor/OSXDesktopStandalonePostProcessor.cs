using System;
using System.IO;
using UnityEditor;
using UnityEditor.Modules;
using UnityEditor.OSXStandalone;
using UnityEditorInternal;
internal class OSXDesktopStandalonePostProcessor : DesktopStandalonePostProcessor
{
	private string StagingAreaContents
	{
		get
		{
			return base.StagingArea + "/UnityPlayer.app/Contents";
		}
	}
	protected override string DestinationFolderForInstallingIntoBuildsFolder
	{
		get
		{
			return "build/MacStandaloneSupport/Variations/macosx32_development/UnityPlayer.app/Contents/Data";
		}
	}
	protected override string StagingAreaPluginsFolder
	{
		get
		{
			return this.StagingAreaContents + "/Plugins";
		}
	}
	private string InstallNameWithoutExtension
	{
		get
		{
			return FileUtil.UnityGetFileNameWithoutExtension(this.m_PostProcessArgs.installPath);
		}
	}
	public OSXDesktopStandalonePostProcessor(BuildPostProcessArgs postProcessArgs) : base(postProcessArgs)
	{
	}
	protected override void RenameFilesInStagingArea()
	{
		File.Move(this.StagingAreaContents + "/MacOS/UnityPlayer", this.StagingAreaContents + "/MacOS/" + this.InstallNameWithoutExtension);
		FileUtil.CopyDirectoryFiltered(base.StagingArea + "/Data", this.StagingAreaContents + "/Data", true, (string f) => true, true);
		FileUtil.DeleteFileOrDirectory(base.StagingArea + "/Data");
		if (base.UseIl2Cpp)
		{
			FileUtil.DeleteFileOrDirectory(this.StagingAreaContents + "/Resources/Data/Managed");
			FileUtil.DeleteFileOrDirectory(this.StagingAreaContents + "/Resources/Data/il2cppOutput");
		}
		string text = PostprocessBuildPlayer.GenerateBundleIdentifier(this.m_PostProcessArgs.companyName, this.m_PostProcessArgs.productName);
		FileUtil.ReplaceText(this.StagingAreaContents + "/Info.plist", new string[]
		{
			"UNITY_BUNDLE_IDENTIFIER",
			text,
			"UnityPlayer",
			this.InstallNameWithoutExtension
		});
		string to = base.StagingArea + "/" + this.InstallNameWithoutExtension + ".app";
		FileUtil.MoveFileOrDirectory(base.StagingArea + "/UnityPlayer.app", to);
	}
	protected override void DeleteDestination()
	{
		FileUtil.DeleteFileOrDirectory(this.m_PostProcessArgs.installPath);
	}
	protected override string GetVariationName()
	{
		string arg = "mono";
		if (base.UseIl2Cpp)
		{
			arg = "il2cpp";
		}
		return string.Format("{0}_{1}", base.GetVariationName(), arg);
	}
	protected override string PlatformStringFor(BuildTarget target)
	{
		switch (target)
		{
		case BuildTarget.StandaloneOSXUniversal:
			return "universal";
		case (BuildTarget)3:
			IL_16:
			if (target != BuildTarget.StandaloneOSXIntel64)
			{
				throw new ArgumentException("Unexpected target: " + target);
			}
			return "macosx64";
		case BuildTarget.StandaloneOSXIntel:
			return "macosx32";
		}
		goto IL_16;
	}
	protected override IIl2CppPlatformProvider GetPlatformProvider(BuildTarget target)
	{
		switch (target)
		{
		case BuildTarget.StandaloneOSXUniversal:
		case BuildTarget.StandaloneOSXIntel:
			goto IL_23;
		case (BuildTarget)3:
			IL_16:
			if (target != BuildTarget.StandaloneOSXIntel64)
			{
				throw new Exception("Build target not supported.");
			}
			goto IL_23;
		}
		goto IL_16;
		IL_23:
		return new OSXStandaloneIl2CppPlatformProvider(target, this.StagingAreaContents + "/Data", base.Development);
	}
}
