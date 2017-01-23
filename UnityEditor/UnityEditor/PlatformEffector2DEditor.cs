using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PlatformEffector2D), true)]
	internal class PlatformEffector2DEditor : Effector2DEditor
	{
		private SerializedProperty m_RotationalOffset;

		private readonly AnimBool m_ShowOneWayRollout = new AnimBool();

		private SerializedProperty m_UseOneWay;

		private SerializedProperty m_UseOneWayGrouping;

		private SerializedProperty m_SurfaceArc;

		private static readonly AnimBool m_ShowSidesRollout = new AnimBool();

		private SerializedProperty m_UseSideFriction;

		private SerializedProperty m_UseSideBounce;

		private SerializedProperty m_SideArc;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_RotationalOffset = base.serializedObject.FindProperty("m_RotationalOffset");
			this.m_ShowOneWayRollout.value = true;
			this.m_ShowOneWayRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_UseOneWay = base.serializedObject.FindProperty("m_UseOneWay");
			this.m_UseOneWayGrouping = base.serializedObject.FindProperty("m_UseOneWayGrouping");
			this.m_SurfaceArc = base.serializedObject.FindProperty("m_SurfaceArc");
			PlatformEffector2DEditor.m_ShowSidesRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_UseSideFriction = base.serializedObject.FindProperty("m_UseSideFriction");
			this.m_UseSideBounce = base.serializedObject.FindProperty("m_UseSideBounce");
			this.m_SideArc = base.serializedObject.FindProperty("m_SideArc");
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_ShowOneWayRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			PlatformEffector2DEditor.m_ShowSidesRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_RotationalOffset, new GUILayoutOption[0]);
			this.m_ShowOneWayRollout.target = EditorGUILayout.Foldout(this.m_ShowOneWayRollout.target, "One Way", true);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowOneWayRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_UseOneWay, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_UseOneWayGrouping, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_SurfaceArc, new GUILayoutOption[0]);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndFadeGroup();
			PlatformEffector2DEditor.m_ShowSidesRollout.target = EditorGUILayout.Foldout(PlatformEffector2DEditor.m_ShowSidesRollout.target, "Sides", true);
			if (EditorGUILayout.BeginFadeGroup(PlatformEffector2DEditor.m_ShowSidesRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_UseSideFriction, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_UseSideBounce, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_SideArc, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			PlatformEffector2D platformEffector2D = (PlatformEffector2D)base.target;
			if (platformEffector2D.enabled)
			{
				if (platformEffector2D.useOneWay)
				{
					PlatformEffector2DEditor.DrawSurfaceArc(platformEffector2D);
				}
				if (!platformEffector2D.useSideBounce || !platformEffector2D.useSideFriction)
				{
					PlatformEffector2DEditor.DrawSideArc(platformEffector2D);
				}
			}
		}

		private static void DrawSurfaceArc(PlatformEffector2D effector)
		{
			float f = -0.0174532924f * effector.rotationalOffset;
			Vector3 normalized = effector.transform.TransformVector(new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f)).normalized;
			if (normalized.sqrMagnitude >= Mathf.Epsilon)
			{
				float num = Mathf.Atan2(normalized.x, normalized.y);
				float num2 = Mathf.Clamp(effector.surfaceArc, 0.5f, 360f);
				float num3 = num2 * 0.5f * 0.0174532924f;
				Vector3 vector = new Vector3(Mathf.Sin(num - num3), Mathf.Cos(num - num3), 0f);
				Vector3 a = new Vector3(Mathf.Sin(num + num3), Mathf.Cos(num + num3), 0f);
				foreach (Collider2D current in from collider in effector.gameObject.GetComponents<Collider2D>()
				where collider.enabled && collider.usedByEffector
				select collider)
				{
					Vector3 center = current.bounds.center;
					float handleSize = HandleUtility.GetHandleSize(center);
					Handles.color = new Color(0f, 1f, 1f, 0.07f);
					Handles.DrawSolidArc(center, Vector3.back, vector, num2, handleSize);
					Handles.color = new Color(0f, 1f, 1f, 0.7f);
					Handles.DrawWireArc(center, Vector3.back, vector, num2, handleSize);
					Handles.DrawDottedLine(center, center + vector * handleSize, 5f);
					Handles.DrawDottedLine(center, center + a * handleSize, 5f);
				}
			}
		}

		private static void DrawSideArc(PlatformEffector2D effector)
		{
			float f = -0.0174532924f * (90f + effector.rotationalOffset);
			Vector3 normalized = effector.transform.TransformVector(new Vector3(Mathf.Sin(f), Mathf.Cos(f), 0f)).normalized;
			if (normalized.sqrMagnitude >= Mathf.Epsilon)
			{
				float num = Mathf.Atan2(normalized.x, normalized.y);
				float num2 = num + 3.14159274f;
				float num3 = Mathf.Clamp(effector.sideArc, 0.5f, 180f);
				float num4 = num3 * 0.5f * 0.0174532924f;
				Vector3 vector = new Vector3(Mathf.Sin(num - num4), Mathf.Cos(num - num4), 0f);
				Vector3 a = new Vector3(Mathf.Sin(num + num4), Mathf.Cos(num + num4), 0f);
				Vector3 vector2 = new Vector3(Mathf.Sin(num2 - num4), Mathf.Cos(num2 - num4), 0f);
				Vector3 a2 = new Vector3(Mathf.Sin(num2 + num4), Mathf.Cos(num2 + num4), 0f);
				foreach (Collider2D current in from collider in effector.gameObject.GetComponents<Collider2D>()
				where collider.enabled && collider.usedByEffector
				select collider)
				{
					Vector3 center = current.bounds.center;
					float num5 = HandleUtility.GetHandleSize(center) * 0.8f;
					Handles.color = new Color(0f, 1f, 0.7f, 0.07f);
					Handles.DrawSolidArc(center, Vector3.back, vector, num3, num5);
					Handles.DrawSolidArc(center, Vector3.back, vector2, num3, num5);
					Handles.color = new Color(0f, 1f, 0.7f, 0.7f);
					Handles.DrawWireArc(center, Vector3.back, vector, num3, num5);
					Handles.DrawWireArc(center, Vector3.back, vector2, num3, num5);
					Handles.DrawDottedLine(center, center + vector * num5, 5f);
					Handles.DrawDottedLine(center, center + a * num5, 5f);
					Handles.DrawDottedLine(center, center + vector2 * num5, 5f);
					Handles.DrawDottedLine(center, center + a2 * num5, 5f);
				}
			}
		}
	}
}
