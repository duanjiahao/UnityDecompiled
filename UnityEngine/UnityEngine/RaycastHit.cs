using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct RaycastHit
	{
		private Vector3 m_Point;

		private Vector3 m_Normal;

		private int m_FaceID;

		private float m_Distance;

		private Vector2 m_UV;

		private Collider m_Collider;

		public Vector3 point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
			set
			{
				this.m_Normal = value;
			}
		}

		public Vector3 barycentricCoordinate
		{
			get
			{
				return new Vector3(1f - (this.m_UV.y + this.m_UV.x), this.m_UV.x, this.m_UV.y);
			}
			set
			{
				this.m_UV = value;
			}
		}

		public float distance
		{
			get
			{
				return this.m_Distance;
			}
			set
			{
				this.m_Distance = value;
			}
		}

		public int triangleIndex
		{
			get
			{
				return this.m_FaceID;
			}
		}

		public Vector2 textureCoord
		{
			get
			{
				Vector2 result;
				RaycastHit.CalculateRaycastTexCoord(out result, this.collider, this.m_UV, this.m_Point, this.m_FaceID, 0);
				return result;
			}
		}

		public Vector2 textureCoord2
		{
			get
			{
				Vector2 result;
				RaycastHit.CalculateRaycastTexCoord(out result, this.collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
				return result;
			}
		}

		[Obsolete("Use textureCoord2 instead")]
		public Vector2 textureCoord1
		{
			get
			{
				Vector2 result;
				RaycastHit.CalculateRaycastTexCoord(out result, this.collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
				return result;
			}
		}

		public Vector2 lightmapCoord
		{
			get
			{
				Vector2 result;
				RaycastHit.CalculateRaycastTexCoord(out result, this.collider, this.m_UV, this.m_Point, this.m_FaceID, 1);
				if (this.collider.GetComponent<Renderer>() != null)
				{
					Vector4 lightmapScaleOffset = this.collider.GetComponent<Renderer>().lightmapScaleOffset;
					result.x = result.x * lightmapScaleOffset.x + lightmapScaleOffset.z;
					result.y = result.y * lightmapScaleOffset.y + lightmapScaleOffset.w;
				}
				return result;
			}
		}

		public Collider collider
		{
			get
			{
				return this.m_Collider;
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				return (!(this.collider != null)) ? null : this.collider.attachedRigidbody;
			}
		}

		public Transform transform
		{
			get
			{
				Rigidbody rigidbody = this.rigidbody;
				if (rigidbody != null)
				{
					return rigidbody.transform;
				}
				if (this.collider != null)
				{
					return this.collider.transform;
				}
				return null;
			}
		}

		private static void CalculateRaycastTexCoord(out Vector2 output, Collider col, Vector2 uv, Vector3 point, int face, int index)
		{
			RaycastHit.INTERNAL_CALL_CalculateRaycastTexCoord(out output, col, ref uv, ref point, face, index);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CalculateRaycastTexCoord(out Vector2 output, Collider col, ref Vector2 uv, ref Vector3 point, int face, int index);
	}
}
