using System;
using UnityEngine.SocialPlatforms;

namespace UnityEngine
{
	public static class Social
	{
		public static ISocialPlatform Active
		{
			get
			{
				return ActivePlatform.Instance;
			}
			set
			{
				ActivePlatform.Instance = value;
			}
		}

		public static ILocalUser localUser
		{
			get
			{
				return Social.Active.localUser;
			}
		}

		public static void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			Social.Active.LoadUsers(userIDs, callback);
		}

		public static void ReportProgress(string achievementID, double progress, Action<bool> callback)
		{
			Social.Active.ReportProgress(achievementID, progress, callback);
		}

		public static void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			Social.Active.LoadAchievementDescriptions(callback);
		}

		public static void LoadAchievements(Action<IAchievement[]> callback)
		{
			Social.Active.LoadAchievements(callback);
		}

		public static void ReportScore(long score, string board, Action<bool> callback)
		{
			Social.Active.ReportScore(score, board, callback);
		}

		public static void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			Social.Active.LoadScores(leaderboardID, callback);
		}

		public static ILeaderboard CreateLeaderboard()
		{
			return Social.Active.CreateLeaderboard();
		}

		public static IAchievement CreateAchievement()
		{
			return Social.Active.CreateAchievement();
		}

		public static void ShowAchievementsUI()
		{
			Social.Active.ShowAchievementsUI();
		}

		public static void ShowLeaderboardUI()
		{
			Social.Active.ShowLeaderboardUI();
		}
	}
}
