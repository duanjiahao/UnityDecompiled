using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BuoyancyEffector2D), true)]
	internal class BuoyancyEffector2DEditor : Effector2DEditor
	{
		private SerializedProperty m_Density;

		private SerializedProperty m_SurfaceLevel;

		private static readonly AnimBool m_ShowDampingRollout = new AnimBool();

		private SerializedProperty m_LinearDrag;

		private SerializedProperty m_AngularDrag;

		private static readonly AnimBool m_ShowFlowRollout = new AnimBool();

		private SerializedProperty m_FlowAngle;

		private SerializedProperty m_FlowMagnitude;

		private SerializedProperty m_FlowVariation;

		public override void OnEnable()
		{
			base.OnEnable();
			this.m_Density = base.serializedObject.FindProperty("m_Density");
			this.m_SurfaceLevel = base.serializedObject.FindProperty("m_SurfaceLevel");
			BuoyancyEffector2DEditor.m_ShowDampingRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_LinearDrag = base.serializedObject.FindProperty("m_LinearDrag");
			this.m_AngularDrag = base.serializedObject.FindProperty("m_AngularDrag");
			BuoyancyEffector2DEditor.m_ShowFlowRollout.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_FlowAngle = base.serializedObject.FindProperty("m_FlowAngle");
			this.m_FlowMagnitude = base.serializedObject.FindProperty("m_FlowMagnitude");
			this.m_FlowVariation = base.serializedObject.FindProperty("m_FlowVariation");
		}

		public override void OnDisable()
		{
			base.OnDisable();
			BuoyancyEffector2DEditor.m_ShowDampingRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			BuoyancyEffector2DEditor.m_ShowFlowRollout.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_Density, new GUILayoutOption[0]);
			EditorGUILayout.PropertyField(this.m_SurfaceLevel, new GUILayoutOption[0]);
			BuoyancyEffector2DEditor.m_ShowDampingRollout.target = EditorGUILayout.Foldout(BuoyancyEffector2DEditor.m_ShowDampingRollout.target, "Damping", true);
			if (EditorGUILayout.BeginFadeGroup(BuoyancyEffector2DEditor.m_ShowDampingRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_LinearDrag, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_AngularDrag, new GUILayoutOption[0]);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndFadeGroup();
			BuoyancyEffector2DEditor.m_ShowFlowRollout.target = EditorGUILayout.Foldout(BuoyancyEffector2DEditor.m_ShowFlowRollout.target, "Flow", true);
			if (EditorGUILayout.BeginFadeGroup(BuoyancyEffector2DEditor.m_ShowFlowRollout.faded))
			{
				EditorGUILayout.PropertyField(this.m_FlowAngle, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_FlowMagnitude, new GUILayoutOption[0]);
				EditorGUILayout.PropertyField(this.m_FlowVariation, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			base.serializedObject.ApplyModifiedProperties();
		}

		public void OnSceneGUI()
		{
			BuoyancyEffector2D buoyancyEffector2D = (BuoyancyEffector2D)base.target;
			if (buoyancyEffector2D.enabled)
			{
				float y = buoyancyEffector2D.transform.position.y + buoyancyEffector2D.transform.lossyScale.y * buoyancyEffector2D.surfaceLevel;
				List<Vector3> list = new List<Vector3>();
				float num = float.NegativeInfinity;
				float num2 = num;
				foreach (Collider2D current in from c in buoyancyEffector2D.gameObject.GetComponents<Collider2D>()
				where c.enabled && c.usedByEffector
				select c)
				{
					Bounds bounds = current.bounds;
					float x = bounds.min.x;
					float x2 = bounds.max.x;
					if (float.IsNegativeInfinity(num))
					{
						num = x;
						num2 = x2;
					}
					else
					{
						if (x < num)
						{
							num = x;
						}
						if (x2 > num2)
						{
							num2 = x2;
						}
					}
					Vector3 item = new Vector3(x, y, 0f);
					Vector3 item2 = new Vector3(x2, y, 0f);
					list.Add(item);
					list.Add(item2);
				}
				Handles.color = Color.red;
				Handles.DrawAAPolyLine(new Vector3[]
				{
					new Vector3(num, y, 0f),
					new Vector3(num2, y, 0f)
				});
				Handles.color = Color.cyan;
				for (int i = 0; i < list.Count - 1; i += 2)
				{
					Handles.DrawAAPolyLine(new Vector3[]
					{
						list[i],
						list[i + 1]
					});
				}
			}
		}
	}
}
