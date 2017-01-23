using System;

namespace UnityEditor
{
	internal interface IGameViewSizeMenuUser
	{
		bool lowResolutionForAspectRatios
		{
			get;
			set;
		}

		bool forceLowResolutionAspectRatios
		{
			get;
		}

		bool showLowResolutionToggle
		{
			get;
		}

		void SizeSelectionCallback(int indexClicked, object objectSelected);
	}
}
