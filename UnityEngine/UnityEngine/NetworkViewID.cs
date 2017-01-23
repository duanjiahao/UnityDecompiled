using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct NetworkViewID
	{
		private int a;

		private int b;

		private int c;

		public static NetworkViewID unassigned
		{
			get
			{
				NetworkViewID result;
				NetworkViewID.INTERNAL_get_unassigned(out result);
				return result;
			}
		}

		public bool isMine
		{
			get
			{
				return NetworkViewID.Internal_IsMine(this);
			}
		}

		public NetworkPlayer owner
		{
			get
			{
				NetworkPlayer result;
				NetworkViewID.Internal_GetOwner(this, out result);
				return result;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_get_unassigned(out NetworkViewID value);

		internal static bool Internal_IsMine(NetworkViewID value)
		{
			return NetworkViewID.INTERNAL_CALL_Internal_IsMine(ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_IsMine(ref NetworkViewID value);

		internal static void Internal_GetOwner(NetworkViewID value, out NetworkPlayer player)
		{
			NetworkViewID.INTERNAL_CALL_Internal_GetOwner(ref value, out player);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_GetOwner(ref NetworkViewID value, out NetworkPlayer player);

		internal static string Internal_GetString(NetworkViewID value)
		{
			return NetworkViewID.INTERNAL_CALL_Internal_GetString(ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string INTERNAL_CALL_Internal_GetString(ref NetworkViewID value);

		internal static bool Internal_Compare(NetworkViewID lhs, NetworkViewID rhs)
		{
			return NetworkViewID.INTERNAL_CALL_Internal_Compare(ref lhs, ref rhs);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_Compare(ref NetworkViewID lhs, ref NetworkViewID rhs);

		public static bool operator ==(NetworkViewID lhs, NetworkViewID rhs)
		{
			return NetworkViewID.Internal_Compare(lhs, rhs);
		}

		public static bool operator !=(NetworkViewID lhs, NetworkViewID rhs)
		{
			return !NetworkViewID.Internal_Compare(lhs, rhs);
		}

		public override int GetHashCode()
		{
			return this.a ^ this.b ^ this.c;
		}

		public override bool Equals(object other)
		{
			bool result;
			if (!(other is NetworkViewID))
			{
				result = false;
			}
			else
			{
				NetworkViewID rhs = (NetworkViewID)other;
				result = NetworkViewID.Internal_Compare(this, rhs);
			}
			return result;
		}

		public override string ToString()
		{
			return NetworkViewID.Internal_GetString(this);
		}
	}
}
