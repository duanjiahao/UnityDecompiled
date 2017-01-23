using System;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UnityEditorInternal.VersionControl
{
	public class AssetModificationHook
	{
		private static Asset GetStatusCachedIfPossible(string from)
		{
			Asset asset = Provider.CacheStatus(from);
			if (asset == null || asset.IsState(Asset.States.Updating))
			{
				Task task = Provider.Status(from, false);
				task.Wait();
				asset = Provider.CacheStatus(from);
			}
			return asset;
		}

		public static AssetMoveResult OnWillMoveAsset(string from, string to)
		{
			AssetMoveResult result;
			if (!Provider.enabled)
			{
				result = AssetMoveResult.DidNotMove;
			}
			else
			{
				Asset statusCachedIfPossible = AssetModificationHook.GetStatusCachedIfPossible(from);
				if (statusCachedIfPossible == null || !statusCachedIfPossible.IsUnderVersionControl)
				{
					result = AssetMoveResult.DidNotMove;
				}
				else if (statusCachedIfPossible.IsState(Asset.States.OutOfSync))
				{
					Debug.LogError("Cannot move version controlled file that is not up to date. Please get latest changes from server");
					result = AssetMoveResult.FailedMove;
				}
				else if (statusCachedIfPossible.IsState(Asset.States.DeletedRemote))
				{
					Debug.LogError("Cannot move version controlled file that is deleted on server. Please get latest changes from server");
					result = AssetMoveResult.FailedMove;
				}
				else if (statusCachedIfPossible.IsState(Asset.States.CheckedOutRemote))
				{
					Debug.LogError("Cannot move version controlled file that is checked out on server. Please get latest changes from server");
					result = AssetMoveResult.FailedMove;
				}
				else if (statusCachedIfPossible.IsState(Asset.States.LockedRemote))
				{
					Debug.LogError("Cannot move version controlled file that is locked on server. Please get latest changes from server");
					result = AssetMoveResult.FailedMove;
				}
				else
				{
					Task task = Provider.Move(from, to);
					task.Wait();
					result = (AssetMoveResult)((!task.success) ? 1 : task.resultCode);
				}
			}
			return result;
		}

		public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
		{
			AssetDeleteResult result;
			if (!Provider.enabled)
			{
				result = AssetDeleteResult.DidNotDelete;
			}
			else
			{
				Task task = Provider.Delete(assetPath);
				task.SetCompletionAction(CompletionAction.UpdatePendingWindow);
				task.Wait();
				result = ((!task.success) ? AssetDeleteResult.FailedDelete : AssetDeleteResult.DidNotDelete);
			}
			return result;
		}

		public static bool IsOpenForEdit(string assetPath, out string message)
		{
			message = "";
			bool result;
			if (!Provider.enabled)
			{
				result = true;
			}
			else if (string.IsNullOrEmpty(assetPath))
			{
				result = true;
			}
			else
			{
				Asset asset = Provider.GetAssetByPath(assetPath);
				if (asset == null)
				{
					Task task = Provider.Status(assetPath, false);
					task.Wait();
					asset = ((task.assetList.Count <= 0) ? null : task.assetList[0]);
				}
				result = (asset != null && Provider.IsOpenForEdit(asset));
			}
			return result;
		}
	}
}
