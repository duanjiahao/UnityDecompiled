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

		private static bool s_VisualizeBounds = false;

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
			TriggerModuleUI.s_VisualizeBounds = EditorPrefs.GetBool("VisualizeTriggerBounds", false);
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (TriggerModuleUI.s_Texts == null)
			{
				TriggerModuleUI.s_Texts = new TriggerModuleUI.Texts();
			}
			this.DoListOfCollisionShapesGUI();
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.inside, this.m_Inside, TriggerModuleUI.s_Texts.overlapOptions, new GUILayoutOption[0]);
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.outside, this.m_Outside, TriggerModuleUI.s_Texts.overlapOptions, new GUILayoutOption[0]);
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.enter, this.m_Enter, TriggerModuleUI.s_Texts.overlapOptions, new GUILayoutOption[0]);
			ModuleUI.GUIPopup(TriggerModuleUI.s_Texts.exit, this.m_Exit, TriggerModuleUI.s_Texts.overlapOptions, new GUILayoutOption[0]);
			ModuleUI.GUIFloat(TriggerModuleUI.s_Texts.radiusScale, this.m_RadiusScale, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			TriggerModuleUI.s_VisualizeBounds = ModuleUI.GUIToggle(TriggerModuleUI.s_Texts.visualizeBounds, TriggerModuleUI.s_VisualizeBounds, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool("VisualizeTriggerBounds", TriggerModuleUI.s_VisualizeBounds);
			}
		}

		private static GameObject CreateDefaultCollider(string name, ParticleSystem parentOfGameObject)
		{
			GameObject gameObject = new GameObject(name);
			GameObject result;
			if (gameObject)
			{
				if (parentOfGameObject)
				{
					gameObject.transform.parent = parentOfGameObject.transform;
				}
				gameObject.AddComponent<SphereCollider>();
				result = gameObject;
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void DoListOfCollisionShapesGUI()
		{
			if (this.m_ParticleSystemUI.multiEdit)
			{
				for (int i = 0; i < 6; i++)
				{
					int num = -1;
					ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
					for (int j = 0; j < particleSystems.Length; j++)
					{
						ParticleSystem particleSystem = particleSystems[j];
						int num2 = (!(particleSystem.trigger.GetCollider(i) != null)) ? 0 : 1;
						if (num == -1)
						{
							num = num2;
						}
						else if (num2 != num)
						{
							EditorGUILayout.HelpBox("Collider list editing is only available when all selected systems contain the same number of colliders", MessageType.Info, true);
							return;
						}
					}
				}
			}
			int num3 = base.GUIListOfFloatObjectToggleFields(TriggerModuleUI.s_Texts.collisionShapes, this.m_ShownCollisionShapes, null, TriggerModuleUI.s_Texts.createCollisionShape, !this.m_ParticleSystemUI.multiEdit, new GUILayoutOption[0]);
			if (num3 >= 0 && !this.m_ParticleSystemUI.multiEdit)
			{
				GameObject gameObject = TriggerModuleUI.CreateDefaultCollider("Collider " + (num3 + 1), this.m_ParticleSystemUI.m_ParticleSystems[0]);
				gameObject.transform.localPosition = new Vector3(0f, 0f, (float)(10 + num3));
				this.m_ShownCollisionShapes[num3].objectReferenceValue = gameObject;
			}
			Rect rect = GUILayoutUtility.GetRect(0f, 16f);
			rect.x = rect.xMax - 24f - 5f;
			rect.width = 12f;
			if (this.m_ShownCollisionShapes.Length > 1)
			{
				if (ModuleUI.MinusButton(rect))
				{
					this.m_ShownCollisionShapes[this.m_ShownCollisionShapes.Length - 1].objectReferenceValue = null;
					List<SerializedProperty> list = new List<SerializedProperty>(this.m_ShownCollisionShapes);
					list.RemoveAt(list.Count - 1);
					this.m_ShownCollisionShapes = list.ToArray();
				}
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

		public override void OnSceneViewGUI()
		{
			if (TriggerModuleUI.s_VisualizeBounds)
			{
				Color color = Handles.color;
				Handles.color = Color.green;
				Matrix4x4 matrix = Handles.matrix;
				Vector3[] array = new Vector3[20];
				Vector3[] array2 = new Vector3[20];
				Vector3[] array3 = new Vector3[20];
				Handles.SetDiscSectionPoints(array, Vector3.zero, Vector3.forward, Vector3.right, 360f, 1f);
				Handles.SetDiscSectionPoints(array2, Vector3.zero, Vector3.up, -Vector3.right, 360f, 1f);
				Handles.SetDiscSectionPoints(array3, Vector3.zero, Vector3.right, Vector3.up, 360f, 1f);
				Vector3[] array4 = new Vector3[array.Length + array2.Length + array3.Length];
				array.CopyTo(array4, 0);
				array2.CopyTo(array4, 20);
				array3.CopyTo(array4, 40);
				ParticleSystem[] particleSystems = this.m_ParticleSystemUI.m_ParticleSystems;
				for (int i = 0; i < particleSystems.Length; i++)
				{
					ParticleSystem particleSystem = particleSystems[i];
					ParticleSystem.Particle[] array5 = new ParticleSystem.Particle[particleSystem.particleCount];
					int particles = particleSystem.GetParticles(array5);
					Matrix4x4 lhs = Matrix4x4.identity;
					if (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
					{
						lhs = particleSystem.GetLocalToWorldMatrix();
					}
					for (int j = 0; j < particles; j++)
					{
						ParticleSystem.Particle particle = array5[j];
						Vector3 currentSize3D = particle.GetCurrentSize3D(particleSystem);
						float num = Math.Max(currentSize3D.x, Math.Max(currentSize3D.y, currentSize3D.z)) * 0.5f * particleSystem.trigger.radiusScale;
						Handles.matrix = lhs * Matrix4x4.TRS(particle.position, Quaternion.identity, new Vector3(num, num, num));
						Handles.DrawPolyLine(array4);
					}
				}
				Handles.color = color;
				Handles.matrix = matrix;
			}
		}
	}
}
