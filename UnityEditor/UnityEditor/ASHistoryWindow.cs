using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASHistoryWindow
	{
		internal class Constants
		{
			public GUIStyle selected = "ServerUpdateChangesetOn";

			public GUIStyle lvHeader = "OL title";

			public GUIStyle button = "Button";

			public GUIStyle label = "PR Label";

			public GUIStyle descriptionLabel = "Label";

			public GUIStyle entryEven = "CN EntryBackEven";

			public GUIStyle entryOdd = "CN EntryBackOdd";

			public GUIStyle boldLabel = "BoldLabel";

			public GUIStyle ping = new GUIStyle("PR Ping");

			public Constants()
			{
				this.ping.overflow.left = -2;
				this.ping.overflow.right = -21;
				this.ping.padding.left = 48;
				this.ping.padding.right = 0;
			}
		}

		[Serializable]
		private class GUIHistoryListItem
		{
			public GUIContent colAuthor;

			public GUIContent colRevision;

			public GUIContent colDate;

			public GUIContent colDescription;

			public ParentViewState assets;

			public int totalLineCount;

			public bool[] boldAssets;

			public int height;

			public bool inFilter;

			public int collapsedItemCount;

			public int startShowingFrom;
		}

		private static ASHistoryWindow.Constants ms_Style = null;

		private static int ms_HistoryControlHash = "HistoryControl".GetHashCode();

		private const int kFirst = -999999;

		private const int kLast = 999999;

		private const int kUncollapsedItemsCount = 5;

		private SplitterState m_HorSplit = new SplitterState(new float[]
		{
			30f,
			70f
		}, new int[]
		{
			60,
			100
		}, null);

		private static Vector2 ms_IconSize = new Vector2(16f, 16f);

		private bool m_NextSelectionMine = false;

		private ASHistoryWindow.GUIHistoryListItem[] m_GUIItems;

		private int m_TotalHeight = 0;

		private Vector2 m_ScrollPos = Vector2.zero;

		private bool m_SplittersOk = false;

		private int m_RowHeight = 16;

		private int m_HistoryControlID = -1;

		private int m_ChangesetSelectionIndex = -1;

		private int m_AssetSelectionIndex = -1;

		private int m_ChangeLogSelectionRev = -1;

		private bool m_BinaryDiff = false;

		private int m_Rev1ForCustomDiff = -1;

		private int m_ScrollViewHeight = 0;

		private string m_ChangeLogSelectionGUID = string.Empty;

		private string m_ChangeLogSelectionAssetName = string.Empty;

		private string m_SelectedPath = string.Empty;

		private string m_SelectedGUID = string.Empty;

		private bool m_FolderSelected = false;

		private bool m_InRevisionSelectMode = false;

		private static GUIContent emptyGUIContent = new GUIContent();

		private GUIContent[] m_DropDownMenuItems = new GUIContent[]
		{
			EditorGUIUtility.TextContent("Show History"),
			ASHistoryWindow.emptyGUIContent,
			EditorGUIUtility.TextContent("Compare to Local"),
			EditorGUIUtility.TextContent("Compare Binary to Local"),
			ASHistoryWindow.emptyGUIContent,
			EditorGUIUtility.TextContent("Compare to Another Revision"),
			EditorGUIUtility.TextContent("Compare Binary to Another Revision"),
			ASHistoryWindow.emptyGUIContent,
			EditorGUIUtility.TextContent("Download This File")
		};

		private GUIContent[] m_DropDownChangesetMenuItems = new GUIContent[]
		{
			EditorGUIUtility.TextContent("Revert Entire Project to This Changeset")
		};

		private EditorWindow m_ParentWindow = null;

		private Changeset[] m_Changesets;

		private ASHistoryFileView m_FileViewWin = new ASHistoryFileView();

		private int ChangeLogSelectionRev
		{
			get
			{
				return this.m_ChangeLogSelectionRev;
			}
			set
			{
				this.m_ChangeLogSelectionRev = value;
				if (this.m_InRevisionSelectMode)
				{
					this.FinishShowCustomDiff();
				}
			}
		}

		public ASHistoryWindow(EditorWindow parent)
		{
			this.m_ParentWindow = parent;
			ASEditorBackend.SettingsIfNeeded();
			if (Selection.objects.Length != 0)
			{
				this.m_FileViewWin.SelType = ASHistoryFileView.SelectionType.Items;
			}
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				switch (selected)
				{
				case 0:
					this.ShowAssetsHistory();
					break;
				case 2:
					this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
					break;
				case 3:
					this.DoShowDiff(true, this.ChangeLogSelectionRev, -1);
					break;
				case 5:
					this.DoShowCustomDiff(false);
					break;
				case 6:
					this.DoShowCustomDiff(true);
					break;
				case 8:
					this.DownloadFile();
					break;
				}
			}
		}

		private void DownloadFile()
		{
			if (this.ChangeLogSelectionRev >= 0 && !(this.m_ChangeLogSelectionGUID == string.Empty))
			{
				if (EditorUtility.DisplayDialog("Download file", string.Concat(new string[]
				{
					"Are you sure you want to download '",
					this.m_ChangeLogSelectionAssetName,
					"' from revision ",
					this.ChangeLogSelectionRev.ToString(),
					" and lose all changes?"
				}), "Download", "Cancel"))
				{
					AssetServer.DoRevertOnNextTick(this.ChangeLogSelectionRev, this.m_ChangeLogSelectionGUID);
				}
			}
		}

		private void ShowAssetsHistory()
		{
			if (AssetServer.IsAssetAvailable(this.m_ChangeLogSelectionGUID) != 0)
			{
				string[] selectionFromGUIDs = new string[]
				{
					this.m_ChangeLogSelectionGUID
				};
				this.m_FileViewWin.SelType = ASHistoryFileView.SelectionType.Items;
				AssetServer.SetSelectionFromGUIDs(selectionFromGUIDs);
			}
			else
			{
				this.m_FileViewWin.SelectDeletedItem(this.m_ChangeLogSelectionGUID);
				this.DoLocalSelectionChange();
			}
		}

		private void ChangesetContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				if (selected == 0)
				{
					this.DoRevertProject();
				}
			}
		}

		private void DoRevertProject()
		{
			if (this.ChangeLogSelectionRev > 0)
			{
				ASEditorBackend.ASWin.RevertProject(this.ChangeLogSelectionRev, this.m_Changesets);
			}
		}

		private int MarkBoldItemsBySelection(ASHistoryWindow.GUIHistoryListItem item)
		{
			List<string> list = new List<string>();
			ParentViewState assets = item.assets;
			int num = -1;
			int num2 = 0;
			int result;
			if (Selection.instanceIDs.Length == 0)
			{
				result = 0;
			}
			else
			{
				int[] instanceIDs = Selection.instanceIDs;
				for (int i = 0; i < instanceIDs.Length; i++)
				{
					int instanceID = instanceIDs[i];
					list.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID)));
				}
				for (int j = 0; j < assets.folders.Length; j++)
				{
					ParentViewFolder parentViewFolder = assets.folders[j];
					if (list.Contains(parentViewFolder.guid))
					{
						item.boldAssets[num2] = true;
						if (num == -1)
						{
							num = num2;
						}
					}
					num2++;
					for (int k = 0; k < parentViewFolder.files.Length; k++)
					{
						if (list.Contains(parentViewFolder.files[k].guid))
						{
							item.boldAssets[num2] = true;
							if (num == -1)
							{
								num = num2;
							}
						}
						num2++;
					}
				}
				result = num;
			}
			return result;
		}

		private int CheckParentViewInFilterAndMarkBoldItems(ASHistoryWindow.GUIHistoryListItem item, string text)
		{
			ParentViewState assets = item.assets;
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < assets.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = assets.folders[i];
				if (parentViewFolder.name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
				{
					item.boldAssets[num2] = true;
					if (num == -1)
					{
						num = num2;
					}
				}
				num2++;
				for (int j = 0; j < parentViewFolder.files.Length; j++)
				{
					if (parentViewFolder.files[j].name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
					{
						item.boldAssets[num2] = true;
						if (num == -1)
						{
							num = num2;
						}
					}
					num2++;
				}
			}
			return num;
		}

		private void MarkBoldItemsByGUID(string guid)
		{
			for (int i = 0; i < this.m_GUIItems.Length; i++)
			{
				ASHistoryWindow.GUIHistoryListItem gUIHistoryListItem = this.m_GUIItems[i];
				ParentViewState assets = gUIHistoryListItem.assets;
				int num = 0;
				gUIHistoryListItem.boldAssets = new bool[assets.GetLineCount()];
				for (int j = 0; j < assets.folders.Length; j++)
				{
					ParentViewFolder parentViewFolder = assets.folders[j];
					if (parentViewFolder.guid == guid)
					{
						gUIHistoryListItem.boldAssets[num] = true;
					}
					num++;
					for (int k = 0; k < parentViewFolder.files.Length; k++)
					{
						if (parentViewFolder.files[k].guid == guid)
						{
							gUIHistoryListItem.boldAssets[num] = true;
						}
						num++;
					}
				}
			}
		}

		public void FilterItems(bool recreateGUIItems)
		{
			this.m_TotalHeight = 0;
			if (this.m_Changesets == null || this.m_Changesets.Length == 0)
			{
				this.m_GUIItems = null;
			}
			else
			{
				if (recreateGUIItems)
				{
					this.m_GUIItems = new ASHistoryWindow.GUIHistoryListItem[this.m_Changesets.Length];
				}
				string filterText = ((ASMainWindow)this.m_ParentWindow).m_SearchField.FilterText;
				bool flag = filterText.Trim() == string.Empty;
				for (int i = 0; i < this.m_Changesets.Length; i++)
				{
					if (recreateGUIItems)
					{
						this.m_GUIItems[i] = new ASHistoryWindow.GUIHistoryListItem();
						this.m_GUIItems[i].colAuthor = new GUIContent(this.m_Changesets[i].owner);
						this.m_GUIItems[i].colRevision = new GUIContent(this.m_Changesets[i].changeset.ToString());
						this.m_GUIItems[i].colDate = new GUIContent(this.m_Changesets[i].date);
						this.m_GUIItems[i].colDescription = new GUIContent(this.m_Changesets[i].message);
						this.m_GUIItems[i].assets = new ParentViewState();
						this.m_GUIItems[i].assets.AddAssetItems(this.m_Changesets[i]);
						this.m_GUIItems[i].totalLineCount = this.m_GUIItems[i].assets.GetLineCount();
						this.m_GUIItems[i].height = this.m_RowHeight * (1 + this.m_GUIItems[i].totalLineCount) + 20 + (int)ASHistoryWindow.ms_Style.descriptionLabel.CalcHeight(this.m_GUIItems[i].colDescription, 3.40282347E+38f);
					}
					this.m_GUIItems[i].boldAssets = new bool[this.m_GUIItems[i].assets.GetLineCount()];
					int num = (!flag) ? this.CheckParentViewInFilterAndMarkBoldItems(this.m_GUIItems[i], filterText) : this.MarkBoldItemsBySelection(this.m_GUIItems[i]);
					this.m_GUIItems[i].inFilter = (flag || num != -1 || this.m_GUIItems[i].colDescription.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.m_GUIItems[i].colRevision.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.m_GUIItems[i].colAuthor.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0 || this.m_GUIItems[i].colDate.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0);
					if (recreateGUIItems && this.m_GUIItems[i].totalLineCount > 5)
					{
						this.m_GUIItems[i].collapsedItemCount = this.m_GUIItems[i].totalLineCount - 5 + 1;
						this.m_GUIItems[i].height = this.m_RowHeight * 6 + 20 + (int)ASHistoryWindow.ms_Style.descriptionLabel.CalcHeight(this.m_GUIItems[i].colDescription, 3.40282347E+38f);
					}
					this.m_GUIItems[i].startShowingFrom = 0;
					if (this.m_GUIItems[i].collapsedItemCount != 0 && this.m_GUIItems[i].totalLineCount > 5 && num >= 4)
					{
						if (num + 5 - 1 > this.m_GUIItems[i].totalLineCount)
						{
							this.m_GUIItems[i].startShowingFrom = this.m_GUIItems[i].totalLineCount - 5 + 1;
						}
						else
						{
							this.m_GUIItems[i].startShowingFrom = num;
						}
					}
					if (this.m_GUIItems[i].inFilter)
					{
						this.m_TotalHeight += this.m_GUIItems[i].height;
					}
				}
			}
		}

		private void UncollapseListItem(ref ASHistoryWindow.GUIHistoryListItem item)
		{
			int num = (item.collapsedItemCount - 1) * this.m_RowHeight;
			item.collapsedItemCount = 0;
			item.startShowingFrom = 0;
			item.height += num;
			this.m_TotalHeight += num;
		}

		private void ClearLV()
		{
			this.m_Changesets = new Changeset[0];
			this.m_TotalHeight = 5;
		}

		public void DoLocalSelectionChange()
		{
			if (this.m_NextSelectionMine)
			{
				this.m_NextSelectionMine = false;
			}
			else
			{
				UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
				string[] array = new string[0];
				switch (this.m_FileViewWin.SelType)
				{
				case ASHistoryFileView.SelectionType.All:
					if (Selection.objects.Length != 0)
					{
						Selection.objects = new UnityEngine.Object[0];
						this.m_NextSelectionMine = true;
					}
					this.m_SelectedPath = "";
					this.m_SelectedGUID = "";
					this.ClearLV();
					break;
				case ASHistoryFileView.SelectionType.Items:
					if (filtered.Length < 1)
					{
						this.m_SelectedPath = string.Empty;
						this.m_SelectedGUID = string.Empty;
						this.ClearLV();
						return;
					}
					this.m_SelectedPath = AssetDatabase.GetAssetPath(filtered[0]);
					this.m_SelectedGUID = AssetDatabase.AssetPathToGUID(this.m_SelectedPath);
					array = this.m_FileViewWin.GetImplicitProjectViewSelection();
					break;
				case ASHistoryFileView.SelectionType.DeletedItemsRoot:
					if (Selection.objects.Length != 0)
					{
						Selection.objects = new UnityEngine.Object[0];
						this.m_NextSelectionMine = true;
					}
					array = this.m_FileViewWin.GetAllDeletedItemGUIDs();
					if (array.Length == 0)
					{
						this.ClearLV();
						return;
					}
					break;
				case ASHistoryFileView.SelectionType.DeletedItems:
					if (Selection.objects.Length != 0)
					{
						Selection.objects = new UnityEngine.Object[0];
						this.m_NextSelectionMine = true;
					}
					array = this.m_FileViewWin.GetSelectedDeletedItemGUIDs();
					break;
				}
				this.m_Changesets = AssetServer.GetHistorySelected(array);
				if (this.m_Changesets != null)
				{
					this.FilterItems(true);
				}
				else
				{
					this.ClearLV();
				}
				if (array != null && this.m_GUIItems != null && array.Length == 1)
				{
					this.MarkBoldItemsByGUID(this.m_SelectedGUID);
				}
				this.m_ParentWindow.Repaint();
			}
		}

		public void OnSelectionChange()
		{
			if (Selection.objects.Length != 0)
			{
				this.m_FileViewWin.SelType = ASHistoryFileView.SelectionType.Items;
			}
			this.DoLocalSelectionChange();
		}

		private void DoShowDiff(bool binary, int ver1, int ver2)
		{
			List<string> list = new List<string>();
			List<CompareInfo> list2 = new List<CompareInfo>();
			if (ver2 == -1 && AssetDatabase.GUIDToAssetPath(this.m_ChangeLogSelectionGUID) == string.Empty)
			{
				Debug.Log("Cannot compare asset " + this.m_ChangeLogSelectionAssetName + " to local version because it does not exist.");
			}
			else
			{
				list.Add(this.m_ChangeLogSelectionGUID);
				list2.Add(new CompareInfo(ver1, ver2, (!binary) ? 0 : 1, (!binary) ? 1 : 0));
				Debug.Log(string.Concat(new string[]
				{
					"Comparing asset ",
					this.m_ChangeLogSelectionAssetName,
					" revisions ",
					ver1.ToString(),
					" and ",
					(ver2 != -1) ? ver2.ToString() : "Local"
				}));
				AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
			}
		}

		private void DoShowCustomDiff(bool binary)
		{
			this.ShowAssetsHistory();
			this.m_InRevisionSelectMode = true;
			this.m_BinaryDiff = binary;
			this.m_Rev1ForCustomDiff = this.ChangeLogSelectionRev;
		}

		private void FinishShowCustomDiff()
		{
			if (this.m_Rev1ForCustomDiff != this.ChangeLogSelectionRev)
			{
				this.DoShowDiff(this.m_BinaryDiff, this.m_Rev1ForCustomDiff, this.ChangeLogSelectionRev);
			}
			else
			{
				Debug.Log("You chose to compare to the same revision.");
			}
			this.m_InRevisionSelectMode = false;
		}

		private void CancelShowCustomDiff()
		{
			this.m_InRevisionSelectMode = false;
		}

		private bool IsComparableAssetSelected()
		{
			return !this.m_FolderSelected && this.m_ChangeLogSelectionGUID != string.Empty;
		}

		private void DrawBadge(Rect offset, ChangeFlags flags, GUIStyle style, GUIContent content, float textColWidth)
		{
			if (Event.current.type == EventType.Repaint)
			{
				GUIContent gUIContent = null;
				if (this.HasFlag(flags, ChangeFlags.Undeleted) || this.HasFlag(flags, ChangeFlags.Created))
				{
					gUIContent = ASMainWindow.constants.badgeNew;
				}
				else if (this.HasFlag(flags, ChangeFlags.Deleted))
				{
					gUIContent = ASMainWindow.constants.badgeDelete;
				}
				else if (this.HasFlag(flags, ChangeFlags.Renamed) || this.HasFlag(flags, ChangeFlags.Moved))
				{
					gUIContent = ASMainWindow.constants.badgeMove;
				}
				if (gUIContent != null)
				{
					float x = style.CalcSize(content).x;
					float x2;
					if (x > textColWidth - (float)gUIContent.image.width)
					{
						x2 = offset.xMax - (float)gUIContent.image.width - 5f;
					}
					else
					{
						x2 = textColWidth - (float)gUIContent.image.width;
					}
					Rect position = new Rect(x2, offset.y + offset.height / 2f - (float)(gUIContent.image.height / 2), (float)gUIContent.image.width, (float)gUIContent.image.height);
					EditorGUIUtility.SetIconSize(Vector2.zero);
					GUIStyle.none.Draw(position, gUIContent, false, false, false, false);
					EditorGUIUtility.SetIconSize(ASHistoryWindow.ms_IconSize);
				}
			}
		}

		private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
		{
			return (flagToCheck & flags) != ChangeFlags.None;
		}

		private void ClearItemSelection()
		{
			this.m_ChangeLogSelectionGUID = string.Empty;
			this.m_ChangeLogSelectionAssetName = string.Empty;
			this.m_FolderSelected = false;
			this.m_AssetSelectionIndex = -1;
		}

		private void DrawParentView(Rect r, ref ASHistoryWindow.GUIHistoryListItem item, int changesetIndex, bool hasFocus)
		{
			ParentViewState assets = item.assets;
			GUIContent gUIContent = new GUIContent();
			Texture2D image = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
			Event current = Event.current;
			hasFocus &= (this.m_HistoryControlID == GUIUtility.keyboardControl);
			r.height = (float)this.m_RowHeight;
			r.y += 3f;
			int num = -1;
			int num2 = (item.collapsedItemCount == 0) ? item.totalLineCount : 4;
			num2 += item.startShowingFrom;
			for (int i = 0; i < assets.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = assets.folders[i];
				gUIContent.text = parentViewFolder.name;
				gUIContent.image = image;
				num++;
				if (num == num2)
				{
					break;
				}
				if (num >= item.startShowingFrom)
				{
					GUIStyle label = ASHistoryWindow.ms_Style.label;
					if (current.type == EventType.MouseDown && r.Contains(current.mousePosition))
					{
						if (this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset && this.m_ChangeLogSelectionGUID == parentViewFolder.guid && EditorGUI.actionKey)
						{
							this.ClearItemSelection();
						}
						else
						{
							this.ChangeLogSelectionRev = this.m_Changesets[changesetIndex].changeset;
							this.m_ChangeLogSelectionGUID = parentViewFolder.guid;
							this.m_ChangeLogSelectionAssetName = parentViewFolder.name;
							this.m_FolderSelected = true;
							this.m_AssetSelectionIndex = num;
						}
						this.m_ChangesetSelectionIndex = changesetIndex;
						GUIUtility.keyboardControl = this.m_HistoryControlID;
						((ASMainWindow)this.m_ParentWindow).m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;
						if (current.clickCount == 2)
						{
							this.ShowAssetsHistory();
							GUIUtility.ExitGUI();
						}
						else if (current.button == 1)
						{
							GUIUtility.hotControl = 0;
							r = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
							EditorUtility.DisplayCustomMenu(r, this.m_DropDownMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
						}
						this.DoScroll();
						current.Use();
					}
					bool flag = this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset && this.m_ChangeLogSelectionGUID == parentViewFolder.guid;
					if (item.boldAssets[num] && !flag)
					{
						GUI.Label(r, string.Empty, ASHistoryWindow.ms_Style.ping);
					}
					if (Event.current.type == EventType.Repaint)
					{
						label.Draw(r, gUIContent, false, false, flag, hasFocus);
						this.DrawBadge(r, parentViewFolder.changeFlags, label, gUIContent, GUIClip.visibleRect.width - 150f);
					}
					r.y += (float)this.m_RowHeight;
				}
				ASHistoryWindow.ms_Style.label.padding.left += 16;
				ASHistoryWindow.ms_Style.boldLabel.padding.left += 16;
				try
				{
					for (int j = 0; j < parentViewFolder.files.Length; j++)
					{
						num++;
						if (num == num2)
						{
							break;
						}
						if (num >= item.startShowingFrom)
						{
							GUIStyle label = ASHistoryWindow.ms_Style.label;
							if (current.type == EventType.MouseDown && r.Contains(current.mousePosition))
							{
								if (this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset && this.m_ChangeLogSelectionGUID == parentViewFolder.files[j].guid && EditorGUI.actionKey)
								{
									this.ClearItemSelection();
								}
								else
								{
									this.ChangeLogSelectionRev = this.m_Changesets[changesetIndex].changeset;
									this.m_ChangeLogSelectionGUID = parentViewFolder.files[j].guid;
									this.m_ChangeLogSelectionAssetName = parentViewFolder.files[j].name;
									this.m_FolderSelected = false;
									this.m_AssetSelectionIndex = num;
								}
								this.m_ChangesetSelectionIndex = changesetIndex;
								GUIUtility.keyboardControl = this.m_HistoryControlID;
								((ASMainWindow)this.m_ParentWindow).m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;
								if (current.clickCount == 2)
								{
									if (this.IsComparableAssetSelected() && this.m_SelectedGUID == this.m_ChangeLogSelectionGUID)
									{
										this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
									}
									else
									{
										this.ShowAssetsHistory();
										GUIUtility.ExitGUI();
									}
								}
								else if (current.button == 1)
								{
									GUIUtility.hotControl = 0;
									r = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
									EditorUtility.DisplayCustomMenu(r, this.m_DropDownMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
								}
								this.DoScroll();
								current.Use();
							}
							gUIContent.text = parentViewFolder.files[j].name;
							gUIContent.image = InternalEditorUtility.GetIconForFile(parentViewFolder.files[j].name);
							bool flag2 = this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset && this.m_ChangeLogSelectionGUID == parentViewFolder.files[j].guid;
							if (item.boldAssets[num] && !flag2)
							{
								GUI.Label(r, string.Empty, ASHistoryWindow.ms_Style.ping);
							}
							if (Event.current.type == EventType.Repaint)
							{
								label.Draw(r, gUIContent, false, false, flag2, hasFocus);
								this.DrawBadge(r, parentViewFolder.files[j].changeFlags, label, gUIContent, GUIClip.visibleRect.width - 150f);
							}
							r.y += (float)this.m_RowHeight;
						}
					}
					if (num == num2)
					{
						break;
					}
				}
				finally
				{
					ASHistoryWindow.ms_Style.label.padding.left -= 16;
					ASHistoryWindow.ms_Style.boldLabel.padding.left -= 16;
				}
			}
			if ((num == num2 || num2 >= item.totalLineCount) && item.collapsedItemCount != 0)
			{
				r.x += 19f;
				if (GUI.Button(r, item.collapsedItemCount.ToString() + " more...", EditorStyles.foldout))
				{
					GUIUtility.keyboardControl = this.m_HistoryControlID;
					this.UncollapseListItem(ref item);
				}
			}
		}

		private int FindFirstUnfilteredItem(int fromIndex, int direction)
		{
			int num = fromIndex;
			int result;
			while (num >= 0 && num < this.m_GUIItems.Length)
			{
				if (this.m_GUIItems[num].inFilter)
				{
					result = num;
					return result;
				}
				num += direction;
			}
			result = -1;
			return result;
		}

		private void MoveSelection(int steps)
		{
			if (this.m_ChangeLogSelectionGUID == string.Empty)
			{
				int num = (int)Mathf.Sign((float)steps);
				steps = Mathf.Abs(steps);
				for (int i = 0; i < steps; i++)
				{
					int num2 = this.FindFirstUnfilteredItem(this.m_ChangesetSelectionIndex + num, num);
					if (num2 == -1)
					{
						break;
					}
					this.m_ChangesetSelectionIndex = num2;
				}
				this.ChangeLogSelectionRev = this.m_Changesets[this.m_ChangesetSelectionIndex].changeset;
			}
			else
			{
				this.m_AssetSelectionIndex += steps;
				if (this.m_AssetSelectionIndex < this.m_GUIItems[this.m_ChangesetSelectionIndex].startShowingFrom)
				{
					this.m_AssetSelectionIndex = this.m_GUIItems[this.m_ChangesetSelectionIndex].startShowingFrom;
				}
				else
				{
					int lineCount = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.GetLineCount();
					if (this.m_AssetSelectionIndex >= 4 + this.m_GUIItems[this.m_ChangesetSelectionIndex].startShowingFrom && this.m_GUIItems[this.m_ChangesetSelectionIndex].collapsedItemCount != 0)
					{
						this.UncollapseListItem(ref this.m_GUIItems[this.m_ChangesetSelectionIndex]);
					}
					if (this.m_AssetSelectionIndex >= lineCount)
					{
						this.m_AssetSelectionIndex = lineCount - 1;
					}
				}
				int num3 = 0;
				int num4 = 0;
				if (this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.IndexToFolderAndFile(this.m_AssetSelectionIndex, ref num3, ref num4))
				{
					if (num4 == -1)
					{
						this.m_ChangeLogSelectionGUID = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[num3].guid;
					}
					else
					{
						this.m_ChangeLogSelectionGUID = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[num3].files[num4].guid;
					}
				}
			}
		}

		private void HandleWebLikeKeyboard()
		{
			Event current = Event.current;
			if (current.GetTypeForControl(this.m_HistoryControlID) == EventType.KeyDown && this.m_GUIItems.Length != 0)
			{
				KeyCode keyCode = current.keyCode;
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
					goto IL_73;
				case KeyCode.KeypadEquals:
				case KeyCode.Insert:
					IL_66:
					if (keyCode != KeyCode.Return)
					{
						return;
					}
					goto IL_73;
				case KeyCode.UpArrow:
					this.MoveSelection(-1);
					goto IL_2BB;
				case KeyCode.DownArrow:
					this.MoveSelection(1);
					goto IL_2BB;
				case KeyCode.RightArrow:
					if (this.m_ChangeLogSelectionGUID == string.Empty && this.m_GUIItems.Length > 0)
					{
						this.m_ChangeLogSelectionGUID = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[0].guid;
						this.m_ChangeLogSelectionAssetName = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[0].name;
						this.m_FolderSelected = true;
						this.m_AssetSelectionIndex = 0;
					}
					goto IL_2BB;
				case KeyCode.LeftArrow:
					this.m_ChangeLogSelectionGUID = string.Empty;
					goto IL_2BB;
				case KeyCode.Home:
					if (this.m_ChangeLogSelectionGUID == string.Empty)
					{
						int num = this.FindFirstUnfilteredItem(0, 1);
						if (num != -1)
						{
							this.m_ChangesetSelectionIndex = num;
						}
						this.ChangeLogSelectionRev = this.m_Changesets[this.m_ChangesetSelectionIndex].changeset;
					}
					else
					{
						this.MoveSelection(-999999);
					}
					goto IL_2BB;
				case KeyCode.End:
					if (this.m_ChangeLogSelectionGUID == string.Empty)
					{
						int num2 = this.FindFirstUnfilteredItem(this.m_GUIItems.Length - 1, -1);
						if (num2 != -1)
						{
							this.m_ChangesetSelectionIndex = num2;
						}
						this.ChangeLogSelectionRev = this.m_Changesets[this.m_ChangesetSelectionIndex].changeset;
					}
					else
					{
						this.MoveSelection(999999);
					}
					goto IL_2BB;
				case KeyCode.PageUp:
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						this.m_ScrollPos.y = this.m_ScrollPos.y - (float)this.m_ScrollViewHeight;
						if (this.m_ScrollPos.y < 0f)
						{
							this.m_ScrollPos.y = 0f;
						}
					}
					else
					{
						this.MoveSelection(-Mathf.RoundToInt((float)(this.m_ScrollViewHeight / this.m_RowHeight)));
					}
					goto IL_2BB;
				case KeyCode.PageDown:
					if (Application.platform == RuntimePlatform.OSXEditor)
					{
						this.m_ScrollPos.y = this.m_ScrollPos.y + (float)this.m_ScrollViewHeight;
					}
					else
					{
						this.MoveSelection(Mathf.RoundToInt((float)(this.m_ScrollViewHeight / this.m_RowHeight)));
					}
					goto IL_2BB;
				}
				goto IL_66;
				IL_73:
				if (this.IsComparableAssetSelected())
				{
					this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
				}
				IL_2BB:
				this.DoScroll();
				current.Use();
			}
		}

		private void WebLikeHistory(bool hasFocus)
		{
			if (this.m_Changesets == null)
			{
				this.m_Changesets = new Changeset[0];
			}
			if (this.m_GUIItems != null)
			{
				this.m_HistoryControlID = GUIUtility.GetControlID(ASHistoryWindow.ms_HistoryControlHash, FocusType.Passive);
				this.HandleWebLikeKeyboard();
				Event current = Event.current;
				EventType typeForControl = current.GetTypeForControl(this.m_HistoryControlID);
				if (typeForControl == EventType.ValidateCommand)
				{
					current.Use();
				}
				else
				{
					GUILayout.Space(1f);
					this.m_ScrollPos = GUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
					int num = 0;
					GUILayoutUtility.GetRect(1f, (float)(this.m_TotalHeight - 1));
					if ((current.type == EventType.Repaint || current.type == EventType.MouseDown || current.type == EventType.MouseUp) && this.m_GUIItems != null)
					{
						for (int i = 0; i < this.m_Changesets.Length; i++)
						{
							if (this.m_GUIItems[i].inFilter)
							{
								if ((float)(num + this.m_GUIItems[i].height) > GUIClip.visibleRect.y && (float)num < GUIClip.visibleRect.yMax)
								{
									float num2 = ASHistoryWindow.ms_Style.descriptionLabel.CalcHeight(this.m_GUIItems[i].colDescription, 3.40282347E+38f);
									Rect rect;
									if (current.type == EventType.Repaint)
									{
										if (this.ChangeLogSelectionRev == this.m_Changesets[i].changeset && Event.current.type == EventType.Repaint)
										{
											rect = new Rect(0f, (float)num, GUIClip.visibleRect.width, (float)(this.m_GUIItems[i].height - 10));
											ASHistoryWindow.ms_Style.selected.Draw(rect, false, false, false, false);
										}
										rect = new Rect(0f, (float)(num + 3), GUIClip.visibleRect.width, (float)this.m_GUIItems[i].height);
										GUI.Label(rect, this.m_GUIItems[i].colAuthor, ASHistoryWindow.ms_Style.boldLabel);
										rect = new Rect(GUIClip.visibleRect.width - 160f, (float)(num + 3), 60f, (float)this.m_GUIItems[i].height);
										GUI.Label(rect, this.m_GUIItems[i].colRevision, ASHistoryWindow.ms_Style.boldLabel);
										rect.x += 60f;
										rect.width = 100f;
										GUI.Label(rect, this.m_GUIItems[i].colDate, ASHistoryWindow.ms_Style.boldLabel);
										rect.x = (float)ASHistoryWindow.ms_Style.boldLabel.margin.left;
										rect.y += (float)this.m_RowHeight;
										rect.width = GUIClip.visibleRect.width;
										rect.height = num2;
										GUI.Label(rect, this.m_GUIItems[i].colDescription, ASHistoryWindow.ms_Style.descriptionLabel);
										rect.y += num2;
									}
									rect = new Rect(0f, (float)num + num2 + (float)this.m_RowHeight, GUIClip.visibleRect.width, (float)this.m_GUIItems[i].height - num2 - (float)this.m_RowHeight);
									this.DrawParentView(rect, ref this.m_GUIItems[i], i, hasFocus);
									if (current.type == EventType.MouseDown)
									{
										rect = new Rect(0f, (float)num, GUIClip.visibleRect.width, (float)(this.m_GUIItems[i].height - 10));
										if (rect.Contains(current.mousePosition))
										{
											this.ChangeLogSelectionRev = this.m_Changesets[i].changeset;
											this.m_ChangesetSelectionIndex = i;
											GUIUtility.keyboardControl = this.m_HistoryControlID;
											((ASMainWindow)this.m_ParentWindow).m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;
											if (current.button == 1)
											{
												GUIUtility.hotControl = 0;
												rect = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
												EditorUtility.DisplayCustomMenu(rect, this.m_DropDownChangesetMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ChangesetContextMenuClick), null);
												Event.current.Use();
											}
											this.DoScroll();
											current.Use();
										}
									}
								}
								num += this.m_GUIItems[i].height;
							}
						}
					}
					else if (this.m_GUIItems == null)
					{
						GUILayout.Label(EditorGUIUtility.TextContent("This item is not yet committed to the Asset Server"), new GUILayoutOption[0]);
					}
					if (Event.current.type == EventType.Repaint)
					{
						this.m_ScrollViewHeight = (int)GUIClip.visibleRect.height;
					}
					GUILayout.EndScrollView();
				}
			}
		}

		private void DoScroll()
		{
			int num = 0;
			int i;
			for (i = 0; i < this.m_ChangesetSelectionIndex; i++)
			{
				if (this.m_GUIItems[i].inFilter)
				{
					num += this.m_GUIItems[i].height;
				}
			}
			float num2;
			float min;
			if (this.m_ChangeLogSelectionGUID != string.Empty)
			{
				num2 = (float)(num + (2 + this.m_AssetSelectionIndex) * this.m_RowHeight + 5);
				min = num2 - (float)this.m_ScrollViewHeight + (float)this.m_RowHeight;
			}
			else
			{
				num2 = (float)num;
				min = num2 - (float)this.m_ScrollViewHeight + (float)this.m_GUIItems[i].height - 10f;
			}
			this.m_ScrollPos.y = Mathf.Clamp(this.m_ScrollPos.y, min, num2);
		}

		public bool DoGUI(bool hasFocus)
		{
			bool enabled = GUI.enabled;
			if (ASHistoryWindow.ms_Style == null)
			{
				ASHistoryWindow.ms_Style = new ASHistoryWindow.Constants();
				ASHistoryWindow.ms_Style.entryEven = new GUIStyle(ASHistoryWindow.ms_Style.entryEven);
				ASHistoryWindow.ms_Style.entryEven.padding.left = 3;
				ASHistoryWindow.ms_Style.entryOdd = new GUIStyle(ASHistoryWindow.ms_Style.entryOdd);
				ASHistoryWindow.ms_Style.entryOdd.padding.left = 3;
				ASHistoryWindow.ms_Style.label = new GUIStyle(ASHistoryWindow.ms_Style.label);
				ASHistoryWindow.ms_Style.boldLabel = new GUIStyle(ASHistoryWindow.ms_Style.boldLabel);
				ASHistoryWindow.ms_Style.label.padding.left = 3;
				ASHistoryWindow.ms_Style.boldLabel.padding.left = 3;
				ASHistoryWindow.ms_Style.boldLabel.padding.top = 0;
				ASHistoryWindow.ms_Style.boldLabel.padding.bottom = 0;
				this.DoLocalSelectionChange();
			}
			EditorGUIUtility.SetIconSize(ASHistoryWindow.ms_IconSize);
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				this.CancelShowCustomDiff();
				Event.current.Use();
			}
			SplitterGUILayout.BeginHorizontalSplit(this.m_HorSplit, new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(0f, 0f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			this.m_FileViewWin.DoGUI(this, rect, hasFocus);
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			this.WebLikeHistory(hasFocus);
			GUILayout.EndVertical();
			SplitterGUILayout.EndHorizontalSplit();
			if (Event.current.type == EventType.Repaint)
			{
				Handles.color = Color.black;
				Handles.DrawLine(new Vector3((float)(this.m_HorSplit.realSizes[0] - 1), rect.y, 0f), new Vector3((float)(this.m_HorSplit.realSizes[0] - 1), rect.yMax, 0f));
				Handles.DrawLine(new Vector3(0f, rect.yMax, 0f), new Vector3((float)Screen.width, rect.yMax, 0f));
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = (this.m_FileViewWin.SelType == ASHistoryFileView.SelectionType.DeletedItems && enabled);
			if (GUILayout.Button(EditorGUIUtility.TextContent("Recover"), ASHistoryWindow.ms_Style.button, new GUILayoutOption[0]))
			{
				this.m_FileViewWin.DoRecover();
			}
			GUILayout.FlexibleSpace();
			if (this.m_InRevisionSelectMode)
			{
				GUI.enabled = enabled;
				GUILayout.Label(EditorGUIUtility.TextContent("Select revision to compare to"), ASHistoryWindow.ms_Style.boldLabel, new GUILayoutOption[0]);
			}
			GUILayout.Space(10f);
			GUI.enabled = (this.IsComparableAssetSelected() && enabled);
			if (GUILayout.Button(EditorGUIUtility.TextContent("Compare to Local Version"), ASHistoryWindow.ms_Style.button, new GUILayoutOption[0]))
			{
				this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
				GUIUtility.ExitGUI();
			}
			GUI.enabled = (this.ChangeLogSelectionRev > 0 && this.m_ChangeLogSelectionGUID != string.Empty && enabled);
			if (GUILayout.Button(EditorGUIUtility.TextContent("Download Selected File"), ASHistoryWindow.ms_Style.button, new GUILayoutOption[0]))
			{
				this.DownloadFile();
			}
			GUILayout.Space(10f);
			GUI.enabled = (this.ChangeLogSelectionRev > 0 && enabled);
			if (GUILayout.Button((this.ChangeLogSelectionRev <= 0) ? "Revert Entire Project" : ("Revert Entire Project to " + this.ChangeLogSelectionRev), ASHistoryWindow.ms_Style.button, new GUILayoutOption[0]))
			{
				this.DoRevertProject();
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (!this.m_SplittersOk && Event.current.type == EventType.Repaint)
			{
				this.m_SplittersOk = true;
				HandleUtility.Repaint();
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			return true;
		}
	}
}
