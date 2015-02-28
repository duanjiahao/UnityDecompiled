using System;
using System.Runtime.CompilerServices;
namespace UnityEngine.iOS
{
	public sealed class ADInterstitialAd
	{
		public delegate void InterstitialWasLoadedDelegate();
		private IntPtr interstitialView;
		private static bool _AlwaysFalseDummy;
		public static event ADInterstitialAd.InterstitialWasLoadedDelegate onInterstitialWasLoaded
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADInterstitialAd.onInterstitialWasLoaded = (ADInterstitialAd.InterstitialWasLoadedDelegate)Delegate.Combine(ADInterstitialAd.onInterstitialWasLoaded, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADInterstitialAd.onInterstitialWasLoaded = (ADInterstitialAd.InterstitialWasLoadedDelegate)Delegate.Remove(ADInterstitialAd.onInterstitialWasLoaded, value);
			}
		}
		public static bool isAvailable
		{
			get
			{
				return ADInterstitialAd.Native_InterstitialAvailable();
			}
		}
		public bool loaded
		{
			get
			{
				return ADInterstitialAd.Native_InterstitialAdLoaded(this.interstitialView);
			}
		}
		public ADInterstitialAd(bool autoReload)
		{
			this.CtorImpl(autoReload);
		}
		public ADInterstitialAd()
		{
			this.CtorImpl(false);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Native_CreateInterstitial(bool autoReload);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_ShowInterstitial(IntPtr view);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_ReloadInterstitial(IntPtr view);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_InterstitialAdLoaded(IntPtr view);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_InterstitialAvailable();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_DestroyInterstitial(IntPtr view);
		private void CtorImpl(bool autoReload)
		{
			if (ADInterstitialAd._AlwaysFalseDummy)
			{
				ADInterstitialAd.FireInterstitialWasLoaded();
			}
			this.interstitialView = ADInterstitialAd.Native_CreateInterstitial(autoReload);
		}
		~ADInterstitialAd()
		{
			ADInterstitialAd.Native_DestroyInterstitial(this.interstitialView);
		}
		public void Show()
		{
			if (this.loaded)
			{
				ADInterstitialAd.Native_ShowInterstitial(this.interstitialView);
			}
			else
			{
				Debug.Log("Calling ADInterstitialAd.Show() when the ad is not loaded");
			}
		}
		public void ReloadAd()
		{
			ADInterstitialAd.Native_ReloadInterstitial(this.interstitialView);
		}
		private static void FireInterstitialWasLoaded()
		{
			if (ADInterstitialAd.onInterstitialWasLoaded != null)
			{
				ADInterstitialAd.onInterstitialWasLoaded();
			}
		}
	}
}
