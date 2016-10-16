using System;

namespace UnityEngine.SocialPlatforms
{
	public interface IScore
	{
		string leaderboardID
		{
			get;
			set;
		}

		long value
		{
			get;
			set;
		}

		DateTime date
		{
			get;
		}

		string formattedValue
		{
			get;
		}

		string userID
		{
			get;
		}

		int rank
		{
			get;
		}

		void ReportScore(Action<bool> callback);
	}
}
