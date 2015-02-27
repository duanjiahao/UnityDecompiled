using System;
using UnityEngine;
namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(EdgeCollider2D))]
	internal class EdgeCollider2DEditor : Collider2DEditorBase
	{
		private PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();
		private bool m_ShowColliderInfo;
		public override void OnEnable()
		{
			base.OnEnable();
		}
		public override void OnInspectorGUI()
		{
			base.BeginColliderInspector();
			this.ColliderInfoGUI();
			base.EndColliderInspector();
			base.CheckColliderErrorState();
		}
		private void ColliderInfoGUI()
		{
			EditorGUI.BeginDisabledGroup(base.targets.Length != 1);
			this.m_ShowColliderInfo = EditorGUILayout.Foldout(this.m_ShowColliderInfo, "Collider Info");
			if (this.m_ShowColliderInfo)
			{
				EdgeCollider2D edgeCollider2D = base.targets[0] as EdgeCollider2D;
				if (edgeCollider2D)
				{
					int pointCount = edgeCollider2D.pointCount;
					string label = (!GUI.enabled) ? "---" : (string.Empty + pointCount);
					EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("Vertices", label, new GUILayoutOption[0]);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.EndDisabledGroup();
		}
		protected override void OnEditStart()
		{
			this.m_PolyUtility.StartEditing(this.target as Collider2D);
		}
		protected override void OnEditEnd()
		{
			this.m_PolyUtility.StopEditing();
		}
		public void OnSceneGUI()
		{
			if (!base.editingCollider)
			{
				return;
			}
			this.m_PolyUtility.OnSceneGUI();
		}
	}
}
