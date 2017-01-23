using System;
using UnityEngine;

namespace UnityEditorInternal
{
	[Obsolete("StateMachine is obsolete. Use UnityEditor.Animations.AnimatorStateMachine instead (UnityUpgradable) -> UnityEditor.Animations.AnimatorStateMachine", true)]
	public class StateMachine : UnityEngine.Object
	{
		public State defaultState
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public Vector3 anyStatePosition
		{
			get
			{
				return default(Vector3);
			}
			set
			{
			}
		}

		public Vector3 parentStateMachinePosition
		{
			get
			{
				return default(Vector3);
			}
			set
			{
			}
		}

		public State GetState(int index)
		{
			return null;
		}

		public State AddState(string stateName)
		{
			return null;
		}

		public StateMachine GetStateMachine(int index)
		{
			return null;
		}

		public StateMachine AddStateMachine(string stateMachineName)
		{
			return null;
		}

		public Transition AddTransition(State src, State dst)
		{
			return null;
		}

		public Transition AddAnyStateTransition(State dst)
		{
			return null;
		}

		public Vector3 GetStateMachinePosition(int i)
		{
			return default(Vector3);
		}

		public Transition[] GetTransitionsFromState(State srcState)
		{
			return null;
		}
	}
}
