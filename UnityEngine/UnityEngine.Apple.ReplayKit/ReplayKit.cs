using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine.Apple.ReplayKit
{
	public static class ReplayKit
	{
		public static extern bool APIAvailable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool recordingAvailable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string lastError
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isRecording
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool StartRecording([DefaultValue("false")] bool enableMicrophone);

		[ExcludeFromDocs]
		public static bool StartRecording()
		{
			bool enableMicrophone = false;
			return ReplayKit.StartRecording(enableMicrophone);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool StopRecording();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Preview();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Discard();
	}
}
