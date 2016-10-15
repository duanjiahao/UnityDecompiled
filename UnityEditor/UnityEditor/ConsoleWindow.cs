using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Console", useTypeNameAsIconName = true)]
	internal class ConsoleWindow : EditorWindow, IHasCustomMenu
	{
		internal class Constants
		{
			public static bool ms_Loaded;

			public static GUIStyle Box;

			public static GUIStyle Button;

			public static GUIStyle MiniButton;

			public static GUIStyle MiniButtonLeft;

			public static GUIStyle MiniButtonMiddle;

			public static GUIStyle MiniButtonRight;

			public static GUIStyle LogStyle;

			public static GUIStyle WarningStyle;

			public static GUIStyle ErrorStyle;

			public static GUIStyle EvenBackground;

			public static GUIStyle OddBackground;

			public static GUIStyle MessageStyle;

			public static GUIStyle StatusError;

			public static GUIStyle StatusWarn;

			public static GUIStyle StatusLog;

			public static GUIStyle Toolbar;

			public static GUIStyle CountBadge;

			public static void Init()
			{
				if (ConsoleWindow.Constants.ms_Loaded)
				{
					return;
				}
				ConsoleWindow.Constants.ms_Loaded = true;
				ConsoleWindow.Constants.Box = "CN Box";
				ConsoleWindow.Constants.Button = "Button";
				ConsoleWindow.Constants.MiniButton = "ToolbarButton";
				ConsoleWindow.Constants.MiniButtonLeft = "ToolbarButton";
				ConsoleWindow.Constants.MiniButtonMiddle = "ToolbarButton";
				ConsoleWindow.Constants.MiniButtonRight = "ToolbarButton";
				ConsoleWindow.Constants.Toolbar = "Toolbar";
				ConsoleWindow.Constants.LogStyle = "CN EntryInfo";
				ConsoleWindow.Constants.WarningStyle = "CN EntryWarn";
				ConsoleWindow.Constants.ErrorStyle = "CN EntryError";
				ConsoleWindow.Constants.EvenBackground = "CN EntryBackEven";
				ConsoleWindow.Constants.OddBackground = "CN EntryBackodd";
				ConsoleWindow.Constants.MessageStyle = "CN Message";
				ConsoleWindow.Constants.StatusError = "CN StatusError";
				ConsoleWindow.Constants.StatusWarn = "CN StatusWarn";
				ConsoleWindow.Constants.StatusLog = "CN StatusInfo";
				ConsoleWindow.Constants.CountBadge = "CN CountBadge";
			}
		}

		private enum Mode
		{
			Error = 1,
			Assert,
			Log = 4,
			Fatal = 16,
			DontPreprocessCondition = 32,
			AssetImportError = 64,
			AssetImportWarning = 128,
			ScriptingError = 256,
			ScriptingWarning = 512,
			ScriptingLog = 1024,
			ScriptCompileError = 2048,
			ScriptCompileWarning = 4096,
			StickyError = 8192,
			MayIgnoreLineNumber = 16384,
			ReportBug = 32768,
			DisplayPreviousErrorInStatusBar = 65536,
			ScriptingException = 131072,
			DontExtractStacktrace = 262144,
			ShouldClearOnPlay = 524288,
			GraphCompileError = 1048576,
			ScriptingAssertion = 2097152
		}

		private enum ConsoleFlags
		{
			Collapse = 1,
			ClearOnPlay,
			ErrorPause = 4,
			Verbose = 8,
			StopForAssert = 16,
			StopForError = 32,
			Autoscroll = 64,
			LogLevelLog = 128,
			LogLevelWarning = 256,
			LogLevelError = 512
		}

		public struct StackTraceLogTypeData
		{
			public LogType logType;

			public StackTraceLogType stackTraceLogType;
		}

		private const int m_RowHeight = 32;

		private ListViewState m_ListView;

		private string m_ActiveText = string.Empty;

		private int m_ActiveInstanceID;

		private bool m_DevBuild;

		private Vector2 m_TextScroll = Vector2.zero;

		private SplitterState spl = new SplitterState(new float[]
		{
			70f,
			30f
		}, new int[]
		{
			32,
			32
		}, null);

		private static bool ms_LoadedIcons;

		internal static Texture2D iconInfo;

		internal static Texture2D iconWarn;

		internal static Texture2D iconError;

		internal static Texture2D iconInfoSmall;

		internal static Texture2D iconWarnSmall;

		internal static Texture2D iconErrorSmall;

		internal static Texture2D iconInfoMono;

		internal static Texture2D iconWarnMono;

		internal static Texture2D iconErrorMono;

		private int ms_LVHeight;

		private static ConsoleWindow ms_ConsoleWindow;

		public ConsoleWindow()
		{
			base.position = new Rect(200f, 200f, 800f, 400f);
			this.m_ListView = new ListViewState(0, 32);
		}

		private static void ShowConsoleWindowImmediate()
		{
			ConsoleWindow.ShowConsoleWindow(true);
		}

		public static void ShowConsoleWindow(bool immediate)
		{
			if (ConsoleWindow.ms_ConsoleWindow == null)
			{
				ConsoleWindow.ms_ConsoleWindow = ScriptableObject.CreateInstance<ConsoleWindow>();
				ConsoleWindow.ms_ConsoleWindow.Show(immediate);
				ConsoleWindow.ms_ConsoleWindow.Focus();
			}
			else
			{
				ConsoleWindow.ms_ConsoleWindow.Show(immediate);
				ConsoleWindow.ms_ConsoleWindow.Focus();
			}
		}

		internal static void LoadIcons()
		{
			if (ConsoleWindow.ms_LoadedIcons)
			{
				return;
			}
			ConsoleWindow.ms_LoadedIcons = true;
			ConsoleWindow.iconInfo = EditorGUIUtility.LoadIcon("console.infoicon");
			ConsoleWindow.iconWarn = EditorGUIUtility.LoadIcon("console.warnicon");
			ConsoleWindow.iconError = EditorGUIUtility.LoadIcon("console.erroricon");
			ConsoleWindow.iconInfoSmall = EditorGUIUtility.LoadIcon("console.infoicon.sml");
			ConsoleWindow.iconWarnSmall = EditorGUIUtility.LoadIcon("console.warnicon.sml");
			ConsoleWindow.iconErrorSmall = EditorGUIUtility.LoadIcon("console.erroricon.sml");
			ConsoleWindow.iconInfoMono = EditorGUIUtility.LoadIcon("console.infoicon.sml");
			ConsoleWindow.iconWarnMono = EditorGUIUtility.LoadIcon("console.warnicon.inactive.sml");
			ConsoleWindow.iconErrorMono = EditorGUIUtility.LoadIcon("console.erroricon.inactive.sml");
			ConsoleWindow.Constants.Init();
		}

		[RequiredByNativeCode]
		public static void LogChanged()
		{
			if (ConsoleWindow.ms_ConsoleWindow == null)
			{
				return;
			}
			ConsoleWindow.ms_ConsoleWindow.DoLogChanged();
		}

		public void DoLogChanged()
		{
			ConsoleWindow.ms_ConsoleWindow.Repaint();
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			ConsoleWindow.ms_ConsoleWindow = this;
			this.m_DevBuild = Unsupported.IsDeveloperBuild();
		}

		private void OnDisable()
		{
			if (ConsoleWindow.ms_ConsoleWindow == this)
			{
				ConsoleWindow.ms_ConsoleWindow = null;
			}
		}

		private static bool HasMode(int mode, ConsoleWindow.Mode modeToCheck)
		{
			return (mode & (int)modeToCheck) != 0;
		}

		private bool HasFlag(ConsoleWindow.ConsoleFlags flags)
		{
			return (LogEntries.consoleFlags & (int)flags) != 0;
		}

		private void SetFlag(ConsoleWindow.ConsoleFlags flags, bool val)
		{
			LogEntries.SetConsoleFlag((int)flags, val);
		}

		internal static Texture2D GetIconForErrorMode(int mode, bool large)
		{
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)3148115))
			{
				return (!large) ? ConsoleWindow.iconErrorSmall : ConsoleWindow.iconError;
			}
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)4736))
			{
				return (!large) ? ConsoleWindow.iconWarnSmall : ConsoleWindow.iconWarn;
			}
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)1028))
			{
				return (!large) ? ConsoleWindow.iconInfoSmall : ConsoleWindow.iconInfo;
			}
			return null;
		}

		internal static GUIStyle GetStyleForErrorMode(int mode)
		{
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)3148115))
			{
				return ConsoleWindow.Constants.ErrorStyle;
			}
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)4736))
			{
				return ConsoleWindow.Constants.WarningStyle;
			}
			return ConsoleWindow.Constants.LogStyle;
		}

		internal static GUIStyle GetStatusStyleForErrorMode(int mode)
		{
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)3148115))
			{
				return ConsoleWindow.Constants.StatusError;
			}
			if (ConsoleWindow.HasMode(mode, (ConsoleWindow.Mode)4736))
			{
				return ConsoleWindow.Constants.StatusWarn;
			}
			return ConsoleWindow.Constants.StatusLog;
		}

		private static string ContextString(LogEntry entry)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ConsoleWindow.HasMode(entry.mode, ConsoleWindow.Mode.Error))
			{
				stringBuilder.Append("Error ");
			}
			else if (ConsoleWindow.HasMode(entry.mode, ConsoleWindow.Mode.Log))
			{
				stringBuilder.Append("Log ");
			}
			else
			{
				stringBuilder.Append("Assert ");
			}
			stringBuilder.Append("in file: ");
			stringBuilder.Append(entry.file);
			stringBuilder.Append(" at line: ");
			stringBuilder.Append(entry.line);
			if (entry.errorNum != 0)
			{
				stringBuilder.Append(" and errorNum: ");
				stringBuilder.Append(entry.errorNum);
			}
			return stringBuilder.ToString();
		}

		private static string GetFirstLine(string s)
		{
			int num = s.IndexOf("\n");
			return (num == -1) ? s : s.Substring(0, num);
		}

		private static string GetFirstTwoLines(string s)
		{
			int num = s.IndexOf("\n");
			if (num != -1)
			{
				num = s.IndexOf("\n", num + 1);
				if (num != -1)
				{
					return s.Substring(0, num);
				}
			}
			return s;
		}

		private void SetActiveEntry(LogEntry entry)
		{
			if (entry != null)
			{
				this.m_ActiveText = entry.condition;
				if (this.m_ActiveInstanceID != entry.instanceID)
				{
					this.m_ActiveInstanceID = entry.instanceID;
					if (entry.instanceID != 0)
					{
						EditorGUIUtility.PingObject(entry.instanceID);
					}
				}
			}
			else
			{
				this.m_ActiveText = string.Empty;
				this.m_ActiveInstanceID = 0;
				this.m_ListView.row = -1;
			}
		}

		private static void ShowConsoleRow(int row)
		{
			ConsoleWindow.ShowConsoleWindow(false);
			if (ConsoleWindow.ms_ConsoleWindow)
			{
				ConsoleWindow.ms_ConsoleWindow.m_ListView.row = row;
				ConsoleWindow.ms_ConsoleWindow.m_ListView.selectionChanged = true;
				ConsoleWindow.ms_ConsoleWindow.Repaint();
			}
		}

		private void OnGUI()
		{
			Event current = Event.current;
			ConsoleWindow.LoadIcons();
			GUILayout.BeginHorizontal(ConsoleWindow.Constants.Toolbar, new GUILayoutOption[0]);
			if (GUILayout.Button("Clear", ConsoleWindow.Constants.MiniButton, new GUILayoutOption[0]))
			{
				LogEntries.Clear();
				GUIUtility.keyboardControl = 0;
			}
			int count = LogEntries.GetCount();
			if (this.m_ListView.totalRows != count && this.m_ListView.scrollPos.y >= (float)(this.m_ListView.rowHeight * this.m_ListView.totalRows - this.ms_LVHeight))
			{
				this.m_ListView.scrollPos.y = (float)(count * 32 - this.ms_LVHeight);
			}
			EditorGUILayout.Space();
			bool flag = this.HasFlag(ConsoleWindow.ConsoleFlags.Collapse);
			this.SetFlag(ConsoleWindow.ConsoleFlags.Collapse, GUILayout.Toggle(flag, "Collapse", ConsoleWindow.Constants.MiniButtonLeft, new GUILayoutOption[0]));
			bool flag2 = flag != this.HasFlag(ConsoleWindow.ConsoleFlags.Collapse);
			if (flag2)
			{
				this.m_ListView.row = -1;
				this.m_ListView.scrollPos.y = (float)(LogEntries.GetCount() * 32);
			}
			this.SetFlag(ConsoleWindow.ConsoleFlags.ClearOnPlay, GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.ClearOnPlay), "Clear on Play", ConsoleWindow.Constants.MiniButtonMiddle, new GUILayoutOption[0]));
			this.SetFlag(ConsoleWindow.ConsoleFlags.ErrorPause, GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.ErrorPause), "Error Pause", ConsoleWindow.Constants.MiniButtonRight, new GUILayoutOption[0]));
			EditorGUILayout.Space();
			if (this.m_DevBuild)
			{
				GUILayout.FlexibleSpace();
				this.SetFlag(ConsoleWindow.ConsoleFlags.StopForAssert, GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.StopForAssert), "Stop for Assert", ConsoleWindow.Constants.MiniButtonLeft, new GUILayoutOption[0]));
				this.SetFlag(ConsoleWindow.ConsoleFlags.StopForError, GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.StopForError), "Stop for Error", ConsoleWindow.Constants.MiniButtonRight, new GUILayoutOption[0]));
			}
			GUILayout.FlexibleSpace();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			LogEntries.GetCountsByType(ref num, ref num2, ref num3);
			bool val = GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.LogLevelLog), new GUIContent((num3 > 999) ? "999+" : num3.ToString(), (num3 <= 0) ? ConsoleWindow.iconInfoMono : ConsoleWindow.iconInfoSmall), ConsoleWindow.Constants.MiniButtonRight, new GUILayoutOption[0]);
			bool val2 = GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.LogLevelWarning), new GUIContent((num2 > 999) ? "999+" : num2.ToString(), (num2 <= 0) ? ConsoleWindow.iconWarnMono : ConsoleWindow.iconWarnSmall), ConsoleWindow.Constants.MiniButtonMiddle, new GUILayoutOption[0]);
			bool val3 = GUILayout.Toggle(this.HasFlag(ConsoleWindow.ConsoleFlags.LogLevelError), new GUIContent((num > 999) ? "999+" : num.ToString(), (num <= 0) ? ConsoleWindow.iconErrorMono : ConsoleWindow.iconErrorSmall), ConsoleWindow.Constants.MiniButtonLeft, new GUILayoutOption[0]);
			this.SetFlag(ConsoleWindow.ConsoleFlags.LogLevelLog, val);
			this.SetFlag(ConsoleWindow.ConsoleFlags.LogLevelWarning, val2);
			this.SetFlag(ConsoleWindow.ConsoleFlags.LogLevelError, val3);
			GUILayout.EndHorizontal();
			this.m_ListView.totalRows = LogEntries.StartGettingEntries();
			SplitterGUILayout.BeginVerticalSplit(this.spl, new GUILayoutOption[0]);
			EditorGUIUtility.SetIconSize(new Vector2(32f, 32f));
			GUIContent gUIContent = new GUIContent();
			int controlID = GUIUtility.GetControlID(FocusType.Native);
			try
			{
				bool flag3 = false;
				bool flag4 = this.HasFlag(ConsoleWindow.ConsoleFlags.Collapse);
				foreach (ListViewElement listViewElement in ListViewGUI.ListView(this.m_ListView, ConsoleWindow.Constants.Box, new GUILayoutOption[0]))
				{
					if (current.type == EventType.MouseDown && current.button == 0 && listViewElement.position.Contains(current.mousePosition))
					{
						if (current.clickCount == 2)
						{
							LogEntries.RowGotDoubleClicked(this.m_ListView.row);
						}
						flag3 = true;
					}
					if (current.type == EventType.Repaint)
					{
						int mode = 0;
						string text = null;
						LogEntries.GetFirstTwoLinesEntryTextAndModeInternal(listViewElement.row, ref mode, ref text);
						GUIStyle gUIStyle = (listViewElement.row % 2 != 0) ? ConsoleWindow.Constants.EvenBackground : ConsoleWindow.Constants.OddBackground;
						gUIStyle.Draw(listViewElement.position, false, false, this.m_ListView.row == listViewElement.row, false);
						gUIContent.text = text;
						GUIStyle styleForErrorMode = ConsoleWindow.GetStyleForErrorMode(mode);
						styleForErrorMode.Draw(listViewElement.position, gUIContent, controlID, this.m_ListView.row == listViewElement.row);
						if (flag4)
						{
							Rect position = listViewElement.position;
							gUIContent.text = LogEntries.GetEntryCount(listViewElement.row).ToString(CultureInfo.InvariantCulture);
							Vector2 vector = ConsoleWindow.Constants.CountBadge.CalcSize(gUIContent);
							position.xMin = position.xMax - vector.x;
							position.yMin += (position.yMax - position.yMin - vector.y) * 0.5f;
							position.x -= 5f;
							GUI.Label(position, gUIContent, ConsoleWindow.Constants.CountBadge);
						}
					}
				}
				if (flag3 && this.m_ListView.scrollPos.y >= (float)(this.m_ListView.rowHeight * this.m_ListView.totalRows - this.ms_LVHeight))
				{
					this.m_ListView.scrollPos.y = (float)(this.m_ListView.rowHeight * this.m_ListView.totalRows - this.ms_LVHeight - 1);
				}
				if (this.m_ListView.totalRows == 0 || this.m_ListView.row >= this.m_ListView.totalRows || this.m_ListView.row < 0)
				{
					if (this.m_ActiveText.Length != 0)
					{
						this.SetActiveEntry(null);
					}
				}
				else
				{
					LogEntry logEntry = new LogEntry();
					LogEntries.GetEntryInternal(this.m_ListView.row, logEntry);
					this.SetActiveEntry(logEntry);
					LogEntries.GetEntryInternal(this.m_ListView.row, logEntry);
					if (this.m_ListView.selectionChanged || !this.m_ActiveText.Equals(logEntry.condition))
					{
						this.SetActiveEntry(logEntry);
					}
				}
				if (GUIUtility.keyboardControl == this.m_ListView.ID && current.type == EventType.KeyDown && current.keyCode == KeyCode.Return && this.m_ListView.row != 0)
				{
					LogEntries.RowGotDoubleClicked(this.m_ListView.row);
					Event.current.Use();
				}
				if (current.type != EventType.Layout && ListViewGUI.ilvState.rectHeight != 1)
				{
					this.ms_LVHeight = ListViewGUI.ilvState.rectHeight;
				}
			}
			finally
			{
				LogEntries.EndGettingEntries();
				EditorGUIUtility.SetIconSize(Vector2.zero);
			}
			this.m_TextScroll = GUILayout.BeginScrollView(this.m_TextScroll, ConsoleWindow.Constants.Box);
			float minHeight = ConsoleWindow.Constants.MessageStyle.CalcHeight(GUIContent.Temp(this.m_ActiveText), base.position.width);
			EditorGUILayout.SelectableLabel(this.m_ActiveText, ConsoleWindow.Constants.MessageStyle, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true),
				GUILayout.MinHeight(minHeight)
			});
			GUILayout.EndScrollView();
			SplitterGUILayout.EndVerticalSplit();
			if ((current.type == EventType.ValidateCommand || current.type == EventType.ExecuteCommand) && current.commandName == "Copy" && this.m_ActiveText != string.Empty)
			{
				if (current.type == EventType.ExecuteCommand)
				{
					EditorGUIUtility.systemCopyBuffer = this.m_ActiveText;
				}
				current.Use();
			}
		}

		public void ToggleLogStackTraces(object userData)
		{
			ConsoleWindow.StackTraceLogTypeData stackTraceLogTypeData = (ConsoleWindow.StackTraceLogTypeData)userData;
			PlayerSettings.SetStackTraceLogType(stackTraceLogTypeData.logType, stackTraceLogTypeData.stackTraceLogType);
		}

		public void ToggleLogStackTracesForAll(object userData)
		{
			using (IEnumerator enumerator = Enum.GetValues(typeof(LogType)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LogType logType = (LogType)((int)enumerator.Current);
					PlayerSettings.SetStackTraceLogType(logType, (StackTraceLogType)((int)userData));
				}
			}
		}

		public void AddItemsToMenu(GenericMenu menu)
		{
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				menu.AddItem(new GUIContent("Open Player Log"), false, new GenericMenu.MenuFunction(InternalEditorUtility.OpenPlayerConsole));
			}
			menu.AddItem(new GUIContent("Open Editor Log"), false, new GenericMenu.MenuFunction(InternalEditorUtility.OpenEditorConsole));
			this.AddStackTraceLoggingMenu(menu);
		}

		private void AddStackTraceLoggingMenu(GenericMenu menu)
		{
			using (IEnumerator enumerator = Enum.GetValues(typeof(LogType)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					LogType logType = (LogType)((int)enumerator.Current);
					using (IEnumerator enumerator2 = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							StackTraceLogType stackTraceLogType = (StackTraceLogType)((int)enumerator2.Current);
							ConsoleWindow.StackTraceLogTypeData stackTraceLogTypeData;
							stackTraceLogTypeData.logType = logType;
							stackTraceLogTypeData.stackTraceLogType = stackTraceLogType;
							menu.AddItem(new GUIContent(string.Concat(new object[]
							{
								"Stack Trace Logging/",
								logType,
								"/",
								stackTraceLogType
							})), PlayerSettings.GetStackTraceLogType(logType) == stackTraceLogType, new GenericMenu.MenuFunction2(this.ToggleLogStackTraces), stackTraceLogTypeData);
						}
					}
				}
			}
			int num = (int)PlayerSettings.GetStackTraceLogType(LogType.Log);
			using (IEnumerator enumerator3 = Enum.GetValues(typeof(LogType)).GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					LogType logType2 = (LogType)((int)enumerator3.Current);
					if (PlayerSettings.GetStackTraceLogType(logType2) != (StackTraceLogType)num)
					{
						num = -1;
						break;
					}
				}
			}
			using (IEnumerator enumerator4 = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					StackTraceLogType stackTraceLogType2 = (StackTraceLogType)((int)enumerator4.Current);
					menu.AddItem(new GUIContent("Stack Trace Logging/All/" + stackTraceLogType2), num == (int)stackTraceLogType2, new GenericMenu.MenuFunction2(this.ToggleLogStackTracesForAll), stackTraceLogType2);
				}
			}
		}
	}
}
