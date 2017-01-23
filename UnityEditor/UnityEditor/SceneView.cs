using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor.AnimatedValues;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Scene", useTypeNameAsIconName = true)]
	public class SceneView : SearchableEditorWindow, IHasCustomMenu
	{
		[Serializable]
		public class SceneViewState
		{
			public bool showFog = true;

			public bool showMaterialUpdate = false;

			public bool showSkybox = true;

			public bool showFlares = true;

			public bool showImageEffects = true;

			public SceneViewState()
			{
			}

			public SceneViewState(SceneView.SceneViewState other)
			{
				this.showFog = other.showFog;
				this.showMaterialUpdate = other.showMaterialUpdate;
				this.showSkybox = other.showSkybox;
				this.showFlares = other.showFlares;
				this.showImageEffects = other.showImageEffects;
			}

			public bool IsAllOn()
			{
				return this.showFog && this.showMaterialUpdate && this.showSkybox && this.showFlares && this.showImageEffects;
			}

			public void Toggle(bool value)
			{
				this.showFog = value;
				this.showMaterialUpdate = value;
				this.showSkybox = value;
				this.showFlares = value;
				this.showImageEffects = value;
			}
		}

		public delegate void OnSceneFunc(SceneView sceneView);

		private struct CursorRect
		{
			public Rect rect;

			public MouseCursor cursor;

			public CursorRect(Rect rect, MouseCursor cursor)
			{
				this.rect = rect;
				this.cursor = cursor;
			}
		}

		internal enum DraggingLockedState
		{
			NotDragging,
			Dragging,
			LookAt
		}

		private static SceneView s_LastActiveSceneView;

		private static SceneView s_CurrentDrawingSceneView;

		private static readonly PrefColor kSceneViewBackground = new PrefColor("Scene/Background", 0.278431f, 0.278431f, 0.278431f, 0f);

		private static readonly PrefColor kSceneViewWire = new PrefColor("Scene/Wireframe", 0f, 0f, 0f, 0.5f);

		private static readonly PrefColor kSceneViewWireOverlay = new PrefColor("Scene/Wireframe Overlay", 0f, 0f, 0f, 0.25f);

		private static readonly PrefColor kSceneViewSelectedOutline = new PrefColor("Scene/Selected Outline", 1f, 0.4f, 0f, 0.0470588244f);

		private static readonly PrefColor kSceneViewSelectedWire = new PrefColor("Scene/Selected Wireframe", 0.368627459f, 0.466666669f, 0.607843161f, 0.2509804f);

		internal static Color kSceneViewFrontLight = new Color(0.769f, 0.769f, 0.769f, 1f);

		internal static Color kSceneViewUpLight = new Color(0.212f, 0.227f, 0.259f, 1f);

		internal static Color kSceneViewMidLight = new Color(0.114f, 0.125f, 0.133f, 1f);

		internal static Color kSceneViewDownLight = new Color(0.047f, 0.043f, 0.035f, 1f);

		[NonSerialized]
		private static readonly Quaternion kDefaultRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));

		private const float kDefaultViewSize = 10f;

		[NonSerialized]
		private static readonly Vector3 kDefaultPivot = Vector3.zero;

		private const float kOrthoThresholdAngle = 3f;

		private const float kOneOverSqrt2 = 0.707106769f;

		[NonSerialized]
		private ActiveEditorTracker m_Tracker;

		[SerializeField]
		public bool m_SceneLighting = true;

		public double lastFramingTime = 0.0;

		private const double k_MaxDoubleKeypressTime = 0.5;

		private static readonly PrefKey k2DMode = new PrefKey("Tools/2D Mode", "2");

		private static bool waitingFor2DModeKeyUp;

		[SerializeField]
		private bool m_2DMode;

		[SerializeField]
		private bool m_isRotationLocked = false;

		internal UnityEngine.Object m_OneClickDragObject;

		public bool m_AudioPlay = false;

		private static SceneView s_AudioSceneView;

		[SerializeField]
		private AnimVector3 m_Position = new AnimVector3(SceneView.kDefaultPivot);

		public static SceneView.OnSceneFunc onSceneGUIDelegate;

		internal static SceneView.OnSceneFunc onPreSceneGUIDelegate;

		public DrawCameraMode m_RenderMode = DrawCameraMode.Textured;

		[SerializeField]
		internal SceneView.SceneViewState m_SceneViewState;

		[SerializeField]
		private SceneViewGrid grid;

		[SerializeField]
		internal SceneViewRotation svRot;

		[SerializeField]
		internal AnimQuaternion m_Rotation = new AnimQuaternion(SceneView.kDefaultRotation);

		[SerializeField]
		private AnimFloat m_Size = new AnimFloat(10f);

		[SerializeField]
		internal AnimBool m_Ortho = new AnimBool();

		[NonSerialized]
		private Camera m_Camera;

		[SerializeField]
		private Quaternion m_LastSceneViewRotation;

		[SerializeField]
		private bool m_LastSceneViewOrtho;

		private static MouseCursor s_LastCursor = MouseCursor.Arrow;

		private static readonly List<SceneView.CursorRect> s_MouseRects = new List<SceneView.CursorRect>();

		private bool s_DraggingCursorIsCached;

		[NonSerialized]
		private Light[] m_Light = new Light[3];

		private RectSelection m_RectSelection;

		private const float kPerspectiveFov = 90f;

		private static ArrayList s_SceneViews = new ArrayList();

		private static Material s_AlphaOverlayMaterial;

		private static Material s_DeferredOverlayMaterial;

		private static Shader s_ShowOverdrawShader;

		private static Shader s_ShowMipsShader;

		private static Shader s_ShowLightmapsShader;

		private static Shader s_AuraShader;

		private static Shader s_GrayScaleShader;

		private static Texture2D s_MipColorsTexture;

		private GUIContent m_Lighting;

		private GUIContent m_Fx;

		private GUIContent m_AudioPlayContent;

		private GUIContent m_GizmosContent;

		private GUIContent m_2DModeContent;

		private GUIContent m_RenderDocContent;

		private static Tool s_CurrentTool;

		private double m_StartSearchFilterTime = -1.0;

		private RenderTexture m_SceneTargetTexture;

		private int m_MainViewControlID;

		[SerializeField]
		private Shader m_ReplacementShader;

		[SerializeField]
		private string m_ReplacementString;

		internal bool m_ShowSceneViewWindows = false;

		private SceneViewOverlay m_SceneViewOverlay;

		private EditorCache m_DragEditorCache;

		private SceneView.DraggingLockedState m_DraggingLockedState;

		[SerializeField]
		private UnityEngine.Object m_LastLockedObject;

		[SerializeField]
		private bool m_ViewIsLockedToObject;

		private static GUIStyle s_DropDownStyle;

		private bool m_RequestedSceneViewFiltering;

		private double m_lastRenderedTime;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache0;

		[CompilerGenerated]
		private static EditorApplication.CallbackFunction <>f__mg$cache1;

		[CompilerGenerated]
		private static ComponentUtility.IsDesiredComponent <>f__mg$cache2;

		[CompilerGenerated]
		private static ComponentUtility.IsDesiredComponent <>f__mg$cache3;

		public static SceneView lastActiveSceneView
		{
			get
			{
				return SceneView.s_LastActiveSceneView;
			}
		}

		public static SceneView currentDrawingSceneView
		{
			get
			{
				return SceneView.s_CurrentDrawingSceneView;
			}
		}

		public bool in2DMode
		{
			get
			{
				return this.m_2DMode;
			}
			set
			{
				if (this.m_2DMode != value && Tools.viewTool != ViewTool.FPS && Tools.viewTool != ViewTool.Orbit)
				{
					this.m_2DMode = value;
					this.On2DModeChange();
				}
			}
		}

		public bool isRotationLocked
		{
			get
			{
				return this.m_isRotationLocked;
			}
			set
			{
				if (this.m_isRotationLocked != value)
				{
					this.m_isRotationLocked = value;
				}
			}
		}

		public DrawCameraMode renderMode
		{
			get
			{
				return this.m_RenderMode;
			}
			set
			{
				this.m_RenderMode = value;
			}
		}

		public Quaternion lastSceneViewRotation
		{
			get
			{
				if (this.m_LastSceneViewRotation == new Quaternion(0f, 0f, 0f, 0f))
				{
					this.m_LastSceneViewRotation = Quaternion.identity;
				}
				return this.m_LastSceneViewRotation;
			}
			set
			{
				this.m_LastSceneViewRotation = value;
			}
		}

		internal float cameraDistance
		{
			get
			{
				float num = this.m_Ortho.Fade(90f, 0f);
				float result;
				if (!this.camera.orthographic)
				{
					result = this.size / Mathf.Tan(num * 0.5f * 0.0174532924f);
				}
				else
				{
					result = this.size * 2f;
				}
				return result;
			}
		}

		public static ArrayList sceneViews
		{
			get
			{
				return SceneView.s_SceneViews;
			}
		}

		public Camera camera
		{
			get
			{
				return this.m_Camera;
			}
		}

		internal SceneView.DraggingLockedState draggingLocked
		{
			get
			{
				return this.m_DraggingLockedState;
			}
			set
			{
				this.m_DraggingLockedState = value;
			}
		}

		internal bool viewIsLockedToObject
		{
			get
			{
				return this.m_ViewIsLockedToObject;
			}
			set
			{
				if (value)
				{
					this.m_LastLockedObject = Selection.activeObject;
				}
				else
				{
					this.m_LastLockedObject = null;
				}
				this.m_ViewIsLockedToObject = value;
				this.draggingLocked = SceneView.DraggingLockedState.LookAt;
			}
		}

		private GUIStyle effectsDropDownStyle
		{
			get
			{
				if (SceneView.s_DropDownStyle == null)
				{
					SceneView.s_DropDownStyle = "GV Gizmo DropDown";
				}
				return SceneView.s_DropDownStyle;
			}
		}

		public Vector3 pivot
		{
			get
			{
				return this.m_Position.value;
			}
			set
			{
				this.m_Position.value = value;
			}
		}

		public Quaternion rotation
		{
			get
			{
				return this.m_Rotation.value;
			}
			set
			{
				this.m_Rotation.value = value;
			}
		}

		public float size
		{
			get
			{
				return this.m_Size.value;
			}
			set
			{
				if (value > 40000f)
				{
					value = 40000f;
				}
				this.m_Size.value = value;
			}
		}

		public bool orthographic
		{
			get
			{
				return this.m_Ortho.value;
			}
			set
			{
				this.m_Ortho.value = value;
			}
		}

		internal Quaternion cameraTargetRotation
		{
			get
			{
				return this.m_Rotation.target;
			}
		}

		internal Vector3 cameraTargetPosition
		{
			get
			{
				return this.m_Position.target + this.m_Rotation.target * new Vector3(0f, 0f, this.cameraDistance);
			}
		}

		public SceneView()
		{
			this.m_HierarchyType = HierarchyType.GameObjects;
		}

		internal static void AddCursorRect(Rect rect, MouseCursor cursor)
		{
			if (Event.current.type == EventType.Repaint)
			{
				SceneView.s_MouseRects.Add(new SceneView.CursorRect(rect, cursor));
			}
		}

		public void SetSceneViewShaderReplace(Shader shader, string replaceString)
		{
			this.m_ReplacementShader = shader;
			this.m_ReplacementString = replaceString;
		}

		public static bool FrameLastActiveSceneView()
		{
			return !(SceneView.lastActiveSceneView == null) && SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent("FrameSelected"));
		}

		public static bool FrameLastActiveSceneViewWithLock()
		{
			return !(SceneView.lastActiveSceneView == null) && SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent("FrameSelectedWithLock"));
		}

		private Editor[] GetActiveEditors()
		{
			if (this.m_Tracker == null)
			{
				this.m_Tracker = ActiveEditorTracker.sharedTracker;
			}
			return this.m_Tracker.activeEditors;
		}

		public static Camera[] GetAllSceneCameras()
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < SceneView.s_SceneViews.Count; i++)
			{
				Camera camera = ((SceneView)SceneView.s_SceneViews[i]).m_Camera;
				if (camera != null)
				{
					arrayList.Add(camera);
				}
			}
			return (Camera[])arrayList.ToArray(typeof(Camera));
		}

		public static void RepaintAll()
		{
			IEnumerator enumerator = SceneView.s_SceneViews.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					SceneView sceneView = (SceneView)enumerator.Current;
					sceneView.Repaint();
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode mode, bool setAll)
		{
			if (this.m_SearchFilter == "" || searchFilter == "")
			{
				this.m_StartSearchFilterTime = EditorApplication.timeSinceStartup;
			}
			base.SetSearchFilter(searchFilter, mode, setAll);
		}

		internal void OnFocus()
		{
			if (!Application.isPlaying && this.m_AudioPlay && this.m_Camera != null)
			{
				this.RefreshAudioPlay();
			}
		}

		internal void OnLostFocus()
		{
			GameView gameView = (GameView)WindowLayout.FindEditorWindowOfType(typeof(GameView));
			if (gameView && gameView.m_Parent != null && this.m_Parent != null && gameView.m_Parent == this.m_Parent)
			{
				gameView.m_Parent.backgroundValid = false;
			}
			if (SceneView.s_LastActiveSceneView == this)
			{
				SceneViewMotion.ResetMotion();
			}
		}

		public override void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			this.m_RectSelection = new RectSelection(this);
			if (this.grid == null)
			{
				this.grid = new SceneViewGrid();
			}
			this.grid.Register(this);
			if (this.svRot == null)
			{
				this.svRot = new SceneViewRotation();
			}
			this.svRot.Register(this);
			base.autoRepaintOnSceneChange = true;
			this.m_Rotation.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Position.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Size.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Ortho.valueChanged.AddListener(new UnityAction(base.Repaint));
			base.wantsMouseMove = true;
			base.dontClearBackground = true;
			SceneView.s_SceneViews.Add(this);
			this.m_Lighting = EditorGUIUtility.IconContent("SceneviewLighting", "Lighting|The scene lighting is used when toggled on. When toggled off a light attached to the scene view camera is used.");
			this.m_Fx = EditorGUIUtility.IconContent("SceneviewFx", "Fx|Toggles skybox, fog and lens flare effects.");
			this.m_AudioPlayContent = EditorGUIUtility.IconContent("SceneviewAudio", "AudioPlay|Toggles audio on or off.");
			this.m_GizmosContent = new GUIContent("Gizmos");
			this.m_2DModeContent = new GUIContent("2D");
			this.m_RenderDocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc");
			this.m_SceneViewOverlay = new SceneViewOverlay(this);
			Delegate arg_190_0 = EditorApplication.modifierKeysChanged;
			if (SceneView.<>f__mg$cache0 == null)
			{
				SceneView.<>f__mg$cache0 = new EditorApplication.CallbackFunction(SceneView.RepaintAll);
			}
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(arg_190_0, SceneView.<>f__mg$cache0);
			this.m_DraggingLockedState = SceneView.DraggingLockedState.NotDragging;
			this.CreateSceneCameraAndLights();
			if (this.m_2DMode)
			{
				this.LookAt(this.pivot, Quaternion.identity, this.size, true, true);
			}
			base.OnEnable();
		}

		internal void Awake()
		{
			if (this.m_SceneViewState == null)
			{
				this.m_SceneViewState = new SceneView.SceneViewState();
			}
			if (this.m_2DMode || EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
			{
				this.m_LastSceneViewRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
				this.m_LastSceneViewOrtho = false;
				this.m_Rotation.value = Quaternion.identity;
				this.m_Ortho.value = true;
				this.m_2DMode = true;
				if (Tools.current == Tool.Move)
				{
					Tools.current = Tool.Rect;
				}
			}
		}

		internal static void PlaceGameObjectInFrontOfSceneView(GameObject go)
		{
			if (SceneView.s_SceneViews.Count >= 1)
			{
				SceneView sceneView = SceneView.s_LastActiveSceneView;
				if (!sceneView)
				{
					sceneView = (SceneView.s_SceneViews[0] as SceneView);
				}
				if (sceneView)
				{
					sceneView.MoveToView(go.transform);
				}
			}
		}

		public override void OnDisable()
		{
			Delegate arg_23_0 = EditorApplication.modifierKeysChanged;
			if (SceneView.<>f__mg$cache1 == null)
			{
				SceneView.<>f__mg$cache1 = new EditorApplication.CallbackFunction(SceneView.RepaintAll);
			}
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(arg_23_0, SceneView.<>f__mg$cache1);
			if (this.m_Camera)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Camera.gameObject, true);
			}
			if (this.m_Light[0])
			{
				UnityEngine.Object.DestroyImmediate(this.m_Light[0].gameObject, true);
			}
			if (this.m_Light[1])
			{
				UnityEngine.Object.DestroyImmediate(this.m_Light[1].gameObject, true);
			}
			if (this.m_Light[2])
			{
				UnityEngine.Object.DestroyImmediate(this.m_Light[2].gameObject, true);
			}
			if (SceneView.s_MipColorsTexture)
			{
				UnityEngine.Object.DestroyImmediate(SceneView.s_MipColorsTexture, true);
			}
			SceneView.s_SceneViews.Remove(this);
			if (SceneView.s_LastActiveSceneView == this)
			{
				if (SceneView.s_SceneViews.Count > 0)
				{
					SceneView.s_LastActiveSceneView = (SceneView.s_SceneViews[0] as SceneView);
				}
				else
				{
					SceneView.s_LastActiveSceneView = null;
				}
			}
			this.CleanupEditorDragFunctions();
			base.OnDisable();
		}

		public void OnDestroy()
		{
			if (this.m_AudioPlay)
			{
				this.m_AudioPlay = false;
				this.RefreshAudioPlay();
			}
		}

		private void DoToolbarGUI()
		{
			GUILayout.BeginHorizontal("toolbar", new GUILayoutOption[0]);
			GUIContent gUIContent = SceneRenderModeWindow.GetGUIContent(this.m_RenderMode);
			Rect rect = GUILayoutUtility.GetRect(gUIContent, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			if (EditorGUI.ButtonMouseDown(rect, gUIContent, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				PopupWindow.Show(last, new SceneRenderModeWindow(this));
				GUIUtility.ExitGUI();
			}
			EditorGUILayout.Space();
			this.in2DMode = GUILayout.Toggle(this.in2DMode, this.m_2DModeContent, "toolbarbutton", new GUILayoutOption[0]);
			EditorGUILayout.Space();
			this.m_SceneLighting = GUILayout.Toggle(this.m_SceneLighting, this.m_Lighting, "toolbarbutton", new GUILayoutOption[0]);
			if (this.renderMode == DrawCameraMode.ShadowCascades)
			{
				this.m_SceneLighting = true;
			}
			GUI.enabled = !Application.isPlaying;
			GUI.changed = false;
			this.m_AudioPlay = GUILayout.Toggle(this.m_AudioPlay, this.m_AudioPlayContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				this.RefreshAudioPlay();
			}
			GUI.enabled = true;
			Rect rect2 = GUILayoutUtility.GetRect(this.m_Fx, this.effectsDropDownStyle);
			Rect position = new Rect(rect2.xMax - (float)this.effectsDropDownStyle.border.right, rect2.y, (float)this.effectsDropDownStyle.border.right, rect2.height);
			if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
			{
				Rect last2 = GUILayoutUtility.topLevel.GetLast();
				PopupWindow.Show(last2, new SceneFXWindow(this));
				GUIUtility.ExitGUI();
			}
			bool flag = GUI.Toggle(rect2, this.m_SceneViewState.IsAllOn(), this.m_Fx, this.effectsDropDownStyle);
			if (flag != this.m_SceneViewState.IsAllOn())
			{
				this.m_SceneViewState.Toggle(flag);
			}
			EditorGUILayout.Space();
			GUILayout.FlexibleSpace();
			if (this.m_MainViewControlID != GUIUtility.keyboardControl && Event.current.type == EventType.KeyDown && !string.IsNullOrEmpty(this.m_SearchFilter))
			{
				KeyCode keyCode = Event.current.keyCode;
				if (keyCode == KeyCode.UpArrow || keyCode == KeyCode.DownArrow)
				{
					if (Event.current.keyCode == KeyCode.UpArrow)
					{
						base.SelectPreviousSearchResult();
					}
					else
					{
						base.SelectNextSearchResult();
					}
					this.FrameSelected(false);
					Event.current.Use();
					GUIUtility.ExitGUI();
					return;
				}
			}
			if (RenderDoc.IsLoaded())
			{
				using (new EditorGUI.DisabledScope(!RenderDoc.IsSupported()))
				{
					if (GUILayout.Button(this.m_RenderDocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						this.m_Parent.CaptureRenderDoc();
						GUIUtility.ExitGUI();
					}
				}
			}
			Rect rect3 = GUILayoutUtility.GetRect(this.m_GizmosContent, EditorStyles.toolbarDropDown);
			if (EditorGUI.ButtonMouseDown(rect3, this.m_GizmosContent, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				Rect last3 = GUILayoutUtility.topLevel.GetLast();
				if (AnnotationWindow.ShowAtPosition(last3, false))
				{
					GUIUtility.ExitGUI();
				}
			}
			GUILayout.Space(6f);
			base.SearchFieldGUI(EditorGUILayout.kLabelFloatMaxW);
			GUILayout.EndHorizontal();
		}

		private void RefreshAudioPlay()
		{
			if (SceneView.s_AudioSceneView != null && SceneView.s_AudioSceneView != this)
			{
				if (SceneView.s_AudioSceneView.m_AudioPlay)
				{
					SceneView.s_AudioSceneView.m_AudioPlay = false;
					SceneView.s_AudioSceneView.Repaint();
				}
			}
			AudioSource[] array = (AudioSource[])UnityEngine.Object.FindObjectsOfType(typeof(AudioSource));
			AudioSource[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				AudioSource audioSource = array2[i];
				if (audioSource.playOnAwake)
				{
					if (!this.m_AudioPlay)
					{
						audioSource.Stop();
					}
					else if (!audioSource.isPlaying)
					{
						audioSource.Play();
					}
				}
			}
			AudioUtil.SetListenerTransform((!this.m_AudioPlay) ? null : this.m_Camera.transform);
			SceneView.s_AudioSceneView = this;
		}

		public void OnSelectionChange()
		{
			if (Selection.activeObject != null && this.m_LastLockedObject != Selection.activeObject)
			{
				this.viewIsLockedToObject = false;
			}
			base.Repaint();
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
				menu.AddItem(new GUIContent("Load RenderDoc"), false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
			}
		}

		[MenuItem("GameObject/Set as first sibling %=")]
		internal static void MenuMoveToFront()
		{
			Transform[] transforms = Selection.transforms;
			for (int i = 0; i < transforms.Length; i++)
			{
				Transform transform = transforms[i];
				Undo.SetTransformParent(transform, transform.parent, "Set as first sibling");
				transform.SetAsFirstSibling();
			}
		}

		[MenuItem("GameObject/Set as first sibling %=", true)]
		internal static bool ValidateMenuMoveToFront()
		{
			bool result;
			if (Selection.activeTransform != null)
			{
				Transform parent = Selection.activeTransform.parent;
				result = (parent != null && parent.GetChild(0) != Selection.activeTransform);
			}
			else
			{
				result = false;
			}
			return result;
		}

		[MenuItem("GameObject/Set as last sibling %-")]
		internal static void MenuMoveToBack()
		{
			Transform[] transforms = Selection.transforms;
			for (int i = 0; i < transforms.Length; i++)
			{
				Transform transform = transforms[i];
				Undo.SetTransformParent(transform, transform.parent, "Set as last sibling");
				transform.SetAsLastSibling();
			}
		}

		[MenuItem("GameObject/Set as last sibling %-", true)]
		internal static bool ValidateMenuMoveToBack()
		{
			bool result;
			if (Selection.activeTransform != null)
			{
				Transform parent = Selection.activeTransform.parent;
				result = (parent != null && parent.GetChild(parent.childCount - 1) != Selection.activeTransform);
			}
			else
			{
				result = false;
			}
			return result;
		}

		[MenuItem("GameObject/Move To View %&f")]
		internal static void MenuMoveToView()
		{
			if (SceneView.ValidateMoveToView())
			{
				SceneView.s_LastActiveSceneView.MoveToView();
			}
		}

		[MenuItem("GameObject/Move To View %&f", true)]
		private static bool ValidateMoveToView()
		{
			return SceneView.s_LastActiveSceneView != null && Selection.transforms.Length != 0;
		}

		[MenuItem("GameObject/Align With View %#f")]
		internal static void MenuAlignWithView()
		{
			if (SceneView.ValidateAlignWithView())
			{
				SceneView.s_LastActiveSceneView.AlignWithView();
			}
		}

		[MenuItem("GameObject/Align With View %#f", true)]
		internal static bool ValidateAlignWithView()
		{
			return SceneView.s_LastActiveSceneView != null && Selection.activeTransform != null;
		}

		[MenuItem("GameObject/Align View to Selected")]
		internal static void MenuAlignViewToSelected()
		{
			if (SceneView.ValidateAlignViewToSelected())
			{
				SceneView.s_LastActiveSceneView.AlignViewToObject(Selection.activeTransform);
			}
		}

		[MenuItem("GameObject/Align View to Selected", true)]
		internal static bool ValidateAlignViewToSelected()
		{
			return SceneView.s_LastActiveSceneView != null && Selection.activeTransform != null;
		}

		[MenuItem("GameObject/Toggle Active State &#a")]
		internal static void ActivateSelection()
		{
			if (Selection.activeTransform != null)
			{
				GameObject[] gameObjects = Selection.gameObjects;
				Undo.RecordObjects(gameObjects, "Toggle Active State");
				bool active = !Selection.activeGameObject.activeSelf;
				GameObject[] array = gameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					GameObject gameObject = array[i];
					gameObject.SetActive(active);
				}
			}
		}

		[MenuItem("GameObject/Toggle Active State &#a", true)]
		internal static bool ValidateActivateSelection()
		{
			return Selection.activeTransform != null;
		}

		private static void CreateMipColorsTexture()
		{
			if (!SceneView.s_MipColorsTexture)
			{
				SceneView.s_MipColorsTexture = new Texture2D(32, 32, TextureFormat.RGBA32, true);
				SceneView.s_MipColorsTexture.hideFlags = HideFlags.HideAndDontSave;
				Color[] array = new Color[]
				{
					new Color(0f, 0f, 1f, 0.8f),
					new Color(0f, 0.5f, 1f, 0.4f),
					new Color(1f, 1f, 1f, 0f),
					new Color(1f, 0.7f, 0f, 0.2f),
					new Color(1f, 0.3f, 0f, 0.6f),
					new Color(1f, 0f, 0f, 0.8f)
				};
				int num = Mathf.Min(6, SceneView.s_MipColorsTexture.mipmapCount);
				for (int i = 0; i < num; i++)
				{
					int num2 = Mathf.Max(SceneView.s_MipColorsTexture.width >> i, 1);
					int num3 = Mathf.Max(SceneView.s_MipColorsTexture.height >> i, 1);
					Color[] array2 = new Color[num2 * num3];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = array[i];
					}
					SceneView.s_MipColorsTexture.SetPixels(array2, i);
				}
				SceneView.s_MipColorsTexture.filterMode = FilterMode.Trilinear;
				SceneView.s_MipColorsTexture.Apply(false);
				Shader.SetGlobalTexture("_SceneViewMipcolorsTexture", SceneView.s_MipColorsTexture);
			}
		}

		public void SetSceneViewFiltering(bool enable)
		{
			this.m_RequestedSceneViewFiltering = enable;
		}

		private bool UseSceneFiltering()
		{
			return !string.IsNullOrEmpty(this.m_SearchFilter) || this.m_RequestedSceneViewFiltering;
		}

		internal bool SceneViewIsRenderingHDR()
		{
			return !this.UseSceneFiltering() && this.m_Camera != null && this.m_Camera.hdr;
		}

		private void HandleClickAndDragToFocus()
		{
			Event current = Event.current;
			if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
			{
				SceneView.s_LastActiveSceneView = this;
			}
			else if (SceneView.s_LastActiveSceneView == null)
			{
				SceneView.s_LastActiveSceneView = this;
			}
			if (current.type == EventType.MouseDrag)
			{
				this.draggingLocked = SceneView.DraggingLockedState.Dragging;
			}
			else if (GUIUtility.hotControl == 0 && this.draggingLocked == SceneView.DraggingLockedState.Dragging)
			{
				this.draggingLocked = SceneView.DraggingLockedState.LookAt;
			}
			if (current.type == EventType.MouseDown)
			{
				Tools.s_ButtonDown = current.button;
				if (current.button == 1 && Application.platform == RuntimePlatform.OSXEditor)
				{
					base.Focus();
				}
			}
		}

		private void SetupFogAndShadowDistance(out bool oldFog, out float oldShadowDistance)
		{
			oldFog = RenderSettings.fog;
			oldShadowDistance = QualitySettings.shadowDistance;
			if (Event.current.type == EventType.Repaint)
			{
				if (!this.m_SceneViewState.showFog)
				{
					Unsupported.SetRenderSettingsUseFogNoDirty(false);
				}
				if (this.m_Camera.orthographic)
				{
					Unsupported.SetQualitySettingsShadowDistanceTemporarily(QualitySettings.shadowDistance + 0.5f * this.cameraDistance);
				}
			}
		}

		private void RestoreFogAndShadowDistance(bool oldFog, float oldShadowDistance)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Unsupported.SetRenderSettingsUseFogNoDirty(oldFog);
				Unsupported.SetQualitySettingsShadowDistanceTemporarily(oldShadowDistance);
			}
		}

		private void CreateCameraTargetTexture(Rect cameraRect, bool hdr)
		{
			bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
			int num = Mathf.Max(1, QualitySettings.antiAliasing);
			if (this.IsSceneCameraDeferred())
			{
				num = 1;
			}
			RenderTextureFormat renderTextureFormat = (!hdr || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf;
			if (this.m_SceneTargetTexture != null)
			{
				bool flag2 = this.m_SceneTargetTexture != null && flag == this.m_SceneTargetTexture.sRGB;
				if (this.m_SceneTargetTexture.format != renderTextureFormat || this.m_SceneTargetTexture.antiAliasing != num || !flag2)
				{
					UnityEngine.Object.DestroyImmediate(this.m_SceneTargetTexture);
					this.m_SceneTargetTexture = null;
				}
			}
			Rect cameraRect2 = Handles.GetCameraRect(cameraRect);
			int num2 = (int)cameraRect2.width;
			int num3 = (int)cameraRect2.height;
			if (this.m_SceneTargetTexture == null)
			{
				this.m_SceneTargetTexture = new RenderTexture(0, 0, 24, renderTextureFormat);
				this.m_SceneTargetTexture.name = "SceneView RT";
				this.m_SceneTargetTexture.antiAliasing = num;
				this.m_SceneTargetTexture.hideFlags = HideFlags.HideAndDontSave;
			}
			if (this.m_SceneTargetTexture.width != num2 || this.m_SceneTargetTexture.height != num3)
			{
				this.m_SceneTargetTexture.Release();
				this.m_SceneTargetTexture.width = num2;
				this.m_SceneTargetTexture.height = num3;
			}
			this.m_SceneTargetTexture.Create();
		}

		internal bool IsSceneCameraDeferred()
		{
			return !(this.m_Camera == null) && (this.m_Camera.actualRenderingPath == RenderingPath.DeferredLighting || this.m_Camera.actualRenderingPath == RenderingPath.DeferredShading);
		}

		internal static bool DoesCameraDrawModeSupportDeferred(DrawCameraMode mode)
		{
			return mode == DrawCameraMode.Normal || mode == DrawCameraMode.Textured || mode == DrawCameraMode.TexturedWire || mode == DrawCameraMode.ShadowCascades || mode == DrawCameraMode.RenderPaths || mode == DrawCameraMode.AlphaChannel || mode == DrawCameraMode.DeferredDiffuse || mode == DrawCameraMode.DeferredSpecular || mode == DrawCameraMode.DeferredSmoothness || mode == DrawCameraMode.DeferredNormal || mode == DrawCameraMode.Charting || mode == DrawCameraMode.Systems || mode == DrawCameraMode.Albedo || mode == DrawCameraMode.Emissive || mode == DrawCameraMode.Irradiance || mode == DrawCameraMode.Directionality || mode == DrawCameraMode.Baked || mode == DrawCameraMode.Clustering || mode == DrawCameraMode.LitClustering;
		}

		internal static bool DoesCameraDrawModeSupportHDR(DrawCameraMode mode)
		{
			return mode == DrawCameraMode.Textured || mode == DrawCameraMode.TexturedWire;
		}

		private void PrepareCameraTargetTexture(Rect cameraRect)
		{
			bool hdr = this.SceneViewIsRenderingHDR();
			this.CreateCameraTargetTexture(cameraRect, hdr);
			this.m_Camera.targetTexture = this.m_SceneTargetTexture;
			if (this.UseSceneFiltering() || !SceneView.DoesCameraDrawModeSupportDeferred(this.m_RenderMode))
			{
				if (this.IsSceneCameraDeferred())
				{
					this.m_Camera.renderingPath = RenderingPath.Forward;
				}
			}
		}

		private void PrepareCameraReplacementShader()
		{
			if (Event.current.type == EventType.Repaint)
			{
				Handles.SetSceneViewColors(SceneView.kSceneViewWire, SceneView.kSceneViewWireOverlay, SceneView.kSceneViewSelectedOutline, SceneView.kSceneViewSelectedWire);
				if (this.m_RenderMode == DrawCameraMode.Overdraw)
				{
					if (!SceneView.s_ShowOverdrawShader)
					{
						SceneView.s_ShowOverdrawShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewShowOverdraw.shader") as Shader);
					}
					this.m_Camera.SetReplacementShader(SceneView.s_ShowOverdrawShader, "RenderType");
				}
				else if (this.m_RenderMode == DrawCameraMode.Mipmaps)
				{
					if (!SceneView.s_ShowMipsShader)
					{
						SceneView.s_ShowMipsShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewShowMips.shader") as Shader);
					}
					if (SceneView.s_ShowMipsShader != null && SceneView.s_ShowMipsShader.isSupported)
					{
						SceneView.CreateMipColorsTexture();
						this.m_Camera.SetReplacementShader(SceneView.s_ShowMipsShader, "RenderType");
					}
					else
					{
						this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
					}
				}
				else
				{
					this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
				}
			}
		}

		private bool SceneCameraRendersIntoRT()
		{
			return this.m_Camera.targetTexture != null || HandleUtility.CameraNeedsToRenderIntoRT(this.m_Camera);
		}

		private void DoDrawCamera(Rect cameraRect, out bool pushedGUIClip)
		{
			pushedGUIClip = false;
			if (this.m_Camera.gameObject.activeInHierarchy)
			{
				DrawGridParameters gridParam = this.grid.PrepareGridRender(this.camera, this.pivot, this.m_Rotation.target, this.m_Size.value, this.m_Ortho.target, AnnotationUtility.showGrid);
				Event current = Event.current;
				if (this.UseSceneFiltering())
				{
					if (current.type == EventType.Repaint)
					{
						Handles.EnableCameraFx(this.m_Camera, true);
						Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowRest);
						float num = Mathf.Clamp01((float)(EditorApplication.timeSinceStartup - this.m_StartSearchFilterTime));
						Handles.DrawCamera(cameraRect, this.m_Camera, this.m_RenderMode);
						Handles.DrawCameraFade(this.m_Camera, num);
						Handles.EnableCameraFx(this.m_Camera, false);
						Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowFiltered);
						if (!SceneView.s_AuraShader)
						{
							SceneView.s_AuraShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewAura.shader") as Shader);
						}
						this.m_Camera.SetReplacementShader(SceneView.s_AuraShader, "");
						Handles.DrawCamera(cameraRect, this.m_Camera, this.m_RenderMode);
						this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
						Handles.DrawCamera(cameraRect, this.m_Camera, this.m_RenderMode, gridParam);
						if (num < 1f)
						{
							base.Repaint();
						}
					}
					if (current.type == EventType.Repaint)
					{
						RenderTexture.active = null;
					}
					GUI.EndGroup();
					GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
					if (current.type == EventType.Repaint)
					{
						GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
						GUI.DrawTexture(cameraRect, this.m_SceneTargetTexture, ScaleMode.StretchToFill, false, 0f);
						GL.sRGBWrite = false;
					}
					Handles.SetCamera(cameraRect, this.m_Camera);
					this.HandleSelectionAndOnSceneGUI();
				}
				else
				{
					if (this.SceneCameraRendersIntoRT())
					{
						GUIClip.Push(new Rect(0f, 0f, base.position.width, base.position.height), Vector2.zero, Vector2.zero, true);
						pushedGUIClip = true;
					}
					Handles.DrawCameraStep1(cameraRect, this.m_Camera, this.m_RenderMode, gridParam);
					this.DrawRenderModeOverlay(cameraRect);
				}
			}
		}

		private void DoClearCamera(Rect cameraRect)
		{
			float verticalFOV = this.GetVerticalFOV(90f);
			float fieldOfView = this.m_Camera.fieldOfView;
			this.m_Camera.fieldOfView = verticalFOV;
			Handles.ClearCamera(cameraRect, this.m_Camera);
			this.m_Camera.fieldOfView = fieldOfView;
		}

		private void SetupCustomSceneLighting()
		{
			if (!this.m_SceneLighting)
			{
				this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
				if (Event.current.type == EventType.Repaint)
				{
					InternalEditorUtility.SetCustomLighting(this.m_Light, SceneView.kSceneViewMidLight);
				}
			}
		}

		private void CleanupCustomSceneLighting()
		{
			if (!this.m_SceneLighting)
			{
				if (Event.current.type == EventType.Repaint)
				{
					InternalEditorUtility.RemoveCustomLighting();
				}
			}
		}

		private void HandleViewToolCursor()
		{
			if (Tools.viewToolActive && Event.current.type == EventType.Repaint)
			{
				MouseCursor mouseCursor = MouseCursor.Arrow;
				switch (Tools.viewTool)
				{
				case ViewTool.Orbit:
					mouseCursor = MouseCursor.Orbit;
					break;
				case ViewTool.Pan:
					mouseCursor = MouseCursor.Pan;
					break;
				case ViewTool.Zoom:
					mouseCursor = MouseCursor.Zoom;
					break;
				case ViewTool.FPS:
					mouseCursor = MouseCursor.FPS;
					break;
				}
				if (mouseCursor != MouseCursor.Arrow)
				{
					SceneView.AddCursorRect(new Rect(0f, 17f, base.position.width, base.position.height - 17f), mouseCursor);
				}
			}
		}

		private static bool ComponentHasImageEffectAttribute(Component c)
		{
			return !(c == null) && Attribute.IsDefined(c.GetType(), typeof(ImageEffectAllowedInSceneView));
		}

		private void UpdateImageEffects(bool enable)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Camera mainCamera = SceneView.GetMainCamera();
				if (!enable || mainCamera == null)
				{
					GameObject arg_57_0 = this.m_Camera.gameObject;
					if (SceneView.<>f__mg$cache2 == null)
					{
						SceneView.<>f__mg$cache2 = new ComponentUtility.IsDesiredComponent(SceneView.ComponentHasImageEffectAttribute);
					}
					ComponentUtility.DestroyComponentsMatching(arg_57_0, SceneView.<>f__mg$cache2);
				}
				else
				{
					GameObject arg_8F_0 = mainCamera.gameObject;
					GameObject arg_8F_1 = this.m_Camera.gameObject;
					if (SceneView.<>f__mg$cache3 == null)
					{
						SceneView.<>f__mg$cache3 = new ComponentUtility.IsDesiredComponent(SceneView.ComponentHasImageEffectAttribute);
					}
					ComponentUtility.ReplaceComponentsIfDifferent(arg_8F_0, arg_8F_1, SceneView.<>f__mg$cache3);
				}
			}
		}

		private void DoOnPreSceneGUICallbacks(Rect cameraRect)
		{
			if (!this.UseSceneFiltering())
			{
				Handles.SetCamera(cameraRect, this.m_Camera);
				this.CallOnPreSceneGUI();
			}
		}

		private void RepaintGizmosThatAreRenderedOnTopOfSceneView()
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.svRot.OnGUI(this);
			}
		}

		private void InputForGizmosThatAreRenderedOnTopOfSceneView()
		{
			if (Event.current.type != EventType.Repaint)
			{
				this.svRot.OnGUI(this);
			}
		}

		internal void OnGUI()
		{
			SceneView.s_CurrentDrawingSceneView = this;
			Event current = Event.current;
			if (current.type == EventType.Repaint)
			{
				SceneView.s_MouseRects.Clear();
				Profiler.BeginSample("SceneView.Repaint");
			}
			Color color = GUI.color;
			this.HandleClickAndDragToFocus();
			if (current.type == EventType.Layout)
			{
				this.m_ShowSceneViewWindows = (SceneView.lastActiveSceneView == this);
			}
			this.m_SceneViewOverlay.Begin();
			bool oldFog;
			float oldShadowDistance;
			this.SetupFogAndShadowDistance(out oldFog, out oldShadowDistance);
			this.DoToolbarGUI();
			GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
			GUI.color = Color.white;
			EditorGUIUtility.labelWidth = 100f;
			this.SetupCamera();
			RenderingPath renderingPath = this.m_Camera.renderingPath;
			this.SetupCustomSceneLighting();
			GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height - 17f);
			Rect cameraRect = EditorGUIUtility.PointsToPixels(rect);
			this.HandleViewToolCursor();
			this.PrepareCameraTargetTexture(cameraRect);
			this.DoClearCamera(cameraRect);
			this.m_Camera.cullingMask = Tools.visibleLayers;
			this.InputForGizmosThatAreRenderedOnTopOfSceneView();
			this.DoOnPreSceneGUICallbacks(cameraRect);
			this.PrepareCameraReplacementShader();
			this.m_MainViewControlID = GUIUtility.GetControlID(FocusType.Keyboard);
			if (current.GetTypeForControl(this.m_MainViewControlID) == EventType.MouseDown)
			{
				GUIUtility.keyboardControl = this.m_MainViewControlID;
			}
			bool flag;
			this.DoDrawCamera(rect, out flag);
			this.CleanupCustomSceneLighting();
			if (!this.UseSceneFiltering())
			{
				Handles.DrawCameraStep2(this.m_Camera, this.m_RenderMode);
				this.HandleSelectionAndOnSceneGUI();
			}
			if (current.type == EventType.ExecuteCommand || current.type == EventType.ValidateCommand)
			{
				this.CommandsGUI();
			}
			this.RestoreFogAndShadowDistance(oldFog, oldShadowDistance);
			this.m_Camera.renderingPath = renderingPath;
			if (this.UseSceneFiltering())
			{
				Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.ShowFiltered);
			}
			else
			{
				Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.Off);
			}
			this.DefaultHandles();
			if (!this.UseSceneFiltering())
			{
				if (current.type == EventType.Repaint)
				{
					Profiler.BeginSample("SceneView.BlitRT");
					Graphics.SetRenderTarget(null);
				}
				if (flag)
				{
					GUIClip.Pop();
				}
				if (current.type == EventType.Repaint)
				{
					GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
					GUI.DrawTexture(rect, this.m_SceneTargetTexture, ScaleMode.StretchToFill, false);
					GL.sRGBWrite = false;
					Profiler.EndSample();
				}
			}
			Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.Off);
			Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.Off);
			this.HandleDragging();
			this.RepaintGizmosThatAreRenderedOnTopOfSceneView();
			if (SceneView.s_LastActiveSceneView == this)
			{
				SceneViewMotion.ArrowKeys(this);
				SceneViewMotion.DoViewTool(this);
			}
			this.Handle2DModeSwitch();
			GUI.EndGroup();
			GUI.color = color;
			this.m_SceneViewOverlay.End();
			this.HandleMouseCursor();
			if (current.type == EventType.Repaint)
			{
				Profiler.EndSample();
			}
			SceneView.s_CurrentDrawingSceneView = null;
		}

		private void Handle2DModeSwitch()
		{
			Event current = Event.current;
			if (SceneView.k2DMode.activated && !SceneView.waitingFor2DModeKeyUp)
			{
				SceneView.waitingFor2DModeKeyUp = true;
				this.in2DMode = !this.in2DMode;
				current.Use();
			}
			else if (current.type == EventType.KeyUp && current.keyCode == SceneView.k2DMode.KeyboardEvent.keyCode)
			{
				SceneView.waitingFor2DModeKeyUp = false;
			}
		}

		private void HandleMouseCursor()
		{
			Event current = Event.current;
			if (GUIUtility.hotControl == 0)
			{
				this.s_DraggingCursorIsCached = false;
			}
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
			if (!this.s_DraggingCursorIsCached)
			{
				MouseCursor mouseCursor = MouseCursor.Arrow;
				if (current.type == EventType.MouseMove || current.type == EventType.Repaint)
				{
					foreach (SceneView.CursorRect current2 in SceneView.s_MouseRects)
					{
						if (current2.rect.Contains(current.mousePosition))
						{
							mouseCursor = current2.cursor;
							rect = current2.rect;
						}
					}
					if (GUIUtility.hotControl != 0)
					{
						this.s_DraggingCursorIsCached = true;
					}
					if (mouseCursor != SceneView.s_LastCursor)
					{
						SceneView.s_LastCursor = mouseCursor;
						InternalEditorUtility.ResetCursor();
						base.Repaint();
					}
				}
			}
			if (current.type == EventType.Repaint && SceneView.s_LastCursor != MouseCursor.Arrow)
			{
				EditorGUIUtility.AddCursorRect(rect, SceneView.s_LastCursor);
			}
		}

		private void DrawRenderModeOverlay(Rect cameraRect)
		{
			if (this.m_RenderMode == DrawCameraMode.AlphaChannel)
			{
				if (!SceneView.s_AlphaOverlayMaterial)
				{
					SceneView.s_AlphaOverlayMaterial = (EditorGUIUtility.LoadRequired("SceneView/SceneViewAlphaMaterial.mat") as Material);
				}
				Handles.BeginGUI();
				if (Event.current.type == EventType.Repaint)
				{
					Graphics.DrawTexture(cameraRect, EditorGUIUtility.whiteTexture, SceneView.s_AlphaOverlayMaterial);
				}
				Handles.EndGUI();
			}
			if (this.m_RenderMode == DrawCameraMode.DeferredDiffuse || this.m_RenderMode == DrawCameraMode.DeferredSpecular || this.m_RenderMode == DrawCameraMode.DeferredSmoothness || this.m_RenderMode == DrawCameraMode.DeferredNormal)
			{
				if (!SceneView.s_DeferredOverlayMaterial)
				{
					SceneView.s_DeferredOverlayMaterial = (EditorGUIUtility.LoadRequired("SceneView/SceneViewDeferredMaterial.mat") as Material);
				}
				Handles.BeginGUI();
				if (Event.current.type == EventType.Repaint)
				{
					SceneView.s_DeferredOverlayMaterial.SetInt("_DisplayMode", this.m_RenderMode - DrawCameraMode.DeferredDiffuse);
					Graphics.DrawTexture(cameraRect, EditorGUIUtility.whiteTexture, SceneView.s_DeferredOverlayMaterial);
				}
				Handles.EndGUI();
			}
		}

		private void HandleSelectionAndOnSceneGUI()
		{
			this.m_RectSelection.OnGUI();
			this.CallOnSceneGUI();
		}

		public void FixNegativeSize()
		{
			float num = 90f;
			if (this.size < 0f)
			{
				float num2 = this.size / Mathf.Tan(num * 0.5f * 0.0174532924f);
				Vector3 a = this.m_Position.value + this.rotation * new Vector3(0f, 0f, -num2);
				this.size = -this.size;
				num2 = this.size / Mathf.Tan(num * 0.5f * 0.0174532924f);
				this.m_Position.value = a + this.rotation * new Vector3(0f, 0f, num2);
			}
		}

		private float CalcCameraDist()
		{
			float num = this.m_Ortho.Fade(90f, 0f);
			float result;
			if (num > 3f)
			{
				this.m_Camera.orthographic = false;
				result = this.size / Mathf.Tan(num * 0.5f * 0.0174532924f);
			}
			else
			{
				result = 0f;
			}
			return result;
		}

		private void ResetIfNaN()
		{
			if (float.IsInfinity(this.m_Position.value.x) || float.IsNaN(this.m_Position.value.x))
			{
				this.m_Position.value = Vector3.zero;
			}
			if (float.IsInfinity(this.m_Rotation.value.x) || float.IsNaN(this.m_Rotation.value.x))
			{
				this.m_Rotation.value = Quaternion.identity;
			}
		}

		internal static Camera GetMainCamera()
		{
			Camera main = Camera.main;
			Camera result;
			if (main != null)
			{
				result = main;
			}
			else
			{
				Camera[] allCameras = Camera.allCameras;
				if (allCameras != null && allCameras.Length == 1)
				{
					result = allCameras[0];
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		internal static RenderingPath GetSceneViewRenderingPath()
		{
			Camera mainCamera = SceneView.GetMainCamera();
			RenderingPath result;
			if (mainCamera != null)
			{
				result = mainCamera.renderingPath;
			}
			else
			{
				result = RenderingPath.UsePlayerSettings;
			}
			return result;
		}

		internal static bool IsUsingDeferredRenderingPath()
		{
			RenderingPath sceneViewRenderingPath = SceneView.GetSceneViewRenderingPath();
			return sceneViewRenderingPath == RenderingPath.DeferredShading || (sceneViewRenderingPath == RenderingPath.UsePlayerSettings && EditorGraphicsSettings.GetCurrentTierSettings().renderingPath == RenderingPath.DeferredShading);
		}

		internal bool CheckDrawModeForRenderingPath(DrawCameraMode mode)
		{
			RenderingPath actualRenderingPath = this.m_Camera.actualRenderingPath;
			return (mode != DrawCameraMode.DeferredDiffuse && mode != DrawCameraMode.DeferredSpecular && mode != DrawCameraMode.DeferredSmoothness && mode != DrawCameraMode.DeferredNormal) || actualRenderingPath == RenderingPath.DeferredShading;
		}

		private void SetSceneCameraHDRAndDepthModes()
		{
			if (!this.m_SceneLighting || !SceneView.DoesCameraDrawModeSupportHDR(this.m_RenderMode))
			{
				this.m_Camera.hdr = false;
				this.m_Camera.depthTextureMode = DepthTextureMode.None;
				this.m_Camera.clearStencilAfterLightingPass = false;
			}
			else
			{
				Camera mainCamera = SceneView.GetMainCamera();
				if (mainCamera == null)
				{
					this.m_Camera.hdr = false;
					this.m_Camera.depthTextureMode = DepthTextureMode.None;
					this.m_Camera.clearStencilAfterLightingPass = false;
				}
				else
				{
					this.m_Camera.hdr = mainCamera.hdr;
					this.m_Camera.depthTextureMode = mainCamera.depthTextureMode;
					this.m_Camera.clearStencilAfterLightingPass = mainCamera.clearStencilAfterLightingPass;
				}
			}
		}

		private void SetupCamera()
		{
			if (this.m_RenderMode == DrawCameraMode.Overdraw)
			{
				this.m_Camera.backgroundColor = Color.black;
			}
			else
			{
				this.m_Camera.backgroundColor = SceneView.kSceneViewBackground;
			}
			if (Event.current.type == EventType.Repaint)
			{
				this.UpdateImageEffects(!this.UseSceneFiltering() && this.m_RenderMode == DrawCameraMode.Textured && this.m_SceneViewState.showImageEffects);
			}
			EditorUtility.SetCameraAnimateMaterials(this.m_Camera, this.m_SceneViewState.showMaterialUpdate);
			this.ResetIfNaN();
			this.m_Camera.transform.rotation = this.m_Rotation.value;
			float num = this.m_Ortho.Fade(90f, 0f);
			if (num > 3f)
			{
				this.m_Camera.orthographic = false;
				this.m_Camera.fieldOfView = this.GetVerticalFOV(num);
			}
			else
			{
				this.m_Camera.orthographic = true;
				this.m_Camera.orthographicSize = this.GetVerticalOrthoSize();
			}
			this.m_Camera.transform.position = this.m_Position.value + this.m_Camera.transform.rotation * new Vector3(0f, 0f, -this.cameraDistance);
			float num2 = Mathf.Max(1000f, 2000f * this.size);
			this.m_Camera.nearClipPlane = num2 * 5E-06f;
			this.m_Camera.farClipPlane = num2;
			this.m_Camera.renderingPath = SceneView.GetSceneViewRenderingPath();
			if (!this.CheckDrawModeForRenderingPath(this.m_RenderMode))
			{
				this.m_RenderMode = DrawCameraMode.Textured;
			}
			this.SetSceneCameraHDRAndDepthModes();
			if (this.m_RenderMode == DrawCameraMode.Textured || this.m_RenderMode == DrawCameraMode.TexturedWire)
			{
				Handles.EnableCameraFlares(this.m_Camera, this.m_SceneViewState.showFlares);
				Handles.EnableCameraSkybox(this.m_Camera, this.m_SceneViewState.showSkybox);
			}
			else
			{
				Handles.EnableCameraFlares(this.m_Camera, false);
				Handles.EnableCameraSkybox(this.m_Camera, false);
			}
			this.m_Light[0].transform.position = this.m_Camera.transform.position;
			this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
			if (this.m_AudioPlay)
			{
				AudioUtil.SetListenerTransform(this.m_Camera.transform);
				AudioUtil.UpdateAudio();
			}
			if (this.m_ViewIsLockedToObject && Selection.gameObjects.Length > 0)
			{
				SceneView.DraggingLockedState draggingLockedState = this.m_DraggingLockedState;
				if (draggingLockedState != SceneView.DraggingLockedState.Dragging)
				{
					if (draggingLockedState != SceneView.DraggingLockedState.LookAt)
					{
						if (draggingLockedState == SceneView.DraggingLockedState.NotDragging)
						{
							this.m_Position.value = Selection.activeGameObject.transform.position;
						}
					}
					else if (!this.m_Position.value.Equals(Selection.activeGameObject.transform.position))
					{
						if (!EditorApplication.isPlaying)
						{
							this.m_Position.target = Selection.activeGameObject.transform.position;
						}
						else
						{
							this.m_Position.value = Selection.activeGameObject.transform.position;
						}
					}
					else
					{
						this.m_DraggingLockedState = SceneView.DraggingLockedState.NotDragging;
					}
				}
			}
		}

		private void OnBecameVisible()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedMaterials));
		}

		private void OnBecameInvisible()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateAnimatedMaterials));
		}

		private void UpdateAnimatedMaterials()
		{
			if (this.m_SceneViewState.showMaterialUpdate && this.m_lastRenderedTime + 0.032999999821186066 < EditorApplication.timeSinceStartup)
			{
				this.m_lastRenderedTime = EditorApplication.timeSinceStartup;
				base.Repaint();
			}
		}

		internal float GetVerticalFOV(float aspectNeutralFOV)
		{
			float f = Mathf.Tan(aspectNeutralFOV * 0.5f * 0.0174532924f) * 0.707106769f / Mathf.Sqrt(this.m_Camera.aspect);
			return Mathf.Atan(f) * 2f * 57.29578f;
		}

		internal float GetVerticalOrthoSize()
		{
			return this.size * 0.707106769f / Mathf.Sqrt(this.m_Camera.aspect);
		}

		public void LookAt(Vector3 pos)
		{
			this.FixNegativeSize();
			this.m_Position.target = pos;
		}

		public void LookAt(Vector3 pos, Quaternion rot)
		{
			this.FixNegativeSize();
			this.m_Position.target = pos;
			this.m_Rotation.target = rot;
			this.svRot.UpdateGizmoLabel(this, rot * Vector3.forward, this.m_Ortho.target);
		}

		public void LookAtDirect(Vector3 pos, Quaternion rot)
		{
			this.FixNegativeSize();
			this.m_Position.value = pos;
			this.m_Rotation.value = rot;
			this.svRot.UpdateGizmoLabel(this, rot * Vector3.forward, this.m_Ortho.target);
		}

		public void LookAt(Vector3 pos, Quaternion rot, float newSize)
		{
			this.FixNegativeSize();
			this.m_Position.target = pos;
			this.m_Rotation.target = rot;
			this.m_Size.target = Mathf.Abs(newSize);
			this.svRot.UpdateGizmoLabel(this, rot * Vector3.forward, this.m_Ortho.target);
		}

		public void LookAtDirect(Vector3 pos, Quaternion rot, float newSize)
		{
			this.FixNegativeSize();
			this.m_Position.value = pos;
			this.m_Rotation.value = rot;
			this.m_Size.value = Mathf.Abs(newSize);
			this.svRot.UpdateGizmoLabel(this, rot * Vector3.forward, this.m_Ortho.target);
		}

		public void LookAt(Vector3 pos, Quaternion rot, float newSize, bool ortho)
		{
			this.LookAt(pos, rot, newSize, ortho, false);
		}

		public void LookAt(Vector3 pos, Quaternion rot, float newSize, bool ortho, bool instant)
		{
			this.FixNegativeSize();
			if (instant)
			{
				this.m_Position.value = pos;
				this.m_Rotation.value = rot;
				this.m_Size.value = Mathf.Abs(newSize);
				this.m_Ortho.value = ortho;
			}
			else
			{
				this.m_Position.target = pos;
				this.m_Rotation.target = rot;
				this.m_Size.target = Mathf.Abs(newSize);
				this.m_Ortho.target = ortho;
			}
			this.svRot.UpdateGizmoLabel(this, rot * Vector3.forward, this.m_Ortho.target);
		}

		private void DefaultHandles()
		{
			EditorGUI.BeginChangeCheck();
			bool flag = Event.current.GetTypeForControl(GUIUtility.hotControl) == EventType.MouseDrag;
			bool flag2 = Event.current.GetTypeForControl(GUIUtility.hotControl) == EventType.MouseUp;
			if (GUIUtility.hotControl == 0)
			{
				SceneView.s_CurrentTool = ((!Tools.viewToolActive) ? Tools.current : Tool.View);
			}
			Tool tool = (Event.current.type != EventType.Repaint) ? SceneView.s_CurrentTool : Tools.current;
			switch (tool + 1)
			{
			case Tool.Rotate:
				MoveTool.OnGUI(this);
				break;
			case Tool.Scale:
				RotateTool.OnGUI(this);
				break;
			case Tool.Rect:
				ScaleTool.OnGUI(this);
				break;
			case (Tool)5:
				RectTool.OnGUI(this);
				break;
			}
			if (EditorGUI.EndChangeCheck() && EditorApplication.isPlaying && flag)
			{
				Physics2D.SetEditorDragMovement(true, Selection.gameObjects);
			}
			if (EditorApplication.isPlaying && flag2)
			{
				Physics2D.SetEditorDragMovement(false, Selection.gameObjects);
			}
		}

		private void CleanupEditorDragFunctions()
		{
			if (this.m_DragEditorCache != null)
			{
				this.m_DragEditorCache.Dispose();
			}
			this.m_DragEditorCache = null;
		}

		private void CallEditorDragFunctions()
		{
			Event current = Event.current;
			SpriteUtility.OnSceneDrag(this);
			if (current.type != EventType.Used)
			{
				if (DragAndDrop.objectReferences.Length != 0)
				{
					if (this.m_DragEditorCache == null)
					{
						this.m_DragEditorCache = new EditorCache(EditorFeatures.OnSceneDrag);
					}
					UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
					for (int i = 0; i < objectReferences.Length; i++)
					{
						UnityEngine.Object @object = objectReferences[i];
						if (!(@object == null))
						{
							EditorWrapper editorWrapper = this.m_DragEditorCache[@object];
							if (editorWrapper != null)
							{
								editorWrapper.OnSceneDrag(this);
							}
							if (current.type == EventType.Used)
							{
								break;
							}
						}
					}
				}
			}
		}

		private void HandleDragging()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.DragPerform && type != EventType.DragUpdated)
			{
				if (type == EventType.DragExited)
				{
					this.CallEditorDragFunctions();
					this.CleanupEditorDragFunctions();
				}
			}
			else
			{
				this.CallEditorDragFunctions();
				if (current.type != EventType.Used)
				{
					bool flag = current.type == EventType.DragPerform;
					if (DragAndDrop.visualMode != DragAndDropVisualMode.Copy)
					{
						GameObject dropUpon = HandleUtility.PickGameObject(Event.current.mousePosition, true);
						DragAndDrop.visualMode = InternalEditorUtility.SceneViewDrag(dropUpon, this.pivot, Event.current.mousePosition, flag);
					}
					if (flag && DragAndDrop.visualMode != DragAndDropVisualMode.None)
					{
						DragAndDrop.AcceptDrag();
						current.Use();
						GUIUtility.ExitGUI();
					}
					current.Use();
				}
			}
		}

		private void CommandsGUI()
		{
			bool flag = Event.current.type == EventType.ExecuteCommand;
			string commandName = Event.current.commandName;
			switch (commandName)
			{
			case "Find":
				if (flag)
				{
					base.FocusSearchField();
				}
				Event.current.Use();
				break;
			case "FrameSelected":
				if (flag)
				{
					bool lockView = EditorApplication.timeSinceStartup - this.lastFramingTime < 0.5;
					this.FrameSelected(lockView);
					this.lastFramingTime = EditorApplication.timeSinceStartup;
				}
				Event.current.Use();
				break;
			case "FrameSelectedWithLock":
				if (flag)
				{
					this.FrameSelected(true);
				}
				Event.current.Use();
				break;
			case "SoftDelete":
			case "Delete":
				if (flag)
				{
					Unsupported.DeleteGameObjectSelection();
				}
				Event.current.Use();
				break;
			case "Duplicate":
				if (flag)
				{
					Unsupported.DuplicateGameObjectsUsingPasteboard();
				}
				Event.current.Use();
				break;
			case "Copy":
				if (flag)
				{
					Unsupported.CopyGameObjectsToPasteboard();
				}
				Event.current.Use();
				break;
			case "Paste":
				if (flag)
				{
					Unsupported.PasteGameObjectsFromPasteboard();
				}
				Event.current.Use();
				break;
			case "SelectAll":
				if (flag)
				{
					Selection.objects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
				}
				Event.current.Use();
				break;
			}
		}

		public void AlignViewToObject(Transform t)
		{
			this.FixNegativeSize();
			this.size = 10f;
			this.LookAt(t.position + t.forward * this.CalcCameraDist(), t.rotation);
		}

		public void AlignWithView()
		{
			this.FixNegativeSize();
			Vector3 position = this.camera.transform.position;
			Vector3 b = position - Tools.handlePosition;
			float angle;
			Vector3 vector;
			(Quaternion.Inverse(Selection.activeTransform.rotation) * this.camera.transform.rotation).ToAngleAxis(out angle, out vector);
			vector = Selection.activeTransform.TransformDirection(vector);
			Undo.RecordObjects(Selection.transforms, "Align with view");
			Transform[] transforms = Selection.transforms;
			for (int i = 0; i < transforms.Length; i++)
			{
				Transform transform = transforms[i];
				transform.position += b;
				transform.RotateAround(position, vector, angle);
			}
		}

		public void MoveToView()
		{
			this.FixNegativeSize();
			Vector3 b = this.pivot - Tools.handlePosition;
			Undo.RecordObjects(Selection.transforms, "Move to view");
			Transform[] transforms = Selection.transforms;
			for (int i = 0; i < transforms.Length; i++)
			{
				Transform transform = transforms[i];
				transform.position += b;
			}
		}

		public void MoveToView(Transform target)
		{
			target.position = this.pivot;
		}

		public bool FrameSelected()
		{
			return this.FrameSelected(false);
		}

		public bool FrameSelected(bool lockView)
		{
			this.viewIsLockedToObject = lockView;
			this.FixNegativeSize();
			Bounds bounds = InternalEditorUtility.CalculateSelectionBounds(false, Tools.pivotMode == PivotMode.Pivot);
			Editor[] activeEditors = this.GetActiveEditors();
			for (int i = 0; i < activeEditors.Length; i++)
			{
				Editor editor = activeEditors[i];
				MethodInfo method = editor.GetType().GetMethod("HasFrameBounds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (method != null)
				{
					object obj = method.Invoke(editor, null);
					if (obj is bool && (bool)obj)
					{
						MethodInfo method2 = editor.GetType().GetMethod("OnGetFrameBounds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
						if (method2 != null)
						{
							object obj2 = method2.Invoke(editor, null);
							if (obj2 is Bounds)
							{
								bounds = (Bounds)obj2;
							}
						}
					}
				}
			}
			return this.Frame(bounds);
		}

		internal bool Frame(Bounds bounds)
		{
			float num = bounds.extents.magnitude * 1.5f;
			bool result;
			if (num == float.PositiveInfinity)
			{
				result = false;
			}
			else
			{
				if (num == 0f)
				{
					num = 10f;
				}
				this.LookAt(bounds.center, this.m_Rotation.target, num * 2.2f, this.m_Ortho.value, EditorApplication.isPlaying);
				result = true;
			}
			return result;
		}

		private void CreateSceneCameraAndLights()
		{
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("SceneCamera", HideFlags.HideAndDontSave, new Type[]
			{
				typeof(Camera)
			});
			gameObject.AddComponentInternal("FlareLayer");
			gameObject.AddComponentInternal("HaloLayer");
			this.m_Camera = gameObject.GetComponent<Camera>();
			this.m_Camera.enabled = false;
			this.m_Camera.cameraType = CameraType.SceneView;
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject2 = EditorUtility.CreateGameObjectWithHideFlags("SceneLight", HideFlags.HideAndDontSave, new Type[]
				{
					typeof(Light)
				});
				this.m_Light[i] = gameObject2.GetComponent<Light>();
				this.m_Light[i].type = LightType.Directional;
				this.m_Light[i].intensity = 1f;
				this.m_Light[i].enabled = false;
			}
			this.m_Light[0].color = SceneView.kSceneViewFrontLight;
			this.m_Light[1].color = SceneView.kSceneViewUpLight - SceneView.kSceneViewMidLight;
			this.m_Light[1].transform.LookAt(Vector3.down);
			this.m_Light[1].renderMode = LightRenderMode.ForceVertex;
			this.m_Light[2].color = SceneView.kSceneViewDownLight - SceneView.kSceneViewMidLight;
			this.m_Light[2].transform.LookAt(Vector3.up);
			this.m_Light[2].renderMode = LightRenderMode.ForceVertex;
			HandleUtility.handleMaterial.SetColor("_SkyColor", SceneView.kSceneViewUpLight * 1.5f);
			HandleUtility.handleMaterial.SetColor("_GroundColor", SceneView.kSceneViewDownLight * 1.5f);
			HandleUtility.handleMaterial.SetColor("_Color", SceneView.kSceneViewFrontLight * 1.5f);
		}

		private void CallOnSceneGUI()
		{
			Editor[] activeEditors = this.GetActiveEditors();
			for (int i = 0; i < activeEditors.Length; i++)
			{
				Editor editor = activeEditors[i];
				if (EditorGUIUtility.IsGizmosAllowedForObject(editor.target))
				{
					MethodInfo method = editor.GetType().GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					if (method != null)
					{
						Editor.m_AllowMultiObjectAccess = true;
						for (int j = 0; j < editor.targets.Length; j++)
						{
							this.ResetOnSceneGUIState();
							editor.referenceTargetIndex = j;
							EditorGUI.BeginChangeCheck();
							Editor.m_AllowMultiObjectAccess = !editor.canEditMultipleObjects;
							method.Invoke(editor, null);
							Editor.m_AllowMultiObjectAccess = true;
							if (EditorGUI.EndChangeCheck())
							{
								editor.serializedObject.SetIsDifferentCacheDirty();
							}
						}
						this.ResetOnSceneGUIState();
					}
				}
			}
			if (SceneView.onSceneGUIDelegate != null)
			{
				SceneView.onSceneGUIDelegate(this);
				this.ResetOnSceneGUIState();
			}
		}

		private void ResetOnSceneGUIState()
		{
			Handles.matrix = Matrix4x4.identity;
			HandleUtility.s_CustomPickDistance = 5f;
			EditorGUIUtility.ResetGUIState();
			GUI.color = Color.white;
		}

		private void CallOnPreSceneGUI()
		{
			Editor[] activeEditors = this.GetActiveEditors();
			for (int i = 0; i < activeEditors.Length; i++)
			{
				Editor editor = activeEditors[i];
				Handles.matrix = Matrix4x4.identity;
				Component component = editor.target as Component;
				if (!component || component.gameObject.activeInHierarchy)
				{
					MethodInfo method = editor.GetType().GetMethod("OnPreSceneGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
					if (method != null)
					{
						Editor.m_AllowMultiObjectAccess = true;
						for (int j = 0; j < editor.targets.Length; j++)
						{
							editor.referenceTargetIndex = j;
							Editor.m_AllowMultiObjectAccess = !editor.canEditMultipleObjects;
							method.Invoke(editor, null);
							Editor.m_AllowMultiObjectAccess = true;
						}
					}
				}
			}
			if (SceneView.onPreSceneGUIDelegate != null)
			{
				SceneView.onPreSceneGUIDelegate(this);
			}
			Handles.matrix = Matrix4x4.identity;
		}

		internal static void ShowNotification(string notificationText)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneView));
			List<EditorWindow> list = new List<EditorWindow>();
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				SceneView sceneView = (SceneView)array2[i];
				if (sceneView.m_Parent is DockArea)
				{
					DockArea dockArea = (DockArea)sceneView.m_Parent;
					if (dockArea)
					{
						if (dockArea.actualView == sceneView)
						{
							list.Add(sceneView);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				foreach (EditorWindow current in list)
				{
					current.ShowNotification(GUIContent.Temp(notificationText));
				}
			}
			else
			{
				Debug.LogError(notificationText);
			}
		}

		public static void ShowCompileErrorNotification()
		{
			SceneView.ShowNotification("All compiler errors have to be fixed before you can enter playmode!");
		}

		internal static void ShowSceneViewPlayModeSaveWarning()
		{
			GameView gameView = (GameView)WindowLayout.FindEditorWindowOfType(typeof(GameView));
			if (gameView != null)
			{
				gameView.ShowNotification(new GUIContent("You must exit play mode to save the scene!"));
			}
			else
			{
				SceneView.ShowNotification("You must exit play mode to save the scene!");
			}
		}

		private void ResetToDefaults(EditorBehaviorMode behaviorMode)
		{
			if (behaviorMode != EditorBehaviorMode.Mode2D)
			{
				if (behaviorMode != EditorBehaviorMode.Mode3D)
				{
				}
				this.m_2DMode = false;
				this.m_Rotation.value = SceneView.kDefaultRotation;
				this.m_Position.value = SceneView.kDefaultPivot;
				this.m_Size.value = 10f;
				this.m_Ortho.value = false;
			}
			else
			{
				this.m_2DMode = true;
				this.m_Rotation.value = Quaternion.identity;
				this.m_Position.value = SceneView.kDefaultPivot;
				this.m_Size.value = 10f;
				this.m_Ortho.value = true;
				this.m_LastSceneViewRotation = SceneView.kDefaultRotation;
				this.m_LastSceneViewOrtho = false;
			}
		}

		internal void OnNewProjectLayoutWasCreated()
		{
			this.ResetToDefaults(EditorSettings.defaultBehaviorMode);
		}

		private void On2DModeChange()
		{
			if (this.m_2DMode)
			{
				this.lastSceneViewRotation = this.rotation;
				this.m_LastSceneViewOrtho = this.orthographic;
				this.LookAt(this.pivot, Quaternion.identity, this.size, true);
				if (Tools.current == Tool.Move)
				{
					Tools.current = Tool.Rect;
				}
			}
			else
			{
				this.LookAt(this.pivot, this.lastSceneViewRotation, this.size, this.m_LastSceneViewOrtho);
				if (Tools.current == Tool.Rect)
				{
					Tools.current = Tool.Move;
				}
			}
			HandleUtility.ignoreRaySnapObjects = null;
			Tools.vertexDragging = false;
			Tools.handleOffset = Vector3.zero;
		}

		internal static void Report2DAnalytics()
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(SceneView));
			if (array.Length == 1)
			{
				SceneView sceneView = array[0] as SceneView;
				if (sceneView.in2DMode)
				{
					UsabilityAnalytics.Event("2D", "SceneView", "Single 2D", 1);
				}
			}
		}
	}
}
