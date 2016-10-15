using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	internal struct AnimationLayerMixerPlayable
	{
		internal AnimationPlayable handle;

		internal Playable node
		{
			get
			{
				return this.handle.node;
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

		public static AnimationLayerMixerPlayable Create()
		{
			AnimationLayerMixerPlayable result = default(AnimationLayerMixerPlayable);
			AnimationLayerMixerPlayable.InternalCreate(ref result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCreate(ref AnimationLayerMixerPlayable that);

		public void Destroy()
		{
			this.node.Destroy();
		}

		public override bool Equals(object p)
		{
			return Playables.Equals(this, p);
		}

		public override int GetHashCode()
		{
			return this.node.GetHashCode();
		}

		public bool IsValid()
		{
			return Playables.IsValid(this);
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

		public T CastTo<T>() where T : struct
		{
			return this.handle.CastTo<T>();
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

		public static bool operator ==(AnimationLayerMixerPlayable x, Playable y)
		{
			return Playables.Equals(x, y);
		}

		public static bool operator !=(AnimationLayerMixerPlayable x, Playable y)
		{
			return !Playables.Equals(x, y);
		}

		public static implicit operator Playable(AnimationLayerMixerPlayable b)
		{
			return b.node;
		}

		public static implicit operator AnimationPlayable(AnimationLayerMixerPlayable b)
		{
			return b.handle;
		}
	}
}
