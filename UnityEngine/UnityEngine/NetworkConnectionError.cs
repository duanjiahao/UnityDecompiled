using System;

namespace UnityEngine
{
	public enum NetworkConnectionError
	{
		NoError,
		RSAPublicKeyMismatch = 21,
		InvalidPassword = 23,
		ConnectionFailed = 15,
		TooManyConnectedPlayers = 18,
		ConnectionBanned = 22,
		AlreadyConnectedToServer = 16,
		AlreadyConnectedToAnotherServer = -1,
		CreateSocketOrThreadFailure = -2,
		IncorrectParameters = -3,
		EmptyConnectTarget = -4,
		InternalDirectConnectFailed = -5,
		NATTargetNotConnected = 69,
		NATTargetConnectionLost = 71,
		NATPunchthroughFailed = 73
	}
}
