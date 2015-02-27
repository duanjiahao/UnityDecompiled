using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class VisualizationBlendTree
	{
		private AnimatorController m_Controller;
		private StateMachine m_StateMachine;
		private State m_State;
		private BlendTree m_BlendTree;
		private Animator m_Animator;
		private bool m_ControllerIsDirty;
		public Animator animator
		{
			get
			{
				return this.m_Animator;
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
			if (this.m_Controller == null)
			{
				this.m_Controller = new AnimatorController();
				this.m_Controller.hideFlags = HideFlags.DontSave;
				this.m_Controller.AddLayer("preview");
				this.m_StateMachine = this.m_Controller.GetLayerStateMachine(0);
				this.CreateParameters();
				this.m_State = this.m_StateMachine.AddState("preview");
				this.m_State.SetMotionInternal(this.m_BlendTree);
				this.m_State.iKOnFeet = false;
				this.m_State.hideFlags = HideFlags.DontSave;
				AnimatorController.SetAnimatorController(this.m_Animator, this.m_Controller);
				AnimatorController expr_A7 = this.m_Controller;
				expr_A7.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_A7.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
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
				AnimatorController expr_34 = this.m_Controller;
				expr_34.OnAnimatorControllerDirty = (Action)Delegate.Remove(expr_34.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
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
			for (int i = 0; i < this.m_BlendTree.recursiveBlendParameterCount; i++)
			{
				string recursiveBlendParameter = this.m_BlendTree.GetRecursiveBlendParameter(i);
				float inputBlendValue = this.m_BlendTree.GetInputBlendValue(recursiveBlendParameter);
				this.animator.SetFloat(recursiveBlendParameter, inputBlendValue);
			}
			this.animator.EvaluateSM();
		}
	}
}
