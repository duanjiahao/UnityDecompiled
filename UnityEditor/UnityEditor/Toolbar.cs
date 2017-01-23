using System;
using UnityEditor.Collaboration;
using UnityEditor.Connect;
using UnityEditor.Web;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class Toolbar : GUIView
	{
		private enum CollabToolbarState
		{
			NeedToEnableCollab,
			UpToDate,
			Conflict,
			OperationError,
			ServerHasChanges,
			FilesToPush,
			InProgress,
			Disabled,
			Offline
		}

		private static class Styles
		{
			public static readonly GUIStyle collabButtonStyle = new GUIStyle("Dropdown")
			{
				padding = 
				{
					left = 24
				}
			};

			public static readonly GUIStyle dropdown = "Dropdown";

			public static readonly GUIStyle appToolbar = "AppToolbar";
		}

		private static GUIContent[] s_ToolIcons;

		private static GUIContent[] s_ViewToolIcons;

		private static GUIContent[] s_PivotIcons;

		private static GUIContent[] s_PivotRotation;

		private static GUIContent s_LayerContent;

		private static GUIContent[] s_PlayIcons;

		private static GUIContent s_AccountContent;

		private static GUIContent s_CloudIcon;

		private Toolbar.CollabToolbarState m_CollabToolbarState = Toolbar.CollabToolbarState.UpToDate;

		private static GUIContent[] s_CollabIcons;

		private const float kCollabButtonWidth = 78f;

		private ButtonWithAnimatedIconRotation m_CollabButton;

		private string m_DynamicTooltip;

		private static bool m_ShowCollabTooltip = false;

		private static GUIContent[] s_ShownToolIcons = new GUIContent[5];

		public static Toolbar get = null;

		public static bool requestShowCollabToolbar = false;

		[SerializeField]
		private string m_LastLoadedLayoutName;

		private GUIContent currentCollabContent
		{
			get
			{
				GUIContent gUIContent = new GUIContent(Toolbar.s_CollabIcons[(int)this.m_CollabToolbarState]);
				if (!Toolbar.m_ShowCollabTooltip)
				{
					gUIContent.tooltip = null;
				}
				else if (this.m_DynamicTooltip != "")
				{
					gUIContent.tooltip = this.m_DynamicTooltip;
				}
				return gUIContent;
			}
		}

		internal static string lastLoadedLayoutName
		{
			get
			{
				return (!string.IsNullOrEmpty(Toolbar.get.m_LastLoadedLayoutName)) ? Toolbar.get.m_LastLoadedLayoutName : "Layout";
			}
			set
			{
				Toolbar.get.m_LastLoadedLayoutName = value;
				Toolbar.get.Repaint();
			}
		}

		private void InitializeToolIcons()
		{
			if (Toolbar.s_ToolIcons == null)
			{
				Toolbar.s_ToolIcons = new GUIContent[]
				{
					EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects."),
					EditorGUIUtility.IconContent("RotateTool", "|Rotate the selected objects."),
					EditorGUIUtility.IconContent("ScaleTool", "|Scale the selected objects."),
					EditorGUIUtility.IconContent("RectTool"),
					EditorGUIUtility.IconContent("MoveTool On"),
					EditorGUIUtility.IconContent("RotateTool On"),
					EditorGUIUtility.IconContent("ScaleTool On"),
					EditorGUIUtility.IconContent("RectTool On")
				};
				Toolbar.s_ViewToolIcons = new GUIContent[]
				{
					EditorGUIUtility.IconContent("ViewToolOrbit", "|Orbit the Scene view."),
					EditorGUIUtility.IconContent("ViewToolMove"),
					EditorGUIUtility.IconContent("ViewToolZoom"),
					EditorGUIUtility.IconContent("ViewToolOrbit", "|Orbit the Scene view."),
					EditorGUIUtility.IconContent("ViewToolOrbit On"),
					EditorGUIUtility.IconContent("ViewToolMove On"),
					EditorGUIUtility.IconContent("ViewToolZoom On"),
					EditorGUIUtility.IconContent("ViewToolOrbit On")
				};
				Toolbar.s_PivotIcons = new GUIContent[]
				{
					EditorGUIUtility.TextContentWithIcon("Center|The tool handle is placed at the center of the selection.", "ToolHandleCenter"),
					EditorGUIUtility.TextContentWithIcon("Pivot|The tool handle is placed at the active object's pivot point.", "ToolHandlePivot")
				};
				Toolbar.s_PivotRotation = new GUIContent[]
				{
					EditorGUIUtility.TextContentWithIcon("Local|Tool handles are in active object's rotation.", "ToolHandleLocal"),
					EditorGUIUtility.TextContentWithIcon("Global|Tool handles are in global rotation.", "ToolHandleGlobal")
				};
				Toolbar.s_LayerContent = EditorGUIUtility.TextContent("Layers|Which layers are visible in the Scene views.");
				Toolbar.s_PlayIcons = new GUIContent[]
				{
					EditorGUIUtility.IconContent("PlayButton"),
					EditorGUIUtility.IconContent("PauseButton"),
					EditorGUIUtility.IconContent("StepButton"),
					EditorGUIUtility.IconContent("PlayButtonProfile"),
					EditorGUIUtility.IconContent("PlayButton On"),
					EditorGUIUtility.IconContent("PauseButton On"),
					EditorGUIUtility.IconContent("StepButton On"),
					EditorGUIUtility.IconContent("PlayButtonProfile On"),
					EditorGUIUtility.IconContent("PlayButton Anim"),
					EditorGUIUtility.IconContent("PauseButton Anim"),
					EditorGUIUtility.IconContent("StepButton Anim"),
					EditorGUIUtility.IconContent("PlayButtonProfile Anim")
				};
				Toolbar.s_CloudIcon = EditorGUIUtility.IconContent("CloudConnect");
				Toolbar.s_AccountContent = new GUIContent("Account");
				Toolbar.s_CollabIcons = new GUIContent[]
				{
					EditorGUIUtility.TextContentWithIcon("Collab| You need to enable collab.", "CollabNew"),
					EditorGUIUtility.TextContentWithIcon("Collab| You are up to date.", "Collab"),
					EditorGUIUtility.TextContentWithIcon("Collab| Please fix your conflicts prior to publishing.", "CollabConflict"),
					EditorGUIUtility.TextContentWithIcon("Collab| Last operation failed. Please retry later.", "CollabError"),
					EditorGUIUtility.TextContentWithIcon("Collab| Please update, there are server changes.", "CollabPull"),
					EditorGUIUtility.TextContentWithIcon("Collab| You have files to publish.", "CollabPush"),
					EditorGUIUtility.TextContentWithIcon("Collab| Operation in progress.", "CollabProgress"),
					EditorGUIUtility.TextContentWithIcon("Collab| Collab is disabled.", "CollabNew"),
					EditorGUIUtility.TextContentWithIcon("Collab| Please check your network connection.", "CollabOffline")
				};
			}
		}

		public void OnEnable()
		{
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.Repaint));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
			UnityConnect.instance.StateChanged += new UnityEditor.Connect.StateChangedDelegate(this.OnUnityConnectStateChanged);
			UnityConnect.instance.UserStateChanged += new UserStateChangedDelegate(this.OnUnityConnectUserStateChanged);
			Toolbar.get = this;
			Collab.instance.StateChanged += new UnityEditor.Collaboration.StateChangedDelegate(this.OnCollabStateChanged);
			if (this.m_CollabButton == null)
			{
				this.m_CollabButton = new ButtonWithAnimatedIconRotation(() => (float)EditorApplication.timeSinceStartup * 500f, new Action(base.Repaint), 20f, true);
			}
		}

		public void OnDisable()
		{
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(base.Repaint));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
			UnityConnect.instance.StateChanged -= new UnityEditor.Connect.StateChangedDelegate(this.OnUnityConnectStateChanged);
			UnityConnect.instance.UserStateChanged -= new UserStateChangedDelegate(this.OnUnityConnectUserStateChanged);
			Collab.instance.StateChanged -= new UnityEditor.Collaboration.StateChangedDelegate(this.OnCollabStateChanged);
			if (this.m_CollabButton != null)
			{
				this.m_CollabButton.Clear();
			}
		}

		protected override bool OnFocus()
		{
			return false;
		}

		private void OnSelectionChange()
		{
			Tools.OnSelectionChange();
			base.Repaint();
		}

		protected void OnUnityConnectStateChanged(ConnectInfo state)
		{
			this.UpdateCollabToolbarState();
			Toolbar.RepaintToolbar();
		}

		protected void OnUnityConnectUserStateChanged(UserInfo state)
		{
			this.UpdateCollabToolbarState();
		}

		private Rect GetThinArea(Rect pos)
		{
			return new Rect(pos.x, 7f, pos.width, 18f);
		}

		private Rect GetThickArea(Rect pos)
		{
			return new Rect(pos.x, 5f, pos.width, 24f);
		}

		private void ReserveWidthLeft(float width, ref Rect pos)
		{
			pos.x -= width;
			pos.width = width;
		}

		private void ReserveWidthRight(float width, ref Rect pos)
		{
			pos.x += pos.width;
			pos.width = width;
		}

		private void ReserveRight(float width, ref Rect pos)
		{
			pos.x += width;
		}

		private void ReserveBottom(float height, ref Rect pos)
		{
			pos.y += height;
		}

		private void OnGUI()
		{
			float width = 10f;
			float width2 = 20f;
			float num = 32f;
			float num2 = 64f;
			float width3 = 80f;
			this.InitializeToolIcons();
			bool isPlayingOrWillChangePlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
			if (isPlayingOrWillChangePlaymode)
			{
				GUI.color = HostView.kPlayModeDarken;
			}
			if (Event.current.type == EventType.Repaint)
			{
				Toolbar.Styles.appToolbar.Draw(new Rect(0f, 0f, base.position.width, base.position.height), false, false, false, false);
			}
			Rect pos = new Rect(0f, 0f, 0f, 0f);
			this.ReserveWidthRight(width, ref pos);
			this.ReserveWidthRight(num * 5f, ref pos);
			this.DoToolButtons(this.GetThickArea(pos));
			this.ReserveWidthRight(width2, ref pos);
			this.ReserveWidthRight(num2 * 2f, ref pos);
			this.DoPivotButtons(this.GetThinArea(pos));
			float num3 = 100f;
			pos = new Rect((float)Mathf.RoundToInt((base.position.width - num3) / 2f), 0f, 140f, 0f);
			GUILayout.BeginArea(this.GetThickArea(pos));
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.DoPlayButtons(isPlayingOrWillChangePlaymode);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			pos = new Rect(base.position.width, 0f, 0f, 0f);
			this.ReserveWidthLeft(width, ref pos);
			this.ReserveWidthLeft(width3, ref pos);
			this.DoLayoutDropDown(this.GetThinArea(pos));
			this.ReserveWidthLeft(width, ref pos);
			this.ReserveWidthLeft(width3, ref pos);
			this.DoLayersDropDown(this.GetThinArea(pos));
			this.ReserveWidthLeft(width2, ref pos);
			this.ReserveWidthLeft(width3, ref pos);
			if (EditorGUI.ButtonMouseDown(this.GetThinArea(pos), Toolbar.s_AccountContent, FocusType.Passive, Toolbar.Styles.dropdown))
			{
				this.ShowUserMenu(this.GetThinArea(pos));
			}
			this.ReserveWidthLeft(width, ref pos);
			this.ReserveWidthLeft(32f, ref pos);
			if (GUI.Button(this.GetThinArea(pos), Toolbar.s_CloudIcon))
			{
				UnityConnectServiceCollection.instance.ShowService("Hub", true);
			}
			if (UnityConnect.instance.userInfo.whitelisted)
			{
				this.ReserveWidthLeft(width, ref pos);
				this.ReserveWidthLeft(78f, ref pos);
				this.DoCollabDropDown(this.GetThinArea(pos));
			}
			EditorGUI.ShowRepaints();
			Highlighter.ControlHighlightGUI(this);
		}

		private void ShowUserMenu(Rect dropDownRect)
		{
			GenericMenu genericMenu = new GenericMenu();
			if (!UnityConnect.instance.online)
			{
				genericMenu.AddDisabledItem(new GUIContent("Go to account"));
				genericMenu.AddDisabledItem(new GUIContent("Sign in..."));
				if (!Application.HasProLicense())
				{
					genericMenu.AddSeparator("");
					genericMenu.AddDisabledItem(new GUIContent("Upgrade to Pro"));
				}
			}
			else
			{
				string accountUrl = UnityConnect.instance.GetConfigurationURL(CloudConfigUrl.CloudPortal);
				if (UnityConnect.instance.loggedIn)
				{
					genericMenu.AddItem(new GUIContent("Go to account"), false, delegate
					{
						UnityConnect.instance.OpenAuthorizedURLInWebBrowser(accountUrl);
					});
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent("Go to account"));
				}
				if (UnityConnect.instance.loggedIn)
				{
					string text = "Sign out " + UnityConnect.instance.userInfo.displayName;
					genericMenu.AddItem(new GUIContent(text), false, delegate
					{
						UnityConnect.instance.Logout();
					});
				}
				else
				{
					genericMenu.AddItem(new GUIContent("Sign in..."), false, delegate
					{
						UnityConnect.instance.ShowLogin();
					});
				}
				if (!Application.HasProLicense())
				{
					genericMenu.AddSeparator("");
					genericMenu.AddItem(new GUIContent("Upgrade to Pro"), false, delegate
					{
						Application.OpenURL("https://store.unity3d.com/");
					});
				}
			}
			genericMenu.DropDown(dropDownRect);
		}

		private void DoToolButtons(Rect rect)
		{
			GUI.changed = false;
			int num = (int)((!Tools.viewToolActive) ? Tools.current : Tool.View);
			for (int i = 1; i < 5; i++)
			{
				Toolbar.s_ShownToolIcons[i] = Toolbar.s_ToolIcons[i - 1 + ((i != num) ? 0 : 4)];
				Toolbar.s_ShownToolIcons[i].tooltip = Toolbar.s_ToolIcons[i - 1].tooltip;
			}
			Toolbar.s_ShownToolIcons[0] = Toolbar.s_ViewToolIcons[(int)(Tools.viewTool + ((num != 0) ? 0 : 4))];
			num = GUI.Toolbar(rect, num, Toolbar.s_ShownToolIcons, "Command");
			if (GUI.changed)
			{
				Tools.current = (Tool)num;
			}
		}

		private void DoPivotButtons(Rect rect)
		{
			Tools.pivotMode = (PivotMode)EditorGUI.CycleButton(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), (int)Tools.pivotMode, Toolbar.s_PivotIcons, "ButtonLeft");
			if (Tools.current == Tool.Scale && Selection.transforms.Length < 2)
			{
				GUI.enabled = false;
			}
			PivotRotation pivotRotation = (PivotRotation)EditorGUI.CycleButton(new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height), (int)Tools.pivotRotation, Toolbar.s_PivotRotation, "ButtonRight");
			if (Tools.pivotRotation != pivotRotation)
			{
				Tools.pivotRotation = pivotRotation;
				if (pivotRotation == PivotRotation.Global)
				{
					Tools.ResetGlobalHandleRotation();
				}
			}
			if (Tools.current == Tool.Scale)
			{
				GUI.enabled = true;
			}
			if (GUI.changed)
			{
				Tools.RepaintAllToolViews();
			}
		}

		private void DoPlayButtons(bool isOrWillEnterPlaymode)
		{
			bool isPlaying = EditorApplication.isPlaying;
			GUI.changed = false;
			int num = (!isPlaying) ? 0 : 4;
			if (AnimationMode.InAnimationMode())
			{
				num = 8;
			}
			Color color = GUI.color + new Color(0.01f, 0.01f, 0.01f, 0.01f);
			GUI.contentColor = new Color(1f / color.r, 1f / color.g, 1f / color.g, 1f / color.a);
			GUILayout.Toggle(isOrWillEnterPlaymode, Toolbar.s_PlayIcons[num], "CommandLeft", new GUILayoutOption[0]);
			GUI.backgroundColor = Color.white;
			if (GUI.changed)
			{
				Toolbar.TogglePlaying();
				GUIUtility.ExitGUI();
			}
			GUI.changed = false;
			bool isPaused = GUILayout.Toggle(EditorApplication.isPaused, Toolbar.s_PlayIcons[num + 1], "CommandMid", new GUILayoutOption[0]);
			if (GUI.changed)
			{
				EditorApplication.isPaused = isPaused;
				GUIUtility.ExitGUI();
			}
			if (GUILayout.Button(Toolbar.s_PlayIcons[num + 2], "CommandRight", new GUILayoutOption[0]))
			{
				EditorApplication.Step();
				GUIUtility.ExitGUI();
			}
		}

		private void DoLayersDropDown(Rect rect)
		{
			GUIStyle style = "DropDown";
			if (EditorGUI.ButtonMouseDown(rect, Toolbar.s_LayerContent, FocusType.Passive, style))
			{
				if (LayerVisibilityWindow.ShowAtPosition(rect))
				{
					GUIUtility.ExitGUI();
				}
			}
		}

		private void DoLayoutDropDown(Rect rect)
		{
			if (EditorGUI.ButtonMouseDown(rect, GUIContent.Temp(Toolbar.lastLoadedLayoutName), FocusType.Passive, "DropDown"))
			{
				Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
				rect.x = vector.x;
				rect.y = vector.y;
				EditorUtility.Internal_DisplayPopupMenu(rect, "Window/Layouts", this, 0);
			}
		}

		private void ShowPopup(Rect rect)
		{
			AssetDatabase.SaveAssets();
			this.ReserveRight(39f, ref rect);
			this.ReserveBottom(5f, ref rect);
			if (CollabToolbarWindow.ShowCenteredAtPosition(rect))
			{
				GUIUtility.ExitGUI();
			}
		}

		private void DoCollabDropDown(Rect rect)
		{
			this.UpdateCollabToolbarState();
			bool flag = Toolbar.requestShowCollabToolbar;
			Toolbar.requestShowCollabToolbar = false;
			bool whitelisted = Collab.instance.collabInfo.whitelisted;
			bool flag2 = whitelisted && !EditorApplication.isPlaying;
			using (new EditorGUI.DisabledScope(!flag2))
			{
				bool animate = this.m_CollabToolbarState == Toolbar.CollabToolbarState.InProgress;
				EditorGUIUtility.SetIconSize(new Vector2(12f, 12f));
				if (this.m_CollabButton.OnGUI(rect, this.currentCollabContent, animate, Toolbar.Styles.collabButtonStyle))
				{
					flag = true;
				}
				EditorGUIUtility.SetIconSize(Vector2.zero);
			}
			if (flag)
			{
				this.ShowPopup(rect);
			}
		}

		public void OnCollabStateChanged(CollabInfo info)
		{
			this.UpdateCollabToolbarState();
		}

		public void UpdateCollabToolbarState()
		{
			if (Collab.instance.GetCollabInfo().whitelisted)
			{
				Toolbar.CollabToolbarState collabToolbarState = Toolbar.CollabToolbarState.UpToDate;
				bool flag = UnityConnect.instance.connectInfo.online && UnityConnect.instance.connectInfo.loggedIn;
				this.m_DynamicTooltip = "";
				if (flag)
				{
					Collab instance = Collab.instance;
					bool flag2 = instance.JobRunning(0);
					CollabInfo collabInfo = instance.collabInfo;
					if (!collabInfo.ready)
					{
						collabToolbarState = Toolbar.CollabToolbarState.InProgress;
					}
					else if (collabInfo.error)
					{
						collabToolbarState = Toolbar.CollabToolbarState.OperationError;
						this.m_DynamicTooltip = "Last operation failed. " + collabInfo.lastErrorMsg;
					}
					else if (flag2)
					{
						collabToolbarState = Toolbar.CollabToolbarState.InProgress;
					}
					else
					{
						bool flag3 = CollabAccess.Instance.IsServiceEnabled();
						if (!UnityConnect.instance.projectInfo.projectBound || !flag3)
						{
							collabToolbarState = Toolbar.CollabToolbarState.NeedToEnableCollab;
						}
						else if (collabInfo.update)
						{
							collabToolbarState = Toolbar.CollabToolbarState.ServerHasChanges;
						}
						else if (collabInfo.conflict)
						{
							collabToolbarState = Toolbar.CollabToolbarState.Conflict;
						}
						else if (collabInfo.publish)
						{
							collabToolbarState = Toolbar.CollabToolbarState.FilesToPush;
						}
					}
				}
				else
				{
					collabToolbarState = Toolbar.CollabToolbarState.Offline;
				}
				if (collabToolbarState != this.m_CollabToolbarState || CollabToolbarWindow.s_ToolbarIsVisible == Toolbar.m_ShowCollabTooltip)
				{
					this.m_CollabToolbarState = collabToolbarState;
					Toolbar.m_ShowCollabTooltip = !CollabToolbarWindow.s_ToolbarIsVisible;
					Toolbar.RepaintToolbar();
				}
			}
		}

		private static void InternalWillTogglePlaymode()
		{
			InternalEditorUtility.RepaintAllViews();
		}

		private static void TogglePlaying()
		{
			bool isPlaying = !EditorApplication.isPlaying;
			EditorApplication.isPlaying = isPlaying;
			Toolbar.InternalWillTogglePlaymode();
		}

		internal static void RepaintToolbar()
		{
			if (Toolbar.get != null)
			{
				Toolbar.get.Repaint();
			}
		}

		public float CalcHeight()
		{
			return 30f;
		}
	}
}
