using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class ASServerAdminWindow
	{
		private enum Action
		{
			Main,
			CreateUser,
			SetPassword,
			CreateProject,
			ModifyUser
		}

		private const int listLenghts = 20;

		private static ASMainWindow.Constants constants = null;

		private ListViewState lv;

		private ListViewState lv2;

		private SplitterState lvSplit = new SplitterState(new float[]
		{
			5f,
			50f,
			50f,
			150f
		}, new int[]
		{
			20,
			70,
			70,
			100
		}, null);

		private MaintDatabaseRecord[] databases;

		private MaintUserRecord[] users;

		private ASMainWindow parentWin;

		private bool splittersOk = false;

		private bool resetKeyboardControl = false;

		private ASServerAdminWindow.Action currAction = ASServerAdminWindow.Action.Main;

		private string[] servers;

		private string server = string.Empty;

		private string user = string.Empty;

		private string password = string.Empty;

		private string nPassword1 = string.Empty;

		private string nPassword2 = string.Empty;

		private string nProjectName = string.Empty;

		private string nTemplateProjectName = string.Empty;

		private string nFullName = string.Empty;

		private string nUserName = string.Empty;

		private string nEmail = string.Empty;

		private bool projectSelected = false;

		private bool userSelected = false;

		private bool isConnected = false;

		public ASServerAdminWindow(ASMainWindow parentWin)
		{
			this.lv = new ListViewState(0);
			this.lv2 = new ListViewState(0);
			this.parentWin = parentWin;
			this.servers = InternalEditorUtility.GetEditorSettingsList("ASServer", 20);
			this.server = EditorPrefs.GetString("ASAdminServer");
			this.user = "admin";
		}

		private void ServersPopup()
		{
			if (this.servers.Length > 0)
			{
				int num = EditorGUILayout.Popup(-1, this.servers, ASServerAdminWindow.constants.dropDown, new GUILayoutOption[]
				{
					GUILayout.MaxWidth(18f)
				});
				if (num >= 0)
				{
					GUIUtility.keyboardControl = 0;
					GUIUtility.hotControl = 0;
					this.resetKeyboardControl = true;
					this.server = this.servers[num];
					this.parentWin.Repaint();
				}
			}
		}

		private bool WordWrappedLabelButton(string label, string buttonText)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
			bool result = GUILayout.Button(buttonText, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			GUILayout.EndHorizontal();
			return result;
		}

		private bool CanPerformCurrentAction()
		{
			bool result;
			switch (this.currAction)
			{
			case ASServerAdminWindow.Action.Main:
				result = (this.server != string.Empty && this.user != string.Empty);
				break;
			case ASServerAdminWindow.Action.CreateUser:
			{
				bool flag = true;
				for (int i = 0; i < this.nUserName.Length; i++)
				{
					char c = this.nUserName[i];
					if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9') && c != '-' && c != '_')
					{
						flag = false;
						break;
					}
				}
				result = (this.nFullName != string.Empty && this.nUserName != string.Empty && this.nPassword1 != string.Empty && this.nPassword1 == this.nPassword2 && flag);
				break;
			}
			case ASServerAdminWindow.Action.SetPassword:
				result = (this.nPassword1 != string.Empty && this.nPassword1 == this.nPassword2);
				break;
			case ASServerAdminWindow.Action.CreateProject:
				result = (this.nProjectName != string.Empty);
				break;
			case ASServerAdminWindow.Action.ModifyUser:
				result = (this.nFullName != string.Empty);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}

		private void PerformCurrentAction()
		{
			switch (this.currAction)
			{
			case ASServerAdminWindow.Action.Main:
				this.currAction = ASServerAdminWindow.Action.Main;
				this.DoConnect();
				Event.current.Use();
				break;
			case ASServerAdminWindow.Action.CreateUser:
				AssetServer.AdminCreateUser(this.nUserName, this.nFullName, this.nEmail, this.nPassword1);
				this.currAction = ASServerAdminWindow.Action.Main;
				if (this.lv.row > -1)
				{
					this.DoGetUsers();
				}
				Event.current.Use();
				break;
			case ASServerAdminWindow.Action.SetPassword:
				AssetServer.AdminChangePassword(this.users[this.lv2.row].userName, this.nPassword1);
				this.currAction = ASServerAdminWindow.Action.Main;
				Event.current.Use();
				break;
			case ASServerAdminWindow.Action.CreateProject:
				if (AssetServer.AdminCreateDB(this.nProjectName, this.nTemplateProjectName) != 0)
				{
					this.DoRefreshDatabases();
					for (int i = 0; i < this.databases.Length; i++)
					{
						if (this.databases[i].name == this.nProjectName)
						{
							this.lv.row = i;
							this.DoGetUsers();
							break;
						}
					}
				}
				this.currAction = ASServerAdminWindow.Action.Main;
				Event.current.Use();
				break;
			case ASServerAdminWindow.Action.ModifyUser:
				AssetServer.AdminModifyUserInfo(this.databases[this.lv.row].dbName, this.users[this.lv2.row].userName, this.nFullName, this.nEmail);
				this.currAction = ASServerAdminWindow.Action.Main;
				if (this.lv.row > -1)
				{
					this.DoGetUsers();
				}
				Event.current.Use();
				break;
			}
		}

		private void ActionBox()
		{
			bool enabled = GUI.enabled;
			switch (this.currAction)
			{
			case ASServerAdminWindow.Action.Main:
				if (!this.isConnected)
				{
					GUI.enabled = false;
				}
				if (this.WordWrappedLabelButton("Want to create a new project?", "Create"))
				{
					this.nProjectName = "";
					this.nTemplateProjectName = "";
					this.currAction = ASServerAdminWindow.Action.CreateProject;
				}
				if (this.WordWrappedLabelButton("Want to create a new user?", "New User"))
				{
					this.nPassword1 = (this.nPassword2 = string.Empty);
					this.nFullName = (this.nUserName = (this.nEmail = string.Empty));
					this.currAction = ASServerAdminWindow.Action.CreateUser;
				}
				GUI.enabled = (this.isConnected && this.userSelected && enabled);
				if (this.WordWrappedLabelButton("Need to change user password?", "Set Password"))
				{
					this.nPassword1 = (this.nPassword2 = string.Empty);
					this.currAction = ASServerAdminWindow.Action.SetPassword;
				}
				if (this.WordWrappedLabelButton("Need to change user information?", "Edit"))
				{
					this.nFullName = this.users[this.lv2.row].fullName;
					this.nEmail = this.users[this.lv2.row].email;
					this.currAction = ASServerAdminWindow.Action.ModifyUser;
				}
				GUI.enabled = (this.isConnected && this.projectSelected && enabled);
				if (this.WordWrappedLabelButton("Duplicate selected project", "Copy Project"))
				{
					this.nProjectName = "";
					this.nTemplateProjectName = this.databases[this.lv.row].name;
					this.currAction = ASServerAdminWindow.Action.CreateProject;
				}
				if (this.WordWrappedLabelButton("Delete selected project", "Delete Project"))
				{
					if (EditorUtility.DisplayDialog("Delete project", "Are you sure you want to delete project " + this.databases[this.lv.row].name + "? This operation cannot be undone!", "Delete", "Cancel"))
					{
						if (AssetServer.AdminDeleteDB(this.databases[this.lv.row].name) != 0)
						{
							this.DoRefreshDatabases();
							GUIUtility.ExitGUI();
						}
					}
				}
				GUI.enabled = (this.isConnected && this.userSelected && enabled);
				if (this.WordWrappedLabelButton("Delete selected user", "Delete User"))
				{
					if (EditorUtility.DisplayDialog("Delete user", "Are you sure you want to delete user " + this.users[this.lv2.row].userName + "? This operation cannot be undone!", "Delete", "Cancel"))
					{
						if (AssetServer.AdminDeleteUser(this.users[this.lv2.row].userName) != 0)
						{
							if (this.lv.row > -1)
							{
								this.DoGetUsers();
							}
							GUIUtility.ExitGUI();
						}
					}
				}
				GUI.enabled = enabled;
				break;
			case ASServerAdminWindow.Action.CreateUser:
				this.nFullName = EditorGUILayout.TextField("Full Name:", this.nFullName, new GUILayoutOption[0]);
				this.nEmail = EditorGUILayout.TextField("Email Address:", this.nEmail, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				this.nUserName = EditorGUILayout.TextField("User Name:", this.nUserName, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				this.nPassword1 = EditorGUILayout.PasswordField("Password:", this.nPassword1, new GUILayoutOption[0]);
				this.nPassword2 = EditorGUILayout.PasswordField("Repeat Password:", this.nPassword2, new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUI.enabled = (this.CanPerformCurrentAction() && enabled);
				if (GUILayout.Button("Create User", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.PerformCurrentAction();
				}
				GUI.enabled = enabled;
				if (GUILayout.Button("Cancel", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.currAction = ASServerAdminWindow.Action.Main;
				}
				GUILayout.EndHorizontal();
				break;
			case ASServerAdminWindow.Action.SetPassword:
				GUILayout.Label("Setting password for user: " + this.users[this.lv2.row].userName, ASServerAdminWindow.constants.title, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				this.nPassword1 = EditorGUILayout.PasswordField("Password:", this.nPassword1, new GUILayoutOption[0]);
				this.nPassword2 = EditorGUILayout.PasswordField("Repeat Password:", this.nPassword2, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUI.enabled = (this.CanPerformCurrentAction() && enabled);
				if (GUILayout.Button("Change Password", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.PerformCurrentAction();
				}
				GUI.enabled = enabled;
				if (GUILayout.Button("Cancel", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.currAction = ASServerAdminWindow.Action.Main;
				}
				GUILayout.EndHorizontal();
				break;
			case ASServerAdminWindow.Action.CreateProject:
				this.nProjectName = EditorGUILayout.TextField("Project Name:", this.nProjectName, new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUI.enabled = (this.CanPerformCurrentAction() && enabled);
				if (GUILayout.Button((!(this.nTemplateProjectName == "")) ? ("Copy " + this.nTemplateProjectName) : "Create Project", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.PerformCurrentAction();
				}
				GUI.enabled = enabled;
				if (GUILayout.Button("Cancel", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.currAction = ASServerAdminWindow.Action.Main;
				}
				GUILayout.EndHorizontal();
				break;
			case ASServerAdminWindow.Action.ModifyUser:
				this.nFullName = EditorGUILayout.TextField("Full Name:", this.nFullName, new GUILayoutOption[0]);
				this.nEmail = EditorGUILayout.TextField("Email Address:", this.nEmail, new GUILayoutOption[0]);
				GUILayout.Space(5f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUI.enabled = (this.CanPerformCurrentAction() && enabled);
				if (GUILayout.Button("Change", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.PerformCurrentAction();
				}
				GUI.enabled = enabled;
				if (GUILayout.Button("Cancel", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
				{
					this.currAction = ASServerAdminWindow.Action.Main;
				}
				GUILayout.EndHorizontal();
				break;
			}
		}

		private void DoRefreshDatabases()
		{
			MaintDatabaseRecord[] array = AssetServer.AdminRefreshDatabases();
			if (array != null)
			{
				this.databases = array;
				this.isConnected = true;
			}
			else
			{
				this.databases = new MaintDatabaseRecord[0];
				this.lv2.totalRows = 0;
			}
			this.lv.row = -1;
			this.lv.totalRows = this.databases.Length;
			this.lv2.totalRows = 0;
			this.users = new MaintUserRecord[0];
		}

		private void DoConnect()
		{
			EditorPrefs.SetString("ASAdminServer", this.server);
			this.userSelected = false;
			this.isConnected = false;
			this.projectSelected = false;
			this.lv.row = -1;
			this.lv2.row = -1;
			this.lv.totalRows = 0;
			this.lv2.totalRows = 0;
			int port = 10733;
			string text;
			if (this.server.IndexOf(":") > 0)
			{
				int.TryParse(this.server.Substring(this.server.IndexOf(":") + 1), out port);
				text = this.server.Substring(0, this.server.IndexOf(":"));
			}
			else
			{
				text = this.server;
			}
			AssetServer.AdminSetCredentials(text, port, this.user, this.password);
			this.DoRefreshDatabases();
		}

		private void DoGetUsers()
		{
			MaintUserRecord[] array = AssetServer.AdminGetUsers(this.databases[this.lv.row].dbName);
			if (array != null)
			{
				this.users = array;
			}
			else
			{
				this.users = new MaintUserRecord[0];
			}
			this.lv2.totalRows = this.users.Length;
			this.lv2.row = -1;
		}

		public bool DoGUI()
		{
			bool enabled = GUI.enabled;
			if (ASServerAdminWindow.constants == null)
			{
				ASServerAdminWindow.constants = new ASMainWindow.Constants();
				ASServerAdminWindow.constants.toggleSize = ASServerAdminWindow.constants.toggle.CalcSize(new GUIContent("X"));
			}
			if (this.resetKeyboardControl)
			{
				this.resetKeyboardControl = false;
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASServerAdminWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Box("Server Connection", ASServerAdminWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASServerAdminWindow.constants.contentBox, new GUILayoutOption[0]);
			Event current = Event.current;
			if (current.type == EventType.KeyDown && current.keyCode == KeyCode.Return)
			{
				if (this.CanPerformCurrentAction())
				{
					this.PerformCurrentAction();
				}
			}
			if (current.type == EventType.KeyDown && current.keyCode == KeyCode.Escape && this.currAction != ASServerAdminWindow.Action.Main)
			{
				this.currAction = ASServerAdminWindow.Action.Main;
				current.Use();
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.server = EditorGUILayout.TextField("Server Address:", this.server, new GUILayoutOption[0]);
			this.ServersPopup();
			GUILayout.EndHorizontal();
			this.user = EditorGUILayout.TextField("User Name:", this.user, new GUILayoutOption[0]);
			this.password = EditorGUILayout.PasswordField("Password:", this.password, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUI.enabled = (this.CanPerformCurrentAction() && enabled);
			if (GUILayout.Button("Connect", ASServerAdminWindow.constants.smallButton, new GUILayoutOption[0]))
			{
				this.PerformCurrentAction();
			}
			GUI.enabled = enabled;
			GUILayout.EndHorizontal();
			if (AssetServer.GetAssetServerError() != string.Empty)
			{
				GUILayout.Label(AssetServer.GetAssetServerError(), ASServerAdminWindow.constants.errorLabel, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.BeginVertical(ASServerAdminWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Box("Admin Actions", ASServerAdminWindow.constants.title, new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASServerAdminWindow.constants.contentBox, new GUILayoutOption[0]);
			this.ActionBox();
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(ASServerAdminWindow.constants.groupBox, new GUILayoutOption[0]);
			GUILayout.Box("Project", ASServerAdminWindow.constants.title, new GUILayoutOption[0]);
			IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, ASServerAdminWindow.constants.background, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ListViewElement listViewElement = (ListViewElement)enumerator.Current;
					if (listViewElement.row == this.lv.row && Event.current.type == EventType.Repaint)
					{
						ASServerAdminWindow.constants.entrySelected.Draw(listViewElement.position, false, false, false, false);
					}
					GUILayout.Label(this.databases[listViewElement.row].name, new GUILayoutOption[0]);
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
			if (this.lv.selectionChanged)
			{
				if (this.lv.row > -1)
				{
					this.projectSelected = true;
				}
				this.currAction = ASServerAdminWindow.Action.Main;
				this.DoGetUsers();
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(ASServerAdminWindow.constants.groupBox, new GUILayoutOption[0]);
			SplitterGUILayout.BeginHorizontalSplit(this.lvSplit, new GUILayoutOption[0]);
			GUILayout.Box("", ASServerAdminWindow.constants.columnHeader, new GUILayoutOption[0]);
			GUILayout.Box("User", ASServerAdminWindow.constants.columnHeader, new GUILayoutOption[0]);
			GUILayout.Box("Full Name", ASServerAdminWindow.constants.columnHeader, new GUILayoutOption[0]);
			GUILayout.Box("Email", ASServerAdminWindow.constants.columnHeader, new GUILayoutOption[0]);
			SplitterGUILayout.EndHorizontalSplit();
			int left = EditorStyles.label.margin.left;
			IEnumerator enumerator2 = ListViewGUILayout.ListView(this.lv2, ASServerAdminWindow.constants.background, new GUILayoutOption[0]).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					ListViewElement listViewElement2 = (ListViewElement)enumerator2.Current;
					if (listViewElement2.row == this.lv2.row && Event.current.type == EventType.Repaint)
					{
						ASServerAdminWindow.constants.entrySelected.Draw(listViewElement2.position, false, false, false, false);
					}
					bool flag = this.users[listViewElement2.row].enabled != 0;
					bool flag2 = GUI.Toggle(new Rect(listViewElement2.position.x + 2f, listViewElement2.position.y - 1f, ASServerAdminWindow.constants.toggleSize.x, ASServerAdminWindow.constants.toggleSize.y), flag, "");
					GUILayout.Space(ASServerAdminWindow.constants.toggleSize.x);
					if (flag != flag2)
					{
						if (AssetServer.AdminSetUserEnabled(this.databases[this.lv.row].dbName, this.users[listViewElement2.row].userName, this.users[listViewElement2.row].fullName, this.users[listViewElement2.row].email, (!flag2) ? 0 : 1))
						{
							this.users[listViewElement2.row].enabled = ((!flag2) ? 0 : 1);
						}
					}
					GUILayout.Label(this.users[listViewElement2.row].userName, new GUILayoutOption[]
					{
						GUILayout.Width((float)(this.lvSplit.realSizes[1] - left))
					});
					GUILayout.Label(this.users[listViewElement2.row].fullName, new GUILayoutOption[]
					{
						GUILayout.Width((float)(this.lvSplit.realSizes[2] - left))
					});
					GUILayout.Label(this.users[listViewElement2.row].email, new GUILayoutOption[0]);
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			if (this.lv2.selectionChanged)
			{
				if (this.lv2.row > -1)
				{
					this.userSelected = true;
				}
				if (this.currAction == ASServerAdminWindow.Action.SetPassword)
				{
					this.currAction = ASServerAdminWindow.Action.Main;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			if (!this.splittersOk && Event.current.type == EventType.Repaint)
			{
				this.splittersOk = true;
				this.parentWin.Repaint();
			}
			return true;
		}
	}
}
