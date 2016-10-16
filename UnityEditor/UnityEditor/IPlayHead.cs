using System;

namespace UnityEditor
{
	internal interface IPlayHead
	{
		float currentTime
		{
			set;
		}

		bool playing
		{
			get;
		}

		bool syncTimeDuringDrag
		{
			get;
		}
	}
}
