using System;
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(ParticleSystem))]
	internal class ParticleSystemInspector : Editor, ParticleEffectUIOwner
	{
		private ParticleEffectUI m_ParticleEffectUI;

		private GUIContent m_PreviewTitle = new GUIContent("Particle System Curves");

		private GUIContent showWindowText = new GUIContent("Open Editor...");

		private GUIContent closeWindowText = new GUIContent("Close Editor");

		private GUIContent hideWindowText = new GUIContent("Hide Editor");

		private static GUIContent m_PlayBackTitle;

		public static GUIContent playBackTitle
		{
			get
			{
				if (ParticleSystemInspector.m_PlayBackTitle == null)
				{
					ParticleSystemInspector.m_PlayBackTitle = new GUIContent("Particle Effect");
				}
				return ParticleSystemInspector.m_PlayBackTitle;
			}
		}

		private bool selectedInParticleSystemWindow
		{
			get
			{
				GameObject gameObject = (base.target as ParticleSystem).gameObject;
				GameObject x;
				if (ParticleSystemEditorUtils.lockedParticleSystem == null)
				{
					x = Selection.activeGameObject;
				}
				else
				{
					x = ParticleSystemEditorUtils.lockedParticleSystem.gameObject;
				}
				return x == gameObject;
			}
		}

		public void OnEnable()
		{
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
		}

		public void OnDisable()
		{
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			if (this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.Clear();
			}
		}

		private void HierarchyOrProjectWindowWasChanged()
		{
			if (this.ShouldShowInspector())
			{
				this.Init(true);
			}
		}

		private void UndoRedoPerformed()
		{
			if (this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.UndoRedoPerformed();
			}
		}

		private void Init(bool forceInit)
		{
			ParticleSystem particleSystem = base.target as ParticleSystem;
			if (!(particleSystem == null))
			{
				if (this.m_ParticleEffectUI == null)
				{
					this.m_ParticleEffectUI = new ParticleEffectUI(this);
					this.m_ParticleEffectUI.InitializeIfNeeded(particleSystem);
				}
				else if (forceInit)
				{
					this.m_ParticleEffectUI.InitializeIfNeeded(particleSystem);
				}
			}
		}

		private void ShowEdiorButtonGUI()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool selectedInParticleSystemWindow = this.selectedInParticleSystemWindow;
			GameObject gameObject = (base.target as ParticleSystem).gameObject;
			ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
			GUIContent content;
			if (instance && instance.IsVisible() && selectedInParticleSystemWindow)
			{
				if (instance.GetNumTabs() > 1)
				{
					content = this.hideWindowText;
				}
				else
				{
					content = this.closeWindowText;
				}
			}
			else
			{
				content = this.showWindowText;
			}
			if (GUILayout.Button(content, EditorStyles.miniButton, new GUILayoutOption[]
			{
				GUILayout.Width(110f)
			}))
			{
				if (instance && instance.IsVisible() && selectedInParticleSystemWindow)
				{
					if (!instance.ShowNextTabIfPossible())
					{
						instance.Close();
					}
				}
				else
				{
					if (!selectedInParticleSystemWindow)
					{
						ParticleSystemEditorUtils.lockedParticleSystem = null;
						Selection.activeGameObject = gameObject;
					}
					if (instance)
					{
						if (!selectedInParticleSystemWindow)
						{
							instance.Clear();
						}
						instance.Focus();
					}
					else
					{
						this.Clear();
						ParticleSystemWindow.CreateWindow();
						GUIUtility.ExitGUI();
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		public override bool UseDefaultMargins()
		{
			return false;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
			this.ShowEdiorButtonGUI();
			if (this.ShouldShowInspector())
			{
				if (this.m_ParticleEffectUI == null)
				{
					this.Init(true);
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
				this.m_ParticleEffectUI.OnGUI();
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
			}
			else
			{
				this.Clear();
			}
			EditorGUILayout.EndVertical();
		}

		private void Clear()
		{
			if (this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.Clear();
			}
			this.m_ParticleEffectUI = null;
		}

		private bool ShouldShowInspector()
		{
			ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
			return !instance || !instance.IsVisible() || !this.selectedInParticleSystemWindow;
		}

		public void OnSceneGUI()
		{
			if (this.ShouldShowInspector())
			{
				if (this.m_ParticleEffectUI != null)
				{
					this.m_ParticleEffectUI.OnSceneGUI();
				}
			}
		}

		public void OnSceneViewGUI(SceneView sceneView)
		{
			if (this.ShouldShowInspector())
			{
				this.Init(false);
				if (this.m_ParticleEffectUI != null)
				{
					this.m_ParticleEffectUI.OnSceneViewGUI();
				}
			}
		}

		public override bool HasPreviewGUI()
		{
			return this.ShouldShowInspector() && Selection.objects.Length == 1;
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.GetParticleSystemCurveEditor().OnGUI(r);
			}
		}

		public override GUIContent GetPreviewTitle()
		{
			return this.m_PreviewTitle;
		}

		public override void OnPreviewSettings()
		{
		}
	}
}
