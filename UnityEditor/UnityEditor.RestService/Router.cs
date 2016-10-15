using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.RestService
{
	internal sealed class Router
	{
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RegisterHandler(string route, Handler handler);
	}
}
