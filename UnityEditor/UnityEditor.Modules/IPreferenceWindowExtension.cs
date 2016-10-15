using System;

namespace UnityEditor.Modules
{
	internal interface IPreferenceWindowExtension
	{
		void ReadPreferences();

		void WritePreferences();

		bool HasExternalApplications();

		void ShowExternalApplications();
	}
}
