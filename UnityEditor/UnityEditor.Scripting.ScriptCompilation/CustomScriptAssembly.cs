using System;
using System.IO;
using System.Linq;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal class CustomScriptAssembly
	{
		public string FilePath
		{
			get;
			set;
		}

		public string PathPrefix
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string[] References
		{
			get;
			set;
		}

		public CustomScriptAssemblyPlatform[] IncludePlatforms
		{
			get;
			set;
		}

		public CustomScriptAssemblyPlatform[] ExcludePlatforms
		{
			get;
			set;
		}

		public AssemblyFlags AssemblyFlags
		{
			get
			{
				AssemblyFlags result;
				if (this.IncludePlatforms != null && this.IncludePlatforms.Length == 1 && this.IncludePlatforms[0].BuildTarget == BuildTarget.NoTarget)
				{
					result = AssemblyFlags.EditorOnly;
				}
				else
				{
					result = AssemblyFlags.None;
				}
				return result;
			}
		}

		private static CustomScriptAssemblyPlatform[] Platforms
		{
			get;
			set;
		}

		static CustomScriptAssembly()
		{
			CustomScriptAssembly.Platforms = new CustomScriptAssemblyPlatform[23];
			int num = 0;
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("Editor", BuildTarget.NoTarget);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("OSXtandalone32", BuildTarget.StandaloneOSXIntel);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("OSXStandalone64", BuildTarget.StandaloneOSXIntel64);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("OSXStandaloneUniversal", BuildTarget.StandaloneOSXUniversal);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("WindowsStandalone32", BuildTarget.StandaloneWindows);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("WindowsStandalone64", BuildTarget.StandaloneWindows64);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("LinuxStandalone32", BuildTarget.StandaloneLinux);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("LinuxStandalone64", BuildTarget.StandaloneLinux64);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("LinuxStandaloneUniversal", BuildTarget.StandaloneLinuxUniversal);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("iOS", BuildTarget.iOS);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("Android", BuildTarget.Android);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("WebGL", BuildTarget.WebGL);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("WSA", BuildTarget.WSAPlayer);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("Tizen", BuildTarget.Tizen);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("PSVita", BuildTarget.PSP2);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("PS4", BuildTarget.PS4);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("PSMobile", BuildTarget.PSM);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("XboxOne", BuildTarget.XboxOne);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("Nintendo3DS", BuildTarget.N3DS);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("WiiU", BuildTarget.WiiU);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("tvOS", BuildTarget.tvOS);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("SamsungTV", BuildTarget.SamsungTV);
			CustomScriptAssembly.Platforms[num++] = new CustomScriptAssemblyPlatform("Switch", BuildTarget.Switch);
		}

		public bool IsCompatibleWithEditor()
		{
			bool result;
			if (this.ExcludePlatforms != null)
			{
				result = this.ExcludePlatforms.All((CustomScriptAssemblyPlatform p) => p.BuildTarget != BuildTarget.NoTarget);
			}
			else if (this.IncludePlatforms != null)
			{
				result = this.IncludePlatforms.Any((CustomScriptAssemblyPlatform p) => p.BuildTarget == BuildTarget.NoTarget);
			}
			else
			{
				result = true;
			}
			return result;
		}

		public bool IsCompatibleWith(BuildTarget buildTarget, BuildFlags buildFlags)
		{
			bool result;
			if (this.IncludePlatforms == null && this.ExcludePlatforms == null)
			{
				result = true;
			}
			else
			{
				bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
				if (flag)
				{
					result = this.IsCompatibleWithEditor();
				}
				else
				{
					if (flag)
					{
						buildTarget = BuildTarget.NoTarget;
					}
					if (this.ExcludePlatforms != null)
					{
						result = this.ExcludePlatforms.All((CustomScriptAssemblyPlatform p) => p.BuildTarget != buildTarget);
					}
					else
					{
						result = this.IncludePlatforms.Any((CustomScriptAssemblyPlatform p) => p.BuildTarget == buildTarget);
					}
				}
			}
			return result;
		}

		public static CustomScriptAssembly FromCustomScriptAssemblyData(string path, CustomScriptAssemblyData customScriptAssemblyData)
		{
			CustomScriptAssembly result;
			if (customScriptAssemblyData == null)
			{
				result = null;
			}
			else
			{
				string directoryName = Path.GetDirectoryName(path);
				CustomScriptAssembly customScriptAssembly = new CustomScriptAssembly();
				customScriptAssembly.Name = customScriptAssemblyData.name;
				customScriptAssembly.References = customScriptAssemblyData.references;
				customScriptAssembly.FilePath = path;
				customScriptAssembly.PathPrefix = directoryName;
				if (customScriptAssemblyData.includePlatforms != null && customScriptAssemblyData.includePlatforms.Length > 0)
				{
					customScriptAssembly.IncludePlatforms = (from name in customScriptAssemblyData.includePlatforms
					select CustomScriptAssembly.GetPlatformFromName(name)).ToArray<CustomScriptAssemblyPlatform>();
				}
				if (customScriptAssemblyData.excludePlatforms != null && customScriptAssemblyData.excludePlatforms.Length > 0)
				{
					customScriptAssembly.ExcludePlatforms = (from name in customScriptAssemblyData.excludePlatforms
					select CustomScriptAssembly.GetPlatformFromName(name)).ToArray<CustomScriptAssemblyPlatform>();
				}
				result = customScriptAssembly;
			}
			return result;
		}

		public static CustomScriptAssemblyPlatform GetPlatformFromName(string name)
		{
			CustomScriptAssemblyPlatform[] platforms = CustomScriptAssembly.Platforms;
			for (int i = 0; i < platforms.Length; i++)
			{
				CustomScriptAssemblyPlatform result = platforms[i];
				if (string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return result;
				}
			}
			string[] value = (from p in CustomScriptAssembly.Platforms
			select string.Format("'{0}'", p.Name)).ToArray<string>();
			string arg = string.Join(",", value);
			throw new ArgumentException(string.Format("Platform name '{0}' not supported. Supported platform names: {1}", name, arg));
		}

		public static CustomScriptAssemblyPlatform GetPlatformFromBuildTarget(BuildTarget buildTarget)
		{
			CustomScriptAssemblyPlatform[] platforms = CustomScriptAssembly.Platforms;
			for (int i = 0; i < platforms.Length; i++)
			{
				CustomScriptAssemblyPlatform result = platforms[i];
				if (result.BuildTarget == buildTarget)
				{
					return result;
				}
			}
			throw new ArgumentException(string.Format("No CustomScriptAssemblyPlatform setup for BuildTarget '{0}'", buildTarget));
		}
	}
}
