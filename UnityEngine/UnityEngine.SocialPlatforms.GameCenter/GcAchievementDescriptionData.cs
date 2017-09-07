using System;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	[RequiredByNativeCode]
	internal struct GcAchievementDescriptionData
	{
		public string m_Identifier;

		public string m_Title;

		public Texture2D m_Image;

		public string m_AchievedDescription;

		public string m_UnachievedDescription;

		public int m_Hidden;

		public int m_Points;

		public AchievementDescription ToAchievementDescription()
		{
			return new AchievementDescription(this.m_Identifier, this.m_Title, this.m_Image, this.m_AchievedDescription, this.m_UnachievedDescription, this.m_Hidden != 0, this.m_Points);
		}
	}
}
