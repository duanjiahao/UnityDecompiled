using System;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Camera))]
	internal class CameraEditor : Editor
	{
		private enum ProjectionType
		{
			Perspective,
			Orthographic
		}
		private const float kPreviewWindowOffset = 10f;
		private const float kPreviewNormalizedSize = 0.2f;
		private SerializedProperty m_ClearFlags;
		private SerializedProperty m_BackgroundColor;
		private SerializedProperty m_NormalizedViewPortRect;
		private SerializedProperty m_NearClip;
		private SerializedProperty m_FarClip;
		private SerializedProperty m_FieldOfView;
		private SerializedProperty m_Orthographic;
		private SerializedProperty m_OrthographicSize;
		private SerializedProperty m_Depth;
		private SerializedProperty m_CullingMask;
		private SerializedProperty m_RenderingPath;
		private SerializedProperty m_OcclusionCulling;
		private SerializedProperty m_TargetTexture;
		private SerializedProperty m_HDR;
		private AnimBool m_ShowBGColorOptions = new AnimBool();
		private AnimBool m_ShowOrthoOptions = new AnimBool();
		private AnimBool m_ShowDeferredWarning = new AnimBool();
		private Camera m_PreviewCamera;
		private static readonly Color kGizmoCamera = new Color(0.9137255f, 0.9137255f, 0.9137255f, 0.5019608f);
		private GUIContent m_ClipingPlanesLabel = new GUIContent("Clipping Planes");
		private GUIContent m_NearClipPlaneLabel = new GUIContent("Near");
		private GUIContent m_FarClipPlaneLabel = new GUIContent("Far");
		private GUIContent m_ViewportLabel = new GUIContent("Viewport Rect");
		private Camera camera
		{
			get
			{
				return this.target as Camera;
			}
		}
		private bool deferredWarningValue
		{
			get
			{
				return !InternalEditorUtility.HasPro() && (this.camera.renderingPath == RenderingPath.DeferredLighting || (PlayerSettings.renderingPath == RenderingPath.DeferredLighting && this.camera.renderingPath == RenderingPath.UsePlayerSettings));
			}
		}
		private Camera previewCamera
		{
			get
			{
				if (this.m_PreviewCamera == null)
				{
					this.m_PreviewCamera = EditorUtility.CreateGameObjectWithHideFlags("Preview Camera", HideFlags.HideAndDontSave, new Type[]
					{
						typeof(Camera)
					}).GetComponent<Camera>();
				}
				this.m_PreviewCamera.enabled = false;
				return this.m_PreviewCamera;
			}
		}
		public void OnEnable()
		{
			this.m_ClearFlags = base.serializedObject.FindProperty("m_ClearFlags");
			this.m_BackgroundColor = base.serializedObject.FindProperty("m_BackGroundColor");
			this.m_NormalizedViewPortRect = base.serializedObject.FindProperty("m_NormalizedViewPortRect");
			this.m_NearClip = base.serializedObject.FindProperty("near clip plane");
			this.m_FarClip = base.serializedObject.FindProperty("far clip plane");
			this.m_FieldOfView = base.serializedObject.FindProperty("field of view");
			this.m_Orthographic = base.serializedObject.FindProperty("orthographic");
			this.m_OrthographicSize = base.serializedObject.FindProperty("orthographic size");
			this.m_Depth = base.serializedObject.FindProperty("m_Depth");
			this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
			this.m_RenderingPath = base.serializedObject.FindProperty("m_RenderingPath");
			this.m_OcclusionCulling = base.serializedObject.FindProperty("m_OcclusionCulling");
			this.m_TargetTexture = base.serializedObject.FindProperty("m_TargetTexture");
			this.m_HDR = base.serializedObject.FindProperty("m_HDR");
			Camera camera = (Camera)this.target;
			this.m_ShowBGColorOptions.value = (!this.m_ClearFlags.hasMultipleDifferentValues && (camera.clearFlags == CameraClearFlags.Color || camera.clearFlags == CameraClearFlags.Skybox));
			this.m_ShowOrthoOptions.value = camera.orthographic;
			this.m_ShowDeferredWarning.value = this.deferredWarningValue;
			this.m_ShowBGColorOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowOrthoOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowDeferredWarning.valueChanged.AddListener(new UnityAction(base.Repaint));
		}
		private void OnDisable()
		{
			this.m_ShowBGColorOptions.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowOrthoOptions.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowDeferredWarning.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Camera camera = (Camera)this.target;
			this.m_ShowBGColorOptions.target = (!this.m_ClearFlags.hasMultipleDifferentValues && (camera.clearFlags == CameraClearFlags.Color || camera.clearFlags == CameraClearFlags.Skybox));
			this.m_ShowOrthoOptions.target = (!this.m_Orthographic.hasMultipleDifferentValues && camera.orthographic);
			this.m_ShowDeferredWarning.target = this.deferredWarningValue;
			EditorGUILayout.PropertyField(this.m_ClearFlags, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowBGColorOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_BackgroundColor, new GUIContent("Background", "Camera clears the screen to this color before rendering."), new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(this.m_CullingMask, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			CameraEditor.ProjectionType projectionType = (!this.m_Orthographic.boolValue) ? CameraEditor.ProjectionType.Perspective : CameraEditor.ProjectionType.Orthographic;
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = this.m_Orthographic.hasMultipleDifferentValues;
			projectionType = (CameraEditor.ProjectionType)EditorGUILayout.EnumPopup("Projection", projectionType, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Orthographic.boolValue = (projectionType == CameraEditor.ProjectionType.Orthographic);
			}
			if (!this.m_Orthographic.hasMultipleDifferentValues)
			{
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowOrthoOptions.faded))
				{
					EditorGUILayout.PropertyField(this.m_OrthographicSize, new GUIContent("Size"), new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				if (EditorGUILayout.BeginFadeGroup(1f - this.m_ShowOrthoOptions.faded))
				{
					EditorGUILayout.Slider(this.m_FieldOfView, 1f, 179f, new GUIContent("Field of View"), new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
			}
			Rect controlRect = EditorGUILayout.GetControlRect(true, 32f, new GUILayoutOption[0]);
			controlRect.height = 16f;
			GUI.Label(controlRect, this.m_ClipingPlanesLabel);
			controlRect.xMin += EditorGUIUtility.labelWidth - 1f;
			EditorGUIUtility.labelWidth = 32f;
			EditorGUI.PropertyField(controlRect, this.m_NearClip, this.m_NearClipPlaneLabel);
			controlRect.y += 16f;
			EditorGUI.PropertyField(controlRect, this.m_FarClip, this.m_FarClipPlaneLabel);
			EditorGUIUtility.labelWidth = 0f;
			EditorGUILayout.PropertyField(this.m_NormalizedViewPortRect, this.m_ViewportLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_Depth, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_RenderingPath, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowDeferredWarning.faded))
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("CameraEditor.DeferredProOnly");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, false);
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(this.m_TargetTexture, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_OcclusionCulling, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_HDR, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
		public void OnOverlayGUI(UnityEngine.Object target, SceneView sceneView)
		{
			if (target == null)
			{
				return;
			}
			Camera camera = (Camera)target;
			Vector2 sizeOfMainGameView = GameView.GetSizeOfMainGameView();
			if (sizeOfMainGameView.x < 0f)
			{
				sizeOfMainGameView.x = sceneView.position.width;
				sizeOfMainGameView.y = sceneView.position.height;
			}
			Rect rect = camera.rect;
			sizeOfMainGameView.x *= Mathf.Max(rect.width, 0f);
			sizeOfMainGameView.y *= Mathf.Max(rect.height, 0f);
			if (sizeOfMainGameView.x <= 0f || sizeOfMainGameView.y <= 0f)
			{
				return;
			}
			float num = sizeOfMainGameView.x / sizeOfMainGameView.y;
			sizeOfMainGameView.y = 0.2f * sceneView.position.height;
			sizeOfMainGameView.x = sizeOfMainGameView.y * num;
			if (sizeOfMainGameView.y > sceneView.position.height * 0.5f)
			{
				sizeOfMainGameView.y = sceneView.position.height * 0.5f;
				sizeOfMainGameView.x = sizeOfMainGameView.y * num;
			}
			if (sizeOfMainGameView.x > sceneView.position.width * 0.5f)
			{
				sizeOfMainGameView.x = sceneView.position.width * 0.5f;
				sizeOfMainGameView.y = sizeOfMainGameView.x / num;
			}
			Rect rect2 = GUILayoutUtility.GetRect(sizeOfMainGameView.x, sizeOfMainGameView.y);
			rect2.y = sceneView.position.height - rect2.y - rect2.height + 1f;
			if (Event.current.type == EventType.Repaint)
			{
				this.previewCamera.CopyFrom(camera);
				this.previewCamera.targetTexture = null;
				this.previewCamera.pixelRect = rect2;
				Handles.EmitGUIGeometryForCamera(camera, this.previewCamera);
				this.previewCamera.Render();
			}
		}
		private static float GetGameViewAspectRatio(Camera fallbackCamera)
		{
			Vector2 sizeOfMainGameView = GameView.GetSizeOfMainGameView();
			if (sizeOfMainGameView.x < 0f)
			{
				sizeOfMainGameView.x = (float)Screen.width;
				sizeOfMainGameView.y = (float)Screen.height;
			}
			return sizeOfMainGameView.x / sizeOfMainGameView.y;
		}
		private static float GetFrustumAspectRatio(Camera camera)
		{
			Rect rect = camera.rect;
			if (rect.width <= 0f || rect.height <= 0f)
			{
				return -1f;
			}
			float num = rect.width / rect.height;
			return CameraEditor.GetGameViewAspectRatio(camera) * num;
		}
		private static bool GetFrustum(Camera camera, Vector3[] near, Vector3[] far, out float frustumAspect)
		{
			frustumAspect = CameraEditor.GetFrustumAspectRatio(camera);
			if (frustumAspect < 0f)
			{
				return false;
			}
			float num3;
			float num4;
			float num5;
			float num6;
			if (!camera.isOrthoGraphic)
			{
				float num = Mathf.Tan(camera.fieldOfView * 0.0174532924f * 0.5f);
				float num2 = num * frustumAspect;
				num3 = num2 * camera.farClipPlane;
				num4 = num * camera.farClipPlane;
				num5 = num2 * camera.nearClipPlane;
				num6 = num * camera.nearClipPlane;
			}
			else
			{
				float num = camera.orthographicSize;
				float num2 = num * frustumAspect;
				num3 = num2;
				num4 = num;
				num5 = num2;
				num6 = num;
			}
			Matrix4x4 matrix4x = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, Vector3.one);
			if (far != null)
			{
				far[0] = new Vector3(-num3, -num4, camera.farClipPlane);
				far[1] = new Vector3(-num3, num4, camera.farClipPlane);
				far[2] = new Vector3(num3, num4, camera.farClipPlane);
				far[3] = new Vector3(num3, -num4, camera.farClipPlane);
				for (int i = 0; i < 4; i++)
				{
					far[i] = matrix4x.MultiplyPoint(far[i]);
				}
			}
			if (near != null)
			{
				near[0] = new Vector3(-num5, -num6, camera.nearClipPlane);
				near[1] = new Vector3(-num5, num6, camera.nearClipPlane);
				near[2] = new Vector3(num5, num6, camera.nearClipPlane);
				near[3] = new Vector3(num5, -num6, camera.nearClipPlane);
				for (int j = 0; j < 4; j++)
				{
					near[j] = matrix4x.MultiplyPoint(near[j]);
				}
			}
			return true;
		}
		private static void RenderGizmo(Camera camera)
		{
			Vector3[] array = new Vector3[4];
			Vector3[] array2 = new Vector3[4];
			float num;
			if (CameraEditor.GetFrustum(camera, array, array2, out num))
			{
				Color color = Handles.color;
				Handles.color = CameraEditor.kGizmoCamera;
				for (int i = 0; i < 4; i++)
				{
					Handles.DrawLine(array[i], array[(i + 1) % 4]);
					Handles.DrawLine(array2[i], array2[(i + 1) % 4]);
					Handles.DrawLine(array[i], array2[i]);
				}
				Handles.color = color;
			}
		}
		private static bool IsViewPortRectValidToRender(Rect normalizedViewPortRect)
		{
			return normalizedViewPortRect.width > 0f && normalizedViewPortRect.height > 0f && normalizedViewPortRect.x < 1f && normalizedViewPortRect.xMax > 0f && normalizedViewPortRect.y < 1f && normalizedViewPortRect.yMax > 0f;
		}
		public void OnSceneGUI()
		{
			Camera camera = (Camera)this.target;
			if (!CameraEditor.IsViewPortRectValidToRender(camera.rect))
			{
				return;
			}
			SceneViewOverlay.Window(new GUIContent("Camera Preview"), new SceneViewOverlay.WindowFunction(this.OnOverlayGUI), -100, this.target, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
			Color color = Handles.color;
			Color color2 = CameraEditor.kGizmoCamera;
			color2.a *= 2f;
			Handles.color = color2;
			Vector3[] array = new Vector3[4];
			float num;
			if (!CameraEditor.GetFrustum(camera, null, array, out num))
			{
				return;
			}
			Vector3 vector = array[0];
			Vector3 vector2 = array[1];
			Vector3 vector3 = array[2];
			Vector3 vector4 = array[3];
			bool changed = GUI.changed;
			Vector3 vector5 = Vector3.Lerp(vector, vector3, 0.5f);
			float num2 = -1f;
			Vector3 a = CameraEditor.MidPointPositionSlider(vector2, vector3, camera.transform.up);
			if (!GUI.changed)
			{
				a = CameraEditor.MidPointPositionSlider(vector, vector4, -camera.transform.up);
			}
			if (GUI.changed)
			{
				num2 = (a - vector5).magnitude;
			}
			GUI.changed = false;
			a = CameraEditor.MidPointPositionSlider(vector4, vector3, camera.transform.right);
			if (!GUI.changed)
			{
				a = CameraEditor.MidPointPositionSlider(vector, vector2, -camera.transform.right);
			}
			if (GUI.changed)
			{
				num2 = (a - vector5).magnitude / num;
			}
			if (num2 >= 0f)
			{
				Undo.RecordObject(camera, "Adjust Camera");
				if (camera.orthographic)
				{
					camera.orthographicSize = num2;
				}
				else
				{
					Vector3 a2 = vector5 + camera.transform.up * num2;
					camera.fieldOfView = Vector3.Angle(camera.transform.forward, a2 - camera.transform.position) * 2f;
				}
				changed = true;
			}
			GUI.changed = changed;
			Handles.color = color;
		}
		private static Vector3 MidPointPositionSlider(Vector3 position1, Vector3 position2, Vector3 direction)
		{
			Vector3 position3 = Vector3.Lerp(position1, position2, 0.5f);
			return Handles.Slider(position3, direction, HandleUtility.GetHandleSize(position3) * 0.03f, new Handles.DrawCapFunction(Handles.DotCap), 0f);
		}
	}
}
