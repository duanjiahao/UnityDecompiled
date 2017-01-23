using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider))]
	internal class CapsuleColliderEditor : Collider3DEditorBase
	{
		private SerializedProperty m_Center;

		private SerializedProperty m_Radius;

		private SerializedProperty m_Height;

		private SerializedProperty m_Direction;

		private int m_HandleControlID;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_Direction = base.serializedObject.FindProperty("m_Direction");
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
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			bool flag = GUIUtility.hotControl == this.m_HandleControlID;
			CapsuleCollider capsuleCollider = (CapsuleCollider)base.target;
			Color color = Handles.color;
			if (capsuleCollider.enabled)
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
				Handles.color = new Color(1f, 0f, 0f, 0.001f);
			}
			Vector3 capsuleExtents = ColliderUtil.GetCapsuleExtents(capsuleCollider);
			float num = capsuleExtents.y + 2f * capsuleExtents.x;
			float x = capsuleExtents.x;
			Matrix4x4 matrix = ColliderUtil.CalculateCapsuleTransform(capsuleCollider);
			int hotControl = GUIUtility.hotControl;
			float num2 = capsuleCollider.height;
			Vector3 vector = Vector3.left * num * 0.5f;
			float num3 = CapsuleColliderEditor.SizeHandle(vector, Vector3.left, matrix, true);
			if (!GUI.changed)
			{
				num3 = CapsuleColliderEditor.SizeHandle(-vector, Vector3.right, matrix, true);
			}
			if (GUI.changed)
			{
				float num4 = num / capsuleCollider.height;
				num2 += num3 / num4;
			}
			float num5 = capsuleCollider.radius;
			num3 = CapsuleColliderEditor.SizeHandle(Vector3.forward * x, Vector3.forward, matrix, true);
			if (!GUI.changed)
			{
				num3 = CapsuleColliderEditor.SizeHandle(-Vector3.forward * x, -Vector3.forward, matrix, true);
			}
			if (!GUI.changed)
			{
				num3 = CapsuleColliderEditor.SizeHandle(Vector3.up * x, Vector3.up, matrix, true);
			}
			if (!GUI.changed)
			{
				num3 = CapsuleColliderEditor.SizeHandle(-Vector3.up * x, -Vector3.up, matrix, true);
			}
			if (GUI.changed)
			{
				float num6 = Mathf.Max(capsuleExtents.z / capsuleCollider.radius, capsuleExtents.x / capsuleCollider.radius);
				num5 += num3 / num6;
			}
			if (hotControl != GUIUtility.hotControl && GUIUtility.hotControl != 0)
			{
				this.m_HandleControlID = GUIUtility.hotControl;
			}
			if (GUI.changed)
			{
				Undo.RecordObject(capsuleCollider, "Modify Capsule Collider");
				capsuleCollider.radius = Mathf.Max(num5, 1E-05f);
				capsuleCollider.height = Mathf.Max(num2, 1E-05f);
			}
			Handles.color = color;
			GUI.enabled = enabled;
		}

		private static float SizeHandle(Vector3 localPos, Vector3 localPullDir, Matrix4x4 matrix, bool isEdgeHandle)
		{
			Vector3 vector = matrix.MultiplyVector(localPullDir);
			Vector3 vector2 = matrix.MultiplyPoint(localPos);
			float handleSize = HandleUtility.GetHandleSize(vector2);
			bool changed = GUI.changed;
			GUI.changed = false;
			Color color = Handles.color;
			float num = 0f;
			if (isEdgeHandle)
			{
				num = Mathf.Cos(0.7853982f);
			}
			float num2;
			if (Camera.current.orthographic)
			{
				num2 = Vector3.Dot(-Camera.current.transform.forward, vector);
			}
			else
			{
				num2 = Vector3.Dot((Camera.current.transform.position - vector2).normalized, vector);
			}
			if (num2 < -num)
			{
				Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, Handles.color.a * Handles.backfaceAlphaMultiplier);
			}
			Vector3 arg_119_0 = vector2;
			Vector3 arg_119_1 = vector;
			float arg_119_2 = handleSize * 0.03f;
			if (CapsuleColliderEditor.<>f__mg$cache0 == null)
			{
				CapsuleColliderEditor.<>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			Vector3 point = Handles.Slider(arg_119_0, arg_119_1, arg_119_2, CapsuleColliderEditor.<>f__mg$cache0, 0f);
			float result = 0f;
			if (GUI.changed)
			{
				result = HandleUtility.PointOnLineParameter(point, vector2, vector);
			}
			GUI.changed |= changed;
			Handles.color = color;
			return result;
		}
	}
}
