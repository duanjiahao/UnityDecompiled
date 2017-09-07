using System;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	[RequiredByNativeCode]
	internal struct GcAchievementData
	{
		public string m_Identifier;

		public double m_PercentCompleted;

		public int m_Completed;

		public int m_Hidden;

		public int m_LastReportedDate;

		public Achievement ToAchievement()
		{
			string arg_51_0 = this.m_Identifier;
			double arg_51_1 = this.m_PercentCompleted;
			bool arg_51_2 = this.m_Completed != 0;
			bool arg_51_3 = this.m_Hidden != 0;
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return new Achievement(arg_51_0, arg_51_1, arg_51_2, arg_51_3, dateTime.AddSeconds((double)this.m_LastReportedDate));
		}
	}
}
