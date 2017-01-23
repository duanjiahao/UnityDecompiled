using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Experimental.Director
{
	internal sealed class Playables
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object CastToInternal(Type castType, IntPtr handle, int version);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Type GetTypeOfInternal(IntPtr handle, int version);

		internal static void InternalDestroy(ref Playable playable)
		{
			Playables.INTERNAL_CALL_InternalDestroy(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalDestroy(ref Playable playable);

		internal static bool ConnectInternal(ref Playable source, ref Playable target, int sourceOutputPort, int targetInputPort)
		{
			return Playables.INTERNAL_CALL_ConnectInternal(ref source, ref target, sourceOutputPort, targetInputPort);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ConnectInternal(ref Playable source, ref Playable target, int sourceOutputPort, int targetInputPort);

		internal static void DisconnectInternal(ref Playable target, int inputPort)
		{
			Playables.INTERNAL_CALL_DisconnectInternal(ref target, inputPort);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DisconnectInternal(ref Playable target, int inputPort);

		internal static void SetPlayableDeleteOnDisconnect(ref Playable target, bool value)
		{
			Playables.INTERNAL_CALL_SetPlayableDeleteOnDisconnect(ref target, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPlayableDeleteOnDisconnect(ref Playable target, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void BeginIgnoreAllocationTracker();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void EndIgnoreAllocationTracker();

		internal static bool CheckInputBounds(Playable playable, int inputIndex)
		{
			return playable.CheckInputBounds(inputIndex);
		}

		internal static bool IsValid(Playable playable)
		{
			return playable.IsValid();
		}

		internal static int GetInputCountValidated(Playable playable, Type typeofPlayable)
		{
			return playable.inputCount;
		}

		internal static int GetOutputCountValidated(Playable playable, Type typeofPlayable)
		{
			return playable.outputCount;
		}

		internal static PlayState GetPlayStateValidated(Playable playable, Type typeofPlayable)
		{
			return playable.state;
		}

		internal static void SetPlayStateValidated(Playable playable, PlayState playState, Type typeofPlayable)
		{
			playable.state = playState;
		}

		internal static double GetTimeValidated(Playable playable, Type typeofPlayable)
		{
			return playable.time;
		}

		internal static void SetTimeValidated(Playable playable, double time, Type typeofPlayable)
		{
			playable.time = time;
		}

		internal static double GetDurationValidated(Playable playable, Type typeofPlayable)
		{
			return playable.duration;
		}

		internal static void SetDurationValidated(Playable playable, double duration, Type typeofPlayable)
		{
			playable.duration = duration;
		}

		internal static Playable GetInputValidated(Playable playable, int inputPort, Type typeofPlayable)
		{
			return playable.GetInput(inputPort);
		}

		internal static Playable GetOutputValidated(Playable playable, int outputPort, Type typeofPlayable)
		{
			return playable.GetOutput(outputPort);
		}

		internal static void SetInputWeightValidated(Playable playable, int inputIndex, float weight, Type typeofPlayable)
		{
			playable.SetInputWeight(inputIndex, weight);
		}

		internal static void SetInputWeightValidated(Playable playable, Playable input, float weight, Type typeofPlayable)
		{
			playable.SetInputWeight(input, weight);
		}

		internal static float GetInputWeightValidated(Playable playable, int index, Type typeofPlayable)
		{
			return playable.GetInputWeight(index);
		}

		internal static bool Equals(Playable isAPlayable, object mightBeAnythingOrNull)
		{
			return mightBeAnythingOrNull != null && isAPlayable.Equals(mightBeAnythingOrNull);
		}

		internal static bool Equals(Playable lhs, Playable rhs)
		{
			return Playables.CompareVersion(lhs, rhs);
		}

		internal static bool CompareVersion(Playable lhs, Playable rhs)
		{
			return Playable.CompareVersion(lhs, rhs);
		}
	}
}
