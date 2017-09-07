using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms.GameCenter
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class GcLeaderboard
	{
		private IntPtr m_InternalLeaderboard;

		private Leaderboard m_GenericLeaderboard;

		internal GcLeaderboard(Leaderboard board)
		{
			this.m_GenericLeaderboard = board;
		}

		~GcLeaderboard()
		{
			this.Dispose();
		}

		internal bool Contains(Leaderboard board)
		{
			return this.m_GenericLeaderboard == board;
		}

		internal void SetScores(GcScoreData[] scoreDatas)
		{
			if (this.m_GenericLeaderboard != null)
			{
				Score[] array = new Score[scoreDatas.Length];
				for (int i = 0; i < scoreDatas.Length; i++)
				{
					array[i] = scoreDatas[i].ToScore();
				}
				this.m_GenericLeaderboard.SetScores(array);
			}
		}

		internal void SetLocalScore(GcScoreData scoreData)
		{
			if (this.m_GenericLeaderboard != null)
			{
				this.m_GenericLeaderboard.SetLocalUserScore(scoreData.ToScore());
			}
		}

		internal void SetMaxRange(uint maxRange)
		{
			if (this.m_GenericLeaderboard != null)
			{
				this.m_GenericLeaderboard.SetMaxRange(maxRange);
			}
		}

		internal void SetTitle(string title)
		{
			if (this.m_GenericLeaderboard != null)
			{
				this.m_GenericLeaderboard.SetTitle(title);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Internal_LoadScores(string category, int from, int count, string[] userIDs, int playerScope, int timeScope, object callback);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool Loading();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void Dispose();
	}
}
