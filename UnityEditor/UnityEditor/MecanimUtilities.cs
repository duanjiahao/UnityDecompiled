using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	internal class MecanimUtilities
	{
		public static bool HasChildMotion(Motion parent, Motion motion)
		{
			if (parent == motion)
			{
				return true;
			}
			if (parent is BlendTree)
			{
				BlendTree blendTree = parent as BlendTree;
				int childCount = blendTree.childCount;
				for (int i = 0; i < childCount; i++)
				{
					if (MecanimUtilities.HasChildMotion(blendTree.GetMotion(i), motion))
					{
						return true;
					}
				}
			}
			return false;
		}
		public static bool StateMachineRelativePath(StateMachine parent, StateMachine toFind, ref List<StateMachine> hierarchy)
		{
			hierarchy.Add(parent);
			if (parent == toFind)
			{
				return true;
			}
			for (int i = 0; i < parent.stateMachineCount; i++)
			{
				if (MecanimUtilities.StateMachineRelativePath(parent.GetStateMachine(i), toFind, ref hierarchy))
				{
					return true;
				}
			}
			hierarchy.Remove(parent);
			return false;
		}
		internal static bool AreSameAsset(UnityEngine.Object obj1, UnityEngine.Object obj2)
		{
			return AssetDatabase.GetAssetPath(obj1) == AssetDatabase.GetAssetPath(obj2);
		}
		internal static void DestroyStateMachineRecursive(StateMachine stateMachine)
		{
			for (int i = 0; i < stateMachine.stateMachineCount; i++)
			{
				StateMachine stateMachine2 = stateMachine.GetStateMachine(i);
				if (MecanimUtilities.AreSameAsset(stateMachine, stateMachine2))
				{
					MecanimUtilities.DestroyStateMachineRecursive(stateMachine2);
				}
			}
			for (int j = 0; j < stateMachine.stateCount; j++)
			{
				for (int k = 0; k < stateMachine.motionSetCount; k++)
				{
					BlendTree blendTree = stateMachine.GetState(j).GetMotionInternal(k) as BlendTree;
					if (blendTree != null && MecanimUtilities.AreSameAsset(stateMachine, blendTree))
					{
						MecanimUtilities.DestroyBlendTreeRecursive(blendTree);
					}
				}
			}
			Undo.DestroyObjectImmediate(stateMachine);
		}
		internal static void DestroyBlendTreeRecursive(BlendTree blendTree)
		{
			for (int i = 0; i < blendTree.childCount; i++)
			{
				BlendTree blendTree2 = blendTree.GetMotion(i) as BlendTree;
				if (blendTree2 != null && MecanimUtilities.AreSameAsset(blendTree, blendTree2))
				{
					MecanimUtilities.DestroyBlendTreeRecursive(blendTree2);
				}
			}
			Undo.DestroyObjectImmediate(blendTree);
		}
	}
}
