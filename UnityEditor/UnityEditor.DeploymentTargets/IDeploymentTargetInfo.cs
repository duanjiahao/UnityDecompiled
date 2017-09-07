using System;
using UnityEditor.BuildReporting;

namespace UnityEditor.DeploymentTargets
{
	internal interface IDeploymentTargetInfo
	{
		FlagSet<DeploymentTargetSupportFlags> GetSupportFlags();

		BuildCheckResult CheckBuild(BuildReport buildReport);
	}
}
