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
			this.m_jclass = new GlobalJavaObjectRef(jclass);
			this.m_jobject = new GlobalJavaObjectRef(IntPtr.Zero);
		}

		private void _AndroidJavaClass(string className)
		{
			base.DebugPrint("Creating AndroidJavaClass from " + className);
			using (AndroidJavaObject androidJavaObject = AndroidJavaObject.FindClass(className))
			{
				this.m_jclass = new GlobalJavaObjectRef(androidJavaObject.GetRawObject());
				this.m_jobject = new GlobalJavaObjectRef(IntPtr.Zero);
			}
		}
	}
}
