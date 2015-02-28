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
		internal static string GetNETCoreFrameworkReferencesDirectory(WSASDK wsaSDK)
		{
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
				return MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\.NETCore\\v4.5";
			case WSASDK.SDK81:
				return MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\.NETCore\\v4.5.1";
			case WSASDK.PhoneSDK81:
				return MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\WindowsPhoneApp\\v8.1";
			default:
				throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
			}
		}
		private string[] GetNETWSAAssemblies(WSASDK wsaSDK)
		{
			return Directory.GetFiles(MicrosoftCSharpCompiler.GetNETCoreFrameworkReferencesDirectory(wsaSDK), "*.dll");
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
		private void FillNETCoreCompilerOptions(WSASDK wsaSDK, List<string> arguments, ref string argsPrefix)
		{
			argsPrefix = "/noconfig ";
			arguments.Add("/nostdlib+");
			arguments.Add("/define:NETFX_CORE");
			string platformAssemblyPath = MicrosoftCSharpCompiler.GetPlatformAssemblyPath(wsaSDK);
			string arg;
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
				arg = "8.0";
				break;
			case WSASDK.SDK81:
				arg = "8.1";
				break;
			case WSASDK.PhoneSDK81:
				arg = "Phone 8.1";
				break;
			default:
				throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
			}
			if (!File.Exists(platformAssemblyPath))
			{
				throw new Exception(string.Format("'{0}' not found, do you have Windows {1} SDK installed?", platformAssemblyPath, arg));
			}
			arguments.Add("/reference:\"" + platformAssemblyPath + "\"");
			string[] nETWSAAssemblies = this.GetNETWSAAssemblies(wsaSDK);
			for (int i = 0; i < nETWSAAssemblies.Length; i++)
			{
				string str = nETWSAAssemblies[i];
				arguments.Add("/reference:\"" + str + "\"");
			}
			string text;
			string contents;
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
				text = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.AssemblyAttributes.cs");
				contents = this.GetNetWSAAssemblyInfoWindows80();
				break;
			case WSASDK.SDK81:
				text = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.1.AssemblyAttributes.cs");
				contents = this.GetNetWSAAssemblyInfoWindows81();
				break;
			case WSASDK.PhoneSDK81:
				text = Path.Combine(Path.GetTempPath(), "WindowsPhoneApp,Version=v8.1.AssemblyAttributes.cs");
				contents = this.GetNetWSAAssemblyInfoWindowsPhone81();
				break;
			default:
				throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
			}
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.WriteAllText(text, contents);
			arguments.Add(text);
			string text2 = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(this._island._target, BuildOptions.None), "Managed\\WinRTLegacy.dll");
			arguments.Add("/reference:\"" + text2.Replace('/', '\\') + "\"");
		}
		private Program StartCompilerImpl(List<string> arguments, string argsPrefix)
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
			string text = Path.Combine(MicrosoftCSharpCompiler.WindowsDirectory, "Microsoft.NET\\Framework\\v4.0.30319\\Csc.exe");
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
			return this.StartCompilerImpl(list, empty);
		}
		protected override string[] GetStreamContainingCompilerMessages()
		{
			return base.GetStandardOutput();
		}
		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new MicrosoftCSharpCompilerOutputParser();
		}
		protected static string GetPlatformAssemblyPath(WSASDK wsaSDK)
		{
			string text;
			string path;
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v8.0", "InstallationFolder");
				path = "Windows Kits\\8.0";
				break;
			case WSASDK.SDK81:
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\v8.1", "InstallationFolder");
				path = "Windows Kits\\8.1";
				break;
			case WSASDK.PhoneSDK81:
				text = RegistryUtil.GetRegistryStringValue32("SOFTWARE\\Microsoft\\Microsoft SDKs\\WindowsPhoneApp\\v8.1", "InstallationFolder");
				path = "Windows Phone Kits\\8.1";
				break;
			default:
				throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
			}
			if (text == null)
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				text = Path.Combine(folderPath, path);
			}
			return Path.Combine(text, "References\\CommonConfiguration\\Neutral\\Windows.winmd");
		}
	}
}
