using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowState : ScriptableObject, IPlayHead, IAnimationRecordingState
	{
		public enum RefreshType
		{
			None,
			CurvesOnly,
			Everything
		}

		public enum SnapMode
		{
			Disabled,
			SnapToFrame,
			SnapToClipFrame
		}

		public const float kDefaultFrameRate = 60f;

		[SerializeField]
		public AnimationWindowHierarchyState hierarchyState;

		[SerializeField]
		public AnimEditor animEditor;

		[SerializeField]
		public bool showCurveEditor;

		[SerializeField]
		public bool timeInFrames = true;

		[SerializeField]
		private float m_CurrentTime;

		[SerializeField]
		private int m_CurrentFrame;

		[SerializeField]
		private TimeArea m_TimeArea;

		[SerializeField]
		private AnimationWindowSelection m_Selection;

		[SerializeField]
		private HashSet<int> m_SelectedKeyHashes;

		[SerializeField]
		private int m_ActiveKeyframeHash;

		[SerializeField]
		private float m_FrameRate = 60f;

		[SerializeField]
		private AnimationWindowPolicy m_Policy;

		private static List<AnimationWindowKeyframe> s_KeyframeClipboard;

		[NonSerialized]
		public AnimationWindowHierarchyDataSource hierarchyData;

		private List<AnimationWindowCurve> m_ActiveCurvesCache;

		private List<DopeLine> m_dopelinesCache;

		private List<AnimationWindowKeyframe> m_SelectedKeysCache;

		private List<CurveWrapper> m_ActiveCurveWrappersCache;

		private AnimationWindowKeyframe m_ActiveKeyframeCache;

		private HashSet<int> m_ModifiedCurves = new HashSet<int>();

		private EditorCurveBinding? m_lastAddedCurveBinding;

		private AnimationRecordMode m_Recording;

		private int m_PreviousRefreshHash;

		private AnimationWindowState.RefreshType m_Refresh;

		public Action<float> onFrameRateChange;

		public AnimationWindowSelection selection
		{
			get
			{
				if (this.m_Selection == null)
				{
					this.m_Selection = new AnimationWindowSelection();
				}
				return this.m_Selection;
			}
		}

		public AnimationWindowSelectionItem selectedItem
		{
			get
			{
				if (this.m_Selection != null && this.m_Selection.count > 0)
				{
					return this.m_Selection.First();
				}
				return null;
			}
			set
			{
				if (this.m_Selection == null)
				{
					this.m_Selection = new AnimationWindowSelection();
				}
				if (value == null)
				{
					this.m_Selection.Clear();
				}
				else
				{
					this.m_Selection.Set(value);
				}
			}
		}

		public AnimationClip activeAnimationClip
		{
			get
			{
				if (this.selectedItem != null)
				{
					return this.selectedItem.animationClip;
				}
				return null;
			}
		}

		public GameObject activeGameObject
		{
			get
			{
				if (this.selectedItem != null)
				{
					return this.selectedItem.gameObject;
				}
				return null;
			}
		}

		public GameObject activeRootGameObject
		{
			get
			{
				if (this.selectedItem != null)
				{
					return this.selectedItem.rootGameObject;
				}
				return null;
			}
		}

		public Component activeAnimationPlayer
		{
			get
			{
				if (this.selectedItem != null)
				{
					return this.selectedItem.animationPlayer;
				}
				return null;
			}
		}

		public bool animatorIsOptimized
		{
			get
			{
				if (!this.activeRootGameObject)
				{
					return false;
				}
				Animator component = this.activeRootGameObject.GetComponent<Animator>();
				return component != null && component.isOptimizable && !component.hasTransformHierarchy;
			}
		}

		public bool locked
		{
			get
			{
				return this.selection.locked;
			}
			set
			{
				this.selection.locked = value;
			}
		}

		public bool disabled
		{
			get
			{
				return this.selection.disabled;
			}
		}

		public AnimationWindowState.RefreshType refresh
		{
			get
			{
				return this.m_Refresh;
			}
			set
			{
				if (this.m_Refresh < value)
				{
					this.m_Refresh = value;
				}
			}
		}

		public List<AnimationWindowCurve> allCurves
		{
			get
			{
				return this.m_Selection.curves;
			}
		}

		public List<AnimationWindowCurve> activeCurves
		{
			get
			{
				if (this.m_ActiveCurvesCache == null)
				{
					this.m_ActiveCurvesCache = new List<AnimationWindowCurve>();
					if (this.hierarchyState != null && this.hierarchyData != null)
					{
						foreach (int current in this.hierarchyState.selectedIDs)
						{
							TreeViewItem treeViewItem = this.hierarchyData.FindItem(current);
							AnimationWindowHierarchyNode animationWindowHierarchyNode = treeViewItem as AnimationWindowHierarchyNode;
							if (animationWindowHierarchyNode != null)
							{
								AnimationWindowCurve[] curves = animationWindowHierarchyNode.curves;
								if (curves != null)
								{
									AnimationWindowCurve[] array = curves;
									for (int i = 0; i < array.Length; i++)
									{
										AnimationWindowCurve item = array[i];
										if (!this.m_ActiveCurvesCache.Contains(item))
										{
											this.m_ActiveCurvesCache.Add(item);
										}
									}
								}
							}
						}
					}
				}
				return this.m_ActiveCurvesCache;
			}
		}

		public List<CurveWrapper> activeCurveWrappers
		{
			get
			{
				if (this.m_ActiveCurveWrappersCache == null || this.m_ActiveCurvesCache == null)
				{
					List<CurveWrapper> list = new List<CurveWrapper>();
					foreach (AnimationWindowCurve current in this.activeCurves)
					{
						if (!current.isPPtrCurve)
						{
							list.Add(AnimationWindowUtility.GetCurveWrapper(current, current.clip));
						}
					}
					if (!list.Any<CurveWrapper>())
					{
						foreach (AnimationWindowCurve current2 in this.allCurves)
						{
							if (!current2.isPPtrCurve)
							{
								list.Add(AnimationWindowUtility.GetCurveWrapper(current2, current2.clip));
							}
						}
					}
					this.m_ActiveCurveWrappersCache = list;
				}
				return this.m_ActiveCurveWrappersCache;
			}
		}

		public List<DopeLine> dopelines
		{
			get
			{
				if (this.m_dopelinesCache == null)
				{
					this.m_dopelinesCache = new List<DopeLine>();
					if (this.hierarchyData != null)
					{
						foreach (TreeViewItem current in this.hierarchyData.GetRows())
						{
							AnimationWindowHierarchyNode animationWindowHierarchyNode = current as AnimationWindowHierarchyNode;
							if (animationWindowHierarchyNode != null && !(animationWindowHierarchyNode is AnimationWindowHierarchyAddButtonNode))
							{
								AnimationWindowCurve[] curves = animationWindowHierarchyNode.curves;
								if (curves != null)
								{
									DopeLine dopeLine = new DopeLine(current.id, curves);
									dopeLine.tallMode = this.hierarchyState.GetTallMode(animationWindowHierarchyNode);
									dopeLine.objectType = animationWindowHierarchyNode.animatableObjectType;
									dopeLine.hasChildren = !(animationWindowHierarchyNode is AnimationWindowHierarchyPropertyNode);
									dopeLine.isMasterDopeline = (current is AnimationWindowHierarchyMasterNode);
									this.m_dopelinesCache.Add(dopeLine);
								}
							}
						}
					}
				}
				return this.m_dopelinesCache;
			}
		}

		public List<AnimationWindowHierarchyNode> selectedHierarchyNodes
		{
			get
			{
				List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
				if (this.activeAnimationClip != null && this.hierarchyData != null)
				{
					foreach (int current in this.hierarchyState.selectedIDs)
					{
						AnimationWindowHierarchyNode animationWindowHierarchyNode = (AnimationWindowHierarchyNode)this.hierarchyData.FindItem(current);
						if (animationWindowHierarchyNode != null && !(animationWindowHierarchyNode is AnimationWindowHierarchyAddButtonNode))
						{
							list.Add(animationWindowHierarchyNode);
						}
					}
				}
				return list;
			}
		}

		public AnimationWindowKeyframe activeKeyframe
		{
			get
			{
				if (this.m_ActiveKeyframeCache == null)
				{
					foreach (AnimationWindowCurve current in this.allCurves)
					{
						foreach (AnimationWindowKeyframe current2 in current.m_Keyframes)
						{
							if (current2.GetHash() == this.m_ActiveKeyframeHash)
							{
								this.m_ActiveKeyframeCache = current2;
							}
						}
					}
				}
				return this.m_ActiveKeyframeCache;
			}
			set
			{
				this.m_ActiveKeyframeCache = null;
				this.m_ActiveKeyframeHash = ((value == null) ? 0 : value.GetHash());
			}
		}

		public List<AnimationWindowKeyframe> selectedKeys
		{
			get
			{
				if (this.m_SelectedKeysCache == null)
				{
					this.m_SelectedKeysCache = new List<AnimationWindowKeyframe>();
					foreach (AnimationWindowCurve current in this.allCurves)
					{
						foreach (AnimationWindowKeyframe current2 in current.m_Keyframes)
						{
							if (this.KeyIsSelected(current2))
							{
								this.m_SelectedKeysCache.Add(current2);
							}
						}
					}
				}
				return this.m_SelectedKeysCache;
			}
		}

		private HashSet<int> selectedKeyHashes
		{
			get
			{
				HashSet<int> arg_1B_0;
				if ((arg_1B_0 = this.m_SelectedKeyHashes) == null)
				{
					arg_1B_0 = (this.m_SelectedKeyHashes = new HashSet<int>());
				}
				return arg_1B_0;
			}
			set
			{
				this.m_SelectedKeyHashes = value;
			}
		}

		public bool syncTimeDuringDrag
		{
			get
			{
				return false;
			}
		}

		public float clipFrameRate
		{
			get
			{
				if (this.activeAnimationClip == null)
				{
					return 60f;
				}
				return this.activeAnimationClip.frameRate;
			}
			set
			{
				if (this.activeAnimationClip != null && value > 0f && value <= 10000f)
				{
					foreach (AnimationWindowCurve current in this.allCurves)
					{
						foreach (AnimationWindowKeyframe current2 in current.m_Keyframes)
						{
							int frame = AnimationKeyTime.Time(current2.time, this.clipFrameRate).frame;
							current2.time = AnimationKeyTime.Frame(frame, value).time;
						}
						this.SaveCurve(current);
					}
					AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(this.activeAnimationClip);
					AnimationEvent[] array = animationEvents;
					for (int i = 0; i < array.Length; i++)
					{
						AnimationEvent animationEvent = array[i];
						int frame2 = AnimationKeyTime.Time(animationEvent.time, this.clipFrameRate).frame;
						animationEvent.time = AnimationKeyTime.Frame(frame2, value).time;
					}
					AnimationUtility.SetAnimationEvents(this.activeAnimationClip, animationEvents);
					this.activeAnimationClip.frameRate = value;
				}
			}
		}

		public float frameRate
		{
			get
			{
				return this.m_FrameRate;
			}
			set
			{
				if (this.m_FrameRate != value)
				{
					this.m_FrameRate = value;
					if (this.onFrameRateChange != null)
					{
						this.onFrameRateChange(this.m_FrameRate);
					}
				}
			}
		}

		public int frame
		{
			get
			{
				return this.m_CurrentFrame;
			}
			set
			{
				if (this.m_CurrentFrame != value)
				{
					this.m_CurrentFrame = Math.Max(value, 0);
					this.m_CurrentTime = this.FrameToTime((float)this.m_CurrentFrame);
					this.ResampleAnimation();
				}
			}
		}

		public float currentTime
		{
			get
			{
				return this.m_CurrentTime;
			}
			set
			{
				if (!Mathf.Approximately(this.m_CurrentTime, value))
				{
					this.m_CurrentTime = Mathf.Max(value, 0f);
					this.m_CurrentFrame = this.TimeToFrameFloor(this.m_CurrentTime);
					this.ResampleAnimation();
				}
			}
		}

		public AnimationKeyTime time
		{
			get
			{
				return AnimationKeyTime.Frame(this.frame, this.frameRate);
			}
		}

		public bool playing
		{
			get
			{
				return AnimationMode.InAnimationPlaybackMode();
			}
			set
			{
				if (Application.isPlaying)
				{
					return;
				}
				if (value && !AnimationMode.InAnimationPlaybackMode())
				{
					AnimationMode.StartAnimationPlaybackMode();
					this.recording = true;
				}
				if (!value && AnimationMode.InAnimationPlaybackMode())
				{
					AnimationMode.StopAnimationPlaybackMode();
					this.currentTime = this.FrameToTime((float)this.frame);
				}
			}
		}

		public bool canRecord
		{
			get
			{
				return !Application.isPlaying && this.m_Recording != null && this.m_Recording.canEnable;
			}
		}

		public bool recording
		{
			get
			{
				return this.m_Recording != null && this.m_Recording.enable;
			}
			set
			{
				if (value && this.policy != null && !this.policy.allowRecording)
				{
					return;
				}
				if (Application.isPlaying)
				{
					return;
				}
				if (this.m_Recording != null)
				{
					bool enable = this.m_Recording.enable;
					this.m_Recording.enable = value;
					bool enable2 = this.m_Recording.enable;
					if (enable != enable2)
					{
						if (enable2)
						{
							Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
						}
						else
						{
							Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
						}
					}
				}
			}
		}

		public AnimationWindowPolicy policy
		{
			get
			{
				return this.m_Policy;
			}
			set
			{
				this.m_Policy = value;
			}
		}

		public TimeArea timeArea
		{
			get
			{
				return this.m_TimeArea;
			}
			set
			{
				this.m_TimeArea = value;
			}
		}

		public float pixelPerSecond
		{
			get
			{
				return this.timeArea.m_Scale.x;
			}
		}

		public float zeroTimePixel
		{
			get
			{
				return this.timeArea.shownArea.xMin * this.timeArea.m_Scale.x * -1f;
			}
		}

		public float minVisibleTime
		{
			get
			{
				return this.m_TimeArea.shownArea.xMin;
			}
		}

		public float maxVisibleTime
		{
			get
			{
				return this.m_TimeArea.shownArea.xMax;
			}
		}

		public float visibleTimeSpan
		{
			get
			{
				return this.maxVisibleTime - this.minVisibleTime;
			}
		}

		public float minVisibleFrame
		{
			get
			{
				return this.minVisibleTime * this.frameRate;
			}
		}

		public float maxVisibleFrame
		{
			get
			{
				return this.maxVisibleTime * this.frameRate;
			}
		}

		public float visibleFrameSpan
		{
			get
			{
				return this.visibleTimeSpan * this.frameRate;
			}
		}

		public float minTime
		{
			get
			{
				return this.timeRange.x;
			}
		}

		public float maxTime
		{
			get
			{
				return this.timeRange.y;
			}
		}

		public Vector2 timeRange
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				if (this.m_Selection.count > 0)
				{
					num = 3.40282347E+38f;
					num2 = -3.40282347E+38f;
					AnimationWindowSelectionItem[] array = this.m_Selection.ToArray();
					for (int i = 0; i < array.Length; i++)
					{
						AnimationWindowSelectionItem animationWindowSelectionItem = array[i];
						num = Mathf.Min(num, animationWindowSelectionItem.animationClip.startTime + animationWindowSelectionItem.timeOffset);
						num2 = Mathf.Max(num2, animationWindowSelectionItem.animationClip.stopTime + animationWindowSelectionItem.timeOffset);
					}
				}
				return new Vector2(num, num2);
			}
		}

		public void OnGUI()
		{
			this.RefreshHashCheck();
			this.Refresh();
		}

		private void RefreshHashCheck()
		{
			int refreshHash = this.GetRefreshHash();
			if (this.m_PreviousRefreshHash != refreshHash)
			{
				this.refresh = AnimationWindowState.RefreshType.Everything;
				this.m_PreviousRefreshHash = refreshHash;
			}
		}

		private void Refresh()
		{
			if (this.refresh == AnimationWindowState.RefreshType.Everything)
			{
				this.m_Selection.Refresh();
				this.m_ActiveKeyframeCache = null;
				this.m_ActiveCurvesCache = null;
				this.m_dopelinesCache = null;
				this.m_SelectedKeysCache = null;
				this.m_ActiveCurveWrappersCache = null;
				if (this.hierarchyData != null)
				{
					this.hierarchyData.UpdateData();
				}
				EditorCurveBinding? lastAddedCurveBinding = this.m_lastAddedCurveBinding;
				if (lastAddedCurveBinding.HasValue)
				{
					EditorCurveBinding? lastAddedCurveBinding2 = this.m_lastAddedCurveBinding;
					this.OnNewCurveAdded(lastAddedCurveBinding2.Value);
				}
				if (this.activeCurves.Count == 0 && this.dopelines.Count > 0)
				{
					this.SelectHierarchyItem(this.dopelines[0], false, false);
				}
				this.m_Refresh = AnimationWindowState.RefreshType.None;
			}
			else if (this.refresh == AnimationWindowState.RefreshType.CurvesOnly)
			{
				this.m_ActiveKeyframeCache = null;
				this.m_ActiveCurvesCache = null;
				this.m_ActiveCurveWrappersCache = null;
				this.m_SelectedKeysCache = null;
				this.ReloadModifiedAnimationCurveCache();
				this.ReloadModifiedDopelineCache();
				this.m_Refresh = AnimationWindowState.RefreshType.None;
				this.m_ModifiedCurves.Clear();
			}
			if (this.selection.disabled && this.recording)
			{
				this.recording = false;
			}
		}

		private int GetRefreshHash()
		{
			return ((this.m_Selection == null) ? 0 : this.m_Selection.GetRefreshHash()) ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.expandedIDs.Count) ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.GetTallInstancesCount()) ^ ((!this.showCurveEditor) ? 0 : 1);
		}

		public void ForceRefresh()
		{
			this.refresh = AnimationWindowState.RefreshType.Everything;
		}

		public void OnEnable()
		{
			base.hideFlags = HideFlags.HideAndDontSave;
			AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified)Delegate.Combine(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.m_Recording = new AnimationRecordMode();
		}

		public void OnDisable()
		{
			CurveBindingUtility.Cleanup();
			this.recording = false;
			this.playing = false;
			AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified)Delegate.Remove(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			if (this.m_Recording != null)
			{
				this.m_Recording.Dispose();
				this.m_Recording = null;
			}
		}

		public void OnDestroy()
		{
			if (this.m_Selection != null)
			{
				this.m_Selection.Clear();
			}
		}

		public void OnSelectionChanged()
		{
			CurveBindingUtility.Cleanup();
			if (this.onFrameRateChange != null)
			{
				this.onFrameRateChange(this.frameRate);
			}
			if (this.recording)
			{
				this.ResampleAnimation();
			}
		}

		public void UndoRedoPerformed()
		{
			this.refresh = AnimationWindowState.RefreshType.Everything;
			if (this.recording)
			{
				this.ResampleAnimation();
			}
		}

		private void CurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType type)
		{
			if (!this.m_Selection.Exists((AnimationWindowSelectionItem item) => item.animationClip == clip))
			{
				return;
			}
			if (type == AnimationUtility.CurveModifiedType.CurveModified)
			{
				bool flag = false;
				int hashCode = binding.GetHashCode();
				foreach (AnimationWindowCurve current in this.allCurves)
				{
					int hashCode2 = current.binding.GetHashCode();
					if (hashCode2 == hashCode)
					{
						this.m_ModifiedCurves.Add(hashCode2);
						flag = true;
					}
				}
				if (flag)
				{
					this.refresh = AnimationWindowState.RefreshType.CurvesOnly;
				}
				else
				{
					this.m_lastAddedCurveBinding = new EditorCurveBinding?(binding);
					this.refresh = AnimationWindowState.RefreshType.Everything;
				}
			}
			else
			{
				this.refresh = AnimationWindowState.RefreshType.Everything;
			}
		}

		public void SaveCurve(AnimationWindowCurve curve)
		{
			if (!curve.animationIsEditable)
			{
				Debug.LogError("Curve is not editable and shouldn't be saved.");
			}
			Undo.RegisterCompleteObjectUndo(curve.clip, "Edit Curve");
			AnimationRecording.SaveModifiedCurve(curve, curve.clip);
			this.Repaint();
		}

		public void SaveSelectedKeys(List<AnimationWindowKeyframe> currentSelectedKeys)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			foreach (AnimationWindowKeyframe current in currentSelectedKeys)
			{
				if (!list.Contains(current.curve))
				{
					list.Add(current.curve);
				}
				List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
				foreach (AnimationWindowKeyframe current2 in current.curve.m_Keyframes)
				{
					if (!currentSelectedKeys.Contains(current2) && AnimationKeyTime.Time(current.time, this.frameRate).frame == AnimationKeyTime.Time(current2.time, this.frameRate).frame)
					{
						list2.Add(current2);
					}
				}
				foreach (AnimationWindowKeyframe current3 in list2)
				{
					current.curve.m_Keyframes.Remove(current3);
				}
			}
			foreach (AnimationWindowCurve current4 in list)
			{
				this.SaveCurve(current4);
			}
		}

		public void RemoveCurve(AnimationWindowCurve curve)
		{
			if (!curve.animationIsEditable)
			{
				return;
			}
			Undo.RegisterCompleteObjectUndo(curve.clip, "Remove Curve");
			if (curve.isPPtrCurve)
			{
				AnimationUtility.SetObjectReferenceCurve(curve.clip, curve.binding, null);
			}
			else
			{
				AnimationUtility.SetEditorCurve(curve.clip, curve.binding, null);
			}
		}

		public bool AnyKeyIsSelected(DopeLine dopeline)
		{
			foreach (AnimationWindowKeyframe current in dopeline.keys)
			{
				if (this.KeyIsSelected(current))
				{
					return true;
				}
			}
			return false;
		}

		public bool KeyIsSelected(AnimationWindowKeyframe keyframe)
		{
			return this.selectedKeyHashes.Contains(keyframe.GetHash());
		}

		public void SelectKey(AnimationWindowKeyframe keyframe)
		{
			int hash = keyframe.GetHash();
			if (!this.selectedKeyHashes.Contains(hash))
			{
				this.selectedKeyHashes.Add(hash);
			}
			this.m_SelectedKeysCache = null;
		}

		public void SelectKeysFromDopeline(DopeLine dopeline)
		{
			if (dopeline == null)
			{
				return;
			}
			foreach (AnimationWindowKeyframe current in dopeline.keys)
			{
				this.SelectKey(current);
			}
		}

		public void UnselectKey(AnimationWindowKeyframe keyframe)
		{
			int hash = keyframe.GetHash();
			if (this.selectedKeyHashes.Contains(hash))
			{
				this.selectedKeyHashes.Remove(hash);
			}
			this.m_SelectedKeysCache = null;
		}

		public void UnselectKeysFromDopeline(DopeLine dopeline)
		{
			if (dopeline == null)
			{
				return;
			}
			foreach (AnimationWindowKeyframe current in dopeline.keys)
			{
				this.UnselectKey(current);
			}
		}

		public void DeleteSelectedKeys()
		{
			if (this.selectedKeys.Count == 0)
			{
				return;
			}
			foreach (AnimationWindowKeyframe current in this.selectedKeys)
			{
				if (current.curve.animationIsEditable)
				{
					this.UnselectKey(current);
					current.curve.m_Keyframes.Remove(current);
					this.SaveCurve(current.curve);
				}
			}
			this.ResampleAnimation();
		}

		public void MoveSelectedKeys(float deltaTime)
		{
			this.MoveSelectedKeys(deltaTime, false);
		}

		public void MoveSelectedKeys(float deltaTime, bool snapToFrame)
		{
			this.MoveSelectedKeys(deltaTime, snapToFrame, true);
		}

		public void MoveSelectedKeys(float deltaTime, bool snapToFrame, bool saveToClip)
		{
			List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>(this.selectedKeys);
			List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
			foreach (AnimationWindowKeyframe current in list)
			{
				if (current.curve.animationIsEditable)
				{
					current.time += deltaTime;
					if (snapToFrame)
					{
						current.time = this.SnapToFrame(current.time, current.curve.clip.frameRate, !saveToClip);
					}
					list2.Add(current);
				}
			}
			this.ClearKeySelections();
			foreach (AnimationWindowKeyframe current2 in list)
			{
				this.SelectKey(current2);
			}
			if (saveToClip)
			{
				this.SaveSelectedKeys(list2);
				this.ResampleAnimation();
			}
		}

		public void CopyKeys()
		{
			if (AnimationWindowState.s_KeyframeClipboard == null)
			{
				AnimationWindowState.s_KeyframeClipboard = new List<AnimationWindowKeyframe>();
			}
			float num = 3.40282347E+38f;
			AnimationWindowState.s_KeyframeClipboard.Clear();
			foreach (AnimationWindowKeyframe current in this.selectedKeys)
			{
				AnimationWindowState.s_KeyframeClipboard.Add(new AnimationWindowKeyframe(current));
				float num2 = current.time + current.curve.timeOffset;
				if (num2 < num)
				{
					num = num2;
				}
			}
			if (AnimationWindowState.s_KeyframeClipboard.Count > 0)
			{
				foreach (AnimationWindowKeyframe current2 in AnimationWindowState.s_KeyframeClipboard)
				{
					current2.time -= num - current2.curve.timeOffset;
				}
			}
			else
			{
				this.CopyAllActiveCurves();
			}
		}

		public void CopyAllActiveCurves()
		{
			foreach (AnimationWindowCurve current in this.activeCurves)
			{
				foreach (AnimationWindowKeyframe current2 in current.m_Keyframes)
				{
					AnimationWindowState.s_KeyframeClipboard.Add(new AnimationWindowKeyframe(current2));
				}
			}
		}

		public void PasteKeys()
		{
			if (AnimationWindowState.s_KeyframeClipboard == null)
			{
				AnimationWindowState.s_KeyframeClipboard = new List<AnimationWindowKeyframe>();
			}
			HashSet<int> selectedKeyHashes = new HashSet<int>(this.m_SelectedKeyHashes);
			this.ClearKeySelections();
			AnimationWindowCurve animationWindowCurve = null;
			AnimationWindowCurve animationWindowCurve2 = null;
			float startTime = 0f;
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			foreach (AnimationWindowKeyframe current in AnimationWindowState.s_KeyframeClipboard)
			{
				if (!list.Any<AnimationWindowCurve>() || list.Last<AnimationWindowCurve>() != current.curve)
				{
					list.Add(current.curve);
				}
			}
			bool flag = list.Count<AnimationWindowCurve>() == this.activeCurves.Count<AnimationWindowCurve>();
			int num = 0;
			foreach (AnimationWindowKeyframe current2 in AnimationWindowState.s_KeyframeClipboard)
			{
				if (animationWindowCurve2 != null && current2.curve != animationWindowCurve2)
				{
					num++;
				}
				AnimationWindowKeyframe animationWindowKeyframe = new AnimationWindowKeyframe(current2);
				if (flag)
				{
					animationWindowKeyframe.curve = this.activeCurves[num];
				}
				else
				{
					animationWindowKeyframe.curve = AnimationWindowUtility.BestMatchForPaste(animationWindowKeyframe.curve.binding, list, this.activeCurves);
				}
				if (animationWindowKeyframe.curve == null)
				{
					if (this.activeCurves.Count > 0)
					{
						AnimationWindowCurve animationWindowCurve3 = this.activeCurves[0];
						if (animationWindowCurve3.animationIsEditable)
						{
							animationWindowKeyframe.curve = new AnimationWindowCurve(animationWindowCurve3.clip, current2.curve.binding, current2.curve.type);
							animationWindowKeyframe.curve.selectionBindingInterface = animationWindowCurve3.selectionBindingInterface;
							animationWindowKeyframe.time = current2.time;
						}
					}
					else
					{
						AnimationWindowSelectionItem animationWindowSelectionItem = this.m_Selection.First();
						if (animationWindowSelectionItem.animationIsEditable)
						{
							animationWindowKeyframe.curve = new AnimationWindowCurve(animationWindowSelectionItem.animationClip, current2.curve.binding, current2.curve.type);
							animationWindowKeyframe.curve.selectionBindingInterface = animationWindowSelectionItem;
							animationWindowKeyframe.time = current2.time;
						}
					}
				}
				if (animationWindowKeyframe.curve != null && animationWindowKeyframe.curve.animationIsEditable)
				{
					animationWindowKeyframe.time += this.time.time - animationWindowKeyframe.curve.timeOffset;
					if (animationWindowKeyframe.time >= 0f && animationWindowKeyframe.curve != null && animationWindowKeyframe.curve.isPPtrCurve == current2.curve.isPPtrCurve)
					{
						if (animationWindowKeyframe.curve.HasKeyframe(AnimationKeyTime.Time(animationWindowKeyframe.time, animationWindowKeyframe.curve.clip.frameRate)))
						{
							animationWindowKeyframe.curve.RemoveKeyframe(AnimationKeyTime.Time(animationWindowKeyframe.time, animationWindowKeyframe.curve.clip.frameRate));
						}
						if (animationWindowCurve == animationWindowKeyframe.curve)
						{
							animationWindowKeyframe.curve.RemoveKeysAtRange(startTime, animationWindowKeyframe.time);
						}
						animationWindowKeyframe.curve.m_Keyframes.Add(animationWindowKeyframe);
						this.SelectKey(animationWindowKeyframe);
						this.SaveCurve(animationWindowKeyframe.curve);
						animationWindowCurve = animationWindowKeyframe.curve;
						startTime = animationWindowKeyframe.time;
					}
					animationWindowCurve2 = current2.curve;
				}
			}
			if (this.m_SelectedKeyHashes.Count == 0)
			{
				this.m_SelectedKeyHashes = selectedKeyHashes;
			}
			else
			{
				this.ResampleAnimation();
			}
		}

		public void ClearSelections()
		{
			this.ClearKeySelections();
			this.ClearHierarchySelection();
		}

		public void ClearKeySelections()
		{
			this.selectedKeyHashes.Clear();
			this.m_SelectedKeysCache = null;
		}

		public void ClearHierarchySelection()
		{
			this.hierarchyState.selectedIDs.Clear();
			this.m_ActiveCurvesCache = null;
		}

		private void ReloadModifiedDopelineCache()
		{
			if (this.m_dopelinesCache == null)
			{
				return;
			}
			foreach (DopeLine current in this.m_dopelinesCache)
			{
				AnimationWindowCurve[] curves = current.m_Curves;
				for (int i = 0; i < curves.Length; i++)
				{
					AnimationWindowCurve animationWindowCurve = curves[i];
					if (this.m_ModifiedCurves.Contains(animationWindowCurve.binding.GetHashCode()))
					{
						current.LoadKeyframes();
					}
				}
			}
		}

		private void ReloadModifiedAnimationCurveCache()
		{
			foreach (AnimationWindowCurve current in this.allCurves)
			{
				if (this.m_ModifiedCurves.Contains(current.binding.GetHashCode()))
				{
					current.LoadKeyframes(current.clip);
				}
			}
		}

		public void ResampleAnimation()
		{
			if (this.disabled)
			{
				return;
			}
			if (this.policy != null && !this.policy.allowRecording)
			{
				return;
			}
			if (this.animatorIsOptimized)
			{
				return;
			}
			if (this.policy != null && !this.policy.allowRecording)
			{
				return;
			}
			if (!this.canRecord)
			{
				return;
			}
			bool flag = false;
			AnimationWindowSelectionItem[] array = this.selection.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				AnimationWindowSelectionItem animationWindowSelectionItem = array[i];
				if (animationWindowSelectionItem.animationClip != null)
				{
					if (!this.recording)
					{
						this.recording = true;
					}
					Undo.FlushUndoRecordObjects();
					AnimationMode.BeginSampling();
					CurveBindingUtility.SampleAnimationClip(animationWindowSelectionItem.rootGameObject, animationWindowSelectionItem.animationClip, this.currentTime - animationWindowSelectionItem.timeOffset);
					AnimationMode.EndSampling();
					flag = true;
				}
			}
			if (flag)
			{
				SceneView.RepaintAll();
				ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
				if (instance)
				{
					instance.Repaint();
				}
			}
		}

		private void OnNewCurveAdded(EditorCurveBinding newCurve)
		{
			string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(newCurve.propertyName);
			this.ClearHierarchySelection();
			using (List<TreeViewItem>.Enumerator enumerator = this.hierarchyData.GetRows().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AnimationWindowHierarchyNode animationWindowHierarchyNode = (AnimationWindowHierarchyNode)enumerator.Current;
					if (!(animationWindowHierarchyNode.path != newCurve.path) && animationWindowHierarchyNode.animatableObjectType == newCurve.type && !(animationWindowHierarchyNode.propertyName != propertyGroupName))
					{
						this.SelectHierarchyItem(animationWindowHierarchyNode.id, true, false);
						if (newCurve.isPPtrCurve)
						{
							this.hierarchyState.AddTallInstance(animationWindowHierarchyNode.id);
						}
					}
				}
			}
			if (this.recording)
			{
				this.ResampleAnimation();
				InspectorWindow.RepaintAllInspectors();
			}
			this.m_lastAddedCurveBinding = null;
		}

		public void Repaint()
		{
			if (this.animEditor != null)
			{
				this.animEditor.Repaint();
			}
		}

		public List<AnimationWindowKeyframe> GetAggregateKeys(AnimationWindowHierarchyNode hierarchyNode)
		{
			DopeLine dopeLine = this.dopelines.FirstOrDefault((DopeLine e) => e.m_HierarchyNodeID == hierarchyNode.id);
			if (dopeLine == null)
			{
				return null;
			}
			return dopeLine.keys;
		}

		public void OnHierarchySelectionChanged(int[] selectedInstanceIDs)
		{
			this.HandleHierarchySelectionChanged(selectedInstanceIDs, true);
			foreach (DopeLine current in this.dopelines)
			{
				bool flag = selectedInstanceIDs.Contains(current.m_HierarchyNodeID);
				if (flag)
				{
					this.SelectKeysFromDopeline(current);
				}
				else
				{
					this.UnselectKeysFromDopeline(current);
				}
			}
		}

		public void HandleHierarchySelectionChanged(int[] selectedInstanceIDs, bool triggerSceneSelectionSync)
		{
			this.m_ActiveCurvesCache = null;
			if (triggerSceneSelectionSync)
			{
				this.SyncSceneSelection(selectedInstanceIDs);
			}
		}

		public void SelectHierarchyItem(DopeLine dopeline, bool additive)
		{
			this.SelectHierarchyItem(dopeline.m_HierarchyNodeID, additive, true);
		}

		public void SelectHierarchyItem(DopeLine dopeline, bool additive, bool triggerSceneSelectionSync)
		{
			this.SelectHierarchyItem(dopeline.m_HierarchyNodeID, additive, triggerSceneSelectionSync);
		}

		public void SelectHierarchyItem(int hierarchyNodeID, bool additive, bool triggerSceneSelectionSync)
		{
			if (!additive)
			{
				this.ClearHierarchySelection();
			}
			this.hierarchyState.selectedIDs.Add(hierarchyNodeID);
			int[] selectedInstanceIDs = this.hierarchyState.selectedIDs.ToArray();
			this.HandleHierarchySelectionChanged(selectedInstanceIDs, triggerSceneSelectionSync);
		}

		public void UnSelectHierarchyItem(DopeLine dopeline)
		{
			this.UnSelectHierarchyItem(dopeline.m_HierarchyNodeID);
		}

		public void UnSelectHierarchyItem(int hierarchyNodeID)
		{
			this.hierarchyState.selectedIDs.Remove(hierarchyNodeID);
		}

		public List<int> GetAffectedHierarchyIDs(List<AnimationWindowKeyframe> keyframes)
		{
			List<int> list = new List<int>();
			foreach (DopeLine current in this.GetAffectedDopelines(keyframes))
			{
				if (!list.Contains(current.m_HierarchyNodeID))
				{
					list.Add(current.m_HierarchyNodeID);
				}
			}
			return list;
		}

		public List<DopeLine> GetAffectedDopelines(List<AnimationWindowKeyframe> keyframes)
		{
			List<DopeLine> list = new List<DopeLine>();
			foreach (AnimationWindowCurve current in this.GetAffectedCurves(keyframes))
			{
				foreach (DopeLine current2 in this.dopelines)
				{
					if (!list.Contains(current2) && current2.m_Curves.Contains(current))
					{
						list.Add(current2);
					}
				}
			}
			return list;
		}

		public List<AnimationWindowCurve> GetAffectedCurves(List<AnimationWindowKeyframe> keyframes)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			foreach (AnimationWindowKeyframe current in keyframes)
			{
				if (!list.Contains(current.curve))
				{
					list.Add(current.curve);
				}
			}
			return list;
		}

		public DopeLine GetDopeline(int selectedInstanceID)
		{
			foreach (DopeLine current in this.dopelines)
			{
				if (current.m_HierarchyNodeID == selectedInstanceID)
				{
					return current;
				}
			}
			return null;
		}

		private void SyncSceneSelection(int[] selectedNodeIDs)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < selectedNodeIDs.Length; i++)
			{
				int id = selectedNodeIDs[i];
				AnimationWindowHierarchyNode animationWindowHierarchyNode = this.hierarchyData.FindItem(id) as AnimationWindowHierarchyNode;
				if (!(this.activeRootGameObject == null) && animationWindowHierarchyNode != null)
				{
					if (!(animationWindowHierarchyNode is AnimationWindowHierarchyMasterNode))
					{
						Transform transform = this.activeRootGameObject.transform.Find(animationWindowHierarchyNode.path);
						if (transform != null && this.activeRootGameObject != null && this.activeAnimationPlayer == AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(transform))
						{
							list.Add(transform.gameObject.GetInstanceID());
						}
					}
				}
			}
			Selection.instanceIDs = list.ToArray();
		}

		private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
		{
			if (!AnimationMode.InAnimationMode())
			{
				Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				return modifications;
			}
			return AnimationRecording.Process(this, modifications);
		}

		public float PixelToTime(float pixel)
		{
			return this.PixelToTime(pixel, AnimationWindowState.SnapMode.Disabled);
		}

		public float PixelToTime(float pixel, AnimationWindowState.SnapMode snap)
		{
			float num = pixel - this.zeroTimePixel;
			return this.SnapToFrame(num / this.pixelPerSecond, snap);
		}

		public float TimeToPixel(float time)
		{
			return this.TimeToPixel(time, AnimationWindowState.SnapMode.Disabled);
		}

		public float TimeToPixel(float time, AnimationWindowState.SnapMode snap)
		{
			return this.SnapToFrame(time, snap) * this.pixelPerSecond + this.zeroTimePixel;
		}

		public float SnapToFrame(float time, AnimationWindowState.SnapMode snap)
		{
			return this.SnapToFrame(time, snap, false);
		}

		public float SnapToFrame(float time, AnimationWindowState.SnapMode snap, bool preventHashCollision)
		{
			if (snap == AnimationWindowState.SnapMode.Disabled)
			{
				return time;
			}
			float fps = (snap != AnimationWindowState.SnapMode.SnapToFrame) ? this.clipFrameRate : this.frameRate;
			return this.SnapToFrame(time, fps, preventHashCollision);
		}

		public float SnapToFrame(float time, float fps, bool preventHashCollision)
		{
			float num = Mathf.Round(time * fps) / fps;
			if (preventHashCollision)
			{
				num += 0.01f / fps;
			}
			return num;
		}

		public string FormatFrame(int frame, int frameDigits)
		{
			return (frame / (int)this.frameRate).ToString() + ":" + ((float)frame % this.frameRate).ToString().PadLeft(frameDigits, '0');
		}

		public float TimeToFrame(float time)
		{
			return time * this.frameRate;
		}

		public float FrameToTime(float frame)
		{
			return frame / this.frameRate;
		}

		public int TimeToFrameFloor(float time)
		{
			return Mathf.FloorToInt(this.TimeToFrame(time));
		}

		public int TimeToFrameRound(float time)
		{
			return Mathf.RoundToInt(this.TimeToFrame(time));
		}

		public float FrameToPixel(float i, Rect rect)
		{
			return (i - this.minVisibleFrame) * rect.width / this.visibleFrameSpan;
		}

		public float FrameDeltaToPixel(Rect rect)
		{
			return rect.width / this.visibleFrameSpan;
		}

		public float TimeToPixel(float time, Rect rect)
		{
			return this.FrameToPixel(time * this.frameRate, rect);
		}

		public float PixelToTime(float pixelX, Rect rect)
		{
			return pixelX * this.visibleTimeSpan / rect.width + this.minVisibleTime;
		}

		public float PixelDeltaToTime(Rect rect)
		{
			return this.visibleTimeSpan / rect.width;
		}
	}
}
