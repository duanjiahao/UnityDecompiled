using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ListViewGUILayout
	{
		internal class GUILayoutedListViewGroup : GUILayoutGroup
		{
			internal ListViewState state;

			public override void CalcWidth()
			{
				base.CalcWidth();
				this.minWidth = 0f;
				this.maxWidth = 0f;
				this.stretchWidth = 10000;
			}

			public override void CalcHeight()
			{
				this.minHeight = 0f;
				this.maxHeight = 0f;
				base.CalcHeight();
				this.margin.top = 0;
				this.margin.bottom = 0;
				if (this.minHeight == 0f)
				{
					this.minHeight = 1f;
					this.maxHeight = 1f;
					this.state.rowHeight = 1;
				}
				else
				{
					this.state.rowHeight = (int)this.minHeight;
					this.minHeight *= (float)this.state.totalRows;
					this.maxHeight *= (float)this.state.totalRows;
				}
			}

			private void AddYRecursive(GUILayoutEntry e, float y)
			{
				e.rect.y = e.rect.y + y;
				GUILayoutGroup gUILayoutGroup = e as GUILayoutGroup;
				if (gUILayoutGroup != null)
				{
					for (int i = 0; i < gUILayoutGroup.entries.Count; i++)
					{
						this.AddYRecursive(gUILayoutGroup.entries[i], y);
					}
				}
			}

			public void AddY()
			{
				if (this.entries.Count > 0)
				{
					this.AddYRecursive(this.entries[0], this.entries[0].minHeight);
				}
			}

			public void AddY(float val)
			{
				if (this.entries.Count > 0)
				{
					this.AddYRecursive(this.entries[0], val);
				}
			}
		}

		private static int layoutedListViewHash = "layoutedListView".GetHashCode();

		private static ListViewState lvState = null;

		private static int listViewHash = "ListView".GetHashCode();

		private static int[] dummyWidths = new int[1];

		private static Rect dummyRect = new Rect(0f, 0f, 1f, 1f);

		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUILayout.ListView(state, (ListViewOptions)0, string.Empty, style, options);
		}

		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUILayout.ListView(state, (ListViewOptions)0, dragTitle, style, options);
		}

		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, GUIStyle style, params GUILayoutOption[] options)
		{
			return ListViewGUILayout.ListView(state, lvOptions, string.Empty, style, options);
		}

		public static ListViewShared.ListViewElementsEnumerator ListView(ListViewState state, ListViewOptions lvOptions, string dragTitle, GUIStyle style, params GUILayoutOption[] options)
		{
			ListViewGUILayout.lvState = state;
			GUILayout.BeginHorizontal(style, options);
			state.scrollPos = EditorGUILayout.BeginScrollView(state.scrollPos, options);
			ListViewGUILayout.BeginLayoutedListview(state, GUIStyle.none, new GUILayoutOption[0]);
			state.draggedFrom = -1;
			state.draggedTo = -1;
			state.fileNames = null;
			if ((lvOptions & ListViewOptions.wantsReordering) != (ListViewOptions)0)
			{
				state.ilvState.wantsReordering = true;
			}
			if ((lvOptions & ListViewOptions.wantsExternalFiles) != (ListViewOptions)0)
			{
				state.ilvState.wantsExternalFiles = true;
			}
			if ((lvOptions & ListViewOptions.wantsToStartCustomDrag) != (ListViewOptions)0)
			{
				state.ilvState.wantsToStartCustomDrag = true;
			}
			if ((lvOptions & ListViewOptions.wantsToAcceptCustomDrag) != (ListViewOptions)0)
			{
				state.ilvState.wantsToAcceptCustomDrag = true;
			}
			return ListViewGUILayout.DoListView(state, null, dragTitle);
		}

		private static ListViewShared.ListViewElementsEnumerator DoListView(ListViewState state, int[] colWidths, string dragTitle)
		{
			Rect rect = ListViewGUILayout.dummyRect;
			int num = 0;
			int num2 = 0;
			ListViewShared.InternalLayoutedListViewState ilvState = state.ilvState;
			int controlID = GUIUtility.GetControlID(ListViewGUILayout.listViewHash, FocusType.Native);
			state.ID = controlID;
			state.selectionChanged = false;
			ilvState.state = state;
			if (Event.current.type != EventType.Layout)
			{
				rect = new Rect(0f, state.scrollPos.y, GUIClip.visibleRect.width, GUIClip.visibleRect.height);
				if (rect.width <= 0f)
				{
					rect.width = 1f;
				}
				if (rect.height <= 0f)
				{
					rect.height = 1f;
				}
				state.ilvState.rect = rect;
				num = (int)rect.yMin / state.rowHeight;
				num2 = num + (int)Math.Ceiling((double)((rect.yMin % (float)state.rowHeight + rect.height) / (float)state.rowHeight)) - 1;
				ilvState.invisibleRows = num;
				ilvState.endRow = num2;
				ilvState.rectHeight = (int)rect.height;
				if (num < 0)
				{
					num = 0;
				}
				if (num2 >= state.totalRows)
				{
					num2 = state.totalRows - 1;
				}
			}
			if (colWidths == null)
			{
				ListViewGUILayout.dummyWidths[0] = (int)rect.width;
				colWidths = ListViewGUILayout.dummyWidths;
			}
			return new ListViewShared.ListViewElementsEnumerator(ilvState, colWidths, num, num2, dragTitle, new Rect(0f, (float)(num * state.rowHeight), rect.width, (float)state.rowHeight));
		}

		private static void BeginLayoutedListview(ListViewState state, GUIStyle style, params GUILayoutOption[] options)
		{
			ListViewGUILayout.GUILayoutedListViewGroup gUILayoutedListViewGroup = (ListViewGUILayout.GUILayoutedListViewGroup)GUILayoutUtility.BeginLayoutGroup(style, null, typeof(ListViewGUILayout.GUILayoutedListViewGroup));
			gUILayoutedListViewGroup.state = state;
			state.ilvState.group = gUILayoutedListViewGroup;
			GUIUtility.GetControlID(ListViewGUILayout.layoutedListViewHash, FocusType.Native);
			EventType type = Event.current.type;
			if (type == EventType.Layout)
			{
				gUILayoutedListViewGroup.resetCoords = false;
				gUILayoutedListViewGroup.isVertical = true;
				gUILayoutedListViewGroup.ApplyOptions(options);
			}
		}

		public static bool MultiSelection(int prevSelected, int currSelected, ref int initialSelected, ref bool[] selectedItems)
		{
			return ListViewShared.MultiSelection(ListViewGUILayout.lvState.ilvState, prevSelected, currSelected, ref initialSelected, ref selectedItems);
		}

		public static bool HasMouseUp(Rect r)
		{
			return ListViewShared.HasMouseUp(ListViewGUILayout.lvState.ilvState, r, 0);
		}

		public static bool HasMouseDown(Rect r)
		{
			return ListViewShared.HasMouseDown(ListViewGUILayout.lvState.ilvState, r, 0);
		}

		public static bool HasMouseDown(Rect r, int button)
		{
			return ListViewShared.HasMouseDown(ListViewGUILayout.lvState.ilvState, r, button);
		}
	}
}
