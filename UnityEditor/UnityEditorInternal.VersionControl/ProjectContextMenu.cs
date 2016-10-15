using System;
using UnityEditor;
using UnityEditor.VersionControl;

namespace UnityEditorInternal.VersionControl
{
	public class ProjectContextMenu
	{
		private static bool GetLatestTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.GetLatestIsValid(assetListFromSelection);
		}

		private static void GetLatest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.GetLatest(assetListFromSelection).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private static bool SubmitTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.SubmitIsValid(null, assetListFromSelection);
		}

		private static void Submit(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			WindowChange.Open(assetListFromSelection, true);
		}

		private static bool CheckOutTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Both);
		}

		private static void CheckOut(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Checkout(assetListFromSelection, CheckoutMode.Both);
		}

		private static bool CheckOutAssetTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Asset);
		}

		private static void CheckOutAsset(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Checkout(assetListFromSelection, CheckoutMode.Asset);
		}

		private static bool CheckOutMetaTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Meta);
		}

		private static void CheckOutMeta(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Checkout(assetListFromSelection, CheckoutMode.Meta);
		}

		private static bool CheckOutBothTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Both);
		}

		private static void CheckOutBoth(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Checkout(assetListFromSelection, CheckoutMode.Both);
		}

		private static bool MarkAddTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.AddIsValid(assetListFromSelection);
		}

		private static void MarkAdd(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Add(assetListFromSelection, true).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private static bool RevertTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.RevertIsValid(assetListFromSelection, RevertMode.Normal);
		}

		private static void Revert(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			WindowRevert.Open(assetListFromSelection);
		}

		private static bool RevertUnchangedTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.RevertIsValid(assetListFromSelection, RevertMode.Normal);
		}

		private static void RevertUnchanged(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Revert(assetListFromSelection, RevertMode.Unchanged).SetCompletionAction(CompletionAction.UpdatePendingWindow);
			Provider.Status(assetListFromSelection);
		}

		private static bool ResolveTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.ResolveIsValid(assetListFromSelection);
		}

		private static void Resolve(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			WindowResolve.Open(assetListFromSelection);
		}

		private static bool LockTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.LockIsValid(assetListFromSelection);
		}

		private static void Lock(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Lock(assetListFromSelection, true).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private static bool UnlockTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.UnlockIsValid(assetListFromSelection);
		}

		private static void Unlock(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.Lock(assetListFromSelection, false).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private static bool DiffHeadTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.DiffIsValid(assetListFromSelection);
		}

		private static void DiffHead(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.DiffHead(assetListFromSelection, false);
		}

		private static bool DiffHeadWithMetaTest(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			return Provider.enabled && Provider.DiffIsValid(assetListFromSelection);
		}

		private static void DiffHeadWithMeta(MenuCommand cmd)
		{
			AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
			Provider.DiffHead(assetListFromSelection, true);
		}
	}
}
