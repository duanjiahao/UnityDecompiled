using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

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
			this.InitMaxHosts(config.MaxHosts);
			if (config.ThreadPoolSize == 0 || config.ThreadPoolSize > 254)
			{
				throw new ArgumentOutOfRangeException("Worker thread pool size should be >= 1 && < 254 (for server only)");
			}
			this.InitThreadPoolSize(config.ThreadPoolSize);
			this.InitMinTimerTimeout(config.MinTimerTimeout);
			this.InitMaxTimerTimeout(config.MaxTimerTimeout);
			this.InitMinNetSimulatorTimeout(config.MinNetSimulatorTimeout);
			this.InitMaxNetSimulatorTimeout(config.MaxNetSimulatorTimeout);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitWrapper();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitThreadAwakeTimeout(uint ms);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorModel(byte model);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorMaximumReceivedMessages(ushort size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitReactorMaximumSentMessages(ushort size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxPacketSize(ushort size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxHosts(ushort size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitThreadPoolSize(byte size);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMinTimerTimeout(uint ms);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxTimerTimeout(uint ms);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMinNetSimulatorTimeout(uint ms);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InitMaxNetSimulatorTimeout(uint ms);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~GlobalConfigInternal()
		{
			this.Dispose();
		}
	}
}
