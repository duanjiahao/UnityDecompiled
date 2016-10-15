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

		int frame
		{
			get;
		}

		void SaveCurve(AnimationWindowCurve curve);
	}
}
