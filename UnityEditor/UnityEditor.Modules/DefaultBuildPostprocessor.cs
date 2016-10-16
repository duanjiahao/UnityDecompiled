using System;

namespace UnityEditor.Modules
{
	internal abstract class DefaultBuildPostprocessor : IBuildPostprocessor
	{
		public virtual void LaunchPlayer(BuildLaunchPlayerArgs args)
		{
			throw new NotSupportedException();
		}

		public virtual void PostProcess(BuildPostProcessArgs args)
		{
		}

		public virtual bool SupportsInstallInBuildFolder()
		{
			return false;
		}

		public virtual void PostProcessScriptsOnly(BuildPostProcessArgs args)
		{
			if (!this.SupportsScriptsOnlyBuild())
			{
				throw new NotSupportedException();
			}
		}

		public virtual bool SupportsScriptsOnlyBuild()
		{
			return false;
		}

		public virtual string GetScriptLayoutFileFromBuild(BuildOptions options, string installPath, string fileName)
		{
			return string.Empty;
		}

		public virtual string PrepareForBuild(BuildOptions options, BuildTarget target)
		{
			return null;
		}

		public virtual string GetExtension(BuildTarget target, BuildOptions options)
		{
			return string.Empty;
		}
	}
}
