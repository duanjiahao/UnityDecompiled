using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Networking
{
	internal sealed class GlobalConfigInternal : IDisposable
	{
		internal IntPtr m_Ptr;

		public GlobalConfigInternal(GlobalConfig config)
		{
			this.InitWrapper();
			this.InitThreadAwakeTimeout(config.ThreadAwakeTimeout);
			this.InitReactorModel((byte)config.ReactorModel);
			this.InitReactorMaximumReceivedMessages(config.ReactorMaximumReceivedMessages);
			this.InitReactorMaximumSentMessages(config.ReactorMaximumSentMessages);
			this.InitMaxPacketSize(config.MaxPacketSize);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWrapper();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitThreadAwakeTimeout(uint ms);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorModel(byte model);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorMaximumReceivedMessages(ushort size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorMaximumSentMessages(ushort size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxPacketSize(ushort size);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~GlobalConfigInternal()
		{
			this.Dispose();
		}
	}
}
