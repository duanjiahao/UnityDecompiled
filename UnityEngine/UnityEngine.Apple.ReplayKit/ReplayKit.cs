using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine.Apple.ReplayKit
{
	public static class ReplayKit
	{
		public delegate void BroadcastStatusCallback(bool hasStarted, string errorMessage);

		public static extern bool APIAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool recordingAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string lastError
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isRecording
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool broadcastingAPIAvailable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isBroadcasting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string broadcastURL
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool cameraEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool microphoneEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool StartRecording([DefaultValue("false")] bool enableMicrophone, [DefaultValue("false")] bool enableCamera);

		[ExcludeFromDocs]
		public static bool StartRecording(bool enableMicrophone)
		{
			bool enableCamera = false;
			return ReplayKit.StartRecording(enableMicrophone, enableCamera);
		}

		[ExcludeFromDocs]
		public static bool StartRecording()
		{
			bool enableCamera = false;
			bool enableMicrophone = false;
			return ReplayKit.StartRecording(enableMicrophone, enableCamera);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool StopRecording();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Preview();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Discard();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartBroadcasting(ReplayKit.BroadcastStatusCallback callback, [DefaultValue("false")] bool enableMicrophone, [DefaultValue("false")] bool enableCamera);

		[ExcludeFromDocs]
		public static void StartBroadcasting(ReplayKit.BroadcastStatusCallback callback, bool enableMicrophone)
		{
			bool enableCamera = false;
			ReplayKit.StartBroadcasting(callback, enableMicrophone, enableCamera);
		}

		[ExcludeFromDocs]
		public static void StartBroadcasting(ReplayKit.BroadcastStatusCallback callback)
		{
			bool enableCamera = false;
			bool enableMicrophone = false;
			ReplayKit.StartBroadcasting(callback, enableMicrophone, enableCamera);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopBroadcasting();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ShowCameraPreviewAt(float posX, float posY);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void HideCameraPreview();
	}
}
