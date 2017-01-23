using System;
using UnityEditor;

namespace UnityEditorInternal
{
	public sealed class AssetStore
	{
		public static void Open(string assetStoreURL)
		{
			if (assetStoreURL != "")
			{
				AssetStoreWindow.OpenURL(assetStoreURL);
			}
			else
			{
				AssetStoreWindow.Init();
			}
		}
	}
}
