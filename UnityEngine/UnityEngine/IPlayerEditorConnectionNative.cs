using System;

namespace UnityEngine
{
	internal interface IPlayerEditorConnectionNative
	{
		void Initialize();

		void DisconnectAll();

		void SendMessage(Guid messageId, byte[] data, int playerId);

		void RegisterInternal(Guid messageId);

		void UnregisterInternal(Guid messageId);

		bool IsConnected();
	}
}
