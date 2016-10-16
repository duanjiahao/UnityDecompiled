using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class TriggerModuleUI : ModuleUI
	{
		private enum OverlapOptions
		{
			Ignore,
			Kill,
			Callback
		}

		private class Texts
		{
			public GUIContent collisionShapes = EditorGUIUtility.TextContent("Colliders|The list of collision shapes to use for the trigger.");

			public GUIContent createCollisionShape = EditorGUIUtility.TextContent("|Create a GameObject containing a sphere collider and assigns it to the list.");

			public GUIContent inside = EditorGUIUtility.TextContent("Inside|What to do for particles that are inside the collision volume.");

			public GUIContent outside = EditorGUIUtility.TextContent("Outside|What to do for particles that are outside the collision volume.");

			public GUIContent enter = EditorGUIUtility.TextContent("Enter|Triggered once when particles enter the collison volume.");

			public GUIContent exit = EditorGUIUtility.TextContent("Exit|Triggered once when particles leave the collison volume.");

			public GUIContent radiusScale = EditorGUIUtility.TextContent("Radius Scale|Scale particle bounds by this amount to get more precise collisions.");

			public GUIContent visualizeBounds = EditorGUIUtility.TextContent("Visualize Bounds|Render the collision bounds of the particles.");

			public string[] overlapOptions = new string[]
			{
				"Ignore",
				"Kill",
				"Callback"
			};
		}

		private const int k_MaxNumCollisionShapes = 6;

		private SerializedProperty[] m_CollisionShapes = new SerializedProperty[6];

		private SerializedProperty m_Inside;

		private SerializedProperty m_Outside;

		private SerializedProperty m_Enter;

		private SerializedProperty m_Exit;

		private SerializedProperty m_RadiusScale;

		private SerializedProperty[] m_ShownCollisionShapes;

		private bool m_VisualizeBounds;

		private static TriggerModuleUI s_LastInteractedEditor;

		private static TriggerModuleUI.Texts s_Texts;

		public TriggerModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "TriggerModule", displayName)
		{
			this.m_ToolTip = "Allows you to execute script code based on whether particles are inside or outside the collision shapes.";
		}

		protected override void Init()
		{
			if (TriggerModuleUI.s_Texts == null)
			{
				TriggerModuleUI.s_Texts = new TriggerModuleUI.Texts();
			}
			List<SerializedProperty> list = new List<SerializedProperty>();
			for (int i = 0; i < this.m_CollisionShapes.Length; i++)
			{
				this.m_CollisionShapes[i] = base.GetProperty("collisionShape" + i);
				if (i == 0 || this.m_CollisionShapes[i].objectReferenceValue != null)
				{
					list.Add(this.m_CollisionShapes[i]);
				}
			}
			this.m_ShownCollisionShapes = list.ToArray();
			this.m_Inside = base.GetProperty("inside");
			this.m_Outside = base.GetProperty("outside");
			this.m_Enter = base.GetProperty("enter");
			this.m_Exit = base.GetProperty("exit");
			this.m_RadiusScale = base.GetProperty("radiusScale");
			this.m_VisualizeBounds = EditorPrefs.GetBool("VisualizeTriggerBounds", false);
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (TriggerModuleUI.s_Texts == null)
			{
				TriggerModuleUI.s_Texts = new TriggerModuleUI.Texts();
			}
			this.DoListOfCollisionShapesGUI();
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.inside, this.m_Inside, TriggerModuleUI.s_Texts.overlapOptions);
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.outside, this.m_Outside, TriggerModuleUI.s_Texts.overlapOptions);
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.enter, this.m_Enter, TriggerModuleUI.s_Texts.overlapOptions);
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.exit, this.m_Exit, TriggerModuleUI.s_Texts.overlapOptions);
			ModuleUI.GUIFloat(TriggerModuleUI.s_Texts.radiusScale, this.m_RadiusScale);
			EditorGUI.BeginChangeCheck();
			this.m_VisualizeBounds = ModuleUI.GUIToggle(TriggerModuleUI.s_Texts.visualizeBounds, this.m_VisualizeBounds);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("VisualizeTriggerBounds", this.m_VisualizeBounds);
			}
			TriggerModuleUI.s_LastInteractedEditor = this;
		}

		private static GameObject CreateDefaultCollider(string name, ParticleSystem parentOfGameObject)
		{
			GameObject gameObject = new GameObject(name);
			if (gameObject)
			{
				if (parentOfGameObject)
				{
					gameObject.transform.parent = parentOfGameObject.transform;
				}
				gameObject.AddComponent<SphereCollider>();
				return gameObject;
			}
			return null;
		}

		private void DoListOfCollisionShapesGUI()
		{
			int num = base.GUIListOfFloatObjectToggleFields(TriggerModuleUI.s_Texts.collisionShapes, this.m_ShownCollisionShapes, null, TriggerModuleUI.s_Texts.createCollisionShape, true);
			if (num >= 0)
			{
				GameObject gameObject = TriggerModuleUI.CreateDefaultCollider("Collider " + (num + 1), this.m_ParticleSystemUI.m_ParticleSystem);
				gameObject.transform.localPosition = new Vector3(0f, 0f, (float)(10 + num));
				this.m_ShownCollisionShapes[num].objectReferenceValue = gameObject;
			}
			Rect rect = GUILayoutUtility.GetRect(0f, 16f);
			rect.x = rect.xMax - 24f - 5f;
			rect.width = 12f;
			if (this.m_ShownCollisionShapes.Length > 1 && ModuleUI.MinusButton(rect))
			{
				this.m_ShownCollisionShapes[this.m_ShownCollisionShapes.Length - 1].objectReferenceValue = null;
				List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownCollisionShapes);
				list.RemoveAt(list.Count - 1);
				this.m_ShownCollisionShapes = list.ToArray();
			}
			if (this.m_ShownCollisionShapes.Length < 6)
			{
				rect.x += 17f;
				if (ModuleUI.PlusButton(rect))
				{
					List<SerializedProperty> list2 = new List<SerializedProperty>(this.m_ShownCollisionShapes);
					list2.Add(this.m_CollisionShapes[list2.Count]);
					this.m_ShownCollisionShapes = list2.ToArray();
				}
			}
		}

		[DrawGizmo(GizmoType.Active)]
		private static void RenderCollisionBounds(ParticleSystem system, GizmoType gizmoType)
		{
			if (TriggerModuleUI.s_LastInteractedEditor == null)
			{
				return;
			}
			if (!TriggerModuleUI.s_LastInteractedEditor.enabled)
			{
				return;
			}
			if (!TriggerModuleUI.s_LastInteractedEditor.m_VisualizeBounds)
			{
				return;
			}
			if (TriggerModuleUI.s_LastInteractedEditor.m_ParticleSystemUI.m_ParticleSystem != system)
			{
				return;
			}
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[system.particleCount];
			int particles = system.GetParticles(array);
			Color color = Gizmos.color;
			Gizmos.color = Color.green;
			Matrix4x4 matrix = Matrix4x4.identity;
			if (system.simulationSpace == ParticleSystemSimulationSpace.Local)
			{
				matrix = system.GetLocalToWorldMatrix();
			}
			Matrix4x4 matrix2 = Gizmos.matrix;
			Gizmos.matrix = matrix;
			for (int i = 0; i < particles; i++)
			{
				ParticleSystem.Particle particle = array[i];
				Vector3 currentSize3D = particle.GetCurrentSize3D(system);
				Gizmos.DrawWireSphere(particle.position, Math.Max(currentSize3D.x, Math.Max(currentSize3D.y, currentSize3D.z)) * 0.5f * TriggerModuleUI.s_LastInteractedEditor.m_RadiusScale.floatValue);
			}
			Gizmos.color = color;
			Gizmos.matrix = matrix2;
		}
	}
}
