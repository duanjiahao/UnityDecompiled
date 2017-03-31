using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	public struct AnimationPlayableOutput
	{
		internal PlayableOutput m_Output;

		public static AnimationPlayableOutput Null
		{
			get
			{
				return new AnimationPlayableOutput
				{
					m_Output = new PlayableOutput
					{
						m_Version = 69
					}
				};
			}
		}

		internal UnityEngine.Object referenceObject
		{
			get
			{
				return PlayableOutput.GetInternalReferenceObject(ref this.m_Output);
			}
			set
			{
				PlayableOutput.SetInternalReferenceObject(ref this.m_Output, value);
			}
		}

		public UnityEngine.Object userData
		{
			get
			{
				return PlayableOutput.GetInternalUserData(ref this.m_Output);
			}
			set
			{
				PlayableOutput.SetInternalUserData(ref this.m_Output, value);
			}
		}

		public Animator target
		{
			get
			{
				return AnimationPlayableOutput.InternalGetTarget(ref this.m_Output);
			}
			set
			{
				AnimationPlayableOutput.InternalSetTarget(ref this.m_Output, value);
			}
		}

		public float weight
		{
			get
			{
				return PlayableOutput.InternalGetWeight(ref this.m_Output);
			}
			set
			{
				PlayableOutput.InternalSetWeight(ref this.m_Output, value);
			}
		}

		public PlayableHandle sourcePlayable
		{
			get
			{
				return PlayableOutput.InternalGetSourcePlayable(ref this.m_Output);
			}
			set
			{
				PlayableOutput.InternalSetSourcePlayable(ref this.m_Output, ref value);
			}
		}

		public bool IsValid()
		{
			return PlayableOutput.IsValidInternal(ref this.m_Output);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Animator InternalGetTarget(ref PlayableOutput output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetTarget(ref PlayableOutput output, Animator target);
	}
}
