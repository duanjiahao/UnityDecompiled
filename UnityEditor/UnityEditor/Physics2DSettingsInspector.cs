using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CustomEditor(typeof(Physics2DSettings))]
	internal class Physics2DSettingsInspector : Editor
	{
		private Vector2 m_LayerCollisionMatrixScrollPos;

		private bool m_ShowLayerCollisionMatrix = true;

		private static bool s_ShowGizmoSettings;

		private readonly AnimBool m_GizmoSettingsFade = new AnimBool();

		private SerializedProperty m_AlwaysShowColliders;

		private SerializedProperty m_ShowColliderSleep;

		private SerializedProperty m_ShowColliderContacts;

		private SerializedProperty m_ContactArrowScale;

		private SerializedProperty m_ColliderAwakeColor;

		private SerializedProperty m_ColliderAsleepColor;

		private SerializedProperty m_ColliderContactColor;

		public void OnEnable()
		{
			this.m_AlwaysShowColliders = base.serializedObject.FindProperty("m_AlwaysShowColliders");
			this.m_ShowColliderSleep = base.serializedObject.FindProperty("m_ShowColliderSleep");
			this.m_ShowColliderContacts = base.serializedObject.FindProperty("m_ShowColliderContacts");
			this.m_ContactArrowScale = base.serializedObject.FindProperty("m_ContactArrowScale");
			this.m_ColliderAwakeColor = base.serializedObject.FindProperty("m_ColliderAwakeColor");
			this.m_ColliderAsleepColor = base.serializedObject.FindProperty("m_ColliderAsleepColor");
			this.m_ColliderContactColor = base.serializedObject.FindProperty("m_ColliderContactColor");
			this.m_GizmoSettingsFade.value = Physics2DSettingsInspector.s_ShowGizmoSettings;
			this.m_GizmoSettingsFade.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public void OnDisable()
		{
			this.m_GizmoSettingsFade.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.DrawDefaultInspector();
			Physics2DSettingsInspector.s_ShowGizmoSettings = EditorGUILayout.Foldout(Physics2DSettingsInspector.s_ShowGizmoSettings, "Gizmos");
			this.m_GizmoSettingsFade.target = Physics2DSettingsInspector.s_ShowGizmoSettings;
			if (this.m_GizmoSettingsFade.value)
			{
				base.serializedObject.Update();
				if (EditorGUILayout.BeginFadeGroup(this.m_GizmoSettingsFade.faded))
				{
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(this.m_AlwaysShowColliders, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ShowColliderSleep, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ColliderAwakeColor, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ColliderAsleepColor, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ShowColliderContacts, new GUILayoutOption[0]);
					EditorGUILayout.Slider(this.m_ContactArrowScale, 0.1f, 1f, this.m_ContactArrowScale.displayName, new GUILayoutOption[0]);
					EditorGUILayout.PropertyField(this.m_ColliderContactColor, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndFadeGroup();
				base.serializedObject.ApplyModifiedProperties();
			}
			LayerMatrixGUI.DoGUI("Layer Collision Matrix", ref this.m_ShowLayerCollisionMatrix, ref this.m_LayerCollisionMatrixScrollPos, new LayerMatrixGUI.GetValueFunc(Physics2DSettingsInspector.GetValue), new LayerMatrixGUI.SetValueFunc(Physics2DSettingsInspector.SetValue));
		}

		private static bool GetValue(int layerA, int layerB)
		{
			return !Physics2D.GetIgnoreLayerCollision(layerA, layerB);
		}

		private static void SetValue(int layerA, int layerB, bool val)
		{
			Physics2D.IgnoreLayerCollision(layerA, layerB, !val);
		}
	}
}
