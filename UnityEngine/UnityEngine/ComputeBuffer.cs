using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace UnityEngine
{
	public sealed class ComputeBuffer : IDisposable
	{
		internal IntPtr m_Ptr;

		public extern int count
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int stride
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public ComputeBuffer(int count, int stride) : this(count, stride, ComputeBufferType.Default)
		{
		}

		public ComputeBuffer(int count, int stride, ComputeBufferType type)
		{
			if (count <= 0)
			{
				throw new ArgumentException("Attempting to create a zero length compute buffer", "count");
			}
			if (stride < 0)
			{
				throw new ArgumentException("Attempting to create a compute buffer with a negative stride", "stride");
			}
			this.m_Ptr = IntPtr.Zero;
			ComputeBuffer.InitBuffer(this, count, stride, type);
		}

		~ComputeBuffer()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			ComputeBuffer.DestroyBuffer(this);
			this.m_Ptr = IntPtr.Zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitBuffer(ComputeBuffer buf, int count, int stride, ComputeBufferType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyBuffer(ComputeBuffer buf);

		public void Release()
		{
			this.Dispose();
		}

		[SecuritySafeCritical]
		public void SetData(Array data)
		{
			this.InternalSetData(data, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetData(Array data, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCounterValue(uint counterValue);

		[SecuritySafeCritical]
		public void GetData(Array data)
		{
			this.InternalGetData(data, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalGetData(Array data, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffset);

		public IntPtr GetNativeBufferPtr()
		{
			IntPtr result;
			ComputeBuffer.INTERNAL_CALL_GetNativeBufferPtr(this, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeBufferPtr(ComputeBuffer self, out IntPtr value);
	}
}
