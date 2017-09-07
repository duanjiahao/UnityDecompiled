using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UnityEditorInternal
{
	public abstract class Il2CppNativeCodeBuilder
	{
		public abstract string CompilerPlatform
		{
			get;
		}

		public abstract string CompilerArchitecture
		{
			get;
		}

		public virtual string CompilerFlags
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string LinkerFlags
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual bool SetsUpEnvironment
		{
			get
			{
				return false;
			}
		}

		public virtual string CacheDirectory
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string PluginPath
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual IEnumerable<string> AdditionalIl2CPPArguments
		{
			get
			{
				return new string[0];
			}
		}

		public virtual bool LinkLibIl2CppStatically
		{
			get
			{
				return true;
			}
		}

		public virtual IEnumerable<string> ConvertIncludesToFullPaths(IEnumerable<string> relativeIncludePaths)
		{
			string workingDirectory = Directory.GetCurrentDirectory();
			return from path in relativeIncludePaths
			select Path.Combine(workingDirectory, path);
		}

		public virtual string ConvertOutputFileToFullPath(string outputFileRelativePath)
		{
			return Path.Combine(Directory.GetCurrentDirectory(), outputFileRelativePath);
		}

		public void SetupStartInfo(ProcessStartInfo startInfo)
		{
			if (this.SetsUpEnvironment)
			{
				this.SetupEnvironment(startInfo);
			}
		}

		protected virtual void SetupEnvironment(ProcessStartInfo startInfo)
		{
		}
	}
}
