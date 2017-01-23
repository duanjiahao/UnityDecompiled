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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_PlayFullScreenMovie(string path, ref Color bgColor, FullScreenMovieControlMode controlMode, FullScreenMovieScalingMode scalingMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Vibrate();

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

		public static void SetActivityIndicatorStyle(TizenActivityIndicatorStyle style)
		{
			Handheld.SetActivityIndicatorStyleImpl((int)style);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetActivityIndicatorStyle();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartActivityIndicator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopActivityIndicator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearShaderCache();
	}
}
