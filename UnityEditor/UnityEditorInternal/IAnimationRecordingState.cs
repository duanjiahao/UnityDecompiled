using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal interface IAnimationRecordingState
	{
		GameObject activeGameObject
		{
			get;
		}

		GameObject activeRootGameObject
		{
			get;
		}

		AnimationClip activeAnimationClip
		{
			get;
		}

		int currentFrame
		{
			get;
		}

		bool addZeroFrame
		{
			get;
		}

		void SaveCurve(AnimationWindowCurve curve);
	}
}
