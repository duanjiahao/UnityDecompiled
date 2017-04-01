using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnityEngine.Collections
{
	[StructLayout(LayoutKind.Sequential)]
	internal class DisposeSentinel
	{
		public delegate void DeallocateDelegate(IntPtr buffer, Allocator allocator);

		private IntPtr m_Ptr;

		private DisposeSentinel.DeallocateDelegate m_DeallocateDelegate;

		private Allocator m_Label;

		private string m_FileName;

		private int m_LineNumber;

		public static DisposeSentinel Create(IntPtr ptr, Allocator label, int callSiteStackDepth, DisposeSentinel.DeallocateDelegate deallocateDelegate = null)
		{
			DisposeSentinel result;
			if (NativeLeakDetection.Mode == NativeLeakDetectionMode.Enabled)
			{
				DisposeSentinel disposeSentinel = new DisposeSentinel();
				StackFrame stackFrame = new StackFrame(callSiteStackDepth + 2, true);
				disposeSentinel.m_FileName = stackFrame.GetFileName();
				disposeSentinel.m_LineNumber = stackFrame.GetFileLineNumber();
				disposeSentinel.m_Ptr = ptr;
				disposeSentinel.m_Label = label;
				disposeSentinel.m_DeallocateDelegate = deallocateDelegate;
				result = disposeSentinel;
			}
			else
			{
				result = null;
			}
			return result;
		}

		protected override void Finalize()
		{
			try
			{
				if (this.m_Ptr != IntPtr.Zero)
				{
					string msg = string.Format("A NativeArray created with Allocator.Persistent has not been disposed, resulting in a memory leak. It was allocated at {0}:{1}.", this.m_FileName, this.m_LineNumber);
					UnsafeUtility.LogError(msg, this.m_FileName, this.m_LineNumber);
					if (this.m_DeallocateDelegate != null)
					{
						this.m_DeallocateDelegate(this.m_Ptr, this.m_Label);
					}
					else
					{
						UnsafeUtility.Free(this.m_Ptr, this.m_Label);
					}
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public static void Clear(ref DisposeSentinel sentinel)
		{
			if (sentinel != null)
			{
				sentinel.m_Ptr = IntPtr.Zero;
				sentinel = null;
			}
		}
	}
}
