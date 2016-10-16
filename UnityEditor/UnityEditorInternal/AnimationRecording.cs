using System;
using System.Collections.Generic;
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

			public bool useEuler;

			public UndoPropertyModification eulerX;

			public UndoPropertyModification eulerY;

			public UndoPropertyModification eulerZ;
		}

		private const string kLocalRotation = "m_LocalRotation";

		private const string kLocalEulerAnglesHint = "m_LocalEulerAnglesHint";

		private static bool HasAnyRecordableModifications(GameObject root, UndoPropertyModification[] modifications)
		{
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding editorCurveBinding;
				if (AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out editorCurveBinding) != null)
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
				AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i].previousValue, root, out lhs);
				if (lhs == binding)
				{
					return modifications[i].previousValue;
				}
			}
			return null;
		}

		private static void CollectRotationModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications, ref Dictionary<object, AnimationRecording.RotationModification> rotationModifications)
		{
			List<UndoPropertyModification> list = new List<UndoPropertyModification>();
			UndoPropertyModification[] array = modifications;
			for (int i = 0; i < array.Length; i++)
			{
				UndoPropertyModification undoPropertyModification = array[i];
				EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
				PropertyModification previousValue = undoPropertyModification.previousValue;
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
					list.Add(undoPropertyModification);
				}
				else
				{
					list.Add(undoPropertyModification);
				}
			}
			if (rotationModifications.Count > 0)
			{
				modifications = list.ToArray();
			}
		}

		private static void AddRotationPropertyModification(IAnimationRecordingState state, UndoPropertyModification modification)
		{
			if (modification.previousValue == null)
			{
				return;
			}
			EditorCurveBinding binding = default(EditorCurveBinding);
			if (AnimationUtility.PropertyModificationToEditorCurveBinding(modification.previousValue, state.activeRootGameObject, out binding) != null)
			{
				AnimationMode.AddPropertyModification(binding, modification.previousValue, modification.keepPrefabOverride);
			}
		}

		private static void ProcessRotationModifications(IAnimationRecordingState state, ref UndoPropertyModification[] modifications)
		{
			Dictionary<object, AnimationRecording.RotationModification> dictionary = new Dictionary<object, AnimationRecording.RotationModification>();
			AnimationRecording.CollectRotationModifications(state, ref modifications, ref dictionary);
			foreach (KeyValuePair<object, AnimationRecording.RotationModification> current in dictionary)
			{
				AnimationRecording.RotationModification value = current.Value;
				Transform transform = current.Key as Transform;
				if (!(transform == null))
				{
					EditorCurveBinding binding = default(EditorCurveBinding);
					Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(value.lastQuatModification.currentValue, state.activeRootGameObject, out binding);
					AnimationRecording.AddRotationPropertyModification(state, value.x);
					AnimationRecording.AddRotationPropertyModification(state, value.y);
					AnimationRecording.AddRotationPropertyModification(state, value.z);
					AnimationRecording.AddRotationPropertyModification(state, value.w);
					if (value.useEuler)
					{
						Vector3 localEulerAngles = transform.GetLocalEulerAngles(RotationOrder.OrderZXY);
						Vector3 localEulerAngles2 = transform.GetLocalEulerAngles(RotationOrder.OrderZXY);
						object obj;
						if (AnimationRecording.ValueFromPropertyModification(value.eulerX.previousValue, binding, out obj))
						{
							localEulerAngles.x = (float)obj;
						}
						object obj2;
						if (AnimationRecording.ValueFromPropertyModification(value.eulerY.previousValue, binding, out obj2))
						{
							localEulerAngles.y = (float)obj2;
						}
						object obj3;
						if (AnimationRecording.ValueFromPropertyModification(value.eulerZ.previousValue, binding, out obj3))
						{
							localEulerAngles.z = (float)obj3;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.eulerX.currentValue, binding, out obj))
						{
							localEulerAngles2.x = (float)obj;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.eulerY.currentValue, binding, out obj2))
						{
							localEulerAngles2.y = (float)obj2;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.eulerZ.currentValue, binding, out obj3))
						{
							localEulerAngles2.z = (float)obj3;
						}
						AnimationRecording.AddRotationKey(state, binding, type, localEulerAngles, localEulerAngles2);
					}
					else
					{
						Quaternion localRotation = transform.localRotation;
						Quaternion localRotation2 = transform.localRotation;
						object obj4;
						if (AnimationRecording.ValueFromPropertyModification(value.x.previousValue, binding, out obj4))
						{
							localRotation.x = (float)obj4;
						}
						object obj5;
						if (AnimationRecording.ValueFromPropertyModification(value.y.previousValue, binding, out obj5))
						{
							localRotation.y = (float)obj5;
						}
						object obj6;
						if (AnimationRecording.ValueFromPropertyModification(value.z.previousValue, binding, out obj6))
						{
							localRotation.z = (float)obj6;
						}
						object obj7;
						if (AnimationRecording.ValueFromPropertyModification(value.w.previousValue, binding, out obj7))
						{
							localRotation.w = (float)obj7;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.x.currentValue, binding, out obj4))
						{
							localRotation2.x = (float)obj4;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.y.currentValue, binding, out obj5))
						{
							localRotation2.y = (float)obj5;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.z.currentValue, binding, out obj6))
						{
							localRotation2.z = (float)obj6;
						}
						if (AnimationRecording.ValueFromPropertyModification(value.w.currentValue, binding, out obj7))
						{
							localRotation2.w = (float)obj7;
						}
						AnimationRecording.AddRotationKey(state, binding, type, localRotation.eulerAngles, localRotation2.eulerAngles);
					}
				}
			}
		}

		public static UndoPropertyModification[] Process(IAnimationRecordingState state, UndoPropertyModification[] modifications)
		{
			GameObject activeRootGameObject = state.activeRootGameObject;
			if (activeRootGameObject == null)
			{
				return modifications;
			}
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			Animator component = activeRootGameObject.GetComponent<Animator>();
			if (!AnimationRecording.HasAnyRecordableModifications(activeRootGameObject, modifications))
			{
				return modifications;
			}
			AnimationRecording.ProcessRotationModifications(state, ref modifications);
			List<UndoPropertyModification> list = new List<UndoPropertyModification>();
			for (int i = 0; i < modifications.Length; i++)
			{
				EditorCurveBinding binding = default(EditorCurveBinding);
				PropertyModification previousValue = modifications[i].previousValue;
				Type type = AnimationUtility.PropertyModificationToEditorCurveBinding(previousValue, activeRootGameObject, out binding);
				if (type != null)
				{
					if (component != null && component.isHuman && binding.type == typeof(Transform) && component.IsBoneTransform(previousValue.target as Transform))
					{
						Debug.LogWarning("Keyframing for humanoid rig is not supported!", previousValue.target as Transform);
					}
					else
					{
						AnimationMode.AddPropertyModification(binding, previousValue, modifications[i].keepPrefabOverride);
						EditorCurveBinding[] array = RotationCurveInterpolation.RemapAnimationBindingForAddKey(binding, activeAnimationClip);
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								AnimationRecording.AddKey(state, array[j], type, AnimationRecording.FindPropertyModification(activeRootGameObject, modifications, array[j]));
							}
						}
						else
						{
							AnimationRecording.AddKey(state, binding, type, previousValue);
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

		private static void AddKey(IAnimationRecordingState state, EditorCurveBinding binding, Type type, PropertyModification modification)
		{
			GameObject activeRootGameObject = state.activeRootGameObject;
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			if ((activeAnimationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
			{
				return;
			}
			AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(activeAnimationClip, binding, type);
			object currentValue = CurveBindingUtility.GetCurrentValue(activeRootGameObject, binding);
			if (animationWindowCurve.length == 0)
			{
				object value = null;
				if (!AnimationRecording.ValueFromPropertyModification(modification, binding, out value))
				{
					value = currentValue;
				}
				if (state.frame != 0)
				{
					AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, value, type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
				}
			}
			AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, type, AnimationKeyTime.Frame(state.frame, activeAnimationClip.frameRate));
			state.SaveCurve(animationWindowCurve);
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
			if ((activeAnimationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
			{
				return;
			}
			EditorCurveBinding[] array = RotationCurveInterpolation.RemapAnimationBindingForRotationAddKey(binding, activeAnimationClip);
			for (int i = 0; i < 3; i++)
			{
				AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(activeAnimationClip, array[i], type);
				if (animationWindowCurve.length == 0 && state.frame != 0)
				{
					AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, previousEulerAngles[i], type, AnimationKeyTime.Frame(0, activeAnimationClip.frameRate));
				}
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentEulerAngles[i], type, AnimationKeyTime.Frame(state.frame, activeAnimationClip.frameRate));
				state.SaveCurve(animationWindowCurve);
			}
		}
	}
}
