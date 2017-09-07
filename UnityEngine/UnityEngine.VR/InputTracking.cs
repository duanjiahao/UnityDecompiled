using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEngine.VR
{
	[RequiredByNativeCode]
	public static class InputTracking
	{
		private enum TrackingStateEventType
		{
			NodeAdded,
			NodeRemoved,
			TrackingAcquired,
			TrackingLost
		}

		public static event Action<VRNodeState> trackingAcquired
		{
			add
			{
				Action<VRNodeState> action = InputTracking.trackingAcquired;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.trackingAcquired, (Action<VRNodeState>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<VRNodeState> action = InputTracking.trackingAcquired;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.trackingAcquired, (Action<VRNodeState>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public static event Action<VRNodeState> trackingLost
		{
			add
			{
				Action<VRNodeState> action = InputTracking.trackingLost;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.trackingLost, (Action<VRNodeState>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<VRNodeState> action = InputTracking.trackingLost;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.trackingLost, (Action<VRNodeState>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public static event Action<VRNodeState> nodeAdded
		{
			add
			{
				Action<VRNodeState> action = InputTracking.nodeAdded;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.nodeAdded, (Action<VRNodeState>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<VRNodeState> action = InputTracking.nodeAdded;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.nodeAdded, (Action<VRNodeState>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public static event Action<VRNodeState> nodeRemoved
		{
			add
			{
				Action<VRNodeState> action = InputTracking.nodeRemoved;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.nodeRemoved, (Action<VRNodeState>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<VRNodeState> action = InputTracking.nodeRemoved;
				Action<VRNodeState> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<VRNodeState>>(ref InputTracking.nodeRemoved, (Action<VRNodeState>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public static extern bool disablePositionalTracking
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[RequiredByNativeCode]
		private static void InvokeTrackingEvent(InputTracking.TrackingStateEventType eventType, VRNode nodeType, long uniqueID, bool tracked)
		{
			VRNodeState obj = default(VRNodeState);
			obj.uniqueID = (ulong)uniqueID;
			obj.nodeType = nodeType;
			obj.tracked = tracked;
			Action<VRNodeState> action;
			switch (eventType)
			{
			case InputTracking.TrackingStateEventType.NodeAdded:
				action = InputTracking.nodeAdded;
				break;
			case InputTracking.TrackingStateEventType.NodeRemoved:
				action = InputTracking.nodeRemoved;
				break;
			case InputTracking.TrackingStateEventType.TrackingAcquired:
				action = InputTracking.trackingAcquired;
				break;
			case InputTracking.TrackingStateEventType.TrackingLost:
				action = InputTracking.trackingLost;
				break;
			default:
				throw new ArgumentException("TrackingEventHandler - Invalid EventType: " + eventType);
			}
			if (action != null)
			{
				action(obj);
			}
		}

		public static Vector3 GetLocalPosition(VRNode node)
		{
			Vector3 result;
			InputTracking.INTERNAL_CALL_GetLocalPosition(node, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalPosition(VRNode node, out Vector3 value);

		public static Quaternion GetLocalRotation(VRNode node)
		{
			Quaternion result;
			InputTracking.INTERNAL_CALL_GetLocalRotation(node, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetLocalRotation(VRNode node, out Quaternion value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Recenter();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetNodeName(ulong uniqueID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeStatesInternal(object nodeStates);

		public static void GetNodeStates(List<VRNodeState> nodeStates)
		{
			if (nodeStates == null)
			{
				throw new ArgumentNullException("nodeStates");
			}
			nodeStates.Clear();
			InputTracking.GetNodeStatesInternal(nodeStates);
		}

		static InputTracking()
		{
			// Note: this type is marked as 'beforefieldinit'.
			InputTracking.trackingAcquired = null;
			InputTracking.trackingLost = null;
			InputTracking.nodeAdded = null;
			InputTracking.nodeRemoved = null;
		}
	}
}
