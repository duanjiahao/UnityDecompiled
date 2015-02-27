using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal class AnimationRecording
	{
		private static bool HasAnyRecordableModifications(GameObject root, UndoPropertyModification[] modifications)
		{
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding editorCurveBinding;
				if (AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].propertyModification, root, out editorCurveBinding) != null)
				{
					return true;
				}
			}
			return false;
		}
		private static PropertyModification FindPropertyModification(GameObject root, UndoPropertyModification[] modifications, EditorCurveBinding binding)
		{
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding lhs;
				AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].propertyModification, root, out lhs);
				if (lhs == binding)
				{
					return modifications[i].propertyModification;
				}
			}
			return null;
		}
		public static UndoPropertyModification[] Process(AnimationWindowState state, UndoPropertyModification[] modifications)
		{
			GameObject rootGameObject = state.m_RootGameObject;
			AnimationClip activeAnimationClip = state.m_ActiveAnimationClip;
			Animator component = rootGameObject.GetComponent<Animator>();
			if (!AnimationRecording.HasAnyRecordableModifications(rootGameObject, modifications))
			{
				return modifications;
			}
			List<UndoPropertyModification> list = new List<UndoPropertyModification>();
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding binding = default(EditorCurveBinding);
				PropertyModification propertyModification = modifications[i].propertyModification;
				Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(propertyModification, rootGameObject, out binding);
				if (type != null)
				{
					if (component != null && component.isHuman && binding.type == typeof(Transform) && component.IsBoneTransform(propertyModification.target as Transform))
					{
						Debug.LogWarning("Keyframing for humanoid rig is not supported!", propertyModification.target as Transform);
					}
					else
					{
						AnimationMode.AddPropertyModification(binding, propertyModification, modifications[i].keepPrefabOverride);
						EditorCurveBinding[] array = RotationCurveInterpolation.RemapAnimationBindingForAddKey(binding, activeAnimationClip);
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								AnimationRecording.AddKey(state, array[j], type, AnimationRecording.FindPropertyModification(rootGameObject, modifications, array[j]));
							}
						}
						else
						{
							AnimationRecording.AddKey(state, binding, type, propertyModification);
						}
					}
				}
				else
				{
					list.Add(modifications[i]);
				}
			}
			return list.ToArray();
		}
		private static bool ValueFromPropertyModification(PropertyModification modification, EditorCurveBinding binding, out object outObject)
		{
			if (modification == null)
			{
				outObject = null;
				return false;
			}
			if (binding.isPPtrCurve)
			{
				outObject = modification.objectReference;
				return true;
			}
			float num;
			if (float.TryParse(modification.value, out num))
			{
				outObject = num;
				return true;
			}
			outObject = null;
			return false;
		}
		private static void AddKey(AnimationWindowState state, EditorCurveBinding binding, Type type, PropertyModification modification)
		{
			GameObject rootGameObject = state.m_RootGameObject;
			AnimationClip activeAnimationClip = state.m_ActiveAnimationClip;
			AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(activeAnimationClip, binding, type);
			object currentValue = AnimationWindowUtility.GetCurrentValue(rootGameObject, binding);
			object value = null;
			if (animationWindowCurve.length == 0 && state.m_Frame != 0)
			{
				if (!AnimationRecording.ValueFromPropertyModification(modification, binding, out value))
				{
					value = currentValue;
				}
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, value, type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
			}
			AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, type, AnimationKeyTime.Frame(state.m_Frame, activeAnimationClip.frameRate));
			state.SaveCurve(animationWindowCurve);
		}
	}
}
