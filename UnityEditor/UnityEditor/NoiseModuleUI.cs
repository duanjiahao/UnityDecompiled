using System;
using UnityEngine;

namespace UnityEditor
{
	internal class NoiseModuleUI : ModuleUI
	{
		private class Texts
		{
			public GUIContent separateAxes = EditorGUIUtility.TextContent("Separate Axes|If enabled, you can control the noise separately for each axis.");

			public GUIContent strength = EditorGUIUtility.TextContent("Strength|How strong the overall noise effect is.");

			public GUIContent frequency = EditorGUIUtility.TextContent("Frequency|Low values create soft, smooth noise, and high values create rapidly changing noise.");

			public GUIContent damping = EditorGUIUtility.TextContent("Damping|If enabled, strength is proportional to frequency.");

			public GUIContent octaves = EditorGUIUtility.TextContent("Octaves|Layers of noise that combine to produce final noise (Adding octaves increases the performance cost substantially!)");

			public GUIContent octaveMultiplier = EditorGUIUtility.TextContent("Octave Multiplier|When combining each octave, scale the intensity by this amount.");

			public GUIContent octaveScale = EditorGUIUtility.TextContent("Octave Scale|When combining each octave, zoom in by this amount.");

			public GUIContent quality = EditorGUIUtility.TextContent("Quality|Generate 1D, 2D or 3D noise.");

			public GUIContent scrollSpeed = EditorGUIUtility.TextContent("Scroll Speed|Scroll the noise map over the particle system.");

			public GUIContent remap = EditorGUIUtility.TextContent("Remap|Remap the final noise values into a new range.");

			public GUIContent remapCurve = EditorGUIUtility.TextContent("Remap Curve");

			public GUIContent x = EditorGUIUtility.TextContent("X");

			public GUIContent y = EditorGUIUtility.TextContent("Y");

			public GUIContent z = EditorGUIUtility.TextContent("Z");

			public GUIContent previewTexture = EditorGUIUtility.TextContent("Preview|Preview the noise as a texture.");

			public GUIContent previewTextureMultiEdit = EditorGUIUtility.TextContent("Preview (Disabled)|Preview is disabled in multi-object editing mode.");

			public string[] qualityDropdown = new string[]
			{
				"Low (1D)",
				"Medium (2D)",
				"High (3D)"
			};
		}

		private SerializedMinMaxCurve m_StrengthX;

		private SerializedMinMaxCurve m_StrengthY;

		private SerializedMinMaxCurve m_StrengthZ;

		private SerializedProperty m_SeparateAxes;

		private SerializedProperty m_Frequency;

		private SerializedProperty m_Damping;

		private SerializedProperty m_Octaves;

		private SerializedProperty m_OctaveMultiplier;

		private SerializedProperty m_OctaveScale;

		private SerializedProperty m_Quality;

		private SerializedMinMaxCurve m_ScrollSpeed;

		private SerializedMinMaxCurve m_RemapX;

		private SerializedMinMaxCurve m_RemapY;

		private SerializedMinMaxCurve m_RemapZ;

		private SerializedProperty m_RemapEnabled;

		private const int k_PreviewSize = 96;

		private static Texture2D s_PreviewTexture;

		private static bool s_PreviewTextureDirty = true;

		private GUIStyle previewTextureStyle;

		private static NoiseModuleUI.Texts s_Texts;

		public NoiseModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "NoiseModule", displayName)
		{
			this.m_ToolTip = "Add noise/turbulence to particle movement.";
		}

