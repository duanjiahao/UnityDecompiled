using System;

namespace UnityEditor.Scripting
{
	internal struct MonoIsland
	{
		public readonly BuildTarget _target;

		public readonly string _classlib_profile;

		public readonly string[] _files;

		public readonly string[] _references;

		public readonly string[] _defines;

		public readonly string _output;

		public MonoIsland(BuildTarget target, string classlib_profile, string[] files, string[] references, string[] defines, string output)
		{
			this._target = target;
			this._classlib_profile = classlib_profile;
			this._files = files;
			this._references = references;
			this._defines = defines;
			this._output = output;
		}

		public string GetExtensionOfSourceFiles()
		{
			return (this._files.Length <= 0) ? "NA" : ScriptCompilers.GetExtensionOfSourceFile(this._files[0]);
		}
	}
}
