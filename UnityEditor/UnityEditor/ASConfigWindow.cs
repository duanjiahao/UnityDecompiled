using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASConfigWindow
	{
		private const int listLenghts = 20;

		private static ASMainWindow.Constants constants = null;

		private ListViewState serversLv = new ListViewState(0);

		private ListViewState projectsLv = new ListViewState(0);

		private string serverAddress = string.Empty;

		private string projectName = string.Empty;

		private string userName = string.Empty;

		private string password = string.Empty;

		private string port = string.Empty;

		private bool resetKeyboardControl = false;

		private string[] projectsList;

		private string[] serversList;

		private PListConfig plc;

		private ASMainWindow parentWin;

		public ASConfigWindow(ASMainWindow parent)
		{
			this.parentWin = parent;
			this.LoadConfig();
		}

		private void LoadConfig()
		{
			PListConfig pListConfig = new PListConfig("Library/ServerPreferences.plist");
			this.serverAddress = pListConfig["Maint Server"];
			this.userName = pListConfig["Maint UserName"];
			this.port = pListConfig["Maint port number"];
			this.projectName = pListConfig["Maint project name"];
			this.password = ASEditorBackend.GetPassword(this.serverAddress, this.userName);
			if (this.port != string.Empty && this.port != 10733.ToString())
			{
				this.serverAddress = this.serverAddress + ":" + this.port;
			}
			this.serversList = InternalEditorUtility.GetEditorSettingsList("ASServer", 20);
			this.serversLv.totalRows = this.serversList.Length;
			if (ArrayUtility.Contains<string>(this.serversList, this.serverAddress))
			{
				this.serversLv.row = ArrayUtility.IndexOf<string>(this.serversList, this.serverAddress);
			}
		}

		private void GetUserAndPassword()
		{
			string user = ASEditorBackend.GetUser(this.serverAddress);
			if (user != string.Empty)
			{
				this.userName = user;
			}
			user = ASEditorBackend.GetPassword(this.serverAddress, user);
			if (user != string.Empty)
			{
				this.password = user;
			}
		}

		private void GetDefaultPListConfig()
		{
			this.plc = new PListConfig("Library/ServerPreferences.plist");
			this.plc["Maint Server"] = "";
			this.plc["Maint UserName"] = "";
			this.plc["Maint database name"] = "";
			this.plc["Maint port number"] = "";
			this.plc["Maint project name"] = "";
			this.plc["Maint Password"] = "";
			if (this.plc["Maint settings type"] == string.Empty)
			{
				this.plc["Maint settings type"] = "manual";
			}
			if (this.plc["Maint Timeout"] == string.Empty)
			{
				this.plc["Maint Timeout"] = "5";
			}
			if (this.plc["Maint Connection Settings"] == string.Empty)
			{
				this.plc["Maint Connection Settings"] = "";
			}
		}

		private void DoShowProjects()
		{
			int num = 10733;
			string text = this.serverAddress;
			if (text.IndexOf(":") > 0)
			{
				int.TryParse(text.Substring(text.IndexOf(":") + 1), out num);
				text = text.Substring(0, text.IndexOf(":"));
			}
			AssetServer.AdminSetCredentials(text, num, this.userName, this.password);
			MaintDatabaseRecord[] array = AssetServer.AdminRefreshDatabases();
			if (array != null)
			{
				this.projectsList = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.projectsList[i] = array[i].name;
				}
				this.projectsLv.totalRows = array.Length;
				this.GetDefaultPListConfig();
				this.plc["Maint Server"] = text;
				this.plc["Maint UserName"] = this.userName;
				this.plc["Maint port number"] = this.port;
				this.plc.Save();
				ASEditorBackend.SetPassword(text, this.userName, this.password);
				ASEditorBackend.AddUser(this.serverAddress, this.userName);
			}
			else
			{
				this.projectsLv.totalRows = 0;
			}
		}

		private void ClearConfig()
		{
			if (EditorUtility.DisplayDialog("Clear Configuration", "Are you sure you want to disconnect from Asset Server project and clear all configuration values?", "Clear", "Cancel"))
			{
				this.plc = new PListConfig("Library/ServerPreferences.plist");
				this.plc.Clear();
				this.plc.Save();
				this.LoadConfig();
				this.projectsLv.totalRows = 0;
				ASEditorBackend.InitializeMaintBinding();
				this.resetKeyboardControl = true;
			}
		}

		private void DoConnect()
		{
			AssetServer.RemoveMaintErrorsFromConsole();
			int num = 10733;
			string text = this.serverAddress;
			if (text.IndexOf(":") > 0)
			{
				int.TryParse(text.Substring(text.IndexOf(":") + 1), out num);
				text = text.Substring(0, text.IndexOf(":"));
			}
			this.port = num.ToString();
			string databaseName = AssetServer.GetDatabaseName(text, this.userName, this.password, this.port, this.projectName);
			this.GetDefaultPListConfig();
			this.plc["Maint Server"] = text;
			this.plc["Maint UserName"] = this.userName;
			this.plc["Maint database name"] = databaseName;
			this.plc["Maint port number"] = this.port;
			this.plc["Maint project name"] = this.projectName;
			this.plc.Save();
			if (ArrayUtility.Contains<string>(this.serversList, this.serverAddress))
			{
				ArrayUtility.Remove<string>(ref this.serversList, this.serverAddress);
			}
			ArrayUtility.Insert<string>(ref this.serversList, 0, this.serverAddress);
			ASEditorBackend.AddUser(this.serverAddress, this.userName);
			ASEditorBackend.SetPassword(this.serverAddress, this.userName, this.password);
			InternalEditorUtility.SaveEditorSettingsList("ASServer", this.serversList, 20);
			if (databaseName != string.Empty)
			{
				ASEditorBackend.InitializeMaintBinding();
				this.parentWin.Reinit();
				GUIUtility.ExitGUI();
			}
			else
			{
				this.parentWin.NeedsSetup = true;
				this.parentWin.Repaint();
			}
		}

		private void ServersPopup()
		{
			if (this.serversList.Length > 0)
			{
				int num = EditorGUILayout.Popup(-1, this.serversList, ASConfigWindow.constants.dropDown, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(18f)
				});
				if (num >= 0)
				{
					GUIUtility.keyboardControl = 0;
					GUIUtility.hotControl = 0;
					this.resetKeyboardControl = true;
					this.serverAddress = this.serversList[num];
					this.parentWin.Repaint();
				}
			}
		}

		private void DoConfigGUI()
		{
			bool enabled = GUI.enabled;
			bool changed = GUI.changed;
			GUI.changed = false;
			bool flag = false;
			bool flag2 = false;
			Event current = Event.current;
			if (current.type == EventType.KeyDown)
			{
				bool flag3;
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					flag3 = (current.character == '\n' || current.character == '\u0003' || current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter);
				}
				else
				{
					if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
					{
						current.Use();
					}
					flag3 = (current.character == '\n' || current.character == '\u0003');
				}
				if (flag3)
				{
					string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
					if (nameOfFocusedControl != null)
					{
						if (nameOfFocusedControl == "password")
						{
							flag = true;
							goto IL_115;
						}
						if (nameOfFocusedControl == "project")
						{
							flag2 = true;
							goto IL_115;
						}
					}
					current.Use();
					IL_115:;
				}
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.serverAddress = EditorGUILayout.TextField("Server", this.serverAddress, new GUILayoutOption[0]);
			this.ServersPopup();
			GUILayout.EndHorizontal();
			if (GUI.changed)
			{
				this.GetUserAndPassword();
			}
			GUI.changed |= changed;
			this.userName = EditorGUILayout.TextField("User Name", this.userName, new GUILayoutOption[0]);
			GUI.SetNextControlName("password");
			this.password = EditorGUILayout.PasswordField("Password", this.password, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.userName != string.Empty && this.password != string.Empty && this.serverAddress != string.Empty && enabled);
			if (GUILayout.Button("Show Projects", new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}) || (flag && GUI.enabled))
			{
				this.DoShowProjects();
				this.projectName = "";
				EditorGUI.FocusTextInControl("project");
			}
			bool enabled2 = GUI.enabled;
			GUI.enabled = enabled;
			if (GUILayout.Button("Clear Configuration", new GUILayoutOption[0]))
			{
				this.ClearConfig();
			}
			GUI.enabled = enabled2;
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			changed = GUI.changed;
			GUI.changed = false;
			GUI.SetNextControlName("project");
			this.projectName = EditorGUILayout.TextField("Project Name", this.projectName, new GUILayoutOption[0]);
			GUI.changed |= changed;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.userName != string.Empty && this.password != string.Empty && this.serverAddress != string.Empty && this.projectName != string.Empty && enabled);
			if (GUILayout.Button("Connect", ASConfigWindow.constants.bigButton, new GUILayoutOption[]
			{
				GUILayout.MinWidth(100f)
			}) || (flag2 && GUI.enabled))
			{
				this.DoConnect();
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
		}

		private void DoProjectsGUI()
		{
			GUILayout.BeginVertical(ASConfigWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Label("Projects on Server", ASConfigWindow.constants.title, new GUILayoutOption[0]);
			IEnumerator enumerator = ListViewGUILayout.ListView(this.projectsLv, ASConfigWindow.constants.background, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					if (listViewElement.row == this.projectsLv.row && Event.current.type == EventType.Repaint)
					{
						ASConfigWindow.constants.entrySelected.Draw(listViewElement.position, false, false, false, false);
					}
					GUILayout.Label(this.projectsList[listViewElement.row], ASConfigWindow.constants.element, new GUILayoutOption[0]);
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
			if (this.projectsLv.selectionChanged)
			{
				this.projectName = this.projectsList[this.projectsLv.row];
			}
			GUILayout.EndVertical();
		}

		public bool DoGUI()
		{
			if (ASConfigWindow.constants == null)
			{
				ASConfigWindow.constants = new ASMainWindow.Constants();
			}
			if (this.resetKeyboardControl)
			{
				this.resetKeyboardControl = false;
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASConfigWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Box("Server Connection", ASConfigWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASConfigWindow.constants.contentBox, new GUILayoutOption[0]);
			this.DoConfigGUI();
			if (AssetServer.GetAssetServerError() != string.Empty)
			{
				GUILayout.Space(10f);
				GUILayout.Label(AssetServer.GetAssetServerError(), ASConfigWindow.constants.errorLabel, new GUILayoutOption[0]);
				GUILayout.Space(10f);
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			this.DoProjectsGUI();
			GUILayout.EndHorizontal();
			return true;
		}
	}
}
