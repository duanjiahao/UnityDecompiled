using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	public struct AnimatorControllerPlayable
	{
		internal AnimationPlayable handle;

		internal Playable node
		{
			get
			{
				return this.handle.node;
			}
		}

		public RuntimeAnimatorController animatorController
		{
			get
			{
				return AnimatorControllerPlayable.GetAnimatorControllerInternal(ref this);
			}
		}

		public int layerCount
		{
			get
			{
				return AnimatorControllerPlayable.GetLayerCountInternal(ref this);
			}
		}

		public int parameterCount
		{
			get
			{
				return AnimatorControllerPlayable.GetParameterCountInternal(ref this);
			}
		}

		public PlayState state
		{
			get
			{
				return Playables.GetPlayStateValidated(this, base.GetType());
			}
			set
			{
				Playables.SetPlayStateValidated(this, value, base.GetType());
			}
		}

		public double time
		{
			get
			{
				return Playables.GetTimeValidated(this, base.GetType());
			}
			set
			{
				Playables.SetTimeValidated(this, value, base.GetType());
			}
		}

		public double duration
		{
			get
			{
				return Playables.GetDurationValidated(this, base.GetType());
			}
			set
			{
				Playables.SetDurationValidated(this, value, base.GetType());
			}
		}

		public static AnimatorControllerPlayable Create(RuntimeAnimatorController controller)
		{
			AnimatorControllerPlayable result = default(AnimatorControllerPlayable);
			AnimatorControllerPlayable.InternalCreate(controller, ref result);
			return result;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalCreate(RuntimeAnimatorController controller, ref AnimatorControllerPlayable that);

		public void Destroy()
		{
			this.node.Destroy();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RuntimeAnimatorController GetAnimatorControllerInternal(ref AnimatorControllerPlayable that);

		public float GetFloat(string name)
		{
			return AnimatorControllerPlayable.GetFloatString(ref this, name);
		}

		public float GetFloat(int id)
		{
			return AnimatorControllerPlayable.GetFloatID(ref this, id);
		}

		public void SetFloat(string name, float value)
		{
			AnimatorControllerPlayable.SetFloatString(ref this, name, value);
		}

		public void SetFloat(int id, float value)
		{
			AnimatorControllerPlayable.SetFloatID(ref this, id, value);
		}

		public bool GetBool(string name)
		{
			return AnimatorControllerPlayable.GetBoolString(ref this, name);
		}

		public bool GetBool(int id)
		{
			return AnimatorControllerPlayable.GetBoolID(ref this, id);
		}

		public void SetBool(string name, bool value)
		{
			AnimatorControllerPlayable.SetBoolString(ref this, name, value);
		}

		public void SetBool(int id, bool value)
		{
			AnimatorControllerPlayable.SetBoolID(ref this, id, value);
		}

		public int GetInteger(string name)
		{
			return AnimatorControllerPlayable.GetIntegerString(ref this, name);
		}

		public int GetInteger(int id)
		{
			return AnimatorControllerPlayable.GetIntegerID(ref this, id);
		}

		public void SetInteger(string name, int value)
		{
			AnimatorControllerPlayable.SetIntegerString(ref this, name, value);
		}

		public void SetInteger(int id, int value)
		{
			AnimatorControllerPlayable.SetIntegerID(ref this, id, value);
		}

		public void SetTrigger(string name)
		{
			AnimatorControllerPlayable.SetTriggerString(ref this, name);
		}

		public void SetTrigger(int id)
		{
			AnimatorControllerPlayable.SetTriggerID(ref this, id);
		}

		public void ResetTrigger(string name)
		{
			AnimatorControllerPlayable.ResetTriggerString(ref this, name);
		}

		public void ResetTrigger(int id)
		{
			AnimatorControllerPlayable.ResetTriggerID(ref this, id);
		}

		public bool IsParameterControlledByCurve(string name)
		{
			return AnimatorControllerPlayable.IsParameterControlledByCurveString(ref this, name);
		}

		public bool IsParameterControlledByCurve(int id)
		{
			return AnimatorControllerPlayable.IsParameterControlledByCurveID(ref this, id);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerCountInternal(ref AnimatorControllerPlayable that);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetLayerNameInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public string GetLayerName(int layerIndex)
		{
			return AnimatorControllerPlayable.GetLayerNameInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerIndexInternal(ref AnimatorControllerPlayable that, string layerName);

		public int GetLayerIndex(string layerName)
		{
			return AnimatorControllerPlayable.GetLayerIndexInternal(ref this, layerName);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetLayerWeightInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public float GetLayerWeight(int layerIndex)
		{
			return AnimatorControllerPlayable.GetLayerWeightInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerWeightInternal(ref AnimatorControllerPlayable that, int layerIndex, float weight);

		public void SetLayerWeight(int layerIndex, float weight)
		{
			AnimatorControllerPlayable.SetLayerWeightInternal(ref this, layerIndex, weight);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorStateInfo GetCurrentAnimatorStateInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetCurrentAnimatorStateInfoInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorStateInfo GetNextAnimatorStateInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetNextAnimatorStateInfoInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorTransitionInfo GetAnimatorTransitionInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorTransitionInfoInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorClipInfo[] GetCurrentAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetCurrentAnimatorClipInfoInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorClipInfo[] GetNextAnimatorClipInfoInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetNextAnimatorClipInfoInternal(ref this, layerIndex);
		}

		internal string ResolveHash(int hash)
		{
			return AnimatorControllerPlayable.ResolveHashInternal(ref this, hash);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ResolveHashInternal(ref AnimatorControllerPlayable that, int hash);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInTransitionInternal(ref AnimatorControllerPlayable that, int layerIndex);

		public bool IsInTransition(int layerIndex)
		{
			return AnimatorControllerPlayable.IsInTransitionInternal(ref this, layerIndex);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetParameterCountInternal(ref AnimatorControllerPlayable that);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorControllerParameter[] GetParametersArrayInternal(ref AnimatorControllerPlayable that);

		public AnimatorControllerParameter GetParameter(int index)
		{
			AnimatorControllerParameter[] parametersArrayInternal = AnimatorControllerPlayable.GetParametersArrayInternal(ref this);
			if (index < 0 && index >= parametersArrayInternal.Length)
			{
				throw new IndexOutOfRangeException("index");
			}
			return parametersArrayInternal[index];
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int StringToHash(string name);

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

		public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, fixedTime);
		}

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

		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this, stateNameHash, transitionDuration, layer, fixedTime);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime);

		[ExcludeFromDocs]
		private static void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, int layer)
		{
			float fixedTime = 0f;
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref that, stateNameHash, transitionDuration, layer, fixedTime);
		}

		[ExcludeFromDocs]
		private static void CrossFadeInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration)
		{
			float fixedTime = 0f;
			int layer = -1;
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref that, stateNameHash, transitionDuration, layer, fixedTime);
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
			AnimatorControllerPlayable.CrossFadeInternal(ref this, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, normalizedTime);
		}

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

		public void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this, stateNameHash, transitionDuration, layer, normalizedTime);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

		[ExcludeFromDocs]
		private static void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			AnimatorControllerPlayable.CrossFadeInternal(ref that, stateNameHash, transitionDuration, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		private static void CrossFadeInternal(ref AnimatorControllerPlayable that, int stateNameHash, float transitionDuration)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			AnimatorControllerPlayable.CrossFadeInternal(ref that, stateNameHash, transitionDuration, layer, normalizedTime);
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

		public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this, AnimatorControllerPlayable.StringToHash(stateName), layer, fixedTime);
		}

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

		public void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this, stateNameHash, layer, fixedTime);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);

		[ExcludeFromDocs]
		private static void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash, int layer)
		{
			float fixedTime = float.NegativeInfinity;
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref that, stateNameHash, layer, fixedTime);
		}

		[ExcludeFromDocs]
		private static void PlayInFixedTimeInternal(ref AnimatorControllerPlayable that, int stateNameHash)
		{
			float fixedTime = float.NegativeInfinity;
			int layer = -1;
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref that, stateNameHash, layer, fixedTime);
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
			this.PlayInternal(ref this, AnimatorControllerPlayable.StringToHash(stateName), layer, normalizedTime);
		}

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

		public void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			this.PlayInternal(ref this, stateNameHash, layer, normalizedTime);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

		[ExcludeFromDocs]
		private void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			this.PlayInternal(ref that, stateNameHash, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		private void PlayInternal(ref AnimatorControllerPlayable that, int stateNameHash)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			this.PlayInternal(ref that, stateNameHash, layer, normalizedTime);
		}

		public bool HasState(int layerIndex, int stateID)
		{
			return this.HasStateInternal(ref this, layerIndex, stateID);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool HasStateInternal(ref AnimatorControllerPlayable that, int layerIndex, int stateID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatString(ref AnimatorControllerPlayable that, string name, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatID(ref AnimatorControllerPlayable that, int id, float value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatString(ref AnimatorControllerPlayable that, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatID(ref AnimatorControllerPlayable that, int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolString(ref AnimatorControllerPlayable that, string name, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolID(ref AnimatorControllerPlayable that, int id, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolString(ref AnimatorControllerPlayable that, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolID(ref AnimatorControllerPlayable that, int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerString(ref AnimatorControllerPlayable that, string name, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerID(ref AnimatorControllerPlayable that, int id, int value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerString(ref AnimatorControllerPlayable that, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerID(ref AnimatorControllerPlayable that, int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerString(ref AnimatorControllerPlayable that, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerID(ref AnimatorControllerPlayable that, int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerString(ref AnimatorControllerPlayable that, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerID(ref AnimatorControllerPlayable that, int id);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveString(ref AnimatorControllerPlayable that, string name);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveID(ref AnimatorControllerPlayable that, int id);

		public bool IsValid()
		{
			return Playables.IsValid(this);
		}

		public T CastTo<T>() where T : struct
		{
			return this.handle.CastTo<T>();
		}

		public override bool Equals(object p)
		{
			return Playables.Equals(this, p);
		}

		public override int GetHashCode()
		{
			return this.node.GetHashCode();
		}

		public static implicit operator Playable(AnimatorControllerPlayable s)
		{
			return s.node;
		}

		public static implicit operator AnimationPlayable(AnimatorControllerPlayable s)
		{
			return s.handle;
		}

		public static bool operator ==(AnimatorControllerPlayable x, Playable y)
		{
			return Playables.Equals(x, y);
		}

		public static bool operator !=(AnimatorControllerPlayable x, Playable y)
		{
			return !Playables.Equals(x, y);
		}
	}
}
