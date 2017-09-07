using System;
using System.Diagnostics;
using System.IO;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor
{
	internal class MonoProcessUtility
	{
		public static string ProcessToString(Process process)
		{
			return string.Concat(new string[]
			{
				process.StartInfo.FileName,
				" ",
				process.StartInfo.Arguments,
				" current dir : ",
				process.StartInfo.WorkingDirectory,
				"\n"
			});
		}

		public static void RunMonoProcess(Process process, string name, string resultingFile)
		{
			MonoProcessRunner monoProcessRunner = new MonoProcessRunner();
			bool flag = monoProcessRunner.Run(process);
			if (process.ExitCode != 0 || !File.Exists(resultingFile))
			{
				string text = string.Concat(new object[]
				{
					"Failed ",
					name,
					": ",
					MonoProcessUtility.ProcessToString(process),
					" result file exists: ",
					File.Exists(resultingFile),
					". Timed out: ",
					!flag
				});
				text += "\n\n";
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"stdout:\n",
					monoProcessRunner.Output,
					"\n"
				});
				text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					"stderr:\n",
					monoProcessRunner.Error,
					"\n"
				});
				Console.WriteLine(text);
				throw new UnityException(text);
			}
		}

		public static Process PrepareMonoProcess(string workDir)
		{
			Process process = new Process();
			string text = (Application.platform != RuntimePlatform.WindowsEditor) ? "mono" : "mono.exe";
			process.StartInfo.FileName = Paths.Combine(new string[]
			{
				MonoInstallationFinder.GetMonoInstallation(),
				"bin",
				text
			});
			process.StartInfo.EnvironmentVariables["_WAPI_PROCESS_HANDLE_OFFSET"] = "5";
			string profile = BuildPipeline.CompatibilityProfileToClassLibFolder(ApiCompatibilityLevel.NET_2_0);
			process.StartInfo.EnvironmentVariables["MONO_PATH"] = MonoInstallationFinder.GetProfileDirectory(profile);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WorkingDirectory = workDir;
			return process;
		}

		public static Process PrepareMonoProcessBleedingEdge(string workDir)
		{
			Process process = new Process();
			string text = (Application.platform != RuntimePlatform.WindowsEditor) ? "mono" : "mono.exe";
			process.StartInfo.FileName = Paths.Combine(new string[]
			{
				MonoInstallationFinder.GetMonoBleedingEdgeInstallation(),
				"bin",
				text
			});
			process.StartInfo.EnvironmentVariables["_WAPI_PROCESS_HANDLE_OFFSET"] = "5";
			string profile = BuildPipeline.CompatibilityProfileToClassLibFolder(ApiCompatibilityLevel.NET_4_6);
			process.StartInfo.EnvironmentVariables["MONO_PATH"] = MonoInstallationFinder.GetProfileDirectory(profile);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WorkingDirectory = workDir;
			return process;
		}
	}
}
