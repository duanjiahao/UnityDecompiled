using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public class TrackedReference
	{
		internal IntPtr m_Ptr;

		protected TrackedReference()
		{
		}

		public override bool Equals(object o)
		{
			return o as TrackedReference == this;
		}

		public override int GetHashCode()
		{
			return (int)this.m_Ptr;
		}

		public static bool operator ==(TrackedReference x, TrackedReference y)
		{
			if (y == null && x == null)
			{
				return true;
			}
			if (y == null)
			{
				return x.m_Ptr == IntPtr.Zero;
			}
			if (x == null)
			{
				return y.m_Ptr == IntPtr.Zero;
			}
			return x.m_Ptr == y.m_Ptr;
		}

		public static bool operator !=(TrackedReference x, TrackedReference y)
		{
			return !(x == y);
		}

		public static implicit operator bool(TrackedReference exists)
		{
			return exists != null;
		}
	}
}
