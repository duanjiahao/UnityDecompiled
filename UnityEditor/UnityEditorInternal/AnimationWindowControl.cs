using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowControl : IAnimationWindowControl, IAnimationContextualResponder
	{
		private class CandidateRecordingState : IAnimationRecordingState
		{
			public GameObject activeGameObject
			{
				get;
				private set;
			}

			public GameObject activeRootGameObject
			{
				get;
				private set;
			}

			public AnimationClip activeAnimationClip
			{
				get;
				private set;
			}

			public int currentFrame
			{
				get
				{
					return 0;
				}
			}

			public bool addZeroFrame
			{
				get
				{
					return false;
				}
			}

			public CandidateRecordingState(AnimationWindowState state, AnimationClip candidateClip)
			{
				this.activeGameObject = state.activeGameObject;
				this.activeRootGameObject = state.activeRootGameObject;
				this.activeAnimationClip = candidateClip;
			}

			public bool DiscardModification(PropertyModification modification)
			{
				return !AnimationMode.IsPropertyAnimated(modification.target, modification.propertyPath);
			}

			public void SaveCurve(AnimationWindowCurve curve)
			{
				Undo.RegisterCompleteObjectUndo(curve.clip, "Edit Candidate Curve");
				AnimationRecording.SaveModifiedCurve(curve, curve.clip);
			}

			public void AddPropertyModification(EditorCurveBinding binding, PropertyModification propertyModification, bool keepPrefabOverride)
			{
				AnimationMode.AddCandidate(binding, propertyModification, keepPrefabOverride);
			}
		}

		private enum RecordingStateMode
		{
			ManualKey,
			AutoKey
		}

		private class RecordingState : IAnimationRecordingState
		{
			private AnimationWindowState m_State;

			private AnimationWindowControl.RecordingStateMode m_Mode;

			public GameObject activeGameObject
			{
				get
				{
					return this.m_State.activeGameObject;
				}
			}

			public GameObject activeRootGameObject
			{
				get
				{
					return this.m_State.activeRootGameObject;
				}
			}

			public AnimationClip activeAnimationClip
			{
				get
				{
					return this.m_State.activeAnimationClip;
				}
			}

			public int currentFrame
			{
				get
				{
					return this.m_State.currentFrame;
				}
			}

			public bool addZeroFrame
			{
				get
				{
					return this.m_Mode == AnimationWindowControl.RecordingStateMode.AutoKey;
				}
			}

			public bool addPropertyModification
			{
				get
				{
					return this.m_State.previewing;
				}
			}

			public RecordingState(AnimationWindowState state, AnimationWindowControl.RecordingStateMode mode)
			{
				this.m_State = state;
				this.m_Mode = mode;
			}

			public bool DiscardModification(PropertyModification modification)
			{
				return false;
			}

			public void SaveCurve(AnimationWindowCurve curve)
			{
				this.m_State.SaveCurve(curve);
			}

			public void AddPropertyModification(EditorCurveBinding binding, PropertyModification propertyModification, bool keepPrefabOverride)
			{
				AnimationMode.AddPropertyModification(binding, propertyModification, keepPrefabOverride);
			}
		}

		[SerializeField]
		private AnimationKeyTime m_Time;

		[NonSerialized]
		private float m_PreviousUpdateTime;

		[NonSerialized]
		public AnimationWindowState state;

		[SerializeField]
		private AnimationClip m_CandidateClip;

		[NonSerialized]
		private List<UndoPropertyModification> m_Candidates;

		[SerializeField]
		private AnimationModeDriver m_Driver;

		[SerializeField]
		private AnimationModeDriver m_CandidateDriver;

		public AnimEditor animEditor
		{
			get
			{
				return this.state.animEditor;
			}
		}

		public override AnimationKeyTime time
		{
			get
			{
				return this.m_Time;
			}
		}

		public override bool canPlay
		{
			get
			{
				return this.canPreview;
			}
		}

		public override bool playing
		{
			get
			{
				return AnimationMode.InAnimationPlaybackMode() && this.previewing;
			}
		}

		public override bool canPreview
		{
			get
			{
				return this.state.selection.canPreview && (AnimationMode.InAnimationMode(this.GetAnimationModeDriver()) || !AnimationMode.InAnimationMode());
			}
		}

		public override bool previewing
		{
			get
			{
				return AnimationMode.InAnimationMode(this.GetAnimationModeDriver());
			}
		}

		public override bool canRecord
		{
			get
			{
				return this.state.selection.canRecord && this.canPreview;
			}
		}

		public override bool recording
		{
			get
			{
				return this.previewing && AnimationMode.InAnimationRecording();
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
		}

		public void OnDisable()
		{
			this.StopPreview();
			this.StopPlayback();
			if (AnimationMode.InAnimationMode(this.GetAnimationModeDriver()))
			{
				AnimationMode.StopAnimationMode(this.GetAnimationModeDriver());
			}
		}

		public override void OnSelectionChanged()
		{
			if (this.state != null)
			{
				this.m_Time = AnimationKeyTime.Time(0f, this.state.frameRate);
			}
			this.StopPreview();
			this.StopPlayback();
		}

		public override void GoToTime(float time)
		{
			this.SetCurrentTime(time);
		}

		public override void GoToFrame(int frame)
		{
			this.SetCurrentFrame(frame);
		}

		public override void StartScrubTime()
		{
		}

		public override void ScrubTime(float time)
		{
			this.SetCurrentTime(time);
		}

		public override void EndScrubTime()
		{
		}

		public override void GoToPreviousFrame()
		{
			this.SetCurrentFrame(this.time.frame - 1);
		}

		public override void GoToNextFrame()
		{
			this.SetCurrentFrame(this.time.frame + 1);
		}

		public override void GoToPreviousKeyframe()
		{
			List<AnimationWindowCurve> list = (!this.state.showCurveEditor || this.state.activeCurves.Count <= 0) ? this.state.allCurves : this.state.activeCurves;
			float previousKeyframeTime = AnimationWindowUtility.GetPreviousKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
			this.SetCurrentTime(this.state.SnapToFrame(previousKeyframeTime, AnimationWindowState.SnapMode.SnapToClipFrame));
		}

		public void GoToPreviousKeyframe(PropertyModification[] modifications)
		{
			EditorCurveBinding[] array = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.state.activeAnimationClip);
			if (array.Length != 0)
			{
				List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
				for (int i = 0; i < this.state.allCurves.Count; i++)
				{
					AnimationWindowCurve curve = this.state.allCurves[i];
					if (Array.Exists<EditorCurveBinding>(array, (EditorCurveBinding binding) => curve.binding.Equals(binding)))
					{
						list.Add(curve);
					}
				}
				float previousKeyframeTime = AnimationWindowUtility.GetPreviousKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
				this.SetCurrentTime(this.state.SnapToFrame(previousKeyframeTime, AnimationWindowState.SnapMode.SnapToClipFrame));
				this.state.Repaint();
			}
		}

		public override void GoToNextKeyframe()
		{
			List<AnimationWindowCurve> list = (!this.state.showCurveEditor || this.state.activeCurves.Count <= 0) ? this.state.allCurves : this.state.activeCurves;
			float nextKeyframeTime = AnimationWindowUtility.GetNextKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
			this.SetCurrentTime(this.state.SnapToFrame(nextKeyframeTime, AnimationWindowState.SnapMode.SnapToClipFrame));
		}

		public void GoToNextKeyframe(PropertyModification[] modifications)
		{
			EditorCurveBinding[] array = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.state.activeAnimationClip);
			if (array.Length != 0)
			{
				List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
				for (int i = 0; i < this.state.allCurves.Count; i++)
				{
					AnimationWindowCurve curve = this.state.allCurves[i];
					if (Array.Exists<EditorCurveBinding>(array, (EditorCurveBinding binding) => curve.binding.Equals(binding)))
					{
						list.Add(curve);
					}
				}
				float nextKeyframeTime = AnimationWindowUtility.GetNextKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
				this.SetCurrentTime(this.state.SnapToFrame(nextKeyframeTime, AnimationWindowState.SnapMode.SnapToClipFrame));
				this.state.Repaint();
			}
		}

		public override void GoToFirstKeyframe()
		{
			if (this.state.activeAnimationClip)
			{
				this.SetCurrentTime(this.state.activeAnimationClip.startTime);
			}
		}

		public override void GoToLastKeyframe()
		{
			if (this.state.activeAnimationClip)
			{
				this.SetCurrentTime(this.state.activeAnimationClip.stopTime);
			}
		}

		private void SnapTimeToFrame()
		{
			float currentTime = this.state.FrameToTime((float)this.time.frame);
			this.SetCurrentTime(currentTime);
		}

		private void SetCurrentTime(float value)
		{
			if (!Mathf.Approximately(value, this.time.time))
			{
				this.m_Time = AnimationKeyTime.Time(value, this.state.frameRate);
				this.StartPreview();
				this.ClearCandidates();
				this.ResampleAnimation();
			}
		}

		private void SetCurrentFrame(int value)
		{
			if (value != this.time.frame)
			{
				this.m_Time = AnimationKeyTime.Frame(value, this.state.frameRate);
				this.StartPreview();
				this.ClearCandidates();
				this.ResampleAnimation();
			}
		}

		public override bool StartPlayback()
		{
			bool result;
			if (!this.canPlay)
			{
				result = false;
			}
			else
			{
				if (!this.playing)
				{
					AnimationMode.StartAnimationPlaybackMode();
					this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
					this.StartPreview();
					this.ClearCandidates();
				}
				result = true;
			}
			return result;
		}

		public override void StopPlayback()
		{
			if (AnimationMode.InAnimationPlaybackMode())
			{
				AnimationMode.StopAnimationPlaybackMode();
				this.SnapTimeToFrame();
			}
		}

		public override bool PlaybackUpdate()
		{
			bool result;
			if (!this.playing)
			{
				result = false;
			}
			else
			{
				float num = Time.realtimeSinceStartup - this.m_PreviousUpdateTime;
				this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
				float num2 = this.time.time + num;
				if (num2 > this.state.maxTime)
				{
					num2 = this.state.minTime;
				}
				this.m_Time = AnimationKeyTime.Time(Mathf.Clamp(num2, this.state.minTime, this.state.maxTime), this.state.frameRate);
				this.ResampleAnimation();
				result = true;
			}
			return result;
		}

		public override bool StartPreview()
		{
			bool result;
			if (this.previewing)
			{
				result = true;
			}
			else if (!this.canPreview)
			{
				result = false;
			}
			else
			{
				AnimationMode.StartAnimationMode(this.GetAnimationModeDriver());
				AnimationPropertyContextualMenu.Instance.SetResponder(this);
				Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				result = true;
			}
			return result;
		}

		public override void StopPreview()
		{
			this.StopPlayback();
			this.StopRecording();
			this.ClearCandidates();
			AnimationMode.StopAnimationMode(this.GetAnimationModeDriver());
			if (AnimationPropertyContextualMenu.Instance.IsResponder(this))
			{
				AnimationPropertyContextualMenu.Instance.SetResponder(null);
			}
			Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
		}

		public override bool StartRecording(UnityEngine.Object targetObject)
		{
			return this.StartRecording();
		}

		private bool StartRecording()
		{
			bool result;
			if (this.recording)
			{
				result = true;
			}
			else if (!this.canRecord)
			{
				result = false;
			}
			else if (this.StartPreview())
			{
				AnimationMode.StartAnimationRecording();
				this.ClearCandidates();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override void StopRecording()
		{
			if (this.recording)
			{
				AnimationMode.StopAnimationRecording();
			}
		}

		private void StartCandidateRecording()
		{
			AnimationMode.StartCandidateRecording(this.GetCandidateDriver());
		}

		private void StopCandidateRecording()
		{
			AnimationMode.StopCandidateRecording();
		}

		public override void ResampleAnimation()
		{
			if (!this.state.disabled)
			{
				if (this.previewing)
				{
					if (this.canPreview)
					{
						bool flag = false;
						AnimationMode.BeginSampling();
						AnimationWindowSelectionItem[] array = this.state.selection.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							AnimationWindowSelectionItem animationWindowSelectionItem = array[i];
							if (animationWindowSelectionItem.animationClip != null)
							{
								Undo.FlushUndoRecordObjects();
								AnimationMode.SampleAnimationClip(animationWindowSelectionItem.rootGameObject, animationWindowSelectionItem.animationClip, this.time.time - animationWindowSelectionItem.timeOffset);
								if (this.m_CandidateClip != null)
								{
									AnimationMode.SampleCandidateClip(animationWindowSelectionItem.rootGameObject, this.m_CandidateClip, 0f);
								}
								flag = true;
							}
						}
						AnimationMode.EndSampling();
						if (flag)
						{
							SceneView.RepaintAll();
							InspectorWindow.RepaintAllInspectors();
							ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
							if (instance)
							{
								instance.Repaint();
							}
						}
					}
				}
			}
		}

		private AnimationModeDriver GetAnimationModeDriver()
		{
			if (this.m_Driver == null)
			{
				this.m_Driver = ScriptableObject.CreateInstance<AnimationModeDriver>();
				this.m_Driver.name = "AnimationWindowDriver";
				AnimationModeDriver expr_34 = this.m_Driver;
				expr_34.isKeyCallback = (AnimationModeDriver.IsKeyCallback)Delegate.Combine(expr_34.isKeyCallback, new AnimationModeDriver.IsKeyCallback((UnityEngine.Object target, string propertyPath) => AnimationMode.IsPropertyAnimated(target, propertyPath) && this.KeyExists(new PropertyModification[]
				{
					new PropertyModification
					{
						target = target,
						propertyPath = propertyPath
					}
				})));
			}
			return this.m_Driver;
		}

		private AnimationModeDriver GetCandidateDriver()
		{
			if (this.m_CandidateDriver == null)
			{
				this.m_CandidateDriver = ScriptableObject.CreateInstance<AnimationModeDriver>();
				this.m_CandidateDriver.name = "AnimationWindowCandidateDriver";
			}
			return this.m_CandidateDriver;
		}

		private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
		{
			UndoPropertyModification[] result;
			if (!AnimationMode.InAnimationMode(this.GetAnimationModeDriver()))
			{
				Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				result = modifications;
			}
			else if (this.recording)
			{
				result = this.ProcessAutoKey(modifications);
			}
			else if (this.previewing)
			{
				result = this.RegisterCandidates(modifications);
			}
			else
			{
				result = modifications;
			}
			return result;
		}

		private UndoPropertyModification[] ProcessAutoKey(UndoPropertyModification[] modifications)
		{
			this.BeginKeyModification();
			AnimationWindowControl.RecordingState recordingState = new AnimationWindowControl.RecordingState(this.state, AnimationWindowControl.RecordingStateMode.AutoKey);
			UndoPropertyModification[] result = AnimationRecording.Process(recordingState, modifications);
			this.EndKeyModification();
			return result;
		}

		private UndoPropertyModification[] RegisterCandidates(UndoPropertyModification[] modifications)
		{
			bool flag = this.m_CandidateClip == null;
			if (flag)
			{
				this.m_CandidateClip = new AnimationClip();
				this.m_CandidateClip.legacy = this.state.activeAnimationClip.legacy;
				this.m_CandidateClip.name = "CandidateClip";
				this.StartCandidateRecording();
			}
			AnimationWindowControl.CandidateRecordingState candidateRecordingState = new AnimationWindowControl.CandidateRecordingState(this.state, this.m_CandidateClip);
			UndoPropertyModification[] array = AnimationRecording.Process(candidateRecordingState, modifications);
			if (flag && array.Length == modifications.Length)
			{
				this.ClearCandidates();
			}
			InspectorWindow.RepaintAllInspectors();
			return array;
		}

		private void RemoveFromCandidates(PropertyModification[] modifications)
		{
			if (!(this.m_CandidateClip == null))
			{
				EditorCurveBinding[] array = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.m_CandidateClip);
				if (array.Length != 0)
				{
					Undo.RegisterCompleteObjectUndo(this.m_CandidateClip, "Edit Candidate Curve");
					for (int i = 0; i < array.Length; i++)
					{
						EditorCurveBinding binding = array[i];
						if (binding.isPPtrCurve)
						{
							AnimationUtility.SetObjectReferenceCurve(this.m_CandidateClip, binding, null);
						}
						else
						{
							AnimationUtility.SetEditorCurve(this.m_CandidateClip, binding, null);
						}
					}
					if (AnimationUtility.GetCurveBindings(this.m_CandidateClip).Length == 0 && AnimationUtility.GetObjectReferenceCurveBindings(this.m_CandidateClip).Length == 0)
					{
						this.ClearCandidates();
					}
				}
			}
		}

		public override void ClearCandidates()
		{
			this.m_CandidateClip = null;
			this.StopCandidateRecording();
		}

		public override void ProcessCandidates()
		{
			if (!(this.m_CandidateClip == null))
			{
				this.BeginKeyModification();
				EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.m_CandidateClip);
				EditorCurveBinding[] objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(this.m_CandidateClip);
				List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
				for (int i = 0; i < this.state.allCurves.Count; i++)
				{
					AnimationWindowCurve animationWindowCurve = this.state.allCurves[i];
					EditorCurveBinding remappedBinding = RotationCurveInterpolation.RemapAnimationBindingForRotationCurves(animationWindowCurve.binding, this.m_CandidateClip);
					if (Array.Exists<EditorCurveBinding>(curveBindings, (EditorCurveBinding binding) => remappedBinding.Equals(binding)) || Array.Exists<EditorCurveBinding>(objectReferenceCurveBindings, (EditorCurveBinding binding) => remappedBinding.Equals(binding)))
					{
						list.Add(animationWindowCurve);
					}
				}
				AnimationWindowUtility.AddKeyframes(this.state, list.ToArray(), this.time);
				this.EndKeyModification();
				this.ClearCandidates();
			}
		}

		private List<AnimationWindowKeyframe> GetKeys(PropertyModification[] modifications)
		{
			List<AnimationWindowKeyframe> list = new List<AnimationWindowKeyframe>();
			EditorCurveBinding[] array = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.state.activeAnimationClip);
			List<AnimationWindowKeyframe> result;
			if (array.Length == 0)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < this.state.allCurves.Count; i++)
				{
					AnimationWindowCurve curve = this.state.allCurves[i];
					if (Array.Exists<EditorCurveBinding>(array, (EditorCurveBinding binding) => curve.binding.Equals(binding)))
					{
						int keyframeIndex = curve.GetKeyframeIndex(this.state.time);
						if (keyframeIndex >= 0)
						{
							list.Add(curve.m_Keyframes[keyframeIndex]);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public bool IsAnimatable(PropertyModification[] modifications)
		{
			bool result;
			for (int i = 0; i < modifications.Length; i++)
			{
				PropertyModification propertyModification = modifications[i];
				if (AnimationWindowUtility.PropertyIsAnimatable(propertyModification.target, propertyModification.propertyPath, this.state.activeRootGameObject))
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public bool IsEditable(UnityEngine.Object targetObject)
		{
			bool result;
			if (this.state.selection.disabled)
			{
				result = false;
			}
			else if (!this.previewing)
			{
				result = false;
			}
			else
			{
				AnimationWindowSelectionItem selectedItem = this.state.selectedItem;
				if (selectedItem != null)
				{
					GameObject gameObject = null;
					if (targetObject is Component)
					{
						gameObject = ((Component)targetObject).gameObject;
					}
					else if (targetObject is GameObject)
					{
						gameObject = (GameObject)targetObject;
					}
					if (gameObject != null)
					{
						Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(gameObject.transform);
						if (selectedItem.animationPlayer == closestAnimationPlayerComponentInParents)
						{
							result = selectedItem.animationIsEditable;
							return result;
						}
					}
				}
				result = false;
			}
			return result;
		}

		public bool KeyExists(PropertyModification[] modifications)
		{
			return this.GetKeys(modifications).Count > 0;
		}

		public bool CandidateExists(PropertyModification[] modifications)
		{
			bool result;
			if (!this.HasAnyCandidates())
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < modifications.Length; i++)
				{
					PropertyModification propertyModification = modifications[i];
					if (AnimationMode.IsPropertyCandidate(propertyModification.target, propertyModification.propertyPath))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public bool CurveExists(PropertyModification[] modifications)
		{
			EditorCurveBinding[] array = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.state.activeAnimationClip);
			bool result;
			if (array.Length == 0)
			{
				result = false;
			}
			else
			{
				EditorCurveBinding[] clipBindings = AnimationUtility.GetCurveBindings(this.state.activeAnimationClip);
				if (clipBindings.Length == 0)
				{
					result = false;
				}
				else if (Array.Exists<EditorCurveBinding>(array, (EditorCurveBinding binding) => Array.Exists<EditorCurveBinding>(clipBindings, (EditorCurveBinding clipBinding) => clipBinding.Equals(binding))))
				{
					result = true;
				}
				else
				{
					EditorCurveBinding[] objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(this.state.activeAnimationClip);
					result = (objectReferenceCurveBindings.Length != 0 && Array.Exists<EditorCurveBinding>(objectReferenceCurveBindings, (EditorCurveBinding binding) => Array.Exists<EditorCurveBinding>(clipBindings, (EditorCurveBinding clipBinding) => clipBinding.Equals(binding))));
				}
			}
			return result;
		}

		public bool HasAnyCandidates()
		{
			return this.m_CandidateClip != null;
		}

		public bool HasAnyCurves()
		{
			return this.state.allCurves.Count > 0;
		}

		public void AddKey(SerializedProperty property)
		{
			this.AddKey(AnimationWindowUtility.SerializedPropertyToPropertyModifications(property));
		}

		public void AddKey(PropertyModification[] modifications)
		{
			UndoPropertyModification[] array = new UndoPropertyModification[modifications.Length];
			for (int i = 0; i < modifications.Length; i++)
			{
				PropertyModification propertyModification = modifications[i];
				array[i].previousValue = propertyModification;
				array[i].currentValue = propertyModification;
			}
			this.BeginKeyModification();
			AnimationWindowControl.RecordingState recordingState = new AnimationWindowControl.RecordingState(this.state, AnimationWindowControl.RecordingStateMode.ManualKey);
			AnimationRecording.Process(recordingState, array);
			this.EndKeyModification();
			this.RemoveFromCandidates(modifications);
			this.ResampleAnimation();
			this.state.Repaint();
		}

		public void RemoveKey(SerializedProperty property)
		{
			this.RemoveKey(AnimationWindowUtility.SerializedPropertyToPropertyModifications(property));
		}

		public void RemoveKey(PropertyModification[] modifications)
		{
			this.BeginKeyModification();
			List<AnimationWindowKeyframe> keys = this.GetKeys(modifications);
			this.state.DeleteKeys(keys);
			this.RemoveFromCandidates(modifications);
			this.EndKeyModification();
			this.ResampleAnimation();
			this.state.Repaint();
		}

		public void RemoveCurve(SerializedProperty property)
		{
			this.RemoveCurve(AnimationWindowUtility.SerializedPropertyToPropertyModifications(property));
		}

		public void RemoveCurve(PropertyModification[] modifications)
		{
			EditorCurveBinding[] array = AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, this.state.activeRootGameObject, this.state.activeAnimationClip);
			if (array.Length != 0)
			{
				this.BeginKeyModification();
				Undo.RegisterCompleteObjectUndo(this.state.activeAnimationClip, "Remove Curve");
				for (int i = 0; i < array.Length; i++)
				{
					EditorCurveBinding binding = array[i];
					if (binding.isPPtrCurve)
					{
						AnimationUtility.SetObjectReferenceCurve(this.state.activeAnimationClip, binding, null);
					}
					else
					{
						AnimationUtility.SetEditorCurve(this.state.activeAnimationClip, binding, null);
					}
				}
				this.EndKeyModification();
				this.RemoveFromCandidates(modifications);
				this.ResampleAnimation();
				this.state.Repaint();
			}
		}

		public void AddCandidateKeys()
		{
			this.ProcessCandidates();
			this.ResampleAnimation();
			this.state.Repaint();
		}

		public void AddAnimatedKeys()
		{
			this.BeginKeyModification();
			AnimationWindowUtility.AddKeyframes(this.state, this.state.allCurves.ToArray(), this.time);
			this.ClearCandidates();
			this.EndKeyModification();
			this.ResampleAnimation();
			this.state.Repaint();
		}

		private void BeginKeyModification()
		{
			if (this.animEditor != null)
			{
				this.animEditor.BeginKeyModification();
			}
		}

		private void EndKeyModification()
		{
			if (this.animEditor != null)
			{
				this.animEditor.EndKeyModification();
			}
		}
	}
}
