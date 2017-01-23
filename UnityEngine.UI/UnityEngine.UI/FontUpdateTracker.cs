using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UI
{
	public static class FontUpdateTracker
	{
		private static Dictionary<Font, HashSet<Text>> m_Tracked = new Dictionary<Font, HashSet<Text>>();

		[CompilerGenerated]
		private static Action<Font> <>f__mg$cache0;

		[CompilerGenerated]
		private static Action<Font> <>f__mg$cache1;

		public static void TrackText(Text t)
		{
			if (!(t.font == null))
			{
				HashSet<Text> hashSet;
				FontUpdateTracker.m_Tracked.TryGetValue(t.font, out hashSet);
				if (hashSet == null)
				{
					if (FontUpdateTracker.m_Tracked.Count == 0)
					{
						if (FontUpdateTracker.<>f__mg$cache0 == null)
						{
							FontUpdateTracker.<>f__mg$cache0 = new Action<Font>(FontUpdateTracker.RebuildForFont);
						}
						Font.textureRebuilt += FontUpdateTracker.<>f__mg$cache0;
					}
					hashSet = new HashSet<Text>();
					FontUpdateTracker.m_Tracked.Add(t.font, hashSet);
				}
				if (!hashSet.Contains(t))
				{
					hashSet.Add(t);
				}
			}
		}

		private static void RebuildForFont(Font f)
		{
			HashSet<Text> hashSet;
			FontUpdateTracker.m_Tracked.TryGetValue(f, out hashSet);
			if (hashSet != null)
			{
				foreach (Text current in hashSet)
				{
					current.FontTextureChanged();
				}
			}
		}

		public static void UntrackText(Text t)
		{
			if (!(t.font == null))
			{
				HashSet<Text> hashSet;
				FontUpdateTracker.m_Tracked.TryGetValue(t.font, out hashSet);
				if (hashSet != null)
				{
					hashSet.Remove(t);
					if (hashSet.Count == 0)
					{
						FontUpdateTracker.m_Tracked.Remove(t.font);
						if (FontUpdateTracker.m_Tracked.Count == 0)
						{
							if (FontUpdateTracker.<>f__mg$cache1 == null)
							{
								FontUpdateTracker.<>f__mg$cache1 = new Action<Font>(FontUpdateTracker.RebuildForFont);
							}
							Font.textureRebuilt -= FontUpdateTracker.<>f__mg$cache1;
						}
					}
				}
			}
		}
	}
}
