using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal;
namespace UnityEditor.Animations
{
	public sealed class AnimatorState : UnityEngine.Object
	{
		private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);
		public extern int nameHash
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Motion motion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float speed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool mirror
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool iKOnFeet
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool writeDefaultValues
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern string tag
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern AnimatorStateTransition[] transitions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern StateMachineBehaviour[] behaviours
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal bool pushUndo
		{
			set
			{
				this.undoHandler.m_PushUndo = value;
			}
		}
		[Obsolete("uniqueName does not exist anymore. Consider using .name instead.", true)]
		public string uniqueName
		{
			get
			{
				return string.Empty;
			}
		}
		[Obsolete("uniqueNameHash does not exist anymore.", true)]
		public int uniqueNameHash
		{
			get
			{
				return -1;
			}
		}
		public AnimatorState()
		{
			AnimatorState.Internal_Create(this);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorState mono);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddBehaviour(int instanceID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType);
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public StateMachineBehaviour AddStateMachineBehaviour(Type stateMachineBehaviourType)
		{
			return (StateMachineBehaviour)this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType);
		}
		public T AddStateMachineBehaviour<T>() where T : StateMachineBehaviour
		{
			return this.AddStateMachineBehaviour(typeof(T)) as T;
		}
		public void AddTransition(AnimatorStateTransition transition)
		{
			this.undoHandler.DoUndo(this, "Transition added");
			AnimatorStateTransition[] transitions = this.transitions;
			ArrayUtility.Add<AnimatorStateTransition>(ref transitions, transition);
			this.transitions = transitions;
		}
		public void RemoveTransition(AnimatorStateTransition transition)
		{
			this.undoHandler.DoUndo(this, "Transition removed");
			AnimatorStateTransition[] transitions = this.transitions;
			ArrayUtility.Remove<AnimatorStateTransition>(ref transitions, transition);
			this.transitions = transitions;
			if (MecanimUtilities.AreSameAsset(this, transition))
			{
				Undo.DestroyObjectImmediate(transition);
			}
		}
		private AnimatorStateTransition AddTransition()
		{
			AnimatorStateTransition animatorStateTransition = new AnimatorStateTransition();
			animatorStateTransition.hasExitTime = false;
			if (AssetDatabase.GetAssetPath(this) != string.Empty)
			{
				AssetDatabase.AddObjectToAsset(animatorStateTransition, AssetDatabase.GetAssetPath(this));
			}
			animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
			this.AddTransition(animatorStateTransition);
			return animatorStateTransition;
		}
		public AnimatorStateTransition AddTransition(AnimatorState destinationState)
		{
			AnimatorStateTransition animatorStateTransition = this.AddTransition();
			animatorStateTransition.destinationState = destinationState;
			return animatorStateTransition;
		}
		public AnimatorStateTransition AddTransition(AnimatorStateMachine destinationStateMachine)
		{
			AnimatorStateTransition animatorStateTransition = this.AddTransition();
			animatorStateTransition.destinationStateMachine = destinationStateMachine;
			return animatorStateTransition;
		}
		private void SetDefaultTransitionExitTime(ref AnimatorStateTransition newTransition)
		{
			newTransition.hasExitTime = true;
			if (this.motion != null && this.motion.averageDuration > 0f)
			{
				float num = 0.25f / this.motion.averageDuration;
				newTransition.duration = num;
				newTransition.exitTime = 1f - num;
			}
		}
		public AnimatorStateTransition AddTransition(AnimatorState destinationState, bool defaultExitTime)
		{
			AnimatorStateTransition result = this.AddTransition(destinationState);
			if (defaultExitTime)
			{
				this.SetDefaultTransitionExitTime(ref result);
			}
			return result;
		}
		public AnimatorStateTransition AddTransition(AnimatorStateMachine destinationStateMachine, bool defaultExitTime)
		{
			AnimatorStateTransition result = this.AddTransition(destinationStateMachine);
			if (defaultExitTime)
			{
				this.SetDefaultTransitionExitTime(ref result);
			}
			return result;
		}
		public AnimatorStateTransition AddExitTransition()
		{
			AnimatorStateTransition animatorStateTransition = this.AddTransition();
			animatorStateTransition.isExit = true;
			return animatorStateTransition;
		}
		public AnimatorStateTransition AddExitTransition(bool defaultExitTime)
		{
			AnimatorStateTransition animatorStateTransition = this.AddTransition();
			animatorStateTransition.isExit = true;
			if (defaultExitTime)
			{
				this.SetDefaultTransitionExitTime(ref animatorStateTransition);
			}
			return animatorStateTransition;
		}
		internal AnimatorStateMachine FindParent(AnimatorStateMachine root)
		{
			if (root.HasState(this, false))
			{
				return root;
			}
			return root.stateMachinesRecursive.Find((ChildAnimatorStateMachine sm) => sm.stateMachine.HasState(this, false)).stateMachine;
		}
		internal AnimatorStateTransition FindTransition(AnimatorState destinationState)
		{
			return new List<AnimatorStateTransition>(this.transitions).Find((AnimatorStateTransition t) => t.destinationState == destinationState);
		}
		[Obsolete("GetMotion() is obsolete. Use motion", true)]
		public Motion GetMotion()
		{
			return null;
		}
	}
}
