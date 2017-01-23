using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class AnimationMode
	{
		internal delegate void AnimationModeChangedCallback(bool newValue);

		private static bool s_InAnimationPlaybackMode = false;

		private static Color s_AnimatedPropertyColorLight = new Color(1f, 0.65f, 0.6f, 1f);

		private static Color s_AnimatedPropertyColorDark = new Color(1f, 0.55f, 0.5f, 1f);

		internal static AnimationMode.AnimationModeChangedCallback animationModeChangedCallback;

		public static Color animatedPropertyColor
		{
			get
			{
				return (!EditorGUIUtility.isProSkin) ? AnimationMode.s_AnimatedPropertyColorLight : AnimationMode.s_AnimatedPropertyColorDark;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPropertyAnimated(UnityEngine.Object target, string propertyPath);

		private static void InternalAnimationModeChanged(bool newValue)
		{
			if (AnimationMode.animationModeChangedCallback != null)
			{
				AnimationMode.animationModeChangedCallback(newValue);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopAnimationMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool InAnimationMode();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartAnimationMode();

		internal static void StopAnimationPlaybackMode()
		{
			AnimationMode.s_InAnimationPlaybackMode = false;
		}

		internal static bool InAnimationPlaybackMode()
		{
			return AnimationMode.s_InAnimationPlaybackMode;
		}

		internal static void StartAnimationPlaybackMode()
		{
			AnimationMode.s_InAnimationPlaybackMode = true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSampling();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSampling();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time);

		public static void AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride)
		{
			AnimationMode.INTERNAL_CALL_AddPropertyModification(ref binding, modification, keepPrefabOverride);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_AddPropertyModification(ref EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);
	}
}
