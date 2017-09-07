using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ColumnView
	{
		public class Styles
		{
			public GUIStyle background = "OL Box";

			public GUIStyle selected = "PR Label";

			public Texture2D categoryArrowIcon = EditorStyles.foldout.normal.background;
		}

		public delegate void ObjectColumnFunction(object value);

		public delegate object ObjectColumnGetDataFunction(object value);

		private static ColumnView.Styles s_Styles;

		private readonly List<ListViewState> m_ListViewStates;

		private readonly List<int> m_CachedSelectedIndices;

		private Vector2 m_ScrollPosition;

		private string m_SearchText = string.Empty;

		public float columnWidth = 150f;

		public int minimumNumberOfColumns = 1;

		private int m_ColumnToFocusKeyboard = -1;

		public string searchText
		{
			get
			{
				return this.m_SearchText;
			}
		}

		public bool isSearching
		{
			get
			{
				return this.searchText != string.Empty;
			}
		}

		public ColumnView()
		{
			this.m_ListViewStates = new List<ListViewState>();
			this.m_CachedSelectedIndices = new List<int>();
		}

		private static void InitStyles()
		{
			if (ColumnView.s_Styles == null)
			{
				ColumnView.s_Styles = new ColumnView.Styles();
			}
		}

		public void SetSelected(int column, int selectionIndex)
		{
			if (this.m_ListViewStates.Count == column)
			{
				this.m_ListViewStates.Add(new ListViewState());
			}
			if (this.m_CachedSelectedIndices.Count == column)
			{
				this.m_CachedSelectedIndices.Add(-1);
			}
			this.m_CachedSelectedIndices[column] = selectionIndex;
			this.m_ListViewStates[column].row = selectionIndex;
		}

		public void SetKeyboardFocusColumn(int column)
		{
			this.m_ColumnToFocusKeyboard = column;
		}

		public void OnGUI(List<ColumnViewElement> elements, ColumnView.ObjectColumnFunction previewColumnFunction)
		{
			this.OnGUI(elements, previewColumnFunction, null, null, null);
		}

		public void OnGUI(List<ColumnViewElement> elements, ColumnView.ObjectColumnFunction previewColumnFunction, ColumnView.ObjectColumnFunction selectedSearchItemFunction, ColumnView.ObjectColumnFunction selectedRegularItemFunction, ColumnView.ObjectColumnGetDataFunction getDataForDraggingFunction)
		{
			ColumnView.InitStyles();
			this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			List<ColumnViewElement> list = elements;
			int i = 0;
			object obj;
			do
			{
				if (this.m_ListViewStates.Count == i)
				{
					this.m_ListViewStates.Add(new ListViewState());
				}
				if (this.m_CachedSelectedIndices.Count == i)
				{
					this.m_CachedSelectedIndices.Add(-1);
				}
				ListViewState listViewState = this.m_ListViewStates[i];
				listViewState.totalRows = list.Count;
				if (i == 0)
				{
					GUILayout.BeginVertical(new GUILayoutOption[]
					{
						GUILayout.MaxWidth(this.columnWidth)
					});
				}
				int num = this.m_CachedSelectedIndices[i];
				num = this.DoListColumn(listViewState, list, i, num, (i != 0) ? null : selectedSearchItemFunction, selectedRegularItemFunction, getDataForDraggingFunction);
				if (Event.current.type == EventType.Layout && this.m_ColumnToFocusKeyboard == i)
				{
					this.m_ColumnToFocusKeyboard = -1;
					GUIUtility.keyboardControl = listViewState.ID;
					if (listViewState.row == -1 && list.Count != 0)
					{
						num = (listViewState.row = 0);
					}
				}
				if (i == 0)
				{
					if (this.isSearching)
					{
						KeyCode keyCode = ColumnView.StealImportantListviewKeys();
						if (keyCode != KeyCode.None)
						{
							ListViewShared.SendKey(this.m_ListViewStates[0], keyCode);
						}
					}
					this.m_SearchText = EditorGUILayout.ToolbarSearchField(this.m_SearchText, new GUILayoutOption[0]);
					GUILayout.EndVertical();
				}
				if (num >= list.Count)
				{
					num = -1;
				}
				if (Event.current.type == EventType.Layout && this.m_CachedSelectedIndices[i] != num && this.m_ListViewStates.Count > i + 1)
				{
					int index = i + 1;
					int count = this.m_ListViewStates.Count - (i + 1);
					this.m_ListViewStates.RemoveRange(index, count);
					this.m_CachedSelectedIndices.RemoveRange(index, count);
				}
				this.m_CachedSelectedIndices[i] = num;
				obj = ((num <= -1) ? null : list[num].value);
				list = (obj as List<ColumnViewElement>);
				i++;
			}
			while (list != null);
			while (i < this.minimumNumberOfColumns)
			{
				this.DoDummyColumn();
				i++;
			}
			ColumnView.DoPreviewColumn(obj, previewColumnFunction);
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
		}

		private static void DoItemSelectedEvent(ColumnView.ObjectColumnFunction selectedRegularItemFunction, object value)
		{
			if (selectedRegularItemFunction != null)
			{
				selectedRegularItemFunction(value);
			}
			Event.current.Use();
		}

		private void DoSearchItemSelectedEvent(ColumnView.ObjectColumnFunction selectedSearchItemFunction, object value)
		{
			this.m_SearchText = string.Empty;
			ColumnView.DoItemSelectedEvent(selectedSearchItemFunction, value);
		}

		private void DoDummyColumn()
		{
			GUILayout.Box(GUIContent.none, ColumnView.s_Styles.background, new GUILayoutOption[]
			{
				GUILayout.Width(this.columnWidth + 1f)
			});
		}

		private static void DoPreviewColumn(object selectedObject, ColumnView.ObjectColumnFunction previewColumnFunction)
		{
			GUILayout.BeginVertical(ColumnView.s_Styles.background, new GUILayoutOption[0]);
			if (previewColumnFunction != null)
			{
				previewColumnFunction(selectedObject);
			}
			GUILayout.EndVertical();
		}

		private int DoListColumn(ListViewState listView, List<ColumnViewElement> columnViewElements, int columnIndex, int selectedIndex, ColumnView.ObjectColumnFunction selectedSearchItemFunction, ColumnView.ObjectColumnFunction selectedRegularItemFunction, ColumnView.ObjectColumnGetDataFunction getDataForDraggingFunction)
		{
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && listView.row > -1)
			{
				if (this.isSearching && selectedSearchItemFunction != null)
				{
					this.DoSearchItemSelectedEvent(selectedSearchItemFunction, columnViewElements[selectedIndex].value);
				}
				if (!this.isSearching && GUIUtility.keyboardControl == listView.ID && selectedRegularItemFunction != null)
				{
					ColumnView.DoItemSelectedEvent(selectedRegularItemFunction, columnViewElements[selectedIndex].value);
				}
			}
			if (GUIUtility.keyboardControl == listView.ID && Event.current.type == EventType.KeyDown && !this.isSearching)
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode != KeyCode.LeftArrow)
				{
					if (keyCode == KeyCode.RightArrow)
					{
						this.m_ColumnToFocusKeyboard = columnIndex + 1;
						Event.current.Use();
					}
				}
				else
				{
					this.m_ColumnToFocusKeyboard = columnIndex - 1;
					Event.current.Use();
				}
			}
			IEnumerator enumerator = ListViewGUILayout.ListView(listView, ColumnView.s_Styles.background, new GUILayoutOption[]
			{
				GUILayout.Width(this.columnWidth)
			}).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement element = (ListViewElement)enumerator.Current;
					ColumnViewElement columnViewElement = columnViewElements[element.row];
					if (element.row == listView.row)
					{
						if (Event.current.type == EventType.Repaint)
						{
							Rect position = element.position;
							position.x += 1f;
							position.y += 1f;
							ColumnView.s_Styles.selected.Draw(position, false, true, true, GUIUtility.keyboardControl == listView.ID);
						}
					}
					GUILayout.Label(columnViewElement.name, new GUILayoutOption[0]);
					if (columnViewElement.value is List<ColumnViewElement>)
					{
						Rect position2 = element.position;
						position2.x = position2.xMax - (float)ColumnView.s_Styles.categoryArrowIcon.width - 5f;
						position2.y += 2f;
						GUI.Label(position2, ColumnView.s_Styles.categoryArrowIcon);
					}
					this.DoDoubleClick(element, columnViewElement, selectedSearchItemFunction, selectedRegularItemFunction);
					ColumnView.DoDragAndDrop(listView, element, columnViewElements, getDataForDraggingFunction);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (Event.current.type == EventType.Layout)
			{
				selectedIndex = listView.row;
			}
			return selectedIndex;
		}

		private static void DoDragAndDrop(ListViewState listView, ListViewElement element, List<ColumnViewElement> columnViewElements, ColumnView.ObjectColumnGetDataFunction getDataForDraggingFunction)
		{
			if (GUIUtility.hotControl == listView.ID && Event.current.type == EventType.MouseDown && element.position.Contains(Event.current.mousePosition) && Event.current.button == 0)
			{
				DragAndDropDelay dragAndDropDelay = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), listView.ID);
				dragAndDropDelay.mouseDownPosition = Event.current.mousePosition;
			}
			if (GUIUtility.hotControl == listView.ID && Event.current.type == EventType.MouseDrag && GUIClip.visibleRect.Contains(Event.current.mousePosition))
			{
				DragAndDropDelay dragAndDropDelay2 = (DragAndDropDelay)GUIUtility.GetStateObject(typeof(DragAndDropDelay), listView.ID);
				if (dragAndDropDelay2.CanStartDrag())
				{
					object obj = (getDataForDraggingFunction != null) ? getDataForDraggingFunction(columnViewElements[listView.row].value) : null;
					if (obj != null)
					{
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.paths = null;
						DragAndDrop.SetGenericData("CustomDragData", obj);
						DragAndDrop.StartDrag(columnViewElements[listView.row].name);
						Event.current.Use();
					}
				}
			}
		}

		private void DoDoubleClick(ListViewElement element, ColumnViewElement columnViewElement, ColumnView.ObjectColumnFunction selectedSearchItemFunction, ColumnView.ObjectColumnFunction selectedRegularItemFunction)
		{
			if (Event.current.type == EventType.MouseDown && element.position.Contains(Event.current.mousePosition) && Event.current.button == 0 && Event.current.clickCount == 2)
			{
				if (this.isSearching)
				{
					this.DoSearchItemSelectedEvent(selectedSearchItemFunction, columnViewElement.value);
				}
				else
				{
					ColumnView.DoItemSelectedEvent(selectedRegularItemFunction, columnViewElement.value);
				}
			}
		}

		private static KeyCode StealImportantListviewKeys()
		{
			KeyCode result;
			if (Event.current.type == EventType.KeyDown)
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow || keyCode == KeyCode.PageUp || keyCode == KeyCode.PageDown)
				{
					Event.current.Use();
					result = keyCode;
					return result;
				}
			}
			result = KeyCode.None;
			return result;
		}
	}
}
