using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(ScrollRect), true)]
	public class ScrollRectEditor : Editor
	{
		private SerializedProperty m_Content;

		private SerializedProperty m_Horizontal;

		private SerializedProperty m_Vertical;

		private SerializedProperty m_MovementType;

		private SerializedProperty m_Elasticity;

		private SerializedProperty m_Inertia;

		private SerializedProperty m_DecelerationRate;

		private SerializedProperty m_ScrollSensitivity;

		private SerializedProperty m_Viewport;

		private SerializedProperty m_HorizontalScrollbar;

		private SerializedProperty m_VerticalScrollbar;

		private SerializedProperty m_HorizontalScrollbarVisibility;

		private SerializedProperty m_VerticalScrollbarVisibility;

		private SerializedProperty m_HorizontalScrollbarSpacing;

		private SerializedProperty m_VerticalScrollbarSpacing;

		private SerializedProperty m_OnValueChanged;

		private AnimBool m_ShowElasticity;

		private AnimBool m_ShowDecelerationRate;

		private bool m_ViewportIsNotChild;

		private bool m_HScrollbarIsNotChild;

		private bool m_VScrollbarIsNotChild;

		private static string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

		private static string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

		protected virtual void OnEnable()
		{
			this.m_Content = base.serializedObject.FindProperty("m_Content");
			this.m_Horizontal = base.serializedObject.FindProperty("m_Horizontal");
			this.m_Vertical = base.serializedObject.FindProperty("m_Vertical");
			this.m_MovementType = base.serializedObject.FindProperty("m_MovementType");
			this.m_Elasticity = base.serializedObject.FindProperty("m_Elasticity");
			this.m_Inertia = base.serializedObject.FindProperty("m_Inertia");
			this.m_DecelerationRate = base.serializedObject.FindProperty("m_DecelerationRate");
			this.m_ScrollSensitivity = base.serializedObject.FindProperty("m_ScrollSensitivity");
			this.m_Viewport = base.serializedObject.FindProperty("m_Viewport");
			this.m_HorizontalScrollbar = base.serializedObject.FindProperty("m_HorizontalScrollbar");
			this.m_VerticalScrollbar = base.serializedObject.FindProperty("m_VerticalScrollbar");
			this.m_HorizontalScrollbarVisibility = base.serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
			this.m_VerticalScrollbarVisibility = base.serializedObject.FindProperty("m_VerticalScrollbarVisibility");
			this.m_HorizontalScrollbarSpacing = base.serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
			this.m_VerticalScrollbarSpacing = base.serializedObject.FindProperty("m_VerticalScrollbarSpacing");
			this.m_OnValueChanged = base.serializedObject.FindProperty("m_OnValueChanged");
			this.m_ShowElasticity = new AnimBool(new UnityAction(base.Repaint));
			this.m_ShowDecelerationRate = new AnimBool(new UnityAction(base.Repaint));
			this.SetAnimBools(true);
		}

		protected virtual void OnDisable()
		{
			this.m_ShowElasticity.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowDecelerationRate.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		private void SetAnimBools(bool instant)
		{
			this.SetAnimBool(this.m_ShowElasticity, !this.m_MovementType.hasMultipleDifferentValues && this.m_MovementType.enumValueIndex == 1, instant);
			this.SetAnimBool(this.m_ShowDecelerationRate, !this.m_Inertia.hasMultipleDifferentValues && this.m_Inertia.boolValue, instant);
		}

		private void SetAnimBool(AnimBool a, bool value, bool instant)
		{
			if (instant)
			{
				a.value = value;
			}
			else
			{
				a.target = value;
			}
		}

		private void CalculateCachedValues()
		{
			this.m_ViewportIsNotChild = false;
			this.m_HScrollbarIsNotChild = false;
			this.m_VScrollbarIsNotChild = false;
			if (base.targets.Length == 1)
			{
				Transform transform = ((ScrollRect)base.target).transform;
				if (this.m_Viewport.objectReferenceValue == null || ((RectTransform)this.m_Viewport.objectReferenceValue).transform.parent != transform)
				{
					this.m_ViewportIsNotChild = true;
				}
				if (this.m_HorizontalScrollbar.objectReferenceValue == null || ((Scrollbar)this.m_HorizontalScrollbar.objectReferenceValue).transform.parent != transform)
				{
					this.m_HScrollbarIsNotChild = true;
				}
				if (this.m_VerticalScrollbar.objectReferenceValue == null || ((Scrollbar)this.m_VerticalScrollbar.objectReferenceValue).transform.parent != transform)
				{
					this.m_VScrollbarIsNotChild = true;
				}
			}
		}

		public override void OnInspectorGUI()
		{
			this.SetAnimBools(false);
			base.serializedObject.Update();
			this.CalculateCachedValues();
			EditorGUILayout.PropertyField(this.m_Content, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Horizontal, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Vertical, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MovementType, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowElasticity.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_Elasticity, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(this.m_Inertia, new GUILayoutOption[0]);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowDecelerationRate.faded))
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_DecelerationRate, new GUILayoutOption[0]);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.PropertyField(this.m_ScrollSensitivity, new GUILayoutOption[0]);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_Viewport, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_HorizontalScrollbar, new GUILayoutOption[0]);
			if (this.m_HorizontalScrollbar.objectReferenceValue && !this.m_HorizontalScrollbar.hasMultipleDifferentValues)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_HorizontalScrollbarVisibility, new GUIContent("Visibility"), new GUILayoutOption[0]);
				if (this.m_HorizontalScrollbarVisibility.enumValueIndex == 2 && !this.m_HorizontalScrollbarVisibility.hasMultipleDifferentValues)
				{
					if (this.m_ViewportIsNotChild || this.m_HScrollbarIsNotChild)
					{
						EditorGUILayout.HelpBox(ScrollRectEditor.s_HError, MessageType.Error);
					}
					EditorGUILayout.PropertyField(this.m_HorizontalScrollbarSpacing, new GUIContent("Spacing"), new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.PropertyField(this.m_VerticalScrollbar, new GUILayoutOption[0]);
			if (this.m_VerticalScrollbar.objectReferenceValue && !this.m_VerticalScrollbar.hasMultipleDifferentValues)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(this.m_VerticalScrollbarVisibility, new GUIContent("Visibility"), new GUILayoutOption[0]);
				if (this.m_VerticalScrollbarVisibility.enumValueIndex == 2 && !this.m_VerticalScrollbarVisibility.hasMultipleDifferentValues)
				{
					if (this.m_ViewportIsNotChild || this.m_VScrollbarIsNotChild)
					{
						EditorGUILayout.HelpBox(ScrollRectEditor.s_VError, MessageType.Error);
					}
					EditorGUILayout.PropertyField(this.m_VerticalScrollbarSpacing, new GUIContent("Spacing"), new GUILayoutOption[0]);
				}
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_OnValueChanged, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
