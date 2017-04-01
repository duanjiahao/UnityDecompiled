using System;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(CompositeCollider2D))]
	internal class CompositeCollider2DEditor : Collider2DEditorBase
	{
		private SerializedProperty m_GeometryType;

		private SerializedProperty m_GenerationType;

		private SerializedProperty m_VertexDistance;

		private SerializedProperty m_EdgeRadius;

		private readonly AnimBool m_ShowEdgeRadius = new AnimBool();

		private readonly AnimBool m_ShowManualGenerationButton = new AnimBool();

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_GeometryType = base.serializedObject.FindProperty("m_GeometryType");
			this.m_GenerationType = base.serializedObject.FindProperty("m_GenerationType");
			this.m_VertexDistance = base.serializedObject.FindProperty("m_VertexDistance");
			this.m_EdgeRadius = base.serializedObject.FindProperty("m_EdgeRadius");
			this.m_ShowEdgeRadius.value = ((from x in base.targets
			where (x as CompositeCollider2D).geometryType == CompositeCollider2D.GeometryType.Polygons
			select x).Count<UnityEngine.Object>() == 0);
			this.m_ShowEdgeRadius.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowManualGenerationButton.value = ((from x in base.targets
			where (x as CompositeCollider2D).generationType != CompositeCollider2D.GenerationType.Manual
			select x).Count<UnityEngine.Object>() == 0);
			this.m_ShowManualGenerationButton.valueChanged.AddListener(new UnityAction(base.Repaint));
		}

		public override void OnDisable()
		{
			this.m_ShowEdgeRadius.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowManualGenerationButton.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			base.OnInspectorGUI();
			EditorGUILayout.PropertyField(this.m_GeometryType, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_GenerationType, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_VertexDistance, new GUILayoutOption[0]);
			this.m_ShowManualGenerationButton.target = ((from x in base.targets
			where (x as CompositeCollider2D).generationType != CompositeCollider2D.GenerationType.Manual
			select x).Count<UnityEngine.Object>() == 0);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowManualGenerationButton.faded))
			{
				if (GUILayout.Button("Regenerate Collider", new GUILayoutOption[0]))
				{
					UnityEngine.Object[] targets = base.targets;
					for (int i = 0; i < targets.Length; i++)
					{
						UnityEngine.Object @object = targets[i];
						(@object as CompositeCollider2D).GenerateGeometry();
					}
				}
			}
			EditorGUILayout.EndFadeGroup();
			this.m_ShowEdgeRadius.target = ((from x in base.targets
			where (x as CompositeCollider2D).geometryType == CompositeCollider2D.GeometryType.Polygons
			select x).Count<UnityEngine.Object>() == 0);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowEdgeRadius.faded))
			{
				EditorGUILayout.PropertyField(this.m_EdgeRadius, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			if ((from x in base.targets
			where (x as CompositeCollider2D).geometryType == CompositeCollider2D.GeometryType.Outlines && (x as CompositeCollider2D).attachedRigidbody != null && (x as CompositeCollider2D).attachedRigidbody.bodyType == RigidbodyType2D.Dynamic
			select x).Count<UnityEngine.Object>() > 0)
			{
				EditorGUILayout.HelpBox("Outline geometry is composed of edges and will not preserve the original collider's center-of-mass or rotational inertia.  The CompositeCollider2D is attached to a Dynamic Rigidbody2D so you may need to explicitly set these if they are required.", MessageType.Info);
			}
			base.serializedObject.ApplyModifiedProperties();
			base.FinalizeInspectorGUI();
		}
	}
}
