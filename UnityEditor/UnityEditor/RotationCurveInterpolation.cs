using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	internal class RotationCurveInterpolation
	{
		public struct State
		{
			public bool allAreNonBaked;

			public bool allAreBaked;

			public bool allAreRaw;

			public bool allAreRotations;
		}

		public enum Mode
		{
			Baked,
			NonBaked,
			RawQuaternions,
			RawEuler,
			Undefined
		}

		public static char[] kPostFix = new char[]
		{
			'x',
			'y',
			'z',
			'w'
		};

		public static RotationCurveInterpolation.Mode GetModeFromCurveData(EditorCurveBinding data)
		{
			if (AnimationWindowUtility.IsTransformType(data.type) && data.propertyName.StartsWith("localEulerAngles"))
			{
				if (data.propertyName.StartsWith("localEulerAnglesBaked"))
				{
					return RotationCurveInterpolation.Mode.Baked;
				}
				if (data.propertyName.StartsWith("localEulerAnglesRaw"))
				{
					return RotationCurveInterpolation.Mode.RawEuler;
				}
				return RotationCurveInterpolation.Mode.NonBaked;
			}
			else
			{
				if (AnimationWindowUtility.IsTransformType(data.type) && data.propertyName.StartsWith("m_LocalRotation"))
				{
					return RotationCurveInterpolation.Mode.RawQuaternions;
				}
				return RotationCurveInterpolation.Mode.Undefined;
			}
		}

		public static RotationCurveInterpolation.State GetCurveState(AnimationClip clip, EditorCurveBinding[] selection)
		{
			RotationCurveInterpolation.State result;
			result.allAreRaw = true;
			result.allAreNonBaked = true;
			result.allAreBaked = true;
			result.allAreRotations = true;
			for (int i = 0; i < selection.Length; i++)
			{
				EditorCurveBinding data = selection[i];
				RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(data);
				result.allAreBaked &= (modeFromCurveData == RotationCurveInterpolation.Mode.Baked);
				result.allAreNonBaked &= (modeFromCurveData == RotationCurveInterpolation.Mode.NonBaked);
				result.allAreRaw &= (modeFromCurveData == RotationCurveInterpolation.Mode.RawEuler);
				result.allAreRotations &= (modeFromCurveData != RotationCurveInterpolation.Mode.Undefined);
			}
			return result;
		}

		public static int GetCurveIndexFromName(string name)
		{
			return (int)(RotationCurveInterpolation.ExtractComponentCharacter(name) - 'x');
		}

		public static char ExtractComponentCharacter(string name)
		{
			return name[name.Length - 1];
		}

		public static string GetPrefixForInterpolation(RotationCurveInterpolation.Mode newInterpolationMode)
		{
			if (newInterpolationMode == RotationCurveInterpolation.Mode.Baked)
			{
				return "localEulerAnglesBaked";
			}
			if (newInterpolationMode == RotationCurveInterpolation.Mode.NonBaked)
			{
				return "localEulerAngles";
			}
			if (newInterpolationMode == RotationCurveInterpolation.Mode.RawEuler)
			{
				return "localEulerAnglesRaw";
			}
			if (newInterpolationMode == RotationCurveInterpolation.Mode.RawQuaternions)
			{
				return "m_LocalRotation";
			}
			return null;
		}

		internal static EditorCurveBinding[] ConvertRotationPropertiesToDefaultInterpolation(AnimationClip clip, EditorCurveBinding[] selection)
		{
			RotationCurveInterpolation.Mode newInterpolationMode = (!clip.legacy) ? RotationCurveInterpolation.Mode.RawEuler : RotationCurveInterpolation.Mode.Baked;
			return RotationCurveInterpolation.ConvertRotationPropertiesToInterpolationType(selection, newInterpolationMode);
		}

		internal static EditorCurveBinding[] ConvertRotationPropertiesToInterpolationType(EditorCurveBinding[] selection, RotationCurveInterpolation.Mode newInterpolationMode)
		{
			if (selection.Length != 4)
			{
				return selection;
			}
			if (RotationCurveInterpolation.GetModeFromCurveData(selection[0]) == RotationCurveInterpolation.Mode.RawQuaternions)
			{
				EditorCurveBinding[] array = new EditorCurveBinding[]
				{
					selection[0],
					selection[1],
					selection[2]
				};
				string prefixForInterpolation = RotationCurveInterpolation.GetPrefixForInterpolation(newInterpolationMode);
				array[0].propertyName = prefixForInterpolation + ".x";
				array[1].propertyName = prefixForInterpolation + ".y";
				array[2].propertyName = prefixForInterpolation + ".z";
				return array;
			}
			return selection;
		}

		private static EditorCurveBinding[] GenerateTransformCurveBindingArray(string path, string property, Type type, int count)
		{
			EditorCurveBinding[] array = new EditorCurveBinding[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = EditorCurveBinding.FloatCurve(path, type, property + RotationCurveInterpolation.kPostFix[i]);
			}
			return array;
		}

		public static EditorCurveBinding[] RemapAnimationBindingForAddKey(EditorCurveBinding binding, AnimationClip clip)
		{
			if (!AnimationWindowUtility.IsTransformType(binding.type))
			{
				return null;
			}
			if (binding.propertyName.StartsWith("m_LocalPosition."))
			{
				if (binding.type == typeof(Transform))
				{
					return RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "m_LocalPosition.", binding.type, 3);
				}
				return null;
			}
			else
			{
				if (binding.propertyName.StartsWith("m_LocalScale."))
				{
					return RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "m_LocalScale.", binding.type, 3);
				}
				if (binding.propertyName.StartsWith("m_LocalRotation"))
				{
					return RotationCurveInterpolation.SelectRotationBindingForAddKey(binding, clip);
				}
				return null;
			}
		}

		public static EditorCurveBinding[] RemapAnimationBindingForRotationAddKey(EditorCurveBinding binding, AnimationClip clip)
		{
			if (!AnimationWindowUtility.IsTransformType(binding.type))
			{
				return null;
			}
			if (binding.propertyName.StartsWith("m_LocalRotation"))
			{
				return RotationCurveInterpolation.SelectRotationBindingForAddKey(binding, clip);
			}
			return null;
		}

		private static EditorCurveBinding[] SelectRotationBindingForAddKey(EditorCurveBinding binding, AnimationClip clip)
		{
			EditorCurveBinding binding2 = binding;
			binding2.propertyName = "localEulerAnglesBaked.x";
			if (AnimationUtility.GetEditorCurve(clip, binding2) != null)
			{
				return RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesBaked.", binding.type, 3);
			}
			binding2.propertyName = "localEulerAngles.x";
			if (AnimationUtility.GetEditorCurve(clip, binding2) != null)
			{
				return RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAngles.", binding.type, 3);
			}
			binding2.propertyName = "localEulerAnglesRaw.x";
			if (clip.legacy && AnimationUtility.GetEditorCurve(clip, binding2) == null)
			{
				return RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesBaked.", binding.type, 3);
			}
			return RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesRaw.", binding.type, 3);
		}

		public static EditorCurveBinding RemapAnimationBindingForRotationCurves(EditorCurveBinding curveBinding, AnimationClip clip)
		{
			if (!AnimationWindowUtility.IsTransformType(curveBinding.type))
			{
				return curveBinding;
			}
			if (!curveBinding.propertyName.StartsWith("m_LocalRotation"))
			{
				return curveBinding;
			}
			string str = curveBinding.propertyName.Split(new char[]
			{
				'.'
			})[1];
			EditorCurveBinding editorCurveBinding = curveBinding;
			editorCurveBinding.propertyName = "localEulerAngles." + str;
			AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
			if (editorCurve != null)
			{
				return editorCurveBinding;
			}
			editorCurveBinding.propertyName = "localEulerAnglesBaked." + str;
			editorCurve = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
			if (editorCurve != null)
			{
				return editorCurveBinding;
			}
			editorCurveBinding.propertyName = "localEulerAnglesRaw." + str;
			editorCurve = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
			if (editorCurve != null)
			{
				return editorCurveBinding;
			}
			return curveBinding;
		}

		internal static void SetInterpolation(AnimationClip clip, EditorCurveBinding[] curveBindings, RotationCurveInterpolation.Mode newInterpolationMode)
		{
			Undo.RegisterCompleteObjectUndo(clip, "Rotation Interpolation");
			if (clip.legacy && newInterpolationMode == RotationCurveInterpolation.Mode.RawEuler)
			{
				Debug.LogWarning("Warning, Euler Angles interpolation mode is not fully supported for Legacy animation clips. If you mix clips using Euler Angles interpolation with clips using other interpolation modes (using Animation.CrossFade, Animation.Blend or other methods), you will get erroneous results. Use with caution.", clip);
			}
			List<EditorCurveBinding> list = new List<EditorCurveBinding>();
			List<AnimationCurve> list2 = new List<AnimationCurve>();
			List<EditorCurveBinding> list3 = new List<EditorCurveBinding>();
			for (int i = 0; i < curveBindings.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = curveBindings[i];
				RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(editorCurveBinding);
				if (modeFromCurveData != RotationCurveInterpolation.Mode.Undefined)
				{
					if (modeFromCurveData == RotationCurveInterpolation.Mode.RawQuaternions)
					{
						Debug.LogWarning("Can't convert quaternion curve: " + editorCurveBinding.propertyName);
					}
					else
					{
						AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
						if (editorCurve != null)
						{
							string propertyName = RotationCurveInterpolation.GetPrefixForInterpolation(newInterpolationMode) + '.' + RotationCurveInterpolation.ExtractComponentCharacter(editorCurveBinding.propertyName);
							list.Add(new EditorCurveBinding
							{
								propertyName = propertyName,
								type = editorCurveBinding.type,
								path = editorCurveBinding.path
							});
							list2.Add(editorCurve);
							list3.Add(new EditorCurveBinding
							{
								propertyName = editorCurveBinding.propertyName,
								type = editorCurveBinding.type,
								path = editorCurveBinding.path
							});
						}
					}
				}
			}
			Undo.RegisterCompleteObjectUndo(clip, "Rotation Interpolation");
			foreach (EditorCurveBinding current in list3)
			{
				AnimationUtility.SetEditorCurve(clip, current, null);
			}
			foreach (EditorCurveBinding current2 in list)
			{
				AnimationUtility.SetEditorCurve(clip, current2, list2[list.IndexOf(current2)]);
			}
		}
	}
}
