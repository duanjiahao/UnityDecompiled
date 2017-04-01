using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(AnimatorOverrideController))]
	internal class AnimatorOverrideControllerInspector : Editor
	{
		private SerializedProperty m_Controller;

		private List<KeyValuePair<AnimationClip, AnimationClip>> m_Clips;

		private ReorderableList m_ClipList;

		private string m_Search;

		private void OnEnable()
		{
			AnimatorOverrideController animatorOverrideController = base.target as AnimatorOverrideController;
			this.m_Controller = base.serializedObject.FindProperty("m_Controller");
			this.m_Search = "";
			if (this.m_Clips == null)
			{
				this.m_Clips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
			}
			if (this.m_ClipList == null)
			{
				animatorOverrideController.GetOverrides(this.m_Clips);
				this.m_Clips.Sort(new AnimationClipOverrideComparer());
				this.m_ClipList = new ReorderableList(this.m_Clips, typeof(KeyValuePair<AnimationClip, AnimationClip>), false, true, false, false);
				this.m_ClipList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawClipElement);
				this.m_ClipList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawClipHeader);
				this.m_ClipList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectClip);
				this.m_ClipList.elementHeight = 16f;
			}
			AnimatorOverrideController expr_E2 = animatorOverrideController;
			expr_E2.OnOverrideControllerDirty = (AnimatorOverrideController.OnOverrideControllerDirtyCallback)Delegate.Combine(expr_E2.OnOverrideControllerDirty, new AnimatorOverrideController.OnOverrideControllerDirtyCallback(base.Repaint));
		}

		private void OnDisable()
		{
			AnimatorOverrideController animatorOverrideController = base.target as AnimatorOverrideController;
			AnimatorOverrideController expr_0E = animatorOverrideController;
			expr_0E.OnOverrideControllerDirty = (AnimatorOverrideController.OnOverrideControllerDirtyCallback)Delegate.Remove(expr_0E.OnOverrideControllerDirty, new AnimatorOverrideController.OnOverrideControllerDirtyCallback(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			bool flag = base.targets.Length > 1;
			bool flag2 = false;
			base.serializedObject.UpdateIfRequiredOrScript();
			AnimatorOverrideController animatorOverrideController = base.target as AnimatorOverrideController;
			RuntimeAnimatorController runtimeAnimatorController = (!this.m_Controller.hasMultipleDifferentValues) ? animatorOverrideController.runtimeAnimatorController : null;
			EditorGUI.BeginChangeCheck();
			runtimeAnimatorController = (EditorGUILayout.ObjectField("Controller", runtimeAnimatorController, typeof(UnityEditor.Animations.AnimatorController), false, new GUILayoutOption[0]) as RuntimeAnimatorController);
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < base.targets.Length; i++)
				{
					AnimatorOverrideController animatorOverrideController2 = base.targets[i] as AnimatorOverrideController;
					animatorOverrideController2.runtimeAnimatorController = runtimeAnimatorController;
				}
				flag2 = true;
			}
			GUI.SetNextControlName("OverridesSearch");
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape && GUI.GetNameOfFocusedControl() == "OverridesSearch")
			{
				this.m_Search = "";
			}
			EditorGUI.BeginChangeCheck();
			string search = EditorGUILayout.ToolbarSearchField(this.m_Search, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Search = search;
			}
			using (new EditorGUI.DisabledScope(this.m_Controller == null || (flag && this.m_Controller.hasMultipleDifferentValues) || runtimeAnimatorController == null))
			{
				EditorGUI.BeginChangeCheck();
				animatorOverrideController.GetOverrides(this.m_Clips);
				if (this.m_Search.Length > 0)
				{
					this.FilterOverrides();
				}
				else
				{
					this.m_Clips.Sort(new AnimationClipOverrideComparer());
				}
				this.m_ClipList.list = this.m_Clips;
				this.m_ClipList.DoLayoutList();
				if (EditorGUI.EndChangeCheck())
				{
					for (int j = 0; j < base.targets.Length; j++)
					{
						AnimatorOverrideController animatorOverrideController3 = base.targets[j] as AnimatorOverrideController;
						animatorOverrideController3.ApplyOverrides(this.m_Clips);
					}
					flag2 = true;
				}
			}
			if (flag2)
			{
				animatorOverrideController.PerformOverrideClipListCleanup();
			}
		}

		private void FilterOverrides()
		{
			if (this.m_Search.Length != 0)
			{
				string[] array = this.m_Search.ToLower().Split(new char[]
				{
					' '
				});
				List<KeyValuePair<AnimationClip, AnimationClip>> list = new List<KeyValuePair<AnimationClip, AnimationClip>>();
				List<KeyValuePair<AnimationClip, AnimationClip>> list2 = new List<KeyValuePair<AnimationClip, AnimationClip>>();
				foreach (KeyValuePair<AnimationClip, AnimationClip> current in this.m_Clips)
				{
					string text = current.Key.name;
					text = text.ToLower().Replace(" ", "");
					bool flag = true;
					bool flag2 = false;
					for (int i = 0; i < array.Length; i++)
					{
						string value = array[i];
						if (!text.Contains(value))
						{
							flag = false;
							break;
						}
						if (i == 0 && text.StartsWith(value))
						{
							flag2 = true;
						}
					}
					if (flag)
					{
						if (flag2)
						{
							list.Add(current);
						}
						else
						{
							list2.Add(current);
						}
					}
				}
				this.m_Clips.Clear();
				list.Sort(new AnimationClipOverrideComparer());
				list2.Sort(new AnimationClipOverrideComparer());
				this.m_Clips.AddRange(list);
				this.m_Clips.AddRange(list2);
			}
		}

		private void DrawClipElement(Rect rect, int index, bool selected, bool focused)
		{
			AnimationClip key = this.m_Clips[index].Key;
			AnimationClip animationClip = this.m_Clips[index].Value;
			rect.xMax /= 2f;
			GUI.Label(rect, key.name, EditorStyles.label);
			rect.xMin = rect.xMax;
			rect.xMax *= 2f;
			EditorGUI.BeginChangeCheck();
			animationClip = (EditorGUI.ObjectField(rect, "", animationClip, typeof(AnimationClip), false) as AnimationClip);
			if (EditorGUI.EndChangeCheck())
			{
				this.m_Clips[index] = new KeyValuePair<AnimationClip, AnimationClip>(key, animationClip);
			}
		}

		private void DrawClipHeader(Rect rect)
		{
			rect.xMax /= 2f;
			GUI.Label(rect, "Original", EditorStyles.label);
			rect.xMin = rect.xMax;
			rect.xMax *= 2f;
			GUI.Label(rect, "Override", EditorStyles.label);
		}

		private void SelectClip(ReorderableList list)
		{
			if (0 <= list.index && list.index < this.m_Clips.Count)
			{
				EditorGUIUtility.PingObject(this.m_Clips[list.index].Key);
			}
		}
	}
}
