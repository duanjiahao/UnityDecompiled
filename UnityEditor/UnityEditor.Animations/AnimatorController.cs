using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngineInternal;
namespace UnityEditor.Animations
{
	public sealed class AnimatorController : RuntimeAnimatorController
	{
		private const string kControllerExtension = "controller";
		internal Action OnAnimatorControllerDirty;
		internal static AnimatorController lastActiveController;
		internal static int lastActiveLayerIndex;
		private PushUndoIfNeeded undoHandler = new PushUndoIfNeeded(true);
		public extern AnimatorControllerLayer[] layers
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		public extern AnimatorControllerParameter[] parameters
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
		internal extern bool isAssetBundled
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
		internal bool pushUndo
		{
			set
			{
				this.undoHandler.m_PushUndo = value;
			}
		}
		[Obsolete("parameterCount is obsolete. Use parameters.Length instead.", true)]
		private int parameterCount
		{
			get
			{
				return 0;
			}
		}
		[Obsolete("layerCount is obsolete. Use layers.Length instead.", true)]
		private int layerCount
		{
			get
			{
				return 0;
			}
		}
		public AnimatorController()
		{
			AnimatorController.Internal_Create(this);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create(AnimatorController mono);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnimatorController GetEffectiveAnimatorController(Animator behavior);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimatorController(Animator behavior, AnimatorController controller);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern int IndexOfParameter(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RenameParameter(string prevName, string newName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string MakeUniqueParameterName(string name);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string MakeUniqueLayerName(string name);
		public static StateMachineBehaviourContext[] FindStateMachineBehaviourContext(StateMachineBehaviour behaviour)
		{
			return AnimatorController.Internal_FindStateMachineBehaviourContext(behaviour);
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int CreateStateMachineBehaviour(MonoScript script);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CanAddStateMachineBehaviours();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ScriptableObject Internal_AddStateMachineBehaviourWithType(Type stateMachineBehaviourType, AnimatorState state, int layerIndex);
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public StateMachineBehaviour AddEffectiveStateMachineBehaviour(Type stateMachineBehaviourType, AnimatorState state, int layerIndex)
		{
			return (StateMachineBehaviour)this.Internal_AddStateMachineBehaviourWithType(stateMachineBehaviourType, state, layerIndex);
		}
		public T AddEffectiveStateMachineBehaviour<T>(AnimatorState state, int layerIndex) where T : StateMachineBehaviour
		{
			return this.AddEffectiveStateMachineBehaviour(typeof(T), state, layerIndex) as T;
		}
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityEngine.Object[] CollectObjectsUsingParameter(string parameterName);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddStateEffectiveBehaviour(AnimatorState state, int layerIndex, int instanceID);
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern StateMachineBehaviourContext[] Internal_FindStateMachineBehaviourContext(ScriptableObject scriptableObject);
		internal string GetDefaultBlendTreeParameter()
		{
			for (int i = 0; i < this.parameters.Length; i++)
			{
				if (this.parameters[i].type == AnimatorControllerParameterType.Float)
				{
					return this.parameters[i].name;
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
		internal AnimatorStateMachine FindEffectiveRootStateMachine(int layerIndex)
		{
			AnimatorControllerLayer animatorControllerLayer = this.layers[layerIndex];
			while (animatorControllerLayer.syncedLayerIndex != -1)
			{
				animatorControllerLayer = this.layers[animatorControllerLayer.syncedLayerIndex];
			}
			return animatorControllerLayer.stateMachine;
		}
		public void AddLayer(string name)
		{
			AnimatorControllerLayer animatorControllerLayer = new AnimatorControllerLayer();
			animatorControllerLayer.name = this.MakeUniqueLayerName(name);
			animatorControllerLayer.stateMachine = new AnimatorStateMachine();
			animatorControllerLayer.stateMachine.name = animatorControllerLayer.name;
			animatorControllerLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
			if (AssetDatabase.GetAssetPath(this) != string.Empty)
			{
				AssetDatabase.AddObjectToAsset(animatorControllerLayer.stateMachine, AssetDatabase.GetAssetPath(this));
			}
			this.AddLayer(animatorControllerLayer);
		}
		public void AddLayer(AnimatorControllerLayer layer)
		{
			this.undoHandler.DoUndo(this, "Layer added");
			AnimatorControllerLayer[] layers = this.layers;
			ArrayUtility.Add<AnimatorControllerLayer>(ref layers, layer);
			this.layers = layers;
		}
		public void RemoveLayer(int index)
		{
			this.undoHandler.DoUndo(this, "Layer removed");
			AnimatorControllerLayer[] layers = this.layers;
			if (layers[index].syncedLayerIndex == -1 && layers[index].stateMachine != null)
			{
				layers[index].stateMachine.Clear();
				if (MecanimUtilities.AreSameAsset(this, layers[index].stateMachine))
				{
					Undo.DestroyObjectImmediate(layers[index].stateMachine);
				}
			}
			AnimatorControllerLayer[] array = layers;
			for (int i = 0; i < array.Length; i++)
			{
				AnimatorControllerLayer animatorControllerLayer = array[i];
				if (animatorControllerLayer.syncedLayerIndex == index)
				{
					animatorControllerLayer.syncedLayerIndex = -1;
				}
			}
			ArrayUtility.Remove<AnimatorControllerLayer>(ref layers, layers[index]);
			this.layers = layers;
		}
		public void AddParameter(string name, AnimatorControllerParameterType type)
		{
			this.AddParameter(new AnimatorControllerParameter
			{
				name = this.MakeUniqueParameterName(name),
				type = type
			});
		}
		public void AddParameter(AnimatorControllerParameter paramater)
		{
			this.undoHandler.DoUndo(this, "Parameter added");
			AnimatorControllerParameter[] parameters = this.parameters;
			ArrayUtility.Add<AnimatorControllerParameter>(ref parameters, paramater);
			this.parameters = parameters;
		}
		public void RemoveParameter(int index)
		{
			this.undoHandler.DoUndo(this, "Parameter removed");
			AnimatorControllerParameter[] parameters = this.parameters;
			ArrayUtility.Remove<AnimatorControllerParameter>(ref parameters, parameters[index]);
			this.parameters = parameters;
		}
		public void RemoveParameter(AnimatorControllerParameter parameter)
		{
			this.undoHandler.DoUndo(this, "Parameter removed");
			AnimatorControllerParameter[] parameters = this.parameters;
			ArrayUtility.Remove<AnimatorControllerParameter>(ref parameters, parameter);
			this.parameters = parameters;
		}
		public AnimatorState AddMotion(Motion motion)
		{
			return this.AddMotion(motion, 0);
		}
		public AnimatorState AddMotion(Motion motion, int layerIndex)
		{
			AnimatorState animatorState = this.layers[layerIndex].stateMachine.AddState(motion.name);
			animatorState.motion = motion;
			return animatorState;
		}
		public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree)
		{
			return this.CreateBlendTreeInController(name, out tree, 0);
		}
		public AnimatorState CreateBlendTreeInController(string name, out BlendTree tree, int layerIndex)
		{
			tree = new BlendTree();
			tree.name = name;
			if (AssetDatabase.GetAssetPath(this) != string.Empty)
			{
				AssetDatabase.AddObjectToAsset(tree, AssetDatabase.GetAssetPath(this));
			}
			AnimatorState animatorState = this.layers[layerIndex].stateMachine.AddState(tree.name);
			animatorState.motion = tree;
			return animatorState;
		}
		public static AnimatorController CreateAnimatorControllerAtPath(string path)
		{
			AnimatorController animatorController = new AnimatorController();
			animatorController.name = Path.GetFileName(path);
			AssetDatabase.CreateAsset(animatorController, path);
			animatorController.pushUndo = false;
			animatorController.AddLayer("Base Layer");
			animatorController.pushUndo = true;
			return animatorController;
		}
		public static AnimationClip AllocateAnimatorClip(string name)
		{
			AnimationClip animationClip = AnimationSelection.AllocateAndSetupClip(true);
			animationClip.name = name;
			return animationClip;
		}
		internal static AnimatorController CreateAnimatorControllerForClip(AnimationClip clip, GameObject animatedObject)
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
		public static AnimatorController CreateAnimatorControllerAtPathWithClip(string path, AnimationClip clip)
		{
			AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(path);
			animatorController.AddMotion(clip);
			return animatorController;
		}
		public void SetStateEffectiveMotion(AnimatorState state, Motion motion)
		{
			this.SetStateEffectiveMotion(state, motion, 0);
		}
		public void SetStateEffectiveMotion(AnimatorState state, Motion motion, int layerIndex)
		{
			if (this.layers[layerIndex].syncedLayerIndex == -1)
			{
				this.undoHandler.DoUndo(state, "Set Motion");
				state.motion = motion;
			}
			else
			{
				this.undoHandler.DoUndo(this, "Set Motion");
				AnimatorControllerLayer[] layers = this.layers;
				layers[layerIndex].SetOverrideMotion(state, motion);
				this.layers = layers;
			}
		}
		public Motion GetStateEffectiveMotion(AnimatorState state)
		{
			return this.GetStateEffectiveMotion(state, 0);
		}
		public Motion GetStateEffectiveMotion(AnimatorState state, int layerIndex)
		{
			if (this.layers[layerIndex].syncedLayerIndex == -1)
			{
				return state.motion;
			}
			return this.layers[layerIndex].GetOverrideMotion(state);
		}
		public void SetStateEffectiveBehaviours(AnimatorState state, int layerIndex, StateMachineBehaviour[] behaviours)
		{
			if (this.layers[layerIndex].syncedLayerIndex == -1)
			{
				this.undoHandler.DoUndo(state, "Set Behaviours");
				state.behaviours = behaviours;
			}
			else
			{
				this.undoHandler.DoUndo(this, "Set Behaviours");
				AnimatorControllerLayer[] layers = this.layers;
				layers[layerIndex].SetOverrideBehaviours(state, behaviours);
				this.layers = layers;
			}
		}
		public StateMachineBehaviour[] GetStateEffectiveBehaviours(AnimatorState state, int layerIndex)
		{
			if (this.layers[layerIndex].syncedLayerIndex == -1)
			{
				return state.behaviours;
			}
			return this.layers[layerIndex].GetOverrideBehaviours(state);
		}
	}
}
