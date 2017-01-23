using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class AsyncOperation : YieldInstruction
	{
		internal IntPtr m_Ptr;

		public extern bool isDone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float progress
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int priority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool allowSceneActivation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalDestroy();

		~AsyncOperation()
		{
			this.InternalDestroy();
		}
	}
}
