using System;

namespace UnityEditor.DeploymentTargets
{
	internal class CorruptBuildException : OperationFailedException
	{
		public CorruptBuildException(string message = "Corrupt build.") : base("Corrupt build", message)
		{
		}
	}
}
