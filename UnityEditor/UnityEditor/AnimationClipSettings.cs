using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	public sealed class AnimationClipSettings
	{
		public AnimationClip additiveReferencePoseClip;

		public float additiveReferencePoseTime;

		public float startTime;

		public float stopTime;

		public float orientationOffsetY;

		public float level;

		public float cycleOffset;

		public bool hasAdditiveReferencePose;

		public bool loopTime;

		public bool loopBlend;

		public bool loopBlendOrientation;

		public bool loopBlendPositionY;

		public bool loopBlendPositionXZ;

		public bool keepOriginalOrientation;

		public bool keepOriginalPositionY;

		public bool keepOriginalPositionXZ;

		public bool heightFromFeet;

		public bool mirror;
	}
}
