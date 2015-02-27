using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class UploadingBuildsMonitor : ScriptableObject
	{
		private class Content
		{
			public GUIContent m_ProgressBarText = EditorGUIUtility.TextContent("UploadingBuildsMonitor.ProgressBarText");
			public GUIContent m_NoSessionDialogHeader = EditorGUIUtility.TextContent("UploadingBuildsMonitor.NoSessionDialogHeader");
			public GUIContent m_NoSessionDialogText = EditorGUIUtility.TextContent("UploadingBuildsMonitor.NoSessionDialogText");
			public GUIContent m_NoSessionDialogButtonOK = EditorGUIUtility.TextContent("UploadingBuildsMonitor.NoSessionDialogButtonOK");
			public GUIContent m_OverwriteDialogHeader = EditorGUIUtility.TextContent("UploadingBuildsMonitor.OverwriteDialogHeader");
			public GUIContent m_OverwriteDialogText = EditorGUIUtility.TextContent("UploadingBuildsMonitor.OverwriteDialogText");
			public GUIContent m_OverwriteDialogButtonOK = EditorGUIUtility.TextContent("UploadingBuildsMonitor.OverwriteDialogButtonOK");
			public GUIContent m_OverwriteDialogButtonCancel = EditorGUIUtility.TextContent("UploadingBuildsMonitor.OverwriteDialogButtonCancel");
			public GUIContent m_OverwriteDialogButtonVersion = EditorGUIUtility.TextContent("UploadingBuildsMonitor.OverwriteDialogButtonVersion");
		}
		private static UploadingBuildsMonitor.Content s_Content;
		private static UploadingBuildsMonitor s_UploadingBuildsMonitor;
		private UploadingBuild[] m_UploadingBuilds = new UploadingBuild[0];
		private Dictionary<string, ValueSmoother> m_ProgressSmoothers = new Dictionary<string, ValueSmoother>();
		private bool m_DidInit;
		private UploadingBuildsMonitor()
		{
			if (UploadingBuildsMonitor.s_Content == null)
			{
				UploadingBuildsMonitor.s_Content = new UploadingBuildsMonitor.Content();
			}
		}
		public static void Activate()
		{
			if (UploadingBuildsMonitor.s_UploadingBuildsMonitor == null)
			{
				ScriptableObject.CreateInstance(typeof(UploadingBuildsMonitor));
			}
		}
		public static void Deactivate()
		{
			UnityEngine.Object.DestroyImmediate(UploadingBuildsMonitor.s_UploadingBuildsMonitor);
		}
		public static string GetActiveSessionID()
		{
			return (!string.IsNullOrEmpty(AssetStoreClient.ActiveSessionID)) ? (AssetStoreClient.ActiveSessionID + InternalEditorUtility.GetAuthToken()) : string.Empty;
		}
		public static void HandleNoSession(string displayName)
		{
			UploadingBuildsMonitor.Activate();
			AssetStoreLoginWindow.Login(UploadingBuildsMonitor.s_Content.m_NoSessionDialogText.text, delegate(string errorMessage)
			{
				if (string.IsNullOrEmpty(errorMessage))
				{
					UploadingBuildsUtility.ResumeBuildUpload(displayName);
				}
				else
				{
					UploadingBuildsUtility.RemoveUploadingBuild(displayName);
				}
			});
		}
		public static void OverwritePrompt(string displayName)
		{
			int num = EditorUtility.DisplayDialogComplex(UploadingBuildsMonitor.s_Content.m_OverwriteDialogHeader.text, UploadingBuildsMonitor.s_Content.m_OverwriteDialogText.text, UploadingBuildsMonitor.s_Content.m_OverwriteDialogButtonOK.text, UploadingBuildsMonitor.s_Content.m_OverwriteDialogButtonCancel.text, UploadingBuildsMonitor.s_Content.m_OverwriteDialogButtonVersion.text);
			if (num == 1)
			{
				UploadingBuildsUtility.RemoveUploadingBuild(displayName);
				AsyncProgressBar.Clear();
				UploadingBuildsMonitor.s_UploadingBuildsMonitor.SyncToState();
			}
			else
			{
				UploadingBuildsUtility.ResumeBuildUpload(displayName, num == 0);
			}
		}
		private void OnEnable()
		{
			UploadingBuildsMonitor.s_UploadingBuildsMonitor = this;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			this.SyncToState();
		}
		private void OnDisable()
		{
			UploadingBuildsMonitor.s_UploadingBuildsMonitor = null;
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}
		public static void InternalStateChanged()
		{
			if (UploadingBuildsMonitor.s_UploadingBuildsMonitor != null)
			{
				UploadingBuildsMonitor.s_UploadingBuildsMonitor.SyncToState();
			}
		}
		private void SyncToState()
		{
			this.m_UploadingBuilds = UploadingBuildsUtility.GetUploadingBuilds();
		}
		private ValueSmoother GetProgressSmoother(string url)
		{
			ValueSmoother valueSmoother;
			this.m_ProgressSmoothers.TryGetValue(url, out valueSmoother);
			if (valueSmoother == null)
			{
				valueSmoother = new ValueSmoother();
				this.m_ProgressSmoothers[url] = valueSmoother;
			}
			return valueSmoother;
		}
		private void UploadSmoothing()
		{
			if (this.m_ProgressSmoothers.Count > 0)
			{
				bool flag = false;
				List<string> list = null;
				foreach (KeyValuePair<string, ValueSmoother> current in this.m_ProgressSmoothers)
				{
					current.Value.Update();
					if (current.Value.GetSmoothValue() < 1f)
					{
						flag = true;
					}
					else
					{
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(current.Key);
					}
				}
				if (list != null)
				{
					foreach (string current2 in list)
					{
						this.m_ProgressSmoothers.Remove(current2);
					}
				}
				if (flag)
				{
					this.SyncToState();
				}
			}
		}
		private void Update()
		{
			if (!this.m_DidInit)
			{
				this.SyncToState();
				this.m_DidInit = true;
			}
			this.UploadSmoothing();
			if (this.m_UploadingBuilds.Length > 0)
			{
				this.UpdateBuild(ref this.m_UploadingBuilds[0]);
			}
			else
			{
				UploadingBuildsMonitor.Deactivate();
			}
		}
		private void UpdateBuild(ref UploadingBuild build)
		{
			switch (build.status)
			{
			case UploadingBuildStatus.Authorizing:
			case UploadingBuildStatus.Authorized:
				AsyncProgressBar.Display(UploadingBuildsMonitor.s_Content.m_ProgressBarText.text, 0f);
				break;
			case UploadingBuildStatus.Uploading:
			{
				ValueSmoother progressSmoother = this.GetProgressSmoother(build.url);
				progressSmoother.SetTargetValue(build.progress);
				AsyncProgressBar.Display(UploadingBuildsMonitor.s_Content.m_ProgressBarText.text, progressSmoother.GetSmoothValue());
				break;
			}
			case UploadingBuildStatus.Uploaded:
			case UploadingBuildStatus.UploadFailed:
				UploadingBuildsUtility.RemoveUploadingBuild(build.displayName);
				AsyncProgressBar.Clear();
				this.SyncToState();
				break;
			default:
				Debug.LogError("Unhandled enum");
				break;
			}
		}
	}
}
