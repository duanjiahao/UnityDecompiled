using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	public sealed class AnimatorController : RuntimeAnimatorController
	{
		internal delegate void RemovedParameter(int index);
		internal delegate void RemovedLayer(int index);
		private const string kControllerExtension = "controller";
		public Action OnAnimatorControllerDirty;
		internal AnimatorController.RemovedParameter onRemovedParameter;
		internal AnimatorController.RemovedLayer onRemovedLayer;
		public extern int layerCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public extern int parameterCount
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal extern bool isAssetBundled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		public AnimatorController()
		{
			AnimatorController.Internal_Create(this);
		}
		internal string GetDefaultBlendTreeParameter()
		{
			for (int i = 0; i < this.parameterCount; i++)
			{
				if (this.GetParameterType(i) == AnimatorControllerParameterType.Float)
				{
					return this.GetParameterName(i);
				}
			}
			return "Blend";
		}
		internal static void OnInvalidateAnimatorController(AnimatorController controller)
		{
			if (controller.OnAnimatorControllerDirty != null)
			{
				controller.OnAnimatorControllerDirty();
			}
		}
		public static AnimatorController CreateAnimatorControllerAtPath(string path)
		{
			AnimatorController animatorController = new AnimatorController();
			animatorController.name = Path.GetFileName(path);
			AssetDatabase.CreateAsset(animatorController, path);
			animatorController.AddLayer("Base Layer");
			return animatorController;
		}
		public static AnimationClip AllocateAnimatorClip(string name)
		{
			AnimationClip animationClip = AnimationSelection.AllocateAndSetupClip(true);
			animationClip.name = name;
			return animationClip;
		}
		public static AnimatorController CreateAnimatorControllerForClip(AnimationClip clip, GameObject animatedObject)
		{
			string text = AssetDatabase.GetAssetPath(clip);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			text = Path.Combine(FileUtil.DeleteLastPathNameComponent(text), animatedObject.name + ".controller");
			text = AssetDatabase.GenerateUniqueAssetPath(text);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return AnimatorController.CreateAnimatorControllerAtPathWithClip(text, clip);
		}
		public static State AddAnimationClipToController(AnimatorController controller, AnimationClip clip)
		{
			StateMachine layerStateMachine = controller.GetLayerStateMachine(0);
			Vector3 position = (layerStateMachine.stateCount <= 0) ? new Vector3(200f, 0f, 0f) : (layerStateMachine.GetState(layerStateMachine.stateCount - 1).position + new Vector3(35f, 65f));
			State state = layerStateMachine.AddState(clip.name);
			state.position = position;
			state.SetAnimationClip(clip);
			return state;
		}
		public static AnimatorController CreateAnimatorControllerAtPathWithClip(string path, AnimationClip clip)
		{
			AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(path);
			State state = animatorController.GetLayerStateMachine(0).AddState(clip.name);
			state.SetAnimationClip(clip);
			return animatorController;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorController mono);
		public AnimatorControllerLayer GetLayer(int index)
		{
			return (!this.ValidateLayerIndex(index)) ? null : new AnimatorControllerLayer(this, index);
		}
		public AnimatorControllerLayer AddLayer(string name)
		{
			this.AddLayerInternal(name);
			return this.GetLayer(this.layerCount - 1);
		}
		public void RemoveLayer(int index)
		{
			if (this.onRemovedLayer != null)
			{
				this.onRemovedLayer(index);
			}
			this.RemoveLayerInternal(index);
		}
		public AnimatorControllerParameter GetParameter(int index)
		{
			return (!this.ValidateParameterIndex(index)) ? null : new AnimatorControllerParameter(this, index);
		}
		public AnimatorControllerParameter AddParameter(string name, AnimatorControllerParameterType type)
		{
			this.AddParameterInternal(name, type);
			return this.GetParameter(this.parameterCount - 1);
		}
		public void RemoveParameter(int index)
		{
			if (this.onRemovedParameter != null)
			{
				this.onRemovedParameter(index);
			}
			this.RemoveParameterInternal(index);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int FindParameter(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimatorController GetEffectiveAnimatorController(Animator behavior);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimatorController(Animator behavior, AnimatorController controller);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityEngine.Object[] CollectObjectsUsingParameter(string parameterName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveLayerInternal(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RemoveParameterInternal(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddParameterInternal(string name, AnimatorControllerParameterType type);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddLayerInternal(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool ValidateLayerIndex(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool ValidateParameterIndex(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetLayerName(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerName(int index, string value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern StateMachine GetLayerStateMachine(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerStateMachine(int index, StateMachine value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AvatarMask GetLayerMask(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerMask(int index, AvatarMask value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AnimatorLayerBlendingMode GetLayerBlendingMode(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerBlendingMode(int index, AnimatorLayerBlendingMode value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetLayerSyncedIndex(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerSyncedIndex(int index, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetLayerMotionSetIndex(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetLayerIKPass(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerIKPass(int index, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetSyncedLayerAffectsTiming(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetSyncedLayerAffectsTiming(int index, bool value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetLayerDefaultWeight(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetLayerDefaultWeight(int index, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string GetParameterName(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameterName(int index, string value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern AnimatorControllerParameterType GetParameterType(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameterType(int index, AnimatorControllerParameterType value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetParameterDefaultFloat(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameterDefaultFloat(int index, float value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int GetParameterDefaultInt(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameterDefaultInt(int index, int value);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool GetParameterDefaultBool(int index);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetParameterDefaultBool(int index, bool value);
	}
}
