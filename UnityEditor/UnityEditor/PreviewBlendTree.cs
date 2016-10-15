using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor
{
	internal class PreviewBlendTree
	{
		private AnimatorController m_Controller;

		private AvatarPreview m_AvatarPreview;

		private AnimatorStateMachine m_StateMachine;

		private AnimatorState m_State;

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
			for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
			{
				this.m_Controller.AddParameter(this.m_BlendTree.GetRecursiveBlendParameter(i), AnimatorControllerParameterType.Float);
			}
		}

		private void CreateStateMachine()
		{
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				if (this.m_Controller == null)
				{
					this.m_Controller = new AnimatorController();
					this.m_Controller.pushUndo = false;
					this.m_Controller.AddLayer("preview");
					this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
					this.m_StateMachine.pushUndo = false;
					this.CreateParameters();
					this.m_State = this.m_StateMachine.AddState("preview");
					this.m_State.pushUndo = false;
					this.m_State.motion = this.m_BlendTree;
					this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
					this.m_State.hideFlags = HideFlags.HideAndDontSave;
					this.m_Controller.hideFlags = HideFlags.HideAndDontSave;
					this.m_StateMachine.hideFlags = HideFlags.HideAndDontSave;
					AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
					AnimatorController expr_10F = this.m_Controller;
					expr_10F.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_10F.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
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
			if (this.m_AvatarPreview.PreviewObject == null || this.m_ControllerIsDirty)
			{
				this.m_AvatarPreview.ResetPreviewInstance();
				if (this.m_AvatarPreview.PreviewObject)
				{
					this.ResetStateMachine();
				}
			}
			if (this.m_AvatarPreview.Animator)
			{
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
				if (this.m_AvatarPreview.Animator)
				{
					for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
					{
						string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
						float parameterValue = BlendTreeInspector.GetParameterValue(this.m_AvatarPreview.Animator, this.m_BlendTree, recursiveBlendParameter);
						this.m_AvatarPreview.Animator.SetFloat(recursiveBlendParameter, parameterValue);
					}
				}
				this.m_AvatarPreview.timeControl.loop = true;
				float num = 1f;
				float num2 = 0f;
				if (this.m_AvatarPreview.Animator.layerCount > 0)
				{
					AnimatorStateInfo currentAnimatorStateInfo = this.m_AvatarPreview.Animator.GetCurrentAnimatorStateInfo(0);
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
					else if (num2 < 0f)
					{
						num3 += num;
					}
				}
				this.m_AvatarPreview.Animator.Update(num3);
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
