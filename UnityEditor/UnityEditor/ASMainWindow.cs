using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Server", useTypeNameAsIconName = true)]
	internal class ASMainWindow : EditorWindow, IHasCustomMenu
	{
		internal class Constants
		{
			public GUIStyle background = "OL Box";

			public GUIStyle groupBox;

			public GUIStyle groupBoxNoMargin;

			public GUIStyle contentBox = "GroupBox";

			public GUIStyle entrySelected = "ServerUpdateChangesetOn";

			public GUIStyle entryNormal = "ServerUpdateChangeset";

			public GUIStyle element = "OL elem";

			public GUIStyle header = "OL header";

			public GUIStyle title = "OL Title";

			public GUIStyle columnHeader = "OL Title";

			public GUIStyle serverUpdateLog = "ServerUpdateLog";

			public GUIStyle serverUpdateInfo = "ServerUpdateInfo";

			public GUIStyle smallButton = "Button";

			public GUIStyle errorLabel = "ErrorLabel";

			public GUIStyle miniButton = "MiniButton";

			public GUIStyle button = "Button";

			public GUIStyle largeButton = "ButtonMid";

			public GUIStyle bigButton = "LargeButton";

			public GUIStyle entryEven = "CN EntryBackEven";

			public GUIStyle entryOdd = "CN EntryBackOdd";

			public GUIStyle dropDown = "MiniPullDown";

			public GUIStyle toggle = "Toggle";

			public GUIContent badgeDelete = EditorGUIUtility.IconContent("AS Badge Delete");

			public GUIContent badgeMove = EditorGUIUtility.IconContent("AS Badge Move");

			public GUIContent badgeNew = EditorGUIUtility.IconContent("AS Badge New");

			public Vector2 toggleSize;

			public Constants()
			{
				this.groupBoxNoMargin = new GUIStyle();
				this.groupBox = new GUIStyle();
				this.groupBox.margin = new RectOffset(10, 10, 10, 10);
				this.contentBox = new GUIStyle(this.contentBox);
				this.contentBox.margin = new RectOffset(0, 0, 0, 0);
				this.contentBox.overflow = new RectOffset(0, 1, 0, 1);
				this.contentBox.padding = new RectOffset(8, 8, 7, 7);
				this.title = new GUIStyle(this.title);
				RectOffset arg_232_0 = this.title.padding;
				int num = this.contentBox.padding.left + 2;
				this.title.padding.right = num;
				arg_232_0.left = num;
				this.background = new GUIStyle(this.background);
				this.background.padding.top = 1;
			}
		}

		public enum ShowSearchField
		{
			None,
			ProjectView,
			HistoryList
		}

		internal enum Page
		{
			NotInitialized = -1,
			Overview,
			Update,
			Commit,
			History,
			ServerConfig,
			Admin
		}

		[Serializable]
		public class SearchField
		{
			private string m_FilterText = string.Empty;

			private bool m_Show;

			public string FilterText
			{
				get
				{
					return this.m_FilterText;
				}
			}

			public bool Show
			{
				get
				{
					return this.m_Show;
				}
				set
				{
					this.m_Show = value;
				}
			}

			public bool DoGUI()
			{
				GUI.SetNextControlName("SearchFilter");
				string text = EditorGUILayout.ToolbarSearchField(this.m_FilterText, new GUILayoutOption[0]);
				if (this.m_FilterText != text)
				{
					this.m_FilterText = text;
					return true;
				}
				return false;
			}
		}

		private const ASMainWindow.Page lastMainPage = ASMainWindow.Page.Commit;

		public static ASMainWindow.Constants constants;

		public AssetsItem[] sharedCommits;

		public AssetsItem[] sharedDeletedItems;

		public Changeset[] sharedChangesets;

		private GUIContent[] changesetContents;

		public ASMainWindow.ShowSearchField m_ShowSearch;

		public ASMainWindow.ShowSearchField m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;

		public ASMainWindow.SearchField m_SearchField = new ASMainWindow.SearchField();

		private string[] pageTitles = new string[]
		{
			"Overview",
			"Update",
			"Commit",
			string.Empty
		};

		private string[] dropDownMenuItems = new string[]
		{
			"Connection",
			string.Empty,
			"Show History",
			"Discard Changes",
			string.Empty,
			"Server Administration"
		};

		private string[] unconfiguredDropDownMenuItems = new string[]
		{
			"Connection",
			string.Empty,
			"Server Administration"
		};

		private string[] commitDropDownMenuItems = new string[]
		{
			"Commit",
			string.Empty,
			"Compare",
			"Compare Binary",
			string.Empty,
			"Discard"
		};

		private bool needsSetup = true;

		private string connectionString = string.Empty;

		private int maxNickLength = 1;

		private bool showSmallWindow;

		private int widthToHideButtons = 591;

		private bool wasHidingButtons;

		private ASMainWindow.Page selectedPage = ASMainWindow.Page.NotInitialized;

		private ListViewState lv = new ListViewState(0);

		private ParentViewState pv = new ParentViewState();

		internal ASHistoryWindow asHistoryWin;

		internal ASUpdateWindow asUpdateWin;

		internal ASCommitWindow asCommitWin;

		internal ASServerAdminWindow asAdminWin;

		internal ASConfigWindow asConfigWin;

		private bool error;

		private bool isInitialUpdate;

		private Vector2 iconSize = new Vector2(16f, 16f);

		private SplitterState splitter = new SplitterState(new float[]
		{
			50f,
			50f
		}, new int[]
		{
			80,
			80
		}, null);

		private bool committing;

		private bool selectionChangedWhileCommitting;

		private string commitMessage = string.Empty;

		private bool pvHasSelection;

		private bool somethingDiscardableSelected;

		private bool mySelection;

		private bool focusCommitMessage;

		private int lastRevertSelectionChanged = -1;

		private bool m_CheckedMaint;

		public bool NeedsSetup
		{
			get
			{
				return this.needsSetup;
			}
			set
			{
				this.needsSetup = value;
			}
		}

		public bool Error
		{
			get
			{
				return this.error;
			}
		}

		public ASMainWindow()
		{
			base.position = new Rect(50f, 50f, 800f, 600f);
		}

		public void LogError(string errorStr)
		{
			Debug.LogError(errorStr);
			AssetServer.SetAssetServerError(errorStr, false);
			this.error = true;
		}

		private void Awake()
		{
			this.pv.lv = new ListViewState(0);
			this.isInitialUpdate = true;
		}

		private void NotifyClosingCommit()
		{
			if (this.asCommitWin != null)
			{
				this.asCommitWin.OnClose();
			}
		}

		private void OnDestroy()
		{
			this.sharedCommits = null;
			this.sharedDeletedItems = null;
			this.sharedChangesets = null;
			this.changesetContents = null;
			if (this.selectedPage == ASMainWindow.Page.Commit)
			{
				this.NotifyClosingCommit();
			}
		}

		private void DoSelectionChange()
		{
			if (this.committing)
			{
				this.selectionChangedWhileCommitting = true;
				return;
			}
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
			this.pvHasSelection = ASCommitWindow.MarkSelected(this.pv, list);
		}

		private void OnSelectionChange()
		{
			switch (this.selectedPage)
			{
			case ASMainWindow.Page.Overview:
				if (!this.mySelection)
				{
					this.DoSelectionChange();
					base.Repaint();
				}
				else
				{
					this.mySelection = false;
				}
				this.somethingDiscardableSelected = ASCommitWindow.SomethingDiscardableSelected(this.pv);
				break;
			case ASMainWindow.Page.Update:
				this.asUpdateWin.OnSelectionChange();
				break;
			case ASMainWindow.Page.Commit:
				this.asCommitWin.OnSelectionChange();
				break;
			case ASMainWindow.Page.History:
				this.asHistoryWin.OnSelectionChange();
				break;
			}
		}

		internal void Reinit()
		{
			this.SwitchSelectedPage(ASMainWindow.Page.Overview);
			base.Repaint();
		}

		public void DoDiscardChanges(bool lastActionsResult)
		{
			List<string> list = new List<string>();
			bool flag = false;
			if (flag)
			{
				list.AddRange(AssetServer.CollectDeepSelection());
			}
			else
			{
				list.AddRange(AssetServer.GetAllRootGUIDs());
				list.AddRange(AssetServer.CollectAllChildren(AssetServer.GetRootGUID(), AssetServer.GetAllRootGUIDs()));
			}
			if (list.Count == 0)
			{
				list.AddRange(AssetServer.GetAllRootGUIDs());
			}
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitOnSuccess");
			AssetServer.DoUpdateWithoutConflictResolutionOnNextTick(list.ToArray());
		}

		private bool WordWrappedLabelButton(string label, string buttonText)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			bool result = GUILayout.Button(buttonText, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			});
			GUILayout.EndHorizontal();
			return result;
		}

		private bool ToolbarToggle(bool pressed, string title, GUIStyle style)
		{
			bool changed = GUI.changed;
			GUI.changed = false;
			GUILayout.Toggle(pressed, title, style, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				return true;
			}
			GUI.changed |= changed;
			return false;
		}

		private bool RightButton(string title)
		{
			return this.RightButton(title, ASMainWindow.constants.smallButton);
		}

		private bool RightButton(string title, GUIStyle style)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool result = GUILayout.Button(title, style, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			return result;
		}

		public void ShowConflictResolutions(string[] conflicting)
		{
			if (this.asUpdateWin == null)
			{
				this.LogError("Found unexpected conflicts. Please use Bug Reporter to report a bug.");
				return;
			}
			this.asUpdateWin.ShowConflictResolutions(conflicting);
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			if (!this.needsSetup)
			{
				menu.AddItem(new GUIContent("Refresh"), false, new GenericMenu.MenuFunction(this.ActionRefresh));
				menu.AddSeparator(string.Empty);
			}
			menu.AddItem(new GUIContent("Connection"), false, new GenericMenu.MenuFunction2(this.ActionSwitchPage), ASMainWindow.Page.ServerConfig);
			menu.AddSeparator(string.Empty);
			if (!this.needsSetup)
			{
				menu.AddItem(new GUIContent("Show History"), false, new GenericMenu.MenuFunction2(this.ActionSwitchPage), ASMainWindow.Page.History);
				menu.AddItem(new GUIContent("Discard Changes"), false, new GenericMenu.MenuFunction(this.ActionDiscardChanges));
				menu.AddSeparator(string.Empty);
			}
			menu.AddItem(new GUIContent("Server Administration"), false, new GenericMenu.MenuFunction2(this.ActionSwitchPage), ASMainWindow.Page.Admin);
		}

		public bool UpdateNeedsRefresh()
		{
			return this.sharedChangesets == null || AssetServer.GetRefreshUpdate();
		}

		public bool CommitNeedsRefresh()
		{
			return this.sharedCommits == null || this.sharedDeletedItems == null || AssetServer.GetRefreshCommit();
		}

		private void ContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				string text = this.dropDownMenuItems[selected];
				switch (text)
				{
				case "Connection":
					this.ActionSwitchPage(ASMainWindow.Page.ServerConfig);
					break;
				case "Show History":
					this.ActionSwitchPage(ASMainWindow.Page.History);
					break;
				case "Discard Changes":
					this.ActionDiscardChanges();
					break;
				case "Server Administration":
					this.ActionSwitchPage(ASMainWindow.Page.Admin);
					break;
				}
			}
		}

		private void CommitContextMenuClick(object userData, string[] options, int selected)
		{
			if (selected >= 0)
			{
				string text = this.commitDropDownMenuItems[selected];
				switch (text)
				{
				case "Commit":
					this.StartCommitting();
					break;
				case "Compare":
					ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), false);
					break;
				case "Compare Binary":
					ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), true);
					break;
				case "Discard":
					this.DoMyRevert(false);
					break;
				}
			}
		}

		public void CommitItemsChanged()
		{
			this.InitCommits();
			this.DisplayedItemsChanged();
			if (this.selectedPage == ASMainWindow.Page.Commit)
			{
				this.asCommitWin.Update();
			}
			base.Repaint();
		}

		public void RevertProject(int toRevision, Changeset[] changesets)
		{
			AssetServer.SetStickyChangeset(toRevision);
			this.asUpdateWin = new ASUpdateWindow(this, changesets);
			this.asUpdateWin.SetSelectedRevisionLine(0);
			this.asUpdateWin.DoUpdate(false);
			this.selectedPage = ASMainWindow.Page.Update;
		}

		public void ShowHistory()
		{
			this.SwitchSelectedPage(ASMainWindow.Page.Overview);
			this.isInitialUpdate = false;
			this.SwitchSelectedPage(ASMainWindow.Page.History);
		}

		private void ActionRefresh()
		{
			switch (this.selectedPage)
			{
			case ASMainWindow.Page.Overview:
				AssetServer.CheckForServerUpdates();
				this.InitiateRefreshAssetsAndUpdateStatusWithCallback("CBInitOverviewPage");
				break;
			case ASMainWindow.Page.Update:
				AssetServer.CheckForServerUpdates();
				if (this.UpdateNeedsRefresh())
				{
					this.InitiateUpdateStatusWithCallback("CBInitUpdatePage");
				}
				break;
			case ASMainWindow.Page.Commit:
				this.asCommitWin.InitiateReinit();
				break;
			case ASMainWindow.Page.History:
				AssetServer.CheckForServerUpdates();
				if (this.UpdateNeedsRefresh())
				{
					this.InitiateUpdateStatusWithCallback("CBInitHistoryPage");
				}
				break;
			default:
				this.Reinit();
				break;
			}
		}

		private void ActionSwitchPage(object page)
		{
			this.SwitchSelectedPage((ASMainWindow.Page)((int)page));
		}

		private void ActionDiscardChanges()
		{
			if (EditorUtility.DisplayDialog("Discard all changes", "Are you sure you want to discard all local changes made in the project?", "Discard", "Cancel"))
			{
				AssetServer.RemoveMaintErrorsFromConsole();
				if (!ASEditorBackend.SettingsIfNeeded())
				{
					Debug.Log("Asset Server connection for current project is not set up");
					this.error = true;
					return;
				}
				this.error = false;
				AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBDoDiscardChanges");
				AssetServer.DoUpdateStatusOnNextTick();
			}
		}

		private void SwitchSelectedPage(ASMainWindow.Page page)
		{
			ASMainWindow.Page page2 = this.selectedPage;
			this.selectedPage = page;
			this.SelectedPageChanged();
			if (this.error)
			{
				this.selectedPage = page2;
				this.error = false;
			}
		}

		private void InitiateUpdateStatusWithCallback(string callbackName)
		{
			if (!ASEditorBackend.SettingsIfNeeded())
			{
				Debug.Log("Asset Server connection for current project is not set up");
				this.error = true;
				return;
			}
			this.error = false;
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", callbackName);
			AssetServer.DoUpdateStatusOnNextTick();
		}

		private void InitiateRefreshAssetsWithCallback(string callbackName)
		{
			if (!ASEditorBackend.SettingsIfNeeded())
			{
				Debug.Log("Asset Server connection for current project is not set up");
				this.error = true;
				return;
			}
			this.error = false;
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", callbackName);
			AssetServer.DoRefreshAssetsOnNextTick();
		}

		private void InitiateRefreshAssetsAndUpdateStatusWithCallback(string callbackName)
		{
			if (!ASEditorBackend.SettingsIfNeeded())
			{
				Debug.Log("Asset Server connection for current project is not set up");
				this.error = true;
				return;
			}
			this.error = false;
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", callbackName);
			AssetServer.DoRefreshAssetsAndUpdateStatusOnNextTick();
		}

		private void SelectedPageChanged()
		{
			AssetServer.ClearAssetServerError();
			if (this.committing)
			{
				this.CancelCommit();
			}
			switch (this.selectedPage)
			{
			case ASMainWindow.Page.Overview:
				if (ASEditorBackend.SettingsAreValid())
				{
					AssetServer.CheckForServerUpdates();
					if (this.UpdateNeedsRefresh())
					{
						this.InitiateUpdateStatusWithCallback("CBInitOverviewPage");
					}
					else
					{
						this.InitOverviewPage(true);
					}
				}
				else
				{
					this.connectionString = "Asset Server connection for current project is not set up";
					this.sharedChangesets = new Changeset[0];
					this.changesetContents = new GUIContent[0];
					this.needsSetup = true;
				}
				break;
			case ASMainWindow.Page.Update:
				this.InitUpdatePage(true);
				break;
			case ASMainWindow.Page.Commit:
				this.asCommitWin = new ASCommitWindow(this, (!this.pvHasSelection) ? null : ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false).ToArray());
				this.asCommitWin.InitiateReinit();
				break;
			case ASMainWindow.Page.History:
				this.pageTitles[3] = "History";
				this.InitHistoryPage(true);
				break;
			case ASMainWindow.Page.ServerConfig:
				this.pageTitles[3] = "Connection";
				this.asConfigWin = new ASConfigWindow(this);
				break;
			case ASMainWindow.Page.Admin:
				this.pageTitles[3] = "Administration";
				this.asAdminWin = new ASServerAdminWindow(this);
				if (this.error)
				{
					return;
				}
				break;
			}
		}

		public void InitUpdatePage(bool lastActionsResult)
		{
			if (!lastActionsResult)
			{
				this.Reinit();
				return;
			}
			if (this.UpdateNeedsRefresh())
			{
				this.GetUpdates();
			}
			if (this.sharedChangesets == null)
			{
				this.Reinit();
				return;
			}
			this.asUpdateWin = new ASUpdateWindow(this, this.sharedChangesets);
			this.asUpdateWin.SetSelectedRevisionLine(0);
		}

		private void InitCommits()
		{
			if (this.CommitNeedsRefresh())
			{
				if (AssetServer.GetAssetServerError() == string.Empty)
				{
					this.sharedCommits = ASCommitWindow.GetCommits();
					this.sharedDeletedItems = AssetServer.GetLocalDeletedItems();
				}
				else
				{
					this.sharedCommits = new AssetsItem[0];
					this.sharedDeletedItems = new AssetsItem[0];
				}
			}
			this.pv.Clear();
			this.pv.AddAssetItems(this.sharedCommits);
			this.pv.AddAssetItems(this.sharedDeletedItems);
			this.pv.SetLineCount();
			this.pv.selectedItems = new bool[this.pv.lv.totalRows];
			this.pv.initialSelectedItem = -1;
			AssetServer.ClearRefreshCommit();
		}

		private void GetUpdates()
		{
			AssetServer.ClearAssetServerError();
			this.sharedChangesets = AssetServer.GetNewItems();
			Array.Reverse(this.sharedChangesets);
			this.changesetContents = null;
			this.maxNickLength = 1;
			AssetServer.ClearRefreshUpdate();
			if (AssetServer.GetAssetServerError() != string.Empty)
			{
				this.sharedChangesets = null;
			}
		}

		public void DisplayedItemsChanged()
		{
			float[] array = new float[2];
			bool flag = this.sharedChangesets != null && this.sharedChangesets.Length != 0;
			bool flag2 = this.pv.lv.totalRows != 0;
			if ((flag && flag2) || (!flag && !flag2))
			{
				array[0] = (array[1] = 0.5f);
			}
			else
			{
				array[0] = (float)((!flag) ? 0 : 1);
				array[1] = (float)((!flag2) ? 0 : 1);
			}
			this.splitter = new SplitterState(array, new int[]
			{
				80,
				80
			}, null);
			this.DoSelectionChange();
		}

		public void InitOverviewPage(bool lastActionsResult)
		{
			if (!lastActionsResult)
			{
				this.needsSetup = true;
				this.sharedChangesets = null;
				this.sharedCommits = null;
				this.sharedDeletedItems = null;
				return;
			}
			PListConfig pListConfig = new PListConfig("Library/ServerPreferences.plist");
			this.connectionString = string.Concat(new string[]
			{
				pListConfig["Maint UserName"],
				" @ ",
				pListConfig["Maint Server"],
				" : ",
				pListConfig["Maint project name"]
			});
			if (this.UpdateNeedsRefresh())
			{
				this.GetUpdates();
			}
			this.needsSetup = (this.sharedChangesets == null || AssetServer.HasConnectionError());
			this.InitCommits();
			this.DisplayedItemsChanged();
		}

		public void InitHistoryPage(bool lastActionsResult)
		{
			if (!lastActionsResult)
			{
				this.Reinit();
				return;
			}
			this.asHistoryWin = new ASHistoryWindow(this);
			if (this.asHistoryWin == null)
			{
				this.Reinit();
				return;
			}
		}

		private void OverviewPageGUI()
		{
			bool enabled = GUI.enabled;
			this.showSmallWindow = (base.position.width <= (float)this.widthToHideButtons);
			if (Event.current.type == EventType.Layout)
			{
				this.wasHidingButtons = this.showSmallWindow;
			}
			else if (this.showSmallWindow != this.wasHidingButtons)
			{
				GUIUtility.ExitGUI();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (!this.showSmallWindow)
			{
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				this.ShortServerInfo();
				if (this.needsSetup)
				{
					GUI.enabled = false;
				}
				this.OtherServerCommands();
				GUI.enabled = enabled;
				this.ServerAdministration();
				GUI.enabled = (!this.needsSetup && enabled);
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Width((base.position.width - 30f) / 2f)
				});
			}
			else
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			}
			GUI.enabled = (!this.needsSetup && enabled);
			SplitterGUILayout.BeginVerticalSplit(this.splitter, new GUILayoutOption[0]);
			this.ShortUpdateList();
			this.ShortCommitList();
			SplitterGUILayout.EndVerticalSplit();
			GUILayout.EndHorizontal();
			GUILayout.EndHorizontal();
			GUI.enabled = enabled;
		}

		private void OtherServerCommands()
		{
			GUILayout.BeginVertical(ASMainWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Label("Asset Server Actions", ASMainWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASMainWindow.constants.contentBox, new GUILayoutOption[0]);
			if (this.WordWrappedLabelButton("Browse the complete history of the project", "Show History"))
			{
				this.SwitchSelectedPage(ASMainWindow.Page.History);
				GUIUtility.ExitGUI();
			}
			GUILayout.Space(5f);
			if (this.WordWrappedLabelButton("Discard all local changes you made to the project", "Discard Changes"))
			{
				this.ActionDiscardChanges();
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private void ShortServerInfo()
		{
			GUILayout.BeginVertical(ASMainWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Label("Current Project", ASMainWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASMainWindow.constants.contentBox, new GUILayoutOption[0]);
			if (this.WordWrappedLabelButton(this.connectionString, "Connection"))
			{
				this.SwitchSelectedPage(ASMainWindow.Page.ServerConfig);
			}
			if (AssetServer.GetAssetServerError() != string.Empty)
			{
				GUILayout.Space(10f);
				GUILayout.Label(AssetServer.GetAssetServerError(), ASMainWindow.constants.errorLabel, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private void ServerAdministration()
		{
			GUILayout.BeginVertical(ASMainWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Label("Asset Server Administration", ASMainWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASMainWindow.constants.contentBox, new GUILayoutOption[0]);
			if (this.WordWrappedLabelButton("Create and administer Asset Server projects", "Administration"))
			{
				this.SwitchSelectedPage(ASMainWindow.Page.Admin);
				GUIUtility.ExitGUI();
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
		}

		private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
		{
			return (flagToCheck & flags) != ChangeFlags.None;
		}

		private void MySelectionToGlobalSelection()
		{
			this.mySelection = true;
			List<string> parentViewSelectedItems = ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false);
			parentViewSelectedItems.Remove(AssetServer.GetRootGUID());
			if (parentViewSelectedItems.Count > 0)
			{
				AssetServer.SetSelectionFromGUID(parentViewSelectedItems[0]);
			}
			this.pvHasSelection = this.pv.HasTrue();
			this.somethingDiscardableSelected = ASCommitWindow.SomethingDiscardableSelected(this.pv);
		}

		private void DoCommitParentView()
		{
			bool shift = Event.current.shift;
			bool actionKey = EditorGUI.actionKey;
			int row = this.pv.lv.row;
			int num = -1;
			int num2 = -1;
			bool flag = false;
			foreach (ListViewElement listViewElement in ListViewGUILayout.ListView(this.pv.lv, ASMainWindow.constants.background, new GUILayoutOption[0]))
			{
				if (GUIUtility.keyboardControl == this.pv.lv.ID && Event.current.type == EventType.KeyDown && actionKey)
				{
					Event.current.Use();
				}
				if (num == -1 && !this.pv.IndexToFolderAndFile(listViewElement.row, ref num, ref num2))
				{
					break;
				}
				ParentViewFolder parentViewFolder = this.pv.folders[num];
				if (this.pv.selectedItems[listViewElement.row] && Event.current.type == EventType.Repaint)
				{
					ASMainWindow.constants.entrySelected.Draw(listViewElement.position, false, false, false, false);
				}
				if (!this.committing)
				{
					if (ListViewGUILayout.HasMouseUp(listViewElement.position))
					{
						if (!shift && !actionKey)
						{
							flag |= ListViewGUILayout.MultiSelection(row, this.pv.lv.row, ref this.pv.initialSelectedItem, ref this.pv.selectedItems);
						}
					}
					else if (ListViewGUILayout.HasMouseDown(listViewElement.position))
					{
						if (Event.current.clickCount == 2)
						{
							ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), false);
							GUIUtility.ExitGUI();
						}
						else
						{
							if (!this.pv.selectedItems[listViewElement.row] || shift || actionKey)
							{
								flag |= ListViewGUILayout.MultiSelection(row, listViewElement.row, ref this.pv.initialSelectedItem, ref this.pv.selectedItems);
							}
							this.pv.selectedFile = num2;
							this.pv.selectedFolder = num;
							this.pv.lv.row = listViewElement.row;
						}
					}
					else if (ListViewGUILayout.HasMouseDown(listViewElement.position, 1))
					{
						if (!this.pv.selectedItems[listViewElement.row])
						{
							flag = true;
							this.pv.ClearSelection();
							this.pv.selectedItems[listViewElement.row] = true;
							this.pv.selectedFile = num2;
							this.pv.selectedFolder = num;
							this.pv.lv.row = listViewElement.row;
						}
						GUIUtility.hotControl = 0;
						Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
						EditorUtility.DisplayCustomMenu(position, this.commitDropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.CommitContextMenuClick), null);
						Event.current.Use();
					}
				}
				ChangeFlags changeFlags;
				if (num2 != -1)
				{
					Texture2D texture2D = AssetDatabase.GetCachedIcon(parentViewFolder.name + "/" + parentViewFolder.files[num2].name) as Texture2D;
					if (texture2D == null)
					{
						texture2D = InternalEditorUtility.GetIconForFile(parentViewFolder.files[num2].name);
					}
					GUILayout.Label(new GUIContent(parentViewFolder.files[num2].name, texture2D), ASMainWindow.constants.element, new GUILayoutOption[0]);
					changeFlags = parentViewFolder.files[num2].changeFlags;
				}
				else
				{
					GUILayout.Label(parentViewFolder.name, ASMainWindow.constants.header, new GUILayoutOption[0]);
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
					Rect position2 = new Rect(listViewElement.position.x + listViewElement.position.width - (float)gUIContent.image.width - 5f, listViewElement.position.y + listViewElement.position.height / 2f - (float)(gUIContent.image.height / 2), (float)gUIContent.image.width, (float)gUIContent.image.height);
					EditorGUIUtility.SetIconSize(Vector2.zero);
					GUIStyle.none.Draw(position2, gUIContent, false, false, false, false);
					EditorGUIUtility.SetIconSize(this.iconSize);
				}
				this.pv.NextFileFolder(ref num, ref num2);
			}
			if (!this.committing)
			{
				if (GUIUtility.keyboardControl == this.pv.lv.ID)
				{
					if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll")
					{
						Event.current.Use();
					}
					else if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "SelectAll")
					{
						for (int i = 0; i < this.pv.selectedItems.Length; i++)
						{
							this.pv.selectedItems[i] = true;
						}
						flag = true;
						Event.current.Use();
					}
				}
				if (GUIUtility.keyboardControl == this.pv.lv.ID && !actionKey && this.pv.lv.selectionChanged)
				{
					flag |= ListViewGUILayout.MultiSelection(row, this.pv.lv.row, ref this.pv.initialSelectedItem, ref this.pv.selectedItems);
					this.pv.IndexToFolderAndFile(this.pv.lv.row, ref this.pv.selectedFolder, ref this.pv.selectedFile);
				}
				if (this.pv.lv.selectionChanged || flag)
				{
					this.MySelectionToGlobalSelection();
				}
			}
		}

		private void DoCommit()
		{
			if (this.commitMessage == string.Empty && !EditorUtility.DisplayDialog("Commit without description", "Are you sure you want to commit with empty commit description message?", "Commit", "Cancel"))
			{
				GUIUtility.ExitGUI();
			}
			bool refreshCommit = AssetServer.GetRefreshCommit();
			ASCommitWindow aSCommitWindow = new ASCommitWindow(this, ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false).ToArray());
			aSCommitWindow.InitiateReinit();
			if ((refreshCommit || aSCommitWindow.lastTransferMovedDependencies) && ((!refreshCommit && !EditorUtility.DisplayDialog("Committing with dependencies", "Assets selected for committing have dependencies that will also be committed. Press Details to view full changeset", "Commit", "Details")) || refreshCommit))
			{
				this.committing = false;
				this.selectedPage = ASMainWindow.Page.Commit;
				aSCommitWindow.description = this.commitMessage;
				if (refreshCommit)
				{
					aSCommitWindow.showReinitedWarning = 1;
				}
				this.asCommitWin = aSCommitWindow;
				base.Repaint();
				GUIUtility.ExitGUI();
				return;
			}
			string[] itemsToCommit = aSCommitWindow.GetItemsToCommit();
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBOverviewsCommitFinished");
			AssetServer.DoCommitOnNextTick(this.commitMessage, itemsToCommit);
			AssetServer.SetLastCommitMessage(this.commitMessage);
			aSCommitWindow.AddToCommitMessageHistory(this.commitMessage);
			this.committing = false;
			GUIUtility.ExitGUI();
		}

		private void StartCommitting()
		{
			this.committing = true;
			this.commitMessage = string.Empty;
			this.selectionChangedWhileCommitting = false;
			this.focusCommitMessage = true;
		}

		internal void CommitFinished(bool actionResult)
		{
			if (actionResult)
			{
				AssetServer.ClearCommitPersistentData();
				this.InitOverviewPage(true);
			}
			else
			{
				base.Repaint();
			}
		}

		private void CancelCommit()
		{
			this.committing = false;
			if (this.selectionChangedWhileCommitting)
			{
				this.DoSelectionChange();
			}
		}

		private void DoMyRevert(bool afterMarkingDependencies)
		{
			if (!afterMarkingDependencies)
			{
				List<string> parentViewSelectedItems = ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false);
				if (ASCommitWindow.MarkAllFolderDependenciesForDiscarding(this.pv, null))
				{
					this.lastRevertSelectionChanged = 2;
					this.MySelectionToGlobalSelection();
				}
				else
				{
					this.lastRevertSelectionChanged = -1;
				}
				List<string> parentViewSelectedItems2 = ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false);
				if (parentViewSelectedItems.Count != parentViewSelectedItems2.Count)
				{
					this.lastRevertSelectionChanged = 2;
				}
			}
			if (afterMarkingDependencies || this.lastRevertSelectionChanged == -1)
			{
				ASCommitWindow.DoRevert(ASCommitWindow.GetParentViewSelectedItems(this.pv, true, true), "CBInitOverviewPage");
			}
		}

		private void ShortCommitList()
		{
			bool enabled = GUI.enabled;
			GUILayout.BeginVertical((!this.showSmallWindow) ? ASMainWindow.constants.groupBox : ASMainWindow.constants.groupBoxNoMargin, new GUILayoutOption[0]);
			GUILayout.Label("Local Changes", ASMainWindow.constants.title, new GUILayoutOption[0]);
			if (this.pv.lv.totalRows == 0)
			{
				GUILayout.BeginVertical(ASMainWindow.constants.contentBox, new GUILayoutOption[]
				{
					GUILayout.ExpandHeight(true)
				});
				GUILayout.Label("Nothing to commit", new GUILayoutOption[0]);
				GUILayout.EndVertical();
			}
			else
			{
				this.DoCommitParentView();
				GUILayout.BeginHorizontal(ASMainWindow.constants.contentBox, new GUILayoutOption[0]);
				Event current = Event.current;
				if (!this.committing)
				{
					GUI.enabled = (this.pvHasSelection && enabled);
					if (GUILayout.Button("Compare", ASMainWindow.constants.smallButton, new GUILayoutOption[0]))
					{
						ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), false);
						GUIUtility.ExitGUI();
					}
					bool enabled2 = GUI.enabled;
					if (!this.somethingDiscardableSelected)
					{
						GUI.enabled = false;
					}
					if (GUILayout.Button("Discard", ASMainWindow.constants.smallButton, new GUILayoutOption[0]))
					{
						this.DoMyRevert(false);
						GUIUtility.ExitGUI();
					}
					GUI.enabled = enabled2;
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Commit...", ASMainWindow.constants.smallButton, new GUILayoutOption[0]) || (this.pvHasSelection && current.type == EventType.KeyDown && current.keyCode == KeyCode.Return))
					{
						this.StartCommitting();
						current.Use();
					}
					if (current.type == EventType.KeyDown && (current.character == '\n' || current.character == '\u0003'))
					{
						current.Use();
					}
					GUI.enabled = enabled;
					if (GUILayout.Button("Details", ASMainWindow.constants.smallButton, new GUILayoutOption[0]))
					{
						this.SwitchSelectedPage(ASMainWindow.Page.Commit);
						base.Repaint();
						GUIUtility.ExitGUI();
					}
				}
				else
				{
					if (current.type == EventType.KeyDown)
					{
						KeyCode keyCode = current.keyCode;
						if (keyCode != KeyCode.Return)
						{
							if (keyCode != KeyCode.Escape)
							{
								if (current.character == '\n' || current.character == '\u0003')
								{
									current.Use();
								}
							}
							else
							{
								this.CancelCommit();
								current.Use();
							}
						}
						else
						{
							this.DoCommit();
							current.Use();
						}
					}
					GUI.SetNextControlName("commitMessage");
					this.commitMessage = EditorGUILayout.TextField(this.commitMessage, new GUILayoutOption[0]);
					if (GUILayout.Button("Commit", ASMainWindow.constants.smallButton, new GUILayoutOption[]
					{
						GUILayout.Width(60f)
					}))
					{
						this.DoCommit();
					}
					if (GUILayout.Button("Cancel", ASMainWindow.constants.smallButton, new GUILayoutOption[]
					{
						GUILayout.Width(60f)
					}))
					{
						this.CancelCommit();
					}
					if (this.focusCommitMessage)
					{
						EditorGUI.FocusTextInControl("commitMessage");
						this.focusCommitMessage = false;
						base.Repaint();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
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
				this.lastRevertSelectionChanged--;
				base.Repaint();
			}
		}

		private void ShortUpdateList()
		{
			GUILayout.BeginVertical((!this.showSmallWindow) ? ASMainWindow.constants.groupBox : ASMainWindow.constants.groupBoxNoMargin, new GUILayoutOption[0]);
			GUILayout.Label("Updates on Server", ASMainWindow.constants.title, new GUILayoutOption[0]);
			if (this.sharedChangesets == null)
			{
				GUILayout.BeginVertical(ASMainWindow.constants.contentBox, new GUILayoutOption[]
				{
					GUILayout.ExpandHeight(true)
				});
				GUILayout.Label("Could not retrieve changes", new GUILayoutOption[0]);
				GUILayout.EndVertical();
			}
			else if (this.sharedChangesets.Length == 0)
			{
				GUILayout.BeginVertical(ASMainWindow.constants.contentBox, new GUILayoutOption[]
				{
					GUILayout.ExpandHeight(true)
				});
				GUILayout.Label("You are up to date", new GUILayoutOption[0]);
				GUILayout.EndVertical();
			}
			else
			{
				this.lv.totalRows = this.sharedChangesets.Length;
				int num = (int)ASMainWindow.constants.entryNormal.CalcHeight(new GUIContent("X"), 100f);
				ASMainWindow.constants.serverUpdateLog.alignment = TextAnchor.MiddleLeft;
				ASMainWindow.constants.serverUpdateInfo.alignment = TextAnchor.MiddleLeft;
				foreach (ListViewElement listViewElement in ListViewGUILayout.ListView(this.lv, ASMainWindow.constants.background, new GUILayoutOption[0]))
				{
					Rect rect = GUILayoutUtility.GetRect(GUIClip.visibleRect.width, (float)num, new GUILayoutOption[]
					{
						GUILayout.MinHeight((float)num)
					});
					Rect position = rect;
					position.x += 1f;
					position.y += 1f;
					if (listViewElement.row % 2 == 0)
					{
						if (Event.current.type == EventType.Repaint)
						{
							ASMainWindow.constants.entryEven.Draw(position, false, false, false, false);
						}
						position.y += rect.height;
						if (Event.current.type == EventType.Repaint)
						{
							ASMainWindow.constants.entryOdd.Draw(position, false, false, false, false);
						}
					}
					position = rect;
					position.width -= (float)(this.maxNickLength + 25);
					position.x += 10f;
					GUI.Button(position, this.changesetContents[listViewElement.row], ASMainWindow.constants.serverUpdateLog);
					position = rect;
					position.x += position.width - (float)this.maxNickLength - 5f;
					GUI.Label(position, this.sharedChangesets[listViewElement.row].owner, ASMainWindow.constants.serverUpdateInfo);
				}
				ASMainWindow.constants.serverUpdateLog.alignment = TextAnchor.UpperLeft;
				ASMainWindow.constants.serverUpdateInfo.alignment = TextAnchor.UpperLeft;
				GUILayout.BeginHorizontal(ASMainWindow.constants.contentBox, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Update", ASMainWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.selectedPage = ASMainWindow.Page.Update;
					this.InitUpdatePage(true);
					this.asUpdateWin.DoUpdate(false);
				}
				if (GUILayout.Button("Details", ASMainWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.SwitchSelectedPage(ASMainWindow.Page.Update);
					base.Repaint();
					GUIUtility.ExitGUI();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}

		private void DoSelectedPageGUI()
		{
			switch (this.selectedPage)
			{
			case ASMainWindow.Page.Overview:
				this.OverviewPageGUI();
				break;
			case ASMainWindow.Page.Update:
				if (this.asUpdateWin != null && this.asUpdateWin != null)
				{
					this.asUpdateWin.DoGUI();
				}
				break;
			case ASMainWindow.Page.Commit:
				if (this.asCommitWin != null && this.asCommitWin != null)
				{
					this.asCommitWin.DoGUI();
				}
				break;
			case ASMainWindow.Page.History:
				if (this.asHistoryWin != null && !this.asHistoryWin.DoGUI(this.m_Parent.hasFocus))
				{
					this.SwitchSelectedPage(ASMainWindow.Page.Overview);
					GUIUtility.ExitGUI();
				}
				break;
			case ASMainWindow.Page.ServerConfig:
				if (this.asConfigWin != null && !this.asConfigWin.DoGUI())
				{
					this.SwitchSelectedPage(ASMainWindow.Page.Overview);
					GUIUtility.ExitGUI();
				}
				break;
			case ASMainWindow.Page.Admin:
				if (this.asAdminWin != null && !this.asAdminWin.DoGUI())
				{
					this.SwitchSelectedPage(ASMainWindow.Page.Overview);
					GUIUtility.ExitGUI();
				}
				break;
			}
		}

		private void SetShownSearchField(ASMainWindow.ShowSearchField newShow)
		{
			EditorGUI.FocusTextInControl("SearchFilter");
			this.m_SearchField.Show = false;
			this.m_ShowSearch = newShow;
			this.m_SearchField.Show = true;
			this.asHistoryWin.FilterItems(false);
		}

		private void DoSearchToggle(ASMainWindow.ShowSearchField field)
		{
			if (this.selectedPage == ASMainWindow.Page.History)
			{
				if (this.m_SearchField.DoGUI())
				{
					this.asHistoryWin.FilterItems(false);
				}
				GUILayout.Space(10f);
			}
		}

		private bool IsLastOne(int f, int fl, ParentViewState st)
		{
			return st.folders.Length - 1 == f && st.folders[f].files.Length - 1 == fl;
		}

		private void OnGUI()
		{
			if (EditorSettings.externalVersionControl != ExternalVersionControl.Disabled && EditorSettings.externalVersionControl != ExternalVersionControl.AssetServer)
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label("Asset Server is disabled when external version control is used. Go to 'Edit -> Project Settings -> Editor' to re-enable it.", new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				return;
			}
			if (ASMainWindow.constants == null)
			{
				ASMainWindow.constants = new ASMainWindow.Constants();
			}
			if (!this.m_CheckedMaint && Event.current.type != EventType.Layout)
			{
				if (!InternalEditorUtility.HasTeamLicense())
				{
					base.Close();
					GUIUtility.ExitGUI();
				}
				this.m_CheckedMaint = true;
			}
			if (this.maxNickLength == 1 && this.sharedChangesets != null)
			{
				for (int i = 0; i < this.sharedChangesets.Length; i++)
				{
					int num = (int)ASMainWindow.constants.serverUpdateInfo.CalcSize(new GUIContent(this.sharedChangesets[i].owner)).x;
					if (num > this.maxNickLength)
					{
						this.maxNickLength = num;
					}
				}
				this.changesetContents = new GUIContent[this.sharedChangesets.Length];
				ParentViewState parentViewState = new ParentViewState();
				for (int j = 0; j < this.changesetContents.Length; j++)
				{
					int num2 = 15;
					Changeset changeset = this.sharedChangesets[j];
					string text = changeset.message.Split(new char[]
					{
						'\n'
					})[0];
					text = ((text.Length >= 45) ? (text.Substring(0, 42) + "...") : text);
					string text2 = string.Format("[{0} {1}] {2}", changeset.date, changeset.owner, text);
					num2--;
					parentViewState.Clear();
					parentViewState.AddAssetItems(changeset);
					for (int k = 0; k < parentViewState.folders.Length; k++)
					{
						if (--num2 == 0 && !this.IsLastOne(k, 0, parentViewState))
						{
							text2 += "\n(and more...)";
							break;
						}
						text2 = text2 + "\n" + parentViewState.folders[k].name;
						for (int l = 0; l < parentViewState.folders[k].files.Length; l++)
						{
							if (--num2 == 0 && !this.IsLastOne(k, l, parentViewState))
							{
								text2 += "\n(and more...)";
								break;
							}
							text2 = text2 + "\n\t" + parentViewState.folders[k].files[l].name;
						}
						if (num2 == 0)
						{
							break;
						}
					}
					this.changesetContents[j] = new GUIContent(this.sharedChangesets[j].message.Split(new char[]
					{
						'\n'
					})[0], text2);
				}
				if (this.maxNickLength == 1)
				{
					this.maxNickLength = 0;
				}
			}
			if (AssetServer.IsControllerBusy() != 0)
			{
				base.Repaint();
				return;
			}
			if (this.isInitialUpdate)
			{
				this.isInitialUpdate = false;
				this.SwitchSelectedPage(ASMainWindow.Page.Overview);
			}
			if (Event.current.type == EventType.ExecuteCommand && Event.current.commandName == "Find")
			{
				this.SetShownSearchField(this.m_SearchToShow);
				Event.current.Use();
			}
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			int num3 = -1;
			bool enabled = GUI.enabled;
			if (this.ToolbarToggle(this.selectedPage == ASMainWindow.Page.Overview, this.pageTitles[0], EditorStyles.toolbarButton))
			{
				num3 = 0;
			}
			GUI.enabled = (!this.needsSetup && this.sharedChangesets != null && this.sharedChangesets.Length != 0 && enabled);
			if (this.ToolbarToggle(this.selectedPage == ASMainWindow.Page.Update, this.pageTitles[1], EditorStyles.toolbarButton))
			{
				num3 = 1;
			}
			GUI.enabled = (!this.needsSetup && this.pv.lv.totalRows != 0 && enabled);
			if (this.selectedPage > ASMainWindow.Page.Commit)
			{
				if (this.ToolbarToggle(this.selectedPage == ASMainWindow.Page.Commit, this.pageTitles[2], EditorStyles.toolbarButton))
				{
					num3 = 2;
				}
				GUI.enabled = enabled;
				if (this.ToolbarToggle(this.selectedPage > ASMainWindow.Page.Commit, this.pageTitles[3], EditorStyles.toolbarButton))
				{
					num3 = 3;
				}
			}
			else
			{
				if (this.ToolbarToggle(this.selectedPage == ASMainWindow.Page.Commit, this.pageTitles[2], EditorStyles.toolbarButton))
				{
					num3 = 2;
				}
				GUI.enabled = enabled;
			}
			if (num3 != -1 && num3 != (int)this.selectedPage)
			{
				if (this.selectedPage == ASMainWindow.Page.Commit)
				{
					this.NotifyClosingCommit();
				}
				if (num3 <= 2)
				{
					this.SwitchSelectedPage((ASMainWindow.Page)num3);
					GUIUtility.ExitGUI();
				}
			}
			GUILayout.FlexibleSpace();
			if (this.selectedPage == ASMainWindow.Page.History)
			{
				this.DoSearchToggle(ASMainWindow.ShowSearchField.HistoryList);
			}
			if (!this.needsSetup)
			{
				switch (this.selectedPage)
				{
				case ASMainWindow.Page.Overview:
				case ASMainWindow.Page.Update:
				case ASMainWindow.Page.History:
					if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						this.ActionRefresh();
						GUIUtility.ExitGUI();
					}
					break;
				}
			}
			GUILayout.EndHorizontal();
			EditorGUIUtility.SetIconSize(this.iconSize);
			this.DoSelectedPageGUI();
			EditorGUIUtility.SetIconSize(Vector2.zero);
			if (Event.current.type == EventType.ContextClick)
			{
				GUIUtility.hotControl = 0;
				Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
				EditorUtility.DisplayCustomMenu(position, (!this.needsSetup) ? this.dropDownMenuItems : this.unconfiguredDropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
				Event.current.Use();
			}
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
		}
	}
}
