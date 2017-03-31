using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor.RestService
{
	internal sealed class Router
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool RegisterHandler(string route, Handler handler);
	}
}
