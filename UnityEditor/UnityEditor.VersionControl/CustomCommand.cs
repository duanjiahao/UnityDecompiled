using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor.VersionControl
{
	internal sealed class CustomCommand
	{
		private IntPtr m_thisDummy;
		public extern string name
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern string label
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern CommandContext context
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Task StartTask();
	}
}
