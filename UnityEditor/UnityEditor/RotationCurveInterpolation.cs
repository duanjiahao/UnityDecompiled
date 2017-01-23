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
			RotationCurveInterpolation.Mode result;
			if (AnimationWindowUtility.IsTransformType(data.type) && data.propertyName.StartsWith("localEulerAngles"))
			{
				if (data.propertyName.StartsWith("localEulerAnglesBaked"))
				{
					result = RotationCurveInterpolation.Mode.Baked;
				}
				else if (data.propertyName.StartsWith("localEulerAnglesRaw"))
				{
					result = RotationCurveInterpolation.Mode.RawEuler;
				}
				else
				{
					result = RotationCurveInterpolation.Mode.NonBaked;
				}
			}
			else if (AnimationWindowUtility.IsTransformType(data.type) && data.propertyName.StartsWith("m_LocalRotation"))
			{
				result = RotationCurveInterpolation.Mode.RawQuaternions;
			}
			else
			{
				result = RotationCurveInterpolation.Mode.Undefined;
			}
			return result;
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
			string result;
			if (newInterpolationMode == RotationCurveInterpolation.Mode.Baked)
			{
				result = "localEulerAnglesBaked";
			}
			else if (newInterpolationMode == RotationCurveInterpolation.Mode.NonBaked)
			{
				result = "localEulerAngles";
			}
			else if (newInterpolationMode == RotationCurveInterpolation.Mode.RawEuler)
			{
				result = "localEulerAnglesRaw";
			}
			else if (newInterpolationMode == RotationCurveInterpolation.Mode.RawQuaternions)
			{
				result = "m_LocalRotation";
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static EditorCurveBinding[] ConvertRotationPropertiesToDefaultInterpolation(AnimationClip clip, EditorCurveBinding[] selection)
		{
			RotationCurveInterpolation.Mode newInterpolationMode = (!clip.legacy) ? RotationCurveInterpolation.Mode.RawEuler : RotationCurveInterpolation.Mode.Baked;
			return RotationCurveInterpolation.ConvertRotationPropertiesToInterpolationType(selection, newInterpolationMode);
		}

		internal static EditorCurveBinding[] ConvertRotationPropertiesToInterpolationType(EditorCurveBinding[] selection, RotationCurveInterpolation.Mode newInterpolationMode)
		{
			EditorCurveBinding[] result;
			if (selection.Length != 4)
			{
				result = selection;
			}
			else if (RotationCurveInterpolation.GetModeFromCurveData(selection[0]) == RotationCurveInterpolation.Mode.RawQuaternions)
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
				result = array;
			}
			else
			{
				result = selection;
			}
			return result;
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
			EditorCurveBinding[] result;
			if (!AnimationWindowUtility.IsTransformType(binding.type))
			{
				result = null;
			}
			else if (binding.propertyName.StartsWith("m_LocalPosition."))
			{
				if (binding.type == typeof(Transform))
				{
					result = RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "m_LocalPosition.", binding.type, 3);
				}
				else
				{
					result = null;
				}
			}
			else if (binding.propertyName.StartsWith("m_LocalScale."))
			{
				result = RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "m_LocalScale.", binding.type, 3);
			}
			else if (binding.propertyName.StartsWith("m_LocalRotation"))
			{
				result = RotationCurveInterpolation.SelectRotationBindingForAddKey(binding, clip);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static EditorCurveBinding[] RemapAnimationBindingForRotationAddKey(EditorCurveBinding binding, AnimationClip clip)
		{
			EditorCurveBinding[] result;
			if (!AnimationWindowUtility.IsTransformType(binding.type))
			{
				result = null;
			}
			else if (binding.propertyName.StartsWith("m_LocalRotation"))
			{
				result = RotationCurveInterpolation.SelectRotationBindingForAddKey(binding, clip);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private static EditorCurveBinding[] SelectRotationBindingForAddKey(EditorCurveBinding binding, AnimationClip clip)
		{
			EditorCurveBinding binding2 = binding;
			binding2.propertyName = "localEulerAnglesBaked.x";
			EditorCurveBinding[] result;
			if (AnimationUtility.GetEditorCurve(clip, binding2) != null)
			{
				result = RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesBaked.", binding.type, 3);
			}
			else
			{
				binding2.propertyName = "localEulerAngles.x";
				if (AnimationUtility.GetEditorCurve(clip, binding2) != null)
				{
					result = RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAngles.", binding.type, 3);
				}
				else
				{
					binding2.propertyName = "localEulerAnglesRaw.x";
					if (clip.legacy && AnimationUtility.GetEditorCurve(clip, binding2) == null)
					{
						result = RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesBaked.", binding.type, 3);
					}
					else
					{
						result = RotationCurveInterpolation.GenerateTransformCurveBindingArray(binding.path, "localEulerAnglesRaw.", binding.type, 3);
					}
				}
			}
			return result;
		}

		public static EditorCurveBinding RemapAnimationBindingForRotationCurves(EditorCurveBinding curveBinding, AnimationClip clip)
		{
			EditorCurveBinding result;
			if (!AnimationWindowUtility.IsTransformType(curveBinding.type))
			{
				result = curveBinding;
			}
			else
			{
				RotationCurveInterpolation.Mode modeFromCurveData = RotationCurveInterpolation.GetModeFromCurveData(curveBinding);
				if (modeFromCurveData != RotationCurveInterpolation.Mode.Undefined)
				{
					string str = curveBinding.propertyName.Split(new char[]
					{
						'.'
					})[1];
					EditorCurveBinding editorCurveBinding = curveBinding;
					if (modeFromCurveData != RotationCurveInterpolation.Mode.NonBaked)
					{
						editorCurveBinding.propertyName = RotationCurveInterpolation.GetPrefixForInterpolation(RotationCurveInterpolation.Mode.NonBaked) + "." + str;
						AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
						if (editorCurve != null)
						{
							result = editorCurveBinding;
							return result;
						}
					}
					if (modeFromCurveData != RotationCurveInterpolation.Mode.Baked)
					{
						editorCurveBinding.propertyName = RotationCurveInterpolation.GetPrefixForInterpolation(RotationCurveInterpolation.Mode.Baked) + "." + str;
						AnimationCurve editorCurve2 = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
						if (editorCurve2 != null)
						{
							result = editorCurveBinding;
							return result;
						}
					}
					if (modeFromCurveData != RotationCurveInterpolation.Mode.RawEuler)
					{
						editorCurveBinding.propertyName = RotationCurveInterpolation.GetPrefixForInterpolation(RotationCurveInterpolation.Mode.RawEuler) + "." + str;
						AnimationCurve editorCurve3 = AnimationUtility.GetEditorCurve(clip, editorCurveBinding);
						if (editorCurve3 != null)
						{
							result = editorCurveBinding;
							return result;
						}
					}
					result = curveBinding;
				}
				else
				{
					result = curveBinding;
				}
			}
			return result;
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
