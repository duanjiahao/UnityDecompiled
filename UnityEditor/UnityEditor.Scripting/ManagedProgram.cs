using System;
using System.Diagnostics;
using System.IO;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor.Scripting
{
	internal class ManagedProgram : Program
	{
		public ManagedProgram(string monodistribution, string profile, string executable, string arguments, Action<ProcessStartInfo> setupStartInfo) : this(monodistribution, profile, executable, arguments, true, setupStartInfo)
		{
		}

		public ManagedProgram(string monodistribution, string profile, string executable, string arguments, bool setMonoEnvironmentVariables, Action<ProcessStartInfo> setupStartInfo)
		{
			string text = ManagedProgram.PathCombine(new string[]
			{
				monodistribution,
				"bin",
				"mono"
			});
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				text = CommandLineFormatter.PrepareFileName(text + ".exe");
			}
			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				Arguments = CommandLineFormatter.PrepareFileName(executable) + " " + arguments,
				CreateNoWindow = true,
				FileName = text,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				WorkingDirectory = Application.dataPath + "/..",
				UseShellExecute = false
			};
			if (setMonoEnvironmentVariables)
			{
				string value = ManagedProgram.PathCombine(new string[]
				{
					monodistribution,
					"lib",
					"mono",
					profile
				});
				processStartInfo.EnvironmentVariables["MONO_PATH"] = value;
				processStartInfo.EnvironmentVariables["MONO_CFG_DIR"] = ManagedProgram.PathCombine(new string[]
				{
					monodistribution,
					"etc"
				});
			}
			if (setupStartInfo != null)
			{
				setupStartInfo(processStartInfo);
			}
			this._process.StartInfo = processStartInfo;
		}

		private static string PathCombine(params string[] parts)
		{
			string text = parts[0];
			for (int i = 1; i < parts.Length; i++)
			{
				text = Path.Combine(text, parts[i]);
			}
			return text;
		}
	}
}
