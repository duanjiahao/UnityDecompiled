using System;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class AnimatorCondition
	{
		private Transition m_Transition;
		private int m_Index;
		private bool m_Valid;
		public TransitionConditionMode mode
		{
			get
			{
				return (!this.checkValid()) ? ((TransitionConditionMode)0) : this.m_Transition.GetConditionMode(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Transition.SetConditionMode(this.m_Index, value);
				}
			}
		}
		public string parameter
		{
			get
			{
				return (!this.checkValid()) ? string.Empty : this.m_Transition.GetConditionParameter(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Transition.SetConditionParameter(this.m_Index, value);
				}
			}
		}
		public float threshold
		{
			get
			{
				return (!this.checkValid()) ? 0f : this.m_Transition.GetParameterTreshold(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Transition.SetParameterTreshold(this.m_Index, value);
				}
			}
		}
		public float exitTime
		{
			get
			{
				return (!this.checkValid()) ? 0f : this.m_Transition.GetExitTime(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Transition.SetExitTime(this.m_Index, value);
				}
			}
		}
		internal AnimatorCondition(Transition transition, int index)
		{
			this.m_Transition = transition;
			this.m_Index = index;
			transition.onRemovedCondition = (Transition.RemovedCondition)Delegate.Combine(transition.onRemovedCondition, new Transition.RemovedCondition(this.RemoveCondition));
			this.m_Valid = true;
		}
		private bool checkValid()
		{
			if (!this.m_Valid)
			{
				Debug.LogError("AnimatorCondition is not valid anymore. It was removed");
				return false;
			}
			return true;
		}
		private void RemoveCondition(int index)
		{
			if (index == this.m_Index)
			{
				this.m_Valid = false;
			}
			else
			{
				if (index < this.m_Index)
				{
					this.m_Index--;
				}
			}
		}
	}
}
