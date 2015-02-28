using System;
namespace UnityEditor.Modules
{
	internal interface IScriptingImplementations
	{
		ScriptingImplementation[] Supported
		{
			get;
		}
		ScriptingImplementation[] Enabled
		{
			get;
		}
	}
}
