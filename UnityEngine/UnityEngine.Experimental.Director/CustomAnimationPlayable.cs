using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[RequiredByNativeCode]
	public class CustomAnimationPlayable : ScriptPlayable
	{
		internal AnimationPlayable handle;

		internal Playable node
		{
			get
			{
				return this.handle;
			}
		}

		public int inputCount
		{
			get
			{
				return Playables.GetInputCountValidated(this, base.GetType());
			}
		}

		public int outputCount
		{
			get
			{
				return Playables.GetOutputCountValidated(this, base.GetType());
			}
		}

		public PlayState state
		{
			get
			{
				return Playables.GetPlayStateValidated(this, base.GetType());
			}
			set
			{
				Playables.SetPlayStateValidated(this, value, base.GetType());
			}
		}

		public double time
		{
			get
			{
				return Playables.GetTimeValidated(this, base.GetType());
			}
			set
			{
				Playables.SetTimeValidated(this, value, base.GetType());
			}
		}

		public double duration
		{
			get
			{
				return Playables.GetDurationValidated(this, base.GetType());
			}
			set
			{
				Playables.SetDurationValidated(this, value, base.GetType());
			}
		}

		public CustomAnimationPlayable()
		{
			if (!this.handle.IsValid())
			{
				string text = base.GetType().ToString();
				throw new InvalidOperationException(string.Concat(new string[]
				{
					text,
					" must be instantiated using the Playable.Create<",
					text,
					"> method instead of new ",
					text,
					"."
				}));
			}
		}

		internal void SetHandle(int version, IntPtr playableHandle)
		{
			this.handle.handle.m_Handle = playableHandle;
			this.handle.handle.m_Version = version;
		}

		public void Destroy()
		{
			this.node.Destroy();
		}

		public virtual void PrepareFrame(FrameData info)
		{
		}

		public virtual void OnSetTime(float localTime)
		{
		}

		public virtual void OnSetPlayState(PlayState newState)
		{
		}

		public T CastTo<T>() where T : struct
		{
			return this.handle.CastTo<T>();
		}

		public Playable GetInput(int inputPort)
		{
			return Playables.GetInputValidated(this, inputPort, base.GetType());
		}

		public Playable GetOutput(int outputPort)
		{
			return Playables.GetOutputValidated(this, outputPort, base.GetType());
		}

		public float GetInputWeight(int index)
		{
			return Playables.GetInputWeightValidated(this, index, base.GetType());
		}

		public void SetInputWeight(int inputIndex, float weight)
		{
			Playables.SetInputWeightValidated(this, inputIndex, weight, base.GetType());
		}

		public int AddInput(Playable input)
		{
			return AnimationPlayableUtilities.AddInputValidated(this, input, base.GetType());
		}

		public bool SetInput(Playable source, int index)
		{
			return AnimationPlayableUtilities.SetInputValidated(this, source, index, base.GetType());
		}

		public bool SetInputs(IEnumerable<Playable> sources)
		{
			return AnimationPlayableUtilities.SetInputsValidated(this, sources, base.GetType());
		}

		public bool RemoveInput(int index)
		{
			return AnimationPlayableUtilities.RemoveInputValidated(this, index, base.GetType());
		}

		public bool RemoveAllInputs()
		{
			return AnimationPlayableUtilities.RemoveAllInputsValidated(this, base.GetType());
		}

		public static implicit operator Playable(CustomAnimationPlayable s)
		{
			return new Playable
			{
				m_Handle = s.node.m_Handle,
				m_Version = s.node.m_Version
			};
		}

		public static implicit operator AnimationPlayable(CustomAnimationPlayable s)
		{
			return s.handle;
		}
	}
}
