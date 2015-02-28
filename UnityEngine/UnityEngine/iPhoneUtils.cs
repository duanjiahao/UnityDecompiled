using System;
namespace UnityEngine
{
	public sealed class iPhoneUtils
	{
		[Obsolete("isApplicationGenuine property is deprecated (UnityUpgradable). Please use Application.genuine instead.", true)]
		public static bool isApplicationGenuine
		{
			get
			{
				return false;
			}
		}
		[Obsolete("isApplicationGenuineAvailable property is deprecated (UnityUpgradable). Please use Application.genuineCheckAvailable instead.", true)]
		public static bool isApplicationGenuineAvailable
		{
			get
			{
				return false;
			}
		}
		[Obsolete("PlayMovie method is deprecated (UnityUpgradable). Please use Handheld.PlayFullScreenMovie instead.", true)]
		public static void PlayMovie(string path, Color bgColor, iPhoneMovieControlMode controlMode, iPhoneMovieScalingMode scalingMode)
		{
		}
		[Obsolete("PlayMovie method is deprecated (UnityUpgradable). Please use Handheld.PlayFullScreenMovie instead.", true)]
		public static void PlayMovie(string path, Color bgColor, iPhoneMovieControlMode controlMode)
		{
		}
		[Obsolete("PlayMovie method is deprecated (UnityUpgradable). Please use Handheld.PlayFullScreenMovie instead.", true)]
		public static void PlayMovie(string path, Color bgColor)
		{
		}
		[Obsolete("PlayMovieURL method is deprecated (UnityUpgradable). Please use Handheld.PlayFullScreenMovie instead.", true)]
		public static void PlayMovieURL(string url, Color bgColor, iPhoneMovieControlMode controlMode, iPhoneMovieScalingMode scalingMode)
		{
		}
		[Obsolete("PlayMovieURL method is deprecated (UnityUpgradable). Please use Handheld.PlayFullScreenMovie instead.", true)]
		public static void PlayMovieURL(string url, Color bgColor, iPhoneMovieControlMode controlMode)
		{
		}
		[Obsolete("PlayMovieURL method is deprecated (UnityUpgradable). Please use Handheld.PlayFullScreenMovie instead.", true)]
		public static void PlayMovieURL(string url, Color bgColor)
		{
		}
		[Obsolete("Vibrate method is deprecated (UnityUpgradable). Please use Handheld.Vibrate instead.", true)]
		public static void Vibrate()
		{
		}
	}
}
