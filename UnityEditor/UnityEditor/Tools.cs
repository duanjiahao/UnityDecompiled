using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	public sealed class Tools : ScriptableObject
	{
		internal delegate void OnToolChangedFunc(Tool from, Tool to);

		private static Tools s_Get;

		internal static Tools.OnToolChangedFunc onToolChanged;

		internal static ViewTool s_LockedViewTool = ViewTool.None;

		internal static int s_ButtonDown = -1;

		private PivotMode m_PivotMode;

		private bool m_RectBlueprintMode;

		private PivotRotation m_PivotRotation;

		internal static bool s_Hidden = false;

		internal static bool vertexDragging;

		private static Vector3 s_LockHandlePosition;

		private static bool s_LockHandlePositionActive = false;

		private static int s_LockHandleRectAxis;

		private static bool s_LockHandleRectAxisActive = false;

		private int m_VisibleLayers = -1;

		private int m_LockedLayers = -1;

		internal Quaternion m_GlobalHandleRotation = Quaternion.identity;

		private Tool currentTool = Tool.Move;

		private ViewTool m_ViewTool = ViewTool.Pan;

		private static int originalTool;

		private static PrefKey kViewKey = new PrefKey("Tools/View", "q");

		private static PrefKey kMoveKey = new PrefKey("Tools/Move", "w");

		private static PrefKey kRotateKey = new PrefKey("Tools/Rotate", "e");

		private static PrefKey kScaleKey = new PrefKey("Tools/Scale", "r");

		private static PrefKey kRectKey = new PrefKey("Tools/Rect Handles", "t");

		private static PrefKey kPivotMode = new PrefKey("Tools/Pivot Mode", "z");

		private static PrefKey kPivotRotation = new PrefKey("Tools/Pivot Rotation", "x");

		internal static Vector3 handleOffset;

		internal static Vector3 localHandleOffset;

		private static Tools get
		{
			get
			{
				if (!Tools.s_Get)
				{
					Tools.s_Get = ScriptableObject.CreateInstance<Tools>();
					Tools.s_Get.hideFlags = HideFlags.HideAndDontSave;
				}
				return Tools.s_Get;
			}
		}

		public static Tool current
		{
			get
			{
				return Tools.get.currentTool;
			}
			set
			{
				if (Tools.get.currentTool != value)
				{
					Tool from = Tools.get.currentTool;
					Tools.get.currentTool = value;
					if (Tools.onToolChanged != null)
					{
						Tools.onToolChanged(from, value);
					}
					Tools.RepaintAllToolViews();
				}
			}
		}

		public static ViewTool viewTool
		{
			get
			{
				Event current = Event.current;
				if (current != null && Tools.viewToolActive)
				{
					if (Tools.s_LockedViewTool == ViewTool.None)
					{
						bool flag = current.control && Application.platform == RuntimePlatform.OSXEditor;
						bool actionKey = EditorGUI.actionKey;
						bool flag2 = !actionKey && !flag && !current.alt;
						if ((Tools.s_ButtonDown <= 0 && flag2) || (Tools.s_ButtonDown <= 0 && actionKey) || Tools.s_ButtonDown == 2 || (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.in2DMode && (Tools.s_ButtonDown != 1 || !current.alt) && (Tools.s_ButtonDown > 0 || !flag)))
						{
							Tools.get.m_ViewTool = ViewTool.Pan;
						}
						else if ((Tools.s_ButtonDown <= 0 && flag) || (Tools.s_ButtonDown == 1 && current.alt))
						{
							Tools.get.m_ViewTool = ViewTool.Zoom;
						}
						else if (Tools.s_ButtonDown <= 0 && current.alt)
						{
							Tools.get.m_ViewTool = ViewTool.Orbit;
						}
						else if (Tools.s_ButtonDown == 1 && !current.alt)
						{
							Tools.get.m_ViewTool = ViewTool.FPS;
						}
					}
				}
				else
				{
					Tools.get.m_ViewTool = ViewTool.Pan;
				}
				return Tools.get.m_ViewTool;
			}
			set
			{
				Tools.get.m_ViewTool = value;
			}
		}

		internal static bool viewToolActive
		{
			get
			{
				return (GUIUtility.hotControl == 0 || Tools.s_LockedViewTool != ViewTool.None) && (Tools.s_LockedViewTool != ViewTool.None || Tools.current == Tool.View || Event.current.alt || Tools.s_ButtonDown == 1 || Tools.s_ButtonDown == 2);
			}
		}

		public static Vector3 handlePosition
		{
			get
			{
				Transform activeTransform = Selection.activeTransform;
				if (!activeTransform)
				{
					return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
				}
				if (Tools.s_LockHandlePositionActive)
				{
					return Tools.s_LockHandlePosition;
				}
				return Tools.GetHandlePosition();
			}
		}

		public static Rect handleRect
		{
			get
			{
				Bounds bounds = InternalEditorUtility.CalculateSelectionBoundsInSpace(Tools.handlePosition, Tools.handleRotation, Tools.rectBlueprintMode);
				int rectAxisForViewDir = Tools.GetRectAxisForViewDir(bounds, Tools.handleRotation, SceneView.currentDrawingSceneView.camera.transform.forward);
				return Tools.GetRectFromBoundsForAxis(bounds, rectAxisForViewDir);
			}
		}

		public static Quaternion handleRectRotation
		{
			get
			{
				Bounds bounds = InternalEditorUtility.CalculateSelectionBoundsInSpace(Tools.handlePosition, Tools.handleRotation, Tools.rectBlueprintMode);
				int rectAxisForViewDir = Tools.GetRectAxisForViewDir(bounds, Tools.handleRotation, SceneView.currentDrawingSceneView.camera.transform.forward);
				return Tools.GetRectRotationForAxis(Tools.handleRotation, rectAxisForViewDir);
			}
		}

		public static PivotMode pivotMode
		{
			get
			{
				return Tools.get.m_PivotMode;
			}
			set
			{
				if (Tools.get.m_PivotMode != value)
				{
					Tools.get.m_PivotMode = value;
					EditorPrefs.SetInt("PivotMode", (int)Tools.pivotMode);
				}
			}
		}

		public static bool rectBlueprintMode
		{
			get
			{
				return Tools.get.m_RectBlueprintMode;
			}
			set
			{
				if (Tools.get.m_RectBlueprintMode != value)
				{
					Tools.get.m_RectBlueprintMode = value;
					EditorPrefs.SetBool("RectBlueprintMode", Tools.rectBlueprintMode);
				}
			}
		}

		public static Quaternion handleRotation
		{
			get
			{
				PivotRotation pivotRotation = Tools.get.m_PivotRotation;
				if (pivotRotation == PivotRotation.Local)
				{
					return Tools.handleLocalRotation;
				}
				if (pivotRotation != PivotRotation.Global)
				{
					return Quaternion.identity;
				}
				return Tools.get.m_GlobalHandleRotation;
			}
			set
			{
				if (Tools.get.m_PivotRotation == PivotRotation.Global)
				{
					Tools.get.m_GlobalHandleRotation = value;
				}
			}
		}

		public static PivotRotation pivotRotation
		{
			get
			{
				return Tools.get.m_PivotRotation;
			}
			set
			{
				if (Tools.get.m_PivotRotation != value)
				{
					Tools.get.m_PivotRotation = value;
					EditorPrefs.SetInt("PivotRotation", (int)Tools.pivotRotation);
				}
			}
		}

		public static bool hidden
		{
			get
			{
				return Tools.s_Hidden;
			}
			set
			{
				Tools.s_Hidden = value;
			}
		}

		public static int visibleLayers
		{
			get
			{
				return Tools.get.m_VisibleLayers;
			}
			set
			{
				if (Tools.get.m_VisibleLayers != value)
				{
					Tools.get.m_VisibleLayers = value;
					EditorGUIUtility.SetVisibleLayers(value);
					EditorPrefs.SetInt("VisibleLayers", Tools.visibleLayers);
				}
			}
		}

		public static int lockedLayers
		{
			get
			{
				return Tools.get.m_LockedLayers;
			}
			set
			{
				if (Tools.get.m_LockedLayers != value)
				{
					Tools.get.m_LockedLayers = value;
					EditorGUIUtility.SetLockedLayers(value);
					EditorPrefs.SetInt("LockedLayers", Tools.lockedLayers);
				}
			}
		}

		internal static Quaternion handleLocalRotation
		{
			get
			{
				Transform activeTransform = Selection.activeTransform;
				if (!activeTransform)
				{
					return Quaternion.identity;
				}
				if (Tools.rectBlueprintMode && InternalEditorUtility.SupportsRectLayout(activeTransform))
				{
					return activeTransform.parent.rotation;
				}
				return activeTransform.rotation;
			}
		}

		internal static Vector3 GetHandlePosition()
		{
			Transform activeTransform = Selection.activeTransform;
			if (!activeTransform)
			{
				return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			}
			Vector3 b = Tools.handleOffset + Tools.handleRotation * Tools.localHandleOffset;
			PivotMode pivotMode = Tools.get.m_PivotMode;
			if (pivotMode != PivotMode.Center)
			{
				if (pivotMode != PivotMode.Pivot)
				{
					return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
				}
				if (Tools.current == Tool.Rect && Tools.rectBlueprintMode && InternalEditorUtility.SupportsRectLayout(activeTransform))
				{
					return activeTransform.parent.TransformPoint(new Vector3(activeTransform.localPosition.x, activeTransform.localPosition.y, 0f)) + b;
				}
				return activeTransform.position + b;
			}
			else
			{
				if (Tools.current == Tool.Rect)
				{
					return Tools.handleRotation * InternalEditorUtility.CalculateSelectionBoundsInSpace(Vector3.zero, Tools.handleRotation, Tools.rectBlueprintMode).center + b;
				}
				return InternalEditorUtility.CalculateSelectionBounds(true, false).center + b;
			}
		}

		private static int GetRectAxisForViewDir(Bounds bounds, Quaternion rotation, Vector3 viewDir)
		{
			if (Tools.s_LockHandleRectAxisActive)
			{
				return Tools.s_LockHandleRectAxis;
			}
			if (viewDir == Vector3.zero)
			{
				return 2;
			}
			if (bounds.size == Vector3.zero)
			{
				bounds.size = Vector3.one;
			}
			int result = -1;
			float num = -1f;
			for (int i = 0; i < 3; i++)
			{
				Vector3 zero = Vector3.zero;
				Vector3 zero2 = Vector3.zero;
				int index = (i + 1) % 3;
				int index2 = (i + 2) % 3;
				zero[index] = bounds.size[index];
				zero2[index2] = bounds.size[index2];
				float magnitude = Vector3.Cross(Vector3.ProjectOnPlane(rotation * zero, viewDir), Vector3.ProjectOnPlane(rotation * zero2, viewDir)).magnitude;
				if (magnitude > num)
				{
					num = magnitude;
					result = i;
				}
			}
			return result;
		}

		private static Rect GetRectFromBoundsForAxis(Bounds bounds, int axis)
		{
			switch (axis)
			{
			case 0:
				return new Rect(-bounds.max.z, bounds.min.y, bounds.size.z, bounds.size.y);
			case 1:
				return new Rect(bounds.min.x, -bounds.max.z, bounds.size.x, bounds.size.z);
			}
			return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
		}

		private static Quaternion GetRectRotationForAxis(Quaternion rotation, int axis)
		{
			switch (axis)
			{
			case 0:
				return rotation * Quaternion.Euler(0f, 90f, 0f);
			case 1:
				return rotation * Quaternion.Euler(-90f, 0f, 0f);
			}
			return rotation;
		}

		internal static void LockHandleRectRotation()
		{
			Bounds bounds = InternalEditorUtility.CalculateSelectionBoundsInSpace(Tools.handlePosition, Tools.handleRotation, Tools.rectBlueprintMode);
			Tools.s_LockHandleRectAxis = Tools.GetRectAxisForViewDir(bounds, Tools.handleRotation, SceneView.currentDrawingSceneView.camera.transform.forward);
			Tools.s_LockHandleRectAxisActive = true;
		}

		internal static void UnlockHandleRectRotation()
		{
			Tools.s_LockHandleRectAxisActive = false;
		}

		internal static int GetPivotMode()
		{
			return (int)Tools.pivotMode;
		}

		private void OnEnable()
		{
			Tools.s_Get = this;
			EditorApplication.globalEventHandler = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.globalEventHandler, new EditorApplication.CallbackFunction(Tools.ControlsHack));
			Tools.pivotMode = (PivotMode)EditorPrefs.GetInt("PivotMode", 0);
			Tools.rectBlueprintMode = EditorPrefs.GetBool("RectBlueprintMode", false);
			Tools.pivotRotation = (PivotRotation)EditorPrefs.GetInt("PivotRotation", 0);
			Tools.visibleLayers = EditorPrefs.GetInt("VisibleLayers", -1);
			Tools.lockedLayers = EditorPrefs.GetInt("LockedLayers", 0);
		}

		private void OnDisable()
		{
			EditorApplication.globalEventHandler = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.globalEventHandler, new EditorApplication.CallbackFunction(Tools.ControlsHack));
		}

		internal static void OnSelectionChange()
		{
			Tools.ResetGlobalHandleRotation();
			Tools.localHandleOffset = Vector3.zero;
		}

		internal static void ResetGlobalHandleRotation()
		{
			Tools.get.m_GlobalHandleRotation = Quaternion.identity;
		}

		internal static void ControlsHack()
		{
			Event current = Event.current;
			if (Tools.kViewKey.activated)
			{
				Tools.current = Tool.View;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				if (Toolbar.get)
				{
					Toolbar.get.Repaint();
				}
				else
				{
					Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
				}
			}
			if (Tools.kMoveKey.activated)
			{
				Tools.current = Tool.Move;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				if (Toolbar.get)
				{
					Toolbar.get.Repaint();
				}
				else
				{
					Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
				}
			}
			if (Tools.kRotateKey.activated)
			{
				Tools.current = Tool.Rotate;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				if (Toolbar.get)
				{
					Toolbar.get.Repaint();
				}
				else
				{
					Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
				}
			}
			if (Tools.kScaleKey.activated)
			{
				Tools.current = Tool.Scale;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				if (Toolbar.get)
				{
					Toolbar.get.Repaint();
				}
				else
				{
					Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
				}
			}
			if (Tools.kRectKey.activated)
			{
				Tools.current = Tool.Rect;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				if (Toolbar.get)
				{
					Toolbar.get.Repaint();
				}
				else
				{
					Debug.LogError("Press Play twice for sceneview keyboard shortcuts to work");
				}
			}
			if (Tools.kPivotMode.activated)
			{
				Tools.pivotMode = PivotMode.Pivot - (int)Tools.pivotMode;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				Tools.RepaintAllToolViews();
			}
			if (Tools.kPivotRotation.activated)
			{
				Tools.pivotRotation = PivotRotation.Global - (int)Tools.pivotRotation;
				Tools.ResetGlobalHandleRotation();
				current.Use();
				Tools.RepaintAllToolViews();
			}
		}

		internal static void RepaintAllToolViews()
		{
			if (Toolbar.get)
			{
				Toolbar.get.Repaint();
			}
			SceneView.RepaintAll();
			InspectorWindow.RepaintAllInspectors();
		}

		internal static void HandleKeys()
		{
			Tools.ControlsHack();
		}

		internal static void LockHandlePosition(Vector3 pos)
		{
			Tools.s_LockHandlePosition = pos;
			Tools.s_LockHandlePositionActive = true;
		}

		internal static void LockHandlePosition()
		{
			Tools.LockHandlePosition(Tools.handlePosition);
		}

		internal static void UnlockHandlePosition()
		{
			Tools.s_LockHandlePositionActive = false;
		}
	}
}
