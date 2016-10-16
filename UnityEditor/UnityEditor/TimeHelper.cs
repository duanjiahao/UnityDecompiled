using System;

namespace UnityEditor
{
	internal struct TimeHelper
	{
		public float deltaTime;

		private long lastTime;

		public void Begin()
		{
			this.lastTime = DateTime.Now.Ticks;
		}

		public float Update()
		{
			this.deltaTime = (float)(DateTime.Now.Ticks - this.lastTime) / 1E+07f;
			this.lastTime = DateTime.Now.Ticks;
			return this.deltaTime;
		}
	}
}
