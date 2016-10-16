using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor
{
	[StructLayout(LayoutKind.Sequential)]
	internal class EditorMetricEvent : IDisposable
	{
		internal IntPtr m_Ptr;

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern EditorMetricEvent(EditorMetricCollectionType en);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddValueStr(string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddValueInt(string key, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddChildValueBool(string parent, string key, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddChildValueInt(string parent, string key, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddChildValueStr(string parent, string key, string value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Send();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Destroy();

		public void Dispose()
		{
			this.Destroy();
			this.m_Ptr = IntPtr.Zero;
		}

		~EditorMetricEvent()
		{
			this.Dispose();
		}

		internal void AddValue(string key, string value)
		{
			this.AddValueStr(key, value);
		}

		internal void AddValue(string key, int value)
		{
			this.AddValueInt(key, value);
		}

		internal void AddChildValue(string parent, string key, int value)
		{
			this.AddChildValueInt(parent, key, value);
		}

		internal void AddChildValue(string parent, string key, string value)
		{
			this.AddChildValueStr(parent, key, value);
		}

		internal void AddChildValue(string parent, string key, bool value)
		{
			this.AddChildValueBool(parent, key, value);
		}
	}
}
