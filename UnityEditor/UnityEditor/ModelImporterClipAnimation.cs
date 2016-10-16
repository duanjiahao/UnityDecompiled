using System;
using System.Runtime.InteropServices;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ModelImporterClipAnimation
	{
		private string m_TakeName;

		private string m_Name;

		private float m_FirstFrame;

		private float m_LastFrame;

		private int m_WrapMode;

		private int m_Loop;

		private float m_OrientationOffsetY;

		private float m_Level;

		private float m_CycleOffset;

		private float m_AdditiveReferencePoseFrame;

		private int m_HasAdditiveReferencePose;

		private int m_LoopTime;

		private int m_LoopBlend;

		private int m_LoopBlendOrientation;

		private int m_LoopBlendPositionY;

		private int m_LoopBlendPositionXZ;

		private int m_KeepOriginalOrientation;

		private int m_KeepOriginalPositionY;

		private int m_KeepOriginalPositionXZ;

		private int m_HeightFromFeet;

		private int m_Mirror;

		private int m_MaskType;

		private AvatarMask m_MaskSource;

		private int[] m_BodyMask;

		private AnimationEvent[] m_AnimationEvents;

		private ClipAnimationInfoCurve[] m_AdditionnalCurves;

		private TransformMaskElement[] m_TransformMask;

		private bool m_MaskNeedsUpdating;

		public string takeName
		{
			get
			{
				return this.m_TakeName;
			}
			set
			{
				this.m_TakeName = value;
			}
		}

		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public float firstFrame
		{
			get
			{
				return this.m_FirstFrame;
			}
			set
			{
				this.m_FirstFrame = value;
			}
		}

		public float lastFrame
		{
			get
			{
				return this.m_LastFrame;
			}
			set
			{
				this.m_LastFrame = value;
			}
		}

		public WrapMode wrapMode
		{
			get
			{
				return (WrapMode)this.m_WrapMode;
			}
			set
			{
				this.m_WrapMode = (int)value;
			}
		}

		public bool loop
		{
			get
			{
				return this.m_Loop != 0;
			}
			set
			{
				this.m_Loop = ((!value) ? 0 : 1);
			}
		}

		public float rotationOffset
		{
			get
			{
				return this.m_OrientationOffsetY;
			}
			set
			{
				this.m_OrientationOffsetY = value;
			}
		}

		public float heightOffset
		{
			get
			{
				return this.m_Level;
			}
			set
			{
				this.m_Level = value;
			}
		}

		public float cycleOffset
		{
			get
			{
				return this.m_CycleOffset;
			}
			set
			{
				this.m_CycleOffset = value;
			}
		}

		public bool loopTime
		{
			get
			{
				return this.m_LoopTime != 0;
			}
			set
			{
				this.m_LoopTime = ((!value) ? 0 : 1);
			}
		}

		public bool loopPose
		{
			get
			{
				return this.m_LoopBlend != 0;
			}
			set
			{
				this.m_LoopBlend = ((!value) ? 0 : 1);
			}
		}

		public bool lockRootRotation
		{
			get
			{
				return this.m_LoopBlendOrientation != 0;
			}
			set
			{
				this.m_LoopBlendOrientation = ((!value) ? 0 : 1);
			}
		}

		public bool lockRootHeightY
		{
			get
			{
				return this.m_LoopBlendPositionY != 0;
			}
			set
			{
				this.m_LoopBlendPositionY = ((!value) ? 0 : 1);
			}
		}

		public bool lockRootPositionXZ
		{
			get
			{
				return this.m_LoopBlendPositionXZ != 0;
			}
			set
			{
				this.m_LoopBlendPositionXZ = ((!value) ? 0 : 1);
			}
		}

		public bool keepOriginalOrientation
		{
			get
			{
				return this.m_KeepOriginalOrientation != 0;
			}
			set
			{
				this.m_KeepOriginalOrientation = ((!value) ? 0 : 1);
			}
		}

		public bool keepOriginalPositionY
		{
			get
			{
				return this.m_KeepOriginalPositionY != 0;
			}
			set
			{
				this.m_KeepOriginalPositionY = ((!value) ? 0 : 1);
			}
		}

		public bool keepOriginalPositionXZ
		{
			get
			{
				return this.m_KeepOriginalPositionXZ != 0;
			}
			set
			{
				this.m_KeepOriginalPositionXZ = ((!value) ? 0 : 1);
			}
		}

		public bool heightFromFeet
		{
			get
			{
				return this.m_HeightFromFeet != 0;
			}
			set
			{
				this.m_HeightFromFeet = ((!value) ? 0 : 1);
			}
		}

		public bool mirror
		{
			get
			{
				return this.m_Mirror != 0;
			}
			set
			{
				this.m_Mirror = ((!value) ? 0 : 1);
			}
		}

		public ClipAnimationMaskType maskType
		{
			get
			{
				return (ClipAnimationMaskType)this.m_MaskType;
			}
			set
			{
				this.m_MaskType = (int)value;
			}
		}

		public AvatarMask maskSource
		{
			get
			{
				return this.m_MaskSource;
			}
			set
			{
				this.m_MaskSource = value;
			}
		}

		public AnimationEvent[] events
		{
			get
			{
				return this.m_AnimationEvents;
			}
			set
			{
				this.m_AnimationEvents = value;
			}
		}

		public ClipAnimationInfoCurve[] curves
		{
			get
			{
				return this.m_AdditionnalCurves;
			}
			set
			{
				this.m_AdditionnalCurves = value;
			}
		}

		public bool maskNeedsUpdating
		{
			get
			{
				return this.m_MaskNeedsUpdating;
			}
		}

		public float additiveReferencePoseFrame
		{
			get
			{
				return this.m_AdditiveReferencePoseFrame;
			}
			set
			{
				this.m_AdditiveReferencePoseFrame = value;
			}
		}

		public bool hasAdditiveReferencePose
		{
			get
			{
				return this.m_HasAdditiveReferencePose != 0;
			}
			set
			{
				this.m_HasAdditiveReferencePose = ((!value) ? 0 : 1);
			}
		}

		public override bool Equals(object o)
		{
			ModelImporterClipAnimation modelImporterClipAnimation = o as ModelImporterClipAnimation;
			return modelImporterClipAnimation != null && this.takeName == modelImporterClipAnimation.takeName && this.name == modelImporterClipAnimation.name && this.firstFrame == modelImporterClipAnimation.firstFrame && this.lastFrame == modelImporterClipAnimation.lastFrame && this.m_WrapMode == modelImporterClipAnimation.m_WrapMode && this.m_Loop == modelImporterClipAnimation.m_Loop && this.loopPose == modelImporterClipAnimation.loopPose && this.lockRootRotation == modelImporterClipAnimation.lockRootRotation && this.lockRootHeightY == modelImporterClipAnimation.lockRootHeightY && this.lockRootPositionXZ == modelImporterClipAnimation.lockRootPositionXZ && this.mirror == modelImporterClipAnimation.mirror && this.maskType == modelImporterClipAnimation.maskType && this.maskSource == modelImporterClipAnimation.maskSource && this.additiveReferencePoseFrame == modelImporterClipAnimation.additiveReferencePoseFrame && this.hasAdditiveReferencePose == modelImporterClipAnimation.hasAdditiveReferencePose;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}
	}
}
