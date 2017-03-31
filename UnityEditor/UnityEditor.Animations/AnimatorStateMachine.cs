using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEditor.Animations
{
	public sealed class AnimatorStateMachine : UnityEngine.Object
	{
		private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

		public extern ChildAnimatorState[] states
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ChildAnimatorStateMachine[] stateMachines
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorState defaultState
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 anyStatePosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_anyStatePosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_anyStatePosition(ref value);
			}
		}

		public Vector3 entryPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_entryPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_entryPosition(ref value);
			}
		}

		public Vector3 exitPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_exitPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_exitPosition(ref value);
			}
		}

		public Vector3 parentStateMachinePosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_parentStateMachinePosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_parentStateMachinePosition(ref value);
			}
		}

		public extern AnimatorStateTransition[] anyStateTransitions
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorTransition[] entryTransitions
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

		internal extern int transitionCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal bool pushUndo
		{
			set
			{
				this.undoHandler.pushUndo = value;
			}
		}

		internal List<ChildAnimatorState> statesRecursive
		{
			get
			{
				List<ChildAnimatorState> list = new List<ChildAnimatorState>();
				list.AddRange(this.states);
				for (int i = 0; i < this.stateMachines.Length; i++)
				{
					list.AddRange(this.stateMachines[i].stateMachine.statesRecursive);
				}
				return list;
			}
		}

		internal List<ChildAnimatorStateMachine> stateMachinesRecursive
		{
			get
			{
				List<ChildAnimatorStateMachine> list = new List<ChildAnimatorStateMachine>();
				list.AddRange(this.stateMachines);
				for (int i = 0; i < this.stateMachines.Length; i++)
				{
					list.AddRange(this.stateMachines[i].stateMachine.stateMachinesRecursive);
				}
				return list;
			}
		}

		internal List<AnimatorStateTransition> anyStateTransitionsRecursive
		{
			get
			{
				List<AnimatorStateTransition> list = new List<AnimatorStateTransition>();
				list.AddRange(this.anyStateTransitions);
				ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
				for (int i = 0; i < stateMachines.Length; i++)
				{
					ChildAnimatorStateMachine childAnimatorStateMachine = stateMachines[i];
					list.AddRange(childAnimatorStateMachine.stateMachine.anyStateTransitionsRecursive);
				}
				return list;
			}
		}

		[Obsolete("stateCount is obsolete. Use .states.Length  instead.", true)]
		private int stateCount
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("stateMachineCount is obsolete. Use .stateMachines.Length instead.", true)]
		private int stateMachineCount
		{
			get
			{
				return 0;
			}
		}

		[Obsolete("uniqueNameHash does not exist anymore.", true)]
		private int uniqueNameHash
		{
			get
			{
				return -1;
			}
		}

		public AnimatorStateMachine()
		{
			AnimatorStateMachine.Internal_Create(this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorStateMachine mono);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anyStatePosition(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anyStatePosition(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_entryPosition(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_entryPosition(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_exitPosition(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_exitPosition(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_parentStateMachinePosition(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_parentStateMachinePosition(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorTransition[] GetStateMachineTransitions(AnimatorStateMachine sourceStateMachine);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetStateMachineTransitions(AnimatorStateMachine sourceStateMachine, AnimatorTransition[] transitions);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddBehaviour(int instanceID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveBehaviour(int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern MonoScript GetBehaviourMonoScript(int index);

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string MakeUniqueStateName(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string MakeUniqueStateMachineName(string name);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Clear();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveStateInternal(AnimatorState state);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveStateMachineInternal(AnimatorStateMachine stateMachine);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MoveState(AnimatorState state, AnimatorStateMachine target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MoveStateMachine(AnimatorStateMachine stateMachine, AnimatorStateMachine target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasState(AnimatorState state, bool recursive);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasStateMachine(AnimatorStateMachine state, bool recursive);

		internal Vector3 GetStatePosition(AnimatorState state)
		{
			ChildAnimatorState[] states = this.states;
			Vector3 result;
			for (int i = 0; i < states.Length; i++)
			{
				if (state == states[i].state)
				{
					result = states[i].position;
					return result;
				}
			}
			result = Vector3.zero;
			return result;
		}

		internal void SetStatePosition(AnimatorState state, Vector3 position)
		{
			ChildAnimatorState[] states = this.states;
			for (int i = 0; i < states.Length; i++)
			{
				if (state == states[i].state)
				{
					states[i].position = position;
					this.states = states;
					break;
				}
			}
		}

		internal Vector3 GetStateMachinePosition(AnimatorStateMachine stateMachine)
		{
			ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
			Vector3 result;
			for (int i = 0; i < stateMachines.Length; i++)
			{
				if (stateMachine == stateMachines[i].stateMachine)
				{
					result = stateMachines[i].position;
					return result;
				}
			}
			result = Vector3.zero;
			return result;
		}

		internal void SetStateMachinePosition(AnimatorStateMachine stateMachine, Vector3 position)
		{
			ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
			for (int i = 0; i < stateMachines.Length; i++)
			{
				if (stateMachine == stateMachines[i].stateMachine)
				{
					stateMachines[i].position = position;
					this.stateMachines = stateMachines;
					break;
				}
			}
		}

		public AnimatorState AddState(string name)
		{
			return this.AddState(name, (this.states.Length <= 0) ? new Vector3(200f, 0f, 0f) : (this.states[this.states.Length - 1].position + new Vector3(35f, 65f)));
		}

		public AnimatorState AddState(string name, Vector3 position)
		{
			AnimatorState animatorState = new AnimatorState();
			animatorState.hideFlags = HideFlags.HideInHierarchy;
			animatorState.name = this.MakeUniqueStateName(name);
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(animatorState, AssetDatabase.GetAssetPath(this));
			}
			this.AddState(animatorState, position);
			return animatorState;
		}

		public void AddState(AnimatorState state, Vector3 position)
		{
			this.undoHandler.DoUndo(this, "State added");
			ChildAnimatorState item = default(ChildAnimatorState);
			item.state = state;
			item.position = position;
			ChildAnimatorState[] states = this.states;
			ArrayUtility.Add<ChildAnimatorState>(ref states, item);
			this.states = states;
		}

		public void RemoveState(AnimatorState state)
		{
			this.undoHandler.DoUndo(this, "State removed");
			this.undoHandler.DoUndo(state, "State removed");
			this.RemoveStateInternal(state);
		}

		public AnimatorStateMachine AddStateMachine(string name)
		{
			return this.AddStateMachine(name, Vector3.zero);
		}

		public AnimatorStateMachine AddStateMachine(string name, Vector3 position)
		{
			AnimatorStateMachine animatorStateMachine = new AnimatorStateMachine();
			animatorStateMachine.hideFlags = HideFlags.HideInHierarchy;
			animatorStateMachine.name = this.MakeUniqueStateMachineName(name);
			this.AddStateMachine(animatorStateMachine, position);
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(animatorStateMachine, AssetDatabase.GetAssetPath(this));
			}
			return animatorStateMachine;
		}

		public void AddStateMachine(AnimatorStateMachine stateMachine, Vector3 position)
		{
			this.undoHandler.DoUndo(this, "StateMachine " + stateMachine.name + " added");
			ChildAnimatorStateMachine item = default(ChildAnimatorStateMachine);
			item.stateMachine = stateMachine;
			item.position = position;
			ChildAnimatorStateMachine[] stateMachines = this.stateMachines;
			ArrayUtility.Add<ChildAnimatorStateMachine>(ref stateMachines, item);
			this.stateMachines = stateMachines;
		}

		public void RemoveStateMachine(AnimatorStateMachine stateMachine)
		{
			this.undoHandler.DoUndo(this, "StateMachine removed");
			this.undoHandler.DoUndo(stateMachine, "StateMachine removed");
			this.RemoveStateMachineInternal(stateMachine);
		}

		private AnimatorStateTransition AddAnyStateTransition()
		{
			this.undoHandler.DoUndo(this, "AnyState Transition Added");
			AnimatorStateTransition[] anyStateTransitions = this.anyStateTransitions;
			AnimatorStateTransition animatorStateTransition = new AnimatorStateTransition();
			animatorStateTransition.hasExitTime = false;
			animatorStateTransition.hasFixedDuration = true;
			animatorStateTransition.duration = 0.25f;
			animatorStateTransition.exitTime = 0.75f;
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(animatorStateTransition, AssetDatabase.GetAssetPath(this));
			}
			animatorStateTransition.hideFlags = HideFlags.HideInHierarchy;
			ArrayUtility.Add<AnimatorStateTransition>(ref anyStateTransitions, animatorStateTransition);
			this.anyStateTransitions = anyStateTransitions;
			return animatorStateTransition;
		}

		public AnimatorStateTransition AddAnyStateTransition(AnimatorState destinationState)
		{
			AnimatorStateTransition animatorStateTransition = this.AddAnyStateTransition();
			animatorStateTransition.destinationState = destinationState;
			return animatorStateTransition;
		}

		public AnimatorStateTransition AddAnyStateTransition(AnimatorStateMachine destinationStateMachine)
		{
			AnimatorStateTransition animatorStateTransition = this.AddAnyStateTransition();
			animatorStateTransition.destinationStateMachine = destinationStateMachine;
			return animatorStateTransition;
		}

		public bool RemoveAnyStateTransition(AnimatorStateTransition transition)
		{
			bool result;
			if (new List<AnimatorStateTransition>(this.anyStateTransitions).Any((AnimatorStateTransition t) => t == transition))
			{
				this.undoHandler.DoUndo(this, "AnyState Transition Removed");
				AnimatorStateTransition[] anyStateTransitions = this.anyStateTransitions;
				ArrayUtility.Remove<AnimatorStateTransition>(ref anyStateTransitions, transition);
				this.anyStateTransitions = anyStateTransitions;
				if (MecanimUtilities.AreSameAsset(this, transition))
				{
					Undo.DestroyObjectImmediate(transition);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal void RemoveAnyStateTransitionRecursive(AnimatorStateTransition transition)
		{
			if (!this.RemoveAnyStateTransition(transition))
			{
				List<ChildAnimatorStateMachine> stateMachinesRecursive = this.stateMachinesRecursive;
				foreach (ChildAnimatorStateMachine current in stateMachinesRecursive)
				{
					if (current.stateMachine.RemoveAnyStateTransition(transition))
					{
						break;
					}
				}
			}
		}

		public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine)
		{
			AnimatorStateMachine destinationStateMachine = null;
			return this.AddStateMachineTransition(sourceStateMachine, destinationStateMachine);
		}

		public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorStateMachine destinationStateMachine)
		{
			this.undoHandler.DoUndo(this, "StateMachine Transition Added");
			AnimatorTransition[] stateMachineTransitions = this.GetStateMachineTransitions(sourceStateMachine);
			AnimatorTransition animatorTransition = new AnimatorTransition();
			if (destinationStateMachine)
			{
				animatorTransition.destinationStateMachine = destinationStateMachine;
			}
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(animatorTransition, AssetDatabase.GetAssetPath(this));
			}
			animatorTransition.hideFlags = HideFlags.HideInHierarchy;
			ArrayUtility.Add<AnimatorTransition>(ref stateMachineTransitions, animatorTransition);
			this.SetStateMachineTransitions(sourceStateMachine, stateMachineTransitions);
			return animatorTransition;
		}

		public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorState destinationState)
		{
			AnimatorTransition animatorTransition = this.AddStateMachineTransition(sourceStateMachine);
			animatorTransition.destinationState = destinationState;
			return animatorTransition;
		}

		public AnimatorTransition AddStateMachineExitTransition(AnimatorStateMachine sourceStateMachine)
		{
			AnimatorTransition animatorTransition = this.AddStateMachineTransition(sourceStateMachine);
			animatorTransition.isExit = true;
			return animatorTransition;
		}

		public bool RemoveStateMachineTransition(AnimatorStateMachine sourceStateMachine, AnimatorTransition transition)
		{
			this.undoHandler.DoUndo(this, "StateMachine Transition Removed");
			AnimatorTransition[] stateMachineTransitions = this.GetStateMachineTransitions(sourceStateMachine);
			int num = stateMachineTransitions.Length;
			ArrayUtility.Remove<AnimatorTransition>(ref stateMachineTransitions, transition);
			this.SetStateMachineTransitions(sourceStateMachine, stateMachineTransitions);
			if (MecanimUtilities.AreSameAsset(this, transition))
			{
				Undo.DestroyObjectImmediate(transition);
			}
			return num != stateMachineTransitions.Length;
		}

		private AnimatorTransition AddEntryTransition()
		{
			this.undoHandler.DoUndo(this, "Entry Transition Added");
			AnimatorTransition[] entryTransitions = this.entryTransitions;
			AnimatorTransition animatorTransition = new AnimatorTransition();
			if (AssetDatabase.GetAssetPath(this) != "")
			{
				AssetDatabase.AddObjectToAsset(animatorTransition, AssetDatabase.GetAssetPath(this));
			}
			animatorTransition.hideFlags = HideFlags.HideInHierarchy;
			ArrayUtility.Add<AnimatorTransition>(ref entryTransitions, animatorTransition);
			this.entryTransitions = entryTransitions;
			return animatorTransition;
		}

		public AnimatorTransition AddEntryTransition(AnimatorState destinationState)
		{
			AnimatorTransition animatorTransition = this.AddEntryTransition();
			animatorTransition.destinationState = destinationState;
			return animatorTransition;
		}

		public AnimatorTransition AddEntryTransition(AnimatorStateMachine destinationStateMachine)
		{
			AnimatorTransition animatorTransition = this.AddEntryTransition();
			animatorTransition.destinationStateMachine = destinationStateMachine;
			return animatorTransition;
		}

		public bool RemoveEntryTransition(AnimatorTransition transition)
		{
			bool result;
			if (new List<AnimatorTransition>(this.entryTransitions).Any((AnimatorTransition t) => t == transition))
			{
				this.undoHandler.DoUndo(this, "Entry Transition Removed");
				AnimatorTransition[] entryTransitions = this.entryTransitions;
				ArrayUtility.Remove<AnimatorTransition>(ref entryTransitions, transition);
				this.entryTransitions = entryTransitions;
				if (MecanimUtilities.AreSameAsset(this, transition))
				{
					Undo.DestroyObjectImmediate(transition);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal ChildAnimatorState FindState(int nameHash)
		{
			return new List<ChildAnimatorState>(this.states).Find((ChildAnimatorState s) => s.state.nameHash == nameHash);
		}

		internal ChildAnimatorState FindState(string name)
		{
			return new List<ChildAnimatorState>(this.states).Find((ChildAnimatorState s) => s.state.name == name);
		}

		internal bool HasState(AnimatorState state)
		{
			return this.statesRecursive.Any((ChildAnimatorState s) => s.state == state);
		}

		internal bool IsDirectParent(AnimatorStateMachine stateMachine)
		{
			return this.stateMachines.Any((ChildAnimatorStateMachine sm) => sm.stateMachine == stateMachine);
		}

		internal bool HasStateMachine(AnimatorStateMachine child)
		{
			return this.stateMachinesRecursive.Any((ChildAnimatorStateMachine sm) => sm.stateMachine == child);
		}

		internal bool HasTransition(AnimatorState stateA, AnimatorState stateB)
		{
			return stateA.transitions.Any((AnimatorStateTransition t) => t.destinationState == stateB) || stateB.transitions.Any((AnimatorStateTransition t) => t.destinationState == stateA);
		}

		internal AnimatorStateMachine FindParent(AnimatorStateMachine stateMachine)
		{
			AnimatorStateMachine result;
			if (this.stateMachines.Any((ChildAnimatorStateMachine childSM) => childSM.stateMachine == stateMachine))
			{
				result = this;
			}
			else
			{
				result = this.stateMachinesRecursive.Find((ChildAnimatorStateMachine sm) => sm.stateMachine.stateMachines.Any((ChildAnimatorStateMachine childSM) => childSM.stateMachine == stateMachine)).stateMachine;
			}
			return result;
		}

		internal AnimatorStateMachine FindStateMachine(string path)
		{
			AnimatorStateMachine.<FindStateMachine>c__AnonStorey9 <FindStateMachine>c__AnonStorey = new AnimatorStateMachine.<FindStateMachine>c__AnonStorey9();
			<FindStateMachine>c__AnonStorey.smNames = path.Split(new char[]
			{
				'.'
			});
			AnimatorStateMachine animatorStateMachine = this;
			int i = 1;
			while (i < <FindStateMachine>c__AnonStorey.smNames.Length - 1 && animatorStateMachine != null)
			{
				int num = Array.FindIndex<ChildAnimatorStateMachine>(animatorStateMachine.stateMachines, (ChildAnimatorStateMachine t) => t.stateMachine.name == <FindStateMachine>c__AnonStorey.smNames[i]);
				animatorStateMachine = ((num < 0) ? null : animatorStateMachine.stateMachines[num].stateMachine);
				i++;
			}
			return (!(animatorStateMachine == null)) ? animatorStateMachine : this;
		}

		internal AnimatorStateMachine FindStateMachine(AnimatorState state)
		{
			AnimatorStateMachine result;
			if (this.HasState(state, false))
			{
				result = this;
			}
			else
			{
				List<ChildAnimatorStateMachine> stateMachinesRecursive = this.stateMachinesRecursive;
				int num = stateMachinesRecursive.FindIndex((ChildAnimatorStateMachine sm) => sm.stateMachine.HasState(state, false));
				result = ((num < 0) ? null : stateMachinesRecursive[num].stateMachine);
			}
			return result;
		}

		internal AnimatorStateTransition FindTransition(AnimatorState destinationState)
		{
			return new List<AnimatorStateTransition>(this.anyStateTransitions).Find((AnimatorStateTransition t) => t.destinationState == destinationState);
		}

		[Obsolete("GetTransitionsFromState is obsolete. Use AnimatorState.transitions instead.", true)]
		private AnimatorState GetTransitionsFromState(AnimatorState state)
		{
			return null;
		}
	}
}
