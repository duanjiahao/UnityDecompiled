using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AnimationEvent
	{
		internal float m_Time;

		internal string m_FunctionName;

		internal string m_StringParameter;

		internal Object m_ObjectReferenceParameter;

		internal float m_FloatParameter;

		internal int m_IntParameter;

		internal int m_MessageOptions;

		internal AnimationEventSource m_Source;

		internal AnimationState m_StateSender;

		internal AnimatorStateInfo m_AnimatorStateInfo;

		internal AnimatorClipInfo m_AnimatorClipInfo;

		[Obsolete("Use stringParameter instead")]
		public string data
		{
			get
			{
				return this.m_StringParameter;
			}
			set
			{
				this.m_StringParameter = value;
			}
		}

		public string stringParameter
		{
			get
			{
				return this.m_StringParameter;
			}
			set
			{
				this.m_StringParameter = value;
			}
		}

		public float floatParameter
		{
			get
			{
				return this.m_FloatParameter;
			}
			set
			{
				this.m_FloatParameter = value;
			}
		}

		public int intParameter
		{
			get
			{
				return this.m_IntParameter;
			}
			set
			{
				this.m_IntParameter = value;
			}
		}

		public Object objectReferenceParameter
		{
			get
			{
				return this.m_ObjectReferenceParameter;
			}
			set
			{
				this.m_ObjectReferenceParameter = value;
			}
		}

		public string functionName
		{
			get
			{
				return this.m_FunctionName;
			}
			set
			{
				this.m_FunctionName = value;
			}
		}

		public float time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
			}
		}

		public SendMessageOptions messageOptions
		{
			get
			{
				return (SendMessageOptions)this.m_MessageOptions;
			}
			set
			{
				this.m_MessageOptions = (int)value;
			}
		}

		public bool isFiredByLegacy
		{
			get
			{
				return this.m_Source == AnimationEventSource.Legacy;
			}
		}

		public bool isFiredByAnimator
		{
			get
			{
				return this.m_Source == AnimationEventSource.Animator;
			}
		}

		public AnimationState animationState
		{
			get
			{
				if (!this.isFiredByLegacy)
				{
					Debug.LogError("AnimationEvent was not fired by Animation component, you shouldn't use AnimationEvent.animationState");
				}
				return this.m_StateSender;
			}
		}

		public AnimatorStateInfo animatorStateInfo
		{
			get
			{
				if (!this.isFiredByAnimator)
				{
					Debug.LogError("AnimationEvent was not fired by Animator component, you shouldn't use AnimationEvent.animatorStateInfo");
				}
				return this.m_AnimatorStateInfo;
			}
		}

		public AnimatorClipInfo animatorClipInfo
		{
			get
			{
				if (!this.isFiredByAnimator)
				{
					Debug.LogError("AnimationEvent was not fired by Animator component, you shouldn't use AnimationEvent.animatorClipInfo");
				}
				return this.m_AnimatorClipInfo;
			}
		}

		public AnimationEvent()
		{
			this.m_Time = 0f;
			this.m_FunctionName = string.Empty;
			this.m_StringParameter = string.Empty;
			this.m_ObjectReferenceParameter = null;
			this.m_FloatParameter = 0f;
			this.m_IntParameter = 0;
			this.m_MessageOptions = 0;
			this.m_Source = AnimationEventSource.NoSource;
			this.m_StateSender = null;
		}

		internal int GetHash()
		{
			int hashCode = this.functionName.GetHashCode();
			return 33 * hashCode + this.time.GetHashCode();
		}
	}
}
