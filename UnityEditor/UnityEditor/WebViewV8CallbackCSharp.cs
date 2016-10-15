using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal sealed class WebViewV8CallbackCSharp
	{
		[SerializeField]
		private IntPtr m_thisDummy;

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Callback(string result);

		public void OnDestroy()
		{
			this.DestroyCallBack();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyCallBack();
	}
}
