using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	[Serializable]
	internal class AnimationWindowState
	{
		public enum RefreshType
		{
			None,
			CurvesOnly,
			Everything
		}
		[SerializeField]
		public AnimationWindowHierarchyState m_hierarchyState;
		[SerializeField]
		public AnimationClip m_ActiveAnimationClip;
		[SerializeField]
		public GameObject m_ActiveGameObject;
		[SerializeField]
		public GameObject m_RootGameObject;
		[SerializeField]
		public GameObject m_AnimatedGameObject;
		[SerializeField]
		public bool m_AnimationIsPlaying;
		[SerializeField]
		public bool m_ShowCurveEditor;
		[SerializeField]
		public bool m_CurveEditorIsDirty;
		[SerializeField]
		public float m_PlayTime;
		[SerializeField]
		public int m_Frame;
		[SerializeField]
		private Hashtable m_SelectedKeyHashes;
		[SerializeField]
		private int m_ActiveKeyframeHash;
		[SerializeField]
		private Rect m_ShownTimeArea = new Rect(0f, 0f, 2f, 2f);
		[SerializeField]
		private Rect m_ShownTimeAreaInsideMargins = new Rect(0f, 0f, 2f, 2f);
		public Action m_OnHierarchySelectionChanged;
		public AnimationWindowHierarchyDataSource m_HierarchyData;
		public AnimationWindow m_AnimationWindow;
		private List<AnimationWindowCurve> m_AllCurvesCache;
		private List<AnimationWindowCurve> m_ActiveCurvesCache;
		private List<DopeLine> m_dopelinesCache;
		private List<AnimationWindowKeyframe> m_SelectedKeysCache;
		private AnimationWindowKeyframe m_ActiveKeyframeCache;
		private HashSet<int> m_ModifiedCurves = new HashSet<int>();
		private EditorCurveBinding? m_lastAddedCurveBinding;
		private EditorWindow m_Window;
		private int m_PreviousRefreshHash;
		private AnimationWindowState.RefreshType m_Refresh;
		private TimeArea m_timeArea;
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
				if (this.m_AllCurvesCache == null)
				{
					this.m_AllCurvesCache = new List<AnimationWindowCurve>();
					if (this.m_ActiveAnimationClip != null && this.m_ActiveGameObject != null)
					{
						EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.m_ActiveAnimationClip);
						EditorCurveBinding[] objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(this.m_ActiveAnimationClip);
						EditorCurveBinding[] array = curveBindings;
						for (int i = 0; i < array.Length; i++)
						{
							EditorCurveBinding editorCurveBinding = array[i];
							if (AnimationWindowUtility.ShouldShowAnimationWindowCurve(editorCurveBinding))
							{
								this.m_AllCurvesCache.Add(new AnimationWindowCurve(this.m_ActiveAnimationClip, editorCurveBinding, AnimationUtility.GetEditorCurveValueType(this.m_RootGameObject, editorCurveBinding)));
							}
						}
						EditorCurveBinding[] array2 = objectReferenceCurveBindings;
						for (int j = 0; j < array2.Length; j++)
						{
							EditorCurveBinding binding = array2[j];
							this.m_AllCurvesCache.Add(new AnimationWindowCurve(this.m_ActiveAnimationClip, binding, AnimationUtility.GetEditorCurveValueType(this.m_RootGameObject, binding)));
						}
						this.m_AllCurvesCache.Sort();
					}
				}
				return this.m_AllCurvesCache;
			}
		}
		public List<AnimationWindowCurve> activeCurves
		{
			get
			{
				if (this.m_ActiveCurvesCache == null)
				{
					this.m_ActiveCurvesCache = new List<AnimationWindowCurve>();
					if (this.m_hierarchyState != null && this.m_HierarchyData != null)
					{
						foreach (int current in this.m_hierarchyState.selectedIDs)
						{
							TreeViewItem treeViewItem = this.m_HierarchyData.FindItem(current);
							AnimationWindowHierarchyNode animationWindowHierarchyNode = treeViewItem as AnimationWindowHierarchyNode;
							if (animationWindowHierarchyNode != null)
							{
								List<AnimationWindowCurve> curves = this.GetCurves(animationWindowHierarchyNode, true);
								foreach (AnimationWindowCurve current2 in curves)
								{
									if (!this.m_ActiveCurvesCache.Contains(current2))
									{
										this.m_ActiveCurvesCache.Add(current2);
									}
								}
							}
						}
					}
				}
				return this.m_ActiveCurvesCache;
			}
		}
		public List<DopeLine> dopelines
		{
			get
			{
				if (this.m_dopelinesCache == null)
				{
					this.m_dopelinesCache = new List<DopeLine>();
					if (this.m_HierarchyData != null)
					{
						foreach (TreeViewItem current in this.m_HierarchyData.GetVisibleRows())
						{
							AnimationWindowHierarchyNode animationWindowHierarchyNode = current as AnimationWindowHierarchyNode;
							if (animationWindowHierarchyNode != null && !(animationWindowHierarchyNode is AnimationWindowHierarchyAddButtonNode))
							{
								List<AnimationWindowCurve> list;
								if (current is AnimationWindowHierarchyMasterNode)
								{
									list = this.allCurves;
								}
								else
								{
									list = this.GetCurves(animationWindowHierarchyNode, true);
								}
								DopeLine dopeLine = new DopeLine(current.id, list.ToArray());
								dopeLine.tallMode = this.m_hierarchyState.getTallMode(animationWindowHierarchyNode);
								dopeLine.objectType = animationWindowHierarchyNode.animatableObjectType;
								dopeLine.hasChildren = !(animationWindowHierarchyNode is AnimationWindowHierarchyPropertyNode);
								dopeLine.isMasterDopeline = (current is AnimationWindowHierarchyMasterNode);
								this.m_dopelinesCache.Add(dopeLine);
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
				if (this.m_HierarchyData != null)
				{
					foreach (int current in this.m_hierarchyState.selectedIDs)
					{
						AnimationWindowHierarchyNode animationWindowHierarchyNode = (AnimationWindowHierarchyNode)this.m_HierarchyData.FindItem(current);
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
		private Hashtable selectedKeyHashes
		{
			get
			{
				Hashtable arg_1B_0;
				if ((arg_1B_0 = this.m_SelectedKeyHashes) == null)
				{
					arg_1B_0 = (this.m_SelectedKeyHashes = new Hashtable());
				}
				return arg_1B_0;
			}
			set
			{
				this.m_SelectedKeyHashes = value;
			}
		}
		public bool IsReadOnly
		{
			get
			{
				return !this.m_ActiveAnimationClip || !this.IsEditable || (this.m_ActiveAnimationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None;
			}
		}
		public bool IsEditable
		{
			get
			{
				return this.m_ActiveGameObject && !this.IsPrefab;
			}
		}
		public bool IsClipEditable
		{
			get
			{
				return this.m_ActiveAnimationClip && (this.m_ActiveAnimationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None && AssetDatabase.IsOpenForEdit(this.m_ActiveAnimationClip);
			}
		}
		public bool IsPrefab
		{
			get
			{
				return this.m_ActiveGameObject && (EditorUtility.IsPersistent(this.m_ActiveGameObject) || (this.m_ActiveGameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None);
			}
		}
		public bool AnimatorIsOptimized
		{
			get
			{
				if (!this.m_RootGameObject)
				{
					return false;
				}
				Animator component = this.m_RootGameObject.GetComponent<Animator>();
				return component != null && component.isOptimizable && !component.hasTransformHierarchy;
			}
		}
		public float frameRate
		{
			get
			{
				if (this.m_ActiveAnimationClip == null)
				{
					return 60f;
				}
				return this.m_ActiveAnimationClip.frameRate;
			}
			set
			{
				if (this.m_ActiveAnimationClip != null && value > 0f && value <= 10000f)
				{
					foreach (AnimationWindowCurve current in this.allCurves)
					{
						foreach (AnimationWindowKeyframe current2 in current.m_Keyframes)
						{
							int frame = AnimationKeyTime.Time(current2.time, this.frameRate).frame;
							current2.time = AnimationKeyTime.Frame(frame, value).time;
						}
						this.SaveCurve(current);
					}
					AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(this.m_ActiveAnimationClip);
					AnimationEvent[] array = animationEvents;
					for (int i = 0; i < array.Length; i++)
					{
						AnimationEvent animationEvent = array[i];
						int frame2 = AnimationKeyTime.Time(animationEvent.time, this.frameRate).frame;
						animationEvent.time = AnimationKeyTime.Frame(frame2, value).time;
					}
					AnimationUtility.SetAnimationEvents(this.m_ActiveAnimationClip, animationEvents);
					this.m_ActiveAnimationClip.frameRate = value;
					this.m_CurveEditorIsDirty = true;
				}
			}
		}
		public TimeArea timeArea
		{
			get
			{
				return this.m_timeArea;
			}
			set
			{
				if (value != this.m_timeArea && value != null)
				{
					value.SetShownHRangeInsideMargins(this.m_ShownTimeAreaInsideMargins.xMin, this.m_ShownTimeAreaInsideMargins.xMax);
				}
				this.m_timeArea = value;
				if (this.m_timeArea != null)
				{
					this.m_ShownTimeAreaInsideMargins = this.m_timeArea.shownAreaInsideMargins;
					this.m_ShownTimeArea = this.m_timeArea.shownArea;
				}
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
		public float minTime
		{
			get
			{
				return this.m_ShownTimeArea.xMin;
			}
		}
		public float maxTime
		{
			get
			{
				return this.m_ShownTimeArea.xMax;
			}
		}
		public float timeSpan
		{
			get
			{
				return this.maxTime - this.minTime;
			}
		}
		public float minFrame
		{
			get
			{
				return this.minTime * this.frameRate;
			}
		}
		public float maxFrame
		{
			get
			{
				return this.maxTime * this.frameRate;
			}
		}
		public float frameSpan
		{
			get
			{
				return this.timeSpan * this.frameRate;
			}
		}
		public AnimationKeyTime time
		{
			get
			{
				return AnimationKeyTime.Frame(this.m_Frame, this.frameRate);
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
		private int GetRefreshHash()
		{
			return ((!(this.m_ActiveAnimationClip != null)) ? 0 : this.m_ActiveAnimationClip.GetHashCode()) ^ ((!(this.m_RootGameObject != null)) ? 0 : this.m_RootGameObject.GetHashCode()) ^ ((this.m_hierarchyState == null) ? 0 : this.m_hierarchyState.expandedIDs.Count) ^ ((this.m_hierarchyState == null) ? 0 : this.m_hierarchyState.m_TallInstanceIDs.Count) ^ ((!this.m_ShowCurveEditor) ? 0 : 1);
		}
		public void OnEnable(EditorWindow window)
		{
			this.m_Window = window;
			AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified)Delegate.Combine(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
		public void OnDisable()
		{
			this.m_Window = null;
			AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified)Delegate.Remove(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
		public void UndoRedoPerformed()
		{
			this.refresh = AnimationWindowState.RefreshType.Everything;
		}
		private void CurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType type)
		{
			if (clip != this.m_ActiveAnimationClip)
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
			curve.m_Keyframes.Sort((AnimationWindowKeyframe a, AnimationWindowKeyframe b) => a.time.CompareTo(b.time));
			Undo.RegisterCompleteObjectUndo(this.m_ActiveAnimationClip, "Edit Curve");
			if (curve.isPPtrCurve)
			{
				ObjectReferenceKeyframe[] array = curve.ToObjectCurve();
				if (array.Length == 0)
				{
					array = null;
				}
				AnimationUtility.SetObjectReferenceCurve(this.m_ActiveAnimationClip, curve.binding, array);
			}
			else
			{
				AnimationCurve animationCurve = curve.ToAnimationCurve();
				if (animationCurve.keys.Length == 0)
				{
					animationCurve = null;
				}
				else
				{
					QuaternionCurveTangentCalculation.UpdateTangentsFromMode(animationCurve, this.m_ActiveAnimationClip, curve.binding);
				}
				AnimationUtility.SetEditorCurve(this.m_ActiveAnimationClip, curve.binding, animationCurve);
			}
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
			Undo.RegisterCompleteObjectUndo(this.m_ActiveAnimationClip, "Remove Curve");
			if (curve.isPPtrCurve)
			{
				AnimationUtility.SetObjectReferenceCurve(this.m_ActiveAnimationClip, curve.binding, null);
			}
			else
			{
				AnimationUtility.SetEditorCurve(this.m_ActiveAnimationClip, curve.binding, null);
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
				this.selectedKeyHashes.Add(hash, hash);
			}
			this.m_SelectedKeysCache = null;
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
		public void DeleteSelectedKeys()
		{
			if (this.selectedKeys.Count == 0)
			{
				return;
			}
			foreach (AnimationWindowKeyframe current in this.selectedKeys)
			{
				this.UnselectKey(current);
				current.curve.m_Keyframes.Remove(current);
				this.SaveCurve(current.curve);
			}
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
			foreach (AnimationWindowKeyframe current in list)
			{
				current.time += deltaTime;
				if (snapToFrame)
				{
					current.time = this.SnapToFrame(current.time, !saveToClip);
				}
			}
			if (saveToClip)
			{
				this.SaveSelectedKeys(list);
			}
			this.ClearKeySelections();
			foreach (AnimationWindowKeyframe current2 in list)
			{
				this.SelectKey(current2);
			}
		}
		public void ClearKeySelections()
		{
			this.selectedKeyHashes.Clear();
			this.m_SelectedKeysCache = null;
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
			if (this.m_AllCurvesCache == null)
			{
				return;
			}
			foreach (AnimationWindowCurve current in this.m_AllCurvesCache)
			{
				if (this.m_ModifiedCurves.Contains(current.binding.GetHashCode()))
				{
					current.LoadKeyframes(this.m_ActiveAnimationClip);
				}
			}
		}
		private void Refresh()
		{
			if (this.refresh == AnimationWindowState.RefreshType.Everything)
			{
				CurveRendererCache.ClearCurveRendererCache();
				this.m_ActiveKeyframeCache = null;
				this.m_AllCurvesCache = null;
				this.m_ActiveCurvesCache = null;
				this.m_CurveEditorIsDirty = true;
				this.m_dopelinesCache = null;
				this.m_SelectedKeysCache = null;
				if (this.refresh == AnimationWindowState.RefreshType.Everything && this.m_HierarchyData != null)
				{
					this.m_HierarchyData.UpdateData();
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
			else
			{
				if (this.refresh == AnimationWindowState.RefreshType.CurvesOnly)
				{
					this.m_ActiveKeyframeCache = null;
					this.m_ActiveCurvesCache = null;
					this.m_SelectedKeysCache = null;
					this.ReloadModifiedAnimationCurveCache();
					this.ReloadModifiedDopelineCache();
					CurveRendererCache.ClearCurveRendererCache();
					this.m_CurveEditorIsDirty = true;
					this.m_Refresh = AnimationWindowState.RefreshType.None;
					this.m_ModifiedCurves.Clear();
				}
			}
		}
		private void OnNewCurveAdded(EditorCurveBinding newCurve)
		{
			string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(newCurve.propertyName);
			int propertyNodeID = AnimationWindowUtility.GetPropertyNodeID(newCurve.path, newCurve.type, propertyGroupName);
			this.SelectHierarchyItem(propertyNodeID, false, false);
			if (newCurve.isPPtrCurve)
			{
				this.m_hierarchyState.m_TallInstanceIDs.Add(propertyNodeID);
			}
			this.m_lastAddedCurveBinding = null;
		}
		public void Repaint()
		{
			if (this.m_Window != null)
			{
				this.m_Window.Repaint();
			}
		}
		public List<AnimationWindowCurve> GetCurves(AnimationWindowHierarchyNode hierarchyNode, bool entireHierarchy)
		{
			return AnimationWindowUtility.FilterCurves(this.allCurves.ToArray(), hierarchyNode.path, hierarchyNode.animatableObjectType, hierarchyNode.propertyName);
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
			this.m_OnHierarchySelectionChanged();
		}
		public void HandleHierarchySelectionChanged(int[] selectedInstanceIDs, bool triggerSceneSelectionSync)
		{
			this.m_CurveEditorIsDirty = true;
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
			this.m_hierarchyState.selectedIDs.Add(hierarchyNodeID);
			int[] selectedInstanceIDs = this.m_hierarchyState.selectedIDs.ToArray();
			this.HandleHierarchySelectionChanged(selectedInstanceIDs, triggerSceneSelectionSync);
		}
		public void UnSelectHierarchyItem(DopeLine dopeline)
		{
			this.UnSelectHierarchyItem(dopeline.m_HierarchyNodeID);
		}
		public void UnSelectHierarchyItem(int hierarchyNodeID)
		{
			this.m_hierarchyState.selectedIDs.Remove(hierarchyNodeID);
		}
		public void ClearHierarchySelection()
		{
			this.m_hierarchyState.selectedIDs.Clear();
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
		private void SyncSceneSelection(int[] selectedNodeIDs)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < selectedNodeIDs.Length; i++)
			{
				int id = selectedNodeIDs[i];
				AnimationWindowHierarchyNode animationWindowHierarchyNode = this.m_HierarchyData.FindItem(id) as AnimationWindowHierarchyNode;
				if (!(this.m_RootGameObject == null) && animationWindowHierarchyNode != null)
				{
					if (!(animationWindowHierarchyNode is AnimationWindowHierarchyMasterNode))
					{
						Transform transform = this.m_RootGameObject.transform.Find(animationWindowHierarchyNode.path);
						if (transform != null && this.m_RootGameObject != null && this.m_RootGameObject.transform == AnimationWindowUtility.GetClosestAnimationComponentInParents(transform))
						{
							list.Add(transform.gameObject.GetInstanceID());
						}
					}
				}
			}
			Selection.instanceIDs = list.ToArray();
		}
		public float PixelToTime(float pixel)
		{
			return this.PixelToTime(pixel, false);
		}
		public float PixelToTime(float pixel, bool snapToFrame)
		{
			float num = pixel - this.zeroTimePixel;
			if (snapToFrame)
			{
				return this.SnapToFrame(num / this.pixelPerSecond);
			}
			return num / this.pixelPerSecond;
		}
		public float TimeToPixel(float time)
		{
			return this.TimeToPixel(time, false);
		}
		public float TimeToPixel(float time, bool snapToFrame)
		{
			return ((!snapToFrame) ? time : this.SnapToFrame(time)) * this.pixelPerSecond + this.zeroTimePixel;
		}
		public float SnapToFrame(float time)
		{
			return Mathf.Round(time * this.frameRate) / this.frameRate;
		}
		public float SnapToFrame(float time, bool preventHashCollision)
		{
			if (preventHashCollision)
			{
				return Mathf.Round(time * this.frameRate) / this.frameRate + 0.01f / this.frameRate;
			}
			return this.SnapToFrame(time);
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
		public float FrameToTimeFloor(float frame)
		{
			return (frame - 0.5f) / this.frameRate;
		}
		public float FrameToTimeCeiling(float frame)
		{
			return (frame + 0.5f) / this.frameRate;
		}
		public int TimeToFrameFloor(float time)
		{
			return Mathf.FloorToInt(this.TimeToFrame(time));
		}
		public int TimeToFrameRound(float time)
		{
			return Mathf.RoundToInt(this.TimeToFrame(time));
		}
		public float GetTimeSeconds()
		{
			if (this.m_AnimationIsPlaying)
			{
				return this.m_PlayTime;
			}
			return this.FrameToTime((float)this.m_Frame);
		}
		public float FrameToPixel(float i, Rect rect)
		{
			return (i - this.minFrame) * rect.width / this.frameSpan;
		}
		public float FrameDeltaToPixel(Rect rect)
		{
			return rect.width / this.frameSpan;
		}
		public float TimeToPixel(float time, Rect rect)
		{
			return this.FrameToPixel(time * this.frameRate, rect);
		}
		public float PixelToTime(float pixelX, Rect rect)
		{
			return pixelX * this.timeSpan / rect.width + this.minTime;
		}
		public float PixelDeltaToTime(Rect rect)
		{
			return this.timeSpan / rect.width;
		}
		public float SnapTimeToWholeFPS(float time)
		{
			return Mathf.Round(time * this.frameRate) / this.frameRate;
		}
	}
}
