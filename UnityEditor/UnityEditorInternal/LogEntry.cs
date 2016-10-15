using System;
using System.Runtime.InteropServices;

namespace UnityEditorInternal
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class LogEntry
	{
		public string condition;

		public int errorNum;

		public string file;

		public int line;

		public int mode;

		public int instanceID;

		public int identifier;

		public int isWorldPlaying;
	}
}
