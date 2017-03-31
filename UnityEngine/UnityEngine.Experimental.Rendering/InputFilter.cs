using System;

namespace UnityEngine.Experimental.Rendering
{
	public struct InputFilter
	{
		public int renderQueueMin;

		public int renderQueueMax;

		public int layerMask;

		public static InputFilter Default()
		{
			return new InputFilter
			{
				renderQueueMin = 0,
				renderQueueMax = 5000,
				layerMask = -1
			};
		}

		public void SetQueuesOpaque()
		{
			this.renderQueueMin = 0;
			this.renderQueueMax = 2500;
		}

		public void SetQueuesTransparent()
		{
			this.renderQueueMin = 2501;
			this.renderQueueMax = 5000;
		}
	}
}
