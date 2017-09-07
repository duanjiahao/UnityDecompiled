using System;

namespace UnityEditor.DeploymentTargets
{
	internal class OperationFailedException : Exception
	{
		public readonly string title;

		public OperationFailedException(string title, string message) : base(message)
		{
			this.title = title;
		}
	}
}
