using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class FrameDebuggerTreeView
	{
		private class FDTreeViewItem : TreeViewItem
		{
			public FrameDebuggerEvent m_FrameEvent;

			public int m_ChildEventCount;

			public int m_EventIndex;

			public FDTreeViewItem(int id, int depth, FrameDebuggerTreeView.FDTreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
			{
				this.m_EventIndex = id;
			}
		}

		private class FDTreeViewGUI : TreeViewGUI
		{
			private const float kSmallMargin = 4f;

			public FDTreeViewGUI(TreeView treeView) : base(treeView)
			{
			}

			protected override Texture GetIconForItem(TreeViewItem item)
			{
				return null;
			}

			protected override void DrawIconAndLabel(Rect rect, TreeViewItem itemRaw, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
			{
				FrameDebuggerTreeView.FDTreeViewItem fDTreeViewItem = (FrameDebuggerTreeView.FDTreeViewItem)itemRaw;
				float contentIndent = this.GetContentIndent(fDTreeViewItem);
				rect.x += contentIndent;
				rect.width -= contentIndent;
				string text;
				GUIContent content;
				GUIStyle gUIStyle;
				if (fDTreeViewItem.m_ChildEventCount > 0)
				{
					Rect position = rect;
					position.width -= 4f;
					text = fDTreeViewItem.m_ChildEventCount.ToString(CultureInfo.InvariantCulture);
					content = EditorGUIUtility.TempContent(text);
					gUIStyle = FrameDebuggerWindow.styles.rowTextRight;
					gUIStyle.Draw(position, content, false, false, false, false);
					rect.width -= gUIStyle.CalcSize(content).x + 8f;
				}
				if (fDTreeViewItem.id <= 0)
				{
					text = fDTreeViewItem.displayName;
				}
				else
				{
					text = FrameDebuggerWindow.s_FrameEventTypeNames[(int)fDTreeViewItem.m_FrameEvent.type] + fDTreeViewItem.displayName;
				}
				if (string.IsNullOrEmpty(text))
				{
					text = "<unknown scope>";
				}
				content = EditorGUIUtility.TempContent(text);
				gUIStyle = FrameDebuggerWindow.styles.rowText;
				gUIStyle.Draw(rect, content, false, false, false, selected && focused);
			}

			protected override void RenameEnded()
			{
			}
		}

		internal class FDTreeViewDataSource : TreeViewDataSource
		{
			private class FDTreeHierarchyLevel
			{
				internal readonly FrameDebuggerTreeView.FDTreeViewItem item;

				internal readonly List<TreeViewItem> children;

				internal FDTreeHierarchyLevel(int depth, int id, string name, FrameDebuggerTreeView.FDTreeViewItem parent)
				{
					this.item = new FrameDebuggerTreeView.FDTreeViewItem(id, depth, parent, name);
					this.children = new List<TreeViewItem>();
				}
			}

			private FrameDebuggerEvent[] m_FrameEvents;

			public FDTreeViewDataSource(TreeView treeView, FrameDebuggerEvent[] frameEvents) : base(treeView)
			{
				this.m_FrameEvents = frameEvents;
				base.rootIsCollapsable = false;
				base.showRootItem = false;
			}

			public void SetEvents(FrameDebuggerEvent[] frameEvents)
			{
				bool flag = this.m_FrameEvents == null || this.m_FrameEvents.Length < 1;
				this.m_FrameEvents = frameEvents;
				this.m_NeedRefreshVisibleFolders = true;
				this.ReloadData();
				if (flag)
				{
					this.SetExpandedWithChildren(this.m_RootItem, true);
				}
			}

			public override bool IsRenamingItemAllowed(TreeViewItem item)
			{
				return false;
			}

			public override bool CanBeMultiSelected(TreeViewItem item)
			{
				return false;
			}

			private static void CloseLastHierarchyLevel(List<FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel> eventStack, int prevFrameEventIndex)
			{
				int index = eventStack.Count - 1;
				eventStack[index].item.children = eventStack[index].children;
				eventStack[index].item.m_EventIndex = prevFrameEventIndex;
				if (eventStack[index].item.parent != null)
				{
					((FrameDebuggerTreeView.FDTreeViewItem)eventStack[index].item.parent).m_ChildEventCount += eventStack[index].item.m_ChildEventCount;
				}
				eventStack.RemoveAt(index);
			}

			public override void FetchData()
			{
				FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel fDTreeHierarchyLevel = new FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel(0, 0, string.Empty, null);
				List<FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel> list = new List<FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel>();
				list.Add(fDTreeHierarchyLevel);
				int num = -1;
				for (int i = 0; i < this.m_FrameEvents.Length; i++)
				{
					string text = "/" + (FrameDebuggerUtility.GetFrameEventInfoName(i) ?? string.Empty);
					string[] array = text.Split(new char[]
					{
						'/'
					});
					int num2 = 0;
					while (num2 < list.Count && num2 < array.Length)
					{
						if (array[num2] != list[num2].item.displayName)
						{
							break;
						}
						num2++;
					}
					while (list.Count > 0 && list.Count > num2)
					{
						FrameDebuggerTreeView.FDTreeViewDataSource.CloseLastHierarchyLevel(list, i);
					}
					for (int j = num2; j < array.Length; j++)
					{
						FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel fDTreeHierarchyLevel2 = list[list.Count - 1];
						FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel fDTreeHierarchyLevel3 = new FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel(list.Count - 1, --num, array[j], fDTreeHierarchyLevel2.item);
						fDTreeHierarchyLevel2.children.Add(fDTreeHierarchyLevel3.item);
						list.Add(fDTreeHierarchyLevel3);
					}
					GameObject frameEventGameObject = FrameDebuggerUtility.GetFrameEventGameObject(i);
					string displayName = (!frameEventGameObject) ? string.Empty : (" " + frameEventGameObject.name);
					FrameDebuggerTreeView.FDTreeViewDataSource.FDTreeHierarchyLevel fDTreeHierarchyLevel4 = list[list.Count - 1];
					int id = i + 1;
					FrameDebuggerTreeView.FDTreeViewItem fDTreeViewItem = new FrameDebuggerTreeView.FDTreeViewItem(id, list.Count - 1, fDTreeHierarchyLevel4.item, displayName);
					fDTreeViewItem.m_FrameEvent = this.m_FrameEvents[i];
					fDTreeHierarchyLevel4.children.Add(fDTreeViewItem);
					fDTreeHierarchyLevel4.item.m_ChildEventCount++;
				}
				while (list.Count > 0)
				{
					FrameDebuggerTreeView.FDTreeViewDataSource.CloseLastHierarchyLevel(list, this.m_FrameEvents.Length);
				}
				this.m_RootItem = fDTreeHierarchyLevel.item;
			}
		}

		internal readonly TreeView m_TreeView;

		internal FrameDebuggerTreeView.FDTreeViewDataSource m_DataSource;

		private readonly FrameDebuggerWindow m_FrameDebugger;

		public FrameDebuggerTreeView(FrameDebuggerEvent[] frameEvents, TreeViewState treeViewState, FrameDebuggerWindow window, Rect startRect)
		{
			this.m_FrameDebugger = window;
			this.m_TreeView = new TreeView(window, treeViewState);
			this.m_DataSource = new FrameDebuggerTreeView.FDTreeViewDataSource(this.m_TreeView, frameEvents);
			FrameDebuggerTreeView.FDTreeViewGUI gui = new FrameDebuggerTreeView.FDTreeViewGUI(this.m_TreeView);
			this.m_TreeView.Init(startRect, this.m_DataSource, gui, null);
			this.m_TreeView.ReloadData();
			TreeView expr_5E = this.m_TreeView;
			expr_5E.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_5E.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
		}

		private void SelectionChanged(int[] selectedIDs)
		{
			if (selectedIDs.Length < 1)
			{
				return;
			}
			int num = selectedIDs[0];
			int num2 = num;
			if (num2 <= 0)
			{
				FrameDebuggerTreeView.FDTreeViewItem fDTreeViewItem = this.m_TreeView.FindItem(num) as FrameDebuggerTreeView.FDTreeViewItem;
				if (fDTreeViewItem != null)
				{
					num2 = fDTreeViewItem.m_EventIndex;
				}
			}
			if (num2 <= 0)
			{
				return;
			}
			this.m_FrameDebugger.ChangeFrameEventLimit(num2);
		}

		public void SelectFrameEventIndex(int eventIndex)
		{
			int[] selection = this.m_TreeView.GetSelection();
			if (selection.Length > 0)
			{
				FrameDebuggerTreeView.FDTreeViewItem fDTreeViewItem = this.m_TreeView.FindItem(selection[0]) as FrameDebuggerTreeView.FDTreeViewItem;
				if (fDTreeViewItem != null && eventIndex == fDTreeViewItem.m_EventIndex)
				{
					return;
				}
			}
			this.m_TreeView.SetSelection(new int[]
			{
				eventIndex
			}, true);
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
			this.m_TreeView.OnGUI(rect, controlID);
		}
	}
}
