using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	public struct PlayableGraph
	{
		internal IntPtr m_Handle;

		internal int m_Version;

		public bool isDone
		{
			get
			{
				return PlayableGraph.InternalIsDone(ref this);
			}
		}

		public int playableCount
		{
			get
			{
				return PlayableGraph.InternalPlayableCount(ref this);
			}
		}

		public int scriptOutputCount
		{
			get
			{
				return PlayableGraph.internalScriptOutputCount(ref this);
			}
		}

		public int rootPlayableCount
		{
			get
			{
				return PlayableGraph.InternalRootPlayableCount(ref this);
			}
		}

		public bool IsValid()
		{
			return PlayableGraph.IsValidInternal(ref this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidInternal(ref PlayableGraph graph);

		public static PlayableGraph CreateGraph()
		{
			PlayableGraph result = default(PlayableGraph);
			PlayableGraph.InternalCreate(ref result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCreate(ref PlayableGraph graph);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalIsDone(ref PlayableGraph graph);

		public void Play()
		{
			PlayableGraph.InternalPlay(ref this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPlay(ref PlayableGraph graph);

		public void Stop()
		{
			PlayableGraph.InternalStop(ref this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalStop(ref PlayableGraph graph);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalPlayableCount(ref PlayableGraph graph);

		public ScriptPlayableOutput CreateScriptOutput(string name)
		{
			ScriptPlayableOutput scriptPlayableOutput = default(ScriptPlayableOutput);
			ScriptPlayableOutput result;
			if (!PlayableGraph.InternalCreateScriptOutput(ref this, name, out scriptPlayableOutput.m_Output))
			{
				result = ScriptPlayableOutput.Null;
			}
			else
			{
				result = scriptPlayableOutput;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateScriptOutput(ref PlayableGraph graph, string name, out PlayableOutput output);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int internalScriptOutputCount(ref PlayableGraph graph);

		public ScriptPlayableOutput GetScriptOutput(int index)
		{
			ScriptPlayableOutput scriptPlayableOutput = default(ScriptPlayableOutput);
			ScriptPlayableOutput result;
			if (!PlayableGraph.InternalGetScriptOutput(ref this, index, out scriptPlayableOutput.m_Output))
			{
				result = ScriptPlayableOutput.Null;
			}
			else
			{
				result = scriptPlayableOutput;
			}
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalGetScriptOutput(ref PlayableGraph graph, int index, out PlayableOutput output);

		public PlayableHandle CreatePlayable()
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!PlayableGraph.InternalCreatePlayable(ref this, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				result = @null;
			}
			return result;
		}

		[ExcludeFromDocs]
		public PlayableHandle CreateGenericMixerPlayable()
		{
			int inputCount = 0;
			return this.CreateGenericMixerPlayable(inputCount);
		}

		public PlayableHandle CreateGenericMixerPlayable([DefaultValue("0")] int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!PlayableGraph.InternalCreatePlayable(ref this, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				@null.inputCount = inputCount;
				result = @null;
			}
			return result;
		}

		public PlayableHandle CreateScriptPlayable<T>() where T : ScriptPlayable, new()
		{
			PlayableHandle @null = PlayableHandle.Null;
			ScriptPlayable scriptPlayable = PlayableGraph.InternalCreateScriptPlayable(ref this, ref @null, typeof(T)) as ScriptPlayable;
			PlayableHandle result;
			if (scriptPlayable == null)
			{
				result = PlayableHandle.Null;
			}
			else
			{
				scriptPlayable.handle = @null;
				result = @null;
			}
			return result;
		}

		private static bool InternalCreatePlayable(ref PlayableGraph graph, ref PlayableHandle handle)
		{
			return PlayableGraph.INTERNAL_CALL_InternalCreatePlayable(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_InternalCreatePlayable(ref PlayableGraph graph, ref PlayableHandle handle);

		private static object InternalCreateScriptPlayable(ref PlayableGraph graph, ref PlayableHandle handle, Type type)
		{
			return PlayableGraph.INTERNAL_CALL_InternalCreateScriptPlayable(ref graph, ref handle, type);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object INTERNAL_CALL_InternalCreateScriptPlayable(ref PlayableGraph graph, ref PlayableHandle handle, Type type);

		public void Destroy()
		{
			PlayableGraph.DestroyInternal(ref this);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyInternal(ref PlayableGraph graph);

		public bool Connect(PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort)
		{
			return PlayableGraph.ConnectInternal(ref this, source, sourceOutputPort, destination, destinationInputPort);
		}

		public bool Connect(Playable source, int sourceOutputPort, Playable destination, int destinationInputPort)
		{
			return PlayableGraph.ConnectInternal(ref this, source.handle, sourceOutputPort, destination.handle, destinationInputPort);
		}

		private static bool ConnectInternal(ref PlayableGraph graph, PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort)
		{
			return PlayableGraph.INTERNAL_CALL_ConnectInternal(ref graph, ref source, sourceOutputPort, ref destination, destinationInputPort);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ConnectInternal(ref PlayableGraph graph, ref PlayableHandle source, int sourceOutputPort, ref PlayableHandle destination, int destinationInputPort);

		public void Disconnect(Playable playable, int inputPort)
		{
			PlayableHandle handle = playable.handle;
			PlayableGraph.DisconnectInternal(ref this, ref handle, inputPort);
		}

		public void Disconnect(PlayableHandle playable, int inputPort)
		{
			PlayableGraph.DisconnectInternal(ref this, ref playable, inputPort);
		}

		private static void DisconnectInternal(ref PlayableGraph graph, ref PlayableHandle playable, int inputPort)
		{
			PlayableGraph.INTERNAL_CALL_DisconnectInternal(ref graph, ref playable, inputPort);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DisconnectInternal(ref PlayableGraph graph, ref PlayableHandle playable, int inputPort);

		public void DestroyPlayable(PlayableHandle playable)
		{
			PlayableGraph.InternalDestroyPlayable(ref this, ref playable);
		}

		private static void InternalDestroyPlayable(ref PlayableGraph graph, ref PlayableHandle playable)
		{
			PlayableGraph.INTERNAL_CALL_InternalDestroyPlayable(ref graph, ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalDestroyPlayable(ref PlayableGraph graph, ref PlayableHandle playable);

		public void DestroyOutput(ScriptPlayableOutput output)
		{
			PlayableGraph.InternalDestroyOutput(ref this, ref output.m_Output);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalDestroyOutput(ref PlayableGraph graph, ref PlayableOutput output);

		public void DestroySubgraph(PlayableHandle playable)
		{
			PlayableGraph.InternalDestroySubgraph(ref this, playable);
		}

		private static void InternalDestroySubgraph(ref PlayableGraph graph, PlayableHandle playable)
		{
			PlayableGraph.INTERNAL_CALL_InternalDestroySubgraph(ref graph, ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalDestroySubgraph(ref PlayableGraph graph, ref PlayableHandle playable);

		[ExcludeFromDocs]
		public void Evaluate()
		{
			float deltaTime = 0f;
			this.Evaluate(deltaTime);
		}

		public void Evaluate([DefaultValue("0")] float deltaTime)
		{
			PlayableGraph.InternalEvaluate(ref this, deltaTime);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalEvaluate(ref PlayableGraph graph, float deltaTime);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalRootPlayableCount(ref PlayableGraph graph);

		public PlayableHandle GetRootPlayable(int index)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableGraph.InternalGetRootPlayable(index, ref this, ref @null);
			return @null;
		}

		internal static void InternalGetRootPlayable(int index, ref PlayableGraph graph, ref PlayableHandle handle)
		{
			PlayableGraph.INTERNAL_CALL_InternalGetRootPlayable(index, ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_InternalGetRootPlayable(int index, ref PlayableGraph graph, ref PlayableHandle handle);
	}
}
