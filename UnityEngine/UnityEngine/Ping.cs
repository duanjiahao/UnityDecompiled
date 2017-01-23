using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Ping
	{
		internal IntPtr m_Ptr;

		public extern bool isDone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int time
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string ip
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Ping(string address);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DestroyPing();

		~Ping()
		{
			this.DestroyPing();
		}
	}
}
