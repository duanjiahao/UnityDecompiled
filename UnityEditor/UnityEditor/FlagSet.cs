using System;

namespace UnityEditor
{
	[Serializable]
	internal struct FlagSet<T> where T : IConvertible
	{
		private ulong m_Flags;

		public FlagSet(T flags)
		{
			this.m_Flags = Convert.ToUInt64(flags);
		}

		public bool HasFlags(T flags)
		{
			return (this.m_Flags & Convert.ToUInt64(flags)) != 0uL;
		}

		public void SetFlags(T flags, bool value)
		{
			if (value)
			{
				this.m_Flags |= Convert.ToUInt64(flags);
			}
			else
			{
				this.m_Flags &= ~Convert.ToUInt64(flags);
			}
		}

		public static implicit operator FlagSet<T>(T flags)
		{
			return new FlagSet<T>(flags);
		}
	}
}
