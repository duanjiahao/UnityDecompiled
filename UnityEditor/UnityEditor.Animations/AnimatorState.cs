using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEditor.Animations
{
	public sealed class AnimatorState : UnityEngine.Object
	{
		private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

		public extern int nameHash
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Motion motion
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float speed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float cycleOffset
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool mirror
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool iKOnFeet
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool writeDefaultValues
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string tag
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string speedParameter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string cycleOffsetParameter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string mirrorParameter
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool speedParameterActive
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool cycleOffsetParameterActive
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool mirrorParameterActive
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorStateTransition[] transitions
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern StateMachineBehaviour[] behaviours
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal bool pushUndo
		{
			set
			{
				this.undoHandler.pushUndo = value;
			}
		}

		[Obsolete("uniqueName does not exist anymore. Consider using .name instead.", true)]
		public string uniqueName
		{
			get
			{
				return "";
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorState mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddBehaviour(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveBehaviour(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern MonoScript GetBehaviourMonoScript(int index);

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

		private AnimatorStateTransition CreateTransition(bool setDefaultExitTime)
		{
			AnimatorStateTransition animatorStateTransition = new AnimatorStateTransition();
			animatorStateTransition.hasExitTime = false;
			animatorStateTransition.hasFixedDuration = true;
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(animatorStateTransition, AssetDatabase.GetAssetPath(this));
			}
			animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
			if (setDefaultExitTime)
			{
				this.SetDefaultTransitionExitTime(ref animatorStateTransition);
			}
			return animatorStateTransition;
		}

		private void SetDefaultTransitionExitTime(ref AnimatorStateTransition newTransition)
		{
			newTransition.hasExitTime = true;
			if (this.motion != null && this.motion.averageDuration > 0f)
			{
				float num = 0.25f / this.motion.averageDuration;
				newTransition.duration = 0.25f;
				newTransition.exitTime = 1f - num;
			}
			else
			{
				newTransition.duration = 0.25f;
				newTransition.exitTime = 0.75f;
			}
		}

		public AnimatorStateTransition AddTransition(AnimatorState destinationState)
		{
			AnimatorStateTransition animatorStateTransition = this.CreateTransition(false);
			animatorStateTransition.destinationState = destinationState;
			this.AddTransition(animatorStateTransition);
			return animatorStateTransition;
		}

		public AnimatorStateTransition AddTransition(AnimatorStateMachine destinationStateMachine)
		{
			AnimatorStateTransition animatorStateTransition = this.CreateTransition(false);
			animatorStateTransition.destinationStateMachine = destinationStateMachine;
			this.AddTransition(animatorStateTransition);
			return animatorStateTransition;
		}

		public AnimatorStateTransition AddTransition(AnimatorState destinationState, bool defaultExitTime)
		{
			AnimatorStateTransition animatorStateTransition = this.CreateTransition(defaultExitTime);
			animatorStateTransition.destinationState = destinationState;
			this.AddTransition(animatorStateTransition);
			return animatorStateTransition;
		}

		public AnimatorStateTransition AddTransition(AnimatorStateMachine destinationStateMachine, bool defaultExitTime)
		{
			AnimatorStateTransition animatorStateTransition = this.CreateTransition(defaultExitTime);
			animatorStateTransition.destinationStateMachine = destinationStateMachine;
			this.AddTransition(animatorStateTransition);
			return animatorStateTransition;
		}

		public AnimatorStateTransition AddExitTransition()
		{
			return this.AddExitTransition(false);
		}

		public AnimatorStateTransition AddExitTransition(bool defaultExitTime)
		{
			AnimatorStateTransition animatorStateTransition = this.CreateTransition(defaultExitTime);
			animatorStateTransition.isExit = true;
			this.AddTransition(animatorStateTransition);
			return animatorStateTransition;
		}

		internal AnimatorStateMachine FindParent(AnimatorStateMachine root)
		{
			AnimatorStateMachine result;
			if (root.HasState(this, false))
			{
				result = root;
			}
			else
			{
				result = root.stateMachinesRecursive.Find((ChildAnimatorStateMachine sm) => sm.stateMachine.HasState(this, false)).stateMachine;
			}
			return result;
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
