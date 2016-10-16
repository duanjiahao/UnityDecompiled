using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Profiler", useTypeNameAsIconName = true)]
	internal class ProfilerWindow : EditorWindow, IProfilerWindowController
	{
		internal class Styles
		{
			public GUIContent addArea = EditorGUIUtility.TextContent("Add Profiler|Add a profiler area");

			public GUIContent deepProfile = EditorGUIUtility.TextContent("Deep Profile|Instrument all mono calls to investigate scripts");

			public GUIContent profileEditor = EditorGUIUtility.TextContent("Profile Editor|Enable profiling of the editor");

			public GUIContent noData = EditorGUIUtility.TextContent("No frame data available");

			public GUIContent frameDebugger = EditorGUIUtility.TextContent("Frame Debugger|Open Frame Debugger");

			public GUIContent noFrameDebugger = EditorGUIUtility.TextContent("Frame Debugger|Open Frame Debugger (Current frame needs to be selected)");

			public GUIContent gatherObjectReferences = EditorGUIUtility.TextContent("Gather object references|Collect reference information to see where objects are referenced from. Disable this to save memory");

			public GUIContent profilerRecord = EditorGUIUtility.TextContentWithIcon("Record|Record profiling information", "Profiler.Record");

			public GUIContent profilerInstrumentation = EditorGUIUtility.TextContent("Instrumentation|Add Profiler Instrumentation to selected functions");

			public GUIContent prevFrame = EditorGUIUtility.IconContent("Profiler.PrevFrame", "|Go back one frame");

			public GUIContent nextFrame = EditorGUIUtility.IconContent("Profiler.NextFrame", "|Go one frame forwards");

			public GUIContent currentFrame = EditorGUIUtility.TextContent("Current|Go to current frame");

			public GUIContent frame = EditorGUIUtility.TextContent("Frame: ");

			public GUIContent clearData = EditorGUIUtility.TextContent("Clear");

			public GUIContent[] reasons = ProfilerWindow.Styles.GetLocalizedReasons();

			public GUIStyle background = "OL Box";

			public GUIStyle header = "OL title";

			public GUIStyle label = "OL label";

			public GUIStyle entryEven = "OL EntryBackEven";

			public GUIStyle entryOdd = "OL EntryBackOdd";

			public GUIStyle foldout = "IN foldout";

			public GUIStyle profilerGraphBackground = "ProfilerScrollviewBackground";

			public Styles()
			{
				this.profilerGraphBackground.overflow.left = -170;
			}

			internal static GUIContent[] GetLocalizedReasons()
			{
				return new GUIContent[]
				{
					EditorGUIUtility.TextContent("Scene object (Unloaded by loading a new scene or destroying it)"),
					EditorGUIUtility.TextContent("Builtin Resource (Never unloaded)"),
					EditorGUIUtility.TextContent("Object is marked Don't Save. (Must be explicitly destroyed or it will leak)"),
					EditorGUIUtility.TextContent("Asset is dirty and must be saved first (Editor only)"),
					null,
					EditorGUIUtility.TextContent("Asset type created from code or stored in the scene, referenced from native code."),
					EditorGUIUtility.TextContent("Asset type created from code or stored in the scene, referenced from scripts and native code."),
					null,
					EditorGUIUtility.TextContent("Asset referenced from native code."),
					EditorGUIUtility.TextContent("Asset referenced from scripts and native code."),
					EditorGUIUtility.TextContent("Not Applicable")
				};
			}
		}

		private const float kRowHeight = 16f;

		private const float kIndentPx = 16f;

		private const float kBaseIndent = 8f;

		private const float kSmallMargin = 4f;

		private const float kNameColumnSize = 350f;

		private const float kColumnSize = 80f;

		private const float kFoldoutSize = 14f;

		private const int kFirst = -999999;

		private const int kLast = 999999;

		private const string kProfilerColumnSettings = "VisibleProfilerColumnsV2";

		private const string kProfilerDetailColumnSettings = "VisibleProfilerDetailColumns";

		private const string kProfilerGPUColumnSettings = "VisibleProfilerGPUColumns";

		private const string kProfilerGPUDetailColumnSettings = "VisibleProfilerGPUDetailColumns";

		private const string kProfilerVisibleGraphsSettings = "VisibleProfilerGraphs";

		private const string kProfilerEnabledSessionKey = "ProfilerEnabled";

		private const string kSearchControlName = "ProfilerSearchField";

		private static ProfilerWindow.Styles ms_Styles;

		private bool m_FocusSearchField;

		private string m_SearchString = string.Empty;

		private SplitterState m_VertSplit = new SplitterState(new float[]
		{
			50f,
			50f
		}, new int[]
		{
			50,
			50
		}, null);

		private SplitterState m_ViewSplit = new SplitterState(new float[]
		{
			70f,
			30f
		}, new int[]
		{
			450,
			50
		}, null);

		private SplitterState m_NetworkSplit = new SplitterState(new float[]
		{
			20f,
			80f
		}, new int[]
		{
			100,
			100
		}, null);

		[SerializeField]
		private bool m_Recording;

		private AttachProfilerUI m_AttachProfilerUI = new AttachProfilerUI();

		private Vector2 m_GraphPos = Vector2.zero;

		private Vector2[] m_PaneScroll = new Vector2[9];

		private string m_ActiveNativePlatformSupportModule;

		private static List<ProfilerWindow> m_ProfilerWindows = new List<ProfilerWindow>();

		private ProfilerViewType m_ViewType;

		private ProfilerArea m_CurrentArea;

		private ProfilerMemoryView m_ShowDetailedMemoryPane;

		private ProfilerAudioView m_ShowDetailedAudioPane;

		private int m_CurrentFrame = -1;

		private int m_LastFrameFromTick = -1;

		private int m_PrevLastFrame = -1;

		private int m_LastAudioProfilerFrame = -1;

		private ProfilerChart[] m_Charts;

		private float[] m_ChartOldMax = new float[]
		{
			-1f,
			-1f
		};

		private float m_ChartMaxClamp = 70000f;

		private ProfilerHierarchyGUI m_CPUHierarchyGUI;

		private ProfilerHierarchyGUI m_GPUHierarchyGUI;

		private ProfilerHierarchyGUI m_CPUDetailHierarchyGUI;

		private ProfilerHierarchyGUI m_GPUDetailHierarchyGUI;

		private ProfilerTimelineGUI m_CPUTimelineGUI;

		private MemoryTreeList m_ReferenceListView;

		private MemoryTreeListClickable m_MemoryListView;

		private bool m_GatherObjectReferences = true;

		[SerializeField]
		private AudioProfilerTreeViewState m_AudioProfilerTreeViewState;

		private AudioProfilerView m_AudioProfilerView;

		private AudioProfilerBackend m_AudioProfilerBackend;

		private static readonly int s_HashControlID = "ProfilerSearchField".GetHashCode();

		private string[] msgNames = new string[]
		{
			"UserMessage",
			"ObjectDestroy",
			"ClientRpc",
			"ObjectSpawn",
			"Owner",
			"Command",
			"LocalPlayerTransform",
			"SyncEvent",
			"SyncVars",
			"SyncList",
			"ObjectSpawnScene",
			"NetworkInfo",
			"SpawnFinished",
			"ObjectHide",
			"CRC",
			"ClientAuthority"
		};

		private bool[] msgFoldouts = new bool[]
		{
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true,
			true
		};

		private bool wantsMemoryRefresh
		{
			get
			{
				return this.m_MemoryListView.GetRoot() == null;
			}
		}

		private void BuildColumns()
		{
			ProfilerColumn[] array = new ProfilerColumn[]
			{
				ProfilerColumn.FunctionName,
				ProfilerColumn.TotalPercent,
				ProfilerColumn.SelfPercent,
				ProfilerColumn.Calls,
				ProfilerColumn.GCMemory,
				ProfilerColumn.TotalTime,
				ProfilerColumn.SelfTime,
				ProfilerColumn.WarningCount
			};
			ProfilerColumn[] array2 = new ProfilerColumn[]
			{
				ProfilerColumn.ObjectName,
				ProfilerColumn.TotalPercent,
				ProfilerColumn.SelfPercent,
				ProfilerColumn.Calls,
				ProfilerColumn.GCMemory,
				ProfilerColumn.TotalTime,
				ProfilerColumn.SelfTime
			};
			this.m_CPUHierarchyGUI = new ProfilerHierarchyGUI(this, "VisibleProfilerColumnsV2", array, ProfilerWindow.ProfilerColumnNames(array), false, ProfilerColumn.TotalTime);
			this.m_CPUTimelineGUI = new ProfilerTimelineGUI(this);
			string text = EditorGUIUtility.TextContent("Object").text;
			string[] array3 = ProfilerWindow.ProfilerColumnNames(array2);
			array3[0] = text;
			this.m_CPUDetailHierarchyGUI = new ProfilerHierarchyGUI(this, "VisibleProfilerDetailColumns", array2, array3, true, ProfilerColumn.TotalTime);
			ProfilerColumn[] array4 = new ProfilerColumn[]
			{
				ProfilerColumn.FunctionName,
				ProfilerColumn.TotalGPUPercent,
				ProfilerColumn.DrawCalls,
				ProfilerColumn.TotalGPUTime
			};
			ProfilerColumn[] array5 = new ProfilerColumn[]
			{
				ProfilerColumn.ObjectName,
				ProfilerColumn.TotalGPUPercent,
				ProfilerColumn.DrawCalls,
				ProfilerColumn.TotalGPUTime
			};
			this.m_GPUHierarchyGUI = new ProfilerHierarchyGUI(this, "VisibleProfilerGPUColumns", array4, ProfilerWindow.ProfilerColumnNames(array4), false, ProfilerColumn.TotalGPUTime);
			array3 = ProfilerWindow.ProfilerColumnNames(array5);
			array3[0] = text;
			this.m_GPUDetailHierarchyGUI = new ProfilerHierarchyGUI(this, "VisibleProfilerGPUDetailColumns", array5, array3, true, ProfilerColumn.TotalGPUTime);
		}

		private static string[] ProfilerColumnNames(ProfilerColumn[] columns)
		{
			string[] names = Enum.GetNames(typeof(ProfilerColumn));
			string[] array = new string[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				switch (columns[i])
				{
				case ProfilerColumn.FunctionName:
					array[i] = LocalizationDatabase.GetLocalizedString("Overview");
					break;
				case ProfilerColumn.TotalPercent:
					array[i] = LocalizationDatabase.GetLocalizedString("Total");
					break;
				case ProfilerColumn.SelfPercent:
					array[i] = LocalizationDatabase.GetLocalizedString("Self");
					break;
				case ProfilerColumn.Calls:
					array[i] = LocalizationDatabase.GetLocalizedString("Calls");
					break;
				case ProfilerColumn.GCMemory:
					array[i] = LocalizationDatabase.GetLocalizedString("GC Alloc");
					break;
				case ProfilerColumn.TotalTime:
					array[i] = LocalizationDatabase.GetLocalizedString("Time ms");
					break;
				case ProfilerColumn.SelfTime:
					array[i] = LocalizationDatabase.GetLocalizedString("Self ms");
					break;
				case ProfilerColumn.DrawCalls:
					array[i] = LocalizationDatabase.GetLocalizedString("DrawCalls");
					break;
				case ProfilerColumn.TotalGPUTime:
					array[i] = LocalizationDatabase.GetLocalizedString("GPU ms");
					break;
				case ProfilerColumn.SelfGPUTime:
					array[i] = LocalizationDatabase.GetLocalizedString("Self ms");
					break;
				case ProfilerColumn.TotalGPUPercent:
					array[i] = LocalizationDatabase.GetLocalizedString("Total");
					break;
				case ProfilerColumn.SelfGPUPercent:
					array[i] = LocalizationDatabase.GetLocalizedString("Self");
					break;
				case ProfilerColumn.WarningCount:
					array[i] = LocalizationDatabase.GetLocalizedString("|Warnings");
					break;
				case ProfilerColumn.ObjectName:
					array[i] = LocalizationDatabase.GetLocalizedString("Name");
					break;
				default:
					array[i] = "ProfilerColumn." + names[(int)columns[i]];
					break;
				}
			}
			return array;
		}

		public void SetSelectedPropertyPath(string path)
		{
			if (ProfilerDriver.selectedPropertyPath != path)
			{
				ProfilerDriver.selectedPropertyPath = path;
				this.UpdateCharts();
			}
		}

		public void ClearSelectedPropertyPath()
		{
			if (ProfilerDriver.selectedPropertyPath != string.Empty)
			{
				this.m_CPUHierarchyGUI.selectedIndex = -1;
				ProfilerDriver.selectedPropertyPath = string.Empty;
				this.UpdateCharts();
			}
		}

		public ProfilerProperty CreateProperty(bool details)
		{
			ProfilerProperty profilerProperty = new ProfilerProperty();
			ProfilerColumn profilerSortColumn = (this.m_CurrentArea != ProfilerArea.CPU) ? ((!details) ? this.m_GPUHierarchyGUI.sortType : this.m_GPUDetailHierarchyGUI.sortType) : ((!details) ? this.m_CPUHierarchyGUI.sortType : this.m_CPUDetailHierarchyGUI.sortType);
			profilerProperty.SetRoot(this.GetActiveVisibleFrameIndex(), profilerSortColumn, this.m_ViewType);
			profilerProperty.onlyShowGPUSamples = (this.m_CurrentArea == ProfilerArea.GPU);
			return profilerProperty;
		}

		public int GetActiveVisibleFrameIndex()
		{
			return (this.m_CurrentFrame != -1) ? this.m_CurrentFrame : this.m_LastFrameFromTick;
		}

		public void SetSearch(string searchString)
		{
			this.m_SearchString = ((!string.IsNullOrEmpty(searchString)) ? searchString : string.Empty);
		}

		public string GetSearch()
		{
			return this.m_SearchString;
		}

		public bool IsSearching()
		{
			return !string.IsNullOrEmpty(this.m_SearchString) && this.m_SearchString.Length > 0;
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			ProfilerWindow.m_ProfilerWindows.Add(this);
			this.Initialize();
		}

		private void Initialize()
		{
			int len = ProfilerDriver.maxHistoryLength - 1;
			this.m_Charts = new ProfilerChart[9];
			Color[] colors = ProfilerColors.colors;
			for (ProfilerArea profilerArea = ProfilerArea.CPU; profilerArea < ProfilerArea.AreaCount; profilerArea++)
			{
				float dataScale = 1f;
				Chart.ChartType type = Chart.ChartType.Line;
				string[] graphStatisticsPropertiesForArea = ProfilerDriver.GetGraphStatisticsPropertiesForArea(profilerArea);
				int num = graphStatisticsPropertiesForArea.Length;
				if (profilerArea == ProfilerArea.GPU || profilerArea == ProfilerArea.CPU)
				{
					type = Chart.ChartType.StackedFill;
					dataScale = 0.001f;
				}
				ProfilerChart profilerChart = new ProfilerChart(profilerArea, type, dataScale, num);
				for (int i = 0; i < num; i++)
				{
					profilerChart.m_Series[i] = new ChartSeries(graphStatisticsPropertiesForArea[i], len, colors[i]);
				}
				this.m_Charts[(int)profilerArea] = profilerChart;
			}
			if (this.m_ReferenceListView == null)
			{
				this.m_ReferenceListView = new MemoryTreeList(this, null);
			}
			if (this.m_MemoryListView == null)
			{
				this.m_MemoryListView = new MemoryTreeListClickable(this, this.m_ReferenceListView);
			}
			this.UpdateCharts();
			this.BuildColumns();
			ProfilerChart[] charts = this.m_Charts;
			for (int j = 0; j < charts.Length; j++)
			{
				ProfilerChart profilerChart2 = charts[j];
				profilerChart2.LoadAndBindSettings();
			}
		}

		private void CheckForPlatformModuleChange()
		{
			if (this.m_ActiveNativePlatformSupportModule != EditorUtility.GetActiveNativePlatformSupportModuleName())
			{
				ProfilerDriver.ResetHistory();
				this.Initialize();
				this.m_ActiveNativePlatformSupportModule = EditorUtility.GetActiveNativePlatformSupportModuleName();
			}
		}

		private void OnDisable()
		{
			ProfilerWindow.m_ProfilerWindows.Remove(this);
		}

		private void Awake()
		{
			if (!Profiler.supported)
			{
				return;
			}
			this.m_Recording = SessionState.GetBool("ProfilerEnabled", true);
			Profiler.enabled = this.m_Recording;
		}

		private void OnDestroy()
		{
			if (Profiler.supported)
			{
				Profiler.enabled = false;
			}
		}

		private void OnFocus()
		{
			if (Profiler.supported)
			{
				Profiler.enabled = this.m_Recording;
			}
		}

		private void OnLostFocus()
		{
		}

		private static void ShowProfilerWindow()
		{
			EditorWindow.GetWindow<ProfilerWindow>(false);
		}

		[RequiredByNativeCode]
		private static void RepaintAllProfilerWindows()
		{
			foreach (ProfilerWindow current in ProfilerWindow.m_ProfilerWindows)
			{
				if (ProfilerDriver.lastFrameIndex != current.m_LastFrameFromTick)
				{
					current.m_LastFrameFromTick = ProfilerDriver.lastFrameIndex;
					current.RepaintImmediately();
				}
			}
		}

		private static void SetMemoryProfilerInfo(ObjectMemoryInfo[] memoryInfo, int[] referencedIndices)
		{
			foreach (ProfilerWindow current in ProfilerWindow.m_ProfilerWindows)
			{
				if (current.wantsMemoryRefresh)
				{
					current.m_MemoryListView.SetRoot(MemoryElementDataManager.GetTreeRoot(memoryInfo, referencedIndices));
				}
			}
		}

		private static void SetProfileDeepScripts(bool deep)
		{
			bool deepProfiling = ProfilerDriver.deepProfiling;
			if (deepProfiling == deep)
			{
				return;
			}
			bool flag = true;
			if (EditorApplication.isPlaying)
			{
				if (deep)
				{
					flag = EditorUtility.DisplayDialog("Enable deep script profiling", "Enabling deep profiling requires reloading scripts.", "Reload", "Cancel");
				}
				else
				{
					flag = EditorUtility.DisplayDialog("Disable deep script profiling", "Disabling deep profiling requires reloading all scripts", "Reload", "Cancel");
				}
			}
			if (flag)
			{
				ProfilerDriver.deepProfiling = deep;
				InternalEditorUtility.RequestScriptReload();
			}
		}

		private string PickFrameLabel()
		{
			if (this.m_CurrentFrame == -1)
			{
				return "Current";
			}
			return this.m_CurrentFrame + 1 + " / " + (ProfilerDriver.lastFrameIndex + 1);
		}

		private void PrevFrame()
		{
			int previousFrameIndex = ProfilerDriver.GetPreviousFrameIndex(this.m_CurrentFrame);
			if (previousFrameIndex != -1)
			{
				this.SetCurrentFrame(previousFrameIndex);
			}
		}

		private void NextFrame()
		{
			int nextFrameIndex = ProfilerDriver.GetNextFrameIndex(this.m_CurrentFrame);
			if (nextFrameIndex != -1)
			{
				this.SetCurrentFrame(nextFrameIndex);
			}
		}

		private static void DrawEmptyCPUOrRenderingDetailPane()
		{
			GUILayout.Box(string.Empty, ProfilerWindow.ms_Styles.header, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Select Line for per-object breakdown", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void DrawCPUOrRenderingToolbar(ProfilerProperty property)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			string[] displayedOptions = new string[]
			{
				"Hierarchy",
				"Timeline",
				"Raw Hierarchy"
			};
			int[] optionValues = new int[]
			{
				0,
				1,
				2
			};
			this.m_ViewType = (ProfilerViewType)EditorGUILayout.IntPopup((int)this.m_ViewType, displayedOptions, optionValues, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("CPU:{0}ms   GPU:{1}ms", property.frameTime, property.frameGpuTime), EditorStyles.miniLabel, new GUILayoutOption[0]);
			GUI.enabled = (ProfilerDriver.GetNextFrameIndex(this.m_CurrentFrame) == -1);
			if (GUILayout.Button((!GUI.enabled) ? ProfilerWindow.ms_Styles.noFrameDebugger : ProfilerWindow.ms_Styles.frameDebugger, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				FrameDebuggerWindow frameDebuggerWindow = FrameDebuggerWindow.ShowFrameDebuggerWindow();
				frameDebuggerWindow.EnableIfNeeded();
			}
			GUI.enabled = true;
			if (ProfilerInstrumentationPopup.InstrumentationEnabled && GUILayout.Button(ProfilerWindow.ms_Styles.profilerInstrumentation, EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				ProfilerInstrumentationPopup.Show(last);
			}
			GUILayout.FlexibleSpace();
			this.SearchFieldGUI();
			EditorGUILayout.EndHorizontal();
			this.HandleCommandEvents();
		}

		private void HandleCommandEvents()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
			{
				bool flag = type == EventType.ExecuteCommand;
				if (Event.current.commandName == "Find")
				{
					if (flag)
					{
						this.m_FocusSearchField = true;
					}
					current.Use();
				}
			}
		}

		internal void SearchFieldGUI()
		{
			Event current = Event.current;
			Rect rect = GUILayoutUtility.GetRect(50f, 300f, 16f, 16f, EditorStyles.toolbarSearchField);
			if (this.m_ViewType == ProfilerViewType.Timeline)
			{
				return;
			}
			GUI.SetNextControlName("ProfilerSearchField");
			if (this.m_FocusSearchField)
			{
				EditorGUI.FocusTextInControl("ProfilerSearchField");
				if (Event.current.type == EventType.Repaint)
				{
					this.m_FocusSearchField = false;
				}
			}
			if (current.type == EventType.KeyDown && current.keyCode == KeyCode.Escape && GUI.GetNameOfFocusedControl() == "ProfilerSearchField")
			{
				this.m_SearchString = string.Empty;
			}
			if (current.type == EventType.KeyDown && (current.keyCode == KeyCode.DownArrow || current.keyCode == KeyCode.UpArrow) && GUI.GetNameOfFocusedControl() == "ProfilerSearchField")
			{
				this.m_CPUHierarchyGUI.SelectFirstRow();
				this.m_CPUHierarchyGUI.SetKeyboardFocus();
				base.Repaint();
				current.Use();
			}
			bool flag = this.m_CPUHierarchyGUI.selectedIndex != -1;
			EditorGUI.BeginChangeCheck();
			int controlID = GUIUtility.GetControlID(ProfilerWindow.s_HashControlID, FocusType.Keyboard, base.position);
			this.m_SearchString = EditorGUI.ToolbarSearchField(controlID, rect, this.m_SearchString, false);
			if (EditorGUI.EndChangeCheck() && !this.IsSearching() && GUIUtility.keyboardControl == 0 && flag)
			{
				this.m_CPUHierarchyGUI.FrameSelection();
			}
		}

		private static bool CheckFrameData(ProfilerProperty property)
		{
			if (!property.frameDataReady)
			{
				GUILayout.Label(ProfilerWindow.ms_Styles.noData, ProfilerWindow.ms_Styles.background, new GUILayoutOption[0]);
				return false;
			}
			return true;
		}

		private void DrawCPUOrRenderingPane(ProfilerHierarchyGUI mainPane, ProfilerHierarchyGUI detailPane, ProfilerTimelineGUI timelinePane)
		{
			ProfilerProperty profilerProperty = this.CreateProperty(false);
			this.DrawCPUOrRenderingToolbar(profilerProperty);
			if (!ProfilerWindow.CheckFrameData(profilerProperty))
			{
				profilerProperty.Cleanup();
				return;
			}
			if (timelinePane != null && this.m_ViewType == ProfilerViewType.Timeline)
			{
				float num = (float)this.m_VertSplit.realSizes[1];
				num -= EditorStyles.toolbar.CalcHeight(GUIContent.none, 10f) + 2f;
				timelinePane.DoGUI(this.GetActiveVisibleFrameIndex(), base.position.width, base.position.height - num, num);
				profilerProperty.Cleanup();
			}
			else
			{
				SplitterGUILayout.BeginHorizontalSplit(this.m_ViewSplit, new GUILayoutOption[0]);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				bool expandAll = false;
				mainPane.DoGUI(profilerProperty, this.m_SearchString, expandAll);
				profilerProperty.Cleanup();
				GUILayout.EndVertical();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				ProfilerProperty profilerProperty2 = this.CreateProperty(true);
				ProfilerProperty detailedProperty = mainPane.GetDetailedProperty(profilerProperty2);
				profilerProperty2.Cleanup();
				if (detailedProperty != null)
				{
					detailPane.DoGUI(detailedProperty, string.Empty, expandAll);
					detailedProperty.Cleanup();
				}
				else
				{
					ProfilerWindow.DrawEmptyCPUOrRenderingDetailPane();
				}
				GUILayout.EndVertical();
				SplitterGUILayout.EndHorizontalSplit();
			}
		}

		private void DrawMemoryPane(SplitterState splitter)
		{
			this.DrawMemoryToolbar();
			if (this.m_ShowDetailedMemoryPane == ProfilerMemoryView.Simple)
			{
				this.DrawOverviewText(ProfilerArea.Memory);
			}
			else
			{
				this.DrawDetailedMemoryPane(splitter);
			}
		}

		private void DrawDetailedMemoryPane(SplitterState splitter)
		{
			SplitterGUILayout.BeginHorizontalSplit(splitter, new GUILayoutOption[0]);
			this.m_MemoryListView.OnGUI();
			this.m_ReferenceListView.OnGUI();
			SplitterGUILayout.EndHorizontalSplit();
		}

		private static Rect GenerateRect(ref int row, int indent)
		{
			Rect result = new Rect((float)indent * 16f + 8f, (float)row * 16f, 0f, 16f);
			result.xMax = 350f;
			row++;
			return result;
		}

		private void DrawNetworkOperationsPane()
		{
			SplitterGUILayout.BeginHorizontalSplit(this.m_NetworkSplit, new GUILayoutOption[0]);
			GUILayout.Label(ProfilerDriver.GetOverviewText(this.m_CurrentArea, this.GetActiveVisibleFrameIndex()), EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			this.m_PaneScroll[(int)this.m_CurrentArea] = GUILayout.BeginScrollView(this.m_PaneScroll[(int)this.m_CurrentArea], ProfilerWindow.ms_Styles.background);
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Operation Detail", new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Over 5 Ticks", new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Over 10 Ticks", new GUILayoutOption[0]);
			EditorGUILayout.LabelField("Total", new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel++;
			short num = 0;
			while ((int)num < this.msgNames.Length)
			{
				if (NetworkDetailStats.m_NetworkOperations.ContainsKey(num))
				{
					this.msgFoldouts[(int)num] = EditorGUILayout.Foldout(this.msgFoldouts[(int)num], this.msgNames[(int)num] + ":");
					if (this.msgFoldouts[(int)num])
					{
						EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
						NetworkDetailStats.NetworkOperationDetails networkOperationDetails = NetworkDetailStats.m_NetworkOperations[num];
						EditorGUI.indentLevel++;
						foreach (string current in networkOperationDetails.m_Entries.Keys)
						{
							int tick = (int)Time.time;
							NetworkDetailStats.NetworkOperationEntryDetails networkOperationEntryDetails = networkOperationDetails.m_Entries[current];
							if (networkOperationEntryDetails.m_IncomingTotal > 0)
							{
								EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
								EditorGUILayout.LabelField("IN:" + current, new GUILayoutOption[0]);
								EditorGUILayout.LabelField(networkOperationEntryDetails.m_IncomingSequence.GetFiveTick(tick).ToString(), new GUILayoutOption[0]);
								EditorGUILayout.LabelField(networkOperationEntryDetails.m_IncomingSequence.GetTenTick(tick).ToString(), new GUILayoutOption[0]);
								EditorGUILayout.LabelField(networkOperationEntryDetails.m_IncomingTotal.ToString(), new GUILayoutOption[0]);
								EditorGUILayout.EndHorizontal();
							}
							if (networkOperationEntryDetails.m_OutgoingTotal > 0)
							{
								EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
								EditorGUILayout.LabelField("OUT:" + current, new GUILayoutOption[0]);
								EditorGUILayout.LabelField(networkOperationEntryDetails.m_OutgoingSequence.GetFiveTick(tick).ToString(), new GUILayoutOption[0]);
								EditorGUILayout.LabelField(networkOperationEntryDetails.m_OutgoingSequence.GetTenTick(tick).ToString(), new GUILayoutOption[0]);
								EditorGUILayout.LabelField(networkOperationEntryDetails.m_OutgoingTotal.ToString(), new GUILayoutOption[0]);
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUI.indentLevel--;
						EditorGUILayout.EndVertical();
					}
				}
				num += 1;
			}
			EditorGUI.indentLevel--;
			GUILayout.EndScrollView();
			SplitterGUILayout.EndHorizontalSplit();
		}

		private void DrawAudioPane()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			ProfilerAudioView profilerAudioView = this.m_ShowDetailedAudioPane;
			if (GUILayout.Toggle(profilerAudioView == ProfilerAudioView.Stats, "Stats", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				profilerAudioView = ProfilerAudioView.Stats;
			}
			if (GUILayout.Toggle(profilerAudioView == ProfilerAudioView.Channels, "Channels", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				profilerAudioView = ProfilerAudioView.Channels;
			}
			if (GUILayout.Toggle(profilerAudioView == ProfilerAudioView.Groups, "Groups", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				profilerAudioView = ProfilerAudioView.Groups;
			}
			if (GUILayout.Toggle(profilerAudioView == ProfilerAudioView.ChannelsAndGroups, "Channels and groups", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				profilerAudioView = ProfilerAudioView.ChannelsAndGroups;
			}
			if (profilerAudioView != this.m_ShowDetailedAudioPane)
			{
				this.m_ShowDetailedAudioPane = profilerAudioView;
				this.m_LastAudioProfilerFrame = -1;
			}
			if (this.m_ShowDetailedAudioPane == ProfilerAudioView.Stats)
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				this.DrawOverviewText(this.m_CurrentArea);
			}
			else
			{
				GUILayout.Space(5f);
				bool flag = GUILayout.Toggle(AudioUtil.resetAllAudioClipPlayCountsOnPlay, "Reset play count on play", EditorStyles.toolbarButton, new GUILayoutOption[0]);
				if (flag != AudioUtil.resetAllAudioClipPlayCountsOnPlay)
				{
					AudioUtil.resetAllAudioClipPlayCountsOnPlay = flag;
				}
				if (Unsupported.IsDeveloperBuild())
				{
					GUILayout.Space(5f);
					bool @bool = EditorPrefs.GetBool("AudioProfilerShowAllGroups");
					bool flag2 = GUILayout.Toggle(@bool, "Show all groups (dev-builds only)", EditorStyles.toolbarButton, new GUILayoutOption[0]);
					if (@bool != flag2)
					{
						EditorPrefs.SetBool("AudioProfilerShowAllGroups", flag2);
					}
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				Rect rect = GUILayoutUtility.GetRect(20f, 20000f, 10f, 10000f);
				Rect position = new Rect(rect.x, rect.y, 230f, rect.height);
				Rect rect2 = new Rect(position.xMax, rect.y, rect.width - position.width, rect.height);
				string overviewText = ProfilerDriver.GetOverviewText(this.m_CurrentArea, this.GetActiveVisibleFrameIndex());
				Vector2 vector = EditorStyles.wordWrappedLabel.CalcSize(GUIContent.Temp(overviewText));
				this.m_PaneScroll[(int)this.m_CurrentArea] = GUI.BeginScrollView(position, this.m_PaneScroll[(int)this.m_CurrentArea], new Rect(0f, 0f, vector.x, vector.y));
				GUI.Label(new Rect(3f, 3f, vector.x, vector.y), overviewText, EditorStyles.wordWrappedLabel);
				GUI.EndScrollView();
				EditorGUI.DrawRect(new Rect(position.xMax - 1f, position.y, 1f, position.height), Color.black);
				if (this.m_AudioProfilerTreeViewState == null)
				{
					this.m_AudioProfilerTreeViewState = new AudioProfilerTreeViewState();
				}
				if (this.m_AudioProfilerBackend == null)
				{
					this.m_AudioProfilerBackend = new AudioProfilerBackend(this.m_AudioProfilerTreeViewState);
				}
				ProfilerProperty profilerProperty = this.CreateProperty(false);
				if (ProfilerWindow.CheckFrameData(profilerProperty))
				{
					if (this.m_CurrentFrame == -1 || this.m_LastAudioProfilerFrame != this.m_CurrentFrame)
					{
						this.m_LastAudioProfilerFrame = this.m_CurrentFrame;
						AudioProfilerInfo[] audioProfilerInfo = profilerProperty.GetAudioProfilerInfo();
						if (audioProfilerInfo != null && audioProfilerInfo.Length > 0)
						{
							List<AudioProfilerInfoWrapper> list = new List<AudioProfilerInfoWrapper>();
							AudioProfilerInfo[] array = audioProfilerInfo;
							for (int i = 0; i < array.Length; i++)
							{
								AudioProfilerInfo info = array[i];
								bool flag3 = (info.flags & 64) != 0;
								if (this.m_ShowDetailedAudioPane != ProfilerAudioView.Channels || !flag3)
								{
									if (this.m_ShowDetailedAudioPane != ProfilerAudioView.Groups || flag3)
									{
										list.Add(new AudioProfilerInfoWrapper(info, profilerProperty.GetAudioProfilerNameByOffset(info.assetNameOffset), profilerProperty.GetAudioProfilerNameByOffset(info.objectNameOffset), this.m_ShowDetailedAudioPane == ProfilerAudioView.Channels));
									}
								}
							}
							this.m_AudioProfilerBackend.SetData(list);
							if (this.m_AudioProfilerView == null)
							{
								this.m_AudioProfilerView = new AudioProfilerView(this, this.m_AudioProfilerTreeViewState);
								this.m_AudioProfilerView.Init(rect2, this.m_AudioProfilerBackend);
							}
						}
					}
					if (this.m_AudioProfilerView != null)
					{
						this.m_AudioProfilerView.OnGUI(rect2, this.m_ShowDetailedAudioPane == ProfilerAudioView.Channels);
					}
				}
				profilerProperty.Cleanup();
			}
		}

		private static void DrawBackground(int row, bool selected)
		{
			Rect position = new Rect(1f, 16f * (float)row, GUIClip.visibleRect.width, 16f);
			GUIStyle gUIStyle = (row % 2 != 0) ? ProfilerWindow.ms_Styles.entryOdd : ProfilerWindow.ms_Styles.entryEven;
			if (Event.current.type == EventType.Repaint)
			{
				gUIStyle.Draw(position, GUIContent.none, false, false, selected, false);
			}
		}

		private void DrawMemoryToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			this.m_ShowDetailedMemoryPane = (ProfilerMemoryView)EditorGUILayout.EnumPopup(this.m_ShowDetailedMemoryPane, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			GUILayout.Space(5f);
			if (this.m_ShowDetailedMemoryPane == ProfilerMemoryView.Detailed)
			{
				if (GUILayout.Button("Take Sample: " + this.m_AttachProfilerUI.GetConnectedProfiler(), EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					this.RefreshMemoryData();
				}
				this.m_GatherObjectReferences = GUILayout.Toggle(this.m_GatherObjectReferences, ProfilerWindow.ms_Styles.gatherObjectReferences, EditorStyles.toolbarButton, new GUILayoutOption[0]);
				if (this.m_AttachProfilerUI.IsEditor())
				{
					GUILayout.Label("Memory usage in editor is not as it would be in a player", EditorStyles.toolbarButton, new GUILayoutOption[0]);
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void RefreshMemoryData()
		{
			this.m_MemoryListView.SetRoot(null);
			ProfilerDriver.RequestObjectMemoryInfo(this.m_GatherObjectReferences);
		}

		private static void UpdateChartGrid(float timeMax, ChartData data)
		{
			if (timeMax < 1500f)
			{
				data.SetGrid(new float[]
				{
					1000f,
					250f,
					100f
				}, new string[]
				{
					"1ms (1000FPS)",
					"0.25ms (4000FPS)",
					"0.1ms (10000FPS)"
				});
			}
			else if (timeMax < 10000f)
			{
				data.SetGrid(new float[]
				{
					8333f,
					4000f,
					1000f
				}, new string[]
				{
					"8ms (120FPS)",
					"4ms (250FPS)",
					"1ms (1000FPS)"
				});
			}
			else if (timeMax < 30000f)
			{
				data.SetGrid(new float[]
				{
					16667f,
					10000f,
					5000f
				}, new string[]
				{
					"16ms (60FPS)",
					"10ms (100FPS)",
					"5ms (200FPS)"
				});
			}
			else if (timeMax < 100000f)
			{
				data.SetGrid(new float[]
				{
					66667f,
					33333f,
					16667f
				}, new string[]
				{
					"66ms (15FPS)",
					"33ms (30FPS)",
					"16ms (60FPS)"
				});
			}
			else
			{
				data.SetGrid(new float[]
				{
					500000f,
					200000f,
					66667f
				}, new string[]
				{
					"500ms (2FPS)",
					"200ms (5FPS)",
					"66ms (15FPS)"
				});
			}
		}

		private void UpdateCharts()
		{
			int num = ProfilerDriver.maxHistoryLength - 1;
			int num2 = ProfilerDriver.lastFrameIndex - num;
			int num3 = Mathf.Max(ProfilerDriver.firstFrameIndex, num2);
			ProfilerChart[] charts = this.m_Charts;
			for (int i = 0; i < charts.Length; i++)
			{
				ProfilerChart profilerChart = charts[i];
				float num4 = 1f;
				float[] array = new float[profilerChart.m_Series.Length];
				for (int j = 0; j < profilerChart.m_Series.Length; j++)
				{
					int statisticsIdentifier = ProfilerDriver.GetStatisticsIdentifier(profilerChart.m_Series[j].identifierName);
					float num5;
					ProfilerDriver.GetStatisticsValues(statisticsIdentifier, num2, profilerChart.m_DataScale, profilerChart.m_Series[j].data, out num5);
					num5 = Mathf.Max(num5, 0.0001f);
					if (num5 > num4)
					{
						num4 = num5;
					}
					float num6;
					if (profilerChart.m_Type == Chart.ChartType.Line)
					{
						num6 = 1f / (num5 * (1.05f + (float)j * 0.05f));
					}
					else
					{
						num6 = 1f / num5;
					}
					array[j] = num6;
				}
				if (profilerChart.m_Type == Chart.ChartType.Line)
				{
					profilerChart.m_Data.AssignScale(array);
				}
				if (profilerChart.m_Area == ProfilerArea.NetworkMessages || profilerChart.m_Area == ProfilerArea.NetworkOperations)
				{
					for (int k = 0; k < profilerChart.m_Series.Length; k++)
					{
						array[k] = 0.9f / num4;
					}
					profilerChart.m_Data.AssignScale(array);
					profilerChart.m_Data.maxValue = num4;
				}
				profilerChart.m_Data.Assign(profilerChart.m_Series, num2, num3);
			}
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
			bool flag = selectedPropertyPath != string.Empty && this.m_CurrentArea == ProfilerArea.CPU;
			ProfilerChart profilerChart2 = this.m_Charts[0];
			if (flag)
			{
				profilerChart2.m_Data.hasOverlay = true;
				ChartSeries[] series = profilerChart2.m_Series;
				for (int l = 0; l < series.Length; l++)
				{
					ChartSeries chartSeries = series[l];
					int statisticsIdentifier2 = ProfilerDriver.GetStatisticsIdentifier("Selected" + chartSeries.identifierName);
					chartSeries.CreateOverlayData();
					float num7;
					ProfilerDriver.GetStatisticsValues(statisticsIdentifier2, num2, profilerChart2.m_DataScale, chartSeries.overlayData, out num7);
				}
			}
			else
			{
				profilerChart2.m_Data.hasOverlay = false;
			}
			for (ProfilerArea profilerArea = ProfilerArea.CPU; profilerArea <= ProfilerArea.GPU; profilerArea++)
			{
				ProfilerChart profilerChart3 = this.m_Charts[(int)profilerArea];
				float num8 = 0f;
				float num9 = 0f;
				for (int m = 0; m < num; m++)
				{
					float num10 = 0f;
					for (int n = 0; n < profilerChart3.m_Series.Length; n++)
					{
						if (profilerChart3.m_Series[n].enabled)
						{
							num10 += profilerChart3.m_Series[n].data[m];
						}
					}
					if (num10 > num8)
					{
						num8 = num10;
					}
					if (num10 > num9 && m + num2 >= num3 + 1)
					{
						num9 = num10;
					}
				}
				if (num9 != 0f)
				{
					num8 = num9;
				}
				num8 = Math.Min(num8, this.m_ChartMaxClamp);
				if (this.m_ChartOldMax[(int)profilerArea] > 0f)
				{
					num8 = Mathf.Lerp(this.m_ChartOldMax[(int)profilerArea], num8, 0.4f);
				}
				this.m_ChartOldMax[(int)profilerArea] = num8;
				profilerChart3.m_Data.AssignScale(new float[]
				{
					1f / num8
				});
				ProfilerWindow.UpdateChartGrid(num8, profilerChart3.m_Data);
			}
			string notSupportedWarning = null;
			if (ProfilerDriver.isGPUProfilerBuggyOnDriver)
			{
				notSupportedWarning = "Graphics card driver returned invalid timing information. Please update to a newer version if available.";
			}
			else if (!ProfilerDriver.isGPUProfilerSupported)
			{
				notSupportedWarning = "GPU profiling is not supported by the graphics card driver. Please update to a newer version if available.";
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					if (!ProfilerDriver.isGPUProfilerSupportedByOS)
					{
						notSupportedWarning = "GPU profiling requires Mac OS X 10.7 (Lion) and a capable video card. GPU profiling is currently not supported on mobile.";
					}
					else
					{
						notSupportedWarning = "GPU profiling is not supported by the graphics card driver (or it was disabled because of driver bugs).";
					}
				}
			}
			this.m_Charts[1].m_Chart.m_NotSupportedWarning = notSupportedWarning;
		}

		private void AddAreaClick(object userData, string[] options, int selected)
		{
			this.m_Charts[selected].active = true;
		}

		private void DrawMainToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(ProfilerWindow.ms_Styles.addArea, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			if (EditorGUI.ButtonMouseDown(rect, ProfilerWindow.ms_Styles.addArea, FocusType.Native, EditorStyles.toolbarDropDown))
			{
				int num = this.m_Charts.Length;
				string[] array = new string[num];
				bool[] array2 = new bool[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = ((ProfilerArea)i).ToString();
					array2[i] = !this.m_Charts[i].active;
				}
				EditorUtility.DisplayCustomMenu(rect, array, array2, null, new EditorUtility.SelectMenuItemFunction(this.AddAreaClick), null);
			}
			GUILayout.FlexibleSpace();
			bool flag = GUILayout.Toggle(this.m_Recording, ProfilerWindow.ms_Styles.profilerRecord, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (flag != this.m_Recording)
			{
				Profiler.enabled = flag;
				this.m_Recording = flag;
				SessionState.SetBool("ProfilerEnabled", flag);
			}
			ProfilerWindow.SetProfileDeepScripts(GUILayout.Toggle(ProfilerDriver.deepProfiling, ProfilerWindow.ms_Styles.deepProfile, EditorStyles.toolbarButton, new GUILayoutOption[0]));
			ProfilerDriver.profileEditor = GUILayout.Toggle(ProfilerDriver.profileEditor, ProfilerWindow.ms_Styles.profileEditor, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			this.m_AttachProfilerUI.OnGUILayout(this);
			if (GUILayout.Button(ProfilerWindow.ms_Styles.clearData, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				ProfilerDriver.ClearAllFrames();
				NetworkDetailStats.m_NetworkOperations.Clear();
			}
			GUILayout.Space(5f);
			GUILayout.FlexibleSpace();
			this.FrameNavigationControls();
			GUILayout.EndHorizontal();
		}

		private void FrameNavigationControls()
		{
			if (this.m_CurrentFrame > ProfilerDriver.lastFrameIndex)
			{
				this.SetCurrentFrameDontPause(ProfilerDriver.lastFrameIndex);
			}
			GUILayout.Label(ProfilerWindow.ms_Styles.frame, EditorStyles.miniLabel, new GUILayoutOption[0]);
			GUILayout.Label("   " + this.PickFrameLabel(), EditorStyles.miniLabel, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			GUI.enabled = (ProfilerDriver.GetPreviousFrameIndex(this.m_CurrentFrame) != -1);
			if (GUILayout.Button(ProfilerWindow.ms_Styles.prevFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.PrevFrame();
			}
			GUI.enabled = (ProfilerDriver.GetNextFrameIndex(this.m_CurrentFrame) != -1);
			if (GUILayout.Button(ProfilerWindow.ms_Styles.nextFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.NextFrame();
			}
			GUI.enabled = true;
			GUILayout.Space(10f);
			if (GUILayout.Button(ProfilerWindow.ms_Styles.currentFrame, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.SetCurrentFrame(-1);
				this.m_LastFrameFromTick = ProfilerDriver.lastFrameIndex;
			}
		}

		private static void DrawOtherToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawOverviewText(ProfilerArea area)
		{
			this.m_PaneScroll[(int)area] = GUILayout.BeginScrollView(this.m_PaneScroll[(int)area], ProfilerWindow.ms_Styles.background);
			GUILayout.Label(ProfilerDriver.GetOverviewText(area, this.GetActiveVisibleFrameIndex()), EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			GUILayout.EndScrollView();
		}

		private void DrawPane(ProfilerArea area)
		{
			ProfilerWindow.DrawOtherToolbar();
			this.DrawOverviewText(area);
		}

		private void SetCurrentFrameDontPause(int frame)
		{
			this.m_CurrentFrame = frame;
		}

		private void SetCurrentFrame(int frame)
		{
			if (frame != -1 && Profiler.enabled && !ProfilerDriver.profileEditor && this.m_CurrentFrame != frame && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.isPaused = true;
			}
			if (ProfilerInstrumentationPopup.InstrumentationEnabled)
			{
				ProfilerInstrumentationPopup.UpdateInstrumentableFunctions();
			}
			this.SetCurrentFrameDontPause(frame);
		}

		private void OnGUI()
		{
			this.CheckForPlatformModuleChange();
			if (ProfilerWindow.ms_Styles == null)
			{
				ProfilerWindow.ms_Styles = new ProfilerWindow.Styles();
			}
			this.DrawMainToolbar();
			SplitterGUILayout.BeginVerticalSplit(this.m_VertSplit, new GUILayoutOption[0]);
			this.m_GraphPos = EditorGUILayout.BeginScrollView(this.m_GraphPos, ProfilerWindow.ms_Styles.profilerGraphBackground, new GUILayoutOption[0]);
			if (this.m_PrevLastFrame != ProfilerDriver.lastFrameIndex)
			{
				this.UpdateCharts();
				this.m_PrevLastFrame = ProfilerDriver.lastFrameIndex;
			}
			int num = this.m_CurrentFrame;
			Chart.ChartAction[] array = new Chart.ChartAction[this.m_Charts.Length];
			for (int i = 0; i < this.m_Charts.Length; i++)
			{
				ProfilerChart profilerChart = this.m_Charts[i];
				if (profilerChart.active)
				{
					num = profilerChart.DoChartGUI(num, this.m_CurrentArea, out array[i]);
				}
			}
			bool flag = false;
			if (num != this.m_CurrentFrame)
			{
				this.SetCurrentFrame(num);
				flag = true;
			}
			for (int j = 0; j < this.m_Charts.Length; j++)
			{
				ProfilerChart profilerChart2 = this.m_Charts[j];
				if (profilerChart2.active)
				{
					if (array[j] == Chart.ChartAction.Closed)
					{
						if (this.m_CurrentArea == (ProfilerArea)j)
						{
							this.m_CurrentArea = ProfilerArea.CPU;
						}
						profilerChart2.active = false;
					}
					else if (array[j] == Chart.ChartAction.Activated)
					{
						this.m_CurrentArea = (ProfilerArea)j;
						if (this.m_CurrentArea != ProfilerArea.CPU && this.m_CPUHierarchyGUI.selectedIndex != -1)
						{
							this.ClearSelectedPropertyPath();
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				base.Repaint();
				GUIUtility.ExitGUI();
			}
			GUILayout.EndScrollView();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			switch (this.m_CurrentArea)
			{
			case ProfilerArea.CPU:
				this.DrawCPUOrRenderingPane(this.m_CPUHierarchyGUI, this.m_CPUDetailHierarchyGUI, this.m_CPUTimelineGUI);
				goto IL_25B;
			case ProfilerArea.GPU:
				this.DrawCPUOrRenderingPane(this.m_GPUHierarchyGUI, this.m_GPUDetailHierarchyGUI, null);
				goto IL_25B;
			case ProfilerArea.Memory:
				this.DrawMemoryPane(this.m_ViewSplit);
				goto IL_25B;
			case ProfilerArea.Audio:
				this.DrawAudioPane();
				goto IL_25B;
			case ProfilerArea.NetworkMessages:
				this.DrawPane(this.m_CurrentArea);
				goto IL_25B;
			case ProfilerArea.NetworkOperations:
				this.DrawNetworkOperationsPane();
				goto IL_25B;
			}
			this.DrawPane(this.m_CurrentArea);
			IL_25B:
			GUILayout.EndVertical();
			SplitterGUILayout.EndVerticalSplit();
		}

		virtual void Repaint()
		{
			base.Repaint();
		}
	}
}
