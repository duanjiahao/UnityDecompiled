using System;
using System.Collections.Generic;
using UnityEditor.Web;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.Collaboration
{
	internal static class SoftLockData
	{
		internal delegate void OnSoftlockUpdate(string[] assetGUIDs);

		internal static SoftLockData.OnSoftlockUpdate SoftlockSubscriber = null;

		public static void SetSoftlockChanges(string[] assetGUIDs)
		{
			if (SoftLockData.SoftlockSubscriber != null)
			{
				SoftLockData.SoftlockSubscriber(assetGUIDs);
			}
		}

		public static bool AllowsSoftLocks(UnityEngine.Object unityObject)
		{
			if (unityObject == null)
			{
				throw new ArgumentNullException("unityObject");
			}
			return unityObject.GetType().Equals(typeof(SceneAsset)) || SoftLockData.IsPrefab(unityObject);
		}

		public static bool IsPrefab(UnityEngine.Object unityObject)
		{
			PrefabType prefabType = PrefabUtility.GetPrefabType(unityObject);
			return prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.Prefab;
		}

		public static bool IsPrefab(string assetGUID)
		{
			bool result = false;
			UnityEngine.Object unityObject;
			if (AssetAccess.TryGetAssetFromGUID(assetGUID, out unityObject))
			{
				result = SoftLockData.IsPrefab(unityObject);
			}
			return result;
		}

		private static bool TryHasSoftLocks(Scene scene, out bool hasSoftLocks)
		{
			string assetGuid = AssetDatabase.AssetPathToGUID(scene.path);
			return SoftLockData.TryHasSoftLocks(assetGuid, out hasSoftLocks);
		}

		public static bool TryHasSoftLocks(UnityEngine.Object objectWithGUID, out bool hasSoftLocks)
		{
			string assetGuid = null;
			AssetAccess.TryGetAssetGUIDFromObject(objectWithGUID, out assetGuid);
			return SoftLockData.TryHasSoftLocks(assetGuid, out hasSoftLocks);
		}

		public static bool TryHasSoftLocks(string assetGuid, out bool hasSoftLocks)
		{
			hasSoftLocks = false;
			bool result = false;
			int num = 0;
			if (SoftLockData.TryGetSoftlockCount(assetGuid, out num))
			{
				result = true;
				hasSoftLocks = (num > 0);
			}
			return result;
		}

		public static bool TryGetSoftlockCount(Scene scene, out int count)
		{
			count = 0;
			bool result;
			if (!scene.IsValid())
			{
				result = false;
			}
			else
			{
				string assetGuid = AssetDatabase.AssetPathToGUID(scene.path);
				bool flag = SoftLockData.TryGetSoftlockCount(assetGuid, out count);
				result = flag;
			}
			return result;
		}

		public static bool TryGetSoftlockCount(UnityEngine.Object objectWithGUID, out int count)
		{
			string assetGuid = null;
			AssetAccess.TryGetAssetGUIDFromObject(objectWithGUID, out assetGuid);
			return SoftLockData.TryGetSoftlockCount(assetGuid, out count);
		}

		public static bool TryGetSoftlockCount(string assetGuid, out int count)
		{
			bool result = false;
			count = 0;
			List<SoftLock> list = null;
			if (SoftLockData.TryGetLocksOnAssetGUID(assetGuid, out list))
			{
				count = list.Count;
				result = true;
			}
			return result;
		}

		private static bool TryGetLocksOnObject(UnityEngine.Object objectWithGUID, out List<SoftLock> softLocks)
		{
			bool result = false;
			string assetGuid = null;
			if (AssetAccess.TryGetAssetGUIDFromObject(objectWithGUID, out assetGuid))
			{
				result = SoftLockData.TryGetLocksOnAssetGUID(assetGuid, out softLocks);
			}
			else
			{
				softLocks = new List<SoftLock>();
			}
			return result;
		}

		public static bool TryGetLocksOnAssetGUID(string assetGuid, out List<SoftLock> softLocks)
		{
			if (assetGuid == null)
			{
				throw new ArgumentNullException("assetGuid");
			}
			bool result;
			if (!CollabAccess.Instance.IsServiceEnabled() || assetGuid.Length == 0)
			{
				softLocks = new List<SoftLock>();
				result = false;
			}
			else
			{
				SoftLock[] softLocks2 = Collab.instance.GetSoftLocks(assetGuid);
				softLocks = new List<SoftLock>();
				for (int i = 0; i < softLocks2.Length; i++)
				{
					softLocks.Add(softLocks2[i]);
				}
				result = true;
			}
			return result;
		}
	}
}
