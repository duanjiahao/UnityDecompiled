using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(MeshCollider))]
	internal class MeshColliderEditor : Collider3DEditorBase
	{
		private SerializedProperty m_Mesh;
		private SerializedProperty m_Convex;
		private SerializedProperty m_SmoothSphereCollisions;
		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Mesh = base.serializedObject.FindProperty("m_Mesh");
			this.m_Convex = base.serializedObject.FindProperty("m_Convex");
			this.m_SmoothSphereCollisions = base.serializedObject.FindProperty("m_SmoothSphereCollisions");
		}
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Convex, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SmoothSphereCollisions, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Mesh, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
