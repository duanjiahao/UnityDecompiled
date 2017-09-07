using System;

namespace UnityEditor.DeploymentTargets
{
	internal class NoResponseFromDeploymentTargetException : OperationFailedException
	{
		public NoResponseFromDeploymentTargetException(string message = "No response from deployment target.") : base("No response from deployment target", message)
		{
		}
	}
}
