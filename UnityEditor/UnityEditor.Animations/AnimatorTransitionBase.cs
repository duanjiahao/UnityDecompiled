using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.Animations
{
	public class AnimatorTransitionBase : UnityEngine.Object
	{
		private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);

		public extern bool solo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool mute
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isExit
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorStateMachine destinationStateMachine
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorState destinationState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorCondition[] conditions
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
				this.undoHandler.pushUndo = value;
			}
		}

		public string GetDisplayName(UnityEngine.Object source)
		{
			return (!(source is AnimatorState)) ? this.GetDisplayNameStateMachineSource(source as AnimatorStateMachine) : this.GetDisplayNameStateSource(source as AnimatorState);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetDisplayNameStateSource(AnimatorState source);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetDisplayNameStateMachineSource(AnimatorStateMachine source);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string BuildTransitionName(string source, string destination);

		public void AddCondition(AnimatorConditionMode mode, float threshold, string parameter)
		{
			this.undoHandler.DoUndo(this, "Condition added");
			AnimatorCondition[] conditions = this.conditions;
			ArrayUtility.Add<AnimatorCondition>(ref conditions, new AnimatorCondition
			{
				mode = mode,
				parameter = parameter,
				threshold = threshold
			});
			this.conditions = conditions;
		}

		public void RemoveCondition(AnimatorCondition condition)
		{
			this.undoHandler.DoUndo(this, "Condition removed");
			AnimatorCondition[] conditions = this.conditions;
			ArrayUtility.Remove<AnimatorCondition>(ref conditions, condition);
			this.conditions = conditions;
		}
	}
}
