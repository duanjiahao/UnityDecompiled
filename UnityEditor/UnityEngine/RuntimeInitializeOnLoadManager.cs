using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	internal sealed class RuntimeInitializeOnLoadManager
	{
		internal static extern string[] dontStripClassNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern RuntimeInitializeMethodInfo[] methodInfos
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void UpdateMethodExecutionOrders(int[] changedIndices, int[] changedOrder);
	}
}
