using System;

namespace UnityEngine.Playables
{
	public struct ScriptPlayable<T> : IPlayable, IEquatable<ScriptPlayable<T>> where T : class, IPlayableBehaviour, new()
	{
		private PlayableHandle m_Handle;

		private static readonly ScriptPlayable<T> m_NullPlayable = new ScriptPlayable<T>(PlayableHandle.Null);

		public static ScriptPlayable<T> Null
		{
			get
			{
				return ScriptPlayable<T>.m_NullPlayable;
			}
		}

		internal ScriptPlayable(PlayableHandle handle)
		{
			if (handle.IsValid())
			{
				if (!typeof(T).IsAssignableFrom(handle.GetPlayableType()))
				{
					throw new InvalidCastException(string.Format("Incompatible handle: Trying to assign a playable data of type `{0}` that is not compatible with the PlayableBehaviour of type `{1}`.", handle.GetPlayableType(), typeof(T)));
				}
			}
			this.m_Handle = handle;
		}

		public static ScriptPlayable<T> Create(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle handle = ScriptPlayable<T>.CreateHandle(graph, (T)((object)null), inputCount);
			return new ScriptPlayable<T>(handle);
		}

		public static ScriptPlayable<T> Create(PlayableGraph graph, T template, int inputCount = 0)
		{
			PlayableHandle handle = ScriptPlayable<T>.CreateHandle(graph, template, inputCount);
			return new ScriptPlayable<T>(handle);
		}

		private static PlayableHandle CreateHandle(PlayableGraph graph, T template, int inputCount)
		{
			object obj;
			if (template == null)
			{
				obj = ScriptPlayable<T>.CreateScriptInstance();
			}
			else
			{
				obj = ScriptPlayable<T>.CloneScriptInstance(template);
			}
			PlayableHandle result;
			if (obj == null)
			{
				Debug.LogError("Could not create a ScriptPlayable of Type " + typeof(T).ToString());
				result = PlayableHandle.Null;
			}
			else
			{
				PlayableHandle playableHandle = graph.CreatePlayableHandle();
				if (!playableHandle.IsValid())
				{
					result = PlayableHandle.Null;
				}
				else
				{
					playableHandle.SetInputCount(inputCount);
					playableHandle.SetScriptInstance(obj);
					result = playableHandle;
				}
			}
			return result;
		}

		private static object CreateScriptInstance()
		{
			IPlayableBehaviour result;
			if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
			{
				result = (ScriptableObject.CreateInstance(typeof(T)) as T);
			}
			else
			{
				result = Activator.CreateInstance<T>();
			}
			return result;
		}

		private static object CloneScriptInstance(IPlayableBehaviour source)
		{
			UnityEngine.Object @object = source as UnityEngine.Object;
			object result;
			if (@object != null)
			{
				result = ScriptPlayable<T>.CloneScriptInstanceFromEngineObject(@object);
			}
			else
			{
				ICloneable cloneable = source as ICloneable;
				if (cloneable != null)
				{
					result = ScriptPlayable<T>.CloneScriptInstanceFromIClonable(cloneable);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private static object CloneScriptInstanceFromEngineObject(UnityEngine.Object source)
		{
			UnityEngine.Object @object = UnityEngine.Object.Instantiate(source);
			if (@object != null)
			{
				@object.hideFlags |= HideFlags.DontSave;
			}
			return @object;
		}

		private static object CloneScriptInstanceFromIClonable(ICloneable source)
		{
			return source.Clone();
		}

		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		public T GetBehaviour()
		{
			return this.m_Handle.GetObject<T>();
		}

		public static implicit operator Playable(ScriptPlayable<T> playable)
		{
			return new Playable(playable.GetHandle());
		}

		public static explicit operator ScriptPlayable<T>(Playable playable)
		{
			return new ScriptPlayable<T>(playable.GetHandle());
		}

		public bool Equals(ScriptPlayable<T> other)
		{
			return this.GetHandle() == other.GetHandle();
		}
	}
}
