using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ParticleSystemUI
	{
		public enum DefaultTypes
		{
			Root,
			SubBirth,
			SubCollision,
			SubDeath
		}

		protected class Texts
		{
			public GUIContent addModules = new GUIContent(string.Empty, "Show/Hide Modules");

			public GUIContent supportsCullingText = new GUIContent(string.Empty, ParticleSystemStyles.Get().warningIcon);
		}

		public ParticleEffectUI m_ParticleEffectUI;

		public ModuleUI[] m_Modules;

		public ParticleSystem m_ParticleSystem;

		public SerializedObject m_ParticleSystemSerializedObject;

		public SerializedObject m_RendererSerializedObject;

		private static string[] s_ModuleNames;

		private string m_SupportsCullingText;

		private static ParticleSystemUI.Texts s_Texts;

		public void Init(ParticleEffectUI owner, ParticleSystem ps)
		{
			if (ParticleSystemUI.s_ModuleNames == null)
			{
				ParticleSystemUI.s_ModuleNames = ParticleSystemUI.GetUIModuleNames();
			}
			this.m_ParticleEffectUI = owner;
			this.m_ParticleSystem = ps;
			this.m_ParticleSystemSerializedObject = new SerializedObject(this.m_ParticleSystem);
			this.m_RendererSerializedObject = null;
			this.m_SupportsCullingText = null;
			this.m_Modules = ParticleSystemUI.CreateUIModules(this, this.m_ParticleSystemSerializedObject);
			ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
			if (particleSystemRenderer != null)
			{
				this.InitRendererUI();
			}
			this.UpdateParticleSystemInfoString();
		}

		internal ParticleSystemRenderer GetParticleSystemRenderer()
		{
			if (this.m_ParticleSystem != null)
			{
				return this.m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
			}
			return null;
		}

		internal ModuleUI GetParticleSystemRendererModuleUI()
		{
			return this.m_Modules[this.m_Modules.Length - 1];
		}

		private void InitRendererUI()
		{
			ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
			if (particleSystemRenderer == null)
			{
				this.m_ParticleSystem.gameObject.AddComponent<ParticleSystemRenderer>();
			}
			particleSystemRenderer = this.GetParticleSystemRenderer();
			if (particleSystemRenderer != null)
			{
				this.m_RendererSerializedObject = new SerializedObject(particleSystemRenderer);
				this.m_Modules[this.m_Modules.Length - 1] = new RendererModuleUI(this, this.m_RendererSerializedObject, ParticleSystemUI.s_ModuleNames[ParticleSystemUI.s_ModuleNames.Length - 1]);
				EditorUtility.SetSelectedWireframeHidden(particleSystemRenderer, !ParticleEffectUI.m_ShowWireframe);
			}
		}

		private void ClearRenderer()
		{
			this.m_RendererSerializedObject = null;
			ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
			if (particleSystemRenderer != null)
			{
				Undo.DestroyObjectImmediate(particleSystemRenderer);
			}
			this.m_Modules[this.m_Modules.Length - 1] = null;
		}

		public string GetName()
		{
			return this.m_ParticleSystem.gameObject.name;
		}

		public float GetEmitterDuration()
		{
			InitialModuleUI initialModuleUI = this.m_Modules[0] as InitialModuleUI;
			if (initialModuleUI != null)
			{
				return initialModuleUI.m_LengthInSec.floatValue;
			}
			return -1f;
		}

		private ParticleSystem GetSelectedParticleSystem()
		{
			return Selection.activeGameObject.GetComponent<ParticleSystem>();
		}

		public void OnGUI(ParticleSystem root, float width, bool fixedWidth)
		{
			if (ParticleSystemUI.s_Texts == null)
			{
				ParticleSystemUI.s_Texts = new ParticleSystemUI.Texts();
			}
			bool flag = Event.current.type == EventType.Repaint;
			string text = (!this.m_ParticleSystem) ? null : this.m_ParticleSystem.gameObject.name;
			if (fixedWidth)
			{
				EditorGUIUtility.labelWidth = width * 0.55f;
				EditorGUILayout.BeginVertical(new GUILayoutOption[]
				{
					GUILayout.Width(width)
				});
			}
			else
			{
				EditorGUIUtility.labelWidth = 0f;
				EditorGUIUtility.labelWidth -= 4f;
				EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
			}
			for (int i = 0; i < this.m_Modules.Length; i++)
			{
				ModuleUI moduleUI = this.m_Modules[i];
				if (moduleUI != null)
				{
					bool flag2 = moduleUI == this.m_Modules[0];
					if (moduleUI.visibleUI || flag2)
					{
						GUIContent gUIContent = new GUIContent();
						Rect rect;
						GUIStyle style;
						if (flag2)
						{
							rect = GUILayoutUtility.GetRect(width, 25f);
							style = ParticleSystemStyles.Get().emitterHeaderStyle;
						}
						else
						{
							rect = GUILayoutUtility.GetRect(width, 15f);
							style = ParticleSystemStyles.Get().moduleHeaderStyle;
						}
						if (moduleUI.foldout)
						{
							using (new EditorGUI.DisabledScope(!moduleUI.enabled))
							{
								Rect position = EditorGUILayout.BeginVertical(ParticleSystemStyles.Get().modulePadding, new GUILayoutOption[0]);
								position.y -= 4f;
								position.height += 4f;
								GUI.Label(position, GUIContent.none, ParticleSystemStyles.Get().moduleBgStyle);
								moduleUI.OnInspectorGUI(this.m_ParticleSystem);
								EditorGUILayout.EndVertical();
							}
						}
						if (flag2)
						{
							ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
							float num = 21f;
							Rect position2 = new Rect(rect.x + 4f, rect.y + 2f, num, num);
							if (flag && particleSystemRenderer != null)
							{
								bool flag3 = false;
								int num2 = 0;
								RendererModuleUI rendererModuleUI = this.m_Modules[this.m_Modules.Length - 1] as RendererModuleUI;
								if (rendererModuleUI != null)
								{
									if (rendererModuleUI.IsMeshEmitter())
									{
										if (particleSystemRenderer.mesh != null)
										{
											num2 = particleSystemRenderer.mesh.GetInstanceID();
										}
									}
									else if (particleSystemRenderer.sharedMaterial != null)
									{
										num2 = particleSystemRenderer.sharedMaterial.GetInstanceID();
									}
									if (EditorUtility.IsDirty(num2))
									{
										AssetPreview.ClearTemporaryAssetPreviews();
									}
								}
								if (num2 != 0)
								{
									Texture2D assetPreview = AssetPreview.GetAssetPreview(num2);
									if (assetPreview != null)
									{
										GUI.DrawTexture(position2, assetPreview, ScaleMode.StretchToFill, true);
										flag3 = true;
									}
								}
								if (!flag3)
								{
									GUI.Label(position2, GUIContent.none, ParticleSystemStyles.Get().moduleBgStyle);
								}
							}
							if (EditorGUI.ButtonMouseDown(position2, GUIContent.none, FocusType.Passive, GUIStyle.none))
							{
								if (EditorGUI.actionKey)
								{
									List<int> list = new List<int>();
									int instanceID = this.m_ParticleSystem.gameObject.GetInstanceID();
									list.AddRange(Selection.instanceIDs);
									if (!list.Contains(instanceID) || list.Count != 1)
									{
										if (list.Contains(instanceID))
										{
											list.Remove(instanceID);
										}
										else
										{
											list.Add(instanceID);
										}
									}
									Selection.instanceIDs = list.ToArray();
								}
								else
								{
									Selection.instanceIDs = new int[0];
									Selection.activeInstanceID = this.m_ParticleSystem.gameObject.GetInstanceID();
								}
							}
						}
						Rect position3 = new Rect(rect.x + 2f, rect.y + 1f, 13f, 13f);
						if (!flag2 && GUI.Button(position3, GUIContent.none, GUIStyle.none))
						{
							moduleUI.enabled = !moduleUI.enabled;
						}
						Rect position4 = new Rect(rect.x + rect.width - 10f, rect.y + rect.height - 10f, 10f, 10f);
						Rect position5 = new Rect(position4.x - 4f, position4.y - 4f, position4.width + 4f, position4.height + 4f);
						Rect position6 = new Rect(position4.x - 23f, position4.y - 3f, 16f, 16f);
						if (flag2 && EditorGUI.ButtonMouseDown(position5, ParticleSystemUI.s_Texts.addModules, FocusType.Passive, GUIStyle.none))
						{
							this.ShowAddModuleMenu();
						}
						if (!string.IsNullOrEmpty(text))
						{
							gUIContent.text = ((!flag2) ? moduleUI.displayName : text);
						}
						else
						{
							gUIContent.text = moduleUI.displayName;
						}
						gUIContent.tooltip = moduleUI.toolTip;
						bool flag4 = GUI.Toggle(rect, moduleUI.foldout, gUIContent, style);
						if (flag4 != moduleUI.foldout)
						{
							int button = Event.current.button;
							if (button != 0)
							{
								if (button == 1)
								{
									if (flag2)
									{
										this.ShowEmitterMenu();
									}
									else
									{
										this.ShowModuleMenu(i);
									}
								}
							}
							else
							{
								bool foldout = !moduleUI.foldout;
								if (Event.current.control)
								{
									ModuleUI[] modules = this.m_Modules;
									for (int j = 0; j < modules.Length; j++)
									{
										ModuleUI moduleUI2 = modules[j];
										if (moduleUI2 != null && moduleUI2.visibleUI)
										{
											moduleUI2.foldout = foldout;
										}
									}
								}
								else
								{
									moduleUI.foldout = foldout;
								}
							}
						}
						if (!flag2)
						{
							GUI.Toggle(position3, moduleUI.enabled, GUIContent.none, ParticleSystemStyles.Get().checkmark);
						}
						if (flag && flag2)
						{
							GUI.Label(position4, GUIContent.none, ParticleSystemStyles.Get().plus);
						}
						ParticleSystemUI.s_Texts.supportsCullingText.tooltip = this.m_SupportsCullingText;
						if (flag2 && ParticleSystemUI.s_Texts.supportsCullingText.tooltip != null)
						{
							GUI.Label(position6, ParticleSystemUI.s_Texts.supportsCullingText);
						}
						GUILayout.Space(1f);
					}
				}
			}
			GUILayout.Space(-1f);
			EditorGUILayout.EndVertical();
			this.ApplyProperties();
		}

		public void OnSceneGUI()
		{
			if (this.m_Modules == null || this.m_ParticleSystem == null)
			{
				return;
			}
			if (this.m_ParticleSystem.particleCount > 0)
			{
				ParticleSystemRenderer particleSystemRenderer = this.GetParticleSystemRenderer();
				if (ParticleEffectUI.m_ShowBounds)
				{
					Color color = Handles.color;
					Handles.color = Color.yellow;
					Bounds bounds = particleSystemRenderer.bounds;
					Handles.DrawWireCube(bounds.center, bounds.size);
					Handles.color = color;
				}
				EditorUtility.SetSelectedWireframeHidden(particleSystemRenderer, !ParticleEffectUI.m_ShowWireframe);
			}
			this.UpdateProperties();
			InitialModuleUI initial = (InitialModuleUI)this.m_Modules[0];
			ModuleUI[] modules = this.m_Modules;
			for (int i = 0; i < modules.Length; i++)
			{
				ModuleUI moduleUI = modules[i];
				if (moduleUI != null && moduleUI.visibleUI && moduleUI.enabled)
				{
					if (moduleUI.foldout)
					{
						moduleUI.OnSceneGUI(this.m_ParticleSystem, initial);
					}
				}
			}
			this.ApplyProperties();
		}

		public void ApplyProperties()
		{
			bool hasModifiedProperties = this.m_ParticleSystemSerializedObject.hasModifiedProperties;
			this.m_ParticleSystemSerializedObject.ApplyModifiedProperties();
			if (hasModifiedProperties)
			{
				ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_ParticleSystem);
				if (!ParticleEffectUI.IsStopped(root) && ParticleSystemEditorUtils.editorResimulation)
				{
					ParticleSystemEditorUtils.PerformCompleteResimulation();
				}
				this.UpdateParticleSystemInfoString();
			}
			if (this.m_RendererSerializedObject != null)
			{
				this.m_RendererSerializedObject.ApplyModifiedProperties();
			}
		}

		private void UpdateParticleSystemInfoString()
		{
			string empty = string.Empty;
			ModuleUI[] modules = this.m_Modules;
			for (int i = 0; i < modules.Length; i++)
			{
				ModuleUI moduleUI = modules[i];
				if (moduleUI != null && moduleUI.visibleUI && moduleUI.enabled)
				{
					moduleUI.UpdateCullingSupportedString(ref empty);
				}
			}
			if (empty != string.Empty)
			{
				this.m_SupportsCullingText = "Automatic culling is disabled because: " + empty;
			}
			else
			{
				this.m_SupportsCullingText = null;
			}
		}

		public void UpdateProperties()
		{
			this.m_ParticleSystemSerializedObject.UpdateIfDirtyOrScript();
			if (this.m_RendererSerializedObject != null)
			{
				this.m_RendererSerializedObject.UpdateIfDirtyOrScript();
			}
		}

		private void ResetModules()
		{
			ModuleUI[] modules = this.m_Modules;
			for (int i = 0; i < modules.Length; i++)
			{
				ModuleUI moduleUI = modules[i];
				if (moduleUI != null)
				{
					moduleUI.enabled = false;
					if (!ParticleEffectUI.GetAllModulesVisible())
					{
						moduleUI.visibleUI = false;
					}
				}
			}
			if (this.m_Modules[this.m_Modules.Length - 1] == null)
			{
				this.InitRendererUI();
			}
			int[] array = new int[]
			{
				1,
				2,
				this.m_Modules.Length - 1
			};
			for (int j = 0; j < array.Length; j++)
			{
				int num = array[j];
				if (this.m_Modules[num] != null)
				{
					this.m_Modules[num].enabled = true;
					this.m_Modules[num].visibleUI = true;
				}
			}
		}

		private void ShowAddModuleMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			for (int i = 0; i < ParticleSystemUI.s_ModuleNames.Length; i++)
			{
				if (this.m_Modules[i] == null || !this.m_Modules[i].visibleUI)
				{
					genericMenu.AddItem(new GUIContent(ParticleSystemUI.s_ModuleNames[i]), false, new GenericMenu.MenuFunction2(this.AddModuleCallback), i);
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent(ParticleSystemUI.s_ModuleNames[i]));
				}
			}
			genericMenu.AddSeparator(string.Empty);
			genericMenu.AddItem(new GUIContent("Show All Modules"), ParticleEffectUI.GetAllModulesVisible(), new GenericMenu.MenuFunction2(this.AddModuleCallback), 10000);
			genericMenu.ShowAsContext();
			Event.current.Use();
		}

		private void AddModuleCallback(object obj)
		{
			int num = (int)obj;
			if (num >= 0 && num < this.m_Modules.Length)
			{
				if (num == this.m_Modules.Length - 1)
				{
					this.InitRendererUI();
				}
				else
				{
					this.m_Modules[num].enabled = true;
					this.m_Modules[num].foldout = true;
				}
			}
			else
			{
				this.m_ParticleEffectUI.SetAllModulesVisible(!ParticleEffectUI.GetAllModulesVisible());
			}
			this.ApplyProperties();
		}

		private void ModuleMenuCallback(object obj)
		{
			int num = (int)obj;
			bool flag = num == this.m_Modules.Length - 1;
			if (flag)
			{
				this.ClearRenderer();
			}
			else
			{
				if (!ParticleEffectUI.GetAllModulesVisible())
				{
					this.m_Modules[num].visibleUI = false;
				}
				this.m_Modules[num].enabled = false;
			}
		}

		private void ShowModuleMenu(int moduleIndex)
		{
			GenericMenu genericMenu = new GenericMenu();
			if (!ParticleEffectUI.GetAllModulesVisible())
			{
				genericMenu.AddItem(new GUIContent("Remove"), false, new GenericMenu.MenuFunction2(this.ModuleMenuCallback), moduleIndex);
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent("Remove"));
			}
			genericMenu.ShowAsContext();
			Event.current.Use();
		}

		private void EmitterMenuCallback(object obj)
		{
			switch ((int)obj)
			{
			case 0:
				this.m_ParticleEffectUI.CreateParticleSystem(this.m_ParticleSystem, SubModuleUI.SubEmitterType.None);
				break;
			case 1:
				this.ResetModules();
				break;
			case 2:
				EditorGUIUtility.PingObject(this.m_ParticleSystem);
				break;
			}
		}

		private void ShowEmitterMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Show Location"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 2);
			genericMenu.AddSeparator(string.Empty);
			if (this.m_ParticleSystem.gameObject.activeInHierarchy)
			{
				genericMenu.AddItem(new GUIContent("Create Particle System"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 0);
			}
			else
			{
				genericMenu.AddDisabledItem(new GUIContent("Create new Particle System"));
			}
			genericMenu.AddItem(new GUIContent("Reset"), false, new GenericMenu.MenuFunction2(this.EmitterMenuCallback), 1);
			genericMenu.ShowAsContext();
			Event.current.Use();
		}

		private static ModuleUI[] CreateUIModules(ParticleSystemUI e, SerializedObject so)
		{
			int num = 0;
			ModuleUI[] expr_09 = new ModuleUI[19];
			expr_09[0] = new InitialModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[1] = new EmissionModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[2] = new ShapeModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[3] = new VelocityModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[4] = new ClampVelocityModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[5] = new InheritVelocityModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[6] = new ForceModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[7] = new ColorModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[8] = new ColorByVelocityModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[9] = new SizeModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[10] = new SizeByVelocityModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[11] = new RotationModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[12] = new RotationByVelocityModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[13] = new ExternalForcesModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[14] = new CollisionModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[15] = new TriggerModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[16] = new SubModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			expr_09[17] = new UVModuleUI(e, so, ParticleSystemUI.s_ModuleNames[num++]);
			return expr_09;
		}

		public static string[] GetUIModuleNames()
		{
			return new string[]
			{
				string.Empty,
				"Emission",
				"Shape",
				"Velocity over Lifetime",
				"Limit Velocity over Lifetime",
				"Inherit Velocity",
				"Force over Lifetime",
				"Color over Lifetime",
				"Color by Speed",
				"Size over Lifetime",
				"Size by Speed",
				"Rotation over Lifetime",
				"Rotation by Speed",
				"External Forces",
				"Collision",
				"Triggers",
				"Sub Emitters",
				"Texture Sheet Animation",
				"Renderer"
			};
		}
	}
}
