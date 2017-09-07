using System;
using UnityEngine.Animations;

namespace UnityEngine.Playables
{
	public static class AnimationPlayableUtilities
	{
		public static void Play(Animator animator, Playable playable, PlayableGraph graph)
		{
			AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "AnimationClip", animator);
			output.SetSourcePlayable(playable);
			output.SetSourceInputPort(0);
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
		}

		public static AnimationClipPlayable PlayClip(Animator animator, AnimationClip clip, out PlayableGraph graph)
		{
			graph = PlayableGraph.Create();
			AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "AnimationClip", animator);
			AnimationClipPlayable animationClipPlayable = AnimationClipPlayable.Create(graph, clip);
			output.SetSourcePlayable(animationClipPlayable);
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return animationClipPlayable;
		}

		public static AnimationMixerPlayable PlayMixer(Animator animator, int inputCount, out PlayableGraph graph)
		{
			graph = PlayableGraph.Create();
			AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Mixer", animator);
			AnimationMixerPlayable animationMixerPlayable = AnimationMixerPlayable.Create(graph, inputCount, false);
			output.SetSourcePlayable(animationMixerPlayable);
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return animationMixerPlayable;
		}

		public static AnimationLayerMixerPlayable PlayLayerMixer(Animator animator, int inputCount, out PlayableGraph graph)
		{
			graph = PlayableGraph.Create();
			AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Mixer", animator);
			AnimationLayerMixerPlayable animationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(graph, inputCount);
			output.SetSourcePlayable(animationLayerMixerPlayable);
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return animationLayerMixerPlayable;
		}

		public static AnimatorControllerPlayable PlayAnimatorController(Animator animator, RuntimeAnimatorController controller, out PlayableGraph graph)
		{
			graph = PlayableGraph.Create();
			AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "AnimatorControllerPlayable", animator);
			AnimatorControllerPlayable animatorControllerPlayable = AnimatorControllerPlayable.Create(graph, controller);
			output.SetSourcePlayable(animatorControllerPlayable);
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return animatorControllerPlayable;
		}
	}
}
