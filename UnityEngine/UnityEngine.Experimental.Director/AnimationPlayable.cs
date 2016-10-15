using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	public struct AnimationPlayable
	{
		internal Playable handle;

		internal Playable node
		{
			get
			{
				return this.handle;
			}
		}

		public static AnimationPlayable Null
		{
			get
			{
				AnimationPlayable result = default(AnimationPlayable);
				result.handle.m_Version = 10;
				return result;
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

		public void Destroy()
		{
			this.node.Destroy();
		}

		public int AddInput(Playable input)
		{
			if (!Playable.Connect(input, this, -1, -1))
			{
				throw new InvalidOperationException("AddInput Failed. Either the connected playable is incompatible or this AnimationPlayable type doesn't support adding inputs");
			}
			return this.inputCount - 1;
		}

		public bool SetInput(Playable source, int index)
		{
			if (!this.node.CheckInputBounds(index))
			{
				return false;
			}
			if (this.GetInput(index).IsValid())
			{
				Playable.Disconnect(this, index);
			}
			return Playable.Connect(source, this, -1, index);
		}

		public bool SetInputs(IEnumerable<Playable> sources)
		{
			for (int i = 0; i < this.inputCount; i++)
			{
				Playable.Disconnect(this, i);
			}
			bool flag = false;
			int num = 0;
			foreach (Playable current in sources)
			{
				if (num < this.inputCount)
				{
					flag |= Playable.Connect(current, this, -1, num);
				}
				else
				{
					flag |= Playable.Connect(current, this, -1, -1);
				}
				this.node.SetInputWeight(num, 1f);
				num++;
			}
			for (int j = num; j < this.inputCount; j++)
			{
				this.node.SetInputWeight(j, 0f);
			}
			return flag;
		}

		public bool RemoveInput(int index)
		{
			if (!Playables.CheckInputBounds(this, index))
			{
				return false;
			}
			Playable.Disconnect(this, index);
			return true;
		}

		public bool RemoveInput(Playable playable)
		{
			for (int i = 0; i < this.inputCount; i++)
			{
				if (this.GetInput(i) == playable)
				{
					Playable.Disconnect(this, i);
					return true;
				}
			}
			return false;
		}

		public bool RemoveAllInputs()
		{
			int inputCount = this.node.inputCount;
			for (int i = 0; i < inputCount; i++)
			{
				this.RemoveInput(i);
			}
			return true;
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

		public static bool operator ==(AnimationPlayable x, Playable y)
		{
			return Playables.Equals(x, y);
		}

		public static bool operator !=(AnimationPlayable x, Playable y)
		{
			return !Playables.Equals(x, y);
		}

		public static implicit operator Playable(AnimationPlayable b)
		{
			return b.node;
		}
	}
}
