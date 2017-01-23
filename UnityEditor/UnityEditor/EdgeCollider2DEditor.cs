using System;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(EdgeCollider2D))]
	internal class EdgeCollider2DEditor : Collider2DEditorBase
	{
		private PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();

		private bool m_ShowColliderInfo;

		private SerializedProperty m_Points;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Points = base.serializedObject.FindProperty("m_Points");
			this.m_Points.isExpanded = false;
		}

		public override void OnInspectorGUI()
		{
			base.BeginColliderInspector();
			base.OnInspectorGUI();
			if (base.targets.Length == 1)
			{
				EditorGUI.BeginDisabledGroup(base.editingCollider);
				EditorGUILayout.PropertyField(this.m_Points, true, new GUILayoutOption[0]);
				EditorGUI.EndDisabledGroup();
			}
			base.EndColliderInspector();
			base.FinalizeInspectorGUI();
		}

		protected override void OnEditStart()
		{
			this.m_PolyUtility.StartEditing(base.target as Collider2D);
		}

		protected override void OnEditEnd()
		{
			this.m_PolyUtility.StopEditing();
		}

		public void OnSceneGUI()
		{
			if (base.editingCollider)
			{
				this.m_PolyUtility.OnSceneGUI();
			}
		}
	}
}
