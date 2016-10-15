using System;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class StandaloneSelectionItem : AnimationWindowSelectionItem
	{
		public override AnimationClip animationClip
		{
			get
			{
				if (this.animationPlayer == null)
				{
					return null;
				}
				return base.animationClip;
			}
			set
			{
				base.animationClip = value;
			}
		}

		public new static StandaloneSelectionItem Create()
		{
			StandaloneSelectionItem standaloneSelectionItem = ScriptableObject.CreateInstance(typeof(StandaloneSelectionItem)) as StandaloneSelectionItem;
			standaloneSelectionItem.hideFlags = HideFlags.HideAndDontSave;
			return standaloneSelectionItem;
		}
	}
}
