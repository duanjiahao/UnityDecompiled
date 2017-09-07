using System;
using System.Runtime.CompilerServices;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	public static class AnimationPlayableGraphExtensions
	{
		internal static bool InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationOutput(ref graph, name, out handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle);

		internal static void SyncUpdateAndTimeMode(this PlayableGraph graph, Animator animator)
		{
			AnimationPlayableGraphExtensions.InternalSyncUpdateAndTimeMode(ref graph, animator);
		}

		internal static void InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, Animator animator)
		{
			AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalSyncUpdateAndTimeMode(ref graph, animator);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalSyncUpdateAndTimeMode(ref PlayableGraph graph, Animator animator);

		internal static PlayableHandle CreateAnimationMotionXToDeltaPlayable(this PlayableGraph graph)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!AnimationPlayableGraphExtensions.InternalCreateAnimationMotionXToDeltaPlayable(ref graph, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(1);
				result = @null;
			}
			return result;
		}

		private static bool InternalCreateAnimationMotionXToDeltaPlayable(ref PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalCreateAnimationMotionXToDeltaPlayable(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreateAnimationMotionXToDeltaPlayable(ref PlayableGraph graph, ref PlayableHandle handle);

		private static void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutputHandle handle)
		{
			AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalDestroyOutput(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutputHandle handle);

		internal static void DestroyOutput(this PlayableGraph graph, PlayableOutputHandle handle)
		{
			AnimationPlayableGraphExtensions.InternalDestroyOutput(ref graph, ref handle);
		}

		private static int InternalAnimationOutputCount(ref PlayableGraph graph)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalAnimationOutputCount(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_InternalAnimationOutputCount(ref PlayableGraph graph);

		private static bool InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutputHandle handle)
		{
			return AnimationPlayableGraphExtensions.INTERNAL_CALL_InternalGetAnimationOutput(ref graph, index, out handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalGetAnimationOutput(ref PlayableGraph graph, int index, out PlayableOutputHandle handle);
	}
}
