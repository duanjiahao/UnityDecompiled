using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASCommitWindow
	{
		private class Constants
		{
			public GUIStyle box = "OL Box";

			public GUIStyle entrySelected = "ServerUpdateChangesetOn";

			public GUIStyle serverChangeCount = "ServerChangeCount";

			public GUIStyle title = "OL title";

			public GUIStyle element = "OL elem";

			public GUIStyle header = "OL header";

			public GUIStyle button = "Button";

			public GUIStyle serverUpdateInfo = "ServerUpdateInfo";

			public GUIStyle wwText = "AS TextArea";

			public GUIStyle errorLabel = "ErrorLabel";

			public GUIStyle dropDown = "MiniPullDown";

			public GUIStyle bigButton = "LargeButton";
		}

		private const int listLenghts = 20;

		private const int widthToHideButtons = 432;

		private bool wasHidingButtons;

		private bool resetKeyboardControl;

		private static ASCommitWindow.Constants constants;

		private ParentViewState pv1state = new ParentViewState();

		private ParentViewState pv2state = new ParentViewState();

		private bool pv1hasSelection;

		private bool pv2hasSelection;

		private bool somethingDiscardableSelected;

		private bool mySelection;

		private string[] commitMessageList;

		private string[] dropDownMenuItems = new string[]
		{
			string.Empty,
			string.Empty,
			"Compare",
			"Compare Binary",
			string.Empty,
			"Discard"
		};

		private string[] guidsToTransferToTheRightSide;

		private string dragTitle = string.Empty;

		private Vector2 iconSize = new Vector2(16f, 16f);

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
			64
		}, null);

		internal string description = string.Empty;

		private string totalChanges;

		private ASMainWindow parentWin;

		private bool initialUpdate;

		private Vector2 scrollPos = Vector2.zero;

		internal bool lastTransferMovedDependencies;

		internal int lastRevertSelectionChanged = -1;

		internal int showReinitedWarning = -1;

		private static List<string> s_AssetGuids;

		private static string s_Callback;

		public ASCommitWindow(ASMainWindow parentWin, string[] guidsToTransfer)
		{
			this.guidsToTransferToTheRightSide = guidsToTransfer;
			this.parentWin = parentWin;
			this.initialUpdate = true;
		}

		internal void AssetItemsToParentViews()
		{
			this.pv1state.Clear();
			this.pv2state.Clear();
			this.pv1state.AddAssetItems(this.parentWin.sharedCommits);
			this.pv1state.AddAssetItems(this.parentWin.sharedDeletedItems);
			this.pv1state.lv = new ListViewState(0);
			this.pv2state.lv = new ListViewState(0);
			this.pv1state.SetLineCount();
			this.pv2state.SetLineCount();
			if (this.pv1state.lv.totalRows == 0)
			{
				this.parentWin.Reinit();
				return;
			}
			this.pv1state.selectedItems = new bool[this.pv1state.lv.totalRows];
			this.pv2state.selectedItems = new bool[this.pv1state.lv.totalRows];
			int num = 0;
			for (int i = 0; i < this.parentWin.sharedCommits.Length; i++)
			{
				if (this.parentWin.sharedCommits[i].assetIsDir != 0)
				{
					num++;
				}
			}
			for (int j = 0; j < this.parentWin.sharedDeletedItems.Length; j++)
			{
				if (this.parentWin.sharedDeletedItems[j].assetIsDir != 0)
				{
					num++;
				}
			}
			this.totalChanges = (this.pv1state.lv.totalRows - this.pv1state.GetFoldersCount() + num).ToString() + " Local Changes";
			this.GetPersistedData();
		}

		internal void Reinit(bool lastActionsResult)
		{
			this.parentWin.sharedCommits = ASCommitWindow.GetCommits();
			this.parentWin.sharedDeletedItems = AssetServer.GetLocalDeletedItems();
			AssetServer.ClearRefreshCommit();
			this.AssetItemsToParentViews();
		}

		internal void Update()
		{
			this.SetPersistedData();
			this.AssetItemsToParentViews();
			this.GetPersistedData();
		}

		internal void CommitFinished(bool actionResult)
		{
			if (actionResult)
			{
				AssetServer.ClearCommitPersistentData();
				this.parentWin.Reinit();
			}
			else
			{
				this.parentWin.Repaint();
			}
		}

		internal void InitiateReinit()
		{
			if (this.parentWin.CommitNeedsRefresh())
			{
				if (!this.initialUpdate)
				{
					this.SetPersistedData();
				}
				else
				{
					this.initialUpdate = false;
				}
				this.Reinit(true);
			}
			else if (this.initialUpdate)
			{
				this.AssetItemsToParentViews();
				this.initialUpdate = false;
			}
			else
			{
				this.SetPersistedData();
				AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitCommitWindow");
				AssetServer.DoRefreshAssetsOnNextTick();
			}
		}

		private void GetPersistedData()
		{
			this.description = AssetServer.GetLastCommitMessage();
			string[] commitSelectionGUIDs;
			if (this.guidsToTransferToTheRightSide != null && this.guidsToTransferToTheRightSide.Length != 0)
			{
				commitSelectionGUIDs = this.guidsToTransferToTheRightSide;
				this.guidsToTransferToTheRightSide = null;
			}
			else
			{
				commitSelectionGUIDs = AssetServer.GetCommitSelectionGUIDs();
			}
			int i = 0;
			ParentViewFolder[] folders = this.pv1state.folders;
			for (int j = 0; j < folders.Length; j++)
			{
				ParentViewFolder parentViewFolder = folders[j];
				this.pv1state.selectedItems[i++] = (((IList)commitSelectionGUIDs).Contains(parentViewFolder.guid) && AssetServer.IsGUIDValid(parentViewFolder.guid) != 0);
				ParentViewFile[] files = parentViewFolder.files;
				for (int k = 0; k < files.Length; k++)
				{
					ParentViewFile parentViewFile = files[k];
					this.pv1state.selectedItems[i++] = (((IList)commitSelectionGUIDs).Contains(parentViewFile.guid) && AssetServer.IsGUIDValid(parentViewFile.guid) != 0);
				}
			}
			this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
			this.commitMessageList = InternalEditorUtility.GetEditorSettingsList("ASCommitMsgs", 20);
			for (i = 0; i < this.commitMessageList.Length; i++)
			{
				this.commitMessageList[i] = this.commitMessageList[i].Replace('/', '?').Replace('%', '?');
			}
		}

		private void SetPersistedData()
		{
			AssetServer.SetLastCommitMessage(this.description);
			this.AddToCommitMessageHistory(this.description);
			List<string> list = new List<string>();
			ParentViewFolder[] folders = this.pv2state.folders;
			for (int i = 0; i < folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = folders[i];
				if (AssetServer.IsGUIDValid(parentViewFolder.guid) != 0)
				{
					list.Add(parentViewFolder.guid);
				}
				ParentViewFile[] files = parentViewFolder.files;
				for (int j = 0; j < files.Length; j++)
				{
					ParentViewFile parentViewFile = files[j];
					if (AssetServer.IsGUIDValid(parentViewFile.guid) != 0)
					{
						list.Add(parentViewFile.guid);
					}
				}
			}
			AssetServer.SetCommitSelectionGUIDs(list.ToArray());
		}

		internal void OnClose()
		{
			this.SetPersistedData();
		}

		private List<string> GetSelectedItems()
		{
			this.pv1hasSelection = this.pv1state.HasTrue();
			this.pv2hasSelection = this.pv2state.HasTrue();
			List<string> parentViewSelectedItems = ASCommitWindow.GetParentViewSelectedItems((!this.pv2hasSelection) ? this.pv1state : this.pv2state, true, false);
			parentViewSelectedItems.Remove(AssetServer.GetRootGUID());
			return parentViewSelectedItems;
		}

		private void MySelectionToGlobalSelection()
		{
			this.mySelection = true;
			this.somethingDiscardableSelected = ASCommitWindow.SomethingDiscardableSelected((!this.pv2hasSelection) ? this.pv1state : this.pv2state);
			List<string> selectedItems = this.GetSelectedItems();
			if (selectedItems.Count > 0)
			{
				AssetServer.SetSelectionFromGUID(selectedItems[0]);
			}
		}

		internal static bool DoShowDiff(List<string> selectedAssets, bool binary)
		{
			List<string> list = new List<string>();
			List<CompareInfo> list2 = new List<CompareInfo>();
			for (int i = 0; i < selectedAssets.Count; i++)
			{
				int ver = -1;
				int num = AssetServer.GetWorkingItemChangeset(selectedAssets[i]);
				num = AssetServer.GetServerItemChangeset(selectedAssets[i], num);
				if (AssetServer.IsItemDeleted(selectedAssets[i]))
				{
					ver = -2;
				}
				if (num == -1)
				{
					num = -2;
				}
				list.Add(selectedAssets[i]);
				list2.Add(new CompareInfo(num, ver, (!binary) ? 0 : 1, (!binary) ? 1 : 0));
			}
			if (list.Count != 0)
			{
				AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
				return true;
			}
			return false;
		}

		internal static bool IsDiscardableAsset(string guid, ChangeFlags changeFlags)
		{
			return AssetServer.IsConstantGUID(guid) == 0 || (!ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Created) && !ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Undeleted));
		}

		internal static List<string> GetParentViewSelectedItems(ParentViewState state, bool includeFolders, bool excludeUndiscardableOnes)
		{
			List<string> list = new List<string>();
			int num = 0;
			for (int i = 0; i < state.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = state.folders[i];
				bool flag = true;
				bool flag2 = true;
				int num2 = num++;
				int count = list.Count;
				for (int j = 0; j < parentViewFolder.files.Length; j++)
				{
					if (state.selectedItems[num])
					{
						if (!excludeUndiscardableOnes || ASCommitWindow.IsDiscardableAsset(parentViewFolder.files[j].guid, parentViewFolder.files[j].changeFlags))
						{
							list.Add(parentViewFolder.files[j].guid);
							flag = false;
						}
					}
					else
					{
						flag2 = false;
					}
					num++;
				}
				if (includeFolders && state.selectedItems[num2] && (flag || flag2) && AssetServer.IsGUIDValid(parentViewFolder.guid) != 0 && count <= list.Count)
				{
					list.Insert(count, parentViewFolder.guid);
				}
			}
			return list;
		}

		internal static void DoRevert(List<string> assetGuids, string callback)
		{
			if (assetGuids.Count == 0)
			{
				return;
			}
			ASCommitWindow.s_AssetGuids = assetGuids;
			ASCommitWindow.s_Callback = callback;
			AssetServer.SetAfterActionFinishedCallback("ASCommitWindow", "DoRevertAfterDialog");
			AssetServer.ShowDialogOnNextTick("Discard changes", "Are you really sure you want to discard selected changes?", "Discard", "Cancel");
		}

		internal static void DoRevertAfterDialog(bool result)
		{
			if (result)
			{
				AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", ASCommitWindow.s_Callback);
				AssetServer.DoUpdateWithoutConflictResolutionOnNextTick(ASCommitWindow.s_AssetGuids.ToArray());
			}
		}

		internal static bool MarkSelected(ParentViewState activeState, List<string> guids)
		{
			int num = 0;
			bool flag = false;
			ParentViewFolder[] folders = activeState.folders;
			for (int i = 0; i < folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = folders[i];
				bool flag2 = guids.Contains(parentViewFolder.guid);
				activeState.selectedItems[num++] = flag2;
				flag |= flag2;
				ParentViewFile[] files = parentViewFolder.files;
				for (int j = 0; j < files.Length; j++)
				{
					ParentViewFile parentViewFile = files[j];
					flag2 = guids.Contains(parentViewFile.guid);
					activeState.selectedItems[num++] = flag2;
					flag |= flag2;
				}
			}
			return flag;
		}

		internal static AssetsItem[] GetCommits()
		{
			return AssetServer.GetChangedAssetsItems();
		}

		internal void AddToCommitMessageHistory(string description)
		{
			if (description.Trim() != string.Empty)
			{
				if (ArrayUtility.Contains<string>(this.commitMessageList, description))
				{
					ArrayUtility.Remove<string>(ref this.commitMessageList, description);
				}
				ArrayUtility.Insert<string>(ref this.commitMessageList, 0, description);
				InternalEditorUtility.SaveEditorSettingsList("ASCommitMsgs", this.commitMessageList, 20);
			}
		}

		internal static bool ShowDiscardWarning()
		{
			return EditorUtility.DisplayDialog("Discard changes", "More items will be discarded then initially selected. Dependencies of selected items where all marked in commit window. Please review.", "Discard", "Cancel");
		}

		internal bool CanCommit()
		{
			return this.pv2state.folders.Length != 0;
		}

		internal string[] GetItemsToCommit()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.pv2state.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = this.pv2state.folders[i];
				if (AssetServer.IsGUIDValid(parentViewFolder.guid) != 0)
				{
					list.Add(parentViewFolder.guid);
				}
				for (int j = 0; j < parentViewFolder.files.Length; j++)
				{
					if (AssetServer.IsGUIDValid(parentViewFolder.files[j].guid) != 0)
					{
						list.Add(parentViewFolder.files[j].guid);
					}
				}
			}
			return list.ToArray();
		}

		internal void DoCommit()
		{
			if (AssetServer.GetRefreshCommit())
			{
				this.SetPersistedData();
				this.InitiateReinit();
				this.showReinitedWarning = 2;
				this.parentWin.Repaint();
				GUIUtility.ExitGUI();
			}
			if (this.description == string.Empty && !EditorUtility.DisplayDialog("Commit without description", "Are you sure you want to commit with empty commit description message?", "Commit", "Cancel"))
			{
				return;
			}
			string[] itemsToCommit = this.GetItemsToCommit();
			this.SetPersistedData();
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBCommitFinished");
			AssetServer.DoCommitOnNextTick(this.description, itemsToCommit);
			GUIUtility.ExitGUI();
		}

		private bool TransferDependentParentFolders(ref List<string> guidsOfFoldersToRemove, string guid, bool leftToRight)
		{
			bool result = false;
			if (leftToRight)
			{
				while (AssetServer.IsGUIDValid(guid = AssetServer.GetParentGUID(guid, -1)) != 0)
				{
					if (!ASCommitWindow.AllFolderWouldBeMovedAnyway((!leftToRight) ? this.pv2state : this.pv1state, guid))
					{
						int num = ASCommitWindow.IndexOfFolderWithGUID(this.pv1state.folders, guid);
						int num2 = ASCommitWindow.IndexOfFolderWithGUID(this.pv2state.folders, guid);
						if (num != -1 || num2 != -1)
						{
							if (num != -1 && num2 == -1)
							{
								ChangeFlags changeFlags = this.pv1state.folders[num].changeFlags;
								if (ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Undeleted) || ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Created) || ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Moved))
								{
									ArrayUtility.Add<ParentViewFolder>(ref this.pv2state.folders, this.pv1state.folders[num].CloneWithoutFiles());
									result = true;
									if (this.pv1state.folders[num].files.Length == 0)
									{
										this.AddFolderToRemove(ref guidsOfFoldersToRemove, this.pv1state.folders[num].guid);
									}
								}
							}
						}
					}
				}
			}
			else
			{
				ChangeFlags changeFlags2 = this.pv1state.folders[ASCommitWindow.IndexOfFolderWithGUID(this.pv1state.folders, guid)].changeFlags;
				if (!ASCommitWindow.HasFlag(changeFlags2, ChangeFlags.Undeleted) && !ASCommitWindow.HasFlag(changeFlags2, ChangeFlags.Created) && !ASCommitWindow.HasFlag(changeFlags2, ChangeFlags.Moved))
				{
					return false;
				}
				for (int i = 0; i < this.pv2state.folders.Length; i++)
				{
					string guid2 = this.pv2state.folders[i].guid;
					if (AssetServer.GetParentGUID(guid2, -1) == guid)
					{
						int num3 = ASCommitWindow.IndexOfFolderWithGUID(this.pv1state.folders, guid2);
						if (num3 != -1)
						{
							ArrayUtility.AddRange<ParentViewFile>(ref this.pv1state.folders[num3].files, this.pv2state.folders[i].files);
						}
						else
						{
							ArrayUtility.Add<ParentViewFolder>(ref this.pv1state.folders, this.pv2state.folders[i]);
						}
						this.AddFolderToRemove(ref guidsOfFoldersToRemove, guid2);
						this.TransferDependentParentFolders(ref guidsOfFoldersToRemove, guid2, leftToRight);
						result = true;
					}
				}
			}
			return result;
		}

		private bool TransferDeletedDependentParentFolders(ref List<string> guidsOfFoldersToRemove, string guid, bool leftToRight)
		{
			bool flag = false;
			ParentViewState parentViewState = (!leftToRight) ? this.pv2state : this.pv1state;
			ParentViewState parentViewState2 = (!leftToRight) ? this.pv1state : this.pv2state;
			if (leftToRight)
			{
				for (int i = 0; i < parentViewState.folders.Length; i++)
				{
					ParentViewFolder parentViewFolder = parentViewState.folders[i];
					if (AssetServer.GetParentGUID(parentViewFolder.guid, -1) == guid)
					{
						if (!ASCommitWindow.AllFolderWouldBeMovedAnyway(parentViewState, parentViewFolder.guid))
						{
							if (!ASCommitWindow.HasFlag(parentViewFolder.changeFlags, ChangeFlags.Deleted))
							{
								Debug.LogError("Folder of nested deleted folders marked as not deleted (" + parentViewFolder.name + ")");
								return false;
							}
							for (int j = 0; j < parentViewFolder.files.Length; j++)
							{
								if (!ASCommitWindow.HasFlag(parentViewFolder.files[j].changeFlags, ChangeFlags.Deleted))
								{
									Debug.LogError("File of nested deleted folder is marked as not deleted (" + parentViewFolder.files[j].name + ")");
									return false;
								}
							}
							flag |= this.TransferDeletedDependentParentFolders(ref guidsOfFoldersToRemove, parentViewFolder.guid, leftToRight);
							int num = ASCommitWindow.IndexOfFolderWithGUID(parentViewState2.folders, parentViewFolder.guid);
							if (num == -1)
							{
								ArrayUtility.Add<ParentViewFolder>(ref parentViewState2.folders, parentViewFolder);
							}
							this.AddFolderToRemove(ref guidsOfFoldersToRemove, parentViewFolder.guid);
							flag = true;
						}
					}
				}
			}
			else
			{
				while (AssetServer.IsGUIDValid(guid = AssetServer.GetParentGUID(guid, -1)) != 0)
				{
					int num2 = ASCommitWindow.IndexOfFolderWithGUID(this.pv2state.folders, guid);
					if (num2 == -1)
					{
						break;
					}
					if (ASCommitWindow.HasFlag(this.pv2state.folders[num2].changeFlags, ChangeFlags.Deleted))
					{
						ArrayUtility.Add<ParentViewFolder>(ref this.pv1state.folders, this.pv2state.folders[num2]);
						flag = true;
						this.AddFolderToRemove(ref guidsOfFoldersToRemove, this.pv2state.folders[num2].guid);
					}
				}
			}
			return flag;
		}

		private bool DoTransfer(ref ParentViewFolder[] foldersFrom, ref ParentViewFolder[] foldersTo, int folder, int file, ref List<string> guidsOfFoldersToRemove, bool leftToRight)
		{
			ParentViewFolder parentViewFolder = foldersFrom[folder];
			ParentViewFolder parentViewFolder2 = null;
			string name = parentViewFolder.name;
			bool flag = false;
			bool flag2 = false;
			if (file == -1)
			{
				this.AddFolderToRemove(ref guidsOfFoldersToRemove, foldersFrom[folder].guid);
				int num = ParentViewState.IndexOf(foldersTo, name);
				if (num != -1)
				{
					parentViewFolder2 = foldersTo[num];
					ArrayUtility.AddRange<ParentViewFile>(ref parentViewFolder2.files, parentViewFolder.files);
				}
				else
				{
					ArrayUtility.Add<ParentViewFolder>(ref foldersTo, parentViewFolder);
					flag2 = true;
					if (!ASCommitWindow.HasFlag(parentViewFolder.changeFlags, ChangeFlags.Deleted))
					{
						flag = this.TransferDependentParentFolders(ref guidsOfFoldersToRemove, parentViewFolder.guid, leftToRight);
					}
					else
					{
						flag = this.TransferDeletedDependentParentFolders(ref guidsOfFoldersToRemove, parentViewFolder.guid, leftToRight);
					}
				}
			}
			else
			{
				int num2 = ParentViewState.IndexOf(foldersTo, name);
				if (num2 == -1)
				{
					if (ASCommitWindow.HasFlag(parentViewFolder.files[file].changeFlags, ChangeFlags.Deleted) && ASCommitWindow.HasFlag(parentViewFolder.changeFlags, ChangeFlags.Deleted))
					{
						ArrayUtility.Add<ParentViewFolder>(ref foldersTo, parentViewFolder);
						this.AddFolderToRemove(ref guidsOfFoldersToRemove, parentViewFolder.guid);
						num2 = foldersTo.Length - 1;
						if (!ASCommitWindow.AllFolderWouldBeMovedAnyway((!leftToRight) ? this.pv2state : this.pv1state, parentViewFolder.guid))
						{
							flag = true;
						}
						flag |= this.TransferDeletedDependentParentFolders(ref guidsOfFoldersToRemove, parentViewFolder.guid, leftToRight);
					}
					else
					{
						ArrayUtility.Add<ParentViewFolder>(ref foldersTo, parentViewFolder.CloneWithoutFiles());
						num2 = foldersTo.Length - 1;
						flag = this.TransferDependentParentFolders(ref guidsOfFoldersToRemove, parentViewFolder.guid, leftToRight);
					}
					flag2 = true;
				}
				parentViewFolder2 = foldersTo[num2];
				ArrayUtility.Add<ParentViewFile>(ref parentViewFolder2.files, parentViewFolder.files[file]);
				ArrayUtility.RemoveAt<ParentViewFile>(ref parentViewFolder.files, file);
				if (parentViewFolder.files.Length == 0)
				{
					this.AddFolderToRemove(ref guidsOfFoldersToRemove, foldersFrom[folder].guid);
				}
			}
			if (parentViewFolder2 != null)
			{
				Array.Sort<ParentViewFile>(parentViewFolder2.files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
			}
			if (flag2)
			{
				Array.Sort<ParentViewFolder>(foldersTo, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
			}
			return flag;
		}

		private bool MarkDependantFiles(ParentViewState pvState)
		{
			string[] array = new string[0];
			bool result = false;
			if (pvState == this.pv1state)
			{
				array = AssetServer.CollectAllDependencies(ASCommitWindow.GetParentViewSelectedItems(this.pv1state, false, false).ToArray());
				if (array.Length != 0)
				{
					int i = 1;
					int num = 0;
					while (i < pvState.lv.totalRows)
					{
						int j = 0;
						while (j < pvState.folders[num].files.Length)
						{
							if (!pvState.selectedItems[i])
							{
								for (int k = 0; k < array.Length; k++)
								{
									if (array[k] == pvState.folders[num].files[j].guid)
									{
										pvState.selectedItems[i] = true;
										result = true;
										break;
									}
								}
							}
							j++;
							i++;
						}
						num++;
						i++;
					}
				}
			}
			return result;
		}

		private void DoTransferAll(ParentViewState pvState, ParentViewState anotherPvState, int selFolder, int selFile)
		{
			List<string> list = new List<string>();
			bool flag = this.MarkDependantFiles(pvState);
			int i = pvState.lv.totalRows - 1;
			for (int j = pvState.folders.Length - 1; j >= 0; j--)
			{
				ParentViewFolder parentViewFolder = pvState.folders[j];
				bool flag2 = false;
				for (int k = parentViewFolder.files.Length - 1; k >= -1; k--)
				{
					if (!list.Contains(parentViewFolder.guid) && pvState.selectedItems[i])
					{
						if (k != -1 || !flag2)
						{
							flag |= this.DoTransfer(ref pvState.folders, ref anotherPvState.folders, j, k, ref list, pvState == this.pv1state);
						}
						flag2 = true;
					}
					i--;
				}
			}
			for (i = pvState.folders.Length - 1; i >= 0; i--)
			{
				if (list.Contains(pvState.folders[i].guid))
				{
					list.Remove(pvState.folders[i].guid);
					ArrayUtility.RemoveAt<ParentViewFolder>(ref pvState.folders, i);
				}
			}
			this.pv1state.SetLineCount();
			this.pv2state.SetLineCount();
			this.pv1state.ClearSelection();
			this.pv2state.ClearSelection();
			pvState.selectedFile = -1;
			pvState.selectedFolder = -1;
			AssetServer.SetSelectionFromGUID(string.Empty);
			this.lastTransferMovedDependencies = flag;
		}

		private static bool AnyOfTheParentsIsSelected(ref ParentViewState pvState, string guid)
		{
			string text = guid;
			while (AssetServer.IsGUIDValid(text = AssetServer.GetParentGUID(text, -1)) != 0)
			{
				if (ASCommitWindow.AllFolderWouldBeMovedAnyway(pvState, text))
				{
					return true;
				}
			}
			return false;
		}

		public static bool MarkAllFolderDependenciesForDiscarding(ParentViewState pvState, ParentViewState anotherPvState)
		{
			bool result = false;
			bool flag = false;
			int num = 0;
			List<string> list = new List<string>();
			for (int i = 0; i < pvState.folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = pvState.folders[i];
				if (ASCommitWindow.HasFlag(parentViewFolder.changeFlags, ChangeFlags.Deleted))
				{
					bool flag2 = false;
					for (int j = 1; j <= parentViewFolder.files.Length; j++)
					{
						if (pvState.selectedItems[num + j])
						{
							flag2 = true;
							pvState.selectedItems[num] = true;
							list.Add(parentViewFolder.guid);
							break;
						}
					}
					if (pvState.selectedItems[num] || flag2)
					{
						string text = parentViewFolder.guid;
						while (AssetServer.IsGUIDValid(text = AssetServer.GetParentGUID(text, -1)) != 0)
						{
							int num2 = ASCommitWindow.IndexOfFolderWithGUID(pvState.folders, text);
							if (num2 == -1)
							{
								break;
							}
							num2 = ASCommitWindow.FolderIndexToTotalIndex(pvState.folders, num2);
							if (!pvState.selectedItems[num2] && ASCommitWindow.HasFlag(pvState.folders[num2].changeFlags, ChangeFlags.Deleted))
							{
								pvState.selectedItems[num2] = true;
								list.Add(text);
								result = true;
							}
						}
					}
				}
				else if (!ASCommitWindow.AllFolderWouldBeMovedAnyway(pvState, parentViewFolder.guid))
				{
					if (ASCommitWindow.AnyOfTheParentsIsSelected(ref pvState, parentViewFolder.guid))
					{
						pvState.selectedItems[num] = true;
						list.Add(parentViewFolder.guid);
						for (int k = 1; k <= parentViewFolder.files.Length; k++)
						{
							pvState.selectedItems[num + k] = true;
						}
						result = true;
					}
				}
				else
				{
					for (int l = 1; l <= parentViewFolder.files.Length; l++)
					{
						if (!pvState.selectedItems[num + l])
						{
							pvState.selectedItems[num + l] = true;
						}
					}
					list.Add(parentViewFolder.guid);
				}
				num += 1 + pvState.folders[i].files.Length;
			}
			if (anotherPvState != null)
			{
				for (int m = 0; m < anotherPvState.folders.Length; m++)
				{
					ParentViewFolder parentViewFolder2 = anotherPvState.folders[m];
					if (ASCommitWindow.AnyOfTheParentsIsSelected(ref pvState, parentViewFolder2.guid))
					{
						list.Add(parentViewFolder2.guid);
					}
				}
				for (int n = anotherPvState.folders.Length - 1; n >= 0; n--)
				{
					if (list.Contains(anotherPvState.folders[n].guid))
					{
						ParentViewFolder parentViewFolder3 = anotherPvState.folders[n];
						int num3 = ASCommitWindow.FolderSelectionIndexFromGUID(pvState.folders, parentViewFolder3.guid);
						if (num3 != -1)
						{
							ParentViewFolder parentViewFolder4 = pvState.folders[num3];
							int length = pvState.lv.totalRows - num3 - 1 - parentViewFolder4.files.Length;
							int num4 = num3 + 1 + parentViewFolder4.files.Length;
							Array.Copy(pvState.selectedItems, num4, pvState.selectedItems, num4 + parentViewFolder3.files.Length, length);
							ArrayUtility.AddRange<ParentViewFile>(ref parentViewFolder4.files, parentViewFolder3.files);
							for (int num5 = 1; num5 <= parentViewFolder4.files.Length; num5++)
							{
								pvState.selectedItems[num3 + num5] = true;
							}
							Array.Sort<ParentViewFile>(parentViewFolder4.files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
						}
						else
						{
							num3 = 0;
							for (int num6 = 0; num6 < pvState.folders.Length; num6++)
							{
								if (ParentViewState.CompareViewFolder(pvState.folders[num3], parentViewFolder3) > 0)
								{
									break;
								}
								num3 += 1 + pvState.folders[num6].files.Length;
							}
							int length2 = pvState.lv.totalRows - num3;
							int num7 = num3;
							Array.Copy(pvState.selectedItems, num7, pvState.selectedItems, num7 + 1 + parentViewFolder3.files.Length, length2);
							ArrayUtility.Add<ParentViewFolder>(ref pvState.folders, parentViewFolder3);
							for (int num8 = 0; num8 <= parentViewFolder3.files.Length; num8++)
							{
								pvState.selectedItems[num3 + num8] = true;
							}
							flag = true;
						}
						ArrayUtility.RemoveAt<ParentViewFolder>(ref anotherPvState.folders, n);
						result = true;
					}
				}
				anotherPvState.SetLineCount();
			}
			pvState.SetLineCount();
			if (flag)
			{
				Array.Sort<ParentViewFolder>(pvState.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
			}
			return result;
		}

		private static bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
		{
			return (flagToCheck & flags) != ChangeFlags.None;
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
			if (this.pv1hasSelection)
			{
				this.pv1hasSelection = ASCommitWindow.MarkSelected(this.pv1state, list);
			}
			if (!this.pv1hasSelection)
			{
				if (this.pv2hasSelection)
				{
					this.pv2hasSelection = ASCommitWindow.MarkSelected(this.pv2state, list);
				}
				if (!this.pv2hasSelection)
				{
					this.pv1hasSelection = ASCommitWindow.MarkSelected(this.pv1state, list);
					if (!this.pv1hasSelection)
					{
						this.pv2hasSelection = ASCommitWindow.MarkSelected(this.pv2state, list);
					}
				}
			}
		}

		internal void OnSelectionChange()
		{
			if (!this.mySelection)
			{
				this.DoSelectionChange();
				this.parentWin.Repaint();
			}
			else
			{
				this.mySelection = false;
			}
			this.somethingDiscardableSelected = ASCommitWindow.SomethingDiscardableSelected((!this.pv2hasSelection) ? this.pv1state : this.pv2state);
		}

		public static bool SomethingDiscardableSelected(ParentViewState st)
		{
			int num = 0;
			ParentViewFolder[] folders = st.folders;
			for (int i = 0; i < folders.Length; i++)
			{
				ParentViewFolder parentViewFolder = folders[i];
				if (st.selectedItems[num++])
				{
					return true;
				}
				ParentViewFile[] files = parentViewFolder.files;
				for (int j = 0; j < files.Length; j++)
				{
					ParentViewFile parentViewFile = files[j];
					if (st.selectedItems[num++] && ASCommitWindow.IsDiscardableAsset(parentViewFile.guid, parentViewFile.changeFlags))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool AllFolderWouldBeMovedAnyway(ParentViewState pvState, string guid)
		{
			int num = 0;
			for (int i = 0; i < pvState.folders.Length; i++)
			{
				if (pvState.folders[i].guid == guid)
				{
					bool flag = true;
					bool flag2 = true;
					bool flag3 = pvState.selectedItems[num++];
					for (int j = 0; j < pvState.folders[i].files.Length; j++)
					{
						if (pvState.selectedItems[num++])
						{
							flag = false;
						}
						else
						{
							flag2 = false;
						}
					}
					return flag3 && (flag2 || flag);
				}
				num += 1 + pvState.folders[i].files.Length;
			}
			return false;
		}

		private bool DoShowMyDiff(bool binary)
		{
			return ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems((!this.pv2hasSelection) ? this.pv1state : this.pv2state, false, false), binary);
		}

		private void DoMyRevert(bool afterMarkingDependencies)
		{
			if (!afterMarkingDependencies)
			{
				List<string> selectedItems = this.GetSelectedItems();
				bool flag;
				if (this.pv2hasSelection)
				{
					flag = ASCommitWindow.MarkAllFolderDependenciesForDiscarding(this.pv2state, this.pv1state);
				}
				else
				{
					flag = ASCommitWindow.MarkAllFolderDependenciesForDiscarding(this.pv1state, this.pv2state);
				}
				if (flag)
				{
					this.MySelectionToGlobalSelection();
				}
				List<string> selectedItems2 = this.GetSelectedItems();
				if (selectedItems.Count != selectedItems2.Count)
				{
					flag = true;
				}
				this.lastRevertSelectionChanged = ((!flag) ? -1 : 1);
			}
			if (afterMarkingDependencies || this.lastRevertSelectionChanged == -1)
			{
				this.SetPersistedData();
				ASCommitWindow.DoRevert(ASCommitWindow.GetParentViewSelectedItems((!this.pv2hasSelection) ? this.pv1state : this.pv2state, true, true), "CBReinitCommitWindow");
			}
		}

		private void MenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				this.description = this.commitMessageList[selected];
				this.resetKeyboardControl = true;
				this.parentWin.Repaint();
			}
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				string text = this.dropDownMenuItems[selected];
				switch (text)
				{
				case "Compare":
					this.DoShowMyDiff(false);
					break;
				case "Compare Binary":
					this.DoShowMyDiff(true);
					break;
				case "Discard":
					this.DoMyRevert(false);
					break;
				case ">>>":
					this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
					break;
				case "<<<":
					this.DoTransferAll(this.pv2state, this.pv1state, this.pv2state.selectedFolder, this.pv2state.selectedFile);
					break;
				}
			}
		}

		private static int IndexOfFolderWithGUID(ParentViewFolder[] folders, string guid)
		{
			for (int i = 0; i < folders.Length; i++)
			{
				if (folders[i].guid == guid)
				{
					return i;
				}
			}
			return -1;
		}

		private static int FolderIndexToTotalIndex(ParentViewFolder[] folders, int folderIndex)
		{
			int num = 0;
			for (int i = 0; i < folderIndex; i++)
			{
				num += folders[i].files.Length + 1;
			}
			return num;
		}

		private static int FolderSelectionIndexFromGUID(ParentViewFolder[] folders, string guid)
		{
			int num = 0;
			for (int i = 0; i < folders.Length; i++)
			{
				if (guid == folders[i].guid)
				{
					return num;
				}
				num += 1 + folders[i].files.Length;
			}
			return -1;
		}

		private void AddFolderToRemove(ref List<string> guidsOfFoldersToRemove, string guid)
		{
			if (!guidsOfFoldersToRemove.Contains(guid))
			{
				guidsOfFoldersToRemove.Add(guid);
			}
		}

		private bool ParentViewGUI(ParentViewState pvState, ParentViewState anotherPvState, ref bool hasSelection)
		{
			bool flag = false;
			EditorGUIUtility.SetIconSize(this.iconSize);
			ListViewState lv = pvState.lv;
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			int row = lv.row;
			int num = -1;
			int num2 = -1;
			bool flag2 = false;
			foreach (ListViewElement listViewElement in ListViewGUILayout.ListView(lv, (ListViewOptions)12, this.dragTitle, GUIStyle.none, new GUILayoutOption[0]))
			{
				if (num == -1 && !pvState.IndexToFolderAndFile(listViewElement.row, ref num, ref num2))
				{
					flag = true;
					break;
				}
				if (GUIUtility.keyboardControl == lv.ID && Event.current.type == EventType.KeyDown && actionKey)
				{
					Event.current.Use();
				}
				ParentViewFolder parentViewFolder = pvState.folders[num];
				if (pvState.selectedItems[listViewElement.row] && Event.current.type == EventType.Repaint)
				{
					ASCommitWindow.constants.entrySelected.Draw(listViewElement.position, false, false, false, false);
				}
				if (ListViewGUILayout.HasMouseUp(listViewElement.position))
				{
					if (!shift && !actionKey)
					{
						flag2 |= ListViewGUILayout.MultiSelection(row, pvState.lv.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
					}
				}
				else if (ListViewGUILayout.HasMouseDown(listViewElement.position))
				{
					if (Event.current.clickCount == 2)
					{
						this.DoShowMyDiff(false);
						GUIUtility.ExitGUI();
					}
					else
					{
						if (!pvState.selectedItems[listViewElement.row] || shift || actionKey)
						{
							flag2 |= ListViewGUILayout.MultiSelection(row, listViewElement.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
						}
						pvState.selectedFile = num2;
						pvState.selectedFolder = num;
						lv.row = listViewElement.row;
					}
				}
				else if (ListViewGUILayout.HasMouseDown(listViewElement.position, 1))
				{
					if (!pvState.selectedItems[listViewElement.row])
					{
						flag2 = true;
						pvState.ClearSelection();
						pvState.selectedItems[listViewElement.row] = true;
						pvState.selectedFile = num2;
						pvState.selectedFolder = num;
						lv.row = listViewElement.row;
					}
					this.dropDownMenuItems[0] = ((pvState != this.pv1state) ? "<<<" : ">>>");
					GUIUtility.hotControl = 0;
					Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
					EditorUtility.DisplayCustomMenu(position, this.dropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
					Event.current.Use();
				}
				ChangeFlags changeFlags;
				if (num2 != -1)
				{
					Texture2D texture2D = AssetDatabase.GetCachedIcon(parentViewFolder.name + "/" + parentViewFolder.files[num2].name) as Texture2D;
					if (texture2D == null)
					{
						texture2D = InternalEditorUtility.GetIconForFile(parentViewFolder.files[num2].name);
					}
					GUILayout.Label(new GUIContent(parentViewFolder.files[num2].name, texture2D), ASCommitWindow.constants.element, new GUILayoutOption[0]);
					changeFlags = parentViewFolder.files[num2].changeFlags;
				}
				else
				{
					GUILayout.Label(parentViewFolder.name, ASCommitWindow.constants.header, new GUILayoutOption[0]);
					changeFlags = parentViewFolder.changeFlags;
				}
				GUIContent gUIContent = null;
				if (ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Undeleted) || ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Created))
				{
					gUIContent = ASMainWindow.constants.badgeNew;
				}
				else if (ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Deleted))
				{
					gUIContent = ASMainWindow.constants.badgeDelete;
				}
				else if (ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Renamed) || ASCommitWindow.HasFlag(changeFlags, ChangeFlags.Moved))
				{
					gUIContent = ASMainWindow.constants.badgeMove;
				}
				if (gUIContent != null && Event.current.type == EventType.Repaint)
				{
					Rect position2 = new Rect(listViewElement.position.x + listViewElement.position.width - (float)gUIContent.image.width - 5f, listViewElement.position.y + listViewElement.position.height / 2f - (float)(gUIContent.image.height / 2), (float)gUIContent.image.width, (float)gUIContent.image.height);
					EditorGUIUtility.SetIconSize(Vector2.zero);
					GUIStyle.none.Draw(position2, gUIContent, false, false, false, false);
					EditorGUIUtility.SetIconSize(this.iconSize);
				}
				pvState.NextFileFolder(ref num, ref num2);
			}
			if (!flag)
			{
				if (GUIUtility.keyboardControl == lv.ID)
				{
					if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
					{
						Event.current.Use();
					}
					else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
					{
						for (int i = 0; i < pvState.selectedItems.Length; i++)
						{
							pvState.selectedItems[i] = true;
						}
						flag2 = true;
						Event.current.Use();
					}
				}
				if (lv.customDraggedFromID != 0 && lv.customDraggedFromID == anotherPvState.lv.ID)
				{
					this.DoTransferAll(anotherPvState, pvState, pvState.selectedFolder, pvState.selectedFile);
				}
				if (GUIUtility.keyboardControl == lv.ID && !actionKey)
				{
					if (lv.selectionChanged)
					{
						flag2 |= ListViewGUILayout.MultiSelection(row, lv.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
						if (!pvState.IndexToFolderAndFile(lv.row, ref pvState.selectedFolder, ref pvState.selectedFile))
						{
							flag = true;
						}
					}
					else if (pvState.selectedFolder != -1 && Event.current.type == EventType.KeyDown && GUIUtility.keyboardControl == lv.ID && Event.current.keyCode == KeyCode.Return)
					{
						this.DoTransferAll(pvState, anotherPvState, pvState.selectedFolder, pvState.selectedFile);
						ListViewGUILayout.MultiSelection(row, lv.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
						pvState.IndexToFolderAndFile(lv.row, ref pvState.selectedFolder, ref pvState.selectedFile);
						Event.current.Use();
						flag = true;
					}
				}
				if (lv.selectionChanged || flag2)
				{
					if (pvState.IndexToFolderAndFile(lv.row, ref num, ref num2))
					{
						this.dragTitle = ((num2 != -1) ? pvState.folders[num].files[num2].name : pvState.folders[num].name);
					}
					anotherPvState.ClearSelection();
					anotherPvState.lv.row = -1;
					anotherPvState.selectedFile = -1;
					anotherPvState.selectedFolder = -1;
					this.MySelectionToGlobalSelection();
				}
			}
			EditorGUIUtility.SetIconSize(Vector2.zero);
			return !flag;
		}

		internal bool DoGUI()
		{
			bool enabled = GUI.enabled;
			if (ASCommitWindow.constants == null)
			{
				ASCommitWindow.constants = new ASCommitWindow.Constants();
			}
			if (this.resetKeyboardControl)
			{
				this.resetKeyboardControl = false;
				GUIUtility.keyboardControl = 0;
			}
			bool flag = this.parentWin.position.width <= 432f;
			if (Event.current.type == EventType.Layout)
			{
				this.wasHidingButtons = flag;
			}
			else if (flag != this.wasHidingButtons)
			{
				GUIUtility.ExitGUI();
			}
			SplitterGUILayout.BeginHorizontalSplit(this.horSplit, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASCommitWindow.constants.box, new GUILayoutOption[0]);
			GUILayout.Label(this.totalChanges, ASCommitWindow.constants.title, new GUILayoutOption[0]);
			if (!this.ParentViewGUI(this.pv1state, this.pv2state, ref this.pv1hasSelection))
			{
				return true;
			}
			GUILayout.EndVertical();
			SplitterGUILayout.BeginVerticalSplit(this.vertSplit, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASCommitWindow.constants.box, new GUILayoutOption[0]);
			GUILayout.Label("Changeset", ASCommitWindow.constants.title, new GUILayoutOption[0]);
			if (!this.ParentViewGUI(this.pv2state, this.pv1state, ref this.pv2hasSelection))
			{
				return true;
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Commit Message", ASCommitWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (this.commitMessageList.Length > 0)
			{
				GUIContent content = new GUIContent("Recent");
				Rect rect = GUILayoutUtility.GetRect(content, ASCommitWindow.constants.dropDown, null);
				if (GUI.Button(rect, content, ASCommitWindow.constants.dropDown))
				{
					GUIUtility.hotControl = 0;
					string[] array = new string[this.commitMessageList.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = ((this.commitMessageList[i].Length <= 200) ? this.commitMessageList[i] : (this.commitMessageList[i].Substring(0, 200) + " ... "));
					}
					EditorUtility.DisplayCustomMenu(rect, array, null, new EditorUtility.SelectMenuItemFunction(this.MenuClick), null);
					Event.current.Use();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(ASCommitWindow.constants.box, new GUILayoutOption[0]);
			this.scrollPos = EditorGUILayout.BeginVerticalScrollView(this.scrollPos, new GUILayoutOption[0]);
			this.description = EditorGUILayout.TextArea(this.description, ASCommitWindow.constants.wwText, new GUILayoutOption[0]);
			EditorGUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			SplitterGUILayout.EndVerticalSplit();
			SplitterGUILayout.EndHorizontalSplit();
			if (!flag)
			{
				GUILayout.Label("Please drag files you want to commit to Changeset and fill in commit description", new GUILayoutOption[0]);
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (!this.pv1hasSelection && !this.pv2hasSelection)
			{
				GUI.enabled = false;
			}
			if (!flag && GUILayout.Button("Compare", ASCommitWindow.constants.button, new GUILayoutOption[0]))
			{
				this.DoShowMyDiff(false);
				GUIUtility.ExitGUI();
			}
			bool enabled2 = GUI.enabled;
			if (!this.somethingDiscardableSelected)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button((!flag) ? "Discard Selected Changes" : "Discard", ASCommitWindow.constants.button, new GUILayoutOption[0]))
			{
				this.DoMyRevert(false);
				GUIUtility.ExitGUI();
			}
			GUI.enabled = enabled2;
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.pv1hasSelection && enabled);
			if (GUILayout.Button((!flag) ? ">>>" : ">", ASCommitWindow.constants.button, new GUILayoutOption[0]))
			{
				this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
			}
			GUI.enabled = (this.pv2hasSelection && enabled);
			if (GUILayout.Button((!flag) ? "<<<" : "<", ASCommitWindow.constants.button, new GUILayoutOption[0]))
			{
				this.DoTransferAll(this.pv2state, this.pv1state, this.pv2state.selectedFolder, this.pv2state.selectedFile);
			}
			GUI.enabled = (this.pv1state.lv.totalRows != 0 && enabled);
			if (GUILayout.Button("Add All", ASCommitWindow.constants.button, new GUILayoutOption[0]))
			{
				int j = 0;
				while (j < this.pv1state.selectedItems.Length)
				{
					this.pv1state.selectedItems[j++] = true;
				}
				this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
			}
			GUI.enabled = (this.pv2state.lv.totalRows != 0 && enabled);
			if (GUILayout.Button("Remove All", ASCommitWindow.constants.button, new GUILayoutOption[0]))
			{
				int k = 0;
				while (k < this.pv2state.selectedItems.Length)
				{
					this.pv2state.selectedItems[k++] = true;
				}
				this.DoTransferAll(this.pv2state, this.pv1state, this.pv2state.selectedFolder, this.pv2state.selectedFile);
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (!this.CanCommit())
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button("Commit", ASCommitWindow.constants.bigButton, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}))
			{
				this.DoCommit();
			}
			GUI.enabled = enabled;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.KeypadEnter && Application.platform == RuntimePlatform.OSXEditor && this.CanCommit())
			{
				this.DoCommit();
			}
			GUILayout.EndHorizontal();
			if (AssetServer.GetAssetServerError() != string.Empty)
			{
				GUILayout.Space(10f);
				GUILayout.Label(AssetServer.GetAssetServerError(), ASCommitWindow.constants.errorLabel, new GUILayoutOption[0]);
				GUILayout.Space(10f);
			}
			GUILayout.Space(10f);
			if (this.lastRevertSelectionChanged == 0)
			{
				this.lastRevertSelectionChanged = -1;
				if (ASCommitWindow.ShowDiscardWarning())
				{
					this.DoMyRevert(true);
				}
			}
			if (this.lastRevertSelectionChanged > 0)
			{
				if (Event.current.type == EventType.Repaint)
				{
					this.lastRevertSelectionChanged--;
				}
				this.parentWin.Repaint();
			}
			if (this.showReinitedWarning == 0)
			{
				EditorUtility.DisplayDialog("Commits updated", "Commits had to be updated to reflect latest changes", "OK", string.Empty);
				this.showReinitedWarning = -1;
			}
			if (this.showReinitedWarning > 0)
			{
				if (Event.current.type == EventType.Repaint)
				{
					this.showReinitedWarning--;
				}
				this.parentWin.Repaint();
			}
			return true;
		}
	}
}
