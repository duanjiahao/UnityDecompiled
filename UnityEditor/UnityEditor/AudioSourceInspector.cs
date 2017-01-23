using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AudioSource))]
	internal class AudioSourceInspector : Editor
	{
		private class AudioCurveWrapper
		{
			public AudioSourceInspector.AudioCurveType type;

			public GUIContent legend;

			public int id;

			public Color color;

			public SerializedProperty curveProp;

			public float rangeMin;

			public float rangeMax;

			public AudioCurveWrapper(AudioSourceInspector.AudioCurveType type, string legend, int id, Color color, SerializedProperty curveProp, float rangeMin, float rangeMax)
			{
				this.type = type;
				this.legend = new GUIContent(legend);
				this.id = id;
				this.color = color;
				this.curveProp = curveProp;
				this.rangeMin = rangeMin;
				this.rangeMax = rangeMax;
			}
		}

		private enum AudioCurveType
		{
			Volume,
			SpatialBlend,
			Lowpass,
			Spread,
			ReverbZoneMix
		}

		private class Styles
		{
			public GUIStyle labelStyle = "ProfilerBadge";

			public GUIContent rolloffLabel = new GUIContent("Volume Rolloff", "Which type of rolloff curve to use");

			public string controlledByCurveLabel = "Controlled by curve";

			public GUIContent audioClipLabel = new GUIContent("AudioClip", "The AudioClip asset played by the AudioSource. Can be undefined if the AudioSource is generating a live stream of audio via OnAudioFilterRead.");

			public GUIContent panStereoLabel = new GUIContent("Stereo Pan", "Only valid for Mono and Stereo AudioClips. Mono sounds will be panned at constant power left and right. Stereo sounds will Stereo sounds have each left/right value faded up and down according to the specified pan value.");

			public GUIContent spatialBlendLabel = new GUIContent("Spatial Blend", "Sets how much this AudioSource is treated as a 3D source. 3D sources are affected by spatial position and spread. If 3D Pan Level is 0, all spatial attenuation is ignored.");

			public GUIContent reverbZoneMixLabel = new GUIContent("Reverb Zone Mix", "Sets how much of the signal this AudioSource is mixing into the global reverb associated with the zones. [0, 1] is a linear range (like volume) while [1, 1.1] lets you boost the reverb mix by 10 dB.");

			public GUIContent dopplerLevelLabel = new GUIContent("Doppler Level", "Specifies how much the pitch is changed based on the relative velocity between AudioListener and AudioSource.");

			public GUIContent spreadLabel = new GUIContent("Spread", "Sets the spread of a 3d sound in speaker space");

			public GUIContent outputMixerGroupLabel = new GUIContent("Output", "Set whether the sound should play through an Audio Mixer first or directly to the Audio Listener");

			public GUIContent volumeLabel = new GUIContent("Volume", "Sets the overall volume of the sound.");

			public GUIContent pitchLabel = new GUIContent("Pitch", "Sets the frequency of the sound. Use this to slow down or speed up the sound.");

			public GUIContent priorityLabel = new GUIContent("Priority", "Sets the priority of the source. Note that a sound with a larger priority value will more likely be stolen by sounds with smaller priority values.");

			public GUIContent spatializeLabel = new GUIContent("Spatialize", "Enables or disables custom spatialization for the AudioSource.");

			public GUIContent spatializePostEffectsLabel = new GUIContent("Spatialize Post Effects", "Determines if the custom spatializer is applied before or after the effect filters attached to the AudioSource. This flag only has an effect if the spatialize flag is enabled on the AudioSource.");

			public GUIContent priorityLeftLabel = new GUIContent("High");

			public GUIContent priorityRightLabel = new GUIContent("Low");

			public GUIContent spatialLeftLabel = new GUIContent("2D");

			public GUIContent spatialRightLabel = new GUIContent("3D");

			public GUIContent panLeftLabel = new GUIContent("Left");

			public GUIContent panRightLabel = new GUIContent("Right");
		}

		private SerializedProperty m_AudioClip;

		private SerializedProperty m_PlayOnAwake;

		private SerializedProperty m_Volume;

		private SerializedProperty m_Pitch;

		private SerializedProperty m_Loop;

		private SerializedProperty m_Mute;

		private SerializedProperty m_Spatialize;

		private SerializedProperty m_SpatializePostEffects;

		private SerializedProperty m_Priority;

		private SerializedProperty m_PanLevel;

		private SerializedProperty m_DopplerLevel;

		private SerializedProperty m_MinDistance;

		private SerializedProperty m_MaxDistance;

		private SerializedProperty m_Pan2D;

		private SerializedProperty m_RolloffMode;

		private SerializedProperty m_BypassEffects;

		private SerializedProperty m_BypassListenerEffects;

		private SerializedProperty m_BypassReverbZones;

		private SerializedProperty m_OutputAudioMixerGroup;

		private SerializedObject m_LowpassObject;

		private AudioSourceInspector.AudioCurveWrapper[] m_AudioCurves;

		private bool m_RefreshCurveEditor = false;

		private CurveEditor m_CurveEditor = null;

		private Vector3 m_LastSourcePosition;

		private Vector3 m_LastListenerPosition;

		private const int kRolloffCurveID = 0;

		private const int kSpatialBlendCurveID = 1;

		private const int kSpreadCurveID = 2;

		private const int kLowPassCurveID = 3;

		private const int kReverbZoneMixCurveID = 4;

		internal const float kMaxCutoffFrequency = 22000f;

		private const float EPSILON = 0.0001f;

		private static CurveEditorSettings m_CurveEditorSettings = new CurveEditorSettings();

		internal static Color kRolloffCurveColor = new Color(0.9f, 0.3f, 0.2f, 1f);

		internal static Color kSpatialCurveColor = new Color(0.25f, 0.7f, 0.2f, 1f);

		internal static Color kSpreadCurveColor = new Color(0.25f, 0.55f, 0.95f, 1f);

		internal static Color kLowPassCurveColor = new Color(0.8f, 0.25f, 0.9f, 1f);

		internal static Color kReverbZoneMixCurveColor = new Color(0.7f, 0.7f, 0.2f, 1f);

		internal bool[] m_SelectedCurves = new bool[0];

		private bool m_Expanded3D = false;

		private static AudioSourceInspector.Styles ms_Styles;

		[CompilerGenerated]
		private static TargetChoiceHandler.TargetChoiceMenuFunction <>f__mg$cache0;

		private Vector3 GetSourcePos(UnityEngine.Object target)
		{
			AudioSource audioSource = (AudioSource)target;
			Vector3 result;
			if (audioSource == null)
			{
				result = new Vector3(0f, 0f, 0f);
			}
			else
			{
				result = audioSource.transform.position;
			}
			return result;
		}

		private void OnEnable()
		{
			this.m_AudioClip = base.serializedObject.FindProperty("m_audioClip");
			this.m_PlayOnAwake = base.serializedObject.FindProperty("m_PlayOnAwake");
			this.m_Volume = base.serializedObject.FindProperty("m_Volume");
			this.m_Pitch = base.serializedObject.FindProperty("m_Pitch");
			this.m_Loop = base.serializedObject.FindProperty("Loop");
			this.m_Mute = base.serializedObject.FindProperty("Mute");
			this.m_Spatialize = base.serializedObject.FindProperty("Spatialize");
			this.m_SpatializePostEffects = base.serializedObject.FindProperty("SpatializePostEffects");
			this.m_Priority = base.serializedObject.FindProperty("Priority");
			this.m_DopplerLevel = base.serializedObject.FindProperty("DopplerLevel");
			this.m_MinDistance = base.serializedObject.FindProperty("MinDistance");
			this.m_MaxDistance = base.serializedObject.FindProperty("MaxDistance");
			this.m_Pan2D = base.serializedObject.FindProperty("Pan2D");
			this.m_RolloffMode = base.serializedObject.FindProperty("rolloffMode");
			this.m_BypassEffects = base.serializedObject.FindProperty("BypassEffects");
			this.m_BypassListenerEffects = base.serializedObject.FindProperty("BypassListenerEffects");
			this.m_BypassReverbZones = base.serializedObject.FindProperty("BypassReverbZones");
			this.m_OutputAudioMixerGroup = base.serializedObject.FindProperty("OutputAudioMixerGroup");
			this.m_AudioCurves = new AudioSourceInspector.AudioCurveWrapper[]
			{
				new AudioSourceInspector.AudioCurveWrapper(AudioSourceInspector.AudioCurveType.Volume, "Volume", 0, AudioSourceInspector.kRolloffCurveColor, base.serializedObject.FindProperty("rolloffCustomCurve"), 0f, 1f),
				new AudioSourceInspector.AudioCurveWrapper(AudioSourceInspector.AudioCurveType.SpatialBlend, "Spatial Blend", 1, AudioSourceInspector.kSpatialCurveColor, base.serializedObject.FindProperty("panLevelCustomCurve"), 0f, 1f),
				new AudioSourceInspector.AudioCurveWrapper(AudioSourceInspector.AudioCurveType.Spread, "Spread", 2, AudioSourceInspector.kSpreadCurveColor, base.serializedObject.FindProperty("spreadCustomCurve"), 0f, 1f),
				new AudioSourceInspector.AudioCurveWrapper(AudioSourceInspector.AudioCurveType.Lowpass, "Low-Pass", 3, AudioSourceInspector.kLowPassCurveColor, null, 0f, 1f),
				new AudioSourceInspector.AudioCurveWrapper(AudioSourceInspector.AudioCurveType.ReverbZoneMix, "Reverb Zone Mix", 4, AudioSourceInspector.kReverbZoneMixCurveColor, base.serializedObject.FindProperty("reverbZoneMixCustomCurve"), 0f, 1.1f)
			};
			AudioSourceInspector.m_CurveEditorSettings.hRangeMin = 0f;
			AudioSourceInspector.m_CurveEditorSettings.vRangeMin = 0f;
			AudioSourceInspector.m_CurveEditorSettings.vRangeMax = 1.1f;
			AudioSourceInspector.m_CurveEditorSettings.hRangeMax = 1f;
			AudioSourceInspector.m_CurveEditorSettings.vSlider = false;
			AudioSourceInspector.m_CurveEditorSettings.hSlider = false;
			TickStyle tickStyle = new TickStyle();
			tickStyle.tickColor.color = new Color(0f, 0f, 0f, 0.15f);
			tickStyle.distLabel = 30;
			AudioSourceInspector.m_CurveEditorSettings.hTickStyle = tickStyle;
			TickStyle tickStyle2 = new TickStyle();
			tickStyle2.tickColor.color = new Color(0f, 0f, 0f, 0.15f);
			tickStyle2.distLabel = 20;
			AudioSourceInspector.m_CurveEditorSettings.vTickStyle = tickStyle2;
			AudioSourceInspector.m_CurveEditorSettings.undoRedoSelection = true;
			this.m_CurveEditor = new CurveEditor(new Rect(0f, 0f, 1000f, 100f), new CurveWrapper[0], false);
			this.m_CurveEditor.settings = AudioSourceInspector.m_CurveEditorSettings;
			this.m_CurveEditor.margin = 25f;
			this.m_CurveEditor.SetShownHRangeInsideMargins(0f, 1f);
			this.m_CurveEditor.SetShownVRangeInsideMargins(0f, 1.1f);
			this.m_CurveEditor.ignoreScrollWheelUntilClicked = true;
			this.m_LastSourcePosition = this.GetSourcePos(base.target);
			this.m_LastListenerPosition = AudioUtil.GetListenerPos();
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.m_Expanded3D = EditorPrefs.GetBool("AudioSourceExpanded3D", this.m_Expanded3D);
		}

		private void OnDisable()
		{
			this.m_CurveEditor.OnDisable();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			EditorPrefs.SetBool("AudioSourceExpanded3D", this.m_Expanded3D);
		}

		private CurveWrapper[] GetCurveWrapperArray()
		{
			List<CurveWrapper> list = new List<CurveWrapper>();
			AudioSourceInspector.AudioCurveWrapper[] audioCurves = this.m_AudioCurves;
			for (int i = 0; i < audioCurves.Length; i++)
			{
				AudioSourceInspector.AudioCurveWrapper audioCurveWrapper = audioCurves[i];
				if (audioCurveWrapper.curveProp != null)
				{
					AnimationCurve animationCurve = audioCurveWrapper.curveProp.animationCurveValue;
					bool flag;
					if (audioCurveWrapper.type == AudioSourceInspector.AudioCurveType.Volume)
					{
						AudioRolloffMode enumValueIndex = (AudioRolloffMode)this.m_RolloffMode.enumValueIndex;
						if (this.m_RolloffMode.hasMultipleDifferentValues)
						{
							flag = false;
						}
						else if (enumValueIndex == AudioRolloffMode.Custom)
						{
							flag = !audioCurveWrapper.curveProp.hasMultipleDifferentValues;
						}
						else
						{
							flag = (!this.m_MinDistance.hasMultipleDifferentValues && !this.m_MaxDistance.hasMultipleDifferentValues);
							if (enumValueIndex == AudioRolloffMode.Linear)
							{
								animationCurve = AnimationCurve.Linear(this.m_MinDistance.floatValue / this.m_MaxDistance.floatValue, 1f, 1f, 0f);
							}
							else if (enumValueIndex == AudioRolloffMode.Logarithmic)
							{
								animationCurve = AudioSourceInspector.Logarithmic(this.m_MinDistance.floatValue / this.m_MaxDistance.floatValue, 1f, 1f);
							}
						}
					}
					else
					{
						flag = !audioCurveWrapper.curveProp.hasMultipleDifferentValues;
					}
					if (flag)
					{
						if (animationCurve.length == 0)
						{
							Debug.LogError(audioCurveWrapper.legend.text + " curve has no keys!");
						}
						else
						{
							list.Add(this.GetCurveWrapper(animationCurve, audioCurveWrapper));
						}
					}
				}
			}
			return list.ToArray();
		}

		private CurveWrapper GetCurveWrapper(AnimationCurve curve, AudioSourceInspector.AudioCurveWrapper audioCurve)
		{
			float num = EditorGUIUtility.isProSkin ? 1f : 0.9f;
			Color b = new Color(num, num, num, 1f);
			CurveWrapper curveWrapper = new CurveWrapper();
			curveWrapper.id = audioCurve.id;
			curveWrapper.groupId = -1;
			curveWrapper.color = audioCurve.color * b;
			curveWrapper.hidden = false;
			curveWrapper.readOnly = false;
			curveWrapper.renderer = new NormalCurveRenderer(curve);
			curveWrapper.renderer.SetCustomRange(0f, 1f);
			curveWrapper.getAxisUiScalarsCallback = new CurveWrapper.GetAxisScalarsCallback(this.GetAxisScalars);
			return curveWrapper;
		}

		public Vector2 GetAxisScalars()
		{
			return new Vector2(this.m_MaxDistance.floatValue, 1f);
		}

		private static float LogarithmicValue(float distance, float minDistance, float rolloffScale)
		{
			if (distance > minDistance && rolloffScale != 1f)
			{
				distance -= minDistance;
				distance *= rolloffScale;
				distance += minDistance;
			}
			if (distance < 1E-06f)
			{
				distance = 1E-06f;
			}
			return minDistance / distance;
		}

		private static AnimationCurve Logarithmic(float timeStart, float timeEnd, float logBase)
		{
			List<Keyframe> list = new List<Keyframe>();
			float num = 2f;
			timeStart = Mathf.Max(timeStart, 0.0001f);
			float value;
			float num3;
			float num4;
			for (float num2 = timeStart; num2 < timeEnd; num2 *= num)
			{
				value = AudioSourceInspector.LogarithmicValue(num2, timeStart, logBase);
				num3 = num2 / 50f;
				num4 = (AudioSourceInspector.LogarithmicValue(num2 + num3, timeStart, logBase) - AudioSourceInspector.LogarithmicValue(num2 - num3, timeStart, logBase)) / (num3 * 2f);
				list.Add(new Keyframe(num2, value, num4, num4));
			}
			value = AudioSourceInspector.LogarithmicValue(timeEnd, timeStart, logBase);
			num3 = timeEnd / 50f;
			num4 = (AudioSourceInspector.LogarithmicValue(timeEnd + num3, timeStart, logBase) - AudioSourceInspector.LogarithmicValue(timeEnd - num3, timeStart, logBase)) / (num3 * 2f);
			list.Add(new Keyframe(timeEnd, value, num4, num4));
			return new AnimationCurve(list.ToArray());
		}

		private static void InitStyles()
		{
			if (AudioSourceInspector.ms_Styles == null)
			{
				AudioSourceInspector.ms_Styles = new AudioSourceInspector.Styles();
			}
		}

		private void Update()
		{
			Vector3 sourcePos = this.GetSourcePos(base.target);
			Vector3 listenerPos = AudioUtil.GetListenerPos();
			if ((this.m_LastSourcePosition - sourcePos).sqrMagnitude > 0.0001f || (this.m_LastListenerPosition - listenerPos).sqrMagnitude > 0.0001f)
			{
				this.m_LastSourcePosition = sourcePos;
				this.m_LastListenerPosition = listenerPos;
				base.Repaint();
			}
		}

		private void UndoRedoPerformed()
		{
			this.m_RefreshCurveEditor = true;
		}

		private void HandleLowPassFilter()
		{
			AudioSourceInspector.AudioCurveWrapper audioCurveWrapper = this.m_AudioCurves[3];
			AudioLowPassFilter[] array = new AudioLowPassFilter[base.targets.Length];
			for (int i = 0; i < base.targets.Length; i++)
			{
				array[i] = ((AudioSource)base.targets[i]).GetComponent<AudioLowPassFilter>();
				if (array[i] == null)
				{
					this.m_LowpassObject = null;
					audioCurveWrapper.curveProp = null;
					return;
				}
			}
			if (audioCurveWrapper.curveProp == null)
			{
				this.m_LowpassObject = new SerializedObject(array);
				audioCurveWrapper.curveProp = this.m_LowpassObject.FindProperty("lowpassLevelCustomCurve");
				return;
			}
		}

		public override void OnInspectorGUI()
		{
			AudioSourceInspector.InitStyles();
			base.serializedObject.Update();
			if (this.m_LowpassObject != null)
			{
				this.m_LowpassObject.Update();
			}
			this.HandleLowPassFilter();
			AudioSourceInspector.AudioCurveWrapper[] audioCurves = this.m_AudioCurves;
			for (int i = 0; i < audioCurves.Length; i++)
			{
				AudioSourceInspector.AudioCurveWrapper audioCurveWrapper = audioCurves[i];
				CurveWrapper curveWrapperFromID = this.m_CurveEditor.GetCurveWrapperFromID(audioCurveWrapper.id);
				if (audioCurveWrapper.curveProp != null)
				{
					AnimationCurve animationCurveValue = audioCurveWrapper.curveProp.animationCurveValue;
					if (curveWrapperFromID == null != audioCurveWrapper.curveProp.hasMultipleDifferentValues)
					{
						this.m_RefreshCurveEditor = true;
					}
					else if (curveWrapperFromID != null)
					{
						if (curveWrapperFromID.curve.length == 0)
						{
							this.m_RefreshCurveEditor = true;
						}
						else if (animationCurveValue.length >= 1 && animationCurveValue.keys[0].value != curveWrapperFromID.curve.keys[0].value)
						{
							this.m_RefreshCurveEditor = true;
						}
					}
				}
				else if (curveWrapperFromID != null)
				{
					this.m_RefreshCurveEditor = true;
				}
			}
			this.UpdateWrappersAndLegend();
			EditorGUILayout.PropertyField(this.m_AudioClip, AudioSourceInspector.ms_Styles.audioClipLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_OutputAudioMixerGroup, AudioSourceInspector.ms_Styles.outputMixerGroupLabel, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Mute, new GUILayoutOption[0]);
			if (AudioUtil.canUseSpatializerEffect)
			{
				EditorGUILayout.PropertyField(this.m_Spatialize, AudioSourceInspector.ms_Styles.spatializeLabel, new GUILayoutOption[0]);
				using (new EditorGUI.DisabledScope(!this.m_Spatialize.boolValue))
				{
					EditorGUILayout.PropertyField(this.m_SpatializePostEffects, AudioSourceInspector.ms_Styles.spatializePostEffectsLabel, new GUILayoutOption[0]);
				}
			}
			EditorGUILayout.PropertyField(this.m_BypassEffects, new GUILayoutOption[0]);
			if (base.targets.Any((UnityEngine.Object t) => (t as AudioSource).outputAudioMixerGroup != null))
			{
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUILayout.PropertyField(this.m_BypassListenerEffects, new GUILayoutOption[0]);
				}
			}
			else
			{
				EditorGUILayout.PropertyField(this.m_BypassListenerEffects, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_BypassReverbZones, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_PlayOnAwake, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Loop, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUIUtility.sliderLabels.SetLabels(AudioSourceInspector.ms_Styles.priorityLeftLabel, AudioSourceInspector.ms_Styles.priorityRightLabel);
			EditorGUILayout.IntSlider(this.m_Priority, 0, 256, AudioSourceInspector.ms_Styles.priorityLabel, new GUILayoutOption[0]);
			EditorGUIUtility.sliderLabels.SetLabels(null, null);
			EditorGUILayout.Space();
			EditorGUILayout.Slider(this.m_Volume, 0f, 1f, AudioSourceInspector.ms_Styles.volumeLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.Slider(this.m_Pitch, -3f, 3f, AudioSourceInspector.ms_Styles.pitchLabel, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUIUtility.sliderLabels.SetLabels(AudioSourceInspector.ms_Styles.panLeftLabel, AudioSourceInspector.ms_Styles.panRightLabel);
			EditorGUILayout.Slider(this.m_Pan2D, -1f, 1f, AudioSourceInspector.ms_Styles.panStereoLabel, new GUILayoutOption[0]);
			EditorGUIUtility.sliderLabels.SetLabels(null, null);
			EditorGUILayout.Space();
			EditorGUIUtility.sliderLabels.SetLabels(AudioSourceInspector.ms_Styles.spatialLeftLabel, AudioSourceInspector.ms_Styles.spatialRightLabel);
			AudioSourceInspector.AnimProp(AudioSourceInspector.ms_Styles.spatialBlendLabel, this.m_AudioCurves[1].curveProp, 0f, 1f, false);
			EditorGUIUtility.sliderLabels.SetLabels(null, null);
			EditorGUILayout.Space();
			AudioSourceInspector.AnimProp(AudioSourceInspector.ms_Styles.reverbZoneMixLabel, this.m_AudioCurves[4].curveProp, 0f, 1.1f, false);
			EditorGUILayout.Space();
			this.m_Expanded3D = EditorGUILayout.Foldout(this.m_Expanded3D, "3D Sound Settings", true);
			if (this.m_Expanded3D)
			{
				EditorGUI.indentLevel++;
				this.Audio3DGUI();
				EditorGUI.indentLevel--;
			}
			base.serializedObject.ApplyModifiedProperties();
			if (this.m_LowpassObject != null)
			{
				this.m_LowpassObject.ApplyModifiedProperties();
			}
		}

		private static void SetRolloffToTarget(SerializedProperty property, UnityEngine.Object target)
		{
			property.SetToValueOfTarget(target);
			property.serializedObject.FindProperty("rolloffMode").SetToValueOfTarget(target);
			property.serializedObject.ApplyModifiedProperties();
			EditorUtility.ForceReloadInspectors();
		}

		private void Audio3DGUI()
		{
			EditorGUILayout.Slider(this.m_DopplerLevel, 0f, 5f, AudioSourceInspector.ms_Styles.dopplerLevelLabel, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			AudioSourceInspector.AnimProp(AudioSourceInspector.ms_Styles.spreadLabel, this.m_AudioCurves[2].curveProp, 0f, 360f, true);
			if (this.m_RolloffMode.hasMultipleDifferentValues || (this.m_RolloffMode.enumValueIndex == 2 && this.m_AudioCurves[0].curveProp.hasMultipleDifferentValues))
			{
				SerializedProperty arg_C5_0 = this.m_AudioCurves[0].curveProp;
				GUIContent arg_C5_1 = AudioSourceInspector.ms_Styles.rolloffLabel;
				if (AudioSourceInspector.<>f__mg$cache0 == null)
				{
					AudioSourceInspector.<>f__mg$cache0 = new TargetChoiceHandler.TargetChoiceMenuFunction(AudioSourceInspector.SetRolloffToTarget);
				}
				EditorGUILayout.TargetChoiceField(arg_C5_0, arg_C5_1, AudioSourceInspector.<>f__mg$cache0, new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.PropertyField(this.m_RolloffMode, AudioSourceInspector.ms_Styles.rolloffLabel, new GUILayoutOption[0]);
				if (this.m_RolloffMode.enumValueIndex != 2)
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(this.m_MinDistance, new GUILayoutOption[0]);
					if (EditorGUI.EndChangeCheck())
					{
						this.m_MinDistance.floatValue = Mathf.Clamp(this.m_MinDistance.floatValue, 0f, this.m_MaxDistance.floatValue / 1.01f);
					}
				}
				else
				{
					using (new EditorGUI.DisabledScope(true))
					{
						EditorGUILayout.LabelField(this.m_MinDistance.displayName, AudioSourceInspector.ms_Styles.controlledByCurveLabel, new GUILayoutOption[0]);
					}
				}
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_MaxDistance, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_MaxDistance.floatValue = Mathf.Min(Mathf.Max(Mathf.Max(this.m_MaxDistance.floatValue, 0.01f), this.m_MinDistance.floatValue * 1.01f), 1000000f);
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_RefreshCurveEditor = true;
			}
			Rect aspectRect = GUILayoutUtility.GetAspectRect(1.333f, GUI.skin.textField);
			aspectRect.xMin += EditorGUI.indent;
			if (Event.current.type != EventType.Layout && Event.current.type != EventType.Used)
			{
				this.m_CurveEditor.rect = new Rect(aspectRect.x, aspectRect.y, aspectRect.width, aspectRect.height);
			}
			this.UpdateWrappersAndLegend();
			GUI.Label(this.m_CurveEditor.drawRect, GUIContent.none, "TextField");
			this.m_CurveEditor.hRangeLocked = Event.current.shift;
			this.m_CurveEditor.vRangeLocked = EditorGUI.actionKey;
			this.m_CurveEditor.OnGUI();
			if (base.targets.Length == 1)
			{
				AudioSource audioSource = (AudioSource)base.target;
				AudioListener x = (AudioListener)UnityEngine.Object.FindObjectOfType(typeof(AudioListener));
				if (x != null)
				{
					float magnitude = (AudioUtil.GetListenerPos() - audioSource.transform.position).magnitude;
					this.DrawLabel("Listener", magnitude, aspectRect);
				}
			}
			this.DrawLegend();
			AudioSourceInspector.AudioCurveWrapper[] audioCurves = this.m_AudioCurves;
			for (int i = 0; i < audioCurves.Length; i++)
			{
				AudioSourceInspector.AudioCurveWrapper audioCurveWrapper = audioCurves[i];
				if (this.m_CurveEditor.GetCurveWrapperFromID(audioCurveWrapper.id) != null && this.m_CurveEditor.GetCurveWrapperFromID(audioCurveWrapper.id).changed)
				{
					AnimationCurve curve = this.m_CurveEditor.GetCurveWrapperFromID(audioCurveWrapper.id).curve;
					if (curve.length > 0)
					{
						audioCurveWrapper.curveProp.animationCurveValue = curve;
						this.m_CurveEditor.GetCurveWrapperFromID(audioCurveWrapper.id).changed = false;
						if (audioCurveWrapper.type == AudioSourceInspector.AudioCurveType.Volume)
						{
							this.m_RolloffMode.enumValueIndex = 2;
						}
					}
				}
			}
		}

		private void UpdateWrappersAndLegend()
		{
			if (this.m_RefreshCurveEditor)
			{
				this.m_CurveEditor.animationCurves = this.GetCurveWrapperArray();
				this.SyncShownCurvesToLegend(this.GetShownAudioCurves());
				this.m_RefreshCurveEditor = false;
			}
		}

		private void DrawLegend()
		{
			List<Rect> list = new List<Rect>();
			List<AudioSourceInspector.AudioCurveWrapper> shownAudioCurves = this.GetShownAudioCurves();
			Rect rect = GUILayoutUtility.GetRect(10f, 20f);
			rect.x += 4f + EditorGUI.indent;
			rect.width -= 8f + EditorGUI.indent;
			int num = Mathf.Min(75, Mathf.FloorToInt(rect.width / (float)shownAudioCurves.Count));
			for (int i = 0; i < shownAudioCurves.Count; i++)
			{
				list.Add(new Rect(rect.x + (float)(num * i), rect.y, (float)num, rect.height));
			}
			bool flag = false;
			if (shownAudioCurves.Count != this.m_SelectedCurves.Length)
			{
				this.m_SelectedCurves = new bool[shownAudioCurves.Count];
				flag = true;
			}
			if (EditorGUIExt.DragSelection(list.ToArray(), ref this.m_SelectedCurves, GUIStyle.none) || flag)
			{
				bool flag2 = false;
				for (int j = 0; j < shownAudioCurves.Count; j++)
				{
					if (this.m_SelectedCurves[j])
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					for (int k = 0; k < shownAudioCurves.Count; k++)
					{
						this.m_SelectedCurves[k] = true;
					}
				}
				this.SyncShownCurvesToLegend(shownAudioCurves);
			}
			for (int l = 0; l < shownAudioCurves.Count; l++)
			{
				EditorGUI.DrawLegend(list[l], shownAudioCurves[l].color, shownAudioCurves[l].legend.text, this.m_SelectedCurves[l]);
				if (shownAudioCurves[l].curveProp.hasMultipleDifferentValues)
				{
					GUI.Button(new Rect(list[l].x, list[l].y + 20f, list[l].width, 20f), "Different");
				}
			}
		}

		private List<AudioSourceInspector.AudioCurveWrapper> GetShownAudioCurves()
		{
			return (from f in this.m_AudioCurves
			where this.m_CurveEditor.GetCurveWrapperFromID(f.id) != null
			select f).ToList<AudioSourceInspector.AudioCurveWrapper>();
		}

		private void SyncShownCurvesToLegend(List<AudioSourceInspector.AudioCurveWrapper> curves)
		{
			if (curves.Count == this.m_SelectedCurves.Length)
			{
				for (int i = 0; i < curves.Count; i++)
				{
					this.m_CurveEditor.GetCurveWrapperFromID(curves[i].id).hidden = !this.m_SelectedCurves[i];
				}
				this.m_CurveEditor.animationCurves = this.m_CurveEditor.animationCurves;
			}
		}

		private void DrawLabel(string label, float value, Rect r)
		{
			Vector2 vector = AudioSourceInspector.ms_Styles.labelStyle.CalcSize(new GUIContent(label));
			vector.x += 2f;
			Vector2 vector2 = this.m_CurveEditor.DrawingToViewTransformPoint(new Vector2(value / this.m_MaxDistance.floatValue, 0f));
			Vector2 vector3 = this.m_CurveEditor.DrawingToViewTransformPoint(new Vector2(value / this.m_MaxDistance.floatValue, 1f));
			GUI.BeginGroup(r);
			Color color = Handles.color;
			Handles.color = new Color(1f, 0f, 0f, 0.3f);
			Handles.DrawLine(new Vector3(vector2.x, vector2.y, 0f), new Vector3(vector3.x, vector3.y, 0f));
			Handles.DrawLine(new Vector3(vector2.x + 1f, vector2.y, 0f), new Vector3(vector3.x + 1f, vector3.y, 0f));
			Handles.color = color;
			GUI.Label(new Rect(Mathf.Floor(vector3.x - vector.x / 2f), 2f, vector.x, 15f), label, AudioSourceInspector.ms_Styles.labelStyle);
			GUI.EndGroup();
		}

		internal static void AnimProp(GUIContent label, SerializedProperty prop, float min, float max, bool useNormalizedValue)
		{
			AudioSourceInspector.InitStyles();
			if (prop.hasMultipleDifferentValues)
			{
				EditorGUILayout.TargetChoiceField(prop, label, new GUILayoutOption[0]);
			}
			else
			{
				AnimationCurve animationCurveValue = prop.animationCurveValue;
				if (animationCurveValue == null)
				{
					Debug.LogError(label.text + " curve is null!");
				}
				else if (animationCurveValue.length == 0)
				{
					Debug.LogError(label.text + " curve has no keys!");
				}
				else
				{
					if (animationCurveValue.length != 1)
					{
						using (new EditorGUI.DisabledScope(true))
						{
							EditorGUILayout.LabelField(label.text, AudioSourceInspector.ms_Styles.controlledByCurveLabel, new GUILayoutOption[0]);
						}
					}
					else
					{
						float num = (!useNormalizedValue) ? animationCurveValue.keys[0].value : Mathf.Lerp(min, max, animationCurveValue.keys[0].value);
						num = MathUtils.DiscardLeastSignificantDecimal(num);
						EditorGUI.BeginChangeCheck();
						if (max > min)
						{
							num = EditorGUILayout.Slider(label, num, min, max, new GUILayoutOption[0]);
						}
						else
						{
							num = EditorGUILayout.Slider(label, num, max, min, new GUILayoutOption[0]);
						}
						if (EditorGUI.EndChangeCheck())
						{
							Keyframe key = animationCurveValue.keys[0];
							key.time = 0f;
							key.value = ((!useNormalizedValue) ? num : Mathf.InverseLerp(min, max, num));
							animationCurveValue.MoveKey(0, key);
						}
					}
					prop.animationCurveValue = animationCurveValue;
				}
			}
		}

		private void OnSceneGUI()
		{
			AudioSource audioSource = (AudioSource)base.target;
			Color color = Handles.color;
			if (audioSource.enabled)
			{
				Handles.color = new Color(0.5f, 0.7f, 1f, 0.5f);
			}
			else
			{
				Handles.color = new Color(0.3f, 0.4f, 0.6f, 0.5f);
			}
			Vector3 position = audioSource.transform.position;
			EditorGUI.BeginChangeCheck();
			float minDistance = Handles.RadiusHandle(Quaternion.identity, position, audioSource.minDistance, true);
			float maxDistance = Handles.RadiusHandle(Quaternion.identity, position, audioSource.maxDistance, true);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(audioSource, "AudioSource Distance");
				audioSource.minDistance = minDistance;
				audioSource.maxDistance = maxDistance;
				this.m_RefreshCurveEditor = true;
			}
			Handles.color = color;
		}
	}
}
