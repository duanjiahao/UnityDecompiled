using System;
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
			public GUIContent[] subEmitterTypeTexts;

			public GUIContent create = EditorGUIUtility.TextContent("|Create and assign a Particle System as sub emitter");

			public Texts()
			{
				this.subEmitterTypeTexts = new GUIContent[3];
				this.subEmitterTypeTexts[0] = EditorGUIUtility.TextContent("Birth|Start spawning on birth of particle.");
				this.subEmitterTypeTexts[1] = EditorGUIUtility.TextContent("Collision|Spawn on collision of particle. Sub emitter can only emit as burst.");
				this.subEmitterTypeTexts[2] = EditorGUIUtility.TextContent("Death|Spawn on death of particle. Sub emitter can only emit as burst.");
			}
		}

		private const int k_MaxSubPerType = 2;

		private SerializedProperty[,] m_SubEmitters;

		private int m_CheckObjectTypeIndex = -1;

		private int m_CheckObjectIndex = -1;

		private static SubModuleUI.Texts s_Texts;

		public SubModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SubModule", displayName)
		{
			this.m_ToolTip = "Sub emission of particles. This allows each particle to emit particles in another system.";
			this.Init();
		}

		protected override void Init()
		{
			if (this.m_SubEmitters != null)
			{
				return;
			}
			this.m_SubEmitters = new SerializedProperty[3, 2];
			this.m_SubEmitters[0, 0] = base.GetProperty("subEmitterBirth");
			this.m_SubEmitters[0, 1] = base.GetProperty("subEmitterBirth1");
			this.m_SubEmitters[1, 0] = base.GetProperty("subEmitterCollision");
			this.m_SubEmitters[1, 1] = base.GetProperty("subEmitterCollision1");
			this.m_SubEmitters[2, 0] = base.GetProperty("subEmitterDeath");
			this.m_SubEmitters[2, 1] = base.GetProperty("subEmitterDeath1");
		}

		public override void Validate()
		{
		}

		private void CreateAndAssignSubEmitter(SerializedProperty objectRefProp, SubModuleUI.SubEmitterType type)
		{
			GameObject gameObject = this.m_ParticleSystemUI.m_ParticleEffectUI.CreateParticleSystem(this.m_ParticleSystemUI.m_ParticleSystem, type);
			switch (type)
			{
			case SubModuleUI.SubEmitterType.Birth:
				gameObject.name = "SubEmitterBirth";
				break;
			case SubModuleUI.SubEmitterType.Collision:
				gameObject.name = "SubEmitterCollision";
				break;
			case SubModuleUI.SubEmitterType.Death:
				gameObject.name = "SubEmitterDeath";
				break;
			default:
				gameObject.name = "SubEmitter";
				break;
			}
			objectRefProp.objectReferenceValue = gameObject.GetComponent<ParticleSystem>();
		}

		private void Update()
		{
			if (this.m_CheckObjectIndex >= 0 && this.m_CheckObjectTypeIndex >= 0 && !ObjectSelector.isVisible)
			{
				UnityEngine.Object objectReferenceValue = this.m_SubEmitters[this.m_CheckObjectTypeIndex, this.m_CheckObjectIndex].objectReferenceValue;
				ParticleSystem particleSystem = objectReferenceValue as ParticleSystem;
				if (particleSystem != null)
				{
					bool flag = true;
					if (this.ValidateSubemitter(particleSystem))
					{
						string text = ParticleSystemEditorUtils.CheckCircularReferences(particleSystem);
						if (text.Length == 0)
						{
							this.CheckIfChild(objectReferenceValue);
						}
						else
						{
							string message = string.Format("'{0}' could not be assigned as subemitter on '{1}' due to circular referencing!\nBacktrace: {2} \n\nReference will be removed.", particleSystem.gameObject.name, this.m_ParticleSystemUI.m_ParticleSystem.gameObject.name, text);
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
						this.m_SubEmitters[this.m_CheckObjectTypeIndex, this.m_CheckObjectIndex].objectReferenceValue = null;
						this.m_ParticleSystemUI.ApplyProperties();
						this.m_ParticleSystemUI.m_ParticleEffectUI.m_Owner.Repaint();
					}
				}
				this.m_CheckObjectIndex = -1;
				this.m_CheckObjectTypeIndex = -1;
				EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			}
		}

		internal static bool IsChild(ParticleSystem subEmitter, ParticleSystem root)
		{
			if (subEmitter == null || root == null)
			{
				return false;
			}
			ParticleSystem root2 = ParticleSystemEditorUtils.GetRoot(subEmitter);
			return root2 == root;
		}

		private bool ValidateSubemitter(ParticleSystem subEmitter)
		{
			if (subEmitter == null)
			{
				return false;
			}
			ParticleSystem root = this.m_ParticleSystemUI.m_ParticleEffectUI.GetRoot();
			if (root.gameObject.activeInHierarchy && !subEmitter.gameObject.activeInHierarchy)
			{
				string message = string.Format("The assigned sub emitter is part of a prefab and can therefore not be assigned.", new object[0]);
				EditorUtility.DisplayDialog("Invalid Sub Emitter", message, "Ok");
				return false;
			}
			if (!root.gameObject.activeInHierarchy && subEmitter.gameObject.activeInHierarchy)
			{
				string message2 = string.Format("The assigned sub emitter is part of a scene object and can therefore not be assigned to a prefab.", new object[0]);
				EditorUtility.DisplayDialog("Invalid Sub Emitter", message2, "Ok");
				return false;
			}
			return true;
		}

		private void CheckIfChild(UnityEngine.Object subEmitter)
		{
			if (subEmitter != null)
			{
				ParticleSystem root = this.m_ParticleSystemUI.m_ParticleEffectUI.GetRoot();
				if (SubModuleUI.IsChild(subEmitter as ParticleSystem, root))
				{
					return;
				}
				string message = string.Format("The assigned sub emitter is not a child of the current root particle system GameObject: '{0}' and is therefore NOT considered a part of the current effect. Do you want to reparent it?", root.gameObject.name);
				if (EditorUtility.DisplayDialog("Reparent GameObjects", message, "Yes, Reparent", "No"))
				{
					if (EditorUtility.IsPersistent(subEmitter))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(subEmitter) as GameObject;
						if (gameObject != null)
						{
							gameObject.transform.parent = this.m_ParticleSystemUI.m_ParticleSystem.transform;
							gameObject.transform.localPosition = Vector3.zero;
							gameObject.transform.localRotation = Quaternion.identity;
						}
					}
					else
					{
						ParticleSystem particleSystem = subEmitter as ParticleSystem;
						if (particleSystem)
						{
							Undo.SetTransformParent(this.m_ParticleSystemUI.m_ParticleSystem.transform, particleSystem.gameObject.transform.transform, "Reparent sub emitter");
						}
					}
				}
			}
		}

		public override void OnInspectorGUI(ParticleSystem s)
		{
			if (SubModuleUI.s_Texts == null)
			{
				SubModuleUI.s_Texts = new SubModuleUI.Texts();
			}
			UnityEngine.Object[,] expr_1B = new UnityEngine.Object[3, 2];
			expr_1B[0, 0] = this.m_SubEmitters[0, 0].objectReferenceValue;
			expr_1B[0, 1] = this.m_SubEmitters[0, 1].objectReferenceValue;
			expr_1B[1, 0] = this.m_SubEmitters[1, 0].objectReferenceValue;
			expr_1B[1, 1] = this.m_SubEmitters[1, 1].objectReferenceValue;
			expr_1B[2, 0] = this.m_SubEmitters[2, 0].objectReferenceValue;
			expr_1B[2, 1] = this.m_SubEmitters[2, 1].objectReferenceValue;
			UnityEngine.Object[,] array = expr_1B;
			for (int i = 0; i < 3; i++)
			{
				int num = base.GUIListOfFloatObjectToggleFields(SubModuleUI.s_Texts.subEmitterTypeTexts[i], new SerializedProperty[]
				{
					this.m_SubEmitters[i, 0],
					this.m_SubEmitters[i, 1]
				}, null, SubModuleUI.s_Texts.create, true);
				if (num != -1)
				{
					this.CreateAndAssignSubEmitter(this.m_SubEmitters[i, num], (SubModuleUI.SubEmitterType)i);
				}
			}
			UnityEngine.Object[,] expr_131 = new UnityEngine.Object[3, 2];
			expr_131[0, 0] = this.m_SubEmitters[0, 0].objectReferenceValue;
			expr_131[0, 1] = this.m_SubEmitters[0, 1].objectReferenceValue;
			expr_131[1, 0] = this.m_SubEmitters[1, 0].objectReferenceValue;
			expr_131[1, 1] = this.m_SubEmitters[1, 1].objectReferenceValue;
			expr_131[2, 0] = this.m_SubEmitters[2, 0].objectReferenceValue;
			expr_131[2, 1] = this.m_SubEmitters[2, 1].objectReferenceValue;
			UnityEngine.Object[,] array2 = expr_131;
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					if (array[j, k] != array2[j, k])
					{
						if (this.m_CheckObjectIndex == -1 && this.m_CheckObjectTypeIndex == -1)
						{
							EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
						}
						this.m_CheckObjectTypeIndex = j;
						this.m_CheckObjectIndex = k;
					}
				}
			}
		}

		public override void UpdateCullingSupportedString(ref string text)
		{
			text += "\n\tSub Emitters are enabled.";
		}
	}
}
