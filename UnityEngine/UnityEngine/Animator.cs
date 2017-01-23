using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Director;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public sealed class Animator : DirectorPlayer
	{
		public extern bool isOptimizable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isHuman
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasRootMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool isRootPositionOrRotationControlledByCurves
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float humanScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool isInitialized
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 deltaPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_deltaPosition(out result);
				return result;
			}
		}

		public Quaternion deltaRotation
		{
			get
			{
				Quaternion result;
				this.INTERNAL_get_deltaRotation(out result);
				return result;
			}
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_velocity(out result);
				return result;
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_angularVelocity(out result);
				return result;
			}
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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool linearVelocityBlending
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool hasTransformHierarchy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool allowConstantClipSamplingOptimization
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float gravityWeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 bodyPosition
		{
			get
			{
				this.CheckIfInIKPass();
				return this.GetBodyPositionInternal();
			}
			set
			{
				this.CheckIfInIKPass();
				this.SetBodyPositionInternal(value);
			}
		}

		public Quaternion bodyRotation
		{
			get
			{
				this.CheckIfInIKPass();
				return this.GetBodyRotationInternal();
			}
			set
			{
				this.CheckIfInIKPass();
				this.SetBodyRotationInternal(value);
			}
		}

		public extern bool stabilizeFeet
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int layerCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AnimatorControllerParameter[] parameters
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int parameterCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float feetPivotActive
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float pivotWeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector3 pivotPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_pivotPosition(out result);
				return result;
			}
		}

		public extern bool isMatchingTarget
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float speed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public Vector3 targetPosition
		{
			get
			{
				Vector3 result;
				this.INTERNAL_get_targetPosition(out result);
				return result;
			}
		}

		public Quaternion targetRotation
		{
			get
			{
				Quaternion result;
				this.INTERNAL_get_targetRotation(out result);
				return result;
			}
		}

		internal extern Transform avatarRoot
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AnimatorCullingMode cullingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float playbackTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float recorderStartTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float recorderStopTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern AnimatorRecorderMode recorderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Avatar avatar
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool layersAffectMassCenter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float leftFeetBottomHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern float rightFeetBottomHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool supportsOnAnimatorMove
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool logWarnings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool fireEvents
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_deltaPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_deltaRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_velocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_angularVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rootPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rootPosition(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_rootRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_set_rootRotation(ref Quaternion value);

		internal Vector3 GetBodyPositionInternal()
		{
			Vector3 result;
			Animator.INTERNAL_CALL_GetBodyPositionInternal(this, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetBodyPositionInternal(Animator self, out Vector3 value);

		internal void SetBodyPositionInternal(Vector3 bodyPosition)
		{
			Animator.INTERNAL_CALL_SetBodyPositionInternal(this, ref bodyPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetBodyPositionInternal(Animator self, ref Vector3 bodyPosition);

		internal Quaternion GetBodyRotationInternal()
		{
			Quaternion result;
			Animator.INTERNAL_CALL_GetBodyRotationInternal(this, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetBodyRotationInternal(Animator self, out Quaternion value);

		internal void SetBodyRotationInternal(Quaternion bodyRotation)
		{
			Animator.INTERNAL_CALL_SetBodyRotationInternal(this, ref bodyRotation);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetBodyRotationInternal(Animator self, ref Quaternion bodyRotation);

		public Vector3 GetIKPosition(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKPositionInternal(goal);
		}

		internal Vector3 GetIKPositionInternal(AvatarIKGoal goal)
		{
			Vector3 result;
			Animator.INTERNAL_CALL_GetIKPositionInternal(this, goal, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetIKPositionInternal(Animator self, AvatarIKGoal goal, out Vector3 value);

		public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			this.CheckIfInIKPass();
			this.SetIKPositionInternal(goal, goalPosition);
		}

		internal void SetIKPositionInternal(AvatarIKGoal goal, Vector3 goalPosition)
		{
			Animator.INTERNAL_CALL_SetIKPositionInternal(this, goal, ref goalPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIKPositionInternal(Animator self, AvatarIKGoal goal, ref Vector3 goalPosition);

		public Quaternion GetIKRotation(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKRotationInternal(goal);
		}

		internal Quaternion GetIKRotationInternal(AvatarIKGoal goal)
		{
			Quaternion result;
			Animator.INTERNAL_CALL_GetIKRotationInternal(this, goal, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetIKRotationInternal(Animator self, AvatarIKGoal goal, out Quaternion value);

		public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			this.CheckIfInIKPass();
			this.SetIKRotationInternal(goal, goalRotation);
		}

		internal void SetIKRotationInternal(AvatarIKGoal goal, Quaternion goalRotation)
		{
			Animator.INTERNAL_CALL_SetIKRotationInternal(this, goal, ref goalRotation);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIKRotationInternal(Animator self, AvatarIKGoal goal, ref Quaternion goalRotation);

		public float GetIKPositionWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKPositionWeightInternal(goal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetIKPositionWeightInternal(AvatarIKGoal goal);

		public void SetIKPositionWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetIKPositionWeightInternal(goal, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIKPositionWeightInternal(AvatarIKGoal goal, float value);

		public float GetIKRotationWeight(AvatarIKGoal goal)
		{
			this.CheckIfInIKPass();
			return this.GetIKRotationWeightInternal(goal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetIKRotationWeightInternal(AvatarIKGoal goal);

		public void SetIKRotationWeight(AvatarIKGoal goal, float value)
		{
			this.CheckIfInIKPass();
			this.SetIKRotationWeightInternal(goal, value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetIKRotationWeightInternal(AvatarIKGoal goal, float value);

		public Vector3 GetIKHintPosition(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetIKHintPositionInternal(hint);
		}

		internal Vector3 GetIKHintPositionInternal(AvatarIKHint hint)
		{
			Vector3 result;
			Animator.INTERNAL_CALL_GetIKHintPositionInternal(this, hint, out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetIKHintPositionInternal(Animator self, AvatarIKHint hint, out Vector3 value);

		public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
		{
			this.CheckIfInIKPass();
			this.SetIKHintPositionInternal(hint, hintPosition);
		}

		internal void SetIKHintPositionInternal(AvatarIKHint hint, Vector3 hintPosition)
		{
			Animator.INTERNAL_CALL_SetIKHintPositionInternal(this, hint, ref hintPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetIKHintPositionInternal(Animator self, AvatarIKHint hint, ref Vector3 hintPosition);

		public float GetIKHintPositionWeight(AvatarIKHint hint)
		{
			this.CheckIfInIKPass();
			return this.GetHintWeightPositionInternal(hint);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetHintWeightPositionInternal(AvatarIKHint hint);

		public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
		{
			this.CheckIfInIKPass();
			this.SetIKHintPositionWeightInternal(hint, value);
		}

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

		public void SetLookAtWeight(float weight, [UnityEngine.Internal.DefaultValue("0.00f")] float bodyWeight, [UnityEngine.Internal.DefaultValue("1.00f")] float headWeight, [UnityEngine.Internal.DefaultValue("0.00f")] float eyesWeight, [UnityEngine.Internal.DefaultValue("0.50f")] float clampWeight)
		{
			this.CheckIfInIKPass();
			this.SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLookAtWeightInternal(float weight, [UnityEngine.Internal.DefaultValue("0.00f")] float bodyWeight, [UnityEngine.Internal.DefaultValue("1.00f")] float headWeight, [UnityEngine.Internal.DefaultValue("0.00f")] float eyesWeight, [UnityEngine.Internal.DefaultValue("0.50f")] float clampWeight);

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

		public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
		{
			this.CheckIfInIKPass();
			this.SetBoneLocalRotationInternal(humanBoneId, rotation);
		}

		internal void SetBoneLocalRotationInternal(HumanBodyBones humanBoneId, Quaternion rotation)
		{
			Animator.INTERNAL_CALL_SetBoneLocalRotationInternal(this, humanBoneId, ref rotation);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetBoneLocalRotationInternal(Animator self, HumanBodyBones humanBoneId, ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ScriptableObject GetBehaviour(Type type);

		public T GetBehaviour<T>() where T : StateMachineBehaviour
		{
			return this.GetBehaviour(typeof(T)) as T;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern ScriptableObject[] GetBehaviours(Type type);

		internal static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T : StateMachineBehaviour
		{
			T[] result;
			if (rawObjects == null)
			{
				result = null;
			}
			else
			{
				T[] array = new T[rawObjects.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = (T)((object)rawObjects[i]);
				}
				result = array;
			}
			return result;
		}

		public T[] GetBehaviours<T>() where T : StateMachineBehaviour
		{
			return Animator.ConvertStateMachineBehaviour<T>(this.GetBehaviours(typeof(T)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetLayerName(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetLayerIndex(string layerName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetLayerWeight(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLayerWeight(int layerIndex, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetCurrentAnimatorClipInfoCount(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);

		public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			if (clips == null)
			{
				throw new ArgumentNullException("clips");
			}
			this.GetAnimatorClipInfoInternal(layerIndex, true, clips);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetAnimatorClipInfoInternal(int layerIndex, bool isCurrent, object clips);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetNextAnimatorClipInfoCount(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);

		public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			if (clips == null)
			{
				throw new ArgumentNullException("clips");
			}
			this.GetAnimatorClipInfoInternal(layerIndex, false, clips);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsInTransition(int layerIndex);

		public AnimatorControllerParameter GetParameter(int index)
		{
			AnimatorControllerParameter[] parameters = this.parameters;
			if (index < 0 && index >= this.parameters.Length)
			{
				throw new IndexOutOfRangeException("Index must be between 0 and " + this.parameters.Length);
			}
			return parameters[index];
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_pivotPosition(out Vector3 value);

		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [UnityEngine.Internal.DefaultValue("1")] float targetNormalizedTime)
		{
			Animator.INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}

		[ExcludeFromDocs]
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
		{
			float targetNormalizedTime = 1f;
			Animator.INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_MatchTarget(Animator self, ref Vector3 matchPosition, ref Quaternion matchRotation, AvatarTarget targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InterruptMatchTarget([UnityEngine.Internal.DefaultValue("true")] bool completeMatch);

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
		public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
		{
			float fixedTime = 0f;
			this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
		}

		[ExcludeFromDocs]
		public void CrossFadeInFixedTime(string stateName, float transitionDuration)
		{
			float fixedTime = 0f;
			int layer = -1;
			this.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);
		}

		public void CrossFadeInFixedTime(string stateName, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTime)
		{
			this.CrossFadeInFixedTime(Animator.StringToHash(stateName), transitionDuration, layer, fixedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTime);

		[ExcludeFromDocs]
		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
		{
			float fixedTime = 0f;
			this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
		}

		[ExcludeFromDocs]
		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
		{
			float fixedTime = 0f;
			int layer = -1;
			this.CrossFadeInFixedTime(stateNameHash, transitionDuration, layer, fixedTime);
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

		public void CrossFade(string stateName, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.CrossFade(Animator.StringToHash(stateName), transitionDuration, layer, normalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CrossFade(int stateNameHash, float transitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime);

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
		public void PlayInFixedTime(string stateName, int layer)
		{
			float fixedTime = float.NegativeInfinity;
			this.PlayInFixedTime(stateName, layer, fixedTime);
		}

		[ExcludeFromDocs]
		public void PlayInFixedTime(string stateName)
		{
			float fixedTime = float.NegativeInfinity;
			int layer = -1;
			this.PlayInFixedTime(stateName, layer, fixedTime);
		}

		public void PlayInFixedTime(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			this.PlayInFixedTime(Animator.StringToHash(stateName), layer, fixedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void PlayInFixedTime(int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime);

		[ExcludeFromDocs]
		public void PlayInFixedTime(int stateNameHash, int layer)
		{
			float fixedTime = float.NegativeInfinity;
			this.PlayInFixedTime(stateNameHash, layer, fixedTime);
		}

		[ExcludeFromDocs]
		public void PlayInFixedTime(int stateNameHash)
		{
			float fixedTime = float.NegativeInfinity;
			int layer = -1;
			this.PlayInFixedTime(stateNameHash, layer, fixedTime);
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

		public void Play(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.Play(Animator.StringToHash(stateName), layer, normalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Play(int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void INTERNAL_get_targetRotation(out Quaternion value);

		[Obsolete("use mask and layers to control subset of transfroms in a skeleton", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsControlled(Transform transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool IsBoneTransform(Transform transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Transform GetBoneTransform(HumanBodyBones humanBoneId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartPlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopPlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StartRecording(int frameCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopRecording();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasState(int layerIndex, int stateID);

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int StringToHash(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetStats();

		private void CheckIfInIKPass()
		{
			if (this.logWarnings && !this.CheckIfInIKPassInternal())
			{
				Debug.LogWarning("Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool CheckIfInIKPassInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatString(string name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatID(int id, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern float GetFloatID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolString(string name, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBoolID(int id, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetBoolID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntegerString(string name, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIntegerID(int id, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntegerString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetIntegerID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTriggerString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetTriggerID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetTriggerString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ResetTriggerID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsParameterControlledByCurveString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsParameterControlledByCurveID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void OnUpdateModeChanged();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void OnCullingModeChanged();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void WriteDefaultPose();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update(float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Rebind();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ApplyBuiltinRootMotion();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void EvaluateController();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetCurrentStateName(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetNextStateName(int layerIndex);

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

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("GetCurrentAnimationClipState is obsolete. Use GetCurrentAnimatorClipInfo instead (UnityUpgradable) -> GetCurrentAnimatorClipInfo(*)", true)]
		public AnimationInfo[] GetCurrentAnimationClipState(int layerIndex)
		{
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("GetNextAnimationClipState is obsolete. Use GetNextAnimatorClipInfo instead (UnityUpgradable) -> GetNextAnimatorClipInfo(*)", true)]
		public AnimationInfo[] GetNextAnimationClipState(int layerIndex)
		{
			return null;
		}
	}
}
