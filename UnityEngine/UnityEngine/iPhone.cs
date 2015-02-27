using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	public sealed class iPhone
	{
		public sealed class NSError
		{
			private IntPtr _nativeError;
			public int code
			{
				get
				{
					return iPhone.NSError.UnityNSError_Code(this._nativeError);
				}
			}
			public string description
			{
				get
				{
					return Marshal.PtrToStringAnsi(iPhone.NSError.UnityNSError_Description(this._nativeError));
				}
			}
			public string reason
			{
				get
				{
					return Marshal.PtrToStringAnsi(iPhone.NSError.UnityNSError_Reason(this._nativeError));
				}
			}
			private NSError(IntPtr nativeError)
			{
				this._nativeError = nativeError;
				iPhone.UnityNSObject_RetainObject(this._nativeError);
			}
			[DllImport("__Internal")]
			private static extern int UnityNSError_Code(IntPtr errorObj);
			[DllImport("__Internal")]
			private static extern IntPtr UnityNSError_Description(IntPtr errorObj);
			[DllImport("__Internal")]
			private static extern IntPtr UnityNSError_Reason(IntPtr errorObj);
			~NSError()
			{
				iPhone.UnityNSObject_ReleaseObject(this._nativeError);
			}
			public static iPhone.NSError CreateNSError(IntPtr nativeError)
			{
				return (!(nativeError == IntPtr.Zero)) ? new iPhone.NSError(nativeError) : null;
			}
		}
		public sealed class NSNotification
		{
			private IntPtr _nativeNotification;
			public string name
			{
				get
				{
					return Marshal.PtrToStringAnsi(iPhone.NSNotification.UnityNSNotification_Name(this._nativeNotification));
				}
			}
			private NSNotification(IntPtr nativeNotification)
			{
				this._nativeNotification = nativeNotification;
				iPhone.UnityNSObject_RetainObject(this._nativeNotification);
			}
			[DllImport("__Internal")]
			private static extern IntPtr UnityNSNotification_Name(IntPtr notificationObj);
			~NSNotification()
			{
				iPhone.UnityNSObject_ReleaseObject(this._nativeNotification);
			}
			public static iPhone.NSNotification CreateNSNotification(IntPtr nativeNotification)
			{
				return (!(nativeNotification == IntPtr.Zero)) ? new iPhone.NSNotification(nativeNotification) : null;
			}
		}
		public static extern iPhoneGeneration generation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string vendorIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern string advertisingIdentifier
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public static extern bool advertisingTrackingEnabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		[DllImport("__Internal")]
		internal static extern void UnityNSObject_RetainObject(IntPtr obj);
		[DllImport("__Internal")]
		internal static extern void UnityNSObject_ReleaseObject(IntPtr obj);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetNoBackupFlag(string path);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ResetNoBackupFlag(string path);
	}
}
