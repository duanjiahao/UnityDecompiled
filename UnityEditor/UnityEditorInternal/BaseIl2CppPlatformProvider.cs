using System;
using System.IO;
using System.Linq;
using Unity.DataContract;
using UnityEditor;
using UnityEditor.BuildReporting;
using UnityEditor.Modules;

namespace UnityEditorInternal
{
	internal class BaseIl2CppPlatformProvider : IIl2CppPlatformProvider
	{
		public virtual BuildTarget target
		{
			get;
			private set;
		}

		public virtual string libraryFolder
		{
			get;
			private set;
		}

		public virtual bool developmentMode
		{
			get
			{
				return false;
			}
		}

		public virtual bool emitNullChecks
		{
			get
			{
				return true;
			}
		}

		public virtual bool enableStackTraces
		{
			get
			{
				return true;
			}
		}

		public virtual bool enableArrayBoundsCheck
		{
			get
			{
				return true;
			}
		}

		public virtual bool enableDivideByZeroCheck
		{
			get
			{
				return false;
			}
		}

		public virtual bool supportsEngineStripping
		{
			get
			{
				return false;
			}
		}

		public virtual BuildReport buildReport
		{
			get
			{
				return null;
			}
		}

		public virtual string[] includePaths
		{
			get
			{
				return new string[]
				{
					this.GetFolderInPackageOrDefault("bdwgc/include"),
					this.GetFolderInPackageOrDefault("libil2cpp/include")
				};
			}
		}

		public virtual string[] libraryPaths
		{
			get
			{
				return new string[]
				{
					this.GetFileInPackageOrDefault("bdwgc/lib/bdwgc." + this.staticLibraryExtension),
					this.GetFileInPackageOrDefault("libil2cpp/lib/libil2cpp." + this.staticLibraryExtension)
				};
			}
		}

		public virtual string nativeLibraryFileName
		{
			get
			{
				return null;
			}
		}

		public virtual string staticLibraryExtension
		{
			get
			{
				return "a";
			}
		}

		public virtual string il2CppFolder
		{
			get
			{
				Unity.DataContract.PackageInfo packageInfo = BaseIl2CppPlatformProvider.FindIl2CppPackage();
				string result;
				if (packageInfo == null)
				{
					result = Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, "il2cpp"));
				}
				else
				{
					result = packageInfo.basePath;
				}
				return result;
			}
		}

		public virtual string moduleStrippingInformationFolder
		{
			get
			{
				return Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(EditorUserBuildSettings.activeBuildTarget, BuildOptions.None), "Whitelists");
			}
		}

		public BaseIl2CppPlatformProvider(BuildTarget target, string libraryFolder)
		{
			this.target = target;
			this.libraryFolder = libraryFolder;
		}

		public virtual INativeCompiler CreateNativeCompiler()
		{
			return null;
		}

		public virtual Il2CppNativeCodeBuilder CreateIl2CppNativeCodeBuilder()
		{
			return null;
		}

		protected string GetFolderInPackageOrDefault(string path)
		{
			Unity.DataContract.PackageInfo packageInfo = BaseIl2CppPlatformProvider.FindIl2CppPackage();
			string result;
			if (packageInfo == null)
			{
				result = Path.Combine(this.libraryFolder, path);
			}
			else
			{
				string text = Path.Combine(packageInfo.basePath, path);
				result = (Directory.Exists(text) ? text : Path.Combine(this.libraryFolder, path));
			}
			return result;
		}

		protected string GetFileInPackageOrDefault(string path)
		{
			Unity.DataContract.PackageInfo packageInfo = BaseIl2CppPlatformProvider.FindIl2CppPackage();
			string result;
			if (packageInfo == null)
			{
				result = Path.Combine(this.libraryFolder, path);
			}
			else
			{
				string text = Path.Combine(packageInfo.basePath, path);
				result = (File.Exists(text) ? text : Path.Combine(this.libraryFolder, path));
			}
			return result;
		}

		private static Unity.DataContract.PackageInfo FindIl2CppPackage()
		{
			return ModuleManager.packageManager.unityExtensions.FirstOrDefault((Unity.DataContract.PackageInfo e) => e.name == "IL2CPP");
		}
	}
}
