using System;
using UnityEditorInternal.VersionControl;
using UnityEngine;
namespace UnityEditor.VersionControl
{
	internal class WindowPending : EditorWindow
	{
		internal class Styles
		{
			public GUIStyle box = "CN Box";
			public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
		}
		private const float k_ResizerHeight = 17f;
		private const float k_MinIncomingAreaHeight = 50f;
		private const float k_BottomBarHeight = 17f;
		private static WindowPending.Styles s_Styles;
		private static Texture2D changeIcon;
		private Texture2D syncIcon;
		private Texture2D refreshIcon;
		private GUIStyle header;
		[SerializeField]
		private ListControl pendingList;
		[SerializeField]
		private ListControl incomingList;
		private bool m_ShowIncoming;
		private float s_ToolbarButtonsWidth;
		private static GUIContent[] sStatusWheel;
		private static bool s_DidReload;
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
			base.title = "UnityEditor.VersionControl";
			if (this.pendingList == null)
			{
				this.pendingList = new ListControl();
			}
			ListControl expr_27 = this.pendingList;
			expr_27.ExpandEvent = (ListControl.ExpandDelegate)Delegate.Combine(expr_27.ExpandEvent, new ListControl.ExpandDelegate(this.OnExpand));
			ListControl expr_4E = this.pendingList;
			expr_4E.DragEvent = (ListControl.DragDelegate)Delegate.Combine(expr_4E.DragEvent, new ListControl.DragDelegate(this.OnDrop));
			this.pendingList.MenuDefault = "CONTEXT/Pending";
			this.pendingList.MenuFolder = "CONTEXT/Change";
			this.pendingList.DragAcceptOnly = true;
			if (this.incomingList == null)
			{
				this.incomingList = new ListControl();
			}
			ListControl expr_B7 = this.incomingList;
			expr_B7.ExpandEvent = (ListControl.ExpandDelegate)Delegate.Combine(expr_B7.ExpandEvent, new ListControl.ExpandDelegate(this.OnExpandIncoming));
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
		private void OnExpand(ChangeSet change, ListItem item)
		{
			if (!Provider.isActive)
			{
				return;
			}
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
		private void OnExpandIncoming(ChangeSet change, ListItem item)
		{
			if (!Provider.isActive)
			{
				return;
			}
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
		private void UpdateWindow()
		{
			if (!Provider.isActive)
			{
				this.pendingList.Clear();
				base.Repaint();
				return;
			}
			Task task = Provider.ChangeSets();
			task.SetCompletionAction(CompletionAction.OnChangeSetsPendingWindow);
			Task task2 = Provider.Incoming();
			task2.SetCompletionAction(CompletionAction.OnIncomingPendingWindow);
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
				if (completionAction == CompletionAction.UpdatePendingWindow)
				{
					windowPending.UpdateWindow();
				}
				else
				{
					if (completionAction == CompletionAction.OnChangeContentsPendingWindow)
					{
						windowPending.OnChangeContents(task);
					}
					else
					{
						if (completionAction == CompletionAction.OnIncomingPendingWindow)
						{
							windowPending.OnIncoming(task);
						}
						else
						{
							if (completionAction == CompletionAction.OnChangeSetsPendingWindow)
							{
								windowPending.OnChangeSets(task);
							}
							else
							{
								if (completionAction == CompletionAction.OnGotLatestPendingWindow)
								{
									windowPending.OnGotLatest(task);
								}
							}
						}
					}
				}
			}
			if (completionAction == CompletionAction.OnSubmittedChangeWindow)
			{
				WindowChange.OnSubmitted(task);
			}
			else
			{
				if (completionAction == CompletionAction.OnAddedChangeWindow)
				{
					WindowChange.OnAdded(task);
				}
			}
			task.Dispose();
		}
		public static void OnStatusUpdated()
		{
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
			if (listItem2 == null)
			{
				return;
			}
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
			GUILayout.BeginArea(new Rect(0f, 0f, base.position.width, fixedHeight));
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			int num = (this.incomingList.Root != null) ? this.incomingList.Root.ChildCount : 0;
			this.m_ShowIncoming = !GUILayout.Toggle(!this.m_ShowIncoming, "Outgoing", EditorStyles.toolbarButton, new GUILayoutOption[0]);
			GUIContent content = GUIContent.Temp("Incoming" + ((num != 0) ? (" (" + num.ToString() + ")") : string.Empty));
			this.m_ShowIncoming = GUILayout.Toggle(this.m_ShowIncoming, content, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUI.BeginDisabledGroup(Provider.activeTask != null);
			CustomCommand[] customCommands = Provider.customCommands;
			for (int i = 0; i < customCommands.Length; i++)
			{
				CustomCommand customCommand = customCommands[i];
				if (customCommand.context == CommandContext.Global && GUILayout.Button(customCommand.label, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					customCommand.StartTask();
				}
			}
			EditorGUI.EndDisabledGroup();
			bool flag = Mathf.FloorToInt(base.position.width - this.s_ToolbarButtonsWidth) > 0;
			if (flag && GUILayout.Button("Settings", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
				EditorWindow.FocusWindowIfItsOpen<InspectorWindow>();
				GUIUtility.ExitGUI();
			}
			Color color = GUI.color;
			GUI.color = new Color(1f, 1f, 1f, 0.5f);
			bool flag2 = GUILayout.Button(this.refreshIcon, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			GUI.color = color;
			if (current.isKey && GUIUtility.keyboardControl == 0 && current.type == EventType.KeyDown && current.keyCode == KeyCode.F5)
			{
				flag2 = true;
				current.Use();
			}
			if (flag2)
			{
				Provider.InvalidateCache();
				if (Provider.isActive && Provider.onlineState == OnlineState.Online)
				{
					Task task = Provider.ChangeSets();
					task.SetCompletionAction(CompletionAction.OnChangeSetsPendingWindow);
					Task task2 = Provider.Incoming();
					task2.SetCompletionAction(CompletionAction.OnIncomingPendingWindow);
				}
				else
				{
					Provider.UpdateSettings();
				}
			}
			GUILayout.EndArea();
			Rect rect = new Rect(0f, fixedHeight, base.position.width, base.position.height - fixedHeight - 17f);
			bool flag3 = false;
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
				flag3 = false;
			}
			else
			{
				if (this.m_ShowIncoming)
				{
					flag3 |= this.incomingList.OnGUI(rect, base.hasFocus);
				}
				else
				{
					flag3 |= this.pendingList.OnGUI(rect, base.hasFocus);
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
					EditorGUI.BeginDisabledGroup(this.incomingList.Size == 0);
					if (GUI.Button(position, content2, EditorStyles.miniButton))
					{
						Asset item = new Asset(string.Empty);
						Task latest = Provider.GetLatest(new AssetList
						{
							item
						});
						latest.SetCompletionAction(CompletionAction.OnGotLatestPendingWindow);
					}
					EditorGUI.EndDisabledGroup();
				}
			}
			if (flag3)
			{
				base.Repaint();
			}
		}
		internal static bool ProgressGUI(Rect rect, Task activeTask, bool descriptionTextFirst)
		{
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
				return true;
			}
			return false;
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
				this.s_ToolbarButtonsWidth += EditorStyles.toolbarButton.CalcSize(new GUIContent("Settings")).x;
				this.s_ToolbarButtonsWidth += EditorStyles.toolbarButton.CalcSize(new GUIContent(this.refreshIcon)).x;
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
