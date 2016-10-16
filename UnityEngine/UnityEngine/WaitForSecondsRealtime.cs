using System;

namespace UnityEngine
{
	public class WaitForSecondsRealtime : CustomYieldInstruction
	{
		private float waitTime;

		public override bool keepWaiting
		{
			get
			{
				return Time.realtimeSinceStartup < this.waitTime;
			}
		}

		public WaitForSecondsRealtime(float time)
		{
			this.waitTime = Time.realtimeSinceStartup + time;
		}
	}
}
