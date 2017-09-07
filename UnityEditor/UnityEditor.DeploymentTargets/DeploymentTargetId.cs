using System;

namespace UnityEditor.DeploymentTargets
{
	internal struct DeploymentTargetId
	{
		public string id;

		public DeploymentTargetId(string id)
		{
			this.id = id;
		}

		public static implicit operator DeploymentTargetId(string id)
		{
			return new DeploymentTargetId(id);
		}

		public static implicit operator string(DeploymentTargetId id)
		{
			return id.id;
		}
	}
}
