using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.Scripting.Compilers;
internal class MxmlcHelper
{
	internal static string[] MxmlcArgumentsFor(FlashPostProcessSettings settings, FlashFileHelper fileHelper)
	{
		string text = fileHelper.FindUserSwcs().Aggregate(string.Empty, (string current, string includeLib) => current + " -include-libraries=" + fileHelper.PrepareFileName(includeLib));
		string text2 = "-debug=" + ((!settings.IsDevelopment) ? "false" : "true");
		string text3 = "-optimize=" + ((!settings.IsDevelopment) ? "true" : "false");
		string text4 = string.Concat(new object[]
		{
			"-default-size ",
			settings.Width,
			" ",
			settings.Height
		});
		return new string[]
		{
			"-Xmx768m -Dsun.io.useCanonCaches=false -Duser.region=US -Duser.language=en",
			"-jar " + fileHelper.PathForMxmlc(),
			"+flexlib=" + fileHelper.PathForFlexFrameworks(),
			fileHelper.PathForUnityAppDotAs(),
			text2,
			text3,
			"-include-libraries=" + fileHelper.FullPathForUnityNativeSwc(settings.GetUnityNativeSwcForSubTarget(settings.StripPhysics)),
			text,
			MxmlcHelper.AllSourcePaths(fileHelper),
			"-static-link-runtime-shared-libraries=true",
			"-swf-version=" + settings.GetSwfVersionForPlayerVersion(),
			text4,
			"-omit-trace-statements=false",
			"-default-script-limits=1000,60",
			"-target-player=" + settings.GetTargetPlayerForSubtarget(),
			"-verbose-stacktraces=true",
			"-compress=false",
			settings.MxmlcCompileTimeConstants(),
			"-output=" + fileHelper.PathForTempSwf()
		};
	}
	internal static string AllSourcePaths(FlashFileHelper fileHelper)
	{
		IEnumerable<string> paths = new string[]
		{
			fileHelper.PathForAs3Src(),
			fileHelper.PathForConvertedDotNetCode(),
			fileHelper.PathForUserBuildAs3()
		}.Concat(fileHelper.FindUserSourcePaths());
		return MxmlcHelper.FormatSourcePathsForCmdline(paths);
	}
	private static string FormatSourcePathsForCmdline(IEnumerable<string> paths)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (string current in paths)
		{
			if (!flag)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append("-source-path=");
			stringBuilder.Append(CommandLineFormatter.PrepareFileName(current));
			flag = false;
		}
		return stringBuilder.ToString();
	}
}