		protected override void Init()
		{
			if (this.m_StrengthX == null)
			{
				if (NoiseModuleUI.s_Texts == null)
				{
					NoiseModuleUI.s_Texts = new NoiseModuleUI.Texts();
				}
				this.m_StrengthX = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.x, "strength", ModuleUI.kUseSignedRange);
				this.m_StrengthY = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.y, "strengthY", ModuleUI.kUseSignedRange);
				this.m_StrengthZ = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.z, "strengthZ", ModuleUI.kUseSignedRange);
				this.m_SeparateAxes = base.GetProperty("separateAxes");
				this.m_Damping = base.GetProperty("damping");
				this.m_Frequency = base.GetProperty("frequency");
				this.m_Octaves = base.GetProperty("octaves");
				this.m_OctaveMultiplier = base.GetProperty("octaveMultiplier");
				this.m_OctaveScale = base.GetProperty("octaveScale");
				this.m_Quality = base.GetProperty("quality");
				this.m_ScrollSpeed = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.scrollSpeed, "scrollSpeed", ModuleUI.kUseSignedRange);
				this.m_ScrollSpeed.m_AllowRandom = false;
				this.m_RemapX = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.x, "remap", ModuleUI.kUseSignedRange);
				this.m_RemapY = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.y, "remapY", ModuleUI.kUseSignedRange);
				this.m_RemapZ = new SerializedMinMaxCurve(this, NoiseModuleUI.s_Texts.z, "remapZ", ModuleUI.kUseSignedRange);
				this.m_RemapX.m_AllowRandom = false;
				this.m_RemapY.m_AllowRandom = false;
				this.m_RemapZ.m_AllowRandom = false;
				this.m_RemapX.m_AllowConstant = false;
				this.m_RemapY.m_AllowConstant = false;
				this.m_RemapZ.m_AllowConstant = false;
				this.m_RemapEnabled = base.GetProperty("remapEnabled");
				if (NoiseModuleUI.s_PreviewTexture == null)
				{
					NoiseModuleUI.s_PreviewTexture = new Texture2D(96, 96, TextureFormat.RGBA32, false, true);
					NoiseModuleUI.s_PreviewTexture.name = "ParticleNoisePreview";
					NoiseModuleUI.s_PreviewTexture.filterMode = FilterMode.Bilinear;
					NoiseModuleUI.s_PreviewTexture.hideFlags = HideFlags.HideAndDontSave;
					NoiseModuleUI.s_Texts.previewTexture.image = NoiseModuleUI.s_PreviewTexture;
					NoiseModuleUI.s_Texts.previewTextureMultiEdit.image = NoiseModuleUI.s_PreviewTexture;
				}
				NoiseModuleUI.s_PreviewTextureDirty = true;
				this.previewTextureStyle = new GUIStyle(ParticleSystemStyles.Get().label);
				this.previewTextureStyle.alignment = TextAnchor.LowerCenter;
				this.previewTextureStyle.imagePosition = ImagePosition.ImageAbove;
			}
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (NoiseModuleUI.s_Texts == null)
			{
				NoiseModuleUI.s_Texts = new NoiseModuleUI.Texts();
			}
			if (NoiseModuleUI.s_PreviewTextureDirty)
			{
				if (this.m_ParticleSystemUI.multiEdit)
				{
					Color32[] array = new Color32[NoiseModuleUI.s_PreviewTexture.width * NoiseModuleUI.s_PreviewTexture.height];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = new Color32(120, 120, 120, 255);
					}
					NoiseModuleUI.s_PreviewTexture.SetPixels32(array);
					NoiseModuleUI.s_PreviewTexture.Apply(false);
				}
				else
				{
					this.m_ParticleSystemUI.m_ParticleSystems[0].GenerateNoisePreviewTexture(NoiseModuleUI.s_PreviewTexture);
				}
				NoiseModuleUI.s_PreviewTextureDirty = false;
			}
			if (!base.isWindowView)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
			}
			EditorGUI.BeginChangeCheck();
			bool flag = ModuleUI.GUIToggle(NoiseModuleUI.s_Texts.separateAxes, this.m_SeparateAxes, new GUILayoutOption[0]);
			bool flag2 = EditorGUI.EndChangeCheck();
			EditorGUI.BeginChangeCheck();
			if (flag2)
			{
				if (flag)
				{
					this.m_StrengthX.RemoveCurveFromEditor();
					this.m_RemapX.RemoveCurveFromEditor();
				}
				else
				{
					this.m_StrengthX.RemoveCurveFromEditor();
					this.m_StrengthY.RemoveCurveFromEditor();
					this.m_StrengthZ.RemoveCurveFromEditor();
					this.m_RemapX.RemoveCurveFromEditor();
					this.m_RemapY.RemoveCurveFromEditor();
					this.m_RemapZ.RemoveCurveFromEditor();
				}
			}
			if (!this.m_StrengthX.stateHasMultipleDifferentValues)
			{
				this.m_StrengthZ.SetMinMaxState(this.m_StrengthX.state, flag);
				this.m_StrengthY.SetMinMaxState(this.m_StrengthX.state, flag);
			}
			if (!this.m_RemapX.stateHasMultipleDifferentValues)
			{
				this.m_RemapZ.SetMinMaxState(this.m_RemapX.state, flag);
				this.m_RemapY.SetMinMaxState(this.m_RemapX.state, flag);
			}
			if (flag)
			{
				this.m_StrengthX.m_DisplayName = NoiseModuleUI.s_Texts.x;
				base.GUITripleMinMaxCurve(GUIContent.none, NoiseModuleUI.s_Texts.x, this.m_StrengthX, NoiseModuleUI.s_Texts.y, this.m_StrengthY, NoiseModuleUI.s_Texts.z, this.m_StrengthZ, null, new GUILayoutOption[0]);
			}
			else
			{
				this.m_StrengthX.m_DisplayName = NoiseModuleUI.s_Texts.strength;
				ModuleUI.GUIMinMaxCurve(NoiseModuleUI.s_Texts.strength, this.m_StrengthX, new GUILayoutOption[0]);
			}
			ModuleUI.GUIFloat(NoiseModuleUI.s_Texts.frequency, this.m_Frequency, new GUILayoutOption[0]);
			ModuleUI.GUIMinMaxCurve(NoiseModuleUI.s_Texts.scrollSpeed, this.m_ScrollSpeed, new GUILayoutOption[0]);
			ModuleUI.GUIToggle(NoiseModuleUI.s_Texts.damping, this.m_Damping, new GUILayoutOption[0]);
			int num = ModuleUI.GUIInt(NoiseModuleUI.s_Texts.octaves, this.m_Octaves, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(num == 1))
			{
				ModuleUI.GUIFloat(NoiseModuleUI.s_Texts.octaveMultiplier, this.m_OctaveMultiplier, new GUILayoutOption[0]);
				ModuleUI.GUIFloat(NoiseModuleUI.s_Texts.octaveScale, this.m_OctaveScale, new GUILayoutOption[0]);
			}
			ModuleUI.GUIPopup(NoiseModuleUI.s_Texts.quality, this.m_Quality, NoiseModuleUI.s_Texts.qualityDropdown, new GUILayoutOption[0]);
			bool flag3 = ModuleUI.GUIToggle(NoiseModuleUI.s_Texts.remap, this.m_RemapEnabled, new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(!flag3))
			{
				if (flag)
				{
					this.m_RemapX.m_DisplayName = NoiseModuleUI.s_Texts.x;
					base.GUITripleMinMaxCurve(GUIContent.none, NoiseModuleUI.s_Texts.x, this.m_RemapX, NoiseModuleUI.s_Texts.y, this.m_RemapY, NoiseModuleUI.s_Texts.z, this.m_RemapZ, null, new GUILayoutOption[0]);
				}
				else
				{
					this.m_RemapX.m_DisplayName = NoiseModuleUI.s_Texts.remap;
					ModuleUI.GUIMinMaxCurve(NoiseModuleUI.s_Texts.remapCurve, this.m_RemapX, new GUILayoutOption[0]);
				}
			}
			if (!base.isWindowView)
			{
				GUILayout.EndVertical();
			}
			if (EditorGUI.EndChangeCheck() || this.m_ScrollSpeed.scalar.floatValue != 0f || flag3 || flag2)
			{
				NoiseModuleUI.s_PreviewTextureDirty = true;
				this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
			}
			if (this.m_ParticleSystemUI.multiEdit)
			{
				GUILayout.Label(NoiseModuleUI.s_Texts.previewTextureMultiEdit, this.previewTextureStyle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true),
					GUILayout.ExpandHeight(false)
				});
			}
			else
			{
				GUILayout.Label(NoiseModuleUI.s_Texts.previewTexture, this.previewTextureStyle, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true),
					GUILayout.ExpandHeight(false)
				});
			}
			if (!base.isWindowView)
			{
				GUILayout.EndHorizontal();
			}
		}
	}
}
