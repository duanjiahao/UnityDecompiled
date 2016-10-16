using System;

namespace UnityEngine.SocialPlatforms
{
	public struct Range
	{
		public int from;

		public int count;

		public Range(int fromValue, int valueCount)
		{
			this.from = fromValue;
			this.count = valueCount;
		}
	}
}
