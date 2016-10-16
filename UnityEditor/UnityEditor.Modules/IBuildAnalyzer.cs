using System;
using UnityEditor.BuildReporting;

namespace UnityEditor.Modules
{
	internal interface IBuildAnalyzer
	{
		void OnAddedExecutable(BuildReport report, int fileIndex);
	}
}
