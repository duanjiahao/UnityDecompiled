using System;

namespace UnityEngine
{
	public struct HumanPose
	{
		public Vector3 bodyPosition;

		public Quaternion bodyRotation;

		public float[] muscles;

		internal void Init()
		{
			if (this.muscles != null && this.muscles.Length != HumanTrait.MuscleCount)
			{
				throw new ArgumentException("Bad array size for HumanPose.muscles. Size must equal HumanTrait.MuscleCount");
			}
			if (this.muscles == null)
			{
				this.muscles = new float[HumanTrait.MuscleCount];
				if (this.bodyRotation.x == 0f && this.bodyRotation.y == 0f && this.bodyRotation.z == 0f && this.bodyRotation.w == 0f)
				{
					this.bodyRotation.w = 1f;
				}
			}
		}
	}
}
