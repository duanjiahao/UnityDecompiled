using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.Connect;
using UnityEngine;
using UnityEngine.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class PurchasingAccess : CloudServiceAccess
	{
		private const string kServiceName = "Purchasing";

		private const string kServiceDisplayName = "In App Purchasing";

		private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/purchasing";

		private const string kETagPath = "Assets/Plugins/UnityPurchasing/ETag";

		private const string kUnknownPackageETag = "unknown";

		private static readonly Uri kPackageUri;

		private bool m_InstallInProgress;

		static PurchasingAccess()
		{
			PurchasingAccess.kPackageUri = new Uri("https://public-cdn.cloud.unity3d.com/UnityEngine.Cloud.Purchasing.unitypackage");
			UnityConnectServiceData cloudService = new UnityConnectServiceData("Purchasing", "https://public-cdn.cloud.unity3d.com/editor/5.4/production/cloud/purchasing", new PurchasingAccess(), "unity/project/cloud/purchasing");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "Purchasing";
		}

		public override string GetServiceDisplayName()
		{
			return "In App Purchasing";
		}

		public override bool IsServiceEnabled()
		{
			return UnityPurchasingSettings.enabled;
		}

		public override void EnableService(bool enabled)
		{
			UnityPurchasingSettings.enabled = enabled;
		}

		public void InstallUnityPackage()
		{
			if (this.m_InstallInProgress)
			{
				return;
			}
			RemoteCertificateValidationCallback originalCallback = ServicePointManager.ServerCertificateValidationCallback;
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				ServicePointManager.ServerCertificateValidationCallback = ((object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => true);
			}
			this.m_InstallInProgress = true;
			string location = FileUtil.GetUniqueTempPathInProject();
			location = Path.ChangeExtension(location, ".unitypackage");
			WebClient client = new WebClient();
			client.DownloadFileCompleted += delegate(object sender, AsyncCompletedEventArgs args)
			{
				EditorApplication.CallbackFunction handler = null;
				handler = delegate
				{
					ServicePointManager.ServerCertificateValidationCallback = originalCallback;
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, handler);
					this.m_InstallInProgress = false;
					if (args.Error == null)
					{
						this.SaveETag(client);
						AssetDatabase.ImportPackage(location, false);
					}
					else
					{
						Debug.LogWarning("Failed to download IAP package. Please check connectivity and retry.");
						Debug.LogException(args.Error);
					}
				};
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, handler);
			};
			client.DownloadFileAsync(PurchasingAccess.kPackageUri, location);
		}

		public string GetInstalledETag()
		{
			if (File.Exists("Assets/Plugins/UnityPurchasing/ETag"))
			{
				return File.ReadAllText("Assets/Plugins/UnityPurchasing/ETag");
			}
			if (Directory.Exists(Path.GetDirectoryName("Assets/Plugins/UnityPurchasing/ETag")))
			{
				return "unknown";
			}
			return null;
		}

		private void SaveETag(WebClient client)
		{
			string text = client.ResponseHeaders.Get("ETag");
			if (text != null)
			{
				Directory.CreateDirectory(Path.GetDirectoryName("Assets/Plugins/UnityPurchasing/ETag"));
				File.WriteAllText("Assets/Plugins/UnityPurchasing/ETag", text);
			}
		}
	}
}
