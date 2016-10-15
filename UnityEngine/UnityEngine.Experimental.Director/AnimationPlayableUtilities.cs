using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.Director
{
	internal class AnimationPlayableUtilities
	{
		public static AnimationPlayable Null
		{
			get
			{
				AnimationPlayable result = default(AnimationPlayable);
				result.handle.m_Version = 10;
				return result;
			}
		}

		internal static int AddInputValidated(AnimationPlayable target, Playable input, Type typeofTarget)
		{
			return target.AddInput(input);
		}

		internal static bool SetInputValidated(AnimationPlayable target, Playable source, int index, Type typeofTarget)
		{
			return target.SetInput(source, index);
		}

		internal static bool SetInputsValidated(AnimationPlayable target, IEnumerable<Playable> sources, Type typeofTarget)
		{
			return target.SetInputs(sources);
		}

		internal static bool RemoveInputValidated(AnimationPlayable target, int index, Type typeofTarget)
		{
			return target.RemoveInput(index);
		}

		internal static bool RemoveInputValidated(AnimationPlayable target, Playable playable, Type typeofTarget)
		{
			return target.RemoveInput(playable);
		}

		internal static bool RemoveAllInputsValidated(AnimationPlayable target, Type typeofTarget)
		{
			return target.RemoveAllInputs();
		}

		internal static bool SetInputs(AnimationMixerPlayable playable, AnimationClip[] clips)
		{
			if (clips == null)
			{
				throw new NullReferenceException("Parameter clips was null. You need to pass in a valid array of clips.");
			}
			Playables.BeginIgnoreAllocationTracker();
			Playable[] array = new Playable[clips.Length];
			for (int i = 0; i < clips.Length; i++)
			{
				array[i] = AnimationClipPlayable.Create(clips[i]);
				Playable playable2 = array[i];
				Playables.SetPlayableDeleteOnDisconnect(ref playable2, true);
			}
			Playables.EndIgnoreAllocationTracker();
			return AnimationPlayableUtilities.SetInputsValidated(playable, array, typeof(AnimationMixerPlayable));
		}
	}
}
