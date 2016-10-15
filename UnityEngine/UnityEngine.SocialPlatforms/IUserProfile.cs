using System;

namespace UnityEngine.SocialPlatforms
{
	public interface IUserProfile
	{
		string userName
		{
			get;
		}

		string id
		{
			get;
		}

		bool isFriend
		{
			get;
		}

		UserState state
		{
			get;
		}

		Texture2D image
		{
			get;
		}
	}
}
