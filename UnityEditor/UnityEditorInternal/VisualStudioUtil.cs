using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	internal static class VisualStudioUtil
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanVS2017BuildCppCode();
	}
}
