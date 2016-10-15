using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal sealed class Response
	{
		private IntPtr m_nativeRequestPtr;

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SimpleResponse(HttpStatusCode status, string payload);
	}
}
