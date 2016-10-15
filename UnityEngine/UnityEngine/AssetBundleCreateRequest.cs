using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public sealed class AssetBundleCreateRequest : AsyncOperation
	{
		[Flags]
		internal enum CompatibilityCheck
		{
			None = 0,
			TypeTree = 1,
			RuntimeVersion = 2,
			ClassVersion = 4,
			All = 7
		}

		public extern AssetBundle assetBundle
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern AssetBundleCreateRequest.CompatibilityCheck compatibilityChecks
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
