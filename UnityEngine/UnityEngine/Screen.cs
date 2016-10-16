using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Screen
	{
		public static extern Resolution[] resolutions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property lockCursor has been deprecated. Use Cursor.lockState and Cursor.visible instead.")]
		public static bool lockCursor
		{
			get
			{
				return CursorLockMode.Locked == Cursor.lockState;
			}
			set
			{
				if (value)
				{
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
			}
		}

		public static extern Resolution currentResolution
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern int width
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadAndSerializationSafe]
		public static extern int height
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern float dpi
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool fullScreen
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool autorotateToPortrait
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool autorotateToPortraitUpsideDown
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool autorotateToLandscapeLeft
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool autorotateToLandscapeRight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern ScreenOrientation orientation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern int sleepTimeout
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetResolution has been deprecated. Use resolutions instead (UnityUpgradable) -> resolutions", true)]
		public static Resolution[] GetResolution
		{
			get
			{
				return null;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property showCursor has been deprecated. Use Cursor.visible instead (UnityUpgradable) -> UnityEngine.Cursor.visible", true)]
		public static bool showCursor
		{
			get;
			set;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetResolution(int width, int height, bool fullscreen, [UnityEngine.Internal.DefaultValue("0")] int preferredRefreshRate);

		[ExcludeFromDocs]
		public static void SetResolution(int width, int height, bool fullscreen)
		{
			int preferredRefreshRate = 0;
			Screen.SetResolution(width, height, fullscreen, preferredRefreshRate);
		}
	}
}
