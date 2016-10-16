using System;

namespace UnityEditor.Modules
{
	internal interface IScriptingImplementations
	{
		ScriptingImplementation[] Supported();

		ScriptingImplementation[] Enabled();
	}
}
