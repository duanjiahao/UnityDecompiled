using System;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	[EditorWindowTitle(title = "Version Control", icon = "UnityEditor.VersionControl")]
	internal class WindowPending : EditorWindow
	{
		internal class Styles
		{
			public GUIStyle box = "CN Box";

			public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
		}

		private static WindowPending.Styles s_Styles = null;

		private static Texture2D changeIcon = null;

		private Texture2D syncIcon = null;

		private Texture2D refreshIcon = null;

		private GUIStyle header;

		[SerializeField]
		private ListControl pendingList;

		[SerializeField]
		private ListControl incomingList;

		private bool m_ShowIncoming = false;

		private const float k_ResizerHeight = 17f;

		private const float k_MinIncomingAreaHeight = 50f;

		private const float k_BottomBarHeight = 17f;

		private float s_ToolbarButtonsWidth = 0f;

		private float s_SettingsButtonWidth = 0f;

		private float s_DeleteChangesetsButtonWidth = 0f;

		private static GUIContent[] sStatusWheel;

		private static bool s_DidReload = false;

		internal static GUIContent StatusWheel
		{
			get
			{
				if (WindowPending.sStatusWheel == null)
				{
					WindowPending.sStatusWheel = new GUIContent[12];
					for (int i = 0; i < 12; i++)
					{
						GUIContent gUIContent = new GUIContent();
						gUIContent.image = EditorGUIUtility.LoadIcon("WaitSpin" + i.ToString("00"));
						gUIContent.image.hideFlags = HideFlags.HideAndDontSave;
						gUIContent.image.name = "Spinner";
						WindowPending.sStatusWheel[i] = gUIContent;
					}
				}
				int num = (int)Mathf.Repeat(Time.realtimeSinceStartup * 10f, 11.99f);
				return WindowPending.sStatusWheel[num];
			}
		}

		private void InitStyles()
		{
			if (WindowPending.s_Styles == null)
			{
				WindowPending.s_Styles = new WindowPending.Styles();
			}
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			if (this.pendingList == null)
			{
				this.pendingList = new ListControl();
			}
			ListControl expr_29 = this.pendingList;
			expr_29.ExpandEvent = (ListControl.ExpandDelegate)Delegate.Combine(expr_29.ExpandEvent, new ListControl.ExpandDelegate(this.OnExpand));
			ListControl expr_50 = this.pendingList;
			expr_50.DragEvent = (ListControl.DragDelegate)Delegate.Combine(expr_50.DragEvent, new ListControl.DragDelegate(this.OnDrop));
			this.pendingList.MenuDefault = "CONTEXT/Pending";
			this.pendingList.MenuFolder = "CONTEXT/Change";
			this.pendingList.DragAcceptOnly = true;
			if (this.incomingList == null)
			{
				this.incomingList = new ListControl();
			}
			ListControl expr_B9 = this.incomingList;
			expr_B9.ExpandEvent = (ListControl.ExpandDelegate)Delegate.Combine(expr_B9.ExpandEvent, new ListControl.ExpandDelegate(this.OnExpandIncoming));
			this.UpdateWindow();
		}

		public void OnSelectionChange()
		{
			if (!base.hasFocus)
			{
				this.pendingList.Sync();
				base.Repaint();
			}
		}

		private void OnDrop(ChangeSet targetItem)
		{
			AssetList selectedAssets = this.pendingList.SelectedAssets;
			Task task = Provider.ChangeSetMove(selectedAssets, targetItem);
			task.SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		public static void ExpandLatestChangeSet()
		{
			WindowPending[] array = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
			WindowPending[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				WindowPending windowPending = array2[i];
				windowPending.pendingList.ExpandLastItem();
			}
		}

		private void OnExpand(ChangeSet change, ListItem item)
		{
			if (Provider.isActive)
			{
				Task task = Provider.ChangeSetStatus(change);
				task.userIdentifier = item.Identifier;
				task.SetCompletionAction(CompletionAction.OnChangeContentsPendingWindow);
				if (!item.HasChildren)
				{
					Asset asset = new Asset("Updating...");
					ListItem listItem = this.pendingList.Add(item, asset.prettyPath, asset);
					listItem.Dummy = true;
					this.pendingList.Refresh(false);
					base.Repaint();
				}
			}
		}

		private void OnExpandIncoming(ChangeSet change, ListItem item)
		{
			if (Provider.isActive)
			{
				Task task = Provider.IncomingChangeSetAssets(change);
				task.userIdentifier = item.Identifier;
				task.SetCompletionAction(CompletionAction.OnChangeContentsPendingWindow);
				if (!item.HasChildren)
				{
					Asset asset = new Asset("Updating...");
					ListItem listItem = this.incomingList.Add(item, asset.prettyPath, asset);
					listItem.Dummy = true;
					this.incomingList.Refresh(false);
					base.Repaint();
				}
			}
		}

		private void UpdateWindow()
		{
			if (!Provider.isActive)
			{
				this.pendingList.Clear();
				Provider.UpdateSettings();
				base.Repaint();
			}
			else if (Provider.onlineState == OnlineState.Online)
			{
				Task task = Provider.ChangeSets();
				task.SetCompletionAction(CompletionAction.OnChangeSetsPendingWindow);
				Task task2 = Provider.Incoming();
				task2.SetCompletionAction(CompletionAction.OnIncomingPendingWindow);
			}
		}

		private void OnGotLatest(Task t)
		{
			this.UpdateWindow();
		}

		private static void OnVCTaskCompletedEvent(Task task, CompletionAction completionAction)
		{
			WindowPending[] array = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
			WindowPending[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				WindowPending windowPending = array2[i];
				switch (completionAction)
				{
				case CompletionAction.UpdatePendingWindow:
				case CompletionAction.OnCheckoutCompleted:
					windowPending.UpdateWindow();
					break;
				case CompletionAction.OnChangeContentsPendingWindow:
					windowPending.OnChangeContents(task);
					break;
				case CompletionAction.OnIncomingPendingWindow:
					windowPending.OnIncoming(task);
					break;
				case CompletionAction.OnChangeSetsPendingWindow:
					windowPending.OnChangeSets(task);
					break;
				case CompletionAction.OnGotLatestPendingWindow:
					windowPending.OnGotLatest(task);
					break;
				}
			}
			if (completionAction != CompletionAction.OnSubmittedChangeWindow)
			{
				if (completionAction != CompletionAction.OnAddedChangeWindow)
				{
					if (completionAction == CompletionAction.OnCheckoutCompleted)
					{
						if (EditorUserSettings.showFailedCheckout)
						{
							WindowCheckoutFailure.OpenIfCheckoutFailed(task.assetList);
						}
					}
				}
				else
				{
					WindowChange.OnAdded(task);
				}
			}
			else
			{
				WindowChange.OnSubmitted(task);
			}
			task.Dispose();
		}

		public static void OnStatusUpdated()
		{
			WindowPending.UpdateAllWindows();
		}

		public static void UpdateAllWindows()
		{
			WindowPending[] array = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
			WindowPending[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				WindowPending windowPending = array2[i];
				windowPending.UpdateWindow();
			}
		}

		public static void CloseAllWindows()
		{
			WindowPending[] array = Resources.FindObjectsOfTypeAll(typeof(WindowPending)) as WindowPending[];
			WindowPending windowPending = (array.Length <= 0) ? null : array[0];
			if (windowPending != null)
			{
				windowPending.Close();
			}
		}

		private void OnIncoming(Task task)
		{
			this.CreateStaticResources();
			this.PopulateListControl(this.incomingList, task, this.syncIcon);
		}

		private void OnChangeSets(Task task)
		{
			this.CreateStaticResources();
			this.PopulateListControl(this.pendingList, task, WindowPending.changeIcon);
		}

		private void PopulateListControl(ListControl list, Task task, Texture2D icon)
		{
			ChangeSets changeSets = task.changeSets;
			ListItem listItem = list.Root.FirstChild;
			while (listItem != null)
			{
				ChangeSet cs = listItem.Item as ChangeSet;
				if (changeSets.Find((ChangeSet elm) => elm.id == cs.id) == null)
				{
					ListItem listItem2 = listItem;
					listItem = listItem.Next;
					list.Root.Remove(listItem2);
				}
				else
				{
					listItem = listItem.Next;
				}
			}
			foreach (ChangeSet current in changeSets)
			{
				ListItem listItem3 = list.GetChangeSetItem(current);
				if (listItem3 != null)
				{
					listItem3.Item = current;
					listItem3.Name = current.description;
				}
				else
				{
					listItem3 = list.Add(null, current.description, current);
				}
				listItem3.Exclusive = true;
				listItem3.CanAccept = true;
				listItem3.Icon = icon;
			}
			list.Refresh();
			base.Repaint();
		}

		private void OnChangeContents(Task task)
		{
			ListItem listItem = this.pendingList.FindItemWithIdentifier(task.userIdentifier);
			ListItem listItem2 = (listItem != null) ? listItem : this.incomingList.FindItemWithIdentifier(task.userIdentifier);
			if (listItem2 != null)
			{
				ListControl listControl = (listItem != null) ? this.pendingList : this.incomingList;
				listItem2.RemoveAll();
				AssetList assetList = task.assetList;
				if (assetList.Count == 0)
				{
					ListItem listItem3 = listControl.Add(listItem2, "Empty change list", null);
					listItem3.Dummy = true;
				}
				else
				{
					foreach (Asset current in assetList)
					{
						listControl.Add(listItem2, current.prettyPath, current);
					}
				}
				listControl.Refresh(false);
				base.Repaint();
			}
		}

		private ChangeSets GetEmptyChangeSetsCandidates()
		{
			ListControl listControl = this.pendingList;
			ChangeSets emptyChangeSets = listControl.EmptyChangeSets;
			ChangeSets toDelete = new ChangeSets();
			emptyChangeSets.FindAll((ChangeSet item) => item.id != ChangeSet.defaultID).ForEach(delegate(ChangeSet s)
			{
				toDelete.Add(s);
			});
			return toDelete;
		}

		private bool HasEmptyPendingChangesets()
		{
			ChangeSets emptyChangeSetsCandidates = this.GetEmptyChangeSetsCandidates();
			return Provider.DeleteChangeSetsIsValid(emptyChangeSetsCandidates);
		}

		private void DeleteEmptyPendingChangesets()
		{
			ChangeSets emptyChangeSetsCandidates = this.GetEmptyChangeSetsCandidates();
			Provider.DeleteChangeSets(emptyChangeSetsCandidates).SetCompletionAction(CompletionAction.UpdatePendingWindow);
		}

		private void OnGUI()
		{
			this.InitStyles();
			if (!WindowPending.s_DidReload)
			{
				WindowPending.s_DidReload = true;
				this.UpdateWindow();
			}
			this.CreateResources();
			Event current = Event.current;
			float fixedHeight = EditorStyles.toolbar.fixedHeight;
			bool flag = false;
			GUILayout.BeginArea(new Rect(0f, 0f, base.position.width, fixedHeight));
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			int num = (this.incomingList.Root != null) ? this.incomingList.Root.ChildCount : 0;
			this.m_ShowIncoming = !GUILayout.Toggle(!this.m_ShowIncoming, "Outgoing", EditorStyles.toolbarButton, new GUILayoutOption[0]);
			GUIContent content = GUIContent.Temp("Incoming" + ((num != 0) ? (" (" + num.ToString() + ")") : ""));
			this.m_ShowIncoming = GUILayout.Toggle(this.m_ShowIncoming, content, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				flag = true;
			}
			GUILayout.FlexibleSpace();
			using (new EditorGUI.DisabledScope(Provider.activeTask != null))
			{
				CustomCommand[] customCommands = Provider.customCommands;
				for (int i = 0; i < customCommands.Length; i++)
				{
					CustomCommand customCommand = customCommands[i];
					if (customCommand.context == CommandContext.Global && GUILayout.Button(customCommand.label, EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						customCommand.StartTask();
					}
				}
			}
			bool flag2 = Mathf.FloorToInt(base.position.width - this.s_ToolbarButtonsWidth - this.s_SettingsButtonWidth - this.s_DeleteChangesetsButtonWidth) > 0 && this.HasEmptyPendingChangesets();
			if (flag2 && GUILayout.Button("Delete Empty Changesets", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.DeleteEmptyPendingChangesets();
			}
			bool flag3 = Mathf.FloorToInt(base.position.width - this.s_ToolbarButtonsWidth - this.s_SettingsButtonWidth) > 0;
			if (flag3 && GUILayout.Button("Settings", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
				EditorWindow.FocusWindowIfItsOpen<InspectorWindow>();
				GUIUtility.ExitGUI();
			}
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			bool flag4 = GUILayout.Button(this.refreshIcon, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			flag = (flag || flag4);
			GUI.color = color;
			if (current.isKey && GUIUtility.keyboardControl == 0)
			{
				if (current.type == EventType.KeyDown && current.keyCode == KeyCode.F5)
				{
					flag = true;
					current.Use();
				}
			}
			if (flag)
			{
				if (flag4)
				{
					Provider.InvalidateCache();
				}
				this.UpdateWindow();
			}
			GUILayout.EndArea();
			Rect rect = new Rect(0f, fixedHeight, base.position.width, base.position.height - fixedHeight - 17f);
			bool flag5 = false;
			GUILayout.EndHorizontal();
			if (!Provider.isActive)
			{
				Color color2 = GUI.color;
				GUI.color = new Color(0.8f, 0.5f, 0.5f);
				rect.height = fixedHeight;
				GUILayout.BeginArea(rect);
				GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				string text = "DISABLED";
				if (Provider.enabled)
				{
					if (Provider.onlineState == OnlineState.Updating)
					{
						GUI.color = new Color(0.8f, 0.8f, 0.5f);
						text = "CONNECTING...";
					}
					else
					{
						text = "OFFLINE";
					}
				}
				GUILayout.Label(text, EditorStyles.miniLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
				rect.y += rect.height;
				if (!string.IsNullOrEmpty(Provider.offlineReason))
				{
					GUI.Label(rect, Provider.offlineReason);
				}
				GUI.color = color2;
				flag5 = false;
			}
			else
			{
				if (this.m_ShowIncoming)
				{
					flag5 |= this.incomingList.OnGUI(rect, base.hasFocus);
				}
				else
				{
					flag5 |= this.pendingList.OnGUI(rect, base.hasFocus);
				}
				rect.y += rect.height;
				rect.height = 17f;
				GUI.Label(rect, GUIContent.none, WindowPending.s_Styles.bottomBarBg);
				GUIContent content2 = new GUIContent("Apply All Incoming Changes");
				Vector2 vector = EditorStyles.miniButton.CalcSize(content2);
				Rect rect2 = new Rect(rect.x, rect.y - 2f, rect.width - vector.x - 5f, rect.height);
				WindowPending.ProgressGUI(rect2, Provider.activeTask, false);
				if (this.m_ShowIncoming)
				{
					Rect position = rect;
					position.width = vector.x;
					position.height = vector.y;
					position.y = rect.y + 2f;
					position.x = base.position.width - vector.x - 5f;
					using (new EditorGUI.DisabledScope(this.incomingList.Size == 0))
					{
						if (GUI.Button(position, content2, EditorStyles.miniButton))
						{
							Asset item = new Asset("");
							Task latest = Provider.GetLatest(new AssetList
							{
								item
							});
							latest.SetCompletionAction(CompletionAction.OnGotLatestPendingWindow);
						}
					}
				}
			}
			if (flag5)
			{
				base.Repaint();
			}
		}

		internal static bool ProgressGUI(Rect rect, Task activeTask, bool descriptionTextFirst)
		{
			bool result;
			if (activeTask != null && (activeTask.progressPct != -1 || activeTask.secondsSpent != -1 || activeTask.progressMessage.Length != 0))
			{
				string text = activeTask.progressMessage;
				Rect position = rect;
				GUIContent statusWheel = WindowPending.StatusWheel;
				position.width = position.height;
				position.x += 4f;
				position.y += 4f;
				GUI.Label(position, statusWheel);
				rect.x += position.width + 4f;
				text = ((text.Length != 0) ? text : activeTask.description);
				if (activeTask.progressPct == -1)
				{
					rect.width -= position.width + 4f;
					rect.y += 4f;
					GUI.Label(rect, text, EditorStyles.miniLabel);
				}
				else
				{
					rect.width = 120f;
					EditorGUI.ProgressBar(rect, (float)activeTask.progressPct, text);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void CreateResources()
		{
			if (this.refreshIcon == null)
			{
				this.refreshIcon = EditorGUIUtility.LoadIcon("Refresh");
				this.refreshIcon.hideFlags = HideFlags.HideAndDontSave;
				this.refreshIcon.name = "RefreshIcon";
			}
			if (this.header == null)
			{
				this.header = "OL Title";
			}
			this.CreateStaticResources();
			if (this.s_ToolbarButtonsWidth == 0f)
			{
				this.s_ToolbarButtonsWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent("Incoming (xx)")).x;
				this.s_ToolbarButtonsWidth += EditorStyles.toolbarButton.CalcSize(new GUIContent("Outgoing")).x;
				this.s_ToolbarButtonsWidth += EditorStyles.toolbarButton.CalcSize(new GUIContent(this.refreshIcon)).x;
				this.s_SettingsButtonWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent("Settings")).x;
				this.s_DeleteChangesetsButtonWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent("Delete Empty Changesets")).x;
			}
		}

		private void CreateStaticResources()
		{
			if (this.syncIcon == null)
			{
				this.syncIcon = EditorGUIUtility.LoadIcon("vcs_incoming");
				this.syncIcon.hideFlags = HideFlags.HideAndDontSave;
				this.syncIcon.name = "SyncIcon";
			}
			if (WindowPending.changeIcon == null)
			{
				WindowPending.changeIcon = EditorGUIUtility.LoadIcon("vcs_change");
				WindowPending.changeIcon.hideFlags = HideFlags.HideAndDontSave;
				WindowPending.changeIcon.name = "ChangeIcon";
			}
		}
	}
}
