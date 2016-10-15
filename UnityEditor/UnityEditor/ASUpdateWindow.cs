using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASUpdateWindow
	{
		internal class Constants
		{
			public GUIStyle box = "OL Box";

			public GUIStyle entrySelected = "ServerUpdateChangesetOn";

			public GUIStyle entryNormal = "ServerUpdateChangeset";

			public GUIStyle serverUpdateLog = "ServerUpdateLog";

			public GUIStyle serverChangeCount = "ServerChangeCount";

			public GUIStyle title = "OL title";

			public GUIStyle element = "OL elem";

			public GUIStyle header = "OL header";

			public GUIStyle serverUpdateInfo = "ServerUpdateInfo";

			public GUIStyle button = "Button";

			public GUIStyle errorLabel = "ErrorLabel";

			public GUIStyle bigButton = "LargeButton";

			public GUIStyle wwText = "AS TextArea";

			public GUIStyle entryEven = "CN EntryBackEven";

			public GUIStyle entryOdd = "CN EntryBackOdd";
		}

		private ASUpdateWindow.Constants constants;

		private ASUpdateConflictResolveWindow asResolveWin;

		private ASMainWindow parentWin;

		private string[] dropDownMenuItems = new string[]
		{
			"Compare",
			"Compare Binary"
		};

		private Changeset[] changesets;

		private Vector2 iconSize = new Vector2(16f, 16f);

		private string[] messageFirstLines;

		private int maxNickLength;

		private string selectedGUID = string.Empty;

		private bool isDirSelected;

		private ListViewState lv;

		private ParentViewState pv = new ParentViewState();

		private SplitterState horSplit = new SplitterState(new float[]
		{
			50f,
			50f
		}, new int[]
		{
			50,
			50
		}, null);

		private SplitterState vertSplit = new SplitterState(new float[]
		{
			60f,
			30f
		}, new int[]
		{
			32,
			32
		}, null);

		private string totalUpdates;

		private bool showingConflicts;

		public bool ShowingConflicts
		{
			get
			{
				return this.showingConflicts;
			}
		}

		public bool CanContinue
		{
			get
			{
				return this.asResolveWin.CanContinue();
			}
		}

		public ASUpdateWindow(ASMainWindow parentWin, Changeset[] changesets)
		{
			this.changesets = changesets;
			this.parentWin = parentWin;
			this.lv = new ListViewState(changesets.Length, 5);
			this.pv.lv = new ListViewState(0, 5);
			this.messageFirstLines = new string[changesets.Length];
			for (int i = 0; i < changesets.Length; i++)
			{
				this.messageFirstLines[i] = changesets[i].message.Split(new char[]
				{
					'\n'
				})[0];
			}
			this.totalUpdates = changesets.Length.ToString() + ((changesets.Length != 1) ? " Updates" : " Update");
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				string text = this.dropDownMenuItems[selected];
				if (text != null)
				{
					if (ASUpdateWindow.<>f__switch$map14 == null)
					{
						ASUpdateWindow.<>f__switch$map14 = new Dictionary<string, int>(2)
						{
							{
								"Compare",
								0
							},
							{
								"Compare Binary",
								1
							}
						};
					}
					int num;
					if (ASUpdateWindow.<>f__switch$map14.TryGetValue(text, out num))
					{
						if (num != 0)
						{
							if (num == 1)
							{
								this.DoShowDiff(true);
							}
						}
						else
						{
							this.DoShowDiff(false);
						}
					}
				}
			}
		}

		private void DoSelectionChange()
		{
			if (this.lv.row != -1)
			{
				string firstSelected = this.GetFirstSelected();
				if (firstSelected != string.Empty)
				{
					this.selectedGUID = firstSelected;
				}
				if (AssetServer.IsGUIDValid(this.selectedGUID) != 0)
				{
					int num = 0;
					this.pv.lv.row = -1;
					ParentViewFolder[] folders = this.pv.folders;
					for (int i = 0; i < folders.Length; i++)
					{
						ParentViewFolder parentViewFolder = folders[i];
						if (parentViewFolder.guid == this.selectedGUID)
						{
							this.pv.lv.row = num;
							return;
						}
						num++;
						ParentViewFile[] files = parentViewFolder.files;
						for (int j = 0; j < files.Length; j++)
						{
							ParentViewFile parentViewFile = files[j];
							if (parentViewFile.guid == this.selectedGUID)
							{
								this.pv.lv.row = num;
								return;
							}
							num++;
						}
					}
				}
				else
				{
					this.pv.lv.row = -1;
				}
			}
		}

		private string GetFirstSelected()
		{
			UnityEngine.Object[] filtered = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
			return (filtered.Length == 0) ? string.Empty : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(filtered[0]));
		}

		public void OnSelectionChange()
		{
			if (this.showingConflicts)
			{
				this.asResolveWin.OnSelectionChange(this);
			}
			else
			{
				this.DoSelectionChange();
				this.parentWin.Repaint();
			}
		}

		public int GetSelectedRevisionNumber()
		{
			return (this.pv.lv.row <= this.lv.totalRows - 1 && this.lv.row >= 0) ? this.changesets[this.lv.row].changeset : -1;
		}

		public void SetSelectedRevisionLine(int selIndex)
		{
			if (selIndex >= this.lv.totalRows)
			{
				this.pv.Clear();
				this.lv.row = -1;
			}
			else
			{
				this.lv.row = selIndex;
				this.pv.Clear();
				this.pv.AddAssetItems(this.changesets[selIndex]);
				this.pv.SetLineCount();
			}
			this.pv.lv.scrollPos = Vector2.zero;
			this.pv.lv.row = -1;
			this.pv.selectedFolder = -1;
			this.pv.selectedFile = -1;
			this.DoSelectionChange();
		}

		public string[] GetGUIDs()
		{
			List<string> list = new List<string>();
			if (this.lv.row < 0)
			{
				return null;
			}
			for (int i = this.lv.row; i < this.lv.totalRows; i++)
			{
				for (int j = 0; j < this.changesets[i].items.Length; j++)
				{
					if (!list.Contains(this.changesets[i].items[j].guid))
					{
						list.Add(this.changesets[i].items[j].guid);
					}
				}
			}
			return list.ToArray();
		}

		public bool DoUpdate(bool afterResolvingConflicts)
		{
			AssetServer.RemoveMaintErrorsFromConsole();
			if (!ASEditorBackend.SettingsIfNeeded())
			{
				return true;
			}
			this.showingConflicts = false;
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitOnSuccess");
			AssetServer.DoUpdateOnNextTick(!afterResolvingConflicts, "ShowASConflictResolutionsWindow");
			return true;
		}

		public void ShowConflictResolutions(string[] conflicting)
		{
			this.asResolveWin = new ASUpdateConflictResolveWindow(conflicting);
			this.showingConflicts = true;
		}

		private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
		{
			return (flagToCheck & flags) != ChangeFlags.None;
		}

		private void DoSelect(int folderI, int fileI, int row)
		{
			this.pv.selectedFile = fileI;
			this.pv.selectedFolder = folderI;
			this.pv.lv.row = row;
			this.pv.lv.selectionChanged = true;
			if (fileI == -1)
			{
				if (folderI != -1)
				{
					this.selectedGUID = this.pv.folders[folderI].guid;
					this.isDirSelected = true;
				}
				else
				{
					this.selectedGUID = string.Empty;
					this.isDirSelected = false;
				}
			}
			else
			{
				this.selectedGUID = this.pv.folders[folderI].files[fileI].guid;
				this.isDirSelected = false;
			}
		}

		public void UpdateGUI()
		{
			SplitterGUILayout.BeginHorizontalSplit(this.horSplit, new GUILayoutOption[0]);
			GUILayout.BeginVertical(this.constants.box, new GUILayoutOption[0]);
			GUILayout.Label(this.totalUpdates, this.constants.title, new GUILayoutOption[0]);
			foreach (ListViewElement listViewElement in ListViewGUILayout.ListView(this.lv, GUIStyle.none, new GUILayoutOption[0]))
			{
				Rect position = listViewElement.position;
				position.x += 1f;
				position.y += 1f;
				if (Event.current.type == EventType.Repaint)
				{
					if (listViewElement.row % 2 == 0)
					{
						this.constants.entryEven.Draw(position, false, false, false, false);
					}
					else
					{
						this.constants.entryOdd.Draw(position, false, false, false, false);
					}
				}
				GUILayout.BeginVertical((listViewElement.row != this.lv.row) ? this.constants.entryNormal : this.constants.entrySelected, new GUILayoutOption[0]);
				GUILayout.Label(this.messageFirstLines[listViewElement.row], this.constants.serverUpdateLog, new GUILayoutOption[]
				{
					GUILayout.MinWidth(50f)
				});
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(this.changesets[listViewElement.row].changeset.ToString() + " " + this.changesets[listViewElement.row].date, this.constants.serverUpdateInfo, new GUILayoutOption[]
				{
					GUILayout.MinWidth(100f)
				});
				GUILayout.Label(this.changesets[listViewElement.row].owner, this.constants.serverUpdateInfo, new GUILayoutOption[]
				{
					GUILayout.Width((float)this.maxNickLength)
				});
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			if (this.lv.selectionChanged)
			{
				this.SetSelectedRevisionLine(this.lv.row);
			}
			GUILayout.EndVertical();
			SplitterGUILayout.BeginVerticalSplit(this.vertSplit, new GUILayoutOption[0]);
			GUILayout.BeginVertical(this.constants.box, new GUILayoutOption[0]);
			GUILayout.Label("Changeset", this.constants.title, new GUILayoutOption[0]);
			int num = -1;
			int num2 = -1;
			foreach (ListViewElement listViewElement2 in ListViewGUILayout.ListView(this.pv.lv, GUIStyle.none, new GUILayoutOption[0]))
			{
				if (num == -1 && !this.pv.IndexToFolderAndFile(listViewElement2.row, ref num, ref num2))
				{
					return;
				}
				ParentViewFolder parentViewFolder = this.pv.folders[num];
				if (ListViewGUILayout.HasMouseDown(listViewElement2.position))
				{
					if (Event.current.clickCount == 2)
					{
						if (!this.isDirSelected && this.selectedGUID != string.Empty)
						{
							this.DoShowDiff(false);
							GUIUtility.ExitGUI();
						}
					}
					else
					{
						this.pv.lv.scrollPos = ListViewShared.ListViewScrollToRow(this.pv.lv.ilvState, listViewElement2.row);
						this.DoSelect(num, num2, listViewElement2.row);
					}
				}
				else if (ListViewGUILayout.HasMouseDown(listViewElement2.position, 1))
				{
					if (this.lv.row != listViewElement2.row)
					{
						this.DoSelect(num, num2, listViewElement2.row);
					}
					if (!this.isDirSelected && this.selectedGUID != string.Empty)
					{
						GUIUtility.hotControl = 0;
						Rect position2 = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
						EditorUtility.DisplayCustomMenu(position2, this.dropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
						Event.current.Use();
					}
				}
				if (listViewElement2.row == this.pv.lv.row && Event.current.type == EventType.Repaint)
				{
					this.constants.entrySelected.Draw(listViewElement2.position, false, false, false, false);
				}
				ChangeFlags changeFlags;
				if (num2 != -1)
				{
					Texture2D texture2D = AssetDatabase.GetCachedIcon(parentViewFolder.name + "/" + parentViewFolder.files[num2].name) as Texture2D;
					if (texture2D == null)
					{
						texture2D = InternalEditorUtility.GetIconForFile(parentViewFolder.files[num2].name);
					}
					GUILayout.Label(new GUIContent(parentViewFolder.files[num2].name, texture2D), this.constants.element, new GUILayoutOption[0]);
					changeFlags = parentViewFolder.files[num2].changeFlags;
				}
				else
				{
					GUILayout.Label(parentViewFolder.name, this.constants.header, new GUILayoutOption[0]);
					changeFlags = parentViewFolder.changeFlags;
				}
				GUIContent gUIContent = null;
				if (this.HasFlag(changeFlags, ChangeFlags.Undeleted) || this.HasFlag(changeFlags, ChangeFlags.Created))
				{
					gUIContent = ASMainWindow.constants.badgeNew;
				}
				else if (this.HasFlag(changeFlags, ChangeFlags.Deleted))
				{
					gUIContent = ASMainWindow.constants.badgeDelete;
				}
				else if (this.HasFlag(changeFlags, ChangeFlags.Renamed) || this.HasFlag(changeFlags, ChangeFlags.Moved))
				{
					gUIContent = ASMainWindow.constants.badgeMove;
				}
				if (gUIContent != null && Event.current.type == EventType.Repaint)
				{
					Rect position3 = new Rect(listViewElement2.position.x + listViewElement2.position.width - (float)gUIContent.image.width - 5f, listViewElement2.position.y + listViewElement2.position.height / 2f - (float)(gUIContent.image.height / 2), (float)gUIContent.image.width, (float)gUIContent.image.height);
					EditorGUIUtility.SetIconSize(Vector2.zero);
					GUIStyle.none.Draw(position3, gUIContent, false, false, false, false);
					EditorGUIUtility.SetIconSize(this.iconSize);
				}
				this.pv.NextFileFolder(ref num, ref num2);
			}
			if (this.pv.lv.selectionChanged && this.selectedGUID != string.Empty)
			{
				if (this.selectedGUID != AssetServer.GetRootGUID())
				{
					AssetServer.SetSelectionFromGUID(this.selectedGUID);
				}
				else
				{
					AssetServer.SetSelectionFromGUID(string.Empty);
				}
			}
			if (GUIUtility.keyboardControl == this.pv.lv.ID && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && !this.isDirSelected && this.selectedGUID != string.Empty)
			{
				this.DoShowDiff(false);
				GUIUtility.ExitGUI();
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(this.constants.box, new GUILayoutOption[0]);
			GUILayout.Label("Update Message", this.constants.title, new GUILayoutOption[0]);
			GUILayout.TextArea((this.lv.row < 0) ? string.Empty : this.changesets[this.lv.row].message, this.constants.wwText, new GUILayoutOption[0]);
			GUILayout.EndVertical();
			SplitterGUILayout.EndVerticalSplit();
			SplitterGUILayout.EndHorizontalSplit();
		}

		private bool DoShowDiff(bool binary)
		{
			List<string> list = new List<string>();
			List<CompareInfo> list2 = new List<CompareInfo>();
			int num;
			if (AssetServer.IsItemDeleted(this.selectedGUID))
			{
				num = -2;
			}
			else
			{
				num = AssetServer.GetWorkingItemChangeset(this.selectedGUID);
				num = AssetServer.GetServerItemChangeset(this.selectedGUID, num);
			}
			int serverItemChangeset = AssetServer.GetServerItemChangeset(this.selectedGUID, -1);
			int ver = (serverItemChangeset != -1) ? serverItemChangeset : -2;
			list.Add(this.selectedGUID);
			list2.Add(new CompareInfo(num, ver, (!binary) ? 0 : 1, (!binary) ? 1 : 0));
			if (list.Count != 0)
			{
				AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
				return true;
			}
			return false;
		}

		public void Repaint()
		{
			this.parentWin.Repaint();
		}

		public bool DoGUI()
		{
			bool enabled = GUI.enabled;
			if (this.constants == null)
			{
				this.constants = new ASUpdateWindow.Constants();
				this.maxNickLength = 1;
				for (int i = 0; i < this.changesets.Length; i++)
				{
					int num = (int)this.constants.serverUpdateInfo.CalcSize(new GUIContent(this.changesets[i].owner)).x;
					if (num > this.maxNickLength)
					{
						this.maxNickLength = num;
					}
				}
			}
			EditorGUIUtility.SetIconSize(this.iconSize);
			if (this.showingConflicts)
			{
				if (!this.asResolveWin.DoGUI(this))
				{
					this.showingConflicts = false;
				}
			}
			else
			{
				this.UpdateGUI();
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			if (!this.showingConflicts)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUI.enabled = (!this.isDirSelected && this.selectedGUID != string.Empty && enabled);
				if (GUILayout.Button("Compare", this.constants.button, new GUILayoutOption[0]))
				{
					this.DoShowDiff(false);
					GUIUtility.ExitGUI();
				}
				GUI.enabled = enabled;
				GUILayout.FlexibleSpace();
				if (this.changesets.Length == 0)
				{
					GUI.enabled = false;
				}
				if (GUILayout.Button("Update", this.constants.bigButton, new GUILayoutOption[]
				{
					GUILayout.MinWidth(100f)
				}))
				{
					if (this.changesets.Length == 0)
					{
						Debug.Log("Nothing to update.");
					}
					else
					{
						this.DoUpdate(false);
					}
					this.parentWin.Repaint();
					GUIUtility.ExitGUI();
				}
				if (this.changesets.Length == 0)
				{
					GUI.enabled = enabled;
				}
				GUILayout.EndHorizontal();
				if (AssetServer.GetAssetServerError() != string.Empty)
				{
					GUILayout.Space(10f);
					GUILayout.Label(AssetServer.GetAssetServerError(), this.constants.errorLabel, new GUILayoutOption[0]);
					GUILayout.Space(10f);
				}
			}
			GUILayout.Space(10f);
			return true;
		}
	}
}
