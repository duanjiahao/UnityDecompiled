using System;

namespace UnityEngine.SocialPlatforms
{
	public interface ILocalUser : IUserProfile
	{
		IUserProfile[] friends
		{
			get;
		}

		bool authenticated
		{
			get;
		}

		bool underage
		{
			get;
		}

		void Authenticate(Action<bool> callback);

		void LoadFriends(Action<bool> callback);
	}
}
