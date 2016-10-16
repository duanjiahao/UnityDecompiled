using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ColliderEditorUtility
	{
		private const float k_EditColliderbuttonWidth = 22f;

		private const float k_EditColliderbuttonHeight = 22f;

		private const float k_SpaceBetweenLabelAndButton = 5f;

		private static GUIStyle s_EditColliderButtonStyle;

		public static bool InspectorEditButtonGUI(bool editing)
		{
			if (ColliderEditorUtility.s_EditColliderButtonStyle == null)
			{
				ColliderEditorUtility.s_EditColliderButtonStyle = new GUIStyle("Button");
				ColliderEditorUtility.s_EditColliderButtonStyle.padding = new RectOffset(0, 0, 0, 0);
				ColliderEditorUtility.s_EditColliderButtonStyle.margin = new RectOffset(0, 0, 0, 0);
			}
			EditorGUI.BeginChangeCheck();
			Rect controlRect = EditorGUILayout.GetControlRect(true, 22f, new GUILayoutOption[0]);
			Rect position = new Rect(controlRect.xMin + EditorGUIUtility.labelWidth, controlRect.yMin, 22f, 22f);
			GUIContent content = new GUIContent("Edit Collider");
			Vector2 vector = GUI.skin.label.CalcSize(content);
			Rect position2 = new Rect(position.xMax + 5f, controlRect.yMin + (controlRect.height - vector.y) * 0.5f, vector.x, controlRect.height);
			GUILayout.Space(2f);
			bool result = GUI.Toggle(position, editing, EditorGUIUtility.IconContent("EditCollider"), ColliderEditorUtility.s_EditColliderButtonStyle);
			GUI.Label(position2, "Edit Collider");
			if (EditorGUI.EndChangeCheck())
			{
				SceneView.RepaintAll();
			}
			return result;
		}
	}
}
