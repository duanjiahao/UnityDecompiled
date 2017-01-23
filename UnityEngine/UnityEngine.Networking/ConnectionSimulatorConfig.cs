using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Networking
{
	public sealed class ConnectionSimulatorConfig : IDisposable
	{
		internal IntPtr m_Ptr;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ConnectionSimulatorConfig(int outMinDelay, int outAvgDelay, int inMinDelay, int inAvgDelay, float packetLossPercentage);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~ConnectionSimulatorConfig()
		{
			this.Dispose();
		}
	}
}
