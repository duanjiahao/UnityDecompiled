using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class AvatarPreview
	{
		private class Styles
		{
			public GUIContent speedScale = EditorGUIUtility.IconContent("SpeedScale", "Changes animation preview speed");
			public GUIContent pivot = EditorGUIUtility.IconContent("AvatarPivot", "Displays avatar's pivot and mass center");
			public GUIContent ik = new GUIContent("IK", "Activates feet IK preview");
			public GUIContent avatarIcon = EditorGUIUtility.IconContent("Avatar Icon", "Changes the model to use for previewing.");
			public GUIStyle preButton = "preButton";
			public GUIStyle preSlider = "preSlider";
			public GUIStyle preSliderThumb = "preSliderThumb";
			public GUIStyle preLabel = "preLabel";
		}
		private enum PreviewPopupOptions
		{
			Auto,
			DefaultModel,
			Other
		}
		public delegate void OnAvatarChange();
		private const string kIkPref = "AvatarpreviewShowIK";
		private const string kReferencePref = "AvatarpreviewShowReference";
		private const string kSpeedPref = "AvatarpreviewSpeed";
		private const float kTimeControlRectHeight = 21f;
		private const int kPreviewLayer = 31;
		private const string s_PreviewStr = "Preview";
		private const float kFloorFadeDuration = 0.2f;
		private const float kFloorScale = 5f;
		private const float kFloorScaleSmall = 0.2f;
		private const float kFloorTextureScale = 4f;
		private const float kFloorAlpha = 0.5f;
		private const float kFloorShadowAlpha = 0.3f;
		private AvatarPreview.OnAvatarChange m_OnAvatarChangeFunc;
		public TimeControl timeControl;
		public int fps = 60;
		private Material m_FloorMaterial;
		private Material m_FloorMaterialSmall;
		private Material m_ShadowMaskMaterial;
		private Material m_ShadowPlaneMaterial;
		private PreviewRenderUtility m_PreviewUtility;
		private GameObject m_PreviewInstance;
		private GameObject m_ReferenceInstance;
		private GameObject m_DirectionInstance;
		private GameObject m_PivotInstance;
		private GameObject m_RootInstance;
		private float m_BoundingVolumeScale;
		private Motion m_SourcePreviewMotion;
		private Animator m_SourceScenePreviewAnimator;
		private int m_PreviewHint = "Preview".GetHashCode();
		private Texture2D m_FloorTexture;
		private Mesh m_FloorPlane;
		private bool m_ShowReference;
		private bool m_IKOnFeet;
		private bool m_ShowIKOnFeetButton = true;
		private bool m_IsValid;
		private int m_ModelSelectorId = GUIUtility.GetPermanentControlID();
		private float m_PrevFloorHeight;
		private float m_NextFloorHeight;
		private Vector2 m_PreviewDir = new Vector2(120f, -20f);
		private static AvatarPreview.Styles s_Styles;
		private float m_LastNormalizedTime = -1000f;
		private float m_LastStartTime = -1000f;
		private float m_LastStopTime = -1000f;
		private bool m_NextTargetIsForward = true;
		public AvatarPreview.OnAvatarChange OnAvatarChangeFunc
		{
			set
			{
				this.m_OnAvatarChangeFunc = value;
			}
		}
		public bool IKOnFeet
		{
			get
			{
				return this.m_IKOnFeet;
			}
		}
		public bool ShowIKOnFeetButton
		{
			get
			{
				return this.m_ShowIKOnFeetButton;
			}
			set
			{
				this.m_ShowIKOnFeetButton = value;
			}
		}
		public Animator Animator
		{
			get
			{
				return (!(this.m_PreviewInstance != null)) ? null : (this.m_PreviewInstance.GetComponent(typeof(Animator)) as Animator);
			}
		}
		public GameObject PreviewObject
		{
			get
			{
				return this.m_PreviewInstance;
			}
		}
		public ModelImporterAnimationType animationClipType
		{
			get
			{
				return AvatarPreview.GetAnimationType(this.m_SourcePreviewMotion);
			}
		}
		public AvatarPreview(Animator previewObjectInScene, Motion objectOnSameAsset)
		{
			this.InitInstance(previewObjectInScene, objectOnSameAsset);
		}
		private void SetPreviewCharacterEnabled(bool enabled, bool showReference)
		{
			if (this.m_PreviewInstance != null)
			{
				GameObjectInspector.SetEnabledRecursive(this.m_PreviewInstance, enabled);
			}
			GameObjectInspector.SetEnabledRecursive(this.m_ReferenceInstance, showReference && enabled);
			GameObjectInspector.SetEnabledRecursive(this.m_DirectionInstance, showReference && enabled);
			GameObjectInspector.SetEnabledRecursive(this.m_PivotInstance, showReference && enabled);
			GameObjectInspector.SetEnabledRecursive(this.m_RootInstance, showReference && enabled);
		}
		private static AnimationClip GetFirstAnimationClipFromMotion(Motion motion)
		{
			AnimationClip animationClip = motion as AnimationClip;
			if (animationClip)
			{
				return animationClip;
			}
			BlendTree blendTree = motion as BlendTree;
			if (blendTree)
			{
				AnimationClip[] animationClipsFlattened = blendTree.GetAnimationClipsFlattened();
				if (animationClipsFlattened.Length > 0)
				{
					return animationClipsFlattened[0];
				}
			}
			return null;
		}
		public static ModelImporterAnimationType GetAnimationType(GameObject go)
		{
			Animator component = go.GetComponent<Animator>();
			if (component)
			{
				Avatar avatar = (!(component != null)) ? null : component.avatar;
				if (avatar && avatar.isHuman)
				{
					return ModelImporterAnimationType.Human;
				}
				return ModelImporterAnimationType.Generic;
			}
			else
			{
				if (go.GetComponent<Animation>() != null)
				{
					return ModelImporterAnimationType.Legacy;
				}
				return ModelImporterAnimationType.None;
			}
		}
		public static ModelImporterAnimationType GetAnimationType(Motion motion)
		{
			AnimationClip firstAnimationClipFromMotion = AvatarPreview.GetFirstAnimationClipFromMotion(motion);
			if (firstAnimationClipFromMotion)
			{
				return AnimationUtility.GetAnimationType(firstAnimationClipFromMotion);
			}
			return ModelImporterAnimationType.None;
		}
		public static bool IsValidPreviewGameObject(GameObject target, ModelImporterAnimationType requiredClipType)
		{
			if (target != null && !target.activeSelf)
			{
				Debug.LogWarning("Can't preview inactive object, using fallback object");
			}
			return target != null && target.activeSelf && GameObjectInspector.HasRenderablePartsRecurse(target) && AvatarPreview.GetAnimationType(target) == requiredClipType;
		}
		public static GameObject FindBestFittingRenderableGameObjectFromModelAsset(UnityEngine.Object asset, ModelImporterAnimationType animationType)
		{
			if (asset == null)
			{
				return null;
			}
			ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset)) as ModelImporter;
			if (modelImporter == null)
			{
				return null;
			}
			string assetPath = modelImporter.CalculateBestFittingPreviewGameObject();
			GameObject gameObject = AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;
			if (AvatarPreview.IsValidPreviewGameObject(gameObject, animationType))
			{
				return gameObject;
			}
			return null;
		}
		private static GameObject CalculatePreviewGameObject(Animator selectedAnimator, Motion motion, ModelImporterAnimationType animationType)
		{
			AnimationClip firstAnimationClipFromMotion = AvatarPreview.GetFirstAnimationClipFromMotion(motion);
			GameObject gameObject = AvatarPreviewSelection.GetPreview(animationType);
			if (AvatarPreview.IsValidPreviewGameObject(gameObject, animationType))
			{
				return gameObject;
			}
			if (selectedAnimator != null && AvatarPreview.IsValidPreviewGameObject(selectedAnimator.gameObject, animationType))
			{
				return selectedAnimator.gameObject;
			}
			gameObject = AvatarPreview.FindBestFittingRenderableGameObjectFromModelAsset(firstAnimationClipFromMotion, animationType);
			if (gameObject != null)
			{
				return gameObject;
			}
			if (animationType == ModelImporterAnimationType.Human)
			{
				return AvatarPreview.GetHumanoidFallback();
			}
			if (animationType == ModelImporterAnimationType.Generic)
			{
				return AvatarPreview.GetGenericAnimationFallback();
			}
			return null;
		}
		private static GameObject GetGenericAnimationFallback()
		{
			return (GameObject)EditorGUIUtility.Load("Avatar/DefaultGeneric.fbx");
		}
		private static GameObject GetHumanoidFallback()
		{
			return (GameObject)EditorGUIUtility.Load("Avatar/DefaultAvatar.fbx");
		}
		private void InitInstance(Animator scenePreviewObject, Motion motion)
		{
			this.m_SourcePreviewMotion = motion;
			this.m_SourceScenePreviewAnimator = scenePreviewObject;
			if (this.m_PreviewInstance == null)
			{
				GameObject gameObject = AvatarPreview.CalculatePreviewGameObject(scenePreviewObject, motion, this.animationClipType);
				this.m_IsValid = (gameObject != null && gameObject != AvatarPreview.GetGenericAnimationFallback());
				if (gameObject != null)
				{
					this.m_PreviewInstance = (GameObject)EditorUtility.InstantiateRemoveAllNonAnimationComponents(gameObject, Vector3.zero, Quaternion.identity);
					Bounds bounds = new Bounds(this.m_PreviewInstance.transform.position, Vector3.zero);
					GameObjectInspector.GetRenderableBoundsRecurse(ref bounds, this.m_PreviewInstance);
					this.m_BoundingVolumeScale = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
					GameObjectInspector.InitInstantiatedPreviewRecursive(this.m_PreviewInstance);
					if (this.Animator)
					{
						this.Animator.enabled = false;
						this.Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
						this.Animator.applyRootMotion = true;
						this.Animator.logWarnings = false;
						this.Animator.fireEvents = false;
					}
				}
			}
			if (this.timeControl == null)
			{
				this.timeControl = new TimeControl();
			}
			if (this.m_ReferenceInstance == null)
			{
				GameObject original = (GameObject)EditorGUIUtility.Load("Avatar/dial_flat.prefab");
				this.m_ReferenceInstance = (GameObject)UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
				GameObjectInspector.InitInstantiatedPreviewRecursive(this.m_ReferenceInstance);
			}
			if (this.m_DirectionInstance == null)
			{
				GameObject original2 = (GameObject)EditorGUIUtility.Load("Avatar/arrow.fbx");
				this.m_DirectionInstance = (GameObject)UnityEngine.Object.Instantiate(original2, Vector3.zero, Quaternion.identity);
				GameObjectInspector.InitInstantiatedPreviewRecursive(this.m_DirectionInstance);
			}
			if (this.m_PivotInstance == null)
			{
				GameObject original3 = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
				this.m_PivotInstance = (GameObject)UnityEngine.Object.Instantiate(original3, Vector3.zero, Quaternion.identity);
				GameObjectInspector.InitInstantiatedPreviewRecursive(this.m_PivotInstance);
			}
			if (this.m_RootInstance == null)
			{
				GameObject original4 = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
				this.m_RootInstance = (GameObject)UnityEngine.Object.Instantiate(original4, Vector3.zero, Quaternion.identity);
				GameObjectInspector.InitInstantiatedPreviewRecursive(this.m_RootInstance);
			}
			this.m_IKOnFeet = EditorPrefs.GetBool("AvatarpreviewShowIK", false);
			this.m_ShowReference = EditorPrefs.GetBool("AvatarpreviewShowReference", true);
			this.timeControl.playbackSpeed = EditorPrefs.GetFloat("AvatarpreviewSpeed", 1f);
			this.SetPreviewCharacterEnabled(false, false);
		}
		private void Init()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility(true);
				this.m_PreviewUtility.m_CameraFieldOfView = 30f;
				this.m_PreviewUtility.m_Camera.cullingMask = -2147483648;
			}
			if (AvatarPreview.s_Styles == null)
			{
				AvatarPreview.s_Styles = new AvatarPreview.Styles();
			}
			if (this.m_FloorPlane == null)
			{
				this.m_FloorPlane = (Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh);
			}
			if (this.m_FloorTexture == null)
			{
				this.m_FloorTexture = (Texture2D)EditorGUIUtility.Load("Avatar/Textures/AvatarFloor.png");
			}
			if (this.m_FloorMaterial == null)
			{
				Shader shader = EditorGUIUtility.LoadRequired("Previews/PreviewPlaneWithShadow.shader") as Shader;
				this.m_FloorMaterial = new Material(shader);
				this.m_FloorMaterial.mainTexture = this.m_FloorTexture;
				this.m_FloorMaterial.mainTextureScale = Vector2.one * 5f * 4f;
				this.m_FloorMaterial.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0f, 0f));
				this.m_FloorMaterial.hideFlags = HideFlags.DontSave;
				this.m_FloorMaterialSmall = new Material(this.m_FloorMaterial);
				this.m_FloorMaterialSmall.mainTextureScale = Vector2.one * 0.2f * 4f;
				this.m_FloorMaterialSmall.hideFlags = HideFlags.DontSave;
			}
			if (this.m_ShadowMaskMaterial == null)
			{
				Shader shader2 = EditorGUIUtility.LoadRequired("Previews/PreviewShadowMask.shader") as Shader;
				this.m_ShadowMaskMaterial = new Material(shader2);
				this.m_ShadowMaskMaterial.hideFlags = HideFlags.DontSave;
			}
			if (this.m_ShadowPlaneMaterial == null)
			{
				Shader shader3 = EditorGUIUtility.LoadRequired("Previews/PreviewShadowPlaneClip.shader") as Shader;
				this.m_ShadowPlaneMaterial = new Material(shader3);
				this.m_ShadowPlaneMaterial.hideFlags = HideFlags.DontSave;
			}
		}
		public void OnDestroy()
		{
			if (this.m_PreviewUtility != null)
			{
				this.m_PreviewUtility.Cleanup();
				this.m_PreviewUtility = null;
			}
			UnityEngine.Object.DestroyImmediate(this.m_PreviewInstance);
			UnityEngine.Object.DestroyImmediate(this.m_FloorMaterial);
			UnityEngine.Object.DestroyImmediate(this.m_FloorMaterialSmall);
			UnityEngine.Object.DestroyImmediate(this.m_ShadowMaskMaterial);
			UnityEngine.Object.DestroyImmediate(this.m_ShadowPlaneMaterial);
			UnityEngine.Object.DestroyImmediate(this.m_ReferenceInstance);
			UnityEngine.Object.DestroyImmediate(this.m_RootInstance);
			UnityEngine.Object.DestroyImmediate(this.m_PivotInstance);
			UnityEngine.Object.DestroyImmediate(this.m_DirectionInstance);
			if (this.timeControl != null)
			{
				this.timeControl.OnDisable();
			}
		}
		public void DoSelectionChange()
		{
			this.m_OnAvatarChangeFunc();
		}
		private float PreviewSlider(float val, float snapThreshold)
		{
			val = GUILayout.HorizontalSlider(val, 0.1f, 2f, AvatarPreview.s_Styles.preSlider, AvatarPreview.s_Styles.preSliderThumb, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(64f)
			});
			if (val > 0.25f - snapThreshold && val < 0.25f + snapThreshold)
			{
				val = 0.25f;
			}
			else
			{
				if (val > 0.5f - snapThreshold && val < 0.5f + snapThreshold)
				{
					val = 0.5f;
				}
				else
				{
					if (val > 0.75f - snapThreshold && val < 0.75f + snapThreshold)
					{
						val = 0.75f;
					}
					else
					{
						if (val > 1f - snapThreshold && val < 1f + snapThreshold)
						{
							val = 1f;
						}
						else
						{
							if (val > 1.25f - snapThreshold && val < 1.25f + snapThreshold)
							{
								val = 1.25f;
							}
							else
							{
								if (val > 1.5f - snapThreshold && val < 1.5f + snapThreshold)
								{
									val = 1.5f;
								}
								else
								{
									if (val > 1.75f - snapThreshold && val < 1.75f + snapThreshold)
									{
										val = 1.75f;
									}
								}
							}
						}
					}
				}
			}
			return val;
		}
		public void DoPreviewSettings()
		{
			this.Init();
			if (this.m_ShowIKOnFeetButton)
			{
				EditorGUI.BeginChangeCheck();
				this.m_IKOnFeet = GUILayout.Toggle(this.m_IKOnFeet, AvatarPreview.s_Styles.ik, AvatarPreview.s_Styles.preButton, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					EditorPrefs.SetBool("AvatarpreviewShowIK", this.m_IKOnFeet);
				}
			}
			EditorGUI.BeginChangeCheck();
			this.m_ShowReference = GUILayout.Toggle(this.m_ShowReference, AvatarPreview.s_Styles.pivot, AvatarPreview.s_Styles.preButton, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("AvatarpreviewShowReference", this.m_ShowReference);
			}
			GUILayout.Box(AvatarPreview.s_Styles.speedScale, AvatarPreview.s_Styles.preLabel, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			this.timeControl.playbackSpeed = this.PreviewSlider(this.timeControl.playbackSpeed, 0.03f);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetFloat("AvatarpreviewSpeed", this.timeControl.playbackSpeed);
			}
			GUILayout.Label(this.timeControl.playbackSpeed.ToString("f2"), AvatarPreview.s_Styles.preLabel, new GUILayoutOption[0]);
		}
		private RenderTexture RenderPreviewShadowmap(Light light, float scale, Vector3 center, Vector3 floorPos, out Matrix4x4 outShadowMatrix)
		{
			Camera camera = this.m_PreviewUtility.m_Camera;
			camera.orthographic = true;
			camera.orthographicSize = scale * 2f;
			camera.nearClipPlane = 1f * scale;
			camera.farClipPlane = 25f * scale;
			camera.transform.rotation = light.transform.rotation;
			camera.transform.position = center - light.transform.forward * (scale * 5.5f);
			CameraClearFlags clearFlags = camera.clearFlags;
			camera.clearFlags = CameraClearFlags.Color;
			Color backgroundColor = camera.backgroundColor;
			camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			RenderTexture targetTexture = camera.targetTexture;
			RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 16);
			temporary.isPowerOfTwo = true;
			temporary.wrapMode = TextureWrapMode.Clamp;
			temporary.filterMode = FilterMode.Bilinear;
			camera.targetTexture = temporary;
			this.SetPreviewCharacterEnabled(true, false);
			this.m_PreviewUtility.m_Camera.Render();
			RenderTexture.active = temporary;
			GL.PushMatrix();
			GL.LoadOrtho();
			this.m_ShadowMaskMaterial.SetPass(0);
			GL.Begin(7);
			GL.Vertex3(0f, 0f, -99f);
			GL.Vertex3(1f, 0f, -99f);
			GL.Vertex3(1f, 1f, -99f);
			GL.Vertex3(0f, 1f, -99f);
			GL.End();
			GL.LoadProjectionMatrix(camera.projectionMatrix);
			GL.LoadIdentity();
			GL.MultMatrix(camera.worldToCameraMatrix);
			this.m_ShadowPlaneMaterial.SetPass(0);
			GL.Begin(7);
			float num = 5f * scale;
			GL.Vertex(floorPos + new Vector3(-num, 0f, -num));
			GL.Vertex(floorPos + new Vector3(num, 0f, -num));
			GL.Vertex(floorPos + new Vector3(num, 0f, num));
			GL.Vertex(floorPos + new Vector3(-num, 0f, num));
			GL.End();
			GL.PopMatrix();
			Matrix4x4 lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
			outShadowMatrix = lhs * camera.projectionMatrix * camera.worldToCameraMatrix;
			camera.orthographic = false;
			camera.clearFlags = clearFlags;
			camera.backgroundColor = backgroundColor;
			camera.targetTexture = targetTexture;
			return temporary;
		}
		public Texture DoRenderPreview(Rect previewRect, GUIStyle background)
		{
			this.m_PreviewUtility.BeginPreview(previewRect, background);
			Quaternion quaternion;
			Vector3 vector;
			Quaternion quaternion2;
			Vector3 vector2;
			Vector3 pivotPos;
			float num;
			if (this.Animator && this.Animator.isHuman)
			{
				quaternion = this.Animator.rootRotation;
				vector = this.Animator.rootPosition;
				quaternion2 = this.Animator.bodyRotation;
				vector2 = this.Animator.bodyPosition;
				pivotPos = this.Animator.pivotPosition;
				num = this.Animator.humanScale;
			}
			else
			{
				if (this.Animator && this.Animator.hasRootMotion)
				{
					quaternion = this.Animator.rootRotation;
					vector = this.Animator.rootPosition;
					quaternion2 = Quaternion.identity;
					vector2 = GameObjectInspector.GetRenderableCenterRecurse(this.m_PreviewInstance, 2, 8);
					pivotPos = Vector3.zero;
					num = this.m_BoundingVolumeScale / 2f;
				}
				else
				{
					quaternion = Quaternion.identity;
					vector = Vector3.zero;
					quaternion2 = Quaternion.identity;
					vector2 = GameObjectInspector.GetRenderableCenterRecurse(this.m_PreviewInstance, 2, 8);
					pivotPos = Vector3.zero;
					num = this.m_BoundingVolumeScale / 2f;
				}
			}
			bool oldFog = this.SetupPreviewLightingAndFx();
			Vector3 forward = quaternion2 * Vector3.forward;
			forward[1] = 0f;
			Quaternion directionRot = Quaternion.LookRotation(forward);
			Vector3 directionPos = vector;
			Quaternion pivotRot = quaternion;
			this.PositionPreviewObjects(pivotRot, pivotPos, quaternion2, vector2, directionRot, quaternion, vector, directionPos, num);
			bool flag = Mathf.Abs(this.m_NextFloorHeight - this.m_PrevFloorHeight) > num * 0.01f;
			float num3;
			float num4;
			if (flag)
			{
				float num2 = (this.m_NextFloorHeight >= this.m_PrevFloorHeight) ? 0.8f : 0.2f;
				num3 = ((this.timeControl.normalizedTime >= num2) ? this.m_NextFloorHeight : this.m_PrevFloorHeight);
				num4 = Mathf.Clamp01(Mathf.Abs(this.timeControl.normalizedTime - num2) / 0.2f);
			}
			else
			{
				num3 = this.m_PrevFloorHeight;
				num4 = 1f;
			}
			Quaternion identity = Quaternion.identity;
			Vector3 position = new Vector3(0f, 0f, 0f);
			position = this.m_ReferenceInstance.transform.position;
			position.y = num3;
			Matrix4x4 matrix;
			RenderTexture renderTexture = this.RenderPreviewShadowmap(this.m_PreviewUtility.m_Light[0], num, vector2, position, out matrix);
			this.m_PreviewUtility.m_Camera.nearClipPlane = 1f * num;
			this.m_PreviewUtility.m_Camera.farClipPlane = 25f * num;
			Quaternion rotation = Quaternion.Euler(-this.m_PreviewDir.y, -this.m_PreviewDir.x, 0f);
			Vector3 position2 = rotation * (Vector3.forward * -5.5f * num) + vector2;
			this.m_PreviewUtility.m_Camera.transform.position = position2;
			this.m_PreviewUtility.m_Camera.transform.rotation = rotation;
			position.y = num3;
			Material floorMaterial = this.m_FloorMaterial;
			floorMaterial.mainTextureOffset = -floorMaterial.mainTextureScale * 0.5f - new Vector2(position.x, position.z) / num * 0.1f * 4f;
			floorMaterial.SetTexture("_ShadowTexture", renderTexture);
			floorMaterial.SetMatrix("_ShadowTextureMatrix", matrix);
			floorMaterial.SetVector("_Alphas", new Vector4(0.5f * num4, 0.3f * num4, 0f, 0f));
			Matrix4x4 matrix2 = Matrix4x4.TRS(position, identity, Vector3.one * num * 5f);
			Graphics.DrawMesh(this.m_FloorPlane, matrix2, floorMaterial, 31, this.m_PreviewUtility.m_Camera, 0);
			if (flag)
			{
				bool flag2 = this.m_NextFloorHeight > this.m_PrevFloorHeight;
				float num5 = (!flag2) ? this.m_PrevFloorHeight : this.m_NextFloorHeight;
				float from = (!flag2) ? this.m_NextFloorHeight : this.m_PrevFloorHeight;
				float num6 = ((num5 != num3) ? 1f : (1f - num4)) * Mathf.InverseLerp(from, num5, vector.y);
				position.y = num5;
				Material floorMaterialSmall = this.m_FloorMaterialSmall;
				floorMaterialSmall.mainTextureOffset = -floorMaterialSmall.mainTextureScale * 0.5f - new Vector2(position.x, position.z) / num * 0.1f * 4f;
				floorMaterialSmall.SetTexture("_ShadowTexture", renderTexture);
				floorMaterialSmall.SetMatrix("_ShadowTextureMatrix", matrix);
				floorMaterialSmall.SetVector("_Alphas", new Vector4(0.5f * num6, 0f, 0f, 0f));
				Matrix4x4 matrix3 = Matrix4x4.TRS(position, identity, Vector3.one * num * 0.2f);
				Graphics.DrawMesh(this.m_FloorPlane, matrix3, floorMaterialSmall, 31, this.m_PreviewUtility.m_Camera, 0);
			}
			this.SetPreviewCharacterEnabled(true, this.m_ShowReference);
			this.m_PreviewUtility.m_Camera.Render();
			this.SetPreviewCharacterEnabled(false, false);
			AvatarPreview.TeardownPreviewLightingAndFx(oldFog);
			RenderTexture.ReleaseTemporary(renderTexture);
			return this.m_PreviewUtility.EndPreview();
		}
		private bool SetupPreviewLightingAndFx()
		{
			this.m_PreviewUtility.m_Light[0].intensity = 0.7f;
			this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);
			this.m_PreviewUtility.m_Light[1].intensity = 0.7f;
			Color ambient = new Color(0.1f, 0.1f, 0.1f, 0f);
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			return fog;
		}
		private static void TeardownPreviewLightingAndFx(bool oldFog)
		{
			Unsupported.SetRenderSettingsUseFogNoDirty(oldFog);
			InternalEditorUtility.RemoveCustomLighting();
		}
		private void PositionPreviewObjects(Quaternion pivotRot, Vector3 pivotPos, Quaternion bodyRot, Vector3 bodyPos, Quaternion directionRot, Quaternion rootRot, Vector3 rootPos, Vector3 directionPos, float scale)
		{
			this.m_ReferenceInstance.transform.position = rootPos;
			this.m_ReferenceInstance.transform.rotation = rootRot;
			this.m_ReferenceInstance.transform.localScale = Vector3.one * scale * 1.25f;
			this.m_DirectionInstance.transform.position = directionPos;
			this.m_DirectionInstance.transform.rotation = directionRot;
			this.m_DirectionInstance.transform.localScale = Vector3.one * scale * 2f;
			this.m_PivotInstance.transform.position = pivotPos;
			this.m_PivotInstance.transform.rotation = pivotRot;
			this.m_PivotInstance.transform.localScale = Vector3.one * scale * 0.1f;
			this.m_RootInstance.transform.position = bodyPos;
			this.m_RootInstance.transform.rotation = bodyRot;
			this.m_RootInstance.transform.localScale = Vector3.one * scale * 0.25f;
			if (this.Animator)
			{
				float normalizedTime = this.timeControl.normalizedTime;
				float num = this.timeControl.deltaTime / (this.timeControl.stopTime - this.timeControl.startTime);
				if (normalizedTime - num < 0f || normalizedTime - num >= 1f)
				{
					this.m_PrevFloorHeight = this.m_NextFloorHeight;
				}
				if (this.m_LastNormalizedTime != -1000f && this.timeControl.startTime == this.m_LastStartTime && this.timeControl.stopTime == this.m_LastStopTime)
				{
					float num2 = normalizedTime - num - this.m_LastNormalizedTime;
					if (num2 > 0.5f)
					{
						num2 -= 1f;
					}
					else
					{
						if (num2 < -0.5f)
						{
							num2 += 1f;
						}
					}
					if (Mathf.Abs(num2) > Mathf.Max(0.0001f, Mathf.Abs(num * 0.1f)))
					{
						Debug.LogError(string.Concat(new object[]
						{
							"Reported delta does not match with progression of time. Check that the OnInteractivePreviewGUI function only updates the Animator on Repaint events.\nlastNormalTime: ",
							this.m_LastNormalizedTime,
							", normalizedDelta: ",
							num,
							", expectedNormalizedTime: ",
							Mathf.Repeat(this.m_LastNormalizedTime + num, 1f),
							", normalizedTime: ",
							normalizedTime,
							", difference: ",
							num2
						}));
					}
				}
				this.m_LastNormalizedTime = normalizedTime;
				this.m_LastStartTime = this.timeControl.startTime;
				this.m_LastStopTime = this.timeControl.stopTime;
				if (this.m_NextTargetIsForward)
				{
					this.m_NextFloorHeight = this.Animator.targetPosition.y;
				}
				else
				{
					this.m_PrevFloorHeight = this.Animator.targetPosition.y;
				}
				this.m_NextTargetIsForward = !this.m_NextTargetIsForward;
				this.Animator.SetTarget(AvatarTarget.Root, (float)((!this.m_NextTargetIsForward) ? 0 : 1));
			}
		}
		public void DoAvatarPreviewDrag(EventType type)
		{
			if (type == EventType.DragUpdated)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}
			else
			{
				if (type == EventType.DragPerform)
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					GameObject gameObject = DragAndDrop.objectReferences[0] as GameObject;
					if (gameObject && AvatarPreview.GetAnimationType(gameObject) == this.animationClipType)
					{
						DragAndDrop.AcceptDrag();
						this.SetPreview(gameObject);
					}
				}
			}
		}
		public void AvatarTimeControlGUI(Rect rect)
		{
			Rect rect2 = rect;
			rect2.height = 21f;
			this.timeControl.DoTimeControl(rect2);
			rect.y = rect.yMax - 20f;
			float num = this.timeControl.currentTime - this.timeControl.startTime;
			EditorGUI.DropShadowLabel(new Rect(rect.x, rect.y, rect.width, 20f), string.Format("{0,2}:{1:00} ({2:000.0%})", (int)num, this.Repeat(Mathf.FloorToInt(num * (float)this.fps), this.fps), this.timeControl.normalizedTime));
		}
		public void DoAvatarPreview(Rect rect, GUIStyle background)
		{
			this.Init();
			Rect position = new Rect(rect.xMax - 16f, rect.yMax - 16f, 16f, 16f);
			if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, GUIStyle.none))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent("Auto"), false, new GenericMenu.MenuFunction2(this.SetPreviewAvatarOption), AvatarPreview.PreviewPopupOptions.Auto);
				genericMenu.AddItem(new GUIContent("Unity Model"), false, new GenericMenu.MenuFunction2(this.SetPreviewAvatarOption), AvatarPreview.PreviewPopupOptions.DefaultModel);
				genericMenu.AddItem(new GUIContent("Other..."), false, new GenericMenu.MenuFunction2(this.SetPreviewAvatarOption), AvatarPreview.PreviewPopupOptions.Other);
				genericMenu.ShowAsContext();
			}
			Rect rect2 = rect;
			rect2.yMin += 21f;
			rect2.height = Mathf.Max(rect2.height, 64f);
			this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, rect2);
			int controlID = GUIUtility.GetControlID(this.m_PreviewHint, FocusType.Native, rect2);
			Event current = Event.current;
			EventType typeForControl = current.GetTypeForControl(controlID);
			if (typeForControl == EventType.Repaint && this.m_IsValid)
			{
				Texture image = this.DoRenderPreview(rect2, background);
				GUI.DrawTexture(rect2, image, ScaleMode.StretchToFill, false);
			}
			this.AvatarTimeControlGUI(rect);
			GUI.DrawTexture(position, AvatarPreview.s_Styles.avatarIcon.image);
			if (!this.m_IsValid)
			{
				Rect position2 = rect2;
				position2.yMax -= position2.height / 2f - 16f;
				EditorGUI.DropShadowLabel(position2, "No model is available for preview.\nPlease drag a model into this Preview Area.");
			}
			this.DoAvatarPreviewDrag(typeForControl);
			if (current.type == EventType.ExecuteCommand)
			{
				string commandName = current.commandName;
				if (commandName == "ObjectSelectorUpdated" && ObjectSelector.get.objectSelectorID == this.m_ModelSelectorId)
				{
					this.SetPreview(ObjectSelector.GetCurrentObject() as GameObject);
					current.Use();
				}
			}
		}
		private void SetPreviewAvatarOption(object obj)
		{
			AvatarPreview.PreviewPopupOptions previewPopupOptions = (AvatarPreview.PreviewPopupOptions)((int)obj);
			if (previewPopupOptions == AvatarPreview.PreviewPopupOptions.Auto)
			{
				this.SetPreview(null);
			}
			else
			{
				if (previewPopupOptions == AvatarPreview.PreviewPopupOptions.DefaultModel)
				{
					this.SetPreview(AvatarPreview.GetHumanoidFallback());
				}
				else
				{
					if (previewPopupOptions == AvatarPreview.PreviewPopupOptions.Other)
					{
						ObjectSelector.get.Show(null, typeof(GameObject), null, false);
						ObjectSelector.get.objectSelectorID = this.m_ModelSelectorId;
					}
				}
			}
		}
		private void SetPreview(GameObject gameObject)
		{
			AvatarPreviewSelection.SetPreview(this.animationClipType, gameObject);
			UnityEngine.Object.DestroyImmediate(this.m_PreviewInstance);
			this.InitInstance(this.m_SourceScenePreviewAnimator, this.m_SourcePreviewMotion);
			if (this.m_OnAvatarChangeFunc != null)
			{
				this.m_OnAvatarChangeFunc();
			}
		}
		private int Repeat(int t, int length)
		{
			return (t % length + length) % length;
		}
	}
}
