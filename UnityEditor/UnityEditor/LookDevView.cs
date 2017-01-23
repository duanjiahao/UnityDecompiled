using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Look Dev", useTypeNameAsIconName = true)]
	internal class LookDevView : EditorWindow, IHasCustomMenu
	{
		public class Styles
		{
			public readonly GUIStyle sBigTitleInnerStyle = "IN BigTitle inner";

			public readonly GUIStyle sToolBarButton = "toolbarbutton";

			public readonly GUIContent sSingleMode1 = EditorGUIUtility.IconContent("LookDevSingle1", "Single1|Single1 object view");

			public readonly GUIContent sSingleMode2 = EditorGUIUtility.IconContent("LookDevSingle2", "Single2|Single2 object view");

			public readonly GUIContent sSideBySideMode = EditorGUIUtility.IconContent("LookDevSideBySide", "Side|Side by side comparison view");

			public readonly GUIContent sSplitMode = EditorGUIUtility.IconContent("LookDevSplit", "Split|Single object split comparison view");

			public readonly GUIContent sZoneMode = EditorGUIUtility.IconContent("LookDevZone", "Zone|Single object zone comparison view");

			public readonly GUIContent sLinkActive = EditorGUIUtility.IconContent("LookDevMirrorViewsActive", "Link|Links the property between the different views");

			public readonly GUIContent sLinkInactive = EditorGUIUtility.IconContent("LookDevMirrorViewsInactive", "Link|Links the property between the different views");

			public readonly GUIContent sDragAndDropObjsText = EditorGUIUtility.TextContent("Drag and drop Prefabs here.");

			public readonly GUIStyle[] sPropertyLabelStyle = new GUIStyle[]
			{
				new GUIStyle(EditorStyles.miniLabel),
				new GUIStyle(EditorStyles.miniLabel),
				new GUIStyle(EditorStyles.miniLabel)
			};

			public Styles()
			{
				this.sPropertyLabelStyle[0].normal.textColor = LookDevView.m_FirstViewGizmoColor;
				this.sPropertyLabelStyle[1].normal.textColor = LookDevView.m_SecondViewGizmoColor;
			}
		}

		private class PreviewContextCB
		{
			public CommandBuffer m_drawBallCB;

			public CommandBuffer m_patchGBufferCB;

			public MaterialPropertyBlock m_drawBallPB;

			public PreviewContextCB()
			{
				this.m_drawBallCB = new CommandBuffer();
				this.m_drawBallCB.name = "draw ball";
				this.m_patchGBufferCB = new CommandBuffer();
				this.m_patchGBufferCB.name = "patch gbuffer";
				this.m_drawBallPB = new MaterialPropertyBlock();
			}
		}

		private class PreviewContext
		{
			public enum PreviewContextPass
			{
				kView,
				kViewWithShadow,
				kShadow,
				kCount
			}

			public PreviewRenderUtility[] m_PreviewUtility = new PreviewRenderUtility[3];

			public Texture[] m_PreviewResult = new Texture[3];

			public LookDevView.PreviewContextCB[] m_PreviewCB = new LookDevView.PreviewContextCB[3];

			public PreviewContext()
			{
				for (int i = 0; i < 3; i++)
				{
					this.m_PreviewUtility[i] = new PreviewRenderUtility(true);
					this.m_PreviewCB[i] = new LookDevView.PreviewContextCB();
				}
			}
		}

		public static Color32 m_FirstViewGizmoColor;

		public static Color32 m_SecondViewGizmoColor;

		private static string m_configAssetPath = "Library/LookDevConfig.asset";

		private static LookDevView.Styles s_Styles = null;

		private LookDevView.PreviewContext[] m_PreviewUtilityContexts = new LookDevView.PreviewContext[2];

		private GUIContent m_RenderdocContent;

		private GUIContent m_SyncLightVertical;

		private GUIContent m_ResetEnvironment;

		private Rect[] m_PreviewRects = new Rect[3];

		private Rect m_DisplayRect;

		private Vector4 m_ScreenRatio;

		private Vector2 m_OnMouseDownOffsetToGizmo;

		private LookDevEditionContext m_CurrentDragContext = LookDevEditionContext.None;

		private LookDevOperationType m_LookDevOperationType = LookDevOperationType.None;

		private RenderTexture m_FinalCompositionTexture = null;

		private LookDevEnvironmentWindow m_LookDevEnvWindow = null;

		private bool m_ShowLookDevEnvWindow = false;

		private bool m_CaptureRD = false;

		private bool[] m_LookDevModeToggles = new bool[5];

		private float m_GizmoThickness = 0.0028f;

		private float m_GizmoThicknessSelected = 0.015f;

		private float m_GizmoCircleRadius = 0.014f;

		private float m_GizmoCircleRadiusSelected = 0.03f;

		private bool m_ForceGizmoRenderSelector = false;

		private LookDevOperationType m_GizmoRenderMode = LookDevOperationType.None;

		private float m_BlendFactorCircleSelectionRadius = 0.03f;

		private float m_BlendFactorCircleRadius = 0.01f;

		private Rect m_ControlWindowRect;

		private float kLineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		private LookDevConfig m_LookDevConfig = null;

		private LookDevEnvironmentLibrary m_LookDevEnvLibrary = null;

		[SerializeField]
		private LookDevEnvironmentLibrary m_LookDevUserEnvLibrary = null;

		private bool m_DisplayDebugGizmo = false;

		private float kReferenceScale = 1080f;

		private int m_hotControlID = 0;

		private float m_DirBias = 0.01f;

		private float m_DirNormalBias = 0.4f;

		private float m_CurrentObjRotationOffset = 0f;

		private float m_ObjRotationAcc = 0f;

		private float m_EnvRotationAcc = 0f;

		private float kDefaultSceneHeight = -500f;

		private CameraControllerStandard m_CameraController = new CameraControllerStandard();

		public static LookDevView.Styles styles
		{
			get
			{
				if (LookDevView.s_Styles == null)
				{
					LookDevView.s_Styles = new LookDevView.Styles();
				}
				return LookDevView.s_Styles;
			}
		}

		public int hotControl
		{
			get
			{
				return this.m_hotControlID;
			}
		}

		public LookDevConfig config
		{
			get
			{
				return this.m_LookDevConfig;
			}
		}

		public LookDevEnvironmentLibrary envLibrary
		{
			get
			{
				return this.m_LookDevEnvLibrary;
			}
			set
			{
				if (value == null)
				{
					this.m_LookDevEnvLibrary = ScriptableObject.CreateInstance<LookDevEnvironmentLibrary>();
					this.m_LookDevUserEnvLibrary = null;
				}
				else if (value != this.m_LookDevUserEnvLibrary)
				{
					this.m_LookDevUserEnvLibrary = value;
					this.m_LookDevEnvLibrary = UnityEngine.Object.Instantiate<LookDevEnvironmentLibrary>(value);
					this.m_LookDevEnvLibrary.SetLookDevView(this);
				}
				int hdriCount = this.m_LookDevEnvLibrary.hdriCount;
				if (this.m_LookDevConfig.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Left) >= hdriCount || this.m_LookDevConfig.GetIntProperty(LookDevProperty.HDRI, LookDevEditionContext.Right) >= hdriCount)
				{
					this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.HDRI, true);
					this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, 0);
				}
			}
		}

		public LookDevEnvironmentLibrary userEnvLibrary
		{
			get
			{
				return this.m_LookDevUserEnvLibrary;
			}
		}

		public LookDevView()
		{
			for (int i = 0; i < 5; i++)
			{
				this.m_LookDevModeToggles[i] = false;
			}
			base.wantsMouseMove = true;
		}

		public static void DrawFullScreenQuad(Rect previewRect)
		{
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Viewport(previewRect);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0f);
			GL.End();
			GL.PopMatrix();
			GL.sRGBWrite = false;
		}

		public void CreateNewLibrary(string assetPath)
		{
			LookDevEnvironmentLibrary asset = UnityEngine.Object.Instantiate<LookDevEnvironmentLibrary>(this.envLibrary);
			AssetDatabase.CreateAsset(asset, assetPath);
			this.envLibrary = (AssetDatabase.LoadAssetAtPath(assetPath, typeof(LookDevEnvironmentLibrary)) as LookDevEnvironmentLibrary);
		}

		public static void OpenInLookDevTool(UnityEngine.Object go)
		{
			LookDevView window = EditorWindow.GetWindow<LookDevView>();
			window.m_LookDevConfig.SetCurrentPreviewObject(go as GameObject, LookDevEditionContext.Left);
			window.m_LookDevConfig.SetCurrentPreviewObject(go as GameObject, LookDevEditionContext.Right);
			window.Frame(LookDevEditionContext.Left, false);
			window.Repaint();
		}

		private void Initialize()
		{
			LookDevResources.Initialize();
			this.LoadLookDevConfig();
			if (this.m_PreviewUtilityContexts[0] == null)
			{
				for (int i = 0; i < 2; i++)
				{
					this.m_PreviewUtilityContexts[i] = new LookDevView.PreviewContext();
					for (int j = 0; j < 3; j++)
					{
						this.m_PreviewUtilityContexts[i].m_PreviewUtility[j].m_CameraFieldOfView = 30f;
						this.m_PreviewUtilityContexts[i].m_PreviewUtility[j].m_Camera.cullingMask = 1 << Camera.PreviewCullingLayer;
					}
				}
				if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
				{
					Debug.LogWarning("Look Dev is designed for linear color space. Currently project is set to gamma color space. This can be changed in player settings.");
				}
				if (EditorGraphicsSettings.GetCurrentTierSettings().renderingPath != RenderingPath.DeferredShading)
				{
					Debug.LogWarning("Look Dev switched rendering mode to deferred shading for display.");
				}
				if (!Camera.main.hdr)
				{
					Debug.LogWarning("Look Dev switched HDR mode on for display.");
				}
			}
			if (this.m_LookDevEnvLibrary.hdriList.Count == 0)
			{
				this.UpdateContextWithCurrentHDRI(LookDevResources.m_DefaultHDRI);
			}
			if (this.m_LookDevEnvWindow == null)
			{
				this.m_LookDevEnvWindow = new LookDevEnvironmentWindow(this);
			}
		}

		private void Cleanup()
		{
			LookDevResources.Cleanup();
			this.m_LookDevConfig.Cleanup();
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (this.m_PreviewUtilityContexts[i] != null)
					{
						if (this.m_PreviewUtilityContexts[i].m_PreviewUtility[j] != null)
						{
							this.m_PreviewUtilityContexts[i].m_PreviewUtility[j].Cleanup();
							this.m_PreviewUtilityContexts[i].m_PreviewUtility[j] = null;
						}
					}
				}
			}
			if (this.m_FinalCompositionTexture)
			{
				UnityEngine.Object.DestroyImmediate(this.m_FinalCompositionTexture);
				this.m_FinalCompositionTexture = null;
			}
		}

		public void OnDestroy()
		{
			this.SaveLookDevConfig();
			this.Cleanup();
		}

		private void UpdateRenderTexture(Rect rect)
		{
			int num = (int)rect.width;
			int num2 = (int)rect.height;
			if (!this.m_FinalCompositionTexture || this.m_FinalCompositionTexture.width != num || this.m_FinalCompositionTexture.height != num2)
			{
				if (this.m_FinalCompositionTexture)
				{
					UnityEngine.Object.DestroyImmediate(this.m_FinalCompositionTexture);
					this.m_FinalCompositionTexture = null;
				}
				this.m_FinalCompositionTexture = new RenderTexture(num, num2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
				this.m_FinalCompositionTexture.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		private void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
		{
			MeshRenderer meshRenderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
			MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if (meshRenderer && meshFilter && meshFilter.sharedMesh)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = meshRenderer.bounds;
				}
				else
				{
					bounds.Encapsulate(meshRenderer.bounds);
				}
			}
			SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			if (skinnedMeshRenderer && skinnedMeshRenderer.sharedMesh)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = skinnedMeshRenderer.bounds;
				}
				else
				{
					bounds.Encapsulate(skinnedMeshRenderer.bounds);
				}
			}
			SpriteRenderer spriteRenderer = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
			if (spriteRenderer && spriteRenderer.sprite)
			{
				if (bounds.extents == Vector3.zero)
				{
					bounds = spriteRenderer.bounds;
				}
				else
				{
					bounds.Encapsulate(spriteRenderer.bounds);
				}
			}
			IEnumerator enumerator = go.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					this.GetRenderableBoundsRecurse(ref bounds, transform.gameObject);
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

		private void RenderScene(Rect previewRect, LookDevContext lookDevContext, LookDevView.PreviewContext previewUtilityContext, GameObject currentObject, CameraState originalCameraState, bool secondView)
		{
			bool flag = !this.m_LookDevConfig.enableShadowCubemap || (this.m_LookDevConfig.enableShadowCubemap && lookDevContext.shadingMode != -1 && lookDevContext.shadingMode != 2);
			previewUtilityContext.m_PreviewResult[2] = ((!flag) ? this.RenderScene(previewRect, lookDevContext, previewUtilityContext, currentObject, originalCameraState, null, LookDevView.PreviewContext.PreviewContextPass.kShadow, secondView) : Texture2D.whiteTexture);
			CubemapInfo cubemapInfo = this.m_LookDevEnvLibrary.hdriList[lookDevContext.currentHDRIIndex];
			previewUtilityContext.m_PreviewResult[0] = this.RenderScene(previewRect, lookDevContext, previewUtilityContext, currentObject, originalCameraState, cubemapInfo, LookDevView.PreviewContext.PreviewContextPass.kView, secondView);
			previewUtilityContext.m_PreviewResult[1] = this.RenderScene(previewRect, lookDevContext, previewUtilityContext, currentObject, originalCameraState, cubemapInfo.cubemapShadowInfo, LookDevView.PreviewContext.PreviewContextPass.kViewWithShadow, secondView);
		}

		private Texture RenderScene(Rect previewRect, LookDevContext lookDevContext, LookDevView.PreviewContext previewUtilityContext, GameObject currentObject, CameraState originalCameraState, CubemapInfo cubemapInfo, LookDevView.PreviewContext.PreviewContextPass contextPass, bool secondView)
		{
			PreviewRenderUtility previewRenderUtility = previewUtilityContext.m_PreviewUtility[(int)contextPass];
			LookDevView.PreviewContextCB previewContextCB = previewUtilityContext.m_PreviewCB[(int)contextPass];
			previewRenderUtility.BeginPreviewHDR(previewRect, LookDevView.styles.sBigTitleInnerStyle);
			bool flag = contextPass == LookDevView.PreviewContext.PreviewContextPass.kShadow;
			DrawCameraMode shadingMode = (DrawCameraMode)lookDevContext.shadingMode;
			bool flag2 = shadingMode != DrawCameraMode.Normal && shadingMode != DrawCameraMode.TexturedWire;
			float shadowDistance = QualitySettings.shadowDistance;
			Vector3 shadowCascade4Split = QualitySettings.shadowCascade4Split;
			float angleOffset = this.m_LookDevEnvLibrary.hdriList[lookDevContext.currentHDRIIndex].angleOffset;
			float num = -(lookDevContext.envRotation + angleOffset);
			CameraState cameraState = originalCameraState.Clone();
			Vector3 eulerAngles = cameraState.rotation.value.eulerAngles;
			cameraState.rotation.value = Quaternion.Euler(eulerAngles + new Vector3(0f, num, 0f));
			cameraState.pivot.value = new Vector3(0f, this.kDefaultSceneHeight, 0f);
			cameraState.UpdateCamera(previewRenderUtility.m_Camera);
			previewRenderUtility.m_Camera.renderingPath = RenderingPath.DeferredShading;
			previewRenderUtility.m_Camera.clearFlags = ((!flag) ? CameraClearFlags.Skybox : CameraClearFlags.Color);
			previewRenderUtility.m_Camera.backgroundColor = Color.white;
			previewRenderUtility.m_Camera.hdr = true;
			for (int i = 0; i < 2; i++)
			{
				previewRenderUtility.m_Light[i].enabled = false;
				previewRenderUtility.m_Light[i].intensity = 0f;
				previewRenderUtility.m_Light[i].shadows = LightShadows.None;
			}
			if (currentObject != null && flag && this.m_LookDevConfig.enableShadowCubemap && !flag2)
			{
				Bounds bounds = new Bounds(currentObject.transform.position, Vector3.zero);
				this.GetRenderableBoundsRecurse(ref bounds, currentObject);
				float num2 = Mathf.Max(bounds.max.x, Mathf.Max(bounds.max.y, bounds.max.z));
				float num3 = (this.m_LookDevConfig.shadowDistance <= 0f) ? (25f * num2) : this.m_LookDevConfig.shadowDistance;
				float num4 = Mathf.Min(num2 * 2f, 20f) / num3;
				QualitySettings.shadowDistance = num3;
				QualitySettings.shadowCascade4Split = new Vector3(Mathf.Clamp(num4, 0f, 1f), Mathf.Clamp(num4 * 2f, 0f, 1f), Mathf.Clamp(num4 * 6f, 0f, 1f));
				ShadowInfo shadowInfo = this.m_LookDevEnvLibrary.hdriList[lookDevContext.currentHDRIIndex].shadowInfo;
				previewRenderUtility.m_Light[0].intensity = 1f;
				previewRenderUtility.m_Light[0].color = Color.white;
				previewRenderUtility.m_Light[0].shadows = LightShadows.Soft;
				previewRenderUtility.m_Light[0].shadowBias = this.m_DirBias;
				previewRenderUtility.m_Light[0].shadowNormalBias = this.m_DirNormalBias;
				previewRenderUtility.m_Light[0].transform.rotation = Quaternion.Euler(shadowInfo.latitude, shadowInfo.longitude, 0f);
				previewContextCB.m_patchGBufferCB.Clear();
				RenderTargetIdentifier[] colors = new RenderTargetIdentifier[]
				{
					BuiltinRenderTextureType.GBuffer0,
					BuiltinRenderTextureType.GBuffer1
				};
				previewContextCB.m_patchGBufferCB.SetRenderTarget(colors, BuiltinRenderTextureType.CameraTarget);
				previewContextCB.m_patchGBufferCB.DrawMesh(LookDevResources.m_ScreenQuadMesh, Matrix4x4.identity, LookDevResources.m_GBufferPatchMaterial);
				previewRenderUtility.m_Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, previewContextCB.m_patchGBufferCB);
				if (this.m_LookDevConfig.showBalls)
				{
					previewContextCB.m_drawBallCB.Clear();
					RenderTargetIdentifier[] colors2 = new RenderTargetIdentifier[]
					{
						BuiltinRenderTextureType.CameraTarget
					};
					previewContextCB.m_drawBallCB.SetRenderTarget(colors2, BuiltinRenderTextureType.CameraTarget);
					previewContextCB.m_drawBallPB.SetVector("_WindowsSize", new Vector4((float)previewRenderUtility.m_Camera.pixelWidth, (float)previewRenderUtility.m_Camera.pixelHeight, (!secondView) ? 0f : 1f, 0f));
					previewContextCB.m_drawBallCB.DrawMesh(LookDevResources.m_ScreenQuadMesh, Matrix4x4.identity, LookDevResources.m_DrawBallsMaterial, 0, 1, previewContextCB.m_drawBallPB);
					previewRenderUtility.m_Camera.AddCommandBuffer(CameraEvent.AfterLighting, previewContextCB.m_drawBallCB);
				}
			}
			Color ambient = new Color(0f, 0f, 0f, 0f);
			UnityEngine.Rendering.DefaultReflectionMode defaultReflectionMode = RenderSettings.defaultReflectionMode;
			AmbientMode ambientMode = RenderSettings.ambientMode;
			Cubemap customReflection = RenderSettings.customReflection;
			Material skybox = RenderSettings.skybox;
			float ambientIntensity = RenderSettings.ambientIntensity;
			SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
			float reflectionIntensity = RenderSettings.reflectionIntensity;
			RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
			Cubemap cubemap = (cubemapInfo == null) ? null : cubemapInfo.cubemap;
			LookDevResources.m_SkyboxMaterial.SetTexture("_Tex", cubemap);
			LookDevResources.m_SkyboxMaterial.SetFloat("_Exposure", 1f);
			RenderSettings.customReflection = cubemap;
			if (cubemapInfo != null && !cubemapInfo.alreadyComputed && !flag)
			{
				RenderSettings.skybox = LookDevResources.m_SkyboxMaterial;
				DynamicGI.UpdateEnvironment();
				cubemapInfo.ambientProbe = RenderSettings.ambientProbe;
				RenderSettings.skybox = skybox;
				cubemapInfo.alreadyComputed = true;
			}
			RenderSettings.ambientProbe = ((cubemapInfo == null) ? LookDevResources.m_ZeroAmbientProbe : cubemapInfo.ambientProbe);
			RenderSettings.skybox = LookDevResources.m_SkyboxMaterial;
			RenderSettings.ambientIntensity = 1f;
			RenderSettings.ambientMode = AmbientMode.Skybox;
			RenderSettings.reflectionIntensity = 1f;
			if (contextPass == LookDevView.PreviewContext.PreviewContextPass.kView && this.m_LookDevConfig.showBalls)
			{
				Vector4[] array = new Vector4[7];
				this.GetShaderConstantsFromNormalizedSH(RenderSettings.ambientProbe, array);
				previewContextCB.m_drawBallCB.Clear();
				RenderTargetIdentifier[] colors3 = new RenderTargetIdentifier[]
				{
					BuiltinRenderTextureType.GBuffer0,
					BuiltinRenderTextureType.GBuffer1,
					BuiltinRenderTextureType.GBuffer2,
					BuiltinRenderTextureType.CameraTarget
				};
				previewContextCB.m_drawBallCB.SetRenderTarget(colors3, BuiltinRenderTextureType.CameraTarget);
				previewContextCB.m_drawBallPB.SetVector("_SHAr", array[0]);
				previewContextCB.m_drawBallPB.SetVector("_SHAg", array[1]);
				previewContextCB.m_drawBallPB.SetVector("_SHAb", array[2]);
				previewContextCB.m_drawBallPB.SetVector("_SHBr", array[3]);
				previewContextCB.m_drawBallPB.SetVector("_SHBg", array[4]);
				previewContextCB.m_drawBallPB.SetVector("_SHBb", array[5]);
				previewContextCB.m_drawBallPB.SetVector("_SHC", array[6]);
				previewContextCB.m_drawBallPB.SetVector("_WindowsSize", new Vector4((float)previewRenderUtility.m_Camera.pixelWidth, (float)previewRenderUtility.m_Camera.pixelHeight, (!secondView) ? 0f : 1f, 0f));
				previewContextCB.m_drawBallCB.DrawMesh(LookDevResources.m_ScreenQuadMesh, Matrix4x4.identity, LookDevResources.m_DrawBallsMaterial, 0, 0, previewContextCB.m_drawBallPB);
				previewRenderUtility.m_Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, previewContextCB.m_drawBallCB);
			}
			InternalEditorUtility.SetCustomLighting(previewRenderUtility.m_Light, ambient);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			Vector3 eulerAngles2 = Vector3.zero;
			Vector3 position = Vector3.zero;
			if (currentObject != null)
			{
				LODGroup lODGroup = currentObject.GetComponent(typeof(LODGroup)) as LODGroup;
				if (lODGroup != null)
				{
					lODGroup.ForceLOD(lookDevContext.lodIndex);
				}
				this.m_LookDevConfig.SetEnabledRecursive(currentObject, true);
				eulerAngles2 = currentObject.transform.eulerAngles;
				position = currentObject.transform.localPosition;
				currentObject.transform.position = new Vector3(0f, this.kDefaultSceneHeight, 0f);
				currentObject.transform.rotation = Quaternion.identity;
				currentObject.transform.Rotate(0f, num, 0f);
				currentObject.transform.Translate(-originalCameraState.pivot.value);
				currentObject.transform.Rotate(0f, this.m_CurrentObjRotationOffset, 0f);
			}
			if (shadingMode == DrawCameraMode.TexturedWire && !flag)
			{
				Handles.ClearCamera(previewRect, previewRenderUtility.m_Camera);
				Handles.DrawCamera(previewRect, previewRenderUtility.m_Camera, shadingMode);
			}
			else
			{
				previewRenderUtility.m_Camera.Render();
			}
			if (currentObject != null)
			{
				currentObject.transform.eulerAngles = eulerAngles2;
				currentObject.transform.position = position;
				this.m_LookDevConfig.SetEnabledRecursive(currentObject, false);
			}
			if (flag2 && !flag)
			{
				if (Event.current.type == EventType.Repaint)
				{
					float scaleFactor = previewRenderUtility.GetScaleFactor(previewRect.width, previewRect.height);
					LookDevResources.m_DeferredOverlayMaterial.SetInt("_DisplayMode", shadingMode - DrawCameraMode.DeferredDiffuse);
					Graphics.DrawTexture(new Rect(0f, 0f, previewRect.width * scaleFactor, previewRect.height * scaleFactor), EditorGUIUtility.whiteTexture, LookDevResources.m_DeferredOverlayMaterial);
				}
			}
			if (flag)
			{
				previewRenderUtility.m_Camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, previewContextCB.m_patchGBufferCB);
				if (this.m_LookDevConfig.showBalls)
				{
					previewRenderUtility.m_Camera.RemoveCommandBuffer(CameraEvent.AfterLighting, previewContextCB.m_drawBallCB);
				}
			}
			else if (contextPass == LookDevView.PreviewContext.PreviewContextPass.kView && this.m_LookDevConfig.showBalls)
			{
				previewRenderUtility.m_Camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, previewContextCB.m_drawBallCB);
			}
			QualitySettings.shadowCascade4Split = shadowCascade4Split;
			QualitySettings.shadowDistance = shadowDistance;
			RenderSettings.defaultReflectionMode = defaultReflectionMode;
			RenderSettings.ambientMode = ambientMode;
			RenderSettings.customReflection = customReflection;
			RenderSettings.skybox = skybox;
			RenderSettings.ambientIntensity = ambientIntensity;
			RenderSettings.reflectionIntensity = reflectionIntensity;
			RenderSettings.ambientProbe = ambientProbe;
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			InternalEditorUtility.RemoveCustomLighting();
			return previewRenderUtility.EndPreview();
		}

		public void UpdateLookDevModeToggle(LookDevMode lookDevMode, bool value)
		{
			LookDevMode lookDevMode2;
			if (value)
			{
				this.m_LookDevModeToggles[(int)lookDevMode] = value;
				for (int i = 0; i < 5; i++)
				{
					if (i != (int)lookDevMode)
					{
						this.m_LookDevModeToggles[i] = false;
					}
				}
				lookDevMode2 = lookDevMode;
			}
			else
			{
				for (int j = 0; j < 5; j++)
				{
					if (this.m_LookDevModeToggles[j])
					{
					}
				}
				this.m_LookDevModeToggles[(int)lookDevMode] = true;
				lookDevMode2 = lookDevMode;
			}
			this.m_LookDevConfig.lookDevMode = lookDevMode2;
			base.Repaint();
		}

		private void OnUndoRedo()
		{
			base.Repaint();
		}

		private void DoAdditionalGUI()
		{
			if (this.m_LookDevConfig.lookDevMode == LookDevMode.SideBySide)
			{
				int num = 32;
				GUILayout.BeginArea(new Rect((this.m_PreviewRects[2].width - (float)num) / 2f, (this.m_PreviewRects[2].height - (float)num) / 2f, (float)num, (float)num));
				EditorGUI.BeginChangeCheck();
				bool sideBySideCameraLinked = this.m_LookDevConfig.sideBySideCameraLinked;
				bool flag = GUILayout.Toggle(sideBySideCameraLinked, this.GetGUIContentLink(sideBySideCameraLinked), LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevConfig.sideBySideCameraLinked = flag;
					if (flag)
					{
						CameraState cameraStateIn = (this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? this.m_LookDevConfig.cameraStateRight : this.m_LookDevConfig.cameraStateLeft;
						CameraState cameraState = (this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? this.m_LookDevConfig.cameraStateLeft : this.m_LookDevConfig.cameraStateRight;
						cameraState.Copy(cameraStateIn);
					}
				}
				GUILayout.EndArea();
			}
		}

		private GUIStyle GetPropertyLabelStyle(LookDevProperty property)
		{
			GUIStyle result;
			if (this.m_LookDevConfig.IsPropertyLinked(property) || this.m_LookDevConfig.lookDevMode == LookDevMode.Single1 || this.m_LookDevConfig.lookDevMode == LookDevMode.Single2)
			{
				result = LookDevView.styles.sPropertyLabelStyle[2];
			}
			else
			{
				result = LookDevView.styles.sPropertyLabelStyle[this.m_LookDevConfig.currentEditionContextIndex];
			}
			return result;
		}

		private GUIContent GetGUIContentLink(bool active)
		{
			return (!active) ? LookDevView.styles.sLinkInactive : LookDevView.styles.sLinkActive;
		}

		private void DoControlWindow()
		{
			if (this.m_LookDevConfig.showControlWindows)
			{
				float width = 68f;
				float num = 150f;
				float num2 = 30f;
				GUILayout.BeginArea(this.m_ControlWindowRect, LookDevView.styles.sBigTitleInnerStyle);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Height(this.kLineHeight)
				});
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				bool value = GUILayout.Toggle(this.m_LookDevModeToggles[0], LookDevView.styles.sSingleMode1, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.UpdateLookDevModeToggle(LookDevMode.Single1, value);
					this.m_LookDevConfig.UpdateFocus(LookDevEditionContext.Left);
					base.Repaint();
				}
				EditorGUI.BeginChangeCheck();
				value = GUILayout.Toggle(this.m_LookDevModeToggles[1], LookDevView.styles.sSingleMode2, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.UpdateLookDevModeToggle(LookDevMode.Single2, value);
					this.m_LookDevConfig.UpdateFocus(LookDevEditionContext.Right);
					base.Repaint();
				}
				EditorGUI.BeginChangeCheck();
				value = GUILayout.Toggle(this.m_LookDevModeToggles[2], LookDevView.styles.sSideBySideMode, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.UpdateLookDevModeToggle(LookDevMode.SideBySide, value);
				}
				EditorGUI.BeginChangeCheck();
				value = GUILayout.Toggle(this.m_LookDevModeToggles[3], LookDevView.styles.sSplitMode, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.UpdateLookDevModeToggle(LookDevMode.Split, value);
				}
				EditorGUI.BeginChangeCheck();
				value = GUILayout.Toggle(this.m_LookDevModeToggles[4], LookDevView.styles.sZoneMode, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.UpdateLookDevModeToggle(LookDevMode.Zone, value);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Height(this.kLineHeight)
				});
				GUILayout.Label(LookDevViewsWindow.styles.sExposure, this.GetPropertyLabelStyle(LookDevProperty.ExposureValue), new GUILayoutOption[]
				{
					GUILayout.Width(width)
				});
				float num3 = this.m_LookDevConfig.currentLookDevContext.exposureValue;
				EditorGUI.BeginChangeCheck();
				float num4 = Mathf.Round(this.m_LookDevConfig.exposureRange);
				num3 = Mathf.Clamp(GUILayout.HorizontalSlider((float)Math.Round((double)num3, (num3 >= 0f) ? 2 : 1), -num4, num4, new GUILayoutOption[]
				{
					GUILayout.Width(num)
				}), -num4, num4);
				num3 = Mathf.Clamp(EditorGUILayout.FloatField(num3, new GUILayoutOption[]
				{
					GUILayout.Width(num2)
				}), -num4, num4);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevConfig.UpdateFloatProperty(LookDevProperty.ExposureValue, num3);
				}
				EditorGUI.BeginChangeCheck();
				bool flag = this.m_LookDevConfig.IsPropertyLinked(LookDevProperty.ExposureValue);
				bool value2 = GUILayout.Toggle(flag, this.GetGUIContentLink(flag), LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.ExposureValue, value2);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Height(this.kLineHeight)
				});
				using (new EditorGUI.DisabledScope(this.m_LookDevEnvLibrary.hdriList.Count <= 1))
				{
					GUILayout.Label(LookDevViewsWindow.styles.sEnvironment, this.GetPropertyLabelStyle(LookDevProperty.HDRI), new GUILayoutOption[]
					{
						GUILayout.Width(width)
					});
					if (this.m_LookDevEnvLibrary.hdriList.Count > 1)
					{
						int num5 = this.m_LookDevEnvLibrary.hdriList.Count - 1;
						int num6 = this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex;
						EditorGUI.BeginChangeCheck();
						num6 = (int)GUILayout.HorizontalSlider((float)num6, 0f, (float)num5, new GUILayoutOption[]
						{
							GUILayout.Width(num)
						});
						num6 = Mathf.Clamp(EditorGUILayout.IntField(num6, new GUILayoutOption[]
						{
							GUILayout.Width(num2)
						}), 0, num5);
						if (EditorGUI.EndChangeCheck())
						{
							this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, num6);
						}
					}
					else
					{
						GUILayout.HorizontalSlider(0f, 0f, 0f, new GUILayoutOption[]
						{
							GUILayout.Width(num)
						});
						GUILayout.Label(LookDevViewsWindow.styles.sZero, EditorStyles.miniLabel, new GUILayoutOption[]
						{
							GUILayout.Width(num2)
						});
					}
				}
				EditorGUI.BeginChangeCheck();
				bool flag2 = this.m_LookDevConfig.IsPropertyLinked(LookDevProperty.HDRI);
				bool value3 = GUILayout.Toggle(flag2, this.GetGUIContentLink(flag2), LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.HDRI, value3);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Height(this.kLineHeight)
				});
				GUILayout.Label(LookDevViewsWindow.styles.sShadingMode, this.GetPropertyLabelStyle(LookDevProperty.ShadingMode), new GUILayoutOption[]
				{
					GUILayout.Width(width)
				});
				int num7 = this.m_LookDevConfig.currentLookDevContext.shadingMode;
				EditorGUI.BeginChangeCheck();
				num7 = EditorGUILayout.IntPopup("", num7, LookDevViewsWindow.styles.sShadingModeStrings, LookDevViewsWindow.styles.sShadingModeValues, new GUILayoutOption[]
				{
					GUILayout.Width(num2 + num + 4f)
				});
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.ShadingMode, num7);
				}
				EditorGUI.BeginChangeCheck();
				bool flag3 = this.m_LookDevConfig.IsPropertyLinked(LookDevProperty.ShadingMode);
				bool value4 = GUILayout.Toggle(flag3, this.GetGUIContentLink(flag3), LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					this.m_LookDevConfig.UpdatePropertyLink(LookDevProperty.ShadingMode, value4);
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}

		public void Update()
		{
			if (this.m_ObjRotationAcc > 0f || this.m_EnvRotationAcc > 0f)
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

		private float ComputeLookDevEnvWindowWidth()
		{
			bool flag = this.m_DisplayRect.height - 5f < 146f * (float)this.m_LookDevEnvLibrary.hdriCount;
			return 250f + ((!flag) ? 5f : 19f);
		}

		private float ComputeLookDevEnvWindowHeight()
		{
			return this.m_DisplayRect.height;
		}

		private void DoToolbarGUI()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(LookDevSettingsWindow.styles.sTitle, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			if (EditorGUI.ButtonMouseDown(rect, LookDevSettingsWindow.styles.sTitle, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				Rect last = GUILayoutUtility.topLevel.GetLast();
				PopupWindow.Show(last, new LookDevSettingsWindow(this));
				GUIUtility.ExitGUI();
			}
			rect = GUILayoutUtility.GetRect(LookDevViewsWindow.styles.sTitle, EditorStyles.toolbarDropDown, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			if (EditorGUI.ButtonMouseDown(rect, LookDevViewsWindow.styles.sTitle, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				Rect last2 = GUILayoutUtility.topLevel.GetLast();
				PopupWindow.Show(last2, new LookDevViewsWindow(this));
				GUIUtility.ExitGUI();
			}
			this.m_LookDevConfig.enableShadowCubemap = GUILayout.Toggle(this.m_LookDevConfig.enableShadowCubemap, LookDevSettingsWindow.styles.sEnableShadowIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
			this.m_LookDevConfig.rotateObjectMode = GUILayout.Toggle(this.m_LookDevConfig.rotateObjectMode, LookDevSettingsWindow.styles.sEnableObjRotationIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
			this.m_LookDevConfig.rotateEnvMode = GUILayout.Toggle(this.m_LookDevConfig.rotateEnvMode, LookDevSettingsWindow.styles.sEnableEnvRotationIcon, LookDevView.styles.sToolBarButton, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (this.m_ShowLookDevEnvWindow)
			{
				if (GUILayout.Button(this.m_SyncLightVertical, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					Undo.RecordObject(this.m_LookDevEnvLibrary, "Synchronize lights");
					int currentHDRIIndex = this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex;
					for (int i = 0; i < this.m_LookDevEnvLibrary.hdriList.Count; i++)
					{
						this.m_LookDevEnvLibrary.hdriList[i].angleOffset += this.m_LookDevEnvLibrary.hdriList[currentHDRIIndex].shadowInfo.longitude + this.m_LookDevEnvLibrary.hdriList[currentHDRIIndex].angleOffset - (this.m_LookDevEnvLibrary.hdriList[i].shadowInfo.longitude + this.m_LookDevEnvLibrary.hdriList[i].angleOffset);
						this.m_LookDevEnvLibrary.hdriList[i].angleOffset = (this.m_LookDevEnvLibrary.hdriList[i].angleOffset + 360f) % 360f;
					}
					this.m_LookDevEnvLibrary.dirty = true;
					GUIUtility.ExitGUI();
				}
				if (GUILayout.Button(this.m_ResetEnvironment, EditorStyles.toolbarButton, new GUILayoutOption[0]))
				{
					Undo.RecordObject(this.m_LookDevEnvLibrary, "Reset environment");
					for (int j = 0; j < this.m_LookDevEnvLibrary.hdriList.Count; j++)
					{
						this.m_LookDevEnvLibrary.hdriList[j].angleOffset = 0f;
					}
					this.m_LookDevEnvLibrary.dirty = true;
					GUIUtility.ExitGUI();
				}
			}
			if (RenderDoc.IsLoaded())
			{
				using (new EditorGUI.DisabledScope(!RenderDoc.IsSupported()))
				{
					if (GUILayout.Button(this.m_RenderdocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
					{
						this.m_CaptureRD = true;
						GUIUtility.ExitGUI();
					}
				}
			}
			rect = GUILayoutUtility.GetRect(LookDevEnvironmentWindow.styles.sTitle, EditorStyles.iconButton);
			if (EditorGUI.ButtonMouseDown(rect, LookDevEnvironmentWindow.styles.sTitle, FocusType.Passive, EditorStyles.iconButton))
			{
				this.m_ShowLookDevEnvWindow = !this.m_ShowLookDevEnvWindow;
			}
			if (this.m_ShowLookDevEnvWindow)
			{
				Rect gUIRect = default(Rect);
				gUIRect.x = 0f;
				gUIRect.y = 0f;
				gUIRect.width = this.ComputeLookDevEnvWindowWidth();
				gUIRect.height = this.ComputeLookDevEnvWindowHeight();
				Rect rect2 = default(Rect);
				rect2.x = this.m_DisplayRect.width - this.ComputeLookDevEnvWindowWidth();
				rect2.y = this.m_DisplayRect.y;
				rect2.width = this.ComputeLookDevEnvWindowWidth();
				rect2.height = this.ComputeLookDevEnvWindowHeight();
				this.m_LookDevEnvWindow.SetRects(rect2, gUIRect, this.m_DisplayRect);
				GUILayout.Window(0, rect2, new GUI.WindowFunction(this.m_LookDevEnvWindow.OnGUI), "", LookDevView.styles.sBigTitleInnerStyle, new GUILayoutOption[0]);
			}
			GUILayout.EndHorizontal();
		}

		private void UpdateContextWithCurrentHDRI(Cubemap cubemap)
		{
			bool recordUndo = cubemap != LookDevResources.m_DefaultHDRI;
			int num = this.m_LookDevEnvLibrary.hdriList.FindIndex((CubemapInfo x) => x.cubemap == cubemap);
			if (num == -1)
			{
				this.m_LookDevEnvLibrary.InsertHDRI(cubemap);
				num = this.m_LookDevEnvLibrary.hdriList.Count - 1;
			}
			this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, num, recordUndo);
		}

		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
			{
				menu.AddItem(new GUIContent("Load RenderDoc"), false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
			}
		}

		public void ResetView()
		{
			Undo.RecordObject(this.m_LookDevConfig, "Reset View");
			UnityEngine.Object.DestroyImmediate(this.m_LookDevConfig);
			this.m_LookDevConfig = ScriptableObject.CreateInstance<LookDevConfig>();
			this.m_LookDevConfig.SetLookDevView(this);
			this.UpdateLookDevModeToggle(this.m_LookDevConfig.lookDevMode, true);
		}

		private void LoadLookDevConfig()
		{
			if (this.m_LookDevConfig == null)
			{
				ScriptableObjectSaveLoadHelper<LookDevConfig> scriptableObjectSaveLoadHelper = new ScriptableObjectSaveLoadHelper<LookDevConfig>("asset", SaveType.Text);
				LookDevConfig lookDevConfig = scriptableObjectSaveLoadHelper.Load(LookDevView.m_configAssetPath);
				if (lookDevConfig == null)
				{
					this.m_LookDevConfig = ScriptableObject.CreateInstance<LookDevConfig>();
				}
				else
				{
					this.m_LookDevConfig = lookDevConfig;
				}
			}
			this.m_LookDevConfig.SetLookDevView(this);
			this.m_LookDevConfig.UpdateCurrentObjectArray();
			if (this.m_LookDevEnvLibrary == null)
			{
				if (this.m_LookDevUserEnvLibrary != null)
				{
					this.m_LookDevEnvLibrary = UnityEngine.Object.Instantiate<LookDevEnvironmentLibrary>(this.m_LookDevUserEnvLibrary);
				}
				else
				{
					this.envLibrary = null;
				}
			}
			this.m_LookDevEnvLibrary.SetLookDevView(this);
		}

		public void SaveLookDevConfig()
		{
			ScriptableObjectSaveLoadHelper<LookDevConfig> scriptableObjectSaveLoadHelper = new ScriptableObjectSaveLoadHelper<LookDevConfig>("asset", SaveType.Text);
			if (this.m_LookDevConfig != null)
			{
				scriptableObjectSaveLoadHelper.Save(this.m_LookDevConfig, LookDevView.m_configAssetPath);
			}
		}

		public bool SaveLookDevLibrary()
		{
			bool result;
			if (this.m_LookDevUserEnvLibrary != null)
			{
				EditorUtility.CopySerialized(this.m_LookDevEnvLibrary, this.m_LookDevUserEnvLibrary);
				EditorUtility.SetDirty(this.m_LookDevEnvLibrary);
				result = true;
			}
			else
			{
				string text = EditorUtility.SaveFilePanelInProject("Save New Environment Library", "New Env Library", "asset", "");
				if (!string.IsNullOrEmpty(text))
				{
					this.CreateNewLibrary(text);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void OnEnable()
		{
			LookDevView.m_FirstViewGizmoColor = ((!EditorGUIUtility.isProSkin) ? new Color32(0, 127, 255, 255) : new Color32(0, 204, 204, 255));
			LookDevView.m_SecondViewGizmoColor = ((!EditorGUIUtility.isProSkin) ? new Color32(255, 127, 0, 255) : new Color32(255, 107, 33, 255));
			this.LoadLookDevConfig();
			base.autoRepaintOnSceneChange = true;
			base.titleContent = base.GetLocalizedTitleContent();
			this.m_RenderdocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc");
			this.m_SyncLightVertical = EditorGUIUtility.IconContent("LookDevCenterLight", "Sync|Sync all light vertically with current light position in current selected HDRI");
			this.m_ResetEnvironment = EditorGUIUtility.IconContent("LookDevResetEnv", "Reset|Reset all environment");
			this.UpdateLookDevModeToggle(this.m_LookDevConfig.lookDevMode, true);
			this.m_LookDevConfig.cameraStateCommon.rotation.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateCommon.pivot.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateCommon.viewSize.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateLeft.rotation.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateLeft.pivot.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateLeft.viewSize.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateRight.rotation.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateRight.pivot.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LookDevConfig.cameraStateRight.viewSize.valueChanged.AddListener(new UnityAction(base.Repaint));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
			EditorApplication.editorApplicationQuit = (UnityAction)Delegate.Combine(EditorApplication.editorApplicationQuit, new UnityAction(this.OnQuit));
		}

		public void OnDisable()
		{
			this.SaveLookDevConfig();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnUndoRedo));
			EditorApplication.editorApplicationQuit = (UnityAction)Delegate.Remove(EditorApplication.editorApplicationQuit, new UnityAction(this.OnQuit));
		}

		private void OnQuit()
		{
			this.SaveLookDevConfig();
		}

		private void RenderPreviewSingle()
		{
			int num = (this.m_LookDevConfig.lookDevMode != LookDevMode.Single1) ? 1 : 0;
			this.UpdateRenderTexture(this.m_PreviewRects[2]);
			this.RenderScene(this.m_PreviewRects[2], this.m_LookDevConfig.lookDevContexts[num], this.m_PreviewUtilityContexts[num], this.m_LookDevConfig.currentObject[num], this.m_LookDevConfig.cameraState[num], false);
			this.RenderCompositing(this.m_PreviewRects[2], this.m_PreviewUtilityContexts[num], this.m_PreviewUtilityContexts[num], false);
		}

		private void RenderPreviewSideBySide()
		{
			this.UpdateRenderTexture(this.m_PreviewRects[2]);
			this.RenderScene(this.m_PreviewRects[0], this.m_LookDevConfig.lookDevContexts[0], this.m_PreviewUtilityContexts[0], this.m_LookDevConfig.currentObject[0], this.m_LookDevConfig.cameraState[0], false);
			this.RenderScene(this.m_PreviewRects[1], this.m_LookDevConfig.lookDevContexts[1], this.m_PreviewUtilityContexts[1], this.m_LookDevConfig.currentObject[1], this.m_LookDevConfig.cameraState[1], true);
			this.RenderCompositing(this.m_PreviewRects[2], this.m_PreviewUtilityContexts[0], this.m_PreviewUtilityContexts[1], true);
		}

		private void RenderPreviewDualView()
		{
			this.UpdateRenderTexture(this.m_PreviewRects[2]);
			this.RenderScene(this.m_PreviewRects[2], this.m_LookDevConfig.lookDevContexts[0], this.m_PreviewUtilityContexts[0], this.m_LookDevConfig.currentObject[0], this.m_LookDevConfig.cameraState[0], false);
			this.RenderScene(this.m_PreviewRects[2], this.m_LookDevConfig.lookDevContexts[1], this.m_PreviewUtilityContexts[1], this.m_LookDevConfig.currentObject[1], this.m_LookDevConfig.cameraState[1], false);
			this.RenderCompositing(this.m_PreviewRects[2], this.m_PreviewUtilityContexts[0], this.m_PreviewUtilityContexts[1], true);
		}

		private void RenderCompositing(Rect previewRect, LookDevView.PreviewContext previewContext0, LookDevView.PreviewContext previewContext1, bool dualView)
		{
			Vector4 vector = new Vector4(this.m_LookDevConfig.gizmo.center.x, this.m_LookDevConfig.gizmo.center.y, 0f, 0f);
			Vector4 vector2 = new Vector4(this.m_LookDevConfig.gizmo.point2.x, this.m_LookDevConfig.gizmo.point2.y, 0f, 0f);
			Vector4 vector3 = new Vector4(this.m_GizmoThickness, this.m_GizmoThicknessSelected, 0f, 0f);
			Vector4 vector4 = new Vector4(this.m_GizmoCircleRadius, this.m_GizmoCircleRadiusSelected, 0f, 0f);
			int num = (this.m_LookDevConfig.lookDevMode != LookDevMode.Single2) ? 0 : 1;
			int num2 = (this.m_LookDevConfig.lookDevMode != LookDevMode.Single1) ? 1 : 0;
			float y = (this.m_LookDevConfig.lookDevContexts[num].shadingMode != -1 && this.m_LookDevConfig.lookDevContexts[num].shadingMode != 2) ? 0f : this.m_LookDevConfig.lookDevContexts[num].exposureValue;
			float z = (this.m_LookDevConfig.lookDevContexts[num2].shadingMode != -1 && this.m_LookDevConfig.lookDevContexts[num2].shadingMode != 2) ? 0f : this.m_LookDevConfig.lookDevContexts[num2].exposureValue;
			float x = (this.m_CurrentDragContext != LookDevEditionContext.Left) ? ((this.m_CurrentDragContext != LookDevEditionContext.Right) ? 0f : -1f) : 1f;
			CubemapInfo cubemapInfo = this.m_LookDevEnvLibrary.hdriList[this.m_LookDevConfig.lookDevContexts[num].currentHDRIIndex];
			CubemapInfo cubemapInfo2 = this.m_LookDevEnvLibrary.hdriList[this.m_LookDevConfig.lookDevContexts[num2].currentHDRIIndex];
			float shadowIntensity = cubemapInfo.shadowInfo.shadowIntensity;
			float shadowIntensity2 = cubemapInfo2.shadowInfo.shadowIntensity;
			Color shadowColor = cubemapInfo.shadowInfo.shadowColor;
			Color shadowColor2 = cubemapInfo2.shadowInfo.shadowColor;
			Texture texture = previewContext0.m_PreviewResult[0];
			Texture texture2 = previewContext0.m_PreviewResult[1];
			Texture texture3 = previewContext0.m_PreviewResult[2];
			Texture texture4 = previewContext1.m_PreviewResult[0];
			Texture texture5 = previewContext1.m_PreviewResult[1];
			Texture texture6 = previewContext1.m_PreviewResult[2];
			Vector4 vector5 = new Vector4(this.m_LookDevConfig.dualViewBlendFactor, y, z, (this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? -1f : 1f);
			Vector4 vector6 = new Vector4(x, (!this.m_LookDevConfig.enableToneMap) ? -1f : 1f, shadowIntensity, shadowIntensity2);
			Vector4 vector7 = new Vector4(1.4f, 1f, 0.5f, 0.5f);
			Vector4 vector8 = new Vector4(0f, 0f, 5.3f, 1f);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = this.m_FinalCompositionTexture;
			LookDevResources.m_LookDevCompositing.SetTexture("_Tex0Normal", texture);
			LookDevResources.m_LookDevCompositing.SetTexture("_Tex0WithoutSun", texture2);
			LookDevResources.m_LookDevCompositing.SetTexture("_Tex0Shadows", texture3);
			LookDevResources.m_LookDevCompositing.SetColor("_ShadowColor0", shadowColor);
			LookDevResources.m_LookDevCompositing.SetTexture("_Tex1Normal", texture4);
			LookDevResources.m_LookDevCompositing.SetTexture("_Tex1WithoutSun", texture5);
			LookDevResources.m_LookDevCompositing.SetTexture("_Tex1Shadows", texture6);
			LookDevResources.m_LookDevCompositing.SetColor("_ShadowColor1", shadowColor2);
			LookDevResources.m_LookDevCompositing.SetVector("_CompositingParams", vector5);
			LookDevResources.m_LookDevCompositing.SetVector("_CompositingParams2", vector6);
			LookDevResources.m_LookDevCompositing.SetColor("_FirstViewColor", LookDevView.m_FirstViewGizmoColor);
			LookDevResources.m_LookDevCompositing.SetColor("_SecondViewColor", LookDevView.m_SecondViewGizmoColor);
			LookDevResources.m_LookDevCompositing.SetVector("_GizmoPosition", vector);
			LookDevResources.m_LookDevCompositing.SetVector("_GizmoZoneCenter", vector2);
			LookDevResources.m_LookDevCompositing.SetVector("_GizmoSplitPlane", this.m_LookDevConfig.gizmo.plane);
			LookDevResources.m_LookDevCompositing.SetVector("_GizmoSplitPlaneOrtho", this.m_LookDevConfig.gizmo.planeOrtho);
			LookDevResources.m_LookDevCompositing.SetFloat("_GizmoLength", this.m_LookDevConfig.gizmo.length);
			LookDevResources.m_LookDevCompositing.SetVector("_GizmoThickness", vector3);
			LookDevResources.m_LookDevCompositing.SetVector("_GizmoCircleRadius", vector4);
			LookDevResources.m_LookDevCompositing.SetFloat("_BlendFactorCircleRadius", this.m_BlendFactorCircleRadius);
			LookDevResources.m_LookDevCompositing.SetFloat("_GetBlendFactorMaxGizmoDistance", this.GetBlendFactorMaxGizmoDistance());
			LookDevResources.m_LookDevCompositing.SetFloat("_GizmoRenderMode", (!this.m_ForceGizmoRenderSelector) ? ((float)this.m_GizmoRenderMode) : 4f);
			LookDevResources.m_LookDevCompositing.SetVector("_ScreenRatio", this.m_ScreenRatio);
			LookDevResources.m_LookDevCompositing.SetVector("_ToneMapCoeffs1", vector7);
			LookDevResources.m_LookDevCompositing.SetVector("_ToneMapCoeffs2", vector8);
			LookDevResources.m_LookDevCompositing.SetPass((int)this.m_LookDevConfig.lookDevMode);
			LookDevView.DrawFullScreenQuad(new Rect(0f, 0f, previewRect.width, previewRect.height));
			RenderTexture.active = active;
			GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			GUI.DrawTexture(previewRect, this.m_FinalCompositionTexture, ScaleMode.StretchToFill, false);
			GL.sRGBWrite = false;
		}

		private void RenderPreview()
		{
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_LookDevConfig.rotateObjectMode)
				{
					this.m_ObjRotationAcc = Math.Min(this.m_ObjRotationAcc + Time.deltaTime * 0.5f, 1f);
				}
				else
				{
					this.m_ObjRotationAcc = 0f;
				}
				if (this.m_LookDevConfig.rotateEnvMode)
				{
					this.m_EnvRotationAcc = Math.Min(this.m_EnvRotationAcc + Time.deltaTime * 0.5f, 1f);
				}
				else
				{
					this.m_EnvRotationAcc = 0f;
				}
				this.m_CurrentObjRotationOffset = (this.m_CurrentObjRotationOffset + Time.deltaTime * 360f * 0.3f * this.m_LookDevConfig.objRotationSpeed * this.m_ObjRotationAcc) % 360f;
				this.m_LookDevConfig.lookDevContexts[0].envRotation = (this.m_LookDevConfig.lookDevContexts[0].envRotation + Time.deltaTime * 360f * 0.03f * this.m_LookDevConfig.envRotationSpeed * this.m_EnvRotationAcc) % 720f;
				this.m_LookDevConfig.lookDevContexts[1].envRotation = (this.m_LookDevConfig.lookDevContexts[1].envRotation + Time.deltaTime * 360f * 0.03f * this.m_LookDevConfig.envRotationSpeed * this.m_EnvRotationAcc) % 720f;
				switch (this.m_LookDevConfig.lookDevMode)
				{
				case LookDevMode.Single1:
				case LookDevMode.Single2:
					this.RenderPreviewSingle();
					break;
				case LookDevMode.SideBySide:
					this.RenderPreviewSideBySide();
					break;
				case LookDevMode.Split:
				case LookDevMode.Zone:
					this.RenderPreviewDualView();
					break;
				}
			}
		}

		private void DoGizmoDebug()
		{
			if (this.m_DisplayDebugGizmo)
			{
				int num = 7;
				float num2 = 150f;
				float num3 = this.kLineHeight * (float)num;
				float width = 60f;
				float width2 = 90f;
				GUILayout.BeginArea(new Rect(base.position.width - num2 - 10f, base.position.height - num3 - 10f, num2, num3), LookDevView.styles.sBigTitleInnerStyle);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Thickness", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(width2)
				});
				this.m_GizmoThickness = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoThickness, new GUILayoutOption[]
				{
					GUILayout.Width(width)
				}), 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("ThicknessSelected", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(width2)
				});
				this.m_GizmoThicknessSelected = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoThicknessSelected, new GUILayoutOption[]
				{
					GUILayout.Width(width)
				}), 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Radius", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(width2)
				});
				this.m_GizmoCircleRadius = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoCircleRadius, new GUILayoutOption[]
				{
					GUILayout.Width(width)
				}), 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("RadiusSelected", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(width2)
				});
				this.m_GizmoCircleRadiusSelected = Mathf.Clamp(EditorGUILayout.FloatField(this.m_GizmoCircleRadiusSelected, new GUILayoutOption[]
				{
					GUILayout.Width(width)
				}), 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("BlendRadius", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(width2)
				});
				this.m_BlendFactorCircleRadius = Mathf.Clamp(EditorGUILayout.FloatField(this.m_BlendFactorCircleRadius, new GUILayoutOption[]
				{
					GUILayout.Width(width)
				}), 0f, 1f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Selected", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(width2)
				});
				this.m_ForceGizmoRenderSelector = GUILayout.Toggle(this.m_ForceGizmoRenderSelector, "", new GUILayoutOption[0]);
				GUILayout.EndHorizontal();
				if (GUILayout.Button("Reset Gizmo", new GUILayoutOption[0]))
				{
					this.m_LookDevConfig.gizmo.Update(new Vector2(0f, 0f), 0.2f, 0f);
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
			}
		}

		private void UpdateViewSpecific()
		{
			this.UpdatePreviewRects(this.m_DisplayRect);
			this.m_ScreenRatio.Set(this.m_PreviewRects[2].width / this.kReferenceScale, this.m_PreviewRects[2].height / this.kReferenceScale, this.m_PreviewRects[2].width, this.m_PreviewRects[2].height);
			int num = 4;
			float num2 = 292f;
			float num3 = this.kLineHeight * (float)num + EditorGUIUtility.standardVerticalSpacing;
			this.m_ControlWindowRect = new Rect(this.m_PreviewRects[2].width / 2f - num2 / 2f, this.m_PreviewRects[2].height - num3 - 10f, num2, num3);
		}

		private void UpdatePreviewRects(Rect previewRect)
		{
			this.m_PreviewRects[2] = new Rect(previewRect);
			if (this.m_ShowLookDevEnvWindow)
			{
				this.m_PreviewRects[2].width = this.m_PreviewRects[2].width - this.ComputeLookDevEnvWindowWidth();
			}
			this.m_PreviewRects[0] = new Rect(this.m_PreviewRects[2].x, this.m_PreviewRects[2].y, this.m_PreviewRects[2].width / 2f, this.m_PreviewRects[2].height);
			this.m_PreviewRects[1] = new Rect(this.m_PreviewRects[2].width / 2f, this.m_PreviewRects[2].y, this.m_PreviewRects[2].width / 2f, this.m_PreviewRects[2].height);
		}

		private void HandleCamera()
		{
			if (this.m_LookDevOperationType == LookDevOperationType.None && !this.m_ControlWindowRect.Contains(Event.current.mousePosition))
			{
				int currentEditionContextIndex = this.m_LookDevConfig.currentEditionContextIndex;
				int num = (currentEditionContextIndex + 1) % 2;
				this.m_CameraController.Update(this.m_LookDevConfig.cameraState[currentEditionContextIndex], this.m_PreviewUtilityContexts[this.m_LookDevConfig.currentEditionContextIndex].m_PreviewUtility[0].m_Camera);
				if ((this.m_LookDevConfig.lookDevMode == LookDevMode.Single1 || this.m_LookDevConfig.lookDevMode == LookDevMode.Single2 || this.m_LookDevConfig.lookDevMode == LookDevMode.SideBySide) && this.m_LookDevConfig.sideBySideCameraLinked)
				{
					this.m_LookDevConfig.cameraState[num].Copy(this.m_LookDevConfig.cameraState[currentEditionContextIndex]);
				}
				if (this.m_CameraController.currentViewTool == ViewTool.None)
				{
					if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.F)
					{
						if (!EditorGUIUtility.editingTextField)
						{
							this.Frame(this.m_LookDevConfig.currentEditionContext, true);
							Event.current.Use();
						}
					}
				}
				for (int i = 0; i < 3; i++)
				{
					this.m_LookDevConfig.cameraState[0].UpdateCamera(this.m_PreviewUtilityContexts[0].m_PreviewUtility[i].m_Camera);
					this.m_LookDevConfig.cameraState[1].UpdateCamera(this.m_PreviewUtilityContexts[1].m_PreviewUtility[i].m_Camera);
				}
			}
		}

		public void HandleKeyboardShortcut()
		{
			if (Event.current.type != EventType.Layout && !EditorGUIUtility.editingTextField)
			{
				if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.RightArrow)
				{
					this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, Math.Min(this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex + 1, this.m_LookDevEnvLibrary.hdriList.Count - 1));
					Event.current.Use();
				}
				else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.LeftArrow)
				{
					this.m_LookDevConfig.UpdateIntProperty(LookDevProperty.HDRI, Math.Max(this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex - 1, 0));
					Event.current.Use();
				}
				if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.R)
				{
					this.m_LookDevConfig.ResynchronizeObjects();
					Event.current.Use();
				}
			}
		}

		public void Frame()
		{
			this.Frame(true);
		}

		public void Frame(bool animate)
		{
			this.Frame(LookDevEditionContext.Left, animate);
			this.Frame(LookDevEditionContext.Right, animate);
		}

		private void Frame(LookDevEditionContext context, bool animate)
		{
			GameObject gameObject = this.m_LookDevConfig.currentObject[(int)context];
			if (gameObject != null)
			{
				Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);
				this.GetRenderableBoundsRecurse(ref bounds, gameObject);
				float num = bounds.extents.magnitude * 1.5f;
				if (num == 0f)
				{
					num = 10f;
				}
				CameraState cameraState = this.m_LookDevConfig.cameraState[(int)context];
				if (animate)
				{
					cameraState.pivot.target = bounds.center;
					cameraState.viewSize.target = Mathf.Abs(num * 2.2f);
				}
				else
				{
					cameraState.pivot.value = bounds.center;
					cameraState.viewSize.value = Mathf.Abs(num * 2.2f);
				}
			}
			this.m_CurrentObjRotationOffset = 0f;
		}

		private void HandleDragging()
		{
			Event current = Event.current;
			EventType type = current.type;
			switch (type)
			{
			case EventType.Repaint:
				return;
			case EventType.Layout:
				IL_26:
				if (type != EventType.DragExited)
				{
					return;
				}
				this.m_CurrentDragContext = LookDevEditionContext.None;
				return;
			case EventType.DragUpdated:
			{
				bool flag = false;
				UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
				for (int i = 0; i < objectReferences.Length; i++)
				{
					UnityEngine.Object @object = objectReferences[i];
					Cubemap exists = @object as Cubemap;
					if (exists)
					{
						flag = true;
					}
					Material material = @object as Material;
					if (material && material.shader.name.Contains("Skybox/Cubemap"))
					{
						flag = true;
					}
					GameObject gameObject = @object as GameObject;
					if (gameObject)
					{
						if (GameObjectInspector.HasRenderableParts(gameObject))
						{
							flag = true;
						}
					}
					LookDevEnvironmentLibrary exists2 = @object as LookDevEnvironmentLibrary;
					if (exists2)
					{
						flag = true;
					}
				}
				DragAndDrop.visualMode = ((!flag) ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Link);
				this.m_CurrentDragContext = this.GetEditionContext(Event.current.mousePosition);
				current.Use();
				return;
			}
			case EventType.DragPerform:
			{
				bool flag2 = false;
				if (this.m_PreviewRects[2].Contains(current.mousePosition))
				{
					UnityEngine.Object[] objectReferences2 = DragAndDrop.objectReferences;
					for (int j = 0; j < objectReferences2.Length; j++)
					{
						UnityEngine.Object object2 = objectReferences2[j];
						Cubemap cubemap = object2 as Cubemap;
						if (cubemap)
						{
							this.UpdateFocus(Event.current.mousePosition);
							this.UpdateContextWithCurrentHDRI(cubemap);
						}
						Material material2 = object2 as Material;
						if (material2 && material2.shader.name.Contains("Skybox/Cubemap"))
						{
							Cubemap cubemap2 = material2.GetTexture("_Tex") as Cubemap;
							if (cubemap2)
							{
								this.UpdateFocus(Event.current.mousePosition);
								this.UpdateContextWithCurrentHDRI(cubemap2);
							}
						}
						GameObject gameObject2 = object2 as GameObject;
						if (gameObject2)
						{
							if (!flag2 && GameObjectInspector.HasRenderableParts(gameObject2))
							{
								this.UpdateFocus(Event.current.mousePosition);
								Undo.RecordObject(this.m_LookDevConfig, "Set current preview object");
								bool flag3 = this.m_LookDevConfig.SetCurrentPreviewObject(gameObject2);
								this.Frame(this.m_LookDevConfig.currentEditionContext, false);
								if (flag3)
								{
									this.Frame((this.m_LookDevConfig.currentEditionContext != LookDevEditionContext.Left) ? LookDevEditionContext.Left : LookDevEditionContext.Right, false);
								}
								flag2 = true;
							}
						}
						LookDevEnvironmentLibrary lookDevEnvironmentLibrary = object2 as LookDevEnvironmentLibrary;
						if (lookDevEnvironmentLibrary)
						{
							this.envLibrary = lookDevEnvironmentLibrary;
						}
					}
				}
				DragAndDrop.AcceptDrag();
				this.m_CurrentDragContext = LookDevEditionContext.None;
				this.m_LookDevEnvWindow.CancelSelection();
				current.Use();
				return;
			}
			}
			goto IL_26;
		}

		private Vector2 GetNormalizedCoordinates(Vector2 mousePosition, Rect previewRect)
		{
			Vector2 result = new Vector3((mousePosition.x - previewRect.x) / previewRect.width, (mousePosition.y - previewRect.y) / previewRect.height);
			result.x = (result.x * 2f - 1f) * this.m_ScreenRatio.x;
			result.y = -(result.y * 2f - 1f) * this.m_ScreenRatio.y;
			return result;
		}

		private LookDevEditionContext GetEditionContext(Vector2 position)
		{
			LookDevEditionContext result;
			if (!this.m_PreviewRects[2].Contains(position))
			{
				result = LookDevEditionContext.None;
			}
			else
			{
				LookDevEditionContext lookDevEditionContext;
				switch (this.m_LookDevConfig.lookDevMode)
				{
				case LookDevMode.Single1:
					lookDevEditionContext = LookDevEditionContext.Left;
					break;
				case LookDevMode.Single2:
					lookDevEditionContext = LookDevEditionContext.Right;
					break;
				case LookDevMode.SideBySide:
					if (this.m_PreviewRects[0].Contains(position))
					{
						lookDevEditionContext = LookDevEditionContext.Left;
					}
					else
					{
						lookDevEditionContext = LookDevEditionContext.Right;
					}
					break;
				case LookDevMode.Split:
				{
					Vector2 normalizedCoordinates = this.GetNormalizedCoordinates(position, this.m_PreviewRects[2]);
					if (Vector3.Dot(new Vector3(normalizedCoordinates.x, normalizedCoordinates.y, 1f), this.m_LookDevConfig.gizmo.plane) > 0f)
					{
						lookDevEditionContext = LookDevEditionContext.Left;
					}
					else
					{
						lookDevEditionContext = LookDevEditionContext.Right;
					}
					break;
				}
				case LookDevMode.Zone:
				{
					Vector2 normalizedCoordinates2 = this.GetNormalizedCoordinates(position, this.m_PreviewRects[2]);
					if (Vector2.Distance(normalizedCoordinates2, this.m_LookDevConfig.gizmo.point2) - this.m_LookDevConfig.gizmo.length * 2f > 0f)
					{
						lookDevEditionContext = LookDevEditionContext.Left;
					}
					else
					{
						lookDevEditionContext = LookDevEditionContext.Right;
					}
					break;
				}
				default:
					lookDevEditionContext = LookDevEditionContext.Left;
					break;
				}
				result = lookDevEditionContext;
			}
			return result;
		}

		public void UpdateFocus(Vector2 position)
		{
			this.m_LookDevConfig.UpdateFocus(this.GetEditionContext(position));
		}

		private LookDevOperationType GetGizmoZoneOperation(Vector2 mousePosition, Rect previewRect)
		{
			Vector2 normalizedCoordinates = this.GetNormalizedCoordinates(mousePosition, previewRect);
			Vector3 lhs = new Vector3(normalizedCoordinates.x, normalizedCoordinates.y, 1f);
			float f = Vector3.Dot(lhs, this.m_LookDevConfig.gizmo.plane);
			float num = Mathf.Abs(f);
			float num2 = Vector2.Distance(normalizedCoordinates, this.m_LookDevConfig.gizmo.center);
			float num3 = Vector3.Dot(lhs, this.m_LookDevConfig.gizmo.planeOrtho);
			float num4 = (num3 <= 0f) ? -1f : 1f;
			Vector2 a = new Vector2(this.m_LookDevConfig.gizmo.planeOrtho.x, this.m_LookDevConfig.gizmo.planeOrtho.y);
			LookDevOperationType result = LookDevOperationType.None;
			if (num < this.m_GizmoCircleRadiusSelected && num2 < this.m_LookDevConfig.gizmo.length + this.m_GizmoCircleRadiusSelected)
			{
				if (num < this.m_GizmoThicknessSelected)
				{
					result = LookDevOperationType.GizmoTranslation;
				}
				Vector2 b = this.m_LookDevConfig.gizmo.center + num4 * a * this.m_LookDevConfig.gizmo.length;
				float num5 = Vector2.Distance(normalizedCoordinates, b);
				if (num5 <= this.m_GizmoCircleRadiusSelected)
				{
					result = ((num4 <= 0f) ? LookDevOperationType.GizmoRotationZone2 : LookDevOperationType.GizmoRotationZone1);
				}
				float blendFactorMaxGizmoDistance = this.GetBlendFactorMaxGizmoDistance();
				float num6 = this.GetBlendFactorMaxGizmoDistance() + this.m_BlendFactorCircleRadius - this.m_BlendFactorCircleSelectionRadius;
				float num7 = this.m_LookDevConfig.dualViewBlendFactor * this.GetBlendFactorMaxGizmoDistance();
				Vector2 b2 = this.m_LookDevConfig.gizmo.center - a * num7;
				float num8 = Mathf.Lerp(this.m_BlendFactorCircleRadius, this.m_BlendFactorCircleSelectionRadius, Mathf.Clamp((blendFactorMaxGizmoDistance - Mathf.Abs(num7)) / (blendFactorMaxGizmoDistance - num6), 0f, 1f));
				if ((normalizedCoordinates - b2).magnitude < num8)
				{
					result = LookDevOperationType.BlendFactor;
				}
			}
			return result;
		}

		private bool IsOperatingGizmo()
		{
			return this.m_LookDevOperationType == LookDevOperationType.BlendFactor || this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1 || this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone2 || this.m_LookDevOperationType == LookDevOperationType.GizmoTranslation;
		}

		private void HandleMouseInput()
		{
			Event current = Event.current;
			this.m_hotControlID = GUIUtility.GetControlID(FocusType.Passive);
			switch (current.GetTypeForControl(this.m_hotControlID))
			{
			case EventType.MouseDown:
				if ((this.m_LookDevConfig.lookDevMode == LookDevMode.Split || this.m_LookDevConfig.lookDevMode == LookDevMode.Zone) && current.button == 0)
				{
					this.m_LookDevOperationType = this.GetGizmoZoneOperation(Event.current.mousePosition, this.m_PreviewRects[2]);
					this.m_OnMouseDownOffsetToGizmo = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]) - this.m_LookDevConfig.gizmo.center;
				}
				if (this.m_LookDevOperationType == LookDevOperationType.None)
				{
					if (current.shift && current.button == 0)
					{
						this.m_LookDevOperationType = LookDevOperationType.RotateLight;
					}
					else if (current.control && current.button == 0)
					{
						this.m_LookDevOperationType = LookDevOperationType.RotateEnvironment;
					}
				}
				if (!this.IsOperatingGizmo() && !this.m_ControlWindowRect.Contains(Event.current.mousePosition))
				{
					this.UpdateFocus(Event.current.mousePosition);
				}
				GUIUtility.hotControl = this.m_hotControlID;
				break;
			case EventType.MouseUp:
				if (this.m_LookDevOperationType == LookDevOperationType.BlendFactor)
				{
					if (Mathf.Abs(this.m_LookDevConfig.dualViewBlendFactor) < this.m_GizmoCircleRadiusSelected / (this.m_LookDevConfig.gizmo.length - this.m_GizmoCircleRadius))
					{
						this.m_LookDevConfig.dualViewBlendFactor = 0f;
					}
				}
				this.m_LookDevOperationType = LookDevOperationType.None;
				if (this.m_LookDevEnvWindow != null)
				{
					Cubemap currentSelection = this.m_LookDevEnvWindow.GetCurrentSelection();
					if (currentSelection != null)
					{
						this.UpdateFocus(Event.current.mousePosition);
						this.UpdateContextWithCurrentHDRI(currentSelection);
						this.m_LookDevEnvWindow.CancelSelection();
						this.m_CurrentDragContext = LookDevEditionContext.None;
						base.Repaint();
					}
				}
				GUIUtility.hotControl = 0;
				break;
			case EventType.MouseMove:
				this.m_GizmoRenderMode = this.GetGizmoZoneOperation(Event.current.mousePosition, this.m_PreviewRects[2]);
				base.Repaint();
				break;
			case EventType.MouseDrag:
				if (this.m_LookDevOperationType == LookDevOperationType.RotateEnvironment)
				{
					float num = this.m_LookDevConfig.currentLookDevContext.envRotation;
					num = (num + current.delta.x / Mathf.Min(base.position.width, base.position.height) * 140f + 720f) % 720f;
					this.m_LookDevConfig.UpdateFloatProperty(LookDevProperty.EnvRotation, num);
					Event.current.Use();
				}
				else if (this.m_LookDevOperationType == LookDevOperationType.RotateLight && this.m_LookDevConfig.enableShadowCubemap)
				{
					ShadowInfo shadowInfo = this.m_LookDevEnvLibrary.hdriList[this.m_LookDevConfig.currentLookDevContext.currentHDRIIndex].shadowInfo;
					shadowInfo.latitude -= current.delta.y * 0.6f;
					shadowInfo.longitude -= current.delta.x * 0.6f;
					base.Repaint();
				}
				break;
			}
			if (Event.current.rawType == EventType.MouseUp)
			{
				if (this.m_LookDevEnvWindow.GetCurrentSelection() != null)
				{
					this.m_LookDevEnvWindow.CancelSelection();
				}
			}
			if (this.m_LookDevOperationType == LookDevOperationType.GizmoTranslation)
			{
				Vector2 center = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]) - this.m_OnMouseDownOffsetToGizmo;
				Vector2 normalizedCoordinates = this.GetNormalizedCoordinates(new Vector2(this.m_DisplayRect.x, this.m_PreviewRects[2].y + this.m_DisplayRect.height), this.m_PreviewRects[2]);
				Vector2 normalizedCoordinates2 = this.GetNormalizedCoordinates(new Vector2(this.m_DisplayRect.x + this.m_DisplayRect.width, this.m_PreviewRects[2].y), this.m_PreviewRects[2]);
				float num2 = 0.05f;
				center.x = Mathf.Clamp(center.x, normalizedCoordinates.x + num2, normalizedCoordinates2.x - num2);
				center.y = Mathf.Clamp(center.y, normalizedCoordinates.y + num2, normalizedCoordinates2.y - num2);
				this.m_LookDevConfig.gizmo.Update(center, this.m_LookDevConfig.gizmo.length, this.m_LookDevConfig.gizmo.angle);
				base.Repaint();
			}
			if (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1 || this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone2)
			{
				Vector2 normalizedCoordinates3 = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]);
				float num3 = 0.3926991f;
				Vector2 vector;
				Vector2 vector2;
				if (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1)
				{
					vector = normalizedCoordinates3;
					vector2 = this.m_LookDevConfig.gizmo.point2;
				}
				else
				{
					vector = normalizedCoordinates3;
					vector2 = this.m_LookDevConfig.gizmo.point1;
				}
				float magnitude = (vector2 - vector).magnitude;
				float num4 = Mathf.Min(base.position.width, base.position.height);
				float num5 = num4 / this.kReferenceScale * 2f * 0.9f;
				if (magnitude > num5)
				{
					Vector2 a = vector - vector2;
					a.Normalize();
					vector = vector2 + a * num5;
				}
				if (Event.current.shift)
				{
					Vector3 rhs = new Vector3(-1f, 0f, vector2.x);
					float num6 = Vector3.Dot(new Vector3(normalizedCoordinates3.x, normalizedCoordinates3.y, 1f), rhs);
					float num7 = 0.0174532924f * Vector2.Angle(new Vector2(0f, 1f), normalizedCoordinates3 - vector2);
					if (num6 > 0f)
					{
						num7 = 6.28318548f - num7;
					}
					num7 = (float)((int)(num7 / num3)) * num3;
					float magnitude2 = (normalizedCoordinates3 - vector2).magnitude;
					vector = vector2 + new Vector2(Mathf.Sin(num7), Mathf.Cos(num7)) * magnitude2;
				}
				if (this.m_LookDevOperationType == LookDevOperationType.GizmoRotationZone1)
				{
					this.m_LookDevConfig.gizmo.Update(vector, vector2);
				}
				else
				{
					this.m_LookDevConfig.gizmo.Update(vector2, vector);
				}
				base.Repaint();
			}
			if (this.m_LookDevOperationType == LookDevOperationType.BlendFactor)
			{
				Vector2 normalizedCoordinates4 = this.GetNormalizedCoordinates(Event.current.mousePosition, this.m_PreviewRects[2]);
				float value = -Vector3.Dot(new Vector3(normalizedCoordinates4.x, normalizedCoordinates4.y, 1f), this.m_LookDevConfig.gizmo.planeOrtho) / this.GetBlendFactorMaxGizmoDistance();
				this.m_LookDevConfig.dualViewBlendFactor = Mathf.Clamp(value, -1f, 1f);
				base.Repaint();
			}
		}

		private float GetBlendFactorMaxGizmoDistance()
		{
			return this.m_LookDevConfig.gizmo.length - this.m_GizmoCircleRadius - this.m_BlendFactorCircleRadius;
		}

		private void CleanupDeletedHDRI()
		{
			this.m_LookDevEnvLibrary.CleanupDeletedHDRI();
		}

		private void OnGUI()
		{
			if (Event.current.type != EventType.Repaint || this.m_CaptureRD)
			{
			}
			this.Initialize();
			this.CleanupDeletedHDRI();
			base.BeginWindows();
			this.m_DisplayRect = new Rect(0f, this.kLineHeight, base.position.width, base.position.height - this.kLineHeight);
			this.UpdateViewSpecific();
			this.DoToolbarGUI();
			this.HandleDragging();
			this.RenderPreview();
			this.DoControlWindow();
			this.DoAdditionalGUI();
			this.DoGizmoDebug();
			this.HandleMouseInput();
			this.HandleCamera();
			this.HandleKeyboardShortcut();
			if (this.m_LookDevConfig.currentObject[0] == null && this.m_LookDevConfig.currentObject[1] == null)
			{
				Color color = GUI.color;
				GUI.color = Color.gray;
				Vector2 vector = GUI.skin.label.CalcSize(LookDevView.styles.sDragAndDropObjsText);
				Rect position = new Rect(this.m_DisplayRect.width * 0.5f - vector.x * 0.5f, this.m_DisplayRect.height * 0.2f - vector.y * 0.5f, vector.x, vector.y);
				GUI.Label(position, LookDevView.styles.sDragAndDropObjsText);
				GUI.color = color;
			}
			base.EndWindows();
			if (Event.current.type == EventType.Repaint)
			{
				if (this.m_LookDevEnvWindow != null && this.m_LookDevEnvWindow.GetCurrentSelection() != null)
				{
					this.m_CurrentDragContext = this.GetEditionContext(Event.current.mousePosition);
					GUI.DrawTexture(new Rect(Event.current.mousePosition.x - this.m_LookDevEnvWindow.GetSelectedPositionOffset().x, Event.current.mousePosition.y - this.m_LookDevEnvWindow.GetSelectedPositionOffset().y, 250f, 125f), LookDevResources.m_SelectionTexture, ScaleMode.ScaleToFit, true);
				}
				else
				{
					this.m_CurrentDragContext = LookDevEditionContext.None;
				}
			}
			if (Event.current.type == EventType.Repaint && this.m_CaptureRD)
			{
				this.m_CaptureRD = false;
			}
		}

		private void GetShaderConstantsFromNormalizedSH(SphericalHarmonicsL2 ambientProbe, Vector4[] outCoefficients)
		{
			for (int i = 0; i < 3; i++)
			{
				outCoefficients[i].x = ambientProbe[i, 3];
				outCoefficients[i].y = ambientProbe[i, 1];
				outCoefficients[i].z = ambientProbe[i, 2];
				outCoefficients[i].w = ambientProbe[i, 0] - ambientProbe[i, 6];
				outCoefficients[i + 3].x = ambientProbe[i, 4];
				outCoefficients[i + 3].y = ambientProbe[i, 5];
				outCoefficients[i + 3].z = ambientProbe[i, 6] * 3f;
				outCoefficients[i + 3].w = ambientProbe[i, 7];
			}
			outCoefficients[6].x = ambientProbe[0, 8];
			outCoefficients[6].y = ambientProbe[1, 8];
			outCoefficients[6].z = ambientProbe[2, 8];
			outCoefficients[6].w = 1f;
		}
	}
}
