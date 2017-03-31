using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class SubModuleUI : ModuleUI
	{
		public enum SubEmitterType
		{
			None = -1,
			Birth,
			Collision,
			Death,
			TypesMax
		}

		private class Texts
		{
			public GUIContent create = EditorGUIUtility.TextContent("|Create and assign a Particle System as sub emitter");

			public GUIContent inherit = EditorGUIUtility.TextContent("Inherit");

			public string[] subEmitterTypeTexts = new string[]
			{
				"Birth",
				"Collision",
				"Death"
			};

			public string[] propertyStrings = new string[]
			{
				"Color",
				"Size",
				"Rotation"
			};
		}

		private SerializedProperty m_SubEmitters;

		private int m_CheckObjectIndex = -1;

		private static SubModuleUI.Texts s_Texts;

		public SubModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SubModule", displayName)
		{
			this.m_ToolTip = "Sub emission of particles. This allows each particle to emit particles in another system.";
			this.Init();
		}

		protected override void Init()
		{
			if (this.m_SubEmitters == null)
			{
				this.m_SubEmitters = base.GetProperty("subEmitters");
			}
		}

		private void CreateSubEmitter(SerializedProperty objectRefProp, int index, SubModuleUI.SubEmitterType type)
		{
			GameObject gameObject = this.m_ParticleSystemUI.m_ParticleEffectUI.CreateParticleSystem(this.m_ParticleSystemUI.m_ParticleSystems[0], type);
			gameObject.name = "SubEmitter" + index;
			objectRefProp.objectReferenceValue = gameObject.GetComponent<ParticleSystem>();
		}

		private void Update()
		{
			if (this.m_CheckObjectIndex >= 0)
			{
				if (!ObjectSelector.isVisible)
				{
					SerializedProperty arrayElementAtIndex = this.m_SubEmitters.GetArrayElementAtIndex(this.m_CheckObjectIndex);
					SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("emitter");
					UnityEngine.Object objectReferenceValue = serializedProperty.objectReferenceValue;
					ParticleSystem particleSystem = objectReferenceValue as ParticleSystem;
					if (particleSystem != null)
					{
						bool flag = true;
						if (this.ValidateSubemitter(particleSystem))
						{
							string text = ParticleSystemEditorUtils.CheckCircularReferences(particleSystem);
							if (text.Length == 0)
							{
								if (!this.CheckIfChild(objectReferenceValue))
								{
									flag = false;
								}
							}
							else
							{
								string message = string.Format("'{0}' could not be assigned as subemitter on '{1}' due to circular referencing!\nBacktrace: {2} \n\nReference will be removed.", particleSystem.gameObject.name, this.m_ParticleSystemUI.m_ParticleSystems[0].gameObject.name, text);
								EditorUtility.DisplayDialog("Circular References Detected", message, "Ok");
								flag = false;
							}
						}
						else
						{
							flag = false;
						}
						if (!flag)
						{
							serializedProperty.objectReferenceValue = null;
							this.m_ParticleSystemUI.ApplyProperties();
							this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
						}
					}
					this.m_CheckObjectIndex = -1;
					EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
				}
			}
		}

		internal static bool IsChild(ParticleSystem subEmitter, ParticleSystem root)
		{
			bool result;
			if (subEmitter == null || root == null)
			{
				result = false;
			}
			else
			{
				ParticleSystem root2 = ParticleSystemEditorUtils.GetRoot(subEmitter);
				result = (root2 == root);
			}
			return result;
		}

		private bool ValidateSubemitter(ParticleSystem subEmitter)
		{
			bool result;
			if (subEmitter == null)
			{
				result = false;
			}
			else
			{
				ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_ParticleSystemUI.m_ParticleSystems[0]);
				if (root.gameObject.activeInHierarchy && !subEmitter.gameObject.activeInHierarchy)
				{
					string message = string.Format("The assigned sub emitter is part of a prefab and can therefore not be assigned.", new object[0]);
					EditorUtility.DisplayDialog("Invalid Sub Emitter", message, "Ok");
					result = false;
				}
				else if (!root.gameObject.activeInHierarchy && subEmitter.gameObject.activeInHierarchy)
				{
					string message2 = string.Format("The assigned sub emitter is part of a scene object and can therefore not be assigned to a prefab.", new object[0]);
					EditorUtility.DisplayDialog("Invalid Sub Emitter", message2, "Ok");
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private bool CheckIfChild(UnityEngine.Object subEmitter)
		{
			ParticleSystem root = ParticleSystemEditorUtils.GetRoot(this.m_ParticleSystemUI.m_ParticleSystems[0]);
			bool result;
			if (SubModuleUI.IsChild(subEmitter as ParticleSystem, root))
			{
				result = true;
			}
			else
			{
				string message = string.Format("The assigned sub emitter is not a child of the current root particle system GameObject: '{0}' and is therefore NOT considered a part of the current effect. Do you want to reparent it?", root.gameObject.name);
				if (EditorUtility.DisplayDialog("Reparent GameObjects", message, "Yes, Reparent", "No, Remove"))
				{
					if (EditorUtility.IsPersistent(subEmitter))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(subEmitter) as GameObject;
						if (gameObject != null)
						{
							gameObject.transform.parent = this.m_ParticleSystemUI.m_ParticleSystems[0].transform;
							gameObject.transform.localPosition = Vector3.zero;
							gameObject.transform.localRotation = Quaternion.identity;
						}
					}
					else
					{
						ParticleSystem particleSystem = subEmitter as ParticleSystem;
						if (particleSystem)
						{
							Undo.SetTransformParent(particleSystem.gameObject.transform.transform, this.m_ParticleSystemUI.m_ParticleSystems[0].transform, "Reparent sub emitter");
						}
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private List<UnityEngine.Object> GetSubEmitterProperties()
		{
			List<UnityEngine.Object> list = new List<UnityEngine.Object>();
			IEnumerator enumerator = this.m_SubEmitters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializedProperty serializedProperty = (SerializedProperty)enumerator.Current;
				list.Add(serializedProperty.FindPropertyRelative("emitter").objectReferenceValue);
			}
			return list;
		}

		public override void OnInspectorGUI(InitialModuleUI initial)
		{
			if (SubModuleUI.s_Texts == null)
			{
				SubModuleUI.s_Texts = new SubModuleUI.Texts();
			}
			if (this.m_ParticleSystemUI.multiEdit)
			{
				EditorGUILayout.HelpBox("Sub Emitter editing is only available when editing a single Particle System", MessageType.Info, true);
			}
			else
			{
				List<UnityEngine.Object> subEmitterProperties = this.GetSubEmitterProperties();
				GUILayout.BeginHorizontal(new GUILayoutOption[]
				{
					GUILayout.Height(16f)
				});
				GUILayout.Label("", ParticleSystemStyles.Get().label, new GUILayoutOption[]
				{
					GUILayout.ExpandWidth(true)
				});
				GUILayout.Label(SubModuleUI.s_Texts.inherit, ParticleSystemStyles.Get().label, new GUILayoutOption[]
				{
					GUILayout.Width(120f)
				});
				GUILayout.EndHorizontal();
				for (int i = 0; i < this.m_SubEmitters.arraySize; i++)
				{
					this.ShowSubEmitter(i);
				}
				List<UnityEngine.Object> subEmitterProperties2 = this.GetSubEmitterProperties();
				for (int j = 0; j < Mathf.Min(subEmitterProperties.Count, subEmitterProperties2.Count); j++)
				{
					if (subEmitterProperties[j] != subEmitterProperties2[j])
					{
						if (this.m_CheckObjectIndex == -1)
						{
							EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
						}
						this.m_CheckObjectIndex = j;
					}
				}
			}
		}

		private void ShowSubEmitter(int index)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[]
			{
				GUILayout.Height(16f)
			});
			SerializedProperty arrayElementAtIndex = this.m_SubEmitters.GetArrayElementAtIndex(index);
			SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("emitter");
			SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("type");
			SerializedProperty intProp = arrayElementAtIndex.FindPropertyRelative("properties");
			ModuleUI.GUIPopup(GUIContent.none, serializedProperty2, SubModuleUI.s_Texts.subEmitterTypeTexts, new GUILayoutOption[]
			{
				GUILayout.MaxWidth(80f)
			});
			GUILayout.Label("", ParticleSystemStyles.Get().label, new GUILayoutOption[]
			{
				GUILayout.Width(4f)
			});
			ModuleUI.GUIObject(GUIContent.none, serializedProperty, new GUILayoutOption[0]);
			GUIStyle gUIStyle = new GUIStyle("OL Plus");
			if (serializedProperty.objectReferenceValue == null)
			{
				GUILayout.Label("", ParticleSystemStyles.Get().label, new GUILayoutOption[]
				{
					GUILayout.Width(8f)
				});
				GUILayout.BeginVertical(new GUILayoutOption[]
				{
					GUILayout.Width(16f),
					GUILayout.Height(gUIStyle.fixedHeight)
				});
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(GUIContent.none, ParticleSystemStyles.Get().plus, new GUILayoutOption[0]))
				{
					this.CreateSubEmitter(serializedProperty, index, (SubModuleUI.SubEmitterType)serializedProperty2.intValue);
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
			}
			else
			{
				GUILayout.Label("", ParticleSystemStyles.Get().label, new GUILayoutOption[]
				{
					GUILayout.Width(24f)
				});
			}
			ModuleUI.GUIMask(GUIContent.none, intProp, SubModuleUI.s_Texts.propertyStrings, new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			GUILayout.Label("", ParticleSystemStyles.Get().label, new GUILayoutOption[]
			{
				GUILayout.Width(8f)
			});
			if (index == 0)
			{
				if (GUILayout.Button(GUIContent.none, gUIStyle, new GUILayoutOption[]
				{
					GUILayout.Width(16f)
				}))
				{
					this.m_SubEmitters.InsertArrayElementAtIndex(this.m_SubEmitters.arraySize);
					SerializedProperty arrayElementAtIndex2 = this.m_SubEmitters.GetArrayElementAtIndex(this.m_SubEmitters.arraySize - 1);
					SerializedProperty serializedProperty3 = arrayElementAtIndex2.FindPropertyRelative("emitter");
					serializedProperty3.objectReferenceValue = null;
				}
			}
			else if (GUILayout.Button(GUIContent.none, new GUIStyle("OL Minus"), new GUILayoutOption[]
			{
				GUILayout.Width(16f)
			}))
			{
				this.m_SubEmitters.DeleteArrayElementAtIndex(index);
			}
			GUILayout.EndHorizontal();
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			text += "\n\tSub Emitters are enabled.";
		}
	}
}
