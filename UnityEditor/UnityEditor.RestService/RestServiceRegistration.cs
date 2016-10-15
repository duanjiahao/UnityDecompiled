using System;

namespace UnityEditor.RestService
{
	[InitializeOnLoad]
	internal class RestServiceRegistration
	{
		static RestServiceRegistration()
		{
			OpenDocumentsRestHandler.Register();
			ProjectStateRestHandler.Register();
			AssetRestHandler.Register();
			PairingRestHandler.Register();
			PlayModeRestHandler.Register();
		}
	}
}
