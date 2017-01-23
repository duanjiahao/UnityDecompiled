using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Gyroscope
	{
		private int m_GyroIndex;

		public Vector3 rotationRate
		{
			get
			{
				return Gyroscope.rotationRate_Internal(this.m_GyroIndex);
			}
		}

		public Vector3 rotationRateUnbiased
		{
			get
			{
				return Gyroscope.rotationRateUnbiased_Internal(this.m_GyroIndex);
			}
		}

		public Vector3 gravity
		{
			get
			{
				return Gyroscope.gravity_Internal(this.m_GyroIndex);
			}
		}

		public Vector3 userAcceleration
		{
			get
			{
				return Gyroscope.userAcceleration_Internal(this.m_GyroIndex);
			}
		}

		public Quaternion attitude
		{
			get
			{
				return Gyroscope.attitude_Internal(this.m_GyroIndex);
			}
		}

		public bool enabled
		{
			get
			{
				return Gyroscope.getEnabled_Internal(this.m_GyroIndex);
			}
			set
			{
				Gyroscope.setEnabled_Internal(this.m_GyroIndex, value);
			}
		}

		public float updateInterval
		{
			get
			{
				return Gyroscope.getUpdateInterval_Internal(this.m_GyroIndex);
			}
			set
			{
				Gyroscope.setUpdateInterval_Internal(this.m_GyroIndex, value);
			}
		}

		internal Gyroscope(int index)
		{
			this.m_GyroIndex = index;
		}

		private static Vector3 rotationRate_Internal(int idx)
		{
			Vector3 result;
			Gyroscope.INTERNAL_CALL_rotationRate_Internal(idx, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_rotationRate_Internal(int idx, out Vector3 value);

		private static Vector3 rotationRateUnbiased_Internal(int idx)
		{
			Vector3 result;
			Gyroscope.INTERNAL_CALL_rotationRateUnbiased_Internal(idx, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_rotationRateUnbiased_Internal(int idx, out Vector3 value);

		private static Vector3 gravity_Internal(int idx)
		{
			Vector3 result;
			Gyroscope.INTERNAL_CALL_gravity_Internal(idx, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_gravity_Internal(int idx, out Vector3 value);

		private static Vector3 userAcceleration_Internal(int idx)
		{
			Vector3 result;
			Gyroscope.INTERNAL_CALL_userAcceleration_Internal(idx, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_userAcceleration_Internal(int idx, out Vector3 value);

		private static Quaternion attitude_Internal(int idx)
		{
			Quaternion result;
			Gyroscope.INTERNAL_CALL_attitude_Internal(idx, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_attitude_Internal(int idx, out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool getEnabled_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void setEnabled_Internal(int idx, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float getUpdateInterval_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void setUpdateInterval_Internal(int idx, float interval);
	}
}
