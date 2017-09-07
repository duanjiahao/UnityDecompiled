using System;
using UnityEditor.BuildReporting;

namespace UnityEditor.DeploymentTargets
{
	internal class DefaultDeploymentTargetInfo : IDeploymentTargetInfo
	{
		public virtual FlagSet<DeploymentTargetSupportFlags> GetSupportFlags()
		{
			return DeploymentTargetSupportFlags.None;
		}

		public virtual BuildCheckResult CheckBuild(BuildReport buildReport)
		{
			return default(BuildCheckResult);
		}
	}
}
