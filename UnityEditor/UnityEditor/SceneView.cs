using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace UnityEditor
{
	public class SceneView : SearchableEditorWindow
	{
		[Serializable]
		public class SceneViewState
		{
			public bool showFog = true;
			public bool showMaterialUpdate;
			public bool showSkybox = true;
			public bool showFlares = true;
			public SceneViewState()
			{
			}
			public SceneViewState(SceneView.SceneViewState other)
			{
				this.showFog = other.showFog;
				this.showMaterialUpdate = other.showMaterialUpdate;
				this.showSkybox = other.showSkybox;
				this.showFlares = other.showFlares;
			}
			public bool IsAllOn()
			{
				return this.showFog && this.showMaterialUpdate && this.showSkybox && this.showFlares;
			}
			public void Toggle(bool value)
			{
				this.showFog = value;
				this.showMaterialUpdate = value;
				this.showSkybox = value;
				this.showFlares = value;
			}
		}
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
		public delegate void OnSceneFunc(SceneView sceneView);
		private const float kOrthoThresholdAngle = 3f;
		private const float kOneOverSqrt2 = 0.707106769f;
		private const double k_MaxDoubleKeypressTime = 0.5;
		private const float kPerspectiveFov = 90f;
		public const float kToolbarHeight = 17f;
		private static SceneView s_LastActiveSceneView;
		private static SceneView s_CurrentDrawingSceneView;
		private static PrefColor kSceneViewBackground = new PrefColor("Scene/Background", 0.278431f, 0.278431f, 0.278431f, 0f);
		private static PrefColor kSceneViewWire = new PrefColor("Scene/Wireframe", 0f, 0f, 0f, 0.5f);
		private static PrefColor kSceneViewWireOverlay = new PrefColor("Scene/Wireframe Overlay", 0f, 0f, 0f, 0.25f);
		private static PrefColor kSceneViewWireActive = new PrefColor("Scene/Wireframe Active", 0.490196079f, 0.6901961f, 0.980392158f, 0.372549027f);
		private static PrefColor kSceneViewWireSelected = new PrefColor("Scene/Wireframe Selected", 0.368627459f, 0.466666669f, 0.607843161f, 0.25f);
		internal static Color kSceneViewFrontLight = new Color(0.769f, 0.769f, 0.769f, 1f);
		internal static Color kSceneViewUpLight = new Color(0.212f, 0.227f, 0.259f, 1f);
		internal static Color kSceneViewMidLight = new Color(0.114f, 0.125f, 0.133f, 1f);
		internal static Color kSceneViewDownLight = new Color(0.047f, 0.043f, 0.035f, 1f);
		[NonSerialized]
		private static readonly Quaternion kDefaultRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
		[NonSerialized]
		private static readonly float kDefaultViewSize = 10f;
		[NonSerialized]
		private static readonly Vector3 kDefaultPivot = Vector3.zero;
		[NonSerialized]
		private ActiveEditorTracker m_Tracker;
		public bool m_SceneLighting;
		public double lastFramingTime;
		private static PrefKey k2DMode = new PrefKey("Tools/2D Mode", "2");
		private static bool waitingFor2DModeKeyUp;
		[SerializeField]
		private bool m_2DMode;
		internal UnityEngine.Object m_OneClickDragObject;
		[NonSerialized]
		public bool m_AudioPlay;
		private static SceneView s_AudioSceneView = null;
		[SerializeField]
		private AnimVector3 m_Position = new AnimVector3(SceneView.kDefaultPivot);
		private static string[] s_RenderModes = new string[]
		{
			"Textured",
			"Wireframe",
			"Textured Wire",
			"Render Paths",
			"Lightmap Resolution"
		};
		private static string[] s_OverlayModes = new string[]
		{
			"RGB",
			"Alpha",
			"Overdraw",
			"Mipmaps"
		};
		public static SceneView.OnSceneFunc onSceneGUIDelegate;
		public DrawCameraMode m_RenderMode;
		public int m_OverlayMode;
		[SerializeField]
		internal SceneView.SceneViewState m_SceneViewState;
		[SerializeField]
		private SceneViewGrid grid;
		[SerializeField]
		internal SceneViewRotation svRot;
		[SerializeField]
		internal AnimQuaternion m_Rotation = new AnimQuaternion(SceneView.kDefaultRotation);
		[SerializeField]
		private AnimFloat m_Size = new AnimFloat(SceneView.kDefaultViewSize);
		[SerializeField]
		internal AnimBool m_Ortho = new AnimBool();
		[NonSerialized]
		private Camera m_Camera;
		[SerializeField]
		private Quaternion m_LastSceneViewRotation;
		[SerializeField]
		private bool m_LastSceneViewOrtho;
		private static MouseCursor s_LastCursor = MouseCursor.Arrow;
		private static List<SceneView.CursorRect> s_MouseRects = new List<SceneView.CursorRect>();
		private bool s_DraggingCursorIsCashed;
		[NonSerialized]
		private Light[] m_Light = new Light[3];
		private RectSelection m_RectSelection;
		private static ArrayList s_SceneViews = new ArrayList();
		private static Material s_AlphaOverlayMaterial;
		private static Shader s_ShowOverdrawShader;
		private static Shader s_ShowMipsShader;
		private static Shader s_ShowLightmapsShader;
		private static Shader s_AuraShader;
		private static Shader s_GrayScaleShader;
		private static Texture2D s_MipColorsTexture;
		private static GUIContent s_Fx = new GUIContent("Effects");
		private static GUIContent s_Lighting = EditorGUIUtility.IconContent("SceneviewLighting");
		private static GUIContent s_AudioPlay = EditorGUIUtility.IconContent("SceneviewAudio");
		private static GUIContent s_GizmosContent = new GUIContent("Gizmos");
		private static GUIContent s_2DMode = new GUIContent("2D");
		private static Tool s_CurrentTool;
		private double m_StartSearchFilterTime = -1.0;
		private RenderTexture m_SearchFilterTexture;
		private int m_MainViewControlID;
		[SerializeField]
		private Shader m_ReplacementShader;
		[SerializeField]
		private string m_ReplacementString;
		internal bool m_ShowSceneViewWindows;
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
				if (!this.camera.orthographic)
				{
					return this.size / Mathf.Tan(num * 0.5f * 0.0174532924f);
				}
				return this.size * 2f;
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
			base.depthBufferBits = 32;
			base.antiAlias = -1;
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
			foreach (SceneView sceneView in SceneView.s_SceneViews)
			{
				sceneView.Repaint();
			}
		}
		internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode, bool setAll)
		{
			if (this.m_SearchFilter == string.Empty || searchFilter == string.Empty)
			{
				this.m_StartSearchFilterTime = EditorApplication.timeSinceStartup;
			}
			base.SetSearchFilter(searchFilter, searchMode, setAll);
		}
		private void OnFocus()
		{
			if (!Application.isPlaying && this.m_AudioPlay)
			{
				this.ToggleAudio();
			}
		}
		private void OnLostFocus()
		{
			GameView gameView = (GameView)WindowLayout.FindEditorWindowOfType(typeof(GameView));
			if (gameView && gameView.m_Parent != null && this.m_Parent != null && gameView.m_Parent == this.m_Parent)
			{
				gameView.m_Parent.backgroundValid = false;
			}
		}
		public override void OnEnable()
		{
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
			this.m_SceneViewOverlay = new SceneViewOverlay(this);
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(SceneView.RepaintAll));
			this.m_DraggingLockedState = SceneView.DraggingLockedState.NotDragging;
			base.OnEnable();
		}
		private void Awake()
		{
			if (this.m_SceneViewState == null)
			{
				this.m_SceneViewState = new SceneView.SceneViewState();
			}
			if (!BuildPipeline.isBuildingPlayer)
			{
				this.m_SceneLighting = InternalEditorUtility.CalculateShouldEnableLights();
			}
			if (EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
			{
				this.m_2DMode = true;
				this.m_LastSceneViewRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
				this.m_LastSceneViewOrtho = false;
				this.m_Rotation.value = Quaternion.identity;
				this.m_Ortho.value = true;
			}
		}
		private static void OnForceEnableLights()
		{
			foreach (SceneView sceneView in SceneView.s_SceneViews)
			{
				sceneView.m_SceneLighting = true;
			}
		}
		private void OnDidOpenScene()
		{
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}
			foreach (SceneView sceneView in SceneView.s_SceneViews)
			{
				sceneView.m_SceneLighting = InternalEditorUtility.CalculateShouldEnableLights();
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
			EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(SceneView.RepaintAll));
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
				this.ToggleAudio();
			}
		}
		private void DoStatusBarGUI()
		{
			GUILayout.BeginHorizontal("toolbar", new GUILayoutOption[0]);
			this.m_RenderMode = (DrawCameraMode)EditorGUILayout.Popup((int)this.m_RenderMode, SceneView.s_RenderModes, "ToolbarPopup", new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			EditorGUILayout.Space();
			bool enabled = GUI.enabled;
			GUI.enabled = string.IsNullOrEmpty(this.m_SearchFilter);
			this.m_OverlayMode = EditorGUILayout.Popup(this.m_OverlayMode, SceneView.s_OverlayModes, "ToolbarPopup", new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			});
			GUI.enabled = enabled;
			EditorGUILayout.Space();
			this.in2DMode = GUILayout.Toggle(this.in2DMode, SceneView.s_2DMode, "toolbarbutton", new GUILayoutOption[0]);
			this.m_SceneLighting = GUILayout.Toggle(this.m_SceneLighting, SceneView.s_Lighting, "toolbarbutton", new GUILayoutOption[0]);
			GUI.enabled = !Application.isPlaying;
			GUI.changed = false;
			this.m_AudioPlay = GUILayout.Toggle(this.m_AudioPlay, SceneView.s_AudioPlay, EditorStyles.toolbarButton, new GUILayoutOption[0]);
			if (GUI.changed)
			{
				this.ToggleAudio();
			}
			GUI.enabled = true;
			Rect rect = GUILayoutUtility.GetRect(SceneView.s_Fx, this.effectsDropDownStyle);
			Rect position = new Rect(rect.xMax - (float)this.effectsDropDownStyle.border.right, rect.y, (float)this.effectsDropDownStyle.border.right, rect.height);
			if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				if (SceneFXWindow.ShowAtPosition(last, this))
				{
					GUIUtility.ExitGUI();
				}
			}
			bool flag = GUI.Toggle(rect, this.m_SceneViewState.IsAllOn(), SceneView.s_Fx, this.effectsDropDownStyle);
			if (flag != this.m_SceneViewState.IsAllOn())
			{
				this.m_SceneViewState.Toggle(flag);
			}
			GUILayout.Space(6f);
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
			Rect rect2 = GUILayoutUtility.GetRect(SceneView.s_GizmosContent, EditorStyles.toolbarDropDown);
			if (EditorGUI.ButtonMouseDown(rect2, SceneView.s_GizmosContent, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				Rect last2 = GUILayoutUtility.topLevel.GetLast();
				if (AnnotationWindow.ShowAtPosition(last2, false))
				{
					GUIUtility.ExitGUI();
				}
			}
			GUILayout.Space(6f);
			base.SearchFieldGUI();
			GUILayout.EndHorizontal();
		}
		private void ToggleAudio()
		{
			if (SceneView.s_AudioSceneView != null && SceneView.s_AudioSceneView != this && SceneView.s_AudioSceneView.m_AudioPlay)
			{
				SceneView.s_AudioSceneView.m_AudioPlay = false;
				SceneView.s_AudioSceneView.Repaint();
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
					else
					{
						if (!audioSource.isPlaying)
						{
							audioSource.Play();
						}
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
		[MenuItem("GameObject/Set as first sibling %=")]
		private static void MenuMoveToFront()
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
		private static bool ValidateMenuMoveToFront()
		{
			if (Selection.activeTransform != null)
			{
				Transform parent = Selection.activeTransform.parent;
				return parent != null && parent.GetChild(0) != Selection.activeTransform;
			}
			return false;
		}
		[MenuItem("GameObject/Set as last sibling %-")]
		private static void MenuMoveToBack()
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
		private static bool ValidateMenuMoveToBack()
		{
			if (Selection.activeTransform != null)
			{
				Transform parent = Selection.activeTransform.parent;
				return parent != null && parent.GetChild(parent.childCount - 1) != Selection.activeTransform;
			}
			return false;
		}
		[MenuItem("GameObject/Move To View %&f")]
		private static void MenuMoveToView()
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
		private static void MenuAlignWithView()
		{
			if (SceneView.ValidateAlignWithView())
			{
				SceneView.s_LastActiveSceneView.AlignWithView();
			}
		}
		[MenuItem("GameObject/Align With View %#f", true)]
		private static bool ValidateAlignWithView()
		{
			return SceneView.s_LastActiveSceneView != null && Selection.activeTransform != null;
		}
		[MenuItem("GameObject/Align View to Selected")]
		private static void MenuAlignViewToSelected()
		{
			if (SceneView.ValidateAlignViewToSelected())
			{
				SceneView.s_LastActiveSceneView.AlignViewToObject(Selection.activeTransform);
			}
		}
		[MenuItem("GameObject/Align View to Selected", true)]
		private static bool ValidateAlignViewToSelected()
		{
			return SceneView.s_LastActiveSceneView != null && Selection.activeTransform != null;
		}
		[MenuItem("GameObject/Toggle Active State &#a")]
		private static void ActivateSelection()
		{
			if (Selection.activeTransform != null)
			{
				GameObject[] gameObjects = Selection.gameObjects;
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
		private static bool ValidateActivateSelection()
		{
			return Selection.activeTransform != null;
		}
		private static void CreateMipColorsTexture()
		{
			if (SceneView.s_MipColorsTexture)
			{
				return;
			}
			SceneView.s_MipColorsTexture = new Texture2D(32, 32, TextureFormat.ARGB32, true);
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
			Shader.SetGlobalTexture("__SceneViewMipcolorsTexture", SceneView.s_MipColorsTexture);
		}
		public void SetSceneViewFiltering(bool enable)
		{
			this.m_RequestedSceneViewFiltering = enable;
		}
		private bool UseSceneFiltering()
		{
			return !string.IsNullOrEmpty(this.m_SearchFilter) || this.m_RequestedSceneViewFiltering;
		}
		private void OnGUI()
		{
			SceneView.s_CurrentDrawingSceneView = this;
			if (Event.current.type == EventType.Repaint)
			{
				SceneView.s_MouseRects.Clear();
			}
			Color color = GUI.color;
			if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
			{
				SceneView.s_LastActiveSceneView = this;
			}
			else
			{
				if (SceneView.s_LastActiveSceneView == null)
				{
					SceneView.s_LastActiveSceneView = this;
				}
			}
			if (Event.current.type == EventType.MouseDrag)
			{
				this.draggingLocked = SceneView.DraggingLockedState.Dragging;
			}
			else
			{
				if (Event.current.type == EventType.MouseUp)
				{
					this.draggingLocked = SceneView.DraggingLockedState.LookAt;
				}
			}
			if (Event.current.type == EventType.MouseDown)
			{
				Tools.s_ButtonDown = Event.current.button;
				if (Event.current.button == 1 && Application.platform == RuntimePlatform.OSXEditor)
				{
					base.Focus();
				}
			}
			if (Event.current.type == EventType.Layout)
			{
				this.m_ShowSceneViewWindows = (SceneView.lastActiveSceneView == this);
			}
			this.m_SceneViewOverlay.Begin();
			bool fog = RenderSettings.fog;
			float shadowDistance = QualitySettings.shadowDistance;
			if (Event.current.type == EventType.Repaint)
			{
				if (!this.m_SceneViewState.showFog)
				{
					Unsupported.SetRenderSettingsUseFogNoDirty(false);
				}
				if (this.m_Camera.isOrthoGraphic)
				{
					Unsupported.SetQualitySettingsShadowDistanceTemporarily(QualitySettings.shadowDistance + 0.5f * this.cameraDistance);
				}
			}
			this.DoStatusBarGUI();
			GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
			EditorGUIUtility.labelWidth = 100f;
			this.SetupCamera();
			RenderingPath renderingPath = this.m_Camera.renderingPath;
			if (!this.m_SceneLighting)
			{
				this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
				if (Event.current.type == EventType.Repaint)
				{
					InternalEditorUtility.SetCustomLighting(this.m_Light, SceneView.kSceneViewMidLight);
				}
			}
			GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
			Rect rect = new Rect(0f, 0f, base.position.width, base.position.height - 17f);
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
			if (this.UseSceneFiltering())
			{
				EditorUtility.SetTemporarilyAllowIndieRenderTexture(true);
				if (this.m_SearchFilterTexture == null)
				{
					this.m_SearchFilterTexture = new RenderTexture(0, 0, 24);
					this.m_SearchFilterTexture.hideFlags = HideFlags.HideAndDontSave;
				}
				Rect cameraRect = Handles.GetCameraRect(rect);
				if (this.m_SearchFilterTexture.width != (int)cameraRect.width || this.m_SearchFilterTexture.height != (int)cameraRect.height)
				{
					this.m_SearchFilterTexture.Release();
					this.m_SearchFilterTexture.width = (int)cameraRect.width;
					this.m_SearchFilterTexture.height = (int)cameraRect.height;
				}
				this.m_Camera.targetTexture = this.m_SearchFilterTexture;
				if (this.m_Camera.actualRenderingPath == RenderingPath.DeferredLighting)
				{
					this.m_Camera.renderingPath = RenderingPath.Forward;
				}
			}
			else
			{
				this.m_Camera.targetTexture = null;
			}
			float verticalFOV = this.GetVerticalFOV(90f);
			float fieldOfView = this.m_Camera.fieldOfView;
			this.m_Camera.fieldOfView = verticalFOV;
			Handles.ClearCamera(rect, this.m_Camera);
			this.m_Camera.fieldOfView = fieldOfView;
			this.m_Camera.cullingMask = Tools.visibleLayers;
			if (!this.UseSceneFiltering())
			{
				Handles.SetCamera(rect, this.m_Camera);
				this.CallOnPreSceneGUI();
			}
			if (Event.current.type == EventType.Repaint)
			{
				Handles.SetSceneViewColors(SceneView.kSceneViewWire, SceneView.kSceneViewWireOverlay, SceneView.kSceneViewWireActive, SceneView.kSceneViewWireSelected);
				if (this.m_OverlayMode == 2)
				{
					if (!SceneView.s_ShowOverdrawShader)
					{
						SceneView.s_ShowOverdrawShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewShowOverdraw.shader") as Shader);
					}
					this.m_Camera.SetReplacementShader(SceneView.s_ShowOverdrawShader, "RenderType");
				}
				else
				{
					if (this.m_OverlayMode == 3)
					{
						if (!SceneView.s_ShowMipsShader)
						{
							SceneView.s_ShowMipsShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewShowMips.shader") as Shader);
						}
						if (SceneView.s_ShowMipsShader.isSupported)
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
						if (this.m_OverlayMode == 4)
						{
							if (!SceneView.s_ShowLightmapsShader)
							{
								SceneView.s_ShowLightmapsShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewShowLightmap.shader") as Shader);
							}
							if (SceneView.s_ShowLightmapsShader.isSupported)
							{
								this.m_Camera.SetReplacementShader(SceneView.s_ShowLightmapsShader, "RenderType");
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
			}
			this.m_MainViewControlID = GUIUtility.GetControlID(FocusType.Keyboard);
			if (Event.current.GetTypeForControl(this.m_MainViewControlID) == EventType.MouseDown)
			{
				GUIUtility.keyboardControl = this.m_MainViewControlID;
			}
			if (this.m_Camera.gameObject.activeInHierarchy)
			{
				DrawGridParameters gridParam = this.grid.PrepareGridRender(this.camera, this.pivot, this.m_Rotation.target, this.m_Size.value, this.m_Ortho.target, AnnotationUtility.showGrid);
				if (this.UseSceneFiltering())
				{
					if (Event.current.type == EventType.Repaint)
					{
						Handles.EnableCameraFx(this.m_Camera, true);
						Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowRest);
						float num = Mathf.Clamp01((float)(EditorApplication.timeSinceStartup - this.m_StartSearchFilterTime));
						Handles.DrawCamera(rect, this.m_Camera, this.m_RenderMode);
						Handles.DrawCameraFade(this.m_Camera, num);
						RenderTexture.active = null;
						Handles.EnableCameraFx(this.m_Camera, false);
						Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.ShowFiltered);
						if (!SceneView.s_AuraShader)
						{
							SceneView.s_AuraShader = (EditorGUIUtility.LoadRequired("SceneView/SceneViewAura.shader") as Shader);
						}
						this.m_Camera.SetReplacementShader(SceneView.s_AuraShader, string.Empty);
						Handles.DrawCamera(rect, this.m_Camera, this.m_RenderMode);
						this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
						Handles.DrawCamera(rect, this.m_Camera, this.m_RenderMode, gridParam);
						if (num < 1f)
						{
							base.Repaint();
						}
					}
					Rect position = rect;
					GUI.EndGroup();
					GUI.BeginGroup(new Rect(0f, 17f, base.position.width, base.position.height - 17f));
					GUI.DrawTexture(position, this.m_SearchFilterTexture, ScaleMode.StretchToFill, false, 0f);
					Handles.SetCamera(rect, this.m_Camera);
					this.HandleSelectionAndOnSceneGUI();
				}
				else
				{
					Handles.DrawCameraStep1(rect, this.m_Camera, this.m_RenderMode, gridParam);
					this.DrawAlphaOverlay();
				}
			}
			if (!this.m_SceneLighting && Event.current.type == EventType.Repaint)
			{
				InternalEditorUtility.RemoveCustomLighting();
			}
			if (this.UseSceneFiltering())
			{
				EditorUtility.SetTemporarilyAllowIndieRenderTexture(false);
			}
			if (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand)
			{
				this.CommandsGUI();
			}
			if (Event.current.type == EventType.Repaint)
			{
				Unsupported.SetRenderSettingsUseFogNoDirty(fog);
				Unsupported.SetQualitySettingsShadowDistanceTemporarily(shadowDistance);
			}
			this.m_Camera.renderingPath = renderingPath;
			if (!this.UseSceneFiltering())
			{
				Handles.DrawCameraStep2(this.m_Camera, this.m_RenderMode);
			}
			if (this.UseSceneFiltering())
			{
				Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.ShowFiltered);
			}
			else
			{
				Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.Off);
			}
			if (!this.UseSceneFiltering())
			{
				this.HandleSelectionAndOnSceneGUI();
			}
			this.DefaultHandles();
			Handles.SetCameraFilterMode(Camera.current, Handles.FilterMode.Off);
			Handles.SetCameraFilterMode(this.m_Camera, Handles.FilterMode.Off);
			this.HandleDragging();
			this.svRot.HandleContextClick(this);
			this.svRot.OnGUI(this);
			SceneViewMotion.ArrowKeys(this);
			SceneViewMotion.DoViewTool(this.camera.transform, this);
			if (SceneView.k2DMode.activated && !SceneView.waitingFor2DModeKeyUp)
			{
				SceneView.waitingFor2DModeKeyUp = true;
				this.in2DMode = !this.in2DMode;
				Event.current.Use();
			}
			else
			{
				if (Event.current.type == EventType.KeyUp && Event.current.keyCode == SceneView.k2DMode.KeyboardEvent.keyCode)
				{
					SceneView.waitingFor2DModeKeyUp = false;
				}
			}
			GUI.EndGroup();
			GUI.color = color;
			this.m_SceneViewOverlay.End();
			if (GUIUtility.hotControl == 0)
			{
				this.s_DraggingCursorIsCashed = false;
			}
			Rect rect2 = new Rect(0f, 0f, base.position.width, base.position.height);
			if (!this.s_DraggingCursorIsCashed)
			{
				MouseCursor mouseCursor2 = MouseCursor.Arrow;
				if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.Repaint)
				{
					foreach (SceneView.CursorRect current in SceneView.s_MouseRects)
					{
						if (current.rect.Contains(Event.current.mousePosition))
						{
							mouseCursor2 = current.cursor;
							rect2 = current.rect;
						}
					}
					if (GUIUtility.hotControl != 0)
					{
						this.s_DraggingCursorIsCashed = true;
					}
					if (mouseCursor2 != SceneView.s_LastCursor)
					{
						SceneView.s_LastCursor = mouseCursor2;
						InternalEditorUtility.ResetCursor();
						base.Repaint();
					}
				}
			}
			if (Event.current.type == EventType.Repaint && SceneView.s_LastCursor != MouseCursor.Arrow)
			{
				EditorGUIUtility.AddCursorRect(rect2, SceneView.s_LastCursor);
			}
		}
		private void DrawAlphaOverlay()
		{
			if (this.m_OverlayMode != 1)
			{
				return;
			}
			if (!SceneView.s_AlphaOverlayMaterial)
			{
				SceneView.s_AlphaOverlayMaterial = (EditorGUIUtility.LoadRequired("SceneView/SceneViewAlphaMaterial.mat") as Material);
			}
			Handles.BeginGUI();
			if (Event.current.type == EventType.Repaint)
			{
				Graphics.DrawTexture(new Rect(0f, 0f, base.position.width, base.position.height), EditorGUIUtility.whiteTexture, SceneView.s_AlphaOverlayMaterial);
			}
			Handles.EndGUI();
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
			if (num > 3f)
			{
				this.m_Camera.orthographic = false;
				return this.size / Mathf.Tan(num * 0.5f * 0.0174532924f);
			}
			return 0f;
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
		internal static RenderingPath GetSceneViewRenderingPath()
		{
			Camera main = Camera.main;
			if (main != null)
			{
				return main.renderingPath;
			}
			Camera[] allCameras = Camera.allCameras;
			if (allCameras != null && allCameras.Length == 1)
			{
				return allCameras[0].renderingPath;
			}
			return RenderingPath.UsePlayerSettings;
		}
		private void SetupCamera()
		{
			if (!this.m_Camera)
			{
				this.Setup();
			}
			if (this.m_OverlayMode == 2)
			{
				this.m_Camera.backgroundColor = Color.black;
			}
			else
			{
				this.m_Camera.backgroundColor = SceneView.kSceneViewBackground;
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
			Handles.EnableCameraFlares(this.m_Camera, this.m_SceneViewState.showFlares);
			Handles.EnableCameraSkybox(this.m_Camera, this.m_SceneViewState.showSkybox);
			this.m_Light[0].transform.position = this.m_Camera.transform.position;
			this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
			if (this.m_AudioPlay)
			{
				AudioUtil.SetListenerTransform(this.m_Camera.transform);
				AudioUtil.UpdateAudio();
			}
			if (this.m_ViewIsLockedToObject && Selection.gameObjects.Length > 0)
			{
				switch (this.m_DraggingLockedState)
				{
				case SceneView.DraggingLockedState.NotDragging:
					this.m_Position.value = Selection.activeGameObject.transform.position;
					break;
				case SceneView.DraggingLockedState.LookAt:
					if (!this.m_Position.value.Equals(Selection.activeGameObject.transform.position))
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
					break;
				}
			}
		}
		public void Update()
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
		public void LookAt(Vector3 position)
		{
			this.FixNegativeSize();
			this.m_Position.target = position;
		}
		public void LookAt(Vector3 position, Quaternion rotation)
		{
			this.FixNegativeSize();
			this.m_Position.target = position;
			this.m_Rotation.target = rotation;
			this.svRot.UpdateGizmoLabel(this, rotation * Vector3.forward, this.m_Ortho.target);
		}
		public void LookAtDirect(Vector3 position, Quaternion rotation)
		{
			this.FixNegativeSize();
			this.m_Position.value = position;
			this.m_Rotation.value = rotation;
			this.svRot.UpdateGizmoLabel(this, rotation * Vector3.forward, this.m_Ortho.target);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float size)
		{
			this.FixNegativeSize();
			this.m_Position.target = position;
			this.m_Rotation.target = rotation;
			this.m_Size.target = Mathf.Abs(size);
			this.svRot.UpdateGizmoLabel(this, rotation * Vector3.forward, this.m_Ortho.target);
		}
		public void LookAtDirect(Vector3 position, Quaternion rotation, float size)
		{
			this.FixNegativeSize();
			this.m_Position.value = position;
			this.m_Rotation.value = rotation;
			this.m_Size.value = Mathf.Abs(size);
			this.svRot.UpdateGizmoLabel(this, rotation * Vector3.forward, this.m_Ortho.target);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float size, bool orthographic)
		{
			this.LookAt(position, rotation, size, orthographic, false);
		}
		public void LookAt(Vector3 position, Quaternion rotation, float size, bool orthographic, bool instant)
		{
			this.FixNegativeSize();
			if (instant)
			{
				this.m_Position.value = position;
				this.m_Rotation.value = rotation;
				this.m_Size.value = Mathf.Abs(size);
				this.m_Ortho.value = orthographic;
			}
			else
			{
				this.m_Position.target = position;
				this.m_Rotation.target = rotation;
				this.m_Size.target = Mathf.Abs(size);
				this.m_Ortho.target = orthographic;
			}
			this.svRot.UpdateGizmoLabel(this, rotation * Vector3.forward, this.m_Ortho.target);
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
			Tool tool2 = tool;
			switch (tool2 + 1)
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
			if (DragAndDrop.objectReferences.Length == 0)
			{
				return;
			}
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
					if (Event.current.type == EventType.Used)
					{
						return;
					}
				}
			}
		}
		private void HandleDragging()
		{
			Event current = Event.current;
			EventType type = current.type;
			switch (type)
			{
			case EventType.Repaint:
				this.CallEditorDragFunctions();
				return;
			case EventType.Layout:
				IL_25:
				if (type != EventType.DragExited)
				{
					return;
				}
				this.CallEditorDragFunctions();
				this.CleanupEditorDragFunctions();
				return;
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				this.CallEditorDragFunctions();
				bool flag = current.type == EventType.DragPerform;
				if (this.m_2DMode)
				{
					SpriteUtility.OnSceneDrag(this);
				}
				if (current.type == EventType.Used)
				{
					return;
				}
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
				return;
			}
			}
			goto IL_25;
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
			Bounds bounds = InternalEditorUtility.CalculateSelectionBounds(false);
			Editor[] activeEditors = this.GetActiveEditors();
			for (int i = 0; i < activeEditors.Length; i++)
			{
				Editor editor = activeEditors[i];
				MethodInfo method = editor.GetType().GetMethod("HasFrameBounds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (method != null)
				{
					object obj = method.Invoke(editor, null);
					if (obj != null && obj is bool && (bool)obj)
					{
						MethodInfo method2 = editor.GetType().GetMethod("OnGetFrameBounds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
						if (method2 != null)
						{
							object obj2 = method2.Invoke(editor, null);
							if (obj2 != null && obj2 is Bounds)
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
			if (num == float.PositiveInfinity)
			{
				return false;
			}
			if (num == 0f)
			{
				num = 10f;
			}
			this.LookAt(bounds.center, this.m_Rotation.target, num * 2.2f, this.m_Ortho.value, EditorApplication.isPlaying);
			return true;
		}
		private void Setup()
		{
			GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags("SceneCamera", HideFlags.HideAndDontSave, new Type[]
			{
				typeof(Camera)
			});
			gameObject.AddComponent("FlareLayer");
			gameObject.AddComponent("HaloLayer");
			this.m_Camera = gameObject.camera;
			this.m_Camera.enabled = false;
			for (int i = 0; i < 3; i++)
			{
				GameObject gameObject2 = EditorUtility.CreateGameObjectWithHideFlags("SceneLight", HideFlags.HideAndDontSave, new Type[]
				{
					typeof(Light)
				});
				this.m_Light[i] = gameObject2.light;
				this.m_Light[i].type = LightType.Directional;
				this.m_Light[i].intensity = 0.5f;
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
					if (dockArea && dockArea.actualView == sceneView)
					{
						list.Add(sceneView);
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
			if (behaviorMode != EditorBehaviorMode.Mode3D)
			{
				if (behaviorMode == EditorBehaviorMode.Mode2D)
				{
					this.m_2DMode = true;
					this.m_Rotation.value = Quaternion.identity;
					this.m_Position.value = SceneView.kDefaultPivot;
					this.m_Size.value = SceneView.kDefaultViewSize;
					this.m_Ortho.value = true;
					this.m_LastSceneViewRotation = SceneView.kDefaultRotation;
					this.m_LastSceneViewOrtho = false;
					return;
				}
			}
			this.m_2DMode = false;
			this.m_Rotation.value = SceneView.kDefaultRotation;
			this.m_Position.value = SceneView.kDefaultPivot;
			this.m_Size.value = SceneView.kDefaultViewSize;
			this.m_Ortho.value = false;
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
					Analytics.Event("2D", "SceneView", "Single 2D", 1);
				}
			}
		}
	}
}
