using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace UnityEditorInternal
{
	internal static class AnimationWindowUtility
	{
		internal static string s_LastPathUsedForNewClip;

		public static void CreateDefaultCurves(IAnimationRecordingState state, EditorCurveBinding[] properties)
		{
			AnimationClip activeAnimationClip = state.activeAnimationClip;
			GameObject activeRootGameObject = state.activeRootGameObject;
			properties = RotationCurveInterpolation.ConvertRotationPropertiesToDefaultInterpolation(activeAnimationClip, properties);
			EditorCurveBinding[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				EditorCurveBinding binding = array[i];
				state.SaveCurve(AnimationWindowUtility.CreateDefaultCurve(activeAnimationClip, activeRootGameObject, binding));
			}
		}

		public static AnimationWindowCurve CreateDefaultCurve(AnimationClip clip, GameObject rootGameObject, EditorCurveBinding binding)
		{
			Type editorCurveValueType = CurveBindingUtility.GetEditorCurveValueType(rootGameObject, binding);
			AnimationWindowCurve animationWindowCurve = new AnimationWindowCurve(clip, binding, editorCurveValueType);
			object currentValue = CurveBindingUtility.GetCurrentValue(rootGameObject, binding);
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
			List<AnimationWindowCurve> list = (state.activeCurves.Count <= 0) ? state.allCurves : state.activeCurves;
			foreach (AnimationWindowCurve current in list)
			{
				if (current.animationIsEditable)
				{
					AnimationWindowUtility.AddKeyframeToCurve(state, current, AnimationKeyTime.Time(time.time - current.timeOffset, time.frameRate));
				}
			}
		}

		public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowState state, AnimationWindowCurve curve, AnimationKeyTime time)
		{
			object currentValue = CurveBindingUtility.GetCurrentValue(state.activeRootGameObject, curve.binding);
			Type editorCurveValueType = CurveBindingUtility.GetEditorCurveValueType(state.activeRootGameObject, curve.binding);
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
			else if (type == typeof(bool) || type == typeof(float))
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
			return animationWindowKeyframe2;
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
			if (binding.isPPtrCurve)
			{
				return AnimationUtility.GetObjectReferenceCurve(clip, binding) != null;
			}
			if (AnimationWindowUtility.IsRectTransformPosition(binding))
			{
				binding.propertyName = binding.propertyName.Replace(".x", ".z").Replace(".y", ".z");
			}
			if (AnimationWindowUtility.IsRotationCurve(binding))
			{
				return AnimationUtility.GetEditorCurve(clip, binding) != null || AnimationWindowUtility.HasOtherRotationCurve(clip, binding);
			}
			return AnimationUtility.GetEditorCurve(clip, binding) != null;
		}

		internal static bool HasOtherRotationCurve(AnimationClip clip, EditorCurveBinding rotationBinding)
		{
			if (rotationBinding.propertyName.StartsWith("m_LocalRotation"))
			{
				EditorCurveBinding binding = rotationBinding;
				EditorCurveBinding binding2 = rotationBinding;
				EditorCurveBinding binding3 = rotationBinding;
				binding.propertyName = "localEulerAnglesRaw.x";
				binding2.propertyName = "localEulerAnglesRaw.y";
				binding3.propertyName = "localEulerAnglesRaw.z";
				return AnimationUtility.GetEditorCurve(clip, binding) != null || AnimationUtility.GetEditorCurve(clip, binding2) != null || AnimationUtility.GetEditorCurve(clip, binding3) != null;
			}
			EditorCurveBinding binding4 = rotationBinding;
			EditorCurveBinding binding5 = rotationBinding;
			EditorCurveBinding binding6 = rotationBinding;
			EditorCurveBinding binding7 = rotationBinding;
			binding4.propertyName = "m_LocalRotation.x";
			binding5.propertyName = "m_LocalRotation.y";
			binding6.propertyName = "m_LocalRotation.z";
			binding7.propertyName = "m_LocalRotation.w";
			return AnimationUtility.GetEditorCurve(clip, binding4) != null || AnimationUtility.GetEditorCurve(clip, binding5) != null || AnimationUtility.GetEditorCurve(clip, binding6) != null || AnimationUtility.GetEditorCurve(clip, binding7) != null;
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
			propertyName = propertyName.Replace("localEulerAnglesRaw", "Rotation");
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
			if (name == null || name.Length < 3 || name[name.Length - 2] != '.')
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
				IL_67:
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
			goto IL_67;
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

		public static bool GameObjectIsAnimatable(GameObject gameObject, AnimationClip animationClip)
		{
			return !(gameObject == null) && (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.None && !EditorUtility.IsPersistent(gameObject) && (!(animationClip != null) || ((animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None && AssetDatabase.IsOpenForEdit(animationClip)));
		}

		public static bool InitializeGameobjectForAnimation(GameObject animatedObject)
		{
			Component component = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
			if (!(component == null))
			{
				return AnimationWindowUtility.EnsureAnimationPlayerHasClip(component);
			}
			AnimationClip animationClip = AnimationWindowUtility.CreateNewClip(animatedObject.name);
			if (animationClip == null)
			{
				return false;
			}
			component = AnimationWindowUtility.EnsureActiveAnimationPlayer(animatedObject);
			bool flag = AnimationWindowUtility.AddClipToAnimationPlayerComponent(component, animationClip);
			if (!flag)
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
			return flag;
		}

		public static Component EnsureActiveAnimationPlayer(GameObject animatedObject)
		{
			Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
			if (closestAnimationPlayerComponentInParents == null)
			{
				return Undo.AddComponent<Animator>(animatedObject);
			}
			return closestAnimationPlayerComponentInParents;
		}

		private static bool EnsureAnimationPlayerHasClip(Component animationPlayer)
		{
			if (animationPlayer == null)
			{
				return false;
			}
			if (AnimationUtility.GetAnimationClips(animationPlayer.gameObject).Length > 0)
			{
				return true;
			}
			AnimationClip animationClip = AnimationWindowUtility.CreateNewClip(animationPlayer.gameObject.name);
			if (animationClip == null)
			{
				return false;
			}
			AnimationMode.StopAnimationMode();
			return AnimationWindowUtility.AddClipToAnimationPlayerComponent(animationPlayer, animationClip);
		}

		public static bool AddClipToAnimationPlayerComponent(Component animationPlayer, AnimationClip newClip)
		{
			if (animationPlayer is Animator)
			{
				return AnimationWindowUtility.AddClipToAnimatorComponent(animationPlayer as Animator, newClip);
			}
			return animationPlayer is Animation && AnimationWindowUtility.AddClipToAnimationComponent(animationPlayer as Animation, newClip);
		}

		public static bool AddClipToAnimatorComponent(Animator animator, AnimationClip newClip)
		{
			UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.GetEffectiveAnimatorController(animator);
			if (animatorController == null)
			{
				animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerForClip(newClip, animator.gameObject);
				UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
				return animatorController != null;
			}
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
			return true;
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
			if (text == string.Empty)
			{
				return null;
			}
			return AnimationWindowUtility.CreateNewClipAtPath(text);
		}

		internal static AnimationClip CreateNewClipAtPath(string clipPath)
		{
			AnimationWindowUtility.s_LastPathUsedForNewClip = clipPath;
			AnimationClip animationClip = new AnimationClip();
			AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
			animationClipSettings.loopTime = true;
			AnimationUtility.SetAnimationClipSettingsNoDirty(animationClip, animationClipSettings);
			AnimationClip animationClip2 = AssetDatabase.LoadMainAssetAtPath(clipPath) as AnimationClip;
			if (animationClip2)
			{
				EditorUtility.CopySerialized(animationClip, animationClip2);
				AssetDatabase.SaveAssets();
				UnityEngine.Object.DestroyImmediate(animationClip);
				return animationClip2;
			}
			AssetDatabase.CreateAsset(animationClip, clipPath);
			return animationClip;
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
			if (closestAnimatorInParents != null)
			{
				return closestAnimatorInParents;
			}
			Animation closestAnimationInParents = AnimationWindowUtility.GetClosestAnimationInParents(tr);
			if (closestAnimationInParents != null)
			{
				return closestAnimationInParents;
			}
			return null;
		}

		public static Animator GetClosestAnimatorInParents(Transform tr)
		{
			while (!(tr.GetComponent<Animator>() != null))
			{
				if (tr == tr.root)
				{
					return null;
				}
				tr = tr.parent;
			}
			return tr.GetComponent<Animator>();
		}

		public static Animation GetClosestAnimationInParents(Transform tr)
		{
			while (!(tr.GetComponent<Animation>() != null))
			{
				if (tr == tr.root)
				{
					return null;
				}
				tr = tr.parent;
			}
			return tr.GetComponent<Animation>();
		}

		public static void SyncTimeArea(TimeArea from, TimeArea to)
		{
			to.SetDrawRectHack(from.drawRect);
			to.m_Scale = new Vector2(from.m_Scale.x, to.m_Scale.y);
			to.m_Translation = new Vector2(from.m_Translation.x, to.m_Translation.y);
			to.EnforceScaleAndRange();
		}

		public static void DrawEndOfClip(Rect rect, float endOfClipPixel)
		{
			Rect rect2 = new Rect(Mathf.Max(endOfClipPixel, rect.xMin), rect.yMin, rect.width, rect.height);
			Vector3[] array = new Vector3[]
			{
				new Vector3(rect2.xMin, rect2.yMin),
				new Vector3(rect2.xMax, rect2.yMin),
				new Vector3(rect2.xMax, rect2.yMax),
				new Vector3(rect2.xMin, rect2.yMax)
			};
			Color color = (!EditorGUIUtility.isProSkin) ? Color.gray.AlphaMultiplied(0.32f) : Color.gray.RGBMultiplied(0.3f).AlphaMultiplied(0.5f);
			Color color2 = (!EditorGUIUtility.isProSkin) ? Color.white.RGBMultiplied(0.4f) : Color.white.RGBMultiplied(0.4f);
			AnimationWindowUtility.DrawRect(array, color);
			TimeArea.DrawVerticalLine(array[0].x, array[0].y, array[3].y, color2);
			AnimationWindowUtility.DrawLine(array[0], array[3] + new Vector3(0f, -1f, 0f), color2);
		}

		public static void DrawRangeOfClip(Rect rect, float startOfClipPixel, float endOfClipPixel)
		{
			Color color = (!EditorGUIUtility.isProSkin) ? Color.gray.AlphaMultiplied(0.32f) : Color.gray.RGBMultiplied(0.3f).AlphaMultiplied(0.5f);
			Color color2 = (!EditorGUIUtility.isProSkin) ? Color.white.RGBMultiplied(0.4f) : Color.white.RGBMultiplied(0.4f);
			if (startOfClipPixel > rect.xMin)
			{
				Rect rect2 = new Rect(rect.xMin, rect.yMin, Mathf.Min(startOfClipPixel - rect.xMin, rect.width), rect.height);
				Vector3[] array = new Vector3[]
				{
					new Vector3(rect2.xMin, rect2.yMin),
					new Vector3(rect2.xMax, rect2.yMin),
					new Vector3(rect2.xMax, rect2.yMax),
					new Vector3(rect2.xMin, rect2.yMax)
				};
				AnimationWindowUtility.DrawRect(array, color);
				TimeArea.DrawVerticalLine(array[1].x, array[1].y, array[2].y, color2);
				AnimationWindowUtility.DrawLine(array[1], array[2] + new Vector3(0f, -1f, 0f), color2);
			}
			Rect rect3 = new Rect(Mathf.Max(endOfClipPixel, rect.xMin), rect.yMin, rect.width, rect.height);
			Vector3[] array2 = new Vector3[]
			{
				new Vector3(rect3.xMin, rect3.yMin),
				new Vector3(rect3.xMax, rect3.yMin),
				new Vector3(rect3.xMax, rect3.yMax),
				new Vector3(rect3.xMin, rect3.yMax)
			};
			AnimationWindowUtility.DrawRect(array2, color);
			TimeArea.DrawVerticalLine(array2[0].x, array2[0].y, array2[3].y, color2);
			AnimationWindowUtility.DrawLine(array2[0], array2[3] + new Vector3(0f, -1f, 0f), color2);
		}

		public static void DrawPlayHead(float positionX, float minY, float maxY, float alpha)
		{
			TimeArea.DrawVerticalLine(positionX, minY, maxY, Color.red.AlphaMultiplied(alpha));
		}

		public static void DrawVerticalSplitLine(Vector2 start, Vector2 end)
		{
			TimeArea.DrawVerticalLine(start.x, start.y, end.y, (!EditorGUIUtility.isProSkin) ? Color.white.RGBMultiplied(0.6f) : Color.white.RGBMultiplied(0.15f));
		}

		public static CurveWrapper GetCurveWrapper(AnimationWindowCurve curve, AnimationClip clip)
		{
			CurveWrapper curveWrapper = new CurveWrapper();
			curveWrapper.renderer = new NormalCurveRenderer(curve.ToAnimationCurve());
			curveWrapper.renderer.SetWrap(WrapMode.Once, (!clip.isLooping) ? WrapMode.Once : WrapMode.Loop);
			curveWrapper.renderer.SetCustomRange(clip.startTime, clip.stopTime);
			curveWrapper.binding = curve.binding;
			curveWrapper.id = curve.GetCurveID();
			curveWrapper.color = CurveUtility.GetPropertyColor(curve.propertyName);
			curveWrapper.hidden = false;
			curveWrapper.selectionBindingInterface = curve.selectionBindingInterface;
			return curveWrapper;
		}

		public static AnimationWindowKeyframe CurveSelectionToAnimationWindowKeyframe(CurveSelection curveSelection, List<AnimationWindowCurve> allCurves)
		{
			foreach (AnimationWindowCurve current in allCurves)
			{
				int curveID = current.GetCurveID();
				if (curveID == curveSelection.curveID && current.m_Keyframes.Count > curveSelection.key)
				{
					return current.m_Keyframes[curveSelection.key];
				}
			}
			return null;
		}

		public static CurveSelection AnimationWindowKeyframeToCurveSelection(AnimationWindowKeyframe keyframe, CurveEditor curveEditor)
		{
			int curveID = keyframe.curve.GetCurveID();
			CurveWrapper[] animationCurves = curveEditor.animationCurves;
			for (int i = 0; i < animationCurves.Length; i++)
			{
				CurveWrapper curveWrapper = animationCurves[i];
				if (curveWrapper.id == curveID && keyframe.GetIndex() >= 0)
				{
					return new CurveSelection(curveWrapper.id, curveEditor, keyframe.GetIndex());
				}
			}
			return null;
		}

		public static AnimationWindowCurve BestMatchForPaste(EditorCurveBinding binding, List<AnimationWindowCurve> clipboardCurves, List<AnimationWindowCurve> targetCurves)
		{
			foreach (AnimationWindowCurve current in targetCurves)
			{
				if (current.binding == binding)
				{
					AnimationWindowCurve result = current;
					return result;
				}
			}
			foreach (AnimationWindowCurve targetCurve in targetCurves)
			{
				if (targetCurve.binding.propertyName == binding.propertyName && !clipboardCurves.Exists((AnimationWindowCurve clipboardCurve) => clipboardCurve.binding == targetCurve.binding))
				{
					AnimationWindowCurve result = targetCurve;
					return result;
				}
			}
			return null;
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

		private static void DrawLine(Vector2 p1, Vector2 p2, Color color)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(1);
			GL.Color(color);
			GL.Vertex(p1);
			GL.Vertex(p2);
			GL.End();
			GL.PopMatrix();
		}

		private static void DrawRect(Vector3[] corners, Color color)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			HandleUtility.ApplyWireMaterial();
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(7);
			GL.Color(color);
			GL.Vertex(corners[0]);
			GL.Vertex(corners[1]);
			GL.Vertex(corners[2]);
			GL.Vertex(corners[3]);
			GL.End();
			GL.PopMatrix();
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
				return propertyGroupName == "m_LocalPosition" || propertyGroupName == "m_LocalScale" || propertyGroupName == "m_LocalRotation" || propertyGroupName == "localEulerAnglesBaked" || propertyGroupName == "localEulerAngles" || propertyGroupName == "localEulerAnglesRaw";
			}
			if (typeof(Renderer).IsAssignableFrom(binding.type))
			{
				string propertyGroupName2 = AnimationWindowUtility.GetPropertyGroupName(binding.propertyName);
				return propertyGroupName2 == "material._Color";
			}
			return false;
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
