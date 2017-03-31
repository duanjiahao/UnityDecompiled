using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class AssetPreview
	{
		private const int kSharedClientID = 0;

		public static Texture2D GetAssetPreview(UnityEngine.Object asset)
		{
			Texture2D result;
			if (asset != null)
			{
				result = AssetPreview.GetAssetPreview(asset.GetInstanceID());
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static Texture2D GetAssetPreview(int instanceID)
		{
			return AssetPreview.GetAssetPreview(instanceID, 0);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetAssetPreview(int instanceID, int clientID);

		public static bool IsLoadingAssetPreview(int instanceID)
		{
			return AssetPreview.IsLoadingAssetPreview(instanceID, 0);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsLoadingAssetPreview(int instanceID, int clientID);

		public static bool IsLoadingAssetPreviews()
		{
			return AssetPreview.IsLoadingAssetPreviews(0);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsLoadingAssetPreviews(int clientID);

		internal static bool HasAnyNewPreviewTexturesAvailable()
		{
			return AssetPreview.HasAnyNewPreviewTexturesAvailable(0);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasAnyNewPreviewTexturesAvailable(int clientID);

		public static void SetPreviewTextureCacheSize(int size)
		{
			AssetPreview.SetPreviewTextureCacheSize(size, 0);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPreviewTextureCacheSize(int size, int clientID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ClearTemporaryAssetPreviews();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DeletePreviewTextureManagerByID(int clientID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Texture2D GetMiniThumbnail(UnityEngine.Object obj);

		public static Texture2D GetMiniTypeThumbnail(Type type)
		{
			Texture2D result;
			if (typeof(MonoBehaviour).IsAssignableFrom(type))
			{
				result = EditorGUIUtility.LoadIcon(type.FullName.Replace('.', '/') + " Icon");
			}
			else
			{
				result = AssetPreview.INTERNAL_GetMiniTypeThumbnailFromType(type);
			}
			return result;
		}

		internal static Texture2D GetMiniTypeThumbnail(UnityEngine.Object obj)
		{
			return AssetPreview.INTERNAL_GetMiniTypeThumbnailFromObject(obj);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D GetMiniTypeThumbnailFromClassID(int classID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D INTERNAL_GetMiniTypeThumbnailFromObject(UnityEngine.Object monoObj);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Texture2D INTERNAL_GetMiniTypeThumbnailFromType(Type managedType);
	}
}
