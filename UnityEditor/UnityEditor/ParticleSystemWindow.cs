using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ParticleSystemWindow : EditorWindow, ParticleEffectUIOwner
	{
		private class Texts
		{
			public GUIContent lockParticleSystem = new GUIContent("", "Lock the current selected Particle System");

			public GUIContent previewAll = new GUIContent("Simulate All", "Simulate all particle systems that have Play On Awake set");
		}

		private static ParticleSystemWindow s_Instance;

		private ParticleSystem m_Target;

		private ParticleEffectUI m_ParticleEffectUI;

		private bool m_IsVisible;

		private static GUIContent[] s_Icons;

		private static ParticleSystemWindow.Texts s_Texts;

		public Editor customEditor
		{
			get;
			set;
		}

		private ParticleSystemWindow()
		{
		}

		public static void CreateWindow()
		{
			ParticleSystemWindow.s_Instance = EditorWindow.GetWindow<ParticleSystemWindow>();
			ParticleSystemWindow.s_Instance.titleContent = EditorGUIUtility.TextContent("Particle Effect");
			ParticleSystemWindow.s_Instance.minSize = ParticleEffectUI.GetMinSize();
		}

		internal static ParticleSystemWindow GetInstance()
		{
			return ParticleSystemWindow.s_Instance;
		}

		internal bool IsVisible()
		{
			return this.m_IsVisible;
		}

		private void OnEnable()
		{
			ParticleSystemWindow.s_Instance = this;
			this.m_Target = null;
			ParticleEffectUI.m_VerticalLayout = EditorPrefs.GetBool("ShurikenVerticalLayout", false);
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			base.autoRepaintOnSceneChange = false;
		}

		private void OnDisable()
		{
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
			EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
			EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
			Undo.undoRedoPerformed = (Undo.UndoRedoCallback)Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
			this.Clear();
			if (ParticleSystemWindow.s_Instance == this)
			{
				ParticleSystemWindow.s_Instance = null;
			}
		}

		internal void Clear()
		{
			this.m_Target = null;
			if (this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.Clear();
				this.m_ParticleEffectUI = null;
			}
		}

		private void OnPlayModeStateChanged()
		{
			base.Repaint();
		}

		private void UndoRedoPerformed()
		{
			if (this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.UndoRedoPerformed();
			}
			base.Repaint();
		}

		private void OnHierarchyOrProjectWindowWasChanged()
		{
			this.InitEffectUI();
		}

		private void OnBecameVisible()
		{
			if (!this.m_IsVisible)
			{
				this.m_IsVisible = true;
				this.InitEffectUI();
				SceneView.RepaintAll();
				InspectorWindow.RepaintAllInspectors();
			}
		}

		private void OnBecameInvisible()
		{
			this.m_IsVisible = false;
			this.Clear();
			SceneView.RepaintAll();
			InspectorWindow.RepaintAllInspectors();
		}

		private void OnSelectionChange()
		{
			this.InitEffectUI();
			base.Repaint();
		}

		private void InitEffectUI()
		{
			if (this.m_IsVisible)
			{
				ParticleSystem particleSystem = ParticleSystemEditorUtils.lockedParticleSystem;
				if (particleSystem == null && Selection.activeGameObject != null)
				{
					particleSystem = Selection.activeGameObject.GetComponent<ParticleSystem>();
				}
				this.m_Target = particleSystem;
				if (this.m_Target != null)
				{
					if (this.m_ParticleEffectUI == null)
					{
						this.m_ParticleEffectUI = new ParticleEffectUI(this);
					}
					if (this.m_ParticleEffectUI.InitializeIfNeeded(new ParticleSystem[]
					{
						this.m_Target
					}))
					{
						base.Repaint();
					}
				}
				if (this.m_Target == null && this.m_ParticleEffectUI != null)
				{
					this.Clear();
					base.Repaint();
					SceneView.RepaintAll();
					GameView.RepaintAll();
				}
			}
		}

		private void Awake()
		{
		}

		private void DoToolbarGUI()
		{
			GUILayout.BeginHorizontal("Toolbar", new GUILayoutOption[0]);
			using (new EditorGUI.DisabledScope(this.m_ParticleEffectUI == null))
			{
				if (!EditorApplication.isPlaying)
				{
					bool flag = false;
					if (this.m_ParticleEffectUI != null)
					{
						flag = this.m_ParticleEffectUI.IsPlaying();
					}
					if (GUILayout.Button((!flag) ? ParticleEffectUI.texts.play : ParticleEffectUI.texts.pause, "ToolbarButton", new GUILayoutOption[]
					{
						GUILayout.Width(65f)
					}))
					{
						if (this.m_ParticleEffectUI != null)
						{
							if (flag)
							{
								this.m_ParticleEffectUI.Pause();
							}
							else
							{
								this.m_ParticleEffectUI.Play();
							}
						}
						base.Repaint();
					}
					if (GUILayout.Button(ParticleEffectUI.texts.stop, "ToolbarButton", new GUILayoutOption[0]) && this.m_ParticleEffectUI != null)
					{
						this.m_ParticleEffectUI.Stop();
					}
				}
				else
				{
					if (GUILayout.Button(ParticleEffectUI.texts.play, "ToolbarButton", new GUILayoutOption[]
					{
						GUILayout.Width(65f)
					}))
					{
						if (this.m_ParticleEffectUI != null)
						{
							this.m_ParticleEffectUI.Stop();
							this.m_ParticleEffectUI.Play();
						}
					}
					if (GUILayout.Button(ParticleEffectUI.texts.stop, "ToolbarButton", new GUILayoutOption[0]))
					{
						if (this.m_ParticleEffectUI != null)
						{
							this.m_ParticleEffectUI.Stop();
						}
					}
				}
				GUILayout.FlexibleSpace();
				bool flag2 = this.m_ParticleEffectUI != null && this.m_ParticleEffectUI.IsShowOnlySelectedMode();
				bool flag3 = GUILayout.Toggle(flag2, (!flag2) ? "Show: All" : "Show: Selected", ParticleSystemStyles.Get().toolbarButtonLeftAlignText, new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				if (flag3 != flag2 && this.m_ParticleEffectUI != null)
				{
					this.m_ParticleEffectUI.SetShowOnlySelectedMode(flag3);
				}
				ParticleSystemEditorUtils.editorResimulation = GUILayout.Toggle(ParticleSystemEditorUtils.editorResimulation, ParticleEffectUI.texts.resimulation, "ToolbarButton", new GUILayoutOption[0]);
				ParticleEffectUI.m_ShowWireframe = GUILayout.Toggle(ParticleEffectUI.m_ShowWireframe, ParticleEffectUI.texts.wireframe, "ToolbarButton", new GUILayoutOption[0]);
				ParticleEffectUI.m_ShowBounds = GUILayout.Toggle(ParticleEffectUI.m_ShowBounds, ParticleEffectUI.texts.bounds, "ToolbarButton", new GUILayoutOption[0]);
				if (GUILayout.Button((!ParticleEffectUI.m_VerticalLayout) ? ParticleSystemWindow.s_Icons[1] : ParticleSystemWindow.s_Icons[0], "ToolbarButton", new GUILayoutOption[0]))
				{
					ParticleEffectUI.m_VerticalLayout = !ParticleEffectUI.m_VerticalLayout;
					EditorPrefs.SetBool("ShurikenVerticalLayout", ParticleEffectUI.m_VerticalLayout);
					this.Clear();
				}
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Space(3f);
				ParticleSystem lockedParticleSystem = ParticleSystemEditorUtils.lockedParticleSystem;
				bool flag4 = lockedParticleSystem != null;
				bool flag5 = GUILayout.Toggle(flag4, ParticleSystemWindow.s_Texts.lockParticleSystem, "IN LockButton", new GUILayoutOption[0]);
				if (flag4 != flag5)
				{
					if (this.m_ParticleEffectUI != null && this.m_Target != null)
					{
						if (flag5)
						{
							ParticleSystemEditorUtils.lockedParticleSystem = this.m_Target;
						}
						else
						{
							ParticleSystemEditorUtils.lockedParticleSystem = null;
						}
					}
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		private void OnGUI()
		{
			if (ParticleSystemWindow.s_Texts == null)
			{
				ParticleSystemWindow.s_Texts = new ParticleSystemWindow.Texts();
			}
			if (ParticleSystemWindow.s_Icons == null)
			{
				ParticleSystemWindow.s_Icons = new GUIContent[]
				{
					EditorGUIUtility.IconContent("HorizontalSplit"),
					EditorGUIUtility.IconContent("VerticalSplit")
				};
			}
			if (this.m_Target == null && (Selection.activeGameObject != null || ParticleSystemEditorUtils.lockedParticleSystem != null))
			{
				this.InitEffectUI();
			}
			this.DoToolbarGUI();
			if (this.m_Target != null && this.m_ParticleEffectUI != null)
			{
				this.m_ParticleEffectUI.OnGUI();
			}
		}

		public void OnSceneViewGUI(SceneView sceneView)
		{
			if (this.m_IsVisible)
			{
				if (this.m_ParticleEffectUI != null)
				{
					this.m_ParticleEffectUI.OnSceneViewGUI();
				}
			}
		}

		private void OnDidOpenScene()
		{
			base.Repaint();
		}
	}
}
