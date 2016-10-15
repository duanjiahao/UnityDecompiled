using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal sealed class RuntimeInitializeOnLoadManager
	{
		internal static extern string[] dontStripClassNames
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern RuntimeInitializeMethodInfo[] methodInfos
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateMethodExecutionOrders(int[] changedIndices, int[] changedOrder);
	}
}
