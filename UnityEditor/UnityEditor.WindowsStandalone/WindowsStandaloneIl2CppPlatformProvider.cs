using System;
using System.IO;
using UnityEditorInternal;
namespace UnityEditor.WindowsStandalone
{
	internal class WindowsStandaloneIl2CppPlatformProvider : BaseIl2CppPlatformProvider
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
				return "UserAssembly.dll";
			}
		}
		public override string staticLibraryExtension
		{
			get
			{
				return "lib";
			}
		}
		internal WindowsStandaloneIl2CppPlatformProvider(BuildTarget target, string dataFolder, bool developmentBuild) : base(target, Path.Combine(dataFolder, "Libraries"))
		{
			this.m_DevelopmentBuild = developmentBuild;
		}
		public override INativeCompiler CreateNativeCompiler()
		{
			BuildTarget target = this.target;
			ICompilerSettings settings;
			if (target != BuildTarget.StandaloneWindows)
			{
				if (target != BuildTarget.StandaloneWindows64)
				{
					throw new ArgumentException("Unexpected target: " + this.target);
				}
				settings = new MSVCCompilerSettingsx64();
			}
			else
			{
				settings = new MSVCCompilerSettingsx86();
			}
			return new MSVCCompiler(settings, base.GetFileInPackageOrDefault("libil2cpp/include/libil2cpp.def"));
		}
	}
}
