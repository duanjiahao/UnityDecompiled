using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[UsedByNativeCode]
	public struct PlayableGraph
	{
		internal IntPtr m_Handle;

		internal int m_Version;

		public bool IsValid()
		{
			return PlayableGraph.IsValidInternal(ref this);
		}

		private static bool IsValidInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_IsValidInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsValidInternal(ref PlayableGraph graph);

		public static PlayableGraph Create()
		{
			PlayableGraph result = default(PlayableGraph);
			PlayableGraph.CreateInternal(ref result);
			return result;
		}

		internal static void CreateInternal(ref PlayableGraph graph)
		{
			PlayableGraph.INTERNAL_CALL_CreateInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateInternal(ref PlayableGraph graph);

		public bool IsDone()
		{
			return PlayableGraph.IsDoneInternal(ref this);
		}

		internal static bool IsDoneInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_IsDoneInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsDoneInternal(ref PlayableGraph graph);

		public bool IsPlaying()
		{
			return PlayableGraph.IsPlayingInternal(ref this);
		}

		internal static bool IsPlayingInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_IsPlayingInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsPlayingInternal(ref PlayableGraph graph);

		public IExposedPropertyTable GetResolver()
		{
			return PlayableGraph.GetResolverInternal(ref this);
		}

		public void SetResolver(IExposedPropertyTable value)
		{
			PlayableGraph.SetResolverInternal(ref this, value);
		}

		internal static IExposedPropertyTable GetResolverInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_GetResolverInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IExposedPropertyTable INTERNAL_CALL_GetResolverInternal(ref PlayableGraph graph);

		internal static void SetResolverInternal(ref PlayableGraph graph, IExposedPropertyTable resolver)
		{
			PlayableGraph.INTERNAL_CALL_SetResolverInternal(ref graph, resolver);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetResolverInternal(ref PlayableGraph graph, IExposedPropertyTable resolver);

		public void Play()
		{
			PlayableGraph.PlayInternal(ref this);
		}

		internal static void PlayInternal(ref PlayableGraph graph)
		{
			PlayableGraph.INTERNAL_CALL_PlayInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_PlayInternal(ref PlayableGraph graph);

		public void Stop()
		{
			PlayableGraph.StopInternal(ref this);
		}

		internal static void StopInternal(ref PlayableGraph graph)
		{
			PlayableGraph.INTERNAL_CALL_StopInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_StopInternal(ref PlayableGraph graph);

		public int GetPlayableCount()
		{
			return PlayableGraph.GetPlayableCountInternal(ref this);
		}

		internal static int GetPlayableCountInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_GetPlayableCountInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetPlayableCountInternal(ref PlayableGraph graph);

		public DirectorUpdateMode GetTimeUpdateMode()
		{
			return PlayableGraph.GetUpdateModeInternal(ref this);
		}

		public void SetTimeUpdateMode(DirectorUpdateMode value)
		{
			PlayableGraph.SetUpdateModeInternal(ref this, value);
		}

		private static DirectorUpdateMode GetUpdateModeInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_GetUpdateModeInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DirectorUpdateMode INTERNAL_CALL_GetUpdateModeInternal(ref PlayableGraph graph);

		private static void SetUpdateModeInternal(ref PlayableGraph graph, DirectorUpdateMode mode)
		{
			PlayableGraph.INTERNAL_CALL_SetUpdateModeInternal(ref graph, mode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetUpdateModeInternal(ref PlayableGraph graph, DirectorUpdateMode mode);

		internal static bool CreateScriptOutputInternal(ref PlayableGraph graph, string name, out PlayableOutputHandle handle)
		{
			return PlayableGraph.INTERNAL_CALL_CreateScriptOutputInternal(ref graph, name, out handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreateScriptOutputInternal(ref PlayableGraph graph, string name, out PlayableOutputHandle handle);

		internal PlayableHandle CreatePlayableHandle()
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableHandle result;
			if (!PlayableGraph.CreatePlayableHandleInternal(ref this, ref @null))
			{
				result = PlayableHandle.Null;
			}
			else
			{
				result = @null;
			}
			return result;
		}

		private static bool CreatePlayableHandleInternal(ref PlayableGraph graph, ref PlayableHandle handle)
		{
			return PlayableGraph.INTERNAL_CALL_CreatePlayableHandleInternal(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CreatePlayableHandleInternal(ref PlayableGraph graph, ref PlayableHandle handle);

		public void Destroy()
		{
			PlayableGraph.DestroyInternal(ref this);
		}

		private static void DestroyInternal(ref PlayableGraph graph)
		{
			PlayableGraph.INTERNAL_CALL_DestroyInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DestroyInternal(ref PlayableGraph graph);

		private static bool ConnectInternal(ref PlayableGraph graph, PlayableHandle source, int sourceOutputPort, PlayableHandle destination, int destinationInputPort)
		{
			return PlayableGraph.INTERNAL_CALL_ConnectInternal(ref graph, ref source, sourceOutputPort, ref destination, destinationInputPort);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_ConnectInternal(ref PlayableGraph graph, ref PlayableHandle source, int sourceOutputPort, ref PlayableHandle destination, int destinationInputPort);

		private static void DisconnectInternal(ref PlayableGraph graph, PlayableHandle playable, int inputPort)
		{
			PlayableGraph.INTERNAL_CALL_DisconnectInternal(ref graph, ref playable, inputPort);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DisconnectInternal(ref PlayableGraph graph, ref PlayableHandle playable, int inputPort);

		private static void DestroyPlayableInternal(ref PlayableGraph graph, PlayableHandle playable)
		{
			PlayableGraph.INTERNAL_CALL_DestroyPlayableInternal(ref graph, ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DestroyPlayableInternal(ref PlayableGraph graph, ref PlayableHandle playable);

		internal static void DestroyOutputInternal(ref PlayableGraph graph, PlayableOutputHandle handle)
		{
			PlayableGraph.INTERNAL_CALL_DestroyOutputInternal(ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DestroyOutputInternal(ref PlayableGraph graph, ref PlayableOutputHandle handle);

		private static void DestroySubgraphInternal(ref PlayableGraph graph, PlayableHandle playable)
		{
			PlayableGraph.INTERNAL_CALL_DestroySubgraphInternal(ref graph, ref playable);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DestroySubgraphInternal(ref PlayableGraph graph, ref PlayableHandle playable);

		[ExcludeFromDocs]
		public void Evaluate()
		{
			float deltaTime = 0f;
			this.Evaluate(deltaTime);
		}

		public void Evaluate([DefaultValue("0")] float deltaTime)
		{
			PlayableGraph.EvaluateInternal(ref this, deltaTime);
		}

		internal static void EvaluateInternal(ref PlayableGraph graph, float deltaTime)
		{
			PlayableGraph.INTERNAL_CALL_EvaluateInternal(ref graph, deltaTime);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_EvaluateInternal(ref PlayableGraph graph, float deltaTime);

		public int GetRootPlayableCount()
		{
			return PlayableGraph.GetRootPlayableCountInternal(ref this);
		}

		internal static int GetRootPlayableCountInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_GetRootPlayableCountInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetRootPlayableCountInternal(ref PlayableGraph graph);

		public Playable GetRootPlayable(int index)
		{
			PlayableHandle @null = PlayableHandle.Null;
			PlayableGraph.GetRootPlayableInternal(index, ref this, ref @null);
			return new Playable(@null);
		}

		internal static void GetRootPlayableInternal(int index, ref PlayableGraph graph, ref PlayableHandle handle)
		{
			PlayableGraph.INTERNAL_CALL_GetRootPlayableInternal(index, ref graph, ref handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetRootPlayableInternal(int index, ref PlayableGraph graph, ref PlayableHandle handle);

		private static int GetOutputCountInternal(ref PlayableGraph graph)
		{
			return PlayableGraph.INTERNAL_CALL_GetOutputCountInternal(ref graph);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetOutputCountInternal(ref PlayableGraph graph);

		private static bool GetOutputInternal(ref PlayableGraph graph, int index, out PlayableOutputHandle handle)
		{
			return PlayableGraph.INTERNAL_CALL_GetOutputInternal(ref graph, index, out handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetOutputInternal(ref PlayableGraph graph, int index, out PlayableOutputHandle handle);

		private static int GetOutputCountByTypeInternal(ref PlayableGraph graph, Type outputType)
		{
			return PlayableGraph.INTERNAL_CALL_GetOutputCountByTypeInternal(ref graph, outputType);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetOutputCountByTypeInternal(ref PlayableGraph graph, Type outputType);

		private static bool GetOutputByTypeInternal(ref PlayableGraph graph, Type outputType, int index, out PlayableOutputHandle handle)
		{
			return PlayableGraph.INTERNAL_CALL_GetOutputByTypeInternal(ref graph, outputType, index, out handle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetOutputByTypeInternal(ref PlayableGraph graph, Type outputType, int index, out PlayableOutputHandle handle);

		public bool Connect<U, V>(U source, int sourceOutputPort, V destination, int destinationInputPort) where U : struct, IPlayable where V : struct, IPlayable
		{
			return PlayableGraph.ConnectInternal(ref this, source.GetHandle(), sourceOutputPort, destination.GetHandle(), destinationInputPort);
		}

		public void Disconnect<U>(U input, int inputPort) where U : struct, IPlayable
		{
			PlayableGraph.DisconnectInternal(ref this, input.GetHandle(), inputPort);
		}

		public void DestroyPlayable<U>(U playable) where U : struct, IPlayable
		{
			PlayableGraph.DestroyPlayableInternal(ref this, playable.GetHandle());
		}

		public void DestroySubgraph<U>(U playable) where U : struct, IPlayable
		{
			PlayableGraph.DestroySubgraphInternal(ref this, playable.GetHandle());
		}

		public void DestroyOutput<U>(U output) where U : struct, IPlayableOutput
		{
			PlayableGraph.DestroyOutputInternal(ref this, output.GetHandle());
		}

		public int GetOutputCount()
		{
			return PlayableGraph.GetOutputCountInternal(ref this);
		}

		public int GetOutputCountByType<T>() where T : struct, IPlayableOutput
		{
			return PlayableGraph.GetOutputCountByTypeInternal(ref this, typeof(T));
		}

		public PlayableOutput GetOutput(int index)
		{
			PlayableOutputHandle handle;
			PlayableOutput result;
			if (!PlayableGraph.GetOutputInternal(ref this, index, out handle))
			{
				result = PlayableOutput.Null;
			}
			else
			{
				result = new PlayableOutput(handle);
			}
			return result;
		}

		public PlayableOutput GetOutputByType<T>(int index) where T : struct, IPlayableOutput
		{
			PlayableOutputHandle handle;
			PlayableOutput result;
			if (!PlayableGraph.GetOutputByTypeInternal(ref this, typeof(T), index, out handle))
			{
				result = PlayableOutput.Null;
			}
			else
			{
				result = new PlayableOutput(handle);
			}
			return result;
		}
	}
}
