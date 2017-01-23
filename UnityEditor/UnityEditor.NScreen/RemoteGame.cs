using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.NScreen
{
	internal class RemoteGame : EditorWindow, IHasCustomMenu, IGameViewSizeMenuUser
	{
		private const float kMaxWidth = 1280f;

		private const float kMaxHeight = 720f;

		private Process remoteProcess = null;

		public NScreenBridge bridge = null;

		public bool shouldExit = true;

		public bool shouldBuild = false;

		public int id;

		private int oldWidth;

		private int oldHeight;

		private Rect remoteViewRect;

		[CompilerGenerated]
		private static GenericMenu.MenuFunction <>f__mg$cache0;

		private int ToolBarHeight
		{
			get
			{
				return 17;
			}
		}

		public bool lowResolutionForAspectRatios
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public bool forceLowResolutionAspectRatios
		{
			get
			{
				return EditorGUIUtility.pixelsPerPoint == 1f;
			}
		}

		public bool showLowResolutionToggle
		{
			get
			{
				return false;
			}
		}

		internal void StartGame()
		{
			this.DoExitGame();
			base.wantsMouseMove = true;
			this.bridge = new NScreenBridge();
			this.bridge.InitServer(this.id);
			this.bridge.SetResolution((int)base.minSize.x, (int)base.minSize.y);
			this.remoteProcess = new Process();
			this.remoteProcess.EnableRaisingEvents = true;
			this.remoteProcess.Exited += new EventHandler(this.HandleExited);
			this.remoteProcess.StartInfo.FileName = "Temp/NScreen/NScreen.app/Contents/MacOS/NScreen";
			this.remoteProcess.StartInfo.Arguments = "-nscreenid " + this.id;
			this.remoteProcess.StartInfo.UseShellExecute = false;
			try
			{
				this.remoteProcess.Start();
			}
			catch (Win32Exception)
			{
				this.remoteProcess = null;
				this.DoExitGame();
			}
			this.bridge.StartWatchdogForPid(this.remoteProcess.Id);
			this.shouldExit = false;
		}

		internal bool IsRunning()
		{
			return !this.shouldExit;
		}

		internal void StopGame()
		{
			this.shouldExit = true;
		}

		private void Update()
		{
			if (this.shouldExit)
			{
				this.DoExitGame();
			}
			else if (this.bridge != null)
			{
				if (this.oldWidth != (int)base.position.width || this.oldHeight != (int)base.position.height)
				{
					int num = (int)Mathf.Clamp(base.position.width, base.minSize.x, 1280f);
					int num2 = (int)Mathf.Clamp(base.position.height, base.minSize.y, 720f);
					bool flag = true;
					this.remoteViewRect = GameViewSizes.GetConstrainedRect(new Rect(0f, 0f, (float)num, (float)num2), ScriptableSingleton<GameViewSizes>.instance.currentGroupType, ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex, out flag);
					this.remoteViewRect.y = this.remoteViewRect.y + (float)this.ToolBarHeight;
					this.remoteViewRect.height = this.remoteViewRect.height - (float)this.ToolBarHeight;
					this.bridge.SetResolution((int)this.remoteViewRect.width, (int)this.remoteViewRect.height);
					this.oldWidth = (int)base.position.width;
					this.oldHeight = (int)base.position.height;
				}
				this.bridge.Update();
				base.Repaint();
			}
			if (this.shouldBuild)
			{
				this.shouldBuild = false;
				NScreenManager.Build();
			}
		}

		public void SizeSelectionCallback(int indexClicked, object objectSelected)
		{
			if (indexClicked != ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex)
			{
				ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex = indexClicked;
				NScreenManager.RepaintAllGameViews();
			}
		}

		internal void GameViewAspectWasChanged()
		{
			this.oldWidth = 0;
			this.oldHeight = 0;
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUILayout.GameViewSizePopup(ScriptableSingleton<GameViewSizes>.instance.currentGroupType, ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex, this, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(160f)
			});
			GUILayout.FlexibleSpace();
			GUI.enabled = !Application.isPlaying;
			bool buildOnPlay = ScriptableSingleton<NScreenManager>.instance.BuildOnPlay;
			ScriptableSingleton<NScreenManager>.instance.BuildOnPlay = GUILayout.Toggle(ScriptableSingleton<NScreenManager>.instance.BuildOnPlay, "Build on Play", EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (buildOnPlay != ScriptableSingleton<NScreenManager>.instance.BuildOnPlay)
			{
				NScreenManager.RepaintAllGameViews();
			}
			if (GUILayout.Button("Build Now", EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.shouldBuild = true;
			}
			GUI.enabled = Application.isPlaying;
			GUILayout.EndHorizontal();
			if (!this.shouldExit && this.bridge != null)
			{
				Texture2D screenTexture = this.bridge.GetScreenTexture();
				if (screenTexture != null)
				{
					GUI.DrawTexture(this.remoteViewRect, screenTexture);
				}
				if (this == EditorWindow.focusedWindow)
				{
					this.bridge.SetInput((int)Event.current.mousePosition.x - (int)this.remoteViewRect.x, (int)base.position.height - (int)Event.current.mousePosition.y - (int)this.remoteViewRect.y + this.ToolBarHeight - (int)Mathf.Max(0f, base.position.height - 720f), Event.current.button, (int)((!Event.current.isKey) ? ((KeyCode)(-1)) : Event.current.keyCode), (int)Event.current.type);
				}
				else
				{
					this.bridge.ResetInput();
				}
			}
			else
			{
				GUILayout.Label("Game Stopped", new GUILayoutOption[0]);
			}
		}

		private void HandleExited(object sender, EventArgs e)
		{
			this.shouldExit = true;
		}

		internal void DoExitGame()
		{
			if (this.remoteProcess != null && !this.remoteProcess.HasExited)
			{
				this.remoteProcess.Kill();
				this.remoteProcess = null;
				base.Repaint();
			}
			if (this.bridge != null)
			{
				this.bridge.Shutdown();
				this.bridge = null;
				this.oldWidth = 0;
				this.oldHeight = 0;
			}
			base.wantsMouseMove = false;
			this.shouldExit = true;
		}

		private void OnEnable()
		{
			base.titleContent = new GUIContent("Remote Game");
		}

		private void OnDestroy()
		{
			this.DoExitGame();
		}

		public void AddItemsToMenu(GenericMenu menu)
		{
			GUIContent arg_2A_1 = new GUIContent("Add Tab/Remote Game");
			bool arg_2A_2 = false;
			if (RemoteGame.<>f__mg$cache0 == null)
			{
				RemoteGame.<>f__mg$cache0 = new GenericMenu.MenuFunction(NScreenManager.OpenAnotherWindow);
			}
			menu.AddItem(arg_2A_1, arg_2A_2, RemoteGame.<>f__mg$cache0);
		}
	}
}
