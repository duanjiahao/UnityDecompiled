using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Ping
	{
		internal IntPtr m_Ptr;

		public extern bool isDone
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int time
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string ip
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Ping(string address);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DestroyPing();

		~Ping()
		{
			this.DestroyPing();
		}
	}
}
