using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor.AnimatedValues;
using UnityEditor.Modules;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Camera))]
	internal class CameraEditor : Editor
	{
		private class Styles
		{
			public static GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove command buffer");

			public static GUIStyle invisibleButton = "InvisibleButton";
		}

		private enum ProjectionType
		{
			Perspective,
			Orthographic
		}

		private static readonly GUIContent[] kCameraRenderPaths = new GUIContent[]
		{
			new GUIContent("Use Graphics Settings"),
			new GUIContent("Forward"),
			new GUIContent("Deferred"),
			new GUIContent("Legacy Vertex Lit"),
			new GUIContent("Legacy Deferred (light prepass)")
		};

		private static readonly int[] kCameraRenderPathValues = new int[]
		{
			-1,
			1,
			3,
			0,
			2
		};

		private SerializedProperty m_ClearFlags;

		private SerializedProperty m_BackgroundColor;

		private SerializedProperty m_NormalizedViewPortRect;

		private SerializedProperty m_FieldOfView;

		private SerializedProperty m_Orthographic;

		private SerializedProperty m_OrthographicSize;

		private SerializedProperty m_Depth;

		private SerializedProperty m_CullingMask;

		private SerializedProperty m_RenderingPath;

		private SerializedProperty m_OcclusionCulling;

		private SerializedProperty m_TargetTexture;

		private SerializedProperty m_HDR;

		private SerializedProperty[] m_NearAndFarClippingPlanes;

		private SerializedProperty m_StereoConvergence;

		private SerializedProperty m_StereoSeparation;

		private SerializedProperty m_TargetDisplay;

		private SerializedProperty m_TargetEye;

		private static readonly GUIContent[] kTargetEyes = new GUIContent[]
		{
			new GUIContent("Both"),
			new GUIContent("Left"),
			new GUIContent("Right"),
			new GUIContent("None (Main Display)")
		};

		private static readonly int[] kTargetEyeValues = new int[]
		{
			3,
			1,
			2,
			0
		};

		private readonly AnimBool m_ShowBGColorOptions = new AnimBool();

		private readonly AnimBool m_ShowOrthoOptions = new AnimBool();

		private readonly AnimBool m_ShowTargetEyeOption = new AnimBool();

		private Camera m_PreviewCamera;

		private static readonly Color kGizmoCamera = new Color(0.9137255f, 0.9137255f, 0.9137255f, 0.5019608f);

		private const float kPreviewWindowOffset = 10f;

		private const float kPreviewNormalizedSize = 0.2f;

		private readonly GUIContent m_ViewportLabel = new GUIContent("Viewport Rect");

		private bool m_CommandBuffersShown = true;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		private Camera camera
		{
			get
			{
				return base.target as Camera;
			}
		}

		private bool wantDeferredRendering
		{
			get
			{
				bool flag = CameraEditor.IsDeferredRenderingPath(this.camera.renderingPath);
				bool flag2 = CameraEditor.IsDeferredRenderingPath(EditorGraphicsSettings.GetCurrentTierSettings().renderingPath);
				return flag || (this.camera.renderingPath == RenderingPath.UsePlayerSettings && flag2);
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
						typeof(Camera),
						typeof(Skybox)
					}).GetComponent<Camera>();
				}
				this.m_PreviewCamera.enabled = false;
				return this.m_PreviewCamera;
			}
		}

		private bool ShouldShowTargetDisplayProperty()
		{
			GUIContent[] displayNames = ModuleManager.GetDisplayNames(EditorUserBuildSettings.activeBuildTarget.ToString());
			return BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone || displayNames != null;
		}

		private static bool IsDeferredRenderingPath(RenderingPath rp)
		{
			return rp == RenderingPath.DeferredLighting || rp == RenderingPath.DeferredShading;
		}

		public void OnEnable()
		{
			this.m_ClearFlags = base.serializedObject.FindProperty("m_ClearFlags");
			this.m_BackgroundColor = base.serializedObject.FindProperty("m_BackGroundColor");
			this.m_NormalizedViewPortRect = base.serializedObject.FindProperty("m_NormalizedViewPortRect");
			this.m_NearAndFarClippingPlanes = new SerializedProperty[]
			{
				base.serializedObject.FindProperty("near clip plane"),
				base.serializedObject.FindProperty("far clip plane")
			};
			this.m_FieldOfView = base.serializedObject.FindProperty("field of view");
			this.m_Orthographic = base.serializedObject.FindProperty("orthographic");
			this.m_OrthographicSize = base.serializedObject.FindProperty("orthographic size");
			this.m_Depth = base.serializedObject.FindProperty("m_Depth");
			this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
			this.m_RenderingPath = base.serializedObject.FindProperty("m_RenderingPath");
			this.m_OcclusionCulling = base.serializedObject.FindProperty("m_OcclusionCulling");
			this.m_TargetTexture = base.serializedObject.FindProperty("m_TargetTexture");
			this.m_HDR = base.serializedObject.FindProperty("m_HDR");
			this.m_StereoConvergence = base.serializedObject.FindProperty("m_StereoConvergence");
			this.m_StereoSeparation = base.serializedObject.FindProperty("m_StereoSeparation");
			this.m_TargetDisplay = base.serializedObject.FindProperty("m_TargetDisplay");
			this.m_TargetEye = base.serializedObject.FindProperty("m_TargetEye");
			Camera camera = (Camera)base.target;
			this.m_ShowBGColorOptions.value = (!this.m_ClearFlags.hasMultipleDifferentValues && (camera.clearFlags == CameraClearFlags.Color || camera.clearFlags == CameraClearFlags.Skybox));
			this.m_ShowOrthoOptions.value = camera.orthographic;
			this.m_ShowTargetEyeOption.value = (this.m_TargetEye.intValue != 3 || PlayerSettings.virtualRealitySupported);
			this.m_ShowBGColorOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowOrthoOptions.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowTargetEyeOption.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		internal void OnDisable()
		{
			this.m_ShowBGColorOptions.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowOrthoOptions.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowTargetEyeOption.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public void OnDestroy()
		{
			if (this.m_PreviewCamera != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_PreviewCamera.gameObject, true);
			}
		}

		private void DepthTextureModeGUI()
		{
			if (base.targets.Length == 1)
			{
				Camera camera = base.target as Camera;
				if (!(camera == null) && camera.depthTextureMode != DepthTextureMode.None)
				{
					List<string> list = new List<string>();
					if ((camera.depthTextureMode & DepthTextureMode.Depth) != DepthTextureMode.None)
					{
						list.Add("Depth");
					}
					if ((camera.depthTextureMode & DepthTextureMode.DepthNormals) != DepthTextureMode.None)
					{
						list.Add("DepthNormals");
					}
					if ((camera.depthTextureMode & DepthTextureMode.MotionVectors) != DepthTextureMode.None)
					{
						list.Add("MotionVectors");
					}
					if (list.Count != 0)
					{
						StringBuilder stringBuilder = new StringBuilder("Info: renders ");
						for (int i = 0; i < list.Count; i++)
						{
							if (i != 0)
							{
								stringBuilder.Append(" & ");
							}
							stringBuilder.Append(list[i]);
						}
						stringBuilder.Append((list.Count <= 1) ? " texture" : " textures");
						EditorGUILayout.HelpBox(stringBuilder.ToString(), MessageType.None, true);
					}
				}
			}
		}

		private static Rect GetRemoveButtonRect(Rect r)
		{
			Vector2 vector = CameraEditor.Styles.invisibleButton.CalcSize(CameraEditor.Styles.iconRemove);
			return new Rect(r.xMax - vector.x, r.y + (float)((int)(r.height / 2f - vector.y / 2f)), vector.x, vector.y);
		}

		[DrawGizmo(GizmoType.NonSelected)]
		private static void DrawCameraBound(Camera camera, GizmoType gizmoType)
		{
			SceneView currentDrawingSceneView = SceneView.currentDrawingSceneView;
			if (currentDrawingSceneView != null && currentDrawingSceneView.in2DMode)
			{
				if (camera == Camera.main && camera.orthographic)
				{
					CameraEditor.RenderGizmo(camera);
				}
			}
		}

		private void CommandBufferGUI()
		{
			if (base.targets.Length == 1)
			{
				Camera camera = base.target as Camera;
				if (!(camera == null))
				{
					int commandBufferCount = camera.commandBufferCount;
					if (commandBufferCount != 0)
					{
						this.m_CommandBuffersShown = GUILayout.Toggle(this.m_CommandBuffersShown, GUIContent.Temp(commandBufferCount + " command buffers"), EditorStyles.foldout, new GUILayoutOption[0]);
						if (this.m_CommandBuffersShown)
						{
							EditorGUI.indentLevel++;
							CameraEvent[] array = (CameraEvent[])Enum.GetValues(typeof(CameraEvent));
							for (int i = 0; i < array.Length; i++)
							{
								CameraEvent cameraEvent = array[i];
								CommandBuffer[] commandBuffers = camera.GetCommandBuffers(cameraEvent);
								CommandBuffer[] array2 = commandBuffers;
								for (int j = 0; j < array2.Length; j++)
								{
									CommandBuffer commandBuffer = array2[j];
									using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
									{
										Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
										rect.xMin += EditorGUI.indent;
										Rect removeButtonRect = CameraEditor.GetRemoveButtonRect(rect);
										rect.xMax = removeButtonRect.x;
										GUI.Label(rect, string.Format("{0}: {1} ({2})", cameraEvent, commandBuffer.name, EditorUtility.FormatBytes(commandBuffer.sizeInBytes)), EditorStyles.miniLabel);
										if (GUI.Button(removeButtonRect, CameraEditor.Styles.iconRemove, CameraEditor.Styles.invisibleButton))
										{
											camera.RemoveCommandBuffer(cameraEvent, commandBuffer);
											SceneView.RepaintAll();
											GameView.RepaintAll();
											GUIUtility.ExitGUI();
										}
									}
								}
							}
							using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
							{
								GUILayout.FlexibleSpace();
								if (GUILayout.Button("Remove all", EditorStyles.miniButton, new GUILayoutOption[0]))
								{
									camera.RemoveAllCommandBuffers();
									SceneView.RepaintAll();
									GameView.RepaintAll();
								}
							}
							EditorGUI.indentLevel--;
						}
					}
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			Camera camera = (Camera)base.target;
			this.m_ShowBGColorOptions.target = (!this.m_ClearFlags.hasMultipleDifferentValues && (camera.clearFlags == CameraClearFlags.Color || camera.clearFlags == CameraClearFlags.Skybox));
			this.m_ShowOrthoOptions.target = (!this.m_Orthographic.hasMultipleDifferentValues && camera.orthographic);
			this.m_ShowTargetEyeOption.target = (this.m_TargetEye.intValue != 3 || PlayerSettings.virtualRealitySupported);
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
			EditorGUILayout.PropertiesField(EditorGUI.s_ClipingPlanesLabel, this.m_NearAndFarClippingPlanes, EditorGUI.s_NearAndFarLabels, 35f, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_NormalizedViewPortRect, this.m_ViewportLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_Depth, new GUILayoutOption[0]);
			EditorGUILayout.IntPopup(this.m_RenderingPath, CameraEditor.kCameraRenderPaths, CameraEditor.kCameraRenderPathValues, EditorGUIUtility.TempContent("Rendering Path"), new GUILayoutOption[0]);
			if (this.m_ShowOrthoOptions.target && this.wantDeferredRendering)
			{
				EditorGUILayout.HelpBox("Deferred rendering does not work with Orthographic camera, will use Forward.", MessageType.Warning, true);
			}
			EditorGUILayout.PropertyField(this.m_TargetTexture, new GUILayoutOption[0]);
			if (!this.m_TargetTexture.hasMultipleDifferentValues)
			{
				RenderTexture renderTexture = this.m_TargetTexture.objectReferenceValue as RenderTexture;
				if (renderTexture && renderTexture.antiAliasing > 1 && this.wantDeferredRendering)
				{
					EditorGUILayout.HelpBox("Manual MSAA target set with deferred rendering. This will lead to undefined behavior.", MessageType.Warning, true);
				}
			}
			EditorGUILayout.PropertyField(this.m_OcclusionCulling, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_HDR, new GUILayoutOption[0]);
			if (this.m_HDR.boolValue)
			{
				this.DisplayHDRWarnings();
			}
			if (PlayerSettings.virtualRealitySupported)
			{
				EditorGUILayout.PropertyField(this.m_StereoSeparation, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_StereoConvergence, new GUILayoutOption[0]);
			}
			if (this.ShouldShowTargetDisplayProperty())
			{
				int intValue = this.m_TargetDisplay.intValue;
				EditorGUILayout.Space();
				EditorGUILayout.IntPopup(this.m_TargetDisplay, DisplayUtility.GetDisplayNames(), DisplayUtility.GetDisplayIndices(), EditorGUIUtility.TempContent("Target Display"), new GUILayoutOption[0]);
				if (intValue != this.m_TargetDisplay.intValue)
				{
					GameView.RepaintAll();
				}
			}
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowTargetEyeOption.faded))
			{
				EditorGUILayout.IntPopup(this.m_TargetEye, CameraEditor.kTargetEyes, CameraEditor.kTargetEyeValues, EditorGUIUtility.TempContent("Target Eye"), new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			this.DepthTextureModeGUI();
			this.CommandBufferGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void DisplayHDRWarnings()
		{
			Camera camera = base.target as Camera;
			if (camera != null)
			{
				string[] hDRWarnings = camera.GetHDRWarnings();
				if (hDRWarnings.Length > 0)
				{
					EditorGUILayout.HelpBox(string.Join("\n\n", hDRWarnings), MessageType.Warning, true);
				}
			}
		}

		public void OnOverlayGUI(UnityEngine.Object target, SceneView sceneView)
		{
			if (!(target == null))
			{
				Camera camera = (Camera)target;
				Vector2 mainGameViewTargetSize = GameView.GetMainGameViewTargetSize();
				if (mainGameViewTargetSize.x < 0f)
				{
					mainGameViewTargetSize.x = sceneView.position.width;
					mainGameViewTargetSize.y = sceneView.position.height;
				}
				Rect rect = camera.rect;
				mainGameViewTargetSize.x *= Mathf.Max(rect.width, 0f);
				mainGameViewTargetSize.y *= Mathf.Max(rect.height, 0f);
				if (mainGameViewTargetSize.x > 0f && mainGameViewTargetSize.y > 0f)
				{
					float num = mainGameViewTargetSize.x / mainGameViewTargetSize.y;
					mainGameViewTargetSize.y = 0.2f * sceneView.position.height;
					mainGameViewTargetSize.x = mainGameViewTargetSize.y * num;
					if (mainGameViewTargetSize.y > sceneView.position.height * 0.5f)
					{
						mainGameViewTargetSize.y = sceneView.position.height * 0.5f;
						mainGameViewTargetSize.x = mainGameViewTargetSize.y * num;
					}
					if (mainGameViewTargetSize.x > sceneView.position.width * 0.5f)
					{
						mainGameViewTargetSize.x = sceneView.position.width * 0.5f;
						mainGameViewTargetSize.y = mainGameViewTargetSize.x / num;
					}
					Rect rect2 = GUILayoutUtility.GetRect(mainGameViewTargetSize.x, mainGameViewTargetSize.y);
					rect2 = EditorGUIUtility.PointsToPixels(rect2);
					rect2.y = (sceneView.position.height + 1f) * EditorGUIUtility.pixelsPerPoint - rect2.y - rect2.height;
					if (Event.current.type == EventType.Repaint)
					{
						this.previewCamera.CopyFrom(camera);
						Skybox component = this.previewCamera.GetComponent<Skybox>();
						if (component)
						{
							Skybox component2 = camera.GetComponent<Skybox>();
							if (component2 && component2.enabled)
							{
								component.enabled = true;
								component.material = component2.material;
							}
							else
							{
								component.enabled = false;
							}
						}
						this.previewCamera.targetTexture = null;
						this.previewCamera.pixelRect = rect2;
						Handles.EmitGUIGeometryForCamera(camera, this.previewCamera);
						this.previewCamera.Render();
					}
				}
			}
		}

		[RequiredByNativeCode]
		private static float GetGameViewAspectRatio()
		{
			Vector2 mainGameViewTargetSize = GameView.GetMainGameViewTargetSize();
			if (mainGameViewTargetSize.x < 0f)
			{
				mainGameViewTargetSize.x = (float)Screen.width;
				mainGameViewTargetSize.y = (float)Screen.height;
			}
			return mainGameViewTargetSize.x / mainGameViewTargetSize.y;
		}

		private static float GetFrustumAspectRatio(Camera camera)
		{
			Rect rect = camera.rect;
			float result;
			if (rect.width <= 0f || rect.height <= 0f)
			{
				result = -1f;
			}
			else
			{
				float num = rect.width / rect.height;
				result = CameraEditor.GetGameViewAspectRatio() * num;
			}
			return result;
		}

		private static bool GetFrustum(Camera camera, Vector3[] near, Vector3[] far, out float frustumAspect)
		{
			frustumAspect = CameraEditor.GetFrustumAspectRatio(camera);
			bool result;
			if (frustumAspect < 0f)
			{
				result = false;
			}
			else
			{
				if (far != null)
				{
					far[0] = new Vector3(0f, 0f, camera.farClipPlane);
					far[1] = new Vector3(0f, 1f, camera.farClipPlane);
					far[2] = new Vector3(1f, 1f, camera.farClipPlane);
					far[3] = new Vector3(1f, 0f, camera.farClipPlane);
					for (int i = 0; i < 4; i++)
					{
						far[i] = camera.ViewportToWorldPoint(far[i]);
					}
				}
				if (near != null)
				{
					near[0] = new Vector3(0f, 0f, camera.nearClipPlane);
					near[1] = new Vector3(0f, 1f, camera.nearClipPlane);
					near[2] = new Vector3(1f, 1f, camera.nearClipPlane);
					near[3] = new Vector3(1f, 0f, camera.nearClipPlane);
					for (int j = 0; j < 4; j++)
					{
						near[j] = camera.ViewportToWorldPoint(near[j]);
					}
				}
				result = true;
			}
			return result;
		}

		internal static void RenderGizmo(Camera camera)
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
			Camera camera = (Camera)base.target;
			if (CameraEditor.IsViewPortRectValidToRender(camera.rect))
			{
				SceneViewOverlay.Window(new GUIContent("Camera Preview"), new SceneViewOverlay.WindowFunction(this.OnOverlayGUI), -100, base.target, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
				Color color = Handles.color;
				Color color2 = CameraEditor.kGizmoCamera;
				color2.a *= 2f;
				Handles.color = color2;
				Vector3[] array = new Vector3[4];
				float num;
				if (CameraEditor.GetFrustum(camera, null, array, out num))
				{
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
			}
		}

		private static Vector3 MidPointPositionSlider(Vector3 position1, Vector3 position2, Vector3 direction)
		{
			Vector3 vector = Vector3.Lerp(position1, position2, 0.5f);
			Vector3 arg_3E_0 = vector;
			float arg_3E_2 = HandleUtility.GetHandleSize(vector) * 0.03f;
			if (CameraEditor.<>f__mg$cache0 == null)
			{
				CameraEditor.<>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			return Handles.Slider(arg_3E_0, direction, arg_3E_2, CameraEditor.<>f__mg$cache0, 0f);
		}
	}
}
