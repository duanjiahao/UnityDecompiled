using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class TransitionPreview
	{
		private class ParameterInfo
		{
			public string m_Name;
			public float m_Value;
		}
		private class TransitionInfo
		{
			private State m_SrcState;
			private State m_DstState;
			private float m_TransitionDuration;
			private float m_TransitionOffset;
			private float m_ExitTime;
			public TransitionInfo()
			{
				this.Init();
			}
			public bool IsEqual(TransitionPreview.TransitionInfo info)
			{
				return this.m_SrcState == info.m_SrcState && this.m_DstState == info.m_DstState && Mathf.Approximately(this.m_TransitionDuration, info.m_TransitionDuration) && Mathf.Approximately(this.m_TransitionOffset, info.m_TransitionOffset) && Mathf.Approximately(this.m_ExitTime, info.m_ExitTime);
			}
			private void Init()
			{
				this.m_SrcState = null;
				this.m_DstState = null;
				this.m_TransitionDuration = 0f;
				this.m_TransitionOffset = 0f;
				this.m_ExitTime = 0.5f;
			}
			public void Set(Transition transition, State srcState)
			{
				if (transition != null)
				{
					if (srcState != null)
					{
						this.m_SrcState = srcState;
					}
					else
					{
						this.m_SrcState = transition.srcState;
					}
					this.m_DstState = transition.dstState;
					this.m_TransitionDuration = transition.duration;
					this.m_TransitionOffset = transition.offset;
					this.m_ExitTime = 0.5f;
					for (int i = 0; i < transition.conditionCount; i++)
					{
						AnimatorCondition condition = transition.GetCondition(i);
						if (condition.mode == TransitionConditionMode.ExitTime)
						{
							this.m_ExitTime = condition.exitTime;
							break;
						}
					}
				}
				else
				{
					this.Init();
				}
			}
		}
		private AvatarPreview m_AvatarPreview;
		private Timeline m_Timeline;
		private AnimatorController m_Controller;
		private StateMachine m_StateMachine;
		private List<Vector2> m_ParameterMinMax = new List<Vector2>();
		private List<TransitionPreview.ParameterInfo> m_ParameterInfoList;
		private Transition m_RefTransition;
		private TransitionPreview.TransitionInfo m_RefTransitionInfo = new TransitionPreview.TransitionInfo();
		private Transition m_Transition;
		private State m_SrcState;
		private State m_DstState;
		private State m_RefSrcState;
		private bool m_ShowBlendValue;
		private bool m_MustResample = true;
		private bool m_MustSampleMotions;
		private float m_LastEvalTime = -1f;
		private AvatarMask m_LayerMask;
		private int m_LayerIndex;
		private int m_MotionSetIndex;
		private float m_ExitTime = 0.5f;
		private float m_LeftStateWeightA;
		private float m_LeftStateWeightB = 1f;
		private float m_LeftStateTimeA;
		private float m_LeftStateTimeB = 1f;
		private float m_RightStateWeightA;
		private float m_RightStateWeightB = 1f;
		private float m_RightStateTimeA;
		private float m_RightStateTimeB = 1f;
		private List<Timeline.PivotSample> m_SrcPivotList = new List<Timeline.PivotSample>();
		private List<Timeline.PivotSample> m_DstPivotList = new List<Timeline.PivotSample>();
		public bool mustResample
		{
			get
			{
				return this.m_MustResample;
			}
			set
			{
				this.m_MustResample = value;
			}
		}
		private int FindParameterInfo(List<TransitionPreview.ParameterInfo> parameterInfoList, string name)
		{
			int num = -1;
			int num2 = 0;
			while (num2 < parameterInfoList.Count && num == -1)
			{
				if (parameterInfoList[num2].m_Name == name)
				{
					num = num2;
				}
				num2++;
			}
			return num;
		}
		private bool HasExitTime(Transition t)
		{
			for (int i = 0; i < t.conditionCount; i++)
			{
				if (t.GetCondition(i).mode == TransitionConditionMode.ExitTime)
				{
					return true;
				}
			}
			return false;
		}
		private void SetExitTime(Transition t, float time)
		{
			for (int i = 0; i < t.conditionCount; i++)
			{
				AnimatorCondition condition = t.GetCondition(i);
				if (condition.mode == TransitionConditionMode.ExitTime)
				{
					condition.exitTime = time;
					break;
				}
			}
			this.m_ExitTime = time;
		}
		private float GetExitTime(Transition t)
		{
			for (int i = 0; i < t.conditionCount; i++)
			{
				AnimatorCondition condition = t.GetCondition(i);
				if (condition.mode == TransitionConditionMode.ExitTime)
				{
					return condition.exitTime;
				}
			}
			return this.m_ExitTime;
		}
		private void CopyStateForPreview(State src, ref State dst)
		{
			dst.name = src.name;
			dst.iKOnFeet = src.iKOnFeet;
			dst.speed = src.speed;
			dst.mirror = src.mirror;
			dst.SetMotionInternal(src.GetMotionInternal(this.m_MotionSetIndex));
		}
		private void CopyTransitionForPreview(Transition src, ref Transition dst)
		{
			if (src != null)
			{
				dst.duration = src.duration;
				dst.offset = src.offset;
				this.SetExitTime(dst, this.GetExitTime(src));
			}
		}
		private bool MustResample(TransitionPreview.TransitionInfo info)
		{
			return this.mustResample || !info.IsEqual(this.m_RefTransitionInfo);
		}
		private void WriteParametersInController()
		{
			if (this.m_Controller)
			{
				int parameterCount = this.m_Controller.parameterCount;
				for (int i = 0; i < parameterCount; i++)
				{
					string parameterName = this.m_Controller.GetParameterName(i);
					int num = this.FindParameterInfo(this.m_ParameterInfoList, parameterName);
					if (num != -1)
					{
						this.m_AvatarPreview.Animator.SetFloat(parameterName, this.m_ParameterInfoList[num].m_Value);
					}
				}
			}
		}
		private void ResampleTransition(Transition transition, AvatarMask layerMask, TransitionPreview.TransitionInfo info, Animator previewObject)
		{
			this.m_MustResample = false;
			bool flag = this.m_RefTransition != transition;
			this.m_RefTransition = transition;
			this.m_RefTransitionInfo = info;
			this.m_LayerMask = layerMask;
			if (this.m_AvatarPreview != null)
			{
				this.m_AvatarPreview.OnDestroy();
				this.m_AvatarPreview = null;
			}
			this.ClearController();
			Motion motion = (!transition.srcState) ? this.m_RefSrcState.GetMotionInternal(this.m_MotionSetIndex) : transition.srcState.GetMotionInternal(this.m_MotionSetIndex);
			this.Init(previewObject, (!(motion != null)) ? transition.dstState.GetMotionInternal(this.m_MotionSetIndex) : motion);
			if (this.m_Controller == null)
			{
				return;
			}
			this.m_AvatarPreview.Animator.allowConstantClipSamplingOptimization = false;
			this.m_StateMachine.defaultState = this.m_DstState;
			this.m_Transition.mute = true;
			AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
			this.m_AvatarPreview.Animator.Update(1E-05f);
			this.WriteParametersInController();
			this.m_AvatarPreview.Animator.SetLayerWeight(this.m_LayerIndex, 1f);
			float length = this.m_AvatarPreview.Animator.GetCurrentAnimatorStateInfo(this.m_LayerIndex).length;
			this.m_StateMachine.defaultState = this.m_SrcState;
			this.m_Transition.mute = false;
			AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
			this.m_AvatarPreview.Animator.Update(1E-05f);
			this.WriteParametersInController();
			this.m_AvatarPreview.Animator.SetLayerWeight(this.m_LayerIndex, 1f);
			float length2 = this.m_AvatarPreview.Animator.GetCurrentAnimatorStateInfo(this.m_LayerIndex).length;
			if (this.m_LayerIndex > 0)
			{
				this.m_AvatarPreview.Animator.stabilizeFeet = false;
			}
			float num = length2 * this.GetExitTime(this.m_RefTransition) + length2 * this.m_Transition.duration + length;
			if (num > 2000f)
			{
				Debug.LogWarning("Transition duration is longer than 2000 second, Disabling previewer.");
				return;
			}
			float num2 = (length2 <= 0f) ? 0.0333333351f : Mathf.Min(Mathf.Max(length2 / 300f, 0.0333333351f), length2 / 5f);
			float num3 = (length <= 0f) ? 0.0333333351f : Mathf.Min(Mathf.Max(length / 300f, 0.0333333351f), length / 5f);
			float num4 = num2;
			float num5 = 0f;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			this.m_AvatarPreview.Animator.StartRecording(-1);
			this.m_LeftStateWeightA = 0f;
			this.m_LeftStateTimeA = 0f;
			this.m_AvatarPreview.Animator.Update(0f);
			while (!flag4)
			{
				this.m_AvatarPreview.Animator.Update(num4);
				AnimatorStateInfo currentAnimatorStateInfo = this.m_AvatarPreview.Animator.GetCurrentAnimatorStateInfo(this.m_LayerIndex);
				num5 += num4;
				if (!flag2)
				{
					this.m_LeftStateWeightA = currentAnimatorStateInfo.normalizedTime;
					this.m_LeftStateTimeA = num5;
					flag2 = true;
				}
				if (flag3 && num5 >= num)
				{
					flag4 = true;
				}
				if (!flag3 && currentAnimatorStateInfo.IsName(this.m_DstState.uniqueName))
				{
					this.m_RightStateWeightA = currentAnimatorStateInfo.normalizedTime;
					this.m_RightStateTimeA = num5;
					flag3 = true;
				}
				if (!flag3)
				{
					this.m_LeftStateWeightB = currentAnimatorStateInfo.normalizedTime;
					this.m_LeftStateTimeB = num5;
				}
				if (flag3)
				{
					this.m_RightStateWeightB = currentAnimatorStateInfo.normalizedTime;
					this.m_RightStateTimeB = num5;
				}
				if (this.m_AvatarPreview.Animator.IsInTransition(this.m_LayerIndex))
				{
					num4 = num3;
				}
			}
			float stopTime = num5;
			this.m_AvatarPreview.Animator.StopRecording();
			float num6 = (this.m_LeftStateTimeB - this.m_LeftStateTimeA) / (this.m_LeftStateWeightB - this.m_LeftStateWeightA);
			float num7 = (this.m_RightStateTimeB - this.m_RightStateTimeA) / (this.m_RightStateWeightB - this.m_RightStateWeightA);
			if (this.m_MustSampleMotions)
			{
				this.m_MustSampleMotions = false;
				this.m_SrcPivotList.Clear();
				this.m_DstPivotList.Clear();
				num4 = num3;
				this.m_StateMachine.defaultState = this.m_DstState;
				this.m_Transition.mute = true;
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				this.m_AvatarPreview.Animator.SetLayerWeight(this.m_LayerIndex, 1f);
				this.m_AvatarPreview.Animator.Update(1E-07f);
				this.WriteParametersInController();
				num5 = 0f;
				while (num5 <= num7)
				{
					this.m_AvatarPreview.Animator.Update(num4 * 2f);
					num5 += num4 * 2f;
					Timeline.PivotSample pivotSample = new Timeline.PivotSample();
					pivotSample.m_Time = num5;
					pivotSample.m_Weight = this.m_AvatarPreview.Animator.pivotWeight;
					this.m_DstPivotList.Add(pivotSample);
				}
				num4 = num2;
				this.m_StateMachine.defaultState = this.m_SrcState;
				this.m_Transition.mute = true;
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				this.m_AvatarPreview.Animator.Update(1E-07f);
				this.WriteParametersInController();
				this.m_AvatarPreview.Animator.SetLayerWeight(this.m_LayerIndex, 1f);
				num5 = 0f;
				while (num5 <= num6)
				{
					this.m_AvatarPreview.Animator.Update(num4 * 2f);
					num5 += num4 * 2f;
					Timeline.PivotSample pivotSample2 = new Timeline.PivotSample();
					pivotSample2.m_Time = num5;
					pivotSample2.m_Weight = this.m_AvatarPreview.Animator.pivotWeight;
					this.m_SrcPivotList.Add(pivotSample2);
				}
				this.m_Transition.mute = false;
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				this.m_AvatarPreview.Animator.Update(1E-07f);
				this.WriteParametersInController();
			}
			this.m_Timeline.StopTime = (this.m_AvatarPreview.timeControl.stopTime = stopTime);
			this.m_AvatarPreview.timeControl.currentTime = this.m_Timeline.Time;
			if (flag)
			{
				Timeline arg_6DE_0 = this.m_Timeline;
				float num8 = this.m_AvatarPreview.timeControl.currentTime = (this.m_AvatarPreview.timeControl.startTime = 0f);
				this.m_Timeline.StartTime = num8;
				arg_6DE_0.Time = num8;
				this.m_Timeline.ResetRange();
			}
			this.m_AvatarPreview.Animator.StartPlayback();
		}
		public void SetAnyStateTransition(Transition transition, State sourceState, AvatarMask layerMask, int motionSetIndex, Animator previewObject)
		{
			TransitionPreview.TransitionInfo transitionInfo = new TransitionPreview.TransitionInfo();
			transitionInfo.Set(transition, sourceState);
			this.m_MotionSetIndex = motionSetIndex;
			if (this.MustResample(transitionInfo))
			{
				this.m_RefSrcState = sourceState;
				this.ResampleTransition(transition, layerMask, transitionInfo, previewObject);
			}
		}
		public void SetTransition(Transition transition, AvatarMask layerMask, int motionSetIndex, Animator previewObject)
		{
			TransitionPreview.TransitionInfo transitionInfo = new TransitionPreview.TransitionInfo();
			transitionInfo.Set(transition, null);
			this.m_MotionSetIndex = motionSetIndex;
			if (this.MustResample(transitionInfo))
			{
				this.ResampleTransition(transition, layerMask, transitionInfo, previewObject);
			}
		}
		private void OnPreviewAvatarChanged()
		{
			this.m_RefTransitionInfo = new TransitionPreview.TransitionInfo();
			this.ClearController();
			this.CreateController();
			this.CreateParameterInfoList();
		}
		private void ClearController()
		{
			if (this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null)
			{
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, null);
			}
			UnityEngine.Object.DestroyImmediate(this.m_Controller);
			UnityEngine.Object.DestroyImmediate(this.m_SrcState);
			UnityEngine.Object.DestroyImmediate(this.m_DstState);
			UnityEngine.Object.DestroyImmediate(this.m_Transition);
			this.m_StateMachine = null;
			this.m_Controller = null;
			this.m_SrcState = null;
			this.m_DstState = null;
			this.m_Transition = null;
		}
		private void CreateParameterInfoList()
		{
			this.m_ParameterInfoList = new List<TransitionPreview.ParameterInfo>();
			if (this.m_Controller)
			{
				int parameterCount = this.m_Controller.parameterCount;
				for (int i = 0; i < parameterCount; i++)
				{
					TransitionPreview.ParameterInfo parameterInfo = new TransitionPreview.ParameterInfo();
					parameterInfo.m_Name = this.m_Controller.GetParameterName(i);
					this.m_ParameterInfoList.Add(parameterInfo);
				}
			}
		}
		private void CreateController()
		{
			if (this.m_Controller == null && this.m_AvatarPreview != null && this.m_AvatarPreview.Animator != null && this.m_RefTransition != null)
			{
				this.m_LayerIndex = 0;
				this.m_Controller = new AnimatorController();
				this.m_Controller.hideFlags = HideFlags.DontSave;
				this.m_Controller.AddLayer("preview");
				bool flag = true;
				if (this.m_LayerMask != null)
				{
					int humanoidBodyPartCount = this.m_LayerMask.humanoidBodyPartCount;
					int num = 0;
					while (num < humanoidBodyPartCount && flag)
					{
						if (!this.m_LayerMask.GetHumanoidBodyPartActive(num))
						{
							flag = false;
						}
						num++;
					}
					if (!flag)
					{
						this.m_Controller.AddLayer("Additionnal");
						this.m_LayerIndex++;
						this.m_Controller.SetLayerMask(this.m_LayerIndex, this.m_LayerMask);
					}
				}
				this.m_StateMachine = this.m_Controller.GetLayerStateMachine(this.m_LayerIndex);
				State state = (!this.m_RefTransition.srcState) ? this.m_RefSrcState : this.m_RefTransition.srcState;
				this.m_ParameterMinMax.Clear();
				Motion motionInternal = state.GetMotionInternal(this.m_MotionSetIndex);
				if (motionInternal && motionInternal is BlendTree)
				{
					BlendTree blendTree = motionInternal as BlendTree;
					for (int i = 0; i < blendTree.recursiveBlendParameterCount; i++)
					{
						string recursiveBlendParameter = blendTree.GetRecursiveBlendParameter(i);
						if (this.m_Controller.FindParameter(recursiveBlendParameter) == -1)
						{
							this.m_Controller.AddParameter(recursiveBlendParameter, AnimatorControllerParameterType.Float);
							this.m_ParameterMinMax.Add(new Vector2(blendTree.GetRecursiveBlendParameterMin(i), blendTree.GetRecursiveBlendParameterMax(i)));
						}
					}
				}
				Motion motionInternal2 = this.m_RefTransition.dstState.GetMotionInternal(this.m_MotionSetIndex);
				if (motionInternal2 && motionInternal2 is BlendTree)
				{
					BlendTree blendTree2 = motionInternal2 as BlendTree;
					for (int j = 0; j < blendTree2.recursiveBlendParameterCount; j++)
					{
						string recursiveBlendParameter2 = blendTree2.GetRecursiveBlendParameter(j);
						int num2 = this.m_Controller.FindParameter(recursiveBlendParameter2);
						if (num2 == -1)
						{
							this.m_Controller.AddParameter(recursiveBlendParameter2, AnimatorControllerParameterType.Float);
							this.m_ParameterMinMax.Add(new Vector2(blendTree2.GetRecursiveBlendParameterMin(j), blendTree2.GetRecursiveBlendParameterMax(j)));
						}
						else
						{
							this.m_ParameterMinMax[num2] = new Vector2(Mathf.Min(blendTree2.GetRecursiveBlendParameterMin(j), this.m_ParameterMinMax[num2][0]), Mathf.Max(blendTree2.GetRecursiveBlendParameterMax(j), this.m_ParameterMinMax[num2][1]));
						}
					}
				}
				this.m_SrcState = this.m_StateMachine.AddState(state.name);
				this.m_SrcState.hideFlags = HideFlags.DontSave;
				this.m_DstState = this.m_StateMachine.AddState(this.m_RefTransition.dstState.name);
				this.m_DstState.hideFlags = HideFlags.DontSave;
				this.CopyStateForPreview(state, ref this.m_SrcState);
				this.CopyStateForPreview(this.m_RefTransition.dstState, ref this.m_DstState);
				this.m_Transition = this.m_StateMachine.AddTransition(this.m_SrcState, this.m_DstState);
				this.m_Transition.hideFlags = HideFlags.DontSave;
				this.CopyTransitionForPreview(this.m_RefTransition, ref this.m_Transition);
				this.DisableIKOnFeetIfNeeded();
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
			}
		}
		private void DisableIKOnFeetIfNeeded()
		{
			bool flag = false;
			if (this.m_SrcState.GetMotion() == null || this.m_DstState.GetMotion() == null)
			{
				flag = true;
			}
			if (this.m_LayerIndex > 0)
			{
				flag = !this.m_LayerMask.hasFeetIK;
			}
			if (flag)
			{
				this.m_SrcState.iKOnFeet = false;
				this.m_DstState.iKOnFeet = false;
			}
		}
		private void Init(Animator scenePreviewObject, Motion motion)
		{
			if (this.m_AvatarPreview == null)
			{
				this.m_AvatarPreview = new AvatarPreview(scenePreviewObject, motion);
				this.m_AvatarPreview.OnAvatarChangeFunc = new AvatarPreview.OnAvatarChange(this.OnPreviewAvatarChanged);
				this.m_AvatarPreview.ShowIKOnFeetButton = false;
			}
			if (this.m_Timeline == null)
			{
				this.m_Timeline = new Timeline();
				this.m_MustSampleMotions = true;
			}
			this.CreateController();
			if (this.m_ParameterInfoList == null)
			{
				this.CreateParameterInfoList();
			}
		}
		public void DoTransitionPreview()
		{
			if (this.m_Controller == null)
			{
				return;
			}
			if (Event.current.type == EventType.Repaint)
			{
				this.m_AvatarPreview.timeControl.Update();
			}
			this.DoTimeline();
			if (this.m_Controller.parameterCount > 0)
			{
				this.m_ShowBlendValue = EditorGUILayout.Foldout(this.m_ShowBlendValue, "BlendTree Parameters");
				if (this.m_ShowBlendValue)
				{
					for (int i = 0; i < this.m_Controller.parameterCount; i++)
					{
						string parameterName = this.m_Controller.GetParameterName(i);
						float value = this.m_ParameterInfoList[i].m_Value;
						float num = EditorGUILayout.Slider(parameterName, value, this.m_ParameterMinMax[i][0], this.m_ParameterMinMax[i][1], new GUILayoutOption[0]);
						if (num != value)
						{
							this.m_ParameterInfoList[i].m_Value = num;
							this.mustResample = true;
							this.m_MustSampleMotions = true;
						}
					}
				}
			}
		}
		private void DoTimeline()
		{
			float num = (this.m_LeftStateTimeB - this.m_LeftStateTimeA) / (this.m_LeftStateWeightB - this.m_LeftStateWeightA);
			float num2 = (this.m_RightStateTimeB - this.m_RightStateTimeA) / (this.m_RightStateWeightB - this.m_RightStateWeightA);
			float num3 = this.m_Transition.duration * num;
			this.m_Timeline.SrcStartTime = 0f;
			this.m_Timeline.SrcStopTime = num;
			this.m_Timeline.SrcName = this.m_SrcState.name;
			this.m_Timeline.HasExitTime = this.HasExitTime(this.m_RefTransition);
			this.m_Timeline.srcLoop = (this.m_SrcState.GetMotion() && this.m_SrcState.GetMotion().isLooping);
			this.m_Timeline.dstLoop = (this.m_DstState.GetMotion() && this.m_DstState.GetMotion().isLooping);
			this.m_Timeline.TransitionStartTime = this.GetExitTime(this.m_RefTransition) * num;
			this.m_Timeline.TransitionStopTime = this.m_Timeline.TransitionStartTime + num3;
			this.m_Timeline.Time = this.m_AvatarPreview.timeControl.currentTime;
			this.m_Timeline.DstStartTime = this.m_Timeline.TransitionStartTime - this.m_RefTransition.offset * num2;
			this.m_Timeline.DstStopTime = this.m_Timeline.DstStartTime + num2;
			if (this.m_Timeline.TransitionStopTime == float.PositiveInfinity)
			{
				this.m_Timeline.TransitionStopTime = Mathf.Min(this.m_Timeline.DstStopTime, this.m_Timeline.SrcStopTime);
			}
			this.m_Timeline.DstName = this.m_DstState.name;
			this.m_Timeline.SrcPivotList = this.m_SrcPivotList;
			this.m_Timeline.DstPivotList = this.m_DstPivotList;
			Rect controlRect = EditorGUILayout.GetControlRect(false, 150f, EditorStyles.label, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			bool flag = this.m_Timeline.DoTimeline(controlRect);
			if (EditorGUI.EndChangeCheck())
			{
				if (flag)
				{
					Undo.RegisterCompleteObjectUndo(this.m_RefTransition, "Edit Transition");
					this.SetExitTime(this.m_RefTransition, this.m_Timeline.TransitionStartTime / this.m_Timeline.SrcDuration);
					this.m_RefTransition.duration = this.m_Timeline.TransitionDuration / this.m_Timeline.SrcDuration;
					this.m_RefTransition.offset = (this.m_Timeline.TransitionStartTime - this.m_Timeline.DstStartTime) / this.m_Timeline.DstDuration;
				}
				this.m_AvatarPreview.timeControl.nextCurrentTime = Mathf.Clamp(this.m_Timeline.Time, 0f, this.m_AvatarPreview.timeControl.stopTime);
			}
		}
		public void OnDisable()
		{
			this.ClearController();
		}
		public void OnDestroy()
		{
			this.ClearController();
			if (this.m_Timeline != null)
			{
				this.m_Timeline = null;
			}
			if (this.m_AvatarPreview != null)
			{
				this.m_AvatarPreview.OnDestroy();
				this.m_AvatarPreview = null;
			}
		}
		public bool HasPreviewGUI()
		{
			return true;
		}
		public void OnPreviewSettings()
		{
			if (this.m_AvatarPreview != null)
			{
				this.m_AvatarPreview.DoPreviewSettings();
			}
		}
		public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_AvatarPreview != null && this.m_Controller != null)
			{
				if (this.m_LastEvalTime != this.m_AvatarPreview.timeControl.currentTime && Event.current.type == EventType.Repaint)
				{
					this.m_AvatarPreview.Animator.playbackTime = this.m_AvatarPreview.timeControl.currentTime;
					this.m_AvatarPreview.Animator.Update(0f);
					this.m_LastEvalTime = this.m_AvatarPreview.timeControl.currentTime;
				}
				this.m_AvatarPreview.DoAvatarPreview(r, background);
			}
		}
	}
}
