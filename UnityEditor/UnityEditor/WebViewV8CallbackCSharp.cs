using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[Serializable]
	internal sealed class WebViewV8CallbackCSharp
	{
		[SerializeField]
		private IntPtr m_thisDummy;

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Callback(string result);

		public void OnDestroy()
		{
			this.DestroyCallBack();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DestroyCallBack();
	}
}
