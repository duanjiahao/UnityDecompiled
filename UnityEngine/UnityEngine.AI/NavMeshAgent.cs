using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float baseOffset
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isOnOffMeshLink
		{
			[GeneratedByOldBindingsGenerator]
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
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoBraking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool autoRepath
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasPath
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool pathPending
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isPathStale
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern NavMeshPathStatus pathStatus
		{
			[GeneratedByOldBindingsGenerator]
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

		public extern bool isStopped
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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

		public UnityEngine.Object navMeshOwner
		{
			get
			{
				return this.GetOwnerInternal();
			}
		}

		public extern int agentTypeID
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Use areaMask instead.")]
		public extern int walkableMask
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int areaMask
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float speed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float angularSpeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float acceleration
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updatePosition
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updateRotation
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool updateUpAxis
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float radius
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float height
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ObstacleAvoidanceType obstacleAvoidanceType
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int avoidancePriority
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isOnNavMesh
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public bool SetDestination(Vector3 target)
		{
			return NavMeshAgent.INTERNAL_CALL_SetDestination(this, ref target);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SetDestination(NavMeshAgent self, ref Vector3 target);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_destination(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_destination(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_velocity(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_nextPosition(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_nextPosition(ref Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_steeringTarget(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_desiredVelocity(out Vector3 value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ActivateCurrentOffMeshLink(bool activated);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern OffMeshLinkData GetCurrentOffMeshLinkDataInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern OffMeshLinkData GetNextOffMeshLinkDataInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CompleteOffMeshLink();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pathEndPosition(out Vector3 value);

		public bool Warp(Vector3 newPosition)
		{
			return NavMeshAgent.INTERNAL_CALL_Warp(this, ref newPosition);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Warp(NavMeshAgent self, ref Vector3 newPosition);

		public void Move(Vector3 offset)
		{
			NavMeshAgent.INTERNAL_CALL_Move(this, ref offset);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Move(NavMeshAgent self, ref Vector3 offset);

		[Obsolete("Set isStopped to true instead")]
		public void Stop()
		{
			this.StopInternal();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopInternal();

		[Obsolete("Set isStopped to true instead")]
		public void Stop(bool stopUpdates)
		{
			this.StopInternal();
		}

		[Obsolete("Set isStopped to false instead"), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Resume();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResetPath();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetPath(NavMeshPath path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void CopyPathTo(NavMeshPath path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool FindClosestEdge(out NavMeshHit hit);

		public bool Raycast(Vector3 targetPosition, out NavMeshHit hit)
		{
			return NavMeshAgent.INTERNAL_CALL_Raycast(this, ref targetPosition, out hit);
		}

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CalculatePathInternal(NavMeshAgent self, ref Vector3 targetPosition, NavMeshPath path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SamplePathPosition(int areaMask, float maxDistance, out NavMeshHit hit);

		[Obsolete("Use SetAreaCost instead."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLayerCost(int layer, float cost);

		[Obsolete("Use GetAreaCost instead."), GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetLayerCost(int layer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetAreaCost(int areaIndex, float areaCost);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetAreaCost(int areaIndex);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnityEngine.Object GetOwnerInternal();
	}
}
