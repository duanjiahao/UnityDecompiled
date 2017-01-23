using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CapsuleCollider2D))]
	internal class CapsuleCollider2DEditor : Collider2DEditorBase
	{
		private static readonly int k_CapsuleHash = "CapsuleCollider2DEditor".GetHashCode();

		private readonly BoxEditor m_BoxEditor = new BoxEditor(true, CapsuleCollider2DEditor.k_CapsuleHash, true);

		private SerializedProperty m_Size;

		private SerializedProperty m_Direction;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
			this.m_Size = base.serializedObject.FindProperty("m_Size");
			this.m_Direction = base.serializedObject.FindProperty("m_Direction");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Direction, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
			base.FinalizeInspectorGUI();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_BoxEditor.OnDisable();
		}

		public void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)base.target;
				Vector3 v = capsuleCollider2D.offset;
				Vector3 v2 = capsuleCollider2D.size;
				if (this.m_BoxEditor.OnSceneGUI(capsuleCollider2D.transform, Handles.s_ColliderHandleColor, ref v, ref v2))
				{
					Undo.RecordObject(capsuleCollider2D, "Modify collider");
					capsuleCollider2D.offset = v;
					capsuleCollider2D.size = v2;
				}
			}
		}
	}
}
