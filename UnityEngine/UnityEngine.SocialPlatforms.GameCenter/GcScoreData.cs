using System;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	[RequiredByNativeCode]
	internal struct GcScoreData
	{
		public string m_Category;

		public uint m_ValueLow;

		public int m_ValueHigh;

		public int m_Date;

		public string m_FormattedValue;

		public string m_PlayerID;

		public int m_Rank;

		public Score ToScore()
		{
			string arg_4B_0 = this.m_Category;
			long arg_4B_1 = ((long)this.m_ValueHigh << 32) + (long)((ulong)this.m_ValueLow);
			string arg_4B_2 = this.m_PlayerID;
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return new Score(arg_4B_0, arg_4B_1, arg_4B_2, dateTime.AddSeconds((double)this.m_Date), this.m_FormattedValue, this.m_Rank);
		}
	}
}
