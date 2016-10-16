using System;
using UnityEngine;

namespace UnityEditor
{
	public sealed class AssetStoreAsset
	{
		internal class PreviewInfo
		{
			public string packageName;

			public string packageShortUrl;

			public int packageSize;

			public string packageVersion;

			public int packageRating;

			public int packageAssetCount;

			public bool isPurchased;

			public bool isDownloadable;

			public string publisherName;

			public string encryptionKey;

			public string packageUrl;

			public float buildProgress;

			public float downloadProgress;

			public string categoryName;
		}

		public int id;

		public string name;

		public string displayName;

		public string staticPreviewURL;

		public string dynamicPreviewURL;

		public string className;

		public string price;

		public int packageID;

		internal AssetStoreAsset.PreviewInfo previewInfo;

		public Texture2D previewImage;

		internal AssetBundleCreateRequest previewBundleRequest;

		internal AssetBundle previewBundle;

		internal UnityEngine.Object previewAsset;

		internal bool disposed;

		public UnityEngine.Object Preview
		{
			get
			{
				if (this.previewAsset != null)
				{
					return this.previewAsset;
				}
				return this.previewImage;
			}
		}

		public bool HasLivePreview
		{
			get
			{
				return this.previewAsset != null;
			}
		}

		internal string DebugString
		{
			get
			{
				string text = string.Format("id: {0}\nname: {1}\nstaticPreviewURL: {2}\ndynamicPreviewURL: {3}\nclassName: {4}\nprice: {5}\npackageID: {6}", new object[]
				{
					this.id,
					this.name ?? "N/A",
					this.staticPreviewURL ?? "N/A",
					this.dynamicPreviewURL ?? "N/A",
					this.className ?? "N/A",
					this.price,
					this.packageID
				});
				if (this.previewInfo != null)
				{
					text += string.Format("previewInfo {{\n    packageName: {0}\n    packageShortUrl: {1}\n    packageSize: {2}\n    packageVersion: {3}\n    packageRating: {4}\n    packageAssetCount: {5}\n    isPurchased: {6}\n    isDownloadable: {7}\n    publisherName: {8}\n    encryptionKey: {9}\n    packageUrl: {10}\n    buildProgress: {11}\n    downloadProgress: {12}\n    categoryName: {13}\n}}", new object[]
					{
						this.previewInfo.packageName ?? "N/A",
						this.previewInfo.packageShortUrl ?? "N/A",
						this.previewInfo.packageSize,
						this.previewInfo.packageVersion ?? "N/A",
						this.previewInfo.packageRating,
						this.previewInfo.packageAssetCount,
						this.previewInfo.isPurchased,
						this.previewInfo.isDownloadable,
						this.previewInfo.publisherName ?? "N/A",
						this.previewInfo.encryptionKey ?? "N/A",
						this.previewInfo.packageUrl ?? "N/A",
						this.previewInfo.buildProgress,
						this.previewInfo.downloadProgress,
						this.previewInfo.categoryName ?? "N/A"
					});
				}
				return text;
			}
		}

		public AssetStoreAsset()
		{
			this.disposed = false;
		}

		public void Dispose()
		{
			if (this.previewImage != null)
			{
				UnityEngine.Object.DestroyImmediate(this.previewImage);
				this.previewImage = null;
			}
			if (this.previewBundle != null)
			{
				this.previewBundle.Unload(true);
				this.previewBundle = null;
				this.previewAsset = null;
			}
			this.disposed = true;
		}
	}
}
