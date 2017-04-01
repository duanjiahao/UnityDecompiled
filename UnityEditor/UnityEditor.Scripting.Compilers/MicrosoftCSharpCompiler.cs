using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Modules;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor.Scripting.Compilers
{
	internal class MicrosoftCSharpCompiler : ScriptCompilerBase
	{
		private BuildTarget BuildTarget
		{
			get
			{
				return this._island._target;
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
					string fullPath = Path.GetFullPath(Environment.GetEnvironmentVariable("windir") + "\\..\\..");
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
			switch (wsaSDK)
			{
			case WSASDK.SDK80:
			{
				string result = MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\.NETCore\\v4.5";
				return result;
			}
			case WSASDK.SDK81:
			{
				string result = MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\.NETCore\\v4.5.1";
				return result;
			}
			case WSASDK.PhoneSDK81:
			{
				string result = MicrosoftCSharpCompiler.ProgramFilesDirectory + "\\Reference Assemblies\\Microsoft\\Framework\\WindowsPhoneApp\\v8.1";
				return result;
			}
			case WSASDK.UWP:
			{
				string result = null;
				return result;
			}
			}
			throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
		}

		private string[] GetClassLibraries()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this.BuildTarget);
			string[] result;
			if (PlayerSettings.GetScriptingBackend(buildTargetGroup) != ScriptingImplementation.WinRTDotNET)
			{
				string monoAssemblyDirectory = base.GetMonoProfileLibDirectory();
				List<string> list = new List<string>();
				list.AddRange(from dll in MicrosoftCSharpCompiler.GetReferencesFromMonoDistribution()
				select Path.Combine(monoAssemblyDirectory, dll));
				if (PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup) == ApiCompatibilityLevel.NET_4_6)
				{
					string path = Path.Combine(monoAssemblyDirectory, "Facades");
					list.Add(Path.Combine(path, "System.ObjectModel.dll"));
					list.Add(Path.Combine(path, "System.Runtime.dll"));
					list.Add(Path.Combine(path, "System.Runtime.InteropServices.WindowsRuntime.dll"));
				}
				result = list.ToArray();
			}
			else
			{
				if (this.BuildTarget != BuildTarget.WSAPlayer)
				{
					throw new InvalidOperationException(string.Format("MicrosoftCSharpCompiler cannot build for .NET Scripting backend for BuildTarget.{0}.", this.BuildTarget));
				}
				WSASDK wsaSDK = EditorUserBuildSettings.wsaSDK;
				if (wsaSDK != WSASDK.UWP)
				{
					result = Directory.GetFiles(MicrosoftCSharpCompiler.GetNETCoreFrameworkReferencesDirectory(wsaSDK), "*.dll");
				}
				else
				{
					NuGetPackageResolver nuGetPackageResolver = new NuGetPackageResolver
					{
						ProjectLockFile = "UWP\\project.lock.json"
					};
					result = nuGetPackageResolver.Resolve();
				}
			}
			return result;
		}

		private void FillCompilerOptions(List<string> arguments, out string argsPrefix)
		{
			argsPrefix = "/noconfig ";
			arguments.Add("/nostdlib+");
			arguments.Add("/preferreduilang:en-US");
			IPlatformSupportModule platformSupportModule = ModuleManager.FindPlatformSupportModule(ModuleManager.GetTargetStringFromBuildTarget(this.BuildTarget));
			ICompilationExtension compilationExtension = platformSupportModule.CreateCompilationExtension();
			arguments.AddRange(from r in this.GetClassLibraries()
			select "/reference:\"" + r + "\"");
			arguments.AddRange(from r in compilationExtension.GetAdditionalAssemblyReferences()
			select "/reference:\"" + r + "\"");
			arguments.AddRange(from r in compilationExtension.GetWindowsMetadataReferences()
			select "/reference:\"" + r + "\"");
			arguments.AddRange(from d in compilationExtension.GetAdditionalDefines()
			select "/define:" + d);
			arguments.AddRange(compilationExtension.GetAdditionalSourceFiles());
		}

		private static void ThrowCompilerNotFoundException(string path)
		{
			throw new Exception(string.Format("'{0}' not found. Is your Unity installation corrupted?", path));
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
			string text = Paths.Combine(new string[]
			{
				EditorApplication.applicationContentsPath,
				"Tools",
				"Roslyn",
				"CoreRun.exe"
			}).Replace('/', '\\');
			string text2 = Paths.Combine(new string[]
			{
				EditorApplication.applicationContentsPath,
				"Tools",
				"Roslyn",
				"csc.exe"
			}).Replace('/', '\\');
			if (!File.Exists(text))
			{
				MicrosoftCSharpCompiler.ThrowCompilerNotFoundException(text);
			}
			if (!File.Exists(text2))
			{
				MicrosoftCSharpCompiler.ThrowCompilerNotFoundException(text2);
			}
			base.AddCustomResponseFileIfPresent(arguments, "csc.rsp");
			string text3 = CommandLineFormatter.GenerateResponseFile(arguments);
			ProcessStartInfo si = new ProcessStartInfo
			{
				Arguments = string.Concat(new string[]
				{
					"\"",
					text2,
					"\" ",
					argsPrefix,
					"@",
					text3
				}),
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
			List<string> arguments = new List<string>
			{
				"/debug:pdbonly",
				"/optimize+",
				"/target:library",
				"/nowarn:0169",
				"/unsafe",
				"/out:" + str
			};
			string argsPrefix;
			this.FillCompilerOptions(arguments, out argsPrefix);
			return this.StartCompilerImpl(arguments, argsPrefix);
		}

		protected override string[] GetStreamContainingCompilerMessages()
		{
			return base.GetStandardOutput();
		}

		protected override CompilerOutputParserBase CreateOutputParser()
		{
			return new MicrosoftCSharpCompilerOutputParser();
		}
	}
}
