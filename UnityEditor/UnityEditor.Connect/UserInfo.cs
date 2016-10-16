using System;

namespace UnityEditor.Connect
{
	internal struct UserInfo
	{
		private int m_Valid;

		private string m_UserId;

		private string m_UserName;

		private string m_DisplayName;

		private string m_PrimaryOrg;

		private int m_Whitelisted;

		private string m_OrganizationForeignKeys;

		private string m_AccessToken;

		private int m_AccessTokenValiditySeconds;

		public bool valid
		{
			get
			{
				return this.m_Valid != 0;
			}
		}

		public string userId
		{
			get
			{
				return this.m_UserId;
			}
		}

		public string userName
		{
			get
			{
				return this.m_UserName;
			}
		}

		public string displayName
		{
			get
			{
				return this.m_DisplayName;
			}
		}

		public string primaryOrg
		{
			get
			{
				return this.m_PrimaryOrg;
			}
		}

		public bool whitelisted
		{
			get
			{
				return this.m_Whitelisted != 0;
			}
		}

		public string organizationForeignKeys
		{
			get
			{
				return this.m_OrganizationForeignKeys;
			}
		}

		public string accessToken
		{
			get
			{
				return this.m_AccessToken;
			}
		}

		public int accessTokenValiditySeconds
		{
			get
			{
				return this.m_AccessTokenValiditySeconds;
			}
		}
	}
}
