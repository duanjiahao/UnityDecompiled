using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	public static class AudioPlayableGraphExtensions
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int internalAudioOutputCount(ref PlayableGraph graph);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetAudioOutput(ref PlayableGraph graph, int index, out PlayableOutput output);
	}
}
