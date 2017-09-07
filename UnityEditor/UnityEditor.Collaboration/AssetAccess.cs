using System;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	internal static class AssetAccess
	{
		public static bool TryGetAssetGUIDFromObject(UnityEngine.Object objectWithGUID, out string assetGUID)
		{
			if (objectWithGUID == null)
			{
				throw new ArgumentNullException("objectWithGuid");
			}
			bool result = false;
			if (objectWithGUID.GetType() == typeof(SceneAsset))
			{
				result = AssetAccess.TryGetAssetGUIDFromDatabase(objectWithGUID, out assetGUID);
			}
			else if (objectWithGUID.GetType() == typeof(GameObject))
			{
				result = AssetAccess.TryGetPrefabGUID(objectWithGUID, out assetGUID);
			}
			else
			{
				assetGUID = string.Empty;
			}
			return result;
		}

		public static bool TryGetAssetFromGUID(string assetGUID, out UnityEngine.Object asset)
		{
			if (assetGUID == null)
			{
				throw new ArgumentNullException("assetGUID");
			}
			bool result = false;
			string text = AssetDatabase.GUIDToAssetPath(assetGUID);
			if (text == null)
			{
				asset = null;
			}
			else
			{
				asset = AssetDatabase.LoadMainAssetAtPath(text);
				result = (asset != null);
			}
			return result;
		}

		private static bool TryGetPrefabGUID(UnityEngine.Object gameObject, out string assetGUID)
		{
			PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);
			UnityEngine.Object @object = null;
			if (prefabType == PrefabType.PrefabInstance)
			{
				@object = PrefabUtility.GetPrefabParent(gameObject);
			}
			else if (prefabType == PrefabType.Prefab)
			{
				@object = gameObject;
			}
			bool result = false;
			if (@object == null)
			{
				assetGUID = string.Empty;
			}
			else
			{
				result = AssetAccess.TryGetAssetGUIDFromDatabase(@object, out assetGUID);
			}
			return result;
		}

		private static bool TryGetAssetGUIDFromDatabase(UnityEngine.Object objectWithGUID, out string assetGUID)
		{
			if (objectWithGUID == null)
			{
				throw new ArgumentNullException("objectWithGuid");
			}
			string text = null;
			string assetPath = AssetDatabase.GetAssetPath(objectWithGUID);
			if (!string.IsNullOrEmpty(assetPath))
			{
				text = AssetDatabase.AssetPathToGUID(assetPath);
			}
			bool result = false;
			if (string.IsNullOrEmpty(text))
			{
				assetGUID = string.Empty;
			}
			else
			{
				assetGUID = text;
				result = true;
			}
			return result;
		}
	}
}
