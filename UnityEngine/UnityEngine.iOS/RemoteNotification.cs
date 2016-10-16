using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	[RequiredByNativeCode]
	public sealed class RemoteNotification
	{
		private IntPtr notificationWrapper;

		public extern string alertBody
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasAction
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int applicationIconBadgeNumber
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string soundName
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern IDictionary userInfo
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private RemoteNotification()
		{
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Destroy();

		~RemoteNotification()
		{
			this.Destroy();
		}
	}
}
