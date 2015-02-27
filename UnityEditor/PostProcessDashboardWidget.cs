using System;
using UnityEditor;
internal class PostProcessDashboardWidget
{
	public static void PostProcess(BuildTarget target, string installPath, string stagingArea, string playerPackage, string companyName, string productName, int width, int height)
	{
		string text = stagingArea + "/DashboardBuild";
		FileUtil.DeleteFileOrDirectory(text);
		FileUtil.CopyFileOrDirectory(playerPackage, text);
		FileUtil.MoveFileOrDirectory("Temp/unitystream.unity3d", text + "/widget.unity3d");
		PostprocessBuildPlayer.InstallPlugins(text + "/Plugins", target);
		string text2 = PostprocessBuildPlayer.GenerateBundleIdentifier(companyName, productName) + ".widget";
		int num = width + 32;
		int num2 = height + 32;
		string[] input = new string[]
		{
			"UNITY_WIDTH_PLUS32",
			num.ToString(),
			"UNITY_HEIGHT_PLUS32",
			num2.ToString(),
			"UNITY_WIDTH",
			width.ToString(),
			"UNITY_HEIGHT",
			height.ToString(),
			"UNITY_BUNDLE_IDENTIFIER",
			text2,
			"UNITY_BUNDLE_NAME",
			productName
		};
		FileUtil.ReplaceText(text + "/UnityWidget.html", input);
		FileUtil.ReplaceText(text + "/Info.plist", input);
		FileUtil.DeleteFileOrDirectory(installPath);
		FileUtil.MoveFileOrDirectory(text, installPath);
	}
}
