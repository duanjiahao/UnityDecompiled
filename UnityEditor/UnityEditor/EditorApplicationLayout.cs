using System;

namespace UnityEditor
{
	internal class EditorApplicationLayout
	{
		private static GameView m_GameView;

		private static View m_RootSplit;

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
			if (EditorApplicationLayout.m_GameView == null)
			{
				return;
			}
			if (EditorApplicationLayout.m_GameView.maximizeOnPlay)
			{
				DockArea dockArea = EditorApplicationLayout.m_GameView.m_Parent as DockArea;
				if (dockArea != null)
				{
					ContainerWindow window = dockArea.actualView.m_Parent.window;
					if (!window.maximized)
					{
						EditorApplicationLayout.m_RootSplit = WindowLayout.MaximizePrepare(dockArea.actualView);
					}
				}
			}
			EditorApplicationLayout.m_GameView.m_Parent.SetAsStartView();
			Toolbar.RepaintToolbar();
		}

		internal static void FinalizePlaymodeLayout()
		{
			if (EditorApplicationLayout.m_GameView != null)
			{
				if (EditorApplicationLayout.m_RootSplit != null)
				{
					WindowLayout.MaximizePresent(EditorApplicationLayout.m_GameView, EditorApplicationLayout.m_RootSplit);
				}
				EditorApplicationLayout.m_GameView.m_Parent.ClearStartView();
			}
			EditorApplicationLayout.Clear();
		}

		private static void Clear()
		{
			EditorApplicationLayout.m_RootSplit = null;
			EditorApplicationLayout.m_GameView = null;
		}
	}
}
