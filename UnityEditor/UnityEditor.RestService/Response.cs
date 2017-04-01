using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor.RestService
{
	internal sealed class Response
	{
		private IntPtr m_nativeRequestPtr;

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SimpleResponse(HttpStatusCode status, string payload);
	}
}
