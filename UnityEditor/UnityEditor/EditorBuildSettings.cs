using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class EditorBuildSettings
	{
		public static extern EditorBuildSettingsScene[] scenes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
