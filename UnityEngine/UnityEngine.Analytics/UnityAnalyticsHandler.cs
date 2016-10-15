using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Analytics
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class UnityAnalyticsHandler : IDisposable
	{
		[NonSerialized]
		internal IntPtr m_Ptr;

		public UnityAnalyticsHandler()
		{
			this.InternalCreate();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalCreate();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void InternalDestroy();

		~UnityAnalyticsHandler()
		{
			this.InternalDestroy();
		}

		public void Dispose()
		{
			this.InternalDestroy();
			GC.SuppressFinalize(this);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SetUserId(string userId);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SetUserGender(Gender gender);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult SetUserBirthYear(int birthYear);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnalyticsResult Transaction(string productId, double amount, string currency);

		public AnalyticsResult Transaction(string productId, double amount, string currency, string receiptPurchaseData, string signature)
		{
			if (receiptPurchaseData == null)
			{
				receiptPurchaseData = string.Empty;
			}
			if (signature == null)
			{
				signature = string.Empty;
			}
			return this.InternalTransaction(productId, amount, currency, receiptPurchaseData, signature);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnalyticsResult InternalTransaction(string productId, double amount, string currency, string receiptPurchaseData, string signature);

		public AnalyticsResult CustomEvent(string customEventName)
		{
			return this.SendCustomEventName(customEventName);
		}

		public AnalyticsResult CustomEvent(CustomEventData eventData)
		{
			return this.SendCustomEvent(eventData);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnalyticsResult SendCustomEventName(string customEventName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AnalyticsResult SendCustomEvent(CustomEventData eventData);
	}
}
