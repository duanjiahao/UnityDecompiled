using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
		}
		private const float kRowHeight = 16f;
		private const float kFoldoutSize = 14f;
		private const float kIndent = 16f;
		private const float kSmallMargin = 4f;
		private const float kBaseIndent = 4f;
		private const int kFirst = -999999;
		private const int kLast = 999999;
		private const float kScrollbarWidth = 16f;
		private static int hierarchyViewHash = "HierarchyView".GetHashCode();
		private static ProfilerHierarchyGUI.Styles ms_Styles;
		private IProfilerWindowController m_Window;
		private SplitterState m_Splitter;
		private ProfilerColumn[] m_ColumnsToShow;
		private string[] m_ColumnNames;
		private bool[] m_VisibleColumns;
		private float[] m_SplitterRelativeSizes;
		private int[] m_SplitterMinWidths;
		private string m_ColumnSettingsName;
		private Vector2 m_TextScroll = Vector2.zero;
		private GUIContent[] m_HeaderContent;
		private SerializedStringTable m_ExpandedHash = new SerializedStringTable();
		private bool m_ExpandAll;
		private int m_ScrollViewHeight;
		private int m_DoScroll;
		private int m_SelectedIndex = -1;
		private bool m_DetailPane;
		private ProfilerColumn m_SortType = ProfilerColumn.TotalTime;
		private string m_DetailViewSelectedProperty = string.Empty;
		private static ProfilerHierarchyGUI.Styles styles
		{
			get
			{
				ProfilerHierarchyGUI.Styles arg_17_0;
				if ((arg_17_0 = ProfilerHierarchyGUI.ms_Styles) == null)
				{
					arg_17_0 = (ProfilerHierarchyGUI.ms_Styles = new ProfilerHierarchyGUI.Styles());
				}
				return arg_17_0;
			}
		}
		public int selectedIndex
		{
			get
			{
				return this.m_SelectedIndex;
			}
			set
			{
				this.m_SelectedIndex = value;
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
		public ProfilerHierarchyGUI(IProfilerWindowController window, string columnSettingsName, ProfilerColumn[] columnsToShow, string[] columnNames, bool detailPane, ProfilerColumn sort)
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
				this.m_HeaderContent[i] = EditorGUIUtility.TextContent(this.m_ColumnNames[i]);
			}
			if (columnsToShow.Length != columnNames.Length)
			{
				throw new ArgumentException("Number of columns to show does not match number of column names.");
			}
			this.m_VisibleColumns = new bool[columnNames.Length];
			for (int j = 0; j < this.m_VisibleColumns.Length; j++)
			{
				this.m_VisibleColumns[j] = true;
			}
		}
		private void SetupSplitter()
		{
			if (this.m_Splitter != null && this.m_SplitterMinWidths != null)
			{
				return;
			}
			this.m_SplitterRelativeSizes = new float[this.m_ColumnNames.Length];
			this.m_SplitterMinWidths = new int[this.m_ColumnNames.Length];
			for (int i = 0; i < this.m_SplitterRelativeSizes.Length; i++)
			{
				this.m_SplitterMinWidths[i] = (int)ProfilerHierarchyGUI.styles.header.CalcSize(this.m_HeaderContent[i]).x;
				this.m_SplitterRelativeSizes[i] = 70f;
				if (this.m_HeaderContent[i].image != null)
				{
					this.m_SplitterRelativeSizes[i] = 1f;
				}
			}
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
		public ProfilerProperty GetDetailedProperty(ProfilerProperty property)
		{
			bool enterChildren = true;
			string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
			while (property.Next(enterChildren))
			{
				string propertyPath = property.propertyPath;
				if (propertyPath == selectedPropertyPath)
				{
					ProfilerProperty profilerProperty = new ProfilerProperty();
					profilerProperty.InitializeDetailProperty(property);
					return profilerProperty;
				}
				if (property.HasChildren)
				{
					enterChildren = this.IsExpanded(propertyPath);
				}
			}
			return null;
		}
		private void DoScroll()
		{
			this.m_DoScroll = 2;
		}
		private void MoveSelection(int steps)
		{
			int num = this.m_SelectedIndex + steps;
			if (num < 0)
			{
				num = 0;
			}
			ProfilerProperty profilerProperty = this.m_Window.CreateProperty(this.m_DetailPane);
			if (this.m_DetailPane)
			{
				ProfilerProperty detailedProperty = this.GetDetailedProperty(profilerProperty);
				profilerProperty.Cleanup();
				profilerProperty = detailedProperty;
			}
			if (profilerProperty == null)
			{
				return;
			}
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
			profilerProperty.Cleanup();
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
			if (current.GetTypeForControl(id) != EventType.KeyDown || id != GUIUtility.keyboardControl)
			{
				return;
			}
			switch (current.keyCode)
			{
			case KeyCode.UpArrow:
				this.MoveSelection(-1);
				goto IL_16A;
			case KeyCode.DownArrow:
				this.MoveSelection(1);
				goto IL_16A;
			case KeyCode.RightArrow:
				this.SetExpanded(ProfilerDriver.selectedPropertyPath, true);
				goto IL_16A;
			case KeyCode.LeftArrow:
				this.SetExpanded(ProfilerDriver.selectedPropertyPath, false);
				goto IL_16A;
			case KeyCode.Home:
				this.MoveSelection(-999999);
				goto IL_16A;
			case KeyCode.End:
				this.MoveSelection(999999);
				goto IL_16A;
			case KeyCode.PageUp:
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					this.m_TextScroll.y = this.m_TextScroll.y - (float)this.m_ScrollViewHeight;
					if (this.m_TextScroll.y < 0f)
					{
						this.m_TextScroll.y = 0f;
					}
					current.Use();
					return;
				}
				this.MoveSelection(-Mathf.RoundToInt((float)this.m_ScrollViewHeight / 16f));
				goto IL_16A;
			case KeyCode.PageDown:
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					this.m_TextScroll.y = this.m_TextScroll.y + (float)this.m_ScrollViewHeight;
					current.Use();
					return;
				}
				this.MoveSelection(Mathf.RoundToInt((float)this.m_ScrollViewHeight / 16f));
				goto IL_16A;
			}
			return;
			IL_16A:
			this.DoScroll();
			current.Use();
		}
		private void DrawColumnsHeader()
		{
			bool flag = false;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				flag = true;
				Event.current.type = EventType.Used;
			}
			SplitterGUILayout.BeginHorizontalSplit(this.m_Splitter, EditorStyles.toolbar, new GUILayoutOption[0]);
			for (int i = 0; i < this.m_ColumnNames.Length; i++)
			{
				this.DrawTitle(this.m_HeaderContent[i], i);
			}
			SplitterGUILayout.EndHorizontalSplit();
			GUILayout.Box(string.Empty, ProfilerHierarchyGUI.ms_Styles.header, new GUILayoutOption[]
			{
				GUILayout.Width(16f)
			});
			GUILayout.EndHorizontal();
			if (flag)
			{
				Event.current.type = EventType.MouseDown;
				this.HandleHeaderMouse(GUILayoutUtility.GetLastRect());
			}
		}
		private bool IsExpanded(string expanded)
		{
			return this.m_ExpandAll || this.m_ExpandedHash.Contains(expanded);
		}
		private void SetExpanded(ProfilerProperty property, bool expanded)
		{
			this.SetExpanded(property.propertyPath, expanded);
		}
		private int DrawProfilingData(ProfilerProperty property, int id)
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
		private void UnselectIfClickedOnEmptyArea(int rowCount)
		{
			Rect rect = GUILayoutUtility.GetRect(GUIClip.visibleRect.width, 16f * (float)rowCount, new GUILayoutOption[]
			{
				GUILayout.MinHeight(16f * (float)rowCount)
			});
			if (Event.current.type != EventType.MouseDown || Event.current.mousePosition.y <= rect.y || Event.current.mousePosition.y >= (float)Screen.height)
			{
				return;
			}
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
		private void HandleHeaderMouse(Rect columnHeaderRect)
		{
			Event current = Event.current;
			if (current.type != EventType.MouseDown || current.button != 1 || !columnHeaderRect.Contains(current.mousePosition))
			{
				return;
			}
			GUIUtility.hotControl = 0;
			Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
			EditorUtility.DisplayCustomMenu(position, this.m_ColumnNames, this.GetVisibleDropDownIndexList(), new EditorUtility.SelectMenuItemFunction(this.ColumnContextClick), null);
			current.Use();
		}
		private void SetColumnVisible(int index, bool enabled)
		{
			this.SetupSplitter();
			if (index == 0)
			{
				return;
			}
			if (this.m_VisibleColumns[index] == enabled)
			{
				return;
			}
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
		private void DrawTextColumn(ref Rect currentRect, string text, int index, float margin, bool selected)
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
			if (property == null || property.instanceIDs == null || property.instanceIDs.Length == 0 || property.instanceIDs[0] == 0)
			{
				return string.Empty;
			}
			return ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(property, property.instanceIDs[0]);
		}
		private static string DetailViewSelectedPropertyPath(ProfilerProperty property, int instanceId)
		{
			return property.propertyPath + "/" + instanceId;
		}
		private bool DrawProfileDataItem(ProfilerProperty property, int rowCount, bool selected, int id)
		{
			bool flag = false;
			Event current = Event.current;
			Rect rect = new Rect(1f, 16f * (float)rowCount, GUIClip.visibleRect.width, 16f);
			Rect position = rect;
			GUIStyle gUIStyle = (rowCount % 2 != 0) ? ProfilerHierarchyGUI.styles.entryOdd : ProfilerHierarchyGUI.styles.entryEven;
			if (current.type == EventType.Repaint)
			{
				gUIStyle.Draw(position, GUIContent.none, false, false, selected, false);
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
			if (current.type == EventType.Repaint)
			{
				this.DrawTextColumn(ref position, property.GetColumn(this.m_ColumnsToShow[0]), 0, (this.m_ColumnsToShow[0] != ProfilerColumn.FunctionName) ? 4f : num, selected);
				ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleRight;
				int num2 = 1;
				for (int i = 1; i < this.m_VisibleColumns.Length; i++)
				{
					if (this.ColIsVisible(i))
					{
						position.x += (float)this.m_Splitter.realSizes[num2 - 1];
						position.width = (float)this.m_Splitter.realSizes[num2] - 4f;
						num2++;
						ProfilerHierarchyGUI.styles.numberLabel.Draw(position, property.GetColumn(this.m_ColumnsToShow[i]), false, false, false, selected);
					}
				}
				ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleLeft;
			}
			if (current.type == EventType.MouseDown && rect.Contains(current.mousePosition))
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
								EditorGUIUtility.PingObject(@object.GetInstanceID());
							}
							else
							{
								this.m_DetailViewSelectedProperty = string.Empty;
							}
						}
						else
						{
							if (current.clickCount == 2)
							{
								ProfilerHierarchyGUI.SelectObjectsInHierarchyView(property);
								this.m_DetailViewSelectedProperty = ProfilerHierarchyGUI.DetailViewSelectedPropertyPath(property);
							}
						}
					}
					else
					{
						if (property.propertyPath == ProfilerDriver.selectedPropertyPath)
						{
							this.m_Window.ClearSelectedPropertyPath();
						}
						else
						{
							this.m_Window.SetSelectedPropertyPath(property.propertyPath);
						}
					}
					this.DoScroll();
				}
				else
				{
					if (!this.m_DetailPane)
					{
						this.m_Window.ClearSelectedPropertyPath();
					}
					else
					{
						this.m_DetailViewSelectedProperty = string.Empty;
					}
				}
				GUIUtility.keyboardControl = id;
				current.Use();
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
				else
				{
					if (@object != null)
					{
						list.Add(@object);
					}
				}
			}
			if (list.Count != 0)
			{
				Selection.objects = list.ToArray();
			}
		}
		private void DrawTitle(GUIContent name, int index)
		{
			if (!this.ColIsVisible(index))
			{
				return;
			}
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
		private void DoScrolling()
		{
			if (this.m_DoScroll > 0)
			{
				this.m_DoScroll--;
				if (this.m_DoScroll == 0)
				{
					float num = 16f * (float)this.m_SelectedIndex;
					float min = num - (float)this.m_ScrollViewHeight + 16f;
					this.m_TextScroll.y = Mathf.Clamp(this.m_TextScroll.y, min, num);
				}
				else
				{
					this.m_Window.Repaint();
				}
			}
		}
		public void DoGUI(ProfilerProperty property, bool expandAll)
		{
			this.m_ExpandAll = expandAll;
			this.SetupSplitter();
			this.DoScrolling();
			int controlID = GUIUtility.GetControlID(ProfilerHierarchyGUI.hierarchyViewHash, FocusType.Passive);
			this.DrawColumnsHeader();
			this.m_TextScroll = EditorGUILayout.BeginScrollView(this.m_TextScroll, ProfilerHierarchyGUI.ms_Styles.background, new GUILayoutOption[0]);
			int rowCount = this.DrawProfilingData(property, controlID);
			property.Cleanup();
			this.UnselectIfClickedOnEmptyArea(rowCount);
			if (Event.current.type == EventType.Repaint)
			{
				this.m_ScrollViewHeight = (int)GUIClip.visibleRect.height;
			}
			GUILayout.EndScrollView();
			this.HandleKeyboard(controlID);
		}
	}
}
