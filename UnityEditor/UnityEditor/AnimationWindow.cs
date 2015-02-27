using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class AnimationWindow : EditorWindow, TimeUpdater, CurveUpdater
	{
		internal class Styles
		{
			public Texture2D pointIcon = EditorGUIUtility.LoadIcon("animationkeyframe");
			public GUIContent playContent = EditorGUIUtility.IconContent("Animation.Play");
			public GUIContent recordContent = EditorGUIUtility.IconContent("Animation.Record");
			public GUIContent prevKeyContent = EditorGUIUtility.IconContent("Animation.PrevKey");
			public GUIContent nextKeyContent = EditorGUIUtility.IconContent("Animation.NextKey");
			public GUIContent addKeyframeContent = EditorGUIUtility.IconContent("Animation.AddKeyframe");
			public GUIContent addEventContent = EditorGUIUtility.IconContent("Animation.AddEvent");
			public GUIStyle curveEditorBackground = "AnimationCurveEditorBackground";
			public GUIStyle eventBackground = "AnimationEventBackground";
			public GUIStyle keyframeBackground = "AnimationKeyframeBackground";
			public GUIStyle rowOdd = "AnimationRowEven";
			public GUIStyle rowEven = "AnimationRowOdd";
			public GUIStyle TimelineTick = "AnimationTimelineTick";
			public GUIStyle miniToolbar = new GUIStyle(EditorStyles.toolbar);
			public GUIStyle miniToolbarButton = new GUIStyle(EditorStyles.toolbarButton);
			public GUIStyle toolbarLabel = new GUIStyle(EditorStyles.toolbarPopup);
			public Styles()
			{
				this.toolbarLabel.normal.background = null;
				this.miniToolbarButton.padding.top = 0;
				this.miniToolbarButton.padding.bottom = 3;
			}
		}
		internal const int kTickRulerDistMin = 3;
		internal const int kTickRulerDistFull = 80;
		internal const int kTickRulerDistLabel = 40;
		internal const float kTickRulerHeightMax = 0.7f;
		internal const float kTickRulerFatThreshold = 0.5f;
		internal const int kIntFieldWidth = 35;
		internal const int kButtonWidth = 30;
		internal const int kAnimationHeight = 15;
		internal const int kTimeRulerHeight = 17;
		internal const int kEventLineHeight = 18;
		internal const int kKeyframeLineHeight = 15;
		internal const int kHierarchyLeftMargin = 2;
		internal const int kHierarchyFieldWidth = 45;
		internal const int kHierarchyIconWidth = 11;
		internal const int kHierarchyIconButtonWidth = 22;
		internal const int kHierarchyIconHeight = 11;
		internal const int kHierarchyGameobjectHeight = 15;
		internal const int kHierarchyComponentHeight = 15;
		internal const int kHierarchyPropertyHeight = 12;
		internal const int kHierarchyAnimationSpacingHeight = 15;
		internal const int kFoldoutArrowWidth = 15;
		internal const int kIconWidth = 10;
		internal const int kSliderThickness = 15;
		internal const int kSamplesLabelWidth = 45;
		internal const float kCurveEditorPlayheadAlpha = 0.6f;
		private static List<AnimationWindow> s_AnimationWindows = new List<AnimationWindow>();
		private static bool s_LastShowCurveEditor;
		internal static int vPosition;
		internal static bool kEvenRow;
		internal static int s_StartDragFrame = 0;
		internal static PrefColor kEulerXColor = new PrefColor("Testing/EulerX", 1f, 0f, 1f, 1f);
		internal static PrefColor kEulerYColor = new PrefColor("Testing/EulerY", 1f, 1f, 0f, 1f);
		internal static PrefColor kEulerZColor = new PrefColor("Testing/EulerZ", 0f, 1f, 1f, 1f);
		internal static PrefColor kAnimatedColor = new PrefColor("Testing/AnimatedObject", 0f, 0f, 0f, 0.3f);
		internal static PrefKey kAnimationPrevFrame = new PrefKey("Animation/Previous Frame", ",");
		internal static PrefKey kAnimationNextFrame = new PrefKey("Animation/Next Frame", ".");
		internal static PrefKey kAnimationPrevKeyframe = new PrefKey("Animation/Previous Keyframe", "&,");
		internal static PrefKey kAnimationNextKeyframe = new PrefKey("Animation/Next Keyframe", "&.");
		internal static PrefKey kAnimationRecordKeyframe = new PrefKey("Animation/Record Keyframe", "k");
		internal static PrefKey kAnimationShowCurvesToggle = new PrefKey("Animation/Show curves", "c");
		[SerializeField]
		private SerializedStringTable m_ExpandedFoldouts;
		[SerializeField]
		private SerializedStringTable m_ChosenAnimated;
		[SerializeField]
		private SerializedStringTable m_ChosenClip;
		[SerializeField]
		private bool m_ShowAllProperties = true;
		[SerializeField]
		private AnimationEventTimeLine m_AnimationEventTimeLine;
		private AnimationWindowHierarchy m_Hierarchy;
		private DopeSheetEditor m_DopeSheetEditor;
		private CurveEditor m_CurveEditor;
		public static AnimationSelection[] m_Selected;
		[NonSerialized]
		private CurveState[] m_ShownProperties = new CurveState[0];
		[NonSerialized]
		private bool[] m_SelectedProperties = new bool[0];
		private int[] m_EditedCurves = new int[0];
		private Vector2 m_PropertyViewScroll;
		private bool m_CurveEditorToggleChanged;
		private bool m_AutoRecord;
		private bool m_PlayFromNoMode;
		private float m_PrevRealTime;
		private CurveMenuManager m_MenuManager;
		private bool m_PerformFrameSelectedOnCurveEditor = true;
		private bool m_PerformFrameSelectedOnCurveEditorHorizontally;
		internal static AnimationWindow.Styles ms_Styles;
		private SplitterState m_HorizontalSplitter;
		private float indentToContent
		{
			get
			{
				return 2f + EditorGUI.indent + 15f;
			}
		}
		[SerializeField]
		public AnimationWindowState state
		{
			get;
			set;
		}
		public SerializedStringTable expandedFoldouts
		{
			get
			{
				return this.m_ExpandedFoldouts;
			}
			set
			{
				this.m_ExpandedFoldouts = value;
			}
		}
		public SerializedStringTable chosenAnimated
		{
			get
			{
				return this.m_ChosenAnimated;
			}
			set
			{
				this.m_ChosenAnimated = value;
			}
		}
		public SerializedStringTable chosenClip
		{
			get
			{
				return this.m_ChosenClip;
			}
			set
			{
				this.m_ChosenClip = value;
			}
		}
		public bool showAllProperties
		{
			get
			{
				return this.m_ShowAllProperties;
			}
		}
		public float time
		{
			get
			{
				return this.state.FrameToTime((float)this.state.m_Frame);
			}
		}
		public float timeFloor
		{
			get
			{
				return this.state.FrameToTimeFloor((float)this.state.m_Frame);
			}
		}
		public float timeCeiling
		{
			get
			{
				return this.state.FrameToTimeCeiling((float)this.state.m_Frame);
			}
		}
		public static List<AnimationWindow> GetAllAnimationWindows()
		{
			return AnimationWindow.s_AnimationWindows;
		}
		internal Rect GetFoldoutRect(float width)
		{
			return new Rect(2f, (float)AnimationWindow.vPosition, width - 2f - 22f, 15f);
		}
		internal Rect GetFoldoutTextRect(float width)
		{
			float indentToContent = this.indentToContent;
			return new Rect(indentToContent, (float)AnimationWindow.vPosition, width - indentToContent - 22f, 15f);
		}
		internal Vector2 GetPropertyPos(float width)
		{
			return new Vector2(this.indentToContent + 10f, (float)AnimationWindow.vPosition);
		}
		internal Rect GetPropertyLabelRect(float width)
		{
			return this.GetPropertyLabelRect(width, this.indentToContent + 10f);
		}
		internal Rect GetPropertyLabelRect(float width, float pixelIndent)
		{
			return new Rect(pixelIndent, (float)(AnimationWindow.vPosition - 3), width - pixelIndent - 22f - 45f - 15f, 14f);
		}
		internal Rect GetPropertySelectionRect(float width)
		{
			return new Rect(0f, (float)AnimationWindow.vPosition, width - 22f - 45f - 15f, 12f);
		}
		internal Rect GetFieldRect(float width)
		{
			return new Rect(width - 22f - 45f - 5f, (float)AnimationWindow.vPosition, 45f, 12f);
		}
		internal Rect GetIconRect(float width)
		{
			return new Rect(width - 22f + 2f, (float)AnimationWindow.vPosition, 11f, 12f);
		}
		internal Rect GetIconButtonRect(float width)
		{
			return new Rect(width - 22f, (float)AnimationWindow.vPosition, 22f, 12f);
		}
		internal void DrawRowBackground(int width, int height)
		{
			this.DrawRowBackground(width, height, false);
		}
		internal void DrawRowBackground(int width, int height, bool selected)
		{
			AnimationWindow.kEvenRow = !AnimationWindow.kEvenRow;
			GUIStyle gUIStyle = (!AnimationWindow.kEvenRow) ? AnimationWindow.ms_Styles.rowOdd : AnimationWindow.ms_Styles.rowEven;
			gUIStyle.Draw(new Rect(0f, (float)AnimationWindow.vPosition, (float)width, (float)height), false, false, selected, false);
		}
		public void SetDirtyCurves()
		{
			this.state.m_CurveEditorIsDirty = true;
		}
		private void ToggleAnimationMode()
		{
			if (AnimationMode.InAnimationMode())
			{
				this.EndAnimationMode();
			}
			else
			{
				this.BeginAnimationMode(true);
			}
			if (Toolbar.get != null)
			{
				Toolbar.get.Repaint();
			}
		}
		public bool EnsureAnimationMode()
		{
			return AnimationMode.InAnimationMode() || this.BeginAnimationMode(true);
		}
		public void ReEnterAnimationMode()
		{
			if (AnimationMode.InAnimationMode())
			{
				int frame = this.state.m_Frame;
				this.EndAnimationMode();
				this.state.m_Frame = frame;
				this.BeginAnimationMode(false);
			}
		}
		public bool AllHaveClips()
		{
			AnimationSelection[] selected = AnimationWindow.m_Selected;
			for (int i = 0; i < selected.Length; i++)
			{
				AnimationSelection animationSelection = selected[i];
				if (animationSelection.clip == null)
				{
					return false;
				}
			}
			return true;
		}
		public static bool EnsureAllHaveClips()
		{
			AnimationSelection[] selected = AnimationWindow.m_Selected;
			for (int i = 0; i < selected.Length; i++)
			{
				AnimationSelection animationSelection = selected[i];
				if (!animationSelection.EnsureClipPresence())
				{
					return false;
				}
			}
			return true;
		}
		public bool SelectionIsActive()
		{
			return AnimationWindow.m_Selected != null && AnimationWindow.m_Selected.Length != 0 && AnimationWindow.m_Selected[0] != null && !(AnimationWindow.m_Selected[0].clip == null);
		}
		public bool BeginAnimationMode(bool askUserIfMissingClips)
		{
			if (AnimationWindow.m_Selected.Length == 0 || !AnimationWindow.m_Selected[0].GameObjectIsAnimatable)
			{
				return false;
			}
			if (!askUserIfMissingClips && !this.AllHaveClips())
			{
				return false;
			}
			if (askUserIfMissingClips && !AnimationWindow.EnsureAllHaveClips())
			{
				return false;
			}
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			AnimationSelection[] selected = AnimationWindow.m_Selected;
			for (int i = 0; i < selected.Length; i++)
			{
				AnimationSelection animationSelection = selected[i];
				UnityEngine.Object gameObject = animationSelection.animatedObject.transform.root.gameObject;
				if (!list.Contains(gameObject))
				{
					list.Add(gameObject);
				}
			}
			AnimationMode.StartAnimationMode();
			this.ResampleAnimation();
			this.SetAutoRecordMode(true);
			this.SetDirtyCurves();
			Tools.RepaintAllToolViews();
			return true;
		}
		public void EndAnimationMode()
		{
			if (!AnimationMode.InAnimationMode())
			{
				return;
			}
			if (this.state.m_AnimationIsPlaying)
			{
				this.Stop();
			}
			this.state.m_Frame = 0;
			AnimationMode.StopAnimationMode();
			this.SetDirtyCurves();
			Tools.RepaintAllToolViews();
		}
		private void SetPlayMode(bool play)
		{
			if (play != this.state.m_AnimationIsPlaying)
			{
				if (this.state.m_AnimationIsPlaying)
				{
					this.Stop();
				}
				else
				{
					this.Play();
				}
			}
		}
		private void Play()
		{
			bool playFromNoMode = !AnimationMode.InAnimationMode();
			if (!this.EnsureAnimationMode())
			{
				return;
			}
			this.m_PlayFromNoMode = playFromNoMode;
			this.state.m_PlayTime = Mathf.Max(new float[]
			{
				0f,
				this.state.minTime,
				this.state.GetTimeSeconds()
			});
			this.state.m_AnimationIsPlaying = true;
			this.m_PrevRealTime = Time.realtimeSinceStartup;
		}
		private void Stop()
		{
			this.state.m_Frame = Mathf.RoundToInt(this.state.m_PlayTime * this.state.frameRate);
			this.state.m_AnimationIsPlaying = false;
			if (this.m_PlayFromNoMode)
			{
				this.EndAnimationMode();
			}
			else
			{
				this.ReEnterAnimationMode();
			}
			this.m_PlayFromNoMode = false;
		}
		private void Update()
		{
			if (this.state == null)
			{
				return;
			}
			if (this.state.m_AnimationIsPlaying)
			{
				if (!this.SelectionIsActive())
				{
					return;
				}
				float num = Time.realtimeSinceStartup - this.m_PrevRealTime;
				float num2 = 0f;
				float length = this.state.m_ActiveAnimationClip.length;
				this.state.m_PlayTime += num;
				if (this.state.m_PlayTime > length)
				{
					this.state.m_PlayTime = num2;
				}
				this.state.m_PlayTime = Mathf.Clamp(this.state.m_PlayTime, num2, length);
				this.m_PrevRealTime = Time.realtimeSinceStartup;
				this.state.m_Frame = Mathf.RoundToInt(this.state.m_PlayTime * this.state.frameRate);
				this.ResampleAnimation();
				base.Repaint();
			}
			if (this.m_DopeSheetEditor != null && this.m_DopeSheetEditor.m_SpritePreviewLoading)
			{
				base.Repaint();
			}
		}
		private void Next()
		{
			List<AnimationWindowCurve> list = (!this.state.m_ShowCurveEditor) ? this.state.allCurves : this.state.activeCurves;
			this.state.m_PlayTime = this.state.SnapToFrame(AnimationWindowUtility.GetNextKeyframeTime(list.ToArray(), this.state.FrameToTime((float)this.state.m_Frame), this.state.frameRate));
			this.state.m_Frame = this.state.TimeToFrameFloor(this.state.m_PlayTime);
			this.PreviewFrame(this.state.m_Frame);
		}
		private void Prev()
		{
			List<AnimationWindowCurve> list = (!this.state.m_ShowCurveEditor) ? this.state.allCurves : this.state.activeCurves;
			this.state.m_PlayTime = this.state.SnapToFrame(AnimationWindowUtility.GetPreviousKeyframeTime(list.ToArray(), this.state.FrameToTime((float)this.state.m_Frame), this.state.frameRate));
			this.state.m_Frame = this.state.TimeToFrameFloor(this.state.m_PlayTime);
			this.PreviewFrame(this.state.m_Frame);
		}
		public void PreviewFrame(int frame)
		{
			if (!this.EnsureAnimationMode())
			{
				return;
			}
			this.state.m_Frame = frame;
			this.ResampleAnimation();
		}
		private void UndoRedoPerformed()
		{
			if (AnimationMode.InAnimationMode())
			{
				this.PreviewFrame(this.state.m_Frame);
			}
			this.SetDirtyCurves();
			base.Repaint();
		}
		public bool GetAutoRecordMode()
		{
			return this.m_AutoRecord;
		}
		private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
		{
			return AnimationRecording.Process(this.state, modifications);
		}
		public void SetAutoRecordMode(bool record)
		{
			if (this.m_AutoRecord != record)
			{
				if (record)
				{
					record = AnimationWindow.EnsureAllHaveClips();
					Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				}
				else
				{
					Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				}
				this.m_AutoRecord = record;
				if (this.m_AutoRecord)
				{
					this.EnsureAnimationMode();
				}
			}
		}
		private bool IsLinked(CurveState state, bool onlyLinkEuler)
		{
			return state.type == typeof(Transform) && (!onlyLinkEuler || state.propertyName.StartsWith("localEulerAngles."));
		}
		private bool AnyPropertiesSelected()
		{
			bool[] selectedProperties = this.m_SelectedProperties;
			for (int i = 0; i < selectedProperties.Length; i++)
			{
				bool flag = selectedProperties[i];
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
		private void InitSelection()
		{
			this.ClearShownProperties();
			this.GenerateAnimationSelections();
			this.EvaluateFramerate();
			this.SetDirtyCurves();
			if (this.state.m_ActiveAnimationClip != null || this.state.m_ActiveGameObject != null)
			{
				base.Repaint();
			}
		}
		private void RefreshAnimationSelections()
		{
			AnimationSelection[] selected = AnimationWindow.m_Selected;
			for (int i = 0; i < selected.Length; i++)
			{
				AnimationSelection animationSelection = selected[i];
				animationSelection.Refresh();
			}
		}
		private AnimationSelection GetOrAddAnimationSelectionWithObjects(GameObject[] animatedOptions, List<AnimationSelection> animSelected)
		{
			AnimationSelection animationSelection = null;
			string objectListHashCode = AnimationSelection.GetObjectListHashCode(animatedOptions);
			foreach (AnimationSelection current in animSelected)
			{
				if (AnimationSelection.GetObjectListHashCode(current.animatedOptions) == objectListHashCode)
				{
					animationSelection = current;
					break;
				}
			}
			if (animationSelection == null)
			{
				animationSelection = new AnimationSelection(animatedOptions, this.m_ChosenAnimated, this.m_ChosenClip, this);
				animSelected.Add(animationSelection);
			}
			return animationSelection;
		}
		private GameObject[] GetAllParents(Transform tr)
		{
			List<GameObject> list = new List<GameObject>();
			list.Add(tr.gameObject);
			while (tr != tr.root)
			{
				tr = tr.parent;
				list.Add(tr.gameObject);
			}
			list.Reverse();
			return list.ToArray();
		}
		private GameObject[] GetAnimationComponentsInAllParents(Transform tr)
		{
			List<GameObject> list = new List<GameObject>();
			while (true)
			{
				if (tr.animation || tr.GetComponent<Animator>())
				{
					list.Add(tr.gameObject);
				}
				if (tr == tr.root)
				{
					break;
				}
				tr = tr.parent;
			}
			list.Reverse();
			return list.ToArray();
		}
		private GameObject[] GetTrackedGameObjects()
		{
			List<GameObject> list = new List<GameObject>();
			if (AnimationWindow.m_Selected.Length > 0)
			{
				FoldoutTree foldoutTree = AnimationWindow.m_Selected[0].trees[0];
				for (int i = 0; i < foldoutTree.states.Length; i++)
				{
					GameObject obj = foldoutTree.states[i].obj;
					if (obj != null)
					{
						list.Add(obj);
					}
				}
			}
			return list.ToArray();
		}
		private void GenerateAnimationSelections()
		{
			bool flag = AnimationMode.InAnimationMode();
			bool flag2 = false;
			int frame = this.state.m_Frame;
			this.EndAnimationMode();
			List<AnimationSelection> list = new List<AnimationSelection>();
			GameObject gameObject = (!(Selection.activeGameObject != null)) ? this.state.m_ActiveGameObject : Selection.activeGameObject;
			Transform transform = (!gameObject) ? null : gameObject.transform;
			if (transform == null)
			{
				transform = ((!(this.state.m_RootGameObject != null)) ? null : this.state.m_RootGameObject.transform);
				this.state.m_ActiveGameObject = this.state.m_RootGameObject;
				this.state.refresh = AnimationWindowState.RefreshType.Everything;
			}
			if (transform != null)
			{
				GameObject[] array = this.GetAnimationComponentsInAllParents(transform);
				if (array.Length == 0)
				{
					array = this.GetAllParents(transform);
				}
				AnimationSelection orAddAnimationSelectionWithObjects = this.GetOrAddAnimationSelectionWithObjects(array, list);
				if (AnimationWindow.m_Selected != null)
				{
					AnimationSelection[] selected = AnimationWindow.m_Selected;
					for (int i = 0; i < selected.Length; i++)
					{
						AnimationSelection animationSelection = selected[i];
						if (animationSelection.animatedObject == orAddAnimationSelectionWithObjects.animatedObject)
						{
							flag2 = true;
						}
					}
				}
				FoldoutTree tree = new FoldoutTree(transform, this.m_ExpandedFoldouts);
				orAddAnimationSelectionWithObjects.AddTree(tree);
			}
			AnimationWindow.m_Selected = list.ToArray();
			if (AnimationWindow.m_Selected != null && AnimationWindow.m_Selected.Length > 0)
			{
				if (Selection.activeGameObject != null)
				{
					this.state.m_ActiveGameObject = Selection.activeGameObject;
				}
				this.state.m_ActiveAnimationClip = AnimationWindow.m_Selected[0].clip;
				this.state.m_RootGameObject = AnimationWindow.m_Selected[0].avatarRootObject;
				this.state.m_AnimatedGameObject = AnimationWindow.m_Selected[0].animatedObject;
			}
			else
			{
				this.state.m_ActiveAnimationClip = null;
				this.state.m_RootGameObject = null;
				this.state.m_AnimatedGameObject = null;
				this.state.m_ActiveGameObject = null;
				this.state.refresh = AnimationWindowState.RefreshType.Everything;
			}
			if (flag && flag2)
			{
				this.state.m_Frame = frame;
				this.BeginAnimationMode(false);
			}
			if (!flag2)
			{
				AnimationEventPopup.ClosePopup();
				this.m_AnimationEventTimeLine.DeselectAll();
			}
		}
		public void UpdateFrame(int frame)
		{
			if (this.state.m_AnimationIsPlaying || !AnimationMode.InAnimationMode())
			{
				return;
			}
			this.PreviewFrame(frame);
		}
		public void UpdateTime(float time)
		{
			if (this.state.m_AnimationIsPlaying || !AnimationMode.InAnimationMode())
			{
				return;
			}
			this.state.m_Frame = Mathf.RoundToInt(this.state.TimeToFrame(time));
			this.PreviewFrame(this.state.m_Frame);
		}
		public void UpdateCurves(List<int> curveIds, string undoText)
		{
			for (int i = 0; i < this.m_ShownProperties.Length; i++)
			{
				CurveState curveState = this.m_ShownProperties[i];
				if (curveIds.Contains(curveState.GetID()))
				{
					if (!curveState.animated)
					{
						curveState.animationSelection.EnsureClipPresence();
					}
					if (curveState.clip == null)
					{
						Debug.LogError("clip is null");
					}
					curveState.SaveCurve(curveState.curve);
				}
			}
			this.SetDirtyCurves();
		}
		private void ResampleAnimation()
		{
			if (!this.EnsureAnimationMode())
			{
				return;
			}
			Undo.FlushUndoRecordObjects();
			AnimationMode.BeginSampling();
			AnimationSelection[] selected = AnimationWindow.m_Selected;
			for (int i = 0; i < selected.Length; i++)
			{
				AnimationSelection animationSelection = selected[i];
				AnimationClip clip = animationSelection.clip;
				if (!(clip == null))
				{
					GameObject animatedObject = animationSelection.animatedObject;
					if (!(animatedObject == null))
					{
						AnimationMode.SampleAnimationClip(animatedObject, clip, this.state.GetTimeSeconds());
					}
				}
			}
			AnimationMode.EndSampling();
			SceneView.RepaintAll();
		}
		private bool EvaluateFramerate()
		{
			if (AnimationWindow.m_Selected.Length == 0)
			{
				this.state.frameRate = Mathf.Abs(this.state.frameRate);
				return true;
			}
			float num = Mathf.Abs(this.state.frameRate);
			int num2 = 0;
			float a = 0f;
			bool flag = true;
			AnimationSelection[] selected = AnimationWindow.m_Selected;
			for (int i = 0; i < selected.Length; i++)
			{
				AnimationSelection animationSelection = selected[i];
				if (animationSelection.clip != null)
				{
					a = Mathf.Max(a, animationSelection.clip.length);
					if (num2 == 0)
					{
						num2 = Mathf.RoundToInt(animationSelection.clip.frameRate);
					}
					else
					{
						if (animationSelection.clip.frameRate != (float)num2)
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (num2 == 0)
			{
				num2 = 60;
			}
			if (!flag)
			{
				this.state.frameRate = -num;
				return false;
			}
			if (this.state.frameRate != (float)num2)
			{
				this.state.frameRate = (float)num2;
			}
			float num3 = this.state.frameRate / num;
			this.state.m_Frame = Mathf.RoundToInt(num3 * (float)this.state.m_Frame);
			this.m_CurveEditor.hTicks.SetTickModulosForFrameRate(this.state.frameRate);
			return true;
		}
		private void ClearShownProperties()
		{
			this.m_ShownProperties = new CurveState[0];
			this.m_EditedCurves = new int[0];
			this.m_SelectedProperties = new bool[0];
			this.m_CurveEditor.animationCurves = new CurveWrapper[0];
			CurveRendererCache.ClearCurveRendererCache();
		}
		public void RefreshShownCurves(bool forceUpdate)
		{
			this.state.m_CurveEditorIsDirty = false;
			if (!this.SelectionIsActive())
			{
				this.ClearShownProperties();
				return;
			}
			if (this.state.m_ShowCurveEditor || forceUpdate)
			{
				this.SetupEditorCurvesHack();
			}
			else
			{
				this.ClearShownProperties();
			}
			this.EvaluateFramerate();
			this.m_CurveEditor.invSnap = this.state.frameRate;
			bool flag = this.AnyPropertiesSelected();
			CurveWrapper[] array = new CurveWrapper[this.m_EditedCurves.Length];
			for (int i = 0; i < array.Length; i++)
			{
				CurveState curveState = this.m_ShownProperties[this.m_EditedCurves[i]];
				array[i] = new CurveWrapper();
				array[i].id = curveState.GetID();
				if (this.IsLinked(curveState, true))
				{
					array[i].groupId = curveState.GetGroupID();
				}
				else
				{
					array[i].groupId = -1;
				}
				array[i].color = curveState.color;
				array[i].hidden = (flag && !this.m_SelectedProperties[this.m_EditedCurves[i]]);
				if (array[i].readOnly)
				{
					array[i].color.a = 0.3f;
				}
				array[i].renderer = CurveRendererCache.GetCurveRenderer(curveState.clip, curveState.curveBinding);
				array[i].renderer.SetWrap((!curveState.clip.isLooping) ? WrapMode.Once : WrapMode.Loop);
				array[i].renderer.SetCustomRange(0f, curveState.clip.length);
			}
			this.m_CurveEditor.animationCurves = array;
			if (AnimationMode.InAnimationMode() && GUI.changed)
			{
				this.ResampleAnimation();
			}
		}
		public void SetupEditorCurvesHack()
		{
			if (this.SelectionIsActive())
			{
				List<CurveState> list = new List<CurveState>();
				List<AnimationWindowCurve> activeCurves = this.state.activeCurves;
				foreach (AnimationWindowCurve current in activeCurves)
				{
					if (!current.isPPtrCurve)
					{
						CurveState curveState = new CurveState(current.binding);
						curveState.animationSelection = AnimationWindow.m_Selected[0];
						curveState.animated = true;
						curveState.color = CurveUtility.GetPropertyColor(curveState.curveBinding.propertyName);
						list.Add(curveState);
					}
				}
				this.m_ShownProperties = list.ToArray();
				this.m_EditedCurves = new int[this.m_ShownProperties.Length];
				for (int i = 0; i < this.m_EditedCurves.Length; i++)
				{
					this.m_EditedCurves[i] = i;
				}
			}
			else
			{
				this.m_EditedCurves = new int[0];
				this.m_ShownProperties = new CurveState[0];
			}
		}
		private void FrameSelected()
		{
			this.m_PerformFrameSelectedOnCurveEditor = true;
		}
		private void DopeSheetGUI(Rect position)
		{
			this.m_DopeSheetEditor.rect = position;
			position.height -= 15f;
			this.m_DopeSheetEditor.RecalculateBounds();
			this.m_DopeSheetEditor.BeginViewGUI();
			this.m_DopeSheetEditor.OnGUI(position, this.state.m_hierarchyState.scrollPos * -1f);
			Rect position2 = new Rect(position.xMax, position.yMin, 16f, position.height);
			float bottomValue = Mathf.Max(this.m_DopeSheetEditor.contentHeight, position.height);
			this.state.m_hierarchyState.scrollPos.y = GUI.VerticalScrollbar(position2, this.state.m_hierarchyState.scrollPos.y, position.height, 0f, bottomValue);
			this.m_DopeSheetEditor.EndViewGUI();
		}
		public void TimeLineGUI(Rect rect, bool onlyDopesheetLines, bool sparseLines, bool resetKeyboardControl)
		{
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition) && resetKeyboardControl)
			{
				GUIUtility.keyboardControl = 0;
			}
			Color color = GUI.color;
			GUI.BeginGroup(rect);
			rect.x = 0f;
			rect.y = 0f;
			if (Event.current.type != EventType.Repaint)
			{
				GUI.EndGroup();
				return;
			}
			HandleUtility.handleWireMaterial.SetPass(0);
			GL.Begin(1);
			Color backgroundColor = GUI.backgroundColor;
			if (sparseLines)
			{
				this.m_CurveEditor.hTicks.SetTickStrengths((float)this.m_CurveEditor.settings.hTickStyle.distMin, (float)this.m_CurveEditor.settings.hTickStyle.distFull, true);
			}
			else
			{
				this.m_CurveEditor.hTicks.SetTickStrengths(3f, 80f, true);
			}
			Color color2 = this.m_CurveEditor.settings.hTickStyle.color;
			color2.a = 0.75f;
			float num = rect.width / this.state.frameSpan;
			float minFrame = this.state.minFrame;
			for (int i = 0; i < this.m_CurveEditor.hTicks.tickLevels; i++)
			{
				float num2 = this.m_CurveEditor.hTicks.GetStrengthOfLevel(i) * 0.9f;
				float[] ticksAtLevel = this.m_CurveEditor.hTicks.GetTicksAtLevel(i, true);
				for (int j = 0; j < ticksAtLevel.Length; j++)
				{
					if (ticksAtLevel[j] >= 0f)
					{
						int num3 = Mathf.RoundToInt(ticksAtLevel[j] * this.state.frameRate);
						float num4 = 17f * Mathf.Min(1f, num2) * 0.7f;
						float x = ((float)num3 - minFrame) * num;
						if (!onlyDopesheetLines)
						{
							GL.Color(new Color(1f, 1f, 1f, num2 / 0.5f) * color2);
							GL.Vertex(new Vector3(x, 17f - num4 + 0.5f, 0f));
							GL.Vertex(new Vector3(x, 16.5f, 0f));
						}
						if (onlyDopesheetLines)
						{
							GL.Color(new Color(1f, 1f, 1f, num2 / 0.5f) * 0.37f);
							GL.Vertex(new Vector3(x, rect.yMin, 0f));
							GL.Vertex(new Vector3(x, rect.yMax, 0f));
						}
						else
						{
							GL.Color(new Color(1f, 1f, 1f, num2 / 0.5f) * 0.4f);
							GL.Vertex(new Vector3(x, rect.yMin + 17f + 18f, 0f));
							GL.Vertex(new Vector3(x, rect.yMax, 0f));
						}
					}
				}
			}
			GL.End();
			if (!onlyDopesheetLines)
			{
				int length = ((int)this.state.frameRate).ToString().Length;
				int levelWithMinSeparation = this.m_CurveEditor.hTicks.GetLevelWithMinSeparation(40f);
				float[] ticksAtLevel2 = this.m_CurveEditor.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
				for (int k = 0; k < ticksAtLevel2.Length; k++)
				{
					if (ticksAtLevel2[k] >= 0f)
					{
						int num5 = Mathf.RoundToInt(ticksAtLevel2[k] * this.state.frameRate);
						float num6 = Mathf.Floor(this.state.FrameToPixel((float)num5, rect));
						string text = this.state.FormatFrame(num5, length);
						GUI.Label(new Rect(num6 + 3f, -3f, 40f, 20f), text, AnimationWindow.ms_Styles.TimelineTick);
					}
				}
			}
			GUI.EndGroup();
			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
		}
		private void SecondaryTickMarksGUI(Rect rect)
		{
			GUI.BeginGroup(rect);
			if (Event.current.type != EventType.Repaint)
			{
				GUI.EndGroup();
				return;
			}
			this.m_CurveEditor.hTicks.SetTickStrengths((float)this.m_CurveEditor.settings.hTickStyle.distMin, (float)this.m_CurveEditor.settings.hTickStyle.distFull, false);
			HandleUtility.handleWireMaterial.SetPass(0);
			GL.Begin(1);
			for (int i = 0; i < this.m_CurveEditor.hTicks.tickLevels; i++)
			{
				float strengthOfLevel = this.m_CurveEditor.hTicks.GetStrengthOfLevel(i);
				GL.Color(this.m_CurveEditor.settings.hTickStyle.color * new Color(1f, 1f, 1f, strengthOfLevel) * new Color(1f, 1f, 1f, 0.75f));
				float[] ticksAtLevel = this.m_CurveEditor.hTicks.GetTicksAtLevel(i, true);
				for (int j = 0; j < ticksAtLevel.Length; j++)
				{
					if (ticksAtLevel[j] >= 0f)
					{
						int num = Mathf.RoundToInt(ticksAtLevel[j] * this.state.frameRate);
						GL.Vertex(new Vector2(this.state.FrameToPixel((float)num, rect), 0f));
						GL.Vertex(new Vector2(this.state.FrameToPixel((float)num, rect), rect.height));
					}
				}
			}
			GL.End();
			GUI.EndGroup();
		}
		public void OnGUI()
		{
			if (this.state.AnimatorIsOptimized)
			{
				GUILayout.Label("Editing optimized game object hierarchy is not supported.\nPlease select a game object that does not have 'Optimize Game Objects' applied.", new GUILayoutOption[0]);
				return;
			}
			this.state.OnGUI();
			this.InitAllViews();
			if (this.state.m_ActiveGameObject == null)
			{
				AnimationWindow.m_Selected = null;
				this.state.m_ShowCurveEditor = false;
			}
			if (AnimationWindow.m_Selected == null)
			{
				this.OnSelectionChange();
			}
			if (AnimationWindow.m_Selected.Length == 0)
			{
				this.EndAnimationMode();
			}
			if (this.state == null)
			{
				return;
			}
			if (this.m_CurveEditorToggleChanged)
			{
				this.m_CurveEditorToggleChanged = false;
				this.state.m_ShowCurveEditor = !this.state.m_ShowCurveEditor;
				this.HandleEmptyCurveEditor();
			}
			this.state.timeArea = ((!this.state.m_ShowCurveEditor) ? this.m_DopeSheetEditor : this.m_CurveEditor);
			if (this.state.m_CurveEditorIsDirty)
			{
				CurveRendererCache.ClearCurveRendererCache();
				this.RefreshShownCurves(false);
				if (AnimationMode.InAnimationMode())
				{
					this.ResampleAnimation();
				}
			}
			if (this.m_PerformFrameSelectedOnCurveEditor)
			{
				if (this.state.m_ShowCurveEditor)
				{
					this.m_CurveEditor.FrameSelected(this.m_PerformFrameSelectedOnCurveEditorHorizontally, true);
				}
				this.m_PerformFrameSelectedOnCurveEditor = false;
				this.m_PerformFrameSelectedOnCurveEditorHorizontally = false;
			}
			int num = this.m_HorizontalSplitter.realSizes[0];
			Rect rect = new Rect((float)num, 0f, base.position.width - (float)num, base.position.height);
			Rect rect2 = new Rect((float)num, 0f, rect.width - 15f, 17f);
			Rect rect3 = new Rect((float)num, rect2.yMax, rect.width - 15f, 18f);
			Rect area = new Rect((float)num, rect2.yMin, rect.width - 15f, rect3.yMax);
			float width = (!this.state.m_ShowCurveEditor) ? (rect.width - 15f) : rect.width;
			Rect rect4 = new Rect((float)num, rect3.yMax, width, rect.height - 17f - 18f);
			Rect position = (!this.state.m_ShowCurveEditor) ? rect4 : default(Rect);
			Rect rect5 = (!this.state.m_ShowCurveEditor) ? new Rect((float)num, rect4.yMax, rect4.width, 0f) : rect4;
			Rect rect6 = (!this.state.m_ShowCurveEditor) ? rect5 : new Rect(rect5.xMin, rect5.yMin, rect5.width, rect5.height - 15f);
			Rect position2 = new Rect((float)num, 0f, rect.width - 15f, rect.height - rect5.height - 15f);
			this.m_CurveEditor.rect = rect5;
			if (AnimationWindow.ms_Styles == null)
			{
				AnimationWindow.ms_Styles = new AnimationWindow.Styles();
			}
			Handles.lighting = false;
			if (!AnimationMode.InAnimationMode())
			{
				this.SetAutoRecordMode(false);
				this.state.m_Frame = 0;
			}
			GUI.changed = false;
			AnimationWindow.vPosition = 0;
			AnimationWindow.kEvenRow = true;
			SplitterGUILayout.BeginHorizontalSplit(this.m_HorizontalSplitter, new GUILayoutOption[0]);
			EditorGUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.MaxWidth((float)num)
			});
			GUI.changed = false;
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
			{
				GUILayout.Width((float)num)
			});
			EditorGUI.BeginDisabledGroup(!this.state.m_ActiveGameObject || this.state.IsPrefab);
			if (this.m_PlayFromNoMode)
			{
				bool flag = GUILayout.Toggle(false, AnimationWindow.ms_Styles.recordContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
				if (flag)
				{
					this.Stop();
					this.ToggleAnimationMode();
				}
			}
			else
			{
				Color color = GUI.color;
				if (AnimationMode.InAnimationMode())
				{
					GUI.color = color * AnimationMode.animatedPropertyColor;
				}
				bool flag2 = GUILayout.Toggle(AnimationMode.InAnimationMode(), AnimationWindow.ms_Styles.recordContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
				GUI.color = color;
				if (flag2 != AnimationMode.InAnimationMode())
				{
					this.ToggleAnimationMode();
				}
				GUI.color = color;
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(this.state.IsPrefab);
			this.SetPlayMode(GUILayout.Toggle(this.state.m_AnimationIsPlaying, AnimationWindow.ms_Styles.playContent, EditorStyles.toolbarButton, new GUILayoutOption[0]));
			GUILayout.FlexibleSpace();
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(!this.state.m_ActiveGameObject || !this.state.m_ActiveAnimationClip || this.state.IsPrefab);
			if (GUILayout.Button(AnimationWindow.ms_Styles.prevKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.Prev();
			}
			if (GUILayout.Button(AnimationWindow.ms_Styles.nextKeyContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
			{
				this.Next();
			}
			int num2 = EditorGUILayout.IntField(this.state.m_Frame, EditorStyles.toolbarTextField, new GUILayoutOption[]
			{
				GUILayout.Width(35f)
			});
			num2 = Mathf.Max(0, num2);
			if (num2 != this.state.m_Frame)
			{
				this.PreviewFrame(num2);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(!this.state.IsEditable);
			if ((GUILayout.Button(AnimationWindow.ms_Styles.addKeyframeContent, EditorStyles.toolbarButton, new GUILayoutOption[0]) || AnimationWindow.kAnimationRecordKeyframe.activated) && this.EnsureAnimationMode())
			{
				AnimationWindowUtility.AddSelectedKeyframes(this.state, this.state.time);
			}
			if (GUILayout.Button(AnimationWindow.ms_Styles.addEventContent, EditorStyles.toolbarButton, new GUILayoutOption[0]) && AnimationWindow.m_Selected.Length > 0)
			{
				AnimationSelection animationSelection = AnimationWindow.m_Selected[0];
				if (animationSelection.EnsureClipPresence())
				{
					this.m_CurveEditor.SelectNone();
					this.m_AnimationEventTimeLine.AddEvent(this.state);
					this.SetDirtyCurves();
				}
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[]
			{
				GUILayout.Width((float)(num + 1))
			});
			GUI.changed = false;
			AnimationSelection animSel = (AnimationWindow.m_Selected.Length <= 0) ? null : AnimationWindow.m_Selected[0];
			AnimationSelection.OnGUISelection(animSel);
			EditorGUI.BeginDisabledGroup(this.state.IsReadOnly);
			Rect rect7 = GUILayoutUtility.GetRect(0f, 0f, AnimationWindow.ms_Styles.toolbarLabel, new GUILayoutOption[]
			{
				GUILayout.Width(45f)
			});
			rect7.width += 10f;
			GUI.Label(rect7, "Samples", AnimationWindow.ms_Styles.toolbarLabel);
			EditorGUI.BeginChangeCheck();
			int num3 = EditorGUILayout.IntField((int)this.state.frameRate, EditorStyles.toolbarTextField, new GUILayoutOption[]
			{
				GUILayout.Width(35f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				this.state.frameRate = (float)num3;
			}
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
			GUILayoutUtility.GetRect((float)num, base.position.height - 17f - 18f - 15f);
			if (AnimationWindow.m_Selected.Length > 0)
			{
				this.HierarchyGUI(num, (int)rect4.height - 15);
			}
			Handles.color = ((!EditorGUIUtility.isProSkin) ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.15f, 0.15f, 0.15f));
			Handles.DrawLine(new Vector2((float)num, 35f), new Vector2((float)num, (float)((int)rect4.height - 15 + 35)));
			Color color2 = GUI.color;
			GUI.color = new Color(0.95f, 0.95f, 0.95f, 1f);
			EditorGUILayout.BeginHorizontal(AnimationWindow.ms_Styles.miniToolbar, new GUILayoutOption[]
			{
				GUILayout.Width((float)(num + 1)),
				GUILayout.MinHeight(15f)
			});
			GUILayout.FlexibleSpace();
			EditorGUI.BeginDisabledGroup(this.state.m_ActiveAnimationClip == null);
			EditorGUI.BeginChangeCheck();
			GUILayout.Toggle(!this.state.m_ShowCurveEditor, "Dope Sheet", AnimationWindow.ms_Styles.miniToolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			GUILayout.Toggle(this.state.m_ShowCurveEditor, "Curves", AnimationWindow.ms_Styles.miniToolbarButton, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			if (EditorGUI.EndChangeCheck())
			{
				this.m_CurveEditorToggleChanged = true;
			}
			EditorGUI.EndDisabledGroup();
			GUI.color = color2;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUIUtility.GetControlID(3487653, FocusType.Passive);
			GUI.changed = false;
			EditorGUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			EditorGUILayout.BeginHorizontal("Toolbar", new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUI.Label(rect3, GUIContent.none, AnimationWindow.ms_Styles.eventBackground);
			this.TimeLineGUI(rect3, true, true, false);
			if (this.state.m_ShowCurveEditor)
			{
				GUI.Label(this.m_CurveEditor.drawRect, GUIContent.none, AnimationWindow.ms_Styles.curveEditorBackground);
			}
			this.m_CurveEditor.rect = rect5;
			this.m_CurveEditor.hRangeLocked = Event.current.shift;
			this.m_CurveEditor.vRangeLocked = EditorGUI.actionKey;
			GUI.changed = false;
			if (this.state.maxFrame < this.state.minFrame + 5f)
			{
				this.m_CurveEditor.SetShownHRange(this.state.minTime, this.state.minTime + 5f / this.state.frameRate);
			}
			this.m_CurveEditor.vSlider = this.state.m_ShowCurveEditor;
			this.m_CurveEditor.hSlider = this.state.m_ShowCurveEditor;
			this.m_CurveEditor.BeginViewGUI();
			this.m_CurveEditor.GridGUI();
			if (this.state.m_ShowCurveEditor)
			{
				if (this.state.m_ActiveAnimationClip != null)
				{
					AnimationWindow.DrawEndOfClip(rect6, this.state.TimeToPixel(this.state.m_ActiveAnimationClip.length) + rect6.xMin);
				}
				this.HandlePlayhead((float)num, rect6, 0.6f);
				EditorGUI.BeginDisabledGroup(this.state.IsReadOnly);
				this.m_CurveEditor.CurveGUI();
				EditorGUI.EndDisabledGroup();
			}
			this.m_CurveEditor.EndViewGUI();
			this.TimeLineGUI(rect2, false, false, true);
			if (this.state.m_ActiveAnimationClip != null)
			{
				AnimationWindow.DrawEndOfClip(rect2, this.state.TimeToPixel(this.state.m_ActiveAnimationClip.length) + rect2.xMin);
			}
			this.HandlePlayhead((float)num, rect2);
			if (AnimationWindow.m_Selected.Length > 0)
			{
				EditorGUI.BeginDisabledGroup(!this.state.IsEditable);
				this.HandlePlayhead((float)num, rect3);
				this.m_AnimationEventTimeLine.EventLineGUI(rect3, AnimationWindow.m_Selected[0], this.state, this.m_CurveEditor);
				EditorGUI.EndDisabledGroup();
			}
			if (!this.state.m_ShowCurveEditor)
			{
				this.DopeSheetGUI(position);
				this.m_CurveEditor.SetShownHRange(this.state.minTime, this.state.maxTime);
			}
			if (AnimationWindow.m_Selected.Length > 0 && Event.current.button == 0)
			{
				EditorGUI.BeginChangeCheck();
				int num4 = Mathf.RoundToInt(GUI.HorizontalSlider(position2, (float)this.state.m_Frame, this.state.minFrame, this.state.maxFrame, GUIStyle.none, GUIStyle.none));
				if (position2.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
				{
					num4 = Mathf.RoundToInt(this.state.TimeToFrame(this.state.PixelToTime(Event.current.mousePosition.x - (float)num, false)));
				}
				num4 = Mathf.Max(0, num4);
				if (EditorGUI.EndChangeCheck())
				{
					this.PreviewFrame(num4);
				}
			}
			if (this.m_ShownProperties == null)
			{
				Debug.LogError("m_ShownProperties is null");
			}
			else
			{
				if (this.m_CurveEditor.animationCurves.Length != this.m_EditedCurves.Length)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"animationCurves and m_EditedCurves not same length! (",
						this.m_CurveEditor.animationCurves.Length,
						" vs ",
						this.m_EditedCurves.Length,
						")"
					}));
				}
				else
				{
					if (AnimationWindow.m_Selected.Length > 0 && AnimationWindow.m_Selected[0] != null && this.m_ShownProperties.Length > 0)
					{
						int num5 = 0;
						for (int i = 0; i < this.m_EditedCurves.Length; i++)
						{
							if (this.m_CurveEditor.animationCurves[i] == null)
							{
								Debug.LogError("Curve " + i + " is null");
							}
							CurveState curveState = this.m_ShownProperties[this.m_EditedCurves[i]];
							if (this.m_CurveEditor.animationCurves[i].changed)
							{
								this.m_CurveEditor.animationCurves[i].changed = false;
								num5++;
								if (this.EnsureAnimationMode())
								{
									curveState.SaveCurve(this.m_CurveEditor.animationCurves[i].curve);
								}
							}
						}
						if (num5 > 0)
						{
							this.SetDirtyCurves();
						}
					}
				}
			}
			EditorGUILayout.EndVertical();
			SplitterGUILayout.EndHorizontalSplit();
			this.HandleZoomingOutsideTimearea(area);
			this.AnimationControls();
			Rect position3 = base.position;
			position3.x = 0f;
			position3.y = 0f;
			if (Event.current.type == EventType.MouseDown && position3.Contains(Event.current.mousePosition) && Event.current.button == 0 && !Event.current.shift && !Event.current.control)
			{
				GUIUtility.keyboardControl = 0;
				if (Event.current.mousePosition.x < (float)num && Event.current.mousePosition.y > 32f)
				{
					for (int j = 0; j < this.m_SelectedProperties.Length; j++)
					{
						this.m_SelectedProperties[j] = false;
					}
					this.SetDirtyCurves();
				}
				Event.current.Use();
			}
			this.m_AnimationEventTimeLine.DrawInstantTooltip(base.position);
		}
		public static void DrawEndOfClip(Rect rect, float endOfClipPixel)
		{
			Rect rect2 = new Rect(Mathf.Max(endOfClipPixel, rect.xMin), rect.yMin, rect.width, rect.height);
			Vector3[] array = new Vector3[]
			{
				new Vector3(rect2.xMin, rect2.yMin),
				new Vector3(rect2.xMax, rect2.yMin),
				new Vector3(rect2.xMax, rect2.yMax),
				new Vector3(rect2.xMin, rect2.yMax)
			};
			Color color = (!EditorGUIUtility.isProSkin) ? Color.gray.AlphaMultiplied(0.42f) : Color.black.AlphaMultiplied(0.32f);
			Color color2 = (!EditorGUIUtility.isProSkin) ? Color.white.RGBMultiplied(0.4f) : Color.white.RGBMultiplied(0.32f);
			AnimationWindow.DrawRect(array, color);
			AnimationWindow.DrawLine(array[0], array[3] + new Vector3(0f, -1f, 0f), color2);
		}
		private void HandlePlayhead(float hierarchyWidth, Rect r)
		{
			this.HandlePlayhead(hierarchyWidth, r, 1f);
		}
		private void HandlePlayhead(float hierarchyWidth, Rect r, float alpha)
		{
			if (AnimationMode.InAnimationMode() && (float)this.state.m_Frame >= this.state.minFrame - 1f && (float)this.state.m_Frame < this.state.maxFrame)
			{
				float num = this.state.TimeToPixel(this.state.GetTimeSeconds(), false) + hierarchyWidth;
				if (r.xMin < num)
				{
					AnimationWindow.DrawPlayHead(new Vector2(num, r.yMin), new Vector2(num, r.yMax), alpha);
				}
			}
		}
		public static void DrawPlayHead(Vector2 start, Vector2 end, float alpha)
		{
			AnimationWindow.DrawLine(start, end, Color.red.AlphaMultiplied(alpha));
		}
		private static void DrawLine(Vector2 p1, Vector2 p2, Color color)
		{
			HandleUtility.handleWireMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(1);
			GL.Color(color);
			GL.Vertex(p1);
			GL.Vertex(p2);
			GL.End();
			GL.PopMatrix();
		}
		private static void DrawRect(Vector3[] corners, Color color)
		{
			if (corners.Length != 4)
			{
				return;
			}
			HandleUtility.handleWireMaterial.SetPass(0);
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(7);
			GL.Color(color);
			GL.Vertex(corners[0]);
			GL.Vertex(corners[1]);
			GL.Vertex(corners[2]);
			GL.Vertex(corners[3]);
			GL.End();
			GL.PopMatrix();
		}
		private void HandleZoomingOutsideTimearea(Rect area)
		{
			if (this.state.m_ShowCurveEditor)
			{
				this.m_CurveEditor.HandleZoomAndPanEvents(area);
			}
			else
			{
				this.m_DopeSheetEditor.HandleZoomAndPanEvents(area);
			}
		}
		private void HandleEmptyCurveEditor()
		{
			if (this.state.m_ShowCurveEditor && this.state.activeCurves.Count == 0 && this.state.dopelines.Count > 0)
			{
				this.state.SelectHierarchyItem(this.state.dopelines[0], false, false);
			}
		}
		private void InitAllViews()
		{
			if (this.m_Hierarchy == null)
			{
				this.m_Hierarchy = new AnimationWindowHierarchy(this.state, this, new Rect(0f, 0f, 1f, 1f));
			}
			if (this.m_DopeSheetEditor == null)
			{
				this.m_DopeSheetEditor = new DopeSheetEditor(this.state, this);
			}
			if (this.m_AnimationEventTimeLine == null)
			{
				this.m_AnimationEventTimeLine = new AnimationEventTimeLine(this);
			}
		}
		private void HierarchyGUI(int hierarchyWidth, int hierarchyHeight)
		{
			if (GUI.changed)
			{
				this.RefreshAnimationSelections();
			}
			Rect position = new Rect(0f, 35f, (float)hierarchyWidth, (float)hierarchyHeight);
			this.m_Hierarchy.OnGUI(position, this);
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}
		private void DebugTime(string str, float timeA, float timeB)
		{
			Debug.Log(str + " took " + (timeB - timeA));
		}
		public void OnSelectionChange()
		{
			if (this.state != null && Selection.activeGameObject && Selection.activeGameObject != this.state.m_ActiveGameObject)
			{
				this.m_PerformFrameSelectedOnCurveEditorHorizontally = true;
				this.FrameSelected();
			}
			this.InitSelection();
		}
		private void SetGridColors()
		{
			CurveEditorSettings curveEditorSettings = new CurveEditorSettings();
			curveEditorSettings.hTickStyle.distMin = 30;
			curveEditorSettings.hTickStyle.distFull = 80;
			curveEditorSettings.hTickStyle.distLabel = 0;
			if (EditorGUIUtility.isProSkin)
			{
				curveEditorSettings.vTickStyle.color = new Color(1f, 1f, 1f, curveEditorSettings.vTickStyle.color.a);
				curveEditorSettings.vTickStyle.labelColor = new Color(1f, 1f, 1f, curveEditorSettings.vTickStyle.labelColor.a);
			}
			curveEditorSettings.vTickStyle.distMin = 15;
			curveEditorSettings.vTickStyle.distFull = 40;
			curveEditorSettings.vTickStyle.distLabel = 30;
			curveEditorSettings.vTickStyle.stubs = true;
			curveEditorSettings.hRangeMin = 0f;
			curveEditorSettings.hRangeLocked = false;
			curveEditorSettings.vRangeLocked = false;
			this.m_CurveEditor.settings = curveEditorSettings;
		}
		private void AnimationControls()
		{
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.KeyDown)
			{
				if (type != EventType.KeyUp)
				{
				}
			}
			else
			{
				if (AnimationWindow.kAnimationPrevFrame.activated)
				{
					this.PreviewFrame(Mathf.Max(0, this.state.m_Frame - 1));
					current.Use();
				}
				if (AnimationWindow.kAnimationNextFrame.activated)
				{
					this.PreviewFrame(this.state.m_Frame + 1);
					current.Use();
				}
				if (AnimationWindow.kAnimationPrevKeyframe.activated)
				{
					this.Prev();
					current.Use();
				}
				if (AnimationWindow.kAnimationNextKeyframe.activated)
				{
					this.Next();
					current.Use();
				}
				if (AnimationWindow.kAnimationRecordKeyframe.activated)
				{
					AnimationWindowUtility.AddSelectedKeyframes(this.state, this.state.time);
					current.Use();
				}
				if (AnimationWindow.kAnimationShowCurvesToggle.activated)
				{
					this.m_CurveEditorToggleChanged = true;
					current.Use();
				}
			}
		}
		public void Awake()
		{
			if (this.state == null)
			{
				this.state = new AnimationWindowState();
				this.state.m_ShowCurveEditor = false;
				this.state.m_AnimationWindow = this;
			}
			this.state.timeArea = null;
			base.minSize = new Vector2(400f, 200f);
			this.m_HorizontalSplitter = new SplitterState(new float[]
			{
				250f,
				10000f
			}, new int[]
			{
				250,
				150
			}, null);
			this.m_HorizontalSplitter.realSizes[0] = 300;
			base.wantsMouseMove = true;
			AnimationWindow.m_Selected = new AnimationSelection[0];
			if (this.m_ExpandedFoldouts == null)
			{
				this.m_ExpandedFoldouts = new SerializedStringTable();
			}
			if (this.m_ChosenAnimated == null)
			{
				this.m_ChosenAnimated = new SerializedStringTable();
			}
			if (this.m_ChosenClip == null)
			{
				this.m_ChosenClip = new SerializedStringTable();
			}
			this.m_CurveEditor = new CurveEditor(new Rect(base.position.x, base.position.y, 500f, 200f), new CurveWrapper[0], false);
			this.SetGridColors();
			this.m_CurveEditor.m_TimeUpdater = this;
			this.m_CurveEditor.m_DefaultBounds = new Bounds(new Vector3(1f, 1f, 0f), new Vector3(2f, 1000f, 0f));
			this.m_CurveEditor.SetShownHRangeInsideMargins(0f, 2f);
			this.m_CurveEditor.hTicks.SetTickModulosForFrameRate(this.state.frameRate);
			this.InitAllViews();
			this.InitSelection();
		}
		public void OnEnable()
		{
			AnimationWindow.s_AnimationWindows.Add(this);
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			if (this.state != null)
			{
				this.state.OnEnable(this);
			}
			AnimationWindowState expr_48 = this.state;
			expr_48.m_OnHierarchySelectionChanged = (Action)Delegate.Combine(expr_48.m_OnHierarchySelectionChanged, new Action(this.FrameSelected));
			if (AnimationWindow.m_Selected == null)
			{
				return;
			}
			if (this.m_CurveEditor != null)
			{
				this.m_CurveEditor.m_TimeUpdater = this;
				this.SetGridColors();
				this.m_CurveEditor.OnEnable();
			}
			if (this.m_AutoRecord)
			{
				Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
			}
			this.state.m_ShowCurveEditor = AnimationWindow.s_LastShowCurveEditor;
			this.SetDirtyCurves();
		}
		public void OnDisable()
		{
			AnimationWindow.s_AnimationWindows.Remove(this);
			if (this.state != null)
			{
				this.state.OnDisable();
			}
			AnimationWindow.s_LastShowCurveEditor = this.state.m_ShowCurveEditor;
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
			AnimationWindowState expr_78 = this.state;
			expr_78.m_OnHierarchySelectionChanged = (Action)Delegate.Remove(expr_78.m_OnHierarchySelectionChanged, new Action(this.FrameSelected));
			this.m_CurveEditor.OnDisable();
		}
		public void OnDestroy()
		{
			this.EndAnimationMode();
			AnimationEventPopup.ClosePopup();
			CurveRendererCache.ClearCurveRendererCache();
			if (this.m_DopeSheetEditor != null)
			{
				this.m_DopeSheetEditor.OnDestroy();
			}
		}
		public void DrawClipsInSceneView()
		{
		}
	}
}
