using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal sealed class BootConfigData
	{
		private IntPtr m_Ptr;

		private BootConfigData(IntPtr nativeHandle)
		{
			if (nativeHandle == IntPtr.Zero)
			{
				throw new ArgumentException("native handle can not be null");
			}
			this.m_Ptr = nativeHandle;
		}

		public void AddKey(string key)
		{
			BootConfigData.Append(this.m_Ptr, key, null);
		}

		public void Append(string key, string value)
		{
			BootConfigData.Append(this.m_Ptr, key, value);
		}

		public void Set(string key, string value)
		{
			BootConfigData.Set(this.m_Ptr, key, value);
		}

		private static BootConfigData Wrap(IntPtr nativeHandle)
		{
			return new BootConfigData(nativeHandle);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Append(IntPtr nativeHandle, string key, string val);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Set(IntPtr nativeHandle, string key, string val);
	}
}
