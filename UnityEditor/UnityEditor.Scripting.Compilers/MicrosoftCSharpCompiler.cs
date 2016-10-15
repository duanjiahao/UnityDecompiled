using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Scripting.Compilers
{
	internal class MicrosoftCSharpCompiler : ScriptCompilerBase
	{
		private static string[] _uwpReferences;

		internal static string WindowsDirectory
		{
			get
			{
				return Environment.GetEnvironmentVariable("windir");
			}
		}

		internal static string ProgramFilesDirectory
		{
			get
			{
				string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
				string result;
				if (Directory.Exists(environmentVariable))
				{
					result = environmentVariable;
				}
				else
				{
					UnityEngine.Debug.Log("Env variables ProgramFiles(x86) & ProgramFiles didn't exist, trying hard coded paths");
					string fullPath = Path.GetFullPath(MicrosoftCSharpCompiler.WindowsDirectory + "\\..\\..");
					string text = fullPath + "Program Files (x86)";
					string text2 = fullPath + "Program Files";
					if (Directory.Exists(text))
					{
						result = text;
					}
					else
					{
						if (!Directory.Exists(text2))
						{
							throw new Exception(string.Concat(new string[]
							{
								"Path '",
								text,
								"' or '",
								text2,
								"' doesn't exist."
							}));
						}
						result = text2;
					}
				}
				return result;
			}
		}

		public MicrosoftCSharpCompiler(MonoIsland island, bool runUpdater) : base(island)
		{
		}

		private static ScriptingImplementation GetCurrentScriptingBackend()
		{
			int result = 0;
			if (!PlayerSettings.GetPropertyOptionalInt("ScriptingBackend", ref result, BuildTargetGroup.Metro))
			{
				result = 2;
			}
			return (ScriptingImplementation)result;
		}

		private static string[] GetReferencesFromMonoDistribution()
		{
			return new string[]
			{
				"mscorlib.dll",
				"System.dll",
				"System.Core.dll",
				"System.Runtime.Serialization.dll",
				"System.Xml.dll",
				"System.Xml.Linq.dll",
				"UnityScript.dll",
				"UnityScript.Lang.dll",
				"Boo.Lang.dll"
			};
		}

		internal static string GetNETCoreFrameworkReferencesDirectory(WSASDK wsaSDK)
		{
			if (MicrosoftCSharpCompiler.GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP)
			{
				return BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer);
			}
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
				return MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\.NETCore\\v4.5";
			case WSASDK.SDK81:
				return MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\.NETCore\\v4.5.1";
			case WSASDK.PhoneSDK81:
				return MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\WindowsPhoneApp\\v8.1";
			case WSASDK.UWP:
				return null;
			}
			throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
		}

		private string[] GetNETWSAAssemblies(WSASDK wsaSDK)
		{
			if (MicrosoftCSharpCompiler.GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP)
			{
				string monoAssemblyDirectory = BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer);
				return (from dll in MicrosoftCSharpCompiler.GetReferencesFromMonoDistribution()
				select Path.Combine(monoAssemblyDirectory, dll)).ToArray<string>();
			}
			if (wsaSDK != WSASDK.UWP)
			{
				return Directory.GetFiles(MicrosoftCSharpCompiler.GetNETCoreFrameworkReferencesDirectory(wsaSDK), "*.dll");
			}
			NuGetPackageResolver nuGetPackageResolver = new NuGetPackageResolver
			{
				ProjectLockFile = "UWP\\project.lock.json"
			};
			return nuGetPackageResolver.Resolve();
		}

		private string GetNetWSAAssemblyInfoWindows80()
		{
			return string.Join("\r\n", new string[]
			{
				"using System;",
				" using System.Reflection;",
				"[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v4.5\", FrameworkDisplayName = \".NET for Windows Store apps\")]"
			});
		}

		private string GetNetWSAAssemblyInfoWindows81()
		{
			return string.Join("\r\n", new string[]
			{
				"using System;",
				" using System.Reflection;",
				"[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v4.5.1\", FrameworkDisplayName = \".NET for Windows Store apps (Windows 8.1)\")]"
			});
		}

		private string GetNetWSAAssemblyInfoWindowsPhone81()
		{
			return string.Join("\r\n", new string[]
			{
				"using System;",
				" using System.Reflection;",
				"[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\"WindowsPhoneApp,Version=v8.1\", FrameworkDisplayName = \"Windows Phone 8.1\")]"
			});
		}

		private string GetNetWSAAssemblyInfoUWP()
		{
			return string.Join("\r\n", new string[]
			{
				"using System;",
				"using System.Reflection;",
				"[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v5.0\", FrameworkDisplayName = \".NET for Windows Universal\")]"
			});
		}

		private void FillNETCoreCompilerOptions(WSASDK wsaSDK, List<string> arguments, ref string argsPrefix)
		{
			argsPrefix = "/noconfig ";
			arguments.Add("/nostdlib+");
			if (MicrosoftCSharpCompiler.GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
			{
				arguments.Add("/define:NETFX_CORE");
			}
			arguments.Add("/preferreduilang:en-US");
			string platformAssemblyPath = MicrosoftCSharpCompiler.GetPlatformAssemblyPath(wsaSDK);
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
			{
				string arg = "8.0";
				goto IL_BE;
			}
			case WSASDK.SDK81:
			{
				string arg = "8.1";
				goto IL_BE;
			}
			case WSASDK.PhoneSDK81:
			{
				string arg = "Phone 8.1";
				goto IL_BE;
			}
			case WSASDK.UWP:
			{
				string arg = "UAP";
				if (MicrosoftCSharpCompiler.GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
				{
					arguments.Add("/define:WINDOWS_UWP");
				}
				goto IL_BE;
			}
			}
			throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
			IL_BE:
			if (!File.Exists(platformAssemblyPath))
			{
				string arg;
				throw new Exception(string.Format("'{0}' not found, do you have Windows {1} SDK installed?", platformAssemblyPath, arg));
			}
			arguments.Add("/reference:\"" + platformAssemblyPath + "\"");
			string[] additionalReferences = MicrosoftCSharpCompiler.GetAdditionalReferences(wsaSDK);
			if (additionalReferences != null)
			{
				string[] array = additionalReferences;
				for (int i = 0; i < array.Length; i++)
				{
					string str = array[i];
					arguments.Add("/reference:\"" + str + "\"");
				}
			}
			string[] nETWSAAssemblies = this.GetNETWSAAssemblies(wsaSDK);
			for (int j = 0; j < nETWSAAssemblies.Length; j++)
			{
				string str2 = nETWSAAssemblies[j];
				arguments.Add("/reference:\"" + str2 + "\"");
			}
			if (MicrosoftCSharpCompiler.GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
			{
				string text;
				string contents;
				string text2;
				switch (wsaSDK)
				{
				case WSASDK.SDK80:
					text = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.AssemblyAttributes.cs");
					contents = this.GetNetWSAAssemblyInfoWindows80();
					text2 = "Managed\\WinRTLegacy.dll";
					goto IL_257;
				case WSASDK.SDK81:
					text = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.1.AssemblyAttributes.cs");
					contents = this.GetNetWSAAssemblyInfoWindows81();
					text2 = "Managed\\WinRTLegacy.dll";
					goto IL_257;
				case WSASDK.PhoneSDK81:
					text = Path.Combine(Path.GetTempPath(), "WindowsPhoneApp,Version=v8.1.AssemblyAttributes.cs");
					contents = this.GetNetWSAAssemblyInfoWindowsPhone81();
					text2 = "Managed\\Phone\\WinRTLegacy.dll";
					goto IL_257;
				case WSASDK.UWP:
					text = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v5.0.AssemblyAttributes.cs");
					contents = this.GetNetWSAAssemblyInfoUWP();
					text2 = "Managed\\UAP\\WinRTLegacy.dll";
					goto IL_257;
				}
				throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
				IL_257:
				text2 = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(this._island._target, BuildOptions.None), text2);
				arguments.Add("/reference:\"" + text2.Replace('/', '\\') + "\"");
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				File.WriteAllText(text, contents);
				arguments.Add(text);
			}
		}

		private Program StartCompilerImpl(List<string> arguments, string argsPrefix, bool msBuildCompiler)
		{
			string[] references = this._island._references;
			for (int i = 0; i < references.Length; i++)
			{
				string fileName = references[i];
				arguments.Add("/reference:" + ScriptCompilerBase.PrepareFileName(fileName));
			}
			foreach (string current in this._island._defines.Distinct<string>())
			{
				arguments.Add("/define:" + current);
			}
			string[] files = this._island._files;
			for (int j = 0; j < files.Length; j++)
			{
				string fileName2 = files[j];
				arguments.Add(ScriptCompilerBase.PrepareFileName(fileName2).Replace('/', '\\'));
			}
			string text;
			if (msBuildCompiler)
			{
				text = Path.Combine(MicrosoftCSharpCompiler.ProgramFilesDirectory, "MSBuild\\14.0\\Bin\\csc.exe");
			}
			else
			{
				text = Path.Combine(MicrosoftCSharpCompiler.WindowsDirectory, "Microsoft.NET\\Framework\\v4.0.30319\\Csc.exe");
			}
			if (!File.Exists(text))
			{
				throw new Exception("'" + text + "' not found, either .NET 4.5 is not installed or your OS is not Windows 8/8.1.");
			}
			base.AddCustomResponseFileIfPresent(arguments, "csc.rsp");
			string str = CommandLineFormatter.GenerateResponseFile(arguments);
			ProcessStartInfo si = new ProcessStartInfo
			{
				Arguments = argsPrefix + "@" + str,
				FileName = text,
				CreateNoWindow = true
			};
			Program program = new Program(si);
			program.Start();
			return program;
		}

		protected override Program StartCompiler()
		{
			string str = ScriptCompilerBase.PrepareFileName(this._island._output);
			List<string> list = new List<string>
			{
				"/target:library",
				"/nowarn:0169",
				"/out:" + str
			};
			list.InsertRange(0, new string[]
			{
				"/debug:pdbonly",
				"/optimize+"
			});
			string empty = string.Empty;
			if (base.CompilingForWSA() && (PlayerSettings.WSA.compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCore || PlayerSettings.WSA.compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCorePartially))
			{
				this.FillNETCoreCompilerOptions(EditorUserBuildSettings.wsaSDK, list, ref empty);
			}
			return this.StartCompilerImpl(list, empty, EditorUserBuildSettings.wsaSDK == WSASDK.UWP);
		}

		protected override string[] GetStreamContainingCompilerMessages()
		{
			return base.GetStandardOutput();
		}

		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new MicrosoftCSharpCompilerOutputParser();
		}

		internal static string GetWindowsKitDirectory(WSASDK wsaSDK)
		{
			string text;
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
			{
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v8.0", "InstallationFolder", null);
				string path = "Windows Kits\\8.0";
				goto IL_AC;
			}
			case WSASDK.SDK81:
			{
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v8.1", "InstallationFolder", null);
				string path = "Windows Kits\\8.1";
				goto IL_AC;
			}
			case WSASDK.PhoneSDK81:
			{
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\WindowsPhoneApp\\v8.1", "InstallationFolder", null);
				string path = "Windows Phone Kits\\8.1";
				goto IL_AC;
			}
			case WSASDK.UWP:
			{
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v10.0", "InstallationFolder", null);
				string path = "Windows Kits\\10.0";
				goto IL_AC;
			}
			}
			throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
			IL_AC:
			if (text == null)
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				string path;
				text = Path.Combine(folderPath, path);
			}
			return text;
		}

		internal static string[] GetAdditionalReferences(WSASDK wsaSDK)
		{
			if (wsaSDK != WSASDK.UWP)
			{
				return null;
			}
			if (MicrosoftCSharpCompiler._uwpReferences != null)
			{
				return MicrosoftCSharpCompiler._uwpReferences;
			}
			MicrosoftCSharpCompiler._uwpReferences = UWPReferences.GetReferences();
			return MicrosoftCSharpCompiler._uwpReferences;
		}

		protected static string GetPlatformAssemblyPath(WSASDK wsaSDK)
		{
			string windowsKitDirectory = MicrosoftCSharpCompiler.GetWindowsKitDirectory(wsaSDK);
			if (wsaSDK == WSASDK.UWP)
			{
				return Path.Combine(windowsKitDirectory, "UnionMetadata\\Facade\\Windows.winmd");
			}
			return Path.Combine(windowsKitDirectory, "References\\CommonConfiguration\\Neutral\\Windows.winmd");
		}
	}
}
