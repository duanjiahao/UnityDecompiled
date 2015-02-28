using System;
using UnityEngine;
namespace UnityEditor
{
	internal class Collider2DEditorBase : ColliderEditorBase
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			this.CheckColliderErrorState();
			Effector2DEditor.CheckEffectorWarnings(this.target as Collider2D);
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
		protected void BeginColliderInspector()
		{
			base.serializedObject.Update();
			EditorGUI.BeginDisabledGroup(base.targets.Length > 1);
			base.InspectorEditButtonGUI();
			EditorGUI.EndDisabledGroup();
		}
		protected void EndColliderInspector()
		{
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
