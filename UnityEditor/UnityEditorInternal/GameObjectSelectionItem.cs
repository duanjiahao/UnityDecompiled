using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class GameObjectSelectionItem : AnimationWindowSelectionItem
	{
		public override AnimationClip animationClip
		{
			get
			{
				AnimationClip result;
				if (this.animationPlayer == null)
				{
					result = null;
				}
				else
				{
					result = base.animationClip;
				}
				return result;
			}
			set
			{
				base.animationClip = value;
			}
		}

		public static GameObjectSelectionItem Create(GameObject gameObject)
		{
			GameObjectSelectionItem selectionItem = ScriptableObject.CreateInstance(typeof(GameObjectSelectionItem)) as GameObjectSelectionItem;
			selectionItem.hideFlags = HideFlags.HideAndDontSave;
			selectionItem.gameObject = gameObject;
			selectionItem.animationClip = null;
			selectionItem.timeOffset = 0f;
			selectionItem.id = 0;
			if (selectionItem.rootGameObject != null)
			{
				AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(selectionItem.rootGameObject);
				if (selectionItem.animationClip == null && selectionItem.gameObject != null)
				{
					selectionItem.animationClip = ((animationClips.Length <= 0) ? null : animationClips[0]);
				}
				else if (!Array.Exists<AnimationClip>(animationClips, (AnimationClip x) => x == selectionItem.animationClip))
				{
					selectionItem.animationClip = ((animationClips.Length <= 0) ? null : animationClips[0]);
				}
			}
			return selectionItem;
		}

		public override void Synchronize()
		{
			if (this.rootGameObject != null)
			{
				AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.rootGameObject);
				if (animationClips.Length > 0)
				{
					if (!Array.Exists<AnimationClip>(animationClips, (AnimationClip x) => x == this.animationClip))
					{
						this.animationClip = animationClips[0];
					}
				}
				else
				{
					this.animationClip = null;
				}
			}
		}
	}
}
