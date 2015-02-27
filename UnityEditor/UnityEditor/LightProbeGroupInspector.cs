using System;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(LightProbeGroup))]
	internal class LightProbeGroupInspector : Editor
	{
		private LightProbeGroupEditor m_Editor;
		private bool m_EditingProbes;
		private bool m_ShouldFocus;
		public void OnEnable()
		{
			this.m_Editor = new LightProbeGroupEditor(this.target as LightProbeGroup);
			this.m_Editor.PullProbePositions();
			this.m_Editor.DeselectProbes();
			this.m_Editor.PushProbePositions();
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUIDelegate));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}
		private void StartEditProbes()
		{
			if (this.m_EditingProbes)
			{
				return;
			}
			this.m_EditingProbes = true;
			this.m_Editor.SetEditing(true);
			Tools.s_Hidden = true;
			SceneView.RepaintAll();
		}
		private void EndEditProbes()
		{
			if (!this.m_EditingProbes)
			{
				return;
			}
			this.m_Editor.DeselectProbes();
			this.m_EditingProbes = false;
			Tools.s_Hidden = false;
		}
		public void OnDisable()
		{
			this.EndEditProbes();
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUIDelegate));
			if (this.target != null)
			{
				this.m_Editor.PushProbePositions();
			}
		}
		private void UndoRedoPerformed()
		{
			this.m_Editor.MarkTetrahedraDirty();
		}
		public override void OnInspectorGUI()
		{
			bool flag = Application.HasAdvancedLicense();
			EditorGUI.BeginDisabledGroup(!flag);
			this.m_Editor.PullProbePositions();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (GUILayout.Button("Add Probe", new GUILayoutOption[0]))
			{
				Vector3 position = Vector3.zero;
				if (SceneView.lastActiveSceneView)
				{
					position = SceneView.lastActiveSceneView.pivot;
					LightProbeGroup lightProbeGroup = this.target as LightProbeGroup;
					if (lightProbeGroup)
					{
						position = lightProbeGroup.transform.InverseTransformPoint(position);
					}
				}
				this.StartEditProbes();
				this.m_Editor.DeselectProbes();
				this.m_Editor.AddProbe(position);
			}
			if (GUILayout.Button("Delete Selected", new GUILayoutOption[0]))
			{
				this.StartEditProbes();
				this.m_Editor.RemoveSelectedProbes();
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (GUILayout.Button("Select All", new GUILayoutOption[0]))
			{
				this.StartEditProbes();
				this.m_Editor.SelectAllProbes();
			}
			if (GUILayout.Button("Duplicate Selected", new GUILayoutOption[0]))
			{
				this.StartEditProbes();
				this.m_Editor.DuplicateSelectedProbes();
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			this.m_Editor.PushProbePositions();
			EditorGUI.EndDisabledGroup();
			if (!flag)
			{
				GUIContent gUIContent = EditorGUIUtility.TextContent("LightProbeGroup.ProOnly");
				EditorGUILayout.HelpBox(gUIContent.text, MessageType.Warning, true);
			}
		}
		private void InternalOnSceneView()
		{
			if (SceneView.lastActiveSceneView != null && this.m_ShouldFocus)
			{
				this.m_ShouldFocus = false;
				SceneView.lastActiveSceneView.FrameSelected();
			}
			this.m_Editor.PullProbePositions();
			LightProbeGroup lightProbeGroup = this.target as LightProbeGroup;
			if (lightProbeGroup != null)
			{
				if (this.m_Editor.OnSceneGUI(lightProbeGroup.transform))
				{
					this.StartEditProbes();
				}
				else
				{
					this.EndEditProbes();
				}
			}
			this.m_Editor.PushProbePositions();
		}
		public void OnSceneGUI()
		{
			if (!Application.HasAdvancedLicense())
			{
				return;
			}
			if (Event.current.type != EventType.Repaint)
			{
				this.InternalOnSceneView();
			}
		}
		public void OnSceneGUIDelegate(SceneView sceneView)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.InternalOnSceneView();
			}
		}
		public bool HasFrameBounds()
		{
			return this.m_Editor.SelectedCount > 0;
		}
		public Bounds OnGetFrameBounds()
		{
			return this.m_Editor.selectedProbeBounds;
		}
	}
}
