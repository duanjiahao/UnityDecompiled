using System;

namespace UnityEngine.SocialPlatforms
{
	public interface ILeaderboard
	{
		bool loading
		{
			get;
		}

		string id
		{
			get;
			set;
		}

		UserScope userScope
		{
			get;
			set;
		}

		Range range
		{
			get;
			set;
		}

		TimeScope timeScope
		{
			get;
			set;
		}

		IScore localUserScore
		{
			get;
		}

		uint maxRange
		{
			get;
		}

		IScore[] scores
		{
			get;
		}

		string title
		{
			get;
		}

		void SetUserFilter(string[] userIDs);

		void LoadScores(Action<bool> callback);
	}
}
