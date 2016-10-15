using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class OnDemandResourcesRequest : AsyncOperation, IDisposable
	{
		public extern string error
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float loadingPriority
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal OnDemandResourcesRequest()
		{
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResourcePath(string resourceName);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~OnDemandResourcesRequest()
		{
			this.Dispose();
		}
	}
}
