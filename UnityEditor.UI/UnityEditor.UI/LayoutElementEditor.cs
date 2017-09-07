using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(LayoutElement), true)]
	public class LayoutElementEditor : Editor
	{
		private SerializedProperty m_IgnoreLayout;

		private SerializedProperty m_MinWidth;

		private SerializedProperty m_MinHeight;

		private SerializedProperty m_PreferredWidth;

		private SerializedProperty m_PreferredHeight;

		private SerializedProperty m_FlexibleWidth;

		private SerializedProperty m_FlexibleHeight;

		private SerializedProperty m_LayoutPriority;

		protected virtual void OnEnable()
		{
			this.m_IgnoreLayout = base.serializedObject.FindProperty("m_IgnoreLayout");
			this.m_MinWidth = base.serializedObject.FindProperty("m_MinWidth");
			this.m_MinHeight = base.serializedObject.FindProperty("m_MinHeight");
			this.m_PreferredWidth = base.serializedObject.FindProperty("m_PreferredWidth");
			this.m_PreferredHeight = base.serializedObject.FindProperty("m_PreferredHeight");
			this.m_FlexibleWidth = base.serializedObject.FindProperty("m_FlexibleWidth");
			this.m_FlexibleHeight = base.serializedObject.FindProperty("m_FlexibleHeight");
			this.m_LayoutPriority = base.serializedObject.FindProperty("m_LayoutPriority");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_IgnoreLayout, new GUILayoutOption[0]);
			if (!this.m_IgnoreLayout.boolValue)
			{
				EditorGUILayout.Space();
				this.LayoutElementField(this.m_MinWidth, 0f);
				this.LayoutElementField(this.m_MinHeight, 0f);
				this.LayoutElementField(this.m_PreferredWidth, (RectTransform t) => t.rect.width);
				this.LayoutElementField(this.m_PreferredHeight, (RectTransform t) => t.rect.height);
				this.LayoutElementField(this.m_FlexibleWidth, 1f);
				this.LayoutElementField(this.m_FlexibleHeight, 1f);
			}
			EditorGUILayout.PropertyField(this.m_LayoutPriority, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		private void LayoutElementField(SerializedProperty property, float defaultValue)
		{
			this.LayoutElementField(property, (RectTransform _) => defaultValue);
		}

		private void LayoutElementField(SerializedProperty property, Func<RectTransform, float> defaultValue)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
			GUIContent label = EditorGUI.BeginProperty(controlRect, null, property);
			Rect rect = EditorGUI.PrefixLabel(controlRect, label);
			Rect position = rect;
			position.width = 16f;
			Rect position2 = rect;
			position2.xMin += 16f;
			EditorGUI.BeginChangeCheck();
			bool flag = EditorGUI.ToggleLeft(position, GUIContent.none, property.floatValue >= 0f);
			if (EditorGUI.EndChangeCheck())
			{
				property.floatValue = ((!flag) ? -1f : defaultValue((base.target as LayoutElement).transform as RectTransform));
			}
			if (!property.hasMultipleDifferentValues && property.floatValue >= 0f)
			{
				EditorGUIUtility.labelWidth = 4f;
				EditorGUI.BeginChangeCheck();
				float b = EditorGUI.FloatField(position2, new GUIContent(" "), property.floatValue);
				if (EditorGUI.EndChangeCheck())
				{
					property.floatValue = Mathf.Max(0f, b);
				}
				EditorGUIUtility.labelWidth = 0f;
			}
			EditorGUI.EndProperty();
		}
	}
}
