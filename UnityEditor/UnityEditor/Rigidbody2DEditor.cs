using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Rigidbody2D))]
	internal class Rigidbody2DEditor : Editor
	{
		private const int k_ToggleOffset = 30;

		private SerializedProperty m_UseAutoMass;

		private SerializedProperty m_Mass;

		private SerializedProperty m_Constraints;

		private readonly AnimBool m_ShowMass = new AnimBool();

		private static readonly GUIContent m_FreezePositionLabel = new GUIContent("Freeze Position");

		private static readonly GUIContent m_FreezeRotationLabel = new GUIContent("Freeze Rotation");

		public void OnEnable()
		{
			Rigidbody2D rigidbody2D = this.target as Rigidbody2D;
			this.m_UseAutoMass = base.serializedObject.FindProperty("m_UseAutoMass");
			this.m_Mass = base.serializedObject.FindProperty("m_Mass");
			this.m_Constraints = base.serializedObject.FindProperty("m_Constraints");
			this.m_ShowMass.value = !rigidbody2D.useAutoMass;
			this.m_ShowMass.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public void OnDisable()
		{
			this.m_ShowMass.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			Rigidbody2D rigidbody2D = this.target as Rigidbody2D;
			base.serializedObject.Update();
			this.m_ShowMass.target = !rigidbody2D.useAutoMass;
			EditorGUILayout.PropertyField(this.m_UseAutoMass, new GUILayoutOption[0]);
			bool enabled = GUI.enabled;
			GUI.enabled = !rigidbody2D.useAutoMass;
			EditorGUILayout.PropertyField(this.m_Mass, new GUILayoutOption[0]);
			GUI.enabled = enabled;
			base.serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.m_Constraints.isExpanded = EditorGUILayout.Foldout(this.m_Constraints.isExpanded, "Constraints");
			GUILayout.EndHorizontal();
			RigidbodyConstraints2D intValue = (RigidbodyConstraints2D)this.m_Constraints.intValue;
			if (this.m_Constraints.isExpanded)
			{
				EditorGUI.indentLevel++;
				this.ToggleFreezePosition(intValue, Rigidbody2DEditor.m_FreezePositionLabel, 0, 1);
				this.ToggleFreezeRotation(intValue, Rigidbody2DEditor.m_FreezeRotationLabel, 2);
				EditorGUI.indentLevel--;
			}
			if (intValue == RigidbodyConstraints2D.FreezeAll)
			{
				EditorGUILayout.HelpBox("Rather than turning on all constraints, you may want to consider removing the Rigidbody2D component which makes any colliders static.  This gives far better performance overall.", MessageType.Info);
			}
		}

		private void ConstraintToggle(Rect r, string label, RigidbodyConstraints2D value, int bit)
		{
			bool value2 = (value & (RigidbodyConstraints2D)(1 << bit)) != RigidbodyConstraints2D.None;
			EditorGUI.showMixedValue = ((this.m_Constraints.hasMultipleDifferentValuesBitwise & 1 << bit) != 0);
			EditorGUI.BeginChangeCheck();
			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			value2 = EditorGUI.ToggleLeft(r, label, value2);
			EditorGUI.indentLevel = indentLevel;
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(base.targets, "Edit Constraints2D");
				this.m_Constraints.SetBitAtIndexForAllTargetsImmediate(bit, value2);
			}
			EditorGUI.showMixedValue = false;
		}

		private void ToggleFreezePosition(RigidbodyConstraints2D constraints, GUIContent label, int x, int y)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
			int controlID = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, label);
			rect.width = 30f;
			this.ConstraintToggle(rect, "X", constraints, x);
			rect.x += 30f;
			this.ConstraintToggle(rect, "Y", constraints, y);
			GUILayout.EndHorizontal();
		}

		private void ToggleFreezeRotation(RigidbodyConstraints2D constraints, GUIContent label, int z)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
			int controlID = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, label);
			rect.width = 30f;
			this.ConstraintToggle(rect, "Z", constraints, z);
			GUILayout.EndHorizontal();
		}
	}
}
