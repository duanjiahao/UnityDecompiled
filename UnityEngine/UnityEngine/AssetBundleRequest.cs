using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AssetBundleRequest : AsyncOperation
	{
		public extern Object asset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern Object[] allAssets
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
