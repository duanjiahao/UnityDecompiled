using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	internal class Collider2DEditorBase : ColliderEditorBase
	{
		private SerializedProperty m_Density;

		private readonly AnimBool m_ShowDensity = new AnimBool();

		private readonly AnimBool m_ShowInfo = new AnimBool();

		private SerializedProperty m_Material;

		private SerializedProperty m_IsTrigger;

		private SerializedProperty m_UsedByEffector;

		private SerializedProperty m_Offset;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Density = base.serializedObject.FindProperty("m_Density");
			this.m_ShowDensity.value = this.ShouldShowDensity();
			this.m_ShowDensity.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowInfo.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_Material = base.serializedObject.FindProperty("m_Material");
			this.m_IsTrigger = base.serializedObject.FindProperty("m_IsTrigger");
			this.m_UsedByEffector = base.serializedObject.FindProperty("m_UsedByEffector");
			this.m_Offset = base.serializedObject.FindProperty("m_Offset");
		}

		public override void OnDisable()
		{
			this.m_ShowDensity.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowInfo.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			base.OnDisable();
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			this.m_ShowDensity.target = this.ShouldShowDensity();
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

		public void FinalizeInspectorGUI()
		{
			this.ShowColliderInfoProperties();
			this.CheckColliderErrorState();
			Effector2DEditor.CheckEffectorWarnings(base.target as Collider2D);
		}

		private void ShowColliderInfoProperties()
		{
			this.m_ShowInfo.target = EditorGUILayout.Foldout(this.m_ShowInfo.target, "Info", true);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowInfo.faded))
			{
				if (base.targets.Length == 1)
				{
					Collider2D collider2D = base.targets[0] as Collider2D;
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.ObjectField("Attached Body", collider2D.attachedRigidbody, typeof(Rigidbody2D), false, new GUILayoutOption[0]);
					EditorGUILayout.FloatField("Friction", collider2D.friction, new GUILayoutOption[0]);
					EditorGUILayout.FloatField("Bounciness", collider2D.bounciness, new GUILayoutOption[0]);
					EditorGUILayout.FloatField("Shape Count", (float)collider2D.shapeCount, new GUILayoutOption[0]);
					if (collider2D.isActiveAndEnabled)
					{
						EditorGUILayout.BoundsField("Bounds", collider2D.bounds, new GUILayoutOption[0]);
					}
					EditorGUI.EndDisabledGroup();
					base.Repaint();
				}
				else
				{
					EditorGUILayout.HelpBox("Cannot show Info properties when multiple colliders are selected.", MessageType.Info);
				}
			}
			EditorGUILayout.EndFadeGroup();
		}

		private bool ShouldShowDensity()
		{
			bool result;
			if ((from x in base.targets
			select (x as Collider2D).attachedRigidbody).Distinct<Rigidbody2D>().Count<Rigidbody2D>() > 1)
			{
				result = false;
			}
			else
			{
				Rigidbody2D attachedRigidbody = (base.target as Collider2D).attachedRigidbody;
				result = (attachedRigidbody && attachedRigidbody.useAutoMass && attachedRigidbody.bodyType == RigidbodyType2D.Dynamic);
			}
			return result;
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
			ColliderErrorState2D errorState = (base.target as Collider2D).errorState;
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
