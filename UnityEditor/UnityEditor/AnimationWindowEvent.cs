using System;
using UnityEngine;

namespace UnityEditor
{
	internal class AnimationWindowEvent : ScriptableObject
	{
		public GameObject root;

		public AnimationClip clip;

		public AnimationClipInfoProperties clipInfo;

		public int eventIndex;

		public static AnimationWindowEvent CreateAndEdit(GameObject root, AnimationClip clip, float time)
		{
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.time = time;
			AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(clip);
			int num = AnimationWindowEvent.InsertAnimationEvent(ref animationEvents, clip, animationEvent);
			AnimationWindowEvent animationWindowEvent = ScriptableObject.CreateInstance<AnimationWindowEvent>();
			animationWindowEvent.hideFlags = HideFlags.HideInHierarchy;
			animationWindowEvent.name = "Animation Event";
			animationWindowEvent.root = root;
			animationWindowEvent.clip = clip;
			animationWindowEvent.clipInfo = null;
			animationWindowEvent.eventIndex = num;
			return animationWindowEvent;
		}

		public static AnimationWindowEvent Edit(GameObject root, AnimationClip clip, int eventIndex)
		{
			AnimationWindowEvent animationWindowEvent = ScriptableObject.CreateInstance<AnimationWindowEvent>();
			animationWindowEvent.hideFlags = HideFlags.HideInHierarchy;
			animationWindowEvent.name = "Animation Event";
			animationWindowEvent.root = root;
			animationWindowEvent.clip = clip;
			animationWindowEvent.clipInfo = null;
			animationWindowEvent.eventIndex = eventIndex;
			return animationWindowEvent;
		}

		public static AnimationWindowEvent Edit(AnimationClipInfoProperties clipInfo, int eventIndex)
		{
			AnimationWindowEvent animationWindowEvent = ScriptableObject.CreateInstance<AnimationWindowEvent>();
			animationWindowEvent.hideFlags = HideFlags.HideInHierarchy;
			animationWindowEvent.name = "Animation Event";
			animationWindowEvent.root = null;
			animationWindowEvent.clip = null;
			animationWindowEvent.clipInfo = clipInfo;
			animationWindowEvent.eventIndex = eventIndex;
			return animationWindowEvent;
		}

		private static int InsertAnimationEvent(ref AnimationEvent[] events, AnimationClip clip, AnimationEvent evt)
		{
			Undo.RegisterCompleteObjectUndo(clip, "Add Event");
			int num = events.Length;
			for (int i = 0; i < events.Length; i++)
			{
				if (events[i].time > evt.time)
				{
					num = i;
					break;
				}
			}
			ArrayUtility.Insert<AnimationEvent>(ref events, num, evt);
			AnimationUtility.SetAnimationEvents(clip, events);
			events = AnimationUtility.GetAnimationEvents(clip);
			if (events[num].time != evt.time || events[num].functionName != events[num].functionName)
			{
				Debug.LogError("Failed insertion");
			}
			return num;
		}
	}
}
