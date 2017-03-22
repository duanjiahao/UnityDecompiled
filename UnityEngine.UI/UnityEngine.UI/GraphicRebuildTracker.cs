using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI.Collections;

namespace UnityEngine.UI
{
	public static class GraphicRebuildTracker
	{
		private static IndexedSet<Graphic> m_Tracked = new IndexedSet<Graphic>();

		private static bool s_Initialized;

		[CompilerGenerated]
		private static CanvasRenderer.OnRequestRebuild <>f__mg$cache0;

		public static void TrackGraphic(Graphic g)
		{
			if (!GraphicRebuildTracker.s_Initialized)
			{
				if (GraphicRebuildTracker.<>f__mg$cache0 == null)
				{
					GraphicRebuildTracker.<>f__mg$cache0 = new CanvasRenderer.OnRequestRebuild(GraphicRebuildTracker.OnRebuildRequested);
				}
				CanvasRenderer.onRequestRebuild += GraphicRebuildTracker.<>f__mg$cache0;
				GraphicRebuildTracker.s_Initialized = true;
			}
			GraphicRebuildTracker.m_Tracked.AddUnique(g);
		}

		public static void UnTrackGraphic(Graphic g)
		{
			GraphicRebuildTracker.m_Tracked.Remove(g);
		}

		private static void OnRebuildRequested()
		{
			StencilMaterial.ClearAll();
			for (int i = 0; i < GraphicRebuildTracker.m_Tracked.Count; i++)
			{
				GraphicRebuildTracker.m_Tracked[i].OnRebuildRequested();
			}
		}
	}
}
