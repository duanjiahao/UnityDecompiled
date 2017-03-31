using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowState : ScriptableObject, ICurveEditorState, IAnimationRecordingState
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

		private struct LiveEditKeyframe
		{
			public AnimationWindowKeyframe keySnapshot;

			public AnimationWindowKeyframe key;
		}

		private class LiveEditCurve
		{
			public AnimationWindowCurve curve;

			public List<AnimationWindowState.LiveEditKeyframe> selectedKeys = new List<AnimationWindowState.LiveEditKeyframe>();

			public List<AnimationWindowState.LiveEditKeyframe> unselectedKeys = new List<AnimationWindowState.LiveEditKeyframe>();
		}

		[SerializeField]
		public AnimationWindowHierarchyState hierarchyState;

		[SerializeField]
		public AnimEditor animEditor;

		[SerializeField]
		public bool showCurveEditor;

		[SerializeField]
		public bool linkedWithSequencer;

		[SerializeField]
		private TimeArea m_TimeArea;

		[SerializeField]
		private AnimationWindowSelection m_Selection;

		[SerializeField]
		private AnimationWindowKeySelection m_KeySelection;

		[SerializeField]
		private int m_ActiveKeyframeHash;

		[SerializeField]
		private float m_FrameRate = 60f;

		[SerializeField]
		private TimeArea.TimeFormat m_TimeFormat = TimeArea.TimeFormat.TimeFrame;

		[SerializeField]
		private AnimationWindowControl m_ControlInterface;

		[SerializeField]
		private IAnimationWindowControl m_OverrideControlInterface;

		[NonSerialized]
		public Action onStartLiveEdit;

		[NonSerialized]
		public Action onEndLiveEdit;

		[NonSerialized]
		public Action<float> onFrameRateChange;

		private static List<AnimationWindowKeyframe> s_KeyframeClipboard;

		[NonSerialized]
		public AnimationWindowHierarchyDataSource hierarchyData;

		private List<AnimationWindowCurve> m_ActiveCurvesCache;

		private List<DopeLine> m_dopelinesCache;

		private List<AnimationWindowKeyframe> m_SelectedKeysCache;

		private Bounds? m_SelectionBoundsCache;

		private CurveWrapper[] m_ActiveCurveWrappersCache;

		private AnimationWindowKeyframe m_ActiveKeyframeCache;

		private HashSet<int> m_ModifiedCurves = new HashSet<int>();

		private EditorCurveBinding? m_lastAddedCurveBinding;

		private int m_PreviousRefreshHash;

		private AnimationWindowState.RefreshType m_Refresh = AnimationWindowState.RefreshType.None;

		private List<AnimationWindowState.LiveEditCurve> m_LiveEditSnapshot;

		public const float kDefaultFrameRate = 60f;

		public const string kEditCurveUndoLabel = "Edit Curve";

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
				AnimationWindowSelectionItem result;
				if (this.m_Selection != null && this.m_Selection.count > 0)
				{
					result = this.m_Selection.First();
				}
				else
				{
					result = null;
				}
				return result;
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
				AnimationClip result;
				if (this.selectedItem != null)
				{
					result = this.selectedItem.animationClip;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public GameObject activeGameObject
		{
			get
			{
				GameObject result;
				if (this.selectedItem != null)
				{
					result = this.selectedItem.gameObject;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public GameObject activeRootGameObject
		{
			get
			{
				GameObject result;
				if (this.selectedItem != null)
				{
					result = this.selectedItem.rootGameObject;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public Component activeAnimationPlayer
		{
			get
			{
				Component result;
				if (this.selectedItem != null)
				{
					result = this.selectedItem.animationPlayer;
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public bool animatorIsOptimized
		{
			get
			{
				return this.selectedItem != null && this.selectedItem.objectIsOptimized;
			}
		}

		public bool disabled
		{
			get
			{
				return this.selection.disabled;
			}
		}

		public IAnimationWindowControl controlInterface
		{
			get
			{
				IAnimationWindowControl result;
				if (this.m_OverrideControlInterface != null)
				{
					result = this.m_OverrideControlInterface;
				}
				else
				{
					result = this.m_ControlInterface;
				}
				return result;
			}
		}

		public bool addZeroFrame
		{
			get
			{
				return true;
			}
		}

		public IAnimationWindowControl overrideControlInterface
		{
			get
			{
				return this.m_OverrideControlInterface;
			}
			set
			{
				if (this.m_OverrideControlInterface != null)
				{
					UnityEngine.Object.DestroyImmediate(this.m_OverrideControlInterface);
				}
				this.m_OverrideControlInterface = value;
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

		public bool recording
		{
			get
			{
				return this.controlInterface.recording;
			}
		}

		public bool playing
		{
			get
			{
				return this.controlInterface.playing;
			}
		}

		public List<AnimationWindowCurve> allCurves
		{
			get
			{
				return this.selection.curves;
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

		public CurveWrapper[] activeCurveWrappers
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
					this.m_ActiveCurveWrappersCache = list.ToArray();
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

		public Bounds selectionBounds
		{
			get
			{
				Bounds? selectionBoundsCache = this.m_SelectionBoundsCache;
				if (!selectionBoundsCache.HasValue)
				{
					List<AnimationWindowKeyframe> selectedKeys = this.selectedKeys;
					if (selectedKeys.Count > 0)
					{
						AnimationWindowKeyframe animationWindowKeyframe = selectedKeys[0];
						float x = animationWindowKeyframe.time + animationWindowKeyframe.curve.timeOffset;
						float y = (!animationWindowKeyframe.isPPtrCurve) ? ((float)animationWindowKeyframe.value) : 0f;
						Bounds value = new Bounds(new Vector2(x, y), Vector2.zero);
						for (int i = 1; i < selectedKeys.Count; i++)
						{
							animationWindowKeyframe = selectedKeys[i];
							x = animationWindowKeyframe.time + animationWindowKeyframe.curve.timeOffset;
							y = ((!animationWindowKeyframe.isPPtrCurve) ? ((float)animationWindowKeyframe.value) : 0f);
							value.Encapsulate(new Vector2(x, y));
						}
						this.m_SelectionBoundsCache = new Bounds?(value);
					}
					else
					{
						this.m_SelectionBoundsCache = new Bounds?(new Bounds(Vector2.zero, Vector2.zero));
					}
				}
				return this.m_SelectionBoundsCache.Value;
			}
		}

		private HashSet<int> selectedKeyHashes
		{
			get
			{
				if (this.m_KeySelection == null)
				{
					this.m_KeySelection = ScriptableObject.CreateInstance<AnimationWindowKeySelection>();
					this.m_KeySelection.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_KeySelection.selectedKeyHashes;
			}
			set
			{
				if (this.m_KeySelection == null)
				{
					this.m_KeySelection = ScriptableObject.CreateInstance<AnimationWindowKeySelection>();
					this.m_KeySelection.hideFlags = HideFlags.HideAndDontSave;
				}
				this.m_KeySelection.selectedKeyHashes = value;
			}
		}

		public float clipFrameRate
		{
			get
			{
				float result;
				if (this.activeAnimationClip == null)
				{
					result = 60f;
				}
				else
				{
					result = this.activeAnimationClip.frameRate;
				}
				return result;
			}
			set
			{
				if (this.activeAnimationClip != null && value > 0f && value <= 10000f)
				{
					this.ClearKeySelections();
					this.SaveKeySelection("Edit Curve");
					foreach (AnimationWindowCurve current in this.allCurves)
					{
						foreach (AnimationWindowKeyframe current2 in current.m_Keyframes)
						{
							int frame = AnimationKeyTime.Time(current2.time, this.clipFrameRate).frame;
							current2.time = AnimationKeyTime.Frame(frame, value).time;
						}
						this.SaveCurve(current, "Edit Curve");
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
					this.onFrameRateChange(this.m_FrameRate);
				}
			}
		}

		public AnimationKeyTime time
		{
			get
			{
				return this.controlInterface.time;
			}
		}

		public int currentFrame
		{
			get
			{
				return this.time.frame;
			}
			set
			{
				this.controlInterface.GoToFrame(value);
			}
		}

		public float currentTime
		{
			get
			{
				return this.time.time;
			}
			set
			{
				this.controlInterface.GoToTime(value);
			}
		}

		public TimeArea.TimeFormat timeFormat
		{
			get
			{
				return this.m_TimeFormat;
			}
			set
			{
				this.m_TimeFormat = value;
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
				if (this.selection.count > 0)
				{
					num = 3.40282347E+38f;
					num2 = -3.40282347E+38f;
					AnimationWindowSelectionItem[] array = this.selection.ToArray();
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
			this.selection.Synchronize();
			if (this.refresh == AnimationWindowState.RefreshType.Everything)
			{
				this.selection.Refresh();
				this.m_ActiveKeyframeCache = null;
				this.m_ActiveCurvesCache = null;
				this.m_dopelinesCache = null;
				this.m_SelectedKeysCache = null;
				this.m_SelectionBoundsCache = null;
				this.ClearCurveWrapperCache();
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
				this.m_SelectedKeysCache = null;
				this.m_SelectionBoundsCache = null;
				this.ReloadModifiedAnimationCurveCache();
				this.ReloadModifiedDopelineCache();
				this.ReloadModifiedCurveWrapperCache();
				this.m_Refresh = AnimationWindowState.RefreshType.None;
				this.m_ModifiedCurves.Clear();
			}
		}

		private int GetRefreshHash()
		{
			return this.selection.GetRefreshHash() ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.expandedIDs.Count) ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.GetTallInstancesCount()) ^ ((!this.showCurveEditor) ? 0 : 1);
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
			this.onStartLiveEdit = (Action)Delegate.Combine(this.onStartLiveEdit, new Action(delegate
			{
			}));
			this.onEndLiveEdit = (Action)Delegate.Combine(this.onEndLiveEdit, new Action(delegate
			{
			}));
			this.onFrameRateChange = (Action<float>)Delegate.Combine(this.onFrameRateChange, new Action<float>(delegate(float frameRate)
			{
			}));
			if (this.m_ControlInterface == null)
			{
				this.m_ControlInterface = (ScriptableObject.CreateInstance(typeof(AnimationWindowControl)) as AnimationWindowControl);
			}
			this.m_ControlInterface.state = this;
		}

		public void OnDisable()
		{
			AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified)Delegate.Remove(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.m_ControlInterface.OnDisable();
		}

		public void OnDestroy()
		{
			if (this.m_Selection != null)
			{
				this.m_Selection.Clear();
			}
			UnityEngine.Object.DestroyImmediate(this.m_KeySelection);
			UnityEngine.Object.DestroyImmediate(this.m_ControlInterface);
			UnityEngine.Object.DestroyImmediate(this.m_OverrideControlInterface);
		}

		public void OnSelectionChanged()
		{
			this.onFrameRateChange(this.frameRate);
			this.controlInterface.OnSelectionChanged();
		}

		public void UndoRedoPerformed()
		{
			this.refresh = AnimationWindowState.RefreshType.Everything;
			this.controlInterface.ResampleAnimation();
		}

		private void CurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType type)
		{
			AnimationWindowSelectionItem[] array = Array.FindAll<AnimationWindowSelectionItem>(this.selection.ToArray(), (AnimationWindowSelectionItem item) => item.animationClip == clip);
			if (array.Length != 0)
			{
				if (type == AnimationUtility.CurveModifiedType.CurveModified)
				{
					bool flag = false;
					bool flag2 = false;
					int hashCode = binding.GetHashCode();
					for (int i = 0; i < array.Length; i++)
					{
						List<AnimationWindowCurve> curves = array[i].curves;
						for (int j = 0; j < curves.Count; j++)
						{
							AnimationWindowCurve animationWindowCurve = curves[j];
							int bindingHashCode = animationWindowCurve.GetBindingHashCode();
							if (bindingHashCode == hashCode)
							{
								this.m_ModifiedCurves.Add(animationWindowCurve.GetHashCode());
								flag = true;
								flag2 |= animationWindowCurve.binding.isPhantom;
							}
						}
					}
					if (flag && !flag2)
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
		}

		public void SaveKeySelection(string undoLabel)
		{
			if (this.m_KeySelection != null)
			{
				Undo.RegisterCompleteObjectUndo(this.m_KeySelection, undoLabel);
			}
		}

		public void SaveCurve(AnimationWindowCurve curve)
		{
			this.SaveCurve(curve, "Edit Curve");
		}

		public void SaveCurve(AnimationWindowCurve curve, string undoLabel)
		{
			if (!curve.animationIsEditable)
			{
				Debug.LogError("Curve is not editable and shouldn't be saved.");
			}
			Undo.RegisterCompleteObjectUndo(curve.clip, undoLabel);
			AnimationRecording.SaveModifiedCurve(curve, curve.clip);
			this.Repaint();
		}

		private void SaveSelectedKeys(string undoLabel)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			foreach (AnimationWindowState.LiveEditCurve current in this.m_LiveEditSnapshot)
			{
				if (current.curve.animationIsEditable)
				{
					if (!list.Contains(current.curve))
					{
						list.Add(current.curve);
					}
					List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
					using (List<AnimationWindowKeyframe>.Enumerator enumerator2 = current.curve.m_Keyframes.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							AnimationWindowKeyframe other = enumerator2.Current;
							AnimationWindowState $this = this;
							if (!current.selectedKeys.Exists((AnimationWindowState.LiveEditKeyframe liveEditKey) => liveEditKey.key == other))
							{
								if (current.selectedKeys.Exists((AnimationWindowState.LiveEditKeyframe liveEditKey) => AnimationKeyTime.Time(liveEditKey.key.time, $this.frameRate).frame == AnimationKeyTime.Time(other.time, $this.frameRate).frame))
								{
									list2.Add(other);
								}
							}
						}
					}
					foreach (AnimationWindowKeyframe current2 in list2)
					{
						current.curve.m_Keyframes.Remove(current2);
					}
				}
			}
			foreach (AnimationWindowCurve current3 in list)
			{
				this.SaveCurve(current3, undoLabel);
			}
		}

		public void RemoveCurve(AnimationWindowCurve curve, string undoLabel)
		{
			if (curve.animationIsEditable)
			{
				Undo.RegisterCompleteObjectUndo(curve.clip, undoLabel);
				if (curve.isPPtrCurve)
				{
					AnimationUtility.SetObjectReferenceCurve(curve.clip, curve.binding, null);
				}
				else
				{
					AnimationUtility.SetEditorCurve(curve.clip, curve.binding, null);
				}
			}
		}

		public void StartRecording()
		{
			if (this.selectedItem != null)
			{
				this.controlInterface.StartRecording(this.selectedItem.sourceObject);
				this.controlInterface.ResampleAnimation();
			}
		}

		public void StopRecording()
		{
			this.controlInterface.StopRecording();
		}

		public void StartPlayback()
		{
			this.controlInterface.StartPlayback();
		}

		public void StopPlayback()
		{
			this.controlInterface.StopPlayback();
		}

		public bool AnyKeyIsSelected(DopeLine dopeline)
		{
			bool result;
			foreach (AnimationWindowKeyframe current in dopeline.keys)
			{
				if (this.KeyIsSelected(current))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
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
			this.m_SelectionBoundsCache = null;
		}

		public void SelectKeysFromDopeline(DopeLine dopeline)
		{
			if (dopeline != null)
			{
				foreach (AnimationWindowKeyframe current in dopeline.keys)
				{
					this.SelectKey(current);
				}
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
			this.m_SelectionBoundsCache = null;
		}

		public void UnselectKeysFromDopeline(DopeLine dopeline)
		{
			if (dopeline != null)
			{
				foreach (AnimationWindowKeyframe current in dopeline.keys)
				{
					this.UnselectKey(current);
				}
			}
		}

		public void DeleteSelectedKeys()
		{
			if (this.selectedKeys.Count != 0)
			{
				this.DeleteKeys(this.selectedKeys);
			}
		}

		public void DeleteKeys(List<AnimationWindowKeyframe> keys)
		{
			this.SaveKeySelection("Edit Curve");
			foreach (AnimationWindowKeyframe current in keys)
			{
				if (current.curve.animationIsEditable)
				{
					this.UnselectKey(current);
					current.curve.m_Keyframes.Remove(current);
					this.SaveCurve(current.curve, "Edit Curve");
				}
			}
			this.StartRecording();
		}

		public void StartLiveEdit()
		{
			if (this.onStartLiveEdit != null)
			{
				this.onStartLiveEdit();
			}
			this.m_LiveEditSnapshot = new List<AnimationWindowState.LiveEditCurve>();
			this.SaveKeySelection("Edit Curve");
			using (List<AnimationWindowKeyframe>.Enumerator enumerator = this.selectedKeys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AnimationWindowKeyframe selectedKey = enumerator.Current;
					if (!this.m_LiveEditSnapshot.Exists((AnimationWindowState.LiveEditCurve snapshot) => snapshot.curve == selectedKey.curve))
					{
						AnimationWindowState.LiveEditCurve liveEditCurve = new AnimationWindowState.LiveEditCurve();
						liveEditCurve.curve = selectedKey.curve;
						foreach (AnimationWindowKeyframe current in selectedKey.curve.m_Keyframes)
						{
							AnimationWindowState.LiveEditKeyframe item = default(AnimationWindowState.LiveEditKeyframe);
							item.keySnapshot = new AnimationWindowKeyframe(current);
							item.key = current;
							if (this.KeyIsSelected(current))
							{
								liveEditCurve.selectedKeys.Add(item);
							}
							else
							{
								liveEditCurve.unselectedKeys.Add(item);
							}
						}
						this.m_LiveEditSnapshot.Add(liveEditCurve);
					}
				}
			}
		}

		public void EndLiveEdit()
		{
			this.SaveSelectedKeys("Edit Curve");
			this.m_LiveEditSnapshot = null;
			if (this.onEndLiveEdit != null)
			{
				this.onEndLiveEdit();
			}
		}

		public bool InLiveEdit()
		{
			return this.m_LiveEditSnapshot != null;
		}

		public void MoveSelectedKeys(float deltaTime, bool snapToFrame)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.ClearKeySelections();
			foreach (AnimationWindowState.LiveEditCurve current in this.m_LiveEditSnapshot)
			{
				foreach (AnimationWindowState.LiveEditKeyframe current2 in current.selectedKeys)
				{
					if (current.curve.animationIsEditable)
					{
						current2.key.time = Mathf.Max(current2.keySnapshot.time + deltaTime, 0f);
						if (snapToFrame)
						{
							current2.key.time = this.SnapToFrame(current2.key.time, current.curve.clip.frameRate);
						}
					}
					this.SelectKey(current2.key);
				}
			}
			if (!flag)
			{
				this.EndLiveEdit();
			}
		}

		public void TransformSelectedKeys(Matrix4x4 matrix, bool flipX, bool flipY, bool snapToFrame)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.ClearKeySelections();
			foreach (AnimationWindowState.LiveEditCurve current in this.m_LiveEditSnapshot)
			{
				foreach (AnimationWindowState.LiveEditKeyframe current2 in current.selectedKeys)
				{
					if (current.curve.animationIsEditable)
					{
						Vector3 v = new Vector3(current2.keySnapshot.time, (!current2.keySnapshot.isPPtrCurve) ? ((float)current2.keySnapshot.value) : 0f, 0f);
						v = matrix.MultiplyPoint3x4(v);
						current2.key.time = Mathf.Max((!snapToFrame) ? v.x : this.SnapToFrame(v.x, current.curve.clip.frameRate), 0f);
						if (flipX)
						{
							current2.key.inTangent = ((current2.keySnapshot.outTangent == float.PositiveInfinity) ? float.PositiveInfinity : (-current2.keySnapshot.outTangent));
							current2.key.outTangent = ((current2.keySnapshot.inTangent == float.PositiveInfinity) ? float.PositiveInfinity : (-current2.keySnapshot.inTangent));
						}
						if (!current2.key.isPPtrCurve)
						{
							current2.key.value = v.y;
							if (flipY)
							{
								current2.key.inTangent = ((current2.key.inTangent == float.PositiveInfinity) ? float.PositiveInfinity : (-current2.key.inTangent));
								current2.key.outTangent = ((current2.key.outTangent == float.PositiveInfinity) ? float.PositiveInfinity : (-current2.key.outTangent));
							}
						}
					}
					this.SelectKey(current2.key);
				}
			}
			if (!flag)
			{
				this.EndLiveEdit();
			}
		}

		public void TransformRippleKeys(Matrix4x4 matrix, float t1, float t2, bool flipX, bool snapToFrame)
		{
			bool flag = this.InLiveEdit();
			if (!flag)
			{
				this.StartLiveEdit();
			}
			this.ClearKeySelections();
			foreach (AnimationWindowState.LiveEditCurve current in this.m_LiveEditSnapshot)
			{
				foreach (AnimationWindowState.LiveEditKeyframe current2 in current.selectedKeys)
				{
					if (current.curve.animationIsEditable)
					{
						Vector3 v = new Vector3(current2.keySnapshot.time, 0f, 0f);
						v = matrix.MultiplyPoint3x4(v);
						current2.key.time = Mathf.Max((!snapToFrame) ? v.x : this.SnapToFrame(v.x, current.curve.clip.frameRate), 0f);
						if (flipX)
						{
							current2.key.inTangent = ((current2.keySnapshot.outTangent == float.PositiveInfinity) ? float.PositiveInfinity : (-current2.keySnapshot.outTangent));
							current2.key.outTangent = ((current2.keySnapshot.inTangent == float.PositiveInfinity) ? float.PositiveInfinity : (-current2.keySnapshot.inTangent));
						}
					}
					this.SelectKey(current2.key);
				}
				if (current.curve.animationIsEditable)
				{
					foreach (AnimationWindowState.LiveEditKeyframe current3 in current.unselectedKeys)
					{
						if (current3.keySnapshot.time > t2)
						{
							Vector3 v2 = new Vector3((!flipX) ? t2 : t1, 0f, 0f);
							v2 = matrix.MultiplyPoint3x4(v2);
							float num = v2.x - t2;
							if (num > 0f)
							{
								float num2 = current3.keySnapshot.time + num;
								current3.key.time = Mathf.Max((!snapToFrame) ? num2 : this.SnapToFrame(num2, current.curve.clip.frameRate), 0f);
							}
							else
							{
								current3.key.time = current3.keySnapshot.time;
							}
						}
						else if (current3.keySnapshot.time < t1)
						{
							Vector3 v3 = new Vector3((!flipX) ? t1 : t2, 0f, 0f);
							v3 = matrix.MultiplyPoint3x4(v3);
							float num3 = v3.x - t1;
							if (num3 < 0f)
							{
								float num4 = current3.keySnapshot.time + num3;
								current3.key.time = Mathf.Max((!snapToFrame) ? num4 : this.SnapToFrame(num4, current.curve.clip.frameRate), 0f);
							}
							else
							{
								current3.key.time = current3.keySnapshot.time;
							}
						}
					}
				}
			}
			if (!flag)
			{
				this.EndLiveEdit();
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
			this.SaveKeySelection("Edit Curve");
			HashSet<int> selectedKeyHashes = new HashSet<int>(this.selectedKeyHashes);
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
							animationWindowKeyframe.curve = new AnimationWindowCurve(animationWindowCurve3.clip, current2.curve.binding, current2.curve.valueType);
							animationWindowKeyframe.curve.selectionBinding = animationWindowCurve3.selectionBinding;
							animationWindowKeyframe.time = current2.time;
						}
					}
					else
					{
						AnimationWindowSelectionItem animationWindowSelectionItem = this.selection.First();
						if (animationWindowSelectionItem.animationIsEditable)
						{
							animationWindowKeyframe.curve = new AnimationWindowCurve(animationWindowSelectionItem.animationClip, current2.curve.binding, current2.curve.valueType);
							animationWindowKeyframe.curve.selectionBinding = animationWindowSelectionItem;
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
						this.SaveCurve(animationWindowKeyframe.curve, "Edit Curve");
						animationWindowCurve = animationWindowKeyframe.curve;
						startTime = animationWindowKeyframe.time;
					}
					animationWindowCurve2 = current2.curve;
				}
			}
			if (this.selectedKeyHashes.Count == 0)
			{
				this.selectedKeyHashes = selectedKeyHashes;
			}
			else
			{
				this.StartRecording();
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
			this.m_SelectionBoundsCache = null;
		}

		public void ClearHierarchySelection()
		{
			this.hierarchyState.selectedIDs.Clear();
			this.m_ActiveCurvesCache = null;
		}

		private void ClearCurveWrapperCache()
		{
			if (this.m_ActiveCurveWrappersCache != null)
			{
				for (int i = 0; i < this.m_ActiveCurveWrappersCache.Length; i++)
				{
					CurveWrapper curveWrapper = this.m_ActiveCurveWrappersCache[i];
					if (curveWrapper.renderer != null)
					{
						curveWrapper.renderer.FlushCache();
					}
				}
				this.m_ActiveCurveWrappersCache = null;
			}
		}

		private void ReloadModifiedDopelineCache()
		{
			if (this.m_dopelinesCache != null)
			{
				for (int i = 0; i < this.m_dopelinesCache.Count; i++)
				{
					DopeLine dopeLine = this.m_dopelinesCache[i];
					AnimationWindowCurve[] curves = dopeLine.curves;
					for (int j = 0; j < curves.Length; j++)
					{
						if (this.m_ModifiedCurves.Contains(curves[j].GetHashCode()))
						{
							dopeLine.InvalidateKeyframes();
							break;
						}
					}
				}
			}
		}

		private void ReloadModifiedCurveWrapperCache()
		{
			if (this.m_ActiveCurveWrappersCache != null)
			{
				Dictionary<int, AnimationWindowCurve> dictionary = new Dictionary<int, AnimationWindowCurve>();
				for (int i = 0; i < this.m_ActiveCurveWrappersCache.Length; i++)
				{
					CurveWrapper curveWrapper = this.m_ActiveCurveWrappersCache[i];
					if (this.m_ModifiedCurves.Contains(curveWrapper.id))
					{
						AnimationWindowCurve animationWindowCurve = this.allCurves.Find((AnimationWindowCurve c) => c.GetHashCode() == curveWrapper.id);
						if (animationWindowCurve != null)
						{
							if (animationWindowCurve.clip.startTime != curveWrapper.renderer.RangeStart() || animationWindowCurve.clip.stopTime != curveWrapper.renderer.RangeEnd())
							{
								this.ClearCurveWrapperCache();
								return;
							}
							dictionary[i] = animationWindowCurve;
						}
					}
				}
				for (int j = 0; j < dictionary.Count; j++)
				{
					KeyValuePair<int, AnimationWindowCurve> keyValuePair = dictionary.ElementAt(j);
					CurveWrapper curveWrapper2 = this.m_ActiveCurveWrappersCache[keyValuePair.Key];
					if (curveWrapper2.renderer != null)
					{
						curveWrapper2.renderer.FlushCache();
					}
					this.m_ActiveCurveWrappersCache[keyValuePair.Key] = AnimationWindowUtility.GetCurveWrapper(keyValuePair.Value, keyValuePair.Value.clip);
				}
			}
		}

		private void ReloadModifiedAnimationCurveCache()
		{
			for (int i = 0; i < this.allCurves.Count; i++)
			{
				AnimationWindowCurve animationWindowCurve = this.allCurves[i];
				if (this.m_ModifiedCurves.Contains(animationWindowCurve.GetHashCode()))
				{
					animationWindowCurve.LoadKeyframes(animationWindowCurve.clip);
				}
			}
		}

		private void OnNewCurveAdded(EditorCurveBinding newCurve)
		{
			string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(newCurve.propertyName);
			this.ClearHierarchySelection();
			if (this.hierarchyData != null)
			{
				using (IEnumerator<TreeViewItem> enumerator = this.hierarchyData.GetRows().GetEnumerator())
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
				this.controlInterface.ResampleAnimation();
				this.m_lastAddedCurveBinding = null;
			}
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
			DopeLine dopeLine = this.dopelines.FirstOrDefault((DopeLine e) => e.hierarchyNodeID == hierarchyNode.id);
			List<AnimationWindowKeyframe> result;
			if (dopeLine == null)
			{
				result = null;
			}
			else
			{
				result = dopeLine.keys;
			}
			return result;
		}

		public void OnHierarchySelectionChanged(int[] selectedInstanceIDs)
		{
			this.HandleHierarchySelectionChanged(selectedInstanceIDs, true);
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
			this.SelectHierarchyItem(dopeline.hierarchyNodeID, additive, true);
		}

		public void SelectHierarchyItem(DopeLine dopeline, bool additive, bool triggerSceneSelectionSync)
		{
			this.SelectHierarchyItem(dopeline.hierarchyNodeID, additive, triggerSceneSelectionSync);
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
			this.UnSelectHierarchyItem(dopeline.hierarchyNodeID);
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
				if (!list.Contains(current.hierarchyNodeID))
				{
					list.Add(current.hierarchyNodeID);
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
					if (!list.Contains(current2) && current2.curves.Contains(current))
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
			DopeLine result;
			foreach (DopeLine current in this.dopelines)
			{
				if (current.hierarchyNodeID == selectedInstanceID)
				{
					result = current;
					return result;
				}
			}
			result = null;
			return result;
		}

		private void SyncSceneSelection(int[] selectedNodeIDs)
		{
			if (!(this.selectedItem == null) && this.selectedItem.canSyncSceneSelection)
			{
				GameObject rootGameObject = this.selectedItem.rootGameObject;
				if (!(rootGameObject == null))
				{
					List<int> list = new List<int>();
					for (int i = 0; i < selectedNodeIDs.Length; i++)
					{
						int id = selectedNodeIDs[i];
						AnimationWindowHierarchyNode animationWindowHierarchyNode = this.hierarchyData.FindItem(id) as AnimationWindowHierarchyNode;
						if (animationWindowHierarchyNode != null)
						{
							if (!(animationWindowHierarchyNode is AnimationWindowHierarchyMasterNode))
							{
								Transform transform = rootGameObject.transform.Find(animationWindowHierarchyNode.path);
								if (transform != null && rootGameObject != null && this.activeAnimationPlayer == AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(transform))
								{
									list.Add(transform.gameObject.GetInstanceID());
								}
							}
						}
					}
					Selection.instanceIDs = list.ToArray();
				}
			}
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
			float result;
			if (snap == AnimationWindowState.SnapMode.Disabled)
			{
				result = time;
			}
			else
			{
				float fps = (snap != AnimationWindowState.SnapMode.SnapToFrame) ? this.clipFrameRate : this.frameRate;
				result = this.SnapToFrame(time, fps);
			}
			return result;
		}

		public float SnapToFrame(float time, float fps)
		{
			return Mathf.Round(time * fps) / fps;
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
