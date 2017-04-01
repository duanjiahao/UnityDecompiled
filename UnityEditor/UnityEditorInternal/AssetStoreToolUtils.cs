using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditorInternal
{
	public sealed class AssetStoreToolUtils
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool BuildAssetStoreAssetBundle(UnityEngine.Object targetObject, string targetPath);

		public static bool PreviewAssetStoreAssetBundleInInspector(AssetBundle bundle, AssetStoreAsset info)
		{
			info.id = 0;
			info.previewAsset = bundle.mainAsset;
			AssetStoreAssetSelection.Clear();
			AssetStoreAssetSelection.AddAssetInternal(info);
			Selection.activeObject = AssetStoreAssetInspector.Instance;
			AssetStoreAssetInspector.Instance.Repaint();
			return true;
		}

		public static void UpdatePreloadingInternal()
		{
			AssetStoreUtils.UpdatePreloading();
		}
	}
}
