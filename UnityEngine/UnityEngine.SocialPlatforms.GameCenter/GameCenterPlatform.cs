using System;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	public class GameCenterPlatform : Local
	{
		public static void ResetAllAchievements(Action<bool> callback)
		{
			Debug.Log("ResetAllAchievements - no effect in editor");
			callback(true);
		}

		public static void ShowDefaultAchievementCompletionBanner(bool value)
		{
			Debug.Log("ShowDefaultAchievementCompletionBanner - no effect in editor");
		}

		public static void ShowLeaderboardUI(string leaderboardID, TimeScope timeScope)
		{
			Debug.Log("ShowLeaderboardUI - no effect in editor");
		}
	}
}
