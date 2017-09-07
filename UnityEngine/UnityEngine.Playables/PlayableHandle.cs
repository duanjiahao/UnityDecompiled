using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[UsedByNativeCode]
	public struct PlayableHandle
	{
		internal IntPtr m_Handle;

		internal int m_Version;

		public static PlayableHandle Null
		{
			get
			{
				return new PlayableHandle
				{
					m_Version = 10
				};
			}
		}

		internal T GetObject<T>() where T : class, IPlayableBehaviour
		{
			T result;
			if (!this.IsValid())
			{
				result = (T)((object)null);
			}
			else
			{
				object scriptInstance = PlayableHandle.GetScriptInstance(ref this);
				if (scriptInstance == null)
				{
					result = (T)((object)null);
				}
				else
				{
					result = (T)((object)scriptInstance);
				}
			}
			return result;
		}

		private static object GetScriptInstance(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetScriptInstance(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object INTERNAL_CALL_GetScriptInstance(ref PlayableHandle playable);

		internal void SetScriptInstance(object scriptInstance)
		{
			PlayableHandle.SetScriptInstance(ref this, scriptInstance);
		}

		private static void SetScriptInstance(ref PlayableHandle playable, object scriptInstance)
		{
			PlayableHandle.INTERNAL_CALL_SetScriptInstance(ref playable, scriptInstance);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetScriptInstance(ref PlayableHandle playable, object scriptInstance);

		internal bool IsValid()
		{
			return PlayableHandle.IsValidInternal(ref this);
		}

		private static bool IsValidInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_IsValidInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsValidInternal(ref PlayableHandle playable);

		private static Type GetPlayableTypeOf(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetPlayableTypeOf(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type INTERNAL_CALL_GetPlayableTypeOf(ref PlayableHandle playable);

		internal Type GetPlayableType()
		{
			return PlayableHandle.GetPlayableTypeOf(ref this);
		}

		internal bool IsPlayableOfType<T>()
		{
			return PlayableHandle.GetPlayableTypeOf(ref this) == typeof(T);
		}

		internal PlayableGraph GetGraph()
		{
			PlayableGraph result = default(PlayableGraph);
			PlayableHandle.GetGraphInternal(ref this, ref result);
			return result;
		}

		internal int GetInputCount()
		{
			return PlayableHandle.GetInputCountInternal(ref this);
		}

		internal void SetInputCount(int value)
		{
			PlayableHandle.SetInputCountInternal(ref this, value);
		}

		internal int GetOutputCount()
		{
			return PlayableHandle.GetOutputCountInternal(ref this);
		}

		internal void SetOutputCount(int value)
		{
			PlayableHandle.SetOutputCountInternal(ref this, value);
		}

		internal PlayState GetPlayState()
		{
			return PlayableHandle.GetPlayStateInternal(ref this);
		}

		internal void SetPlayState(PlayState value)
		{
			PlayableHandle.SetPlayStateInternal(ref this, value);
		}

		internal double GetSpeed()
		{
			return PlayableHandle.GetSpeedInternal(ref this);
		}

		internal void SetSpeed(double value)
		{
			PlayableHandle.SetSpeedInternal(ref this, value);
		}

		internal double GetTime()
		{
			return PlayableHandle.GetTimeInternal(ref this);
		}

		internal void SetTime(double value)
		{
			PlayableHandle.SetTimeInternal(ref this, value);
		}

		internal bool IsDone()
		{
			return PlayableHandle.IsDoneInternal(ref this);
		}

		internal void SetDone(bool value)
		{
			PlayableHandle.SetDoneInternal(ref this, value);
		}

		internal bool GetPropagateSetTime()
		{
			return PlayableHandle.GetPropagateSetTimeInternal(ref this);
		}

		internal void SetPropagateSetTime(bool value)
		{
			PlayableHandle.SetPropagateSetTimeInternal(ref this, value);
		}

		internal bool CanChangeInputs()
		{
			return PlayableHandle.CanChangeInputsInternal(ref this);
		}

		internal bool CanSetWeights()
		{
			return PlayableHandle.CanSetWeightsInternal(ref this);
		}

		internal bool CanDestroy()
		{
			return PlayableHandle.CanDestroyInternal(ref this);
		}

		private static bool CanChangeInputsInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_CanChangeInputsInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CanChangeInputsInternal(ref PlayableHandle playable);

		private static bool CanSetWeightsInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_CanSetWeightsInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CanSetWeightsInternal(ref PlayableHandle playable);

		private static bool CanDestroyInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_CanDestroyInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CanDestroyInternal(ref PlayableHandle playable);

		private static PlayState GetPlayStateInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetPlayStateInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PlayState INTERNAL_CALL_GetPlayStateInternal(ref PlayableHandle playable);

		private static void SetPlayStateInternal(ref PlayableHandle playable, PlayState playState)
		{
			PlayableHandle.INTERNAL_CALL_SetPlayStateInternal(ref playable, playState);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPlayStateInternal(ref PlayableHandle playable, PlayState playState);

		private static double GetSpeedInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetSpeedInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetSpeedInternal(ref PlayableHandle playable);

		private static void SetSpeedInternal(ref PlayableHandle playable, double speed)
		{
			PlayableHandle.INTERNAL_CALL_SetSpeedInternal(ref playable, speed);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetSpeedInternal(ref PlayableHandle playable, double speed);

		private static double GetTimeInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetTimeInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetTimeInternal(ref PlayableHandle playable);

		private static void SetTimeInternal(ref PlayableHandle playable, double time)
		{
			PlayableHandle.INTERNAL_CALL_SetTimeInternal(ref playable, time);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTimeInternal(ref PlayableHandle playable, double time);

		private static bool IsDoneInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_IsDoneInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsDoneInternal(ref PlayableHandle playable);

		private static void SetDoneInternal(ref PlayableHandle playable, bool isDone)
		{
			PlayableHandle.INTERNAL_CALL_SetDoneInternal(ref playable, isDone);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetDoneInternal(ref PlayableHandle playable, bool isDone);

		internal double GetDuration()
		{
			return PlayableHandle.GetDurationInternal(ref this);
		}

		internal void SetDuration(double value)
		{
			PlayableHandle.SetDurationInternal(ref this, value);
		}

		private static double GetDurationInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetDurationInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetDurationInternal(ref PlayableHandle playable);

		private static void SetDurationInternal(ref PlayableHandle playable, double duration)
		{
			PlayableHandle.INTERNAL_CALL_SetDurationInternal(ref playable, duration);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetDurationInternal(ref PlayableHandle playable, double duration);

		private static bool GetPropagateSetTimeInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetPropagateSetTimeInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetPropagateSetTimeInternal(ref PlayableHandle playable);

		private static void SetPropagateSetTimeInternal(ref PlayableHandle playable, bool value)
		{
			PlayableHandle.INTERNAL_CALL_SetPropagateSetTimeInternal(ref playable, value);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPropagateSetTimeInternal(ref PlayableHandle playable, bool value);

		private static void GetGraphInternal(ref PlayableHandle playable, ref PlayableGraph graph)
		{
			PlayableHandle.INTERNAL_CALL_GetGraphInternal(ref playable, ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetGraphInternal(ref PlayableHandle playable, ref PlayableGraph graph);

		private static int GetInputCountInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetInputCountInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetInputCountInternal(ref PlayableHandle playable);

		private static void SetInputCountInternal(ref PlayableHandle playable, int count)
		{
			PlayableHandle.INTERNAL_CALL_SetInputCountInternal(ref playable, count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInputCountInternal(ref PlayableHandle playable, int count);

		private static int GetOutputCountInternal(ref PlayableHandle playable)
		{
			return PlayableHandle.INTERNAL_CALL_GetOutputCountInternal(ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetOutputCountInternal(ref PlayableHandle playable);

		private static void SetOutputCountInternal(ref PlayableHandle playable, int count)
		{
			PlayableHandle.INTERNAL_CALL_SetOutputCountInternal(ref playable, count);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetOutputCountInternal(ref PlayableHandle playable, int count);

		internal Playable GetInput(int inputPort)
		{
			return new Playable(PlayableHandle.GetInputInternal(ref this, inputPort));
		}

		private static PlayableHandle GetInputInternal(ref PlayableHandle playable, int index)
		{
			PlayableHandle result;
			PlayableHandle.INTERNAL_CALL_GetInputInternal(ref playable, index, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetInputInternal(ref PlayableHandle playable, int index, out PlayableHandle value);

		internal Playable GetOutput(int outputPort)
		{
			return new Playable(PlayableHandle.GetOutputInternal(ref this, outputPort));
		}

		private static PlayableHandle GetOutputInternal(ref PlayableHandle playable, int index)
		{
			PlayableHandle result;
			PlayableHandle.INTERNAL_CALL_GetOutputInternal(ref playable, index, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetOutputInternal(ref PlayableHandle playable, int index, out PlayableHandle value);

		private static void SetInputWeightFromIndexInternal(ref PlayableHandle playable, int index, float weight)
		{
			PlayableHandle.INTERNAL_CALL_SetInputWeightFromIndexInternal(ref playable, index, weight);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInputWeightFromIndexInternal(ref PlayableHandle playable, int index, float weight);

		internal bool SetInputWeight(int inputIndex, float weight)
		{
			bool result;
			if (this.CheckInputBounds(inputIndex))
			{
				PlayableHandle.SetInputWeightFromIndexInternal(ref this, inputIndex, weight);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static float GetInputWeightFromIndexInternal(ref PlayableHandle playable, int index)
		{
			return PlayableHandle.INTERNAL_CALL_GetInputWeightFromIndexInternal(ref playable, index);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetInputWeightFromIndexInternal(ref PlayableHandle playable, int index);

		internal void SetInputWeight(PlayableHandle input, float weight)
		{
			PlayableHandle.SetInputWeightInternal(ref this, ref input, weight);
		}

		private static void SetInputWeightInternal(ref PlayableHandle playable, ref PlayableHandle input, float weight)
		{
			PlayableHandle.INTERNAL_CALL_SetInputWeightInternal(ref playable, ref input, weight);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInputWeightInternal(ref PlayableHandle playable, ref PlayableHandle input, float weight);

		internal float GetInputWeight(int inputIndex)
		{
			float result;
			if (this.CheckInputBounds(inputIndex))
			{
				result = PlayableHandle.GetInputWeightFromIndexInternal(ref this, inputIndex);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		internal void Destroy()
		{
			this.GetGraph().DestroyPlayable<Playable>(new Playable(this));
		}

		public static bool operator ==(PlayableHandle x, PlayableHandle y)
		{
			return PlayableHandle.CompareVersion(x, y);
		}

		public static bool operator !=(PlayableHandle x, PlayableHandle y)
		{
			return !PlayableHandle.CompareVersion(x, y);
		}

		public override bool Equals(object p)
		{
			return p is PlayableHandle && PlayableHandle.CompareVersion(this, (PlayableHandle)p);
		}

		public override int GetHashCode()
		{
			return this.m_Handle.GetHashCode() ^ this.m_Version.GetHashCode();
		}

		internal static bool CompareVersion(PlayableHandle lhs, PlayableHandle rhs)
		{
			return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
		}

		internal bool CheckInputBounds(int inputIndex)
		{
			return this.CheckInputBounds(inputIndex, false);
		}

		internal bool CheckInputBounds(int inputIndex, bool acceptAny)
		{
			bool result;
			if (inputIndex == -1 && acceptAny)
			{
				result = true;
			}
			else
			{
				if (inputIndex < 0)
				{
					throw new IndexOutOfRangeException("Index must be greater than 0");
				}
				if (this.GetInputCount() <= inputIndex)
				{
					throw new IndexOutOfRangeException(string.Concat(new object[]
					{
						"inputIndex ",
						inputIndex,
						" is greater than the number of available inputs (",
						this.GetInputCount(),
						")."
					}));
				}
				result = true;
			}
			return result;
		}
	}
}
