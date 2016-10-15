using System;

namespace UnityEngine
{
	[Obsolete("ADBannerView class is deprecated. Please use iOS.ADBannerView instead (UnityUpgradable) -> UnityEngine.iOS.ADBannerView", true)]
	public sealed class ADBannerView
	{
		public enum Layout
		{
			Top,
			Bottom,
			TopLeft = 0,
			TopRight = 4,
			TopCenter = 8,
			BottomLeft = 1,
			BottomRight = 5,
			BottomCenter = 9,
			CenterLeft = 2,
			CenterRight = 6,
			Center = 10,
			Manual = -1
		}

		public enum Type
		{
			Banner,
			MediumRect
		}

		public delegate void BannerWasClickedDelegate();

		public delegate void BannerWasLoadedDelegate();

		public static event ADBannerView.BannerWasClickedDelegate onBannerWasClicked
		{
			add
			{
			}
			remove
			{
			}
		}

		public static event ADBannerView.BannerWasLoadedDelegate onBannerWasLoaded
		{
			add
			{
			}
			remove
			{
			}
		}

		public bool loaded
		{
			get
			{
				return false;
			}
		}

		public bool visible
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public ADBannerView.Layout layout
		{
			get
			{
				return ADBannerView.Layout.Top;
			}
			set
			{
			}
		}

		public Vector2 position
		{
			get
			{
				return default(Vector2);
			}
			set
			{
			}
		}

		public Vector2 size
		{
			get
			{
				return default(Vector2);
			}
		}

		public ADBannerView(ADBannerView.Type type, ADBannerView.Layout layout)
		{
		}

		public static bool IsAvailable(ADBannerView.Type type)
		{
			return false;
		}
	}
}
