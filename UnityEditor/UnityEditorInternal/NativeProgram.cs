using System;
using System.Diagnostics;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class NativeProgram : Program
	{
		public NativeProgram(string executable, string arguments)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				Arguments = arguments,
				CreateNoWindow = true,
				FileName = executable,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				WorkingDirectory = Application.dataPath + "/..",
				UseShellExecute = false
			};
			this._process.StartInfo = startInfo;
		}
	}
}
