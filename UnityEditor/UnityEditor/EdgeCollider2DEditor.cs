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
			base.OnInspectorGUI();
			base.EndColliderInspector();
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
