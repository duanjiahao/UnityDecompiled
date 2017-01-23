using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	public struct Playable
	{
		internal IntPtr m_Handle;

		internal int m_Version;

		public static Playable Null
		{
			get
			{
				return new Playable
				{
					m_Version = 10
				};
			}
		}

		public int inputCount
		{
			get
			{
				return Playable.GetInputCountInternal(ref this);
			}
		}

		public int outputCount
		{
			get
			{
				return Playable.GetOutputCountInternal(ref this);
			}
		}

		public PlayState state
		{
			get
			{
				return Playable.GetPlayStateInternal(ref this);
			}
			set
			{
				Playable.SetPlayStateInternal(ref this, value);
			}
		}

		public double time
		{
			get
			{
				return Playable.GetTimeInternal(ref this);
			}
			set
			{
				Playable.SetTimeInternal(ref this, value);
			}
		}

		internal bool canChangeInputs
		{
			get
			{
				return Playable.CanChangeInputsInternal(ref this);
			}
		}

		internal bool canSetWeights
		{
			get
			{
				return Playable.CanSetWeightsInternal(ref this);
			}
		}

		internal bool canDestroy
		{
			get
			{
				return Playable.CanDestroyInternal(ref this);
			}
		}

		public double duration
		{
			get
			{
				return Playable.GetDurationInternal(ref this);
			}
			set
			{
				Playable.SetDurationInternal(ref this, value);
			}
		}

		public void Destroy()
		{
			Playables.InternalDestroy(ref this);
		}

		public bool IsValid()
		{
			return Playable.IsValidInternal(ref this);
		}

		private static bool IsValidInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_IsValidInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_IsValidInternal(ref Playable playable);

		public T CastTo<T>() where T : struct
		{
			return (T)((object)Playables.CastToInternal(typeof(T), this.m_Handle, this.m_Version));
		}

		public static Type GetTypeOf(Playable playable)
		{
			return Playables.GetTypeOfInternal(playable.m_Handle, playable.m_Version);
		}

		public static bool Connect(Playable source, Playable target)
		{
			return Playable.Connect(source, target, -1, -1);
		}

		public static bool Connect(Playable source, Playable target, int sourceOutputPort, int targetInputPort)
		{
			return Playables.ConnectInternal(ref source, ref target, sourceOutputPort, targetInputPort);
		}

		public static void Disconnect(Playable target, int inputPort)
		{
			if (target.CheckInputBounds(inputPort))
			{
				Playables.DisconnectInternal(ref target, inputPort);
			}
		}

		public static T Create<T>() where T : CustomAnimationPlayable, new()
		{
			return Playable.InternalCreate(typeof(T)) as T;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object InternalCreate(Type type);

		private static bool CanChangeInputsInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_CanChangeInputsInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CanChangeInputsInternal(ref Playable playable);

		private static bool CanSetWeightsInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_CanSetWeightsInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CanSetWeightsInternal(ref Playable playable);

		private static bool CanDestroyInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_CanDestroyInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CanDestroyInternal(ref Playable playable);

		private static PlayState GetPlayStateInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_GetPlayStateInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PlayState INTERNAL_CALL_GetPlayStateInternal(ref Playable playable);

		private static void SetPlayStateInternal(ref Playable playable, PlayState playState)
		{
			Playable.INTERNAL_CALL_SetPlayStateInternal(ref playable, playState);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetPlayStateInternal(ref Playable playable, PlayState playState);

		private static double GetTimeInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_GetTimeInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetTimeInternal(ref Playable playable);

		private static void SetTimeInternal(ref Playable playable, double time)
		{
			Playable.INTERNAL_CALL_SetTimeInternal(ref playable, time);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetTimeInternal(ref Playable playable, double time);

		private static double GetDurationInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_GetDurationInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double INTERNAL_CALL_GetDurationInternal(ref Playable playable);

		private static void SetDurationInternal(ref Playable playable, double duration)
		{
			Playable.INTERNAL_CALL_SetDurationInternal(ref playable, duration);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetDurationInternal(ref Playable playable, double duration);

		private static int GetInputCountInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_GetInputCountInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetInputCountInternal(ref Playable playable);

		private static int GetOutputCountInternal(ref Playable playable)
		{
			return Playable.INTERNAL_CALL_GetOutputCountInternal(ref playable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetOutputCountInternal(ref Playable playable);

		public Playable[] GetInputs()
		{
			List<Playable> list = new List<Playable>();
			int inputCount = this.inputCount;
			for (int i = 0; i < inputCount; i++)
			{
				list.Add(this.GetInput(i));
			}
			return list.ToArray();
		}

		public Playable GetInput(int inputPort)
		{
			return Playable.GetInputInternal(ref this, inputPort);
		}

		private static Playable GetInputInternal(ref Playable playable, int index)
		{
			Playable result;
			Playable.INTERNAL_CALL_GetInputInternal(ref playable, index, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetInputInternal(ref Playable playable, int index, out Playable value);

		public Playable[] GetOutputs()
		{
			List<Playable> list = new List<Playable>();
			int outputCount = this.outputCount;
			for (int i = 0; i < outputCount; i++)
			{
				list.Add(this.GetOutput(i));
			}
			return list.ToArray();
		}

		public Playable GetOutput(int outputPort)
		{
			return Playable.GetOutputInternal(ref this, outputPort);
		}

		private static Playable GetOutputInternal(ref Playable playable, int index)
		{
			Playable result;
			Playable.INTERNAL_CALL_GetOutputInternal(ref playable, index, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetOutputInternal(ref Playable playable, int index, out Playable value);

		private static void SetInputWeightFromIndexInternal(ref Playable playable, int index, float weight)
		{
			Playable.INTERNAL_CALL_SetInputWeightFromIndexInternal(ref playable, index, weight);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInputWeightFromIndexInternal(ref Playable playable, int index, float weight);

		private static void SetInputWeightInternal(ref Playable playable, Playable input, float weight)
		{
			Playable.INTERNAL_CALL_SetInputWeightInternal(ref playable, ref input, weight);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetInputWeightInternal(ref Playable playable, ref Playable input, float weight);

		public void SetInputWeight(Playable input, float weight)
		{
			Playable.SetInputWeightInternal(ref this, input, weight);
		}

		public bool SetInputWeight(int inputIndex, float weight)
		{
			bool result;
			if (this.CheckInputBounds(inputIndex))
			{
				Playable.SetInputWeightFromIndexInternal(ref this, inputIndex, weight);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public float GetInputWeight(int index)
		{
			return Playable.GetInputWeightInternal(ref this, index);
		}

		private static float GetInputWeightInternal(ref Playable playable, int index)
		{
			return Playable.INTERNAL_CALL_GetInputWeightInternal(ref playable, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_GetInputWeightInternal(ref Playable playable, int index);

		public static bool operator ==(Playable x, Playable y)
		{
			return Playable.CompareVersion(x, y);
		}

		public static bool operator !=(Playable x, Playable y)
		{
			return !Playable.CompareVersion(x, y);
		}

		public override bool Equals(object p)
		{
			return p != null && p.GetHashCode() == this.GetHashCode();
		}

		public override int GetHashCode()
		{
			return (int)this.m_Handle ^ this.m_Version;
		}

		internal static bool CompareVersion(Playable lhs, Playable rhs)
		{
			return lhs.m_Handle == rhs.m_Handle && lhs.m_Version == rhs.m_Version;
		}

		internal bool CheckInputBounds(int inputIndex)
		{
			return this.CheckInputBounds(inputIndex, false);
		}

		internal bool CheckInputBounds(int inputIndex, bool acceptAny)
		{
			bool result;
			if (inputIndex == -1 && acceptAny)
			{
				result = true;
			}
			else
			{
				if (inputIndex < 0)
				{
					throw new IndexOutOfRangeException("Index must be greater than 0");
				}
				Playable[] inputs = this.GetInputs();
				if (inputs.Length <= inputIndex)
				{
					throw new IndexOutOfRangeException(string.Concat(new object[]
					{
						"inputIndex ",
						inputIndex,
						" is greater than the number of available inputs (",
						inputs.Length,
						")."
					}));
				}
				result = true;
			}
			return result;
		}
	}
}
