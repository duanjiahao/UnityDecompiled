using System;

namespace UnityEngine
{
	internal class GlobalJavaObjectRef
	{
		private bool m_disposed = false;

		protected IntPtr m_jobject;

		public GlobalJavaObjectRef(IntPtr jobject)
		{
			this.m_jobject = ((!(jobject == IntPtr.Zero)) ? AndroidJNI.NewGlobalRef(jobject) : IntPtr.Zero);
		}

		~GlobalJavaObjectRef()
		{
			this.Dispose();
		}

		public static implicit operator IntPtr(GlobalJavaObjectRef obj)
		{
			return obj.m_jobject;
		}

		public void Dispose()
		{
			if (!this.m_disposed)
			{
				this.m_disposed = true;
				if (this.m_jobject != IntPtr.Zero)
				{
					AndroidJNISafe.DeleteGlobalRef(this.m_jobject);
				}
			}
		}
	}
}
