using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	[RequiredByNativeCode]
	public sealed class GameCenterPlatform : ISocialPlatform
	{
		private static Action<bool, string> s_AuthenticateCallback;

		private static AchievementDescription[] s_adCache = new AchievementDescription[0];

		private static UserProfile[] s_friends = new UserProfile[0];

		private static UserProfile[] s_users = new UserProfile[0];

		private static Action<bool> s_ResetAchievements;

		private static LocalUser m_LocalUser;

		private static List<GcLeaderboard> m_GcBoards = new List<GcLeaderboard>();

		public ILocalUser localUser
		{
			get
			{
				if (GameCenterPlatform.m_LocalUser == null)
				{
					GameCenterPlatform.m_LocalUser = new LocalUser();
				}
				if (GameCenterPlatform.Internal_Authenticated() && GameCenterPlatform.m_LocalUser.id == "0")
				{
					GameCenterPlatform.PopulateLocalUser();
				}
				return GameCenterPlatform.m_LocalUser;
			}
		}

		[RequiredByNativeCode]
		private static void ClearAchievementDescriptions(int size)
		{
			if (GameCenterPlatform.s_adCache == null || GameCenterPlatform.s_adCache.Length != size)
			{
				GameCenterPlatform.s_adCache = new AchievementDescription[size];
			}
		}

		[RequiredByNativeCode]
		private static void SetAchievementDescription(GcAchievementDescriptionData data, int number)
		{
			GameCenterPlatform.s_adCache[number] = data.ToAchievementDescription();
		}

		[RequiredByNativeCode]
		private static void SetAchievementDescriptionImage(Texture2D texture, int number)
		{
			if (GameCenterPlatform.s_adCache.Length <= number || number < 0)
			{
				Debug.Log("Achievement description number out of bounds when setting image");
			}
			else
			{
				GameCenterPlatform.s_adCache[number].SetImage(texture);
			}
		}

		[RequiredByNativeCode]
		private static void TriggerAchievementDescriptionCallback(Action<IAchievementDescription[]> callback)
		{
			if (callback != null && GameCenterPlatform.s_adCache != null)
			{
				if (GameCenterPlatform.s_adCache.Length == 0)
				{
					Debug.Log("No achivevement descriptions returned");
				}
				callback(GameCenterPlatform.s_adCache);
			}
		}

		[RequiredByNativeCode]
		private static void AuthenticateCallbackWrapper(int result, string error)
		{
			GameCenterPlatform.PopulateLocalUser();
			if (GameCenterPlatform.s_AuthenticateCallback != null)
			{
				GameCenterPlatform.s_AuthenticateCallback(result == 1, error);
				GameCenterPlatform.s_AuthenticateCallback = null;
			}
		}

		[RequiredByNativeCode]
		private static void ClearFriends(int size)
		{
			GameCenterPlatform.SafeClearArray(ref GameCenterPlatform.s_friends, size);
		}

		[RequiredByNativeCode]
		private static void SetFriends(GcUserProfileData data, int number)
		{
			data.AddToArray(ref GameCenterPlatform.s_friends, number);
		}

		[RequiredByNativeCode]
		private static void SetFriendImage(Texture2D texture, int number)
		{
			GameCenterPlatform.SafeSetUserImage(ref GameCenterPlatform.s_friends, texture, number);
		}

		[RequiredByNativeCode]
		private static void TriggerFriendsCallbackWrapper(Action<bool> callback, int result)
		{
			if (GameCenterPlatform.s_friends != null)
			{
				GameCenterPlatform.m_LocalUser.SetFriends(GameCenterPlatform.s_friends);
			}
			if (callback != null)
			{
				callback(result == 1);
			}
		}

		[RequiredByNativeCode]
		private static void AchievementCallbackWrapper(Action<IAchievement[]> callback, GcAchievementData[] result)
		{
			if (callback != null)
			{
				if (result.Length == 0)
				{
					Debug.Log("No achievements returned");
				}
				Achievement[] array = new Achievement[result.Length];
				for (int i = 0; i < result.Length; i++)
				{
					array[i] = result[i].ToAchievement();
				}
				callback(array);
			}
		}

		[RequiredByNativeCode]
		private static void ProgressCallbackWrapper(Action<bool> callback, bool success)
		{
			if (callback != null)
			{
				callback(success);
			}
		}

		[RequiredByNativeCode]
		private static void ScoreCallbackWrapper(Action<bool> callback, bool success)
		{
			if (callback != null)
			{
				callback(success);
			}
		}

		[RequiredByNativeCode]
		private static void ScoreLoaderCallbackWrapper(Action<IScore[]> callback, GcScoreData[] result)
		{
			if (callback != null)
			{
				Score[] array = new Score[result.Length];
				for (int i = 0; i < result.Length; i++)
				{
					array[i] = result[i].ToScore();
				}
				callback(array);
			}
		}

		void ISocialPlatform.LoadFriends(ILocalUser user, Action<bool> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				GameCenterPlatform.Internal_LoadFriends(callback);
			}
		}

		void ISocialPlatform.Authenticate(ILocalUser user, Action<bool> callback)
		{
			((ISocialPlatform)this).Authenticate(user, delegate(bool success, string error)
			{
				callback(success);
			});
		}

		void ISocialPlatform.Authenticate(ILocalUser user, Action<bool, string> callback)
		{
			GameCenterPlatform.s_AuthenticateCallback = callback;
			GameCenterPlatform.Internal_Authenticate();
		}

		[RequiredByNativeCode]
		private static void PopulateLocalUser()
		{
			GameCenterPlatform.m_LocalUser.SetAuthenticated(GameCenterPlatform.Internal_Authenticated());
			GameCenterPlatform.m_LocalUser.SetUserName(GameCenterPlatform.Internal_UserName());
			GameCenterPlatform.m_LocalUser.SetUserID(GameCenterPlatform.Internal_UserID());
			GameCenterPlatform.m_LocalUser.SetUnderage(GameCenterPlatform.Internal_Underage());
			GameCenterPlatform.m_LocalUser.SetImage(GameCenterPlatform.Internal_UserImage());
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(new AchievementDescription[0]);
				}
			}
			else
			{
				GameCenterPlatform.Internal_LoadAchievementDescriptions(callback);
			}
		}

		public void ReportProgress(string id, double progress, Action<bool> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				GameCenterPlatform.Internal_ReportProgress(id, progress, callback);
			}
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(new Achievement[0]);
				}
			}
			else
			{
				GameCenterPlatform.Internal_LoadAchievements(callback);
			}
		}

		public void ReportScore(long score, string board, Action<bool> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				GameCenterPlatform.Internal_ReportScore(score, board, callback);
			}
		}

		public void LoadScores(string category, Action<IScore[]> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(new Score[0]);
				}
			}
			else
			{
				GameCenterPlatform.Internal_LoadScores(category, callback);
			}
		}

		public void LoadScores(ILeaderboard board, Action<bool> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				Leaderboard leaderboard = (Leaderboard)board;
				GcLeaderboard gcLeaderboard = new GcLeaderboard(leaderboard);
				GameCenterPlatform.m_GcBoards.Add(gcLeaderboard);
				string[] array = leaderboard.GetUserFilter();
				if (array.Length == 0)
				{
					array = null;
				}
				gcLeaderboard.Internal_LoadScores(board.id, board.range.from, board.range.count, array, (int)board.userScope, (int)board.timeScope, callback);
			}
		}

		[RequiredByNativeCode]
		private static void LeaderboardCallbackWrapper(Action<bool> callback, bool success)
		{
			if (callback != null)
			{
				callback(success);
			}
		}

		public bool GetLoading(ILeaderboard board)
		{
			bool result;
			if (!this.VerifyAuthentication())
			{
				result = false;
			}
			else
			{
				foreach (GcLeaderboard current in GameCenterPlatform.m_GcBoards)
				{
					if (current.Contains((Leaderboard)board))
					{
						result = current.Loading();
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private bool VerifyAuthentication()
		{
			bool result;
			if (!this.localUser.authenticated)
			{
				Debug.Log("Must authenticate first");
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public void ShowAchievementsUI()
		{
			if (this.VerifyAuthentication())
			{
				GameCenterPlatform.Internal_ShowAchievementsUI();
			}
		}

		public void ShowLeaderboardUI()
		{
			if (this.VerifyAuthentication())
			{
				GameCenterPlatform.Internal_ShowLeaderboardUI();
			}
		}

		[RequiredByNativeCode]
		private static void ClearUsers(int size)
		{
			GameCenterPlatform.SafeClearArray(ref GameCenterPlatform.s_users, size);
		}

		[RequiredByNativeCode]
		private static void SetUser(GcUserProfileData data, int number)
		{
			data.AddToArray(ref GameCenterPlatform.s_users, number);
		}

		[RequiredByNativeCode]
		private static void SetUserImage(Texture2D texture, int number)
		{
			GameCenterPlatform.SafeSetUserImage(ref GameCenterPlatform.s_users, texture, number);
		}

		[RequiredByNativeCode]
		private static void TriggerUsersCallbackWrapper(Action<IUserProfile[]> callback)
		{
			if (callback != null)
			{
				callback(GameCenterPlatform.s_users);
			}
		}

		public void LoadUsers(string[] userIds, Action<IUserProfile[]> callback)
		{
			if (!this.VerifyAuthentication())
			{
				if (callback != null)
				{
					callback(new UserProfile[0]);
				}
			}
			else
			{
				GameCenterPlatform.Internal_LoadUsers(userIds, callback);
			}
		}

		[RequiredByNativeCode]
		private static void SafeSetUserImage(ref UserProfile[] array, Texture2D texture, int number)
		{
			if (array.Length <= number || number < 0)
			{
				Debug.Log("Invalid texture when setting user image");
				texture = new Texture2D(76, 76);
			}
			if (array.Length > number && number >= 0)
			{
				array[number].SetImage(texture);
			}
			else
			{
				Debug.Log("User number out of bounds when setting image");
			}
		}

		private static void SafeClearArray(ref UserProfile[] array, int size)
		{
			if (array == null || array.Length != size)
			{
				array = new UserProfile[size];
			}
		}

		public ILeaderboard CreateLeaderboard()
		{
			return new Leaderboard();
		}

		public IAchievement CreateAchievement()
		{
			return new Achievement();
		}

		[RequiredByNativeCode]
		private static void TriggerResetAchievementCallback(bool result)
		{
			if (GameCenterPlatform.s_ResetAchievements != null)
			{
				GameCenterPlatform.s_ResetAchievements(result);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_Authenticate();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_Authenticated();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_UserName();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string Internal_UserID();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool Internal_Underage();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D Internal_UserImage();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_LoadFriends(object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_LoadAchievementDescriptions(object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_LoadAchievements(object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ReportProgress(string id, double progress, object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ReportScore(long score, string category, object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_LoadScores(string category, object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ShowAchievementsUI();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ShowLeaderboardUI();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_LoadUsers(string[] userIds, object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ResetAllAchievements();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ShowDefaultAchievementBanner(bool value);

		public static void ResetAllAchievements(Action<bool> callback)
		{
			GameCenterPlatform.s_ResetAchievements = callback;
			GameCenterPlatform.Internal_ResetAllAchievements();
			Debug.Log("ResetAllAchievements - no effect in editor");
			if (callback != null)
			{
				callback(true);
			}
		}

		public static void ShowDefaultAchievementCompletionBanner(bool value)
		{
			GameCenterPlatform.Internal_ShowDefaultAchievementBanner(value);
			Debug.Log("ShowDefaultAchievementCompletionBanner - no effect in editor");
		}

		public static void ShowLeaderboardUI(string leaderboardID, TimeScope timeScope)
		{
			GameCenterPlatform.Internal_ShowSpecificLeaderboardUI(leaderboardID, (int)timeScope);
			Debug.Log("ShowLeaderboardUI - no effect in editor");
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_ShowSpecificLeaderboardUI(string leaderboardID, int timeScope);
	}
}
