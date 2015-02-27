using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
namespace UnityEditor
{
	internal class MonoProcessUtility
	{
		public static string ProcessToString(Process process)
		{
			string text = string.Empty;
			foreach (string text2 in process.StartInfo.EnvironmentVariables.Keys)
			{
				text += string.Format("{0} = '{1}'\n", text2, process.StartInfo.EnvironmentVariables[text2]);
			}
			return string.Concat(new string[]
			{
				process.StartInfo.FileName,
				" ",
				process.StartInfo.Arguments,
				" current dir : ",
				process.StartInfo.WorkingDirectory,
				"\n Env: ",
				text
			});
		}
		public static void RunMonoProcess(Process process, string name, string resultingFile)
		{
			StringBuilder output = new StringBuilder(string.Empty);
			StringBuilder error = new StringBuilder(string.Empty);
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs dataLine)
			{
				if (!string.IsNullOrEmpty(dataLine.Data))
				{
					output.Append(dataLine.Data);
				}
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs dataLine)
			{
				if (!string.IsNullOrEmpty(dataLine.Data))
				{
					error.Append(dataLine.Data);
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			bool flag = process.WaitForExit(300000);
			if (process.ExitCode != 0 || !File.Exists(resultingFile))
			{
				process.CancelOutputRead();
				process.CancelErrorRead();
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
					output,
					"\n\n"
				});
				text = text + "stderr:\n" + error;
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
