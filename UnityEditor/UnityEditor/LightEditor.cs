using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Light))]
	internal class LightEditor : Editor
	{
		private class Styles
		{
			public static GUIStyle sliderBox;

			public static GUIStyle sliderThumb;

			public readonly GUIContent Type = EditorGUIUtility.TextContent("Type|Specifies the current type of light. Possible types are Directional, Spot, Point, and Area lights.");

			public readonly GUIContent Range = EditorGUIUtility.TextContent("Range|Controls how far the light is emitted from the center of the object.");

			public readonly GUIContent SpotAngle = EditorGUIUtility.TextContent("Spot Angle|Controls the angle in degrees at the base of a Spot light�s cone.");

			public readonly GUIContent Color = EditorGUIUtility.TextContent("Color|Controls the color being emitted by the light.");

			public readonly GUIContent UseColorTemperature = EditorGUIUtility.TextContent("Use color temperature mode|Cho0se between RGB and temperature mode for light's color.");

			public readonly GUIContent ColorFilter = EditorGUIUtility.TextContent("Filter|A colored gel can be put in front of the light source to tint the light.");

			public readonly GUIContent ColorTemperature = EditorGUIUtility.TextContent("Temperature|Also known as CCT (Correlated color temperature). The color temperature of the electromagnetic radiation emitted from an ideal black body is defined as its surface temperature in Kelvin. White is 6500K");

			public readonly GUIContent Intensity = EditorGUIUtility.TextContent("Intensity|Controls the brightness of the light. Light color is multiplied by this value.");

			public readonly GUIContent LightmappingMode = EditorGUIUtility.TextContent("Mode|Specifies the light mode used to determine if and how a light will be baked. Possible modes are Baked, Mixed, and Realtime.");

			public readonly GUIContent LightBounceIntensity = EditorGUIUtility.TextContent("Indirect Multiplier|Controls the intensity of indirect light being contributed to the scene. A value of 0 will cause Dynamic lights to be removed from realtime global illumination and Static and Stationary lights to no longer emit indirect lighting. Has no effect when both Realtime and Baked Global Illumination are disabled.");

			public readonly GUIContent ShadowType = EditorGUIUtility.TextContent("Shadow Type|Specifies whether Hard Shadows, Soft Shadows, or No Shadows will be cast by the light.");

			public readonly GUIContent ShadowRealtimeSettings = EditorGUIUtility.TextContent("Realtime Shadows|Settings for realtime direct shadows.");

			public readonly GUIContent ShadowStrength = EditorGUIUtility.TextContent("Strength|Controls how dark the shadows cast by the light will be.");

			public readonly GUIContent ShadowResolution = EditorGUIUtility.TextContent("Resolution|Controls the rendered resolution of the shadow maps. A higher resolution will increase the fidelity of shadows at the cost of GPU performance and memory usage.");

			public readonly GUIContent ShadowBias = EditorGUIUtility.TextContent("Bias|Controls the distance at which the shadows will be pushed away from the light. Useful for avoiding false self-shadowing artifacts.");

			public readonly GUIContent ShadowNormalBias = EditorGUIUtility.TextContent("Normal Bias|Controls distance at which the shadow casting surfaces will be shrunk along the surface normal. Useful for avoiding false self-shadowing artifacts.");

			public readonly GUIContent ShadowNearPlane = EditorGUIUtility.TextContent("Near Plane|Controls the value for the near clip plane when rendering shadows. Currently clamped to 0.1 units or 1% of the lights range property, whichever is lower.");

			public readonly GUIContent BakedShadowRadius = EditorGUIUtility.TextContent("Baked Shadow Radius|Controls the amount of artificial softening applied to the edges of shadows cast by the Point or Spot light.");

			public readonly GUIContent BakedShadowAngle = EditorGUIUtility.TextContent("Baked Shadow Angle|Controls the amount of artificial softening applied to the edges of shadows cast by directional lights.");

			public readonly GUIContent Cookie = EditorGUIUtility.TextContent("Cookie|Specifies the Texture mask to cast shadows, create silhouettes, or patterned illumination for the light.");

			public readonly GUIContent CookieSize = EditorGUIUtility.TextContent("Cookie Size|Controls the size of the cookie mask currently assigned to the light.");

			public readonly GUIContent DrawHalo = EditorGUIUtility.TextContent("Draw Halo|When enabled, draws a spherical halo of light with a radius equal to the lights range value.");

			public readonly GUIContent Flare = EditorGUIUtility.TextContent("Flare|Specifies the flare object to be used by the light to render lens flares in the scene.");

			public readonly GUIContent RenderMode = EditorGUIUtility.TextContent("Render Mode|Specifies the importance of the light which impacts lighting fidelity and performance. Options are Auto, Important, and Not Important. This only affects Forward Rendering");

			public readonly GUIContent CullingMask = EditorGUIUtility.TextContent("Culling Mask|Specifies which layers will be affected or excluded from the light�s effect on objects in the scene.");

			public readonly GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "Remove command buffer");

			public readonly GUIStyle invisibleButton = "InvisibleButton";

			public readonly GUIContent AreaWidth = EditorGUIUtility.TextContent("Width|Controls the width in units of the area light.");

			public readonly GUIContent AreaHeight = EditorGUIUtility.TextContent("Height|Controls the height in units of the area light.");

			public readonly GUIContent BakingWarning = EditorGUIUtility.TextContent("Light mode is currently overridden to Realtime mode. Enable Baked Global Illumination to use Mixed or Baked light modes.");

			public readonly GUIContent IndirectBounceShadowWarning = EditorGUIUtility.TextContent("Realtime indirect bounce shadowing is not supported for Spot and Point lights.");

			public readonly GUIContent CookieWarning = EditorGUIUtility.TextContent("Cookie textures for spot lights should be set to clamp, not repeat, to avoid artifacts.");

			public readonly GUIContent DisabledLightWarning = EditorGUIUtility.TextContent("Lighting has been disabled in at least one Scene view. Any changes applied to lights in the Scene will not be updated in these views until Lighting has been enabled again.");

			static Styles()
			{
				LightEditor.Styles.sliderBox = new GUIStyle("ColorPickerBox");
				LightEditor.Styles.sliderThumb = new GUIStyle("ColorPickerHorizThumb");
				LightEditor.Styles.sliderBox.overflow = new RectOffset(0, 0, -4, -4);
				LightEditor.Styles.sliderBox.padding = new RectOffset(0, 0, 1, 1);
			}
		}

		private SerializedProperty m_Type;

		private SerializedProperty m_Range;

		private SerializedProperty m_SpotAngle;

		private SerializedProperty m_CookieSize;

		private SerializedProperty m_Color;

		private SerializedProperty m_Intensity;

		private SerializedProperty m_BounceIntensity;

		private SerializedProperty m_ColorTemperature;

		private SerializedProperty m_UseColorTemperature;

		private SerializedProperty m_Cookie;

		private SerializedProperty m_ShadowsType;

		private SerializedProperty m_ShadowsStrength;

		private SerializedProperty m_ShadowsResolution;

		private SerializedProperty m_ShadowsBias;

		private SerializedProperty m_ShadowsNormalBias;

		private SerializedProperty m_ShadowsNearPlane;

		private SerializedProperty m_Halo;

		private SerializedProperty m_Flare;

		private SerializedProperty m_RenderMode;

		private SerializedProperty m_CullingMask;

		private SerializedProperty m_Lightmapping;

		private SerializedProperty m_AreaSizeX;

		private SerializedProperty m_AreaSizeY;

		private SerializedProperty m_BakedShadowRadius;

		private SerializedProperty m_BakedShadowAngle;

		private Texture2D m_KelvinGradientTexture;

		private const float kMinKelvin = 1000f;

		private const float kMaxKelvin = 20000f;

		private const float kSliderPower = 2f;

		private AnimBool m_AnimShowSpotOptions = new AnimBool();

		private AnimBool m_AnimShowPointOptions = new AnimBool();

		private AnimBool m_AnimShowDirOptions = new AnimBool();

		private AnimBool m_AnimShowAreaOptions = new AnimBool();

		private AnimBool m_AnimShowRuntimeOptions = new AnimBool();

		private AnimBool m_AnimShowShadowOptions = new AnimBool();

		private AnimBool m_AnimBakedShadowAngleOptions = new AnimBool();

		private AnimBool m_AnimBakedShadowRadiusOptions = new AnimBool();

		private AnimBool m_AnimShowLightBounceIntensity = new AnimBool();

		private bool m_CommandBuffersShown = true;

		private static LightEditor.Styles s_Styles;

		internal static Color kGizmoLight = new Color(0.996078432f, 0.992156863f, 0.533333361f, 0.5019608f);

		internal static Color kGizmoDisabledLight = new Color(0.5294118f, 0.454901963f, 0.196078435f, 0.5019608f);

		private bool typeIsSame
		{
			get
			{
				return !this.m_Type.hasMultipleDifferentValues;
			}
		}

		private bool shadowTypeIsSame
		{
			get
			{
				return !this.m_ShadowsType.hasMultipleDifferentValues;
			}
		}

		private bool lightmappingTypeIsSame
		{
			get
			{
				return !this.m_Lightmapping.hasMultipleDifferentValues;
			}
		}

		private Light light
		{
			get
			{
				return base.target as Light;
			}
		}

		private bool isRealtime
		{
			get
			{
				return this.m_Lightmapping.intValue == 4;
			}
		}

		private bool isCompletelyBaked
		{
			get
			{
				return this.m_Lightmapping.intValue == 2;
			}
		}

		private bool isBakedOrMixed
		{
			get
			{
				return !this.isRealtime;
			}
		}

		private Texture cookie
		{
			get
			{
				return this.m_Cookie.objectReferenceValue as Texture;
			}
		}

		private bool spotOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Spot;
			}
		}

		private bool pointOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Point;
			}
		}

		private bool dirOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Directional;
			}
		}

		private bool areaOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Area;
			}
		}

		private bool runtimeOptionsValue
		{
			get
			{
				return this.typeIsSame && this.light.type != LightType.Area && !this.isCompletelyBaked;
			}
		}

		private bool bakedShadowRadius
		{
			get
			{
				return this.typeIsSame && (this.light.type == LightType.Point || this.light.type == LightType.Spot) && this.isBakedOrMixed;
			}
		}

		private bool bakedShadowAngle
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Directional && this.isBakedOrMixed;
			}
		}

		private bool shadowOptionsValue
		{
			get
			{
				return this.shadowTypeIsSame && this.light.shadows != LightShadows.None;
			}
		}

		private bool bounceWarningValue
		{
			get
			{
				return this.typeIsSame && (this.light.type == LightType.Point || this.light.type == LightType.Spot) && this.lightmappingTypeIsSame && this.isRealtime && !this.m_BounceIntensity.hasMultipleDifferentValues && this.m_BounceIntensity.floatValue > 0f;
			}
		}

		private bool bakingWarningValue
		{
			get
			{
				return !Lightmapping.bakedGI && this.lightmappingTypeIsSame && this.isBakedOrMixed;
			}
		}

		private bool showLightBounceIntensity
		{
			get
			{
				return true;
			}
		}

		private bool cookieWarningValue
		{
			get
			{
				return this.typeIsSame && this.light.type == LightType.Spot && !this.m_Cookie.hasMultipleDifferentValues && this.cookie && this.cookie.wrapMode != TextureWrapMode.Clamp;
			}
		}

		private bool isPrefab
		{
			get
			{
				PrefabType prefabType = PrefabUtility.GetPrefabType(base.target);
				return prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab;
			}
		}

		private void SetOptions(AnimBool animBool, bool initialize, bool targetValue)
		{
			if (initialize)
			{
				animBool.value = targetValue;
				animBool.valueChanged.AddListener(new UnityAction(base.Repaint));
			}
			else
			{
				animBool.target = targetValue;
			}
		}

		private void UpdateShowOptions(bool initialize)
		{
			this.SetOptions(this.m_AnimShowSpotOptions, initialize, this.spotOptionsValue);
			this.SetOptions(this.m_AnimShowPointOptions, initialize, this.pointOptionsValue);
			this.SetOptions(this.m_AnimShowDirOptions, initialize, this.dirOptionsValue);
			this.SetOptions(this.m_AnimShowAreaOptions, initialize, this.areaOptionsValue);
			this.SetOptions(this.m_AnimShowShadowOptions, initialize, this.shadowOptionsValue);
			this.SetOptions(this.m_AnimShowRuntimeOptions, initialize, this.runtimeOptionsValue);
			this.SetOptions(this.m_AnimBakedShadowAngleOptions, initialize, this.bakedShadowAngle);
			this.SetOptions(this.m_AnimBakedShadowRadiusOptions, initialize, this.bakedShadowRadius);
			this.SetOptions(this.m_AnimShowLightBounceIntensity, initialize, this.showLightBounceIntensity);
		}

		private void LightUsageGUI()
		{
			if (EditorGUILayout.BeginFadeGroup(1f - this.m_AnimShowAreaOptions.faded))
			{
				LightModeUtil.Get().DrawElement(this.m_Lightmapping, LightEditor.s_Styles.LightmappingMode);
				if (this.bakingWarningValue)
				{
					EditorGUILayout.HelpBox(LightEditor.s_Styles.BakingWarning.text, MessageType.Info);
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private void OnEnable()
		{
			this.m_Type = base.serializedObject.FindProperty("m_Type");
			this.m_Range = base.serializedObject.FindProperty("m_Range");
			this.m_SpotAngle = base.serializedObject.FindProperty("m_SpotAngle");
			this.m_CookieSize = base.serializedObject.FindProperty("m_CookieSize");
			this.m_Color = base.serializedObject.FindProperty("m_Color");
			this.m_Intensity = base.serializedObject.FindProperty("m_Intensity");
			this.m_BounceIntensity = base.serializedObject.FindProperty("m_BounceIntensity");
			this.m_ColorTemperature = base.serializedObject.FindProperty("m_ColorTemperature");
			this.m_UseColorTemperature = base.serializedObject.FindProperty("m_UseColorTemperature");
			this.m_Cookie = base.serializedObject.FindProperty("m_Cookie");
			this.m_ShadowsType = base.serializedObject.FindProperty("m_Shadows.m_Type");
			this.m_ShadowsStrength = base.serializedObject.FindProperty("m_Shadows.m_Strength");
			this.m_ShadowsResolution = base.serializedObject.FindProperty("m_Shadows.m_Resolution");
			this.m_ShadowsBias = base.serializedObject.FindProperty("m_Shadows.m_Bias");
			this.m_ShadowsNormalBias = base.serializedObject.FindProperty("m_Shadows.m_NormalBias");
			this.m_ShadowsNearPlane = base.serializedObject.FindProperty("m_Shadows.m_NearPlane");
			this.m_Halo = base.serializedObject.FindProperty("m_DrawHalo");
			this.m_Flare = base.serializedObject.FindProperty("m_Flare");
			this.m_RenderMode = base.serializedObject.FindProperty("m_RenderMode");
			this.m_CullingMask = base.serializedObject.FindProperty("m_CullingMask");
			this.m_Lightmapping = base.serializedObject.FindProperty("m_Lightmapping");
			this.m_AreaSizeX = base.serializedObject.FindProperty("m_AreaSize.x");
			this.m_AreaSizeY = base.serializedObject.FindProperty("m_AreaSize.y");
			this.m_BakedShadowRadius = base.serializedObject.FindProperty("m_ShadowRadius");
			this.m_BakedShadowAngle = base.serializedObject.FindProperty("m_ShadowAngle");
			this.UpdateShowOptions(true);
			if (this.m_KelvinGradientTexture == null)
			{
				this.m_KelvinGradientTexture = LightEditor.CreateKelvinGradientTexture("KelvinGradientTexture", 300, 16, 1000f, 20000f);
			}
		}

		private void OnDestroy()
		{
			if (this.m_KelvinGradientTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_KelvinGradientTexture);
			}
		}

		private void CommandBufferGUI()
		{
			if (base.targets.Length == 1)
			{
				Light light = base.target as Light;
				if (!(light == null))
				{
					int commandBufferCount = light.commandBufferCount;
					if (commandBufferCount != 0)
					{
						this.m_CommandBuffersShown = GUILayout.Toggle(this.m_CommandBuffersShown, GUIContent.Temp(commandBufferCount + " command buffers"), EditorStyles.foldout, new GUILayoutOption[0]);
						if (this.m_CommandBuffersShown)
						{
							EditorGUI.indentLevel++;
							LightEvent[] array = (LightEvent[])Enum.GetValues(typeof(LightEvent));
							for (int i = 0; i < array.Length; i++)
							{
								LightEvent lightEvent = array[i];
								CommandBuffer[] commandBuffers = light.GetCommandBuffers(lightEvent);
								CommandBuffer[] array2 = commandBuffers;
								for (int j = 0; j < array2.Length; j++)
								{
									CommandBuffer commandBuffer = array2[j];
									using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
									{
										Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
										rect.xMin += EditorGUI.indent;
										Rect removeButtonRect = LightEditor.GetRemoveButtonRect(rect);
										rect.xMax = removeButtonRect.x;
										GUI.Label(rect, string.Format("{0}: {1} ({2})", lightEvent, commandBuffer.name, EditorUtility.FormatBytes(commandBuffer.sizeInBytes)), EditorStyles.miniLabel);
										if (GUI.Button(removeButtonRect, LightEditor.s_Styles.iconRemove, LightEditor.s_Styles.invisibleButton))
										{
											light.RemoveCommandBuffer(lightEvent, commandBuffer);
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
									light.RemoveAllCommandBuffers();
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

		private static Rect GetRemoveButtonRect(Rect r)
		{
			Vector2 vector = LightEditor.s_Styles.invisibleButton.CalcSize(LightEditor.s_Styles.iconRemove);
			return new Rect(r.xMax - vector.x, r.y + (float)((int)(r.height / 2f - vector.y / 2f)), vector.x, vector.y);
		}

		public override void OnInspectorGUI()
		{
			if (LightEditor.s_Styles == null)
			{
				LightEditor.s_Styles = new LightEditor.Styles();
			}
			base.serializedObject.Update();
			this.UpdateShowOptions(false);
			EditorGUILayout.PropertyField(this.m_Type, LightEditor.s_Styles.Type, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			float value = 1f - this.m_AnimShowDirOptions.faded;
			if (EditorGUILayout.BeginFadeGroup(value))
			{
				if (this.m_AnimShowAreaOptions.target)
				{
					GUI.enabled = false;
					string tooltip = "For area lights " + this.m_Range.displayName + " is computed from Width, Height and Intensity";
					GUIContent label = new GUIContent(this.m_Range.displayName, tooltip);
					EditorGUILayout.FloatField(label, this.light.range, new GUILayoutOption[0]);
					GUI.enabled = true;
				}
				else
				{
					EditorGUILayout.PropertyField(this.m_Range, LightEditor.s_Styles.Range, new GUILayoutOption[0]);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_AnimShowSpotOptions.faded))
			{
				EditorGUILayout.Slider(this.m_SpotAngle, 1f, 179f, LightEditor.s_Styles.SpotAngle, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_AnimShowAreaOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_AreaSizeX, LightEditor.s_Styles.AreaWidth, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AreaSizeY, LightEditor.s_Styles.AreaHeight, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (GraphicsSettings.lightsUseLinearIntensity && GraphicsSettings.lightsUseColorTemperature)
			{
				EditorGUILayout.PropertyField(this.m_UseColorTemperature, LightEditor.s_Styles.UseColorTemperature, new GUILayoutOption[0]);
				if (this.m_UseColorTemperature.boolValue)
				{
					EditorGUILayout.LabelField(LightEditor.s_Styles.Color, new GUILayoutOption[0]);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_Color, LightEditor.s_Styles.ColorFilter, new GUILayoutOption[0]);
					EditorGUILayout.SliderWithTexture(LightEditor.s_Styles.ColorTemperature, this.m_ColorTemperature, 1000f, 20000f, 2f, LightEditor.Styles.sliderBox, LightEditor.Styles.sliderThumb, this.m_KelvinGradientTexture, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				else
				{
					EditorGUILayout.PropertyField(this.m_Color, LightEditor.s_Styles.Color, new GUILayoutOption[0]);
				}
			}
			else
			{
				EditorGUILayout.PropertyField(this.m_Color, LightEditor.s_Styles.Color, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
			this.LightUsageGUI();
			EditorGUILayout.PropertyField(this.m_Intensity, LightEditor.s_Styles.Intensity, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(this.m_AnimShowLightBounceIntensity.faded))
			{
				EditorGUILayout.PropertyField(this.m_BounceIntensity, LightEditor.s_Styles.LightBounceIntensity, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if (this.bounceWarningValue)
			{
				EditorGUILayout.HelpBox(LightEditor.s_Styles.IndirectBounceShadowWarning.text, MessageType.Info);
			}
			this.ShadowsGUI();
			if (EditorGUILayout.BeginFadeGroup(this.m_AnimShowRuntimeOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_Cookie, LightEditor.s_Styles.Cookie, new GUILayoutOption[0]);
				if (this.cookieWarningValue)
				{
					EditorGUILayout.HelpBox(LightEditor.s_Styles.CookieWarning.text, MessageType.Warning);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(this.m_AnimShowRuntimeOptions.faded * this.m_AnimShowDirOptions.faded))
			{
				EditorGUILayout.PropertyField(this.m_CookieSize, LightEditor.s_Styles.CookieSize, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(this.m_Halo, LightEditor.s_Styles.DrawHalo, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Flare, LightEditor.s_Styles.Flare, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_RenderMode, LightEditor.s_Styles.RenderMode, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_CullingMask, LightEditor.s_Styles.CullingMask, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			if (SceneView.lastActiveSceneView != null && !SceneView.lastActiveSceneView.m_SceneLighting)
			{
				EditorGUILayout.HelpBox(LightEditor.s_Styles.DisabledLightWarning.text, MessageType.Warning);
			}
			this.CommandBufferGUI();
			base.serializedObject.ApplyModifiedProperties();
		}

		private void ShadowsGUI()
		{
			float num = 1f - this.m_AnimShowAreaOptions.faded;
			if (EditorGUILayout.BeginFadeGroup(num))
			{
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_ShadowsType, LightEditor.s_Styles.ShadowType, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			num *= this.m_AnimShowShadowOptions.faded;
			EditorGUI.indentLevel++;
			if (EditorGUILayout.BeginFadeGroup(num * this.m_AnimBakedShadowRadiusOptions.faded))
			{
				using (new EditorGUI.DisabledScope(this.m_ShadowsType.intValue != 2))
				{
					EditorGUILayout.PropertyField(this.m_BakedShadowRadius, LightEditor.s_Styles.BakedShadowRadius, new GUILayoutOption[0]);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(num * this.m_AnimBakedShadowAngleOptions.faded))
			{
				using (new EditorGUI.DisabledScope(this.m_ShadowsType.intValue != 2))
				{
					EditorGUILayout.Slider(this.m_BakedShadowAngle, 0f, 90f, LightEditor.s_Styles.BakedShadowAngle, new GUILayoutOption[0]);
				}
			}
			EditorGUILayout.EndFadeGroup();
			if (EditorGUILayout.BeginFadeGroup(num * this.m_AnimShowRuntimeOptions.faded))
			{
				EditorGUILayout.LabelField(LightEditor.s_Styles.ShadowRealtimeSettings, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				EditorGUILayout.Slider(this.m_ShadowsStrength, 0f, 1f, LightEditor.s_Styles.ShadowStrength, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ShadowsResolution, LightEditor.s_Styles.ShadowResolution, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ShadowsBias, 0f, 2f, LightEditor.s_Styles.ShadowBias, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_ShadowsNormalBias, 0f, 3f, LightEditor.s_Styles.ShadowNormalBias, new GUILayoutOption[0]);
				float leftValue = Mathf.Min(0.01f * this.m_Range.floatValue, 0.1f);
				EditorGUILayout.Slider(this.m_ShadowsNearPlane, leftValue, 10f, LightEditor.s_Styles.ShadowNearPlane, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}

		private void OnSceneGUI()
		{
			Light light = (Light)base.target;
			Color color = Handles.color;
			if (light.enabled)
			{
				Handles.color = LightEditor.kGizmoLight;
			}
			else
			{
				Handles.color = LightEditor.kGizmoDisabledLight;
			}
			float num = light.range;
			switch (light.type)
			{
			case LightType.Spot:
			{
				Color color2 = Handles.color;
				color2.a = Mathf.Clamp01(color.a * 2f);
				Handles.color = color2;
				Vector2 angleAndRange = new Vector2(light.spotAngle, light.range);
				angleAndRange = Handles.ConeHandle(light.transform.rotation, light.transform.position, angleAndRange, 1f, 1f, true);
				if (GUI.changed)
				{
					Undo.RecordObject(light, "Adjust Spot Light");
					light.spotAngle = angleAndRange.x;
					light.range = Mathf.Max(angleAndRange.y, 0.01f);
				}
				break;
			}
			case LightType.Point:
				num = Handles.RadiusHandle(Quaternion.identity, light.transform.position, num, true);
				if (GUI.changed)
				{
					Undo.RecordObject(light, "Adjust Point Light");
					light.range = num;
				}
				break;
			case LightType.Area:
			{
				EditorGUI.BeginChangeCheck();
				Vector2 areaSize = Handles.DoRectHandles(light.transform.rotation, light.transform.position, light.areaSize);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(light, "Adjust Area Light");
					light.areaSize = areaSize;
				}
				break;
			}
			}
			Handles.color = color;
		}

		private static Texture2D CreateKelvinGradientTexture(string name, int width, int height, float minKelvin, float maxKelvin)
		{
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
			texture2D.name = name;
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color32[] array = new Color32[width * height];
			float num = Mathf.Pow(maxKelvin, 0.5f);
			float num2 = Mathf.Pow(minKelvin, 0.5f);
			for (int i = 0; i < width; i++)
			{
				float num3 = (float)i / (float)(width - 1);
				float f = (num - num2) * num3 + num2;
				float kelvin = Mathf.Pow(f, 2f);
				Color color = Mathf.CorrelatedColorTemperatureToRGB(kelvin);
				for (int j = 0; j < height; j++)
				{
					array[j * width + i] = color.gamma;
				}
			}
			texture2D.SetPixels32(array);
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.Apply();
			return texture2D;
		}
	}
}
