using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace UnityEditor
{
	internal class MonoProcessRunner
	{
		public StringBuilder Output = new StringBuilder(string.Empty);

		public StringBuilder Error = new StringBuilder(string.Empty);

		public bool Run(Process process)
		{
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			Thread thread = new Thread(new ParameterizedThreadStart(this.ReadOutput));
			Thread thread2 = new Thread(new ParameterizedThreadStart(this.ReadErrors));
			process.Start();
			thread.Start(process);
			thread2.Start(process);
			bool result = process.WaitForExit(600000);
			DateTime now = DateTime.Now;
			while ((thread.IsAlive || thread2.IsAlive) && (DateTime.Now - now).TotalMilliseconds < 5.0)
			{
				Thread.Sleep(0);
			}
			if (thread.IsAlive)
			{
				thread.Abort();
			}
			if (thread2.IsAlive)
			{
				thread2.Abort();
			}
			thread.Join();
			thread2.Join();
			return result;
		}

		private void ReadOutput(object process)
		{
			Process process2 = process as Process;
			try
			{
				using (StreamReader standardOutput = process2.StandardOutput)
				{
					this.Output.Append(standardOutput.ReadToEnd());
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		private void ReadErrors(object process)
		{
			Process process2 = process as Process;
			try
			{
				using (StreamReader standardError = process2.StandardError)
				{
					this.Error.Append(standardError.ReadToEnd());
				}
			}
			catch (ThreadAbortException)
			{
			}
		}
	}
}
