using System;
using UnityEngine;

namespace UnityEditor.Animations
{
	public sealed class AnimatorControllerLayer
	{
		private string m_Name;

		private AnimatorStateMachine m_StateMachine;

		private AvatarMask m_AvatarMask;

		private StateMotionPair[] m_Motions;

		private StateBehavioursPair[] m_Behaviours;

		private AnimatorLayerBlendingMode m_BlendingMode;

		private int m_SyncedLayerIndex = -1;

		private bool m_IKPass;

		private float m_DefaultWeight;

		private bool m_SyncedLayerAffectsTiming;

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

		public AnimatorStateMachine stateMachine
		{
			get
			{
				return this.m_StateMachine;
			}
			set
			{
				this.m_StateMachine = value;
			}
		}

		public AvatarMask avatarMask
		{
			get
			{
				return this.m_AvatarMask;
			}
			set
			{
				this.m_AvatarMask = value;
			}
		}

		public AnimatorLayerBlendingMode blendingMode
		{
			get
			{
				return this.m_BlendingMode;
			}
			set
			{
				this.m_BlendingMode = value;
			}
		}

		public int syncedLayerIndex
		{
			get
			{
				return this.m_SyncedLayerIndex;
			}
			set
			{
				this.m_SyncedLayerIndex = value;
			}
		}

		public bool iKPass
		{
			get
			{
				return this.m_IKPass;
			}
			set
			{
				this.m_IKPass = value;
			}
		}

		public float defaultWeight
		{
			get
			{
				return this.m_DefaultWeight;
			}
			set
			{
				this.m_DefaultWeight = value;
			}
		}

		public bool syncedLayerAffectsTiming
		{
			get
			{
				return this.m_SyncedLayerAffectsTiming;
			}
			set
			{
				this.m_SyncedLayerAffectsTiming = value;
			}
		}

		public Motion GetOverrideMotion(AnimatorState state)
		{
			if (this.m_Motions != null)
			{
				StateMotionPair[] motions = this.m_Motions;
				for (int i = 0; i < motions.Length; i++)
				{
					StateMotionPair stateMotionPair = motions[i];
					if (stateMotionPair.m_State == state)
					{
						return stateMotionPair.m_Motion;
					}
				}
			}
			return null;
		}

		public void SetOverrideMotion(AnimatorState state, Motion motion)
		{
			if (this.m_Motions == null)
			{
				this.m_Motions = new StateMotionPair[0];
			}
			for (int i = 0; i < this.m_Motions.Length; i++)
			{
				if (this.m_Motions[i].m_State == state)
				{
					this.m_Motions[i].m_Motion = motion;
					return;
				}
			}
			StateMotionPair item;
			item.m_State = state;
			item.m_Motion = motion;
			ArrayUtility.Add<StateMotionPair>(ref this.m_Motions, item);
		}

		public StateMachineBehaviour[] GetOverrideBehaviours(AnimatorState state)
		{
			if (this.m_Behaviours != null)
			{
				StateBehavioursPair[] behaviours = this.m_Behaviours;
				for (int i = 0; i < behaviours.Length; i++)
				{
					StateBehavioursPair stateBehavioursPair = behaviours[i];
					if (stateBehavioursPair.m_State == state)
					{
						return stateBehavioursPair.m_Behaviours;
					}
				}
			}
			return new StateMachineBehaviour[0];
		}

		public void SetOverrideBehaviours(AnimatorState state, StateMachineBehaviour[] behaviours)
		{
			if (this.m_Behaviours == null)
			{
				this.m_Behaviours = new StateBehavioursPair[0];
			}
			for (int i = 0; i < this.m_Behaviours.Length; i++)
			{
				if (this.m_Behaviours[i].m_State == state)
				{
					this.m_Behaviours[i].m_Behaviours = behaviours;
					return;
				}
			}
			StateBehavioursPair item;
			item.m_State = state;
			item.m_Behaviours = behaviours;
			ArrayUtility.Add<StateBehavioursPair>(ref this.m_Behaviours, item);
		}
	}
}
