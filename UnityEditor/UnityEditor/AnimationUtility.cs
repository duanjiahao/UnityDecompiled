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

		public delegate void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted);

		public enum TangentMode
		{
			Free,
			Auto,
			Linear,
			Constant,
			ClampedAuto
		}

		internal enum PolynomialValid
		{
			Valid,
			InvalidPreWrapMode,
			InvalidPostWrapMode,
			TooManySegments
		}

		public static AnimationUtility.OnCurveWasModified onCurveWasModified;

		[Obsolete("GetAnimationClips(Animation) is deprecated. Use GetAnimationClips(GameObject) instead.")]
		public static AnimationClip[] GetAnimationClips(Animation component)
		{
			return AnimationUtility.GetAnimationClips(component.gameObject);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationClip[] GetAnimationClips(GameObject gameObject);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimationClips(Animation animation, AnimationClip[] clips);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetAnimatableBindings(GameObject targetObject, GameObject root);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern EditorCurveBinding[] GetScriptableObjectAnimatableBindings(ScriptableObject scriptableObject);

		public static bool GetFloatValue(GameObject root, EditorCurveBinding binding, out float data)
		{
			return AnimationUtility.INTERNAL_CALL_GetFloatValue(root, ref binding, out data);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetFloatValue(GameObject root, ref EditorCurveBinding binding, out float data);

		public static Type GetEditorCurveValueType(GameObject root, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetEditorCurveValueType(root, ref binding);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type INTERNAL_CALL_GetEditorCurveValueType(GameObject root, ref EditorCurveBinding binding);

		internal static Type GetScriptableObjectEditorCurveValueType(ScriptableObject scriptableObject, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetScriptableObjectEditorCurveValueType(scriptableObject, ref binding);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type INTERNAL_CALL_GetScriptableObjectEditorCurveValueType(ScriptableObject scriptableObject, ref EditorCurveBinding binding);

		public static bool GetObjectReferenceValue(GameObject root, EditorCurveBinding binding, out UnityEngine.Object targetObject)
		{
			return AnimationUtility.INTERNAL_CALL_GetObjectReferenceValue(root, ref binding, out targetObject);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetObjectReferenceValue(GameObject root, ref EditorCurveBinding binding, out UnityEngine.Object targetObject);

		public static UnityEngine.Object GetAnimatedObject(GameObject root, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetAnimatedObject(root, ref binding);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_GetAnimatedObject(GameObject root, ref EditorCurveBinding binding);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type PropertyModificationToEditorCurveBinding(PropertyModification modification, GameObject gameObject, out EditorCurveBinding binding);

		internal static PropertyModification EditorCurveBindingToPropertyModification(EditorCurveBinding binding, GameObject gameObject)
		{
			return AnimationUtility.INTERNAL_CALL_EditorCurveBindingToPropertyModification(ref binding, gameObject);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PropertyModification INTERNAL_CALL_EditorCurveBindingToPropertyModification(ref EditorCurveBinding binding, GameObject gameObject);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetCurveBindings(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetObjectReferenceCurveBindings(AnimationClip clip);

		public static ObjectReferenceKeyframe[] GetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetObjectReferenceCurve(clip, ref binding);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ObjectReferenceKeyframe[] INTERNAL_CALL_GetObjectReferenceCurve(AnimationClip clip, ref EditorCurveBinding binding);

		private static void Internal_SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
		{
			AnimationUtility.INTERNAL_CALL_Internal_SetObjectReferenceCurve(clip, ref binding, keyframes);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetObjectReferenceCurve(AnimationClip clip, ref EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes);

		public static AnimationCurve GetEditorCurve(AnimationClip clip, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetEditorCurve(clip, ref binding);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationCurve INTERNAL_CALL_GetEditorCurve(AnimationClip clip, ref EditorCurveBinding binding);

		private static void Internal_SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve, bool syncEditorCurve)
		{
			AnimationUtility.INTERNAL_CALL_Internal_SetEditorCurve(clip, ref binding, curve, syncEditorCurve);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetEditorCurve(AnimationClip clip, ref EditorCurveBinding binding, AnimationCurve curve, bool syncEditorCurve);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SyncEditorCurves(AnimationClip clip);

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
			AnimationUtility.Internal_SetEditorCurve(clip, binding, curve, true);
			if (AnimationUtility.onCurveWasModified != null)
			{
				AnimationUtility.onCurveWasModified(clip, binding, (curve == null) ? AnimationUtility.CurveModifiedType.CurveDeleted : AnimationUtility.CurveModifiedType.CurveModified);
			}
		}

		internal static void SetEditorCurves(AnimationClip clip, EditorCurveBinding[] bindings, AnimationCurve[] curves)
		{
			if (clip == null)
			{
				throw new ArgumentNullException("clip");
			}
			if (curves == null)
			{
				throw new ArgumentNullException("curves");
			}
			if (bindings == null)
			{
				throw new ArgumentNullException("bindings");
			}
			if (bindings.Length != curves.Length)
			{
				throw new ArgumentException("bindings and curves array sizes do not match");
			}
			for (int i = 0; i < bindings.Length; i++)
			{
				AnimationUtility.Internal_SetEditorCurve(clip, bindings[i], curves[i], false);
				if (AnimationUtility.onCurveWasModified != null)
				{
					AnimationUtility.onCurveWasModified(clip, bindings[i], (curves[i] == null) ? AnimationUtility.CurveModifiedType.CurveDeleted : AnimationUtility.CurveModifiedType.CurveModified);
				}
			}
			AnimationUtility.Internal_SyncEditorCurves(clip);
		}

		public static void SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
		{
			AnimationUtility.Internal_SetObjectReferenceCurve(clip, binding, keyframes);
			if (AnimationUtility.onCurveWasModified != null)
			{
				AnimationUtility.onCurveWasModified(clip, binding, (keyframes == null) ? AnimationUtility.CurveModifiedType.CurveDeleted : AnimationUtility.CurveModifiedType.CurveModified);
			}
		}

		private static void VerifyCurve(AnimationCurve curve)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
		}

		private static void VerifyCurveAndKeyframeIndex(AnimationCurve curve, int index)
		{
			AnimationUtility.VerifyCurve(curve);
			if (index < 0 || index >= curve.length)
			{
				string message = string.Format("index {0} must be in the range of 0 to {1}.", index, curve.length - 1);
				throw new ArgumentOutOfRangeException("index", message);
			}
		}

		internal static void UpdateTangentsFromModeSurrounding(AnimationCurve curve, int index)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			AnimationUtility.UpdateTangentsFromModeSurroundingInternal(curve, index);
		}

		internal static void UpdateTangentsFromMode(AnimationCurve curve)
		{
			AnimationUtility.VerifyCurve(curve);
			AnimationUtility.UpdateTangentsFromModeInternal(curve);
		}

		public static AnimationUtility.TangentMode GetKeyLeftTangentMode(AnimationCurve curve, int index)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			return AnimationUtility.GetKeyLeftTangentModeInternal(curve, index);
		}

		public static AnimationUtility.TangentMode GetKeyRightTangentMode(AnimationCurve curve, int index)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			return AnimationUtility.GetKeyRightTangentModeInternal(curve, index);
		}

		internal static bool GetKeyBroken(Keyframe key)
		{
			return AnimationUtility.INTERNAL_CALL_GetKeyBroken(ref key);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetKeyBroken(ref Keyframe key);

		internal static void SetKeyLeftTangentMode(ref Keyframe key, AnimationUtility.TangentMode tangentMode)
		{
			AnimationUtility.INTERNAL_CALL_SetKeyLeftTangentMode(ref key, tangentMode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetKeyLeftTangentMode(ref Keyframe key, AnimationUtility.TangentMode tangentMode);

		internal static void SetKeyRightTangentMode(ref Keyframe key, AnimationUtility.TangentMode tangentMode)
		{
			AnimationUtility.INTERNAL_CALL_SetKeyRightTangentMode(ref key, tangentMode);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetKeyRightTangentMode(ref Keyframe key, AnimationUtility.TangentMode tangentMode);

		internal static void SetKeyBroken(ref Keyframe key, bool broken)
		{
			AnimationUtility.INTERNAL_CALL_SetKeyBroken(ref key, broken);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_SetKeyBroken(ref Keyframe key, bool broken);

		public static void SetKeyBroken(AnimationCurve curve, int index, bool broken)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			AnimationUtility.SetKeyBrokenInternal(curve, index, broken);
		}

		public static void SetKeyLeftTangentMode(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			AnimationUtility.SetKeyLeftTangentModeInternal(curve, index, tangentMode);
		}

		public static void SetKeyRightTangentMode(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			AnimationUtility.SetKeyRightTangentModeInternal(curve, index, tangentMode);
		}

		public static bool GetKeyBroken(AnimationCurve curve, int index)
		{
			AnimationUtility.VerifyCurveAndKeyframeIndex(curve, index);
			return AnimationUtility.GetKeyBrokenInternal(curve, index);
		}

		internal static AnimationUtility.TangentMode GetKeyLeftTangentMode(Keyframe key)
		{
			return AnimationUtility.INTERNAL_CALL_GetKeyLeftTangentMode(ref key);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationUtility.TangentMode INTERNAL_CALL_GetKeyLeftTangentMode(ref Keyframe key);

		internal static AnimationUtility.TangentMode GetKeyRightTangentMode(Keyframe key)
		{
			return AnimationUtility.INTERNAL_CALL_GetKeyRightTangentMode(ref key);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationUtility.TangentMode INTERNAL_CALL_GetKeyRightTangentMode(ref Keyframe key);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateTangentsFromModeSurroundingInternal(AnimationCurve curve, int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateTangentsFromModeInternal(AnimationCurve curve);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationUtility.TangentMode GetKeyLeftTangentModeInternal(AnimationCurve curve, int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationUtility.TangentMode GetKeyRightTangentModeInternal(AnimationCurve curve, int index);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeyBrokenInternal(AnimationCurve curve, int index, bool broken);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeyLeftTangentModeInternal(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeyRightTangentModeInternal(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetKeyBrokenInternal(AnimationCurve curve, int index);

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

		[GeneratedByOldBindingsGenerator]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetAnimationEvents(AnimationClip clip, AnimationEvent[] events);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CalculateTransformPath(Transform targetTransform, Transform root);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationClipSettings GetAnimationClipSettings(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings srcClipInfo);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAnimationClipSettingsNoDirty(AnimationClip clip, AnimationClipSettings srcClipInfo);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAdditiveReferencePose(AnimationClip clip, AnimationClip referenceClip, float time);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsValidOptimizedPolynomialCurve(AnimationCurve curve);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ConstrainToPolynomialCurve(AnimationCurve curve);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMaxNumPolynomialSegmentsSupported();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnimationUtility.PolynomialValid IsValidPolynomialCurve(AnimationCurve curve);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnimationClipStats GetAnimationClipStats(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetGenerateMotionCurves(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGenerateMotionCurves(AnimationClip clip, bool value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasGenericRootTransform(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMotionFloatCurves(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMotionCurves(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasRootCurves(AnimationClip clip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AmbiguousBinding(string path, int classID, Transform root);

		internal static Vector3 GetClosestEuler(Quaternion q, Vector3 eulerHint, RotationOrder rotationOrder)
		{
			Vector3 result;
			AnimationUtility.INTERNAL_CALL_GetClosestEuler(ref q, ref eulerHint, rotationOrder, out result);
			return result;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetClosestEuler(ref Quaternion q, ref Vector3 eulerHint, RotationOrder rotationOrder, out Vector3 value);

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
