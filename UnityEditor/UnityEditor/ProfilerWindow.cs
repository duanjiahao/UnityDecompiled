using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class ProfilerWindow : EditorWindow, IProfilerWindowController
	{
		internal class Styles
		{
			public GUIContent addArea = EditorGUIUtility.TextContent("Profiler.AddArea");
			public GUIContent deepProfile = EditorGUIUtility.TextContent("Profiler.DeepProfile");
			public GUIContent profileEditor = EditorGUIUtility.TextContent("Profiler.ProfileEditor");
			public GUIContent noData = EditorGUIUtility.TextContent("Profiler.NoFrameDataAvailable");
			public GUIContent noLicense = EditorGUIUtility.TextContent("Profiler.NoLicense");
			public GUIContent profilerRecord = EditorGUIUtility.TextContent("Profiler.Record");
			public GUIContent prevFrame = EditorGUIUtility.IconContent("Profiler.PrevFrame");
			public GUIContent nextFrame = EditorGUIUtility.IconContent("Profiler.NextFrame");
			public GUIContent currentFrame = EditorGUIUtility.TextContent("Profiler.CurrentFrame");
			public GUIContent frame = EditorGUIUtility.TextContent("Frame: ");
			public GUIContent clearData = EditorGUIUtility.TextContent("Clear");
			public GUIContent[] reasons = EditorGUIUtility.GetTextContentsForEnum(typeof(MemoryInfoGCReason));
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
		internal const string kPrefCharts = "ProfilerChart";
		private static ProfilerWindow.Styles ms_Styles;
		private bool m_HasProfilerLicense;
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
		[SerializeField]
		private bool m_Recording = true;
		private AttachProfilerUI m_AttachProfilerUI = default(AttachProfilerUI);
		private Vector2 m_GraphPos = Vector2.zero;
		private Vector2[] m_PaneScroll = new Vector2[7];
		private static List<ProfilerWindow> m_ProfilerWindows = new List<ProfilerWindow>();
		private ProfilerViewType m_ViewType;
		private ProfilerArea m_CurrentArea;
		private ProfilerMemoryView m_ShowDetailedMemoryPane;
		private int m_CurrentFrame = -1;
		private int m_LastFrameFromTick = -1;
		private int m_PrevLastFrame = -1;
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
			string text = EditorGUIUtility.TextContent("ProfilerColumn.DetailViewObject").text;
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
				array[i] = "ProfilerColumn." + names[(int)columns[i]];
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
		private void OnEnable()
		{
			ProfilerWindow.m_ProfilerWindows.Add(this);
			this.m_HasProfilerLicense = InternalEditorUtility.HasPro();
			int len = ProfilerDriver.maxHistoryLength - 1;
			this.m_Charts = new ProfilerChart[7];
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
			string[] displayedOptions;
			int[] optionValues;
			if (!Unsupported.IsDeveloperBuild())
			{
				displayedOptions = new string[]
				{
					"Hierarchy",
					"Raw Hierarchy"
				};
				optionValues = new int[]
				{
					0,
					2
				};
			}
			else
			{
				displayedOptions = new string[]
				{
					"Hierarchy",
					"Timeline",
					"Raw Hierarchy"
				};
				optionValues = new int[]
				{
					0,
					1,
					2
				};
			}
			this.m_ViewType = (ProfilerViewType)EditorGUILayout.IntPopup((int)this.m_ViewType, displayedOptions, optionValues, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("CPU:{0}ms   GPU:{1}ms", property.frameTime, property.frameGpuTime), EditorStyles.miniLabel, new GUILayoutOption[0]);
			EditorGUILayout.EndHorizontal();
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
				mainPane.DoGUI(profilerProperty, expandAll);
				profilerProperty.Cleanup();
				GUILayout.EndVertical();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				ProfilerProperty profilerProperty2 = this.CreateProperty(true);
				ProfilerProperty detailedProperty = mainPane.GetDetailedProperty(profilerProperty2);
				profilerProperty2.Cleanup();
				if (detailedProperty != null)
				{
					detailPane.DoGUI(detailedProperty, expandAll);
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
			ProfilerDriver.RequestObjectMemoryInfo();
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
			else
			{
				if (timeMax < 10000f)
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
				else
				{
					if (timeMax < 30000f)
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
					else
					{
						if (timeMax < 100000f)
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
				}
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
				float[] array = new float[profilerChart.m_Series.Length];
				for (int j = 0; j < profilerChart.m_Series.Length; j++)
				{
					int statisticsIdentifier = ProfilerDriver.GetStatisticsIdentifier(profilerChart.m_Series[j].identifierName);
					float num4;
					ProfilerDriver.GetStatisticsValues(statisticsIdentifier, num2, profilerChart.m_DataScale, profilerChart.m_Series[j].data, out num4);
					float num5;
					if (profilerChart.m_Type == Chart.ChartType.Line)
					{
						num5 = 1f / (num4 * (1.05f + (float)j * 0.05f));
					}
					else
					{
						num5 = 1f / num4;
					}
					array[j] = num5;
				}
				if (profilerChart.m_Type == Chart.ChartType.Line)
				{
					profilerChart.m_Data.AssignScale(array);
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
				for (int k = 0; k < series.Length; k++)
				{
					ChartSeries chartSeries = series[k];
					int statisticsIdentifier2 = ProfilerDriver.GetStatisticsIdentifier("Selected" + chartSeries.identifierName);
					chartSeries.CreateOverlayData();
					float num6;
					ProfilerDriver.GetStatisticsValues(statisticsIdentifier2, num2, profilerChart2.m_DataScale, chartSeries.overlayData, out num6);
				}
			}
			else
			{
				profilerChart2.m_Data.hasOverlay = false;
			}
			for (ProfilerArea profilerArea = ProfilerArea.CPU; profilerArea <= ProfilerArea.GPU; profilerArea++)
			{
				ProfilerChart profilerChart3 = this.m_Charts[(int)profilerArea];
				float num7 = 0f;
				float num8 = 0f;
				for (int l = 0; l < num; l++)
				{
					float num9 = 0f;
					for (int m = 0; m < profilerChart3.m_Series.Length; m++)
					{
						if (profilerChart3.m_Series[m].enabled)
						{
							num9 += profilerChart3.m_Series[m].data[l];
						}
					}
					if (num9 > num7)
					{
						num7 = num9;
					}
					if (num9 > num8 && l + num2 >= num3 + 1)
					{
						num8 = num9;
					}
				}
				if (num8 != 0f)
				{
					num7 = num8;
				}
				num7 = Math.Min(num7, this.m_ChartMaxClamp);
				if (this.m_ChartOldMax[(int)profilerArea] > 0f)
				{
					num7 = Mathf.Lerp(this.m_ChartOldMax[(int)profilerArea], num7, 0.4f);
				}
				this.m_ChartOldMax[(int)profilerArea] = num7;
				profilerChart3.m_Data.AssignScale(new float[]
				{
					1f / num7
				});
				ProfilerWindow.UpdateChartGrid(num7, profilerChart3.m_Data);
			}
			string notSupportedWarning = null;
			if (ProfilerDriver.isGPUProfilerBuggyOnDriver)
			{
				notSupportedWarning = "Graphics card driver returned invalid timing information. Please update to a newer version if available.";
			}
			else
			{
				if (!ProfilerDriver.isGPUProfilerSupported)
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
			}
			this.m_Charts[1].m_Chart.m_NotSupportedWarning = notSupportedWarning;
		}
		private void AddAreaClick(object userData, string[] options, int selected)
		{
			this.m_Charts[selected].m_Active = true;
			EditorPrefs.SetBool("ProfilerChart" + (ProfilerArea)selected, true);
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
					array2[i] = !this.m_Charts[i].m_Active;
				}
				EditorUtility.DisplayCustomMenu(rect, array, array2, null, new EditorUtility.SelectMenuItemFunction(this.AddAreaClick), null);
			}
			GUILayout.FlexibleSpace();
			this.m_Recording = GUILayout.Toggle(this.m_Recording, ProfilerWindow.ms_Styles.profilerRecord, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			Profiler.enabled = this.m_Recording;
			ProfilerWindow.SetProfileDeepScripts(GUILayout.Toggle(ProfilerDriver.deepProfiling, ProfilerWindow.ms_Styles.deepProfile, EditorStyles.toolbarButton, new GUILayoutOption[0]));
			ProfilerDriver.profileEditor = GUILayout.Toggle(ProfilerDriver.profileEditor, ProfilerWindow.ms_Styles.profileEditor, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			this.m_AttachProfilerUI.OnGUILayout(this);
			if (GUILayout.Button(ProfilerWindow.ms_Styles.clearData, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				ProfilerDriver.ClearAllFrames();
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
		private int GetActiveVisibleFrameIndex()
		{
			return (this.m_CurrentFrame != -1) ? this.m_CurrentFrame : this.m_LastFrameFromTick;
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
			this.SetCurrentFrameDontPause(frame);
		}
		private void OnGUI()
		{
			if (ProfilerWindow.ms_Styles == null)
			{
				ProfilerWindow.ms_Styles = new ProfilerWindow.Styles();
			}
			if (!this.m_HasProfilerLicense)
			{
				GUILayout.Label(ProfilerWindow.ms_Styles.noLicense, EditorStyles.largeLabel, new GUILayoutOption[0]);
				return;
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
				if (profilerChart.m_Active)
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
				if (profilerChart2.m_Active)
				{
					if (array[j] == Chart.ChartAction.Closed)
					{
						if (this.m_CurrentArea == (ProfilerArea)j)
						{
							this.m_CurrentArea = ProfilerArea.CPU;
						}
						profilerChart2.m_Active = false;
						EditorPrefs.SetBool("ProfilerChart" + (ProfilerArea)j, false);
					}
					else
					{
						if (array[j] == Chart.ChartAction.Activated)
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
				goto IL_257;
			case ProfilerArea.GPU:
				this.DrawCPUOrRenderingPane(this.m_GPUHierarchyGUI, this.m_GPUDetailHierarchyGUI, null);
				goto IL_257;
			case ProfilerArea.Memory:
				this.DrawMemoryPane(this.m_ViewSplit);
				goto IL_257;
			}
			this.DrawPane(this.m_CurrentArea);
			IL_257:
			GUILayout.EndVertical();
			SplitterGUILayout.EndVerticalSplit();
		}
		virtual void Repaint()
		{
			base.Repaint();
		}
	}
}
