using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace UnityEditor.Utils
{
	internal class Program : IDisposable
	{
		private ProcessOutputStreamReader _stdout;

		private ProcessOutputStreamReader _stderr;

		private Stream _stdin;

		public Process _process;

		public bool HasExited
		{
			get
			{
				if (this._process == null)
				{
					throw new InvalidOperationException("You cannot call HasExited before calling Start");
				}
				bool result;
				try
				{
					result = this._process.HasExited;
				}
				catch (InvalidOperationException)
				{
					result = true;
				}
				return result;
			}
		}

		public int ExitCode
		{
			get
			{
				return this._process.ExitCode;
			}
		}

		public int Id
		{
			get
			{
				return this._process.Id;
			}
		}

		protected Program()
		{
			this._process = new Process();
		}

		public Program(ProcessStartInfo si) : this()
		{
			this._process.StartInfo = si;
		}

		public void Start()
		{
			this.Start(null);
		}

		public void Start(EventHandler exitCallback)
		{
			if (exitCallback != null)
			{
				this._process.EnableRaisingEvents = true;
				this._process.Exited += exitCallback;
			}
			this._process.StartInfo.RedirectStandardInput = true;
			this._process.StartInfo.RedirectStandardError = true;
			this._process.StartInfo.RedirectStandardOutput = true;
			this._process.StartInfo.UseShellExecute = false;
			this._process.Start();
			this._stdout = new ProcessOutputStreamReader(this._process, this._process.StandardOutput);
			this._stderr = new ProcessOutputStreamReader(this._process, this._process.StandardError);
			this._stdin = this._process.StandardInput.BaseStream;
		}

		public ProcessStartInfo GetProcessStartInfo()
		{
			return this._process.StartInfo;
		}

		public void LogProcessStartInfo()
		{
			if (this._process != null)
			{
				Program.LogProcessStartInfo(this._process.StartInfo);
			}
			else
			{
				Console.WriteLine("Failed to retrieve process startInfo");
			}
		}

		private static void LogProcessStartInfo(ProcessStartInfo si)
		{
			Console.WriteLine("Filename: " + si.FileName);
			Console.WriteLine("Arguments: " + si.Arguments);
			IEnumerator enumerator = si.EnvironmentVariables.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					if (dictionaryEntry.Key.ToString().StartsWith("MONO"))
					{
						Console.WriteLine("{0}: {1}", dictionaryEntry.Key, dictionaryEntry.Value);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			int num = si.Arguments.IndexOf("Temp/UnityTempFile");
			Console.WriteLine("index: " + num);
			if (num > 0)
			{
				string text = si.Arguments.Substring(num);
				Console.WriteLine("Responsefile: " + text + " Contents: ");
				Console.WriteLine(File.ReadAllText(text));
			}
		}

		public string GetAllOutput()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("stdout:");
			string[] standardOutput = this.GetStandardOutput();
			for (int i = 0; i < standardOutput.Length; i++)
			{
				string value = standardOutput[i];
				stringBuilder.AppendLine(value);
			}
			stringBuilder.AppendLine("stderr:");
			string[] errorOutput = this.GetErrorOutput();
			for (int j = 0; j < errorOutput.Length; j++)
			{
				string value2 = errorOutput[j];
				stringBuilder.AppendLine(value2);
			}
			return stringBuilder.ToString();
		}

		public void Dispose()
		{
			this.Kill();
			this._process.Dispose();
		}

		public void Kill()
		{
			if (!this.HasExited)
			{
				this._process.Kill();
				this._process.WaitForExit();
			}
		}

		public Stream GetStandardInput()
		{
			return this._stdin;
		}

		public string[] GetStandardOutput()
		{
			return this._stdout.GetOutput();
		}

		public string GetStandardOutputAsString()
		{
			string[] standardOutput = this.GetStandardOutput();
			return Program.GetOutputAsString(standardOutput);
		}

		public string[] GetErrorOutput()
		{
			return this._stderr.GetOutput();
		}

		public string GetErrorOutputAsString()
		{
			string[] errorOutput = this.GetErrorOutput();
			return Program.GetOutputAsString(errorOutput);
		}

		private static string GetOutputAsString(string[] output)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < output.Length; i++)
			{
				string value = output[i];
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}

		public void WaitForExit()
		{
			this._process.WaitForExit();
		}

		public bool WaitForExit(int milliseconds)
		{
			return this._process.WaitForExit(milliseconds);
		}
	}
}
