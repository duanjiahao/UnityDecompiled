using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor.VersionControl
{
	public sealed class Message
	{
		[Flags]
		public enum Severity
		{
			Data = 0,
			Verbose = 1,
			Info = 2,
			Warning = 3,
			Error = 4
		}
		private IntPtr m_thisDummy;
		public extern Message.Severity severity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern string message
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();
		~Message()
		{
			this.Dispose();
		}
		public void Show()
		{
			Message.Info(this.message);
		}
		private static void Info(string message)
		{
			Debug.Log("Version control:\n" + message);
		}
	}
}
