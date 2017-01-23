using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASHistoryFileView
	{
		public enum SelectionType
		{
			None,
			All,
			Items,
			DeletedItemsRoot,
			DeletedItems
		}

		private class Styles
		{
			public GUIStyle foldout = "IN Foldout";

			public GUIStyle insertion = "PR Insertion";

			public GUIStyle label = "PR Label";

			public GUIStyle ping = new GUIStyle("PR Ping");

			public GUIStyle toolbarButton = "ToolbarButton";

			public Styles()
			{
				this.ping.overflow.left = -2;
				this.ping.overflow.right = -21;
				this.ping.padding.left = 48;
				this.ping.padding.right = 0;
			}
		}

		private int[] m_ExpandedArray = new int[0];

		public Vector2 m_ScrollPosition;

		private static float m_RowHeight = 16f;

		private float m_FoldoutSize = 14f;

		private float m_Indent = 16f;

		private float m_BaseIndent = 16f;

		private float m_SpaceAtTheTop = ASHistoryFileView.m_RowHeight + 6f;

		private static int ms_FileViewHash = "FileView".GetHashCode();

		private int m_FileViewControlID = -1;

		private static bool OSX = Application.platform == RuntimePlatform.OSXEditor;

		private ASHistoryFileView.SelectionType m_SelType = ASHistoryFileView.SelectionType.None;

		private bool m_DeletedItemsToggle = false;

		private DeletedAsset[] m_DeletedItems = null;

		private bool m_DeletedItemsInitialized = false;

		private ParentViewState m_DelPVstate = new ParentViewState();

		private Rect m_ScreenRect;

		private static ASHistoryFileView.Styles ms_Styles = null;

		private GUIContent[] dropDownMenuItems = new GUIContent[]
		{
			new GUIContent("Recover")
		};

		public ASHistoryFileView.SelectionType SelType
		{
			get
			{
				return this.m_SelType;
			}
			set
			{
				if (this.m_SelType == ASHistoryFileView.SelectionType.DeletedItems && value != ASHistoryFileView.SelectionType.DeletedItems)
				{
					for (int i = 0; i < this.m_DelPVstate.selectedItems.Length; i++)
					{
						this.m_DelPVstate.selectedItems[i] = false;
					}
				}
				this.m_SelType = value;
			}
		}

		private bool DeletedItemsToggle
		{
			get
			{
				return this.m_DeletedItemsToggle;
			}
			set
			{
				this.m_DeletedItemsToggle = value;
				if (this.m_DeletedItemsToggle && !this.m_DeletedItemsInitialized)
				{
					this.SetupDeletedItems();
				}
			}
		}

		public ASHistoryFileView()
		{
			this.m_DelPVstate.lv = new ListViewState(0);
			this.m_DelPVstate.lv.totalRows = 0;
		}

		private void SetupDeletedItems()
		{
			this.m_DeletedItems = AssetServer.GetServerDeletedItems();
			this.m_DelPVstate.Clear();
			this.m_DelPVstate.lv = new ListViewState(0);
			this.m_DelPVstate.AddAssetItems(this.m_DeletedItems);
			this.m_DelPVstate.AddAssetItems(AssetServer.GetLocalDeletedItems());
			this.m_DelPVstate.SetLineCount();
			this.m_DelPVstate.selectedItems = new bool[this.m_DelPVstate.lv.totalRows];
			this.m_DeletedItemsInitialized = true;
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				if (selected == 0)
				{
					this.DoRecover();
				}
			}
		}

		public void SelectDeletedItem(string guid)
		{
			this.SelType = ASHistoryFileView.SelectionType.DeletedItems;
			this.DeletedItemsToggle = true;
			int num = 0;
			for (int i = 0; i < this.m_DelPVstate.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = this.m_DelPVstate.folders[i];
				this.m_DelPVstate.selectedItems[num] = false;
				if (parentViewFolder.guid == guid)
				{
					this.m_DelPVstate.selectedItems[num] = true;
					this.ScrollToDeletedItem(num);
					break;
				}
				for (int j = 0; j < parentViewFolder.files.Length; j++)
				{
					num++;
					this.m_DelPVstate.selectedItems[num] = false;
					if (parentViewFolder.files[j].guid == guid)
					{
						this.m_DelPVstate.selectedItems[num] = true;
						this.ScrollToDeletedItem(num);
						return;
					}
				}
				num++;
			}
		}

		public void DoRecover()
		{
			string[] selectedDeletedItemGUIDs = this.GetSelectedDeletedItemGUIDs();
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = 0;
			for (int i = 0; i < selectedDeletedItemGUIDs.Length; i++)
			{
				for (int j = 0; j < this.m_DeletedItems.Length; j++)
				{
					if (this.m_DeletedItems[j].guid == selectedDeletedItemGUIDs[i])
					{
						dictionary[this.m_DeletedItems[j].guid] = j;
						break;
					}
				}
			}
			DeletedAsset[] array = new DeletedAsset[dictionary.Count];
			while (dictionary.Count != 0)
			{
				DeletedAsset deletedAsset = null;
				foreach (KeyValuePair<string, int> current in dictionary)
				{
					deletedAsset = this.m_DeletedItems[current.Value];
					if (!dictionary.ContainsKey(deletedAsset.parent))
					{
						array[num++] = deletedAsset;
						break;
					}
				}
				dictionary.Remove(deletedAsset.guid);
			}
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitASMainWindow");
			AssetServer.DoRecoverOnNextTick(array);
		}

		public string[] GetSelectedDeletedItemGUIDs()
		{
			List<string> list = new List<string>();
			int num = 0;
			for (int i = 0; i < this.m_DelPVstate.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = this.m_DelPVstate.folders[i];
				if (this.m_DelPVstate.selectedItems[num])
				{
					list.Add(parentViewFolder.guid);
				}
				for (int j = 0; j < parentViewFolder.files.Length; j++)
				{
					num++;
					if (this.m_DelPVstate.selectedItems[num])
					{
						list.Add(parentViewFolder.files[j].guid);
					}
				}
				num++;
			}
			return list.ToArray();
		}

		public string[] GetAllDeletedItemGUIDs()
		{
			if (!this.m_DeletedItemsInitialized)
			{
				this.SetupDeletedItems();
			}
			string[] array = new string[this.m_DeletedItems.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.m_DeletedItems[i].guid;
			}
			return array;
		}

		public void FilterItems(string filterText)
		{
		}

		private int ControlIDForProperty(HierarchyProperty property)
		{
			int result;
			if (property != null)
			{
				result = property.row + 10000000;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		private void SetExpanded(int instanceID, bool expand)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < this.m_ExpandedArray.Length; i++)
			{
				hashtable.Add(this.m_ExpandedArray[i], null);
			}
			if (expand != hashtable.Contains(instanceID))
			{
				if (expand)
				{
					hashtable.Add(instanceID, null);
				}
				else
				{
					hashtable.Remove(instanceID);
				}
				this.m_ExpandedArray = new int[hashtable.Count];
				int num = 0;
				IEnumerator enumerator = hashtable.Keys.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						int num2 = (int)enumerator.Current;
						this.m_ExpandedArray[num] = num2;
						num++;
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
			}
			InternalEditorUtility.expandedProjectWindowItems = this.m_ExpandedArray;
		}

		private void SetExpandedRecurse(int instanceID, bool expand)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			if (hierarchyProperty.Find(instanceID, this.m_ExpandedArray))
			{
				this.SetExpanded(instanceID, expand);
				int depth = hierarchyProperty.depth;
				while (hierarchyProperty.Next(null) && hierarchyProperty.depth > depth)
				{
					this.SetExpanded(hierarchyProperty.instanceID, expand);
				}
			}
		}

		private void SelectionClick(HierarchyProperty property)
		{
			if (EditorGUI.actionKey)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.AddRange(Selection.objects);
				if (arrayList.Contains(property.pptrValue))
				{
					arrayList.Remove(property.pptrValue);
				}
				else
				{
					arrayList.Add(property.pptrValue);
				}
				Selection.objects = (arrayList.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[]);
			}
			else if (Event.current.shift)
			{
				HierarchyProperty firstSelected = this.GetFirstSelected();
				HierarchyProperty lastSelected = this.GetLastSelected();
				if (!firstSelected.isValid)
				{
					Selection.activeObject = property.pptrValue;
					return;
				}
				HierarchyProperty hierarchyProperty;
				HierarchyProperty hierarchyProperty2;
				if (property.row > lastSelected.row)
				{
					hierarchyProperty = firstSelected;
					hierarchyProperty2 = property;
				}
				else
				{
					hierarchyProperty = property;
					hierarchyProperty2 = lastSelected;
				}
				ArrayList arrayList2 = new ArrayList();
				arrayList2.Add(hierarchyProperty.pptrValue);
				while (hierarchyProperty.Next(this.m_ExpandedArray))
				{
					arrayList2.Add(hierarchyProperty.pptrValue);
					if (hierarchyProperty.instanceID == hierarchyProperty2.instanceID)
					{
						break;
					}
				}
				Selection.objects = (arrayList2.ToArray(typeof(UnityEngine.Object)) as UnityEngine.Object[]);
			}
			else
			{
				Selection.activeObject = property.pptrValue;
			}
			this.SelType = ASHistoryFileView.SelectionType.Items;
			this.FrameObject(Selection.activeObject);
		}

		private HierarchyProperty GetActiveSelected()
		{
			return this.GetFirstSelected();
		}

		private HierarchyProperty GetFirstSelected()
		{
			int num = 1000000000;
			HierarchyProperty result = null;
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				if (hierarchyProperty.Find(@object.GetInstanceID(), this.m_ExpandedArray) && hierarchyProperty.row < num)
				{
					num = hierarchyProperty.row;
					result = hierarchyProperty;
				}
			}
			return result;
		}

		private HierarchyProperty GetLast()
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			int count = hierarchyProperty.CountRemaining(this.m_ExpandedArray);
			hierarchyProperty.Reset();
			HierarchyProperty result;
			if (hierarchyProperty.Skip(count, this.m_ExpandedArray))
			{
				result = hierarchyProperty;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private HierarchyProperty GetFirst()
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			HierarchyProperty result;
			if (hierarchyProperty.Next(this.m_ExpandedArray))
			{
				result = hierarchyProperty;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void OpenAssetSelection()
		{
			int[] instanceIDs = Selection.instanceIDs;
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				int instanceID = instanceIDs[i];
				if (AssetDatabase.Contains(instanceID))
				{
					AssetDatabase.OpenAsset(instanceID);
				}
			}
		}

		private HierarchyProperty GetLastSelected()
		{
			int num = -1;
			HierarchyProperty result = null;
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				if (hierarchyProperty.Find(@object.GetInstanceID(), this.m_ExpandedArray) && hierarchyProperty.row > num)
				{
					num = hierarchyProperty.row;
					result = hierarchyProperty;
				}
			}
			return result;
		}

		private void AllProjectKeyboard()
		{
			KeyCode keyCode = Event.current.keyCode;
			if (keyCode == KeyCode.DownArrow)
			{
				if (this.GetFirst() != null)
				{
					Selection.activeObject = this.GetFirst().pptrValue;
					this.FrameObject(Selection.activeObject);
					this.SelType = ASHistoryFileView.SelectionType.Items;
					Event.current.Use();
				}
			}
		}

		private void AssetViewKeyboard()
		{
			KeyCode keyCode = Event.current.keyCode;
			switch (keyCode)
			{
			case KeyCode.KeypadEnter:
				goto IL_1C6;
			case KeyCode.KeypadEquals:
			case KeyCode.Insert:
				IL_44:
				if (keyCode != KeyCode.Return)
				{
					return;
				}
				goto IL_1C6;
			case KeyCode.UpArrow:
			{
				Event.current.Use();
				HierarchyProperty firstSelected = this.GetFirstSelected();
				if (firstSelected != null)
				{
					if (firstSelected.instanceID == this.GetFirst().instanceID)
					{
						this.SelType = ASHistoryFileView.SelectionType.All;
						Selection.objects = new UnityEngine.Object[0];
						this.ScrollTo(0f);
					}
					else if (firstSelected.Previous(this.m_ExpandedArray))
					{
						UnityEngine.Object pptrValue = firstSelected.pptrValue;
						this.SelectionClick(firstSelected);
						this.FrameObject(pptrValue);
					}
				}
				goto IL_430;
			}
			case KeyCode.DownArrow:
			{
				Event.current.Use();
				HierarchyProperty lastSelected = this.GetLastSelected();
				if (Application.platform == RuntimePlatform.OSXEditor && Event.current.command)
				{
					this.OpenAssetSelection();
					GUIUtility.ExitGUI();
				}
				else if (lastSelected != null)
				{
					if (lastSelected.instanceID == this.GetLast().instanceID)
					{
						this.SelType = ASHistoryFileView.SelectionType.DeletedItemsRoot;
						Selection.objects = new UnityEngine.Object[0];
						this.ScrollToDeletedItem(-1);
					}
					else if (lastSelected.Next(this.m_ExpandedArray))
					{
						UnityEngine.Object pptrValue2 = lastSelected.pptrValue;
						this.SelectionClick(lastSelected);
						this.FrameObject(pptrValue2);
					}
				}
				goto IL_430;
			}
			case KeyCode.RightArrow:
			{
				HierarchyProperty activeSelected = this.GetActiveSelected();
				if (activeSelected != null)
				{
					this.SetExpanded(activeSelected.instanceID, true);
				}
				goto IL_430;
			}
			case KeyCode.LeftArrow:
			{
				HierarchyProperty activeSelected2 = this.GetActiveSelected();
				if (activeSelected2 != null)
				{
					this.SetExpanded(activeSelected2.instanceID, false);
				}
				goto IL_430;
			}
			case KeyCode.Home:
				if (this.GetFirst() != null)
				{
					Selection.activeObject = this.GetFirst().pptrValue;
					this.FrameObject(Selection.activeObject);
				}
				goto IL_430;
			case KeyCode.End:
				if (this.GetLast() != null)
				{
					Selection.activeObject = this.GetLast().pptrValue;
					this.FrameObject(Selection.activeObject);
				}
				goto IL_430;
			case KeyCode.PageUp:
				Event.current.Use();
				if (ASHistoryFileView.OSX)
				{
					this.m_ScrollPosition.y = this.m_ScrollPosition.y - this.m_ScreenRect.height;
					if (this.m_ScrollPosition.y < 0f)
					{
						this.m_ScrollPosition.y = 0f;
					}
				}
				else
				{
					HierarchyProperty hierarchyProperty = this.GetFirstSelected();
					if (hierarchyProperty != null)
					{
						int num = 0;
						while ((float)num < this.m_ScreenRect.height / ASHistoryFileView.m_RowHeight)
						{
							if (!hierarchyProperty.Previous(this.m_ExpandedArray))
							{
								hierarchyProperty = this.GetFirst();
								break;
							}
							num++;
						}
						UnityEngine.Object pptrValue3 = hierarchyProperty.pptrValue;
						this.SelectionClick(hierarchyProperty);
						this.FrameObject(pptrValue3);
					}
					else if (this.GetFirst() != null)
					{
						Selection.activeObject = this.GetFirst().pptrValue;
						this.FrameObject(Selection.activeObject);
					}
				}
				goto IL_430;
			case KeyCode.PageDown:
				Event.current.Use();
				if (ASHistoryFileView.OSX)
				{
					this.m_ScrollPosition.y = this.m_ScrollPosition.y + this.m_ScreenRect.height;
				}
				else
				{
					HierarchyProperty hierarchyProperty2 = this.GetLastSelected();
					if (hierarchyProperty2 != null)
					{
						int num2 = 0;
						while ((float)num2 < this.m_ScreenRect.height / ASHistoryFileView.m_RowHeight)
						{
							if (!hierarchyProperty2.Next(this.m_ExpandedArray))
							{
								hierarchyProperty2 = this.GetLast();
								break;
							}
							num2++;
						}
						UnityEngine.Object pptrValue4 = hierarchyProperty2.pptrValue;
						this.SelectionClick(hierarchyProperty2);
						this.FrameObject(pptrValue4);
					}
					else if (this.GetLast() != null)
					{
						Selection.activeObject = this.GetLast().pptrValue;
						this.FrameObject(Selection.activeObject);
					}
				}
				goto IL_430;
			}
			goto IL_44;
			IL_1C6:
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				this.OpenAssetSelection();
				GUIUtility.ExitGUI();
			}
			IL_430:
			Event.current.Use();
		}

		private void DeletedItemsRootKeyboard(ASHistoryWindow parentWin)
		{
			switch (Event.current.keyCode)
			{
			case KeyCode.UpArrow:
				this.SelType = ASHistoryFileView.SelectionType.Items;
				if (this.GetLast() != null)
				{
					Selection.activeObject = this.GetLast().pptrValue;
					this.FrameObject(Selection.activeObject);
				}
				break;
			case KeyCode.DownArrow:
				if (this.m_DelPVstate.selectedItems.Length > 0 && this.DeletedItemsToggle)
				{
					this.SelType = ASHistoryFileView.SelectionType.DeletedItems;
					this.m_DelPVstate.selectedItems[0] = true;
					this.m_DelPVstate.lv.row = 0;
					this.ScrollToDeletedItem(0);
				}
				break;
			case KeyCode.RightArrow:
				this.DeletedItemsToggle = true;
				break;
			case KeyCode.LeftArrow:
				this.DeletedItemsToggle = false;
				break;
			default:
				return;
			}
			if (this.SelType != ASHistoryFileView.SelectionType.Items)
			{
				parentWin.DoLocalSelectionChange();
			}
			Event.current.Use();
		}

		private void DeletedItemsKeyboard(ASHistoryWindow parentWin)
		{
			int row = this.m_DelPVstate.lv.row;
			int num = row;
			if (this.DeletedItemsToggle)
			{
				switch (Event.current.keyCode)
				{
				case KeyCode.UpArrow:
					if (num > 0)
					{
						num--;
					}
					else
					{
						this.SelType = ASHistoryFileView.SelectionType.DeletedItemsRoot;
						this.ScrollToDeletedItem(-1);
						parentWin.DoLocalSelectionChange();
					}
					goto IL_1C9;
				case KeyCode.DownArrow:
					if (num < this.m_DelPVstate.lv.totalRows - 1)
					{
						num++;
					}
					goto IL_1C9;
				case KeyCode.Home:
					num = 0;
					goto IL_1C9;
				case KeyCode.End:
					num = this.m_DelPVstate.lv.totalRows - 1;
					goto IL_1C9;
				case KeyCode.PageUp:
					if (ASHistoryFileView.OSX)
					{
						this.m_ScrollPosition.y = this.m_ScrollPosition.y - this.m_ScreenRect.height;
						if (this.m_ScrollPosition.y < 0f)
						{
							this.m_ScrollPosition.y = 0f;
						}
					}
					else
					{
						num -= (int)(this.m_ScreenRect.height / ASHistoryFileView.m_RowHeight);
						if (num < 0)
						{
							num = 0;
						}
					}
					goto IL_1C9;
				case KeyCode.PageDown:
					if (ASHistoryFileView.OSX)
					{
						this.m_ScrollPosition.y = this.m_ScrollPosition.y + this.m_ScreenRect.height;
					}
					else
					{
						num += (int)(this.m_ScreenRect.height / ASHistoryFileView.m_RowHeight);
						if (num > this.m_DelPVstate.lv.totalRows - 1)
						{
							num = this.m_DelPVstate.lv.totalRows - 1;
						}
					}
					goto IL_1C9;
				}
				return;
				IL_1C9:
				Event.current.Use();
				if (num != row)
				{
					this.m_DelPVstate.lv.row = num;
					ListViewShared.MultiSelection(null, row, num, ref this.m_DelPVstate.initialSelectedItem, ref this.m_DelPVstate.selectedItems);
					this.ScrollToDeletedItem(num);
					parentWin.DoLocalSelectionChange();
				}
			}
		}

		private void ScrollToDeletedItem(int index)
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			float num = this.m_SpaceAtTheTop + (float)hierarchyProperty.CountRemaining(this.m_ExpandedArray) * ASHistoryFileView.m_RowHeight + 6f;
			if (index == -1)
			{
				this.ScrollTo(num);
			}
			else
			{
				this.ScrollTo(num + (float)(index + 1) * ASHistoryFileView.m_RowHeight);
			}
		}

		private void KeyboardGUI(ASHistoryWindow parentWin)
		{
			if (Event.current.GetTypeForControl(this.m_FileViewControlID) == EventType.KeyDown && this.m_FileViewControlID == GUIUtility.keyboardControl)
			{
				switch (this.SelType)
				{
				case ASHistoryFileView.SelectionType.All:
					this.AllProjectKeyboard();
					break;
				case ASHistoryFileView.SelectionType.Items:
					this.AssetViewKeyboard();
					break;
				case ASHistoryFileView.SelectionType.DeletedItemsRoot:
					this.DeletedItemsRootKeyboard(parentWin);
					break;
				case ASHistoryFileView.SelectionType.DeletedItems:
					this.DeletedItemsKeyboard(parentWin);
					break;
				}
			}
		}

		private bool FrameObject(UnityEngine.Object target)
		{
			bool result;
			if (target == null)
			{
				result = false;
			}
			else
			{
				HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
				if (hierarchyProperty.Find(target.GetInstanceID(), null))
				{
					while (hierarchyProperty.Parent())
					{
						this.SetExpanded(hierarchyProperty.instanceID, true);
					}
				}
				hierarchyProperty.Reset();
				if (hierarchyProperty.Find(target.GetInstanceID(), this.m_ExpandedArray))
				{
					this.ScrollTo(ASHistoryFileView.m_RowHeight * (float)hierarchyProperty.row + this.m_SpaceAtTheTop);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void ScrollTo(float scrollTop)
		{
			float min = scrollTop - this.m_ScreenRect.height + ASHistoryFileView.m_RowHeight;
			this.m_ScrollPosition.y = Mathf.Clamp(this.m_ScrollPosition.y, min, scrollTop);
		}

		public void DoDeletedItemsGUI(ASHistoryWindow parentWin, Rect theRect, GUIStyle s, float offset, float endOffset, bool focused)
		{
			Event current = Event.current;
			Texture2D image = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			offset += 3f;
			Rect position = new Rect(this.m_Indent, offset, theRect.width - this.m_Indent, ASHistoryFileView.m_RowHeight);
			if (current.type == EventType.MouseDown && position.Contains(current.mousePosition))
			{
				GUIUtility.keyboardControl = this.m_FileViewControlID;
				this.SelType = ASHistoryFileView.SelectionType.DeletedItemsRoot;
				this.ScrollToDeletedItem(-1);
				parentWin.DoLocalSelectionChange();
			}
			position.width -= position.x;
			position.x = 0f;
			GUIContent gUIContent = new GUIContent("Deleted Assets");
			gUIContent.image = image;
			int left = (int)this.m_BaseIndent;
			s.padding.left = left;
			if (current.type == EventType.Repaint)
			{
				s.Draw(position, gUIContent, false, false, this.SelType == ASHistoryFileView.SelectionType.DeletedItemsRoot, focused);
			}
			Rect position2 = new Rect(this.m_BaseIndent - this.m_FoldoutSize, offset, this.m_FoldoutSize, ASHistoryFileView.m_RowHeight);
			if (!this.m_DeletedItemsInitialized || this.m_DelPVstate.lv.totalRows != 0)
			{
				this.DeletedItemsToggle = GUI.Toggle(position2, this.DeletedItemsToggle, GUIContent.none, ASHistoryFileView.ms_Styles.foldout);
			}
			offset += ASHistoryFileView.m_RowHeight;
			if (this.DeletedItemsToggle)
			{
				int row = this.m_DelPVstate.lv.row;
				int num = 0;
				int num2 = -1;
				int num3 = -1;
				int num4 = 0;
				while (offset <= endOffset && num4 < this.m_DelPVstate.lv.totalRows)
				{
					if (offset + ASHistoryFileView.m_RowHeight >= 0f)
					{
						if (num2 == -1)
						{
							this.m_DelPVstate.IndexToFolderAndFile(num4, ref num2, ref num3);
						}
						position = new Rect(0f, offset, (float)Screen.width, ASHistoryFileView.m_RowHeight);
						ParentViewFolder parentViewFolder = this.m_DelPVstate.folders[num2];
						if (current.type == EventType.MouseDown && position.Contains(current.mousePosition))
						{
							if (current.button != 1 || this.SelType != ASHistoryFileView.SelectionType.DeletedItems || !this.m_DelPVstate.selectedItems[num])
							{
								GUIUtility.keyboardControl = this.m_FileViewControlID;
								this.SelType = ASHistoryFileView.SelectionType.DeletedItems;
								this.m_DelPVstate.lv.row = num;
								ListViewShared.MultiSelection(null, row, this.m_DelPVstate.lv.row, ref this.m_DelPVstate.initialSelectedItem, ref this.m_DelPVstate.selectedItems);
								this.ScrollToDeletedItem(num);
								parentWin.DoLocalSelectionChange();
							}
							if (current.button == 1 && this.SelType == ASHistoryFileView.SelectionType.DeletedItems)
							{
								GUIUtility.hotControl = 0;
								Rect position3 = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
								EditorUtility.DisplayCustomMenu(position3, this.dropDownMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
							}
							Event.current.Use();
						}
						if (num3 != -1)
						{
							gUIContent.text = parentViewFolder.files[num3].name;
							gUIContent.image = InternalEditorUtility.GetIconForFile(parentViewFolder.files[num3].name);
							left = (int)(this.m_BaseIndent + this.m_Indent * 2f);
						}
						else
						{
							gUIContent.text = parentViewFolder.name;
							gUIContent.image = image;
							left = (int)(this.m_BaseIndent + this.m_Indent);
						}
						s.padding.left = left;
						if (Event.current.type == EventType.Repaint)
						{
							s.Draw(position, gUIContent, false, false, this.m_DelPVstate.selectedItems[num], focused);
						}
						this.m_DelPVstate.NextFileFolder(ref num2, ref num3);
						num++;
					}
					num4++;
					offset += ASHistoryFileView.m_RowHeight;
				}
			}
		}

		public void DoGUI(ASHistoryWindow parentWin, Rect theRect, bool focused)
		{
			if (ASHistoryFileView.ms_Styles == null)
			{
				ASHistoryFileView.ms_Styles = new ASHistoryFileView.Styles();
			}
			this.m_ScreenRect = theRect;
			Hashtable hashtable = new Hashtable();
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				hashtable.Add(@object.GetInstanceID(), null);
			}
			this.m_FileViewControlID = GUIUtility.GetControlID(ASHistoryFileView.ms_FileViewHash, FocusType.Passive);
			this.KeyboardGUI(parentWin);
			focused &= (GUIUtility.keyboardControl == this.m_FileViewControlID);
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			int num = hierarchyProperty.CountRemaining(this.m_ExpandedArray);
			int num2 = (!this.DeletedItemsToggle) ? 0 : this.m_DelPVstate.lv.totalRows;
			Rect viewRect = new Rect(0f, 0f, 1f, (float)(num + 2 + num2) * ASHistoryFileView.m_RowHeight + 16f);
			this.m_ScrollPosition = GUI.BeginScrollView(this.m_ScreenRect, this.m_ScrollPosition, viewRect);
			theRect.width = ((viewRect.height <= this.m_ScreenRect.height) ? theRect.width : (theRect.width - 18f));
			int num3 = Mathf.RoundToInt(this.m_ScrollPosition.y - 6f - ASHistoryFileView.m_RowHeight) / Mathf.RoundToInt(ASHistoryFileView.m_RowHeight);
			if (num3 > num)
			{
				num3 = num;
			}
			else if (num3 < 0)
			{
				num3 = 0;
				this.m_ScrollPosition.y = 0f;
			}
			GUIContent gUIContent = new GUIContent();
			Event current = Event.current;
			GUIStyle gUIStyle = new GUIStyle(ASHistoryFileView.ms_Styles.label);
			Texture2D image = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			float num4 = (float)num3 * ASHistoryFileView.m_RowHeight + 3f;
			float num5 = this.m_ScreenRect.height + this.m_ScrollPosition.y;
			Rect position = new Rect(0f, num4, theRect.width, ASHistoryFileView.m_RowHeight);
			if (current.type == EventType.MouseDown && position.Contains(current.mousePosition))
			{
				this.SelType = ASHistoryFileView.SelectionType.All;
				GUIUtility.keyboardControl = this.m_FileViewControlID;
				this.ScrollTo(0f);
				parentWin.DoLocalSelectionChange();
				current.Use();
			}
			gUIContent = new GUIContent("Entire Project");
			gUIContent.image = image;
			int left = (int)this.m_BaseIndent;
			gUIStyle.padding.left = 3;
			if (Event.current.type == EventType.Repaint)
			{
				gUIStyle.Draw(position, gUIContent, false, false, this.SelType == ASHistoryFileView.SelectionType.All, focused);
			}
			num4 += ASHistoryFileView.m_RowHeight + 3f;
			hierarchyProperty.Reset();
			hierarchyProperty.Skip(num3, this.m_ExpandedArray);
			while (hierarchyProperty.Next(this.m_ExpandedArray) && num4 <= num5)
			{
				int instanceID = hierarchyProperty.instanceID;
				position = new Rect(0f, num4, theRect.width, ASHistoryFileView.m_RowHeight);
				if (Event.current.type == EventType.Repaint)
				{
					gUIContent.text = hierarchyProperty.name;
					gUIContent.image = hierarchyProperty.icon;
					left = (int)(this.m_BaseIndent + this.m_Indent * (float)hierarchyProperty.depth);
					gUIStyle.padding.left = left;
					bool on = hashtable.Contains(instanceID);
					gUIStyle.Draw(position, gUIContent, false, false, on, focused);
				}
				if (hierarchyProperty.hasChildren)
				{
					bool flag = hierarchyProperty.IsExpanded(this.m_ExpandedArray);
					GUI.changed = false;
					Rect position2 = new Rect(this.m_BaseIndent + this.m_Indent * (float)hierarchyProperty.depth - this.m_FoldoutSize, num4, this.m_FoldoutSize, ASHistoryFileView.m_RowHeight);
					flag = GUI.Toggle(position2, flag, GUIContent.none, ASHistoryFileView.ms_Styles.foldout);
					if (GUI.changed)
					{
						if (Event.current.alt)
						{
							this.SetExpandedRecurse(instanceID, flag);
						}
						else
						{
							this.SetExpanded(instanceID, flag);
						}
					}
				}
				if (current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
				{
					GUIUtility.keyboardControl = this.m_FileViewControlID;
					if (Event.current.clickCount == 2)
					{
						AssetDatabase.OpenAsset(instanceID);
						GUIUtility.ExitGUI();
					}
					else if (position.Contains(current.mousePosition))
					{
						this.SelectionClick(hierarchyProperty);
					}
					current.Use();
				}
				num4 += ASHistoryFileView.m_RowHeight;
			}
			num4 += 3f;
			this.DoDeletedItemsGUI(parentWin, theRect, gUIStyle, num4, num5, focused);
			GUI.EndScrollView();
			EventType type = current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.MouseUp)
				{
					if (GUIUtility.hotControl == this.m_FileViewControlID)
					{
						if (this.m_ScreenRect.Contains(current.mousePosition))
						{
							Selection.activeObject = null;
						}
						GUIUtility.hotControl = 0;
						current.Use();
					}
				}
			}
			else if (current.button == 0 && this.m_ScreenRect.Contains(current.mousePosition))
			{
				GUIUtility.hotControl = this.m_FileViewControlID;
				current.Use();
			}
			this.HandleFraming();
		}

		private void HandleFraming()
		{
			if ((Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand) && Event.current.commandName == "FrameSelected")
			{
				if (Event.current.type == EventType.ExecuteCommand)
				{
					this.DoFramingMindSelectionType();
				}
				HandleUtility.Repaint();
				Event.current.Use();
			}
		}

		private void DoFramingMindSelectionType()
		{
			switch (this.m_SelType)
			{
			case ASHistoryFileView.SelectionType.All:
				this.ScrollTo(0f);
				break;
			case ASHistoryFileView.SelectionType.Items:
				this.FrameObject(Selection.activeObject);
				break;
			case ASHistoryFileView.SelectionType.DeletedItemsRoot:
				this.ScrollToDeletedItem(-1);
				break;
			case ASHistoryFileView.SelectionType.DeletedItems:
				this.ScrollToDeletedItem(this.m_DelPVstate.lv.row);
				break;
			}
		}

		private List<int> GetOneFolderImplicitSelection(HierarchyProperty property, Hashtable selection, bool rootSelected, ref bool retHasSelectionInside, out bool eof)
		{
			int depth = property.depth;
			bool flag = false;
			bool flag2 = false;
			eof = false;
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			do
			{
				if (property.depth > depth)
				{
					list3.AddRange(this.GetOneFolderImplicitSelection(property, selection, rootSelected || flag2, ref flag, out eof));
				}
				if (depth != property.depth || eof)
				{
					break;
				}
				if (rootSelected && !flag)
				{
					list.Add(property.instanceID);
				}
				if (selection.Contains(property.instanceID))
				{
					list2.Add(property.instanceID);
					flag = true;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
				eof = !property.Next(null);
			}
			while (!eof);
			retHasSelectionInside |= flag;
			List<int> list4 = (rootSelected && !flag) ? list : list2;
			list4.AddRange(list3);
			return list4;
		}

		public string[] GetImplicitProjectViewSelection()
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			bool flag = false;
			string[] result;
			if (!hierarchyProperty.Next(null))
			{
				result = new string[0];
			}
			else
			{
				Hashtable hashtable = new Hashtable();
				UnityEngine.Object[] objects = Selection.objects;
				for (int i = 0; i < objects.Length; i++)
				{
					UnityEngine.Object @object = objects[i];
					hashtable.Add(@object.GetInstanceID(), null);
				}
				bool flag2;
				List<int> oneFolderImplicitSelection = this.GetOneFolderImplicitSelection(hierarchyProperty, hashtable, false, ref flag, out flag2);
				string[] array = new string[oneFolderImplicitSelection.Count];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(oneFolderImplicitSelection[j]));
				}
				result = array;
			}
			return result;
		}
	}
}
