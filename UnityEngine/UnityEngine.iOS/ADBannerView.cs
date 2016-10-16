using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.iOS
{
	[RequiredByNativeCode]
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

		public delegate void BannerFailedToLoadDelegate();

		private ADBannerView.Layout _layout;

		private IntPtr _bannerView;

		private static bool _AlwaysFalseDummy;

		public static event ADBannerView.BannerWasClickedDelegate onBannerWasClicked
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADBannerView.onBannerWasClicked = (ADBannerView.BannerWasClickedDelegate)Delegate.Combine(ADBannerView.onBannerWasClicked, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADBannerView.onBannerWasClicked = (ADBannerView.BannerWasClickedDelegate)Delegate.Remove(ADBannerView.onBannerWasClicked, value);
			}
		}

		public static event ADBannerView.BannerWasLoadedDelegate onBannerWasLoaded
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADBannerView.onBannerWasLoaded = (ADBannerView.BannerWasLoadedDelegate)Delegate.Combine(ADBannerView.onBannerWasLoaded, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADBannerView.onBannerWasLoaded = (ADBannerView.BannerWasLoadedDelegate)Delegate.Remove(ADBannerView.onBannerWasLoaded, value);
			}
		}

		public static event ADBannerView.BannerFailedToLoadDelegate onBannerFailedToLoad
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				ADBannerView.onBannerFailedToLoad = (ADBannerView.BannerFailedToLoadDelegate)Delegate.Combine(ADBannerView.onBannerFailedToLoad, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				ADBannerView.onBannerFailedToLoad = (ADBannerView.BannerFailedToLoadDelegate)Delegate.Remove(ADBannerView.onBannerFailedToLoad, value);
			}
		}

		public bool loaded
		{
			get
			{
				return ADBannerView.Native_BannerAdLoaded(this._bannerView);
			}
		}

		public bool visible
		{
			get
			{
				return ADBannerView.Native_BannerAdVisible(this._bannerView);
			}
			set
			{
				ADBannerView.Native_ShowBanner(this._bannerView, value);
			}
		}

		public ADBannerView.Layout layout
		{
			get
			{
				return this._layout;
			}
			set
			{
				this._layout = value;
				ADBannerView.Native_LayoutBanner(this._bannerView, (int)this._layout);
			}
		}

		public Vector2 position
		{
			get
			{
				Vector2 v;
				ADBannerView.Native_BannerPosition(this._bannerView, out v);
				return this.OSToScreenCoords(v);
			}
			set
			{
				Vector2 pos = new Vector2(value.x / (float)Screen.width, value.y / (float)Screen.height);
				ADBannerView.Native_MoveBanner(this._bannerView, pos);
			}
		}

		public Vector2 size
		{
			get
			{
				Vector2 v;
				ADBannerView.Native_BannerSize(this._bannerView, out v);
				return this.OSToScreenCoords(v);
			}
		}

		public ADBannerView(ADBannerView.Type type, ADBannerView.Layout layout)
		{
			if (ADBannerView._AlwaysFalseDummy)
			{
				ADBannerView.FireBannerWasClicked();
				ADBannerView.FireBannerWasLoaded();
				ADBannerView.FireBannerFailedToLoad();
			}
			this._bannerView = ADBannerView.Native_CreateBanner((int)type, (int)layout);
		}

		private static IntPtr Native_CreateBanner(int type, int layout)
		{
			IntPtr result;
			ADBannerView.INTERNAL_CALL_Native_CreateBanner(type, layout, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Native_CreateBanner(int type, int layout, out IntPtr value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_ShowBanner(IntPtr view, bool show);

		private static void Native_MoveBanner(IntPtr view, Vector2 pos)
		{
			ADBannerView.INTERNAL_CALL_Native_MoveBanner(view, ref pos);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Native_MoveBanner(IntPtr view, ref Vector2 pos);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_LayoutBanner(IntPtr view, int layout);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_BannerTypeAvailable(int type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_BannerPosition(IntPtr view, out Vector2 pos);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_BannerSize(IntPtr view, out Vector2 pos);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_BannerAdLoaded(IntPtr view);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Native_BannerAdVisible(IntPtr view);

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Native_DestroyBanner(IntPtr view);

		public static bool IsAvailable(ADBannerView.Type type)
		{
			return ADBannerView.Native_BannerTypeAvailable((int)type);
		}

		~ADBannerView()
		{
			ADBannerView.Native_DestroyBanner(this._bannerView);
		}

		private Vector2 OSToScreenCoords(Vector2 v)
		{
			return new Vector2(v.x * (float)Screen.width, v.y * (float)Screen.height);
		}

		[RequiredByNativeCode]
		private static void FireBannerWasClicked()
		{
			if (ADBannerView.onBannerWasClicked != null)
			{
				ADBannerView.onBannerWasClicked();
			}
		}

		[RequiredByNativeCode]
		private static void FireBannerWasLoaded()
		{
			if (ADBannerView.onBannerWasLoaded != null)
			{
				ADBannerView.onBannerWasLoaded();
			}
		}

		[RequiredByNativeCode]
		private static void FireBannerFailedToLoad()
		{
			if (ADBannerView.onBannerFailedToLoad != null)
			{
				ADBannerView.onBannerFailedToLoad();
			}
		}
	}
}
