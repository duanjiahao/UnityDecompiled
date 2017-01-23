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
			IBuildAnalyzer cachedAnalyzer;
			if (BuildReportHelper.m_CachedAnalyzerTarget == target)
			{
				cachedAnalyzer = BuildReportHelper.m_CachedAnalyzer;
			}
			else
			{
				BuildReportHelper.m_CachedAnalyzer = ModuleManager.GetBuildAnalyzer(target);
				BuildReportHelper.m_CachedAnalyzerTarget = target;
				cachedAnalyzer = BuildReportHelper.m_CachedAnalyzer;
			}
			return cachedAnalyzer;
		}

		public static void OnAddedExecutable(BuildReport report, int fileIndex)
		{
			IBuildAnalyzer analyzerForTarget = BuildReportHelper.GetAnalyzerForTarget(report.buildTarget);
			if (analyzerForTarget != null)
			{
				analyzerForTarget.OnAddedExecutable(report, fileIndex);
			}
		}
	}
}
