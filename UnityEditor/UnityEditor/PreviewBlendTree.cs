using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class PreviewBlendTree
	{
		private AnimatorController m_Controller;
		private AvatarPreview m_AvatarPreview;
		private StateMachine m_StateMachine;
		private State m_State;
		private BlendTree m_BlendTree;
		private bool m_ControllerIsDirty;
		private bool m_PrevIKOnFeet;
		public Animator PreviewAnimator
		{
			get
			{
				return this.m_AvatarPreview.Animator;
			}
		}
		protected virtual void ControllerDirty()
		{
			this.m_ControllerIsDirty = true;
		}
		public void Init(BlendTree blendTree, Animator animator)
		{
			this.m_BlendTree = blendTree;
			if (this.m_AvatarPreview == null)
			{
				this.m_AvatarPreview = new AvatarPreview(animator, this.m_BlendTree);
				this.m_AvatarPreview.OnAvatarChangeFunc = new AvatarPreview.OnAvatarChange(this.OnPreviewAvatarChanged);
				this.m_PrevIKOnFeet = this.m_AvatarPreview.IKOnFeet;
			}
			this.CreateStateMachine();
		}
		public void CreateParameters()
		{
			int parameterCount = this.m_Controller.parameterCount;
			for (int i = 0; i < parameterCount; i++)
			{
				this.m_Controller.RemoveParameter(0);
			}
			for (int j = 0; j < this.m_BlendTree.recursiveBlendParameterCount; j++)
			{
				this.m_Controller.AddParameter(this.m_BlendTree.GetRecursiveBlendParameter(j), AnimatorControllerParameterType.Float);
			}
		}
		private void CreateStateMachine()
		{
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				if (this.m_Controller == null)
				{
					this.m_Controller = new AnimatorController();
					this.m_Controller.hideFlags = HideFlags.DontSave;
					this.m_Controller.AddLayer("preview");
					this.m_StateMachine = this.m_Controller.GetLayerStateMachine(0);
					this.CreateParameters();
					this.m_State = this.m_StateMachine.AddState("preview");
					this.m_State.SetMotionInternal(this.m_BlendTree);
					this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
					this.m_State.hideFlags = HideFlags.DontSave;
					AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
					AnimatorController expr_D7 = this.m_Controller;
					expr_D7.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_D7.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
					this.m_ControllerIsDirty = false;
				}
				if (AnimatorController.GetEffectiveAnimatorController(this.m_AvatarPreview.Animator) != this.m_Controller)
				{
					AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				}
			}
		}
		private void ClearStateMachine()
		{
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, null);
			}
			if (this.m_Controller != null)
			{
				AnimatorController expr_49 = this.m_Controller;
				expr_49.OnAnimatorControllerDirty = (Action)Delegate.Remove(expr_49.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
			}
			UnityEngine.Object.DestroyImmediate(this.m_Controller);
			UnityEngine.Object.DestroyImmediate(this.m_State);
			this.m_StateMachine = null;
			this.m_Controller = null;
			this.m_State = null;
		}
		private void OnPreviewAvatarChanged()
		{
			this.ResetStateMachine();
		}
		public void ResetStateMachine()
		{
			this.ClearStateMachine();
			this.CreateStateMachine();
		}
		public void OnDisable()
		{
			this.ClearStateMachine();
			this.m_AvatarPreview.OnDestroy();
		}
		public void OnDestroy()
		{
			this.ClearStateMachine();
			if (this.m_AvatarPreview != null)
			{
				this.m_AvatarPreview.OnDestroy();
				this.m_AvatarPreview = null;
			}
		}
		private void UpdateAvatarState()
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			Animator animator = this.m_AvatarPreview.Animator;
			if (animator)
			{
				if (this.m_ControllerIsDirty)
				{
					this.ResetStateMachine();
				}
				if (this.m_PrevIKOnFeet != this.m_AvatarPreview.IKOnFeet)
				{
					this.m_PrevIKOnFeet = this.m_AvatarPreview.IKOnFeet;
					Vector3 rootPosition = this.m_AvatarPreview.Animator.rootPosition;
					Quaternion rootRotation = this.m_AvatarPreview.Animator.rootRotation;
					this.ResetStateMachine();
					this.m_AvatarPreview.Animator.Update(this.m_AvatarPreview.timeControl.currentTime);
					this.m_AvatarPreview.Animator.Update(0f);
					this.m_AvatarPreview.Animator.rootPosition = rootPosition;
					this.m_AvatarPreview.Animator.rootRotation = rootRotation;
				}
				if (animator)
				{
					for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
					{
						string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
						float inputBlendValue = this.m_BlendTree.GetInputBlendValue(recursiveBlendParameter);
						animator.SetFloat(recursiveBlendParameter, inputBlendValue);
					}
				}
				this.m_AvatarPreview.timeControl.loop = true;
				float num = 1f;
				float num2 = 0f;
				if (animator.layerCount > 0)
				{
					AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
					num = currentAnimatorStateInfo.length;
					num2 = currentAnimatorStateInfo.normalizedTime;
				}
				this.m_AvatarPreview.timeControl.startTime = 0f;
				this.m_AvatarPreview.timeControl.stopTime = num;
				this.m_AvatarPreview.timeControl.Update();
				float num3 = this.m_AvatarPreview.timeControl.deltaTime;
				if (!this.m_BlendTree.isLooping)
				{
					if (num2 >= 1f)
					{
						num3 -= num;
					}
					else
					{
						if (num2 < 0f)
						{
							num3 += num;
						}
					}
				}
				animator.Update(num3);
			}
		}
		public void TestForReset()
		{
			if (this.m_State != null && this.m_AvatarPreview != null && this.m_State.iKOnFeet != this.m_AvatarPreview.IKOnFeet)
			{
				this.ResetStateMachine();
			}
		}
		public bool HasPreviewGUI()
		{
			return true;
		}
		public void OnPreviewSettings()
		{
			this.m_AvatarPreview.DoPreviewSettings();
		}
		public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			this.UpdateAvatarState();
			this.m_AvatarPreview.DoAvatarPreview(r, background);
		}
	}
}
