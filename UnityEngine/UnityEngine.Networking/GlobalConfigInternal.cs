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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWrapper();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitThreadAwakeTimeout(uint ms);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorModel(byte model);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorMaximumReceivedMessages(ushort size);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorMaximumSentMessages(ushort size);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxPacketSize(ushort size);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~GlobalConfigInternal()
		{
			this.Dispose();
		}
	}
}
