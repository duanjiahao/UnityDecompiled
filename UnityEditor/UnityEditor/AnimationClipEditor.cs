using System;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(AnimationClip))]
	internal class AnimationClipEditor : Editor
	{
		private class Styles
		{
			public GUIContent StartFrame = EditorGUIUtility.TextContent("AnimationClipEditor.StartFrame");
			public GUIContent EndFrame = EditorGUIUtility.TextContent("AnimationClipEditor.EndFrame");
			public GUIContent LoopTime = EditorGUIUtility.TextContent("AnimationClipEditor.LoopTime");
			public GUIContent LoopPose = EditorGUIUtility.TextContent("AnimationClipEditor.LoopPose");
			public GUIContent LoopCycleOffset = EditorGUIUtility.TextContent("AnimationClipEditor.LoopCycleOffset");
			public GUIContent MotionCurves = EditorGUIUtility.TextContent("AnimationClipEditor.MotionCurves");
			public GUIContent BakeIntoPoseOrientation = EditorGUIUtility.TextContent("AnimationClipEditor.BakeIntoPoseOrientation");
			public GUIContent OrientationOffsetY = EditorGUIUtility.TextContent("AnimationClipEditor.OrientationOffsetY");
			public GUIContent BasedUponOrientation = EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponOrientation");
			public GUIContent BasedUponStartOrientation = EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponStartOrientation");
			public GUIContent[] BasedUponRotationHumanOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponRotation.Original"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponRotationHuman.BodyOrientation")
			};
			public GUIContent[] BasedUponRotationOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponRotation.Original"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponRotation.RootNodeRotation")
			};
			public GUIContent BakeIntoPosePositionY = EditorGUIUtility.TextContent("AnimationClipEditor.BakeIntoPosePositionY");
			public GUIContent PositionOffsetY = EditorGUIUtility.TextContent("AnimationClipEditor.PositionOffsetY");
			public GUIContent BasedUponPositionY = EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionY");
			public GUIContent BasedUponStartPositionY = EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponStartPositionY");
			public GUIContent[] BasedUponPositionYHumanOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionY.Original"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionHuman.CenterOfMass"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionYHuman.Feet")
			};
			public GUIContent[] BasedUponPositionYOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionY.Original"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPosition.RootNodePosition")
			};
			public GUIContent BakeIntoPosePositionXZ = EditorGUIUtility.TextContent("AnimationClipEditor.BakeIntoPosePositionXZ");
			public GUIContent BasedUponPositionXZ = EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionXZ");
			public GUIContent BasedUponStartPositionXZ = EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponStartPositionXZ");
			public GUIContent[] BasedUponPositionXZHumanOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionXZ.Original"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionHuman.CenterOfMass")
			};
			public GUIContent[] BasedUponPositionXZOpt = new GUIContent[]
			{
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPositionXZ.Original"),
				EditorGUIUtility.TextContent("AnimationClipEditor.BasedUponPosition.RootNodePosition")
			};
			public GUIContent Mirror = EditorGUIUtility.TextContent("AnimationClipEditor.Mirror");
			public GUIContent Curves = EditorGUIUtility.TextContent("AnimationClipEditor.Curves");
			public GUIContent Mask = EditorGUIUtility.TextContent("AnimationClipEditor.Mask");
			public GUIContent AddEventContent = EditorGUIUtility.IconContent("Animation.AddEvent");
		}
		private const int kSamplesPerSecond = 60;
		private const int kPose = 0;
		private const int kRotation = 1;
		private const int kHeight = 2;
		private const int kPosition = 3;
		private static AnimationClipEditor.Styles styles;
		private static GUIContent s_GreenLightIcon = EditorGUIUtility.IconContent("lightMeter/greenLight");
		private static GUIContent s_LightRimIcon = EditorGUIUtility.IconContent("lightMeter/lightRim");
		private static GUIContent s_OrangeLightIcon = EditorGUIUtility.IconContent("lightMeter/orangeLight");
		private static GUIContent s_RedLightIcon = EditorGUIUtility.IconContent("lightMeter/redLight");
		private static string s_LoopMeterStr = "LoopMeter";
		private static int s_LoopMeterHint = AnimationClipEditor.s_LoopMeterStr.GetHashCode();
		private static string s_LoopOrientationMeterStr = "LoopOrientationMeter";
		private static int s_LoopOrientationMeterHint = AnimationClipEditor.s_LoopOrientationMeterStr.GetHashCode();
		private static string s_LoopPositionYMeterStr = "LoopPostionYMeter";
		private static int s_LoopPositionYMeterHint = AnimationClipEditor.s_LoopPositionYMeterStr.GetHashCode();
		private static string s_LoopPositionXZMeterStr = "LoopPostionXZMeter";
		private static int s_LoopPositionXZMeterHint = AnimationClipEditor.s_LoopPositionXZMeterStr.GetHashCode();
		public static float s_EventTimelineMax = 1.05f;
		private AnimationClipInfoProperties m_ClipInfo;
		private AnimationClip m_Clip;
		private AnimatorController m_Controller;
		private StateMachine m_StateMachine;
		private State m_State;
		private AvatarPreview m_AvatarPreview;
		private TimeArea m_TimeArea;
		private TimeArea m_EventTimeArea;
		private bool m_DraggingRange;
		private bool m_DraggingRangeBegin;
		private bool m_DraggingRangeEnd;
		private bool m_LoopTime;
		private bool m_LoopBlend;
		private bool m_LoopBlendOrientation;
		private bool m_LoopBlendPositionY;
		private bool m_LoopBlendPositionXZ;
		private float m_StartFrame;
		private float m_StopFrame = 1f;
		private AvatarMask m_Mask;
		private AvatarMaskInspector m_MaskInspector;
		private string[] m_ReferenceTransformPaths;
		private bool m_ShowCurves;
		private EventManipulationHandler m_EventManipulationHandler;
		private bool m_ShowEvents;
		private bool m_MaskFoldout;
		private Vector2[][][] m_QualityCurves = new Vector2[4][][];
		private bool m_DirtyQualityCurves;
		private static GUIContent prevKeyContent = EditorGUIUtility.IconContent("Animation.PrevKey");
		private static GUIContent nextKeyContent = EditorGUIUtility.IconContent("Animation.NextKey");
		private static GUIContent addKeyframeContent = EditorGUIUtility.IconContent("Animation.AddKeyframe");
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
		public string[] referenceTransformPaths
		{
			get
			{
				return this.m_ReferenceTransformPaths;
			}
			set
			{
				this.m_ReferenceTransformPaths = value;
			}
		}
		private void UpdateEventsPopupClipInfo(AnimationClipInfoProperties info)
		{
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
			AnimationEventPopup animationEventPopup = (array.Length <= 0) ? null : ((AnimationEventPopup)array[0]);
			if (animationEventPopup && animationEventPopup.clipInfo == this.m_ClipInfo)
			{
				animationEventPopup.clipInfo = info;
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
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				if (this.m_Controller == null)
				{
					this.m_Controller = new AnimatorController();
					this.m_Controller.hideFlags = HideFlags.DontSave;
					this.m_Controller.AddLayer("preview");
					this.m_StateMachine = this.m_Controller.GetLayerStateMachine(0);
					if (this.m_ClipInfo != null)
					{
						this.InitMask();
						this.m_Controller.SetLayerMask(0, this.m_Mask);
					}
				}
				if (this.m_State == null)
				{
					this.m_State = this.m_StateMachine.AddState("preview");
					this.m_State.SetAnimationClip(this.m_Clip);
					this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
					this.m_State.hideFlags = HideFlags.DontSave;
				}
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				if (AnimatorController.GetEffectiveAnimatorController(this.m_AvatarPreview.Animator) != this.m_Controller)
				{
					AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				}
			}
		}
		internal override void OnHeaderIconGUI(Rect iconRect)
		{
			bool flag = AssetPreview.IsLoadingAssetPreview(this.target.GetInstanceID());
			Texture2D texture2D = AssetPreview.GetAssetPreview(this.target);
			if (!texture2D)
			{
				if (flag)
				{
					base.Repaint();
				}
				texture2D = AssetPreview.GetMiniThumbnail(this.target);
			}
			GUI.DrawTexture(iconRect, texture2D);
		}
		internal override void OnHeaderTitleGUI(Rect titleRect, string header)
		{
			if (this.m_ClipInfo != null)
			{
				this.m_ClipInfo.name = EditorGUI.DelayedTextField(titleRect, this.m_ClipInfo.name, null, EditorStyles.textField);
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
			}
		}
		private void DestroyController()
		{
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, null);
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
			if (AnimationClipEditor.styles == null)
			{
				AnimationClipEditor.styles = new AnimationClipEditor.Styles();
			}
			if (this.m_AvatarPreview == null)
			{
				this.m_AvatarPreview = new AvatarPreview(null, this.target as Motion);
				this.m_AvatarPreview.OnAvatarChangeFunc = new AvatarPreview.OnAvatarChange(this.SetPreviewAvatar);
				this.m_AvatarPreview.fps = Mathf.RoundToInt((this.target as AnimationClip).frameRate);
				this.m_AvatarPreview.ShowIKOnFeetButton = (this.target as Motion).isHumanMotion;
			}
		}
		private void InitMask()
		{
			if (this.m_Mask == null)
			{
				this.m_Mask = new AvatarMask();
				this.m_MaskInspector = (AvatarMaskInspector)Editor.CreateEditor(this.m_Mask);
				this.m_MaskInspector.canImport = false;
				if (this.m_ClipInfo != null)
				{
					this.m_MaskInspector.clipInfo = this.m_ClipInfo;
				}
				if (this.m_Mask.transformCount == 0)
				{
					this.SetTransformMaskFromReference();
				}
			}
		}
		private void SetTransformMaskFromReference()
		{
			AvatarMaskUtility.UpdateTransformMask(this.m_Mask, this.m_ReferenceTransformPaths, null);
		}
		private bool IsMaskUpToDate()
		{
			if (this.m_Mask.transformCount != this.m_ReferenceTransformPaths.Length)
			{
				return false;
			}
			for (int i = 0; i < this.m_ReferenceTransformPaths.Length; i++)
			{
				if (this.m_Mask.GetTransformPath(i) != this.m_ReferenceTransformPaths[i])
				{
					return false;
				}
			}
			return true;
		}
		private void OnEnable()
		{
			if (AnimationClipEditor.styles == null)
			{
				AnimationClipEditor.styles = new AnimationClipEditor.Styles();
			}
			this.m_Clip = (this.target as AnimationClip);
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
			this.m_TimeArea.OnEnable();
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
			this.m_EventTimeArea.OnEnable();
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
			if (this.m_MaskInspector)
			{
				UnityEngine.Object.DestroyImmediate(this.m_MaskInspector);
			}
			if (this.m_Mask)
			{
				UnityEngine.Object.DestroyImmediate(this.m_Mask);
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
				float num4 = (j != 0) ? this.m_Clip.length : num2;
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
			AnimationClip animationClip = this.target as AnimationClip;
			AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
			this.m_AvatarPreview.timeControl.loop = true;
			if (flag && this.m_AvatarPreview.PreviewObject != null)
			{
				if (this.m_AvatarPreview.Animator != null)
				{
					if (this.m_State != null)
					{
						this.m_State.iKOnFeet = this.m_AvatarPreview.IKOnFeet;
					}
					float normalizedTime = (this.m_AvatarPreview.timeControl.currentTime - animationClipSettings.startTime) / (animationClipSettings.stopTime - animationClipSettings.startTime);
					this.m_AvatarPreview.Animator.Play(0, 0, normalizedTime);
					this.m_AvatarPreview.Animator.Update(this.m_AvatarPreview.timeControl.deltaTime);
				}
				else
				{
					this.m_AvatarPreview.PreviewObject.SampleAnimation(animationClip, this.m_AvatarPreview.timeControl.currentTime);
				}
			}
			this.m_AvatarPreview.DoAvatarPreview(r, background);
		}
		public void ClipRangeGUI(ref float startFrame, ref float stopFrame, out bool changedStart, out bool changedStop)
		{
			changedStart = false;
			changedStop = false;
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
			GUI.Label(rect, string.Empty, "TE Toolbar");
			if (Event.current.type == EventType.Repaint)
			{
				this.m_TimeArea.rect = rect;
			}
			this.m_TimeArea.BeginViewGUI();
			this.m_TimeArea.EndViewGUI();
			rect.height -= 15f;
			int controlID = GUIUtility.GetControlID(3126789, FocusType.Passive);
			int controlID2 = GUIUtility.GetControlID(3126789, FocusType.Passive);
			GUI.BeginGroup(new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f));
			float num = -1f;
			rect.y = num;
			rect.x = num;
			float num2 = this.m_TimeArea.FrameToPixel(startFrame, this.m_Clip.frameRate, rect);
			float num3 = this.m_TimeArea.FrameToPixel(stopFrame, this.m_Clip.frameRate, rect);
			GUI.Label(new Rect(num2, rect.y, num3 - num2, rect.height), string.Empty, EditorStyles.selectionRect);
			this.m_TimeArea.TimeRuler(rect, this.m_Clip.frameRate);
			float num4 = this.m_TimeArea.TimeToPixel(this.m_AvatarPreview.timeControl.currentTime, rect) - 0.5f;
			Handles.color = new Color(1f, 0f, 0f, 0.5f);
			Handles.DrawLine(new Vector2(num4, rect.yMin), new Vector2(num4, rect.yMax));
			Handles.DrawLine(new Vector2(num4 + 1f, rect.yMin), new Vector2(num4 + 1f, rect.yMax));
			Handles.color = Color.white;
			EditorGUI.BeginDisabledGroup(flag);
			float num5 = startFrame / this.m_Clip.frameRate;
			if (this.m_TimeArea.BrowseRuler(rect, controlID, ref num5, 0f, false, "TL InPoint") != TimeArea.TimeRulerDragMode.None)
			{
				startFrame = num5 * this.m_Clip.frameRate;
				startFrame = MathUtils.RoundBasedOnMinimumDifference(startFrame, this.m_TimeArea.PixelDeltaToTime(rect) * this.m_Clip.frameRate * 10f);
				changedStart = true;
			}
			float num6 = stopFrame / this.m_Clip.frameRate;
			if (this.m_TimeArea.BrowseRuler(rect, controlID2, ref num6, 0f, false, "TL OutPoint") != TimeArea.TimeRulerDragMode.None)
			{
				stopFrame = num6 * this.m_Clip.frameRate;
				stopFrame = MathUtils.RoundBasedOnMinimumDifference(stopFrame, this.m_TimeArea.PixelDeltaToTime(rect) * this.m_Clip.frameRate * 10f);
				changedStop = true;
			}
			EditorGUI.EndDisabledGroup();
			if (GUIUtility.hotControl == controlID)
			{
				changedStart = true;
			}
			if (GUIUtility.hotControl == controlID2)
			{
				changedStop = true;
			}
			GUI.EndGroup();
			EditorGUI.BeginDisabledGroup(flag);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			startFrame = EditorGUILayout.FloatField(AnimationClipEditor.styles.StartFrame, startFrame, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				changedStart = true;
			}
			GUILayout.FlexibleSpace();
			EditorGUI.BeginChangeCheck();
			stopFrame = EditorGUILayout.FloatField(AnimationClipEditor.styles.EndFrame, stopFrame, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				changedStop = true;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			changedStart |= flag2;
			changedStop |= flag2;
			if (changedStart)
			{
				startFrame = Mathf.Clamp(startFrame, this.m_Clip.startTime * this.m_Clip.frameRate, Mathf.Clamp(stopFrame, startFrame + 0.1f, this.m_Clip.stopTime * this.m_Clip.frameRate));
			}
			if (changedStop)
			{
				stopFrame = Mathf.Clamp(stopFrame, startFrame + 0.1f, this.m_Clip.stopTime * this.m_Clip.frameRate);
			}
			if (changedStart || changedStop)
			{
				if (!this.m_DraggingRange)
				{
					this.m_DraggingRangeBegin = true;
				}
				this.m_DraggingRange = true;
			}
			else
			{
				if (this.m_DraggingRange && GUIUtility.hotControl == 0 && Event.current.type == EventType.Repaint)
				{
					this.m_DraggingRangeEnd = true;
					this.m_DraggingRange = false;
					this.m_DirtyQualityCurves = true;
					base.Repaint();
				}
			}
			GUILayout.Space(10f);
		}
		private string GetStatsText()
		{
			string text = string.Empty;
			bool flag = base.targets.Length == 1 && (this.target as Motion).isHumanMotion;
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
				text += string.Format("Curves Pos: {0} Rot: {1} Scale: {2} Muscles: {3} Generic: {4} PPtr: {5}\n", new object[]
				{
					animationClipStats.positionCurves,
					animationClipStats.rotationCurves,
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
			if (this.m_ClipInfo == null)
			{
				return this.m_Clip.length;
			}
			return (this.m_ClipInfo.lastFrame - this.m_ClipInfo.firstFrame) / this.m_Clip.frameRate;
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
			EditorGUI.BeginDisabledGroup(true);
			GUILayout.Label("Length", EditorStyles.miniLabel, new GUILayoutOption[]
			{
				GUILayout.Width(46f)
			});
			GUILayout.Label(this.GetClipLength().ToString("0.000"), EditorStyles.miniLabel, new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(this.m_Clip.frameRate + " FPS", EditorStyles.miniLabel, new GUILayoutOption[0]);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			if (this.m_Clip.isAnimatorMotion)
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
				bool flag = false;
				bool flag2 = false;
				this.ClipRangeGUI(ref firstFrame, ref lastFrame, out flag, out flag2);
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
			int num = (int)((this.m_ClipInfo == null) ? this.m_Clip.wrapMode : ((WrapMode)this.m_ClipInfo.wrapMode));
			num = (int)((WrapModeFixed)EditorGUILayout.EnumPopup("Wrap Mode", (WrapModeFixed)num, new GUILayoutOption[0]));
			if (EditorGUI.EndChangeCheck())
			{
				if (this.m_ClipInfo != null)
				{
					this.m_ClipInfo.wrapMode = num;
				}
				else
				{
					this.m_Clip.wrapMode = (WrapMode)num;
				}
			}
		}
		private void CurveGUI()
		{
			if (this.m_ClipInfo == null)
			{
				return;
			}
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
					string text = EditorGUILayout.DelayedTextField(curveName, null, EditorStyles.textField, new GUILayoutOption[0]);
					if (curveName != text)
					{
						this.m_ClipInfo.SetCurveName(i, text);
					}
					AnimationCurve animationCurve = this.m_ClipInfo.GetCurve(i);
					int length = animationCurve.length;
					bool flag = false;
					int num = length - 1;
					for (int j = 0; j < length; j++)
					{
						if (Mathf.Abs(animationCurve.keys[j].time - normalizedTime) < 0.0001f)
						{
							flag = true;
							num = j;
							break;
						}
						if (animationCurve.keys[j].time > normalizedTime)
						{
							num = j;
							break;
						}
					}
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					if (GUILayout.Button(AnimationClipEditor.prevKeyContent, new GUILayoutOption[0]) && num > 0)
					{
						num--;
						this.m_AvatarPreview.timeControl.normalizedTime = animationCurve.keys[num].time;
					}
					if (GUILayout.Button(AnimationClipEditor.nextKeyContent, new GUILayoutOption[0]))
					{
						if (flag && num < length - 1)
						{
							num++;
						}
						this.m_AvatarPreview.timeControl.normalizedTime = animationCurve.keys[num].time;
					}
					EditorGUI.BeginDisabledGroup(!flag);
					string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
					EditorGUI.kFloatFieldFormatString = "n3";
					float num2 = animationCurve.Evaluate(normalizedTime);
					float num3 = EditorGUILayout.FloatField(num2, new GUILayoutOption[]
					{
						GUILayout.Width(60f)
					});
					EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
					EditorGUI.EndDisabledGroup();
					bool flag2 = false;
					if (num2 != num3)
					{
						if (flag)
						{
							animationCurve.RemoveKey(num);
						}
						flag2 = true;
					}
					EditorGUI.BeginDisabledGroup(flag);
					if (GUILayout.Button(AnimationClipEditor.addKeyframeContent, new GUILayoutOption[0]))
					{
						flag2 = true;
					}
					EditorGUI.EndDisabledGroup();
					if (flag2)
					{
						animationCurve.AddKey(new Keyframe
						{
							time = normalizedTime,
							value = num3,
							inTangent = 0f,
							outTangent = 0f
						});
					}
					GUILayout.EndHorizontal();
					GUILayout.EndVertical();
					animationCurve = EditorGUILayout.CurveField(animationCurve, new GUILayoutOption[]
					{
						GUILayout.Height(40f)
					});
					Rect lastRect = GUILayoutUtility.GetLastRect();
					length = animationCurve.length;
					Handles.color = Color.red;
					Handles.DrawLine(new Vector3(lastRect.x + normalizedTime * lastRect.width, lastRect.y, 0f), new Vector3(lastRect.x + normalizedTime * lastRect.width, lastRect.y + lastRect.height, 0f));
					for (int k = 0; k < length; k++)
					{
						float time = animationCurve.keys[k].time;
						Handles.color = Color.white;
						Handles.DrawLine(new Vector3(lastRect.x + time * lastRect.width, lastRect.y + lastRect.height - 10f, 0f), new Vector3(lastRect.x + time * lastRect.width, lastRect.y + lastRect.height, 0f));
					}
					this.m_ClipInfo.SetCurve(i, animationCurve);
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
		private void EventsGUI()
		{
			if (this.m_ClipInfo == null)
			{
				return;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (GUILayout.Button(AnimationClipEditor.styles.AddEventContent, new GUILayoutOption[]
			{
				GUILayout.Width(25f)
			}))
			{
				this.m_ClipInfo.AddEvent(this.m_AvatarPreview.timeControl.normalizedTime);
				this.m_EventManipulationHandler.SelectEvent(this.m_ClipInfo.GetEvents(), this.m_ClipInfo.GetEventCount() - 1, this.m_ClipInfo);
			}
			Rect rect = GUILayoutUtility.GetRect(10f, 33f);
			rect.xMin += 5f;
			rect.xMax -= 4f;
			GUI.Label(rect, string.Empty, "TE Toolbar");
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
			this.m_EventManipulationHandler.DrawInstantTooltip(rect);
		}
		private void MuscleClipGUI()
		{
			EditorGUI.BeginChangeCheck();
			this.InitController();
			AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(this.m_Clip);
			bool isHumanMotion = (this.target as Motion).isHumanMotion;
			bool flag = isHumanMotion && AnimationUtility.HasMotionCurves(this.m_Clip);
			this.m_StartFrame = ((!this.m_DraggingRange) ? (animationClipSettings.startTime * this.m_Clip.frameRate) : this.m_StartFrame);
			this.m_StopFrame = ((!this.m_DraggingRange) ? (animationClipSettings.stopTime * this.m_Clip.frameRate) : this.m_StopFrame);
			bool flag2 = false;
			bool flag3 = false;
			if (this.m_ClipInfo != null)
			{
				if (isHumanMotion)
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
				this.ClipRangeGUI(ref this.m_StartFrame, ref this.m_StopFrame, out flag2, out flag3);
			}
			float num = this.m_StartFrame / this.m_Clip.frameRate;
			float num2 = this.m_StopFrame / this.m_Clip.frameRate;
			if (!this.m_DraggingRange)
			{
				animationClipSettings.startTime = num;
				animationClipSettings.stopTime = num2;
			}
			this.m_AvatarPreview.timeControl.startTime = num;
			this.m_AvatarPreview.timeControl.stopTime = num2;
			if (flag2)
			{
				this.m_AvatarPreview.timeControl.nextCurrentTime = num;
			}
			if (flag3)
			{
				this.m_AvatarPreview.timeControl.nextCurrentTime = num2;
			}
			EditorGUIUtility.labelWidth = 0f;
			EditorGUIUtility.fieldWidth = 0f;
			MuscleClipQualityInfo muscleClipQualityInfo = MuscleClipEditorUtilities.GetMuscleClipQualityInfo(this.m_Clip, num, num2);
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			this.LoopToggle(controlRect, AnimationClipEditor.styles.LoopTime, ref animationClipSettings.loopTime);
			EditorGUI.BeginDisabledGroup(!animationClipSettings.loopTime);
			EditorGUI.indentLevel++;
			Rect controlRect2 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			this.LoopToggle(controlRect2, AnimationClipEditor.styles.LoopPose, ref animationClipSettings.loopBlend);
			animationClipSettings.cycleOffset = EditorGUILayout.FloatField(AnimationClipEditor.styles.LoopCycleOffset, animationClipSettings.cycleOffset, new GUILayoutOption[0]);
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.Space();
			bool flag4 = isHumanMotion && (flag2 || flag3);
			if (!flag)
			{
				GUILayout.Label("Root Transform Rotation", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				Rect controlRect3 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect3, AnimationClipEditor.styles.BakeIntoPoseOrientation, ref animationClipSettings.loopBlendOrientation);
				int num3 = (!animationClipSettings.keepOriginalOrientation) ? 1 : 0;
				num3 = EditorGUILayout.Popup((!animationClipSettings.loopBlendOrientation) ? AnimationClipEditor.styles.BasedUponStartOrientation : AnimationClipEditor.styles.BasedUponOrientation, num3, (!isHumanMotion) ? AnimationClipEditor.styles.BasedUponRotationOpt : AnimationClipEditor.styles.BasedUponRotationHumanOpt, new GUILayoutOption[0]);
				animationClipSettings.keepOriginalOrientation = (num3 == 0);
				if (flag4)
				{
					EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				}
				else
				{
					animationClipSettings.orientationOffsetY = EditorGUILayout.FloatField(AnimationClipEditor.styles.OrientationOffsetY, animationClipSettings.orientationOffsetY, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				GUILayout.Label("Root Transform Position (Y)", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				Rect controlRect4 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect4, AnimationClipEditor.styles.BakeIntoPosePositionY, ref animationClipSettings.loopBlendPositionY);
				if (isHumanMotion)
				{
					int num4;
					if (animationClipSettings.keepOriginalPositionY)
					{
						num4 = 0;
					}
					else
					{
						if (animationClipSettings.heightFromFeet)
						{
							num4 = 2;
						}
						else
						{
							num4 = 1;
						}
					}
					num4 = EditorGUILayout.Popup((!animationClipSettings.loopBlendPositionY) ? AnimationClipEditor.styles.BasedUponPositionY : AnimationClipEditor.styles.BasedUponStartPositionY, num4, AnimationClipEditor.styles.BasedUponPositionYHumanOpt, new GUILayoutOption[0]);
					if (num4 == 0)
					{
						animationClipSettings.keepOriginalPositionY = true;
						animationClipSettings.heightFromFeet = false;
					}
					else
					{
						if (num4 == 1)
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
				}
				else
				{
					int num5 = (!animationClipSettings.keepOriginalPositionY) ? 1 : 0;
					num5 = EditorGUILayout.Popup((!animationClipSettings.loopBlendPositionY) ? AnimationClipEditor.styles.BasedUponPositionY : AnimationClipEditor.styles.BasedUponStartPositionY, num5, AnimationClipEditor.styles.BasedUponPositionYOpt, new GUILayoutOption[0]);
					animationClipSettings.keepOriginalPositionY = (num5 == 0);
				}
				if (flag4)
				{
					EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				}
				else
				{
					animationClipSettings.level = EditorGUILayout.FloatField(AnimationClipEditor.styles.PositionOffsetY, animationClipSettings.level, new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				GUILayout.Label("Root Transform Position (XZ)", EditorStyles.label, new GUILayoutOption[0]);
				EditorGUI.indentLevel++;
				Rect controlRect5 = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
				this.LoopToggle(controlRect5, AnimationClipEditor.styles.BakeIntoPosePositionXZ, ref animationClipSettings.loopBlendPositionXZ);
				int num6 = (!animationClipSettings.keepOriginalPositionXZ) ? 1 : 0;
				num6 = EditorGUILayout.Popup((!animationClipSettings.loopBlendPositionXZ) ? AnimationClipEditor.styles.BasedUponPositionXZ : AnimationClipEditor.styles.BasedUponStartPositionXZ, num6, (!isHumanMotion) ? AnimationClipEditor.styles.BasedUponPositionXZOpt : AnimationClipEditor.styles.BasedUponPositionXZHumanOpt, new GUILayoutOption[0]);
				animationClipSettings.keepOriginalPositionXZ = (num6 == 0);
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();
				if (isHumanMotion)
				{
					this.LoopQualityLampAndCurve(controlRect2, muscleClipQualityInfo.loop, AnimationClipEditor.s_LoopMeterHint, flag2, flag3, this.m_QualityCurves[0]);
					this.LoopQualityLampAndCurve(controlRect3, muscleClipQualityInfo.loopOrientation, AnimationClipEditor.s_LoopOrientationMeterHint, flag2, flag3, this.m_QualityCurves[1]);
					this.LoopQualityLampAndCurve(controlRect4, muscleClipQualityInfo.loopPositionY, AnimationClipEditor.s_LoopPositionYMeterHint, flag2, flag3, this.m_QualityCurves[2]);
					this.LoopQualityLampAndCurve(controlRect5, muscleClipQualityInfo.loopPositionXZ, AnimationClipEditor.s_LoopPositionXZMeterHint, flag2, flag3, this.m_QualityCurves[3]);
				}
			}
			if (isHumanMotion)
			{
				if (flag)
				{
					this.LoopQualityLampAndCurve(controlRect2, muscleClipQualityInfo.loop, AnimationClipEditor.s_LoopMeterHint, flag2, flag3, this.m_QualityCurves[0]);
				}
				animationClipSettings.mirror = EditorGUILayout.Toggle(AnimationClipEditor.styles.Mirror, animationClipSettings.mirror, new GUILayoutOption[0]);
			}
			if (flag)
			{
				EditorGUILayout.Space();
				GUILayout.Label(AnimationClipEditor.styles.MotionCurves, EditorStyles.label, new GUILayoutOption[0]);
			}
			string statsText = this.GetStatsText();
			if (statsText != string.Empty)
			{
				GUILayout.Label(statsText, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
			EditorGUILayout.Space();
			if (this.m_ClipInfo != null)
			{
				this.InitMask();
				this.m_MaskInspector.showBody = isHumanMotion;
				int indentLevel = EditorGUI.indentLevel;
				bool changed = GUI.changed;
				this.m_MaskFoldout = EditorGUILayout.Foldout(this.m_MaskFoldout, AnimationClipEditor.styles.Mask);
				GUI.changed = changed;
				if (this.m_ClipInfo.maskType == ClipAnimationMaskType.CreateFromThisModel && !this.IsMaskUpToDate())
				{
					GUILayout.BeginHorizontal(EditorStyles.helpBox, new GUILayoutOption[0]);
					GUILayout.Label("Mask does not match hierarchy. Animation might not import correctly", EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.BeginVertical(new GUILayoutOption[0]);
					GUILayout.Space(5f);
					if (GUILayout.Button("Fix Mask", new GUILayoutOption[0]))
					{
						this.SetTransformMaskFromReference();
						this.m_ClipInfo.MaskToClip(this.m_Mask);
					}
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
				if (this.m_MaskFoldout)
				{
					EditorGUI.indentLevel++;
					this.m_MaskInspector.OnInspectorGUI();
				}
				EditorGUI.indentLevel = indentLevel;
			}
			bool flag5 = InternalEditorUtility.HasPro();
			if (flag5 && this.m_ClipInfo != null)
			{
				bool changed = GUI.changed;
				this.m_ShowCurves = EditorGUILayout.Foldout(this.m_ShowCurves, AnimationClipEditor.styles.Curves);
				GUI.changed = changed;
				if (this.m_ShowCurves)
				{
					this.CurveGUI();
				}
			}
			if (this.m_ClipInfo != null)
			{
				bool changed = GUI.changed;
				this.m_ShowEvents = EditorGUILayout.Foldout(this.m_ShowEvents, "Events");
				GUI.changed = changed;
				if (this.m_ShowEvents)
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
				animationClipSettings.startTime = 0f;
				animationClipSettings.stopTime = this.m_Clip.length;
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
			if ((EditorGUI.EndChangeCheck() || this.m_DraggingRangeEnd) && !this.m_DraggingRange)
			{
				Undo.RegisterCompleteObjectUndo(this.m_Clip, "Muscle Clip Edit");
				AnimationUtility.SetAnimationClipSettingsNoDirty(this.m_Clip, animationClipSettings);
				EditorUtility.SetDirty(this.m_Clip);
				this.DestroyController();
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
				EditorGUI.BeginDisabledGroup(true);
				EditorGUI.Toggle(r, " ", false);
				EditorGUI.EndDisabledGroup();
			}
		}
		private void LoopQualityLampAndCurve(Rect position, float value, int lightMeterHint, bool changedStart, bool changedStop, Vector2[][] curves)
		{
			if (this.m_ClipInfo == null)
			{
				return;
			}
			GUIStyle gUIStyle = new GUIStyle(EditorStyles.miniLabel);
			gUIStyle.alignment = TextAnchor.MiddleRight;
			Rect position2 = position;
			position2.xMax -= 20f;
			position2.xMin += EditorGUIUtility.labelWidth;
			GUI.Label(position2, "loop match", gUIStyle);
			Event current = Event.current;
			int controlID = GUIUtility.GetControlID(lightMeterHint, FocusType.Native, position);
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
					GUI.DrawTexture(position3, AnimationClipEditor.s_RedLightIcon.image);
				}
				else
				{
					if (value < 0.66f)
					{
						GUI.DrawTexture(position3, AnimationClipEditor.s_OrangeLightIcon.image);
					}
					else
					{
						GUI.DrawTexture(position3, AnimationClipEditor.s_GreenLightIcon.image);
					}
				}
				GUI.DrawTexture(position3, AnimationClipEditor.s_LightRimIcon.image);
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
					else
					{
						if (1f - array[i].y < 0.66f)
						{
							array3[i] = color2;
						}
						else
						{
							array3[i] = color3;
						}
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
