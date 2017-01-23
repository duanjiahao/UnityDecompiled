using System;

namespace UnityEngine.iOS
{
	[Obsolete("iOS.ADInterstitialAd class is obsolete, Apple iAD service discontinued", true)]
	public sealed class ADInterstitialAd
	{
		public delegate void InterstitialWasLoadedDelegate();

		public delegate void InterstitialWasViewedDelegate();

		public static event ADInterstitialAd.InterstitialWasLoadedDelegate onInterstitialWasLoaded
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event ADInterstitialAd.InterstitialWasViewedDelegate onInterstitialWasViewed
		{
			add
			{
			}
			remove
			{
			}
		}

		public static bool isAvailable
		{
			get
			{
				return false;
			}
		}

		public bool loaded
		{
			get
			{
				return false;
			}
		}

		public ADInterstitialAd(bool autoReload)
		{
		}

		public ADInterstitialAd()
		{
		}

		public void Show()
		{
		}

		public void ReloadAd()
		{
		}
	}
}
