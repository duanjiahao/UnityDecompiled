using System;

namespace UnityEngine
{
	[Obsolete("ADInterstitialAd class is deprecated. Please use iOS.ADInterstitialAd instead (UnityUpgradable) -> UnityEngine.iOS.ADInterstitialAd", true)]
	public sealed class ADInterstitialAd
	{
		public delegate void InterstitialWasLoadedDelegate();

		public static event ADInterstitialAd.InterstitialWasLoadedDelegate onInterstitialWasLoaded
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

		~ADInterstitialAd()
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
