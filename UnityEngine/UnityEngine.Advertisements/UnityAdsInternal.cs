using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Advertisements
{
	[RequiredByNativeCode]
	internal sealed class UnityAdsInternal
	{
		public static event UnityAdsDelegate onCampaignsAvailable
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnityAdsInternal.onCampaignsAvailable = (UnityAdsDelegate)Delegate.Combine(UnityAdsInternal.onCampaignsAvailable, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnityAdsInternal.onCampaignsAvailable = (UnityAdsDelegate)Delegate.Remove(UnityAdsInternal.onCampaignsAvailable, value);
			}
		}

		public static event UnityAdsDelegate onCampaignsFetchFailed
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnityAdsInternal.onCampaignsFetchFailed = (UnityAdsDelegate)Delegate.Combine(UnityAdsInternal.onCampaignsFetchFailed, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnityAdsInternal.onCampaignsFetchFailed = (UnityAdsDelegate)Delegate.Remove(UnityAdsInternal.onCampaignsFetchFailed, value);
			}
		}

		public static event UnityAdsDelegate onShow
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnityAdsInternal.onShow = (UnityAdsDelegate)Delegate.Combine(UnityAdsInternal.onShow, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnityAdsInternal.onShow = (UnityAdsDelegate)Delegate.Remove(UnityAdsInternal.onShow, value);
			}
		}

		public static event UnityAdsDelegate onHide
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnityAdsInternal.onHide = (UnityAdsDelegate)Delegate.Combine(UnityAdsInternal.onHide, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnityAdsInternal.onHide = (UnityAdsDelegate)Delegate.Remove(UnityAdsInternal.onHide, value);
			}
		}

		public static event UnityAdsDelegate<string, bool> onVideoCompleted
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnityAdsInternal.onVideoCompleted = (UnityAdsDelegate<string, bool>)Delegate.Combine(UnityAdsInternal.onVideoCompleted, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnityAdsInternal.onVideoCompleted = (UnityAdsDelegate<string, bool>)Delegate.Remove(UnityAdsInternal.onVideoCompleted, value);
			}
		}

		public static event UnityAdsDelegate onVideoStarted
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				UnityAdsInternal.onVideoStarted = (UnityAdsDelegate)Delegate.Combine(UnityAdsInternal.onVideoStarted, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				UnityAdsInternal.onVideoStarted = (UnityAdsDelegate)Delegate.Remove(UnityAdsInternal.onVideoStarted, value);
			}
		}

		public static void RemoveAllEventHandlers()
		{
			UnityAdsInternal.onCampaignsAvailable = null;
			UnityAdsInternal.onCampaignsFetchFailed = null;
			UnityAdsInternal.onShow = null;
			UnityAdsInternal.onHide = null;
			UnityAdsInternal.onVideoCompleted = null;
			UnityAdsInternal.onVideoStarted = null;
		}

		public static void CallUnityAdsCampaignsAvailable()
		{
			UnityAdsDelegate unityAdsDelegate = UnityAdsInternal.onCampaignsAvailable;
			if (unityAdsDelegate != null)
			{
				unityAdsDelegate();
			}
		}

		public static void CallUnityAdsCampaignsFetchFailed()
		{
			UnityAdsDelegate unityAdsDelegate = UnityAdsInternal.onCampaignsFetchFailed;
			if (unityAdsDelegate != null)
			{
				unityAdsDelegate();
			}
		}

		public static void CallUnityAdsShow()
		{
			UnityAdsDelegate unityAdsDelegate = UnityAdsInternal.onShow;
			if (unityAdsDelegate != null)
			{
				unityAdsDelegate();
			}
		}

		public static void CallUnityAdsHide()
		{
			UnityAdsDelegate unityAdsDelegate = UnityAdsInternal.onHide;
			if (unityAdsDelegate != null)
			{
				unityAdsDelegate();
			}
		}

		public static void CallUnityAdsVideoCompleted(string rewardItemKey, bool skipped)
		{
			UnityAdsDelegate<string, bool> unityAdsDelegate = UnityAdsInternal.onVideoCompleted;
			if (unityAdsDelegate != null)
			{
				unityAdsDelegate(rewardItemKey, skipped);
			}
		}

		public static void CallUnityAdsVideoStarted()
		{
			UnityAdsDelegate unityAdsDelegate = UnityAdsInternal.onVideoStarted;
			if (unityAdsDelegate != null)
			{
				unityAdsDelegate();
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RegisterNative();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Init(string gameId, bool testModeEnabled, bool debugModeEnabled, string unityVersion);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Show(string zoneId, string rewardItemKey, string options);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CanShowAds(string zoneId);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLogLevel(int logLevel);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCampaignDataURL(string url);
	}
}
