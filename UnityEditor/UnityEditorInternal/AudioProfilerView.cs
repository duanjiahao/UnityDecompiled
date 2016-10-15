using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AudioProfilerView
	{
		internal class AudioProfilerTreeViewItem : TreeViewItem
		{
			public AudioProfilerInfoWrapper info
			{
				get;
				set;
			}

			public AudioProfilerTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, AudioProfilerInfoWrapper info) : base(id, depth, parent, displayName)
			{
				this.info = info;
			}
		}

		internal class AudioProfilerDataSource : TreeViewDataSource
		{
			private AudioProfilerBackend m_Backend;

			public AudioProfilerDataSource(TreeView treeView, AudioProfilerBackend backend) : base(treeView)
			{
				this.m_Backend = backend;
				this.m_Backend.OnUpdate = new AudioProfilerBackend.DataUpdateDelegate(this.FetchData);
				base.showRootItem = false;
				base.rootIsCollapsable = false;
				this.FetchData();
			}

			private void FillTreeItems(AudioProfilerView.AudioProfilerTreeViewItem parentNode, int depth, int parentId, List<AudioProfilerInfoWrapper> items)
			{
				int num = 0;
				foreach (AudioProfilerInfoWrapper current in items)
				{
					if (parentId == ((!current.addToRoot) ? current.info.parentId : 0))
					{
						num++;
					}
				}
				if (num > 0)
				{
					parentNode.children = new List<TreeViewItem>(num);
					foreach (AudioProfilerInfoWrapper current2 in items)
					{
						if (parentId == ((!current2.addToRoot) ? current2.info.parentId : 0))
						{
							AudioProfilerView.AudioProfilerTreeViewItem audioProfilerTreeViewItem = new AudioProfilerView.AudioProfilerTreeViewItem(current2.info.uniqueId, (!current2.addToRoot) ? depth : 1, parentNode, current2.objectName, current2);
							parentNode.children.Add(audioProfilerTreeViewItem);
							this.FillTreeItems(audioProfilerTreeViewItem, depth + 1, current2.info.uniqueId, items);
						}
					}
				}
			}

			public override void FetchData()
			{
				AudioProfilerView.AudioProfilerTreeViewItem audioProfilerTreeViewItem = new AudioProfilerView.AudioProfilerTreeViewItem(1, 0, null, "ROOT", new AudioProfilerInfoWrapper(default(AudioProfilerInfo), "ROOT", "ROOT", false));
				this.FillTreeItems(audioProfilerTreeViewItem, 1, 0, this.m_Backend.items);
				this.m_RootItem = audioProfilerTreeViewItem;
				this.SetExpandedWithChildren(this.m_RootItem, true);
				this.m_NeedRefreshVisibleFolders = true;
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

		internal class AudioProfilerViewColumnHeader
		{
			private AudioProfilerTreeViewState m_TreeViewState;

			private AudioProfilerBackend m_Backend;

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

			public AudioProfilerViewColumnHeader(AudioProfilerTreeViewState state, AudioProfilerBackend backend)
			{
				this.m_TreeViewState = state;
				this.m_Backend = backend;
				this.minColumnWidth = 10f;
				this.dragWidth = 6f;
			}

			public void OnGUI(Rect rect, bool allowSorting, GUIStyle headerStyle)
			{
				GUIClip.Push(rect, Vector2.zero, Vector2.zero, false);
				float num = -this.m_TreeViewState.scrollPos.x;
				int lastColumnIndex = AudioProfilerInfoHelper.GetLastColumnIndex();
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
					string[] array = new string[]
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
					string text = array[i];
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
				GUIClip.Pop();
			}
		}

		internal class AudioProfilerViewGUI : TreeViewGUI
		{
			private float[] columnWidths
			{
				get
				{
					return this.m_TreeView.state.columnWidths;
				}
			}

			public AudioProfilerViewGUI(TreeView treeView) : base(treeView)
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

			protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
			{
				GUIStyle gUIStyle = (!useBoldFont) ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
				gUIStyle.alignment = TextAnchor.MiddleLeft;
				gUIStyle.padding.left = 0;
				int num = 2;
				base.DrawIconAndLabel(new Rect(rect.x, rect.y, this.columnWidths[0] - (float)num, rect.height), item, label, selected, focused, useBoldFont, isPinging);
				rect.x += this.columnWidths[0] + (float)num;
				AudioProfilerView.AudioProfilerTreeViewItem audioProfilerTreeViewItem = item as AudioProfilerView.AudioProfilerTreeViewItem;
				for (int i = 1; i < this.columnWidths.Length; i++)
				{
					rect.width = this.columnWidths[i] - (float)(2 * num);
					gUIStyle.Draw(rect, AudioProfilerInfoHelper.GetColumnString(audioProfilerTreeViewItem.info, (AudioProfilerInfoHelper.ColumnIndices)i), false, false, selected, focused);
					Handles.color = Color.black;
					Handles.DrawLine(new Vector3(rect.x - (float)num + 1f, rect.y, 0f), new Vector3(rect.x - (float)num + 1f, rect.y + rect.height, 0f));
					rect.x += this.columnWidths[i];
					gUIStyle.alignment = TextAnchor.MiddleRight;
				}
				gUIStyle.alignment = TextAnchor.MiddleLeft;
			}
		}

		private TreeView m_TreeView;

		private AudioProfilerTreeViewState m_TreeViewState;

		private EditorWindow m_EditorWindow;

		private AudioProfilerView.AudioProfilerViewColumnHeader m_ColumnHeader;

		private AudioProfilerBackend m_Backend;

		private GUIStyle m_HeaderStyle;

		private int delayedPingObject;

		public AudioProfilerView(EditorWindow editorWindow, AudioProfilerTreeViewState state)
		{
			this.m_EditorWindow = editorWindow;
			this.m_TreeViewState = state;
		}

		public int GetNumItemsInData()
		{
			return this.m_Backend.items.Count;
		}

		public void Init(Rect rect, AudioProfilerBackend backend)
		{
			if (this.m_HeaderStyle == null)
			{
				this.m_HeaderStyle = new GUIStyle("OL title");
			}
			this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
			if (this.m_TreeView != null)
			{
				return;
			}
			this.m_Backend = backend;
			if (this.m_TreeViewState.columnWidths == null)
			{
				int num = AudioProfilerInfoHelper.GetLastColumnIndex() + 1;
				this.m_TreeViewState.columnWidths = new float[num];
				for (int i = 2; i < num; i++)
				{
					this.m_TreeViewState.columnWidths[i] = (float)((i != 2 && i != 3 && (i < 11 || i > 16)) ? 60 : 75);
				}
				this.m_TreeViewState.columnWidths[0] = 140f;
				this.m_TreeViewState.columnWidths[1] = 140f;
			}
			this.m_TreeView = new TreeView(this.m_EditorWindow, this.m_TreeViewState);
			ITreeViewGUI gui = new AudioProfilerView.AudioProfilerViewGUI(this.m_TreeView);
			ITreeViewDataSource data = new AudioProfilerView.AudioProfilerDataSource(this.m_TreeView, this.m_Backend);
			this.m_TreeView.Init(rect, data, gui, null);
			this.m_ColumnHeader = new AudioProfilerView.AudioProfilerViewColumnHeader(this.m_TreeViewState, this.m_Backend);
			this.m_ColumnHeader.columnWidths = this.m_TreeViewState.columnWidths;
			this.m_ColumnHeader.minColumnWidth = 30f;
			TreeView expr_15A = this.m_TreeView;
			expr_15A.selectionChangedCallback = (Action<int[]>)Delegate.Combine(expr_15A.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
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
				AudioProfilerView.AudioProfilerTreeViewItem audioProfilerTreeViewItem = treeViewItem as AudioProfilerView.AudioProfilerTreeViewItem;
				if (audioProfilerTreeViewItem != null)
				{
					EditorGUIUtility.PingObject(audioProfilerTreeViewItem.info.info.assetInstanceId);
					this.delayedPingObject = audioProfilerTreeViewItem.info.info.objectInstanceId;
					EditorApplication.CallDelayed(new EditorApplication.CallbackFunction(this.PingObjectDelayed), 1f);
				}
			}
		}

		public void OnGUI(Rect rect, bool allowSorting)
		{
			int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, this.m_HeaderStyle.fixedHeight);
			GUI.Label(rect2, string.Empty, this.m_HeaderStyle);
			this.m_ColumnHeader.OnGUI(rect2, allowSorting, this.m_HeaderStyle);
			rect.y += rect2.height;
			rect.height -= rect2.height;
			this.m_TreeView.OnEvent();
			this.m_TreeView.OnGUI(rect, controlID);
		}
	}
}
