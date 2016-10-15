using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(AssetStoreAssetInspector))]
	internal class AssetStoreAssetInspector : Editor
	{
		private class Styles
		{
			public GUIStyle link = new GUIStyle(EditorStyles.label);

			public GUIContent assetStoreLogo = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo");

			public Styles()
			{
				this.link.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
			}
		}

		internal enum PaymentAvailability
		{
			BasketNotEmpty,
			ServiceDisabled,
			AnonymousUser,
			Ok
		}

		private static AssetStoreAssetInspector s_SharedAssetStoreAssetInspector;

		private static AssetStoreAssetInspector.Styles styles;

		private Vector2 pos;

		private bool packageInfoShown = true;

		internal static string s_PurchaseMessage = string.Empty;

		internal static string s_PaymentMethodCard = string.Empty;

		internal static string s_PaymentMethodExpire = string.Empty;

		internal static string s_PriceText = string.Empty;

		private static GUIContent[] sStatusWheel;

		internal static AssetStoreAssetInspector.PaymentAvailability m_PaymentAvailability;

		private int lastAssetID;

		private EditorWrapper m_PreviewEditor;

		private UnityEngine.Object m_PreviewObject;

		public static AssetStoreAssetInspector Instance
		{
			get
			{
				if (AssetStoreAssetInspector.s_SharedAssetStoreAssetInspector == null)
				{
					AssetStoreAssetInspector.s_SharedAssetStoreAssetInspector = ScriptableObject.CreateInstance<AssetStoreAssetInspector>();
					AssetStoreAssetInspector.s_SharedAssetStoreAssetInspector.hideFlags = HideFlags.HideAndDontSave;
				}
				return AssetStoreAssetInspector.s_SharedAssetStoreAssetInspector;
			}
		}

		public static bool OfflineNoticeEnabled
		{
			get;
			set;
		}

		internal static AssetStoreAssetInspector.PaymentAvailability paymentAvailability
		{
			get
			{
				if (AssetStoreClient.LoggedOut())
				{
					AssetStoreAssetInspector.m_PaymentAvailability = AssetStoreAssetInspector.PaymentAvailability.AnonymousUser;
				}
				return AssetStoreAssetInspector.m_PaymentAvailability;
			}
			set
			{
				if (AssetStoreClient.LoggedOut())
				{
					AssetStoreAssetInspector.m_PaymentAvailability = AssetStoreAssetInspector.PaymentAvailability.AnonymousUser;
				}
				else
				{
					AssetStoreAssetInspector.m_PaymentAvailability = value;
				}
			}
		}

		private EditorWrapper previewEditor
		{
			get
			{
				AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
				if (firstAsset == null)
				{
					return null;
				}
				UnityEngine.Object preview = firstAsset.Preview;
				if (preview == null)
				{
					return null;
				}
				if (preview != this.m_PreviewObject)
				{
					this.m_PreviewObject = preview;
					if (this.m_PreviewEditor != null)
					{
						this.m_PreviewEditor.Dispose();
					}
					this.m_PreviewEditor = EditorWrapper.Make(this.m_PreviewObject, EditorFeatures.PreviewGUI);
				}
				return this.m_PreviewEditor;
			}
		}

		private static GUIContent StatusWheel
		{
			get
			{
				if (AssetStoreAssetInspector.sStatusWheel == null)
				{
					AssetStoreAssetInspector.sStatusWheel = new GUIContent[12];
					for (int i = 0; i < 12; i++)
					{
						GUIContent gUIContent = new GUIContent();
						gUIContent.image = EditorGUIUtility.LoadIcon("WaitSpin" + i.ToString("00"));
						AssetStoreAssetInspector.sStatusWheel[i] = gUIContent;
					}
				}
				int num = (int)Mathf.Repeat(Time.realtimeSinceStartup * 10f, 11.99f);
				return AssetStoreAssetInspector.sStatusWheel[num];
			}
		}

		public void OnDownloadProgress(string id, string message, int bytes, int total)
		{
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			if (firstAsset == null)
			{
				return;
			}
			AssetStoreAsset.PreviewInfo previewInfo = firstAsset.previewInfo;
			if (previewInfo == null)
			{
				return;
			}
			if (firstAsset.packageID.ToString() != id)
			{
				return;
			}
			if ((message == "downloading" || message == "connecting") && !AssetStoreAssetInspector.OfflineNoticeEnabled)
			{
				previewInfo.downloadProgress = (float)bytes / (float)total;
			}
			else
			{
				previewInfo.downloadProgress = -1f;
			}
			base.Repaint();
		}

		public void Update()
		{
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			bool flag = firstAsset != null && firstAsset.previewInfo != null && (firstAsset.previewInfo.buildProgress >= 0f || firstAsset.previewInfo.downloadProgress >= 0f);
			if ((firstAsset == null && this.lastAssetID != 0) || (firstAsset != null && this.lastAssetID != firstAsset.id) || flag)
			{
				this.lastAssetID = ((firstAsset != null) ? firstAsset.id : 0);
				base.Repaint();
			}
			if (firstAsset != null && firstAsset.previewBundle != null)
			{
				firstAsset.previewBundle.Unload(false);
				firstAsset.previewBundle = null;
				base.Repaint();
			}
		}

		public override void OnInspectorGUI()
		{
			if (AssetStoreAssetInspector.styles == null)
			{
				AssetStoreAssetInspector.s_SharedAssetStoreAssetInspector = this;
				AssetStoreAssetInspector.styles = new AssetStoreAssetInspector.Styles();
			}
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			AssetStoreAsset.PreviewInfo previewInfo = null;
			if (firstAsset != null)
			{
				previewInfo = firstAsset.previewInfo;
			}
			if (firstAsset != null)
			{
				this.target.name = string.Format("Asset Store: {0}", firstAsset.name);
			}
			else
			{
				this.target.name = "Asset Store";
			}
			EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			bool enabled = GUI.enabled;
			GUI.enabled = (firstAsset != null && firstAsset.packageID != 0);
			if (AssetStoreAssetInspector.OfflineNoticeEnabled)
			{
				Color color = GUI.color;
				GUI.color = Color.yellow;
				GUILayout.Label("Network is offline", new GUILayoutOption[0]);
				GUI.color = color;
			}
			if (firstAsset != null)
			{
				string label = (firstAsset.className != null) ? firstAsset.className.Split(new char[]
				{
					' '
				}, 2)[0] : string.Empty;
				bool flag = firstAsset.id == -firstAsset.packageID;
				if (flag)
				{
					label = "Package";
				}
				if (firstAsset.HasLivePreview)
				{
					label = firstAsset.Preview.GetType().Name;
				}
				EditorGUILayout.LabelField("Type", label, new GUILayoutOption[0]);
				if (flag)
				{
					this.packageInfoShown = true;
				}
				else
				{
					EditorGUILayout.Separator();
					this.packageInfoShown = EditorGUILayout.Foldout(this.packageInfoShown, "Part of package");
				}
				if (this.packageInfoShown)
				{
					EditorGUILayout.LabelField("Name", (previewInfo != null) ? previewInfo.packageName : "-", new GUILayoutOption[0]);
					EditorGUILayout.LabelField("Version", (previewInfo != null) ? previewInfo.packageVersion : "-", new GUILayoutOption[0]);
					string label2 = (previewInfo != null) ? ((firstAsset.price == null || !(firstAsset.price != string.Empty)) ? "free" : firstAsset.price) : "-";
					EditorGUILayout.LabelField("Price", label2, new GUILayoutOption[0]);
					string label3 = (previewInfo == null || previewInfo.packageRating < 0) ? "-" : (previewInfo.packageRating.ToString() + " of 5");
					EditorGUILayout.LabelField("Rating", label3, new GUILayoutOption[0]);
					EditorGUILayout.LabelField("Size", (previewInfo != null) ? AssetStoreAssetInspector.intToSizeString(previewInfo.packageSize) : "-", new GUILayoutOption[0]);
					string label4 = (previewInfo == null || previewInfo.packageAssetCount < 0) ? "-" : previewInfo.packageAssetCount.ToString();
					EditorGUILayout.LabelField("Asset count", label4, new GUILayoutOption[0]);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					EditorGUILayout.PrefixLabel("Web page");
					bool flag2 = previewInfo != null && previewInfo.packageShortUrl != null && previewInfo.packageShortUrl != string.Empty;
					bool enabled2 = GUI.enabled;
					GUI.enabled = flag2;
					if (GUILayout.Button((!flag2) ? EditorGUIUtility.TempContent("-") : new GUIContent(previewInfo.packageShortUrl, "View in browser"), AssetStoreAssetInspector.styles.link, new GUILayoutOption[0]))
					{
						Application.OpenURL(previewInfo.packageShortUrl);
					}
					if (GUI.enabled)
					{
						EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
					}
					GUI.enabled = enabled2;
					GUILayout.EndHorizontal();
					EditorGUILayout.LabelField("Publisher", (previewInfo != null) ? previewInfo.publisherName : "-", new GUILayoutOption[0]);
				}
				if (firstAsset.id != 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					string text;
					if (previewInfo != null && previewInfo.isDownloadable)
					{
						text = "Import package";
					}
					else
					{
						text = "Buy for " + firstAsset.price;
					}
					bool enabled3 = GUI.enabled;
					bool flag3 = previewInfo != null && previewInfo.buildProgress >= 0f;
					bool flag4 = previewInfo != null && previewInfo.downloadProgress >= 0f;
					if (flag3 || flag4 || previewInfo == null)
					{
						text = string.Empty;
						GUI.enabled = false;
					}
					if (GUILayout.Button(text, new GUILayoutOption[]
					{
						GUILayout.Height(40f),
						GUILayout.Width(120f)
					}))
					{
						if (previewInfo.isDownloadable)
						{
							this.ImportPackage(firstAsset);
						}
						else
						{
							this.InitiateBuySelected();
						}
						GUIUtility.ExitGUI();
					}
					if (Event.current.type == EventType.Repaint)
					{
						Rect lastRect = GUILayoutUtility.GetLastRect();
						lastRect.height -= 4f;
						float width = lastRect.width;
						lastRect.width = lastRect.height;
						lastRect.y += 2f;
						lastRect.x += 2f;
						if (flag3 || flag4)
						{
							lastRect.width = width - lastRect.height - 4f;
							lastRect.x += lastRect.height;
							EditorGUI.ProgressBar(lastRect, (!flag4) ? previewInfo.buildProgress : previewInfo.downloadProgress, (!flag4) ? "Building" : "Downloading");
						}
					}
					GUI.enabled = enabled3;
					GUILayout.Space(4f);
					if (GUILayout.Button("Open Asset Store", new GUILayoutOption[]
					{
						GUILayout.Height(40f),
						GUILayout.Width(120f)
					}))
					{
						AssetStoreAssetInspector.OpenItemInAssetStore(firstAsset);
						GUIUtility.ExitGUI();
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}
				GUILayout.FlexibleSpace();
			}
			EditorWrapper previewEditor = this.previewEditor;
			if (previewEditor != null && firstAsset != null && firstAsset.HasLivePreview)
			{
				previewEditor.OnAssetStoreInspectorGUI();
			}
			GUI.enabled = enabled;
			EditorGUILayout.EndVertical();
		}

		public static void OpenItemInAssetStore(AssetStoreAsset activeAsset)
		{
			if (activeAsset.id != 0)
			{
				AssetStore.Open(string.Format("content/{0}?assetID={1}", activeAsset.packageID, activeAsset.id));
				Analytics.Track(string.Format("/AssetStore/ViewInStore/{0}/{1}", activeAsset.packageID, activeAsset.id));
			}
		}

		private static string intToSizeString(int inValue)
		{
			if (inValue < 0)
			{
				return "unknown";
			}
			float num = (float)inValue;
			string[] array = new string[]
			{
				"TB",
				"GB",
				"MB",
				"KB",
				"Bytes"
			};
			int num2 = array.Length - 1;
			while (num > 1000f && num2 >= 0)
			{
				num /= 1000f;
				num2--;
			}
			if (num2 < 0)
			{
				return "<error>";
			}
			return string.Format("{0:#.##} {1}", num, array[num2]);
		}

		public override bool HasPreviewGUI()
		{
			return this.target != null && AssetStoreAssetSelection.Count != 0;
		}

		public void OnEnable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			AssetStoreUtils.RegisterDownloadDelegate(this);
		}

		public void OnDisable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			if (this.m_PreviewEditor != null)
			{
				this.m_PreviewEditor.Dispose();
				this.m_PreviewEditor = null;
			}
			if (this.m_PreviewObject != null)
			{
				this.m_PreviewObject = null;
			}
			AssetStoreUtils.UnRegisterDownloadDelegate(this);
		}

		public override void OnPreviewSettings()
		{
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			if (firstAsset == null)
			{
				return;
			}
			EditorWrapper previewEditor = this.previewEditor;
			if (previewEditor != null && firstAsset.HasLivePreview)
			{
				previewEditor.OnPreviewSettings();
			}
		}

		public override string GetInfoString()
		{
			EditorWrapper previewEditor = this.previewEditor;
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			if (firstAsset == null)
			{
				return "No item selected";
			}
			if (previewEditor != null && firstAsset.HasLivePreview)
			{
				return previewEditor.GetInfoString();
			}
			return string.Empty;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_PreviewObject == null)
			{
				return;
			}
			EditorWrapper previewEditor = this.previewEditor;
			if (previewEditor != null && this.m_PreviewObject is AnimationClip)
			{
				previewEditor.OnPreviewGUI(r, background);
			}
			else
			{
				this.OnInteractivePreviewGUI(r, background);
			}
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			EditorWrapper previewEditor = this.previewEditor;
			if (previewEditor != null)
			{
				previewEditor.OnInteractivePreviewGUI(r, background);
			}
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			if (firstAsset != null && !firstAsset.HasLivePreview && !string.IsNullOrEmpty(firstAsset.dynamicPreviewURL))
			{
				GUIContent statusWheel = AssetStoreAssetInspector.StatusWheel;
				r.y += (r.height - (float)statusWheel.image.height) / 2f;
				r.x += (r.width - (float)statusWheel.image.width) / 2f;
				GUI.Label(r, AssetStoreAssetInspector.StatusWheel);
				base.Repaint();
			}
		}

		public override GUIContent GetPreviewTitle()
		{
			return GUIContent.Temp("Asset Store Preview");
		}

		private void InitiateBuySelected(bool firstAttempt)
		{
			AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
			if (firstAsset == null)
			{
				EditorUtility.DisplayDialog("No asset selected", "Please select asset before buying a package", "ok");
			}
			else if (AssetStoreAssetInspector.paymentAvailability == AssetStoreAssetInspector.PaymentAvailability.AnonymousUser)
			{
				if (AssetStoreClient.LoggedIn())
				{
					AssetStoreAssetSelection.RefreshFromServer(delegate
					{
						this.InitiateBuySelected(false);
					});
				}
				else if (firstAttempt)
				{
					this.LoginAndInitiateBuySelected();
				}
			}
			else if (AssetStoreAssetInspector.paymentAvailability == AssetStoreAssetInspector.PaymentAvailability.ServiceDisabled)
			{
				if (firstAsset.previewInfo == null)
				{
					return;
				}
				AssetStore.Open(string.Format("content/{0}/directpurchase", firstAsset.packageID));
			}
			else if (AssetStoreAssetInspector.paymentAvailability == AssetStoreAssetInspector.PaymentAvailability.BasketNotEmpty)
			{
				if (firstAsset.previewInfo == null)
				{
					return;
				}
				if (firstAttempt)
				{
					AssetStoreAssetSelection.RefreshFromServer(delegate
					{
						this.InitiateBuySelected(false);
					});
				}
				else
				{
					AssetStore.Open(string.Format("content/{0}/basketpurchase", firstAsset.packageID));
				}
			}
			else
			{
				AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindow(firstAsset, AssetStoreAssetInspector.s_PurchaseMessage, AssetStoreAssetInspector.s_PaymentMethodCard, AssetStoreAssetInspector.s_PaymentMethodExpire, AssetStoreAssetInspector.s_PriceText);
			}
		}

		private void InitiateBuySelected()
		{
			this.InitiateBuySelected(true);
		}

		private void LoginAndInitiateBuySelected()
		{
			AssetStoreLoginWindow.Login("Please login to the Asset Store in order to get payment information about the package.", delegate(string errorMessage)
			{
				if (errorMessage != null)
				{
					return;
				}
				AssetStoreAssetSelection.RefreshFromServer(delegate
				{
					this.InitiateBuySelected(false);
				});
			});
		}

		private void ImportPackage(AssetStoreAsset asset)
		{
			if (AssetStoreAssetInspector.paymentAvailability == AssetStoreAssetInspector.PaymentAvailability.AnonymousUser)
			{
				this.LoginAndImport(asset);
			}
			else
			{
				AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindowBuilding(asset);
			}
		}

		private void LoginAndImport(AssetStoreAsset asset)
		{
			AssetStoreLoginWindow.Login("Please login to the Asset Store in order to get download information for the package.", delegate(string errorMessage)
			{
				if (errorMessage != null)
				{
					return;
				}
				AssetStoreAssetSelection.RefreshFromServer(delegate
				{
					AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindowBuilding(asset);
				});
			});
		}
	}
}
