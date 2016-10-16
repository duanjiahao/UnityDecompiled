using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;

namespace UnityEditor
{
	internal class ManagedEditorCodeRebuilder
	{
		private static readonly char[] kNewlineChars = new char[]
		{
			'\r',
			'\n'
		};

		private static bool Run(out CompilerMessage[] messages, bool includeModules)
		{
			int num;
			messages = ManagedEditorCodeRebuilder.ParseResults(ManagedEditorCodeRebuilder.GetOutputStream(ManagedEditorCodeRebuilder.GetJamStartInfo(includeModules), out num));
			return num == 0;
		}

		private static ProcessStartInfo GetJamStartInfo(bool includeModules)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("jam.pl LiveReloadableEditorAssemblies");
			if (includeModules)
			{
				foreach (string current in ModuleManager.GetJamTargets())
				{
					stringBuilder.Append(" ").Append(current);
				}
			}
			return new ProcessStartInfo
			{
				WorkingDirectory = Unsupported.GetBaseUnityDeveloperFolder(),
				RedirectStandardOutput = true,
				RedirectStandardError = false,
				Arguments = stringBuilder.ToString(),
				FileName = "perl"
			};
		}

		private static CompilerMessage[] ParseResults(string text)
		{
			Console.Write(text);
			string[] errorOutput = text.Split(ManagedEditorCodeRebuilder.kNewlineChars, StringSplitOptions.RemoveEmptyEntries);
			string baseUnityDeveloperFolder = Unsupported.GetBaseUnityDeveloperFolder();
			List<CompilerMessage> list = new MonoCSharpCompilerOutputParser().Parse(errorOutput, false).ToList<CompilerMessage>();
			for (int i = 0; i < list.Count; i++)
			{
				CompilerMessage value = list[i];
				value.file = Path.Combine(baseUnityDeveloperFolder, value.file);
				list[i] = value;
			}
			return list.ToArray();
		}

		private static string GetOutputStream(ProcessStartInfo startInfo, out int exitCode)
		{
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			Process process = new Process
			{
				StartInfo = startInfo
			};
			StringBuilder sbStandardOut = new StringBuilder();
			StringBuilder sbStandardError = new StringBuilder();
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs data)
			{
				sbStandardOut.AppendLine(data.Data);
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs data)
			{
				sbStandardError.AppendLine(data.Data);
			};
			process.Start();
			if (startInfo.RedirectStandardError)
			{
				process.BeginErrorReadLine();
			}
			else
			{
				process.BeginOutputReadLine();
			}
			process.WaitForExit();
			string result = (!startInfo.RedirectStandardError) ? sbStandardOut.ToString() : sbStandardError.ToString();
			exitCode = process.ExitCode;
			return result;
		}
	}
}
