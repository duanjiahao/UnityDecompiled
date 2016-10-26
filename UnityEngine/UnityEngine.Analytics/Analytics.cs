using System;
using System.Collections.Generic;

namespace UnityEngine.Analytics
{
	public static class Analytics
	{
		private static UnityAnalyticsHandler s_UnityAnalyticsHandler;

		internal static UnityAnalyticsHandler GetUnityAnalyticsHandler()
		{
			if (Analytics.s_UnityAnalyticsHandler == null)
			{
				Analytics.s_UnityAnalyticsHandler = new UnityAnalyticsHandler();
			}
			return Analytics.s_UnityAnalyticsHandler;
		}

		public static AnalyticsResult SetUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException("Cannot set userId to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			return unityAnalyticsHandler.SetUserId(userId);
		}

		public static AnalyticsResult SetUserGender(Gender gender)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			return unityAnalyticsHandler.SetUserGender(gender);
		}

		public static AnalyticsResult SetUserBirthYear(int birthYear)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (Analytics.s_UnityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			return unityAnalyticsHandler.SetUserBirthYear(birthYear);
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			return unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, null, null);
		}

		public static AnalyticsResult Transaction(string productId, decimal amount, string currency, string receiptPurchaseData, string signature)
		{
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			return unityAnalyticsHandler.Transaction(productId, Convert.ToDouble(amount), currency, receiptPurchaseData, signature);
		}

		public static AnalyticsResult CustomEvent(string customEventName)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			return unityAnalyticsHandler.CustomEvent(customEventName);
		}

		public static AnalyticsResult CustomEvent(string customEventName, Vector3 position)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			CustomEventData customEventData = new CustomEventData(customEventName);
			customEventData.Add("x", (double)Convert.ToDecimal(position.x));
			customEventData.Add("y", (double)Convert.ToDecimal(position.y));
			customEventData.Add("z", (double)Convert.ToDecimal(position.z));
			return unityAnalyticsHandler.CustomEvent(customEventData);
		}

		public static AnalyticsResult CustomEvent(string customEventName, IDictionary<string, object> eventData)
		{
			if (string.IsNullOrEmpty(customEventName))
			{
				throw new ArgumentException("Cannot set custom event name to an empty or null string");
			}
			UnityAnalyticsHandler unityAnalyticsHandler = Analytics.GetUnityAnalyticsHandler();
			if (unityAnalyticsHandler == null)
			{
				return AnalyticsResult.NotInitialized;
			}
			if (eventData == null)
			{
				return unityAnalyticsHandler.CustomEvent(customEventName);
			}
			CustomEventData customEventData = new CustomEventData(customEventName);
			customEventData.Add(eventData);
			return unityAnalyticsHandler.CustomEvent(customEventData);
		}
	}
}
