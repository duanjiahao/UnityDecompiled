using System;
using System.Diagnostics;
using System.IO;
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

		public static string GetMonoExec(BuildTarget buildTarget)
		{
			string monoBinDirectory = BuildPipeline.GetMonoBinDirectory(buildTarget);
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				return Path.Combine(monoBinDirectory, "mono");
			}
			return Path.Combine(monoBinDirectory, "mono.exe");
		}

		public static string GetMonoPath(BuildTarget buildTarget)
		{
			string monoLibDirectory = BuildPipeline.GetMonoLibDirectory(buildTarget);
			return monoLibDirectory + Path.PathSeparator + ".";
		}

		public static Process PrepareMonoProcess(BuildTarget target, string workDir)
		{
			Process process = new Process();
			process.StartInfo.FileName = MonoProcessUtility.GetMonoExec(target);
			process.StartInfo.EnvironmentVariables["_WAPI_PROCESS_HANDLE_OFFSET"] = "5";
			process.StartInfo.EnvironmentVariables["MONO_PATH"] = MonoProcessUtility.GetMonoPath(target);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WorkingDirectory = workDir;
			return process;
		}
	}
}
