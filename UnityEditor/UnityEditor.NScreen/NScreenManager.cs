using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.NScreen
{
	internal class NScreenManager : ScriptableSingleton<NScreenManager>
	{
		[SerializeField]
		private int m_LatestId = 0;

		[SerializeField]
		private bool m_BuildOnPlay = true;

		[SerializeField]
		private int m_SelectedSizeIndex;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache0;

		internal bool BuildOnPlay
		{
			get
			{
				return this.m_BuildOnPlay || !this.HasBuild;
			}
			set
			{
				this.m_BuildOnPlay = value;
			}
		}

		internal bool HasBuild
		{
			get
			{
				return Directory.Exists("Temp/NScreen/NScreen.app");
			}
		}

		internal int SelectedSizeIndex
		{
			get
			{
				return this.m_SelectedSizeIndex;
			}
			set
			{
				this.m_SelectedSizeIndex = value;
			}
		}

		static NScreenManager()
		{
			Delegate arg_23_0 = EditorApplication.playmodeStateChanged;
			if (NScreenManager.<>f__mg$cache0 == null)
			{
				NScreenManager.<>f__mg$cache0 = new EditorApplication.CallbackFunction(NScreenManager.PlayModeStateChanged);
			}
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(arg_23_0, NScreenManager.<>f__mg$cache0);
		}

		internal void ResetIds()
		{
			this.m_LatestId = 0;
		}

		internal int GetNewId()
		{
			return ++this.m_LatestId;
		}

		internal static void PlayModeStateChanged()
		{
			if (!EditorApplication.isPaused)
			{
				if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode && Resources.FindObjectsOfTypeAll<RemoteGame>().Length > 0 && ScriptableSingleton<NScreenManager>.instance.BuildOnPlay)
				{
					NScreenManager.Build();
				}
				if (EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
				{
					NScreenManager.StartAll();
				}
				else if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
				{
					NScreenManager.StopAll();
				}
				else if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
				{
					NScreenManager.RepaintAllGameViews();
				}
			}
		}

		internal static void Init()
		{
			RemoteGame remoteGame = (RemoteGame)EditorWindow.GetWindow(typeof(RemoteGame));
			if (EditorApplication.isPlaying && !remoteGame.IsRunning())
			{
				remoteGame.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
				remoteGame.StartGame();
			}
		}

		internal static void OpenAnotherWindow()
		{
			RemoteGame remoteGame = ScriptableObject.CreateInstance<RemoteGame>();
			ContainerWindow[] windows = ContainerWindow.windows;
			for (int i = 0; i < windows.Length; i++)
			{
				ContainerWindow containerWindow = windows[i];
				View[] allChildren = containerWindow.rootView.allChildren;
				for (int j = 0; j < allChildren.Length; j++)
				{
					View view = allChildren[j];
					DockArea dockArea = view as DockArea;
					if (!(dockArea == null))
					{
						if (dockArea.m_Panes.Any((EditorWindow pane) => pane.GetType() == typeof(RemoteGame)))
						{
							dockArea.AddTab(remoteGame);
							break;
						}
					}
				}
			}
			remoteGame.Show();
			if (EditorApplication.isPlaying)
			{
				remoteGame.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
				remoteGame.StartGame();
			}
		}

		internal static void StartAll()
		{
			ScriptableSingleton<NScreenManager>.instance.ResetIds();
			RemoteGame[] array = Resources.FindObjectsOfTypeAll<RemoteGame>();
			for (int i = 0; i < array.Length; i++)
			{
				RemoteGame remoteGame = array[i];
				remoteGame.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
				remoteGame.StartGame();
			}
		}

		internal static void StopAll()
		{
			RemoteGame[] array = Resources.FindObjectsOfTypeAll<RemoteGame>();
			for (int i = 0; i < array.Length; i++)
			{
				RemoteGame remoteGame = array[i];
				remoteGame.StopGame();
			}
		}

		internal static void RepaintAllGameViews()
		{
			RemoteGame[] array = Resources.FindObjectsOfTypeAll<RemoteGame>();
			for (int i = 0; i < array.Length; i++)
			{
				RemoteGame remoteGame = array[i];
				remoteGame.Repaint();
				remoteGame.GameViewAspectWasChanged();
			}
		}

		internal static void Build()
		{
			string[] array = new string[EditorBuildSettings.scenes.Length];
			int num = 0;
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				if (EditorBuildSettings.scenes[i].enabled)
				{
					array[num] = EditorBuildSettings.scenes[i].path;
					num++;
				}
			}
			Array.Resize<string>(ref array, num);
			Directory.CreateDirectory("Temp/NScreen");
			ResolutionDialogSetting displayResolutionDialog = PlayerSettings.displayResolutionDialog;
			bool runInBackground = PlayerSettings.runInBackground;
			bool defaultIsFullScreen = PlayerSettings.defaultIsFullScreen;
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
			PlayerSettings.runInBackground = true;
			PlayerSettings.defaultIsFullScreen = false;
			try
			{
				BuildPlayerOptions buildPlayerOptions = default(BuildPlayerOptions);
				buildPlayerOptions.scenes = array;
				buildPlayerOptions.options = BuildOptions.None;
				buildPlayerOptions.locationPathName = "Temp/NScreen/NScreen.app";
				if (IntPtr.Size == 4)
				{
					buildPlayerOptions.target = BuildTarget.StandaloneOSXIntel;
				}
				else
				{
					buildPlayerOptions.target = BuildTarget.StandaloneOSXIntel64;
				}
				BuildPipeline.BuildPlayer(buildPlayerOptions);
			}
			finally
			{
				PlayerSettings.displayResolutionDialog = displayResolutionDialog;
				PlayerSettings.runInBackground = runInBackground;
				PlayerSettings.defaultIsFullScreen = defaultIsFullScreen;
			}
		}
	}
}
