using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Scripting;

namespace UnityEditor.Networking.PlayerConnection
{
	[Serializable]
	public class EditorConnection : ScriptableSingleton<EditorConnection>, IEditorPlayerConnection
	{
		internal static IPlayerEditorConnectionNative connectionNative;

		[SerializeField]
		private PlayerEditorConnectionEvents m_PlayerEditorConnectionEvents = new PlayerEditorConnectionEvents();

		[SerializeField]
		private List<int> m_connectedPlayers = new List<int>();

		public List<ConnectedPlayer> ConnectedPlayers
		{
			get
			{
				return (from x in this.m_connectedPlayers
				select new ConnectedPlayer(x)).ToList<ConnectedPlayer>();
			}
		}

		public void Initialize()
		{
			this.GetEditorConnectionNativeApi().Initialize();
		}

		private void Cleanup()
		{
			this.UnregisterAllPersistedListeners(this.m_PlayerEditorConnectionEvents.connectionEvent);
			this.UnregisterAllPersistedListeners(this.m_PlayerEditorConnectionEvents.disconnectionEvent);
			this.m_PlayerEditorConnectionEvents.messageTypeSubscribers.Clear();
		}

		private void UnregisterAllPersistedListeners(UnityEventBase connectionEvent)
		{
			int persistentEventCount = connectionEvent.GetPersistentEventCount();
			for (int i = 0; i < persistentEventCount; i++)
			{
				connectionEvent.UnregisterPersistentListener(i);
			}
		}

		private IPlayerEditorConnectionNative GetEditorConnectionNativeApi()
		{
			return EditorConnection.connectionNative ?? new EditorConnectionInternal();
		}

		public void Register(Guid messageId, UnityAction<MessageEventArgs> callback)
		{
			if (messageId == Guid.Empty)
			{
				throw new ArgumentException("Cant be Guid.Empty", "messageId");
			}
			if (!this.m_PlayerEditorConnectionEvents.messageTypeSubscribers.Any((PlayerEditorConnectionEvents.MessageTypeSubscribers x) => x.MessageTypeId == messageId))
			{
				this.GetEditorConnectionNativeApi().RegisterInternal(messageId);
			}
			this.m_PlayerEditorConnectionEvents.AddAndCreate(messageId).AddPersistentListener(callback, UnityEventCallState.EditorAndRuntime);
		}

		public void Unregister(Guid messageId, UnityAction<MessageEventArgs> callback)
		{
			this.m_PlayerEditorConnectionEvents.UnregisterManagedCallback(messageId, callback);
			if (!this.m_PlayerEditorConnectionEvents.messageTypeSubscribers.Any((PlayerEditorConnectionEvents.MessageTypeSubscribers x) => x.MessageTypeId == messageId))
			{
				this.GetEditorConnectionNativeApi().UnregisterInternal(messageId);
			}
		}

		public void RegisterConnection(UnityAction<int> callback)
		{
			foreach (int current in this.m_connectedPlayers)
			{
				callback(current);
			}
			this.m_PlayerEditorConnectionEvents.connectionEvent.AddPersistentListener(callback, UnityEventCallState.EditorAndRuntime);
		}

		public void RegisterDisconnection(UnityAction<int> callback)
		{
			this.m_PlayerEditorConnectionEvents.disconnectionEvent.AddPersistentListener(callback, UnityEventCallState.EditorAndRuntime);
		}

		public void Send(Guid messageId, byte[] data, int playerId)
		{
			if (messageId == Guid.Empty)
			{
				throw new ArgumentException("Cant be Guid.Empty", "messageId");
			}
			this.GetEditorConnectionNativeApi().SendMessage(messageId, data, playerId);
		}

		public void Send(Guid messageId, byte[] data)
		{
			this.Send(messageId, data, 0);
		}

		[RequiredByNativeCode]
		private static void MessageCallbackInternal(IntPtr data, ulong size, ulong guid, string messageId)
		{
			byte[] array = null;
			if (size > 0uL)
			{
				array = new byte[size];
				Marshal.Copy(data, array, 0, (int)size);
			}
			ScriptableSingleton<EditorConnection>.instance.m_PlayerEditorConnectionEvents.InvokeMessageIdSubscribers(new Guid(messageId), array, (int)guid);
		}

		[RequiredByNativeCode]
		private static void ConnectedCallbackInternal(int playerId)
		{
			ScriptableSingleton<EditorConnection>.instance.m_connectedPlayers.Add(playerId);
			ScriptableSingleton<EditorConnection>.instance.m_PlayerEditorConnectionEvents.connectionEvent.Invoke(playerId);
		}

		[RequiredByNativeCode]
		private static void DisconnectedCallback(int playerId)
		{
			ScriptableSingleton<EditorConnection>.instance.m_connectedPlayers.Remove(playerId);
			ScriptableSingleton<EditorConnection>.instance.m_PlayerEditorConnectionEvents.disconnectionEvent.Invoke(playerId);
			if (!ScriptableSingleton<EditorConnection>.instance.ConnectedPlayers.Any<ConnectedPlayer>())
			{
				ScriptableSingleton<EditorConnection>.instance.Cleanup();
			}
		}
	}
}
