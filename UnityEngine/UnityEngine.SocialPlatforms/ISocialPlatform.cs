using System;

namespace UnityEngine.SocialPlatforms
{
	public interface ISocialPlatform
	{
		ILocalUser localUser
		{
			get;
		}

		void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);

		void ReportProgress(string achievementID, double progress, Action<bool> callback);

		void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback);

		void LoadAchievements(Action<IAchievement[]> callback);

		IAchievement CreateAchievement();

		void ReportScore(long score, string board, Action<bool> callback);

		void LoadScores(string leaderboardID, Action<IScore[]> callback);

		ILeaderboard CreateLeaderboard();

		void ShowAchievementsUI();

		void ShowLeaderboardUI();

		void Authenticate(ILocalUser user, Action<bool> callback);

		void Authenticate(ILocalUser user, Action<bool, string> callback);

		void LoadFriends(ILocalUser user, Action<bool> callback);

		void LoadScores(ILeaderboard board, Action<bool> callback);

		bool GetLoading(ILeaderboard board);
	}
}
