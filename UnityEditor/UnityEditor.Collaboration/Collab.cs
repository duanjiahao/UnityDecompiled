using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Connect;
using UnityEditor.Web;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Collaboration
{
	[InitializeOnLoad]
	internal sealed class Collab : AssetPostprocessor
	{
		[Flags]
		public enum CollabStates : ulong
		{
			kCollabNone = 0uL,
			kCollabLocal = 1uL,
			kCollabSynced = 2uL,
			kCollabOutOfSync = 4uL,
			kCollabMissing = 8uL,
			kCollabCheckedOutLocal = 16uL,
			kCollabCheckedOutRemote = 32uL,
			kCollabDeletedLocal = 64uL,
			kCollabDeletedRemote = 128uL,
			kCollabAddedLocal = 256uL,
			kCollabAddedRemote = 512uL,
			kCollabConflicted = 1024uL,
			kCollabMovedLocal = 2048uL,
			kCollabMovedRemote = 4096uL,
			kCollabUpdating = 8192uL,
			kCollabReadOnly = 16384uL,
			kCollabMetaFile = 32768uL,
			kCollabUseMine = 65536uL,
			kCollabUseTheir = 131072uL,
			kCollabChanges = 262144uL,
			kCollabMerged = 524288uL,
			kCollabPendingMerge = 1048576uL,
			kCollabFolderMetaFile = 2097152uL,
			kCollabInvalidState = 4194304uL
		}

		internal enum CollabStateID
		{
			None,
			Uninitialized,
			Initialized
		}

		private static Collab s_Instance;

		private static bool s_IsFirstStateChange;

		public string[] currentProjectBrowserSelection;

		public static string[] clientType;

		internal static string editorPrefCollabClientType;

		[CompilerGenerated]
		private static UnityEditor.Connect.StateChangedDelegate <>f__mg$cache0;

		public event StateChangedDelegate StateChanged
		{
			add
			{
				StateChangedDelegate stateChangedDelegate = this.StateChanged;
				StateChangedDelegate stateChangedDelegate2;
				do
				{
					stateChangedDelegate2 = stateChangedDelegate;
					stateChangedDelegate = Interlocked.CompareExchange<StateChangedDelegate>(ref this.StateChanged, (StateChangedDelegate)Delegate.Combine(stateChangedDelegate2, value), stateChangedDelegate);
				}
				while (stateChangedDelegate != stateChangedDelegate2);
			}
			remove
			{
				StateChangedDelegate stateChangedDelegate = this.StateChanged;
				StateChangedDelegate stateChangedDelegate2;
				do
				{
					stateChangedDelegate2 = stateChangedDelegate;
					stateChangedDelegate = Interlocked.CompareExchange<StateChangedDelegate>(ref this.StateChanged, (StateChangedDelegate)Delegate.Remove(stateChangedDelegate2, value), stateChangedDelegate);
				}
				while (stateChangedDelegate != stateChangedDelegate2);
			}
		}

		public extern CollabInfo collabInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public string projectBrowserSingleSelectionPath
		{
			get;
			set;
		}

		public string projectBrowserSingleMetaSelectionPath
		{
			get;
			set;
		}

		public static Collab instance
		{
			get
			{
				return Collab.s_Instance;
			}
		}

		static Collab()
		{
			Collab.s_IsFirstStateChange = true;
			Collab.clientType = new string[]
			{
				"Cloud Server",
				"Mock Server"
			};
			Collab.editorPrefCollabClientType = "CollabConfig_Client";
			Collab.s_Instance = new Collab();
			Collab.s_Instance.projectBrowserSingleSelectionPath = string.Empty;
			Collab.s_Instance.projectBrowserSingleMetaSelectionPath = string.Empty;
			JSProxyMgr.GetInstance().AddGlobalObject("unity/collab", Collab.s_Instance);
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectPath();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsConnected();

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool JobRunning(int a_jobID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Disconnect();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProgressInfo GetJobProgress(int jobID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CancelJob(int jobID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetAssetStateInternal(string guid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetSelectedAssetStateInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Publish(string comment);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(string revisionID, bool updateToRevision);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RevertFile(string path, bool forceOverwrite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Change[] GetCollabConflicts();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictResolvedMine(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictsResolvedMine(string[] paths);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictResolvedTheirs(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictsResolvedTheirs(string[] paths);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearConflictResolved(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearConflictsResolved(string[] paths);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LaunchConflictExternalMerge(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowConflictDifferences(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowDifferences(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Collab.CollabStateID GetCollabState();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Change[] GetChangesToPublish();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResyncSnapshot();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GoBackToRevision(string revisionID, bool updateToRevision);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SendNotification();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResyncToRevision(string revisionID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearErrors();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCollabEnabledForCurrentProject(bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern SoftLock[] GetSoftLocks(string assetGuid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool WasWhitelistedRequestSent();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Revision[] GetRevisions();

		public static string GetProjectClientType()
		{
			string configValue = EditorUserSettings.GetConfigValue(Collab.editorPrefCollabClientType);
			return (!string.IsNullOrEmpty(configValue)) ? configValue : Collab.clientType[0];
		}

		[MenuItem("Window/Collab/Get Revisions", false, 1000, true)]
		public static void TestGetRevisions()
		{
			Revision[] revisions = Collab.instance.GetRevisions();
			if (revisions.Length == 0)
			{
				Debug.Log("No revisions");
			}
			else
			{
				int num = revisions.Length;
				Revision[] array = revisions;
				for (int i = 0; i < array.Length; i++)
				{
					Revision revision = array[i];
					Debug.Log(string.Concat(new object[]
					{
						"Revision #",
						num,
						": ",
						revision.revisionID
					}));
					num--;
				}
			}
		}

		public void CancelJobWithoutException(int jobId)
		{
			try
			{
				this.CancelJob(jobId);
			}
			catch (Exception ex)
			{
				Debug.Log("Cannot cancel job, reason:" + ex.Message);
			}
		}

		public Collab.CollabStates GetAssetState(string guid)
		{
			return (Collab.CollabStates)this.GetAssetStateInternal(guid);
		}

		public Collab.CollabStates GetSelectedAssetState()
		{
			return (Collab.CollabStates)this.GetSelectedAssetStateInternal();
		}

		public void UpdateEditorSelectionCache()
		{
			List<string> list = new List<string>();
			string[] assetGUIDsDeepSelection = Selection.assetGUIDsDeepSelection;
			for (int i = 0; i < assetGUIDsDeepSelection.Length; i++)
			{
				string guid = assetGUIDsDeepSelection[i];
				string text = AssetDatabase.GUIDToAssetPath(guid);
				list.Add(text);
				string text2 = text + ".meta";
				if (File.Exists(text2))
				{
					list.Add(text2);
				}
			}
			this.currentProjectBrowserSelection = list.ToArray();
		}

		public CollabInfo GetCollabInfo()
		{
			return this.collabInfo;
		}

		public static bool IsDiffToolsAvailable()
		{
			return InternalEditorUtility.GetAvailableDiffTools().Length > 0;
		}

		public void SaveAssets()
		{
			AssetDatabase.SaveAssets();
		}

		public static void SwitchToDefaultMode()
		{
			bool flag = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;
			SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
			if (lastActiveSceneView != null && lastActiveSceneView.in2DMode != flag)
			{
				lastActiveSceneView.in2DMode = flag;
			}
		}

		private static void OnStateChanged()
		{
			if (Collab.s_IsFirstStateChange)
			{
				Collab.s_IsFirstStateChange = false;
				UnityConnect arg_34_0 = UnityConnect.instance;
				if (Collab.<>f__mg$cache0 == null)
				{
					Collab.<>f__mg$cache0 = new UnityEditor.Connect.StateChangedDelegate(Collab.OnUnityConnectStateChanged);
				}
				arg_34_0.StateChanged += Collab.<>f__mg$cache0;
			}
			StateChangedDelegate stateChanged = Collab.instance.StateChanged;
			if (stateChanged != null)
			{
				stateChanged(Collab.instance.collabInfo);
			}
		}

		private static void OnUnityConnectStateChanged(ConnectInfo state)
		{
			Collab.instance.SendNotification();
		}
	}
}
