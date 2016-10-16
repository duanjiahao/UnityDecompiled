using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct Bounds
	{
		private Vector3 m_Center;

		private Vector3 m_Extents;

		public Vector3 center
		{
			get
			{
				return this.m_Center;
			}
			set
			{
				this.m_Center = value;
			}
		}

		public Vector3 size
		{
			get
			{
				return this.m_Extents * 2f;
			}
			set
			{
				this.m_Extents = value * 0.5f;
			}
		}

		public Vector3 extents
		{
			get
			{
				return this.m_Extents;
			}
			set
			{
				this.m_Extents = value;
			}
		}

		public Vector3 min
		{
			get
			{
				return this.center - this.extents;
			}
			set
			{
				this.SetMinMax(value, this.max);
			}
		}

		public Vector3 max
		{
			get
			{
				return this.center + this.extents;
			}
			set
			{
				this.SetMinMax(this.min, value);
			}
		}

		public Bounds(Vector3 center, Vector3 size)
		{
			this.m_Center = center;
			this.m_Extents = size * 0.5f;
		}

		public override int GetHashCode()
		{
			return this.center.GetHashCode() ^ this.extents.GetHashCode() << 2;
		}

		public override bool Equals(object other)
		{
			if (!(other is Bounds))
			{
				return false;
			}
			Bounds bounds = (Bounds)other;
			return this.center.Equals(bounds.center) && this.extents.Equals(bounds.extents);
		}

		public void SetMinMax(Vector3 min, Vector3 max)
		{
			this.extents = (max - min) * 0.5f;
			this.center = min + this.extents;
		}

		public void Encapsulate(Vector3 point)
		{
			this.SetMinMax(Vector3.Min(this.min, point), Vector3.Max(this.max, point));
		}

		public void Encapsulate(Bounds bounds)
		{
			this.Encapsulate(bounds.center - bounds.extents);
			this.Encapsulate(bounds.center + bounds.extents);
		}

		public void Expand(float amount)
		{
			amount *= 0.5f;
			this.extents += new Vector3(amount, amount, amount);
		}

		public void Expand(Vector3 amount)
		{
			this.extents += amount * 0.5f;
		}

		public bool Intersects(Bounds bounds)
		{
			return this.min.x <= bounds.max.x && this.max.x >= bounds.min.x && this.min.y <= bounds.max.y && this.max.y >= bounds.min.y && this.min.z <= bounds.max.z && this.max.z >= bounds.min.z;
		}

		[ThreadAndSerializationSafe]
		private static bool Internal_Contains(Bounds m, Vector3 point)
		{
			return Bounds.INTERNAL_CALL_Internal_Contains(ref m, ref point);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_Contains(ref Bounds m, ref Vector3 point);

		public bool Contains(Vector3 point)
		{
			return Bounds.Internal_Contains(this, point);
		}

		private static float Internal_SqrDistance(Bounds m, Vector3 point)
		{
			return Bounds.INTERNAL_CALL_Internal_SqrDistance(ref m, ref point);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float INTERNAL_CALL_Internal_SqrDistance(ref Bounds m, ref Vector3 point);

		public float SqrDistance(Vector3 point)
		{
			return Bounds.Internal_SqrDistance(this, point);
		}

		private static bool Internal_IntersectRay(ref Ray ray, ref Bounds bounds, out float distance)
		{
			return Bounds.INTERNAL_CALL_Internal_IntersectRay(ref ray, ref bounds, out distance);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Internal_IntersectRay(ref Ray ray, ref Bounds bounds, out float distance);

		public bool IntersectRay(Ray ray)
		{
			float num;
			return Bounds.Internal_IntersectRay(ref ray, ref this, out num);
		}

		public bool IntersectRay(Ray ray, out float distance)
		{
			return Bounds.Internal_IntersectRay(ref ray, ref this, out distance);
		}

		private static Vector3 Internal_GetClosestPoint(ref Bounds bounds, ref Vector3 point)
		{
			Vector3 result;
			Bounds.INTERNAL_CALL_Internal_GetClosestPoint(ref bounds, ref point, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_GetClosestPoint(ref Bounds bounds, ref Vector3 point, out Vector3 value);

		public Vector3 ClosestPoint(Vector3 point)
		{
			return Bounds.Internal_GetClosestPoint(ref this, ref point);
		}

		public override string ToString()
		{
			return UnityString.Format("Center: {0}, Extents: {1}", new object[]
			{
				this.m_Center,
				this.m_Extents
			});
		}

		public string ToString(string format)
		{
			return UnityString.Format("Center: {0}, Extents: {1}", new object[]
			{
				this.m_Center.ToString(format),
				this.m_Extents.ToString(format)
			});
		}

		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			return !(lhs == rhs);
		}
	}
}
