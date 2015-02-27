using System;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class AnimatorControllerLayer
	{
		private AnimatorController m_Controller;
		private int m_Index;
		private bool m_Valid;
		public string name
		{
			get
			{
				return (!this.checkValid()) ? string.Empty : this.m_Controller.GetLayerName(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetLayerName(this.m_Index, value);
				}
			}
		}
		public StateMachine stateMachine
		{
			get
			{
				return (!this.checkValid()) ? null : this.m_Controller.GetLayerStateMachine(this.m_Index);
			}
		}
		public AvatarMask avatarMask
		{
			get
			{
				return (!this.checkValid()) ? null : this.m_Controller.GetLayerMask(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetLayerMask(this.m_Index, value);
				}
			}
		}
		public AnimatorLayerBlendingMode blendingMode
		{
			get
			{
				return (!this.checkValid()) ? AnimatorLayerBlendingMode.Override : this.m_Controller.GetLayerBlendingMode(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetLayerBlendingMode(this.m_Index, value);
				}
			}
		}
		public int syncedLayerIndex
		{
			get
			{
				return (!this.checkValid()) ? 0 : this.m_Controller.GetLayerSyncedIndex(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetLayerSyncedIndex(this.m_Index, value);
				}
			}
		}
		public int motionSetIndex
		{
			get
			{
				return (!this.checkValid()) ? 0 : this.m_Controller.GetLayerMotionSetIndex(this.m_Index);
			}
		}
		public bool iKPass
		{
			get
			{
				return this.checkValid() && this.m_Controller.GetLayerIKPass(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetLayerIKPass(this.m_Index, value);
				}
			}
		}
		public float defaultWeight
		{
			get
			{
				return (!this.checkValid()) ? 0f : this.m_Controller.GetLayerDefaultWeight(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetLayerDefaultWeight(this.m_Index, value);
				}
			}
		}
		public bool syncedLayerAffectsTiming
		{
			get
			{
				return this.checkValid() && this.m_Controller.GetSyncedLayerAffectsTiming(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetSyncedLayerAffectsTiming(this.m_Index, value);
				}
			}
		}
		public AnimatorControllerLayer(AnimatorController controller, int index)
		{
			this.m_Controller = controller;
			this.m_Index = index;
			controller.onRemovedLayer = (AnimatorController.RemovedLayer)Delegate.Combine(controller.onRemovedLayer, new AnimatorController.RemovedLayer(this.RemoveLayer));
			this.m_Valid = true;
		}
		private bool checkValid()
		{
			if (!this.m_Valid)
			{
				Debug.LogError("AnimatorControllerLayer is not valid anymore. It was removed");
				return false;
			}
			return true;
		}
		private void RemoveLayer(int index)
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
