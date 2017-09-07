using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditorInternal
{
	internal static class AnimationWindowUtility
	{
		internal static string s_LastPathUsedForNewClip;

		public static void CreateDefaultCurves(AnimationWindowState state, AnimationWindowSelectionItem selectionItem, EditorCurveBinding[] properties)
		{
			properties = RotationCurveInterpolation.ConvertRotationPropertiesToDefaultInterpolation(selectionItem.animationClip, properties);
			EditorCurveBinding[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding binding = array[i];
				state.SaveCurve(AnimationWindowUtility.CreateDefaultCurve(selectionItem, binding));
			}
		}

		public static AnimationWindowCurve CreateDefaultCurve(AnimationWindowSelectionItem selectionItem, EditorCurveBinding binding)
		{
			AnimationClip animationClip = selectionItem.animationClip;
			Type editorCurveValueType = selectionItem.GetEditorCurveValueType(binding);
			AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(animationClip, binding, editorCurveValueType);
			object currentValue = CurveBindingUtility.GetCurrentValue(selectionItem.rootGameObject, binding);
			if (animationClip.length == 0f)
			{
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, animationClip.frameRate));
			}
			else
			{
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, animationClip.frameRate));
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, editorCurveValueType, AnimationKeyTime.Time(animationClip.length, animationClip.frameRate));
			}
			return animationWindowCurve;
		}

		public static bool ShouldShowAnimationWindowCurve(EditorCurveBinding curveBinding)
		{
			return !AnimationWindowUtility.IsTransformType(curveBinding.type) || !curveBinding.propertyName.EndsWith(".w");
		}

		public static bool IsNodeLeftOverCurve(AnimationWindowHierarchyNode node)
		{
			EditorCurveBinding? binding = node.binding;
			bool result;
			if (binding.HasValue)
			{
				if (node.curves.Length > 0)
				{
					AnimationWindowSelectionItem selectionBinding = node.curves[0].selectionBinding;
					if (selectionBinding != null)
					{
						if (selectionBinding.rootGameObject == null && selectionBinding.scriptableObject == null)
						{
							result = false;
							return result;
						}
						AnimationWindowSelectionItem arg_77_0 = selectionBinding;
						EditorCurveBinding? binding2 = node.binding;
						result = (arg_77_0.GetEditorCurveValueType(binding2.Value) == null);
						return result;
					}
				}
			}
			if (node.hasChildren)
			{
				using (List<TreeViewItem>.Enumerator enumerator = node.children.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TreeViewItem current = enumerator.Current;
						result = AnimationWindowUtility.IsNodeLeftOverCurve(current as AnimationWindowHierarchyNode);
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		public static bool IsNodeAmbiguous(AnimationWindowHierarchyNode node)
		{
			EditorCurveBinding? binding = node.binding;
			bool result;
			if (binding.HasValue)
			{
				if (node.curves.Length > 0)
				{
					AnimationWindowSelectionItem selectionBinding = node.curves[0].selectionBinding;
					if (selectionBinding != null)
					{
						if (selectionBinding.rootGameObject != null)
						{
							result = AnimationUtility.AmbiguousBinding(node.binding.Value.path, node.binding.Value.m_ClassID, selectionBinding.rootGameObject.transform);
							return result;
						}
					}
				}
			}
			if (node.hasChildren)
			{
				using (List<TreeViewItem>.Enumerator enumerator = node.children.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TreeViewItem current = enumerator.Current;
						result = AnimationWindowUtility.IsNodeAmbiguous(current as AnimationWindowHierarchyNode);
						return result;
					}
				}
			}
			result = false;
			return result;
		}

		public static bool IsNodePhantom(AnimationWindowHierarchyNode node)
		{
			EditorCurveBinding? binding = node.binding;
			return binding.HasValue && node.binding.Value.isPhantom;
		}

		public static void AddSelectedKeyframes(AnimationWindowState state, AnimationKeyTime time)
		{
			List<AnimationWindowCurve> list = (state.activeCurves.Count <= 0) ? state.allCurves : state.activeCurves;
			AnimationWindowUtility.AddKeyframes(state, list.ToArray(), time);
		}

		public static void AddKeyframes(AnimationWindowState state, AnimationWindowCurve[] curves, AnimationKeyTime time)
		{
			string undoLabel = "Add Key";
			state.SaveKeySelection(undoLabel);
			state.ClearKeySelections();
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (animationWindowCurve.animationIsEditable)
				{
					AnimationKeyTime time2 = AnimationKeyTime.Time(time.time - animationWindowCurve.timeOffset, time.frameRate);
					object currentValue = CurveBindingUtility.GetCurrentValue(state, animationWindowCurve);
					AnimationWindowKeyframe animationWindowKeyframe = AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, animationWindowCurve.valueType, time2);
					if (animationWindowKeyframe != null)
					{
						state.SaveCurve(animationWindowCurve, undoLabel);
						state.SelectKey(animationWindowKeyframe);
					}
				}
			}
		}

		public static void RemoveKeyframes(AnimationWindowState state, AnimationWindowCurve[] curves, AnimationKeyTime time)
		{
			string undoLabel = "Remove Key";
			state.SaveKeySelection(undoLabel);
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (animationWindowCurve.animationIsEditable)
				{
					AnimationKeyTime time2 = AnimationKeyTime.Time(time.time - animationWindowCurve.timeOffset, time.frameRate);
					animationWindowCurve.RemoveKeyframe(time2);
					state.SaveCurve(animationWindowCurve, undoLabel);
				}
			}
		}

		public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowCurve curve, object value, Type type, AnimationKeyTime time)
		{
			AnimationWindowKeyframe animationWindowKeyframe = curve.FindKeyAtTime(time);
			AnimationWindowKeyframe result;
			if (animationWindowKeyframe != null)
			{
				animationWindowKeyframe.value = value;
				result = animationWindowKeyframe;
			}
			else
			{
				AnimationWindowKeyframe animationWindowKeyframe2 = null;
				if (curve.isPPtrCurve)
				{
					animationWindowKeyframe2 = new AnimationWindowKeyframe();
					animationWindowKeyframe2.time = time.time;
					animationWindowKeyframe2.value = value;
					animationWindowKeyframe2.curve = curve;
					curve.AddKeyframe(animationWindowKeyframe2, time);
				}
				else if (type == typeof(bool) || type == typeof(float))
				{
					animationWindowKeyframe2 = new AnimationWindowKeyframe();
					AnimationCurve animationCurve = curve.ToAnimationCurve();
					Keyframe key = new Keyframe(time.time, (float)value);
					if (type == typeof(bool))
					{
						AnimationUtility.SetKeyLeftTangentMode(ref key, AnimationUtility.TangentMode.Constant);
						AnimationUtility.SetKeyRightTangentMode(ref key, AnimationUtility.TangentMode.Constant);
						AnimationUtility.SetKeyBroken(ref key, true);
						animationWindowKeyframe2.m_TangentMode = key.tangentMode;
						animationWindowKeyframe2.m_InTangent = float.PositiveInfinity;
						animationWindowKeyframe2.m_OutTangent = float.PositiveInfinity;
					}
					else
					{
						int num = animationCurve.AddKey(key);
						if (num != -1)
						{
							CurveUtility.SetKeyModeFromContext(animationCurve, num);
							animationWindowKeyframe2.m_TangentMode = animationCurve[num].tangentMode;
						}
					}
					animationWindowKeyframe2.time = time.time;
					animationWindowKeyframe2.value = value;
					animationWindowKeyframe2.curve = curve;
					curve.AddKeyframe(animationWindowKeyframe2, time);
				}
				result = animationWindowKeyframe2;
			}
			return result;
		}

		public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, bool entireHierarchy)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			if (curves != null)
			{
				for (int i = 0; i < curves.Length; i++)
				{
					AnimationWindowCurve animationWindowCurve = curves[i];
					if (animationWindowCurve.path.Equals(path) || (entireHierarchy && animationWindowCurve.path.Contains(path)))
					{
						list.Add(animationWindowCurve);
					}
				}
			}
			return list;
		}

		public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, Type animatableObjectType)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			if (curves != null)
			{
				for (int i = 0; i < curves.Length; i++)
				{
					AnimationWindowCurve animationWindowCurve = curves[i];
					if (animationWindowCurve.path.Equals(path) && animationWindowCurve.type == animatableObjectType)
					{
						list.Add(animationWindowCurve);
					}
				}
			}
			return list;
		}

		public static bool IsCurveCreated(AnimationClip clip, EditorCurveBinding binding)
		{
			bool result;
			if (binding.isPPtrCurve)
			{
				result = (AnimationUtility.GetObjectReferenceCurve(clip, binding) != null);
			}
			else
			{
				if (AnimationWindowUtility.IsRectTransformPosition(binding))
				{
					binding.propertyName = binding.propertyName.Replace(".x", ".z").Replace(".y", ".z");
				}
				if (AnimationWindowUtility.IsRotationCurve(binding))
				{
					result = (AnimationUtility.GetEditorCurve(clip, binding) != null || AnimationWindowUtility.HasOtherRotationCurve(clip, binding));
				}
				else
				{
					result = (AnimationUtility.GetEditorCurve(clip, binding) != null);
				}
			}
			return result;
		}

		internal static bool HasOtherRotationCurve(AnimationClip clip, EditorCurveBinding rotationBinding)
		{
			bool result;
			if (rotationBinding.propertyName.StartsWith("m_LocalRotation"))
			{
				EditorCurveBinding binding = rotationBinding;
				EditorCurveBinding binding2 = rotationBinding;
				EditorCurveBinding binding3 = rotationBinding;
				binding.propertyName = "localEulerAnglesRaw.x";
				binding2.propertyName = "localEulerAnglesRaw.y";
				binding3.propertyName = "localEulerAnglesRaw.z";
				result = (AnimationUtility.GetEditorCurve(clip, binding) != null || AnimationUtility.GetEditorCurve(clip, binding2) != null || AnimationUtility.GetEditorCurve(clip, binding3) != null);
			}
			else
			{
				EditorCurveBinding binding4 = rotationBinding;
				EditorCurveBinding binding5 = rotationBinding;
				EditorCurveBinding binding6 = rotationBinding;
				EditorCurveBinding binding7 = rotationBinding;
				binding4.propertyName = "m_LocalRotation.x";
				binding5.propertyName = "m_LocalRotation.y";
				binding6.propertyName = "m_LocalRotation.z";
				binding7.propertyName = "m_LocalRotation.w";
				result = (AnimationUtility.GetEditorCurve(clip, binding4) != null || AnimationUtility.GetEditorCurve(clip, binding5) != null || AnimationUtility.GetEditorCurve(clip, binding6) != null || AnimationUtility.GetEditorCurve(clip, binding7) != null);
			}
			return result;
		}

		internal static bool IsRotationCurve(EditorCurveBinding curveBinding)
		{
			string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(curveBinding.propertyName);
			return propertyGroupName == "m_LocalRotation" || propertyGroupName == "localEulerAnglesRaw";
		}

		public static bool IsRectTransformPosition(EditorCurveBinding curveBinding)
		{
			return curveBinding.type == typeof(RectTransform) && AnimationWindowUtility.GetPropertyGroupName(curveBinding.propertyName) == "m_LocalPosition";
		}

		public static bool ContainsFloatKeyframes(List<AnimationWindowKeyframe> keyframes)
		{
			bool result;
			if (keyframes == null || keyframes.Count == 0)
			{
				result = false;
			}
			else
			{
				foreach (AnimationWindowKeyframe current in keyframes)
				{
					if (!current.isPPtrCurve)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, Type animatableObjectType, string propertyName)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			if (curves != null)
			{
				string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(propertyName);
				bool flag = propertyGroupName == propertyName;
				for (int i = 0; i < curves.Length; i++)
				{
					AnimationWindowCurve animationWindowCurve = curves[i];
					bool flag2 = (!flag) ? animationWindowCurve.propertyName.Equals(propertyName) : AnimationWindowUtility.GetPropertyGroupName(animationWindowCurve.propertyName).Equals(propertyGroupName);
					if (animationWindowCurve.path.Equals(path) && animationWindowCurve.type == animatableObjectType && flag2)
					{
						list.Add(animationWindowCurve);
					}
				}
			}
			return list;
		}

		public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
		{
			object result;
			if (curveBinding.isPPtrCurve)
			{
				UnityEngine.Object @object;
				AnimationUtility.GetObjectReferenceValue(rootGameObject, curveBinding, out @object);
				result = @object;
			}
			else
			{
				float num;
				AnimationUtility.GetFloatValue(rootGameObject, curveBinding, out num);
				result = num;
			}
			return result;
		}

		public static List<EditorCurveBinding> GetAnimatableProperties(GameObject gameObject, GameObject root, Type valueType)
		{
			EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, root);
			List<EditorCurveBinding> list = new List<EditorCurveBinding>();
			EditorCurveBinding[] array = animatableBindings;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = array[i];
				if (AnimationUtility.GetEditorCurveValueType(root, editorCurveBinding) == valueType)
				{
					list.Add(editorCurveBinding);
				}
			}
			return list;
		}

		public static List<EditorCurveBinding> GetAnimatableProperties(GameObject gameObject, GameObject root, Type objectType, Type valueType)
		{
			EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(gameObject, root);
			List<EditorCurveBinding> list = new List<EditorCurveBinding>();
			EditorCurveBinding[] array = animatableBindings;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = array[i];
				if (editorCurveBinding.type == objectType && AnimationUtility.GetEditorCurveValueType(root, editorCurveBinding) == valueType)
				{
					list.Add(editorCurveBinding);
				}
			}
			return list;
		}

		public static List<EditorCurveBinding> GetAnimatableProperties(ScriptableObject scriptableObject, Type valueType)
		{
			EditorCurveBinding[] scriptableObjectAnimatableBindings = AnimationUtility.GetScriptableObjectAnimatableBindings(scriptableObject);
			List<EditorCurveBinding> list = new List<EditorCurveBinding>();
			EditorCurveBinding[] array = scriptableObjectAnimatableBindings;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding editorCurveBinding = array[i];
				if (AnimationUtility.GetScriptableObjectEditorCurveValueType(scriptableObject, editorCurveBinding) == valueType)
				{
					list.Add(editorCurveBinding);
				}
			}
			return list;
		}

		public static bool PropertyIsAnimatable(UnityEngine.Object targetObject, string propertyPath, UnityEngine.Object rootObject)
		{
			bool result;
			if (targetObject is ScriptableObject)
			{
				ScriptableObject scriptableObject = (ScriptableObject)targetObject;
				EditorCurveBinding[] scriptableObjectAnimatableBindings = AnimationUtility.GetScriptableObjectAnimatableBindings(scriptableObject);
				result = Array.Exists<EditorCurveBinding>(scriptableObjectAnimatableBindings, (EditorCurveBinding binding) => binding.propertyName == propertyPath);
			}
			else
			{
				GameObject gameObject = targetObject as GameObject;
				if (targetObject is Component)
				{
					gameObject = ((Component)targetObject).gameObject;
				}
				if (gameObject != null)
				{
					PropertyModification propertyModification = new PropertyModification();
					propertyModification.propertyPath = propertyPath;
					propertyModification.target = targetObject;
					EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
					result = (AnimationUtility.PropertyModificationToEditorCurveBinding(propertyModification, (!(rootObject == null)) ? ((GameObject)rootObject) : gameObject, out editorCurveBinding) != null);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public static PropertyModification[] SerializedPropertyToPropertyModifications(SerializedProperty property)
		{
			List<SerializedProperty> list = new List<SerializedProperty>();
			list.Add(property);
			if (property.hasChildren)
			{
				SerializedProperty serializedProperty = property.Copy();
				SerializedProperty endProperty = property.GetEndProperty(false);
				while (serializedProperty.Next(true) && !SerializedProperty.EqualContents(serializedProperty, endProperty) && serializedProperty.propertyPath.StartsWith(property.propertyPath))
				{
					list.Add(serializedProperty.Copy());
				}
			}
			if (property.propertyPath.StartsWith("m_LocalRotation"))
			{
				SerializedObject serializedObject = property.serializedObject;
				if (serializedObject.targetObject is Transform)
				{
					SerializedProperty serializedProperty2 = serializedObject.FindProperty("m_LocalEulerAnglesHint");
					if (serializedProperty2 != null && serializedProperty2.hasChildren)
					{
						SerializedProperty serializedProperty3 = serializedProperty2.Copy();
						SerializedProperty endProperty2 = serializedProperty2.GetEndProperty(false);
						while (serializedProperty3.Next(true) && !SerializedProperty.EqualContents(serializedProperty3, endProperty2) && serializedProperty3.propertyPath.StartsWith(serializedProperty2.propertyPath))
						{
							list.Add(serializedProperty3.Copy());
						}
					}
				}
			}
			List<PropertyModification> list2 = new List<PropertyModification>();
			for (int i = 0; i < list.Count; i++)
			{
				SerializedProperty serializedProperty4 = list[i];
				bool flag = serializedProperty4.propertyType == SerializedPropertyType.ObjectReference;
				bool flag2 = serializedProperty4.propertyType == SerializedPropertyType.Float;
				bool flag3 = serializedProperty4.propertyType == SerializedPropertyType.Boolean;
				bool flag4 = serializedProperty4.propertyType == SerializedPropertyType.Integer;
				if (flag || flag2 || flag3 || flag4)
				{
					SerializedObject serializedObject2 = serializedProperty4.serializedObject;
					UnityEngine.Object[] targetObjects = serializedObject2.targetObjects;
					if (serializedProperty4.hasMultipleDifferentValues)
					{
						for (int j = 0; j < targetObjects.Length; j++)
						{
							SerializedObject serializedObject3 = new SerializedObject(targetObjects[j]);
							SerializedProperty serializedProperty5 = serializedObject3.FindProperty(serializedProperty4.propertyPath);
							string value = string.Empty;
							UnityEngine.Object objectReference = null;
							if (flag)
							{
								objectReference = serializedProperty5.objectReferenceValue;
							}
							else if (flag2)
							{
								value = serializedProperty5.floatValue.ToString();
							}
							else if (flag4)
							{
								value = serializedProperty5.intValue.ToString();
							}
							else
							{
								value = serializedProperty5.boolValue.ToString();
							}
							list2.Add(new PropertyModification
							{
								target = targetObjects[j],
								propertyPath = serializedProperty5.propertyPath,
								value = value,
								objectReference = objectReference
							});
						}
					}
					else
					{
						string value2 = string.Empty;
						UnityEngine.Object objectReference2 = null;
						if (flag)
						{
							objectReference2 = serializedProperty4.objectReferenceValue;
						}
						else if (flag2)
						{
							value2 = serializedProperty4.floatValue.ToString();
						}
						else if (flag4)
						{
							value2 = serializedProperty4.intValue.ToString();
						}
						else
						{
							value2 = serializedProperty4.boolValue.ToString();
						}
						for (int k = 0; k < targetObjects.Length; k++)
						{
							list2.Add(new PropertyModification
							{
								target = targetObjects[k],
								propertyPath = serializedProperty4.propertyPath,
								value = value2,
								objectReference = objectReference2
							});
						}
					}
				}
			}
			return list2.ToArray();
		}

		public static EditorCurveBinding[] PropertyModificationsToEditorCurveBindings(PropertyModification[] modifications, GameObject rootGameObject, AnimationClip animationClip)
		{
			EditorCurveBinding[] result;
			if (modifications == null)
			{
				result = new EditorCurveBinding[0];
			}
			else
			{
				HashSet<EditorCurveBinding> hashSet = new HashSet<EditorCurveBinding>();
				for (int i = 0; i < modifications.Length; i++)
				{
					EditorCurveBinding editorCurveBinding = default(EditorCurveBinding);
					if (AnimationUtility.PropertyModificationToEditorCurveBinding(modifications[i], rootGameObject, out editorCurveBinding) != null)
					{
						EditorCurveBinding[] array = RotationCurveInterpolation.RemapAnimationBindingForAddKey(editorCurveBinding, animationClip);
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								hashSet.Add(array[j]);
							}
						}
						else
						{
							hashSet.Add(editorCurveBinding);
						}
					}
				}
				result = hashSet.ToArray<EditorCurveBinding>();
			}
			return result;
		}

		public static EditorCurveBinding[] SerializedPropertyToEditorCurveBindings(SerializedProperty property, GameObject rootGameObject, AnimationClip animationClip)
		{
			PropertyModification[] modifications = AnimationWindowUtility.SerializedPropertyToPropertyModifications(property);
			return AnimationWindowUtility.PropertyModificationsToEditorCurveBindings(modifications, rootGameObject, animationClip);
		}

		public static bool CurveExists(EditorCurveBinding binding, AnimationWindowCurve[] curves)
		{
			bool result;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (binding.propertyName == animationWindowCurve.binding.propertyName && binding.type == animationWindowCurve.binding.type && binding.path == animationWindowCurve.binding.path)
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public static EditorCurveBinding GetRenamedBinding(EditorCurveBinding binding, string newPath)
		{
			return new EditorCurveBinding
			{
				path = newPath,
				propertyName = binding.propertyName,
				type = binding.type
			};
		}

		public static void RenameCurvePath(AnimationWindowCurve curve, EditorCurveBinding newBinding, AnimationClip clip)
		{
			if (curve.isPPtrCurve)
			{
				AnimationUtility.SetObjectReferenceCurve(clip, curve.binding, null);
				AnimationUtility.SetObjectReferenceCurve(clip, newBinding, curve.ToObjectCurve());
			}
			else
			{
				AnimationUtility.SetEditorCurve(clip, curve.binding, null);
				AnimationUtility.SetEditorCurve(clip, newBinding, curve.ToAnimationCurve());
			}
		}

		public static string GetPropertyDisplayName(string propertyName)
		{
			propertyName = propertyName.Replace("m_LocalPosition", "Position");
			propertyName = propertyName.Replace("m_LocalScale", "Scale");
			propertyName = propertyName.Replace("m_LocalRotation", "Rotation");
			propertyName = propertyName.Replace("localEulerAnglesBaked", "Rotation");
			propertyName = propertyName.Replace("localEulerAnglesRaw", "Rotation");
			propertyName = propertyName.Replace("localEulerAngles", "Rotation");
			propertyName = propertyName.Replace("m_Materials.Array.data", "Material Reference");
			propertyName = ObjectNames.NicifyVariableName(propertyName);
			propertyName = propertyName.Replace("m_", "");
			return propertyName;
		}

		public static bool ShouldPrefixWithTypeName(Type animatableObjectType, string propertyName)
		{
			return animatableObjectType != typeof(Transform) && animatableObjectType != typeof(RectTransform) && (animatableObjectType != typeof(SpriteRenderer) || !(propertyName == "m_Sprite"));
		}

		public static string GetNicePropertyDisplayName(Type animatableObjectType, string propertyName)
		{
			string result;
			if (AnimationWindowUtility.ShouldPrefixWithTypeName(animatableObjectType, propertyName))
			{
				result = ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + AnimationWindowUtility.GetPropertyDisplayName(propertyName);
			}
			else
			{
				result = AnimationWindowUtility.GetPropertyDisplayName(propertyName);
			}
			return result;
		}

		public static string GetNicePropertyGroupDisplayName(Type animatableObjectType, string propertyGroupName)
		{
			string result;
			if (AnimationWindowUtility.ShouldPrefixWithTypeName(animatableObjectType, propertyGroupName))
			{
				result = ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + AnimationWindowUtility.NicifyPropertyGroupName(animatableObjectType, propertyGroupName);
			}
			else
			{
				result = AnimationWindowUtility.NicifyPropertyGroupName(animatableObjectType, propertyGroupName);
			}
			return result;
		}

		public static string NicifyPropertyGroupName(Type animatableObjectType, string propertyGroupName)
		{
			string text = AnimationWindowUtility.GetPropertyGroupName(AnimationWindowUtility.GetPropertyDisplayName(propertyGroupName));
			if (animatableObjectType == typeof(RectTransform) & text.Equals("Position"))
			{
				text = "Position (Z)";
			}
			return text;
		}

		public static int GetComponentIndex(string name)
		{
			int result;
			if (name == null || name.Length < 3 || name[name.Length - 2] != '.')
			{
				result = -1;
			}
			else
			{
				char c = name[name.Length - 1];
				switch (c)
				{
				case 'w':
					result = 3;
					break;
				case 'x':
					result = 0;
					break;
				case 'y':
					result = 1;
					break;
				case 'z':
					result = 2;
					break;
				default:
					if (c != 'a')
					{
						if (c != 'b')
						{
							if (c != 'g')
							{
								if (c != 'r')
								{
									result = -1;
								}
								else
								{
									result = 0;
								}
							}
							else
							{
								result = 1;
							}
						}
						else
						{
							result = 2;
						}
					}
					else
					{
						result = 3;
					}
					break;
				}
			}
			return result;
		}

		public static string GetPropertyGroupName(string propertyName)
		{
			string result;
			if (AnimationWindowUtility.GetComponentIndex(propertyName) != -1)
			{
				result = propertyName.Substring(0, propertyName.Length - 2);
			}
			else
			{
				result = propertyName;
			}
			return result;
		}

		public static float GetNextKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
		{
			float num = 3.40282347E+38f;
			float num2 = currentTime + 1f / frameRate;
			bool flag = false;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
				{
					float num3 = current.time + animationWindowCurve.timeOffset;
					if (num3 < num && num3 >= num2)
					{
						num = num3;
						flag = true;
					}
				}
			}
			return (!flag) ? currentTime : num;
		}

		public static float GetPreviousKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
		{
			float num = -3.40282347E+38f;
			float num2 = Mathf.Max(0f, currentTime - 1f / frameRate);
			bool flag = false;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
				{
					float num3 = current.time + animationWindowCurve.timeOffset;
					if (num3 > num && num3 <= num2)
					{
						num = num3;
						flag = true;
					}
				}
			}
			return (!flag) ? currentTime : num;
		}

		public static bool InitializeGameobjectForAnimation(GameObject animatedObject)
		{
			Component component = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
			bool result;
			if (component == null)
			{
				AnimationClip animationClip = AnimationWindowUtility.CreateNewClip(animatedObject.name);
				if (animationClip == null)
				{
					result = false;
				}
				else
				{
					component = AnimationWindowUtility.EnsureActiveAnimationPlayer(animatedObject);
					bool flag = AnimationWindowUtility.AddClipToAnimationPlayerComponent(component, animationClip);
					if (!flag)
					{
						UnityEngine.Object.DestroyImmediate(component);
					}
					result = flag;
				}
			}
			else
			{
				result = AnimationWindowUtility.EnsureAnimationPlayerHasClip(component);
			}
			return result;
		}

		public static Component EnsureActiveAnimationPlayer(GameObject animatedObject)
		{
			Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
			Component result;
			if (closestAnimationPlayerComponentInParents == null)
			{
				result = Undo.AddComponent<Animator>(animatedObject);
			}
			else
			{
				result = closestAnimationPlayerComponentInParents;
			}
			return result;
		}

		private static bool EnsureAnimationPlayerHasClip(Component animationPlayer)
		{
			bool result;
			if (animationPlayer == null)
			{
				result = false;
			}
			else if (AnimationUtility.GetAnimationClips(animationPlayer.gameObject).Length > 0)
			{
				result = true;
			}
			else
			{
				AnimationClip animationClip = AnimationWindowUtility.CreateNewClip(animationPlayer.gameObject.name);
				if (animationClip == null)
				{
					result = false;
				}
				else
				{
					AnimationMode.StopAnimationMode();
					result = AnimationWindowUtility.AddClipToAnimationPlayerComponent(animationPlayer, animationClip);
				}
			}
			return result;
		}

		public static bool AddClipToAnimationPlayerComponent(Component animationPlayer, AnimationClip newClip)
		{
			bool result;
			if (animationPlayer is Animator)
			{
				result = AnimationWindowUtility.AddClipToAnimatorComponent(animationPlayer as Animator, newClip);
			}
			else
			{
				result = (animationPlayer is Animation && AnimationWindowUtility.AddClipToAnimationComponent(animationPlayer as Animation, newClip));
			}
			return result;
		}

		public static bool AddClipToAnimatorComponent(Animator animator, AnimationClip newClip)
		{
			UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.GetEffectiveAnimatorController(animator);
			bool result;
			if (animatorController == null)
			{
				animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerForClip(newClip, animator.gameObject);
				UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
				result = (animatorController != null);
			}
			else
			{
				ChildAnimatorState childAnimatorState = animatorController.layers[0].stateMachine.FindState(newClip.name);
				if (childAnimatorState.Equals(default(ChildAnimatorState)))
				{
					animatorController.AddMotion(newClip);
				}
				else if (childAnimatorState.state && childAnimatorState.state.motion == null)
				{
					childAnimatorState.state.motion = newClip;
				}
				else if (childAnimatorState.state && childAnimatorState.state.motion != newClip)
				{
					animatorController.AddMotion(newClip);
				}
				result = true;
			}
			return result;
		}

		public static bool AddClipToAnimationComponent(Animation animation, AnimationClip newClip)
		{
			AnimationWindowUtility.SetClipAsLegacy(newClip);
			animation.AddClip(newClip, newClip.name);
			return true;
		}

		internal static AnimationClip CreateNewClip(string gameObjectName)
		{
			string message = string.Format("Create a new animation for the game object '{0}':", gameObjectName);
			string path = ProjectWindowUtil.GetActiveFolderPath();
			if (AnimationWindowUtility.s_LastPathUsedForNewClip != null)
			{
				string directoryName = Path.GetDirectoryName(AnimationWindowUtility.s_LastPathUsedForNewClip);
				if (directoryName != null && Directory.Exists(directoryName))
				{
					path = directoryName;
				}
			}
			string text = EditorUtility.SaveFilePanelInProject("Create New Animation", "New Animation", "anim", message, path);
			AnimationClip result;
			if (text == "")
			{
				result = null;
			}
			else
			{
				result = AnimationWindowUtility.CreateNewClipAtPath(text);
			}
			return result;
		}

		internal static AnimationClip CreateNewClipAtPath(string clipPath)
		{
			AnimationWindowUtility.s_LastPathUsedForNewClip = clipPath;
			AnimationClip animationClip = new AnimationClip();
			AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
			animationClipSettings.loopTime = true;
			AnimationUtility.SetAnimationClipSettingsNoDirty(animationClip, animationClipSettings);
			AnimationClip animationClip2 = AssetDatabase.LoadMainAssetAtPath(clipPath) as AnimationClip;
			AnimationClip result;
			if (animationClip2)
			{
				EditorUtility.CopySerialized(animationClip, animationClip2);
				AssetDatabase.SaveAssets();
				UnityEngine.Object.DestroyImmediate(animationClip);
				result = animationClip2;
			}
			else
			{
				AssetDatabase.CreateAsset(animationClip, clipPath);
				result = animationClip;
			}
			return result;
		}

		private static void SetClipAsLegacy(AnimationClip clip)
		{
			SerializedObject serializedObject = new SerializedObject(clip);
			serializedObject.FindProperty("m_Legacy").boolValue = true;
			serializedObject.ApplyModifiedProperties();
		}

		internal static AnimationClip AllocateAndSetupClip(bool useAnimator)
		{
			AnimationClip animationClip = new AnimationClip();
			if (useAnimator)
			{
				AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
				animationClipSettings.loopTime = true;
				AnimationUtility.SetAnimationClipSettingsNoDirty(animationClip, animationClipSettings);
			}
			return animationClip;
		}

		public static int GetPropertyNodeID(int setId, string path, Type type, string propertyName)
		{
			return (setId.ToString() + path + type.Name + propertyName).GetHashCode();
		}

		public static Component GetClosestAnimationPlayerComponentInParents(Transform tr)
		{
			Animator closestAnimatorInParents = AnimationWindowUtility.GetClosestAnimatorInParents(tr);
			Component result;
			if (closestAnimatorInParents != null)
			{
				result = closestAnimatorInParents;
			}
			else
			{
				Animation closestAnimationInParents = AnimationWindowUtility.GetClosestAnimationInParents(tr);
				if (closestAnimationInParents != null)
				{
					result = closestAnimationInParents;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static Animator GetClosestAnimatorInParents(Transform tr)
		{
			Animator result;
			while (!(tr.GetComponent<Animator>() != null))
			{
				if (tr == tr.root)
				{
					result = null;
					return result;
				}
				tr = tr.parent;
			}
			result = tr.GetComponent<Animator>();
			return result;
		}

		public static Animation GetClosestAnimationInParents(Transform tr)
		{
			Animation result;
			while (!(tr.GetComponent<Animation>() != null))
			{
				if (tr == tr.root)
				{
					result = null;
					return result;
				}
				tr = tr.parent;
			}
			result = tr.GetComponent<Animation>();
			return result;
		}

		public static void SyncTimeArea(TimeArea from, TimeArea to)
		{
			to.SetDrawRectHack(from.drawRect);
			to.m_Scale = new Vector2(from.m_Scale.x, to.m_Scale.y);
			to.m_Translation = new Vector2(from.m_Translation.x, to.m_Translation.y);
			to.EnforceScaleAndRange();
		}

		public static void DrawInRangeOverlay(Rect rect, Color color, float startOfClipPixel, float endOfClipPixel)
		{
			if (endOfClipPixel >= rect.xMin)
			{
				if (color.a > 0f)
				{
					Rect rect2 = Rect.MinMaxRect(Mathf.Max(startOfClipPixel, rect.xMin), rect.yMin, Mathf.Min(endOfClipPixel, rect.xMax), rect.yMax);
					AnimationWindowUtility.DrawRect(rect2, color);
				}
			}
		}

		public static void DrawOutOfRangeOverlay(Rect rect, Color color, float startOfClipPixel, float endOfClipPixel)
		{
			Color color2 = Color.white.RGBMultiplied(0.4f);
			if (startOfClipPixel > rect.xMin)
			{
				Rect rect2 = Rect.MinMaxRect(rect.xMin, rect.yMin, Mathf.Min(startOfClipPixel, rect.xMax), rect.yMax);
				AnimationWindowUtility.DrawRect(rect2, color);
				TimeArea.DrawVerticalLine(rect2.xMax, rect2.yMin, rect2.yMax, color2);
			}
			Rect rect3 = Rect.MinMaxRect(Mathf.Max(endOfClipPixel, rect.xMin), rect.yMin, rect.xMax, rect.yMax);
			AnimationWindowUtility.DrawRect(rect3, color);
			TimeArea.DrawVerticalLine(rect3.xMin, rect3.yMin, rect3.yMax, color2);
		}

		public static void DrawSelectionOverlay(Rect rect, Color color, float startPixel, float endPixel)
		{
			startPixel = Mathf.Max(startPixel, rect.xMin);
			endPixel = Mathf.Max(endPixel, rect.xMin);
			Rect rect2 = Rect.MinMaxRect(startPixel, rect.yMin, endPixel, rect.yMax);
			AnimationWindowUtility.DrawRect(rect2, color);
		}

		public static void DrawRect(Rect rect, Color color)
		{
			if (Event.current.type == EventType.Repaint)
			{
				HandleUtility.ApplyWireMaterial();
				GL.PushMatrix();
				GL.MultMatrix(Handles.matrix);
				GL.Begin(7);
				GL.Color(color);
				GL.Vertex(rect.min);
				GL.Vertex(new Vector2(rect.xMax, rect.yMin));
				GL.Vertex(rect.max);
				GL.Vertex(new Vector2(rect.xMin, rect.yMax));
				GL.End();
				GL.PopMatrix();
			}
		}

		public static CurveWrapper GetCurveWrapper(AnimationWindowCurve curve, AnimationClip clip)
		{
			CurveWrapper curveWrapper = new CurveWrapper();
			curveWrapper.renderer = new NormalCurveRenderer(curve.ToAnimationCurve());
			curveWrapper.renderer.SetWrap(WrapMode.Once, (!clip.isLooping) ? WrapMode.Once : WrapMode.Loop);
			curveWrapper.renderer.SetCustomRange(clip.startTime, clip.stopTime);
			curveWrapper.binding = curve.binding;
			curveWrapper.id = curve.GetHashCode();
			curveWrapper.color = CurveUtility.GetPropertyColor(curve.propertyName);
			curveWrapper.hidden = false;
			curveWrapper.selectionBindingInterface = curve.selectionBinding;
			return curveWrapper;
		}

		public static AnimationWindowKeyframe CurveSelectionToAnimationWindowKeyframe(CurveSelection curveSelection, List<AnimationWindowCurve> allCurves)
		{
			AnimationWindowKeyframe result;
			foreach (AnimationWindowCurve current in allCurves)
			{
				int hashCode = current.GetHashCode();
				if (hashCode == curveSelection.curveID && current.m_Keyframes.Count > curveSelection.key)
				{
					result = current.m_Keyframes[curveSelection.key];
					return result;
				}
			}
			result = null;
			return result;
		}

		public static CurveSelection AnimationWindowKeyframeToCurveSelection(AnimationWindowKeyframe keyframe, CurveEditor curveEditor)
		{
			int hashCode = keyframe.curve.GetHashCode();
			CurveWrapper[] animationCurves = curveEditor.animationCurves;
			CurveSelection result;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				if (curveWrapper.id == hashCode && keyframe.GetIndex() >= 0)
				{
					result = new CurveSelection(curveWrapper.id, keyframe.GetIndex());
					return result;
				}
			}
			result = null;
			return result;
		}

		public static AnimationWindowCurve BestMatchForPaste(EditorCurveBinding binding, List<AnimationWindowCurve> clipboardCurves, List<AnimationWindowCurve> targetCurves)
		{
			AnimationWindowCurve result;
			foreach (AnimationWindowCurve current in targetCurves)
			{
				if (current.binding == binding)
				{
					result = current;
					return result;
				}
			}
			using (List<AnimationWindowCurve>.Enumerator enumerator2 = targetCurves.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					AnimationWindowCurve targetCurve = enumerator2.Current;
					if (targetCurve.binding.propertyName == binding.propertyName)
					{
						if (!clipboardCurves.Exists((AnimationWindowCurve clipboardCurve) => clipboardCurve.binding == targetCurve.binding))
						{
							result = targetCurve;
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}

		internal static Rect FromToRect(Vector2 start, Vector2 end)
		{
			Rect result = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
			if (result.width < 0f)
			{
				result.x += result.width;
				result.width = -result.width;
			}
			if (result.height < 0f)
			{
				result.y += result.height;
				result.height = -result.height;
			}
			return result;
		}

		public static bool IsTransformType(Type type)
		{
			return type == typeof(Transform) || type == typeof(RectTransform);
		}

		public static bool ForceGrouping(EditorCurveBinding binding)
		{
			bool result;
			if (binding.type == typeof(Transform))
			{
				result = true;
			}
			else if (binding.type == typeof(RectTransform))
			{
				string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(binding.propertyName);
				result = (propertyGroupName == "m_LocalPosition" || propertyGroupName == "m_LocalScale" || propertyGroupName == "m_LocalRotation" || propertyGroupName == "localEulerAnglesBaked" || propertyGroupName == "localEulerAngles" || propertyGroupName == "localEulerAnglesRaw");
			}
			else if (typeof(Renderer).IsAssignableFrom(binding.type))
			{
				string propertyGroupName2 = AnimationWindowUtility.GetPropertyGroupName(binding.propertyName);
				result = (propertyGroupName2 == "material._Color");
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static void ControllerChanged()
		{
			foreach (AnimationWindow current in AnimationWindow.GetAllAnimationWindows())
			{
				current.OnControllerChange();
			}
		}
	}
}
