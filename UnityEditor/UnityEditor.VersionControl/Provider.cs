using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	public sealed class Provider
	{
		public static extern bool enabled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isActive
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool requiresNetwork
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool hasChangelistSupport
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool hasCheckoutSupport
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool isVersioningFolders
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern OnlineState onlineState
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string offlineReason
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Task activeTask
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern Texture2D overlayAtlas
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern CustomCommand[] customCommands
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static Rect GetAtlasRectForState(int state)
		{
			Rect result;
			Provider.INTERNAL_CALL_GetAtlasRectForState(state, out result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetAtlasRectForState(int state, out Rect value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Plugin GetActivePlugin();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ConfigField[] GetActiveConfigFields();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsCustomCommandEnabled(string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Asset Internal_CacheStatus(string assetPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Status(Asset[] assets, bool recursively);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_StatusStrings(string[] assetsProjectPaths, bool recursively);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_StatusAbsolutePath(string assetPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_CheckoutIsValid(Asset[] assets, CheckoutMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Checkout(Asset[] assets, CheckoutMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_CheckoutStrings(string[] assets, CheckoutMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_PromptAndCheckoutIfNeeded(string[] assets, string promptIfCheckoutIsNeeded);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Delete(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_DeleteAtProjectPath(string assetProjectPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_MoveAsStrings(string from, string to);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_AddIsValid(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Add(Asset[] assets, bool recursive);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_DeleteChangeSetsIsValid(ChangeSet[] changes);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_DeleteChangeSets(ChangeSet[] changesets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_RevertChangeSets(ChangeSet[] changesets, RevertMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_SubmitIsValid(ChangeSet changeset, Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Submit(ChangeSet changeset, Asset[] assets, string description, bool saveOnly);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_DiffIsValid(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_DiffHead(Asset[] assets, bool includingMetaFiles);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_ResolveIsValid(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Resolve(Asset[] assets, ResolveMethod resolveMethod);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Merge(Asset[] assets, MergeMethod method);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_LockIsValid(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_UnlockIsValid(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Lock(Asset[] assets, bool locked);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_RevertIsValid(Asset[] assets, RevertMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_Revert(Asset[] assets, RevertMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_GetLatestIsValid(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_GetLatest(Asset[] assets);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_SetFileMode(Asset[] assets, FileMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_SetFileModeStrings(string[] assets, FileMode mode);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_ChangeSetDescription(ChangeSet changeset);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_ChangeSetStatus(ChangeSet changeset);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Task ChangeSets();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_ChangeSetMove(Asset[] assets, ChangeSet target);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Task Incoming();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Task Internal_IncomingChangeSetAssets(ChangeSet changeset);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsOpenForEdit(Asset asset);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Task UpdateSettings();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Asset GetAssetByPath(string unityPath);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Asset GetAssetByGUID(string guid);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Asset[] Internal_GetAssetArrayFromSelection();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GenerateID();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearCache();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InvalidateCache();

		internal static Asset CacheStatus(string assetPath)
		{
			return Provider.Internal_CacheStatus(assetPath);
		}

		public static Task Status(AssetList assets)
		{
			return Provider.Internal_Status(assets.ToArray(), true);
		}

		public static Task Status(Asset asset)
		{
			return Provider.Internal_Status(new Asset[]
			{
				asset
			}, true);
		}

		public static Task Status(AssetList assets, bool recursively)
		{
			return Provider.Internal_Status(assets.ToArray(), recursively);
		}

		public static Task Status(Asset asset, bool recursively)
		{
			return Provider.Internal_Status(new Asset[]
			{
				asset
			}, recursively);
		}

		public static Task Status(string[] assets)
		{
			return Provider.Internal_StatusStrings(assets, true);
		}

		public static Task Status(string[] assets, bool recursively)
		{
			return Provider.Internal_StatusStrings(assets, recursively);
		}

		public static Task Status(string asset)
		{
			return Provider.Internal_StatusStrings(new string[]
			{
				asset
			}, true);
		}

		public static Task Status(string asset, bool recursively)
		{
			return Provider.Internal_StatusStrings(new string[]
			{
				asset
			}, recursively);
		}

		public static Task Move(string from, string to)
		{
			return Provider.Internal_MoveAsStrings(from, to);
		}

		public static bool CheckoutIsValid(AssetList assets)
		{
			return Provider.CheckoutIsValid(assets, CheckoutMode.Exact);
		}

		public static bool CheckoutIsValid(AssetList assets, CheckoutMode mode)
		{
			return Provider.Internal_CheckoutIsValid(assets.ToArray(), mode);
		}

		public static Task Checkout(AssetList assets, CheckoutMode mode)
		{
			return Provider.Internal_Checkout(assets.ToArray(), mode);
		}

		public static Task Checkout(string[] assets, CheckoutMode mode)
		{
			return Provider.Internal_CheckoutStrings(assets, mode);
		}

		public static Task Checkout(UnityEngine.Object[] assets, CheckoutMode mode)
		{
			AssetList assetList = new AssetList();
			for (int i = 0; i < assets.Length; i++)
			{
				UnityEngine.Object assetObject = assets[i];
				string assetPath = AssetDatabase.GetAssetPath(assetObject);
				Asset assetByPath = Provider.GetAssetByPath(assetPath);
				assetList.Add(assetByPath);
			}
			return Provider.Internal_Checkout(assetList.ToArray(), mode);
		}

		public static bool CheckoutIsValid(Asset asset)
		{
			return Provider.CheckoutIsValid(asset, CheckoutMode.Exact);
		}

		public static bool CheckoutIsValid(Asset asset, CheckoutMode mode)
		{
			return Provider.Internal_CheckoutIsValid(new Asset[]
			{
				asset
			}, mode);
		}

		public static Task Checkout(Asset asset, CheckoutMode mode)
		{
			return Provider.Internal_Checkout(new Asset[]
			{
				asset
			}, mode);
		}

		public static Task Checkout(string asset, CheckoutMode mode)
		{
			return Provider.Internal_CheckoutStrings(new string[]
			{
				asset
			}, mode);
		}

		public static Task Checkout(UnityEngine.Object asset, CheckoutMode mode)
		{
			string assetPath = AssetDatabase.GetAssetPath(asset);
			Asset assetByPath = Provider.GetAssetByPath(assetPath);
			return Provider.Internal_Checkout(new Asset[]
			{
				assetByPath
			}, mode);
		}

		internal static bool PromptAndCheckoutIfNeeded(string[] assets, string promptIfCheckoutIsNeeded)
		{
			return Provider.Internal_PromptAndCheckoutIfNeeded(assets, promptIfCheckoutIsNeeded);
		}

		public static Task Delete(string assetProjectPath)
		{
			return Provider.Internal_DeleteAtProjectPath(assetProjectPath);
		}

		public static Task Delete(AssetList assets)
		{
			return Provider.Internal_Delete(assets.ToArray());
		}

		public static Task Delete(Asset asset)
		{
			return Provider.Internal_Delete(new Asset[]
			{
				asset
			});
		}

		public static bool AddIsValid(AssetList assets)
		{
			return Provider.Internal_AddIsValid(assets.ToArray());
		}

		public static Task Add(AssetList assets, bool recursive)
		{
			return Provider.Internal_Add(assets.ToArray(), recursive);
		}

		public static Task Add(Asset asset, bool recursive)
		{
			return Provider.Internal_Add(new Asset[]
			{
				asset
			}, recursive);
		}

		public static bool DeleteChangeSetsIsValid(ChangeSets changesets)
		{
			return Provider.Internal_DeleteChangeSetsIsValid(changesets.ToArray());
		}

		public static Task DeleteChangeSets(ChangeSets changesets)
		{
			return Provider.Internal_DeleteChangeSets(changesets.ToArray());
		}

		internal static Task RevertChangeSets(ChangeSets changesets, RevertMode mode)
		{
			return Provider.Internal_RevertChangeSets(changesets.ToArray(), mode);
		}

		public static bool SubmitIsValid(ChangeSet changeset, AssetList assets)
		{
			return Provider.Internal_SubmitIsValid(changeset, (assets == null) ? null : assets.ToArray());
		}

		public static Task Submit(ChangeSet changeset, AssetList list, string description, bool saveOnly)
		{
			return Provider.Internal_Submit(changeset, (list == null) ? null : list.ToArray(), description, saveOnly);
		}

		public static bool DiffIsValid(AssetList assets)
		{
			Asset[] assets2 = assets.ToArray();
			return Provider.Internal_DiffIsValid(assets2);
		}

		public static Task DiffHead(AssetList assets, bool includingMetaFiles)
		{
			return Provider.Internal_DiffHead(assets.ToArray(), includingMetaFiles);
		}

		public static bool ResolveIsValid(AssetList assets)
		{
			Asset[] assets2 = assets.ToArray();
			return Provider.Internal_ResolveIsValid(assets2);
		}

		public static Task Resolve(AssetList assets, ResolveMethod resolveMethod)
		{
			return Provider.Internal_Resolve(assets.ToArray(), resolveMethod);
		}

		public static Task Merge(AssetList assets, MergeMethod method)
		{
			return Provider.Internal_Merge(assets.ToArray(), method);
		}

		public static bool LockIsValid(AssetList assets)
		{
			return Provider.Internal_LockIsValid(assets.ToArray());
		}

		public static bool LockIsValid(Asset asset)
		{
			return Provider.Internal_LockIsValid(new Asset[]
			{
				asset
			});
		}

		public static bool UnlockIsValid(AssetList assets)
		{
			return Provider.Internal_UnlockIsValid(assets.ToArray());
		}

		public static bool UnlockIsValid(Asset asset)
		{
			return Provider.Internal_UnlockIsValid(new Asset[]
			{
				asset
			});
		}

		public static Task Lock(AssetList assets, bool locked)
		{
			return Provider.Internal_Lock(assets.ToArray(), locked);
		}

		public static Task Lock(Asset asset, bool locked)
		{
			return Provider.Internal_Lock(new Asset[]
			{
				asset
			}, locked);
		}

		public static bool RevertIsValid(AssetList assets, RevertMode mode)
		{
			return Provider.Internal_RevertIsValid(assets.ToArray(), mode);
		}

		public static Task Revert(AssetList assets, RevertMode mode)
		{
			return Provider.Internal_Revert(assets.ToArray(), mode);
		}

		public static bool RevertIsValid(Asset asset, RevertMode mode)
		{
			return Provider.Internal_RevertIsValid(new Asset[]
			{
				asset
			}, mode);
		}

		public static Task Revert(Asset asset, RevertMode mode)
		{
			return Provider.Internal_Revert(new Asset[]
			{
				asset
			}, mode);
		}

		public static bool GetLatestIsValid(AssetList assets)
		{
			return Provider.Internal_GetLatestIsValid(assets.ToArray());
		}

		public static bool GetLatestIsValid(Asset asset)
		{
			return Provider.Internal_GetLatestIsValid(new Asset[]
			{
				asset
			});
		}

		public static Task GetLatest(AssetList assets)
		{
			return Provider.Internal_GetLatest(assets.ToArray());
		}

		public static Task GetLatest(Asset asset)
		{
			return Provider.Internal_GetLatest(new Asset[]
			{
				asset
			});
		}

		internal static Task SetFileMode(AssetList assets, FileMode mode)
		{
			return Provider.Internal_SetFileMode(assets.ToArray(), mode);
		}

		internal static Task SetFileMode(string[] assets, FileMode mode)
		{
			return Provider.Internal_SetFileModeStrings(assets, mode);
		}

		public static Task ChangeSetDescription(ChangeSet changeset)
		{
			return Provider.Internal_ChangeSetDescription(changeset);
		}

		public static Task ChangeSetStatus(ChangeSet changeset)
		{
			return Provider.Internal_ChangeSetStatus(changeset);
		}

		public static Task ChangeSetStatus(string changesetID)
		{
			ChangeSet changeset = new ChangeSet(string.Empty, changesetID);
			return Provider.Internal_ChangeSetStatus(changeset);
		}

		public static Task IncomingChangeSetAssets(ChangeSet changeset)
		{
			return Provider.Internal_IncomingChangeSetAssets(changeset);
		}

		public static Task IncomingChangeSetAssets(string changesetID)
		{
			ChangeSet changeset = new ChangeSet(string.Empty, changesetID);
			return Provider.Internal_IncomingChangeSetAssets(changeset);
		}

		public static Task ChangeSetMove(AssetList assets, ChangeSet changeset)
		{
			return Provider.Internal_ChangeSetMove(assets.ToArray(), changeset);
		}

		public static Task ChangeSetMove(Asset asset, ChangeSet changeset)
		{
			return Provider.Internal_ChangeSetMove(new Asset[]
			{
				asset
			}, changeset);
		}

		public static Task ChangeSetMove(AssetList assets, string changesetID)
		{
			ChangeSet target = new ChangeSet(string.Empty, changesetID);
			return Provider.Internal_ChangeSetMove(assets.ToArray(), target);
		}

		public static Task ChangeSetMove(Asset asset, string changesetID)
		{
			ChangeSet target = new ChangeSet(string.Empty, changesetID);
			return Provider.Internal_ChangeSetMove(new Asset[]
			{
				asset
			}, target);
		}

		public static AssetList GetAssetListFromSelection()
		{
			AssetList assetList = new AssetList();
			Asset[] array = Provider.Internal_GetAssetArrayFromSelection();
			Asset[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Asset item = array2[i];
				assetList.Add(item);
			}
			return assetList;
		}
	}
}
