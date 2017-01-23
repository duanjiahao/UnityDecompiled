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

		private static BuildTarget GetMonoExecTarget(BuildTarget buildTarget)
		{
			BuildTarget result = buildTarget;
			switch (buildTarget)
			{
			case BuildTarget.PSP2:
			case BuildTarget.PS4:
			case BuildTarget.XboxOne:
			case BuildTarget.WiiU:
				result = BuildTarget.StandaloneWindows64;
				break;
			}
			return result;
		}

		public static string GetMonoExec(BuildTarget buildTarget)
		{
			string monoBinDirectory = BuildPipeline.GetMonoBinDirectory(buildTarget);
			string result;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				result = Path.Combine(monoBinDirectory, "mono.exe");
			}
			else
			{
				result = Path.Combine(monoBinDirectory, "mono");
			}
			return result;
		}

		public static string GetMonoPath(BuildTarget buildTarget)
		{
			string monoLibDirectory = BuildPipeline.GetMonoLibDirectory(buildTarget);
			return monoLibDirectory + Path.PathSeparator + ".";
		}

		public static Process PrepareMonoProcess(BuildTarget target, string workDir)
		{
			BuildTarget monoExecTarget = MonoProcessUtility.GetMonoExecTarget(target);
			Process process = new Process();
			process.StartInfo.FileName = MonoProcessUtility.GetMonoExec(monoExecTarget);
			process.StartInfo.EnvironmentVariables["_WAPI_PROCESS_HANDLE_OFFSET"] = "5";
			process.StartInfo.EnvironmentVariables["MONO_PATH"] = MonoProcessUtility.GetMonoPath(monoExecTarget);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WorkingDirectory = workDir;
			return process;
		}
	}
}
