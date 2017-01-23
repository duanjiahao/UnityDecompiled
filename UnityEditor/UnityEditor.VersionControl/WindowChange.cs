using System;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace UnityEditor.VersionControl
{
	internal class WindowChange : EditorWindow
	{
		private ListControl submitList = new ListControl();

		private AssetList assetList = new AssetList();

		private ChangeSet changeSet = new ChangeSet();

		private string description = string.Empty;

		private bool allowSubmit = false;

		private Task taskStatus = null;

		private Task taskDesc = null;

		private Task taskStat = null;

		private Task taskSubmit = null;

		private Task taskAdd = null;

		private const int kSubmitNotStartedResultCode = 256;

		private const int kSubmitRunningResultCode = 0;

		private int submitResultCode = 256;

		private string submitErrorMessage = null;

		private int m_TextAreaControlID = 0;

		private const string c_defaultDescription = "";

		public void OnEnable()
		{
			base.position = new Rect(100f, 100f, 700f, 395f);
			base.minSize = new Vector2(700f, 395f);
			this.submitList.ReadOnly = true;
			this.taskStatus = null;
			this.taskDesc = null;
			this.taskStat = null;
			this.taskSubmit = null;
			this.submitResultCode = 256;
			this.submitErrorMessage = null;
		}

		public void OnDisable()
		{
			this.m_TextAreaControlID = 0;
		}

		public static void Open(AssetList list, bool submit)
		{
			WindowChange.Open(null, list, submit);
		}

		public static void Open(ChangeSet change, AssetList assets, bool submit)
		{
			WindowChange window = EditorWindow.GetWindow<WindowChange>(true, "Version Control Changeset");
			window.allowSubmit = submit;
			window.DoOpen(change, assets);
		}

		private string SanitizeDescription(string desc)
		{
			string result;
			if (Provider.GetActivePlugin() != null && Provider.GetActivePlugin().name != "Perforce")
			{
				result = desc;
			}
			else
			{
				int num = desc.IndexOf('\'');
				if (num == -1)
				{
					result = desc;
				}
				else
				{
					num++;
					int num2 = desc.IndexOf('\'', num);
					if (num2 == -1)
					{
						result = desc;
					}
					else
					{
						result = desc.Substring(num, num2 - num).Trim(new char[]
						{
							' ',
							'\t'
						});
					}
				}
			}
			return result;
		}

		private void DoOpen(ChangeSet change, AssetList assets)
		{
			this.taskSubmit = null;
			this.submitResultCode = 256;
			this.submitErrorMessage = null;
			this.changeSet = change;
			this.description = ((change != null) ? this.SanitizeDescription(change.description) : "");
			this.assetList = null;
			if (change == null)
			{
				this.taskStatus = Provider.Status(assets);
			}
			else
			{
				this.taskDesc = Provider.ChangeSetDescription(change);
				this.taskStat = Provider.ChangeSetStatus(change);
			}
		}

		private void RefreshList()
		{
			this.submitList.Clear();
			foreach (Asset current in this.assetList)
			{
				this.submitList.Add(null, current.prettyPath, current);
			}
			if (this.assetList.Count == 0)
			{
				ChangeSet changeSet = new ChangeSet("Empty change list");
				ListItem listItem = this.submitList.Add(null, changeSet.description, changeSet);
				listItem.Dummy = true;
			}
			this.submitList.Refresh();
			base.Repaint();
		}

		internal static void OnSubmitted(Task task)
		{
			WindowChange[] array = Resources.FindObjectsOfTypeAll(typeof(WindowChange)) as WindowChange[];
			if (array.Length != 0)
			{
				WindowChange windowChange = array[0];
				windowChange.assetList = task.assetList;
				windowChange.submitResultCode = task.resultCode;
				windowChange.submitErrorMessage = null;
				if ((task.resultCode & 2) != 0)
				{
					string str = "";
					Message[] messages = task.messages;
					for (int i = 0; i < messages.Length; i++)
					{
						Message message = messages[i];
						if (message.severity == Message.Severity.Error)
						{
							WindowChange expr_7F = windowChange;
							expr_7F.submitErrorMessage = expr_7F.submitErrorMessage + str + message.message;
						}
					}
				}
				if ((task.resultCode & 3) != 0)
				{
					WindowPending.UpdateAllWindows();
					bool flag = windowChange.changeSet == null;
					if (flag)
					{
						Task task2 = Provider.Status("");
						task2.Wait();
						WindowPending.ExpandLatestChangeSet();
					}
				}
				if ((task.resultCode & 1) != 0)
				{
					windowChange.ResetAndClose();
				}
				else
				{
					windowChange.RefreshList();
				}
			}
		}

		internal static void OnAdded(Task task)
		{
			WindowChange[] array = Resources.FindObjectsOfTypeAll(typeof(WindowChange)) as WindowChange[];
			if (array.Length != 0)
			{
				WindowChange windowChange = array[0];
				windowChange.taskSubmit = null;
				windowChange.submitResultCode = 256;
				windowChange.submitErrorMessage = null;
				windowChange.taskAdd = null;
				windowChange.taskStatus = Provider.Status(windowChange.assetList, false);
				windowChange.assetList = null;
				WindowPending.UpdateAllWindows();
			}
		}

		private void OnGUI()
		{
			if ((this.submitResultCode & 4) != 0)
			{
				this.OnConflictingFilesGUI();
			}
			else if ((this.submitResultCode & 8) != 0)
			{
				this.OnUnaddedFilesGUI();
			}
			else if ((this.submitResultCode & 2) != 0)
			{
				this.OnErrorGUI();
			}
			else
			{
				this.OnSubmitGUI();
			}
		}

		private void OnSubmitGUI()
		{
			bool flag = this.submitResultCode != 256;
			if (flag)
			{
				GUI.enabled = false;
			}
			Event current = Event.current;
			if (current.isKey && current.keyCode == KeyCode.Escape)
			{
				base.Close();
			}
			GUILayout.Label("Description", EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (this.taskStatus != null && this.taskStatus.resultCode != 0)
			{
				this.assetList = this.taskStatus.assetList.Filter(true, new Asset.States[]
				{
					Asset.States.CheckedOutLocal,
					Asset.States.DeletedLocal,
					Asset.States.AddedLocal
				});
				this.RefreshList();
				this.taskStatus = null;
			}
			else if (this.taskDesc != null && this.taskDesc.resultCode != 0)
			{
				this.description = ((this.taskDesc.text.Length <= 0) ? "" : this.taskDesc.text);
				if (this.description.Trim() == "<enter description here>")
				{
					this.description = string.Empty;
				}
				this.taskDesc = null;
			}
			else if (this.taskStat != null && this.taskStat.resultCode != 0)
			{
				this.assetList = this.taskStat.assetList;
				this.RefreshList();
				this.taskStat = null;
			}
			Task task = (this.taskStatus == null || this.taskStatus.resultCode != 0) ? ((this.taskDesc == null || this.taskDesc.resultCode != 0) ? ((this.taskStat == null || this.taskStat.resultCode != 0) ? this.taskSubmit : this.taskStat) : this.taskDesc) : this.taskStatus;
			GUI.enabled = ((this.taskDesc == null || this.taskDesc.resultCode != 0) && this.submitResultCode == 256);
			this.description = EditorGUILayout.TextArea(this.description, new GUILayoutOption[]
			{
				GUILayout.Height(150f)
			}).Trim();
			if (this.m_TextAreaControlID == 0)
			{
				this.m_TextAreaControlID = EditorGUIUtility.s_LastControlID;
			}
			if (this.m_TextAreaControlID != 0)
			{
				GUIUtility.keyboardControl = this.m_TextAreaControlID;
				EditorGUIUtility.editingTextField = true;
			}
			GUI.enabled = true;
			GUILayout.Label("Files", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			Rect screenRect = new Rect(6f, 206f, base.position.width - 12f, base.position.height - 248f);
			GUILayout.BeginArea(screenRect);
			GUILayout.Box("", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true)
			});
			GUILayout.EndArea();
			this.submitList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (this.submitResultCode == 256)
			{
				if (task != null)
				{
					GUIContent gUIContent = GUIContent.Temp("Getting info");
					gUIContent.image = WindowPending.StatusWheel.image;
					GUILayout.Label(gUIContent, new GUILayoutOption[0]);
					gUIContent.image = null;
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
				{
					this.ResetAndClose();
				}
				GUI.enabled = (task == null && !string.IsNullOrEmpty(this.description));
				bool flag2 = current.isKey && current.shift && current.keyCode == KeyCode.Return;
				bool flag3 = flag2 && !this.allowSubmit;
				if (Provider.hasChangelistSupport && (GUILayout.Button("Save", new GUILayoutOption[0]) || flag3))
				{
					this.Save(false);
				}
				if (this.allowSubmit)
				{
					bool enabled = GUI.enabled;
					GUI.enabled = (this.assetList != null && this.assetList.Count > 0 && !string.IsNullOrEmpty(this.description));
					if (GUILayout.Button("Submit", new GUILayoutOption[0]) || flag2)
					{
						this.Save(true);
					}
					GUI.enabled = enabled;
				}
			}
			else
			{
				bool flag4 = (this.submitResultCode & 1) != 0;
				GUI.enabled = flag4;
				string text = "";
				if (flag4)
				{
					text = "Finished successfully";
				}
				else if (task != null)
				{
					GUILayout.Label(WindowPending.StatusWheel, new GUILayoutOption[0]);
					text = task.progressMessage;
					if (text.Length == 0)
					{
						text = "Running...";
					}
				}
				GUILayout.Label(text, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Close", new GUILayoutOption[0]))
				{
					this.ResetAndClose();
				}
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal();
			GUILayout.Space(12f);
			if (task != null)
			{
				base.Repaint();
			}
		}

		private void OnErrorGUI()
		{
			GUILayout.Label("Submit failed", EditorStyles.boldLabel, new GUILayoutOption[0]);
			string text = "";
			if (!string.IsNullOrEmpty(this.submitErrorMessage))
			{
				text = this.submitErrorMessage + "\n";
			}
			text += "See console for details. You can get more details by increasing log level in EditorSettings.";
			GUILayout.Label(text, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close", new GUILayoutOption[0]))
			{
				this.ResetAndClose();
				WindowPending.UpdateAllWindows();
			}
			GUILayout.EndHorizontal();
		}

		private void OnConflictingFilesGUI()
		{
			string text = "";
			foreach (Asset current in this.assetList)
			{
				if (current.IsState(Asset.States.Conflicted))
				{
					text = text + current.prettyPath + "\n";
				}
			}
			GUILayout.Label("Conflicting files", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label("Some files need to be resolved before submitting:", new GUILayoutOption[0]);
			GUI.enabled = false;
			GUILayout.TextArea(text, new GUILayoutOption[]
			{
				GUILayout.ExpandHeight(true)
			});
			GUI.enabled = true;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Close", new GUILayoutOption[0]))
			{
				this.ResetAndClose();
			}
			GUILayout.EndHorizontal();
		}

		private void OnUnaddedFilesGUI()
		{
			AssetList assetList = new AssetList();
			string text = "";
			foreach (Asset current in this.assetList)
			{
				if (!current.IsState(Asset.States.OutOfSync) && !current.IsState(Asset.States.Synced) && !current.IsState(Asset.States.AddedLocal))
				{
					text = text + current.prettyPath + "\n";
					assetList.Add(current);
				}
			}
			GUILayout.Label("Files to add", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.Label("Some additional files need to be added:", new GUILayoutOption[0]);
			GUI.enabled = false;
			GUILayout.TextArea(text, new GUILayoutOption[0]);
			GUI.enabled = true;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add files", new GUILayoutOption[0]))
			{
				this.taskAdd = Provider.Add(assetList, false);
				this.taskAdd.SetCompletionAction(CompletionAction.OnAddedChangeWindow);
			}
			if (GUILayout.Button("Abort", new GUILayoutOption[0]))
			{
				this.ResetAndClose();
			}
			GUILayout.EndHorizontal();
		}

		private void ResetAndClose()
		{
			this.taskSubmit = null;
			this.submitResultCode = 256;
			this.submitErrorMessage = null;
			base.Close();
		}

		private void Save(bool submit)
		{
			if (this.description.Trim() == "")
			{
				Debug.LogError("Version control: Please enter a valid change description");
			}
			else
			{
				AssetDatabase.SaveAssets();
				this.taskSubmit = Provider.Submit(this.changeSet, this.assetList, this.description, !submit);
				this.submitResultCode = 0;
				this.submitErrorMessage = null;
				this.taskSubmit.SetCompletionAction(CompletionAction.OnSubmittedChangeWindow);
			}
		}
	}
}
