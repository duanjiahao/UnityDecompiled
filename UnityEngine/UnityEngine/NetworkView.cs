using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[Obsolete("Unity Multiplayer and NetworkIdentity component instead.")]
	public sealed class NetworkView : Behaviour
	{
		public extern Component observed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern NetworkStateSynchronization stateSynchronization
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public NetworkViewID viewID
		{
			get
			{
				NetworkViewID result;
				this.Internal_GetViewID(out result);
				return result;
			}
			set
			{
				this.Internal_SetViewID(value);
			}
		}

		public extern int group
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool isMine
		{
			get
			{
				return this.viewID.isMine;
			}
		}

		public NetworkPlayer owner
		{
			get
			{
				return this.viewID.owner;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RPC(NetworkView view, string name, RPCMode mode, object[] args);

		private static void Internal_RPC_Target(NetworkView view, string name, NetworkPlayer target, object[] args)
		{
			NetworkView.INTERNAL_CALL_Internal_RPC_Target(view, name, ref target, args);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_RPC_Target(NetworkView view, string name, ref NetworkPlayer target, object[] args);

		[Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system.")]
		public void RPC(string name, RPCMode mode, params object[] args)
		{
			NetworkView.Internal_RPC(this, name, mode, args);
		}

		[Obsolete("NetworkView RPC functions are deprecated. Refer to the new Multiplayer Networking system.")]
		public void RPC(string name, NetworkPlayer target, params object[] args)
		{
			NetworkView.Internal_RPC_Target(this, name, target, args);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetViewID(out NetworkViewID viewID);

		private void Internal_SetViewID(NetworkViewID viewID)
		{
			NetworkView.INTERNAL_CALL_Internal_SetViewID(this, ref viewID);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetViewID(NetworkView self, ref NetworkViewID viewID);

		public bool SetScope(NetworkPlayer player, bool relevancy)
		{
			return NetworkView.INTERNAL_CALL_SetScope(this, ref player, relevancy);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SetScope(NetworkView self, ref NetworkPlayer player, bool relevancy);

		public static NetworkView Find(NetworkViewID viewID)
		{
			return NetworkView.INTERNAL_CALL_Find(ref viewID);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkView INTERNAL_CALL_Find(ref NetworkViewID viewID);
	}
}
