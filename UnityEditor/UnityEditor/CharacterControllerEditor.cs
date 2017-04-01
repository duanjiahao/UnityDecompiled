using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CharacterController))]
	internal class CharacterControllerEditor : Editor
	{
		private SerializedProperty m_Height;

		private SerializedProperty m_Radius;

		private SerializedProperty m_SlopeLimit;

		private SerializedProperty m_StepOffset;

		private SerializedProperty m_SkinWidth;

		private SerializedProperty m_MinMoveDistance;

		private SerializedProperty m_Center;

		private int m_HandleControlID;

		[CompilerGenerated]
		private static Handles.CapFunction <>f__mg$cache0;

		public void OnEnable()
		{
			this.m_Height = base.serializedObject.FindProperty("m_Height");
			this.m_Radius = base.serializedObject.FindProperty("m_Radius");
			this.m_SlopeLimit = base.serializedObject.FindProperty("m_SlopeLimit");
			this.m_StepOffset = base.serializedObject.FindProperty("m_StepOffset");
			this.m_SkinWidth = base.serializedObject.FindProperty("m_SkinWidth");
			this.m_MinMoveDistance = base.serializedObject.FindProperty("m_MinMoveDistance");
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_HandleControlID = -1;
		}

		public void OnDisable()
		{
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_SlopeLimit, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_StepOffset, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SkinWidth, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_MinMoveDistance, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Radius, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Height, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			bool flag = GUIUtility.hotControl == this.m_HandleControlID;
			CharacterController characterController = (CharacterController)base.target;
			Color color = Handles.color;
			if (characterController.enabled)
			{
				Handles.color = Handles.s_ColliderHandleColor;
			}
			else
			{
				Handles.color = Handles.s_ColliderHandleColorDisabled;
			}
			bool enabled = GUI.enabled;
			if (!Event.current.shift && !flag)
			{
				GUI.enabled = false;
				Handles.color = new Color(1f, 0f, 0f, 0.001f);
			}
			float num = characterController.height * characterController.transform.lossyScale.y;
			float num2 = characterController.radius * Mathf.Max(characterController.transform.lossyScale.x, characterController.transform.lossyScale.z);
			num = Mathf.Max(num, num2 * 2f);
			Matrix4x4 matrix = Matrix4x4.TRS(characterController.transform.TransformPoint(characterController.center), Quaternion.identity, Vector3.one);
			int hotControl = GUIUtility.hotControl;
			Vector3 vector = Vector3.up * num * 0.5f;
			float num3 = CharacterControllerEditor.SizeHandle(vector, Vector3.up, matrix, true);
			if (!GUI.changed)
			{
				num3 = CharacterControllerEditor.SizeHandle(-vector, Vector3.down, matrix, true);
			}
			if (GUI.changed)
			{
				Undo.RecordObject(characterController, "Character Controller Resize");
				float num4 = num / characterController.height;
				characterController.height += num3 / num4;
			}
			num3 = CharacterControllerEditor.SizeHandle(Vector3.left * num2, Vector3.left, matrix, true);
			if (!GUI.changed)
			{
				num3 = CharacterControllerEditor.SizeHandle(-Vector3.left * num2, -Vector3.left, matrix, true);
			}
			if (!GUI.changed)
			{
				num3 = CharacterControllerEditor.SizeHandle(Vector3.forward * num2, Vector3.forward, matrix, true);
			}
			if (!GUI.changed)
			{
				num3 = CharacterControllerEditor.SizeHandle(-Vector3.forward * num2, -Vector3.forward, matrix, true);
			}
			if (GUI.changed)
			{
				Undo.RecordObject(characterController, "Character Controller Resize");
				float num5 = num2 / characterController.radius;
				characterController.radius += num3 / num5;
			}
			if (hotControl != GUIUtility.hotControl && GUIUtility.hotControl != 0)
			{
				this.m_HandleControlID = GUIUtility.hotControl;
			}
			if (GUI.changed)
			{
				characterController.radius = Mathf.Max(characterController.radius, 1E-05f);
				characterController.height = Mathf.Max(characterController.height, 1E-05f);
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
			if (CharacterControllerEditor.<>f__mg$cache0 == null)
			{
				CharacterControllerEditor.<>f__mg$cache0 = new Handles.CapFunction(Handles.DotHandleCap);
			}
			Vector3 point = Handles.Slider(arg_119_0, arg_119_1, arg_119_2, CharacterControllerEditor.<>f__mg$cache0, 0f);
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
