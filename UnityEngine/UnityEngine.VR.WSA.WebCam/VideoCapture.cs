using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.WebCam
{
	public sealed class VideoCapture : IDisposable
	{
		public enum CaptureResultType
		{
			Success,
			UnknownError
		}

		public enum AudioState
		{
			MicAudio,
			ApplicationAudio,
			ApplicationAndMicAudio,
			None
		}

		public struct VideoCaptureResult
		{
			public VideoCapture.CaptureResultType resultType;

			public long hResult;

			public bool success
			{
				get
				{
					return this.resultType == VideoCapture.CaptureResultType.Success;
				}
			}
		}

		public delegate void OnVideoCaptureResourceCreatedCallback(VideoCapture captureObject);

		public delegate void OnVideoModeStartedCallback(VideoCapture.VideoCaptureResult result);

		public delegate void OnVideoModeStoppedCallback(VideoCapture.VideoCaptureResult result);

		public delegate void OnStartedRecordingVideoCallback(VideoCapture.VideoCaptureResult result);

		public delegate void OnStoppedRecordingVideoCallback(VideoCapture.VideoCaptureResult result);

		private static readonly long HR_SUCCESS = 0L;

		private static Resolution[] s_SupportedResolutions;

		private IntPtr m_NativePtr;

		public static IEnumerable<Resolution> SupportedResolutions
		{
			get
			{
				if (VideoCapture.s_SupportedResolutions == null)
				{
					VideoCapture.s_SupportedResolutions = VideoCapture.GetSupportedResolutions_Internal();
				}
				return VideoCapture.s_SupportedResolutions;
			}
		}

		public bool IsRecording
		{
			get
			{
				if (this.m_NativePtr == IntPtr.Zero)
				{
					throw new InvalidOperationException("You must create a Video Capture Object before using it.");
				}
				return this.IsRecording_Internal(this.m_NativePtr);
			}
		}

		private VideoCapture(IntPtr nativeCaptureObject)
		{
			this.m_NativePtr = nativeCaptureObject;
		}

		private static VideoCapture.VideoCaptureResult MakeCaptureResult(VideoCapture.CaptureResultType resultType, long hResult)
		{
			return new VideoCapture.VideoCaptureResult
			{
				resultType = resultType,
				hResult = hResult
			};
		}

		private static VideoCapture.VideoCaptureResult MakeCaptureResult(long hResult)
		{
			VideoCapture.VideoCaptureResult result = default(VideoCapture.VideoCaptureResult);
			VideoCapture.CaptureResultType resultType;
			if (hResult == VideoCapture.HR_SUCCESS)
			{
				resultType = VideoCapture.CaptureResultType.Success;
			}
			else
			{
				resultType = VideoCapture.CaptureResultType.UnknownError;
			}
			result.resultType = resultType;
			result.hResult = hResult;
			return result;
		}

		public static IEnumerable<float> GetSupportedFrameRatesForResolution(Resolution resolution)
		{
			return VideoCapture.GetSupportedFrameRatesForResolution_Internal(resolution.width, resolution.height);
		}

		public static void CreateAsync(bool showHolograms, VideoCapture.OnVideoCaptureResourceCreatedCallback onCreatedCallback)
		{
			if (onCreatedCallback == null)
			{
				throw new ArgumentNullException("onCreatedCallback");
			}
			VideoCapture.Instantiate_Internal(showHolograms, onCreatedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnCreatedVideoCaptureResourceDelegate(VideoCapture.OnVideoCaptureResourceCreatedCallback callback, IntPtr nativePtr)
		{
			if (nativePtr == IntPtr.Zero)
			{
				callback(null);
			}
			else
			{
				callback(new VideoCapture(nativePtr));
			}
		}

		public void StartVideoModeAsync(CameraParameters setupParams, VideoCapture.AudioState audioState, VideoCapture.OnVideoModeStartedCallback onVideoModeStartedCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Video Capture Object before starting its video mode.");
			}
			if (onVideoModeStartedCallback == null)
			{
				throw new ArgumentNullException("onVideoModeStartedCallback");
			}
			if (setupParams.cameraResolutionWidth == 0 || setupParams.cameraResolutionHeight == 0)
			{
				throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
			}
			if (setupParams.frameRate == 0f)
			{
				throw new ArgumentOutOfRangeException("setupParams", "The camera frame rate must be set to a supported recording frame rate.");
			}
			this.StartVideoMode_Internal(this.m_NativePtr, (int)audioState, onVideoModeStartedCallback, setupParams.hologramOpacity, setupParams.frameRate, setupParams.cameraResolutionWidth, setupParams.cameraResolutionHeight, (int)setupParams.pixelFormat);
		}

		[RequiredByNativeCode]
		private static void InvokeOnVideoModeStartedDelegate(VideoCapture.OnVideoModeStartedCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		public void StopVideoModeAsync(VideoCapture.OnVideoModeStoppedCallback onVideoModeStoppedCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Video Capture Object before stopping its video mode.");
			}
			if (onVideoModeStoppedCallback == null)
			{
				throw new ArgumentNullException("onVideoModeStoppedCallback");
			}
			this.StopVideoMode_Internal(this.m_NativePtr, onVideoModeStoppedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnVideoModeStoppedDelegate(VideoCapture.OnVideoModeStoppedCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		public void StartRecordingAsync(string filename, VideoCapture.OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Video Capture Object before recording video.");
			}
			if (onStartedRecordingVideoCallback == null)
			{
				throw new ArgumentNullException("onStartedRecordingVideoCallback");
			}
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentNullException("filename");
			}
			filename = filename.Replace("/", "\\");
			string directoryName = Path.GetDirectoryName(filename);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				throw new ArgumentException("The specified directory does not exist.", "filename");
			}
			this.StartRecordingVideoToDisk_Internal(this.m_NativePtr, filename, onStartedRecordingVideoCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnStartedRecordingVideoToDiskDelegate(VideoCapture.OnStartedRecordingVideoCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		public void StopRecordingAsync(VideoCapture.OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Video Capture Object before recording video.");
			}
			if (onStoppedRecordingVideoCallback == null)
			{
				throw new ArgumentNullException("onStoppedRecordingVideoCallback");
			}
			this.StopRecordingVideoToDisk_Internal(this.m_NativePtr, onStoppedRecordingVideoCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnStoppedRecordingVideoToDiskDelegate(VideoCapture.OnStoppedRecordingVideoCallback callback, long hResult)
		{
			callback(VideoCapture.MakeCaptureResult(hResult));
		}

		public IntPtr GetUnsafePointerToVideoDeviceController()
		{
			return VideoCapture.GetUnsafePointerToVideoDeviceController_Internal(this.m_NativePtr);
		}

		public void Dispose()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				VideoCapture.Dispose_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		~VideoCapture()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				VideoCapture.DisposeThreaded_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Resolution[] GetSupportedResolutions_Internal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float[] GetSupportedFrameRatesForResolution_Internal(int resolutionWidth, int resolutionHeight);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsRecording_Internal(IntPtr videoCaptureObj);

		private static IntPtr Instantiate_Internal(bool showHolograms, VideoCapture.OnVideoCaptureResourceCreatedCallback onCreatedCallback)
		{
			IntPtr result;
			VideoCapture.INTERNAL_CALL_Instantiate_Internal(showHolograms, onCreatedCallback, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Instantiate_Internal(bool showHolograms, VideoCapture.OnVideoCaptureResourceCreatedCallback onCreatedCallback, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void StartVideoMode_Internal(IntPtr videoCaptureObj, int audioState, VideoCapture.OnVideoModeStartedCallback onVideoModeStartedCallback, float hologramOpacity, float frameRate, int cameraResolutionWidth, int cameraResolutionHeight, int pixelFormat);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void StopVideoMode_Internal(IntPtr videoCaptureObj, VideoCapture.OnVideoModeStoppedCallback onVideoModeStoppedCallback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void StartRecordingVideoToDisk_Internal(IntPtr videoCaptureObj, string filename, VideoCapture.OnStartedRecordingVideoCallback onStartedRecordingVideoCallback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void StopRecordingVideoToDisk_Internal(IntPtr videoCaptureObj, VideoCapture.OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispose_Internal(IntPtr videoCaptureObj);

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeThreaded_Internal(IntPtr videoCaptureObj);

		[ThreadAndSerializationSafe]
		private static IntPtr GetUnsafePointerToVideoDeviceController_Internal(IntPtr videoCaptureObj)
		{
			IntPtr result;
			VideoCapture.INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(videoCaptureObj, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(IntPtr videoCaptureObj, out IntPtr value);
	}
}
