using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal class ScriptAssembly
	{
		public BuildTarget BuildTarget
		{
			get;
			set;
		}

		public ApiCompatibilityLevel ApiCompatibilityLevel
		{
			get;
			set;
		}

		public string Filename
		{
			get;
			set;
		}

		public string OutputDirectory
		{
			get;
			set;
		}

		public ScriptAssembly[] ScriptAssemblyReferences
		{
			get;
			set;
		}

		public string[] References
		{
			get;
			set;
		}

		public string[] Defines
		{
			get;
			set;
		}

		public string[] Files
		{
			get;
			set;
		}

		public bool RunUpdater
		{
			get;
			set;
		}

		public string FullPath
		{
			get
			{
				return Path.Combine(this.OutputDirectory, this.Filename);
			}
		}

		public string GetExtensionOfSourceFiles()
		{
			return (this.Files.Length <= 0) ? "NA" : Path.GetExtension(this.Files[0]).ToLower().Substring(1);
		}

		public MonoIsland ToMonoIsland(BuildFlags buildFlags, string buildOutputDirectory)
		{
			bool editor = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
			bool development_player = (buildFlags & BuildFlags.BuildingDevelopmentBuild) == BuildFlags.BuildingDevelopmentBuild;
			IEnumerable<string> first = from a in this.ScriptAssemblyReferences
			select Path.Combine(a.OutputDirectory, a.Filename);
			string[] references = first.Concat(this.References).ToArray<string>();
			string output = Path.Combine(buildOutputDirectory, this.Filename);
			return new MonoIsland(this.BuildTarget, editor, development_player, this.ApiCompatibilityLevel, this.Files, references, this.Defines, output);
		}
	}
}
