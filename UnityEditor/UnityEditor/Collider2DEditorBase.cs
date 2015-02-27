using System;
using UnityEngine;
namespace UnityEditor
{
	internal class Collider2DEditorBase : ColliderEditorBase
	{
		protected SerializedProperty m_Material;
		protected SerializedProperty m_IsTrigger;
		public virtual void OnEnable()
		{
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_IsTrigger = base.serializedObject.FindProperty("m_IsTrigger");
		}
		public virtual void OnDisable()
		{
			base.editingCollider = false;
		}
		protected void BeginColliderInspector()
		{
			base.serializedObject.Update();
			EditorGUI.BeginDisabledGroup(base.targets.Length > 1);
			base.InspectorEditButtonGUI();
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
		}
		protected void EndColliderInspector()
		{
			base.serializedObject.ApplyModifiedProperties();
		}
		protected void CheckColliderErrorState()
		{
			ColliderErrorState2D errorState = (this.target as Collider2D).errorState;
			if (errorState != ColliderErrorState2D.NoShapes)
			{
				if (errorState == ColliderErrorState2D.RemovedShapes)
				{
					EditorGUILayout.HelpBox("The collider created collision shape(s) but some were removed as they failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("The collider did not create any collision shapes as they all failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
			}
		}
	}
}
