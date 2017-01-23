using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	internal sealed class LogEntries
	{
		public static extern int consoleFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RowGotDoubleClicked(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetStatusText();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetStatusMask();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StartGettingEntries();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetConsoleFlag(int bit, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndGettingEntries();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetCountsByType(ref int errorCount, ref int warningCount, ref int logCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GetFirstTwoLinesEntryTextAndModeInternal(int row, ref int mask, ref string outString);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetEntryInternal(int row, LogEntry outputEntry);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetEntryCount(int row);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Clear();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetStatusViewErrorIndex();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClickStatusBar(int count);
	}
}
