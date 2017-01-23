using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class AssetStoreAssetsInfo : AssetStoreResultBase<AssetStoreAssetsInfo>
	{
		internal enum Status
		{
			BasketNotEmpty,
			ServiceDisabled,
			AnonymousUser,
			Ok
		}

		internal AssetStoreAssetsInfo.Status status;

		internal Dictionary<int, AssetStoreAsset> assets = new Dictionary<int, AssetStoreAsset>();

		internal bool paymentTokenAvailable;

		internal string paymentMethodCard;

		internal string paymentMethodExpire;

		internal float price;

		internal float vat;

		internal string currency;

		internal string priceText;

		internal string vatText;

		internal string message;

		internal AssetStoreAssetsInfo(AssetStoreResultBase<AssetStoreAssetsInfo>.Callback c, List<AssetStoreAsset> assets) : base(c)
		{
			foreach (AssetStoreAsset current in assets)
			{
				this.assets[current.id] = current;
			}
		}

		protected override void Parse(Dictionary<string, JSONValue> dict)
		{
			Dictionary<string, JSONValue> dictionary = dict["purchase_info"].AsDict(true);
			string a = dictionary["status"].AsString(true);
			if (a == "basket-not-empty")
			{
				this.status = AssetStoreAssetsInfo.Status.BasketNotEmpty;
			}
			else if (a == "service-disabled")
			{
				this.status = AssetStoreAssetsInfo.Status.ServiceDisabled;
			}
			else if (a == "user-anonymous")
			{
				this.status = AssetStoreAssetsInfo.Status.AnonymousUser;
			}
			else if (a == "ok")
			{
				this.status = AssetStoreAssetsInfo.Status.Ok;
			}
			this.paymentTokenAvailable = dictionary["payment_token_available"].AsBool();
			if (dictionary.ContainsKey("payment_method_card"))
			{
				this.paymentMethodCard = dictionary["payment_method_card"].AsString(true);
			}
			if (dictionary.ContainsKey("payment_method_expire"))
			{
				this.paymentMethodExpire = dictionary["payment_method_expire"].AsString(true);
			}
			this.price = dictionary["price"].AsFloat(true);
			this.vat = dictionary["vat"].AsFloat(true);
			this.priceText = dictionary["price_text"].AsString(true);
			this.vatText = dictionary["vat_text"].AsString(true);
			this.currency = dictionary["currency"].AsString(true);
			this.message = ((!dictionary.ContainsKey("message")) ? null : dictionary["message"].AsString(true));
			List<JSONValue> list = dict["results"].AsList(true);
			foreach (JSONValue current in list)
			{
				int key;
				if (current["id"].IsString())
				{
					key = int.Parse(current["id"].AsString());
				}
				else
				{
					key = (int)current["id"].AsFloat();
				}
				AssetStoreAsset assetStoreAsset;
				if (this.assets.TryGetValue(key, out assetStoreAsset))
				{
					if (assetStoreAsset.previewInfo == null)
					{
						assetStoreAsset.previewInfo = new AssetStoreAsset.PreviewInfo();
					}
					AssetStoreAsset.PreviewInfo previewInfo = assetStoreAsset.previewInfo;
					assetStoreAsset.className = current["class_names"].AsString(true).Trim();
					previewInfo.packageName = current["package_name"].AsString(true).Trim();
					previewInfo.packageShortUrl = current["short_url"].AsString(true).Trim();
					assetStoreAsset.price = ((!current.ContainsKey("price_text")) ? null : current["price_text"].AsString(true).Trim());
					previewInfo.packageSize = int.Parse((!current.Get("package_size").IsNull()) ? current["package_size"].AsString(true) : "-1");
					assetStoreAsset.packageID = int.Parse(current["package_id"].AsString());
					previewInfo.packageVersion = current["package_version"].AsString();
					previewInfo.packageRating = int.Parse((!current.Get("rating").IsNull() && current["rating"].AsString(true).Length != 0) ? current["rating"].AsString(true) : "-1");
					previewInfo.packageAssetCount = int.Parse((!current["package_asset_count"].IsNull()) ? current["package_asset_count"].AsString(true) : "-1");
					previewInfo.isPurchased = (current.ContainsKey("purchased") && current["purchased"].AsBool(true));
					previewInfo.isDownloadable = (previewInfo.isPurchased || assetStoreAsset.price == null);
					previewInfo.publisherName = current["publisher_name"].AsString(true).Trim();
					previewInfo.packageUrl = ((!current.Get("package_url").IsNull()) ? current["package_url"].AsString(true) : "");
					previewInfo.encryptionKey = ((!current.Get("encryption_key").IsNull()) ? current["encryption_key"].AsString(true) : "");
					previewInfo.categoryName = ((!current.Get("category_name").IsNull()) ? current["category_name"].AsString(true) : "");
					previewInfo.buildProgress = -1f;
					previewInfo.downloadProgress = -1f;
				}
			}
		}
	}
}
