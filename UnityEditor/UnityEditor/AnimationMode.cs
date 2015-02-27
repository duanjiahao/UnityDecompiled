using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace UnityEditor
{
	public sealed class AnimationMode
	{
		private static Color s_AnimatedPropertyColorLight = new Color(1f, 0.65f, 0.6f, 1f);
		private static Color s_AnimatedPropertyColorDark = new Color(1f, 0.55f, 0.5f, 1f);
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
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StopAnimationMode();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool InAnimationMode();
		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void StartAnimationMode();
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
