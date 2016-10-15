using System;
using UnityEditor;
using UnityEditor.VersionControl;

namespace UnityEditorInternal.VersionControl
{
	public class PendingWindowContextMenu
	{
		private static bool SubmitTest(int userData)
		{
			return Provider.SubmitIsValid(null, ListControl.FromID(userData).SelectedAssets);
		}

		private static void Submit(int userData)
		{
			WindowChange.Open(ListControl.FromID(userData).SelectedAssets, true);
		}

		private static bool RevertTest(int userData)
		{
			return Provider.RevertIsValid(ListControl.FromID(userData).SelectedAssets, RevertMode.Normal);
		}

		private static void Revert(int userData)
		{
			WindowRevert.Open(ListControl.FromID(userData).SelectedAssets);
		}

		private static bool RevertUnchangedTest(int userData)
		{
			return Provider.RevertIsValid(ListControl.FromID(userData).SelectedAssets, RevertMode.Normal);
		}

		private static void RevertUnchanged(int userData)
		{
			AssetList selectedAssets = ListControl.FromID(userData).SelectedAssets;
			Provider.Revert(selectedAssets, RevertMode.Unchanged).SetCompletionAction(CompletionAction.UpdatePendingWindow);
			Provider.Status(selectedAssets);
		}

		private static bool ResolveTest(int userData)
		{
			return Provider.ResolveIsValid(ListControl.FromID(userData).SelectedAssets);
		}

		private static void Resolve(int userData)
		{
			WindowResolve.Open(ListControl.FromID(userData).SelectedAssets);
		}

		private static bool LockTest(int userData)
		{
			return Provider.LockIsValid(ListControl.FromID(userData).SelectedAssets);
		}

		private static void Lock(int userData)
		{
			AssetList selectedAssets = ListControl.FromID(userData).SelectedAssets;
			Provider.Lock(selectedAssets, true).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private static bool UnlockTest(int userData)
		{
			return Provider.UnlockIsValid(ListControl.FromID(userData).SelectedAssets);
		}

		private static void Unlock(int userData)
		{
			AssetList selectedAssets = ListControl.FromID(userData).SelectedAssets;
			Provider.Lock(selectedAssets, false).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private static bool DiffHeadTest(int userData)
		{
			return Provider.DiffIsValid(ListControl.FromID(userData).SelectedAssets);
		}

		private static void DiffHead(int userData)
		{
			Provider.DiffHead(ListControl.FromID(userData).SelectedAssets, false);
		}

		private static bool DiffHeadWithMetaTest(int userData)
		{
			return Provider.DiffIsValid(ListControl.FromID(userData).SelectedAssets);
		}

		private static void DiffHeadWithMeta(int userData)
		{
			Provider.DiffHead(ListControl.FromID(userData).SelectedAssets, true);
		}

		private static bool ShowInExplorerTest(int userData)
		{
			return ListControl.FromID(userData).SelectedAssets.Count > 0;
		}

		private static void ShowInExplorer(int userData)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				EditorApplication.ExecuteMenuItem("Assets/Reveal in Finder");
			}
			else
			{
				EditorApplication.ExecuteMenuItem("Assets/Show in Explorer");
			}
		}

		private static bool NewChangeSetTest(int userData)
		{
			return Provider.isActive;
		}

		private static void NewChangeSet(int userData)
		{
			WindowChange.Open(ListControl.FromID(userData).SelectedAssets, false);
		}
	}
}
