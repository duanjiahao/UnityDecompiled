using System;

namespace UnityEngine
{
	public class AndroidJavaClass : AndroidJavaObject
	{
		public AndroidJavaClass(string className)
		{
			this._AndroidJavaClass(className);
		}

		internal AndroidJavaClass(IntPtr jclass)
		{
			if (jclass == IntPtr.Zero)
			{
				throw new Exception("JNI: Init'd AndroidJavaClass with null ptr!");
			}
			this.m_jclass = AndroidJNI.NewGlobalRef(jclass);
			this.m_jobject = IntPtr.Zero;
		}

		private void _AndroidJavaClass(string className)
		{
			base.DebugPrint("Creating AndroidJavaClass from " + className);
			using (AndroidJavaObject androidJavaObject = AndroidJavaObject.FindClass(className))
			{
				this.m_jclass = AndroidJNI.NewGlobalRef(androidJavaObject.GetRawObject());
				this.m_jobject = IntPtr.Zero;
			}
		}
	}
}
