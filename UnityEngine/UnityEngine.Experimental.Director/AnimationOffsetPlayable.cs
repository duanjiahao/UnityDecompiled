using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	internal struct AnimationOffsetPlayable
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

		public Vector3 position
		{
			get
			{
				return AnimationOffsetPlayable.GetPosition(ref this);
			}
			set
			{
				AnimationOffsetPlayable.SetPosition(ref this, value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				return AnimationOffsetPlayable.GetRotation(ref this);
			}
			set
			{
				AnimationOffsetPlayable.SetRotation(ref this, value);
			}
		}

		public static AnimationOffsetPlayable Create()
		{
			AnimationOffsetPlayable result = default(AnimationOffsetPlayable);
			AnimationOffsetPlayable.InternalCreate(ref result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCreate(ref AnimationOffsetPlayable that);

		public void Destroy()
		{
			this.node.Destroy();
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

		public static bool operator ==(AnimationOffsetPlayable x, Playable y)
		{
			return Playables.Equals(x, y);
		}

		public static bool operator !=(AnimationOffsetPlayable x, Playable y)
		{
			return !Playables.Equals(x, y);
		}

		public override bool Equals(object p)
		{
			return Playables.Equals(this, p);
		}

		public override int GetHashCode()
		{
			return this.node.GetHashCode();
		}

		public static implicit operator Playable(AnimationOffsetPlayable b)
		{
			return b.node;
		}

		public static implicit operator AnimationPlayable(AnimationOffsetPlayable b)
		{
			return b.handle;
		}

		public bool IsValid()
		{
			return Playables.IsValid(this);
		}

		public T CastTo<T>() where T : struct
		{
			return this.handle.CastTo<T>();
		}

		public int AddInput(Playable input)
		{
			return AnimationPlayableUtilities.AddInputValidated(this, input, base.GetType());
		}

		public bool RemoveInput(int index)
		{
			return AnimationPlayableUtilities.RemoveInputValidated(this, index, base.GetType());
		}

		public bool RemoveAllInputs()
		{
			return AnimationPlayableUtilities.RemoveAllInputsValidated(this, base.GetType());
		}

		private static Vector3 GetPosition(ref AnimationOffsetPlayable that)
		{
			Vector3 result;
			AnimationOffsetPlayable.INTERNAL_CALL_GetPosition(ref that, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetPosition(ref AnimationOffsetPlayable that, out Vector3 value);

		private static void SetPosition(ref AnimationOffsetPlayable that, Vector3 value)
		{
			AnimationOffsetPlayable.INTERNAL_CALL_SetPosition(ref that, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPosition(ref AnimationOffsetPlayable that, ref Vector3 value);

		private static Quaternion GetRotation(ref AnimationOffsetPlayable that)
		{
			Quaternion result;
			AnimationOffsetPlayable.INTERNAL_CALL_GetRotation(ref that, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetRotation(ref AnimationOffsetPlayable that, out Quaternion value);

		private static void SetRotation(ref AnimationOffsetPlayable that, Quaternion value)
		{
			AnimationOffsetPlayable.INTERNAL_CALL_SetRotation(ref that, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetRotation(ref AnimationOffsetPlayable that, ref Quaternion value);
	}
}
