using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationClipSelectionItem : AnimationWindowSelectionItem
	{
		public override bool canRecord
		{
			get
			{
				return false;
			}
		}

		public override bool canChangeAnimationClip
		{
			get
			{
				return false;
			}
		}

		public override bool canSyncSceneSelection
		{
			get
			{
				return false;
			}
		}

		public static AnimationClipSelectionItem Create(AnimationClip animationClip, UnityEngine.Object sourceObject)
		{
			AnimationClipSelectionItem animationClipSelectionItem = ScriptableObject.CreateInstance(typeof(AnimationClipSelectionItem)) as AnimationClipSelectionItem;
			animationClipSelectionItem.hideFlags = HideFlags.HideAndDontSave;
			animationClipSelectionItem.gameObject = (sourceObject as GameObject);
			animationClipSelectionItem.scriptableObject = (sourceObject as ScriptableObject);
			animationClipSelectionItem.animationClip = animationClip;
			animationClipSelectionItem.timeOffset = 0f;
			animationClipSelectionItem.id = 0;
			return animationClipSelectionItem;
		}
	}
}
