using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.RestService
{
	internal sealed class Response
	{
		private IntPtr m_nativeRequestPtr;

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SimpleResponse(HttpStatusCode status, string payload);
	}
}
