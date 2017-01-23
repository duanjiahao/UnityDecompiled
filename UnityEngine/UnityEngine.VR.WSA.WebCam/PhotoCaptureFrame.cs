using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.VR.WSA.WebCam
{
	public sealed class PhotoCaptureFrame : IDisposable
	{
		private IntPtr m_NativePtr;

		public int dataLength
		{
			get;
			private set;
		}

		public bool hasLocationData
		{
			get;
			private set;
		}

		public CapturePixelFormat pixelFormat
		{
			get;
			private set;
		}

		internal PhotoCaptureFrame(IntPtr nativePtr)
		{
			this.m_NativePtr = nativePtr;
			this.dataLength = PhotoCaptureFrame.GetDataLength(nativePtr);
			this.hasLocationData = PhotoCaptureFrame.GetHasLocationData(nativePtr);
			this.pixelFormat = PhotoCaptureFrame.GetCapturePixelFormat(nativePtr);
			GC.AddMemoryPressure((long)this.dataLength);
		}

		public bool TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix)
		{
			cameraToWorldMatrix = Matrix4x4.identity;
			bool result;
			if (this.hasLocationData)
			{
				cameraToWorldMatrix = PhotoCaptureFrame.GetCameraToWorldMatrix(this.m_NativePtr);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
		{
			bool result;
			if (this.hasLocationData)
			{
				projectionMatrix = PhotoCaptureFrame.GetProjection(this.m_NativePtr);
				result = true;
			}
			else
			{
				projectionMatrix = Matrix4x4.identity;
				result = false;
			}
			return result;
		}

		public bool TryGetProjectionMatrix(float nearClipPlane, float farClipPlane, out Matrix4x4 projectionMatrix)
		{
			bool result;
			if (this.hasLocationData)
			{
				float num = 0.01f;
				if (nearClipPlane < num)
				{
					nearClipPlane = num;
				}
				if (farClipPlane < nearClipPlane + num)
				{
					farClipPlane = nearClipPlane + num;
				}
				projectionMatrix = PhotoCaptureFrame.GetProjection(this.m_NativePtr);
				float num2 = 1f / (farClipPlane - nearClipPlane);
				float m = -(farClipPlane + nearClipPlane) * num2;
				float m2 = -(2f * farClipPlane * nearClipPlane) * num2;
				projectionMatrix.m22 = m;
				projectionMatrix.m23 = m2;
				result = true;
			}
			else
			{
				projectionMatrix = Matrix4x4.identity;
				result = false;
			}
			return result;
		}

		public void UploadImageDataToTexture(Texture2D targetTexture)
		{
			if (targetTexture == null)
			{
				throw new ArgumentNullException("targetTexture");
			}
			if (this.pixelFormat != CapturePixelFormat.BGRA32)
			{
				throw new ArgumentException("Uploading PhotoCaptureFrame to a texture is only supported with BGRA32 CameraFrameFormat!");
			}
			PhotoCaptureFrame.UploadImageDataToTexture_Internal(this.m_NativePtr, targetTexture);
		}

		public IntPtr GetUnsafePointerToBuffer()
		{
			return PhotoCaptureFrame.GetUnsafePointerToBuffer(this.m_NativePtr);
		}

		public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
		{
			if (byteBuffer == null)
			{
				throw new ArgumentNullException("byteBuffer");
			}
			byte[] array = PhotoCaptureFrame.CopyRawImageDataIntoBuffer_Internal(this.m_NativePtr);
			int num = array.Length;
			if (byteBuffer.Capacity < num)
			{
				byteBuffer.Capacity = num;
			}
			byteBuffer.Clear();
			byteBuffer.AddRange(array);
		}

		private void Cleanup()
		{
			if (this.m_NativePtr != IntPtr.Zero)
			{
				GC.RemoveMemoryPressure((long)this.dataLength);
				PhotoCaptureFrame.Dispose_Internal(this.m_NativePtr);
				this.m_NativePtr = IntPtr.Zero;
			}
		}

		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		~PhotoCaptureFrame()
		{
			this.Cleanup();
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CapturePixelFormat GetCapturePixelFormat(IntPtr photoCaptureFrame);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHasLocationData(IntPtr photoCaptureFrame);

		[ThreadAndSerializationSafe]
		private static Matrix4x4 GetCameraToWorldMatrix(IntPtr photoCaptureFrame)
		{
			Matrix4x4 result;
			PhotoCaptureFrame.INTERNAL_CALL_GetCameraToWorldMatrix(photoCaptureFrame, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetCameraToWorldMatrix(IntPtr photoCaptureFrame, out Matrix4x4 value);

		[ThreadAndSerializationSafe]
		private static Matrix4x4 GetProjection(IntPtr photoCaptureFrame)
		{
			Matrix4x4 result;
			PhotoCaptureFrame.INTERNAL_CALL_GetProjection(photoCaptureFrame, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetProjection(IntPtr photoCaptureFrame, out Matrix4x4 value);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDataLength(IntPtr photoCaptureFrame);

		[ThreadAndSerializationSafe]
		private static IntPtr GetUnsafePointerToBuffer(IntPtr photoCaptureFrame)
		{
			IntPtr result;
			PhotoCaptureFrame.INTERNAL_CALL_GetUnsafePointerToBuffer(photoCaptureFrame, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetUnsafePointerToBuffer(IntPtr photoCaptureFrame, out IntPtr value);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetData_Internal(IntPtr photoCaptureFrame, IntPtr targetBuffer);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern byte[] CopyRawImageDataIntoBuffer_Internal(IntPtr photoCaptureFrame);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UploadImageDataToTexture_Internal(IntPtr photoCaptureFrame, Texture2D targetTexture);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Dispose_Internal(IntPtr photoCaptureFrame);
	}
}
