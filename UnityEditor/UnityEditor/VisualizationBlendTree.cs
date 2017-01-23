using System;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditor
{
	internal class VisualizationBlendTree
	{
		private AnimatorController m_Controller;

		private AnimatorStateMachine m_StateMachine;

		private AnimatorState m_State;

		private BlendTree m_BlendTree;

		private Animator m_Animator;

		private bool m_ControllerIsDirty = false;

		public Animator animator
		{
			get
			{
				return this.m_Animator;
			}
		}

		public bool controllerDirty
		{
			get
			{
				return this.m_ControllerIsDirty;
			}
		}

		public void Init(BlendTree blendTree, Animator animator)
		{
			this.m_BlendTree = blendTree;
			this.m_Animator = animator;
			this.m_Animator.logWarnings = false;
			this.m_Animator.fireEvents = false;
			this.m_Animator.enabled = false;
			this.m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.CreateStateMachine();
		}

		protected virtual void ControllerDirty()
		{
			this.m_ControllerIsDirty = true;
		}

		private void CreateParameters()
		{
			for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
			{
				this.m_Controller.AddParameter(this.m_BlendTree.GetRecursiveBlendParameter(i), AnimatorControllerParameterType.Float);
			}
		}

		private void CreateStateMachine()
		{
			if (this.m_Controller == null)
			{
				this.m_Controller = new AnimatorController();
				this.m_Controller.pushUndo = false;
				this.m_Controller.AddLayer("viz");
				this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
				this.m_StateMachine.pushUndo = false;
				this.CreateParameters();
				this.m_State = this.m_StateMachine.AddState("viz");
				this.m_State.pushUndo = false;
				this.m_State.motion = this.m_BlendTree;
				this.m_State.iKOnFeet = false;
				this.m_State.hideFlags = HideFlags.HideAndDontSave;
				this.m_StateMachine.hideFlags = HideFlags.HideAndDontSave;
				this.m_Controller.hideFlags = HideFlags.HideAndDontSave;
				AnimatorController.SetAnimatorController(this.m_Animator, this.m_Controller);
				AnimatorController expr_E1 = this.m_Controller;
				expr_E1.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_E1.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
				this.m_ControllerIsDirty = false;
			}
		}

		private void ClearStateMachine()
		{
			if (this.m_Animator != null)
			{
				AnimatorController.SetAnimatorController(this.m_Animator, null);
			}
			if (this.m_Controller != null)
			{
				AnimatorController expr_35 = this.m_Controller;
				expr_35.OnAnimatorControllerDirty = (Action)Delegate.Remove(expr_35.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
			}
			UnityEngine.Object.DestroyImmediate(this.m_Controller);
			UnityEngine.Object.DestroyImmediate(this.m_State);
			this.m_StateMachine = null;
			this.m_Controller = null;
			this.m_State = null;
		}

		public void Reset()
		{
			this.ClearStateMachine();
			this.CreateStateMachine();
		}

		public void Destroy()
		{
			this.ClearStateMachine();
		}

		public void Update()
		{
			if (this.m_ControllerIsDirty)
			{
				this.Reset();
			}
			int recursiveBlendParameterCount = this.m_BlendTree.recursiveBlendParameterCount;
			if (this.m_Controller.parameters.Length >= recursiveBlendParameterCount)
			{
				for (int i = 0; i < recursiveBlendParameterCount; i++)
				{
					string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
					float parameterValue = BlendTreeInspector.GetParameterValue(this.animator, this.m_BlendTree, recursiveBlendParameter);
					this.animator.SetFloat(recursiveBlendParameter, parameterValue);
				}
				this.animator.EvaluateController();
			}
		}
	}
}
