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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPropertyAnimated(UnityEngine.Object target, string propertyPath);

		private static void InternalAnimationModeChanged(bool newValue)
		{
			if (AnimationMode.animationModeChangedCallback != null)
			{
				AnimationMode.animationModeChangedCallback(newValue);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopAnimationMode();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool InAnimationMode();

		[WrapperlessIcall]
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

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginSampling();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void EndSampling();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SampleAnimationClip(GameObject gameObject, AnimationClip clip, float time);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddPropertyModification(EditorCurveBinding binding, PropertyModification modification, bool keepPrefabOverride);
	}
}
