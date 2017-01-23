using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PointEffector2D), true)]
	internal class PointEffector2DEditor : Effector2DEditor
	{
		private readonly AnimBool m_ShowForceRollout = new AnimBool();

		private SerializedProperty m_ForceMagnitude;

		private SerializedProperty m_ForceVariation;

		private SerializedProperty m_ForceSource;

		private SerializedProperty m_ForceTarget;

		private SerializedProperty m_ForceMode;

		private SerializedProperty m_DistanceScale;

		private static readonly AnimBool m_ShowDampingRollout = new AnimBool();

		private SerializedProperty m_Drag;

		private SerializedProperty m_AngularDrag;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_ShowForceRollout.value = true;
			this.m_ShowForceRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ForceMagnitude = base.serializedObject.FindProperty("m_ForceMagnitude");
			this.m_ForceVariation = base.serializedObject.FindProperty("m_ForceVariation");
			this.m_ForceSource = base.serializedObject.FindProperty("m_ForceSource");
			this.m_ForceTarget = base.serializedObject.FindProperty("m_ForceTarget");
			this.m_ForceMode = base.serializedObject.FindProperty("m_ForceMode");
			this.m_DistanceScale = base.serializedObject.FindProperty("m_DistanceScale");
			PointEffector2DEditor.m_ShowDampingRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Drag = base.serializedObject.FindProperty("m_Drag");
			this.m_AngularDrag = base.serializedObject.FindProperty("m_AngularDrag");
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_ShowForceRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			PointEffector2DEditor.m_ShowDampingRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			base.serializedObject.Update();
			this.m_ShowForceRollout.target = EditorGUILayout.Foldout(this.m_ShowForceRollout.target, "Force", true);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowForceRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_ForceMagnitude, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ForceVariation, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_DistanceScale, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ForceSource, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ForceTarget, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ForceMode, new GUILayoutOption[0]);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndFadeGroup();
			PointEffector2DEditor.m_ShowDampingRollout.target = EditorGUILayout.Foldout(PointEffector2DEditor.m_ShowDampingRollout.target, "Damping", true);
			if (EditorGUILayout.BeginFadeGroup(PointEffector2DEditor.m_ShowDampingRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_Drag, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AngularDrag, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
