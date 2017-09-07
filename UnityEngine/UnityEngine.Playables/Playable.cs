using System;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	[RequiredByNativeCode]
	public struct Playable : IPlayable, IEquatable<Playable>
	{
		private PlayableHandle m_Handle;

		private static readonly Playable m_NullPlayable = new Playable(PlayableHandle.Null);

		public static Playable Null
		{
			get
			{
				return Playable.m_NullPlayable;
			}
		}

		internal Playable(PlayableHandle handle)
		{
			this.m_Handle = handle;
		}

		public static Playable Create(PlayableGraph graph, int inputCount = 0)
		{
			Playable playable = new Playable(graph.CreatePlayableHandle());
			playable.SetInputCount(inputCount);
			return playable;
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public bool IsPlayableOfType<T>() where T : struct, IPlayable
		{
			return this.GetHandle().IsPlayableOfType<T>();
		}

		public Type GetPlayableType()
		{
			return this.GetHandle().GetPlayableType();
		}

		public bool Equals(Playable other)
		{
			return this.GetHandle() == other.GetHandle();
		}
	}
}
