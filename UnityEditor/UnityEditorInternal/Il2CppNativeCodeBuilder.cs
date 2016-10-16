using System;
using System.Collections.Generic;
using System.Diagnostics;

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

		public virtual IEnumerable<string> ConvertIncludesToFullPaths(IEnumerable<string> relativeIncludePaths)
		{
			return relativeIncludePaths;
		}

		public virtual string ConvertOutputFileToFullPath(string outputFileRelativePath)
		{
			return outputFileRelativePath;
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
