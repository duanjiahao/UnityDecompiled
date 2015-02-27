using System;
using System.IO;
using UnityEditorInternal;
namespace UnityEditor.LinuxStandalone
{
	internal class LinuxStandaloneIl2CppPlatformProvider : BaseIl2CppPlatformProvider
	{
		public bool platformHasPrecompiledLibIl2Cpp
		{
			get
			{
				return false;
			}
		}
		public override bool emitNullChecks
		{
			get
			{
				return false;
			}
		}
		public override bool enableStackTraces
		{
			get
			{
				return false;
			}
		}
		public override string nativeLibraryFileName
		{
			get
			{
				return "libUserAssembly.so";
			}
		}
		public LinuxStandaloneIl2CppPlatformProvider(BuildTarget target, string dataFolder) : base(target, Path.Combine(dataFolder, "Libraries"))
		{
		}
		public override INativeCompiler CreateNativeCompiler()
		{
			BuildTarget target = this.target;
			ICompilerSettings settings;
			if (target != BuildTarget.StandaloneLinux)
			{
				if (target != BuildTarget.StandaloneLinux64)
				{
					throw new Exception("Not sure how to handle Linux universal yet...");
				}
				settings = new GccCompilerSettingsx86_64();
			}
			else
			{
				settings = new GccCompilerSettingsx86();
			}
			return new GccCompiler(settings);
		}
	}
}
