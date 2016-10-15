using System;

namespace UnityEngine.SocialPlatforms.Impl
{
	public class UserProfile : IUserProfile
	{
		protected string m_UserName;

		protected string m_ID;

		protected bool m_IsFriend;

		protected UserState m_State;

		protected Texture2D m_Image;

		public string userName
		{
			get
			{
				return this.m_UserName;
			}
		}

		public string id
		{
			get
			{
				return this.m_ID;
			}
		}

		public bool isFriend
		{
			get
			{
				return this.m_IsFriend;
			}
		}

		public UserState state
		{
			get
			{
				return this.m_State;
			}
		}

		public Texture2D image
		{
			get
			{
				return this.m_Image;
			}
		}

		public UserProfile()
		{
			this.m_UserName = "Uninitialized";
			this.m_ID = "0";
			this.m_IsFriend = false;
			this.m_State = UserState.Offline;
			this.m_Image = new Texture2D(32, 32);
		}

		public UserProfile(string name, string id, bool friend) : this(name, id, friend, UserState.Offline, new Texture2D(0, 0))
		{
		}

		public UserProfile(string name, string id, bool friend, UserState state, Texture2D image)
		{
			this.m_UserName = name;
			this.m_ID = id;
			this.m_IsFriend = friend;
			this.m_State = state;
			this.m_Image = image;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.id,
				" - ",
				this.userName,
				" - ",
				this.isFriend,
				" - ",
				this.state
			});
		}

		public void SetUserName(string name)
		{
			this.m_UserName = name;
		}

		public void SetUserID(string id)
		{
			this.m_ID = id;
		}

		public void SetImage(Texture2D image)
		{
			this.m_Image = image;
		}

		public void SetIsFriend(bool value)
		{
			this.m_IsFriend = value;
		}

		public void SetState(UserState state)
		{
			this.m_State = state;
		}
	}
}
