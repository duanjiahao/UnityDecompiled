using System;
using UnityEditor;
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

		bool DiscardModification(PropertyModification modification);

		void SaveCurve(AnimationWindowCurve curve);

		void AddPropertyModification(EditorCurveBinding binding, PropertyModification propertyModification, bool keepPrefabOverride);
	}
}
