using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(SurfaceEffector2D), true)]
	internal class SurfaceEffector2DEditor : Effector2DEditor
	{
		private readonly AnimBool m_ShowForceRollout = new AnimBool();

		private SerializedProperty m_Speed;

		private SerializedProperty m_SpeedVariation;

		private SerializedProperty m_ForceScale;

		private static readonly AnimBool m_ShowOptionsRollout = new AnimBool();

		private SerializedProperty m_UseContactForce;

		private SerializedProperty m_UseFriction;

		private SerializedProperty m_UseBounce;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_ShowForceRollout.value = true;
			this.m_ShowForceRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Speed = base.serializedObject.FindProperty("m_Speed");
			this.m_SpeedVariation = base.serializedObject.FindProperty("m_SpeedVariation");
			this.m_ForceScale = base.serializedObject.FindProperty("m_ForceScale");
			SurfaceEffector2DEditor.m_ShowOptionsRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_UseContactForce = base.serializedObject.FindProperty("m_UseContactForce");
			this.m_UseFriction = base.serializedObject.FindProperty("m_UseFriction");
			this.m_UseBounce = base.serializedObject.FindProperty("m_UseBounce");
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_ShowForceRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			SurfaceEffector2DEditor.m_ShowOptionsRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			base.serializedObject.Update();
			this.m_ShowForceRollout.target = EditorGUILayout.Foldout(this.m_ShowForceRollout.target, "Force", true);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowForceRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_Speed, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_SpeedVariation, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_ForceScale, new GUILayoutOption[0]);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndFadeGroup();
			SurfaceEffector2DEditor.m_ShowOptionsRollout.target = EditorGUILayout.Foldout(SurfaceEffector2DEditor.m_ShowOptionsRollout.target, "Options", true);
			if (EditorGUILayout.BeginFadeGroup(SurfaceEffector2DEditor.m_ShowOptionsRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_UseContactForce, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_UseFriction, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_UseBounce, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
