using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	internal class ParticleEffectUI
	{
		private enum PlayState
		{
			Stopped,
			Playing,
			Paused
		}

		private enum OwnerType
		{
			Inspector,
			ParticleSystemWindow
		}

		internal class Texts
		{
			public GUIContent previewSpeed = new GUIContent("Playback Speed");

			public GUIContent previewTime = new GUIContent("Playback Time");

			public GUIContent particleCount = new GUIContent("Particles");

			public GUIContent subEmitterParticleCount = new GUIContent("Sub Emitter Particles");

			public GUIContent play = new GUIContent("Simulate");

			public GUIContent stop = new GUIContent("Stop");

			public GUIContent pause = new GUIContent("Pause");

			public GUIContent addParticleSystem = new GUIContent("", "Create Particle System");

			public GUIContent wireframe = new GUIContent("Selection", "Show particles with the selection outline/wireframe, based on the selection options in the Gizmos menu");

			public GUIContent bounds = new GUIContent("Bounds", "Show world space bounding boxes");

			public GUIContent resimulation = new GUIContent("Resimulate", "If resimulate is enabled the particle system will show changes made to the system immediately (including changes made to the particle system transform)");

			public string secondsFloatFieldFormatString = "f2";
		}

		public ParticleEffectUIOwner m_Owner;

		public ParticleSystemUI[] m_Emitters;

		private ParticleSystemCurveEditor m_ParticleSystemCurveEditor;

		private List<ParticleSystem> m_SelectedParticleSystems;

		private bool m_ShowOnlySelectedMode;

		private TimeHelper m_TimeHelper = default(TimeHelper);

		public static bool m_ShowWireframe = false;

		public static bool m_ShowBounds = false;

		public static bool m_VerticalLayout;

		private const string k_SimulationStateId = "SimulationState";

		private const string k_ShowSelectedId = "ShowSelected";

		private static readonly Vector2 k_MinEmitterAreaSize = new Vector2(125f, 100f);

		private static readonly Vector2 k_MinCurveAreaSize = new Vector2(100f, 100f);

		private float m_EmitterAreaWidth = 230f;

		private float m_CurveEditorAreaHeight = 330f;

		private Vector2 m_EmitterAreaScrollPos = Vector2.zero;

		private static readonly Color k_DarkSkinDisabledColor = new Color(0.66f, 0.66f, 0.66f, 0.95f);

		private static readonly Color k_LightSkinDisabledColor = new Color(0.84f, 0.84f, 0.84f, 0.95f);

		private static ParticleEffectUI.Texts s_Texts;

		private static PrefKey kPlay = new PrefKey("ParticleSystem/Play", ",");

		private static PrefKey kStop = new PrefKey("ParticleSystem/Stop", ".");

		private static PrefKey kForward = new PrefKey("ParticleSystem/Forward", "m");

		private static PrefKey kReverse = new PrefKey("ParticleSystem/Reverse", "n");

		private int m_IsDraggingTimeHotControlID = -1;

		internal static ParticleEffectUI.Texts texts
		{
			get
			{
				if (ParticleEffectUI.s_Texts == null)
				{
					ParticleEffectUI.s_Texts = new ParticleEffectUI.Texts();
				}
				return ParticleEffectUI.s_Texts;
			}
		}

		public bool multiEdit
		{
			get
			{
				return this.m_SelectedParticleSystems != null && this.m_SelectedParticleSystems.Count > 1;
			}
		}

		public ParticleEffectUI(ParticleEffectUIOwner owner)
		{
			this.m_Owner = owner;
		}

		private bool ShouldManagePlaybackState(ParticleSystem root)
		{
			bool flag = false;
			if (root != null)
			{
				flag = root.gameObject.activeInHierarchy;
			}
			return !EditorApplication.isPlaying && flag;
		}

		private static Color GetDisabledColor()
		{
			return EditorGUIUtility.isProSkin ? ParticleEffectUI.k_DarkSkinDisabledColor : ParticleEffectUI.k_LightSkinDisabledColor;
		}

		internal static ParticleSystem[] GetParticleSystems(ParticleSystem root)
		{
			List<ParticleSystem> list = new List<ParticleSystem>();
			list.Add(root);
			ParticleEffectUI.GetDirectParticleSystemChildrenRecursive(root.transform, list);
			return list.ToArray();
		}

		private static void GetDirectParticleSystemChildrenRecursive(Transform transform, List<ParticleSystem> particleSystems)
		{
			IEnumerator enumerator = transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					ParticleSystem component = transform2.gameObject.GetComponent<ParticleSystem>();
					if (component != null)
					{
						particleSystems.Add(component);
						ParticleEffectUI.GetDirectParticleSystemChildrenRecursive(transform2, particleSystems);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public bool InitializeIfNeeded(IEnumerable<ParticleSystem> systems)
		{
			bool flag = false;
			ParticleSystem[] array = systems.ToArray<ParticleSystem>();
			bool flag2 = array.Count<ParticleSystem>() > 1;
			bool flag3 = false;
			ParticleSystem particleSystem = null;
			ParticleSystem[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				ParticleSystem particleSystem2 = array2[i];
				if (flag2)
				{
					ParticleSystem[] array3 = new ParticleSystem[]
					{
						particleSystem2
					};
					particleSystem = particleSystem2;
					goto IL_F7;
				}
				ParticleSystem root = ParticleSystemEditorUtils.GetRoot(particleSystem2);
				if (!(root == null))
				{
					ParticleSystem[] array3 = ParticleEffectUI.GetParticleSystems(root);
					if (this.m_SelectedParticleSystems != null && this.m_SelectedParticleSystems.Count > 0)
					{
						if (root == ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystems[0]))
						{
							if (this.m_ParticleSystemCurveEditor != null && this.m_Emitters != null && array3.Length == this.m_Emitters.Length)
							{
								this.m_SelectedParticleSystems = new List<ParticleSystem>();
								this.m_SelectedParticleSystems.Add(particleSystem2);
								goto IL_1AC;
							}
						}
					}
					particleSystem = root;
					goto IL_F7;
				}
				IL_1AC:
				i++;
				continue;
				IL_F7:
				if (this.m_ParticleSystemCurveEditor != null)
				{
					this.Clear();
				}
				flag3 = true;
				if (!flag)
				{
					this.m_SelectedParticleSystems = new List<ParticleSystem>();
					flag = true;
				}
				this.m_SelectedParticleSystems.Add(particleSystem2);
				if (!flag2)
				{
					this.m_ParticleSystemCurveEditor = new ParticleSystemCurveEditor();
					this.m_ParticleSystemCurveEditor.Init();
					ParticleSystem[] array3;
					int num = array3.Length;
					if (num > 0)
					{
						this.m_Emitters = new ParticleSystemUI[num];
						for (int j = 0; j < num; j++)
						{
							this.m_Emitters[j] = new ParticleSystemUI();
							this.m_Emitters[j].Init(this, new ParticleSystem[]
							{
								array3[j]
							});
						}
					}
				}
				goto IL_1AC;
			}
			if (flag3)
			{
				if (flag2)
				{
					this.m_ParticleSystemCurveEditor = new ParticleSystemCurveEditor();
					this.m_ParticleSystemCurveEditor.Init();
					int count = this.m_SelectedParticleSystems.Count;
					if (count > 0)
					{
						this.m_Emitters = new ParticleSystemUI[1];
						this.m_Emitters[0] = new ParticleSystemUI();
						this.m_Emitters[0].Init(this, this.m_SelectedParticleSystems.ToArray());
					}
				}
				ParticleSystemUI[] emitters = this.m_Emitters;
				for (int k = 0; k < emitters.Length; k++)
				{
					ParticleSystemUI particleSystemUI = emitters[k];
					ModuleUI[] modules = particleSystemUI.m_Modules;
					for (int l = 0; l < modules.Length; l++)
					{
						ModuleUI moduleUI = modules[l];
						if (moduleUI != null)
						{
							moduleUI.Validate();
						}
					}
				}
				if (ParticleEffectUI.GetAllModulesVisible())
				{
					this.SetAllModulesVisible(true);
				}
				ParticleSystemEditorUtils.PerformCompleteResimulation();
				this.m_EmitterAreaWidth = EditorPrefs.GetFloat("ParticleSystemEmitterAreaWidth", ParticleEffectUI.k_MinEmitterAreaSize.x);
				this.m_CurveEditorAreaHeight = EditorPrefs.GetFloat("ParticleSystemCurveEditorAreaHeight", ParticleEffectUI.k_MinCurveAreaSize.y);
				this.m_ShowOnlySelectedMode = (this.m_Owner is ParticleSystemWindow && SessionState.GetBool("ShowSelected" + particleSystem.GetInstanceID(), false));
				this.m_EmitterAreaScrollPos.x = SessionState.GetFloat("CurrentEmitterAreaScroll", 0f);
				if (this.ShouldManagePlaybackState(particleSystem))
				{
					Vector3 vector = SessionState.GetVector3("SimulationState" + particleSystem.GetInstanceID(), Vector3.zero);
					if (particleSystem.GetInstanceID() == (int)vector.x)
					{
						float z = vector.z;
						if (z > 0f)
						{
							ParticleSystemEditorUtils.editorPlaybackTime = z;
						}
					}
					this.Play();
				}
			}
			return flag3;
		}

		internal void UndoRedoPerformed()
		{
			this.Refresh();
			ParticleSystemUI[] emitters = this.m_Emitters;
			for (int i = 0; i < emitters.Length; i++)
			{
				ParticleSystemUI particleSystemUI = emitters[i];
				ModuleUI[] modules = particleSystemUI.m_Modules;
				for (int j = 0; j < modules.Length; j++)
				{
					ModuleUI moduleUI = modules[j];
					if (moduleUI != null)
					{
						moduleUI.CheckVisibilityState();
						if (moduleUI.foldout)
						{
							moduleUI.UndoRedoPerformed();
						}
					}
				}
			}
			this.m_Owner.Repaint();
		}

		public void Clear()
		{
			ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystems[0]);
			if (this.ShouldManagePlaybackState(root))
			{
				if (root != null)
				{
					ParticleEffectUI.PlayState playState;
					if (this.IsPlaying())
					{
						playState = ParticleEffectUI.PlayState.Playing;
					}
					else if (this.IsPaused())
					{
						playState = ParticleEffectUI.PlayState.Paused;
					}
					else
					{
						playState = ParticleEffectUI.PlayState.Stopped;
					}
					int instanceID = root.GetInstanceID();
					SessionState.SetVector3("SimulationState" + instanceID, new Vector3((float)instanceID, (float)playState, ParticleSystemEditorUtils.editorPlaybackTime));
				}
			}
			this.m_ParticleSystemCurveEditor.OnDisable();
			Tools.s_Hidden = false;
			if (root != null)
			{
				SessionState.SetBool("ShowSelected" + root.GetInstanceID(), this.m_ShowOnlySelectedMode);
			}
			this.SetShowOnlySelectedMode(false);
			GameView.RepaintAll();
			SceneView.RepaintAll();
		}

		public static Vector2 GetMinSize()
		{
			return ParticleEffectUI.k_MinEmitterAreaSize + ParticleEffectUI.k_MinCurveAreaSize;
		}

		public void Refresh()
		{
			this.UpdateProperties();
			this.m_ParticleSystemCurveEditor.Refresh();
		}

		public string GetNextParticleSystemName()
		{
			string nextName = "";
			string result;
			for (int i = 2; i < 50; i++)
			{
				nextName = "Particle System " + i;
				bool flag = false;
				ParticleSystemUI[] emitters = this.m_Emitters;
				for (int j = 0; j < emitters.Length; j++)
				{
					ParticleSystemUI particleSystemUI = emitters[j];
					if (particleSystemUI.m_ParticleSystems.FirstOrDefault((ParticleSystem o) => o.name == nextName) != null)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					result = nextName;
					return result;
				}
			}
			result = "Particle System";
			return result;
		}

		public bool IsParticleSystemUIVisible(ParticleSystemUI psUI)
		{
			ParticleEffectUI.OwnerType ownerType = (!(this.m_Owner is ParticleSystemInspector)) ? ParticleEffectUI.OwnerType.ParticleSystemWindow : ParticleEffectUI.OwnerType.Inspector;
			bool result;
			if (ownerType == ParticleEffectUI.OwnerType.ParticleSystemWindow)
			{
				result = true;
			}
			else
			{
				ParticleSystem[] particleSystems = psUI.m_ParticleSystems;
				for (int i = 0; i < particleSystems.Length; i++)
				{
					ParticleSystem ps = particleSystems[i];
					if (this.m_SelectedParticleSystems.FirstOrDefault((ParticleSystem o) => o == ps) != null)
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		public void PlayOnAwakeChanged(bool newPlayOnAwake)
		{
			ParticleSystemUI[] emitters = this.m_Emitters;
			for (int i = 0; i < emitters.Length; i++)
			{
				ParticleSystemUI particleSystemUI = emitters[i];
				InitialModuleUI initialModuleUI = particleSystemUI.m_Modules[0] as InitialModuleUI;
				initialModuleUI.m_PlayOnAwake.boolValue = newPlayOnAwake;
				particleSystemUI.ApplyProperties();
			}
		}

		public GameObject CreateParticleSystem(ParticleSystem parentOfNewParticleSystem, SubModuleUI.SubEmitterType defaultType)
		{
			string nextParticleSystemName = this.GetNextParticleSystemName();
			GameObject gameObject = new GameObject(nextParticleSystemName, new Type[]
			{
				typeof(ParticleSystem)
			});
			GameObject result;
			if (gameObject)
			{
				if (parentOfNewParticleSystem)
				{
					gameObject.transform.parent = parentOfNewParticleSystem.transform;
				}
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
				if (defaultType != SubModuleUI.SubEmitterType.None)
				{
					component.SetupDefaultType((int)defaultType);
				}
				SessionState.SetFloat("CurrentEmitterAreaScroll", this.m_EmitterAreaScrollPos.x);
				ParticleSystemRenderer component2 = gameObject.GetComponent<ParticleSystemRenderer>();
				Renderer arg_AF_0 = component2;
				Material[] expr_A2 = new Material[2];
				expr_A2[0] = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
				arg_AF_0.materials = expr_A2;
				Undo.RegisterCreatedObjectUndo(gameObject, "Create ParticleSystem");
				result = gameObject;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public ParticleSystemCurveEditor GetParticleSystemCurveEditor()
		{
			return this.m_ParticleSystemCurveEditor;
		}

		private void SceneViewGUICallback(UnityEngine.Object target, SceneView sceneView)
		{
			this.PlayStopGUI();
		}

		public void OnSceneViewGUI()
		{
			ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystems[0]);
			if (root && root.gameObject.activeInHierarchy)
			{
				SceneViewOverlay.Window(ParticleSystemInspector.playBackTitle, new SceneViewOverlay.WindowFunction(this.SceneViewGUICallback), 400, SceneViewOverlay.WindowDisplayOption.OneWindowPerTitle);
			}
			ParticleSystemUI[] emitters = this.m_Emitters;
			for (int i = 0; i < emitters.Length; i++)
			{
				ParticleSystemUI particleSystemUI = emitters[i];
				particleSystemUI.OnSceneViewGUI();
			}
		}

		internal void PlayBackTimeGUI()
		{
			EventType type = Event.current.type;
			int hotControl = GUIUtility.hotControl;
			string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
			EditorGUI.BeginChangeCheck();
			EditorGUI.kFloatFieldFormatString = ParticleEffectUI.s_Texts.secondsFloatFieldFormatString;
			float num = EditorGUILayout.FloatField(ParticleEffectUI.s_Texts.previewTime, ParticleSystemEditorUtils.editorPlaybackTime, new GUILayoutOption[0]);
			EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
			if (EditorGUI.EndChangeCheck())
			{
				if (type == EventType.MouseDrag)
				{
					ParticleSystemEditorUtils.editorIsScrubbing = true;
					float editorSimulationSpeed = ParticleSystemEditorUtils.editorSimulationSpeed;
					float editorPlaybackTime = ParticleSystemEditorUtils.editorPlaybackTime;
					float num2 = num - editorPlaybackTime;
					num = editorPlaybackTime + num2 * (0.05f * editorSimulationSpeed);
				}
				num = Mathf.Max(num, 0f);
				ParticleSystemEditorUtils.editorPlaybackTime = num;
				foreach (ParticleSystem current in this.m_SelectedParticleSystems)
				{
					ParticleSystem root = ParticleSystemEditorUtils.GetRoot(current);
					if (root.isStopped)
					{
						root.Play();
						root.Pause();
					}
				}
				ParticleSystemEditorUtils.PerformCompleteResimulation();
			}
			if (type == EventType.MouseDown && GUIUtility.hotControl != hotControl)
			{
				this.m_IsDraggingTimeHotControlID = GUIUtility.hotControl;
				ParticleSystemEditorUtils.editorIsScrubbing = true;
			}
			if (this.m_IsDraggingTimeHotControlID != -1 && GUIUtility.hotControl != this.m_IsDraggingTimeHotControlID)
			{
				this.m_IsDraggingTimeHotControlID = -1;
				ParticleSystemEditorUtils.editorIsScrubbing = false;
			}
			float num3 = 0f;
			foreach (ParticleSystem current2 in this.m_SelectedParticleSystems)
			{
				num3 += (float)current2.particleCount;
			}
			EditorGUILayout.FloatField(ParticleEffectUI.s_Texts.particleCount, num3, new GUILayoutOption[0]);
			bool flag = false;
			int num4 = 0;
			foreach (ParticleSystem current3 in this.m_SelectedParticleSystems)
			{
				int num5 = 0;
				if (current3.CountSubEmitterParticles(ref num5))
				{
					flag = true;
					num4 += num5;
				}
			}
			if (flag)
			{
				EditorGUILayout.FloatField(ParticleEffectUI.s_Texts.subEmitterParticleCount, (float)num4, new GUILayoutOption[0]);
			}
		}

		private void HandleKeyboardShortcuts()
		{
			Event current = Event.current;
			if (current.type == EventType.KeyDown)
			{
				int num = 0;
				if (current.keyCode == ParticleEffectUI.kPlay.keyCode)
				{
					if (EditorApplication.isPlaying)
					{
						this.Stop();
						this.Play();
					}
					else if (!ParticleSystemEditorUtils.editorIsPlaying)
					{
						this.Play();
					}
					else
					{
						this.Pause();
					}
					current.Use();
				}
				else if (current.keyCode == ParticleEffectUI.kStop.keyCode)
				{
					this.Stop();
					current.Use();
				}
				else if (current.keyCode == ParticleEffectUI.kReverse.keyCode)
				{
					num = -1;
				}
				else if (current.keyCode == ParticleEffectUI.kForward.keyCode)
				{
					num = 1;
				}
				if (num != 0)
				{
					ParticleSystemEditorUtils.editorIsScrubbing = true;
					float editorSimulationSpeed = ParticleSystemEditorUtils.editorSimulationSpeed;
					float num2 = ((!current.shift) ? 1f : 3f) * this.m_TimeHelper.deltaTime * ((num <= 0) ? -3f : 3f);
					ParticleSystemEditorUtils.editorPlaybackTime = Mathf.Max(0f, ParticleSystemEditorUtils.editorPlaybackTime + num2 * editorSimulationSpeed);
					foreach (ParticleSystem current2 in this.m_SelectedParticleSystems)
					{
						ParticleSystem root = ParticleSystemEditorUtils.GetRoot(current2);
						if (root.isStopped)
						{
							root.Play();
							root.Pause();
						}
					}
					ParticleSystemEditorUtils.PerformCompleteResimulation();
					current.Use();
				}
			}
			if (current.type == EventType.KeyUp && (current.keyCode == ParticleEffectUI.kReverse.keyCode || current.keyCode == ParticleEffectUI.kForward.keyCode))
			{
				ParticleSystemEditorUtils.editorIsScrubbing = false;
			}
		}

		internal static bool IsStopped(ParticleSystem root)
		{
			return !ParticleSystemEditorUtils.editorIsPlaying && !ParticleSystemEditorUtils.editorIsPaused && !ParticleSystemEditorUtils.editorIsScrubbing;
		}

		internal bool IsPaused()
		{
			return !this.IsPlaying() && !ParticleEffectUI.IsStopped(ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystems[0]));
		}

		internal bool IsPlaying()
		{
			return ParticleSystemEditorUtils.editorIsPlaying;
		}

		internal void Play()
		{
			bool flag = false;
			foreach (ParticleSystem current in this.m_SelectedParticleSystems)
			{
				ParticleSystem root = ParticleSystemEditorUtils.GetRoot(current);
				if (root)
				{
					root.Play();
					flag = true;
				}
			}
			if (flag)
			{
				ParticleSystemEditorUtils.editorIsScrubbing = false;
				this.m_Owner.Repaint();
			}
		}

		internal void Pause()
		{
			bool flag = false;
			foreach (ParticleSystem current in this.m_SelectedParticleSystems)
			{
				ParticleSystem root = ParticleSystemEditorUtils.GetRoot(current);
				if (root)
				{
					root.Pause();
					flag = true;
				}
			}
			if (flag)
			{
				ParticleSystemEditorUtils.editorIsScrubbing = true;
				this.m_Owner.Repaint();
			}
		}

		internal void Stop()
		{
			ParticleSystemEditorUtils.editorIsScrubbing = false;
			ParticleSystemEditorUtils.editorPlaybackTime = 0f;
			ParticleSystemEditorUtils.StopEffect();
			this.m_Owner.Repaint();
		}

		internal void PlayStopGUI()
		{
			if (ParticleEffectUI.s_Texts == null)
			{
				ParticleEffectUI.s_Texts = new ParticleEffectUI.Texts();
			}
			Event current = Event.current;
			if (current.type == EventType.Layout)
			{
				this.m_TimeHelper.Update();
			}
			if (!EditorApplication.isPlaying)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				bool flag = ParticleSystemEditorUtils.editorIsPlaying && !ParticleSystemEditorUtils.editorIsPaused;
				if (GUILayout.Button((!flag) ? ParticleEffectUI.s_Texts.play : ParticleEffectUI.s_Texts.pause, "ButtonLeft", new GUILayoutOption[0]))
				{
					if (flag)
					{
						this.Pause();
					}
					else
					{
						this.Play();
					}
				}
				if (GUILayout.Button(ParticleEffectUI.s_Texts.stop, "ButtonRight", new GUILayoutOption[0]))
				{
					this.Stop();
				}
				GUILayout.EndHorizontal();
				string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
				EditorGUI.kFloatFieldFormatString = ParticleEffectUI.s_Texts.secondsFloatFieldFormatString;
				ParticleSystemEditorUtils.editorSimulationSpeed = Mathf.Clamp(EditorGUILayout.FloatField(ParticleEffectUI.s_Texts.previewSpeed, ParticleSystemEditorUtils.editorSimulationSpeed, new GUILayoutOption[0]), 0f, 10f);
				EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
				this.PlayBackTimeGUI();
			}
			else
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				if (GUILayout.Button(ParticleEffectUI.s_Texts.play, new GUILayoutOption[0]))
				{
					this.Stop();
					this.Play();
				}
				if (GUILayout.Button(ParticleEffectUI.s_Texts.stop, new GUILayoutOption[0]))
				{
					this.Stop();
				}
				GUILayout.EndHorizontal();
			}
			this.HandleKeyboardShortcuts();
		}

		private void InspectorParticleSystemGUI()
		{
			GUILayout.BeginVertical(ParticleSystemStyles.Get().effectBgStyle, new GUILayoutOption[0]);
			ParticleSystem selectedSystem = (this.m_SelectedParticleSystems.Count <= 0) ? null : this.m_SelectedParticleSystems[0];
			if (selectedSystem != null)
			{
				ParticleSystemUI particleSystemUI = this.m_Emitters.FirstOrDefault((ParticleSystemUI o) => o.m_ParticleSystems[0] == selectedSystem);
				if (particleSystemUI != null)
				{
					float width = GUIClip.visibleRect.width - 18f;
					particleSystemUI.OnGUI(width, false);
				}
			}
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			ParticleSystemEditorUtils.editorResimulation = GUILayout.Toggle(ParticleSystemEditorUtils.editorResimulation, ParticleEffectUI.s_Texts.resimulation, EditorStyles.toggle, new GUILayoutOption[0]);
			ParticleEffectUI.m_ShowWireframe = GUILayout.Toggle(ParticleEffectUI.m_ShowWireframe, ParticleEffectUI.texts.wireframe, EditorStyles.toggle, new GUILayoutOption[0]);
			ParticleEffectUI.m_ShowBounds = GUILayout.Toggle(ParticleEffectUI.m_ShowBounds, ParticleEffectUI.texts.bounds, EditorStyles.toggle, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			this.HandleKeyboardShortcuts();
		}

		private void DrawSelectionMarker(Rect rect)
		{
			rect.x += 1f;
			rect.y += 1f;
			rect.width -= 2f;
			rect.height -= 2f;
			ParticleSystemStyles.Get().selectionMarker.Draw(rect, GUIContent.none, false, true, true, false);
		}

		private List<ParticleSystemUI> GetSelectedParticleSystemUIs()
		{
			List<ParticleSystemUI> list = new List<ParticleSystemUI>();
			int[] instanceIDs = Selection.instanceIDs;
			ParticleSystemUI[] emitters = this.m_Emitters;
			for (int i = 0; i < emitters.Length; i++)
			{
				ParticleSystemUI particleSystemUI = emitters[i];
				if (instanceIDs.Contains(particleSystemUI.m_ParticleSystems[0].gameObject.GetInstanceID()))
				{
					list.Add(particleSystemUI);
				}
			}
			return list;
		}

		private void MultiParticleSystemGUI(bool verticalLayout)
		{
			GUILayout.BeginVertical(ParticleSystemStyles.Get().effectBgStyle, new GUILayoutOption[0]);
			this.m_EmitterAreaScrollPos = EditorGUILayout.BeginScrollView(this.m_EmitterAreaScrollPos, new GUILayoutOption[0]);
			Rect position = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			this.m_EmitterAreaScrollPos -= EditorGUI.MouseDeltaReader(position, Event.current.alt);
			GUILayout.Space(3f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(3f);
			Color color = GUI.color;
			bool flag = Event.current.type == EventType.Repaint;
			bool flag2 = this.IsShowOnlySelectedMode();
			List<ParticleSystemUI> selectedParticleSystemUIs = this.GetSelectedParticleSystemUIs();
			for (int i = 0; i < this.m_Emitters.Length; i++)
			{
				if (i != 0)
				{
					GUILayout.Space(ModuleUI.k_SpaceBetweenModules);
				}
				bool flag3 = selectedParticleSystemUIs.Contains(this.m_Emitters[i]);
				ModuleUI particleSystemRendererModuleUI = this.m_Emitters[i].GetParticleSystemRendererModuleUI();
				if (flag && particleSystemRendererModuleUI != null && !particleSystemRendererModuleUI.enabled)
				{
					GUI.color = ParticleEffectUI.GetDisabledColor();
				}
				if (flag && flag2 && !flag3)
				{
					GUI.color = ParticleEffectUI.GetDisabledColor();
				}
				Rect rect = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
				if (flag && flag3 && this.m_Emitters.Length > 1)
				{
					this.DrawSelectionMarker(rect);
				}
				this.m_Emitters[i].OnGUI(ModuleUI.k_CompactFixedModuleWidth, true);
				EditorGUILayout.EndVertical();
				GUI.color = color;
			}
			GUILayout.Space(5f);
			if (GUILayout.Button(ParticleEffectUI.s_Texts.addParticleSystem, "OL Plus", new GUILayoutOption[]
			{
				GUILayout.Width(20f)
			}))
			{
				this.CreateParticleSystem(ParticleSystemEditorUtils.GetRoot(this.m_SelectedParticleSystems[0]), SubModuleUI.SubEmitterType.None);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(4f);
			this.m_EmitterAreaScrollPos -= EditorGUI.MouseDeltaReader(position, true);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
			GUILayout.EndVertical();
			this.HandleKeyboardShortcuts();
		}

		private void WindowCurveEditorGUI(bool verticalLayout)
		{
			Rect rect;
			if (verticalLayout)
			{
				rect = GUILayoutUtility.GetRect(13f, this.m_CurveEditorAreaHeight, new GUILayoutOption[]
				{
					GUILayout.MinHeight(this.m_CurveEditorAreaHeight)
				});
			}
			else
			{
				EditorWindow editorWindow = (EditorWindow)this.m_Owner;
				rect = GUILayoutUtility.GetRect(editorWindow.position.width - this.m_EmitterAreaWidth, editorWindow.position.height - 17f);
			}
			this.ResizeHandling(verticalLayout);
			this.m_ParticleSystemCurveEditor.OnGUI(rect);
		}

		private Rect ResizeHandling(bool verticalLayout)
		{
			Rect lastRect;
			if (verticalLayout)
			{
				lastRect = GUILayoutUtility.GetLastRect();
				lastRect.y += -5f;
				lastRect.height = 5f;
				float y = EditorGUI.MouseDeltaReader(lastRect, true).y;
				if (y != 0f)
				{
					this.m_CurveEditorAreaHeight -= y;
					this.ClampWindowContentSizes();
					EditorPrefs.SetFloat("ParticleSystemCurveEditorAreaHeight", this.m_CurveEditorAreaHeight);
				}
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.SplitResizeUpDown);
				}
			}
			else
			{
				lastRect = new Rect(this.m_EmitterAreaWidth - 5f, 0f, 5f, GUIClip.visibleRect.height);
				float x = EditorGUI.MouseDeltaReader(lastRect, true).x;
				if (x != 0f)
				{
					this.m_EmitterAreaWidth += x;
					this.ClampWindowContentSizes();
					EditorPrefs.SetFloat("ParticleSystemEmitterAreaWidth", this.m_EmitterAreaWidth);
				}
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.SplitResizeLeftRight);
				}
			}
			return lastRect;
		}

		private void ClampWindowContentSizes()
		{
			EventType type = Event.current.type;
			if (type != EventType.Layout)
			{
				float width = GUIClip.visibleRect.width;
				float height = GUIClip.visibleRect.height;
				bool verticalLayout = ParticleEffectUI.m_VerticalLayout;
				if (verticalLayout)
				{
					this.m_CurveEditorAreaHeight = Mathf.Clamp(this.m_CurveEditorAreaHeight, ParticleEffectUI.k_MinCurveAreaSize.y, height - ParticleEffectUI.k_MinEmitterAreaSize.y);
				}
				else
				{
					this.m_EmitterAreaWidth = Mathf.Clamp(this.m_EmitterAreaWidth, ParticleEffectUI.k_MinEmitterAreaSize.x, width - ParticleEffectUI.k_MinCurveAreaSize.x);
				}
			}
		}

		public void OnGUI()
		{
			if (ParticleEffectUI.s_Texts == null)
			{
				ParticleEffectUI.s_Texts = new ParticleEffectUI.Texts();
			}
			if (this.m_Emitters != null)
			{
				this.UpdateProperties();
				ParticleEffectUI.OwnerType ownerType = (!(this.m_Owner is ParticleSystemInspector)) ? ParticleEffectUI.OwnerType.ParticleSystemWindow : ParticleEffectUI.OwnerType.Inspector;
				if (ownerType != ParticleEffectUI.OwnerType.ParticleSystemWindow)
				{
					if (ownerType != ParticleEffectUI.OwnerType.Inspector)
					{
						Debug.LogError("Unhandled enum");
					}
					else
					{
						this.InspectorParticleSystemGUI();
					}
				}
				else
				{
					this.ClampWindowContentSizes();
					bool verticalLayout = ParticleEffectUI.m_VerticalLayout;
					if (verticalLayout)
					{
						this.MultiParticleSystemGUI(verticalLayout);
						this.WindowCurveEditorGUI(verticalLayout);
					}
					else
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[0]);
						this.MultiParticleSystemGUI(verticalLayout);
						this.WindowCurveEditorGUI(verticalLayout);
						GUILayout.EndHorizontal();
					}
				}
				this.ApplyModifiedProperties();
			}
		}

		private void ApplyModifiedProperties()
		{
			for (int i = 0; i < this.m_Emitters.Length; i++)
			{
				this.m_Emitters[i].ApplyProperties();
			}
		}

		internal void UpdateProperties()
		{
			for (int i = 0; i < this.m_Emitters.Length; i++)
			{
				this.m_Emitters[i].UpdateProperties();
			}
		}

		internal static bool GetAllModulesVisible()
		{
			return EditorPrefs.GetBool("ParticleSystemShowAllModules", true);
		}

		internal void SetAllModulesVisible(bool showAll)
		{
			EditorPrefs.SetBool("ParticleSystemShowAllModules", showAll);
			ParticleSystemUI[] emitters = this.m_Emitters;
			for (int i = 0; i < emitters.Length; i++)
			{
				ParticleSystemUI particleSystemUI = emitters[i];
				for (int j = 0; j < particleSystemUI.m_Modules.Length; j++)
				{
					ModuleUI moduleUI = particleSystemUI.m_Modules[j];
					if (moduleUI != null)
					{
						if (showAll)
						{
							if (!moduleUI.visibleUI)
							{
								moduleUI.visibleUI = true;
							}
						}
						else
						{
							bool flag = true;
							if (moduleUI is RendererModuleUI)
							{
								if (particleSystemUI.m_ParticleSystems.FirstOrDefault((ParticleSystem o) => o.GetComponent<ParticleSystemRenderer>() == null) == null)
								{
									flag = false;
								}
							}
							if (flag && !moduleUI.enabled)
							{
								moduleUI.visibleUI = false;
							}
						}
					}
				}
			}
		}

		internal bool IsShowOnlySelectedMode()
		{
			return this.m_ShowOnlySelectedMode;
		}

		internal void SetShowOnlySelectedMode(bool enable)
		{
			this.m_ShowOnlySelectedMode = enable;
		}
	}
}
