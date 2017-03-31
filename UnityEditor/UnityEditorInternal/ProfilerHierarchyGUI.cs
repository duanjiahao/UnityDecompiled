using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditorInternal
{
	internal class ProfilerHierarchyGUI
	{
		internal class Styles
		{
			public GUIStyle background = "OL Box";

			public GUIStyle header = "OL title";

			public GUIStyle rightHeader = "OL title TextRight";

			public GUIStyle entryEven = "OL EntryBackEven";

			public GUIStyle entryOdd = "OL EntryBackOdd";

			public GUIStyle numberLabel = "OL Label";

			public GUIStyle foldout = "IN foldout";

			public GUIStyle miniPullDown = "MiniPullDown";

			public GUIContent disabledSearchText = new GUIContent("Showing search results are disabled while recording with deep profiling.\nStop recording to view search results.");

			public GUIContent notShowingAllResults = new GUIContent("...", "Narrow your search. Not all search results can be shown.");

			public GUIContent instrumentationIcon = EditorGUIUtility.IconContent("Profiler.Record", "Record|Record profiling information");
		}

		internal class SearchResults
		{
			private struct SearchResult
			{
				public string propertyPath;

				public string[] columnValues;
			}

			private ProfilerHierarchyGUI.SearchResults.SearchResult[] m_SearchResults;

			private int m_NumResultsUsed;

			private ProfilerColumn[] m_ColumnsToShow;

			private int m_SelectedSearchIndex;

			private bool m_FoundAllResults;

			private string m_LastSearchString;

			private int m_LastFrameIndex;

			private ProfilerColumn m_LastSortType;

			public int numRows
			{
				get
				{
					return this.m_NumResultsUsed + ((!this.m_FoundAllResults) ? 1 : 0);
				}
			}

			public int selectedSearchIndex
			{
				get
				{
					return this.m_SelectedSearchIndex;
				}
				set
				{
					if (value < this.m_NumResultsUsed)
					{
						this.m_SelectedSearchIndex = value;
					}
					else
					{
						this.m_SelectedSearchIndex = -1;
					}
					if (this.m_SelectedSearchIndex >= 0)
					{
						string propertyPath = this.m_SearchResults[this.m_SelectedSearchIndex].propertyPath;
						if (propertyPath != ProfilerDriver.selectedPropertyPath)
						{
							ProfilerDriver.selectedPropertyPath = propertyPath;
						}
					}
				}
			}

			public void Init(int maxNumberSearchResults)
			{
				this.m_SearchResults = new ProfilerHierarchyGUI.SearchResults.SearchResult[maxNumberSearchResults];
				this.m_NumResultsUsed = 0;
				this.m_LastSearchString = "";
				this.m_LastFrameIndex = -1;
				this.m_FoundAllResults = false;
				this.m_ColumnsToShow = null;
				this.m_SelectedSearchIndex = -1;
			}

			public void Filter(ProfilerProperty property, ProfilerColumn[] columns, string searchString, int frameIndex, ProfilerColumn sortType)
			{
				if (!(searchString == this.m_LastSearchString) || frameIndex != this.m_LastFrameIndex || sortType != this.m_LastSortType)
				{
					this.m_LastSearchString = searchString;
					this.m_LastFrameIndex = frameIndex;
					this.m_LastSortType = sortType;
					this.IterateProfilingData(property, columns, searchString);
				}
			}

			private void IterateProfilingData(ProfilerProperty property, ProfilerColumn[] columns, string searchString)
			{
				this.m_NumResultsUsed = 0;
				this.m_ColumnsToShow = columns;
				this.m_FoundAllResults = true;
				this.m_SelectedSearchIndex = -1;
				int num = 0;
				string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
				while (property.Next(true))
				{
					if (num >= this.m_SearchResults.Length)
					{
						this.m_FoundAllResults = false;
						break;
					}
					string propertyPath = property.propertyPath;
					int startIndex = Mathf.Max(propertyPath.LastIndexOf('/'), 0);
					if (propertyPath.IndexOf(searchString, startIndex, StringComparison.CurrentCultureIgnoreCase) > -1)
					{
						string[] array = new string[this.m_ColumnsToShow.Length];
						for (int i = 0; i < this.m_ColumnsToShow.Length; i++)
						{
							array[i] = property.GetColumn(this.m_ColumnsToShow[i]);
						}
						this.m_SearchResults[num].propertyPath = propertyPath;
						this.m_SearchResults[num].columnValues = array;
						if (propertyPath == selectedPropertyPath)
						{
							this.m_SelectedSearchIndex = num;
						}
						num++;
					}
				}
				this.m_NumResultsUsed = num;
			}

			public void Draw(ProfilerHierarchyGUI gui, int controlID)
			{
				this.HandleCommandEvents(gui);
				Event current = Event.current;
				string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
				int num;
				int num2;
				ProfilerHierarchyGUI.SearchResults.GetFirstAndLastRowVisible(this.m_NumResultsUsed, 16f, gui.m_TextScroll.y, (float)gui.m_ScrollViewHeight, out num, out num2);
				for (int i = num; i <= num2; i++)
				{
					bool flag = selectedPropertyPath == this.m_SearchResults[i].propertyPath;
					Rect rowRect = gui.GetRowRect(i);
					GUIStyle rowBackgroundStyle = gui.GetRowBackgroundStyle(i);
					if (current.type == EventType.MouseDown && rowRect.Contains(current.mousePosition))
					{
						this.m_SelectedSearchIndex = i;
						gui.RowMouseDown(this.m_SearchResults[i].propertyPath);
						GUIUtility.keyboardControl = controlID;
						current.Use();
					}
					if (current.type == EventType.Repaint)
					{
						rowBackgroundStyle.Draw(rowRect, GUIContent.none, false, false, flag, GUIUtility.keyboardControl == controlID);
						if (rowRect.Contains(current.mousePosition))
						{
							string text = this.m_SearchResults[i].propertyPath.Replace("/", "/\n");
							if (this.m_SelectedSearchIndex >= 0)
							{
								text += "\n\n(Press 'F' to frame selection)";
							}
							GUI.Label(rowRect, GUIContent.Temp(string.Empty, text));
						}
						gui.DrawTextColumn(ref rowRect, this.m_SearchResults[i].columnValues[0], 0, 4f, flag);
						ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleRight;
						int num3 = 1;
						for (int j = 1; j < gui.m_VisibleColumns.Length; j++)
						{
							if (gui.ColIsVisible(j))
							{
								rowRect.x += (float)gui.m_Splitter.realSizes[num3 - 1];
								rowRect.width = (float)gui.m_Splitter.realSizes[num3] - 4f;
								num3++;
								ProfilerHierarchyGUI.styles.numberLabel.Draw(rowRect, this.m_SearchResults[i].columnValues[j], false, false, false, flag);
							}
						}
						ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleLeft;
					}
				}
				if (!this.m_FoundAllResults && current.type == EventType.Repaint)
				{
					int numResultsUsed = this.m_NumResultsUsed;
					Rect position = new Rect(1f, 16f * (float)numResultsUsed, GUIClip.visibleRect.width, 16f);
					GUIStyle gUIStyle = (numResultsUsed % 2 != 0) ? ProfilerHierarchyGUI.styles.entryOdd : ProfilerHierarchyGUI.styles.entryEven;
					GUI.Label(position, GUIContent.Temp(string.Empty, ProfilerHierarchyGUI.styles.notShowingAllResults.tooltip), GUIStyle.none);
					gUIStyle.Draw(position, GUIContent.none, false, false, false, false);
					gui.DrawTextColumn(ref position, ProfilerHierarchyGUI.styles.notShowingAllResults.text, 0, 4f, false);
				}
			}

			private static void GetFirstAndLastRowVisible(int numRows, float rowHeight, float scrollBarY, float scrollAreaHeight, out int firstRowVisible, out int lastRowVisible)
			{
				firstRowVisible = (int)Mathf.Floor(scrollBarY / rowHeight);
				lastRowVisible = firstRowVisible + (int)Mathf.Ceil(scrollAreaHeight / rowHeight);
				firstRowVisible = Mathf.Max(firstRowVisible, 0);
				lastRowVisible = Mathf.Min(lastRowVisible, numRows - 1);
			}

			public void MoveSelection(int steps, ProfilerHierarchyGUI gui)
			{
				int num = Mathf.Clamp(this.m_SelectedSearchIndex + steps, 0, this.m_NumResultsUsed - 1);
				if (num != this.m_SelectedSearchIndex)
				{
					this.m_SelectedSearchIndex = num;
					gui.m_Window.SetSelectedPropertyPath(this.m_SearchResults[num].propertyPath);
				}
			}

			private void HandleCommandEvents(ProfilerHierarchyGUI gui)
			{
				Event current = Event.current;
				EventType type = current.type;
				if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
				{
					bool flag = type == EventType.ExecuteCommand;
					if (Event.current.commandName == "FrameSelected")
					{
						if (flag)
						{
							gui.FrameSelection();
						}
						current.Use();
					}
				}
			}
		}

		private static int hierarchyViewHash = "HierarchyView".GetHashCode();

		private const float kRowHeight = 16f;

		private const float kFoldoutSize = 14f;

		private const float kIndent = 16f;

		private const float kSmallMargin = 4f;

		private const float kBaseIndent = 4f;

		private const float kInstrumentationButtonWidth = 30f;

		private const float kInstrumentationButtonOffset = 5f;

		private const int kFirst = -999999;

		private const int kLast = 999999;

		private const float kScrollbarWidth = 16f;

		protected static ProfilerHierarchyGUI.Styles ms_Styles;

		private IProfilerWindowController m_Window;

		private SplitterState m_Splitter = null;

		private ProfilerColumn[] m_ColumnsToShow;

		private string[] m_ColumnNames;

		private bool[] m_VisibleColumns;

		private float[] m_SplitterRelativeSizes;

		private int[] m_SplitterMinWidths;

		private string m_ColumnSettingsName;

		private Vector2 m_TextScroll = Vector2.zero;

		private GUIContent[] m_HeaderContent;

		private GUIContent m_SearchHeader;

		private SerializedStringTable m_ExpandedHash = new SerializedStringTable();

		private bool m_ExpandAll;

		private int m_ScrollViewHeight;

		private int m_DoScroll;

		private int m_SelectedIndex = -1;

		private bool m_DetailPane;

		private ProfilerHierarchyGUI.SearchResults m_SearchResults;

		private bool m_SetKeyboardFocus;

		private ProfilerColumn m_SortType = ProfilerColumn.TotalTime;

		private string m_DetailViewSelectedProperty = string.Empty;

		private ProfilerDetailedObjectsView m_DetailedObjectsView;

		protected static ProfilerHierarchyGUI.Styles styles
		{
			get
			{
				ProfilerHierarchyGUI.Styles arg_18_0;
				if ((arg_18_0 = ProfilerHierarchyGUI.ms_Styles) == null)
				{
					arg_18_0 = (ProfilerHierarchyGUI.ms_Styles = new ProfilerHierarchyGUI.Styles());
				}
				return arg_18_0;
			}
		}

		public int selectedIndex
		{
			get
			{
				int result;
				if (this.IsSearchActive())
				{
					result = this.m_SearchResults.selectedSearchIndex;
				}
				else
				{
					result = this.m_SelectedIndex;
				}
				return result;
			}
			set
			{
				if (this.IsSearchActive())
				{
					this.m_SearchResults.selectedSearchIndex = value;
				}
				else
				{
					this.m_SelectedIndex = value;
				}
			}
		}

		public ProfilerColumn sortType
		{
			get
			{
				return this.m_SortType;
			}
			private set
			{
				this.m_SortType = value;
			}
		}

		public ProfilerDetailedObjectsView detailedObjectsView
		{
			get
			{
				return this.m_DetailedObjectsView;
			}
		}

		public ProfilerHierarchyGUI(IProfilerWindowController window, ProfilerHierarchyGUI detailedObjectsView, string columnSettingsName, ProfilerColumn[] columnsToShow, string[] columnNames, bool detailPane, ProfilerColumn sort)
		{
			this.m_Window = window;
			this.m_ColumnNames = columnNames;
			this.m_ColumnSettingsName = columnSettingsName;
			this.m_ColumnsToShow = columnsToShow;
			this.m_DetailPane = detailPane;
			this.m_SortType = sort;
			this.m_HeaderContent = new GUIContent[columnNames.Length];
			this.m_Splitter = null;
			for (int i = 0; i < this.m_HeaderContent.Length; i++)
			{
				this.m_HeaderContent[i] = ((!this.m_ColumnNames[i].StartsWith("|")) ? new GUIContent(this.m_ColumnNames[i]) : EditorGUIUtility.IconContent("ProfilerColumn." + columnsToShow[i].ToString(), this.m_ColumnNames[i]));
			}
			if (columnsToShow.Length != columnNames.Length)
			{
				throw new ArgumentException("Number of columns to show does not match number of column names.");
			}
			this.m_SearchHeader = new GUIContent("Search");
			this.m_VisibleColumns = new bool[columnNames.Length];
			for (int j = 0; j < this.m_VisibleColumns.Length; j++)
			{
				this.m_VisibleColumns[j] = true;
			}
			this.m_SearchResults = new ProfilerHierarchyGUI.SearchResults();
			this.m_SearchResults.Init(100);
			this.m_DetailedObjectsView = new ProfilerDetailedObjectsView(detailedObjectsView, this);
			this.m_Window.Repaint();
		}

		public void SetKeyboardFocus()
		{
			this.m_SetKeyboardFocus = true;
		}

		public void SelectFirstRow()
		{
			this.MoveSelection(-999999);
		}

		private void SetupSplitter()
		{
			if (this.m_Splitter == null || this.m_SplitterMinWidths == null)
			{
				this.m_SplitterRelativeSizes = new float[this.m_ColumnNames.Length + 1];
				this.m_SplitterMinWidths = new int[this.m_ColumnNames.Length + 1];
				for (int i = 0; i < this.m_ColumnNames.Length; i++)
				{
					this.m_SplitterMinWidths[i] = (int)ProfilerHierarchyGUI.styles.header.CalcSize(this.m_HeaderContent[i]).x;
					this.m_SplitterRelativeSizes[i] = 70f;
					if (this.m_HeaderContent[i].image != null)
					{
						this.m_SplitterRelativeSizes[i] = 1f;
					}
				}
				this.m_SplitterMinWidths[this.m_ColumnNames.Length] = 16;
				this.m_SplitterRelativeSizes[this.m_ColumnNames.Length] = 0f;
				if (this.m_ColumnsToShow[0] == ProfilerColumn.FunctionName)
				{
					this.m_SplitterRelativeSizes[0] = 400f;
					this.m_SplitterMinWidths[0] = 100;
				}
				this.m_Splitter = new SplitterState(this.m_SplitterRelativeSizes, this.m_SplitterMinWidths, null);
				string @string = EditorPrefs.GetString(this.m_ColumnSettingsName);
				for (int j = 0; j < this.m_ColumnNames.Length; j++)
				{
					if (j < @string.Length && @string[j] == '0')
					{
						this.SetColumnVisible(j, false);
					}
				}
			}
		}

		public ProfilerProperty GetDetailedProperty()
		{
			ProfilerProperty rootProfilerProperty = this.m_Window.GetRootProfilerProperty(this.sortType);
			bool enterChildren = true;
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
			ProfilerProperty result;
			while (rootProfilerProperty.Next(enterChildren))
			{
				string propertyPath = rootProfilerProperty.propertyPath;
				if (propertyPath == selectedPropertyPath)
				{
					ProfilerProperty profilerProperty = new ProfilerProperty();
					profilerProperty.InitializeDetailProperty(rootProfilerProperty);
					result = profilerProperty;
					return result;
				}
				if (rootProfilerProperty.HasChildren)
				{
					enterChildren = this.IsExpanded(propertyPath);
				}
			}
			result = null;
			return result;
		}

		public void ClearCaches()
		{
			if (this.m_DetailedObjectsView != null)
			{
				this.m_DetailedObjectsView.ClearCache();
			}
		}

		private void DoScroll()
		{
			this.m_DoScroll = 2;
		}

		public void FrameSelection()
		{
			if (!string.IsNullOrEmpty(ProfilerDriver.selectedPropertyPath))
			{
				this.m_Window.SetSearch(string.Empty);
				string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
				string[] array = selectedPropertyPath.Split(new char[]
				{
					'/'
				});
				string text = array[0];
				for (int i = 1; i < array.Length; i++)
				{
					this.SetExpanded(text, true);
					text = text + "/" + array[i];
				}
				this.DoScroll();
			}
		}

		private void MoveSelection(int steps)
		{
			if (this.IsSearchActive())
			{
				this.m_SearchResults.MoveSelection(steps, this);
			}
			else
			{
				int num = this.m_SelectedIndex + steps;
				if (num < 0)
				{
					num = 0;
				}
				ProfilerProperty profilerProperty = (!this.m_DetailPane) ? this.m_Window.GetRootProfilerProperty(this.m_SortType) : this.GetDetailedProperty();
				if (profilerProperty != null)
				{
					bool enterChildren = true;
					int num2 = 0;
					int instanceId = -1;
					while (profilerProperty.Next(enterChildren))
					{
						if (this.m_DetailPane && profilerProperty.instanceIDs != null && profilerProperty.instanceIDs.Length > 0 && profilerProperty.instanceIDs[0] != 0)
						{
							instanceId = profilerProperty.instanceIDs[0];
						}
						if (num2 == num)
						{
							break;
						}
						if (profilerProperty.HasChildren)
						{
							enterChildren = (!this.m_DetailPane && this.IsExpanded(profilerProperty.propertyPath));
						}
						num2++;
					}
					if (this.m_DetailPane)
					{
						this.m_DetailViewSelectedProperty = ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(profilerProperty, instanceId);
					}
					else
					{
						this.m_Window.SetSelectedPropertyPath(profilerProperty.propertyPath);
					}
				}
			}
		}

		private void SetExpanded(string expandedName, bool expanded)
		{
			if (expanded != this.IsExpanded(expandedName))
			{
				if (expanded)
				{
					this.m_ExpandedHash.Set(expandedName);
				}
				else
				{
					this.m_ExpandedHash.Remove(expandedName);
				}
			}
		}

		private void HandleKeyboard(int id)
		{
			Event current = Event.current;
			if (current.GetTypeForControl(id) == EventType.KeyDown && id == GUIUtility.keyboardControl)
			{
				bool flag = this.IsSearchActive();
				int num = 0;
				switch (current.keyCode)
				{
				case KeyCode.UpArrow:
					num = -1;
					goto IL_174;
				case KeyCode.DownArrow:
					num = 1;
					goto IL_174;
				case KeyCode.RightArrow:
					if (!flag)
					{
						this.SetExpanded(ProfilerDriver.selectedPropertyPath, true);
					}
					goto IL_174;
				case KeyCode.LeftArrow:
					if (!flag)
					{
						this.SetExpanded(ProfilerDriver.selectedPropertyPath, false);
					}
					goto IL_174;
				case KeyCode.Home:
					num = -999999;
					goto IL_174;
				case KeyCode.End:
					num = 999999;
					goto IL_174;
				case KeyCode.PageUp:
					if (Application.platform != RuntimePlatform.OSXEditor)
					{
						num = -Mathf.RoundToInt((float)this.m_ScrollViewHeight / 16f);
						goto IL_174;
					}
					this.m_TextScroll.y = this.m_TextScroll.y - (float)this.m_ScrollViewHeight;
					if (this.m_TextScroll.y < 0f)
					{
						this.m_TextScroll.y = 0f;
					}
					current.Use();
					break;
				case KeyCode.PageDown:
					if (Application.platform != RuntimePlatform.OSXEditor)
					{
						num = Mathf.RoundToInt((float)this.m_ScrollViewHeight / 16f);
						goto IL_174;
					}
					this.m_TextScroll.y = this.m_TextScroll.y + (float)this.m_ScrollViewHeight;
					current.Use();
					break;
				}
				return;
				IL_174:
				if (num != 0)
				{
					this.MoveSelection(num);
				}
				this.DoScroll();
				current.Use();
			}
		}

		private bool IsSearchActive()
		{
			return this.m_Window.IsSearching();
		}

		private void DrawColumnsHeader(string searchString)
		{
			bool flag = false;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				flag = true;
				Event.current.type = EventType.Used;
			}
			SplitterGUILayout.BeginHorizontalSplit(this.m_Splitter, GUIStyle.none, new GUILayoutOption[0]);
			this.DrawTitle((!this.IsSearchActive()) ? this.m_HeaderContent[0] : this.m_SearchHeader, 0);
			for (int i = 1; i < this.m_ColumnNames.Length; i++)
			{
				this.DrawTitle(this.m_HeaderContent[i], i);
			}
			SplitterGUILayout.EndHorizontalSplit();
			GUILayout.EndHorizontal();
			if (flag)
			{
				Event.current.type = EventType.MouseDown;
				this.HandleHeaderMouse(GUILayoutUtility.GetLastRect());
			}
			GUILayout.Space(1f);
		}

		private bool IsExpanded(string expanded)
		{
			return this.m_ExpandAll || this.m_ExpandedHash.Contains(expanded);
		}

		private void SetExpanded(ProfilerProperty property, bool expanded)
		{
			this.SetExpanded(property.propertyPath, expanded);
		}

		private int DrawProfilingData(ProfilerProperty property, string searchString, int id)
		{
			int num;
			if (this.IsSearchActive())
			{
				num = this.DrawSearchResult(property, searchString, id);
			}
			else
			{
				num = this.DrawTreeView(property, id);
			}
			if (num == 0)
			{
				Rect rowRect = this.GetRowRect(0);
				rowRect.height = 1f;
				GUI.Label(rowRect, GUIContent.none, ProfilerHierarchyGUI.styles.entryEven);
			}
			return num;
		}

		private int DrawSearchResult(ProfilerProperty property, string searchString, int id)
		{
			int result;
			if (!this.AllowSearching())
			{
				this.DoSearchingDisabledInfoGUI();
				result = 0;
			}
			else
			{
				this.m_SearchResults.Filter(property, this.m_ColumnsToShow, searchString, this.m_Window.GetActiveVisibleFrameIndex(), this.sortType);
				this.m_SearchResults.Draw(this, id);
				result = this.m_SearchResults.numRows;
			}
			return result;
		}

		private int DrawTreeView(ProfilerProperty property, int id)
		{
			this.m_SelectedIndex = -1;
			bool enterChildren = true;
			int num = 0;
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
			while (property.Next(enterChildren))
			{
				string propertyPath = property.propertyPath;
				bool flag = (!this.m_DetailPane) ? (propertyPath == selectedPropertyPath) : (this.m_DetailViewSelectedProperty != string.Empty && this.m_DetailViewSelectedProperty == ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(property));
				if (flag)
				{
					this.m_SelectedIndex = num;
				}
				bool flag2 = Event.current.type != EventType.Layout;
				flag2 &= (this.m_ScrollViewHeight == 0 || ((float)num * 16f <= (float)this.m_ScrollViewHeight + this.m_TextScroll.y && (float)(num + 1) * 16f > this.m_TextScroll.y));
				if (flag2)
				{
					enterChildren = this.DrawProfileDataItem(property, num, flag, id);
				}
				else
				{
					enterChildren = (property.HasChildren && this.IsExpanded(propertyPath));
				}
				num++;
			}
			return num;
		}

		private void DoSearchingDisabledInfoGUI()
		{
			using (new EditorGUI.DisabledScope(true))
			{
				TextAnchor alignment = EditorStyles.label.alignment;
				EditorStyles.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(0f, 10f, GUIClip.visibleRect.width, 30f), ProfilerHierarchyGUI.styles.disabledSearchText, EditorStyles.label);
				EditorStyles.label.alignment = alignment;
			}
		}

		private bool AllowSearching()
		{
			bool flag = Profiler.enabled && (ProfilerDriver.profileEditor || EditorApplication.isPlaying);
			return !flag || !ProfilerDriver.deepProfiling;
		}

		private void UnselectIfClickedOnEmptyArea(int rowCount)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIClip.visibleRect.width, 16f * (float)rowCount, new GUILayoutOption[]
			{
				GUILayout.MinHeight(16f * (float)rowCount)
			});
			if (Event.current.type == EventType.MouseDown && Event.current.mousePosition.y > rect.y && Event.current.mousePosition.y < (float)Screen.height)
			{
				if (!this.m_DetailPane)
				{
					this.m_Window.ClearSelectedPropertyPath();
				}
				else
				{
					this.m_DetailViewSelectedProperty = string.Empty;
				}
				Event.current.Use();
			}
		}

		private void HandleHeaderMouse(Rect columnHeaderRect)
		{
			Event current = Event.current;
			if (current.type == EventType.MouseDown && current.button == 1 && columnHeaderRect.Contains(current.mousePosition))
			{
				GUIUtility.hotControl = 0;
				Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
				EditorUtility.DisplayCustomMenu(position, this.m_ColumnNames, this.GetVisibleDropDownIndexList(), new EditorUtility.SelectMenuItemFunction(this.ColumnContextClick), null);
				current.Use();
			}
		}

		private void SetColumnVisible(int index, bool enabled)
		{
			this.SetupSplitter();
			if (index != 0)
			{
				if (this.m_VisibleColumns[index] != enabled)
				{
					this.m_VisibleColumns[index] = enabled;
					int num = 0;
					for (int i = 0; i < index; i++)
					{
						if (this.ColIsVisible(i))
						{
							num++;
						}
					}
					if (enabled)
					{
						ArrayUtility.Insert<float>(ref this.m_Splitter.relativeSizes, num, this.m_SplitterRelativeSizes[index]);
						ArrayUtility.Insert<int>(ref this.m_Splitter.minSizes, num, this.m_SplitterMinWidths[index]);
					}
					else
					{
						ArrayUtility.RemoveAt<float>(ref this.m_Splitter.relativeSizes, num);
						ArrayUtility.RemoveAt<int>(ref this.m_Splitter.minSizes, num);
					}
					this.m_Splitter = new SplitterState(this.m_Splitter.relativeSizes, this.m_Splitter.minSizes, null);
					this.SaveColumns();
				}
			}
		}

		private int[] GetVisibleDropDownIndexList()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.m_ColumnNames.Length; i++)
			{
				if (this.m_VisibleColumns[i])
				{
					list.Add(i);
				}
			}
			return list.ToArray();
		}

		private void SaveColumns()
		{
			string text = string.Empty;
			for (int i = 0; i < this.m_VisibleColumns.Length; i++)
			{
				text += ((!this.ColIsVisible(i)) ? '0' : '1');
			}
			EditorPrefs.SetString(this.m_ColumnSettingsName, text);
		}

		private bool ColIsVisible(int index)
		{
			return index >= 0 && index <= this.m_VisibleColumns.Length && this.m_VisibleColumns[index];
		}

		private void ColumnContextClick(object userData, string[] options, int selected)
		{
			this.SetColumnVisible(selected, !this.ColIsVisible(selected));
		}

		protected void DrawTextColumn(ref Rect currentRect, string text, int index, float margin, bool selected)
		{
			if (index != 0)
			{
				currentRect.x += (float)this.m_Splitter.realSizes[index - 1];
			}
			currentRect.x += margin;
			currentRect.width = (float)this.m_Splitter.realSizes[index] - margin;
			ProfilerHierarchyGUI.styles.numberLabel.Draw(currentRect, text, false, false, false, selected);
			currentRect.x -= margin;
		}

		private static string DetailViewSelectedPropertyPath(ProfilerProperty property)
		{
			string result;
			if (property == null || property.instanceIDs == null || property.instanceIDs.Length == 0 || property.instanceIDs[0] == 0)
			{
				result = string.Empty;
			}
			else
			{
				result = ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(property, property.instanceIDs[0]);
			}
			return result;
		}

		private static string DetailViewSelectedPropertyPath(ProfilerProperty property, int instanceId)
		{
			return property.propertyPath + "/" + instanceId;
		}

		private Rect GetRowRect(int rowIndex)
		{
			return new Rect(1f, 16f * (float)rowIndex, GUIClip.visibleRect.width, 16f);
		}

		private GUIStyle GetRowBackgroundStyle(int rowIndex)
		{
			return (rowIndex % 2 != 0) ? ProfilerHierarchyGUI.styles.entryOdd : ProfilerHierarchyGUI.styles.entryEven;
		}

		private void RowMouseDown(string propertyPath)
		{
			if (propertyPath == ProfilerDriver.selectedPropertyPath)
			{
				this.m_Window.ClearSelectedPropertyPath();
			}
			else
			{
				this.m_Window.SetSelectedPropertyPath(propertyPath);
			}
		}

		private bool DrawProfileDataItem(ProfilerProperty property, int rowCount, bool selected, int id)
		{
			bool flag = false;
			Event current = Event.current;
			Rect rowRect = this.GetRowRect(rowCount);
			Rect position = rowRect;
			GUIStyle rowBackgroundStyle = this.GetRowBackgroundStyle(rowCount);
			if (current.type == EventType.Repaint)
			{
				rowBackgroundStyle.Draw(position, GUIContent.none, false, false, selected, false);
			}
			float num = (float)property.depth * 16f + 4f;
			if (property.HasChildren)
			{
				flag = this.IsExpanded(property.propertyPath);
				GUI.changed = false;
				num -= 14f;
				Rect position2 = new Rect(num, position.y, 14f, 16f);
				flag = GUI.Toggle(position2, flag, GUIContent.none, ProfilerHierarchyGUI.styles.foldout);
				if (GUI.changed)
				{
					this.SetExpanded(property, flag);
				}
				num += 16f;
			}
			string column = property.GetColumn(this.m_ColumnsToShow[0]);
			if (current.type == EventType.Repaint)
			{
				this.DrawTextColumn(ref position, column, 0, (this.m_ColumnsToShow[0] != ProfilerColumn.FunctionName) ? 4f : num, selected);
			}
			if (ProfilerInstrumentationPopup.InstrumentationEnabled)
			{
				if (ProfilerInstrumentationPopup.FunctionHasInstrumentationPopup(column))
				{
					float num2 = position.x + num + 5f + ProfilerHierarchyGUI.styles.numberLabel.CalcSize(new GUIContent(column)).x;
					num2 = Mathf.Clamp(num2, 0f, (float)this.m_Splitter.realSizes[0] - 30f + 2f);
					Rect rect = new Rect(num2, position.y, 30f, 16f);
					if (GUI.Button(rect, ProfilerHierarchyGUI.styles.instrumentationIcon, ProfilerHierarchyGUI.styles.miniPullDown))
					{
						ProfilerInstrumentationPopup.Show(rect, column);
					}
				}
			}
			if (current.type == EventType.Repaint)
			{
				ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleRight;
				int num3 = 1;
				for (int i = 1; i < this.m_VisibleColumns.Length; i++)
				{
					if (this.ColIsVisible(i))
					{
						position.x += (float)this.m_Splitter.realSizes[num3 - 1];
						position.width = (float)this.m_Splitter.realSizes[num3] - 4f;
						num3++;
						ProfilerHierarchyGUI.styles.numberLabel.Draw(position, property.GetColumn(this.m_ColumnsToShow[i]), false, false, false, selected);
					}
				}
				ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleLeft;
			}
			if (current.type == EventType.MouseDown)
			{
				if (rowRect.Contains(current.mousePosition))
				{
					GUIUtility.hotControl = 0;
					if (!EditorGUI.actionKey)
					{
						if (this.m_DetailPane)
						{
							if (current.clickCount == 1 && property.instanceIDs.Length > 0)
							{
								string text = ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(property);
								if (this.m_DetailViewSelectedProperty != text)
								{
									this.m_DetailViewSelectedProperty = text;
									UnityEngine.Object @object = EditorUtility.InstanceIDToObject(property.instanceIDs[0]);
									if (@object is Component)
									{
										@object = ((Component)@object).gameObject;
									}
									if (@object != null)
									{
										EditorGUIUtility.PingObject(@object.GetInstanceID());
									}
								}
								else
								{
									this.m_DetailViewSelectedProperty = string.Empty;
								}
							}
							else if (current.clickCount == 2)
							{
								ProfilerHierarchyGUI.SelectObjectsInHierarchyView(property);
								this.m_DetailViewSelectedProperty = ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(property);
							}
						}
						else
						{
							this.RowMouseDown(property.propertyPath);
						}
						this.DoScroll();
					}
					else if (!this.m_DetailPane)
					{
						this.m_Window.ClearSelectedPropertyPath();
					}
					else
					{
						this.m_DetailViewSelectedProperty = string.Empty;
					}
					GUIUtility.keyboardControl = id;
					current.Use();
				}
			}
			if (selected && GUIUtility.keyboardControl == id && current.type == EventType.KeyDown && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter))
			{
				ProfilerHierarchyGUI.SelectObjectsInHierarchyView(property);
			}
			return flag;
		}

		private static void SelectObjectsInHierarchyView(ProfilerProperty property)
		{
			int[] instanceIDs = property.instanceIDs;
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			int[] array = instanceIDs;
			for (int i = 0; i < array.Length; i++)
			{
				int instanceID = array[i];
				UnityEngine.Object @object = EditorUtility.InstanceIDToObject(instanceID);
				Component component = @object as Component;
				if (component != null)
				{
					list.Add(component.gameObject);
				}
				else if (@object != null)
				{
					list.Add(@object);
				}
			}
			if (list.Count != 0)
			{
				Selection.objects = list.ToArray();
			}
		}

		private void DrawTitle(GUIContent name, int index)
		{
			if (this.ColIsVisible(index))
			{
				ProfilerColumn profilerColumn = this.m_ColumnsToShow[index];
				bool value = this.sortType == profilerColumn;
				bool flag = (index != 0) ? GUILayout.Toggle(value, name, ProfilerHierarchyGUI.styles.rightHeader, new GUILayoutOption[]
				{
					GUILayout.Width((float)this.m_SplitterMinWidths[index])
				}) : GUILayout.Toggle(value, name, ProfilerHierarchyGUI.styles.header, new GUILayoutOption[0]);
				if (flag)
				{
					this.sortType = profilerColumn;
				}
			}
		}

		private void DoScrolling()
		{
			if (this.m_DoScroll > 0)
			{
				this.m_DoScroll--;
				if (this.m_DoScroll == 0)
				{
					float num = 16f * (float)this.selectedIndex;
					float min = num - (float)this.m_ScrollViewHeight + 16f;
					this.m_TextScroll.y = Mathf.Clamp(this.m_TextScroll.y, min, num);
				}
				else
				{
					this.m_Window.Repaint();
				}
			}
		}

		public void DoGUI(ProfilerProperty property, string searchString, bool expandAll)
		{
			this.m_ExpandAll = expandAll;
			this.SetupSplitter();
			this.DoScrolling();
			int controlID = GUIUtility.GetControlID(ProfilerHierarchyGUI.hierarchyViewHash, FocusType.Keyboard);
			this.DrawColumnsHeader(searchString);
			this.m_TextScroll = EditorGUILayout.BeginScrollView(this.m_TextScroll, ProfilerHierarchyGUI.ms_Styles.background, new GUILayoutOption[0]);
			int rowCount = this.DrawProfilingData(property, searchString, controlID);
			this.UnselectIfClickedOnEmptyArea(rowCount);
			if (Event.current.type == EventType.Repaint)
			{
				this.m_ScrollViewHeight = (int)GUIClip.visibleRect.height;
			}
			EditorGUILayout.EndScrollView();
			this.HandleKeyboard(controlID);
			if (this.m_SetKeyboardFocus && Event.current.type == EventType.Repaint)
			{
				this.m_SetKeyboardFocus = false;
				GUIUtility.keyboardControl = controlID;
				this.m_Window.Repaint();
			}
		}
	}
}
