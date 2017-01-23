using System;
using System.Runtime.CompilerServices;

namespace UnityEditor.RestService
{
	internal sealed class Router
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RegisterHandler(string route, Handler handler);
	}
}
