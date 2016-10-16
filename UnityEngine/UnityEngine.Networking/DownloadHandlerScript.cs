using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Networking
{
	[StructLayout(LayoutKind.Sequential)]
	public class DownloadHandlerScript : DownloadHandler
	{
		public DownloadHandlerScript()
		{
			base.InternalCreateScript();
		}

		public DownloadHandlerScript(byte[] preallocatedBuffer)
		{
			if (preallocatedBuffer == null || preallocatedBuffer.Length < 1)
			{
				throw new ArgumentException("Cannot create a preallocated-buffer DownloadHandlerScript backed by a null or zero-length array");
			}
			base.InternalCreateScript();
			this.InternalSetPreallocatedBuffer(preallocatedBuffer);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetPreallocatedBuffer(byte[] buffer);
	}
}
