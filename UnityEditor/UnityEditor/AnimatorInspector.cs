using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(Animator))]
	internal class AnimatorInspector : Editor
	{
		private SerializedProperty m_Controller;
		private SerializedProperty m_Avatar;
		private SerializedProperty m_ApplyRootMotion;
		private SerializedProperty m_UpdateMode;
		private SerializedProperty m_CullingMode;
		private void OnEnable()
		{
			this.m_Controller = base.serializedObject.FindProperty("m_Controller");
			this.m_Avatar = base.serializedObject.FindProperty("m_Avatar");
			this.m_ApplyRootMotion = base.serializedObject.FindProperty("m_ApplyRootMotion");
			this.m_UpdateMode = base.serializedObject.FindProperty("m_UpdateMode");
			this.m_CullingMode = base.serializedObject.FindProperty("m_CullingMode");
		}
		public override void OnInspectorGUI()
		{
			bool flag = base.targets.Length > 1;
			Animator animator = this.target as Animator;
			base.serializedObject.UpdateIfDirtyOrScript();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_Controller, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				EditorApplication.RepaintAnimationWindow();
			}
			EditorGUILayout.PropertyField(this.m_Avatar, new GUILayoutOption[0]);
			if (animator.supportsOnAnimatorMove && !flag)
			{
				EditorGUILayout.LabelField("Apply Root Motion", "Handled by Script", new GUILayoutOption[0]);
			}
			else
			{
				EditorGUILayout.PropertyField(this.m_ApplyRootMotion, new GUILayoutOption[0]);
			}
			EditorGUILayout.PropertyField(this.m_UpdateMode, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_CullingMode, new GUILayoutOption[0]);
			if (!flag)
			{
				EditorGUILayout.HelpBox(animator.GetStats(), MessageType.Info, true);
			}
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
