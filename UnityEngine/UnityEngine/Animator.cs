using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
namespace UnityEngine
{
	public sealed class Animator : Behaviour
	{
		public extern bool isOptimizable
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern bool isHuman
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern bool hasRootMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern bool isRootPositionOrRotationControlledByCurves
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float humanScale
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Vector3 deltaPosition
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Quaternion deltaRotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Vector3 velocity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Vector3 angularVelocity
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public Vector3 rootPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_rootPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_rootPosition(ref value);
			}
		}
		public Quaternion rootRotation
		{
			get
			{
				Quaternion result;
				this.INTERNAL_get_rootRotation(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_rootRotation(ref value);
			}
		}
		public extern bool applyRootMotion
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool linearVelocityBlending
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		[Obsolete("Use Animator.updateMode instead")]
		public bool animatePhysics
		{
			get
			{
				return this.updateMode == AnimatorUpdateMode.AnimatePhysics;
			}
			set
			{
				this.updateMode = ((!value) ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.AnimatePhysics);
			}
		}
		public extern AnimatorUpdateMode updateMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool hasTransformHierarchy
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern bool allowConstantClipSamplingOptimization
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float gravityWeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public Vector3 bodyPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_bodyPosition(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_bodyPosition(ref value);
			}
		}
		public Quaternion bodyRotation
		{
			get
			{
				Quaternion result;
				this.INTERNAL_get_bodyRotation(out result);
				return result;
			}
			set
			{
				this.INTERNAL_set_bodyRotation(ref value);
			}
		}
		public extern bool stabilizeFeet
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern int layerCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern AnimatorControllerParameter[] parameters
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float feetPivotActive
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float pivotWeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Vector3 pivotPosition
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern bool isMatchingTarget
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float speed
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern Vector3 targetPosition
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern Quaternion targetRotation
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern Transform avatarRoot
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern AnimatorCullingMode cullingMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float playbackTime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float recorderStartTime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float recorderStopTime
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern AnimatorRecorderMode recorderMode
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern Avatar avatar
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool layersAffectMassCenter
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern float leftFeetBottomHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern float rightFeetBottomHeight
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern bool supportsOnAnimatorMove
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		private extern bool isInManagerList
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern bool logWarnings
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern bool fireEvents
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public float GetFloat(string name)
		{
			return this.GetFloatString(name);
		}
		public float GetFloat(int id)
		{
			return this.GetFloatID(id);
		}
		public void SetFloat(string name, float value)
		{
			this.SetFloatString(name, value);
		}
		public void SetFloat(string name, float value, float dampTime, float deltaTime)
		{
			this.SetFloatStringDamp(name, value, dampTime, deltaTime);
		}
		public void SetFloat(int id, float value)
		{
			this.SetFloatID(id, value);
		}
		public void SetFloat(int id, float value, float dampTime, float deltaTime)
		{
			this.SetFloatIDDamp(id, value, dampTime, deltaTime);
		}
		public bool GetBool(string name)
		{
			return this.GetBoolString(name);
		}
		public bool GetBool(int id)
		{
			return this.GetBoolID(id);
		}
		public void SetBool(string name, bool value)
		{
			this.SetBoolString(name, value);
		}
		public void SetBool(int id, bool value)
		{
			this.SetBoolID(id, value);
		}
		public int GetInteger(string name)
		{
			return this.GetIntegerString(name);
		}
		public int GetInteger(int id)
		{
			return this.GetIntegerID(id);
		}
		public void SetInteger(string name, int value)
		{
			this.SetIntegerString(name, value);
		}
		public void SetInteger(int id, int value)
		{
			this.SetIntegerID(id, value);
		}
		public void SetTrigger(string name)
		{
			this.SetTriggerString(name);
		}
		public void SetTrigger(int id)
		{
			this.SetTriggerID(id);
		}
		public void ResetTrigger(string name)
		{
			this.ResetTriggerString(name);
		}
		public void ResetTrigger(int id)
		{
			this.ResetTriggerID(id);
		}
		public bool IsParameterControlledByCurve(string name)
		{
			return this.IsParameterControlledByCurveString(name);
		}
		public bool IsParameterControlledByCurve(int id)
		{
			return this.IsParameterControlledByCurveID(id);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rootPosition(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rootPosition(ref Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rootRotation(out Quaternion value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rootRotation(ref Quaternion value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bodyPosition(out Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_bodyPosition(ref Vector3 value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_bodyRotation(out Quaternion value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_bodyRotation(ref Quaternion value);
		public Vector3 GetIKPosition(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKPositionInternal(goal);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Vector3 GetIKPositionInternal(AvatarIKGoal goal);
		public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			this.CheckIfInIKPass();
			this.SetIKPositionInternal(goal, goalPosition);
		}
		internal void SetIKPositionInternal(AvatarIKGoal goal, Vector3 goalPosition)
		{
			Animator.INTERNAL_CALL_SetIKPositionInternal(this, goal, ref goalPosition);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIKPositionInternal(Animator self, AvatarIKGoal goal, ref Vector3 goalPosition);
		public Quaternion GetIKRotation(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKRotationInternal(goal);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Quaternion GetIKRotationInternal(AvatarIKGoal goal);
		public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			this.CheckIfInIKPass();
			this.SetIKRotationInternal(goal, goalRotation);
		}
		internal void SetIKRotationInternal(AvatarIKGoal goal, Quaternion goalRotation)
		{
			Animator.INTERNAL_CALL_SetIKRotationInternal(this, goal, ref goalRotation);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIKRotationInternal(Animator self, AvatarIKGoal goal, ref Quaternion goalRotation);
		public float GetIKPositionWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKPositionWeightInternal(goal);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetIKPositionWeightInternal(AvatarIKGoal goal);
		public void SetIKPositionWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetIKPositionWeightInternal(goal, value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIKPositionWeightInternal(AvatarIKGoal goal, float value);
		public float GetIKRotationWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKRotationWeightInternal(goal);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetIKRotationWeightInternal(AvatarIKGoal goal);
		public void SetIKRotationWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetIKRotationWeightInternal(goal, value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIKRotationWeightInternal(AvatarIKGoal goal, float value);
		public Vector3 GetIKHintPosition(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetIKHintPositionInternal(hint);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Vector3 GetIKHintPositionInternal(AvatarIKHint hint);
		public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
		{
			this.CheckIfInIKPass();
			this.SetIKHintPositionInternal(hint, hintPosition);
		}
		internal void SetIKHintPositionInternal(AvatarIKHint hint, Vector3 hintPosition)
		{
			Animator.INTERNAL_CALL_SetIKHintPositionInternal(this, hint, ref hintPosition);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIKHintPositionInternal(Animator self, AvatarIKHint hint, ref Vector3 hintPosition);
		public float GetIKHintPositionWeight(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetHintWeightPositionInternal(hint);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetHintWeightPositionInternal(AvatarIKHint hint);
		public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
		{
			this.CheckIfInIKPass();
			this.SetIKHintPositionWeightInternal(hint, value);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIKHintPositionWeightInternal(AvatarIKHint hint, float value);
		public void SetLookAtPosition(Vector3 lookAtPosition)
		{
			this.CheckIfInIKPass();
			this.SetLookAtPositionInternal(lookAtPosition);
		}
		internal void SetLookAtPositionInternal(Vector3 lookAtPosition)
		{
			Animator.INTERNAL_CALL_SetLookAtPositionInternal(this, ref lookAtPosition);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetLookAtPositionInternal(Animator self, ref Vector3 lookAtPosition);
		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
		{
			float clampWeight = 0.5f;
			this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight, float bodyWeight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			float headWeight = 1f;
			this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			float headWeight = 1f;
			float bodyWeight = 0f;
			this.SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		public void SetLookAtWeight(float weight, [DefaultValue("0.00f")] float bodyWeight, [DefaultValue("1.00f")] float headWeight, [DefaultValue("0.00f")] float eyesWeight, [DefaultValue("0.50f")] float clampWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLookAtWeightInternal(float weight, [DefaultValue("0.00f")] float bodyWeight, [DefaultValue("1.00f")] float headWeight, [DefaultValue("0.00f")] float eyesWeight, [DefaultValue("0.50f")] float clampWeight);
		[ExcludeFromDocs]
		internal void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight)
		{
			float clampWeight = 0.5f;
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[ExcludeFromDocs]
		internal void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[ExcludeFromDocs]
		internal void SetLookAtWeightInternal(float weight, float bodyWeight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			float headWeight = 1f;
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[ExcludeFromDocs]
		internal void SetLookAtWeightInternal(float weight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			float headWeight = 1f;
			float bodyWeight = 0f;
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ScriptableObject GetBehaviour(Type type);
		public T GetBehaviour<T>() where T : StateMachineBehaviour
		{
			return this.GetBehaviour(typeof(T)) as T;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ScriptableObject[] GetBehaviours(Type type);
		internal static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T : StateMachineBehaviour
		{
			if (rawObjects == null)
			{
				return null;
			}
			T[] array = new T[rawObjects.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (T)((object)rawObjects[i]);
			}
			return array;
		}
		public T[] GetBehaviours<T>() where T : StateMachineBehaviour
		{
			return Animator.ConvertStateMachineBehaviour<T>(this.GetBehaviours(typeof(T)));
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetLayerName(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLayerIndex(string layerName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetLayerWeight(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLayerWeight(int layerIndex, float weight);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);
		[Obsolete("GetCurrentAnimationClipState is obsolete. Use GetCurrentAnimatorClipInfo instead (UnityUpgradable).", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetCurrentAnimationClipState(int layerIndex);
		[Obsolete("GetNextAnimationClipState is obsolete. Use GetNextAnimatorClipInfo instead (UnityUpgradable).", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetNextAnimationClipState(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsInTransition(int layerIndex);
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime)
		{
			Animator.INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}
		[ExcludeFromDocs]
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
		{
			float targetNormalizedTime = 1f;
			Animator.INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MatchTarget(Animator self, ref Vector3 matchPosition, ref Quaternion matchRotation, AvatarTarget targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InterruptMatchTarget([DefaultValue("true")] bool completeMatch);
		[ExcludeFromDocs]
		public void InterruptMatchTarget()
		{
			bool completeMatch = true;
			this.InterruptMatchTarget(completeMatch);
		}
		[Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
		public void ForceStateNormalizedTime(float normalizedTime)
		{
			this.Play(0, 0, normalizedTime);
		}
		[ExcludeFromDocs]
		public void CrossFade(string stateName, float transitionDuration, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			this.CrossFade(stateName, transitionDuration, layer, normalizedTime);
		}
		[ExcludeFromDocs]
		public void CrossFade(string stateName, float transitionDuration)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			this.CrossFade(stateName, transitionDuration, layer, normalizedTime);
		}
		public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.CrossFade(Animator.StringToHash(stateName), transitionDuration, layer, normalizedTime);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
		[ExcludeFromDocs]
		public void CrossFade(int stateNameHash, float transitionDuration, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			this.CrossFade(stateNameHash, transitionDuration, layer, normalizedTime);
		}
		[ExcludeFromDocs]
		public void CrossFade(int stateNameHash, float transitionDuration)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			this.CrossFade(stateNameHash, transitionDuration, layer, normalizedTime);
		}
		[ExcludeFromDocs]
		public void Play(string stateName, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			this.Play(stateName, layer, normalizedTime);
		}
		[ExcludeFromDocs]
		public void Play(string stateName)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			this.Play(stateName, layer, normalizedTime);
		}
		public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.Play(Animator.StringToHash(stateName), layer, normalizedTime);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);
		[ExcludeFromDocs]
		public void Play(int stateNameHash, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			this.Play(stateNameHash, layer, normalizedTime);
		}
		[ExcludeFromDocs]
		public void Play(int stateNameHash)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			this.Play(stateNameHash, layer, normalizedTime);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);
		[Obsolete("use mask and layers to control subset of transfroms in a skeleton", true), WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsControlled(Transform transform);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsBoneTransform(Transform transform);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Transform GetBoneTransform(HumanBodyBones humanBoneId);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartPlayback();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopPlayback();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartRecording(int frameCount);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopRecording();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasState(int layerIndex, int stateID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToHash(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetStats();
		private void CheckIfInIKPass()
		{
			if (this.logWarnings && !this.CheckIfInIKPassInternal())
			{
				Debug.LogWarning("Setting and getting IK Goals should only be done in OnAnimatorIK or OnStateIK");
			}
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool CheckIfInIKPassInternal();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatString(string name, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatID(int id, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatString(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatID(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolString(string name, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolID(int id, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolString(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolID(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntegerString(string name, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntegerID(int id, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntegerString(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntegerID(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTriggerString(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTriggerID(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetTriggerString(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetTriggerID(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsParameterControlledByCurveString(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsParameterControlledByCurveID(int id);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void WriteDefaultPose();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(float deltaTime);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Rebind();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void EvaluateSM();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetCurrentStateName(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetNextStateName(int layerIndex);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string ResolveHash(int hash);
		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(string name)
		{
			return Vector3.zero;
		}
		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(int id)
		{
			return Vector3.zero;
		}
		[Obsolete("SetVector is deprecated.")]
		public void SetVector(string name, Vector3 value)
		{
		}
		[Obsolete("SetVector is deprecated.")]
		public void SetVector(int id, Vector3 value)
		{
		}
		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(string name)
		{
			return Quaternion.identity;
		}
		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(int id)
		{
			return Quaternion.identity;
		}
		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(string name, Quaternion value)
		{
		}
		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(int id, Quaternion value)
		{
		}
	}
}
