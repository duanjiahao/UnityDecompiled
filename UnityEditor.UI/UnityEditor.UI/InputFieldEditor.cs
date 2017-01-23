using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(InputField), true)]
	public class InputFieldEditor : SelectableEditor
	{
		private SerializedProperty m_TextComponent;

		private SerializedProperty m_Text;

		private SerializedProperty m_ContentType;

		private SerializedProperty m_LineType;

		private SerializedProperty m_InputType;

		private SerializedProperty m_CharacterValidation;

		private SerializedProperty m_KeyboardType;

		private SerializedProperty m_CharacterLimit;

		private SerializedProperty m_CaretBlinkRate;

		private SerializedProperty m_CaretWidth;

		private SerializedProperty m_CaretColor;

		private SerializedProperty m_CustomCaretColor;

		private SerializedProperty m_SelectionColor;

		private SerializedProperty m_HideMobileInput;

		private SerializedProperty m_Placeholder;

		private SerializedProperty m_OnValueChanged;

		private SerializedProperty m_OnEndEdit;

		private SerializedProperty m_ReadOnly;

		private AnimBool m_CustomColor;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_TextComponent = base.serializedObject.FindProperty("m_TextComponent");
			this.m_Text = base.serializedObject.FindProperty("m_Text");
			this.m_ContentType = base.serializedObject.FindProperty("m_ContentType");
			this.m_LineType = base.serializedObject.FindProperty("m_LineType");
			this.m_InputType = base.serializedObject.FindProperty("m_InputType");
			this.m_CharacterValidation = base.serializedObject.FindProperty("m_CharacterValidation");
			this.m_KeyboardType = base.serializedObject.FindProperty("m_KeyboardType");
			this.m_CharacterLimit = base.serializedObject.FindProperty("m_CharacterLimit");
			this.m_CaretBlinkRate = base.serializedObject.FindProperty("m_CaretBlinkRate");
			this.m_CaretWidth = base.serializedObject.FindProperty("m_CaretWidth");
			this.m_CaretColor = base.serializedObject.FindProperty("m_CaretColor");
			this.m_CustomCaretColor = base.serializedObject.FindProperty("m_CustomCaretColor");
			this.m_SelectionColor = base.serializedObject.FindProperty("m_SelectionColor");
			this.m_HideMobileInput = base.serializedObject.FindProperty("m_HideMobileInput");
			this.m_Placeholder = base.serializedObject.FindProperty("m_Placeholder");
			this.m_OnValueChanged = base.serializedObject.FindProperty("m_OnValueChanged");
			this.m_OnEndEdit = base.serializedObject.FindProperty("m_OnEndEdit");
			this.m_ReadOnly = base.serializedObject.FindProperty("m_ReadOnly");
			this.m_CustomColor = new AnimBool(this.m_CustomCaretColor.boolValue);
			this.m_CustomColor.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_CustomColor.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.OnInspectorGUI();
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_TextComponent, new GUILayoutOption[0]);
			if (this.m_TextComponent != null && this.m_TextComponent.objectReferenceValue != null)
			{
				Text text = this.m_TextComponent.objectReferenceValue as Text;
				if (text.supportRichText)
				{
					EditorGUILayout.HelpBox("Using Rich Text with input is unsupported.", MessageType.Warning);
				}
			}
			using (new EditorGUI.DisabledScope(this.m_TextComponent == null || this.m_TextComponent.objectReferenceValue == null))
			{
				EditorGUILayout.PropertyField(this.m_Text, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CharacterLimit, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_ContentType, new GUILayoutOption[0]);
				if (!this.m_ContentType.hasMultipleDifferentValues)
				{
					EditorGUI.indentLevel++;
					if (this.m_ContentType.enumValueIndex == 0 || this.m_ContentType.enumValueIndex == 1 || this.m_ContentType.enumValueIndex == 9)
					{
						EditorGUILayout.PropertyField(this.m_LineType, new GUILayoutOption[0]);
					}
					if (this.m_ContentType.enumValueIndex == 9)
					{
						EditorGUILayout.PropertyField(this.m_InputType, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_KeyboardType, new GUILayoutOption[0]);
						EditorGUILayout.PropertyField(this.m_CharacterValidation, new GUILayoutOption[0]);
					}
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_Placeholder, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CaretBlinkRate, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CaretWidth, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_CustomCaretColor, new GUILayoutOption[0]);
				this.m_CustomColor.target = this.m_CustomCaretColor.boolValue;
				if (EditorGUILayout.BeginFadeGroup(this.m_CustomColor.faded))
				{
					EditorGUILayout.PropertyField(this.m_CaretColor, new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				EditorGUILayout.PropertyField(this.m_SelectionColor, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_HideMobileInput, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ReadOnly, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(this.m_OnValueChanged, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_OnEndEdit, new GUILayoutOption[0]);
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
