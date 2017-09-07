using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Scripting;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class Runner
	{
		internal static void RunManagedProgram(string exe, string args)
		{
			Runner.RunManagedProgram(exe, args, Application.dataPath + "/..", null, null);
		}

		internal static void RunManagedProgram(string exe, string args, string workingDirectory, CompilerOutputParserBase parser, Action<ProcessStartInfo> setupStartInfo)
		{
			Program p;
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo
				{
					Arguments = args,
					CreateNoWindow = true,
					FileName = exe
				};
				if (setupStartInfo != null)
				{
					setupStartInfo(processStartInfo);
				}
				p = new Program(processStartInfo);
			}
			else
			{
				p = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, exe, args, false, setupStartInfo);
			}
			Runner.RunProgram(p, exe, args, workingDirectory, parser);
		}

		internal static void RunNetCoreProgram(string exe, string args, string workingDirectory, CompilerOutputParserBase parser, Action<ProcessStartInfo> setupStartInfo)
		{
			NetCoreProgram p = new NetCoreProgram(exe, args, setupStartInfo);
			Runner.RunProgram(p, exe, args, workingDirectory, parser);
		}

		public static void RunNativeProgram(string exe, string args)
		{
			using (NativeProgram nativeProgram = new NativeProgram(exe, args))
			{
				nativeProgram.Start();
				nativeProgram.WaitForExit();
				if (nativeProgram.ExitCode != 0)
				{
					UnityEngine.Debug.LogError(string.Concat(new string[]
					{
						"Failed running ",
						exe,
						" ",
						args,
						"\n\n",
						nativeProgram.GetAllOutput()
					}));
					throw new Exception(string.Format("{0} did not run properly!", exe));
				}
			}
		}

		private static void RunProgram(Program p, string exe, string args, string workingDirectory, CompilerOutputParserBase parser)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				p.GetProcessStartInfo().WorkingDirectory = workingDirectory;
				p.Start();
				p.WaitForExit();
				stopwatch.Stop();
				Console.WriteLine("{0} exited after {1} ms.", exe, stopwatch.ElapsedMilliseconds);
				IEnumerable<CompilerMessage> enumerable = null;
				if (parser != null)
				{
					string[] errorOutput = p.GetErrorOutput();
					string[] standardOutput = p.GetStandardOutput();
					enumerable = parser.Parse(errorOutput, standardOutput, true);
				}
				if (p.ExitCode != 0)
				{
					if (enumerable != null)
					{
						foreach (CompilerMessage current in enumerable)
						{
							UnityEngine.Debug.LogPlayerBuildError(current.message, current.file, current.line, current.column);
						}
					}
					UnityEngine.Debug.LogError(string.Concat(new string[]
					{
						"Failed running ",
						exe,
						" ",
						args,
						"\n\n",
						p.GetAllOutput()
					}));
					throw new Exception(string.Format("{0} did not run properly!", exe));
				}
				if (enumerable != null)
				{
					foreach (CompilerMessage current2 in enumerable)
					{
						Console.WriteLine(string.Concat(new object[]
						{
							current2.message,
							" - ",
							current2.file,
							" - ",
							current2.line,
							" - ",
							current2.column
						}));
					}
				}
			}
			finally
			{
				if (p != null)
				{
					((IDisposable)p).Dispose();
				}
			}
		}
	}
}
