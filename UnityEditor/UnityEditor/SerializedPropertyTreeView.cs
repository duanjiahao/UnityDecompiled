using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor
{
	internal class SerializedPropertyTreeView : TreeView
	{
		internal static class Styles
		{
			public static readonly GUIStyle entryEven = "OL EntryBackEven";

			public static readonly GUIStyle entryOdd = "OL EntryBackOdd";

			public static readonly string focusHelper = "SerializedPropertyTreeViewFocusHelper";

			public static readonly string serializeFilterSelection = "_FilterSelection";

			public static readonly string serializeFilterDisable = "_FilterDisable";

			public static readonly string serializeFilterInvert = "_FilterInvert";

			public static readonly string serializeTreeViewState = "_TreeViewState";

			public static readonly string serializeColumnHeaderState = "_ColumnHeaderState";

			public static readonly string serializeFilter = "_Filter_";

			public static readonly GUIContent filterSelection = EditorGUIUtility.TextContent("Lock Selection|Limits the table contents to the active selection.");

			public static readonly GUIContent filterDisable = EditorGUIUtility.TextContent("Disable All|Disables all filters.");

			public static readonly GUIContent filterInvert = EditorGUIUtility.TextContent("Invert Result|Inverts the filtered results.");
		}

		internal class SerializedPropertyItem : TreeViewItem
		{
			private SerializedPropertyDataStore.Data m_Data;

			public SerializedPropertyItem(int id, int depth, SerializedPropertyDataStore.Data ltd) : base(id, depth, (ltd == null) ? "root" : ltd.name)
			{
				this.m_Data = ltd;
			}

			public SerializedPropertyDataStore.Data GetData()
			{
				return this.m_Data;
			}
		}

		internal class Column : MultiColumnHeaderState.Column
		{
			public delegate void DrawEntry(Rect r, SerializedProperty prop, SerializedProperty[] dependencies);

			public delegate int CompareEntry(SerializedProperty lhs, SerializedProperty rhs);

			public delegate void CopyDelegate(SerializedProperty target, SerializedProperty source);

			public string propertyName;

			public int[] dependencyIndices;

			public SerializedPropertyTreeView.Column.DrawEntry drawDelegate;

			public SerializedPropertyTreeView.Column.CompareEntry compareDelegate;

			public SerializedPropertyTreeView.Column.CopyDelegate copyDelegate;

			public SerializedPropertyFilters.IFilter filter;
		}

		private struct ColumnInternal
		{
			public SerializedProperty[] dependencyProps;
		}

		internal class DefaultDelegates
		{
			public static readonly SerializedPropertyTreeView.Column.DrawEntry s_DrawDefault = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dependencies)
			{
				Profiler.BeginSample("PropDrawDefault");
				EditorGUI.PropertyField(r, prop, GUIContent.none);
				Profiler.EndSample();
			};

			public static readonly SerializedPropertyTreeView.Column.DrawEntry s_DrawCheckbox = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dependencies)
			{
				Profiler.BeginSample("PropDrawCheckbox");
				float num = r.width / 2f - 8f;
				r.x += ((num < 0f) ? 0f : num);
				EditorGUI.PropertyField(r, prop, GUIContent.none);
				Profiler.EndSample();
			};

			public static readonly SerializedPropertyTreeView.Column.DrawEntry s_DrawName = delegate(Rect r, SerializedProperty prop, SerializedProperty[] dependencies)
			{
			};

			public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareFloat = (SerializedProperty lhs, SerializedProperty rhs) => lhs.floatValue.CompareTo(rhs.floatValue);

			public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareCheckbox = (SerializedProperty lhs, SerializedProperty rhs) => lhs.boolValue.CompareTo(rhs.boolValue);

			public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareEnum = (SerializedProperty lhs, SerializedProperty rhs) => lhs.enumValueIndex.CompareTo(rhs.enumValueIndex);

			public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareInt = (SerializedProperty lhs, SerializedProperty rhs) => lhs.intValue.CompareTo(rhs.intValue);

			public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareColor = delegate(SerializedProperty lhs, SerializedProperty rhs)
			{
				float num;
				float num2;
				float num3;
				Color.RGBToHSV(lhs.colorValue, out num, out num2, out num3);
				float value;
				float num4;
				float num5;
				Color.RGBToHSV(rhs.colorValue, out value, out num4, out num5);
				return num.CompareTo(value);
			};

			public static readonly SerializedPropertyTreeView.Column.CompareEntry s_CompareName = (SerializedProperty lhs, SerializedProperty rhs) => 0;

			public static readonly SerializedPropertyTreeView.Column.CopyDelegate s_CopyDefault = delegate(SerializedProperty target, SerializedProperty source)
			{
				target.serializedObject.CopyFromSerializedProperty(source);
			};
		}

		private SerializedPropertyDataStore m_DataStore;

		private SerializedPropertyTreeView.ColumnInternal[] m_ColumnsInternal;

		private List<TreeViewItem> m_Items;

		private int m_ChangedId;

		private bool m_bFilterSelection;

		private int[] m_SelectionFilter;

		public SerializedPropertyTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, SerializedPropertyDataStore dataStore) : base(state, multicolumnHeader)
		{
			this.m_DataStore = dataStore;
			int num = base.multiColumnHeader.state.columns.Length;
			this.m_ColumnsInternal = new SerializedPropertyTreeView.ColumnInternal[num];
			for (int i = 0; i < num; i++)
			{
				SerializedPropertyTreeView.Column column = this.Col(i);
				if (column.propertyName != null)
				{
					this.m_ColumnsInternal[i].dependencyProps = new SerializedProperty[column.propertyName.Length];
				}
			}
			base.multiColumnHeader.sortingChanged += new MultiColumnHeader.HeaderCallback(this.OnSortingChanged);
			base.multiColumnHeader.visibleColumnsChanged += new MultiColumnHeader.HeaderCallback(this.OnVisibleColumnChanged);
			base.showAlternatingRowBackgrounds = true;
			base.showBorder = true;
			base.rowHeight = 18f;
		}

		public void SerializeState(string uid)
		{
			SessionState.SetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterSelection, this.m_bFilterSelection);
			for (int i = 0; i < base.multiColumnHeader.state.columns.Length; i++)
			{
				SerializedPropertyFilters.IFilter filter = this.Col(i).filter;
				if (filter != null)
				{
					string value = filter.SerializeState();
					if (!string.IsNullOrEmpty(value))
					{
						SessionState.SetString(uid + SerializedPropertyTreeView.Styles.serializeFilter + i, value);
					}
				}
			}
			SessionState.SetString(uid + SerializedPropertyTreeView.Styles.serializeTreeViewState, JsonUtility.ToJson(base.state));
			SessionState.SetString(uid + SerializedPropertyTreeView.Styles.serializeColumnHeaderState, JsonUtility.ToJson(base.multiColumnHeader.state));
		}

		public void DeserializeState(string uid)
		{
			this.m_bFilterSelection = SessionState.GetBool(uid + SerializedPropertyTreeView.Styles.serializeFilterSelection, false);
			for (int i = 0; i < base.multiColumnHeader.state.columns.Length; i++)
			{
				SerializedPropertyFilters.IFilter filter = this.Col(i).filter;
				if (filter != null)
				{
					string @string = SessionState.GetString(uid + SerializedPropertyTreeView.Styles.serializeFilter + i, null);
					if (!string.IsNullOrEmpty(@string))
					{
						filter.DeserializeState(@string);
					}
				}
			}
			string string2 = SessionState.GetString(uid + SerializedPropertyTreeView.Styles.serializeTreeViewState, "");
			string string3 = SessionState.GetString(uid + SerializedPropertyTreeView.Styles.serializeColumnHeaderState, "");
			if (!string.IsNullOrEmpty(string2))
			{
				JsonUtility.FromJsonOverwrite(string2, base.state);
			}
			if (!string.IsNullOrEmpty(string3))
			{
				JsonUtility.FromJsonOverwrite(string3, base.multiColumnHeader.state);
			}
		}

		public bool IsFilteredDirty()
		{
			return this.m_ChangedId != 0 && (this.m_ChangedId != GUIUtility.keyboardControl || !EditorGUIUtility.editingTextField);
		}

		public bool Update()
		{
			IList<TreeViewItem> rows = this.GetRows();
			int num;
			int num2;
			base.GetFirstAndLastVisibleRows(out num, out num2);
			bool flag = false;
			if (num2 != -1)
			{
				for (int i = num; i <= num2; i++)
				{
					flag = (flag || ((SerializedPropertyTreeView.SerializedPropertyItem)rows[i]).GetData().Update());
				}
			}
			return flag;
		}

		public void FullReload()
		{
			this.m_Items = null;
			base.Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			return new SerializedPropertyTreeView.SerializedPropertyItem(-1, -1, null);
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			if (this.m_Items == null)
			{
				SerializedPropertyDataStore.Data[] elements = this.m_DataStore.GetElements();
				this.m_Items = new List<TreeViewItem>(elements.Length);
				for (int i = 0; i < elements.Length; i++)
				{
					SerializedPropertyTreeView.SerializedPropertyItem item2 = new SerializedPropertyTreeView.SerializedPropertyItem(elements[i].objectId, 0, elements[i]);
					this.m_Items.Add(item2);
				}
			}
			IEnumerable<TreeViewItem> enumerable = this.m_Items;
			if (this.m_bFilterSelection)
			{
				if (this.m_SelectionFilter == null)
				{
					this.m_SelectionFilter = Selection.instanceIDs;
				}
				enumerable = from item in this.m_Items
				where this.m_SelectionFilter.Contains(item.id)
				select item;
			}
			else
			{
				this.m_SelectionFilter = null;
			}
			enumerable = this.Filter(enumerable);
			List<TreeViewItem> list = enumerable.ToList<TreeViewItem>();
			if (base.multiColumnHeader.sortedColumnIndex >= 0)
			{
				this.Sort(list, base.multiColumnHeader.sortedColumnIndex);
			}
			this.m_ChangedId = 0;
			TreeViewUtility.SetParentAndChildrenForItems(list, root);
			return list;
		}

		protected override void RowGUI(TreeView.RowGUIArgs args)
		{
			SerializedPropertyTreeView.SerializedPropertyItem item = (SerializedPropertyTreeView.SerializedPropertyItem)args.item;
			for (int i = 0; i < args.GetNumVisibleColumns(); i++)
			{
				this.CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
			}
		}

		private void CellGUI(Rect cellRect, SerializedPropertyTreeView.SerializedPropertyItem item, int columnIndex, ref TreeView.RowGUIArgs args)
		{
			Profiler.BeginSample("SerializedPropertyTreeView.CellGUI");
			base.CenterRectUsingSingleLineHeight(ref cellRect);
			SerializedPropertyDataStore.Data data = item.GetData();
			SerializedPropertyTreeView.Column column = (SerializedPropertyTreeView.Column)base.multiColumnHeader.GetColumn(columnIndex);
			if (column.drawDelegate == SerializedPropertyTreeView.DefaultDelegates.s_DrawName)
			{
				Profiler.BeginSample("SerializedPropertyTreeView.OnItemGUI.LabelField");
				TreeView.DefaultGUI.Label(cellRect, data.name, base.IsSelected(args.item.id), false);
				Profiler.EndSample();
			}
			else if (column.drawDelegate != null)
			{
				SerializedProperty[] properties = data.properties;
				int num = (column.dependencyIndices == null) ? 0 : column.dependencyIndices.Length;
				for (int i = 0; i < num; i++)
				{
					this.m_ColumnsInternal[columnIndex].dependencyProps[i] = properties[column.dependencyIndices[i]];
				}
				if (args.item.id == base.state.lastClickedID && base.HasFocus() && columnIndex == base.multiColumnHeader.state.visibleColumns[(base.multiColumnHeader.state.visibleColumns[0] != 0) ? 0 : 1])
				{
					GUI.SetNextControlName(SerializedPropertyTreeView.Styles.focusHelper);
				}
				SerializedProperty serializedProperty = data.properties[columnIndex];
				EditorGUI.BeginChangeCheck();
				Profiler.BeginSample("SerializedPropertyTreeView.OnItemGUI.drawDelegate");
				column.drawDelegate(cellRect, serializedProperty, this.m_ColumnsInternal[columnIndex].dependencyProps);
				Profiler.EndSample();
				if (EditorGUI.EndChangeCheck())
				{
					this.m_ChangedId = ((column.filter == null || !column.filter.Active()) ? this.m_ChangedId : GUIUtility.keyboardControl);
					data.Store();
					IList<int> selection = base.GetSelection();
					if (selection.Contains(data.objectId))
					{
						IList<TreeViewItem> list = base.FindRows(selection);
						Undo.RecordObjects((from r in list
						select ((SerializedPropertyTreeView.SerializedPropertyItem)r).GetData().serializedObject.targetObject).ToArray<UnityEngine.Object>(), "Modify Multiple Properties");
						foreach (TreeViewItem current in list)
						{
							if (current.id != args.item.id)
							{
								SerializedPropertyDataStore.Data data2 = ((SerializedPropertyTreeView.SerializedPropertyItem)current).GetData();
								if (SerializedPropertyTreeView.IsEditable(data2.serializedObject.targetObject))
								{
									if (column.copyDelegate != null)
									{
										column.copyDelegate(data2.properties[columnIndex], serializedProperty);
									}
									else
									{
										SerializedPropertyTreeView.DefaultDelegates.s_CopyDefault(data2.properties[columnIndex], serializedProperty);
									}
									data2.Store();
								}
							}
						}
					}
				}
				Profiler.EndSample();
			}
		}

		private static bool IsEditable(UnityEngine.Object target)
		{
			return (target.hideFlags & HideFlags.NotEditable) == HideFlags.None;
		}

		protected override void BeforeRowsGUI()
		{
			IList<TreeViewItem> rows = this.GetRows();
			int num;
			int num2;
			base.GetFirstAndLastVisibleRows(out num, out num2);
			if (num2 != -1)
			{
				for (int i = num; i <= num2; i++)
				{
					((SerializedPropertyTreeView.SerializedPropertyItem)rows[i]).GetData().Update();
				}
			}
			IList<TreeViewItem> list = base.FindRows(base.GetSelection());
			using (IEnumerator<TreeViewItem> enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SerializedPropertyTreeView.SerializedPropertyItem serializedPropertyItem = (SerializedPropertyTreeView.SerializedPropertyItem)enumerator.Current;
					serializedPropertyItem.GetData().Update();
				}
			}
			base.BeforeRowsGUI();
		}

		public void OnFilterGUI(Rect r)
		{
			EditorGUI.BeginChangeCheck();
			float width = r.width;
			float num = 16f;
			r.width = num;
			this.m_bFilterSelection = EditorGUI.Toggle(r, this.m_bFilterSelection);
			r.x += num;
			r.width = GUI.skin.label.CalcSize(SerializedPropertyTreeView.Styles.filterSelection).x;
			EditorGUI.LabelField(r, SerializedPropertyTreeView.Styles.filterSelection);
			r.width = Mathf.Min(width - (r.x + r.width), 300f);
			r.x = width - r.width + 10f;
			for (int i = 0; i < base.multiColumnHeader.state.columns.Length; i++)
			{
				if (this.IsColumnVisible(i))
				{
					SerializedPropertyTreeView.Column column = this.Col(i);
					if (column.filter != null && column.filter.GetType().Equals(typeof(SerializedPropertyFilters.Name)))
					{
						column.filter.OnGUI(r);
					}
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				base.Reload();
			}
		}

		protected override void SelectionChanged(IList<int> selectedIds)
		{
			Selection.instanceIDs = selectedIds.ToArray<int>();
		}

		protected override void KeyEvent()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.character == '\t')
				{
					GUI.FocusControl(SerializedPropertyTreeView.Styles.focusHelper);
					Event.current.Use();
				}
			}
		}

		private void OnVisibleColumnChanged(MultiColumnHeader header)
		{
			base.Reload();
		}

		private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
		{
			IList<TreeViewItem> rows = this.GetRows();
			this.Sort(rows, multiColumnHeader.sortedColumnIndex);
		}

		private void Sort(IList<TreeViewItem> rows, int sortIdx)
		{
			bool flag = base.multiColumnHeader.IsSortedAscending(sortIdx);
			SerializedPropertyTreeView.Column.CompareEntry comp = this.Col(sortIdx).compareDelegate;
			List<TreeViewItem> list = rows as List<TreeViewItem>;
			if (comp != null)
			{
				Comparison<TreeViewItem> comparison;
				Comparison<TreeViewItem> comparison2;
				if (comp == SerializedPropertyTreeView.DefaultDelegates.s_CompareName)
				{
					comparison = ((TreeViewItem lhs, TreeViewItem rhs) => EditorUtility.NaturalCompare(((SerializedPropertyTreeView.SerializedPropertyItem)lhs).GetData().name, ((SerializedPropertyTreeView.SerializedPropertyItem)rhs).GetData().name));
					comparison2 = ((TreeViewItem lhs, TreeViewItem rhs) => -EditorUtility.NaturalCompare(((SerializedPropertyTreeView.SerializedPropertyItem)lhs).GetData().name, ((SerializedPropertyTreeView.SerializedPropertyItem)rhs).GetData().name));
				}
				else
				{
					comparison = ((TreeViewItem lhs, TreeViewItem rhs) => comp(((SerializedPropertyTreeView.SerializedPropertyItem)lhs).GetData().properties[sortIdx], ((SerializedPropertyTreeView.SerializedPropertyItem)rhs).GetData().properties[sortIdx]));
					comparison2 = ((TreeViewItem lhs, TreeViewItem rhs) => -comp(((SerializedPropertyTreeView.SerializedPropertyItem)lhs).GetData().properties[sortIdx], ((SerializedPropertyTreeView.SerializedPropertyItem)rhs).GetData().properties[sortIdx]));
				}
				list.Sort((!flag) ? comparison2 : comparison);
			}
		}

		private IEnumerable<TreeViewItem> Filter(IEnumerable<TreeViewItem> rows)
		{
			IEnumerable<TreeViewItem> enumerable = rows;
			int num = this.m_ColumnsInternal.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.IsColumnVisible(i))
				{
					SerializedPropertyTreeView.Column c = this.Col(i);
					int idx = i;
					if (c.filter != null)
					{
						if (c.filter.Active())
						{
							if (c.filter.GetType().Equals(typeof(SerializedPropertyFilters.Name)))
							{
								SerializedPropertyFilters.Name f = (SerializedPropertyFilters.Name)c.filter;
								enumerable = from item in enumerable
								where f.Filter(((SerializedPropertyTreeView.SerializedPropertyItem)item).GetData().name)
								select item;
							}
							else
							{
								enumerable = from item in enumerable
								where c.filter.Filter(((SerializedPropertyTreeView.SerializedPropertyItem)item).GetData().properties[idx])
								select item;
							}
						}
					}
				}
			}
			return enumerable;
		}

		private bool IsColumnVisible(int idx)
		{
			bool result;
			for (int i = 0; i < base.multiColumnHeader.state.visibleColumns.Length; i++)
			{
				if (base.multiColumnHeader.state.visibleColumns[i] == idx)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private SerializedPropertyTreeView.Column Col(int idx)
		{
			return (SerializedPropertyTreeView.Column)base.multiColumnHeader.state.columns[idx];
		}
	}
}
