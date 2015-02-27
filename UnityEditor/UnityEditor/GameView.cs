using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace UnityEditor
{
	internal class GameView : EditorWindow
	{
		private const int kToolbarHeight = 17;
		private const int kBorderSize = 5;
		[SerializeField]
		private bool m_MaximizeOnPlay;
		[SerializeField]
		private bool m_Gizmos;
		[SerializeField]
		private bool m_Stats;
		[SerializeField]
		private int[] m_SelectedSizes = new int[0];
		private int m_SizeChangeID = -2147483648;
		private GUIContent gizmosContent = new GUIContent("Gizmos");
		private static GUIStyle s_GizmoButtonStyle;
		private static GUIStyle s_ResolutionWarningStyle;
		private static List<GameView> s_GameViews = new List<GameView>();
		private static GameView s_LastFocusedGameView = null;
		private static Rect s_MainGameViewRect = new Rect(0f, 0f, 640f, 480f);
		private Vector2 m_ShownResolution = Vector2.zero;
		private AnimBool m_ResolutionTooLargeWarning = new AnimBool(false);
		public bool maximizeOnPlay
		{
			get
			{
				return this.m_MaximizeOnPlay;
			}
			set
			{
				this.m_MaximizeOnPlay = value;
			}
		}
		private int selectedSizeIndex
		{
			get
			{
				return this.m_SelectedSizes[(int)GameView.currentSizeGroupType];
			}
			set
			{
				this.m_SelectedSizes[(int)GameView.currentSizeGroupType] = value;
			}
		}
		private static GameViewSizeGroupType currentSizeGroupType
		{
			get
			{
				return ScriptableSingleton<GameViewSizes>.instance.currentGroupType;
			}
		}
		private GameViewSize currentGameViewSize
		{
			get
			{
				return ScriptableSingleton<GameViewSizes>.instance.currentGroup.GetGameViewSize(this.selectedSizeIndex);
			}
		}
		private Rect gameViewRenderRect
		{
			get
			{
				return new Rect(0f, 17f, base.position.width, base.position.height - 17f);
			}
		}
		public GameView()
		{
			base.depthBufferBits = 32;
			base.antiAlias = -1;
			base.autoRepaintOnSceneChange = true;
		}
		public void OnValidate()
		{
			this.EnsureSelectedSizeAreValid();
		}
		public void OnEnable()
		{
			this.EnsureSelectedSizeAreValid();
			base.dontClearBackground = true;
			GameView.s_GameViews.Add(this);
			this.m_ResolutionTooLargeWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ResolutionTooLargeWarning.speed = 0.3f;
		}
		public void OnDisable()
		{
			GameView.s_GameViews.Remove(this);
			this.m_ResolutionTooLargeWarning.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoDelayedGameViewChanged));
		}
		internal static GameView GetMainGameView()
		{
			if (GameView.s_LastFocusedGameView == null && GameView.s_GameViews != null && GameView.s_GameViews.Count > 0)
			{
				GameView.s_LastFocusedGameView = GameView.s_GameViews[0];
			}
			return GameView.s_LastFocusedGameView;
		}
		public static void RepaintAll()
		{
			if (GameView.s_GameViews == null)
			{
				return;
			}
			foreach (GameView current in GameView.s_GameViews)
			{
				current.Repaint();
			}
		}
		internal static Vector2 GetSizeOfMainGameView()
		{
			Rect mainGameViewRenderRect = GameView.GetMainGameViewRenderRect();
			return new Vector2(mainGameViewRenderRect.width, mainGameViewRenderRect.height);
		}
		internal static Rect GetMainGameViewRenderRect()
		{
			GameView mainGameView = GameView.GetMainGameView();
			if (mainGameView != null)
			{
				GameView.s_MainGameViewRect = mainGameView.GetConstrainedGameViewRenderRect();
			}
			return GameView.s_MainGameViewRect;
		}
		private void GameViewAspectWasChanged()
		{
			base.SetInternalGameViewRect(GameView.GetConstrainedGameViewRenderRect(this.gameViewRenderRect, this.selectedSizeIndex));
			EditorApplication.SetSceneRepaintDirty();
		}
		private void OnFocus()
		{
			GameView.s_LastFocusedGameView = this;
			InternalEditorUtility.OnGameViewFocus(true);
		}
		private void OnLostFocus()
		{
			if (!EditorApplicationLayout.IsInitializingPlaymodeLayout())
			{
				Unsupported.SetAllowCursorLock(false);
				Unsupported.SetAllowCursorHide(false);
			}
			InternalEditorUtility.OnGameViewFocus(false);
		}
		private void DelayedGameViewChanged()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoDelayedGameViewChanged));
		}
		private void DoDelayedGameViewChanged()
		{
			this.GameViewAspectWasChanged();
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoDelayedGameViewChanged));
		}
		internal override void OnResized()
		{
			this.DelayedGameViewChanged();
		}
		private void EnsureSelectedSizeAreValid()
		{
			int num = Enum.GetNames(typeof(GameViewSizeGroupType)).Length;
			if (this.m_SelectedSizes.Length != num)
			{
				Array.Resize<int>(ref this.m_SelectedSizes, num);
			}
			IEnumerator enumerator = Enum.GetValues(typeof(GameViewSizeGroupType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GameViewSizeGroupType gameViewSizeGroupType = (GameViewSizeGroupType)((int)enumerator.Current);
					GameViewSizeGroup group = ScriptableSingleton<GameViewSizes>.instance.GetGroup(gameViewSizeGroupType);
					int num2 = (int)gameViewSizeGroupType;
					this.m_SelectedSizes[num2] = Mathf.Clamp(this.m_SelectedSizes[num2], 0, group.GetTotalCount() - 1);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		public bool IsShowingGizmos()
		{
			return this.m_Gizmos;
		}
		private void OnSelectionChange()
		{
			if (this.m_Gizmos)
			{
				base.Repaint();
			}
		}
		internal static Rect GetConstrainedGameViewRenderRect(Rect renderRect, int sizeIndex)
		{
			bool flag;
			return GameView.GetConstrainedGameViewRenderRect(renderRect, sizeIndex, out flag);
		}
		internal static Rect GetConstrainedGameViewRenderRect(Rect renderRect, int sizeIndex, out bool fitsInsideRect)
		{
			return GameViewSizes.GetConstrainedRect(renderRect, GameView.currentSizeGroupType, sizeIndex, out fitsInsideRect);
		}
		internal Rect GetConstrainedGameViewRenderRect()
		{
			if (this.m_Parent == null)
			{
				return GameView.s_MainGameViewRect;
			}
			this.m_Pos = this.m_Parent.borderSize.Remove(this.m_Parent.position);
			return GameView.GetConstrainedGameViewRenderRect(this.gameViewRenderRect, this.selectedSizeIndex);
		}
		private void SelectionCallback(int indexClicked, object objectSelected)
		{
			if (indexClicked != this.selectedSizeIndex)
			{
				this.selectedSizeIndex = indexClicked;
				base.dontClearBackground = true;
				this.GameViewAspectWasChanged();
			}
		}
		private void DoToolbarGUI()
		{
			ScriptableSingleton<GameViewSizes>.instance.RefreshStandaloneAndWebplayerDefaultSizes();
			if (ScriptableSingleton<GameViewSizes>.instance.GetChangeID() != this.m_SizeChangeID)
			{
				this.EnsureSelectedSizeAreValid();
				this.m_SizeChangeID = ScriptableSingleton<GameViewSizes>.instance.GetChangeID();
			}
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			EditorGUILayout.GameViewSizePopup(GameView.currentSizeGroupType, this.selectedSizeIndex, new Action<int, object>(this.SelectionCallback), EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(160f)
			});
			GUILayout.FlexibleSpace();
			this.m_MaximizeOnPlay = GUILayout.Toggle(this.m_MaximizeOnPlay, "Maximize on Play", EditorStyles.toolbarButton, new GUILayoutOption[0]);
			this.m_Stats = GUILayout.Toggle(this.m_Stats, "Stats", EditorStyles.toolbarButton, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(this.gizmosContent, GameView.s_GizmoButtonStyle);
			Rect position = new Rect(rect.xMax - (float)GameView.s_GizmoButtonStyle.border.right, rect.y, (float)GameView.s_GizmoButtonStyle.border.right, rect.height);
			if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				if (AnnotationWindow.ShowAtPosition(last, true))
				{
					GUIUtility.ExitGUI();
				}
			}
			this.m_Gizmos = GUI.Toggle(rect, this.m_Gizmos, this.gizmosContent, GameView.s_GizmoButtonStyle);
			GUILayout.EndHorizontal();
		}
		private void OnGUI()
		{
			if (GameView.s_GizmoButtonStyle == null)
			{
				GameView.s_GizmoButtonStyle = "GV Gizmo DropDown";
				GameView.s_ResolutionWarningStyle = new GUIStyle("PreOverlayLabel");
				GameView.s_ResolutionWarningStyle.alignment = TextAnchor.UpperLeft;
				GameView.s_ResolutionWarningStyle.padding = new RectOffset(6, 6, 1, 1);
			}
			this.DoToolbarGUI();
			Rect gameViewRenderRect = this.gameViewRenderRect;
			bool fitsInsideRect;
			Rect constrainedGameViewRenderRect = GameView.GetConstrainedGameViewRenderRect(gameViewRenderRect, this.selectedSizeIndex, out fitsInsideRect);
			Rect rect = GUIClip.Unclip(constrainedGameViewRenderRect);
			base.SetInternalGameViewRect(rect);
			EditorGUIUtility.AddCursorRect(constrainedGameViewRenderRect, MouseCursor.CustomCursor);
			EventType type = Event.current.type;
			if (type == EventType.MouseDown && gameViewRenderRect.Contains(Event.current.mousePosition))
			{
				Unsupported.SetAllowCursorLock(true);
				Unsupported.SetAllowCursorHide(true);
			}
			else
			{
				if (type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
				{
					Unsupported.SetAllowCursorLock(false);
				}
			}
			if (type == EventType.Repaint)
			{
				if (!this.currentGameViewSize.isFreeAspectRatio || !InternalEditorUtility.HasFullscreenCamera())
				{
					GUI.Box(gameViewRenderRect, GUIContent.none, "GameViewBackground");
				}
				Vector2 s_EditorScreenPointOffset = GUIUtility.s_EditorScreenPointOffset;
				GUIUtility.s_EditorScreenPointOffset = Vector2.zero;
				SavedGUIState savedGUIState = SavedGUIState.Create();
				EditorGUIUtility.RenderGameViewCameras(rect, this.m_Gizmos, true);
				savedGUIState.ApplyAndForget();
				GUIUtility.s_EditorScreenPointOffset = s_EditorScreenPointOffset;
			}
			else
			{
				if (type != EventType.Layout && type != EventType.Used)
				{
					if (WindowLayout.s_MaximizeKey.activated && (!EditorApplication.isPlaying || EditorApplication.isPaused))
					{
						return;
					}
					bool flag = constrainedGameViewRenderRect.Contains(Event.current.mousePosition);
					if (Event.current.rawType == EventType.MouseDown && !flag)
					{
						return;
					}
					Event.current.mousePosition = new Vector2(Event.current.mousePosition.x - constrainedGameViewRenderRect.x, Event.current.mousePosition.y - constrainedGameViewRenderRect.y);
					EditorGUIUtility.QueueGameViewInputEvent(Event.current);
					bool flag2 = true;
					if (Event.current.rawType == EventType.MouseUp && !flag)
					{
						flag2 = false;
					}
					if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
					{
						flag2 = false;
					}
					if (flag2)
					{
						Event.current.Use();
					}
					else
					{
						Event.current.mousePosition = new Vector2(Event.current.mousePosition.x + constrainedGameViewRenderRect.x, Event.current.mousePosition.y + constrainedGameViewRenderRect.y);
					}
				}
			}
			this.ShowResolutionWarning(new Rect(gameViewRenderRect.x, gameViewRenderRect.y, 200f, 20f), fitsInsideRect, constrainedGameViewRenderRect.size);
			if (this.m_Stats)
			{
				GameViewGUI.GameViewStatsGUI();
			}
		}
		private void ShowResolutionWarning(Rect position, bool fitsInsideRect, Vector2 shownSize)
		{
			if (!fitsInsideRect && shownSize != this.m_ShownResolution)
			{
				this.m_ShownResolution = shownSize;
				this.m_ResolutionTooLargeWarning.value = true;
			}
			if (fitsInsideRect && this.m_ShownResolution != Vector2.zero)
			{
				this.m_ShownResolution = Vector2.zero;
				this.m_ResolutionTooLargeWarning.value = false;
			}
			this.m_ResolutionTooLargeWarning.target = (!fitsInsideRect && !EditorApplication.isPlaying);
			if (this.m_ResolutionTooLargeWarning.faded > 0f)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(this.m_ResolutionTooLargeWarning.faded * 2f));
				EditorGUI.DropShadowLabel(position, string.Format("Using resolution {0}x{1}", shownSize.x, shownSize.y), GameView.s_ResolutionWarningStyle);
				GUI.color = color;
			}
		}
	}
}
