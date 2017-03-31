using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	internal static class VisualStudioUtil
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanVS2017BuildCppCode();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] FindVisualStudioDevEnvPaths(int visualStudioVersion, string requiredWorkload);
	}
}
