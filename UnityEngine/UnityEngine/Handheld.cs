using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.iOS;

namespace UnityEngine
{
	public sealed class Handheld
	{
		[Obsolete("Property Handheld.use32BitDisplayBuffer has been deprecated. Modifying it has no effect, use PlayerSettings instead.")]
		public static extern bool use32BitDisplayBuffer
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static bool PlayFullScreenMovie(string path, [DefaultValue("Color.black")] Color bgColor, [DefaultValue("FullScreenMovieControlMode.Full")] FullScreenMovieControlMode controlMode, [DefaultValue("FullScreenMovieScalingMode.AspectFit")] FullScreenMovieScalingMode scalingMode)
		{
			return Handheld.INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[ExcludeFromDocs]
		public static bool PlayFullScreenMovie(string path, Color bgColor, FullScreenMovieControlMode controlMode)
		{
			FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;
			return Handheld.INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[ExcludeFromDocs]
		public static bool PlayFullScreenMovie(string path, Color bgColor)
		{
			FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;
			FullScreenMovieControlMode controlMode = FullScreenMovieControlMode.Full;
			return Handheld.INTERNAL_CALL_PlayFullScreenMovie(path, ref bgColor, controlMode, scalingMode);
		}

		[ExcludeFromDocs]
		public static bool PlayFullScreenMovie(string path)
		{
			FullScreenMovieScalingMode scalingMode = FullScreenMovieScalingMode.AspectFit;
			FullScreenMovieControlMode controlMode = FullScreenMovieControlMode.Full;
			Color black = Color.black;
			return Handheld.INTERNAL_CALL_PlayFullScreenMovie(path, ref black, controlMode, scalingMode);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_PlayFullScreenMovie(string path, ref Color bgColor, FullScreenMovieControlMode controlMode, FullScreenMovieScalingMode scalingMode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Vibrate();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetActivityIndicatorStyleImpl(int style);

		public static void SetActivityIndicatorStyle(ActivityIndicatorStyle style)
		{
			Handheld.SetActivityIndicatorStyleImpl((int)style);
		}

		public static void SetActivityIndicatorStyle(AndroidActivityIndicatorStyle style)
		{
			Handheld.SetActivityIndicatorStyleImpl((int)style);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetActivityIndicatorStyle();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartActivityIndicator();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopActivityIndicator();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearShaderCache();
	}
}
