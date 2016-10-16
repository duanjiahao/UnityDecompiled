using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngineInternal;

namespace UnityEngine
{
	public sealed class Network
	{
		public static extern string incomingPassword
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern NetworkLogLevel logLevel
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern NetworkPlayer[] connections
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static NetworkPlayer player
		{
			get
			{
				NetworkPlayer result;
				result.index = Network.Internal_GetPlayer();
				return result;
			}
		}

		public static extern bool isClient
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isServer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern NetworkPeerType peerType
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float sendRate
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isMessageQueueRunning
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static double time
		{
			get
			{
				double result;
				Network.Internal_GetTime(out result);
				return result;
			}
		}

		public static extern int minimumAllocatableViewIDs
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("No longer needed. This is now explicitly set in the InitializeServer function call. It is implicitly set when calling Connect depending on if an IP/port combination is used (useNat=false) or a GUID is used(useNat=true).")]
		public static extern bool useNat
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string natFacilitatorIP
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int natFacilitatorPort
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string connectionTesterIP
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int connectionTesterPort
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int maxConnections
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string proxyIP
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int proxyPort
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useProxy
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string proxyPassword
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_InitializeServerDeprecated(int connections, int listenPort);

		[Obsolete("Use the IntializeServer(connections, listenPort, useNat) function instead")]
		public static NetworkConnectionError InitializeServer(int connections, int listenPort)
		{
			return Network.Internal_InitializeServerDeprecated(connections, listenPort);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InitializeSecurity();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

		[ExcludeFromDocs]
		private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort)
		{
			string empty = string.Empty;
			return Network.Internal_ConnectToSingleIP(IP, remotePort, localPort, empty);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_ConnectToGuid(string guid, string password);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

		[ExcludeFromDocs]
		private static NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort)
		{
			string empty = string.Empty;
			return Network.Internal_ConnectToIPs(IP, remotePort, localPort, empty);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string IP, int remotePort)
		{
			string empty = string.Empty;
			return Network.Connect(IP, remotePort, empty);
		}

		public static NetworkConnectionError Connect(string IP, int remotePort, [DefaultValue("\"\"")] string password)
		{
			return Network.Internal_ConnectToSingleIP(IP, remotePort, 0, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string[] IPs, int remotePort)
		{
			string empty = string.Empty;
			return Network.Connect(IPs, remotePort, empty);
		}

		public static NetworkConnectionError Connect(string[] IPs, int remotePort, [DefaultValue("\"\"")] string password)
		{
			return Network.Internal_ConnectToIPs(IPs, remotePort, 0, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string GUID)
		{
			string empty = string.Empty;
			return Network.Connect(GUID, empty);
		}

		public static NetworkConnectionError Connect(string GUID, [DefaultValue("\"\"")] string password)
		{
			return Network.Internal_ConnectToGuid(GUID, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(HostData hostData)
		{
			string empty = string.Empty;
			return Network.Connect(hostData, empty);
		}

		public static NetworkConnectionError Connect(HostData hostData, [DefaultValue("\"\"")] string password)
		{
			if (hostData == null)
			{
				throw new NullReferenceException();
			}
			if (hostData.guid.Length > 0 && hostData.useNat)
			{
				return Network.Connect(hostData.guid, password);
			}
			return Network.Connect(hostData.ip, hostData.port, password);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Disconnect([DefaultValue("200")] int timeout);

		[ExcludeFromDocs]
		public static void Disconnect()
		{
			int timeout = 200;
			Network.Disconnect(timeout);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPlayer();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_AllocateViewID(out NetworkViewID viewID);

		public static NetworkViewID AllocateViewID()
		{
			NetworkViewID result;
			Network.Internal_AllocateViewID(out result);
			return result;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object prefab, Vector3 position, Quaternion rotation, int group)
		{
			return Network.INTERNAL_CALL_Instantiate(prefab, ref position, ref rotation, group);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object INTERNAL_CALL_Instantiate(Object prefab, ref Vector3 position, ref Quaternion rotation, int group);

		public static void Destroy(NetworkViewID viewID)
		{
			Network.INTERNAL_CALL_Destroy(ref viewID);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Destroy(ref NetworkViewID viewID);

		public static void Destroy(GameObject gameObject)
		{
			if (gameObject != null)
			{
				NetworkView component = gameObject.GetComponent<NetworkView>();
				if (component != null)
				{
					Network.Destroy(component.viewID);
				}
				else
				{
					Debug.LogError("Couldn't destroy game object because no network view is attached to it.", gameObject);
				}
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyPlayerObjects(NetworkPlayer playerID);

		private static void Internal_RemoveRPCs(NetworkPlayer playerID, NetworkViewID viewID, uint channelMask)
		{
			Network.INTERNAL_CALL_Internal_RemoveRPCs(playerID, ref viewID, channelMask);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_RemoveRPCs(NetworkPlayer playerID, ref NetworkViewID viewID, uint channelMask);

		public static void RemoveRPCs(NetworkPlayer playerID)
		{
			Network.Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, 4294967295u);
		}

		public static void RemoveRPCs(NetworkPlayer playerID, int group)
		{
			Network.Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, 1u << group);
		}

		public static void RemoveRPCs(NetworkViewID viewID)
		{
			Network.Internal_RemoveRPCs(NetworkPlayer.unassigned, viewID, 4294967295u);
		}

		public static void RemoveRPCsInGroup(int group)
		{
			Network.Internal_RemoveRPCs(NetworkPlayer.unassigned, NetworkViewID.unassigned, 1u << group);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLevelPrefix(int prefix);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLastPing(NetworkPlayer player);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAveragePing(NetworkPlayer player);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetReceivingEnabled(NetworkPlayer player, int group, bool enabled);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSendingGlobal(int group, bool enabled);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSendingSpecific(NetworkPlayer player, int group, bool enabled);

		public static void SetSendingEnabled(int group, bool enabled)
		{
			Network.Internal_SetSendingGlobal(group, enabled);
		}

		public static void SetSendingEnabled(NetworkPlayer player, int group, bool enabled)
		{
			Network.Internal_SetSendingSpecific(player, group, enabled);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTime(out double t);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ConnectionTesterStatus TestConnection([DefaultValue("false")] bool forceTest);

		[ExcludeFromDocs]
		public static ConnectionTesterStatus TestConnection()
		{
			bool forceTest = false;
			return Network.TestConnection(forceTest);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ConnectionTesterStatus TestConnectionNAT([DefaultValue("false")] bool forceTest);

		[ExcludeFromDocs]
		public static ConnectionTesterStatus TestConnectionNAT()
		{
			bool forceTest = false;
			return Network.TestConnectionNAT(forceTest);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HavePublicAddress();
	}
}
