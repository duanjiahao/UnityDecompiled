using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SphereCollider))]
	internal class SphereColliderEditor : Collider3DEditorBase
	{
		private SerializedProperty m_Center;

		private SerializedProperty m_Radius;

		private int m_HandleControlID;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_HandleControlID = -1;
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			bool flag = GUIUtility.hotControl == this.m_HandleControlID;
			SphereCollider sphereCollider = (SphereCollider)base.target;
			Color color = Handles.color;
			if (sphereCollider.enabled)
			{
				Handles.color = Handles.s_ColliderHandleColor;
			}
			else
			{
				Handles.color = Handles.s_ColliderHandleColorDisabled;
			}
			bool enabled = GUI.enabled;
			if (!base.editingCollider && !flag)
			{
				GUI.enabled = false;
				Handles.color = new Color(0f, 0f, 0f, 0.001f);
			}
			Vector3 lossyScale = sphereCollider.transform.lossyScale;
			float num = Mathf.Max(Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y)), Mathf.Abs(lossyScale.z));
			float num2 = num * sphereCollider.radius;
			num2 = Mathf.Abs(num2);
			num2 = Mathf.Max(num2, 1E-05f);
			Vector3 position = sphereCollider.transform.TransformPoint(sphereCollider.center);
			Quaternion rotation = sphereCollider.transform.rotation;
			int hotControl = GUIUtility.hotControl;
			float num3 = Handles.RadiusHandle(rotation, position, num2, true);
			if (GUI.changed)
			{
				Undo.RecordObject(sphereCollider, "Adjust Radius");
				sphereCollider.radius = num3 * 1f / num;
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
