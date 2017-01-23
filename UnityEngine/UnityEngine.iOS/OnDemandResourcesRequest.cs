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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float loadingPriority
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal OnDemandResourcesRequest()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetResourcePath(string resourceName);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Dispose();

		~OnDemandResourcesRequest()
		{
			this.Dispose();
		}
	}
}
