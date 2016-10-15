using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Modules;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Game", useTypeNameAsIconName = true)]
	internal class GameView : EditorWindow, IHasCustomMenu
	{
		private static class Styles
		{
			public static GUIContent gizmosContent;

			public static GUIContent zoomSliderContent;

			public static GUIContent maximizeOnPlayContent;

			public static GUIContent muteContent;

			public static GUIContent statsContent;

			public static GUIContent frameDebuggerOnContent;

			public static GUIContent loadRenderDocContent;

			public static GUIContent renderdocContent;

			public static GUIStyle gizmoButtonStyle;

			public static GUIStyle gameViewBackgroundStyle;

			static Styles()
			{
				GameView.Styles.gizmosContent = EditorGUIUtility.TextContent("Gizmos");
				GameView.Styles.zoomSliderContent = EditorGUIUtility.TextContent("Scale|Size of the game view on the screen");
				GameView.Styles.maximizeOnPlayContent = EditorGUIUtility.TextContent("Maximize on Play");
				GameView.Styles.muteContent = EditorGUIUtility.TextContent("Mute audio");
				GameView.Styles.statsContent = EditorGUIUtility.TextContent("Stats");
				GameView.Styles.frameDebuggerOnContent = EditorGUIUtility.TextContent("Frame Debugger on");
				GameView.Styles.loadRenderDocContent = EditorGUIUtility.TextContent("Load RenderDoc");
				GameView.Styles.gameViewBackgroundStyle = "GameViewBackground";
				GameView.Styles.gizmoButtonStyle = "GV Gizmo DropDown";
				GameView.Styles.renderdocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc");
			}
		}

		private const int kToolbarHeight = 17;

		private const int kBorderSize = 5;

		private const int kScaleSliderMinWidth = 30;

		private const int kScaleSliderMaxWidth = 150;

		private const int kScaleSliderSnapThreshold = 4;

		private const int kScaleLabelWidth = 30;

		private const float kMinScale = 1f;

		private const float kMaxScale = 5f;

		private const float kScrollZoomSnapDelay = 0.2f;

		private readonly Vector2 kWarningSize = new Vector2(400f, 140f);

		private readonly Color kClearBlack = new Color(0f, 0f, 0f, 0f);

		[SerializeField]
		private bool m_MaximizeOnPlay;

		[SerializeField]
		private bool m_Gizmos;

		[SerializeField]
		private bool m_Stats;

		[SerializeField]
		private int[] m_SelectedSizes = new int[0];

		[SerializeField]
		private int m_TargetDisplay;

		[SerializeField]
		private ZoomableArea m_ZoomArea;

		[SerializeField]
		private float m_defaultScale = -1f;

		[SerializeField]
		private RenderTexture m_TargetTexture;

		[SerializeField]
		private ColorSpace m_CurrentColorSpace = ColorSpace.Uninitialized;

		[SerializeField]
		private Vector2 m_LastWindowPixelSize;

		private int m_SizeChangeID = -2147483648;

		private static GUIStyle s_ResolutionWarningStyle;

		private static List<GameView> s_GameViews = new List<GameView>();

		private static GameView s_LastFocusedGameView = null;

		private static double s_LastScrollTime;

		private float minScale
		{
			get
			{
				return Mathf.Min(1f, this.ScaleThatFitsTargetInView(this.targetSize, this.viewInWindow.size));
			}
		}

		private float maxScale
		{
			get
			{
				return Mathf.Max(5f * EditorGUIUtility.pixelsPerPoint, this.ScaleThatFitsTargetInView(this.targetSize, this.viewInWindow.size));
			}
		}

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

		private Rect viewInWindow
		{
			get
			{
				return new Rect(0f, 17f, base.position.width, base.position.height - 17f);
			}
		}

		internal Vector2 targetSize
		{
			get
			{
				Rect startRect = EditorGUIUtility.PointsToPixels(this.viewInWindow);
				return GameViewSizes.GetRenderTargetSize(startRect, GameView.currentSizeGroupType, this.selectedSizeIndex);
			}
		}

		private Rect targetInContent
		{
			get
			{
				return EditorGUIUtility.PixelsToPoints(new Rect(-0.5f * this.targetSize, this.targetSize));
			}
		}

		private Rect targetInView
		{
			get
			{
				return new Rect(this.m_ZoomArea.DrawingToViewTransformPoint(this.targetInContent.position), this.m_ZoomArea.DrawingToViewTransformVector(this.targetInContent.size));
			}
		}

		private Rect deviceFlippedTargetInView
		{
			get
			{
				if (SystemInfo.usesOpenGLTextureCoords)
				{
					return this.targetInView;
				}
				Rect targetInView = this.targetInView;
				targetInView.y += targetInView.height;
				targetInView.height = -targetInView.height;
				return targetInView;
			}
		}

		private Rect viewInParent
		{
			get
			{
				Rect viewInWindow = this.viewInWindow;
				RectOffset borderSize = this.m_Parent.borderSize;
				viewInWindow.x += (float)borderSize.left;
				viewInWindow.y += (float)(borderSize.top + borderSize.bottom);
				return viewInWindow;
			}
		}

		private Rect targetInParent
		{
			get
			{
				return new Rect(this.targetInView.position + this.viewInParent.position, this.targetInView.size);
			}
		}

		private Rect clippedTargetInParent
		{
			get
			{
				return Rect.MinMaxRect(Mathf.Max(this.targetInParent.xMin, this.viewInParent.xMin), Mathf.Max(this.targetInParent.yMin, this.viewInParent.yMin), Mathf.Min(this.targetInParent.xMax, this.viewInParent.xMax), Mathf.Min(this.targetInParent.yMax, this.viewInParent.yMax));
			}
		}

		private Rect warningPosition
		{
			get
			{
				return new Rect((this.viewInWindow.size - this.kWarningSize) * 0.5f, this.kWarningSize);
			}
		}

		private Vector2 gameMouseOffset
		{
			get
			{
				return -this.viewInWindow.position - this.targetInView.position;
			}
		}

		private float gameMouseScale
		{
			get
			{
				return EditorGUIUtility.pixelsPerPoint / this.m_ZoomArea.scale.y;
			}
		}

		public GameView()
		{
			base.autoRepaintOnSceneChange = true;
			this.m_TargetDisplay = 0;
			this.InitializeZoomArea();
		}

		private Vector2 WindowToGameMousePosition(Vector2 windowMousePosition)
		{
			return (windowMousePosition + this.gameMouseOffset) * this.gameMouseScale;
		}

		public void OnValidate()
		{
			this.EnsureSelectedSizeAreValid();
		}

		private void InitializeZoomArea()
		{
			this.m_ZoomArea = new ZoomableArea(true, false);
			this.m_ZoomArea.uniformScale = true;
			this.m_ZoomArea.upDirection = ZoomableArea.YDirection.Negative;
		}

		public void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			this.EnsureSelectedSizeAreValid();
			base.dontClearBackground = true;
			GameView.s_GameViews.Add(this);
		}

		public void OnDisable()
		{
			GameView.s_GameViews.Remove(this);
			if (this.m_TargetTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_TargetTexture);
			}
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
			return GameView.GetMainGameViewTargetSize();
		}

		internal static Vector2 GetMainGameViewTargetSize()
		{
			GameView mainGameView = GameView.GetMainGameView();
			if (mainGameView != null && mainGameView.m_Parent)
			{
				return mainGameView.targetSize;
			}
			return new Vector2(640f, 480f);
		}

		private void UpdateZoomAreaAndParent()
		{
			bool flag = Mathf.Approximately(this.m_ZoomArea.scale.y, this.m_defaultScale);
			this.ConfigureZoomArea();
			this.m_defaultScale = this.DefaultScaleForTargetInView(this.targetSize, this.viewInWindow.size);
			if (flag)
			{
				this.m_ZoomArea.SetTransform(Vector2.zero, Vector2.one * this.m_defaultScale);
				this.EnforceZoomAreaConstraints();
			}
			this.CopyDimensionsToParentView();
			this.m_LastWindowPixelSize = base.position.size * EditorGUIUtility.pixelsPerPoint;
			EditorApplication.SetSceneRepaintDirty();
		}

		private void AllowCursorLockAndHide(bool enable)
		{
			Unsupported.SetAllowCursorLock(enable);
			Unsupported.SetAllowCursorHide(enable);
		}

		private void OnFocus()
		{
			this.AllowCursorLockAndHide(true);
			GameView.s_LastFocusedGameView = this;
			InternalEditorUtility.OnGameViewFocus(true);
		}

		private void OnLostFocus()
		{
			if (!EditorApplicationLayout.IsInitializingPlaymodeLayout())
			{
				this.AllowCursorLockAndHide(false);
			}
			InternalEditorUtility.OnGameViewFocus(false);
		}

		internal void CopyDimensionsToParentView()
		{
			base.SetParentGameViewDimensions(this.targetInParent, this.clippedTargetInParent, this.targetSize);
		}

		private void EnsureSelectedSizeAreValid()
		{
			int num = Enum.GetNames(typeof(GameViewSizeGroupType)).Length;
			if (this.m_SelectedSizes.Length != num)
			{
				Array.Resize<int>(ref this.m_SelectedSizes, num);
			}
			using (IEnumerator enumerator = Enum.GetValues(typeof(GameViewSizeGroupType)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameViewSizeGroupType gameViewSizeGroupType = (GameViewSizeGroupType)((int)enumerator.Current);
					GameViewSizeGroup group = ScriptableSingleton<GameViewSizes>.instance.GetGroup(gameViewSizeGroupType);
					int num2 = (int)gameViewSizeGroupType;
					this.m_SelectedSizes[num2] = Mathf.Clamp(this.m_SelectedSizes[num2], 0, group.GetTotalCount() - 1);
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

		private void LoadRenderDoc()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				RenderDoc.Load();
				ShaderUtil.RecreateGfxDevice();
			}
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
			{
				menu.AddItem(GameView.Styles.loadRenderDocContent, false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
			}
		}

		private bool ShouldShowMultiDisplayOption()
		{
			GUIContent[] displayNames = ModuleManager.GetDisplayNames(EditorUserBuildSettings.activeBuildTarget.ToString());
			return BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone || displayNames != null;
		}

		private void SizeSelectionCallback(int indexClicked, object objectSelected)
		{
			if (indexClicked != this.selectedSizeIndex)
			{
				this.selectedSizeIndex = indexClicked;
				if (this.currentGameViewSize.width > SystemInfo.maxTextureSize || this.currentGameViewSize.height > SystemInfo.maxTextureSize)
				{
					Debug.LogErrorFormat("GameView size clamped to maximum texture size for this system ({0})", new object[]
					{
						SystemInfo.maxTextureSize
					});
				}
				base.dontClearBackground = true;
				this.UpdateZoomAreaAndParent();
			}
		}

		private void SnapZoomDelayed()
		{
			if (EditorApplication.timeSinceStartup > GameView.s_LastScrollTime + 0.20000000298023224)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.SnapZoomDelayed));
				this.SnapZoom(this.m_ZoomArea.scale.y);
				base.Repaint();
			}
		}

		private void SnapZoom(float newZoom)
		{
			float num = Mathf.Log10(newZoom);
			float num2 = Mathf.Log10(this.minScale);
			float num3 = Mathf.Log10(this.maxScale);
			float num4 = 3.40282347E+38f;
			if (num > num2 && num < num3)
			{
				int num5 = 1;
				while ((float)num5 <= this.maxScale)
				{
					float num6 = 150f * Mathf.Abs(num - Mathf.Log10((float)num5)) / (num3 - num2);
					if (num6 < 4f && num6 < num4)
					{
						newZoom = (float)num5;
						num4 = num6;
					}
					num5++;
				}
			}
			Rect shownAreaInsideMargins = this.m_ZoomArea.shownAreaInsideMargins;
			Vector2 focalPoint = shownAreaInsideMargins.position + shownAreaInsideMargins.size * 0.5f;
			this.m_ZoomArea.SetScaleFocused(focalPoint, Vector2.one * newZoom);
		}

		private void DoZoomSlider()
		{
			GUILayout.Label(GameView.Styles.zoomSliderContent, EditorStyles.miniLabel, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			float num = Mathf.Log10(this.m_ZoomArea.scale.y);
			float leftValue = Mathf.Log10(this.minScale);
			float rightValue = Mathf.Log10(this.maxScale);
			num = GUILayout.HorizontalSlider(num, leftValue, rightValue, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(150f),
				GUILayout.MinWidth(30f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				float newZoom = Mathf.Pow(10f, num);
				this.SnapZoom(newZoom);
			}
			GUIContent gUIContent = EditorGUIUtility.TempContent(string.Format("{0}x", this.m_ZoomArea.scale.y.ToString("G2")));
			gUIContent.tooltip = GameView.Styles.zoomSliderContent.tooltip;
			GUILayout.Label(gUIContent, EditorStyles.miniLabel, new GUILayoutOption[]
			{
				GUILayout.Width(30f)
			});
			gUIContent.tooltip = string.Empty;
		}

		private void DoToolbarGUI()
		{
			ScriptableSingleton<GameViewSizes>.instance.RefreshStandaloneAndRemoteDefaultSizes();
			if (ScriptableSingleton<GameViewSizes>.instance.GetChangeID() != this.m_SizeChangeID)
			{
				this.EnsureSelectedSizeAreValid();
				this.m_SizeChangeID = ScriptableSingleton<GameViewSizes>.instance.GetChangeID();
			}
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			if (this.ShouldShowMultiDisplayOption())
			{
				int num = EditorGUILayout.Popup(this.m_TargetDisplay, DisplayUtility.GetDisplayNames(), EditorStyles.toolbarPopup, new GUILayoutOption[]
				{
					GUILayout.Width(80f)
				});
				EditorGUILayout.Space();
				if (num != this.m_TargetDisplay)
				{
					this.m_TargetDisplay = num;
					this.UpdateZoomAreaAndParent();
				}
			}
			EditorGUILayout.GameViewSizePopup(GameView.currentSizeGroupType, this.selectedSizeIndex, new Action<int, object>(this.SizeSelectionCallback), EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(160f)
			});
			this.DoZoomSlider();
			if (FrameDebuggerUtility.IsLocalEnabled())
			{
				GUILayout.FlexibleSpace();
				Color color = GUI.color;
				GUI.color *= AnimationMode.animatedPropertyColor;
				GUILayout.Label(GameView.Styles.frameDebuggerOnContent, EditorStyles.miniLabel, new GUILayoutOption[0]);
				GUI.color = color;
				if (Event.current.type == EventType.Repaint)
				{
					FrameDebuggerWindow.RepaintAll();
				}
			}
			GUILayout.FlexibleSpace();
			if (RenderDoc.IsLoaded())
			{
				using (new EditorGUI.DisabledScope(!RenderDoc.IsSupported()))
				{
					if (GUILayout.Button(GameView.Styles.renderdocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						this.m_Parent.CaptureRenderDoc();
						GUIUtility.ExitGUI();
					}
				}
			}
			this.m_MaximizeOnPlay = GUILayout.Toggle(this.m_MaximizeOnPlay, GameView.Styles.maximizeOnPlayContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			EditorUtility.audioMasterMute = GUILayout.Toggle(EditorUtility.audioMasterMute, GameView.Styles.muteContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			this.m_Stats = GUILayout.Toggle(this.m_Stats, GameView.Styles.statsContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(GameView.Styles.gizmosContent, GameView.Styles.gizmoButtonStyle);
			Rect position = new Rect(rect.xMax - (float)GameView.Styles.gizmoButtonStyle.border.right, rect.y, (float)GameView.Styles.gizmoButtonStyle.border.right, rect.height);
			if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				if (AnnotationWindow.ShowAtPosition(last, true))
				{
					GUIUtility.ExitGUI();
				}
			}
			this.m_Gizmos = GUI.Toggle(rect, this.m_Gizmos, GameView.Styles.gizmosContent, GameView.Styles.gizmoButtonStyle);
			GUILayout.EndHorizontal();
		}

		private void ClearTargetTexture()
		{
			if (this.m_TargetTexture.IsCreated())
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = this.m_TargetTexture;
				GL.Clear(true, true, this.kClearBlack);
				RenderTexture.active = active;
			}
		}

		private void ConfigureTargetTexture(int width, int height)
		{
			bool flag = false;
			if (this.m_TargetTexture && this.m_CurrentColorSpace != QualitySettings.activeColorSpace)
			{
				UnityEngine.Object.DestroyImmediate(this.m_TargetTexture);
			}
			if (!this.m_TargetTexture)
			{
				this.m_CurrentColorSpace = QualitySettings.activeColorSpace;
				this.m_TargetTexture = new RenderTexture(0, 0, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				this.m_TargetTexture.name = "GameView RT";
				this.m_TargetTexture.filterMode = FilterMode.Point;
				this.m_TargetTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			int num = Mathf.Max(1, QualitySettings.antiAliasing);
			if (this.m_TargetTexture.width != width || this.m_TargetTexture.height != height || this.m_TargetTexture.antiAliasing != num)
			{
				this.m_TargetTexture.Release();
				this.m_TargetTexture.width = width;
				this.m_TargetTexture.height = height;
				this.m_TargetTexture.antiAliasing = num;
				flag = true;
			}
			this.m_TargetTexture.Create();
			if (flag)
			{
				this.ClearTargetTexture();
			}
		}

		private float ScaleThatFitsTargetInView(Vector2 targetInPixels, Vector2 viewInPoints)
		{
			Vector2 vector = EditorGUIUtility.PixelsToPoints(targetInPixels);
			Vector2 vector2 = new Vector2(viewInPoints.x / vector.x, viewInPoints.y / vector.y);
			return Mathf.Min(vector2.x, vector2.y);
		}

		private float DefaultScaleForTargetInView(Vector2 targetToFit, Vector2 viewSize)
		{
			float num = this.ScaleThatFitsTargetInView(targetToFit, viewSize);
			if (num > 1f)
			{
				num = Mathf.Min(this.maxScale * EditorGUIUtility.pixelsPerPoint, (float)Mathf.FloorToInt(num));
			}
			return num;
		}

		private void ConfigureZoomArea()
		{
			this.m_ZoomArea.rect = this.viewInWindow;
			this.m_ZoomArea.hBaseRangeMin = this.targetInContent.xMin;
			this.m_ZoomArea.vBaseRangeMin = this.targetInContent.yMin;
			this.m_ZoomArea.hBaseRangeMax = this.targetInContent.xMax;
			this.m_ZoomArea.vBaseRangeMax = this.targetInContent.yMax;
			ZoomableArea arg_92_0 = this.m_ZoomArea;
			float num = this.minScale;
			this.m_ZoomArea.vScaleMin = num;
			arg_92_0.hScaleMin = num;
			ZoomableArea arg_B4_0 = this.m_ZoomArea;
			num = this.maxScale;
			this.m_ZoomArea.vScaleMax = num;
			arg_B4_0.hScaleMax = num;
		}

		private void EnforceZoomAreaConstraints()
		{
			Rect shownArea = this.m_ZoomArea.shownArea;
			if (shownArea.width > this.targetInContent.width)
			{
				shownArea.x = -0.5f * shownArea.width;
			}
			else
			{
				shownArea.x = Mathf.Clamp(shownArea.x, this.targetInContent.xMin, this.targetInContent.xMax - shownArea.width);
			}
			if (shownArea.height > this.targetInContent.height)
			{
				shownArea.y = -0.5f * shownArea.height;
			}
			else
			{
				shownArea.y = Mathf.Clamp(shownArea.y, this.targetInContent.yMin, this.targetInContent.yMax - shownArea.height);
			}
			this.m_ZoomArea.shownArea = shownArea;
		}

		private void OnGUI()
		{
			if (base.position.size * EditorGUIUtility.pixelsPerPoint != this.m_LastWindowPixelSize)
			{
				this.UpdateZoomAreaAndParent();
			}
			this.DoToolbarGUI();
			this.CopyDimensionsToParentView();
			EditorGUIUtility.AddCursorRect(this.viewInWindow, MouseCursor.CustomCursor);
			EventType type = Event.current.type;
			if (type == EventType.MouseDown && this.viewInWindow.Contains(Event.current.mousePosition))
			{
				this.AllowCursorLockAndHide(true);
			}
			else if (type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
			{
				Unsupported.SetAllowCursorLock(false);
			}
			bool flag = !EditorApplication.isPlaying || EditorApplication.isPaused;
			this.m_ZoomArea.hSlider = (flag && this.m_ZoomArea.shownArea.width < this.targetInContent.width);
			this.m_ZoomArea.vSlider = (flag && this.m_ZoomArea.shownArea.height < this.targetInContent.height);
			this.m_ZoomArea.enableMouseInput = flag;
			this.ConfigureZoomArea();
			Vector2 mousePosition = Event.current.mousePosition;
			Vector2 mousePosition2 = this.WindowToGameMousePosition(mousePosition);
			GUI.color = Color.white;
			EventType type2 = Event.current.type;
			this.m_ZoomArea.BeginViewGUI();
			if (type == EventType.Repaint)
			{
				GUI.Box(this.m_ZoomArea.drawRect, GUIContent.none, GameView.Styles.gameViewBackgroundStyle);
				Vector2 s_EditorScreenPointOffset = GUIUtility.s_EditorScreenPointOffset;
				GUIUtility.s_EditorScreenPointOffset = Vector2.zero;
				SavedGUIState savedGUIState = SavedGUIState.Create();
				this.ConfigureTargetTexture((int)this.targetSize.x, (int)this.targetSize.y);
				if (!EditorApplication.isPlaying)
				{
					this.ClearTargetTexture();
				}
				int targetDisplay = 0;
				if (this.ShouldShowMultiDisplayOption())
				{
					targetDisplay = this.m_TargetDisplay;
				}
				if (this.m_TargetTexture.IsCreated())
				{
					EditorGUIUtility.RenderGameViewCamerasInternal(this.m_TargetTexture, targetDisplay, GUIClip.Unclip(this.viewInWindow), mousePosition2, this.m_Gizmos);
					savedGUIState.ApplyAndForget();
					GUIUtility.s_EditorScreenPointOffset = s_EditorScreenPointOffset;
					GUI.BeginGroup(this.m_ZoomArea.drawRect);
					GL.sRGBWrite = (this.m_CurrentColorSpace == ColorSpace.Linear);
					GUI.DrawTexture(this.deviceFlippedTargetInView, this.m_TargetTexture, ScaleMode.StretchToFill, false);
					GL.sRGBWrite = false;
					GUI.EndGroup();
				}
			}
			else if (type != EventType.Layout && type != EventType.Used)
			{
				if (WindowLayout.s_MaximizeKey.activated && (!EditorApplication.isPlaying || EditorApplication.isPaused))
				{
					return;
				}
				bool flag2 = this.viewInWindow.Contains(Event.current.mousePosition);
				if (Event.current.rawType == EventType.MouseDown && !flag2)
				{
					return;
				}
				Event.current.mousePosition = mousePosition2;
				Event.current.displayIndex = this.m_TargetDisplay;
				EditorGUIUtility.QueueGameViewInputEvent(Event.current);
				bool flag3 = true;
				if (Event.current.rawType == EventType.MouseUp && !flag2)
				{
					flag3 = false;
				}
				if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
				{
					flag3 = false;
				}
				if (flag3)
				{
					Event.current.Use();
				}
				else
				{
					Event.current.mousePosition = mousePosition;
				}
			}
			this.m_ZoomArea.EndViewGUI();
			if (type2 == EventType.ScrollWheel && Event.current.type == EventType.Used)
			{
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.SnapZoomDelayed));
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.SnapZoomDelayed));
				GameView.s_LastScrollTime = EditorApplication.timeSinceStartup;
			}
			this.EnforceZoomAreaConstraints();
			if (this.m_TargetTexture)
			{
				if (this.m_ZoomArea.scale.y < 1f)
				{
					this.m_TargetTexture.filterMode = FilterMode.Bilinear;
				}
				else
				{
					this.m_TargetTexture.filterMode = FilterMode.Point;
				}
			}
			if (!EditorGUIUtility.IsDisplayReferencedByCameras(this.m_TargetDisplay))
			{
				GUI.Label(this.warningPosition, GUIContent.none, EditorStyles.notificationBackground);
				string arg = (!this.ShouldShowMultiDisplayOption()) ? string.Empty : DisplayUtility.GetDisplayNames()[this.m_TargetDisplay].text;
				string t = string.Format("{0}\nNo cameras rendering", arg);
				EditorGUI.DoDropShadowLabel(this.warningPosition, EditorGUIUtility.TempContent(t), EditorStyles.notificationText, 0.3f);
			}
			if (this.m_Stats)
			{
				GameViewGUI.GameViewStatsGUI();
			}
		}
	}
}
