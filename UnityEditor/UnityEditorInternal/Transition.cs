using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class Transition : UnityEngine.Object
	{
		internal delegate void RemovedCondition(int index);
		private static readonly GUIContent s_TransitionTempContent = new GUIContent();
		internal Transition.RemovedCondition onRemovedCondition;
		public extern string uniqueName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int uniqueNameHash
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int conditionCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float duration
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float offset
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool atomic
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
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
		public extern bool canTransitionToSelf
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern State srcState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern State dstState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern string shortDisplayName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public GUIContent GetTransitionContentForRect(Rect rect)
		{
			Transition.s_TransitionTempContent.text = this.uniqueName;
			if (EditorStyles.label.CalcSize(Transition.s_TransitionTempContent).x > rect.width)
			{
				Transition.s_TransitionTempContent.text = this.shortDisplayName;
			}
			return Transition.s_TransitionTempContent;
		}
		public AnimatorCondition GetCondition(int index)
		{
			return (!this.ValidateConditionIndex(index)) ? null : new AnimatorCondition(this, index);
		}
		public AnimatorCondition AddCondition()
		{
			this.AddConditionInternal();
			return this.GetCondition(this.conditionCount - 1);
		}
		public void RemoveCondition(int index)
		{
			if (this.onRemovedCondition != null)
			{
				this.onRemovedCondition(index);
			}
			this.RemoveConditionInternal(index);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddConditionInternal();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveConditionInternal(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool ValidateConditionIndex(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern TransitionConditionMode GetConditionMode(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetConditionMode(int index, TransitionConditionMode mode);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetConditionParameter(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetConditionParameter(int index, string eventName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetParameterTreshold(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameterTreshold(int index, float threshold);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetExitTime(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetExitTime(int index, float exit);
	}
}
