using System;
using System.IO;
using UnityEditorInternal;
namespace UnityEditor.OSXStandalone
{
	internal class OSXStandaloneIl2CppPlatformProvider : BaseIl2CppPlatformProvider
	{
		private readonly bool m_DevelopmentBuild;
		public override bool emitNullChecks
		{
			get
			{
				return this.m_DevelopmentBuild;
			}
		}
		public override bool enableStackTraces
		{
			get
			{
				return this.m_DevelopmentBuild;
			}
		}
		public override string nativeLibraryFileName
		{
			get
			{
				return "UserAssembly.dylib";
			}
		}
		public OSXStandaloneIl2CppPlatformProvider(BuildTarget target, string dataFolder, bool developmentBuild) : base(target, Path.Combine(dataFolder, "Libraries"))
		{
			this.m_DevelopmentBuild = developmentBuild;
		}
		public override INativeCompiler CreateNativeCompiler()
		{
			BuildTarget target = this.target;
			if (target != BuildTarget.StandaloneOSXIntel)
			{
				throw new Exception("This OSX arch isn't supported yet. " + this.target);
			}
			ICompilerSettings settings = new ClangCompilerSettingsx86();
			return new ClangCompiler(settings);
		}
	}
}
