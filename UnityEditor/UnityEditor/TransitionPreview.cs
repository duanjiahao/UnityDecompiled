using System;
using System.Collections.Generic;
using UnityEditor.Animations;
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
			private AnimatorState m_SrcState;

			private AnimatorState m_DstState;

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

			public void Set(AnimatorStateTransition transition, AnimatorState srcState, AnimatorState dstState)
			{
				if (transition != null)
				{
					this.m_SrcState = srcState;
					this.m_DstState = dstState;
					this.m_TransitionDuration = transition.duration;
					this.m_TransitionOffset = transition.offset;
					this.m_ExitTime = 0.5f;
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

		private AnimatorStateMachine m_StateMachine;

		private List<Vector2> m_ParameterMinMax = new List<Vector2>();

		private List<TransitionPreview.ParameterInfo> m_ParameterInfoList;

		private AnimatorStateTransition m_RefTransition;

		private TransitionPreview.TransitionInfo m_RefTransitionInfo = new TransitionPreview.TransitionInfo();

		private AnimatorStateTransition m_Transition;

		private AnimatorState m_SrcState;

		private AnimatorState m_DstState;

		private AnimatorState m_RefSrcState;

		private AnimatorState m_RefDstState;

		private Motion m_SrcMotion;

		private Motion m_DstMotion;

		private bool m_ShowBlendValue;

		private bool m_MustResample = true;

		private bool m_MustSampleMotions;

		private float m_LastEvalTime = -1f;

		private bool m_IsResampling;

		private AvatarMask m_LayerMask;

		private int m_LayerIndex;

		private bool m_ValidTransition = true;

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

		private void SetMotion(AnimatorState state, int layerIndex, Motion motion)
		{
			AnimatorControllerLayer[] layers = this.m_Controller.layers;
			state.motion = motion;
			this.m_Controller.layers = layers;
		}

		private void CopyStateForPreview(AnimatorState src, ref AnimatorState dst)
		{
			dst.iKOnFeet = src.iKOnFeet;
			dst.speed = src.speed;
			dst.mirror = src.mirror;
			dst.motion = src.motion;
		}

		private void CopyTransitionForPreview(AnimatorStateTransition src, ref AnimatorStateTransition dst)
		{
			if (src != null)
			{
				dst.duration = src.duration;
				dst.offset = src.offset;
				dst.exitTime = src.exitTime;
				dst.hasFixedDuration = src.hasFixedDuration;
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
				int num = this.m_Controller.parameters.Length;
				for (int i = 0; i < num; i++)
				{
					string name = this.m_Controller.parameters[i].name;
					int num2 = this.FindParameterInfo(this.m_ParameterInfoList, name);
					if (num2 != -1)
					{
						this.m_AvatarPreview.Animator.SetFloat(name, this.m_ParameterInfoList[num2].m_Value);
					}
				}
			}
		}

		private void ResampleTransition(AnimatorStateTransition transition, AvatarMask layerMask, TransitionPreview.TransitionInfo info, Animator previewObject)
		{
			this.m_IsResampling = true;
			this.m_MustResample = false;
			this.m_ValidTransition = true;
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
			Motion motion = this.m_RefSrcState.motion;
			this.Init(previewObject, (!(motion != null)) ? this.m_RefDstState.motion : motion);
			if (this.m_Controller == null)
			{
				this.m_IsResampling = false;
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
			float num = length2 * this.m_RefTransition.exitTime + this.m_Transition.duration * ((!this.m_RefTransition.hasFixedDuration) ? length2 : 1f) + length;
			if (num > 2000f)
			{
				Debug.LogWarning("Transition duration is longer than 2000 second, Disabling previewer.");
				this.m_ValidTransition = false;
				this.m_IsResampling = false;
				return;
			}
			float num2 = (this.m_RefTransition.exitTime <= 0f) ? length2 : (length2 * this.m_RefTransition.exitTime);
			float num3 = (num2 <= 0f) ? 0.0333333351f : Mathf.Min(Mathf.Max(num2 / 300f, 0.0333333351f), num2 / 5f);
			float num4 = (length <= 0f) ? 0.0333333351f : Mathf.Min(Mathf.Max(length / 300f, 0.0333333351f), length / 5f);
			num3 = Mathf.Max(num3, num / 600f);
			num4 = Mathf.Max(num4, num / 600f);
			float num5 = num3;
			float num6 = 0f;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			this.m_AvatarPreview.Animator.StartRecording(-1);
			this.m_LeftStateWeightA = 0f;
			this.m_LeftStateTimeA = 0f;
			this.m_AvatarPreview.Animator.Update(0f);
			while (!flag4 && num6 < num)
			{
				this.m_AvatarPreview.Animator.Update(num5);
				AnimatorStateInfo currentAnimatorStateInfo = this.m_AvatarPreview.Animator.GetCurrentAnimatorStateInfo(this.m_LayerIndex);
				num6 += num5;
				if (!flag2)
				{
					this.m_LeftStateWeightA = (this.m_LeftStateWeightB = currentAnimatorStateInfo.normalizedTime);
					this.m_LeftStateTimeA = (this.m_LeftStateTimeB = num6);
					flag2 = true;
				}
				if (flag3 && num6 >= num)
				{
					flag4 = true;
				}
				if (!flag3 && currentAnimatorStateInfo.IsName(this.m_DstState.name))
				{
					this.m_RightStateWeightA = currentAnimatorStateInfo.normalizedTime;
					this.m_RightStateTimeA = num6;
					flag3 = true;
				}
				if (!flag3)
				{
					this.m_LeftStateWeightB = currentAnimatorStateInfo.normalizedTime;
					this.m_LeftStateTimeB = num6;
				}
				if (flag3)
				{
					this.m_RightStateWeightB = currentAnimatorStateInfo.normalizedTime;
					this.m_RightStateTimeB = num6;
				}
				if (this.m_AvatarPreview.Animator.IsInTransition(this.m_LayerIndex))
				{
					num5 = num4;
				}
			}
			float stopTime = num6;
			this.m_AvatarPreview.Animator.StopRecording();
			if (Mathf.Approximately(this.m_LeftStateWeightB, this.m_LeftStateWeightA) || Mathf.Approximately(this.m_RightStateWeightB, this.m_RightStateWeightA))
			{
				Debug.LogWarning("Difference in effective length between states is too big. Transition preview will be disabled.");
				this.m_ValidTransition = false;
				this.m_IsResampling = false;
				return;
			}
			float num7 = (this.m_LeftStateTimeB - this.m_LeftStateTimeA) / (this.m_LeftStateWeightB - this.m_LeftStateWeightA);
			float num8 = (this.m_RightStateTimeB - this.m_RightStateTimeA) / (this.m_RightStateWeightB - this.m_RightStateWeightA);
			if (this.m_MustSampleMotions)
			{
				this.m_MustSampleMotions = false;
				this.m_SrcPivotList.Clear();
				this.m_DstPivotList.Clear();
				num5 = num4;
				this.m_StateMachine.defaultState = this.m_DstState;
				this.m_Transition.mute = true;
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				this.m_AvatarPreview.Animator.Update(0f);
				this.m_AvatarPreview.Animator.SetLayerWeight(this.m_LayerIndex, 1f);
				this.m_AvatarPreview.Animator.Update(1E-07f);
				this.WriteParametersInController();
				for (num6 = 0f; num6 <= num8; num6 += num5 * 2f)
				{
					Timeline.PivotSample pivotSample = new Timeline.PivotSample();
					pivotSample.m_Time = num6;
					pivotSample.m_Weight = this.m_AvatarPreview.Animator.pivotWeight;
					this.m_DstPivotList.Add(pivotSample);
					this.m_AvatarPreview.Animator.Update(num5 * 2f);
				}
				num5 = num3;
				this.m_StateMachine.defaultState = this.m_SrcState;
				this.m_Transition.mute = true;
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				this.m_AvatarPreview.Animator.Update(1E-07f);
				this.WriteParametersInController();
				this.m_AvatarPreview.Animator.SetLayerWeight(this.m_LayerIndex, 1f);
				for (num6 = 0f; num6 <= num7; num6 += num5 * 2f)
				{
					Timeline.PivotSample pivotSample2 = new Timeline.PivotSample();
					pivotSample2.m_Time = num6;
					pivotSample2.m_Weight = this.m_AvatarPreview.Animator.pivotWeight;
					this.m_SrcPivotList.Add(pivotSample2);
					this.m_AvatarPreview.Animator.Update(num5 * 2f);
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
				Timeline arg_7B0_0 = this.m_Timeline;
				float num9 = this.m_AvatarPreview.timeControl.currentTime = (this.m_AvatarPreview.timeControl.startTime = 0f);
				this.m_Timeline.StartTime = num9;
				arg_7B0_0.Time = num9;
				this.m_Timeline.ResetRange();
			}
			this.m_AvatarPreview.Animator.StartPlayback();
			this.m_IsResampling = false;
		}

		public void SetTransition(AnimatorStateTransition transition, AnimatorState sourceState, AnimatorState destinationState, AnimatorControllerLayer srcLayer, Animator previewObject)
		{
			this.m_RefSrcState = sourceState;
			this.m_RefDstState = destinationState;
			TransitionPreview.TransitionInfo transitionInfo = new TransitionPreview.TransitionInfo();
			transitionInfo.Set(transition, sourceState, destinationState);
			if (this.MustResample(transitionInfo))
			{
				this.ResampleTransition(transition, srcLayer.avatarMask, transitionInfo, previewObject);
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
			if (this.m_Controller && this.m_Controller.parameters != null)
			{
				int num = this.m_Controller.parameters.Length;
				for (int i = 0; i < num; i++)
				{
					TransitionPreview.ParameterInfo parameterInfo = new TransitionPreview.ParameterInfo();
					parameterInfo.m_Name = this.m_Controller.parameters[i].name;
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
				this.m_Controller.pushUndo = false;
				this.m_Controller.hideFlags = HideFlags.HideAndDontSave;
				this.m_Controller.AddLayer("preview");
				bool flag = true;
				if (this.m_LayerMask != null)
				{
					AvatarMaskBodyPart avatarMaskBodyPart = AvatarMaskBodyPart.Root;
					while (avatarMaskBodyPart < AvatarMaskBodyPart.LastBodyPart && flag)
					{
						if (!this.m_LayerMask.GetHumanoidBodyPartActive(avatarMaskBodyPart))
						{
							flag = false;
						}
						avatarMaskBodyPart++;
					}
					if (!flag)
					{
						this.m_Controller.AddLayer("Additionnal");
						this.m_LayerIndex++;
						AnimatorControllerLayer[] layers = this.m_Controller.layers;
						layers[this.m_LayerIndex].avatarMask = this.m_LayerMask;
						this.m_Controller.layers = layers;
					}
				}
				this.m_StateMachine = this.m_Controller.layers[this.m_LayerIndex].stateMachine;
				this.m_StateMachine.pushUndo = false;
				this.m_StateMachine.hideFlags = HideFlags.HideAndDontSave;
				this.m_SrcMotion = this.m_RefSrcState.motion;
				this.m_DstMotion = this.m_RefDstState.motion;
				this.m_ParameterMinMax.Clear();
				if (this.m_SrcMotion && this.m_SrcMotion is BlendTree)
				{
					BlendTree blendTree = this.m_SrcMotion as BlendTree;
					for (int i = 0; i < blendTree.recursiveBlendParameterCount; i++)
					{
						string recursiveBlendParameter = blendTree.GetRecursiveBlendParameter(i);
						if (this.m_Controller.IndexOfParameter(recursiveBlendParameter) == -1)
						{
							this.m_Controller.AddParameter(recursiveBlendParameter, AnimatorControllerParameterType.Float);
							this.m_ParameterMinMax.Add(new Vector2(blendTree.GetRecursiveBlendParameterMin(i), blendTree.GetRecursiveBlendParameterMax(i)));
						}
					}
				}
				if (this.m_DstMotion && this.m_DstMotion is BlendTree)
				{
					BlendTree blendTree2 = this.m_DstMotion as BlendTree;
					for (int j = 0; j < blendTree2.recursiveBlendParameterCount; j++)
					{
						string recursiveBlendParameter2 = blendTree2.GetRecursiveBlendParameter(j);
						int num = this.m_Controller.IndexOfParameter(recursiveBlendParameter2);
						if (num == -1)
						{
							this.m_Controller.AddParameter(recursiveBlendParameter2, AnimatorControllerParameterType.Float);
							this.m_ParameterMinMax.Add(new Vector2(blendTree2.GetRecursiveBlendParameterMin(j), blendTree2.GetRecursiveBlendParameterMax(j)));
						}
						else
						{
							this.m_ParameterMinMax[num] = new Vector2(Mathf.Min(blendTree2.GetRecursiveBlendParameterMin(j), this.m_ParameterMinMax[num][0]), Mathf.Max(blendTree2.GetRecursiveBlendParameterMax(j), this.m_ParameterMinMax[num][1]));
						}
					}
				}
				this.m_SrcState = this.m_StateMachine.AddState(this.m_RefSrcState.name);
				this.m_SrcState.pushUndo = false;
				this.m_SrcState.hideFlags = HideFlags.HideAndDontSave;
				this.m_DstState = this.m_StateMachine.AddState(this.m_RefDstState.name);
				this.m_DstState.pushUndo = false;
				this.m_DstState.hideFlags = HideFlags.HideAndDontSave;
				this.CopyStateForPreview(this.m_RefSrcState, ref this.m_SrcState);
				this.CopyStateForPreview(this.m_RefDstState, ref this.m_DstState);
				this.m_Transition = this.m_SrcState.AddTransition(this.m_DstState, true);
				this.m_Transition.pushUndo = false;
				this.m_Transition.hideFlags = HideFlags.DontSave;
				this.CopyTransitionForPreview(this.m_RefTransition, ref this.m_Transition);
				this.DisableIKOnFeetIfNeeded();
				AnimatorController.SetAnimatorController(this.m_AvatarPreview.Animator, this.m_Controller);
				AnimatorController expr_3F0 = this.m_Controller;
				expr_3F0.OnAnimatorControllerDirty = (Action)Delegate.Combine(expr_3F0.OnAnimatorControllerDirty, new Action(this.ControllerDirty));
			}
		}

		private void ControllerDirty()
		{
			if (!this.m_IsResampling)
			{
				this.m_MustResample = true;
			}
		}

		private void DisableIKOnFeetIfNeeded()
		{
			bool flag = false;
			if (this.m_SrcMotion == null || this.m_DstMotion == null)
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
			AnimatorControllerParameter[] parameters = this.m_Controller.parameters;
			if (parameters.Length > 0)
			{
				this.m_ShowBlendValue = EditorGUILayout.Foldout(this.m_ShowBlendValue, "BlendTree Parameters");
				if (this.m_ShowBlendValue)
				{
					for (int i = 0; i < parameters.Length; i++)
					{
						AnimatorControllerParameter animatorControllerParameter = this.m_Controller.parameters[i];
						float value = this.m_ParameterInfoList[i].m_Value;
						float num = EditorGUILayout.Slider(animatorControllerParameter.name, value, this.m_ParameterMinMax[i][0], this.m_ParameterMinMax[i][1], new GUILayoutOption[0]);
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
			if (!this.m_ValidTransition)
			{
				return;
			}
			float num = (this.m_LeftStateTimeB - this.m_LeftStateTimeA) / (this.m_LeftStateWeightB - this.m_LeftStateWeightA);
			float num2 = (this.m_RightStateTimeB - this.m_RightStateTimeA) / (this.m_RightStateWeightB - this.m_RightStateWeightA);
			float num3 = this.m_Transition.duration * ((!this.m_RefTransition.hasFixedDuration) ? num : 1f);
			this.m_Timeline.SrcStartTime = 0f;
			this.m_Timeline.SrcStopTime = num;
			this.m_Timeline.SrcName = this.m_RefSrcState.name;
			this.m_Timeline.HasExitTime = this.m_RefTransition.hasExitTime;
			this.m_Timeline.srcLoop = (this.m_SrcMotion && this.m_SrcMotion.isLooping);
			this.m_Timeline.dstLoop = (this.m_DstMotion && this.m_DstMotion.isLooping);
			this.m_Timeline.TransitionStartTime = this.m_RefTransition.exitTime * num;
			this.m_Timeline.TransitionStopTime = this.m_Timeline.TransitionStartTime + num3;
			this.m_Timeline.Time = this.m_AvatarPreview.timeControl.currentTime;
			this.m_Timeline.DstStartTime = this.m_Timeline.TransitionStartTime - this.m_RefTransition.offset * num2;
			this.m_Timeline.DstStopTime = this.m_Timeline.DstStartTime + num2;
			this.m_Timeline.SampleStopTime = this.m_AvatarPreview.timeControl.stopTime;
			if (this.m_Timeline.TransitionStopTime == float.PositiveInfinity)
			{
				this.m_Timeline.TransitionStopTime = Mathf.Min(this.m_Timeline.DstStopTime, this.m_Timeline.SrcStopTime);
			}
			this.m_Timeline.DstName = this.m_RefDstState.name;
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
					this.m_RefTransition.exitTime = this.m_Timeline.TransitionStartTime / this.m_Timeline.SrcDuration;
					this.m_RefTransition.duration = this.m_Timeline.TransitionDuration / ((!this.m_RefTransition.hasFixedDuration) ? this.m_Timeline.SrcDuration : 1f);
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
