using System;
using System.Collections.Generic;
using UnityEditor.BuildReporting;

namespace UnityEditor.DeploymentTargets
{
	internal interface IDeploymentTargetsExtension
	{
		List<DeploymentTargetIdAndStatus> GetKnownTargets(ProgressHandler progressHandler = null);

		IDeploymentTargetInfo GetTargetInfo(DeploymentTargetId targetId, ProgressHandler progressHandler = null);

		void LaunchBuildOnTarget(BuildReport buildReport, DeploymentTargetId targetId, ProgressHandler progressHandler = null);
	}
}
