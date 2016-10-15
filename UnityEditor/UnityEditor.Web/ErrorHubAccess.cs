using System;
using UnityEditor.Connect;

namespace UnityEditor.Web
{
	[InitializeOnLoad]
	internal class ErrorHubAccess : CloudServiceAccess
	{
		public const string kServiceName = "ErrorHub";

		private static string kServiceUrl;

		public static ErrorHubAccess instance
		{
			get;
			private set;
		}

		public string errorMessage
		{
			get;
			set;
		}

		static ErrorHubAccess()
		{
			ErrorHubAccess.kServiceUrl = "file://" + EditorApplication.userJavascriptPackagesPath + "unityeditor-cloud-hub/dist/index.html?failure=unity_connect";
			ErrorHubAccess.instance = new ErrorHubAccess();
			UnityConnectServiceData cloudService = new UnityConnectServiceData("ErrorHub", ErrorHubAccess.kServiceUrl, ErrorHubAccess.instance, "unity/project/cloud/errorhub");
			UnityConnectServiceCollection.instance.AddService(cloudService);
		}

		public override string GetServiceName()
		{
			return "ErrorHub";
		}
	}
}
