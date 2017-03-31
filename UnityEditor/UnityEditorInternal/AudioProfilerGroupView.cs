using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AudioProfilerGroupView
	{
		internal class AudioProfilerGroupTreeViewItem : TreeViewItem
		{
			public AudioProfilerGroupInfoWrapper info
			{
				get;
				set;
			}

			public AudioProfilerGroupTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, AudioProfilerGroupInfoWrapper info) : base(id, depth, parent, displayName)
			{
				this.info = info;
			}
		}

		internal class AudioProfilerDataSource : TreeViewDataSource
		{
			private AudioProfilerGroupViewBackend m_Backend;

			public AudioProfilerDataSource(TreeViewController treeView, AudioProfilerGroupViewBackend backend) : base(treeView)
			{
				this.m_Backend = backend;
				this.m_Backend.OnUpdate = new AudioProfilerGroupViewBackend.DataUpdateDelegate(this.FetchData);
				base.showRootItem = false;
				base.rootIsCollapsable = false;
				this.FetchData();
			}

			private void FillTreeItems(AudioProfilerGroupView.AudioProfilerGroupTreeViewItem parentNode, int depth, int parentId, List<AudioProfilerGroupInfoWrapper> items)
			{
				int num = 0;
				foreach (AudioProfilerGroupInfoWrapper current in items)
				{
					if (parentId == ((!current.addToRoot) ? current.info.parentId : 0))
					{
						num++;
					}
				}
				if (num > 0)
				{
					parentNode.children = new List<TreeViewItem>(num);
					foreach (AudioProfilerGroupInfoWrapper current2 in items)
					{
						if (parentId == ((!current2.addToRoot) ? current2.info.parentId : 0))
						{
							AudioProfilerGroupView.AudioProfilerGroupTreeViewItem audioProfilerGroupTreeViewItem = new AudioProfilerGroupView.AudioProfilerGroupTreeViewItem(current2.info.uniqueId, (!current2.addToRoot) ? depth : 1, parentNode, current2.objectName, current2);
							parentNode.children.Add(audioProfilerGroupTreeViewItem);
							this.FillTreeItems(audioProfilerGroupTreeViewItem, depth + 1, current2.info.uniqueId, items);
						}
					}
				}
			}

			public override void FetchData()
			{
				AudioProfilerGroupView.AudioProfilerGroupTreeViewItem audioProfilerGroupTreeViewItem = new AudioProfilerGroupView.AudioProfilerGroupTreeViewItem(1, 0, null, "ROOT", new AudioProfilerGroupInfoWrapper(default(AudioProfilerGroupInfo), "ROOT", "ROOT", false));
				this.FillTreeItems(audioProfilerGroupTreeViewItem, 1, 0, this.m_Backend.items);
				this.m_RootItem = audioProfilerGroupTreeViewItem;
				this.SetExpandedWithChildren(this.m_RootItem, true);
				this.m_NeedRefreshRows = true;
			}

			public override bool CanBeParent(TreeViewItem item)
			{
				return item.hasChildren;
			}

			public override bool IsRenamingItemAllowed(TreeViewItem item)
			{
				return false;
			}
		}

		internal class AudioProfilerGroupViewColumnHeader
		{
			private AudioProfilerGroupTreeViewState m_TreeViewState;

			private AudioProfilerGroupViewBackend m_Backend;

			private string[] headers = new string[]
			{
				"Object",
				"Asset",
				"Volume",
				"Audibility",
				"Plays",
				"3D",
				"Paused",
				"Muted",
				"Virtual",
				"OneShot",
				"Looped",
				"Distance",
				"MinDist",
				"MaxDist",
				"Time",
				"Duration",
				"Frequency",
				"Stream",
				"Compressed",
				"NonBlocking",
				"User",
				"Memory",
				"MemoryPoint"
			};

			public float[] columnWidths
			{
				get;
				set;
			}

			public float minColumnWidth
			{
				get;
				set;
			}

			public float dragWidth
			{
				get;
				set;
			}

			public AudioProfilerGroupViewColumnHeader(AudioProfilerGroupTreeViewState state, AudioProfilerGroupViewBackend backend)
			{
				this.m_TreeViewState = state;
				this.m_Backend = backend;
				this.minColumnWidth = 10f;
				this.dragWidth = 6f;
			}

			public void OnGUI(Rect rect, bool allowSorting, GUIStyle headerStyle)
			{
				GUI.BeginClip(rect);
				float num = -this.m_TreeViewState.scrollPos.x;
				int lastColumnIndex = AudioProfilerGroupInfoHelper.GetLastColumnIndex();
				for (int i = 0; i <= lastColumnIndex; i++)
				{
					Rect position = new Rect(num, 0f, this.columnWidths[i], rect.height - 1f);
					num += this.columnWidths[i];
					Rect position2 = new Rect(num - this.dragWidth / 2f, 0f, 3f, rect.height);
					float x = EditorGUI.MouseDeltaReader(position2, true).x;
					if (x != 0f)
					{
						this.columnWidths[i] += x;
						this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
					}
					string text = this.headers[i];
					if (allowSorting && i == this.m_TreeViewState.selectedColumn)
					{
						text += ((!this.m_TreeViewState.sortByDescendingOrder) ? " ▲" : " ▼");
					}
					GUI.Box(position, text, headerStyle);
					if (allowSorting && Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
					{
						this.m_TreeViewState.SetSelectedColumn(i);
						this.m_Backend.UpdateSorting();
					}
					if (Event.current.type == EventType.Repaint)
					{
						EditorGUIUtility.AddCursorRect(position2, MouseCursor.SplitResizeLeftRight);
					}
				}
				GUI.EndClip();
			}
		}

		internal class AudioProfilerGroupViewGUI : TreeViewGUI
		{
			private float[] columnWidths
			{
				get
				{
					return ((AudioProfilerGroupTreeViewState)this.m_TreeView.state).columnWidths;
				}
			}

			public AudioProfilerGroupViewGUI(TreeViewController treeView) : base(treeView)
			{
				this.k_IconWidth = 0f;
			}

			protected override Texture GetIconForItem(TreeViewItem item)
			{
				return null;
			}

			protected override void RenameEnded()
			{
			}

			protected override void SyncFakeItem()
			{
			}

			public override Vector2 GetTotalSize()
			{
				Vector2 totalSize = base.GetTotalSize();
				totalSize.x = 0f;
				float[] columnWidths = this.columnWidths;
				for (int i = 0; i < columnWidths.Length; i++)
				{
					float num = columnWidths[i];
					totalSize.x += num;
				}
				return totalSize;
			}

			protected override void OnContentGUI(Rect rect, int row, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
			{
				if (Event.current.type == EventType.Repaint)
				{
					GUIStyle gUIStyle = (!useBoldFont) ? TreeViewGUI.Styles.lineStyle : TreeViewGUI.Styles.lineBoldStyle;
					TextAnchor alignment = gUIStyle.alignment;
					gUIStyle.alignment = TextAnchor.MiddleLeft;
					gUIStyle.padding.left = 0;
					int num = 2;
					base.OnContentGUI(new Rect(rect.x, rect.y, this.columnWidths[0] - (float)num, rect.height), row, item, label, selected, focused, useBoldFont, isPinging);
					rect.x += this.columnWidths[0] + (float)num;
					AudioProfilerGroupView.AudioProfilerGroupTreeViewItem audioProfilerGroupTreeViewItem = item as AudioProfilerGroupView.AudioProfilerGroupTreeViewItem;
					for (int i = 1; i < this.columnWidths.Length; i++)
					{
						rect.width = this.columnWidths[i] - (float)(2 * num);
						gUIStyle.Draw(rect, AudioProfilerGroupInfoHelper.GetColumnString(audioProfilerGroupTreeViewItem.info, (AudioProfilerGroupInfoHelper.ColumnIndices)i), false, false, selected, focused);
						Handles.color = Color.black;
						Handles.DrawLine(new Vector3(rect.x - (float)num + 1f, rect.y, 0f), new Vector3(rect.x - (float)num + 1f, rect.y + rect.height, 0f));
						rect.x += this.columnWidths[i];
						gUIStyle.alignment = TextAnchor.MiddleRight;
					}
					gUIStyle.alignment = alignment;
				}
			}
		}

		private TreeViewController m_TreeView;

		private AudioProfilerGroupTreeViewState m_TreeViewState;

		private EditorWindow m_EditorWindow;

		private AudioProfilerGroupView.AudioProfilerGroupViewColumnHeader m_ColumnHeader;

		private AudioProfilerGroupViewBackend m_Backend;

		private GUIStyle m_HeaderStyle;

		private int delayedPingObject;

		public AudioProfilerGroupView(EditorWindow editorWindow, AudioProfilerGroupTreeViewState state)
		{
			this.m_EditorWindow = editorWindow;
			this.m_TreeViewState = state;
		}

		public int GetNumItemsInData()
		{
			return this.m_Backend.items.Count;
		}

		public void Init(Rect rect, AudioProfilerGroupViewBackend backend)
		{
			if (this.m_HeaderStyle == null)
			{
				this.m_HeaderStyle = new GUIStyle("OL title");
			}
			this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
			if (this.m_TreeView == null)
			{
				this.m_Backend = backend;
				if (this.m_TreeViewState.columnWidths == null || this.m_TreeViewState.columnWidths.Length == 0)
				{
					int num = AudioProfilerGroupInfoHelper.GetLastColumnIndex() + 1;
					this.m_TreeViewState.columnWidths = new float[num];
					for (int i = 2; i < num; i++)
					{
						this.m_TreeViewState.columnWidths[i] = (float)((i != 2 && i != 3 && (i < 11 || i > 16)) ? 60 : 75);
					}
					this.m_TreeViewState.columnWidths[0] = 140f;
					this.m_TreeViewState.columnWidths[1] = 140f;
				}
				this.m_TreeView = new TreeViewController(this.m_EditorWindow, this.m_TreeViewState);
				ITreeViewGUI gui = new AudioProfilerGroupView.AudioProfilerGroupViewGUI(this.m_TreeView);
				ITreeViewDataSource data = new AudioProfilerGroupView.AudioProfilerDataSource(this.m_TreeView, this.m_Backend);
				this.m_TreeView.Init(rect, data, gui, null);
				this.m_ColumnHeader = new AudioProfilerGroupView.AudioProfilerGroupViewColumnHeader(this.m_TreeViewState, this.m_Backend);
				this.m_ColumnHeader.columnWidths = this.m_TreeViewState.columnWidths;
				this.m_ColumnHeader.minColumnWidth = 30f;
				TreeViewController expr_175 = this.m_TreeView;
				expr_175.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_175.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
			}
		}

		private void PingObjectDelayed()
		{
			EditorGUIUtility.PingObject(this.delayedPingObject);
		}

		public void OnTreeSelectionChanged(int[] selection)
		{
			if (selection.Length == 1)
			{
				TreeViewItem treeViewItem = this.m_TreeView.FindItem(selection[0]);
				AudioProfilerGroupView.AudioProfilerGroupTreeViewItem audioProfilerGroupTreeViewItem = treeViewItem as AudioProfilerGroupView.AudioProfilerGroupTreeViewItem;
				if (audioProfilerGroupTreeViewItem != null)
				{
					EditorGUIUtility.PingObject(audioProfilerGroupTreeViewItem.info.info.assetInstanceId);
					this.delayedPingObject = audioProfilerGroupTreeViewItem.info.info.objectInstanceId;
					EditorApplication.CallDelayed(new EditorApplication.CallbackFunction(this.PingObjectDelayed), 1f);
				}
			}
		}

		public void OnGUI(Rect rect, bool allowSorting)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_HeaderStyle.fixedHeight);
			GUI.Label(rect2, "", this.m_HeaderStyle);
			this.m_ColumnHeader.OnGUI(rect2, allowSorting, this.m_HeaderStyle);
			rect.y += rect2.height;
			rect.height -= rect2.height;
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(rect, controlID);
		}
	}
}
