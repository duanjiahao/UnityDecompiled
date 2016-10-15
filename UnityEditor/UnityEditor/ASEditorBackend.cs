using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	internal class ASEditorBackend
	{
		public const string kServerSettingsFile = "Library/ServerPreferences.plist";

		public const string kUserName = "Maint UserName";

		public const string kPassword = "Maint Password";

		public const string kTimeout = "Maint Timeout";

		public const string kSettingsType = "Maint settings type";

		public const string kConnectionSettings = "Maint Connection Settings";

		public const string kPortNumber = "Maint port number";

		public const string kServer = "Maint Server";

		public const string kDatabaseName = "Maint database name";

		public const string kProjectName = "Maint project name";

		public const int kDefaultServerPort = 10733;

		public static ASMainWindow asMainWin;

		private static string s_TestingConflictResClass;

		private static string s_TestingConflictResFunction;

		public static ASMainWindow ASWin
		{
			get
			{
				return (!(ASEditorBackend.asMainWin != null)) ? EditorWindow.GetWindowDontShow<ASMainWindow>() : ASEditorBackend.asMainWin;
			}
		}

		public static void DoAS()
		{
			if (!ASEditorBackend.ASWin.Error)
			{
				ASEditorBackend.ASWin.Show();
				ASEditorBackend.ASWin.Focus();
			}
		}

		public static void ShowASConflictResolutionsWindow(string[] conflicting)
		{
			ASEditorBackend.ASWin.ShowConflictResolutions(conflicting);
		}

		public static void CommitItemsChanged()
		{
			if (ASEditorBackend.asMainWin != null || (ASEditorBackend.asMainWin == null && Resources.FindObjectsOfTypeAll(typeof(ASMainWindow)).Length != 0))
			{
				ASEditorBackend.ASWin.CommitItemsChanged();
			}
		}

		public static void CBReinitCommitWindow(int actionResult)
		{
			if (ASEditorBackend.ASWin.asCommitWin != null)
			{
				ASEditorBackend.ASWin.asCommitWin.Reinit(actionResult != 0);
			}
		}

		public static void CBCommitFinished(int actionResult)
		{
			if (ASEditorBackend.ASWin.asCommitWin != null)
			{
				ASEditorBackend.ASWin.asCommitWin.CommitFinished(actionResult != 0);
			}
		}

		public static void CBOverviewsCommitFinished(int actionResult)
		{
			if (ASEditorBackend.ASWin != null)
			{
				ASEditorBackend.ASWin.CommitFinished(actionResult != 0);
			}
		}

		public static void CBReinitOnSuccess(int actionResult)
		{
			if (actionResult != 0)
			{
				ASEditorBackend.ASWin.Reinit();
			}
			else
			{
				ASEditorBackend.ASWin.Repaint();
			}
		}

		public static void CBReinitASMainWindow()
		{
			ASEditorBackend.ASWin.Reinit();
		}

		public static void CBDoDiscardChanges(int actionResult)
		{
			ASEditorBackend.ASWin.DoDiscardChanges(actionResult != 0);
		}

		public static void CBInitUpdatePage(int actionResult)
		{
			ASEditorBackend.ASWin.InitUpdatePage(actionResult != 0);
		}

		public static void CBInitHistoryPage(int actionResult)
		{
			ASEditorBackend.ASWin.InitHistoryPage(actionResult != 0);
		}

		public static void CBInitOverviewPage(int actionResult)
		{
			ASEditorBackend.ASWin.InitOverviewPage(actionResult != 0);
		}

		public static bool SettingsIfNeeded()
		{
			return ASEditorBackend.InitializeMaintBinding();
		}

		public static bool SettingsAreValid()
		{
			PListConfig pListConfig = new PListConfig("Library/ServerPreferences.plist");
			string text = pListConfig["Maint UserName"];
			string text2 = pListConfig["Maint Server"];
			string text3 = pListConfig["Maint database name"];
			string text4 = pListConfig["Maint Timeout"];
			string text5 = pListConfig["Maint port number"];
			return text.Length != 0 && text2.Length != 0 && text3.Length != 0 && text4.Length != 0 && text5.Length != 0;
		}

		internal static string GetPassword(string server, string user)
		{
			string key = "ASPassword::" + server + "::" + user;
			return EditorPrefs.GetString(key, string.Empty);
		}

		internal static void SetPassword(string server, string user, string password)
		{
			string key = "ASPassword::" + server + "::" + user;
			EditorPrefs.SetString(key, password);
		}

		internal static void AddUser(string server, string user)
		{
			string key = "ASUser::" + server;
			EditorPrefs.SetString(key, user);
		}

		internal static string GetUser(string server)
		{
			string key = "ASUser::" + server;
			return EditorPrefs.GetString(key, string.Empty);
		}

		public static bool InitializeMaintBinding()
		{
			PListConfig pListConfig = new PListConfig("Library/ServerPreferences.plist");
			string text = pListConfig["Maint UserName"];
			string text2 = pListConfig["Maint Server"];
			string text3 = pListConfig["Maint project name"];
			string text4 = pListConfig["Maint database name"];
			string text5 = pListConfig["Maint port number"];
			int timeout;
			if (!int.TryParse(pListConfig["Maint Timeout"], out timeout))
			{
				timeout = 5;
			}
			if (text2.Length == 0 || text3.Length == 0 || text4.Length == 0 || text.Length == 0)
			{
				AssetServer.SetProjectName(string.Empty);
				return false;
			}
			AssetServer.SetProjectName(string.Format("{0} @ {1}", text3, text2));
			string connectionString = string.Concat(new string[]
			{
				"host='",
				text2,
				"' user='",
				text,
				"' password='",
				ASEditorBackend.GetPassword(text2, text),
				"' dbname='",
				text4,
				"' port='",
				text5,
				"' sslmode=disable ",
				pListConfig["Maint Connection Settings"]
			});
			AssetServer.Initialize(text, connectionString, timeout);
			return true;
		}

		public static void Testing_SetActionFinishedCallback(string klass, string name)
		{
			AssetServer.SaveString("s_TestingClass", klass);
			AssetServer.SaveString("s_TestingFunction", name);
			AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "Testing_DummyCallback");
		}

		private static void Testing_DummyCallback(bool success)
		{
			ASEditorBackend.Testing_Invoke(AssetServer.GetAndRemoveString("s_TestingClass"), AssetServer.GetAndRemoveString("s_TestingFunction"), new object[]
			{
				success
			});
		}

		private static void Testing_SetExceptionHandler(string exceptionHandlerClass, string exceptionHandlerFunction)
		{
			AssetServer.SaveString("s_ExceptionHandlerClass", exceptionHandlerClass);
			AssetServer.SaveString("s_ExceptionHandlerFunction", exceptionHandlerFunction);
		}

		private static void Testing_Invoke(string klass, string method, params object[] prms)
		{
			try
			{
				AppDomain currentDomain = AppDomain.CurrentDomain;
				Assembly[] assemblies = currentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					Assembly assembly = assemblies[i];
					if (assembly.GetName().Name != "UnityEditor" && assembly.GetName().Name != "UnityEngine")
					{
						Type[] typesFromAssembly = AssemblyHelper.GetTypesFromAssembly(assembly);
						for (int j = 0; j < typesFromAssembly.Length; j++)
						{
							Type type = typesFromAssembly[j];
							if (type.Name == klass)
							{
								type.InvokeMember(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, prms);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (GUIUtility.ShouldRethrowException(ex))
				{
					throw;
				}
				ASEditorBackend.Testing_Invoke(AssetServer.GetString("s_ExceptionHandlerClass"), AssetServer.GetString("s_ExceptionHandlerFunction"), new object[]
				{
					ex
				});
			}
		}

		public static void Testing_SetActiveDatabase(string host, int port, string projectName, string dbName, string user, string pwd)
		{
			PListConfig pListConfig = new PListConfig("Library/ServerPreferences.plist");
			pListConfig["Maint Server"] = host;
			pListConfig["Maint UserName"] = user;
			pListConfig["Maint database name"] = dbName;
			pListConfig["Maint port number"] = port.ToString();
			pListConfig["Maint project name"] = projectName;
			pListConfig["Maint Password"] = string.Empty;
			pListConfig["Maint settings type"] = "manual";
			pListConfig["Maint Timeout"] = "5";
			pListConfig["Maint Connection Settings"] = string.Empty;
			pListConfig.Save();
		}

		public static bool Testing_SetupDatabase(string host, int port, string adminUser, string adminPwd, string user, string pwd, string projectName)
		{
			AssetServer.AdminSetCredentials(host, port, adminUser, adminPwd);
			MaintDatabaseRecord[] array = AssetServer.AdminRefreshDatabases();
			if (array == null)
			{
				return false;
			}
			MaintDatabaseRecord[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				MaintDatabaseRecord maintDatabaseRecord = array2[i];
				if (maintDatabaseRecord.name == projectName)
				{
					AssetServer.AdminDeleteDB(projectName);
				}
			}
			if (AssetServer.AdminCreateDB(projectName) == 0)
			{
				return false;
			}
			string databaseName = AssetServer.GetDatabaseName(host, adminUser, adminPwd, port.ToString(), projectName);
			if (!AssetServer.AdminSetUserEnabled(databaseName, user, user, string.Empty, 1))
			{
				return false;
			}
			ASEditorBackend.Testing_SetActiveDatabase(host, port, projectName, databaseName, user, pwd);
			return true;
		}

		public static string[] Testing_GetAllDatabaseNames()
		{
			MaintDatabaseRecord[] array = AssetServer.AdminRefreshDatabases();
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = array[i].name;
			}
			return array2;
		}

		public static void Testing_SetConflictResolutionFunction(string klass, string fn)
		{
			ASEditorBackend.s_TestingConflictResClass = klass;
			ASEditorBackend.s_TestingConflictResFunction = fn;
		}

		public static void Testing_DummyConflictResolutionFunction(string[] conflicting)
		{
			ASEditorBackend.Testing_Invoke(ASEditorBackend.s_TestingConflictResClass, ASEditorBackend.s_TestingConflictResFunction, new object[]
			{
				conflicting
			});
		}
	}
}
