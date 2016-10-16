using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor.Utils;
using UnityEngine;

internal abstract class NativeCompiler : INativeCompiler
{
	private class Counter
	{
		public int index;
	}

	protected virtual string objectFileExtension
	{
		get
		{
			return "o";
		}
	}

	public abstract void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths);

	protected virtual void SetupProcessStartInfo(ProcessStartInfo startInfo)
	{
	}

	protected void Execute(string arguments, string compilerPath)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo(compilerPath, arguments);
		this.SetupProcessStartInfo(startInfo);
		this.RunProgram(startInfo);
	}

	protected void ExecuteCommand(string command, params string[] arguments)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments.Aggregate((string buff, string s) => buff + " " + s));
		this.SetupProcessStartInfo(startInfo);
		this.RunProgram(startInfo);
	}

	private void RunProgram(ProcessStartInfo startInfo)
	{
		using (Program program = new Program(startInfo))
		{
			program.Start();
			while (!program.WaitForExit(100))
			{
			}
			string text = string.Empty;
			string[] standardOutput = program.GetStandardOutput();
			if (standardOutput.Length > 0)
			{
				text = standardOutput.Aggregate((string buf, string s) => buf + Environment.NewLine + s);
			}
			string[] errorOutput = program.GetErrorOutput();
			if (errorOutput.Length > 0)
			{
				text += errorOutput.Aggregate((string buf, string s) => buf + Environment.NewLine + s);
			}
			if (program.ExitCode != 0)
			{
				UnityEngine.Debug.LogError(string.Concat(new string[]
				{
					"Failed running ",
					startInfo.FileName,
					" ",
					startInfo.Arguments,
					"\n\n",
					text
				}));
				throw new Exception("IL2CPP compile failed.");
			}
		}
	}

	protected static string Aggregate(IEnumerable<string> items, string prefix, string suffix)
	{
		return items.Aggregate(string.Empty, (string current, string additionalFile) => current + prefix + additionalFile + suffix);
	}

	internal static void ParallelFor<T>(T[] sources, Action<T> action)
	{
		Thread[] array = new Thread[Environment.ProcessorCount];
		NativeCompiler.Counter parameter = new NativeCompiler.Counter();
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Thread(delegate(object obj)
			{
				NativeCompiler.Counter counter = (NativeCompiler.Counter)obj;
				int num;
				while ((num = Interlocked.Increment(ref counter.index)) <= sources.Length)
				{
					action(sources[num - 1]);
				}
			});
		}
		Thread[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			Thread thread = array2[j];
			thread.Start(parameter);
		}
		Thread[] array3 = array;
		for (int k = 0; k < array3.Length; k++)
		{
			Thread thread2 = array3[k];
			thread2.Join();
		}
	}

	protected internal static IEnumerable<string> AllSourceFilesIn(string directory)
	{
		return Directory.GetFiles(directory, "*.cpp", SearchOption.AllDirectories).Concat(Directory.GetFiles(directory, "*.c", SearchOption.AllDirectories));
	}

	protected internal static bool IsSourceFile(string source)
	{
		string extension = Path.GetExtension(source);
		return extension == "cpp" || extension == "c";
	}

	protected string ObjectFileFor(string source)
	{
		return Path.ChangeExtension(source, this.objectFileExtension);
	}
}
