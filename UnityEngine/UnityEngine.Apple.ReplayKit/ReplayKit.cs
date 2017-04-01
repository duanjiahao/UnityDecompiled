using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Apple.ReplayKit
{
	public static class ReplayKit
	{
		public delegate void BroadcastStatusCallback(bool hasStarted, string errorMessage);

		public static extern bool APIAvailable
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool recordingAvailable
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string lastError
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isRecording
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool broadcastingAPIAvailable
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isBroadcasting
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string broadcastURL
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool cameraEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool microphoneEnabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool StopRecording();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Preview();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Discard();

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopBroadcasting();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool ShowCameraPreviewAt(float posX, float posY);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void HideCameraPreview();
	}
}
