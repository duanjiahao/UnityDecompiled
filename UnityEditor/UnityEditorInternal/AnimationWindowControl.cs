using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationWindowControl : IAnimationWindowControl
	{
		[SerializeField]
		private AnimationKeyTime m_Time;

		[NonSerialized]
		private AnimationRecordMode m_Recording;

		[NonSerialized]
		private float m_PreviousUpdateTime;

		[NonSerialized]
		public AnimationWindowState state;

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
				return this.canRecord;
			}
		}

		public override bool playing
		{
			get
			{
				return AnimationMode.InAnimationPlaybackMode() && this.recording;
			}
		}

		public override bool canRecord
		{
			get
			{
				return this.state.selection.canRecord && this.m_Recording != null && this.m_Recording.canEnable;
			}
		}

		public override bool recording
		{
			get
			{
				return this.m_Recording != null && this.m_Recording.enable;
			}
		}

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Recording = new AnimationRecordMode();
		}

		public void OnDisable()
		{
			this.StopRecording();
			this.StopPlayback();
			if (this.m_Recording != null)
			{
				this.m_Recording.Dispose();
				this.m_Recording = null;
			}
		}

		public override void OnSelectionChanged()
		{
			if (this.state != null)
			{
				this.m_Time = AnimationKeyTime.Time(0f, this.state.frameRate);
			}
			this.StopRecording();
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

		public override void GoToNextKeyframe()
		{
			List<AnimationWindowCurve> list = (!this.state.showCurveEditor || this.state.activeCurves.Count <= 0) ? this.state.allCurves : this.state.activeCurves;
			float nextKeyframeTime = AnimationWindowUtility.GetNextKeyframeTime(list.ToArray(), this.time.time, this.state.clipFrameRate);
			this.SetCurrentTime(this.state.SnapToFrame(nextKeyframeTime, AnimationWindowState.SnapMode.SnapToClipFrame));
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
				this.StartRecording();
				this.ResampleAnimation();
			}
		}

		private void SetCurrentFrame(int value)
		{
			if (value != this.time.frame)
			{
				this.m_Time = AnimationKeyTime.Frame(value, this.state.frameRate);
				this.StartRecording();
				this.ResampleAnimation();
			}
		}

		public override void StartPlayback()
		{
			if (this.canPlay)
			{
				if (!AnimationMode.InAnimationPlaybackMode())
				{
					AnimationMode.StartAnimationPlaybackMode();
					this.m_PreviousUpdateTime = Time.realtimeSinceStartup;
					this.StartRecording();
				}
			}
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

		public override void StartRecording(UnityEngine.Object targetObject)
		{
			this.StartRecording();
		}

		private void StartRecording()
		{
			if (this.canRecord)
			{
				if (this.m_Recording != null)
				{
					if (!this.m_Recording.enable)
					{
						this.m_Recording.enable = true;
						Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
					}
				}
			}
		}

		public override void StopRecording()
		{
			this.StopPlayback();
			if (this.m_Recording != null)
			{
				if (this.m_Recording.enable)
				{
					this.m_Recording.enable = false;
					Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				}
			}
		}

		public override void ResampleAnimation()
		{
			if (!this.state.disabled)
			{
				if (this.recording)
				{
					if (this.canRecord)
					{
						bool flag = false;
						AnimationWindowSelectionItem[] array = this.state.selection.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							AnimationWindowSelectionItem animationWindowSelectionItem = array[i];
							if (animationWindowSelectionItem.animationClip != null)
							{
								Undo.FlushUndoRecordObjects();
								AnimationMode.BeginSampling();
								AnimationMode.SampleAnimationClip(animationWindowSelectionItem.rootGameObject, animationWindowSelectionItem.animationClip, this.time.time - animationWindowSelectionItem.timeOffset);
								AnimationMode.EndSampling();
								flag = true;
							}
						}
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

		private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
		{
			UndoPropertyModification[] result;
			if (!AnimationMode.InAnimationMode())
			{
				Undo.postprocessModifications = (Undo.PostprocessModifications)Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
				result = modifications;
			}
			else
			{
				result = AnimationRecording.Process(this.state, modifications);
			}
			return result;
		}
	}
}
