using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace UnityEditorInternal.VersionControl
{
	[Serializable]
	public class ListControl
	{
		public enum SelectDirection
		{
			Up,
			Down,
			Current
		}

		[Serializable]
		public class ListState
		{
			[SerializeField]
			public float Scroll;

			[SerializeField]
			public List<string> Expanded = new List<string>();
		}

		public delegate void ExpandDelegate(ChangeSet expand, ListItem item);

		public delegate void DragDelegate(ChangeSet target);

		public delegate void ActionDelegate(ListItem item, int actionIdx);

		private const float c_lineHeight = 16f;

		private const float c_scrollWidth = 14f;

		private const string c_changeKeyPrefix = "_chkeyprfx_";

		private const string c_metaSuffix = ".meta";

		internal const string c_emptyChangeListMessage = "Empty change list";

		private ListControl.ExpandDelegate expandDelegate;

		private ListControl.DragDelegate dragDelegate;

		private ListControl.ActionDelegate actionDelegate;

		private ListItem root = new ListItem();

		private ListItem active;

		private List<ListItem> visibleList = new List<ListItem>();

		private Texture2D blueTex;

		private Texture2D greyTex;

		private Texture2D yellowTex;

		[SerializeField]
		private ListControl.ListState m_listState;

		private Dictionary<string, ListItem> pathSearch = new Dictionary<string, ListItem>();

		private Texture2D defaultIcon;

		private bool readOnly;

		private bool scrollVisible;

		private string menuFolder;

		private string menuDefault;

		private bool dragAcceptOnly;

		private ListItem dragTarget;

		private int dragCount;

		private ListControl.SelectDirection dragAdjust = ListControl.SelectDirection.Current;

		private Dictionary<string, ListItem> selectList = new Dictionary<string, ListItem>();

		private ListItem singleSelect;

		private GUIContent calcSizeTmpContent = new GUIContent();

		[NonSerialized]
		private int uniqueID;

		private static int s_uniqueIDCount = 1;

		private static Dictionary<int, ListControl> s_uniqueIDList = new Dictionary<int, ListControl>();

		public ListControl.ListState listState
		{
			get
			{
				if (this.m_listState == null)
				{
					this.m_listState = new ListControl.ListState();
				}
				return this.m_listState;
			}
		}

		public ListControl.ExpandDelegate ExpandEvent
		{
			get
			{
				return this.expandDelegate;
			}
			set
			{
				this.expandDelegate = value;
			}
		}

		public ListControl.DragDelegate DragEvent
		{
			get
			{
				return this.dragDelegate;
			}
			set
			{
				this.dragDelegate = value;
			}
		}

		public ListControl.ActionDelegate ActionEvent
		{
			get
			{
				return this.actionDelegate;
			}
			set
			{
				this.actionDelegate = value;
			}
		}

		public ListItem Root
		{
			get
			{
				return this.root;
			}
		}

		public AssetList SelectedAssets
		{
			get
			{
				AssetList assetList = new AssetList();
				foreach (KeyValuePair<string, ListItem> current in this.selectList)
				{
					if (current.Value.Item is Asset)
					{
						assetList.Add(current.Value.Item as Asset);
					}
				}
				return assetList;
			}
		}

		public ChangeSets SelectedChangeSets
		{
			get
			{
				ChangeSets changeSets = new ChangeSets();
				foreach (KeyValuePair<string, ListItem> current in this.selectList)
				{
					if (current.Value != null && current.Value.Item is ChangeSet)
					{
						changeSets.Add(current.Value.Item as ChangeSet);
					}
				}
				return changeSets;
			}
		}

		public ChangeSets EmptyChangeSets
		{
			get
			{
				ListItem listItem = this.root.FirstChild;
				ChangeSets changeSets = new ChangeSets();
				while (listItem != null)
				{
					ChangeSet change = listItem.Change;
					bool flag = change != null && listItem.HasChildren && listItem.FirstChild.Item == null && listItem.FirstChild.Name == "Empty change list";
					if (flag)
					{
						changeSets.Add(change);
					}
					listItem = listItem.Next;
				}
				return changeSets;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		public string MenuFolder
		{
			get
			{
				return this.menuFolder;
			}
			set
			{
				this.menuFolder = value;
			}
		}

		public string MenuDefault
		{
			get
			{
				return this.menuDefault;
			}
			set
			{
				this.menuDefault = value;
			}
		}

		public bool DragAcceptOnly
		{
			get
			{
				return this.dragAcceptOnly;
			}
			set
			{
				this.dragAcceptOnly = value;
			}
		}

		public int Size
		{
			get
			{
				return this.visibleList.Count;
			}
		}

		public ListControl()
		{
			this.uniqueID = ListControl.s_uniqueIDCount++;
			ListControl.s_uniqueIDList.Add(this.uniqueID, this);
			this.active = this.root;
			this.Clear();
		}

		public static ListControl FromID(int id)
		{
			ListControl result;
			try
			{
				result = ListControl.s_uniqueIDList[id];
			}
			catch
			{
				result = null;
			}
			return result;
		}

		~ListControl()
		{
			ListControl.s_uniqueIDList.Remove(this.uniqueID);
		}

		public ListItem FindItemWithIdentifier(int identifier)
		{
			return this.root.FindWithIdentifierRecurse(identifier);
		}

		public ListItem Add(ListItem parent, string name, Asset asset)
		{
			ListItem listItem = (parent == null) ? this.root : parent;
			ListItem listItem2 = new ListItem();
			listItem2.Name = name;
			listItem2.Asset = asset;
			listItem.Add(listItem2);
			ListItem twinAsset = this.GetTwinAsset(listItem2);
			if (twinAsset != null && listItem2.Asset != null && twinAsset.Asset.state == (listItem2.Asset.state & ~Asset.States.MetaFile))
			{
				listItem2.Hidden = true;
			}
			if (listItem2.Asset == null)
			{
				return listItem2;
			}
			if (listItem2.Asset.path.Length > 0)
			{
				this.pathSearch[listItem2.Asset.path.ToLower()] = listItem2;
			}
			return listItem2;
		}

		public ListItem Add(ListItem parent, string name, ChangeSet change)
		{
			ListItem listItem = (parent == null) ? this.root : parent;
			ListItem listItem2 = new ListItem();
			listItem2.Name = name;
			listItem2.Change = (change ?? new ChangeSet(name));
			listItem.Add(listItem2);
			this.pathSearch["_chkeyprfx_" + change.id.ToString()] = listItem2;
			return listItem2;
		}

		internal ListItem GetChangeSetItem(ChangeSet change)
		{
			if (change == null)
			{
				return null;
			}
			for (ListItem listItem = this.root.FirstChild; listItem != null; listItem = listItem.Next)
			{
				ChangeSet changeSet = listItem.Item as ChangeSet;
				if (changeSet != null && changeSet.id == change.id)
				{
					return listItem;
				}
			}
			return null;
		}

		public void Clear()
		{
			this.root.Clear();
			this.pathSearch.Clear();
			this.root.Name = "ROOT";
			this.root.Expanded = true;
		}

		public void Refresh()
		{
			this.Refresh(true);
		}

		public void Refresh(bool updateExpanded)
		{
			if (updateExpanded)
			{
				this.LoadExpanded(this.root);
				this.root.Name = "ROOT";
				this.root.Expanded = true;
				this.listState.Expanded.Clear();
				this.CallExpandedEvent(this.root, false);
			}
			this.SelectedRefresh();
		}

		public void Sync()
		{
			this.SelectedClear();
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				if (AssetDatabase.IsMainAsset(@object))
				{
					string str = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
					string path = str + AssetDatabase.GetAssetPath(@object);
					ListItem listItem = this.PathSearchFind(path);
					if (listItem != null)
					{
						this.SelectedAdd(listItem);
					}
				}
			}
		}

		public bool OnGUI(Rect area, bool focus)
		{
			bool flag = false;
			this.CreateResources();
			Event current = Event.current;
			int openCount = this.active.OpenCount;
			int num = (int)(area.height / 16f);
			if (current.type == EventType.ScrollWheel)
			{
				flag = true;
				this.listState.Scroll += current.delta.y;
				this.listState.Scroll = Mathf.Clamp(this.listState.Scroll, 0f, (float)(openCount - num));
			}
			if (openCount > num)
			{
				Rect position = new Rect(area.x + area.width - 14f, area.y, 14f, area.height);
				area.width -= 14f;
				float scroll = this.listState.Scroll;
				this.listState.Scroll = GUI.VerticalScrollbar(position, this.listState.Scroll, (float)num, 0f, (float)openCount);
				this.listState.Scroll = Mathf.Clamp(this.listState.Scroll, 0f, (float)(openCount - num));
				if (scroll != this.listState.Scroll)
				{
					flag = true;
				}
				if (!this.scrollVisible)
				{
					this.scrollVisible = true;
				}
			}
			else if (this.scrollVisible)
			{
				this.scrollVisible = false;
			}
			this.UpdateVisibleList(area, this.listState.Scroll);
			if (focus && !this.readOnly)
			{
				if (current.isKey)
				{
					flag = true;
					this.HandleKeyInput(current);
				}
				this.HandleSelectAll();
				flag = (this.HandleMouse(area) || flag);
				if (current.type == EventType.DragUpdated && area.Contains(current.mousePosition))
				{
					if (current.mousePosition.y < area.y + 16f)
					{
						this.listState.Scroll = Mathf.Clamp(this.listState.Scroll - 1f, 0f, (float)(openCount - num));
					}
					else if (current.mousePosition.y > area.y + area.height - 16f)
					{
						this.listState.Scroll = Mathf.Clamp(this.listState.Scroll + 1f, 0f, (float)(openCount - num));
					}
				}
			}
			this.DrawItems(area, focus);
			return flag;
		}

		private bool HandleMouse(Rect area)
		{
			Event current = Event.current;
			bool result = false;
			bool flag = area.Contains(current.mousePosition);
			if (current.type == EventType.MouseDown && flag)
			{
				result = true;
				this.dragCount = 0;
				GUIUtility.keyboardControl = 0;
				this.singleSelect = this.GetItemAt(area, current.mousePosition);
				if (this.singleSelect != null && !this.singleSelect.Dummy)
				{
					if (current.button == 0 && current.clickCount > 1 && this.singleSelect.Asset != null)
					{
						this.singleSelect.Asset.Edit();
					}
					if (current.button < 2)
					{
						float num = area.x + (float)((this.singleSelect.Indent - 1) * 18);
						if (current.mousePosition.x >= num && current.mousePosition.x < num + 16f && this.singleSelect.CanExpand)
						{
							this.singleSelect.Expanded = !this.singleSelect.Expanded;
							this.CallExpandedEvent(this.singleSelect, true);
							this.singleSelect = null;
						}
						else if (current.control || current.command)
						{
							if (current.button == 1)
							{
								this.SelectedAdd(this.singleSelect);
							}
							else
							{
								this.SelectedToggle(this.singleSelect);
							}
							this.singleSelect = null;
						}
						else if (current.shift)
						{
							this.SelectionFlow(this.singleSelect);
							this.singleSelect = null;
						}
						else if (!this.IsSelected(this.singleSelect))
						{
							this.SelectedSet(this.singleSelect);
							this.singleSelect = null;
						}
					}
				}
				else if (current.button == 0)
				{
					this.SelectedClear();
					this.singleSelect = null;
				}
			}
			else if ((current.type == EventType.MouseUp || current.type == EventType.ContextClick) && flag)
			{
				GUIUtility.keyboardControl = 0;
				this.singleSelect = this.GetItemAt(area, current.mousePosition);
				this.dragCount = 0;
				result = true;
				if (this.singleSelect != null && !this.singleSelect.Dummy)
				{
					if (current.type == EventType.ContextClick)
					{
						this.singleSelect = null;
						if (!this.IsSelectedAsset() && !string.IsNullOrEmpty(this.menuFolder))
						{
							ListControl.s_uniqueIDList[this.uniqueID] = this;
							EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), this.menuFolder, new MenuCommand(null, this.uniqueID));
						}
						else if (!string.IsNullOrEmpty(this.menuDefault))
						{
							ListControl.s_uniqueIDList[this.uniqueID] = this;
							EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), this.menuDefault, new MenuCommand(null, this.uniqueID));
						}
					}
					else if (current.type != EventType.ContextClick && current.button == 0 && !current.control && !current.command && !current.shift && this.IsSelected(this.singleSelect))
					{
						this.SelectedSet(this.singleSelect);
						this.singleSelect = null;
					}
				}
			}
			if (current.type == EventType.MouseDrag && flag)
			{
				this.dragCount++;
				if (this.dragCount > 2 && Selection.objects.Length > 0)
				{
					DragAndDrop.PrepareStartDrag();
					if (this.singleSelect != null)
					{
						DragAndDrop.objectReferences = new UnityEngine.Object[]
						{
							this.singleSelect.Asset.Load()
						};
					}
					else
					{
						DragAndDrop.objectReferences = Selection.objects;
					}
					DragAndDrop.StartDrag("Move");
				}
			}
			if (current.type == EventType.DragUpdated)
			{
				result = true;
				DragAndDrop.visualMode = DragAndDropVisualMode.Move;
				this.dragTarget = this.GetItemAt(area, current.mousePosition);
				if (this.dragTarget != null)
				{
					if (this.IsSelected(this.dragTarget))
					{
						this.dragTarget = null;
					}
					else if (this.dragAcceptOnly)
					{
						if (!this.dragTarget.CanAccept)
						{
							this.dragTarget = null;
						}
					}
					else
					{
						bool flag2 = !this.dragTarget.CanAccept || this.dragTarget.PrevOpenVisible != this.dragTarget.Parent;
						bool flag3 = !this.dragTarget.CanAccept || this.dragTarget.NextOpenVisible != this.dragTarget.FirstChild;
						float num2 = (!this.dragTarget.CanAccept) ? 8f : 2f;
						int num3 = (int)((current.mousePosition.y - area.y) / 16f);
						float num4 = area.y + (float)num3 * 16f;
						this.dragAdjust = ListControl.SelectDirection.Current;
						if (flag2 && current.mousePosition.y <= num4 + num2)
						{
							this.dragAdjust = ListControl.SelectDirection.Up;
						}
						else if (flag3 && current.mousePosition.y >= num4 + 16f - num2)
						{
							this.dragAdjust = ListControl.SelectDirection.Down;
						}
					}
				}
			}
			if (current.type == EventType.DragPerform && this.dragTarget != null)
			{
				ListItem listItem = (this.dragAdjust != ListControl.SelectDirection.Current) ? this.dragTarget.Parent : this.dragTarget;
				if (this.dragDelegate != null && listItem != null && listItem.CanAccept)
				{
					this.dragDelegate(listItem.Change);
				}
				this.dragTarget = null;
			}
			if (current.type == EventType.DragExited)
			{
				this.dragTarget = null;
			}
			return result;
		}

		private void DrawItems(Rect area, bool focus)
		{
			float num = area.y;
			foreach (ListItem current in this.visibleList)
			{
				float num2 = area.x + (float)((current.Indent - 1) * 18);
				bool selected = !this.readOnly && this.IsSelected(current);
				if (current.Parent != null && current.Parent.Parent != null && current.Parent.Parent.Parent == null)
				{
					num2 -= 16f;
				}
				this.DrawItem(current, area, num2, num, focus, selected);
				num += 16f;
			}
		}

		private void HandleSelectAll()
		{
			if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
			{
				Event.current.Use();
			}
			else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
			{
				this.SelectedAll();
				Event.current.Use();
			}
		}

		private void CreateResources()
		{
			if (this.blueTex == null)
			{
				this.blueTex = new Texture2D(1, 1);
				this.blueTex.SetPixel(0, 0, new Color(0.23f, 0.35f, 0.55f));
				this.blueTex.hideFlags = HideFlags.HideAndDontSave;
				this.blueTex.name = "BlueTex";
				this.blueTex.Apply();
			}
			if (this.greyTex == null)
			{
				this.greyTex = new Texture2D(1, 1);
				this.greyTex.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f));
				this.greyTex.hideFlags = HideFlags.HideAndDontSave;
				this.greyTex.name = "GrayTex";
				this.greyTex.Apply();
			}
			if (this.yellowTex == null)
			{
				this.yellowTex = new Texture2D(1, 1);
				this.yellowTex.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.2f));
				this.yellowTex.name = "YellowTex";
				this.yellowTex.hideFlags = HideFlags.HideAndDontSave;
				this.yellowTex.Apply();
			}
			if (this.defaultIcon == null)
			{
				this.defaultIcon = EditorGUIUtility.LoadIcon("vcs_document");
				this.defaultIcon.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		private void HandleKeyInput(Event e)
		{
			if (e.type != EventType.KeyDown)
			{
				return;
			}
			if (this.selectList.Count == 0)
			{
				return;
			}
			if (e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.DownArrow)
			{
				ListItem listItem;
				if (e.keyCode == KeyCode.UpArrow)
				{
					listItem = this.SelectedFirstIn(this.active);
					if (listItem != null)
					{
						listItem = listItem.PrevOpenSkip;
					}
				}
				else
				{
					listItem = this.SelectedLastIn(this.active);
					if (listItem != null)
					{
						listItem = listItem.NextOpenSkip;
					}
				}
				if (listItem != null)
				{
					if (!this.ScrollUpTo(listItem))
					{
						this.ScrollDownTo(listItem);
					}
					if (e.shift)
					{
						this.SelectionFlow(listItem);
					}
					else
					{
						this.SelectedSet(listItem);
					}
				}
			}
			if (e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow)
			{
				ListItem listItem2 = this.SelectedCurrentIn(this.active);
				listItem2.Expanded = (e.keyCode == KeyCode.RightArrow);
				this.CallExpandedEvent(listItem2, true);
			}
			if (e.keyCode == KeyCode.Return && GUIUtility.keyboardControl == 0)
			{
				ListItem listItem3 = this.SelectedCurrentIn(this.active);
				listItem3.Asset.Edit();
			}
		}

		private void DrawItem(ListItem item, Rect area, float x, float y, bool focus, bool selected)
		{
			bool flag = item == this.dragTarget;
			bool flag2 = selected;
			if (selected)
			{
				Texture2D image = (!focus) ? this.greyTex : this.blueTex;
				GUI.DrawTexture(new Rect(area.x, y, area.width, 16f), image, ScaleMode.StretchToFill, false);
			}
			else if (flag)
			{
				ListControl.SelectDirection selectDirection = this.dragAdjust;
				if (selectDirection != ListControl.SelectDirection.Up)
				{
					if (selectDirection != ListControl.SelectDirection.Down)
					{
						if (item.CanAccept)
						{
							GUI.DrawTexture(new Rect(area.x, y, area.width, 16f), this.yellowTex, ScaleMode.StretchToFill, false);
							flag2 = true;
						}
					}
					else
					{
						GUI.DrawTexture(new Rect(x, y + 16f - 1f, area.width, 2f), this.yellowTex, ScaleMode.StretchToFill, false);
					}
				}
				else if (item.PrevOpenVisible != item.Parent)
				{
					GUI.DrawTexture(new Rect(x, y - 1f, area.width, 2f), this.yellowTex, ScaleMode.StretchToFill, false);
				}
			}
			else if (this.dragTarget != null && item == this.dragTarget.Parent && this.dragAdjust != ListControl.SelectDirection.Current)
			{
				GUI.DrawTexture(new Rect(area.x, y, area.width, 16f), this.yellowTex, ScaleMode.StretchToFill, false);
				flag2 = true;
			}
			if (item.HasActions)
			{
				for (int i = 0; i < item.Actions.Length; i++)
				{
					this.calcSizeTmpContent.text = item.Actions[i];
					Vector2 vector = GUI.skin.button.CalcSize(this.calcSizeTmpContent);
					if (GUI.Button(new Rect(x, y, vector.x, 16f), item.Actions[i]))
					{
						this.actionDelegate(item, i);
					}
					x += vector.x + 4f;
				}
			}
			if (item.CanExpand)
			{
				EditorGUI.Foldout(new Rect(x, y, 16f, 16f), item.Expanded, GUIContent.none);
			}
			Texture texture = item.Icon;
			Color color = GUI.color;
			Color contentColor = GUI.contentColor;
			if (item.Dummy)
			{
				GUI.color = new Color(0.65f, 0.65f, 0.65f);
			}
			if (!item.Dummy)
			{
				if (texture == null)
				{
					texture = InternalEditorUtility.GetIconForFile(item.Name);
				}
				Rect rect = new Rect(x + 14f, y, 16f, 16f);
				if (texture != null)
				{
					GUI.DrawTexture(rect, texture);
				}
				if (item.Asset != null)
				{
					Rect itemRect = rect;
					itemRect.width += 12f;
					itemRect.x -= 6f;
					Overlay.DrawOverlay(item.Asset, itemRect);
				}
			}
			string text = this.DisplayName(item);
			Vector2 vector2 = EditorStyles.label.CalcSize(EditorGUIUtility.TempContent(text));
			float num = x + 32f;
			if (flag2)
			{
				GUI.contentColor = new Color(3f, 3f, 3f);
				GUI.Label(new Rect(num, y, area.width - num, 18f), text);
			}
			else
			{
				GUI.Label(new Rect(num, y, area.width - num, 18f), text);
			}
			if (this.HasHiddenMetaFile(item))
			{
				GUI.color = new Color(0.55f, 0.55f, 0.55f);
				float num2 = num + vector2.x + 2f;
				GUI.Label(new Rect(num2, y, area.width - num2, 18f), "+meta");
			}
			GUI.contentColor = contentColor;
			GUI.color = color;
		}

		private void UpdateVisibleList(Rect area, float scrollPos)
		{
			float num = area.y;
			float num2 = area.y + area.height - 16f;
			ListItem nextOpenVisible = this.active.NextOpenVisible;
			this.visibleList.Clear();
			for (float num3 = 0f; num3 < scrollPos; num3 += 1f)
			{
				if (nextOpenVisible == null)
				{
					return;
				}
				nextOpenVisible = nextOpenVisible.NextOpenVisible;
			}
			ListItem listItem = nextOpenVisible;
			while (listItem != null && num < num2)
			{
				this.visibleList.Add(listItem);
				num += 16f;
				listItem = listItem.NextOpenVisible;
			}
		}

		private ListItem GetItemAt(Rect area, Vector2 pos)
		{
			int num = (int)((pos.y - area.y) / 16f);
			if (num >= 0 && num < this.visibleList.Count)
			{
				return this.visibleList[num];
			}
			return null;
		}

		private bool ScrollUpTo(ListItem item)
		{
			int num = (int)this.listState.Scroll;
			ListItem listItem = (this.visibleList.Count <= 0) ? null : this.visibleList[0];
			while (listItem != null && num >= 0)
			{
				if (listItem == item)
				{
					this.listState.Scroll = (float)num;
					return true;
				}
				num--;
				listItem = listItem.PrevOpenVisible;
			}
			return false;
		}

		private bool ScrollDownTo(ListItem item)
		{
			int num = (int)this.listState.Scroll;
			ListItem listItem = (this.visibleList.Count <= 0) ? null : this.visibleList[this.visibleList.Count - 1];
			while (listItem != null && num >= 0)
			{
				if (listItem == item)
				{
					this.listState.Scroll = (float)num;
					return true;
				}
				num++;
				listItem = listItem.NextOpenVisible;
			}
			return false;
		}

		private void LoadExpanded(ListItem item)
		{
			if (item.Change != null)
			{
				item.Expanded = this.listState.Expanded.Contains(item.Change.id);
			}
			for (ListItem listItem = item.FirstChild; listItem != null; listItem = listItem.Next)
			{
				this.LoadExpanded(listItem);
			}
		}

		internal void ExpandLastItem()
		{
			if (this.root.LastChild != null)
			{
				this.root.LastChild.Expanded = true;
				this.CallExpandedEvent(this.root.LastChild, true);
			}
		}

		private void CallExpandedEvent(ListItem item, bool remove)
		{
			if (item.Change != null)
			{
				if (item.Expanded)
				{
					if (this.expandDelegate != null)
					{
						this.expandDelegate(item.Change, item);
					}
					this.listState.Expanded.Add(item.Change.id);
				}
				else if (remove)
				{
					this.listState.Expanded.Remove(item.Change.id);
				}
			}
			for (ListItem listItem = item.FirstChild; listItem != null; listItem = listItem.Next)
			{
				this.CallExpandedEvent(listItem, remove);
			}
		}

		private ListItem PathSearchFind(string path)
		{
			ListItem result;
			try
			{
				result = this.pathSearch[path.ToLower()];
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private void PathSearchUpdate(ListItem item)
		{
			if (item.Asset != null && item.Asset.path.Length > 0)
			{
				this.pathSearch.Add(item.Asset.path.ToLower(), item);
			}
			else if (item.Change != null)
			{
				this.pathSearch.Add("_chkeyprfx_" + item.Change.id.ToString(), item);
				return;
			}
			for (ListItem listItem = item.FirstChild; listItem != null; listItem = listItem.Next)
			{
				this.PathSearchUpdate(listItem);
			}
		}

		private bool IsSelected(ListItem item)
		{
			if (item.Asset != null)
			{
				return this.selectList.ContainsKey(item.Asset.path.ToLower());
			}
			return item.Change != null && this.selectList.ContainsKey("_chkeyprfx_" + item.Change.id.ToString());
		}

		private bool IsSelectedAsset()
		{
			foreach (KeyValuePair<string, ListItem> current in this.selectList)
			{
				if (current.Value != null && current.Value.Asset != null)
				{
					return true;
				}
			}
			return false;
		}

		private void SelectedClear()
		{
			this.selectList.Clear();
			Selection.activeObject = null;
			Selection.instanceIDs = new int[0];
		}

		private void SelectedRefresh()
		{
			Dictionary<string, ListItem> dictionary = new Dictionary<string, ListItem>();
			foreach (KeyValuePair<string, ListItem> current in this.selectList)
			{
				dictionary[current.Key] = this.PathSearchFind(current.Key);
			}
			this.selectList = dictionary;
		}

		public void SelectedSet(ListItem item)
		{
			if (item.Dummy)
			{
				return;
			}
			this.SelectedClear();
			if (item.Asset != null)
			{
				this.SelectedAdd(item);
			}
			else if (item.Change != null)
			{
				this.selectList["_chkeyprfx_" + item.Change.id.ToString()] = item;
			}
		}

		public void SelectedAll()
		{
			this.SelectedClear();
			this.SelectedAllHelper(this.Root);
		}

		private void SelectedAllHelper(ListItem _root)
		{
			for (ListItem listItem = _root.FirstChild; listItem != null; listItem = listItem.Next)
			{
				if (listItem.HasChildren)
				{
					this.SelectedAllHelper(listItem);
				}
				if (listItem.Asset != null)
				{
					this.SelectedAdd(listItem);
				}
			}
		}

		private ListItem GetTwinAsset(ListItem item)
		{
			ListItem prev = item.Prev;
			if (item.Name.EndsWith(".meta") && prev != null && prev.Asset != null && AssetDatabase.GetTextMetaFilePathFromAssetPath(prev.Asset.path).ToLower() == item.Asset.path.ToLower())
			{
				return prev;
			}
			return null;
		}

		private ListItem GetTwinMeta(ListItem item)
		{
			ListItem next = item.Next;
			if (!item.Name.EndsWith(".meta") && next != null && next.Asset != null && next.Asset.path.ToLower() == AssetDatabase.GetTextMetaFilePathFromAssetPath(item.Asset.path).ToLower())
			{
				return next;
			}
			return null;
		}

		private ListItem GetTwin(ListItem item)
		{
			ListItem twinAsset = this.GetTwinAsset(item);
			if (twinAsset != null)
			{
				return twinAsset;
			}
			return this.GetTwinMeta(item);
		}

		public void SelectedAdd(ListItem item)
		{
			if (item.Dummy)
			{
				return;
			}
			ListItem listItem = this.SelectedCurrentIn(this.active);
			if (item.Exclusive || (listItem != null && listItem.Exclusive))
			{
				this.SelectedSet(item);
				return;
			}
			string text = item.Asset.path.ToLower();
			int count = this.selectList.Count;
			this.selectList[text] = item;
			ListItem twin = this.GetTwin(item);
			if (twin != null)
			{
				this.selectList[twin.Asset.path.ToLower()] = twin;
			}
			if (count == this.selectList.Count)
			{
				return;
			}
			int[] instanceIDs = Selection.instanceIDs;
			int num = 0;
			if (instanceIDs != null)
			{
				num = instanceIDs.Length;
			}
			text = ((!text.EndsWith(".meta")) ? text : text.Substring(0, text.Length - 5));
			int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(text.TrimEnd(new char[]
			{
				'/'
			}));
			if (mainAssetInstanceID != 0)
			{
				int[] array = new int[num + 1];
				array[num] = mainAssetInstanceID;
				Array.Copy(instanceIDs, array, num);
				Selection.instanceIDs = array;
			}
		}

		private void SelectedRemove(ListItem item)
		{
			string text = item.Asset.path.ToLower();
			this.selectList.Remove(text);
			this.selectList.Remove((!text.EndsWith(".meta")) ? (text + ".meta") : text.Substring(0, text.Length - 5));
			text = ((!text.EndsWith(".meta")) ? text : text.Substring(0, text.Length - 5));
			int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(text.TrimEnd(new char[]
			{
				'/'
			}));
			int[] instanceIDs = Selection.instanceIDs;
			if (mainAssetInstanceID != 0 && instanceIDs.Length > 0)
			{
				int num = Array.IndexOf<int>(instanceIDs, mainAssetInstanceID);
				if (num < 0)
				{
					return;
				}
				int[] array = new int[instanceIDs.Length - 1];
				Array.Copy(instanceIDs, array, num);
				if (num < instanceIDs.Length - 1)
				{
					Array.Copy(instanceIDs, num + 1, array, num, instanceIDs.Length - num - 1);
				}
				Selection.instanceIDs = array;
			}
		}

		private void SelectedToggle(ListItem item)
		{
			if (this.IsSelected(item))
			{
				this.SelectedRemove(item);
			}
			else
			{
				this.SelectedAdd(item);
			}
		}

		private void SelectionFlow(ListItem item)
		{
			if (this.selectList.Count == 0)
			{
				this.SelectedSet(item);
			}
			else if (!this.SelectionFlowDown(item))
			{
				this.SelectionFlowUp(item);
			}
		}

		private bool SelectionFlowUp(ListItem item)
		{
			ListItem listItem = item;
			for (ListItem listItem2 = item; listItem2 != null; listItem2 = listItem2.PrevOpenVisible)
			{
				if (this.IsSelected(listItem2))
				{
					listItem = listItem2;
				}
			}
			if (item == listItem)
			{
				return false;
			}
			this.SelectedClear();
			this.SelectedAdd(listItem);
			for (ListItem listItem2 = item; listItem2 != listItem; listItem2 = listItem2.PrevOpenVisible)
			{
				this.SelectedAdd(listItem2);
			}
			return true;
		}

		private bool SelectionFlowDown(ListItem item)
		{
			ListItem listItem = item;
			for (ListItem listItem2 = item; listItem2 != null; listItem2 = listItem2.NextOpenVisible)
			{
				if (this.IsSelected(listItem2))
				{
					listItem = listItem2;
				}
			}
			if (item == listItem)
			{
				return false;
			}
			this.SelectedClear();
			this.SelectedAdd(listItem);
			for (ListItem listItem2 = item; listItem2 != listItem; listItem2 = listItem2.NextOpenVisible)
			{
				this.SelectedAdd(listItem2);
			}
			return true;
		}

		private ListItem SelectedCurrentIn(ListItem root)
		{
			foreach (KeyValuePair<string, ListItem> current in this.selectList)
			{
				if (current.Value.IsChildOf(root))
				{
					return current.Value;
				}
			}
			return null;
		}

		private ListItem SelectedFirstIn(ListItem root)
		{
			ListItem listItem = this.SelectedCurrentIn(root);
			for (ListItem listItem2 = listItem; listItem2 != null; listItem2 = listItem2.PrevOpenVisible)
			{
				if (this.IsSelected(listItem2))
				{
					listItem = listItem2;
				}
			}
			return listItem;
		}

		private ListItem SelectedLastIn(ListItem root)
		{
			ListItem listItem = this.SelectedCurrentIn(root);
			for (ListItem listItem2 = listItem; listItem2 != null; listItem2 = listItem2.NextOpenVisible)
			{
				if (this.IsSelected(listItem2))
				{
					listItem = listItem2;
				}
			}
			return listItem;
		}

		private string DisplayName(ListItem item)
		{
			string text = item.Name;
			string text2 = string.Empty;
			while (text2 == string.Empty)
			{
				int num = text.IndexOf('\n');
				if (num < 0)
				{
					break;
				}
				text2 = text.Substring(0, num).Trim();
				text = text.Substring(num + 1);
			}
			if (text2 != string.Empty)
			{
				text = text2;
			}
			text = text.Trim();
			if (text == string.Empty && item.Change != null)
			{
				text = item.Change.id.ToString() + " " + item.Change.description;
			}
			return text;
		}

		private bool HasHiddenMetaFile(ListItem item)
		{
			ListItem twinMeta = this.GetTwinMeta(item);
			return twinMeta != null && twinMeta.Hidden;
		}
	}
}
