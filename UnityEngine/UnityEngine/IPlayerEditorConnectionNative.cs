using System;

namespace UnityEngine
{
	internal interface IPlayerEditorConnectionNative
	{
		void Initialize();

		void SendMessage(Guid messageId, byte[] data, int playerId);

		void RegisterInternal(Guid messageId);

		void UnregisterInternal(Guid messageId);

		bool IsConnected();
	}
}
