using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BoxCollider2D))]
	internal class BoxCollider2DEditor : Collider2DEditorBase
	{
		private static readonly int s_BoxHash = "BoxCollider2DEditor".GetHashCode();

		private readonly BoxEditor m_BoxEditor = new BoxEditor(true, BoxCollider2DEditor.s_BoxHash, true);

		private SerializedProperty m_Size;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_BoxEditor.OnEnable();
			this.m_BoxEditor.SetAlwaysDisplayHandles(true);
			this.m_Size = base.serializedObject.FindProperty("m_Size");
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.InspectorEditButtonGUI();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
			base.serializedObject.ApplyModifiedProperties();
			base.CheckAllErrorsAndWarnings();
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_BoxEditor.OnDisable();
		}

		public void OnSceneGUI()
		{
			if (!base.editingCollider)
			{
				return;
			}
			BoxCollider2D boxCollider2D = (BoxCollider2D)this.target;
			Vector3 v = boxCollider2D.offset;
			Vector3 v2 = boxCollider2D.size;
			if (this.m_BoxEditor.OnSceneGUI(boxCollider2D.transform, Handles.s_ColliderHandleColor, ref v, ref v2))
			{
				Undo.RecordObject(boxCollider2D, "Modify collider");
				boxCollider2D.offset = v;
				boxCollider2D.size = v2;
			}
		}
	}
}
