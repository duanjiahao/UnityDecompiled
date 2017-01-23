using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	public sealed class NavMeshAgent : Behaviour
	{
		public Vector3 destination
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_destination(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_destination(ref value);
			}
		}

		public extern float stoppingDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_velocity(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_velocity(ref value);
			}
		}

		public Vector3 nextPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_nextPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_nextPosition(ref value);
			}
		}

		public Vector3 steeringTarget
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_steeringTarget(out result);
				return result;
			}
		}

		public Vector3 desiredVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_desiredVelocity(out result);
				return result;
			}
		}

		public extern float remainingDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float baseOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isOnOffMeshLink
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public OffMeshLinkData currentOffMeshLinkData
		{
			get
			{
				return this.GetCurrentOffMeshLinkDataInternal();
			}
		}

		public OffMeshLinkData nextOffMeshLinkData
		{
			get
			{
				return this.GetNextOffMeshLinkDataInternal();
			}
		}

		public extern bool autoTraverseOffMeshLink
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoBraking
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoRepath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool pathPending
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isPathStale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern NavMeshPathStatus pathStatus
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 pathEndPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_pathEndPosition(out result);
				return result;
			}
		}

		public NavMeshPath path
		{
			get
			{
				NavMeshPath navMeshPath = new NavMeshPath();
				this.CopyPathTo(navMeshPath);
				return navMeshPath;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException();
				}
				this.SetPath(value);
			}
		}

		[Obsolete("Use areaMask instead.")]
		public extern int walkableMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int areaMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float speed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angularSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float acceleration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updatePosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updateRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float height
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ObstacleAvoidanceType obstacleAvoidanceType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int avoidancePriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isOnNavMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public bool SetDestination(Vector3 target)
		{
			return NavMeshAgent.INTERNAL_CALL_SetDestination(this, ref target);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SetDestination(NavMeshAgent self, ref Vector3 target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_destination(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_destination(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_velocity(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_nextPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_nextPosition(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_steeringTarget(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_desiredVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ActivateCurrentOffMeshLink(bool activated);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern OffMeshLinkData GetCurrentOffMeshLinkDataInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern OffMeshLinkData GetNextOffMeshLinkDataInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CompleteOffMeshLink();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pathEndPosition(out Vector3 value);

		public bool Warp(Vector3 newPosition)
		{
			return NavMeshAgent.INTERNAL_CALL_Warp(this, ref newPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Warp(NavMeshAgent self, ref Vector3 newPosition);

		public void Move(Vector3 offset)
		{
			NavMeshAgent.INTERNAL_CALL_Move(this, ref offset);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Move(NavMeshAgent self, ref Vector3 offset);

		public void Stop()
		{
			this.StopInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopInternal();

		[Obsolete("Use Stop() instead")]
		public void Stop(bool stopUpdates)
		{
			this.StopInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Resume();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetPath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetPath(NavMeshPath path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void CopyPathTo(NavMeshPath path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool FindClosestEdge(out NavMeshHit hit);

		public bool Raycast(Vector3 targetPosition, out NavMeshHit hit)
		{
			return NavMeshAgent.INTERNAL_CALL_Raycast(this, ref targetPosition, out hit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Raycast(NavMeshAgent self, ref Vector3 targetPosition, out NavMeshHit hit);

		public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
		{
			path.ClearCorners();
			return this.CalculatePathInternal(targetPosition, path);
		}

		private bool CalculatePathInternal(Vector3 targetPosition, NavMeshPath path)
		{
			return NavMeshAgent.INTERNAL_CALL_CalculatePathInternal(this, ref targetPosition, path);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CalculatePathInternal(NavMeshAgent self, ref Vector3 targetPosition, NavMeshPath path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SamplePathPosition(int areaMask, float maxDistance, out NavMeshHit hit);

		[Obsolete("Use SetAreaCost instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLayerCost(int layer, float cost);

		[Obsolete("Use GetAreaCost instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetLayerCost(int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAreaCost(int areaIndex, float areaCost);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetAreaCost(int areaIndex);
	}
}
