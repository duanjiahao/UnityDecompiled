using System;
using System.Collections.Generic;
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
			Runner.RunManagedProgram(exe, args, Application.dataPath + "/..", null);
		}
		internal static void RunManagedProgram(string exe, string args, string workingDirectory, CompilerOutputParserBase parser)
		{
			using (ManagedProgram managedProgram = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, args))
			{
				managedProgram.GetProcessStartInfo().WorkingDirectory = workingDirectory;
				managedProgram.Start();
				managedProgram.WaitForExit();
				if (managedProgram.ExitCode != 0)
				{
					if (parser != null)
					{
						string[] errorOutput = managedProgram.GetErrorOutput();
						string[] standardOutput = managedProgram.GetStandardOutput();
						IEnumerable<CompilerMessage> enumerable = parser.Parse(errorOutput, standardOutput, true);
						foreach (CompilerMessage current in enumerable)
						{
							Debug.LogPlayerBuildError(current.message, current.file, current.line, current.column);
						}
					}
					Debug.LogError(string.Concat(new string[]
					{
						"Failed running ",
						exe,
						" ",
						args,
						"\n\n",
						managedProgram.GetAllOutput()
					}));
					throw new Exception(string.Format("{0} did not run properly!", exe));
				}
			}
		}
		public static void RunNativeProgram(string exe, string args)
		{
			using (NativeProgram nativeProgram = new NativeProgram(exe, args))
			{
				nativeProgram.Start();
				nativeProgram.WaitForExit();
				if (nativeProgram.ExitCode != 0)
				{
					Debug.LogError(string.Concat(new string[]
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
	}
}
