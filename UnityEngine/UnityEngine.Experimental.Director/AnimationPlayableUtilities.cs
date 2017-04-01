using System;

namespace UnityEngine.Experimental.Director
{
	public class AnimationPlayableUtilities
	{
		public static void Play(Animator animator, PlayableHandle playable, PlayableGraph graph)
		{
			graph.CreateAnimationOutput("AnimationClip", animator).sourcePlayable = playable;
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
		}

		public static PlayableHandle PlayClip(Animator animator, AnimationClip clip, out PlayableGraph graph)
		{
			graph = PlayableGraph.CreateGraph();
			AnimationPlayableOutput animationPlayableOutput = graph.CreateAnimationOutput("AnimationClip", animator);
			PlayableHandle playableHandle = graph.CreateAnimationClipPlayable(clip);
			animationPlayableOutput.sourcePlayable = playableHandle;
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return playableHandle;
		}

		public static PlayableHandle PlayMixer(Animator animator, int inputCount, out PlayableGraph graph)
		{
			graph = PlayableGraph.CreateGraph();
			AnimationPlayableOutput animationPlayableOutput = graph.CreateAnimationOutput("Mixer", animator);
			PlayableHandle playableHandle = graph.CreateAnimationMixerPlayable(inputCount);
			animationPlayableOutput.sourcePlayable = playableHandle;
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return playableHandle;
		}

		public static PlayableHandle PlayAnimatorController(Animator animator, RuntimeAnimatorController controller, out PlayableGraph graph)
		{
			graph = PlayableGraph.CreateGraph();
			AnimationPlayableOutput animationPlayableOutput = graph.CreateAnimationOutput("AnimatorControllerPlayable", animator);
			PlayableHandle playableHandle = graph.CreateAnimatorControllerPlayable(controller);
			animationPlayableOutput.sourcePlayable = playableHandle;
			graph.SyncUpdateAndTimeMode(animator);
			graph.Play();
			return playableHandle;
		}
	}
}
