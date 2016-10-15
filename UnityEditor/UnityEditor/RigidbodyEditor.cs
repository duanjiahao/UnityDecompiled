using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Rigidbody))]
	internal class RigidbodyEditor : Editor
	{
		private SerializedProperty m_Constraints;

		private static GUIContent m_FreezePositionLabel = new GUIContent("Freeze Position");

		private static GUIContent m_FreezeRotationLabel = new GUIContent("Freeze Rotation");

		public void OnEnable()
		{
			this.m_Constraints = base.serializedObject.FindProperty("m_Constraints");
		}

		private void ConstraintToggle(Rect r, string label, RigidbodyConstraints value, int bit)
		{
			bool value2 = (value & (RigidbodyConstraints)(1 << bit)) != RigidbodyConstraints.None;
			EditorGUI.showMixedValue = ((this.m_Constraints.hasMultipleDifferentValuesBitwise & 1 << bit) != 0);
			EditorGUI.BeginChangeCheck();
			int indentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			value2 = EditorGUI.ToggleLeft(r, label, value2);
			EditorGUI.indentLevel = indentLevel;
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObjects(base.targets, "Edit Constraints");
				this.m_Constraints.SetBitAtIndexForAllTargetsImmediate(bit, value2);
			}
			EditorGUI.showMixedValue = false;
		}

		private void ToggleBlock(RigidbodyConstraints constraints, GUIContent label, int x, int y, int z)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
			int controlID = GUIUtility.GetControlID(7231, FocusType.Keyboard, rect);
			rect = EditorGUI.PrefixLabel(rect, controlID, label);
			rect.width = 30f;
			this.ConstraintToggle(rect, "X", constraints, x);
			rect.x += 30f;
			this.ConstraintToggle(rect, "Y", constraints, y);
			rect.x += 30f;
			this.ConstraintToggle(rect, "Z", constraints, z);
			GUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.m_Constraints.isExpanded = EditorGUILayout.Foldout(this.m_Constraints.isExpanded, "Constraints");
			GUILayout.EndHorizontal();
			base.serializedObject.Update();
			RigidbodyConstraints intValue = (RigidbodyConstraints)this.m_Constraints.intValue;
			if (this.m_Constraints.isExpanded)
			{
				EditorGUI.indentLevel++;
				this.ToggleBlock(intValue, RigidbodyEditor.m_FreezePositionLabel, 1, 2, 3);
				this.ToggleBlock(intValue, RigidbodyEditor.m_FreezeRotationLabel, 4, 5, 6);
				EditorGUI.indentLevel--;
			}
		}
	}
}
