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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Callback(string result);

		public void OnDestroy()
		{
			this.DestroyCallBack();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyCallBack();
	}
}
