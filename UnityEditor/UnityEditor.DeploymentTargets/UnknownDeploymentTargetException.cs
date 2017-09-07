using System;

namespace UnityEditor.DeploymentTargets
{
	internal class UnknownDeploymentTargetException : OperationFailedException
	{
		public UnknownDeploymentTargetException(string message = "Unknown deployment target.") : base("Cannot find deployment target", message)
		{
		}
	}
}
