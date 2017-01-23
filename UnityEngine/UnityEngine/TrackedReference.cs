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

		public static bool operator ==(TrackedReference x, TrackedReference y)
		{
			bool result;
			if (y == null && x == null)
			{
				result = true;
			}
			else if (y == null)
			{
				result = (x.m_Ptr == IntPtr.Zero);
			}
			else if (x == null)
			{
				result = (y.m_Ptr == IntPtr.Zero);
			}
			else
			{
				result = (x.m_Ptr == y.m_Ptr);
			}
			return result;
		}

		public static bool operator !=(TrackedReference x, TrackedReference y)
		{
			return !(x == y);
		}

		public override bool Equals(object o)
		{
			return o as TrackedReference == this;
		}

		public override int GetHashCode()
		{
			return (int)this.m_Ptr;
		}

		public static implicit operator bool(TrackedReference exists)
		{
			return exists != null;
		}
	}
}
