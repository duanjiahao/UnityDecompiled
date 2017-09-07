using System;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	[RequiredByNativeCode]
	internal struct GcUserProfileData
	{
		public string userName;

		public string userID;

		public int isFriend;

		public Texture2D image;

		public UserProfile ToUserProfile()
		{
			return new UserProfile(this.userName, this.userID, this.isFriend == 1, UserState.Offline, this.image);
		}

		public void AddToArray(ref UserProfile[] array, int number)
		{
			if (array.Length > number && number >= 0)
			{
				array[number] = this.ToUserProfile();
			}
			else
			{
				Debug.Log("Index number out of bounds when setting user data");
			}
		}
	}
}
