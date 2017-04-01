using System;
using UnityEngine.Events;

namespace UnityEngine.Networking.PlayerConnection
{
	internal interface IEditorPlayerConnection
	{
		void Register(Guid messageId, UnityAction<MessageEventArgs> callback);

		void Unregister(Guid messageId, UnityAction<MessageEventArgs> callback);

		void RegisterConnection(UnityAction<int> callback);

		void RegisterDisconnection(UnityAction<int> callback);

		void Send(Guid messageId, byte[] data);
	}
}
