using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor.Connect;
using UnityEditor.SceneManagement;
using UnityEditor.Web;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

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
			kCollabIgnored = 8uL,
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
			kCollabMerged = 262144uL,
			kCollabPendingMerge = 524288uL,
			kCollabFolderMetaFile = 1048576uL,
			KCollabContentChanged = 2097152uL,
			KCollabContentConflicted = 4194304uL,
			KCollabContentDeleted = 8388608uL,
			kCollabInvalidState = 1073741824uL,
			kAnyLocalChanged = 2384uL,
			kAnyLocalEdited = 2320uL
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
			[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectPath();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsConnected();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AnyJobRunning();

		[GeneratedByOldBindingsGenerator, ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool JobRunning(int a_jobID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Disconnect();

		public ProgressInfo GetJobProgress(int jobId)
		{
			return this.GetJobProgressByType(jobId);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProgressInfo GetJobProgressByType(int jobType);

		public void CancelJob(int jobId)
		{
			this.CancelJobByType(jobId);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CancelJobByType(int jobType);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetAssetStateInternal(string guid);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetSelectedAssetStateInternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Publish(string comment, [DefaultValue("false")] bool useSelectedAssets);

		[ExcludeFromDocs]
		public void Publish(string comment)
		{
			bool useSelectedAssets = false;
			this.Publish(comment, useSelectedAssets);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ValidateSelectiveCommit();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(string revisionID, bool updateToRevision);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RevertFile(string path, bool forceOverwrite);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Change[] GetCollabConflicts();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictResolvedMine(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictsResolvedMine(string[] paths);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictResolvedTheirs(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictsResolvedTheirs(string[] paths);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearConflictResolved(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearConflictsResolved(string[] paths);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LaunchConflictExternalMerge(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CheckConflictsResolvedExternal();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowConflictDifferences(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowDifferences(string path);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Collab.CollabStateID GetCollabState();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Change[] GetChangesToPublish();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResyncSnapshot();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GoBackToRevision(string revisionID, bool updateToRevision);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SendNotification();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResyncToRevision(string revisionID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetError(int filter, out int code, out int priority, out int behaviour, out string errorMsg, out string errorShortMsg);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetError(int errorCode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearError(int errorCode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearErrors();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCollabEnabledForCurrentProject(bool enabled);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsCollabEnabledForCurrentProject();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern SoftLock[] GetSoftLocks(string assetGuid);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Revision[] GetRevisions();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool AreTestsRunning();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTestsRunning(bool running);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearAllFailures();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void FailNextOperation(int operation, int code);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TimeOutNextOperation(int operation, int timeOutSec);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearNextOperationFailure();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void FailNextOperationForFile(string path, int operation, int code);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void TimeOutNextOperationForFile(string path, int operation, int timeOutSec);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearNextOperationFailureForFile(string path);

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

		public void CancelJobWithoutException(int jobType)
		{
			try
			{
				this.CancelJobByType(jobType);
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

		private static void PublishDialog(string changelist)
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				CollabPublishDialog collabPublishDialog = CollabPublishDialog.ShowCollabWindow(changelist);
				if (collabPublishDialog.Options.DoPublish)
				{
					Collab.instance.Publish(collabPublishDialog.Options.Comments, true);
				}
			}
		}

		private static void CannotPublishDialog(string infoMessage)
		{
			CollabCannotPublishDialog.ShowCollabWindow(infoMessage);
		}

		private static void OnUnityConnectStateChanged(ConnectInfo state)
		{
			Collab.instance.SendNotification();
		}
	}
}
