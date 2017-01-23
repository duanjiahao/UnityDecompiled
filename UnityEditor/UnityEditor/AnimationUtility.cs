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

		public static AnimationUtility.OnCurveWasModified onCurveWasModified;

		private static int kBrokenMask = 1;

		private static int kLeftTangentMask = 30;

		private static int kRightTangentMask = 480;

		[Obsolete("GetAnimationClips(Animation) is deprecated. Use GetAnimationClips(GameObject) instead.")]
		public static AnimationClip[] GetAnimationClips(Animation component)
		{
			return AnimationUtility.GetAnimationClips(component.gameObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationClip[] GetAnimationClips(GameObject gameObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimationClips(Animation animation, AnimationClip[] clips);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetAnimatableBindings(GameObject targetObject, GameObject root);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern EditorCurveBinding[] GetScriptableObjectAnimatableBindings(ScriptableObject scriptableObject);

		public static bool GetFloatValue(GameObject root, EditorCurveBinding binding, out float data)
		{
			return AnimationUtility.INTERNAL_CALL_GetFloatValue(root, ref binding, out data);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetFloatValue(GameObject root, ref EditorCurveBinding binding, out float data);

		public static Type GetEditorCurveValueType(GameObject root, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetEditorCurveValueType(root, ref binding);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type INTERNAL_CALL_GetEditorCurveValueType(GameObject root, ref EditorCurveBinding binding);

		internal static Type GetScriptableObjectEditorCurveValueType(ScriptableObject scriptableObject, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetScriptableObjectEditorCurveValueType(scriptableObject, ref binding);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type INTERNAL_CALL_GetScriptableObjectEditorCurveValueType(ScriptableObject scriptableObject, ref EditorCurveBinding binding);

		public static bool GetObjectReferenceValue(GameObject root, EditorCurveBinding binding, out UnityEngine.Object targetObject)
		{
			return AnimationUtility.INTERNAL_CALL_GetObjectReferenceValue(root, ref binding, out targetObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_GetObjectReferenceValue(GameObject root, ref EditorCurveBinding binding, out UnityEngine.Object targetObject);

		public static UnityEngine.Object GetAnimatedObject(GameObject root, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetAnimatedObject(root, ref binding);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnityEngine.Object INTERNAL_CALL_GetAnimatedObject(GameObject root, ref EditorCurveBinding binding);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type PropertyModificationToEditorCurveBinding(PropertyModification modification, GameObject gameObject, out EditorCurveBinding binding);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetCurveBindings(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern EditorCurveBinding[] GetObjectReferenceCurveBindings(AnimationClip clip);

		public static ObjectReferenceKeyframe[] GetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetObjectReferenceCurve(clip, ref binding);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ObjectReferenceKeyframe[] INTERNAL_CALL_GetObjectReferenceCurve(AnimationClip clip, ref EditorCurveBinding binding);

		private static void Internal_SetObjectReferenceCurve(AnimationClip clip, EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes)
		{
			AnimationUtility.INTERNAL_CALL_Internal_SetObjectReferenceCurve(clip, ref binding, keyframes);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetObjectReferenceCurve(AnimationClip clip, ref EditorCurveBinding binding, ObjectReferenceKeyframe[] keyframes);

		public static AnimationCurve GetEditorCurve(AnimationClip clip, EditorCurveBinding binding)
		{
			return AnimationUtility.INTERNAL_CALL_GetEditorCurve(clip, ref binding);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationCurve INTERNAL_CALL_GetEditorCurve(AnimationClip clip, ref EditorCurveBinding binding);

		private static void Internal_SetEditorCurve(AnimationClip clip, EditorCurveBinding binding, AnimationCurve curve)
		{
			AnimationUtility.INTERNAL_CALL_Internal_SetEditorCurve(clip, ref binding, curve);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_SetEditorCurve(AnimationClip clip, ref EditorCurveBinding binding, AnimationCurve curve);

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
			float result;
			if (Mathf.Approximately(num, 0f))
			{
				result = 0f;
			}
			else
			{
				result = (curve[index].value - curve[toIndex].value) / num;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CalculateAutoTangent(AnimationCurve curve, int index);

		private static void Internal_UpdateTangents(AnimationCurve curve, int index)
		{
			if (index >= 0 && index < curve.length)
			{
				Keyframe key = curve[index];
				if (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Linear && index >= 1)
				{
					key.inTangent = AnimationUtility.Internal_CalculateLinearTangent(curve, index, index - 1);
					curve.MoveKey(index, key);
				}
				if (AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Linear && index + 1 < curve.length)
				{
					key.outTangent = AnimationUtility.Internal_CalculateLinearTangent(curve, index, index + 1);
					curve.MoveKey(index, key);
				}
				if (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.ClampedAuto || AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.ClampedAuto)
				{
					AnimationUtility.Internal_CalculateAutoTangent(curve, index);
				}
				if (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Auto || AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Auto)
				{
					curve.SmoothTangents(index, 0f);
				}
				if (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Free && AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Free && !AnimationUtility.GetKeyBroken(key))
				{
					key.outTangent = key.inTangent;
					curve.MoveKey(index, key);
				}
				if (AnimationUtility.GetKeyLeftTangentMode(key) == AnimationUtility.TangentMode.Constant)
				{
					key.inTangent = float.PositiveInfinity;
					curve.MoveKey(index, key);
				}
				if (AnimationUtility.GetKeyRightTangentMode(key) == AnimationUtility.TangentMode.Constant)
				{
					key.outTangent = float.PositiveInfinity;
					curve.MoveKey(index, key);
				}
			}
		}

		internal static void UpdateTangentsFromModeSurrounding(AnimationCurve curve, int index)
		{
			AnimationUtility.Internal_UpdateTangents(curve, index - 2);
			AnimationUtility.Internal_UpdateTangents(curve, index - 1);
			AnimationUtility.Internal_UpdateTangents(curve, index);
			AnimationUtility.Internal_UpdateTangents(curve, index + 1);
			AnimationUtility.Internal_UpdateTangents(curve, index + 2);
		}

		internal static void UpdateTangentsFromMode(AnimationCurve curve)
		{
			for (int i = 0; i < curve.length; i++)
			{
				AnimationUtility.Internal_UpdateTangents(curve, i);
			}
		}

		internal static AnimationUtility.TangentMode GetKeyLeftTangentMode(Keyframe key)
		{
			return (AnimationUtility.TangentMode)((key.tangentMode & AnimationUtility.kLeftTangentMask) >> 1);
		}

		internal static AnimationUtility.TangentMode GetKeyRightTangentMode(Keyframe key)
		{
			return (AnimationUtility.TangentMode)((key.tangentMode & AnimationUtility.kRightTangentMask) >> 5);
		}

		internal static bool GetKeyBroken(Keyframe key)
		{
			return (key.tangentMode & AnimationUtility.kBrokenMask) != 0;
		}

		internal static void SetKeyLeftTangentMode(ref Keyframe key, AnimationUtility.TangentMode tangentMode)
		{
			key.tangentMode &= ~AnimationUtility.kLeftTangentMask;
			key.tangentMode |= (int)((int)tangentMode << 1);
		}

		internal static void SetKeyRightTangentMode(ref Keyframe key, AnimationUtility.TangentMode tangentMode)
		{
			key.tangentMode &= ~AnimationUtility.kRightTangentMask;
			key.tangentMode |= (int)((int)tangentMode << 5);
		}

		internal static void SetKeyBroken(ref Keyframe key, bool broken)
		{
			if (broken)
			{
				key.tangentMode |= AnimationUtility.kBrokenMask;
			}
			else
			{
				key.tangentMode &= ~AnimationUtility.kBrokenMask;
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
				throw new ArgumentException("Index out of bounds.");
			}
			Keyframe key = curve[index];
			AnimationUtility.SetKeyBroken(ref key, broken);
			curve.MoveKey(index, key);
			AnimationUtility.UpdateTangentsFromModeSurrounding(curve, index);
		}

		public static void SetKeyLeftTangentMode(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			if (index < 0 || index >= curve.length)
			{
				throw new ArgumentException("Index out of bounds.");
			}
			Keyframe key = curve[index];
			if (tangentMode != AnimationUtility.TangentMode.Free)
			{
				AnimationUtility.SetKeyBroken(ref key, true);
			}
			AnimationUtility.SetKeyLeftTangentMode(ref key, tangentMode);
			curve.MoveKey(index, key);
			AnimationUtility.UpdateTangentsFromModeSurrounding(curve, index);
		}

		public static void SetKeyRightTangentMode(AnimationCurve curve, int index, AnimationUtility.TangentMode tangentMode)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			if (index < 0 || index >= curve.length)
			{
				throw new ArgumentException("Index out of bounds.");
			}
			Keyframe key = curve[index];
			if (tangentMode != AnimationUtility.TangentMode.Free)
			{
				AnimationUtility.SetKeyBroken(ref key, true);
			}
			AnimationUtility.SetKeyRightTangentMode(ref key, tangentMode);
			curve.MoveKey(index, key);
			AnimationUtility.UpdateTangentsFromModeSurrounding(curve, index);
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetAnimationEvents(AnimationClip clip, AnimationEvent[] events);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string CalculateTransformPath(Transform targetTransform, Transform root);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AnimationClipSettings GetAnimationClipSettings(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAnimationClipSettings(AnimationClip clip, AnimationClipSettings srcClipInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAnimationClipSettingsNoDirty(AnimationClip clip, AnimationClipSettings srcClipInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAdditiveReferencePose(AnimationClip clip, AnimationClip referenceClip, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidPolynomialCurve(AnimationCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ConstrainToPolynomialCurve(AnimationCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CurveSupportsProcedural(AnimationCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern AnimationClipStats GetAnimationClipStats(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetGenerateMotionCurves(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetGenerateMotionCurves(AnimationClip clip, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasGenericRootTransform(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMotionFloatCurves(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasMotionCurves(AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool HasRootCurves(AnimationClip clip);

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
