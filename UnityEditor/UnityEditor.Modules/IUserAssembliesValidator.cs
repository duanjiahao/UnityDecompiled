using System;

namespace UnityEditor.Modules
{
	internal interface IUserAssembliesValidator
	{
		bool canRunInBackground
		{
			get;
		}

		void Validate(string[] userAssemblies);

		void Cleanup();
	}
}
