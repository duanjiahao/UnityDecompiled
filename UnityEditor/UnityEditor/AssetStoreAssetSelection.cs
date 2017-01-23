using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal static class AssetStoreAssetSelection
	{
		public delegate void AssetsRefreshed();

		internal static Dictionary<int, AssetStoreAsset> s_SelectedAssets;

		public static int Count
		{
			get
			{
				return (AssetStoreAssetSelection.s_SelectedAssets != null) ? AssetStoreAssetSelection.s_SelectedAssets.Count : 0;
			}
		}

		public static bool Empty
		{
			get
			{
				return AssetStoreAssetSelection.s_SelectedAssets == null || AssetStoreAssetSelection.s_SelectedAssets.Count == 0;
			}
		}

		public static void AddAsset(AssetStoreAsset searchResult, Texture2D placeholderPreviewImage)
		{
			if (placeholderPreviewImage != null)
			{
				searchResult.previewImage = AssetStoreAssetSelection.ScaleImage(placeholderPreviewImage, 256, 256);
			}
			searchResult.previewInfo = null;
			searchResult.previewBundleRequest = null;
			if (!string.IsNullOrEmpty(searchResult.dynamicPreviewURL) && searchResult.previewBundle == null)
			{
				searchResult.disposed = false;
				AsyncHTTPClient client = new AsyncHTTPClient(searchResult.dynamicPreviewURL);
				client.doneCallback = delegate(AsyncHTTPClient c)
				{
					if (!client.IsSuccess())
					{
						Console.WriteLine("Error downloading dynamic preview: " + client.text);
						searchResult.dynamicPreviewURL = null;
						AssetStoreAssetSelection.DownloadStaticPreview(searchResult);
					}
					else
					{
						AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
						if (!searchResult.disposed && firstAsset != null && searchResult.id == firstAsset.id)
						{
							try
							{
								AssetBundleCreateRequest cr = AssetBundle.LoadFromMemoryAsync(c.bytes);
								cr.DisableCompatibilityChecks();
								searchResult.previewBundleRequest = cr;
								EditorApplication.CallbackFunction callback = null;
								double startTime = EditorApplication.timeSinceStartup;
								callback = delegate
								{
									AssetStoreUtils.UpdatePreloading();
									if (!cr.isDone)
									{
										double timeSinceStartup = EditorApplication.timeSinceStartup;
										if (timeSinceStartup - startTime > 10.0)
										{
											EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, callback);
											Console.WriteLine("Timed out fetch live preview bundle " + (searchResult.dynamicPreviewURL ?? "<n/a>"));
										}
									}
									else
									{
										EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, callback);
										AssetStoreAsset firstAsset2 = AssetStoreAssetSelection.GetFirstAsset();
										if (!searchResult.disposed && firstAsset2 != null && searchResult.id == firstAsset2.id)
										{
											searchResult.previewBundle = cr.assetBundle;
											if (cr.assetBundle == null || cr.assetBundle.mainAsset == null)
											{
												searchResult.dynamicPreviewURL = null;
												AssetStoreAssetSelection.DownloadStaticPreview(searchResult);
											}
											else
											{
												searchResult.previewAsset = searchResult.previewBundle.mainAsset;
											}
										}
									}
								};
								EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, callback);
							}
							catch (Exception ex)
							{
								Console.Write(ex.Message);
								Debug.Log(ex.Message);
							}
						}
					}
				};
				client.Begin();
			}
			else if (!string.IsNullOrEmpty(searchResult.staticPreviewURL))
			{
				AssetStoreAssetSelection.DownloadStaticPreview(searchResult);
			}
			AssetStoreAssetSelection.AddAssetInternal(searchResult);
			AssetStoreAssetSelection.RefreshFromServer(null);
		}

		internal static void AddAssetInternal(AssetStoreAsset searchResult)
		{
			if (AssetStoreAssetSelection.s_SelectedAssets == null)
			{
				AssetStoreAssetSelection.s_SelectedAssets = new Dictionary<int, AssetStoreAsset>();
			}
			AssetStoreAssetSelection.s_SelectedAssets[searchResult.id] = searchResult;
		}

		private static void DownloadStaticPreview(AssetStoreAsset searchResult)
		{
			AsyncHTTPClient client = new AsyncHTTPClient(searchResult.staticPreviewURL);
			client.doneCallback = delegate(AsyncHTTPClient c)
			{
				if (!client.IsSuccess())
				{
					Console.WriteLine("Error downloading static preview: " + client.text);
				}
				else
				{
					Texture2D texture = c.texture;
					Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false, true);
					AssetStorePreviewManager.ScaleImage(texture2D.width, texture2D.height, texture, texture2D, null);
					searchResult.previewImage = texture2D;
					UnityEngine.Object.DestroyImmediate(texture);
					AssetStoreAssetInspector.Instance.Repaint();
				}
			};
			client.Begin();
		}

		public static void RefreshFromServer(AssetStoreAssetSelection.AssetsRefreshed callback)
		{
			if (AssetStoreAssetSelection.s_SelectedAssets.Count != 0)
			{
				List<AssetStoreAsset> list = new List<AssetStoreAsset>();
				foreach (KeyValuePair<int, AssetStoreAsset> current in AssetStoreAssetSelection.s_SelectedAssets)
				{
					list.Add(current.Value);
				}
				AssetStoreClient.AssetsInfo(list, delegate(AssetStoreAssetsInfo results)
				{
					AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.ServiceDisabled;
					if (results.error != null && results.error != "")
					{
						Console.WriteLine("Error performing Asset Store Info search: " + results.error);
						AssetStoreAssetInspector.OfflineNoticeEnabled = true;
						if (callback != null)
						{
							callback();
						}
					}
					else
					{
						AssetStoreAssetInspector.OfflineNoticeEnabled = false;
						if (results.status == AssetStoreAssetsInfo.Status.Ok)
						{
							AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.Ok;
						}
						else if (results.status == AssetStoreAssetsInfo.Status.BasketNotEmpty)
						{
							AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.BasketNotEmpty;
						}
						else if (results.status == AssetStoreAssetsInfo.Status.AnonymousUser)
						{
							AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.AnonymousUser;
						}
						AssetStoreAssetInspector.s_PurchaseMessage = results.message;
						AssetStoreAssetInspector.s_PaymentMethodCard = results.paymentMethodCard;
						AssetStoreAssetInspector.s_PaymentMethodExpire = results.paymentMethodExpire;
						AssetStoreAssetInspector.s_PriceText = results.priceText;
						AssetStoreAssetInspector.Instance.Repaint();
						if (callback != null)
						{
							callback();
						}
					}
				});
			}
		}

		private static Texture2D ScaleImage(Texture2D source, int w, int h)
		{
			Texture2D result;
			if (source.width % 4 != 0)
			{
				result = null;
			}
			else
			{
				Texture2D texture2D = new Texture2D(w, h, TextureFormat.RGB24, false, true);
				Color[] pixels = texture2D.GetPixels(0);
				double num = 1.0 / (double)w;
				double num2 = 1.0 / (double)h;
				double num3 = 0.0;
				double num4 = 0.0;
				int num5 = 0;
				for (int i = 0; i < h; i++)
				{
					int j = 0;
					while (j < w)
					{
						pixels[num5] = source.GetPixelBilinear((float)num3, (float)num4);
						num3 += num;
						j++;
						num5++;
					}
					num3 = 0.0;
					num4 += num2;
				}
				texture2D.SetPixels(pixels, 0);
				texture2D.Apply();
				result = texture2D;
			}
			return result;
		}

		public static bool ContainsAsset(int id)
		{
			return AssetStoreAssetSelection.s_SelectedAssets != null && AssetStoreAssetSelection.s_SelectedAssets.ContainsKey(id);
		}

		public static void Clear()
		{
			if (AssetStoreAssetSelection.s_SelectedAssets != null)
			{
				foreach (KeyValuePair<int, AssetStoreAsset> current in AssetStoreAssetSelection.s_SelectedAssets)
				{
					current.Value.Dispose();
				}
				AssetStoreAssetSelection.s_SelectedAssets.Clear();
			}
		}

		public static AssetStoreAsset GetFirstAsset()
		{
			AssetStoreAsset result;
			if (AssetStoreAssetSelection.s_SelectedAssets == null)
			{
				result = null;
			}
			else
			{
				Dictionary<int, AssetStoreAsset>.Enumerator enumerator = AssetStoreAssetSelection.s_SelectedAssets.GetEnumerator();
				if (!enumerator.MoveNext())
				{
					result = null;
				}
				else
				{
					KeyValuePair<int, AssetStoreAsset> current = enumerator.Current;
					result = current.Value;
				}
			}
			return result;
		}
	}
}
