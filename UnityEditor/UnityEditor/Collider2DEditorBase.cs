using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	internal class Collider2DEditorBase : ColliderEditorBase
	{
		private SerializedProperty m_Density;

		private readonly AnimBool m_ShowDensity = new AnimBool();

		private SerializedProperty m_Material;

		private SerializedProperty m_IsTrigger;

		private SerializedProperty m_UsedByEffector;

		private SerializedProperty m_Offset;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Density = base.serializedObject.FindProperty("m_Density");
			Collider2D collider2D = this.target as Collider2D;
			this.m_ShowDensity.value = (collider2D.attachedRigidbody && collider2D.attachedRigidbody.useAutoMass);
			this.m_ShowDensity.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_IsTrigger = base.serializedObject.FindProperty("m_IsTrigger");
			this.m_UsedByEffector = base.serializedObject.FindProperty("m_UsedByEffector");
			this.m_Offset = base.serializedObject.FindProperty("m_Offset");
		}

		public override void OnDisable()
		{
			this.m_ShowDensity.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			base.OnDisable();
		}

		public override void OnInspectorGUI()
		{
			Collider2D collider2D = this.target as Collider2D;
			base.serializedObject.Update();
			this.m_ShowDensity.target = (collider2D.attachedRigidbody && collider2D.attachedRigidbody.useAutoMass);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowDensity.faded))
			{
				EditorGUILayout.PropertyField(this.m_Density, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.PropertyField(this.m_Material, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_IsTrigger, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_UsedByEffector, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_Offset, new GUILayoutOption[0]);
		}

		public void CheckAllErrorsAndWarnings()
		{
			this.CheckColliderErrorState();
			Effector2DEditor.CheckEffectorWarnings(this.target as Collider2D);
		}

		internal override void OnForceReloadInspector()
		{
			base.OnForceReloadInspector();
			if (base.editingCollider)
			{
				base.ForceQuitEditMode();
			}
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
			using (new EditorGUI.DisabledScope(base.targets.Length > 1))
			{
				base.InspectorEditButtonGUI();
			}
		}

		protected void EndColliderInspector()
		{
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}
