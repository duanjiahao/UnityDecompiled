using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.VR.WSA.WebCam
{
	public sealed class PhotoCapture : IDisposable
	{
		public enum CaptureResultType
		{
			Success,
			UnknownError
		}

		public struct PhotoCaptureResult
		{
			public PhotoCapture.CaptureResultType resultType;

			public long hResult;

			public bool success
			{
				get
				{
					return this.resultType == PhotoCapture.CaptureResultType.Success;
				}
			}
		}

		public delegate void OnCaptureResourceCreatedCallback(PhotoCapture captureObject);

		public delegate void OnPhotoModeStartedCallback(PhotoCapture.PhotoCaptureResult result);

		public delegate void OnPhotoModeStoppedCallback(PhotoCapture.PhotoCaptureResult result);

		public delegate void OnCapturedToDiskCallback(PhotoCapture.PhotoCaptureResult result);

		public delegate void OnCapturedToMemoryCallback(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);

		private static readonly long HR_SUCCESS = 0L;

		private static Resolution[] s_SupportedResolutions;

		private IntPtr m_NativePtr;

		public static IEnumerable<Resolution> SupportedResolutions
		{
			get
			{
				if (PhotoCapture.s_SupportedResolutions == null)
				{
					PhotoCapture.s_SupportedResolutions = PhotoCapture.GetSupportedResolutions_Internal();
				}
				return PhotoCapture.s_SupportedResolutions;
			}
		}

		private PhotoCapture(IntPtr nativeCaptureObject)
		{
			this.m_NativePtr = nativeCaptureObject;
		}

		private static PhotoCapture.PhotoCaptureResult MakeCaptureResult(PhotoCapture.CaptureResultType resultType, long hResult)
		{
			return new PhotoCapture.PhotoCaptureResult
			{
				resultType = resultType,
				hResult = hResult
			};
		}

		private static PhotoCapture.PhotoCaptureResult MakeCaptureResult(long hResult)
		{
			PhotoCapture.PhotoCaptureResult result = default(PhotoCapture.PhotoCaptureResult);
			PhotoCapture.CaptureResultType resultType;
			if (hResult == PhotoCapture.HR_SUCCESS)
			{
				resultType = PhotoCapture.CaptureResultType.Success;
			}
			else
			{
				resultType = PhotoCapture.CaptureResultType.UnknownError;
			}
			result.resultType = resultType;
			result.hResult = hResult;
			return result;
		}

		public static void CreateAsync(bool showHolograms, PhotoCapture.OnCaptureResourceCreatedCallback onCreatedCallback)
		{
			if (onCreatedCallback == null)
			{
				throw new ArgumentNullException("onCreatedCallback");
			}
			PhotoCapture.Instantiate_Internal(showHolograms, onCreatedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnCreatedResourceDelegate(PhotoCapture.OnCaptureResourceCreatedCallback callback, IntPtr nativePtr)
		{
			if (nativePtr == IntPtr.Zero)
			{
				callback(null);
			}
			else
			{
				callback(new PhotoCapture(nativePtr));
			}
		}

		public void StartPhotoModeAsync(CameraParameters setupParams, PhotoCapture.OnPhotoModeStartedCallback onPhotoModeStartedCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Photo Capture Object before starting its photo mode.");
			}
			if (onPhotoModeStartedCallback == null)
			{
				throw new ArgumentException("onPhotoModeStartedCallback");
			}
			if (setupParams.cameraResolutionWidth == 0 || setupParams.cameraResolutionHeight == 0)
			{
				throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
			}
			this.StartPhotoMode_Internal(this.m_NativePtr, onPhotoModeStartedCallback, setupParams.hologramOpacity, setupParams.frameRate, setupParams.cameraResolutionWidth, setupParams.cameraResolutionHeight, (int)setupParams.pixelFormat);
		}

		[RequiredByNativeCode]
		private static void InvokeOnPhotoModeStartedDelegate(PhotoCapture.OnPhotoModeStartedCallback callback, long hResult)
		{
			callback(PhotoCapture.MakeCaptureResult(hResult));
		}

		public void StopPhotoModeAsync(PhotoCapture.OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Photo Capture Object before stopping its photo mode.");
			}
			if (onPhotoModeStoppedCallback == null)
			{
				throw new ArgumentException("onPhotoModeStoppedCallback");
			}
			this.StopPhotoMode_Internal(this.m_NativePtr, onPhotoModeStoppedCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnPhotoModeStoppedDelegate(PhotoCapture.OnPhotoModeStoppedCallback callback, long hResult)
		{
			callback(PhotoCapture.MakeCaptureResult(hResult));
		}

		public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, PhotoCapture.OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Photo Capture Object before taking a photo.");
			}
			if (onCapturedPhotoToDiskCallback == null)
			{
				throw new ArgumentNullException("onCapturedPhotoToDiskCallback");
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
			this.CapturePhotoToDisk_Internal(this.m_NativePtr, filename, (int)fileOutputFormat, onCapturedPhotoToDiskCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnCapturedPhotoToDiskDelegate(PhotoCapture.OnCapturedToDiskCallback callback, long hResult)
		{
			callback(PhotoCapture.MakeCaptureResult(hResult));
		}

		public void TakePhotoAsync(PhotoCapture.OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
		{
			if (this.m_NativePtr == IntPtr.Zero)
			{
				throw new InvalidOperationException("You must create a Photo Capture Object before taking a photo.");
			}
			if (onCapturedPhotoToMemoryCallback == null)
			{
				throw new ArgumentNullException("onCapturedPhotoToMemoryCallback");
			}
			this.CapturePhotoToMemory_Internal(this.m_NativePtr, onCapturedPhotoToMemoryCallback);
		}

		[RequiredByNativeCode]
		private static void InvokeOnCapturedPhotoToMemoryDelegate(PhotoCapture.OnCapturedToMemoryCallback callback, long hResult, IntPtr photoCaptureFramePtr)
		{
			PhotoCaptureFrame photoCaptureFrame = null;
			if (photoCaptureFramePtr != IntPtr.Zero)
			{
				photoCaptureFrame = new PhotoCaptureFrame(photoCaptureFramePtr);
			}
			callback(PhotoCapture.MakeCaptureResult(hResult), photoCaptureFrame);
		}

		public IntPtr GetUnsafePointerToVideoDeviceController()
		{
			return PhotoCapture.GetUnsafePointerToVideoDeviceController_Internal(this.m_NativePtr);
		}

		public void Dispose()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				PhotoCapture.Dispose_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		~PhotoCapture()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				PhotoCapture.DisposeThreaded_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Resolution[] GetSupportedResolutions_Internal();

		private static IntPtr Instantiate_Internal(bool showHolograms, PhotoCapture.OnCaptureResourceCreatedCallback onCreatedCallback)
		{
			IntPtr result;
			PhotoCapture.INTERNAL_CALL_Instantiate_Internal(showHolograms, onCreatedCallback, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Instantiate_Internal(bool showHolograms, PhotoCapture.OnCaptureResourceCreatedCallback onCreatedCallback, out IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void StartPhotoMode_Internal(IntPtr photoCaptureObj, PhotoCapture.OnPhotoModeStartedCallback onPhotoModeStartedCallback, float hologramOpacity, float frameRate, int cameraResolutionWidth, int cameraResolutionHeight, int pixelFormat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void StopPhotoMode_Internal(IntPtr photoCaptureObj, PhotoCapture.OnPhotoModeStoppedCallback onPhotoModeStoppedCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CapturePhotoToDisk_Internal(IntPtr photoCaptureObj, string filename, int fileOutputFormat, PhotoCapture.OnCapturedToDiskCallback onCapturedPhotoToDiskCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CapturePhotoToMemory_Internal(IntPtr photoCaptureObj, PhotoCapture.OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispose_Internal(IntPtr photoCaptureObj);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeThreaded_Internal(IntPtr photoCaptureObj);

		[ThreadAndSerializationSafe]
		private static IntPtr GetUnsafePointerToVideoDeviceController_Internal(IntPtr photoCaptureObj)
		{
			IntPtr result;
			PhotoCapture.INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(photoCaptureObj, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetUnsafePointerToVideoDeviceController_Internal(IntPtr photoCaptureObj, out IntPtr value);
	}
}
