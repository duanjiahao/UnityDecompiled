using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UnityEditor.Utils
{
	internal class ProcessOutputStreamReader
	{
		private readonly Func<bool> hostProcessExited;

		private readonly StreamReader stream;

		internal List<string> lines;

		private Thread thread;

		internal ProcessOutputStreamReader(Process p, StreamReader stream) : this(() => p.HasExited, stream)
		{
		}

		internal ProcessOutputStreamReader(Func<bool> hostProcessExited, StreamReader stream)
		{
			this.hostProcessExited = hostProcessExited;
			this.stream = stream;
			this.lines = new List<string>();
			this.thread = new Thread(new ThreadStart(this.ThreadFunc));
			this.thread.Start();
		}

		private void ThreadFunc()
		{
			if (this.hostProcessExited())
			{
				return;
			}
			while (this.stream.BaseStream != null)
			{
				string text = this.stream.ReadLine();
				if (text == null)
				{
					return;
				}
				List<string> obj = this.lines;
				lock (obj)
				{
					this.lines.Add(text);
				}
			}
		}

		internal string[] GetOutput()
		{
			if (this.hostProcessExited())
			{
				this.thread.Join();
			}
			List<string> obj = this.lines;
			string[] result;
			lock (obj)
			{
				result = this.lines.ToArray();
			}
			return result;
		}
	}
}
