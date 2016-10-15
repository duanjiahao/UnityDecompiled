using System;

namespace UnityEngine
{
	internal class AndroidJavaRunnableProxy : AndroidJavaProxy
	{
		private AndroidJavaRunnable mRunnable;

		public AndroidJavaRunnableProxy(AndroidJavaRunnable runnable) : base("java/lang/Runnable")
		{
			this.mRunnable = runnable;
		}

		public void run()
		{
			this.mRunnable();
		}
	}
}
