using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityEditorInternal
{
	internal static class AnimationWindowUtility
	{
		public static void CreateDefaultCurves(AnimationWindowState state, EditorCurveBinding[] properties)
		{
			AnimationClip activeAnimationClip = state.m_ActiveAnimationClip;
			GameObject rootGameObject = state.m_RootGameObject;
			properties = RotationCurveInterpolation.ConvertRotationPropertiesToDefaultInterpolation(state.m_ActiveAnimationClip, properties);
			EditorCurveBinding[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding binding = array[i];
				state.SaveCurve(AnimationWindowUtility.CreateDefaultCurve(activeAnimationClip, rootGameObject, binding));
			}
		}
		public static AnimationWindowCurve CreateDefaultCurve(AnimationClip clip, GameObject rootGameObject, EditorCurveBinding binding)
		{
			Type editorCurveValueType = AnimationUtility.GetEditorCurveValueType(rootGameObject, binding);
			AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(clip, binding, editorCurveValueType);
			object currentValue = AnimationWindowUtility.GetCurrentValue(rootGameObject, binding);
			if (clip.length == 0f)
			{
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, clip.frameRate));
			}
			else
			{
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, clip.frameRate));
				AnimationWindowUtility.AddKeyframeToCurve(animationWindowCurve, currentValue, editorCurveValueType, AnimationKeyTime.Time(clip.length, clip.frameRate));
			}
			return animationWindowCurve;
		}
		public static bool ShouldShowAnimationWindowCurve(EditorCurveBinding curveBinding)
		{
			return !AnimationWindowUtility.IsTransformType(curveBinding.type) || !curveBinding.propertyName.EndsWith(".w");
		}
		public static bool IsNodeLeftOverCurve(AnimationWindowHierarchyNode node, GameObject rootGameObject)
		{
			if (rootGameObject == null)
			{
				return false;
			}
			EditorCurveBinding? binding = node.binding;
			if (binding.HasValue)
			{
				EditorCurveBinding? binding2 = node.binding;
				return AnimationUtility.GetEditorCurveValueType(rootGameObject, binding2.Value) == null;
			}
			if (node.hasChildren)
			{
				using (List<TreeViewItem>.Enumerator enumerator = node.children.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TreeViewItem current = enumerator.Current;
						return AnimationWindowUtility.IsNodeLeftOverCurve(current as AnimationWindowHierarchyNode, rootGameObject);
					}
				}
				return false;
			}
			return false;
		}
		public static bool IsNodeAmbiguous(AnimationWindowHierarchyNode node, GameObject rootGameObject)
		{
			if (rootGameObject == null)
			{
				return false;
			}
			EditorCurveBinding? binding = node.binding;
			if (binding.HasValue)
			{
				return AnimationUtility.AmbiguousBinding(node.binding.Value.path, node.binding.Value.m_ClassID, rootGameObject.transform);
			}
			if (node.hasChildren)
			{
				using (List<TreeViewItem>.Enumerator enumerator = node.children.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						TreeViewItem current = enumerator.Current;
						return AnimationWindowUtility.IsNodeAmbiguous(current as AnimationWindowHierarchyNode, rootGameObject);
					}
				}
				return false;
			}
			return false;
		}
		public static void AddSelectedKeyframes(AnimationWindowState state, AnimationKeyTime time)
		{
			foreach (AnimationWindowCurve current in state.activeCurves)
			{
				AnimationWindowUtility.AddKeyframeToCurve(state, current, time);
			}
		}
		public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowState state, AnimationWindowCurve curve, AnimationKeyTime time)
		{
			object currentValue = AnimationWindowUtility.GetCurrentValue(state.m_RootGameObject, curve.binding);
			Type editorCurveValueType = AnimationUtility.GetEditorCurveValueType(state.m_RootGameObject, curve.binding);
			AnimationWindowKeyframe result = AnimationWindowUtility.AddKeyframeToCurve(curve, currentValue, editorCurveValueType, time);
			state.SaveCurve(curve);
			return result;
		}
		public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowCurve curve, object value, Type type, AnimationKeyTime time)
		{
			AnimationWindowKeyframe animationWindowKeyframe = curve.FindKeyAtTime(time);
			if (animationWindowKeyframe != null)
			{
				animationWindowKeyframe.value = value;
				return animationWindowKeyframe;
			}
			AnimationWindowKeyframe animationWindowKeyframe2 = new AnimationWindowKeyframe();
			animationWindowKeyframe2.time = time.time;
			if (curve.isPPtrCurve)
			{
				animationWindowKeyframe2.value = value;
				animationWindowKeyframe2.curve = curve;
				curve.AddKeyframe(animationWindowKeyframe2, time);
			}
			else
			{
				if (type == typeof(bool) || type == typeof(float))
				{
					AnimationCurve animationCurve = curve.ToAnimationCurve();
					Keyframe key = new Keyframe(time.time, (float)value);
					if (type == typeof(bool))
					{
						CurveUtility.SetKeyTangentMode(ref key, 0, TangentMode.Stepped);
						CurveUtility.SetKeyTangentMode(ref key, 1, TangentMode.Stepped);
						CurveUtility.SetKeyBroken(ref key, true);
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
					animationWindowKeyframe2.value = value;
					animationWindowKeyframe2.curve = curve;
					curve.AddKeyframe(animationWindowKeyframe2, time);
				}
			}
			return animationWindowKeyframe2;
		}
		public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, bool entireHierarchy)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (animationWindowCurve.path.Equals(path) || (entireHierarchy && animationWindowCurve.path.Contains(path)))
				{
					list.Add(animationWindowCurve);
				}
			}
			return list;
		}
		public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, Type animatableObjectType)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (animationWindowCurve.path.Equals(path) && animationWindowCurve.type == animatableObjectType)
				{
					list.Add(animationWindowCurve);
				}
			}
			return list;
		}
		public static bool IsCurveCreated(AnimationClip clip, EditorCurveBinding binding)
		{
			if (binding.isPPtrCurve)
			{
				return AnimationUtility.GetObjectReferenceCurve(clip, binding) != null;
			}
			if (AnimationWindowUtility.IsRectTransformPosition(binding))
			{
				binding.propertyName = binding.propertyName.Replace(".x", ".z").Replace(".y", ".z");
			}
			return AnimationUtility.GetEditorCurve(clip, binding) != null;
		}
		public static bool IsRectTransformPosition(EditorCurveBinding curveBinding)
		{
			return curveBinding.type == typeof(RectTransform) && AnimationWindowUtility.GetPropertyGroupName(curveBinding.propertyName) == "m_LocalPosition";
		}
		public static bool ContainsFloatKeyframes(List<AnimationWindowKeyframe> keyframes)
		{
			if (keyframes == null || keyframes.Count == 0)
			{
				return false;
			}
			foreach (AnimationWindowKeyframe current in keyframes)
			{
				if (!current.isPPtrCurve)
				{
					return true;
				}
			}
			return false;
		}
		public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, Type animatableObjectType, string propertyName)
		{
			List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
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
			return list;
		}
		public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
		{
			if (curveBinding.isPPtrCurve)
			{
				UnityEngine.Object result;
				AnimationUtility.GetObjectReferenceValue(rootGameObject, curveBinding, out result);
				return result;
			}
			float num;
			AnimationUtility.GetFloatValue(rootGameObject, curveBinding, out num);
			return num;
		}
		public static List<EditorCurveBinding> GetAnimatableProperties(GameObject root, GameObject gameObject, Type valueType)
		{
			EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(root, gameObject);
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
		public static List<EditorCurveBinding> GetAnimatableProperties(GameObject root, GameObject gameObject, Type objectType, Type valueType)
		{
			EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(root, gameObject);
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
		public static bool CurveExists(EditorCurveBinding binding, AnimationWindowCurve[] curves)
		{
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				if (binding.propertyName == animationWindowCurve.binding.propertyName && binding.type == animationWindowCurve.binding.type && binding.path == animationWindowCurve.binding.path)
				{
					return true;
				}
			}
			return false;
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
			AnimationUtility.SetEditorCurve(clip, curve.binding, null);
			AnimationUtility.SetEditorCurve(clip, newBinding, curve.ToAnimationCurve());
		}
		public static string GetPropertyDisplayName(string propertyName)
		{
			propertyName = propertyName.Replace("m_LocalPosition", "Position");
			propertyName = propertyName.Replace("m_LocalScale", "Scale");
			propertyName = propertyName.Replace("m_LocalRotation", "Rotation");
			propertyName = propertyName.Replace("localEulerAnglesBaked", "Rotation");
			propertyName = propertyName.Replace("localEulerAngles", "Rotation");
			propertyName = propertyName.Replace("m_Materials.Array.data", "Material Reference");
			propertyName = ObjectNames.NicifyVariableName(propertyName);
			propertyName = propertyName.Replace("m_", string.Empty);
			return propertyName;
		}
		public static bool ShouldPrefixWithTypeName(Type animatableObjectType, string propertyName)
		{
			return animatableObjectType != typeof(Transform) && animatableObjectType != typeof(RectTransform) && (animatableObjectType != typeof(SpriteRenderer) || !(propertyName == "m_Sprite"));
		}
		public static string GetNicePropertyDisplayName(Type animatableObjectType, string propertyName)
		{
			if (AnimationWindowUtility.ShouldPrefixWithTypeName(animatableObjectType, propertyName))
			{
				return ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + AnimationWindowUtility.GetPropertyDisplayName(propertyName);
			}
			return AnimationWindowUtility.GetPropertyDisplayName(propertyName);
		}
		public static string GetNicePropertyGroupDisplayName(Type animatableObjectType, string propertyGroupName)
		{
			if (AnimationWindowUtility.ShouldPrefixWithTypeName(animatableObjectType, propertyGroupName))
			{
				return ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + AnimationWindowUtility.NicifyPropertyGroupName(animatableObjectType, propertyGroupName);
			}
			return AnimationWindowUtility.NicifyPropertyGroupName(animatableObjectType, propertyGroupName);
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
			if (name.Length < 3 || name[name.Length - 2] != '.')
			{
				return -1;
			}
			char c = name[name.Length - 1];
			char c2 = c;
			switch (c2)
			{
			case 'r':
				return 0;
			case 's':
			case 't':
			case 'u':
			case 'v':
				IL_61:
				if (c2 == 'a')
				{
					return 3;
				}
				if (c2 == 'b')
				{
					return 2;
				}
				if (c2 != 'g')
				{
					return -1;
				}
				return 1;
			case 'w':
				return 3;
			case 'x':
				return 0;
			case 'y':
				return 1;
			case 'z':
				return 2;
			}
			goto IL_61;
		}
		public static string GetPropertyGroupName(string propertyName)
		{
			if (AnimationWindowUtility.GetComponentIndex(propertyName) != -1)
			{
				return propertyName.Substring(0, propertyName.Length - 2);
			}
			return propertyName;
		}
		public static float GetNextKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
		{
			float num = 3.40282347E+38f;
			float val = currentTime + 1f / frameRate;
			bool flag = false;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
				{
					if (current.time < num && current.time > currentTime)
					{
						num = Math.Max(current.time, val);
						flag = true;
					}
				}
			}
			return (!flag) ? currentTime : num;
		}
		public static float GetPreviousKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
		{
			float num = -3.40282347E+38f;
			float b = Mathf.Max(0f, currentTime - 1f / frameRate);
			bool flag = false;
			for (int i = 0; i < curves.Length; i++)
			{
				AnimationWindowCurve animationWindowCurve = curves[i];
				foreach (AnimationWindowKeyframe current in animationWindowCurve.m_Keyframes)
				{
					if (current.time > num && current.time < currentTime)
					{
						num = Mathf.Min(current.time, b);
						flag = true;
					}
				}
			}
			return (!flag) ? currentTime : num;
		}
		public static bool GameObjectIsAnimatable(GameObject gameObject, AnimationClip animationClip)
		{
			return !(gameObject == null) && (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.None && !EditorUtility.IsPersistent(gameObject) && (!(animationClip != null) || ((animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None && AssetDatabase.IsOpenForEdit(animationClip)));
		}
		public static int GetPropertyNodeID(string path, Type type, string propertyName)
		{
			return (path + type.Name + propertyName).GetHashCode();
		}
		public static Transform GetClosestAnimationComponentInParents(Transform tr)
		{
			while (!tr.GetComponent<Animation>() && !tr.GetComponent<Animator>())
			{
				if (tr == tr.root)
				{
					return null;
				}
				tr = tr.parent;
			}
			return tr;
		}
		public static bool IsTransformType(Type type)
		{
			return type == typeof(Transform) || type == typeof(RectTransform);
		}
		public static bool ForceGrouping(EditorCurveBinding binding)
		{
			if (binding.type == typeof(Transform))
			{
				return true;
			}
			if (binding.type == typeof(RectTransform))
			{
				string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(binding.propertyName);
				return propertyGroupName == "m_LocalPosition" || propertyGroupName == "m_LocalScale" || propertyGroupName == "m_LocalRotation" || propertyGroupName == "localEulerAnglesBaked" || propertyGroupName == "localEulerAngles";
			}
			return false;
		}
	}
}
