using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditorInternal
{
	public sealed class ScriptUpdatingManager
	{
		public static extern int numberOfTimesAsked
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool WaitForVCSServerConnection(bool reportTimeout);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReportExpectedUpdateFailure();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReportGroupedAPIUpdaterFailure(string msg);
	}
}
