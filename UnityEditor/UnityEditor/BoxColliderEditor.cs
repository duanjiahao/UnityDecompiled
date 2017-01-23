using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BoxCollider))]
	internal class BoxColliderEditor : Collider3DEditorBase
	{
		private static readonly int s_BoxHash = "BoxColliderEditor".GetHashCode();

		private SerializedProperty m_Center;

		private SerializedProperty m_Size;

		private readonly BoxEditor m_BoxEditor = new BoxEditor(true, BoxColliderEditor.s_BoxHash);

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Center = base.serializedObject.FindProperty("m_Center");
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_BoxEditor.OnDisable();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.target;
				Vector3 center = boxCollider.center;
				Vector3 size = boxCollider.size;
				Color color = Handles.s_ColliderHandleColor;
				if (!boxCollider.enabled)
				{
					color = Handles.s_ColliderHandleColorDisabled;
				}
				if (this.m_BoxEditor.OnSceneGUI(boxCollider.transform, color, ref center, ref size))
				{
					Undo.RecordObject(boxCollider, "Modify Box Collider");
					boxCollider.center = center;
					boxCollider.size = size;
				}
			}
		}
	}
}
