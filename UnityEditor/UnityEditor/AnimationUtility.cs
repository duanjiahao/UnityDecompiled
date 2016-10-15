using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public sealed class AnimationUtility
	{
		public enum CurveModifiedType
		{
			CurveDeleted,
			CurveModified,
			ClipModified
		}

		public enum TangentMode
		{
			Free,
			Auto,
			Linear,
			Constant
		}

		public delegate void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted);

		public static AnimationUtility.OnCurveWasModified onCurveWasModified;

		private static int kBrokenMask = 1;

		private static int kLeftTangentMask = 6;

		private static int kRightTangentMask = 24;

		[Obsolete("GetAnimationClips(Animation) is deprecated. Use GetAnimationClips(GameObject) instead.")]
		public static AnimationClip[] GetAnimationClips(Animation component)
		{
			return AnimationUtility.GetAnimationClips(component.gameObject);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationClip[] GetAnimationClips(GameObject gameObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimationClips(Animation animation, AnimationClip[] clips);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetAnimatableBindings(GameObject targetObject, GameObject root);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetFloatValue(GameObject root, EditorCurveBinding binding, out float data);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type GetEditorCurveValueType(GameObject root, EditorCurveBinding binding);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetObjectReferenceValue(GameObject root, EditorCurveBinding binding, out UnityEngine.Object targetObject);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern UnityEngine.Object GetAnimatedObject(GameObject root, EditorCurveBinding binding);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type PropertyModificationToEditorCurveBinding(PropertyModification modification, GameObject gameObject, out EditorCurveBinding binding);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetCurveBindings(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetObjectReferenceCurveBindings(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ObjectReferenceKeyframe[] GetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationCurve GetEditorCurve(AnimationClip clip, EditorCurveBinding binding);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve);

		[RequiredByNativeCode]
		private static void Internal_CallAnimationClipAwake(AnimationClip clip)
		{
			if (AnimationUtility.onCurveWasModified != null)
			{
				AnimationUtility.onCurveWasModified(clip, default(EditorCurveBinding), AnimationUtility.CurveModifiedType.ClipModified);
			}
		}

		public static void SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve)
		{
			AnimationUtility.Internal_SetEditorCurve(clip, binding, curve);
			if (AnimationUtility.onCurveWasModified != null)
			{
				AnimationUtility.onCurveWasModified(clip, binding, (curve == null) ? AnimationUtility.CurveModifiedType.CurveDeleted : AnimationUtility.CurveModifiedType.CurveModified);
			}
		}

		public static void SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
		{
			AnimationUtility.Internal_SetObjectReferenceCurve(clip, binding, keyframes);
			if (AnimationUtility.onCurveWasModified != null)
			{
				AnimationUtility.onCurveWasModified(clip, binding, (keyframes == null) ? AnimationUtility.CurveModifiedType.CurveDeleted : AnimationUtility.CurveModifiedType.CurveModified);
			}
		}

		private static float Internal_CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
		{
			float num = curve[index].time - curve[toIndex].time;
			if (Mathf.Approximately(num, 0f))
			{
				return 0f;
			}
			return (curve[index].value - curve[toIndex].value) / num;
		}

		private static AnimationUtility.TangentMode Internal_GetKeyLeftTangentMode(Keyframe key)
		{
			return (AnimationUtility.TangentMode)((key.tangentMode & AnimationUtility.kLeftTangentMask) >> 1);
		}

		private static AnimationUtility.TangentMode Internal_GetKeyRightTangentMode(Keyframe key)
		{
			return (AnimationUtility.TangentMode)((key.tangentMode & AnimationUtility.kRightTangentMask) >> 3);
		}

		private static bool Internal_GetKeyBroken(Keyframe key)
		{
			return (key.tangentMode & AnimationUtility.kBrokenMask) != 0;
		}

		private static void Internal_UpdateTangents(AnimationCurve curve, int index)
		{
			if (index < 0 || index >= curve.length)
			{
				throw new ArgumentException("Index out of bounds.");
			}
			Keyframe key = curve[index];
			if (AnimationUtility.Internal_GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Linear && index >= 1)
			{
				key.inTangent = AnimationUtility.Internal_CalculateLinearTangent(curve, index, index - 1);
				curve.MoveKey(index, key);
			}
			if (AnimationUtility.Internal_GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Linear && index + 1 < curve.length)
			{
				key.outTangent = AnimationUtility.Internal_CalculateLinearTangent(curve, index, index + 1);
				curve.MoveKey(index, key);
			}
			if (AnimationUtility.Internal_GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Auto || AnimationUtility.Internal_GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Auto)
			{
				curve.SmoothTangents(index, 0f);
			}
			if (AnimationUtility.Internal_GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Free && AnimationUtility.Internal_GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Free && !AnimationUtility.Internal_GetKeyBroken(key))
			{
				key.outTangent = key.inTangent;
				curve.MoveKey(index, key);
			}
			if (AnimationUtility.Internal_GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Constant)
			{
				key.inTangent = float.PositiveInfinity;
				curve.MoveKey(index, key);
			}
			if (AnimationUtility.Internal_GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Constant)
			{
				key.outTangent = float.PositiveInfinity;
				curve.MoveKey(index, key);
			}
		}

		public static void SetKeyBroken(AnimationCurve curve, int index, bool broken)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			if (index < 0 || index >= curve.length)
			{
				return;
			}
			Keyframe key = curve[index];
			if (broken)
			{
				key.tangentMode |= AnimationUtility.kBrokenMask;
			}
			else
			{
				key.tangentMode &= ~AnimationUtility.kBrokenMask;
			}
			curve.MoveKey(index, key);
			AnimationUtility.Internal_UpdateTangents(curve, index);
		}

		public static void SetKeyLeftTangentMode(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			if (index < 0 || index >= curve.length)
			{
				return;
			}
			Keyframe key = curve[index];
			if (tangentMode != AnimationUtility.TangentMode.Free)
			{
				key.tangentMode |= AnimationUtility.kBrokenMask;
			}
			key.tangentMode &= ~AnimationUtility.kLeftTangentMask;
			key.tangentMode |= (int)((int)tangentMode << 1);
			curve.MoveKey(index, key);
			AnimationUtility.Internal_UpdateTangents(curve, index);
		}

		public static void SetKeyRightTangentMode(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			if (index < 0 || index >= curve.length)
			{
				return;
			}
			Keyframe key = curve[index];
			if (tangentMode != AnimationUtility.TangentMode.Free)
			{
				key.tangentMode |= AnimationUtility.kBrokenMask;
			}
			key.tangentMode &= ~AnimationUtility.kRightTangentMask;
			key.tangentMode |= (int)((int)tangentMode << 3);
			curve.MoveKey(index, key);
			AnimationUtility.Internal_UpdateTangents(curve, index);
		}

		[Obsolete("GetAllCurves is deprecated. Use GetCurveBindings and GetObjectReferenceCurveBindings instead."), ExcludeFromDocs]
		public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip)
		{
			bool includeCurveData = true;
			return AnimationUtility.GetAllCurves(clip, includeCurveData);
		}

		[Obsolete("GetAllCurves is deprecated. Use GetCurveBindings and GetObjectReferenceCurveBindings instead.")]
		public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip, [DefaultValue("true")] bool includeCurveData)
		{
			EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
			AnimationClipCurveData[] array = new AnimationClipCurveData[curveBindings.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new AnimationClipCurveData(curveBindings[i]);
				if (includeCurveData)
				{
					array[i].curve = AnimationUtility.GetEditorCurve(clip, curveBindings[i]);
				}
			}
			return array;
		}

		[Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
		public static bool GetFloatValue(GameObject root, string relativePath, Type type, string propertyName, out float data)
		{
			return AnimationUtility.GetFloatValue(root, EditorCurveBinding.FloatCurve(relativePath, type, propertyName), out data);
		}

		[Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
		public static void SetEditorCurve(AnimationClip clip, string relativePath, Type type, string propertyName, AnimationCurve curve)
		{
			AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(relativePath, type, propertyName), curve);
		}

		[Obsolete("This overload is deprecated. Use the one with EditorCurveBinding instead.")]
		public static AnimationCurve GetEditorCurve(AnimationClip clip, string relativePath, Type type, string propertyName)
		{
			return AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(relativePath, type, propertyName));
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationEvent[] GetAnimationEvents(AnimationClip clip);

		public static void SetAnimationEvents(AnimationClip clip, AnimationEvent[] events)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("clip");
			}
			if (events == null)
			{
				throw new ArgumentNullException("events");
			}
			AnimationUtility.Internal_SetAnimationEvents(clip, events);
			if (AnimationUtility.onCurveWasModified != null)
			{
				AnimationUtility.onCurveWasModified(clip, default(EditorCurveBinding), AnimationUtility.CurveModifiedType.ClipModified);
			}
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetAnimationEvents(AnimationClip clip, AnimationEvent[] events);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CalculateTransformPath(Transform targetTransform, Transform root);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationClipSettings GetAnimationClipSettings(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings srcClipInfo);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAnimationClipSettingsNoDirty(AnimationClip clip, AnimationClipSettings srcClipInfo);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAdditiveReferencePose(AnimationClip clip, AnimationClip referenceClip, float time);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidPolynomialCurve(AnimationCurve curve);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ConstrainToPolynomialCurve(AnimationCurve curve);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CurveSupportsProcedural(AnimationCurve curve);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnimationClipStats GetAnimationClipStats(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetGenerateMotionCurves(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGenerateMotionCurves(AnimationClip clip, bool value);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasGenericRootTransform(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMotionFloatCurves(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMotionCurves(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasRootCurves(AnimationClip clip);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AmbiguousBinding(string path, int classID, Transform root);

		[Obsolete("Use AnimationMode.InAnimationMode instead")]
		public static bool InAnimationMode()
		{
			return AnimationMode.InAnimationMode();
		}

		[Obsolete("Use AnimationMode.StartAnimationmode instead")]
		public static void StartAnimationMode(UnityEngine.Object[] objects)
		{
			Debug.LogWarning("AnimationUtility.StartAnimationMode is deprecated. Use AnimationMode.StartAnimationMode with the new APIs. The objects passed to this function will no longer be reverted automatically. See AnimationMode.AddPropertyModification");
			AnimationMode.StartAnimationMode();
		}

		[Obsolete("Use AnimationMode.StopAnimationMode instead")]
		public static void StopAnimationMode()
		{
			AnimationMode.StopAnimationMode();
		}

		[Obsolete("SetAnimationType is no longer supported", true)]
		public static void SetAnimationType(AnimationClip clip, ModelImporterAnimationType type)
		{
		}
	}
}
