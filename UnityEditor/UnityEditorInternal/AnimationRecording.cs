using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternal
{
	internal class AnimationRecording
	{
		internal class RotationModification
		{
			public UndoPropertyModification x;

			public UndoPropertyModification y;

			public UndoPropertyModification z;

			public UndoPropertyModification w;

			public UndoPropertyModification lastQuatModification;

			public bool useEuler = false;

			public UndoPropertyModification eulerX;

			public UndoPropertyModification eulerY;

			public UndoPropertyModification eulerZ;
		}

		private const string kLocalRotation = "m_LocalRotation";

		private const string kLocalEulerAnglesHint = "m_LocalEulerAnglesHint";

		private static bool HasAnyRecordableModifications(GameObject root, UndoPropertyModification[] modifications)
		{
			bool result;
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding editorCurveBinding;
				if (AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out editorCurveBinding) != null)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		private static PropertyModification FindPropertyModification(GameObject root, UndoPropertyModification[] modifications, EditorCurveBinding binding)
		{
			PropertyModification result;
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding lhs;
				AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out lhs);
				if (lhs == binding)
				{
					result = modifications[i].previousValue;
					return result;
				}
			}
			result = null;
			return result;
		}

		private static PropertyModification CreateDummyPropertyModification(GameObject root, PropertyModification baseProperty, EditorCurveBinding binding)
		{
			PropertyModification propertyModification = new PropertyModification();
			propertyModification.target = baseProperty.target;
			propertyModification.propertyPath = binding.propertyName;
			object currentValue = CurveBindingUtility.GetCurrentValue(root, binding);
			if (binding.isPPtrCurve)
			{
				propertyModification.objectReference = (UnityEngine.Object)currentValue;
			}
			else
			{
				propertyModification.value = ((float)currentValue).ToString();
			}
			return propertyModification;
		}

		private static UndoPropertyModification[] FilterModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications)
		{
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			GameObject activeRootGameObject = state.activeRootGameObject;
			List<UndoPropertyModification> list = new List<UndoPropertyModification>();
			List<UndoPropertyModification> list2 = new List<UndoPropertyModification>();
			for (int i = 0; i < modifications.Length; i++)
			{
				UndoPropertyModification item = modifications[i];
				PropertyModification previousValue = item.previousValue;
				if (state.DiscardModification(previousValue))
				{
					list.Add(item);
				}
				else
				{
					EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
					if (AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, activeRootGameObject, out editorCurveBinding) != null)
					{
						list2.Add(item);
					}
					else
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				modifications = list2.ToArray();
			}
			return list.ToArray();
		}

		private static void CollectRotationModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications, ref Dictionary<object, AnimationRecording.RotationModification> rotationModifications)
		{
			List<UndoPropertyModification> list = new List<UndoPropertyModification>();
			UndoPropertyModification[] array = modifications;
			for (int i = 0; i < array.Length; i++)
			{
				UndoPropertyModification undoPropertyModification = array[i];
				PropertyModification previousValue = undoPropertyModification.previousValue;
				if (!(previousValue.target is Transform))
				{
					list.Add(undoPropertyModification);
				}
				else
				{
					EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
					AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, state.activeRootGameObject, out editorCurveBinding);
					if (editorCurveBinding.propertyName.StartsWith("m_LocalRotation"))
					{
						AnimationRecording.RotationModification rotationModification;
						if (!rotationModifications.TryGetValue(previousValue.target, out rotationModification))
						{
							rotationModification = new AnimationRecording.RotationModification();
							rotationModifications[previousValue.target] = rotationModification;
						}
						if (editorCurveBinding.propertyName.EndsWith("x"))
						{
							rotationModification.x = undoPropertyModification;
						}
						else if (editorCurveBinding.propertyName.EndsWith("y"))
						{
							rotationModification.y = undoPropertyModification;
						}
						else if (editorCurveBinding.propertyName.EndsWith("z"))
						{
							rotationModification.z = undoPropertyModification;
						}
						else if (editorCurveBinding.propertyName.EndsWith("w"))
						{
							rotationModification.w = undoPropertyModification;
						}
						rotationModification.lastQuatModification = undoPropertyModification;
					}
					else if (previousValue.propertyPath.StartsWith("m_LocalEulerAnglesHint"))
					{
						AnimationRecording.RotationModification rotationModification2;
						if (!rotationModifications.TryGetValue(previousValue.target, out rotationModification2))
						{
							rotationModification2 = new AnimationRecording.RotationModification();
							rotationModifications[previousValue.target] = rotationModification2;
						}
						rotationModification2.useEuler = true;
						if (previousValue.propertyPath.EndsWith("x"))
						{
							rotationModification2.eulerX = undoPropertyModification;
						}
						else if (previousValue.propertyPath.EndsWith("y"))
						{
							rotationModification2.eulerY = undoPropertyModification;
						}
						else if (previousValue.propertyPath.EndsWith("z"))
						{
							rotationModification2.eulerZ = undoPropertyModification;
						}
					}
					else
					{
						list.Add(undoPropertyModification);
					}
				}
			}
			if (rotationModifications.Count > 0)
			{
				modifications = list.ToArray();
			}
		}

		private static void DiscardRotationModification(AnimationRecording.RotationModification rotationModification, ref List<UndoPropertyModification> discardedModifications)
		{
			if (rotationModification.x.currentValue != null)
			{
				discardedModifications.Add(rotationModification.x);
			}
			if (rotationModification.y.currentValue != null)
			{
				discardedModifications.Add(rotationModification.y);
			}
			if (rotationModification.z.currentValue != null)
			{
				discardedModifications.Add(rotationModification.z);
			}
			if (rotationModification.w.currentValue != null)
			{
				discardedModifications.Add(rotationModification.w);
			}
			if (rotationModification.eulerX.currentValue != null)
			{
				discardedModifications.Add(rotationModification.eulerX);
			}
			if (rotationModification.eulerY.currentValue != null)
			{
				discardedModifications.Add(rotationModification.eulerY);
			}
			if (rotationModification.eulerZ.currentValue != null)
			{
				discardedModifications.Add(rotationModification.eulerZ);
			}
		}

		private static UndoPropertyModification[] FilterRotationModifications(IAnimationRecordingState state, ref Dictionary<object, AnimationRecording.RotationModification> rotationModifications)
		{
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			GameObject activeRootGameObject = state.activeRootGameObject;
			List<object> list = new List<object>();
			List<UndoPropertyModification> list2 = new List<UndoPropertyModification>();
			foreach (KeyValuePair<object, AnimationRecording.RotationModification> current in rotationModifications)
			{
				AnimationRecording.RotationModification value = current.Value;
				if (state.DiscardModification(value.lastQuatModification.currentValue))
				{
					AnimationRecording.DiscardRotationModification(value, ref list2);
					list.Add(current.Key);
				}
				else
				{
					EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
					if (AnimationUtility.PropertyModificationToEditorCurveBinding(value.lastQuatModification.currentValue, activeRootGameObject, out editorCurveBinding) == null)
					{
						AnimationRecording.DiscardRotationModification(value, ref list2);
						list.Add(current.Key);
					}
				}
			}
			foreach (object current2 in list)
			{
				rotationModifications.Remove(current2);
			}
			return list2.ToArray();
		}

		private static void AddRotationPropertyModification(IAnimationRecordingState state, EditorCurveBinding baseBinding, UndoPropertyModification modification)
		{
			if (modification.previousValue != null)
			{
				EditorCurveBinding binding = baseBinding;
				binding.propertyName = modification.previousValue.propertyPath;
				state.AddPropertyModification(binding, modification.previousValue, modification.keepPrefabOverride);
			}
		}

		private static UndoPropertyModification[] ProcessRotationModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications)
		{
			Dictionary<object, AnimationRecording.RotationModification> dictionary = new Dictionary<object, AnimationRecording.RotationModification>();
			AnimationRecording.CollectRotationModifications(state, ref modifications, ref dictionary);
			UndoPropertyModification[] result = AnimationRecording.FilterRotationModifications(state, ref dictionary);
			foreach (KeyValuePair<object, AnimationRecording.RotationModification> current in dictionary)
			{
				AnimationRecording.RotationModification value = current.Value;
				Transform transform = current.Key as Transform;
				if (!(transform == null))
				{
					EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
					Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(value.lastQuatModification.currentValue, state.activeRootGameObject, out editorCurveBinding);
					if (type != null)
					{
						AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.x);
						AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.y);
						AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.z);
						AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.w);
						Quaternion localRotation = transform.localRotation;
						Quaternion localRotation2 = transform.localRotation;
						object obj;
						if (AnimationRecording.ValueFromPropertyModification(value.x.previousValue, editorCurveBinding, out obj))
						{
							localRotation.x = (float)obj;
						}
						object obj2;
						if (AnimationRecording.ValueFromPropertyModification(value.y.previousValue, editorCurveBinding, out obj2))
						{
							localRotation.y = (float)obj2;
						}
						object obj3;
						if (AnimationRecording.ValueFromPropertyModification(value.z.previousValue, editorCurveBinding, out obj3))
						{
							localRotation.z = (float)obj3;
						}
						object obj4;
						if (AnimationRecording.ValueFromPropertyModification(value.w.previousValue, editorCurveBinding, out obj4))
						{
							localRotation.w = (float)obj4;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.x.currentValue, editorCurveBinding, out obj))
						{
							localRotation2.x = (float)obj;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.y.currentValue, editorCurveBinding, out obj2))
						{
							localRotation2.y = (float)obj2;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.z.currentValue, editorCurveBinding, out obj3))
						{
							localRotation2.z = (float)obj3;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.w.currentValue, editorCurveBinding, out obj4))
						{
							localRotation2.w = (float)obj4;
						}
						if (value.useEuler)
						{
							AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.eulerX);
							AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.eulerY);
							AnimationRecording.AddRotationPropertyModification(state, editorCurveBinding, value.eulerZ);
							Vector3 vector = transform.GetLocalEulerAngles(RotationOrder.OrderZXY);
							Vector3 vector2 = vector;
							object obj5;
							if (AnimationRecording.ValueFromPropertyModification(value.eulerX.previousValue, editorCurveBinding, out obj5))
							{
								vector.x = (float)obj5;
							}
							object obj6;
							if (AnimationRecording.ValueFromPropertyModification(value.eulerY.previousValue, editorCurveBinding, out obj6))
							{
								vector.y = (float)obj6;
							}
							object obj7;
							if (AnimationRecording.ValueFromPropertyModification(value.eulerZ.previousValue, editorCurveBinding, out obj7))
							{
								vector.z = (float)obj7;
							}
							if (AnimationRecording.ValueFromPropertyModification(value.eulerX.currentValue, editorCurveBinding, out obj5))
							{
								vector2.x = (float)obj5;
							}
							if (AnimationRecording.ValueFromPropertyModification(value.eulerY.currentValue, editorCurveBinding, out obj6))
							{
								vector2.y = (float)obj6;
							}
							if (AnimationRecording.ValueFromPropertyModification(value.eulerZ.currentValue, editorCurveBinding, out obj7))
							{
								vector2.z = (float)obj7;
							}
							vector = AnimationUtility.GetClosestEuler(localRotation, vector, RotationOrder.OrderZXY);
							vector2 = AnimationUtility.GetClosestEuler(localRotation2, vector2, RotationOrder.OrderZXY);
							AnimationRecording.AddRotationKey(state, editorCurveBinding, type, vector, vector2);
						}
						else
						{
							AnimationRecording.AddRotationKey(state, editorCurveBinding, type, localRotation.eulerAngles, localRotation2.eulerAngles);
						}
					}
				}
			}
			return result;
		}

		public static UndoPropertyModification[] ProcessModifications(IAnimationRecordingState state, UndoPropertyModification[] modifications)
		{
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			GameObject activeRootGameObject = state.activeRootGameObject;
			Animator component = activeRootGameObject.GetComponent<Animator>();
			UndoPropertyModification[] result = AnimationRecording.FilterModifications(state, ref modifications);
			int i = 0;
			while (i < modifications.Length)
			{
				EditorCurveBinding binding = default(EditorCurveBinding);
				PropertyModification previousValue = modifications[i].previousValue;
				Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, activeRootGameObject, out binding);
				if (type != null)
				{
					if (component != null && component.isHuman && binding.type == typeof(Transform) && component.gameObject.transform != previousValue.target && component.IsBoneTransform(previousValue.target as Transform))
					{
						Debug.LogWarning("Keyframing for humanoid rig is not supported!", previousValue.target as Transform);
					}
					else
					{
						EditorCurveBinding[] array = RotationCurveInterpolation.RemapAnimationBindingForAddKey(binding, activeAnimationClip);
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								PropertyModification propertyModification = AnimationRecording.FindPropertyModification(activeRootGameObject, modifications, array[j]);
								if (propertyModification == null)
								{
									propertyModification = AnimationRecording.CreateDummyPropertyModification(activeRootGameObject, previousValue, array[j]);
								}
								state.AddPropertyModification(array[j], propertyModification, modifications[i].keepPrefabOverride);
								AnimationRecording.AddKey(state, array[j], type, propertyModification);
							}
						}
						else
						{
							state.AddPropertyModification(binding, previousValue, modifications[i].keepPrefabOverride);
							AnimationRecording.AddKey(state, binding, type, previousValue);
						}
					}
				}
				IL_19A:
				i++;
				continue;
				goto IL_19A;
			}
			return result;
		}

		public static UndoPropertyModification[] Process(IAnimationRecordingState state, UndoPropertyModification[] modifications)
		{
			GameObject activeRootGameObject = state.activeRootGameObject;
			UndoPropertyModification[] result;
			if (activeRootGameObject == null)
			{
				result = modifications;
			}
			else if (!AnimationRecording.HasAnyRecordableModifications(activeRootGameObject, modifications))
			{
				result = modifications;
			}
			else
			{
				UndoPropertyModification[] first = AnimationRecording.ProcessRotationModifications(state, ref modifications);
				UndoPropertyModification[] second = AnimationRecording.ProcessModifications(state, modifications);
				result = first.Concat(second).ToArray<UndoPropertyModification>();
			}
			return result;
		}

		private static bool ValueFromPropertyModification(PropertyModification modification, EditorCurveBinding binding, out object outObject)
		{
			bool result;
			float num;
			if (modification == null)
			{
				outObject = null;
				result = false;
			}
			else if (binding.isPPtrCurve)
			{
				outObject = modification.objectReference;
				result = true;
			}
			else if (float.TryParse(modification.value, out num))
			{
				outObject = num;
				result = true;
			}
			else
			{
				outObject = null;
				result = false;
			}
			return result;
		}

		private static void AddKey(IAnimationRecordingState state, EditorCurveBinding binding, Type type, PropertyModification modification)
		{
			GameObject activeRootGameObject = state.activeRootGameObject;
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			if ((activeAnimationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None)
			{
				AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(activeAnimationClip, binding, type);
				object currentValue = CurveBindingUtility.GetCurrentValue(activeRootGameObject, binding);
				if (state.addZeroFrame)
				{
					if (animationWindowCurve.length == 0)
					{
						object value = null;
						if (!AnimationRecording.ValueFromPropertyModification(modification, binding, out value))
						{
							value = currentValue;
						}
						if (state.currentFrame != 0)
						{
							AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, value, type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
						}
					}
				}
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, type, AnimationKeyTime.Frame(state.currentFrame, activeAnimationClip.frameRate));
				state.SaveCurve(animationWindowCurve);
			}
		}

		public static void SaveModifiedCurve(AnimationWindowCurve curve, AnimationClip clip)
		{
			curve.m_Keyframes.Sort((AnimationWindowKeyframe a, AnimationWindowKeyframe b) => a.time.CompareTo(b.time));
			if (curve.isPPtrCurve)
			{
				ObjectReferenceKeyframe[] array = curve.ToObjectCurve();
				if (array.Length == 0)
				{
					array = null;
				}
				AnimationUtility.SetObjectReferenceCurve(clip, curve.binding, array);
			}
			else
			{
				AnimationCurve animationCurve = curve.ToAnimationCurve();
				if (animationCurve.keys.Length == 0)
				{
					animationCurve = null;
				}
				else
				{
					QuaternionCurveTangentCalculation.UpdateTangentsFromMode(animationCurve, clip, curve.binding);
				}
				AnimationUtility.SetEditorCurve(clip, curve.binding, animationCurve);
			}
		}

		private static void AddRotationKey(IAnimationRecordingState state, EditorCurveBinding binding, Type type, Vector3 previousEulerAngles, Vector3 currentEulerAngles)
		{
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			if ((activeAnimationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None)
			{
				EditorCurveBinding[] array = RotationCurveInterpolation.RemapAnimationBindingForRotationAddKey(binding, activeAnimationClip);
				for (int i = 0; i < 3; i++)
				{
					AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(activeAnimationClip, array[i], type);
					if (state.addZeroFrame)
					{
						if (animationWindowCurve.length == 0)
						{
							if (state.currentFrame != 0)
							{
								AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, previousEulerAngles[i], type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
							}
						}
					}
					AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentEulerAngles[i], type, AnimationKeyTime.Frame(state.currentFrame, activeAnimationClip.frameRate));
					state.SaveCurve(animationWindowCurve);
				}
			}
		}
	}
}
