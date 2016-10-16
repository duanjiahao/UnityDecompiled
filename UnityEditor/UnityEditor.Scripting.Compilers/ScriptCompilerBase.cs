using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor.Scripting.Compilers
{
	internal abstract class ScriptCompilerBase : IDisposable
	{
		private Program process;

		private string _responseFile;

		protected MonoIsland _island;

		protected ScriptCompilerBase(MonoIsland island)
		{
			this._island = island;
		}

		protected abstract Program StartCompiler();

		protected abstract CompilerOutputParserBase CreateOutputParser();

		protected string[] GetErrorOutput()
		{
			return this.process.GetErrorOutput();
		}

		protected string[] GetStandardOutput()
		{
			return this.process.GetStandardOutput();
		}

		protected bool CompilingForWSA()
		{
			return this._island._target == BuildTarget.WSAPlayer;
		}

		public void BeginCompiling()
		{
			if (this.process != null)
			{
				throw new InvalidOperationException("Compilation has already begun!");
			}
			this.process = this.StartCompiler();
		}

		public virtual void Dispose()
		{
			if (this.process != null)
			{
				this.process.Dispose();
				this.process = null;
			}
			if (this._responseFile != null)
			{
				File.Delete(this._responseFile);
				this._responseFile = null;
			}
		}

		public virtual bool Poll()
		{
			return this.process == null || this.process.HasExited;
		}

		protected void AddCustomResponseFileIfPresent(List<string> arguments, string responseFileName)
		{
			string text = Path.Combine("Assets", responseFileName);
			if (!File.Exists(text))
			{
				return;
			}
			arguments.Add("@" + text);
		}

		protected string GenerateResponseFile(List<string> arguments)
		{
			this._responseFile = CommandLineFormatter.GenerateResponseFile(arguments);
			return this._responseFile;
		}

		protected static string PrepareFileName(string fileName)
		{
			return CommandLineFormatter.PrepareFileName(fileName);
		}

		public virtual CompilerMessage[] GetCompilerMessages()
		{
			if (!this.Poll())
			{
				Debug.LogWarning("Compile process is not finished yet. This should not happen.");
			}
			this.DumpStreamOutputToLog();
			return this.CreateOutputParser().Parse(this.GetStreamContainingCompilerMessages(), this.CompilationHadFailure()).ToArray<CompilerMessage>();
		}

		protected bool CompilationHadFailure()
		{
			return this.process.ExitCode != 0;
		}

		protected virtual string[] GetStreamContainingCompilerMessages()
		{
			List<string> list = new List<string>();
			list.AddRange(this.GetErrorOutput());
			list.Add(string.Empty);
			list.AddRange(this.GetStandardOutput());
			return list.ToArray();
		}

		private void DumpStreamOutputToLog()
		{
			bool flag = this.CompilationHadFailure();
			string[] errorOutput = this.GetErrorOutput();
			if (flag || errorOutput.Length != 0)
			{
				Console.WriteLine(string.Empty);
				Console.WriteLine("-----Compiler Commandline Arguments:");
				this.process.LogProcessStartInfo();
				string[] standardOutput = this.GetStandardOutput();
				Console.WriteLine(string.Concat(new object[]
				{
					"-----CompilerOutput:-stdout--exitcode: ",
					this.process.ExitCode,
					"--compilationhadfailure: ",
					flag,
					"--outfile: ",
					this._island._output
				}));
				string[] array = standardOutput;
				for (int i = 0; i < array.Length; i++)
				{
					string value = array[i];
					Console.WriteLine(value);
				}
				Console.WriteLine("-----CompilerOutput:-stderr----------");
				string[] array2 = errorOutput;
				for (int j = 0; j < array2.Length; j++)
				{
					string value2 = array2[j];
					Console.WriteLine(value2);
				}
				Console.WriteLine("-----EndCompilerOutput---------------");
			}
		}
	}
}
