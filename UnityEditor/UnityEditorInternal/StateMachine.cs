using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class StateMachine : UnityEngine.Object
	{
		internal IEnumerable<State> states
		{
			get
			{
				State[] array = new State[this.stateCount];
				for (int i = 0; i < this.stateCount; i++)
				{
					array[i] = this.GetState(i);
				}
				return array;
			}
		}
		internal List<State> statesRecursive
		{
			get
			{
				List<State> list = new List<State>();
				list.AddRange(this.states);
				for (int i = 0; i < this.stateMachineCount; i++)
				{
					list.AddRange(this.GetStateMachine(i).statesRecursive);
				}
				return list;
			}
		}
		internal IEnumerable<StateMachine> stateMachines
		{
			get
			{
				StateMachine[] array = new StateMachine[this.stateMachineCount];
				for (int i = 0; i < this.stateMachineCount; i++)
				{
					array[i] = this.GetStateMachine(i);
				}
				return array;
			}
		}
		internal List<StateMachine> stateMachinesRecursive
		{
			get
			{
				List<StateMachine> list = new List<StateMachine>();
				list.AddRange(this.stateMachines);
				for (int i = 0; i < this.stateMachineCount; i++)
				{
					list.AddRange(this.GetStateMachine(i).stateMachinesRecursive);
				}
				return list;
			}
		}
		internal List<Transition> transitions
		{
			get
			{
				List<State> statesRecursive = this.statesRecursive;
				List<Transition> list = new List<Transition>();
				foreach (State current in statesRecursive)
				{
					list.AddRange(this.GetTransitionsFromState(current));
				}
				list.AddRange(this.GetTransitionsFromState(null));
				return list;
			}
		}
		public extern int stateCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int stateMachineCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern State defaultState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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
		public extern int motionSetCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern int transitionCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal Vector3 GetStateMachinePosition(StateMachine stateMachine)
		{
			for (int i = 0; i < this.stateMachineCount; i++)
			{
				if (stateMachine == this.GetStateMachine(i))
				{
					return this.GetStateMachinePosition(i);
				}
			}
			Assert.Fail(string.Concat(new string[]
			{
				"Can't find state machine (",
				stateMachine.name,
				") in parent state machine (",
				base.name,
				")."
			}));
			return Vector3.zero;
		}
		internal void SetStateMachinePosition(StateMachine stateMachine, Vector3 position)
		{
			for (int i = 0; i < this.stateMachineCount; i++)
			{
				if (stateMachine == this.GetStateMachine(i))
				{
					this.SetStateMachinePosition(i, position);
					return;
				}
			}
			Assert.Fail(string.Concat(new string[]
			{
				"Can't find state machine (",
				stateMachine.name,
				") in parent state machine (",
				base.name,
				")."
			}));
		}
		internal State CreateState(Vector3 position)
		{
			Undo.RegisterCompleteObjectUndo(this, "State added");
			State state = this.AddState("New State");
			state.position = position;
			return state;
		}
		internal State FindState(string stateUniqueName)
		{
			return this.statesRecursive.Find((State s) => s.uniqueName == stateUniqueName);
		}
		internal State FindState(int stateUniqueNameHash)
		{
			return this.statesRecursive.Find((State s) => s.uniqueNameHash == stateUniqueNameHash);
		}
		internal Transition FindTransition(string transitionUniqueName)
		{
			return this.transitions.Find((Transition t) => t.uniqueName == transitionUniqueName);
		}
		internal Transition FindTransition(int transitionUniqueName)
		{
			return this.transitions.Find((Transition t) => t.uniqueNameHash == transitionUniqueName);
		}
		internal bool HasState(State state)
		{
			return this.statesRecursive.Any((State s) => s == state);
		}
		internal bool IsDirectParent(StateMachine stateMachine)
		{
			return this.stateMachines.Any((StateMachine sm) => sm == stateMachine);
		}
		internal bool HasStateMachine(StateMachine child)
		{
			return this.stateMachinesRecursive.Any((StateMachine sm) => sm == child);
		}
		internal bool HasTransition(State stateA, State stateB)
		{
			return this.GetTransitionsFromState(stateA).Any((Transition t) => t.dstState == stateB) || this.GetTransitionsFromState(stateB).Any((Transition t) => t.dstState == stateA);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern State GetState(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern State AddState(string stateName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveState(State state);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern StateMachine GetStateMachine(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern StateMachine AddStateMachine(string stateMachineName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveStateMachine(StateMachine stateMachine);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Transition AddTransition(State src, State dst);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Transition AddAnyStateTransition(State dst);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveTransition(Transition transition);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_anyStatePosition(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_anyStatePosition(ref Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_parentStateMachinePosition(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_parentStateMachinePosition(ref Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Vector3 GetStateMachinePosition(int i);
		public void SetStateMachinePosition(int i, Vector3 pos)
		{
			StateMachine.INTERNAL_CALL_SetStateMachinePosition(this, i, ref pos);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetStateMachinePosition(StateMachine self, int i, ref Vector3 pos);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Transition[] GetTransitionsFromState(State srcState);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MoveState(State state, StateMachine target);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MoveStateMachine(StateMachine stateMachine, StateMachine target);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasState(State state, bool recursive);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasStateMachine(StateMachine state, bool recursive);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTransitionsFromState(State srcState, Transition[] transitions);
	}
}
