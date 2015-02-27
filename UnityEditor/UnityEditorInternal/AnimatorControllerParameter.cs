using System;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class AnimatorControllerParameter
	{
		private AnimatorController m_Controller;
		private int m_Index;
		private bool m_Valid;
		public string name
		{
			get
			{
				return (!this.checkValid()) ? string.Empty : this.m_Controller.GetParameterName(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetParameterName(this.m_Index, value);
				}
			}
		}
		public AnimatorControllerParameterType type
		{
			get
			{
				return (!this.checkValid()) ? AnimatorControllerParameterType.Float : this.m_Controller.GetParameterType(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetParameterType(this.m_Index, value);
				}
			}
		}
		public float defaultFloat
		{
			get
			{
				return (!this.checkValid()) ? 0f : this.m_Controller.GetParameterDefaultFloat(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetParameterDefaultFloat(this.m_Index, value);
				}
			}
		}
		public int defaultInt
		{
			get
			{
				return (!this.checkValid()) ? 0 : this.m_Controller.GetParameterDefaultInt(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetParameterDefaultInt(this.m_Index, value);
				}
			}
		}
		public bool defaultBool
		{
			get
			{
				return this.checkValid() && this.m_Controller.GetParameterDefaultBool(this.m_Index);
			}
			set
			{
				if (this.checkValid())
				{
					this.m_Controller.SetParameterDefaultBool(this.m_Index, value);
				}
			}
		}
		public AnimatorControllerParameter(AnimatorController controller, int index)
		{
			this.m_Controller = controller;
			this.m_Index = index;
			this.m_Valid = true;
			controller.onRemovedParameter = (AnimatorController.RemovedParameter)Delegate.Combine(controller.onRemovedParameter, new AnimatorController.RemovedParameter(this.RemoveParameter));
		}
		private bool checkValid()
		{
			if (!this.m_Valid)
			{
				Debug.LogError("AnimatorControllerParameter is not valid anymore. It was removed");
				return false;
			}
			return true;
		}
		private void RemoveParameter(int index)
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
