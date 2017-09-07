using System;
using System.Diagnostics;
using System.IO;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor.Scripting
{
	internal class NetCoreProgram : Program
	{
		private static bool s_NetCoreAvailableChecked = false;

		private static bool s_NetCoreAvailable = false;

		public NetCoreProgram(string executable, string arguments, Action<ProcessStartInfo> setupStartInfo)
		{
			if (!NetCoreProgram.IsNetCoreAvailable())
			{
				UnityEngine.Debug.LogError("Creating NetCoreProgram, but IsNetCoreAvailable() == false; fix the caller!");
			}
			ProcessStartInfo processStartInfo = NetCoreProgram.CreateDotNetCoreStartInfoForArgs(CommandLineFormatter.PrepareFileName(executable) + " " + arguments);
			if (setupStartInfo != null)
			{
				setupStartInfo(processStartInfo);
			}
			this._process.StartInfo = processStartInfo;
		}

		private static ProcessStartInfo CreateDotNetCoreStartInfoForArgs(string arguments)
		{
			string text = Paths.Combine(new string[]
			{
				NetCoreProgram.GetSdkRoot(),
				"dotnet"
			});
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				text = CommandLineFormatter.PrepareFileName(text + ".exe");
			}
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				Arguments = arguments,
				CreateNoWindow = true,
				FileName = text,
				WorkingDirectory = Application.dataPath + "/.."
			};
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				string text2 = Path.Combine(Path.Combine(Path.Combine(NetCoreProgram.GetNetCoreRoot(), "NativeDeps"), "osx"), "lib");
				if (processStartInfo.EnvironmentVariables.ContainsKey("DYLD_LIBRARY_PATH"))
				{
					processStartInfo.EnvironmentVariables["DYLD_LIBRARY_PATH"] = string.Format("{0}:{1}", text2, processStartInfo.EnvironmentVariables["DYLD_LIBRARY_PATH"]);
				}
				else
				{
					processStartInfo.EnvironmentVariables.Add("DYLD_LIBRARY_PATH", text2);
				}
			}
			return processStartInfo;
		}

		private static string GetSdkRoot()
		{
			return Path.Combine(NetCoreProgram.GetNetCoreRoot(), "Sdk");
		}

		private static string GetNetCoreRoot()
		{
			return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "NetCore");
		}

		public static bool IsNetCoreAvailable()
		{
			bool result;
			if (!NetCoreProgram.s_NetCoreAvailableChecked)
			{
				NetCoreProgram.s_NetCoreAvailableChecked = true;
				ProcessStartInfo si = NetCoreProgram.CreateDotNetCoreStartInfoForArgs("--version");
				Program program = new Program(si);
				try
				{
					program.Start();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogWarningFormat("Disabling CoreCLR, got exception trying to run with --version: {0}", new object[]
					{
						ex
					});
					result = false;
					return result;
				}
				program.WaitForExit(5000);
				if (!program.HasExited)
				{
					program.Kill();
					UnityEngine.Debug.LogWarning("Disabling CoreCLR, timed out trying to run with --version");
					result = false;
					return result;
				}
				if (program.ExitCode != 0)
				{
					UnityEngine.Debug.LogWarningFormat("Disabling CoreCLR, got non-zero exit code: {0}, stderr: '{1}'", new object[]
					{
						program.ExitCode,
						program.GetErrorOutputAsString()
					});
					result = false;
					return result;
				}
				NetCoreProgram.s_NetCoreAvailable = true;
			}
			result = NetCoreProgram.s_NetCoreAvailable;
			return result;
		}
	}
}
