using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.RestService
{
	internal sealed class Request
	{
		private IntPtr m_nativeRequestPtr;

		public extern string Payload
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string Url
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int MessageType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int Depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool Info
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetParam(string paramName);
	}
}
