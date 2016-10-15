using System;

namespace UnityEditor.Modules
{
	internal interface IDevice
	{
		RemoteAddress StartRemoteSupport();

		void StopRemoteSupport();

		RemoteAddress StartPlayerConnectionSupport();

		void StopPlayerConnectionSupport();
	}
}
