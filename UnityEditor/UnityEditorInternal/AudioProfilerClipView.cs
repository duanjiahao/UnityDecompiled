using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AudioProfilerClipView
	{
		internal class AudioProfilerClipTreeViewItem : TreeViewItem
		{
			public AudioProfilerClipInfoWrapper info
			{
				get;
				set;
			}

			public AudioProfilerClipTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, AudioProfilerClipInfoWrapper info) : base(id, depth, parent, displayName)
			{
				this.info = info;
			}
		}

		internal class AudioProfilerDataSource : TreeViewDataSource
		{
			private AudioProfilerClipViewBackend m_Backend;

			public AudioProfilerDataSource(TreeViewController treeView, AudioProfilerClipViewBackend backend) : base(treeView)
			{
				this.m_Backend = backend;
				this.m_Backend.OnUpdate = new AudioProfilerClipViewBackend.DataUpdateDelegate(this.FetchData);
				base.showRootItem = false;
				base.rootIsCollapsable = false;
				this.FetchData();
			}

			private void FillTreeItems(AudioProfilerClipView.AudioProfilerClipTreeViewItem parentNode, int depth, int parentId, List<AudioProfilerClipInfoWrapper> items)
			{
				parentNode.children = new List<TreeViewItem>(items.Count);
				int num = 1;
				foreach (AudioProfilerClipInfoWrapper current in items)
				{
					AudioProfilerClipView.AudioProfilerClipTreeViewItem item = new AudioProfilerClipView.AudioProfilerClipTreeViewItem(++num, 1, parentNode, current.assetName, current);
					parentNode.children.Add(item);
				}
			}

			public override void FetchData()
			{
				AudioProfilerClipView.AudioProfilerClipTreeViewItem audioProfilerClipTreeViewItem = new AudioProfilerClipView.AudioProfilerClipTreeViewItem(1, 0, null, "ROOT", new AudioProfilerClipInfoWrapper(default(AudioProfilerClipInfo), "ROOT"));
				this.FillTreeItems(audioProfilerClipTreeViewItem, 1, 0, this.m_Backend.items);
				this.m_RootItem = audioProfilerClipTreeViewItem;
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

		internal class AudioProfilerClipViewColumnHeader
		{
			private AudioProfilerClipTreeViewState m_TreeViewState;

			private AudioProfilerClipViewBackend m_Backend;

			private string[] headers = new string[]
			{
				"Asset",
				"Load State",
				"Internal Load State",
				"Age",
				"Disposed",
				"Num Voices"
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

			public AudioProfilerClipViewColumnHeader(AudioProfilerClipTreeViewState state, AudioProfilerClipViewBackend backend)
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
				int lastColumnIndex = AudioProfilerClipInfoHelper.GetLastColumnIndex();
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

		internal class AudioProfilerClipViewGUI : TreeViewGUI
		{
			private float[] columnWidths
			{
				get
				{
					return this.m_TreeView.state.columnWidths;
				}
			}

			public AudioProfilerClipViewGUI(TreeViewController treeView) : base(treeView)
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
					GUIStyle gUIStyle = (!useBoldFont) ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
					gUIStyle.alignment = TextAnchor.MiddleLeft;
					gUIStyle.padding.left = 0;
					int num = 2;
					base.OnContentGUI(new Rect(rect.x, rect.y, this.columnWidths[0] - (float)num, rect.height), row, item, label, selected, focused, useBoldFont, isPinging);
					rect.x += this.columnWidths[0] + (float)num;
					AudioProfilerClipView.AudioProfilerClipTreeViewItem audioProfilerClipTreeViewItem = item as AudioProfilerClipView.AudioProfilerClipTreeViewItem;
					for (int i = 1; i < this.columnWidths.Length; i++)
					{
						rect.width = this.columnWidths[i] - (float)(2 * num);
						gUIStyle.alignment = TextAnchor.MiddleRight;
						gUIStyle.Draw(rect, AudioProfilerClipInfoHelper.GetColumnString(audioProfilerClipTreeViewItem.info, (AudioProfilerClipInfoHelper.ColumnIndices)i), false, false, selected, focused);
						Handles.color = Color.black;
						Handles.DrawLine(new Vector3(rect.x - (float)num + 1f, rect.y, 0f), new Vector3(rect.x - (float)num + 1f, rect.y + rect.height, 0f));
						rect.x += this.columnWidths[i];
					}
					gUIStyle.alignment = TextAnchor.MiddleLeft;
				}
			}
		}

		private TreeViewController m_TreeView;

		private AudioProfilerClipTreeViewState m_TreeViewState;

		private EditorWindow m_EditorWindow;

		private AudioProfilerClipView.AudioProfilerClipViewColumnHeader m_ColumnHeader;

		private AudioProfilerClipViewBackend m_Backend;

		private GUIStyle m_HeaderStyle;

		private int delayedPingObject;

		public AudioProfilerClipView(EditorWindow editorWindow, AudioProfilerClipTreeViewState state)
		{
			this.m_EditorWindow = editorWindow;
			this.m_TreeViewState = state;
		}

		public int GetNumItemsInData()
		{
			return this.m_Backend.items.Count;
		}

		public void Init(Rect rect, AudioProfilerClipViewBackend backend)
		{
			if (this.m_HeaderStyle == null)
			{
				this.m_HeaderStyle = new GUIStyle("OL title");
			}
			this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
			if (this.m_TreeView == null)
			{
				this.m_Backend = backend;
				if (this.m_TreeViewState.columnWidths == null)
				{
					int num = AudioProfilerClipInfoHelper.GetLastColumnIndex() + 1;
					this.m_TreeViewState.columnWidths = new float[num];
					for (int i = 0; i < num; i++)
					{
						this.m_TreeViewState.columnWidths[i] = (float)((i != 0) ? ((i != 2) ? 80 : 110) : 300);
					}
				}
				this.m_TreeView = new TreeViewController(this.m_EditorWindow, this.m_TreeViewState);
				ITreeViewGUI gui = new AudioProfilerClipView.AudioProfilerClipViewGUI(this.m_TreeView);
				ITreeViewDataSource data = new AudioProfilerClipView.AudioProfilerDataSource(this.m_TreeView, this.m_Backend);
				this.m_TreeView.Init(rect, data, gui, null);
				this.m_ColumnHeader = new AudioProfilerClipView.AudioProfilerClipViewColumnHeader(this.m_TreeViewState, this.m_Backend);
				this.m_ColumnHeader.columnWidths = this.m_TreeViewState.columnWidths;
				this.m_ColumnHeader.minColumnWidth = 30f;
				TreeViewController expr_138 = this.m_TreeView;
				expr_138.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_138.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
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
				AudioProfilerClipView.AudioProfilerClipTreeViewItem audioProfilerClipTreeViewItem = treeViewItem as AudioProfilerClipView.AudioProfilerClipTreeViewItem;
				if (audioProfilerClipTreeViewItem != null)
				{
					EditorGUIUtility.PingObject(audioProfilerClipTreeViewItem.info.info.assetInstanceId);
				}
			}
		}

		public void OnGUI(Rect rect)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_HeaderStyle.fixedHeight);
			GUI.Label(rect2, "", this.m_HeaderStyle);
			this.m_ColumnHeader.OnGUI(rect2, true, this.m_HeaderStyle);
			rect.y += rect2.height;
			rect.height -= rect2.height;
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(rect, controlID);
		}
	}
}
