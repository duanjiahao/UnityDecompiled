using System;

namespace UnityEditor
{
	internal class EditorApplicationLayout
	{
		private static GameView m_GameView = null;

		private static bool m_MaximizePending = false;

		internal static bool IsInitializingPlaymodeLayout()
		{
			return EditorApplicationLayout.m_GameView != null;
		}

		internal static void SetPlaymodeLayout()
		{
			EditorApplicationLayout.InitPlaymodeLayout();
			EditorApplicationLayout.FinalizePlaymodeLayout();
		}

		internal static void SetStopmodeLayout()
		{
			WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(false);
			Toolbar.RepaintToolbar();
		}

		internal static void SetPausemodeLayout()
		{
			EditorApplicationLayout.SetStopmodeLayout();
		}

		internal static void InitPlaymodeLayout()
		{
			EditorApplicationLayout.m_GameView = (WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(true) as GameView);
			if (!(EditorApplicationLayout.m_GameView == null))
			{
				if (EditorApplicationLayout.m_GameView.maximizeOnPlay)
				{
					DockArea dockArea = EditorApplicationLayout.m_GameView.m_Parent as DockArea;
					if (dockArea != null)
					{
						EditorApplicationLayout.m_MaximizePending = WindowLayout.MaximizePrepare(dockArea.actualView);
					}
				}
				EditorApplicationLayout.m_GameView.m_Parent.SetAsStartView();
				Toolbar.RepaintToolbar();
			}
		}

		internal static void FinalizePlaymodeLayout()
		{
			if (EditorApplicationLayout.m_GameView != null)
			{
				if (EditorApplicationLayout.m_MaximizePending)
				{
					WindowLayout.MaximizePresent(EditorApplicationLayout.m_GameView);
				}
				EditorApplicationLayout.m_GameView.m_Parent.ClearStartView();
			}
			EditorApplicationLayout.Clear();
		}

		private static void Clear()
		{
			EditorApplicationLayout.m_MaximizePending = false;
			EditorApplicationLayout.m_GameView = null;
		}
	}
}
