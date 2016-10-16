using System;
using UnityEditor.Modules;

namespace UnityEditor.BuildReporting
{
	internal static class BuildReportHelper
	{
		private static IBuildAnalyzer m_CachedAnalyzer;

		private static BuildTarget m_CachedAnalyzerTarget;

		private static IBuildAnalyzer GetAnalyzerForTarget(BuildTarget target)
		{
			if (BuildReportHelper.m_CachedAnalyzerTarget == target)
			{
				return BuildReportHelper.m_CachedAnalyzer;
			}
			BuildReportHelper.m_CachedAnalyzer = ModuleManager.GetBuildAnalyzer(target);
			BuildReportHelper.m_CachedAnalyzerTarget = target;
			return BuildReportHelper.m_CachedAnalyzer;
		}

		public static void OnAddedExecutable(BuildReport report, int fileIndex)
		{
			IBuildAnalyzer analyzerForTarget = BuildReportHelper.GetAnalyzerForTarget(report.buildTarget);
			if (analyzerForTarget == null)
			{
				return;
			}
			analyzerForTarget.OnAddedExecutable(report, fileIndex);
		}
	}
}
