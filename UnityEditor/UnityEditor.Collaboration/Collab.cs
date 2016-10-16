using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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

		private static Collab s_Instance;

		private static bool s_IsFirstStateChange;

		public string[] currentProjectBrowserSelection;

		public static string[] clientType;

		internal static string editorPrefCollabClientType;

		public event StateChangedDelegate StateChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.StateChanged = (StateChangedDelegate)Delegate.Combine(this.StateChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.StateChanged = (StateChangedDelegate)Delegate.Remove(this.StateChanged, value);
			}
		}

		public extern CollabInfo collabInfo
		{
			[WrapperlessIcall]
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

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetProjectPath();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsConnected();

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool JobRunning(int a_jobID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Disconnect();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern ProgressInfo GetJobProgress(int jobID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CancelJob(int jobID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetAssetStateInternal(string guid);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetSelectedAssetStateInternal();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Publish(string comment);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(string revisionID, bool updateToRevision);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RevertFile(string path, bool forceOverwrite);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Change[] GetCollabConflicts();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictResolvedMine(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictsResolvedMine(string[] paths);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictResolvedTheirs(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool SetConflictsResolvedTheirs(string[] paths);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearConflictResolved(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool ClearConflictsResolved(string[] paths);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void LaunchConflictExternalMerge(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowConflictDifferences(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ShowDifferences(string path);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Change[] GetChangesToPublish();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResyncSnapshot();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GoBackToRevision(string revisionID, bool updateToRevision);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SendNotification();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ResyncToRevision(string revisionID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearErrors();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCollabEnabledForCurrentProject(bool enabled);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void OnPostprocessAssetbundleNameChanged(string assetPath, string previousAssetBundleName, string newAssetBundleName);

		public static string GetProjectClientType()
		{
			string configValue = EditorUserSettings.GetConfigValue(Collab.editorPrefCollabClientType);
			return (!string.IsNullOrEmpty(configValue)) ? configValue : Collab.clientType[0];
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

		private static void OnStateChanged()
		{
			if (Collab.s_IsFirstStateChange)
			{
				Collab.s_IsFirstStateChange = false;
				UnityConnect.instance.StateChanged += new UnityEditor.Connect.StateChangedDelegate(Collab.OnUnityConnectStateChanged);
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
