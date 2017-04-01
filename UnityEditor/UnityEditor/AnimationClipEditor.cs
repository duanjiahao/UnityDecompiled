using System;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(AnimationClip))]
	internal class AnimationClipEditor : Editor
	{
		private static class Styles
		{
			public static GUIContent StartFrame = EditorGUIUtility.TextContent("Start|Start frame of the clip.");

			public static GUIContent EndFrame = EditorGUIUtility.TextContent("End|End frame of the clip.");

			public static GUIContent HasAdditiveReferencePose = EditorGUIUtility.TextContent("Additive Reference Pose|Enable to define the additive reference pose frame.");

			public static GUIContent AdditiveReferencePoseFrame = EditorGUIUtility.TextContent("Pose Frame|Pose Frame.");

			public static GUIContent LoopTime = EditorGUIUtility.TextContent("Loop Time|Enable to make the animation plays through and then restarts when the end is reached.");

			public static GUIContent LoopPose = EditorGUIUtility.TextContent("Loop Pose|Enable to make the animation loop seamlessly.");

			public static GUIContent LoopCycleOffset = EditorGUIUtility.TextContent("Cycle Offset|Offset to the cycle of a looping animation, if we want to start it at a different time.");

			public static GUIContent MotionCurves = EditorGUIUtility.TextContent("Root Motion is driven by curves");

			public static GUIContent BakeIntoPoseOrientation = EditorGUIUtility.TextContent("Bake Into Pose|Enable to make root rotation be baked into the movement of the bones. Disable to make root rotation be stored as root motion.");

			public static GUIContent OrientationOffsetY = EditorGUIUtility.TextContent("Offset|Offset to the root rotation (in degrees).");

			public static GUIContent BasedUponOrientation = EditorGUIUtility.TextContent("Based Upon|What the root rotation is based upon.");

			public static GUIContent BasedUponStartOrientation = EditorGUIUtility.TextContent("Based Upon (at Start)|What the root rotation is based upon.");

			public static GUIContent[] BasedUponRotationHumanOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Original|Keeps the rotation as it is authored in the source file."),
				EditorGUIUtility.TextContent("Body Orientation|Keeps the upper body pointing forward.")
			};

			public static GUIContent[] BasedUponRotationOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Original|Keeps the rotation as it is authored in the source file."),
				EditorGUIUtility.TextContent("Root Node Rotation|Keeps the upper body pointing forward.")
			};

			public static GUIContent BakeIntoPosePositionY = EditorGUIUtility.TextContent("Bake Into Pose|Enable to make vertical root motion be baked into the movement of the bones. Disable to make vertical root motion be stored as root motion.");

			public static GUIContent PositionOffsetY = EditorGUIUtility.TextContent("Offset|Offset to the vertical root position.");

			public static GUIContent BasedUponPositionY = EditorGUIUtility.TextContent("Based Upon|What the vertical root position is based upon.");

			public static GUIContent BasedUponStartPositionY = EditorGUIUtility.TextContent("Based Upon (at Start)|What the vertical root position is based upon.");

			public static GUIContent[] BasedUponPositionYHumanOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Original|Keeps the vertical position as it is authored in the source file."),
				EditorGUIUtility.TextContent("Center of Mass|Keeps the center of mass aligned with root transform position."),
				EditorGUIUtility.TextContent("Feet|Keeps the feet aligned with the root transform position.")
			};

			public static GUIContent[] BasedUponPositionYOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Original|Keeps the vertical position as it is authored in the source file."),
				EditorGUIUtility.TextContent("Root Node Position")
			};

			public static GUIContent BakeIntoPosePositionXZ = EditorGUIUtility.TextContent("Bake Into Pose|Enable to make horizontal root motion be baked into the movement of the bones. Disable to make horizontal root motion be stored as root motion.");

			public static GUIContent BasedUponPositionXZ = EditorGUIUtility.TextContent("Based Upon|What the horizontal root position is based upon.");

			public static GUIContent BasedUponStartPositionXZ = EditorGUIUtility.TextContent("Based Upon (at Start)|What the horizontal root position is based upon.");

			public static GUIContent[] BasedUponPositionXZHumanOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Original|Keeps the horizontal position as it is authored in the source file."),
				EditorGUIUtility.TextContent("Center of Mass|Keeps the center of mass aligned with root transform position.")
			};

			public static GUIContent[] BasedUponPositionXZOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Original|Keeps the horizontal position as it is authored in the source file."),
				EditorGUIUtility.TextContent("Root Node Position")
			};

			public static GUIContent Mirror = EditorGUIUtility.TextContent("Mirror|Mirror left and right in this clip.");

			public static GUIContent Curves = EditorGUIUtility.TextContent("Curves|Parameter-related curves.");

			public static GUIContent AddEventContent = EditorGUIUtility.IconContent("Animation.AddEvent", "|Add Event.");

			public static GUIContent GreenLightIcon = EditorGUIUtility.IconContent("lightMeter/greenLight");

			public static GUIContent LightRimIcon = EditorGUIUtility.IconContent("lightMeter/lightRim");

			public static GUIContent OrangeLightIcon = EditorGUIUtility.IconContent("lightMeter/orangeLight");

			public static GUIContent RedLightIcon = EditorGUIUtility.IconContent("lightMeter/redLight");

			public static GUIContent PrevKeyContent = EditorGUIUtility.IconContent("Animation.PrevKey", "|Go to previous key frame.");

			public static GUIContent NextKeyContent = EditorGUIUtility.IconContent("Animation.NextKey", "|Go to next key frame.");

			public static GUIContent AddKeyframeContent = EditorGUIUtility.IconContent("Animation.AddKeyframe", "|Add Keyframe.");
		}

		private static string s_LoopMeterStr = "LoopMeter";

		private static int s_LoopMeterHint = AnimationClipEditor.s_LoopMeterStr.GetHashCode();

		private static string s_LoopOrientationMeterStr = "LoopOrientationMeter";

		private static int s_LoopOrientationMeterHint = AnimationClipEditor.s_LoopOrientationMeterStr.GetHashCode();

		private static string s_LoopPositionYMeterStr = "LoopPostionYMeter";

		private static int s_LoopPositionYMeterHint = AnimationClipEditor.s_LoopPositionYMeterStr.GetHashCode();

		private static string s_LoopPositionXZMeterStr = "LoopPostionXZMeter";

		private static int s_LoopPositionXZMeterHint = AnimationClipEditor.s_LoopPositionXZMeterStr.GetHashCode();

		public static float s_EventTimelineMax = 1.05f;

		private AvatarMask m_Mask = null;

		private AnimationClipInfoProperties m_ClipInfo = null;

		private AnimationClip m_Clip = null;

		private UnityEditor.Animations.AnimatorController m_Controller = null;

		private AnimatorStateMachine m_StateMachine;

		private AnimatorState m_State;

		private AvatarPreview m_AvatarPreview = null;

		private TimeArea m_TimeArea;

		private TimeArea m_EventTimeArea;

		private bool m_DraggingRange = false;

		private bool m_DraggingRangeBegin = false;

		private bool m_DraggingRangeEnd = false;

		private float m_DraggingStartFrame = 0f;

		private float m_DraggingStopFrame = 0f;

		private float m_DraggingAdditivePoseFrame = 0f;

		private bool m_LoopTime = false;

		private bool m_LoopBlend = false;

		private bool m_LoopBlendOrientation = false;

		private bool m_LoopBlendPositionY = false;

		private bool m_LoopBlendPositionXZ = false;

		private float m_StartFrame = 0f;

		private float m_StopFrame = 1f;

		private float m_AdditivePoseFrame = 0f;

		private float m_InitialClipLength = 0f;

		private static bool m_ShowCurves = false;

		private EventManipulationHandler m_EventManipulationHandler;

		private static bool m_ShowEvents = false;

		private bool m_NeedsToGenerateClipInfo = false;

		private const int kSamplesPerSecond = 60;

		private const int kPose = 0;

		private const int kRotation = 1;

		private const int kHeight = 2;

		private const int kPosition = 3;

		private Vector2[][][] m_QualityCurves = new Vector2[4][][];

		private bool m_DirtyQualityCurves = false;

		public AvatarMask mask
		{
			get
			{
				return this.m_Mask;
			}
			set
			{
				this.m_Mask = value;
			}
		}

		public string[] takeNames
		{
			get;
			set;
		}

		public int takeIndex
		{
			get;
			set;
		}

		public bool needsToGenerateClipInfo
		{
			get
			{
				return this.m_NeedsToGenerateClipInfo;
			}
			set
			{
				this.m_NeedsToGenerateClipInfo = value;
			}
		}

		internal static void EditWithImporter(AnimationClip clip)
		{
			ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;
			if (modelImporter)
			{
				Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(modelImporter.assetPath);
				ModelImporterEditor modelImporterEditor = Editor.CreateEditor(modelImporter) as ModelImporterEditor;
				EditorPrefs.SetInt(modelImporterEditor.GetType().Name + "ActiveEditorIndex", 2);
				int value = 0;
				ModelImporterClipAnimation[] clipAnimations = modelImporter.clipAnimations;
				for (int i = 0; i < clipAnimations.Length; i++)
				{
					if (clipAnimations[i].name == clip.name)
					{
						value = i;
					}
				}
				EditorPrefs.SetInt("ModelImporterClipEditor.ActiveClipIndex", value);
			}
		}

		private void UpdateEventsPopupClipInfo(AnimationClipInfoProperties info)
		{
			if (this.m_EventManipulationHandler != null)
			{
				this.m_EventManipulationHandler.UpdateEvents(info);
			}
		}

		public void ShowRange(AnimationClipInfoProperties info)
		{
			this.UpdateEventsPopupClipInfo(info);
			this.m_ClipInfo = info;
			info.AssignToPreviewClip(this.m_Clip);
		}

		private void InitController()
		{
			if (!this.m_Clip.legacy)
			{
				if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
				{
					if (this.m_Controller == null)
					{
						this.m_Controller = new UnityEditor.Animations.AnimatorController();
						this.m_Controller.pushUndo = false;
						this.m_Controller.hideFlags = HideFlags.HideAndDontSave;
						this.m_Controller.AddLayer("preview");
						this.m_StateMachine = this.m_Controller.layers[0].stateMachine;
						this.m_StateMachine.pushUndo = false;
						this.m_StateMachine.hideFlags = HideFlags.HideAndDontSave;
						if (this.mask != null)
						{
							UnityEditor.Animations.AnimatorControllerLayer[] layers = this.m_Controller.layers;
							layers[0].avatarMask = this.mask;
							this.m_Controller.layers = layers;
						}
					}
					if (this.m_State == null)
					{
						this.m_State = this.m_StateMachine.AddState("preview");
						this.m_State.pushUndo = false;
						UnityEditor.Animations.AnimatorControllerLayer[] layers2 = this.m_Controller.layers;
						this.m_State.motion = this.m_Clip;
						this.m_Controller.layers = layers2;
						this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
						this.m_State.hideFlags = HideFlags.HideAndDontSave;
					}
					UnityEditor.Animations.AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
					if (UnityEditor.Animations.AnimatorController.GetEffectiveAnimatorController(this.m_AvatarPreview.Animator) != this.m_Controller)
					{
						UnityEditor.Animations.AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
					}
				}
			}
		}

		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			bool flag = AssetPreview.IsLoadingAssetPreview(base.target.GetInstanceID());
			Texture2D texture2D = AssetPreview.GetAssetPreview(base.target);
			if (!texture2D)
			{
				if (flag)
				{
					base.Repaint();
				}
				texture2D = AssetPreview.GetMiniThumbnail(base.target);
			}
			GUI.DrawTexture(iconRect, texture2D);
		}

		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			if (this.m_ClipInfo != null)
			{
				this.m_ClipInfo.name = EditorGUI.DelayedTextField(titleRect, this.m_ClipInfo.name, EditorStyles.textField);
			}
			else
			{
				base.OnHeaderTitleGUI(titleRect, header);
			}
		}

		internal override void OnHeaderControlsGUI()
		{
			if (this.m_ClipInfo != null && this.takeNames != null && this.takeNames.Length > 1)
			{
				EditorGUIUtility.labelWidth = 80f;
				this.takeIndex = EditorGUILayout.Popup("Source Take", this.takeIndex, this.takeNames, new GUILayoutOption[0]);
			}
			else
			{
				base.OnHeaderControlsGUI();
				ModelImporter x = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(base.target)) as ModelImporter;
				if (x != null && this.m_ClipInfo == null)
				{
					if (GUILayout.Button("Edit...", EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						AnimationClipEditor.EditWithImporter(base.target as AnimationClip);
					}
				}
			}
		}

		private void DestroyController()
		{
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				UnityEditor.Animations.AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, null);
			}
			UnityEngine.Object.DestroyImmediate(this.m_Controller);
			UnityEngine.Object.DestroyImmediate(this.m_State);
			this.m_Controller = null;
			this.m_StateMachine = null;
			this.m_State = null;
		}

		private void SetPreviewAvatar()
		{
			this.DestroyController();
			this.InitController();
		}

		private void Init()
		{
			if (this.m_AvatarPreview == null)
			{
				this.m_AvatarPreview = new AvatarPreview(null, base.target as Motion);
				this.m_AvatarPreview.OnAvatarChangeFunc = new AvatarPreview.OnAvatarChange(this.SetPreviewAvatar);
				this.m_AvatarPreview.fps = Mathf.RoundToInt((base.target as AnimationClip).frameRate);
				this.m_AvatarPreview.ShowIKOnFeetButton = (base.target as Motion).isHumanMotion;
			}
		}

		private void OnEnable()
		{
			this.m_Clip = (base.target as AnimationClip);
			this.m_InitialClipLength = this.m_Clip.stopTime - this.m_Clip.startTime;
			if (this.m_TimeArea == null)
			{
				this.m_TimeArea = new TimeArea(true);
				this.m_TimeArea.hRangeLocked = false;
				this.m_TimeArea.vRangeLocked = true;
				this.m_TimeArea.hSlider = true;
				this.m_TimeArea.vSlider = false;
				this.m_TimeArea.hRangeMin = this.m_Clip.startTime;
				this.m_TimeArea.hRangeMax = this.m_Clip.stopTime;
				this.m_TimeArea.margin = 10f;
				this.m_TimeArea.scaleWithWindow = true;
				this.m_TimeArea.SetShownHRangeInsideMargins(this.m_Clip.startTime, this.m_Clip.stopTime);
				this.m_TimeArea.hTicks.SetTickModulosForFrameRate(this.m_Clip.frameRate);
				this.m_TimeArea.ignoreScrollWheelUntilClicked = true;
			}
			if (this.m_EventTimeArea == null)
			{
				this.m_EventTimeArea = new TimeArea(true);
				this.m_EventTimeArea.hRangeLocked = true;
				this.m_EventTimeArea.vRangeLocked = true;
				this.m_EventTimeArea.hSlider = false;
				this.m_EventTimeArea.vSlider = false;
				this.m_EventTimeArea.hRangeMin = 0f;
				this.m_EventTimeArea.hRangeMax = AnimationClipEditor.s_EventTimelineMax;
				this.m_EventTimeArea.margin = 10f;
				this.m_EventTimeArea.scaleWithWindow = true;
				this.m_EventTimeArea.SetShownHRangeInsideMargins(0f, AnimationClipEditor.s_EventTimelineMax);
				this.m_EventTimeArea.hTicks.SetTickModulosForFrameRate(60f);
				this.m_EventTimeArea.ignoreScrollWheelUntilClicked = true;
			}
			if (this.m_EventManipulationHandler == null)
			{
				this.m_EventManipulationHandler = new EventManipulationHandler(this.m_EventTimeArea);
			}
		}

		private void OnDisable()
		{
			this.DestroyController();
			if (this.m_AvatarPreview != null)
			{
				this.m_AvatarPreview.OnDestroy();
			}
		}

		public override bool HasPreviewGUI()
		{
			this.Init();
			return this.m_AvatarPreview != null;
		}

		public override void OnPreviewSettings()
		{
			this.m_AvatarPreview.DoPreviewSettings();
		}

		private void CalculateQualityCurves()
		{
			for (int i = 0; i < 4; i++)
			{
				this.m_QualityCurves[i] = new Vector2[2][];
			}
			for (int j = 0; j < 2; j++)
			{
				float num = Mathf.Clamp(this.m_ClipInfo.firstFrame / this.m_Clip.frameRate, this.m_Clip.startTime, this.m_Clip.stopTime);
				float num2 = Mathf.Clamp(this.m_ClipInfo.lastFrame / this.m_Clip.frameRate, this.m_Clip.startTime, this.m_Clip.stopTime);
				float fixedTime = (j != 0) ? num : num2;
				float num3 = (j != 0) ? num : 0f;
				float num4 = (j != 0) ? this.m_Clip.stopTime : num2;
				int num5 = Mathf.FloorToInt(num3 * 60f);
				int num6 = Mathf.CeilToInt(num4 * 60f);
				this.m_QualityCurves[0][j] = new Vector2[num6 - num5 + 1];
				this.m_QualityCurves[1][j] = new Vector2[num6 - num5 + 1];
				this.m_QualityCurves[2][j] = new Vector2[num6 - num5 + 1];
				this.m_QualityCurves[3][j] = new Vector2[num6 - num5 + 1];
				QualityCurvesTime time = default(QualityCurvesTime);
				time.fixedTime = fixedTime;
				time.variableEndStart = num3;
				time.variableEndEnd = num4;
				time.q = j;
				MuscleClipEditorUtilities.CalculateQualityCurves(this.m_Clip, time, this.m_QualityCurves[0][j], this.m_QualityCurves[1][j], this.m_QualityCurves[2][j], this.m_QualityCurves[3][j]);
			}
			this.m_DirtyQualityCurves = false;
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			bool flag = Event.current.type == EventType.Repaint;
			this.InitController();
			if (flag)
			{
				this.m_AvatarPreview.timeControl.Update();
			}
			AnimationClip animationClip = base.target as AnimationClip;
			AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
			this.m_AvatarPreview.timeControl.loop = true;
			if (flag && this.m_AvatarPreview.PreviewObject != null)
			{
				if (!animationClip.legacy && this.m_AvatarPreview.Animator != null)
				{
					if (this.m_State != null)
					{
						this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
					}
					float normalizedTime = (animationClipSettings.stopTime - animationClipSettings.startTime == 0f) ? 0f : ((this.m_AvatarPreview.timeControl.currentTime - animationClipSettings.startTime) / (animationClipSettings.stopTime - animationClipSettings.startTime));
					this.m_AvatarPreview.Animator.Play(0, 0, normalizedTime);
					this.m_AvatarPreview.Animator.Update(this.m_AvatarPreview.timeControl.deltaTime);
				}
				else
				{
					animationClip.SampleAnimation(this.m_AvatarPreview.PreviewObject, this.m_AvatarPreview.timeControl.currentTime);
				}
			}
			this.m_AvatarPreview.DoAvatarPreview(r, background);
		}

		public void ClipRangeGUI(ref float startFrame, ref float stopFrame, out bool changedStart, out bool changedStop, bool showAdditivePoseFrame, ref float additivePoseframe, out bool changedAdditivePoseframe)
		{
			changedStart = false;
			changedStop = false;
			changedAdditivePoseframe = false;
			this.m_DraggingRangeBegin = false;
			this.m_DraggingRangeEnd = false;
			bool flag = startFrame + 0.01f < this.m_Clip.startTime * this.m_Clip.frameRate || startFrame - 0.01f > this.m_Clip.stopTime * this.m_Clip.frameRate || stopFrame + 0.01f < this.m_Clip.startTime * this.m_Clip.frameRate || stopFrame - 0.01f > this.m_Clip.stopTime * this.m_Clip.frameRate;
			bool flag2 = false;
			if (flag)
			{
				GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
				GUILayout.Label("The clip range is outside of the range of the source take.", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(5f);
				if (GUILayout.Button("Clamp Range", new GUILayoutOption[0]))
				{
					flag2 = true;
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
			Rect rect = GUILayoutUtility.GetRect(10f, 33f);
			GUI.Label(rect, "", "TE Toolbar");
			if (Event.current.type == EventType.Repaint)
			{
				this.m_TimeArea.rect = rect;
			}
			this.m_TimeArea.BeginViewGUI();
			this.m_TimeArea.EndViewGUI();
			rect.height -= 15f;
			int controlID = GUIUtility.GetControlID(3126789, FocusType.Passive);
			int controlID2 = GUIUtility.GetControlID(3126789, FocusType.Passive);
			int controlID3 = GUIUtility.GetControlID(3126789, FocusType.Passive);
			GUI.BeginGroup(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f));
			float num = -1f;
			rect.y = num;
			rect.x = num;
			float num2 = this.m_TimeArea.FrameToPixel(startFrame, this.m_Clip.frameRate, rect);
			float num3 = this.m_TimeArea.FrameToPixel(stopFrame, this.m_Clip.frameRate, rect);
			GUI.Label(new Rect(num2, rect.y, num3 - num2, rect.height), "", EditorStyles.selectionRect);
			this.m_TimeArea.TimeRuler(rect, this.m_Clip.frameRate);
			float num4 = this.m_TimeArea.TimeToPixel(this.m_AvatarPreview.timeControl.currentTime, rect) - 0.5f;
			Handles.color = new Color(1f, 0f, 0f, 0.5f);
			Handles.DrawLine(new Vector2(num4, rect.yMin), new Vector2(num4, rect.yMax));
			Handles.DrawLine(new Vector2(num4 + 1f, rect.yMin), new Vector2(num4 + 1f, rect.yMax));
			Handles.color = Color.white;
			using (new EditorGUI.DisabledScope(flag))
			{
				float num5 = startFrame / this.m_Clip.frameRate;
				TimeArea.TimeRulerDragMode timeRulerDragMode = this.m_TimeArea.BrowseRuler(rect, controlID, ref num5, 0f, false, "TL InPoint");
				if (timeRulerDragMode == TimeArea.TimeRulerDragMode.Cancel)
				{
					startFrame = this.m_DraggingStartFrame;
				}
				else if (timeRulerDragMode != TimeArea.TimeRulerDragMode.None)
				{
					startFrame = num5 * this.m_Clip.frameRate;
					startFrame = MathUtils.RoundBasedOnMinimumDifference(startFrame, this.m_TimeArea.PixelDeltaToTime(rect) * this.m_Clip.frameRate * 10f);
					changedStart = true;
				}
				float num6 = stopFrame / this.m_Clip.frameRate;
				TimeArea.TimeRulerDragMode timeRulerDragMode2 = this.m_TimeArea.BrowseRuler(rect, controlID2, ref num6, 0f, false, "TL OutPoint");
				if (timeRulerDragMode2 == TimeArea.TimeRulerDragMode.Cancel)
				{
					stopFrame = this.m_DraggingStopFrame;
				}
				else if (timeRulerDragMode2 != TimeArea.TimeRulerDragMode.None)
				{
					stopFrame = num6 * this.m_Clip.frameRate;
					stopFrame = MathUtils.RoundBasedOnMinimumDifference(stopFrame, this.m_TimeArea.PixelDeltaToTime(rect) * this.m_Clip.frameRate * 10f);
					changedStop = true;
				}
				if (showAdditivePoseFrame)
				{
					float num7 = additivePoseframe / this.m_Clip.frameRate;
					TimeArea.TimeRulerDragMode timeRulerDragMode3 = this.m_TimeArea.BrowseRuler(rect, controlID3, ref num7, 0f, false, "TL playhead");
					if (timeRulerDragMode3 == TimeArea.TimeRulerDragMode.Cancel)
					{
						additivePoseframe = this.m_DraggingAdditivePoseFrame;
					}
					else if (timeRulerDragMode3 != TimeArea.TimeRulerDragMode.None)
					{
						additivePoseframe = num7 * this.m_Clip.frameRate;
						additivePoseframe = MathUtils.RoundBasedOnMinimumDifference(additivePoseframe, this.m_TimeArea.PixelDeltaToTime(rect) * this.m_Clip.frameRate * 10f);
						changedAdditivePoseframe = true;
					}
				}
			}
			if (GUIUtility.hotControl == controlID)
			{
				changedStart = true;
			}
			if (GUIUtility.hotControl == controlID2)
			{
				changedStop = true;
			}
			if (GUIUtility.hotControl == controlID3)
			{
				changedAdditivePoseframe = true;
			}
			GUI.EndGroup();
			using (new EditorGUI.DisabledScope(flag))
			{
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				startFrame = EditorGUILayout.FloatField(AnimationClipEditor.Styles.StartFrame, Mathf.Round(startFrame * 1000f) / 1000f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					changedStart = true;
				}
				GUILayout.FlexibleSpace();
				EditorGUI.BeginChangeCheck();
				stopFrame = EditorGUILayout.FloatField(AnimationClipEditor.Styles.EndFrame, Mathf.Round(stopFrame * 1000f) / 1000f, new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck())
				{
					changedStop = true;
				}
				EditorGUILayout.EndHorizontal();
			}
			changedStart |= flag2;
			changedStop |= flag2;
			if (changedStart)
			{
				startFrame = Mathf.Clamp(startFrame, this.m_Clip.startTime * this.m_Clip.frameRate, Mathf.Clamp(stopFrame, this.m_Clip.startTime * this.m_Clip.frameRate, stopFrame));
			}
			if (changedStop)
			{
				stopFrame = Mathf.Clamp(stopFrame, startFrame, this.m_Clip.stopTime * this.m_Clip.frameRate);
			}
			if (changedAdditivePoseframe)
			{
				additivePoseframe = Mathf.Clamp(additivePoseframe, this.m_Clip.startTime * this.m_Clip.frameRate, this.m_Clip.stopTime * this.m_Clip.frameRate);
			}
			if (changedStart || changedStop || changedAdditivePoseframe)
			{
				if (!this.m_DraggingRange)
				{
					this.m_DraggingRangeBegin = true;
				}
				this.m_DraggingRange = true;
			}
			else if (this.m_DraggingRange && GUIUtility.hotControl == 0 && Event.current.type == EventType.Repaint)
			{
				this.m_DraggingRangeEnd = true;
				this.m_DraggingRange = false;
				this.m_DirtyQualityCurves = true;
				base.Repaint();
			}
			GUILayout.Space(10f);
		}

		private string GetStatsText()
		{
			string text = "";
			bool flag = base.targets.Length == 1 && (base.target as Motion).isHumanMotion;
			if (flag)
			{
				text += "Average Velocity: ";
				text += this.m_Clip.averageSpeed.ToString("0.000");
				text += "\nAverage Angular Y Speed: ";
				text += (this.m_Clip.averageAngularSpeed * 180f / 3.14159274f).ToString("0.0");
				text += " deg/s";
			}
			if (this.m_ClipInfo == null)
			{
				AnimationClipStats animationClipStats = default(AnimationClipStats);
				animationClipStats.Reset();
				for (int i = 0; i < base.targets.Length; i++)
				{
					AnimationClip animationClip = base.targets[i] as AnimationClip;
					if (animationClip != null)
					{
						AnimationClipStats animationClipStats2 = AnimationUtility.GetAnimationClipStats(animationClip);
						animationClipStats.Combine(animationClipStats2);
					}
				}
				if (text.Length != 0)
				{
					text += '\n';
				}
				float num = (float)animationClipStats.constantCurves / (float)animationClipStats.totalCurves * 100f;
				float num2 = (float)animationClipStats.denseCurves / (float)animationClipStats.totalCurves * 100f;
				float num3 = (float)animationClipStats.streamCurves / (float)animationClipStats.totalCurves * 100f;
				text += string.Format("Curves Pos: {0} Quaternion: {1} Euler: {2} Scale: {3} Muscles: {4} Generic: {5} PPtr: {6}\n", new object[]
				{
					animationClipStats.positionCurves,
					animationClipStats.quaternionCurves,
					animationClipStats.eulerCurves,
					animationClipStats.scaleCurves,
					animationClipStats.muscleCurves,
					animationClipStats.genericCurves,
					animationClipStats.pptrCurves
				});
				text += string.Format("Curves Total: {0}, Constant: {1} ({2}%) Dense: {3} ({4}%) Stream: {5} ({6}%)\n", new object[]
				{
					animationClipStats.totalCurves,
					animationClipStats.constantCurves,
					num.ToString("0.0"),
					animationClipStats.denseCurves,
					num2.ToString("0.0"),
					animationClipStats.streamCurves,
					num3.ToString("0.0")
				});
				text += EditorUtility.FormatBytes(animationClipStats.size);
			}
			return text;
		}

		private float GetClipLength()
		{
			float result;
			if (this.m_ClipInfo == null)
			{
				result = this.m_Clip.length;
			}
			else
			{
				result = (this.m_ClipInfo.lastFrame - this.m_ClipInfo.firstFrame) / this.m_Clip.frameRate;
			}
			return result;
		}

		internal override void OnAssetStoreInspectorGUI()
		{
			this.OnInspectorGUI();
		}

		public override void OnInspectorGUI()
		{
			this.Init();
			EditorGUIUtility.labelWidth = 50f;
			EditorGUIUtility.fieldWidth = 30f;
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(true))
			{
				GUILayout.Label("Length", EditorStyles.miniLabel, new GUILayoutOption[]
				{
					GUILayout.Width(46f)
				});
				GUILayout.Label(this.GetClipLength().ToString("0.000"), EditorStyles.miniLabel, new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.Label(this.m_Clip.frameRate + " FPS", EditorStyles.miniLabel, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndHorizontal();
			if (!this.m_Clip.legacy)
			{
				this.MuscleClipGUI();
			}
			else
			{
				this.AnimationClipGUI();
			}
		}

		private void AnimationClipGUI()
		{
			if (this.m_ClipInfo != null)
			{
				float firstFrame = this.m_ClipInfo.firstFrame;
				float lastFrame = this.m_ClipInfo.lastFrame;
				float num = 0f;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				this.ClipRangeGUI(ref firstFrame, ref lastFrame, out flag, out flag2, false, ref num, out flag3);
				if (flag)
				{
					this.m_ClipInfo.firstFrame = firstFrame;
				}
				if (flag2)
				{
					this.m_ClipInfo.lastFrame = lastFrame;
				}
				this.m_AvatarPreview.timeControl.startTime = firstFrame / this.m_Clip.frameRate;
				this.m_AvatarPreview.timeControl.stopTime = lastFrame / this.m_Clip.frameRate;
			}
			else
			{
				this.m_AvatarPreview.timeControl.startTime = 0f;
				this.m_AvatarPreview.timeControl.stopTime = this.m_Clip.length;
			}
			EditorGUIUtility.labelWidth = 0f;
			EditorGUIUtility.fieldWidth = 0f;
			if (this.m_ClipInfo != null)
			{
				this.m_ClipInfo.loop = EditorGUILayout.Toggle("Add Loop Frame", this.m_ClipInfo.loop, new GUILayoutOption[0]);
			}
			EditorGUI.BeginChangeCheck();
			int num2 = (int)((this.m_ClipInfo == null) ? this.m_Clip.wrapMode : ((WrapMode)this.m_ClipInfo.wrapMode));
			num2 = (int)((WrapModeFixed)EditorGUILayout.EnumPopup("Wrap Mode", (WrapModeFixed)num2, new GUILayoutOption[0]));
			if (EditorGUI.EndChangeCheck())
			{
				if (this.m_ClipInfo != null)
				{
					this.m_ClipInfo.wrapMode = num2;
				}
				else
				{
					this.m_Clip.wrapMode = (WrapMode)num2;
				}
			}
		}

		private void CurveGUI()
		{
			if (this.m_ClipInfo != null)
			{
				if (this.m_AvatarPreview.timeControl.currentTime == float.NegativeInfinity)
				{
					this.m_AvatarPreview.timeControl.Update();
				}
				float normalizedTime = this.m_AvatarPreview.timeControl.normalizedTime;
				for (int i = 0; i < this.m_ClipInfo.GetCurveCount(); i++)
				{
					GUILayout.Space(5f);
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button(GUIContent.none, "OL Minus", new GUILayoutOption[]
					{
						GUILayout.Width(17f)
					}))
					{
						this.m_ClipInfo.RemoveCurve(i);
					}
					else
					{
						GUILayout.BeginVertical(new GUILayoutOption[]
						{
							GUILayout.Width(125f)
						});
						string curveName = this.m_ClipInfo.GetCurveName(i);
						string text = EditorGUILayout.DelayedTextField(curveName, EditorStyles.textField, new GUILayoutOption[0]);
						if (curveName != text)
						{
							this.m_ClipInfo.SetCurveName(i, text);
						}
						SerializedProperty curveProperty = this.m_ClipInfo.GetCurveProperty(i);
						AnimationCurve animationCurveValue = curveProperty.animationCurveValue;
						int length = animationCurveValue.length;
						bool flag = false;
						int num = length - 1;
						for (int j = 0; j < length; j++)
						{
							if (Mathf.Abs(animationCurveValue.keys[j].time - normalizedTime) < 0.0001f)
							{
								flag = true;
								num = j;
								break;
							}
							if (animationCurveValue.keys[j].time > normalizedTime)
							{
								num = j;
								break;
							}
						}
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						if (GUILayout.Button(AnimationClipEditor.Styles.PrevKeyContent, new GUILayoutOption[0]))
						{
							if (num > 0)
							{
								num--;
								this.m_AvatarPreview.timeControl.normalizedTime = animationCurveValue.keys[num].time;
							}
						}
						if (GUILayout.Button(AnimationClipEditor.Styles.NextKeyContent, new GUILayoutOption[0]))
						{
							if (flag && num < length - 1)
							{
								num++;
							}
							this.m_AvatarPreview.timeControl.normalizedTime = animationCurveValue.keys[num].time;
						}
						float num2;
						float num3;
						using (new EditorGUI.DisabledScope(!flag))
						{
							string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
							EditorGUI.kFloatFieldFormatString = "n3";
							num2 = animationCurveValue.Evaluate(normalizedTime);
							num3 = EditorGUILayout.FloatField(num2, new GUILayoutOption[]
							{
								GUILayout.Width(60f)
							});
							EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
						}
						bool flag2 = false;
						if (num2 != num3)
						{
							if (flag)
							{
								animationCurveValue.RemoveKey(num);
							}
							flag2 = true;
						}
						using (new EditorGUI.DisabledScope(flag))
						{
							if (GUILayout.Button(AnimationClipEditor.Styles.AddKeyframeContent, new GUILayoutOption[0]))
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							animationCurveValue.AddKey(new Keyframe
							{
								time = normalizedTime,
								value = num3,
								inTangent = 0f,
								outTangent = 0f
							});
							this.m_ClipInfo.SetCurve(i, animationCurveValue);
							AnimationCurvePreviewCache.ClearCache();
						}
						GUILayout.EndHorizontal();
						GUILayout.EndVertical();
						EditorGUILayout.CurveField(curveProperty, EditorGUI.kCurveColor, default(Rect), GUIContent.none, new GUILayoutOption[]
						{
							GUILayout.Height(40f)
						});
						Rect lastRect = GUILayoutUtility.GetLastRect();
						length = animationCurveValue.length;
						Handles.color = Color.red;
						Handles.DrawLine(new Vector3(lastRect.x + normalizedTime * lastRect.width, lastRect.y, 0f), new Vector3(lastRect.x + normalizedTime * lastRect.width, lastRect.y + lastRect.height, 0f));
						for (int k = 0; k < length; k++)
						{
							float time = animationCurveValue.keys[k].time;
							Handles.color = Color.white;
							Handles.DrawLine(new Vector3(lastRect.x + time * lastRect.width, lastRect.y + lastRect.height - 10f, 0f), new Vector3(lastRect.x + time * lastRect.width, lastRect.y + lastRect.height, 0f));
						}
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (GUILayout.Button(GUIContent.none, "OL Plus", new GUILayoutOption[]
				{
					GUILayout.Width(17f)
				}))
				{
					this.m_ClipInfo.AddCurve();
				}
				GUILayout.EndHorizontal();
			}
		}

		private void EventsGUI()
		{
			if (this.m_ClipInfo != null)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (GUILayout.Button(AnimationClipEditor.Styles.AddEventContent, new GUILayoutOption[]
				{
					GUILayout.Width(25f)
				}))
				{
					this.m_ClipInfo.AddEvent(Mathf.Clamp01(this.m_AvatarPreview.timeControl.normalizedTime));
					this.m_EventManipulationHandler.SelectEvent(this.m_ClipInfo.GetEvents(), this.m_ClipInfo.GetEventCount() - 1, this.m_ClipInfo);
					this.needsToGenerateClipInfo = true;
				}
				Rect rect = GUILayoutUtility.GetRect(10f, 33f);
				rect.xMin += 5f;
				rect.xMax -= 4f;
				GUI.Label(rect, "", "TE Toolbar");
				if (Event.current.type == EventType.Repaint)
				{
					this.m_EventTimeArea.rect = rect;
				}
				rect.height -= 15f;
				this.m_EventTimeArea.TimeRuler(rect, 100f);
				GUI.BeginGroup(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f));
				Rect rect2 = new Rect(-1f, -1f, rect.width, rect.height);
				AnimationEvent[] events = this.m_ClipInfo.GetEvents();
				if (this.m_EventManipulationHandler.HandleEventManipulation(rect2, ref events, this.m_ClipInfo))
				{
					this.m_ClipInfo.SetEvents(events);
				}
				float num = this.m_EventTimeArea.TimeToPixel(this.m_AvatarPreview.timeControl.normalizedTime, rect2) - 0.5f;
				Handles.color = new Color(1f, 0f, 0f, 0.5f);
				Handles.DrawLine(new Vector2(num, rect2.yMin), new Vector2(num, rect2.yMax));
				Handles.DrawLine(new Vector2(num + 1f, rect2.yMin), new Vector2(num + 1f, rect2.yMax));
				Handles.color = Color.white;
				GUI.EndGroup();
				GUILayout.EndHorizontal();
				this.m_EventManipulationHandler.Draw(rect);
			}
		}

		private void MuscleClipGUI()
		{
			EditorGUI.BeginChangeCheck();
			this.InitController();
			AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(this.m_Clip);
			bool isHumanMotion = (base.target as Motion).isHumanMotion;
			bool flag = AnimationUtility.HasMotionCurves(this.m_Clip);
			bool flag2 = AnimationUtility.HasRootCurves(this.m_Clip);
			bool flag3 = AnimationUtility.HasGenericRootTransform(this.m_Clip);
			bool flag4 = AnimationUtility.HasMotionFloatCurves(this.m_Clip);
			bool flag5 = flag2 || flag;
			this.m_StartFrame = ((!this.m_DraggingRange) ? (animationClipSettings.startTime * this.m_Clip.frameRate) : this.m_StartFrame);
			this.m_StopFrame = ((!this.m_DraggingRange) ? (animationClipSettings.stopTime * this.m_Clip.frameRate) : this.m_StopFrame);
			this.m_AdditivePoseFrame = ((!this.m_DraggingRange) ? (animationClipSettings.additiveReferencePoseTime * this.m_Clip.frameRate) : this.m_AdditivePoseFrame);
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			if (this.m_ClipInfo != null)
			{
				if (flag5)
				{
					if (this.m_DirtyQualityCurves)
					{
						this.CalculateQualityCurves();
					}
					if (this.m_QualityCurves[0] == null && Event.current.type == EventType.Repaint)
					{
						this.m_DirtyQualityCurves = true;
						base.Repaint();
					}
				}
				this.ClipRangeGUI(ref this.m_StartFrame, ref this.m_StopFrame, out flag6, out flag7, animationClipSettings.hasAdditiveReferencePose, ref this.m_AdditivePoseFrame, out flag8);
			}
			float num = this.m_StartFrame / this.m_Clip.frameRate;
			float num2 = this.m_StopFrame / this.m_Clip.frameRate;
			float num3 = this.m_AdditivePoseFrame / this.m_Clip.frameRate;
			if (!this.m_DraggingRange)
			{
				animationClipSettings.startTime = num;
				animationClipSettings.stopTime = num2;
				animationClipSettings.additiveReferencePoseTime = num3;
			}
			this.m_AvatarPreview.timeControl.startTime = num;
			this.m_AvatarPreview.timeControl.stopTime = num2;
			if (flag6)
			{
				this.m_AvatarPreview.timeControl.nextCurrentTime = num;
			}
			if (flag7)
			{
				this.m_AvatarPreview.timeControl.nextCurrentTime = num2;
			}
			if (flag8)
			{
				this.m_AvatarPreview.timeControl.nextCurrentTime = num3;
			}
			EditorGUIUtility.labelWidth = 0f;
			EditorGUIUtility.fieldWidth = 0f;
			MuscleClipQualityInfo muscleClipQualityInfo = MuscleClipEditorUtilities.GetMuscleClipQualityInfo(this.m_Clip, num, num2);
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			this.LoopToggle(controlRect, AnimationClipEditor.Styles.LoopTime, ref animationClipSettings.loopTime);
			Rect controlRect2;
			using (new EditorGUI.DisabledScope(!animationClipSettings.loopTime))
			{
				EditorGUI.indentLevel++;
				controlRect2 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect2, AnimationClipEditor.Styles.LoopPose, ref animationClipSettings.loopBlend);
				animationClipSettings.cycleOffset = EditorGUILayout.FloatField(AnimationClipEditor.Styles.LoopCycleOffset, animationClipSettings.cycleOffset, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space();
			bool flag9 = isHumanMotion && (flag6 || flag7);
			if (flag2 && !flag)
			{
				GUILayout.Label("Root Transform Rotation", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				Rect controlRect3 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect3, AnimationClipEditor.Styles.BakeIntoPoseOrientation, ref animationClipSettings.loopBlendOrientation);
				int num4 = (!animationClipSettings.keepOriginalOrientation) ? 1 : 0;
				num4 = EditorGUILayout.Popup((!animationClipSettings.loopBlendOrientation) ? AnimationClipEditor.Styles.BasedUponStartOrientation : AnimationClipEditor.Styles.BasedUponOrientation, num4, (!isHumanMotion) ? AnimationClipEditor.Styles.BasedUponRotationOpt : AnimationClipEditor.Styles.BasedUponRotationHumanOpt, new GUILayoutOption[0]);
				animationClipSettings.keepOriginalOrientation = (num4 == 0);
				if (flag9)
				{
					EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				}
				else
				{
					animationClipSettings.orientationOffsetY = EditorGUILayout.FloatField(AnimationClipEditor.Styles.OrientationOffsetY, animationClipSettings.orientationOffsetY, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				GUILayout.Label("Root Transform Position (Y)", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				Rect controlRect4 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect4, AnimationClipEditor.Styles.BakeIntoPosePositionY, ref animationClipSettings.loopBlendPositionY);
				if (isHumanMotion)
				{
					int num5;
					if (animationClipSettings.keepOriginalPositionY)
					{
						num5 = 0;
					}
					else if (animationClipSettings.heightFromFeet)
					{
						num5 = 2;
					}
					else
					{
						num5 = 1;
					}
					num5 = EditorGUILayout.Popup((!animationClipSettings.loopBlendPositionY) ? AnimationClipEditor.Styles.BasedUponPositionY : AnimationClipEditor.Styles.BasedUponStartPositionY, num5, AnimationClipEditor.Styles.BasedUponPositionYHumanOpt, new GUILayoutOption[0]);
					if (num5 == 0)
					{
						animationClipSettings.keepOriginalPositionY = true;
						animationClipSettings.heightFromFeet = false;
					}
					else if (num5 == 1)
					{
						animationClipSettings.keepOriginalPositionY = false;
						animationClipSettings.heightFromFeet = false;
					}
					else
					{
						animationClipSettings.keepOriginalPositionY = false;
						animationClipSettings.heightFromFeet = true;
					}
				}
				else
				{
					int num6 = (!animationClipSettings.keepOriginalPositionY) ? 1 : 0;
					num6 = EditorGUILayout.Popup((!animationClipSettings.loopBlendPositionY) ? AnimationClipEditor.Styles.BasedUponPositionY : AnimationClipEditor.Styles.BasedUponStartPositionY, num6, AnimationClipEditor.Styles.BasedUponPositionYOpt, new GUILayoutOption[0]);
					animationClipSettings.keepOriginalPositionY = (num6 == 0);
				}
				if (flag9)
				{
					EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				}
				else
				{
					animationClipSettings.level = EditorGUILayout.FloatField(AnimationClipEditor.Styles.PositionOffsetY, animationClipSettings.level, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				GUILayout.Label("Root Transform Position (XZ)", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				Rect controlRect5 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect5, AnimationClipEditor.Styles.BakeIntoPosePositionXZ, ref animationClipSettings.loopBlendPositionXZ);
				int num7 = (!animationClipSettings.keepOriginalPositionXZ) ? 1 : 0;
				num7 = EditorGUILayout.Popup((!animationClipSettings.loopBlendPositionXZ) ? AnimationClipEditor.Styles.BasedUponPositionXZ : AnimationClipEditor.Styles.BasedUponStartPositionXZ, num7, (!isHumanMotion) ? AnimationClipEditor.Styles.BasedUponPositionXZOpt : AnimationClipEditor.Styles.BasedUponPositionXZHumanOpt, new GUILayoutOption[0]);
				animationClipSettings.keepOriginalPositionXZ = (num7 == 0);
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				if (flag5)
				{
					if (isHumanMotion)
					{
						this.LoopQualityLampAndCurve(controlRect2, muscleClipQualityInfo.loop, AnimationClipEditor.s_LoopMeterHint, flag6, flag7, this.m_QualityCurves[0]);
					}
					this.LoopQualityLampAndCurve(controlRect3, muscleClipQualityInfo.loopOrientation, AnimationClipEditor.s_LoopOrientationMeterHint, flag6, flag7, this.m_QualityCurves[1]);
					this.LoopQualityLampAndCurve(controlRect4, muscleClipQualityInfo.loopPositionY, AnimationClipEditor.s_LoopPositionYMeterHint, flag6, flag7, this.m_QualityCurves[2]);
					this.LoopQualityLampAndCurve(controlRect5, muscleClipQualityInfo.loopPositionXZ, AnimationClipEditor.s_LoopPositionXZMeterHint, flag6, flag7, this.m_QualityCurves[3]);
				}
			}
			if (isHumanMotion)
			{
				if (flag)
				{
					this.LoopQualityLampAndCurve(controlRect2, muscleClipQualityInfo.loop, AnimationClipEditor.s_LoopMeterHint, flag6, flag7, this.m_QualityCurves[0]);
				}
				animationClipSettings.mirror = EditorGUILayout.Toggle(AnimationClipEditor.Styles.Mirror, animationClipSettings.mirror, new GUILayoutOption[0]);
			}
			if (this.m_ClipInfo != null)
			{
				animationClipSettings.hasAdditiveReferencePose = EditorGUILayout.Toggle(AnimationClipEditor.Styles.HasAdditiveReferencePose, animationClipSettings.hasAdditiveReferencePose, new GUILayoutOption[0]);
				using (new EditorGUI.DisabledScope(!animationClipSettings.hasAdditiveReferencePose))
				{
					EditorGUI.indentLevel++;
					this.m_AdditivePoseFrame = EditorGUILayout.FloatField(AnimationClipEditor.Styles.AdditiveReferencePoseFrame, this.m_AdditivePoseFrame, new GUILayoutOption[0]);
					this.m_AdditivePoseFrame = Mathf.Clamp(this.m_AdditivePoseFrame, this.m_Clip.startTime * this.m_Clip.frameRate, this.m_Clip.stopTime * this.m_Clip.frameRate);
					animationClipSettings.additiveReferencePoseTime = this.m_AdditivePoseFrame / this.m_Clip.frameRate;
					EditorGUI.indentLevel--;
				}
			}
			if (flag)
			{
				EditorGUILayout.Space();
				GUILayout.Label(AnimationClipEditor.Styles.MotionCurves, EditorStyles.label, new GUILayoutOption[0]);
			}
			if (this.m_ClipInfo == null && flag3 && !flag4)
			{
				EditorGUILayout.Space();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				if (flag)
				{
					if (GUILayout.Button("Remove Root Motion Curves", new GUILayoutOption[0]))
					{
						AnimationUtility.SetGenerateMotionCurves(this.m_Clip, false);
					}
				}
				else if (GUILayout.Button("Generate Root Motion Curves", new GUILayoutOption[0]))
				{
					AnimationUtility.SetGenerateMotionCurves(this.m_Clip, true);
				}
				GUILayout.EndHorizontal();
			}
			string statsText = this.GetStatsText();
			if (statsText != "")
			{
				GUILayout.Label(statsText, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
			if (this.m_ClipInfo != null)
			{
				bool changed = GUI.changed;
				AnimationClipEditor.m_ShowCurves = EditorGUILayout.Foldout(AnimationClipEditor.m_ShowCurves, AnimationClipEditor.Styles.Curves, true);
				GUI.changed = changed;
				if (AnimationClipEditor.m_ShowCurves)
				{
					this.CurveGUI();
				}
			}
			if (this.m_ClipInfo != null)
			{
				bool changed = GUI.changed;
				AnimationClipEditor.m_ShowEvents = EditorGUILayout.Foldout(AnimationClipEditor.m_ShowEvents, "Events", true);
				GUI.changed = changed;
				if (AnimationClipEditor.m_ShowEvents)
				{
					this.EventsGUI();
				}
			}
			if (this.m_DraggingRangeBegin)
			{
				this.m_LoopTime = animationClipSettings.loopTime;
				this.m_LoopBlend = animationClipSettings.loopBlend;
				this.m_LoopBlendOrientation = animationClipSettings.loopBlendOrientation;
				this.m_LoopBlendPositionY = animationClipSettings.loopBlendPositionY;
				this.m_LoopBlendPositionXZ = animationClipSettings.loopBlendPositionXZ;
				animationClipSettings.loopTime = false;
				animationClipSettings.loopBlend = false;
				animationClipSettings.loopBlendOrientation = false;
				animationClipSettings.loopBlendPositionY = false;
				animationClipSettings.loopBlendPositionXZ = false;
				this.m_DraggingStartFrame = animationClipSettings.startTime * this.m_Clip.frameRate;
				this.m_DraggingStopFrame = animationClipSettings.stopTime * this.m_Clip.frameRate;
				this.m_DraggingAdditivePoseFrame = animationClipSettings.additiveReferencePoseTime * this.m_Clip.frameRate;
				animationClipSettings.startTime = 0f;
				animationClipSettings.stopTime = this.m_InitialClipLength;
				AnimationUtility.SetAnimationClipSettingsNoDirty(this.m_Clip, animationClipSettings);
				this.DestroyController();
			}
			if (this.m_DraggingRangeEnd)
			{
				animationClipSettings.loopTime = this.m_LoopTime;
				animationClipSettings.loopBlend = this.m_LoopBlend;
				animationClipSettings.loopBlendOrientation = this.m_LoopBlendOrientation;
				animationClipSettings.loopBlendPositionY = this.m_LoopBlendPositionY;
				animationClipSettings.loopBlendPositionXZ = this.m_LoopBlendPositionXZ;
			}
			if (EditorGUI.EndChangeCheck() || this.m_DraggingRangeEnd)
			{
				if (!this.m_DraggingRange)
				{
					Undo.RegisterCompleteObjectUndo(this.m_Clip, "Muscle Clip Edit");
					AnimationUtility.SetAnimationClipSettingsNoDirty(this.m_Clip, animationClipSettings);
					EditorUtility.SetDirty(this.m_Clip);
					this.DestroyController();
				}
			}
		}

		private void LoopToggle(Rect r, GUIContent content, ref bool val)
		{
			if (!this.m_DraggingRange)
			{
				val = EditorGUI.Toggle(r, content, val);
			}
			else
			{
				EditorGUI.LabelField(r, content, GUIContent.none);
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUI.Toggle(r, " ", false);
				}
			}
		}

		private void LoopQualityLampAndCurve(Rect position, float value, int lightMeterHint, bool changedStart, bool changedStop, Vector2[][] curves)
		{
			if (this.m_ClipInfo != null)
			{
				GUIStyle gUIStyle = new GUIStyle(EditorStyles.miniLabel);
				gUIStyle.alignment = TextAnchor.MiddleRight;
				Rect position2 = position;
				position2.xMax -= 20f;
				position2.xMin += EditorGUIUtility.labelWidth;
				GUI.Label(position2, "loop match", gUIStyle);
				Event current = Event.current;
				int controlID = GUIUtility.GetControlID(lightMeterHint, FocusType.Passive, position);
				EventType typeForControl = current.GetTypeForControl(controlID);
				if (typeForControl == EventType.Repaint)
				{
					Rect position3 = position;
					float num = (22f - position3.height) / 2f;
					position3.y -= num;
					position3.xMax += num;
					position3.height = 22f;
					position3.xMin = position3.xMax - 22f;
					if (value < 0.33f)
					{
						GUI.DrawTexture(position3, AnimationClipEditor.Styles.RedLightIcon.image);
					}
					else if (value < 0.66f)
					{
						GUI.DrawTexture(position3, AnimationClipEditor.Styles.OrangeLightIcon.image);
					}
					else
					{
						GUI.DrawTexture(position3, AnimationClipEditor.Styles.GreenLightIcon.image);
					}
					GUI.DrawTexture(position3, AnimationClipEditor.Styles.LightRimIcon.image);
				}
				if (changedStart || changedStop)
				{
					Rect rect = position;
					rect.y += rect.height + 1f;
					rect.height = 18f;
					GUI.color = new Color(0f, 0f, 0f, EditorGUIUtility.isProSkin ? 0.3f : 0.8f);
					GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
					rect = new RectOffset(-1, -1, -1, -1).Add(rect);
					if (!EditorGUIUtility.isProSkin)
					{
						GUI.color = new Color(0.3529412f, 0.3529412f, 0.3529412f, 1f);
					}
					else
					{
						GUI.color = new Color(0.254901975f, 0.254901975f, 0.254901975f, 1f);
					}
					GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
					GUI.color = Color.white;
					GUI.BeginGroup(rect);
					Matrix4x4 drawingToViewMatrix = this.m_TimeArea.drawingToViewMatrix;
					drawingToViewMatrix.m00 = rect.width / this.m_TimeArea.shownArea.width;
					drawingToViewMatrix.m11 = rect.height - 1f;
					drawingToViewMatrix.m03 = -this.m_TimeArea.shownArea.x * rect.width / this.m_TimeArea.shownArea.width;
					drawingToViewMatrix.m13 = 0f;
					Vector2[] array = curves[(!changedStart) ? 1 : 0];
					Vector3[] array2 = new Vector3[array.Length];
					Color[] array3 = new Color[array.Length];
					Color color = new Color(1f, 0.3f, 0.3f);
					Color color2 = new Color(1f, 0.8f, 0f);
					Color color3 = new Color(0f, 1f, 0f);
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = array[i];
						array2[i] = drawingToViewMatrix.MultiplyPoint3x4(array2[i]);
						if (1f - array[i].y < 0.33f)
						{
							array3[i] = color;
						}
						else if (1f - array[i].y < 0.66f)
						{
							array3[i] = color2;
						}
						else
						{
							array3[i] = color3;
						}
					}
					Handles.DrawAAPolyLine(array3, array2);
					GUI.color = new Color(0.3f, 0.6f, 1f);
					float x = drawingToViewMatrix.MultiplyPoint3x4(new Vector3(((!changedStart) ? this.m_StopFrame : this.m_StartFrame) / this.m_Clip.frameRate, 0f, 0f)).x;
					GUI.DrawTexture(new Rect(x, 0f, 1f, rect.height), EditorGUIUtility.whiteTexture);
					x = drawingToViewMatrix.MultiplyPoint3x4(new Vector3(((!changedStart) ? this.m_StartFrame : this.m_StopFrame) / this.m_Clip.frameRate, 0f, 0f)).x;
					GUI.DrawTexture(new Rect(x, 0f, 1f, rect.height), EditorGUIUtility.whiteTexture);
					GUI.color = Color.white;
					GUI.EndGroup();
				}
			}
		}
	}
}
