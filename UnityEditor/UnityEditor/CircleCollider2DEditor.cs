using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CircleCollider2D))]
	internal class CircleCollider2DEditor : Collider2DEditorBase
	{
		private int m_HandleControlID;

		private SerializedProperty m_Radius;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_HandleControlID = -1;
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
			base.FinalizeInspectorGUI();
		}

		public void OnSceneGUI()
		{
			if (!Tools.viewToolActive)
			{
				bool flag = GUIUtility.hotControl == this.m_HandleControlID;
				CircleCollider2D circleCollider2D = (CircleCollider2D)base.target;
				Color color = Handles.color;
				Handles.color = Handles.s_ColliderHandleColor;
				bool enabled = GUI.enabled;
				if (!base.editingCollider && !flag)
				{
					GUI.enabled = false;
					Handles.color = new Color(0f, 0f, 0f, 0.001f);
				}
				Vector3 lossyScale = circleCollider2D.transform.lossyScale;
				float num = Mathf.Max(Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y)), Mathf.Abs(lossyScale.z));
				float num2 = num * circleCollider2D.radius;
				num2 = Mathf.Abs(num2);
				num2 = Mathf.Max(num2, 1E-05f);
				Vector3 position = circleCollider2D.transform.TransformPoint(circleCollider2D.offset);
				int hotControl = GUIUtility.hotControl;
				float num3 = Handles.RadiusHandle(Quaternion.identity, position, num2, true);
				if (GUI.changed)
				{
					Undo.RecordObject(circleCollider2D, "Adjust Radius");
					circleCollider2D.radius = num3 * 1f / num;
				}
				if (hotControl != GUIUtility.hotControl && GUIUtility.hotControl != 0)
				{
					this.m_HandleControlID = GUIUtility.hotControl;
				}
				Handles.color = color;
				GUI.enabled = enabled;
			}
		}
	}
}
