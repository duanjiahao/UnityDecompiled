using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class NetworkView : Behaviour
	{
		public extern Component observed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern NetworkStateSynchronization stateSynchronization
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RPC(NetworkView view, string name, RPCMode mode, object[] args);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RPC_Target(NetworkView view, string name, NetworkPlayer target, object[] args);

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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_GetViewID(out NetworkViewID viewID);

		private void Internal_SetViewID(NetworkViewID viewID)
		{
			NetworkView.INTERNAL_CALL_Internal_SetViewID(this, ref viewID);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetViewID(NetworkView self, ref NetworkViewID viewID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetScope(NetworkPlayer player, bool relevancy);

		public static NetworkView Find(NetworkViewID viewID)
		{
			return NetworkView.INTERNAL_CALL_Find(ref viewID);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkView INTERNAL_CALL_Find(ref NetworkViewID viewID);
	}
}
