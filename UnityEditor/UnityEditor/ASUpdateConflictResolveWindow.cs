using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASUpdateConflictResolveWindow
	{
		private class Constants
		{
			public GUIStyle ButtonLeft = "ButtonLeft";

			public GUIStyle ButtonMiddle = "ButtonMid";

			public GUIStyle ButtonRight = "ButtonRight";

			public GUIStyle EntrySelected = "ServerUpdateChangesetOn";

			public GUIStyle EntryNormal = "ServerUpdateInfo";

			public GUIStyle lvHeader = "OL title";

			public GUIStyle selected = "ServerUpdateChangesetOn";

			public GUIStyle background = "OL Box";

			public GUIStyle button = "Button";

			public GUIStyle bigButton = "LargeButton";
		}

		private ListViewState lv1 = new ListViewState();

		private ListViewState lv2 = new ListViewState();

		private bool[] selectedLV1Items;

		private bool[] selectedLV2Items;

		private bool[] deletionConflict;

		private int initialSelectedLV1Item = -1;

		private int initialSelectedLV2Item = -1;

		private bool lv1HasSelection = false;

		private bool lv2HasSelection = false;

		private SplitterState lvHeaderSplit1 = new SplitterState(new float[]
		{
			20f,
			80f
		}, new int[]
		{
			100,
			100
		}, null);

		private SplitterState lvHeaderSplit2 = new SplitterState(new float[]
		{
			20f,
			80f
		}, new int[]
		{
			100,
			100
		}, null);

		private static string[] conflictButtonTexts = new string[]
		{
			"Skip Asset",
			"Discard My Changes",
			"Ignore Server Changes",
			"Merge",
			"Unresolved"
		};

		private static string[] nameConflictButtonTexts = new string[]
		{
			"Rename Local Asset",
			"Rename Server Asset"
		};

		private string[] dropDownMenuItems = new string[]
		{
			"Compare",
			"Compare Binary"
		};

		private string[] downloadConflicts = new string[0];

		private string[] nameConflicts = new string[0];

		private string[] dConflictPaths = new string[0];

		private string[] dNamingPaths = new string[0];

		private DownloadResolution[] downloadResolutions = new DownloadResolution[0];

		private NameConflictResolution[] namingResolutions = new NameConflictResolution[0];

		private int downloadConflictsToResolve = 0;

		private bool showDownloadConflicts;

		private bool showNamingConflicts;

		private bool mySelection = false;

		private bool enableContinueButton = false;

		private bool enableMergeButton = true;

		private bool splittersOk = false;

		private Vector2 iconSize = new Vector2(16f, 16f);

		private ASUpdateConflictResolveWindow.Constants constants = null;

		private string[] downloadResolutionString = new string[]
		{
			"Unresolved",
			"Skip Asset",
			"Discard My Changes",
			"Ignore Server Changes",
			"Merge"
		};

		private string[] namingResolutionString = new string[]
		{
			"Unresolved",
			"Rename Local Asset",
			"Rename Server Asset"
		};

		public ASUpdateConflictResolveWindow(string[] conflicting)
		{
			this.downloadConflictsToResolve = 0;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			ArrayList arrayList3 = new ArrayList();
			ArrayList arrayList4 = new ArrayList();
			for (int i = 0; i < conflicting.Length; i++)
			{
				AssetStatus statusGUID = AssetServer.GetStatusGUID(conflicting[i]);
				if (statusGUID == AssetStatus.Conflict)
				{
					arrayList.Add(conflicting[i]);
					DownloadResolution downloadResolution = AssetServer.GetDownloadResolution(conflicting[i]);
					arrayList2.Add(downloadResolution);
					if (downloadResolution == DownloadResolution.Unresolved)
					{
						this.downloadConflictsToResolve++;
					}
				}
				if (AssetServer.GetPathNameConflict(conflicting[i]) != null && statusGUID != AssetStatus.ServerOnly)
				{
					arrayList4.Add(conflicting[i]);
					NameConflictResolution nameConflictResolution = AssetServer.GetNameConflictResolution(conflicting[i]);
					arrayList3.Add(nameConflictResolution);
					if (nameConflictResolution == NameConflictResolution.Unresolved)
					{
						this.downloadConflictsToResolve++;
					}
				}
			}
			this.downloadConflicts = (arrayList.ToArray(typeof(string)) as string[]);
			this.downloadResolutions = (arrayList2.ToArray(typeof(DownloadResolution)) as DownloadResolution[]);
			this.namingResolutions = (arrayList3.ToArray(typeof(NameConflictResolution)) as NameConflictResolution[]);
			this.nameConflicts = (arrayList4.ToArray(typeof(string)) as string[]);
			this.enableContinueButton = (this.downloadConflictsToResolve == 0);
			this.dConflictPaths = new string[this.downloadConflicts.Length];
			this.deletionConflict = new bool[this.downloadConflicts.Length];
			for (int j = 0; j < this.downloadConflicts.Length; j++)
			{
				if (AssetServer.HasDeletionConflict(this.downloadConflicts[j]))
				{
					this.dConflictPaths[j] = ParentViewFolder.MakeNiceName(AssetServer.GetDeletedItemPathAndName(this.downloadConflicts[j]));
					this.deletionConflict[j] = true;
				}
				else
				{
					this.dConflictPaths[j] = ParentViewFolder.MakeNiceName(AssetServer.GetAssetPathName(this.downloadConflicts[j]));
					this.deletionConflict[j] = false;
				}
			}
			this.dNamingPaths = new string[this.nameConflicts.Length];
			for (int k = 0; k < this.nameConflicts.Length; k++)
			{
				this.dNamingPaths[k] = ParentViewFolder.MakeNiceName(AssetServer.GetAssetPathName(this.nameConflicts[k]));
			}
			this.showDownloadConflicts = (this.downloadConflicts.Length > 0);
			this.showNamingConflicts = (this.nameConflicts.Length > 0);
			this.lv1.totalRows = this.downloadConflicts.Length;
			this.lv2.totalRows = this.nameConflicts.Length;
			this.selectedLV1Items = new bool[this.downloadConflicts.Length];
			this.selectedLV2Items = new bool[this.nameConflicts.Length];
			this.DoSelectionChange();
		}

		public string[] GetDownloadConflicts()
		{
			return this.downloadConflicts;
		}

		public string[] GetNameConflicts()
		{
			return this.nameConflicts;
		}

		public bool CanContinue()
		{
			return this.enableContinueButton;
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				string text = this.dropDownMenuItems[selected];
				if (text != null)
				{
					if (!(text == "Compare"))
					{
						if (text == "Compare Binary")
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

		private void ResolveSelectedDownloadConflicts(DownloadResolution res)
		{
			int num = -1;
			bool flag = false;
			int i = 0;
			while (i < this.downloadConflicts.Length)
			{
				if (this.selectedLV1Items[i])
				{
					string guid = this.downloadConflicts[i];
					if (res == DownloadResolution.Merge && (AssetServer.AssetIsBinaryByGUID(guid) || AssetServer.IsItemDeleted(guid)))
					{
						flag = true;
					}
					else
					{
						if (res != DownloadResolution.Unresolved)
						{
							if (AssetServer.GetDownloadResolution(guid) == DownloadResolution.Unresolved)
							{
								this.downloadConflictsToResolve--;
							}
						}
						else
						{
							this.downloadConflictsToResolve++;
						}
						this.downloadResolutions[i] = res;
						AssetServer.SetDownloadResolution(guid, res);
						num = ((num != -1) ? -2 : i);
					}
				}
				IL_9F:
				i++;
				continue;
				goto IL_9F;
			}
			this.enableContinueButton = (this.downloadConflictsToResolve == 0);
			if (num >= 0)
			{
				this.selectedLV1Items[num] = false;
				if (num < this.selectedLV1Items.Length - 1)
				{
					this.selectedLV1Items[num + 1] = true;
				}
			}
			this.enableMergeButton = this.AtLeastOneSelectedAssetCanBeMerged();
			if (flag)
			{
				EditorUtility.DisplayDialog("Some conflicting changes cannot be merged", "Notice that not all selected changes where selected for merging. This happened because not all of them can be merged (e.g. assets are binary or deleted).", "OK");
			}
		}

		private void ResolveSelectedNamingConflicts(NameConflictResolution res)
		{
			if (res != NameConflictResolution.Unresolved)
			{
				for (int i = 0; i < this.nameConflicts.Length; i++)
				{
					if (this.selectedLV2Items[i])
					{
						string guid = this.nameConflicts[i];
						if (AssetServer.GetNameConflictResolution(guid) == NameConflictResolution.Unresolved)
						{
							this.downloadConflictsToResolve--;
						}
						this.namingResolutions[i] = res;
						AssetServer.SetNameConflictResolution(guid, res);
					}
				}
				this.enableContinueButton = (this.downloadConflictsToResolve == 0);
			}
		}

		private bool DoShowDiff(bool binary)
		{
			List<string> list = new List<string>();
			List<CompareInfo> list2 = new List<CompareInfo>();
			for (int i = 0; i < this.selectedLV1Items.Length; i++)
			{
				if (this.selectedLV1Items[i])
				{
					int serverItemChangeset = AssetServer.GetServerItemChangeset(this.downloadConflicts[i], -1);
					int ver = (!AssetServer.HasDeletionConflict(this.downloadConflicts[i])) ? -1 : -2;
					list.Add(this.downloadConflicts[i]);
					list2.Add(new CompareInfo(serverItemChangeset, ver, (!binary) ? 0 : 1, (!binary) ? 1 : 0));
				}
			}
			bool result;
			if (list.Count != 0)
			{
				AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private string[] GetSelectedGUIDs()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.downloadConflicts.Length; i++)
			{
				if (this.selectedLV1Items[i])
				{
					list.Add(this.downloadConflicts[i]);
				}
			}
			return list.ToArray();
		}

		private string[] GetSelectedNamingGUIDs()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.nameConflicts.Length; i++)
			{
				if (this.selectedLV2Items[i])
				{
					list.Add(this.nameConflicts[i]);
				}
			}
			return list.ToArray();
		}

		private bool HasTrue(ref bool[] array)
		{
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i])
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private void DoSelectionChange()
		{
			HierarchyProperty hierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
			List<string> list = new List<string>(Selection.objects.Length);
			UnityEngine.Object[] objects = Selection.objects;
			for (int i = 0; i < objects.Length; i++)
			{
				UnityEngine.Object @object = objects[i];
				if (hierarchyProperty.Find(@object.GetInstanceID(), null))
				{
					list.Add(hierarchyProperty.guid);
				}
			}
			for (int j = 0; j < this.downloadConflicts.Length; j++)
			{
				this.selectedLV1Items[j] = list.Contains(this.downloadConflicts[j]);
			}
			for (int k = 0; k < this.nameConflicts.Length; k++)
			{
				this.selectedLV2Items[k] = list.Contains(this.nameConflicts[k]);
			}
			this.lv1HasSelection = this.HasTrue(ref this.selectedLV1Items);
			this.lv2HasSelection = this.HasTrue(ref this.selectedLV2Items);
			this.enableMergeButton = this.AtLeastOneSelectedAssetCanBeMerged();
		}

		public void OnSelectionChange(ASUpdateWindow parentWin)
		{
			if (!this.mySelection)
			{
				this.DoSelectionChange();
				parentWin.Repaint();
			}
			else
			{
				this.mySelection = false;
			}
		}

		private bool AtLeastOneSelectedAssetCanBeMerged()
		{
			bool result;
			for (int i = 0; i < this.downloadConflicts.Length; i++)
			{
				if (this.selectedLV1Items[i])
				{
					if (!AssetServer.AssetIsBinaryByGUID(this.downloadConflicts[i]) && !AssetServer.IsItemDeleted(this.downloadConflicts[i]))
					{
						result = true;
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		private void DoDownloadConflictsGUI()
		{
			bool enabled = GUI.enabled;
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("The following assets have been changed both on the server and in the local project.\nPlease select a conflict resolution for each before continuing the update.", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.lv1HasSelection && enabled);
			if (GUILayout.Button(ASUpdateConflictResolveWindow.conflictButtonTexts[0], this.constants.ButtonLeft, new GUILayoutOption[0]))
			{
				this.ResolveSelectedDownloadConflicts(DownloadResolution.SkipAsset);
			}
			if (GUILayout.Button(ASUpdateConflictResolveWindow.conflictButtonTexts[1], this.constants.ButtonMiddle, new GUILayoutOption[0]))
			{
				this.ResolveSelectedDownloadConflicts(DownloadResolution.TrashMyChanges);
			}
			if (GUILayout.Button(ASUpdateConflictResolveWindow.conflictButtonTexts[2], this.constants.ButtonMiddle, new GUILayoutOption[0]))
			{
				this.ResolveSelectedDownloadConflicts(DownloadResolution.TrashServerChanges);
			}
			if (!this.enableMergeButton)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button(ASUpdateConflictResolveWindow.conflictButtonTexts[3], this.constants.ButtonRight, new GUILayoutOption[0]))
			{
				this.ResolveSelectedDownloadConflicts(DownloadResolution.Merge);
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			SplitterGUILayout.BeginHorizontalSplit(this.lvHeaderSplit1, new GUILayoutOption[0]);
			GUILayout.Box("Action", this.constants.lvHeader, new GUILayoutOption[0]);
			GUILayout.Box("Asset", this.constants.lvHeader, new GUILayoutOption[0]);
			SplitterGUILayout.EndHorizontalSplit();
			int row = this.lv1.row;
			bool flag = false;
			IEnumerator enumerator = ListViewGUILayout.ListView(this.lv1, this.constants.background, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					if (GUIUtility.keyboardControl == this.lv1.ID && Event.current.type == EventType.KeyDown && actionKey)
					{
						Event.current.Use();
					}
					if (this.selectedLV1Items[listViewElement.row] && Event.current.type == EventType.Repaint)
					{
						this.constants.selected.Draw(listViewElement.position, false, false, false, false);
					}
					if (ListViewGUILayout.HasMouseUp(listViewElement.position))
					{
						if (!shift && !actionKey)
						{
							flag |= ListViewGUILayout.MultiSelection(row, this.lv1.row, ref this.initialSelectedLV1Item, ref this.selectedLV1Items);
						}
					}
					else if (ListViewGUILayout.HasMouseDown(listViewElement.position))
					{
						if (Event.current.clickCount == 2 && !AssetServer.AssetIsDir(this.downloadConflicts[listViewElement.row]))
						{
							this.DoShowDiff(false);
							GUIUtility.ExitGUI();
						}
						else
						{
							if (!this.selectedLV1Items[listViewElement.row] || shift || actionKey)
							{
								flag |= ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.initialSelectedLV1Item, ref this.selectedLV1Items);
							}
							this.lv1.row = listViewElement.row;
						}
					}
					else if (ListViewGUILayout.HasMouseDown(listViewElement.position, 1))
					{
						if (!this.selectedLV1Items[listViewElement.row])
						{
							flag = true;
							for (int i = 0; i < this.selectedLV1Items.Length; i++)
							{
								this.selectedLV1Items[i] = false;
							}
							this.lv1.selectionChanged = true;
							this.selectedLV1Items[listViewElement.row] = true;
							this.lv1.row = listViewElement.row;
						}
						GUIUtility.hotControl = 0;
						Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
						EditorUtility.DisplayCustomMenu(position, this.dropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
						Event.current.Use();
					}
					GUILayout.Label(this.downloadResolutionString[(int)this.downloadResolutions[listViewElement.row]], new GUILayoutOption[]
					{
						GUILayout.Width((float)this.lvHeaderSplit1.realSizes[0]),
						GUILayout.Height(18f)
					});
					if (this.deletionConflict[listViewElement.row] && Event.current.type == EventType.Repaint)
					{
						GUIContent badgeDelete = ASMainWindow.constants.badgeDelete;
						Rect position2 = new Rect(listViewElement.position.x + (float)this.lvHeaderSplit1.realSizes[0] - (float)badgeDelete.image.width - 5f, listViewElement.position.y + listViewElement.position.height / 2f - (float)(badgeDelete.image.height / 2), (float)badgeDelete.image.width, (float)badgeDelete.image.height);
						EditorGUIUtility.SetIconSize(Vector2.zero);
						GUIStyle.none.Draw(position2, badgeDelete, false, false, false, false);
						EditorGUIUtility.SetIconSize(this.iconSize);
					}
					GUILayout.Label(new GUIContent(this.dConflictPaths[listViewElement.row], (!AssetServer.AssetIsDir(this.downloadConflicts[listViewElement.row])) ? InternalEditorUtility.GetIconForFile(this.dConflictPaths[listViewElement.row]) : EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName)), new GUILayoutOption[]
					{
						GUILayout.Width((float)this.lvHeaderSplit1.realSizes[1]),
						GUILayout.Height(18f)
					});
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
			GUILayout.EndVertical();
			if (GUIUtility.keyboardControl == this.lv1.ID)
			{
				if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
				{
					Event.current.Use();
				}
				else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
				{
					for (int j = 0; j < this.selectedLV1Items.Length; j++)
					{
						this.selectedLV1Items[j] = true;
					}
					flag = true;
					Event.current.Use();
				}
				if (this.lv1.selectionChanged && !actionKey)
				{
					flag |= ListViewGUILayout.MultiSelection(row, this.lv1.row, ref this.initialSelectedLV1Item, ref this.selectedLV1Items);
				}
				else if (GUIUtility.keyboardControl == this.lv1.ID && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && !AssetServer.AssetIsDir(this.downloadConflicts[this.lv1.row]))
				{
					this.DoShowDiff(false);
					GUIUtility.ExitGUI();
				}
			}
			if (this.lv1.selectionChanged || flag)
			{
				this.mySelection = true;
				AssetServer.SetSelectionFromGUIDs(this.GetSelectedGUIDs());
				this.lv1HasSelection = this.HasTrue(ref this.selectedLV1Items);
				this.enableMergeButton = this.AtLeastOneSelectedAssetCanBeMerged();
			}
		}

		private void DoNamingConflictsGUI()
		{
			bool enabled = GUI.enabled;
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label("The following assets have the same name as an existing asset on the server.\nPlease select which one to rename before continuing the update.", new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.lv2HasSelection && enabled);
			if (GUILayout.Button(ASUpdateConflictResolveWindow.nameConflictButtonTexts[0], this.constants.ButtonLeft, new GUILayoutOption[0]))
			{
				this.ResolveSelectedNamingConflicts(NameConflictResolution.RenameLocal);
			}
			if (GUILayout.Button(ASUpdateConflictResolveWindow.nameConflictButtonTexts[1], this.constants.ButtonRight, new GUILayoutOption[0]))
			{
				this.ResolveSelectedNamingConflicts(NameConflictResolution.RenameRemote);
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			SplitterGUILayout.BeginHorizontalSplit(this.lvHeaderSplit2, new GUILayoutOption[0]);
			GUILayout.Box("Action", this.constants.lvHeader, new GUILayoutOption[0]);
			GUILayout.Box("Asset", this.constants.lvHeader, new GUILayoutOption[0]);
			SplitterGUILayout.EndHorizontalSplit();
			int row = this.lv2.row;
			bool flag = false;
			IEnumerator enumerator = ListViewGUILayout.ListView(this.lv2, this.constants.background, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					if (GUIUtility.keyboardControl == this.lv2.ID && Event.current.type == EventType.KeyDown && actionKey)
					{
						Event.current.Use();
					}
					if (this.selectedLV2Items[listViewElement.row] && Event.current.type == EventType.Repaint)
					{
						this.constants.selected.Draw(listViewElement.position, false, false, false, false);
					}
					if (ListViewGUILayout.HasMouseUp(listViewElement.position))
					{
						if (!shift && !actionKey)
						{
							flag |= ListViewGUILayout.MultiSelection(row, this.lv2.row, ref this.initialSelectedLV2Item, ref this.selectedLV2Items);
						}
					}
					else if (ListViewGUILayout.HasMouseDown(listViewElement.position))
					{
						if (!this.selectedLV2Items[listViewElement.row] || shift || actionKey)
						{
							flag |= ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.initialSelectedLV2Item, ref this.selectedLV2Items);
						}
						this.lv2.row = listViewElement.row;
					}
					GUILayout.Label(this.namingResolutionString[(int)this.namingResolutions[listViewElement.row]], new GUILayoutOption[]
					{
						GUILayout.Width((float)this.lvHeaderSplit2.realSizes[0]),
						GUILayout.Height(18f)
					});
					GUILayout.Label(new GUIContent(this.dNamingPaths[listViewElement.row], (!AssetServer.AssetIsDir(this.nameConflicts[listViewElement.row])) ? InternalEditorUtility.GetIconForFile(this.dNamingPaths[listViewElement.row]) : EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName)), new GUILayoutOption[]
					{
						GUILayout.Width((float)this.lvHeaderSplit2.realSizes[1]),
						GUILayout.Height(18f)
					});
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
			GUILayout.EndVertical();
			if (GUIUtility.keyboardControl == this.lv2.ID)
			{
				if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
				{
					Event.current.Use();
				}
				else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
				{
					for (int i = 0; i < this.selectedLV2Items.Length; i++)
					{
						this.selectedLV2Items[i] = true;
					}
					flag = true;
					Event.current.Use();
				}
				if (this.lv2.selectionChanged && !actionKey)
				{
					flag |= ListViewGUILayout.MultiSelection(row, this.lv2.row, ref this.initialSelectedLV2Item, ref this.selectedLV2Items);
				}
			}
			if (this.lv2.selectionChanged || flag)
			{
				this.mySelection = true;
				AssetServer.SetSelectionFromGUIDs(this.GetSelectedNamingGUIDs());
				this.lv2HasSelection = this.HasTrue(ref this.selectedLV2Items);
			}
		}

		public bool DoGUI(ASUpdateWindow parentWin)
		{
			if (this.constants == null)
			{
				this.constants = new ASUpdateConflictResolveWindow.Constants();
			}
			bool enabled = GUI.enabled;
			EditorGUIUtility.SetIconSize(this.iconSize);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (this.showDownloadConflicts)
			{
				this.DoDownloadConflictsGUI();
			}
			if (this.showNamingConflicts)
			{
				this.DoNamingConflictsGUI();
			}
			GUILayout.EndVertical();
			EditorGUIUtility.SetIconSize(Vector2.zero);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.enabled = (this.lv1HasSelection && enabled);
			if (GUILayout.Button("Compare", this.constants.button, new GUILayoutOption[0]))
			{
				if (!this.DoShowDiff(false))
				{
					Debug.Log("No differences found");
				}
				GUIUtility.ExitGUI();
			}
			GUI.enabled = enabled;
			GUILayout.FlexibleSpace();
			GUI.enabled = (parentWin.CanContinue && enabled);
			bool result;
			if (GUILayout.Button("Continue", this.constants.bigButton, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}))
			{
				parentWin.DoUpdate(true);
				result = false;
			}
			else
			{
				GUI.enabled = enabled;
				if (GUILayout.Button("Cancel", this.constants.bigButton, new GUILayoutOption[]
				{
					GUILayout.MinWidth(100f)
				}))
				{
					result = false;
				}
				else
				{
					GUILayout.EndHorizontal();
					if (!this.splittersOk && Event.current.type == EventType.Repaint)
					{
						this.splittersOk = true;
						parentWin.Repaint();
					}
					result = true;
				}
			}
			return result;
		}
	}
}
