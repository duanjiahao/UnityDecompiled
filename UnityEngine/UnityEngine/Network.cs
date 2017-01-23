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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern NetworkLogLevel logLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern NetworkPlayer[] connections
		{
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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isServer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern NetworkPeerType peerType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float sendRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isMessageQueueRunning
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("No longer needed. This is now explicitly set in the InitializeServer function call. It is implicitly set when calling Connect depending on if an IP/port combination is used (useNat=false) or a GUID is used(useNat=true).")]
		public static extern bool useNat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string natFacilitatorIP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int natFacilitatorPort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string connectionTesterIP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int connectionTesterPort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int maxConnections
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string proxyIP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int proxyPort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool useProxy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern string proxyPassword
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_InitializeServerDeprecated(int connections, int listenPort);

		[Obsolete("Use the IntializeServer(connections, listenPort, useNat) function instead")]
		public static NetworkConnectionError InitializeServer(int connections, int listenPort)
		{
			return Network.Internal_InitializeServerDeprecated(connections, listenPort);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InitializeSecurity();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

		[ExcludeFromDocs]
		private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort)
		{
			string password = "";
			return Network.Internal_ConnectToSingleIP(IP, remotePort, localPort, password);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_ConnectToGuid(string guid, string password);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

		[ExcludeFromDocs]
		private static NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort)
		{
			string password = "";
			return Network.Internal_ConnectToIPs(IP, remotePort, localPort, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string IP, int remotePort)
		{
			string password = "";
			return Network.Connect(IP, remotePort, password);
		}

		public static NetworkConnectionError Connect(string IP, int remotePort, [DefaultValue("\"\"")] string password)
		{
			return Network.Internal_ConnectToSingleIP(IP, remotePort, 0, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string[] IPs, int remotePort)
		{
			string password = "";
			return Network.Connect(IPs, remotePort, password);
		}

		public static NetworkConnectionError Connect(string[] IPs, int remotePort, [DefaultValue("\"\"")] string password)
		{
			return Network.Internal_ConnectToIPs(IPs, remotePort, 0, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string GUID)
		{
			string password = "";
			return Network.Connect(GUID, password);
		}

		public static NetworkConnectionError Connect(string GUID, [DefaultValue("\"\"")] string password)
		{
			return Network.Internal_ConnectToGuid(GUID, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(HostData hostData)
		{
			string password = "";
			return Network.Connect(hostData, password);
		}

		public static NetworkConnectionError Connect(HostData hostData, [DefaultValue("\"\"")] string password)
		{
			if (hostData == null)
			{
				throw new NullReferenceException();
			}
			NetworkConnectionError result;
			if (hostData.guid.Length > 0 && hostData.useNat)
			{
				result = Network.Connect(hostData.guid, password);
			}
			else
			{
				result = Network.Connect(hostData.ip, hostData.port, password);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Disconnect([DefaultValue("200")] int timeout);

		[ExcludeFromDocs]
		public static void Disconnect()
		{
			int timeout = 200;
			Network.Disconnect(timeout);
		}

		public static void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification)
		{
			Network.INTERNAL_CALL_CloseConnection(ref target, sendDisconnectionNotification);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CloseConnection(ref NetworkPlayer target, bool sendDisconnectionNotification);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetPlayer();

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object INTERNAL_CALL_Instantiate(Object prefab, ref Vector3 position, ref Quaternion rotation, int group);

		public static void Destroy(NetworkViewID viewID)
		{
			Network.INTERNAL_CALL_Destroy(ref viewID);
		}

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

		public static void DestroyPlayerObjects(NetworkPlayer playerID)
		{
			Network.INTERNAL_CALL_DestroyPlayerObjects(ref playerID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_DestroyPlayerObjects(ref NetworkPlayer playerID);

		private static void Internal_RemoveRPCs(NetworkPlayer playerID, NetworkViewID viewID, uint channelMask)
		{
			Network.INTERNAL_CALL_Internal_RemoveRPCs(ref playerID, ref viewID, channelMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_RemoveRPCs(ref NetworkPlayer playerID, ref NetworkViewID viewID, uint channelMask);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLevelPrefix(int prefix);

		public static int GetLastPing(NetworkPlayer player)
		{
			return Network.INTERNAL_CALL_GetLastPing(ref player);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetLastPing(ref NetworkPlayer player);

		public static int GetAveragePing(NetworkPlayer player)
		{
			return Network.INTERNAL_CALL_GetAveragePing(ref player);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_GetAveragePing(ref NetworkPlayer player);

		public static void SetReceivingEnabled(NetworkPlayer player, int group, bool enabled)
		{
			Network.INTERNAL_CALL_SetReceivingEnabled(ref player, group, enabled);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetReceivingEnabled(ref NetworkPlayer player, int group, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSendingGlobal(int group, bool enabled);

		private static void Internal_SetSendingSpecific(NetworkPlayer player, int group, bool enabled)
		{
			Network.INTERNAL_CALL_Internal_SetSendingSpecific(ref player, group, enabled);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetSendingSpecific(ref NetworkPlayer player, int group, bool enabled);

		public static void SetSendingEnabled(int group, bool enabled)
		{
			Network.Internal_SetSendingGlobal(group, enabled);
		}

		public static void SetSendingEnabled(NetworkPlayer player, int group, bool enabled)
		{
			Network.Internal_SetSendingSpecific(player, group, enabled);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetTime(out double t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ConnectionTesterStatus TestConnection([DefaultValue("false")] bool forceTest);

		[ExcludeFromDocs]
		public static ConnectionTesterStatus TestConnection()
		{
			bool forceTest = false;
			return Network.TestConnection(forceTest);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ConnectionTesterStatus TestConnectionNAT([DefaultValue("false")] bool forceTest);

		[ExcludeFromDocs]
		public static ConnectionTesterStatus TestConnectionNAT()
		{
			bool forceTest = false;
			return Network.TestConnectionNAT(forceTest);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HavePublicAddress();
	}
}
