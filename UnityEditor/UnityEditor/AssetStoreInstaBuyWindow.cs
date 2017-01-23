using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class AssetStoreInstaBuyWindow : EditorWindow
	{
		private enum PurchaseStatus
		{
			Init,
			InProgress,
			Declined,
			Complete,
			StartBuild,
			Building,
			Downloading
		}

		private const int kStandardHeight = 160;

		private static GUIContent s_AssetStoreLogo;

		private string m_Password = "";

		private AssetStoreAsset m_Asset = null;

		private string m_Message = "";

		private AssetStoreInstaBuyWindow.PurchaseStatus m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Init;

		private double m_NextAllowedBuildRequestTime = 0.0;

		private const double kBuildPollInterval = 2.0;

		private const int kMaxPolls = 150;

		private int m_BuildAttempts = 0;

		private string m_PurchaseMessage = null;

		private string m_PaymentMethodCard = null;

		private string m_PaymentMethodExpire = null;

		private string m_PriceText = null;

		public static AssetStoreInstaBuyWindow ShowAssetStoreInstaBuyWindow(AssetStoreAsset asset, string purchaseMessage, string paymentMethodCard, string paymentMethodExpire, string priceText)
		{
			AssetStoreInstaBuyWindow windowWithRect = EditorWindow.GetWindowWithRect<AssetStoreInstaBuyWindow>(new Rect(100f, 100f, 400f, 160f), true, "Buy package from Asset Store");
			AssetStoreInstaBuyWindow result;
			if (windowWithRect.m_Purchasing != AssetStoreInstaBuyWindow.PurchaseStatus.Init)
			{
				EditorUtility.DisplayDialog("Download in progress", "There is already a package download in progress. You can only have one download running at a time", "Close");
				result = windowWithRect;
			}
			else
			{
				windowWithRect.position = new Rect(100f, 100f, 400f, 160f);
				windowWithRect.m_Parent.window.m_DontSaveToLayout = true;
				windowWithRect.m_Asset = asset;
				windowWithRect.m_Password = "";
				windowWithRect.m_Message = "";
				windowWithRect.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Init;
				windowWithRect.m_NextAllowedBuildRequestTime = 0.0;
				windowWithRect.m_BuildAttempts = 0;
				windowWithRect.m_PurchaseMessage = purchaseMessage;
				windowWithRect.m_PaymentMethodCard = paymentMethodCard;
				windowWithRect.m_PaymentMethodExpire = paymentMethodExpire;
				windowWithRect.m_PriceText = priceText;
				UsabilityAnalytics.Track(string.Format("/AssetStore/ShowInstaBuy/{0}/{1}", windowWithRect.m_Asset.packageID, windowWithRect.m_Asset.id));
				result = windowWithRect;
			}
			return result;
		}

		public static void ShowAssetStoreInstaBuyWindowBuilding(AssetStoreAsset asset)
		{
			AssetStoreInstaBuyWindow assetStoreInstaBuyWindow = AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindow(asset, "", "", "", "");
			if (assetStoreInstaBuyWindow.m_Purchasing != AssetStoreInstaBuyWindow.PurchaseStatus.Init)
			{
				EditorUtility.DisplayDialog("Download in progress", "There is already a package download in progress. You can only have one download running at a time", "Close");
			}
			else
			{
				assetStoreInstaBuyWindow.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.StartBuild;
				assetStoreInstaBuyWindow.m_BuildAttempts = 1;
				asset.previewInfo.buildProgress = 0f;
				UsabilityAnalytics.Track(string.Format("/AssetStore/ShowInstaFree/{0}/{1}", assetStoreInstaBuyWindow.m_Asset.packageID, assetStoreInstaBuyWindow.m_Asset.id));
			}
		}

		private static void LoadLogos()
		{
			if (AssetStoreInstaBuyWindow.s_AssetStoreLogo == null)
			{
				AssetStoreInstaBuyWindow.s_AssetStoreLogo = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo");
			}
		}

		public void OnInspectorUpdate()
		{
			if (this.m_Purchasing == AssetStoreInstaBuyWindow.PurchaseStatus.StartBuild && this.m_NextAllowedBuildRequestTime <= EditorApplication.timeSinceStartup)
			{
				this.m_NextAllowedBuildRequestTime = EditorApplication.timeSinceStartup + 2.0;
				this.BuildPackage();
			}
		}

		private void OnEnable()
		{
			AssetStoreUtils.RegisterDownloadDelegate(this);
		}

		public void OnDisable()
		{
			AssetStoreAsset.PreviewInfo previewInfo = (this.m_Asset != null) ? this.m_Asset.previewInfo : null;
			if (previewInfo != null)
			{
				previewInfo.downloadProgress = -1f;
				previewInfo.buildProgress = -1f;
			}
			AssetStoreUtils.UnRegisterDownloadDelegate(this);
			this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Init;
		}

		public void OnDownloadProgress(string id, string message, int bytes, int total)
		{
			AssetStoreAsset.PreviewInfo previewInfo = (this.m_Asset != null) ? this.m_Asset.previewInfo : null;
			if (previewInfo != null && !(this.m_Asset.packageID.ToString() != id))
			{
				if (message == "downloading" || message == "connecting")
				{
					previewInfo.downloadProgress = (float)bytes / (float)total;
				}
				else
				{
					previewInfo.downloadProgress = -1f;
				}
				base.Repaint();
			}
		}

		public void OnGUI()
		{
			AssetStoreInstaBuyWindow.LoadLogos();
			if (this.m_Asset != null)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(10f);
				switch (this.m_Purchasing)
				{
				case AssetStoreInstaBuyWindow.PurchaseStatus.Init:
					this.PasswordGUI();
					break;
				case AssetStoreInstaBuyWindow.PurchaseStatus.InProgress:
					if (this.m_Purchasing == AssetStoreInstaBuyWindow.PurchaseStatus.InProgress)
					{
						GUI.enabled = false;
					}
					this.PasswordGUI();
					break;
				case AssetStoreInstaBuyWindow.PurchaseStatus.Declined:
					this.PurchaseDeclinedGUI();
					break;
				case AssetStoreInstaBuyWindow.PurchaseStatus.Complete:
					this.PurchaseSuccessGUI();
					break;
				case AssetStoreInstaBuyWindow.PurchaseStatus.StartBuild:
				case AssetStoreInstaBuyWindow.PurchaseStatus.Building:
				case AssetStoreInstaBuyWindow.PurchaseStatus.Downloading:
					this.DownloadingGUI();
					break;
				}
				GUILayout.EndVertical();
			}
		}

		private void PasswordGUI()
		{
			AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.Label(AssetStoreInstaBuyWindow.s_AssetStoreLogo, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Complete purchase by entering your AssetStore password", EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag = this.m_PurchaseMessage != null && this.m_PurchaseMessage != "";
			bool flag2 = this.m_Message != null && this.m_Message != "";
			float num = (float)(160 + ((!flag) ? 0 : 20) + ((!flag2) ? 0 : 20));
			if (num != base.position.height)
			{
				base.position = new Rect(base.position.x, base.position.y, base.position.width, num);
			}
			if (flag)
			{
				GUILayout.Label(this.m_PurchaseMessage, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			}
			if (flag2)
			{
				Color color = GUI.color;
				GUI.color = Color.red;
				GUILayout.Label(this.m_Message, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
				GUI.color = color;
			}
			GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			string text = string.Format("Credit card: {0} (expires {1})", this.m_PaymentMethodCard, this.m_PaymentMethodExpire);
			GUILayout.Label(text, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.Space(8f);
			EditorGUILayout.LabelField("Amount", this.m_PriceText, new GUILayoutOption[0]);
			this.m_Password = EditorGUILayout.PasswordField("Password", this.m_Password, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(8f);
			if (GUILayout.Button("Just put to basket...", new GUILayoutOption[0]))
			{
				AssetStore.Open(string.Format("content/{0}/basketpurchase", this.m_Asset.packageID));
				UsabilityAnalytics.Track(string.Format("/AssetStore/PutToBasket/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
				this.m_Asset = null;
				base.Close();
				GUIUtility.ExitGUI();
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
			{
				UsabilityAnalytics.Track(string.Format("/AssetStore/CancelInstaBuy/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
				this.m_Asset = null;
				base.Close();
				GUIUtility.ExitGUI();
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Complete purchase", new GUILayoutOption[0]))
			{
				this.CompletePurchase();
			}
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
		}

		private void PurchaseSuccessGUI()
		{
			AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.Label(AssetStoreInstaBuyWindow.s_AssetStoreLogo, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Purchase completed succesfully", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label("You will receive a receipt in your email soon.", new GUILayoutOption[0]);
			bool flag = this.m_Message != null && this.m_Message != "";
			float num = (float)(160 + ((!flag) ? 0 : 20));
			if (num != base.position.height)
			{
				base.position = new Rect(base.position.x, base.position.y, base.position.width, num);
			}
			if (flag)
			{
				GUILayout.Label(this.m_Message, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			}
			GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(8f);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close", new GUILayoutOption[0]))
			{
				UsabilityAnalytics.Track(string.Format("/AssetStore/PurchaseOk/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
				this.m_Asset = null;
				base.Close();
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Import package", new GUILayoutOption[0]))
			{
				UsabilityAnalytics.Track(string.Format("/AssetStore/PurchaseOkImport/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
				this.m_BuildAttempts = 1;
				this.m_Asset.previewInfo.buildProgress = 0f;
				this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.StartBuild;
			}
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
		}

		private void DownloadingGUI()
		{
			AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.Label(AssetStoreInstaBuyWindow.s_AssetStoreLogo, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			string text = "Importing";
			GUILayout.Label(text, EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.Label("    ", new GUILayoutOption[0]);
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.height += 1f;
				bool flag = previewInfo.downloadProgress >= 0f;
				EditorGUI.ProgressBar(lastRect, (!flag) ? previewInfo.buildProgress : previewInfo.downloadProgress, (!flag) ? "Building" : "Downloading");
			}
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Abort", new GUILayoutOption[0]))
			{
				base.Close();
			}
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
		}

		private void PurchaseDeclinedGUI()
		{
			AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(5f);
			GUILayout.Label(AssetStoreInstaBuyWindow.s_AssetStoreLogo, GUIStyle.none, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Purchase declined", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label("No money has been drawn from you credit card", new GUILayoutOption[0]);
			bool flag = this.m_Message != null && this.m_Message != "";
			float num = (float)(160 + ((!flag) ? 0 : 20));
			if (num != base.position.height)
			{
				base.position = new Rect(base.position.x, base.position.y, base.position.width, num);
			}
			if (flag)
			{
				GUILayout.Label(this.m_Message, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			}
			GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(8f);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close", new GUILayoutOption[0]))
			{
				UsabilityAnalytics.Track(string.Format("/AssetStore/DeclinedAbort/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
				this.m_Asset = null;
				base.Close();
			}
			GUILayout.Space(5f);
			if (GUILayout.Button("Put to basket", new GUILayoutOption[0]))
			{
				AssetStore.Open(string.Format("content/{0}/basketpurchase", this.m_Asset.packageID));
				UsabilityAnalytics.Track(string.Format("/AssetStore/DeclinedPutToBasket/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
				this.m_Asset = null;
				base.Close();
			}
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
		}

		private void CompletePurchase()
		{
			this.m_Message = "";
			string password = this.m_Password;
			this.m_Password = "";
			this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.InProgress;
			AssetStoreClient.DirectPurchase(this.m_Asset.packageID, password, delegate(PurchaseResult result)
			{
				this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Init;
				if (result.error != null)
				{
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Declined;
					this.m_Message = "An error occured while completing you purhase.";
					base.Close();
				}
				string text = null;
				switch (result.status)
				{
				case PurchaseResult.Status.BasketNotEmpty:
					this.m_Message = "Something else has been put in our Asset Store basket while doing this purchase.";
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Declined;
					break;
				case PurchaseResult.Status.ServiceDisabled:
					this.m_Message = "Single click purchase has been disabled while doing this purchase.";
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Declined;
					break;
				case PurchaseResult.Status.AnonymousUser:
					this.m_Message = "You have been logged out from somewhere else while doing this purchase.";
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Declined;
					break;
				case PurchaseResult.Status.PasswordMissing:
					this.m_Message = result.message;
					base.Repaint();
					break;
				case PurchaseResult.Status.PasswordWrong:
					this.m_Message = result.message;
					base.Repaint();
					break;
				case PurchaseResult.Status.PurchaseDeclined:
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Declined;
					if (result.message != null)
					{
						this.m_Message = result.message;
					}
					base.Repaint();
					break;
				case PurchaseResult.Status.Ok:
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Complete;
					if (result.message != null)
					{
						this.m_Message = result.message;
					}
					base.Repaint();
					break;
				}
				if (text != null)
				{
					EditorUtility.DisplayDialog("Purchase failed", text + " This purchase has been cancelled.", "Add this item to basket", "Cancel");
				}
			});
			UsabilityAnalytics.Track(string.Format("/AssetStore/InstaBuy/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
		}

		private void BuildPackage()
		{
			AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
			if (previewInfo != null)
			{
				if (this.m_BuildAttempts++ > 150)
				{
					EditorUtility.DisplayDialog("Building timed out", "Timed out during building of package", "Close");
					base.Close();
				}
				else
				{
					previewInfo.downloadProgress = -1f;
					this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Building;
					AssetStoreClient.BuildPackage(this.m_Asset, delegate(BuildPackageResult result)
					{
						if (!(this == null))
						{
							if (result.error != null)
							{
								Debug.Log(result.error);
								if (EditorUtility.DisplayDialog("Error building package", "The server was unable to build the package. Please re-import.", "Ok"))
								{
									base.Close();
								}
							}
							else
							{
								if (this.m_Asset == null || this.m_Purchasing != AssetStoreInstaBuyWindow.PurchaseStatus.Building || result.packageID != this.m_Asset.packageID)
								{
									base.Close();
								}
								string packageUrl = result.asset.previewInfo.packageUrl;
								if (packageUrl != null && packageUrl != "")
								{
									this.DownloadPackage();
								}
								else
								{
									this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.StartBuild;
								}
								base.Repaint();
							}
						}
					});
				}
			}
		}

		private void DownloadPackage()
		{
			AssetStoreAsset.PreviewInfo item = this.m_Asset.previewInfo;
			this.m_Purchasing = AssetStoreInstaBuyWindow.PurchaseStatus.Downloading;
			item.downloadProgress = 0f;
			item.buildProgress = -1f;
			AssetStoreContext.Download(this.m_Asset.packageID.ToString(), item.packageUrl, item.encryptionKey, item.packageName, item.publisherName, item.categoryName, delegate(string id, string status, int bytes, int total)
			{
				if (!(this == null))
				{
					item.downloadProgress = -1f;
					if (status != "ok")
					{
						Debug.LogErrorFormat("Error downloading package {0} ({1})", new object[]
						{
							item.packageName,
							id
						});
						Debug.LogError(status);
						this.Close();
					}
					else
					{
						if (this.m_Asset == null || this.m_Purchasing != AssetStoreInstaBuyWindow.PurchaseStatus.Downloading || id != this.m_Asset.packageID.ToString())
						{
							this.Close();
						}
						if (!AssetStoreContext.OpenPackageInternal(id))
						{
							Debug.LogErrorFormat("Error importing package {0} ({1})", new object[]
							{
								item.packageName,
								id
							});
						}
						this.Close();
					}
				}
			});
		}
	}
}
