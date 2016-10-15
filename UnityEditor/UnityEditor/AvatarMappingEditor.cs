using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class AvatarMappingEditor : AvatarSubEditor
	{
		internal class Styles
		{
			public GUIContent[] BodyPartMapping = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Avatar"),
				EditorGUIUtility.TextContent("Body"),
				EditorGUIUtility.TextContent("Head"),
				EditorGUIUtility.TextContent("Left Arm"),
				EditorGUIUtility.TextContent("Left Fingers"),
				EditorGUIUtility.TextContent("Right Arm"),
				EditorGUIUtility.TextContent("Right Fingers"),
				EditorGUIUtility.TextContent("Left Leg"),
				EditorGUIUtility.TextContent("Right Leg")
			};

			public GUIContent RequiredBone = EditorGUIUtility.TextContent("Optional Bones");

			public GUIContent DoneCharacter = EditorGUIUtility.TextContent("Done");

			public GUIContent mapping = EditorGUIUtility.TextContent("Mapping");

			public GUIContent clearMapping = EditorGUIUtility.TextContent("Clear");

			public GUIContent autoMapping = EditorGUIUtility.TextContent("Automap");

			public GUIContent bipedMapping = EditorGUIUtility.TextContent("Biped");

			public GUIContent loadMapping = EditorGUIUtility.TextContent("Load");

			public GUIContent saveMapping = EditorGUIUtility.TextContent("Save");

			public GUIContent pose = EditorGUIUtility.TextContent("Pose");

			public GUIContent resetPose = EditorGUIUtility.TextContent("Reset");

			public GUIContent sampleBindPose = EditorGUIUtility.TextContent("Sample Bind-Pose");

			public GUIContent enforceTPose = EditorGUIUtility.TextContent("Enforce T-Pose");

			public GUIContent bipedPose = EditorGUIUtility.TextContent("Biped Pose");

			public GUIContent ShowError = EditorGUIUtility.TextContent("Show Error (s)...");

			public GUIContent CloseError = EditorGUIUtility.TextContent("Close Error (s)");

			public GUIContent dotFill = EditorGUIUtility.IconContent("AvatarInspector/DotFill");

			public GUIContent dotFrame = EditorGUIUtility.IconContent("AvatarInspector/DotFrame");

			public GUIContent dotFrameDotted = EditorGUIUtility.IconContent("AvatarInspector/DotFrameDotted");

			public GUIContent dotSelection = EditorGUIUtility.IconContent("AvatarInspector/DotSelection");

			public GUIStyle box = new GUIStyle("box");

			public GUIStyle toolbar = "TE Toolbar";

			public GUIStyle toolbarDropDown = "TE ToolbarDropDown";

			public GUIStyle errorLabel = new GUIStyle(EditorStyles.wordWrappedMiniLabel);

			public Styles()
			{
				this.box.padding = new RectOffset(0, 0, 0, 0);
				this.box.margin = new RectOffset(0, 0, 0, 0);
				this.errorLabel.normal.textColor = new Color(0.6f, 0f, 0f, 1f);
			}
		}

		private static AvatarMappingEditor.Styles s_Styles;

		protected bool[] m_BodyPartToggle;

		protected bool[] m_BodyPartFoldout;

		protected int m_BodyView;

		[SerializeField]
		protected AvatarSetupTool.BoneWrapper[] m_Bones;

		internal static int s_SelectedBoneIndex = -1;

		internal static bool s_DirtySelection;

		protected bool m_HasSkinnedMesh;

		private bool m_IsBiped;

		private Editor m_CurrentTransformEditor;

		private bool m_CurrentTransformEditorFoldout;

		protected int[][] m_BodyPartHumanBone = new int[][]
		{
			new int[]
			{
				-1
			},
			new int[]
			{
				0,
				7,
				8
			},
			new int[]
			{
				9,
				10,
				21,
				22,
				23
			},
			new int[]
			{
				11,
				13,
				15,
				17
			},
			new int[]
			{
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36,
				37,
				38
			},
			new int[]
			{
				12,
				14,
				16,
				18
			},
			new int[]
			{
				39,
				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50,
				51,
				52,
				53
			},
			new int[]
			{
				1,
				3,
				5,
				19
			},
			new int[]
			{
				2,
				4,
				6,
				20
			}
		};

		private Vector2 m_FoldoutScroll = Vector2.zero;

		internal static AvatarMappingEditor.Styles styles
		{
			get
			{
				if (AvatarMappingEditor.s_Styles == null)
				{
					AvatarMappingEditor.s_Styles = new AvatarMappingEditor.Styles();
				}
				return AvatarMappingEditor.s_Styles;
			}
		}

		public AvatarMappingEditor()
		{
			this.m_BodyPartToggle = new bool[9];
			this.m_BodyPartFoldout = new bool[9];
			for (int i = 0; i < 9; i++)
			{
				this.m_BodyPartToggle[i] = false;
				this.m_BodyPartFoldout[i] = true;
			}
		}

		public override void Enable(AvatarEditor inspector)
		{
			base.Enable(inspector);
			this.Init();
		}

		public override void Disable()
		{
			if (this.m_CurrentTransformEditor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_CurrentTransformEditor);
			}
			base.Disable();
		}

		public override void OnDestroy()
		{
			if (this.m_CurrentTransformEditor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_CurrentTransformEditor);
			}
			base.OnDestroy();
		}

		protected void Init()
		{
			if (base.gameObject == null)
			{
				return;
			}
			if (AvatarSetupTool.sHumanParent.Length != HumanTrait.BoneCount)
			{
				throw new Exception("Avatar's Human parent list is out of sync");
			}
			this.m_IsBiped = AvatarBipedMapper.IsBiped(base.gameObject.transform, null);
			if (this.m_Bones == null)
			{
				this.m_Bones = AvatarSetupTool.GetHumanBones(base.serializedObject, base.modelBones);
			}
			this.ValidateMapping();
			if (this.m_CurrentTransformEditor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_CurrentTransformEditor);
				this.m_CurrentTransformEditor = null;
			}
			this.m_CurrentTransformEditorFoldout = true;
			this.m_HasSkinnedMesh = (base.gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null);
			this.InitPose();
			SceneView.RepaintAll();
		}

		protected override void ResetValues()
		{
			base.ResetValues();
			this.ResetBones();
			this.Init();
		}

		protected void ResetBones()
		{
			for (int i = 0; i < this.m_Bones.Length; i++)
			{
				this.m_Bones[i].Reset(base.serializedObject, base.modelBones);
			}
		}

		protected bool IsValidHuman()
		{
			Animator component = base.gameObject.GetComponent<Animator>();
			if (component == null)
			{
				return false;
			}
			Avatar avatar = component.avatar;
			return avatar != null && avatar.isHuman;
		}

		protected void InitPose()
		{
			if (this.IsValidHuman())
			{
				Animator component = base.gameObject.GetComponent<Animator>();
				component.WriteDefaultPose();
				AvatarSetupTool.TransferDescriptionToPose(base.serializedObject, base.root);
			}
		}

		protected void ValidateMapping()
		{
			for (int i = 0; i < this.m_Bones.Length; i++)
			{
				string error;
				this.m_Bones[i].state = this.GetBoneState(i, out error);
				this.m_Bones[i].error = error;
			}
		}

		private void EnableBodyParts(bool[] toggles, params int[] parts)
		{
			for (int i = 0; i < this.m_BodyPartToggle.Length; i++)
			{
				toggles[i] = false;
			}
			for (int j = 0; j < parts.Length; j++)
			{
				int num = parts[j];
				toggles[num] = true;
			}
		}

		private void HandleBodyView(int bodyView)
		{
			if (bodyView == 0)
			{
				this.EnableBodyParts(this.m_BodyPartToggle, new int[]
				{
					1,
					3,
					5,
					7,
					8
				});
			}
			if (bodyView == 1)
			{
				this.EnableBodyParts(this.m_BodyPartToggle, new int[]
				{
					2
				});
			}
			if (bodyView == 2)
			{
				this.EnableBodyParts(this.m_BodyPartToggle, new int[]
				{
					4
				});
			}
			if (bodyView == 3)
			{
				this.EnableBodyParts(this.m_BodyPartToggle, new int[]
				{
					6
				});
			}
		}

		public override void OnInspectorGUI()
		{
			if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
			{
				AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
				for (int i = 0; i < this.m_Bones.Length; i++)
				{
					this.m_Bones[i].Serialize(base.serializedObject);
				}
			}
			this.UpdateSelectedBone();
			if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete) && AvatarMappingEditor.s_SelectedBoneIndex != -1 && AvatarMappingEditor.s_SelectedBoneIndex < this.m_Bones.Length)
			{
				Undo.RegisterCompleteObjectUndo(this, "Avatar mapping modified");
				AvatarSetupTool.BoneWrapper boneWrapper = this.m_Bones[AvatarMappingEditor.s_SelectedBoneIndex];
				boneWrapper.bone = null;
				boneWrapper.state = BoneState.None;
				boneWrapper.Serialize(base.serializedObject);
				Selection.activeTransform = null;
				GUI.changed = true;
				Event.current.Use();
			}
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(string.Empty, "TE NodeBackground", new GUILayoutOption[0]);
			this.m_BodyView = AvatarControl.ShowBoneMapping(this.m_BodyView, new AvatarControl.BodyPartFeedback(this.IsValidBodyPart), this.m_Bones, base.serializedObject, this);
			this.HandleBodyView(this.m_BodyView);
			GUILayout.EndVertical();
			this.m_FoldoutScroll = GUILayout.BeginScrollView(this.m_FoldoutScroll, AvatarMappingEditor.styles.box, new GUILayoutOption[]
			{
				GUILayout.MinHeight(80f),
				GUILayout.MaxHeight(500f),
				GUILayout.ExpandHeight(true)
			});
			this.DisplayFoldout();
			GUILayout.FlexibleSpace();
			GUILayout.EndScrollView();
			if (EditorGUI.EndChangeCheck())
			{
				this.ValidateMapping();
				SceneView.RepaintAll();
			}
			this.DisplayMappingButtons();
			GUILayout.EndVertical();
			if (GUIUtility.hotControl == 0)
			{
				this.TransferPoseIfChanged();
			}
			base.ApplyRevertGUI();
			if (Selection.activeTransform != null)
			{
				if (this.m_CurrentTransformEditor != null && this.m_CurrentTransformEditor.target != Selection.activeTransform)
				{
					UnityEngine.Object.DestroyImmediate(this.m_CurrentTransformEditor);
				}
				if (this.m_CurrentTransformEditor == null)
				{
					this.m_CurrentTransformEditor = Editor.CreateEditor(Selection.activeTransform);
				}
				EditorGUILayout.Space();
				this.m_CurrentTransformEditorFoldout = EditorGUILayout.InspectorTitlebar(this.m_CurrentTransformEditorFoldout, Selection.activeTransform, true);
				if (this.m_CurrentTransformEditorFoldout && this.m_CurrentTransformEditor != null)
				{
					this.m_CurrentTransformEditor.OnInspectorGUI();
				}
			}
			else if (this.m_CurrentTransformEditor != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_CurrentTransformEditor);
				this.m_CurrentTransformEditor = null;
			}
		}

		protected void DebugPoseButtons()
		{
			if (GUILayout.Button("Default Pose", new GUILayoutOption[0]) && this.IsValidHuman())
			{
				Animator component = base.gameObject.GetComponent<Animator>();
				component.WriteDefaultPose();
			}
			if (GUILayout.Button("Description Pose", new GUILayoutOption[0]))
			{
				AvatarSetupTool.TransferDescriptionToPose(base.serializedObject, base.root);
			}
		}

		protected void TransferPoseIfChanged()
		{
			GameObject[] gameObjects = Selection.gameObjects;
			for (int i = 0; i < gameObjects.Length; i++)
			{
				GameObject gameObject = gameObjects[i];
				if (this.TransformChanged(gameObject.transform))
				{
					AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
					this.m_Inspector.Repaint();
					break;
				}
			}
		}

		protected void DisplayMappingButtons()
		{
			GUILayout.BeginHorizontal(string.Empty, AvatarMappingEditor.styles.toolbar, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Rect rect = GUILayoutUtility.GetRect(AvatarMappingEditor.styles.mapping, AvatarMappingEditor.styles.toolbarDropDown);
			if (GUI.Button(rect, AvatarMappingEditor.styles.mapping, AvatarMappingEditor.styles.toolbarDropDown))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(AvatarMappingEditor.styles.clearMapping, false, new GenericMenu.MenuFunction(this.ClearMapping));
				if (this.m_IsBiped)
				{
					genericMenu.AddItem(AvatarMappingEditor.styles.bipedMapping, false, new GenericMenu.MenuFunction(this.PerformBipedMapping));
				}
				genericMenu.AddItem(AvatarMappingEditor.styles.autoMapping, false, new GenericMenu.MenuFunction(this.PerformAutoMapping));
				genericMenu.AddItem(AvatarMappingEditor.styles.loadMapping, false, new GenericMenu.MenuFunction(this.ApplyTemplate));
				genericMenu.AddItem(AvatarMappingEditor.styles.saveMapping, false, new GenericMenu.MenuFunction(this.SaveHumanTemplate));
				genericMenu.DropDown(rect);
			}
			rect = GUILayoutUtility.GetRect(AvatarMappingEditor.styles.pose, AvatarMappingEditor.styles.toolbarDropDown);
			if (GUI.Button(rect, AvatarMappingEditor.styles.pose, AvatarMappingEditor.styles.toolbarDropDown))
			{
				GenericMenu genericMenu2 = new GenericMenu();
				genericMenu2.AddItem(AvatarMappingEditor.styles.resetPose, false, new GenericMenu.MenuFunction(this.CopyPrefabPose));
				if (this.m_IsBiped)
				{
					genericMenu2.AddItem(AvatarMappingEditor.styles.bipedPose, false, new GenericMenu.MenuFunction(this.BipedPose));
				}
				if (this.m_HasSkinnedMesh)
				{
					genericMenu2.AddItem(AvatarMappingEditor.styles.sampleBindPose, false, new GenericMenu.MenuFunction(this.SampleBindPose));
				}
				else
				{
					genericMenu2.AddItem(AvatarMappingEditor.styles.sampleBindPose, false, null);
				}
				genericMenu2.AddItem(AvatarMappingEditor.styles.enforceTPose, false, new GenericMenu.MenuFunction(this.MakePoseValid));
				genericMenu2.DropDown(rect);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		protected void CopyPrefabPose()
		{
			AvatarSetupTool.CopyPose(base.gameObject, base.prefab);
			AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
			this.m_Inspector.Repaint();
		}

		protected void SampleBindPose()
		{
			AvatarSetupTool.SampleBindPose(base.gameObject);
			AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
			this.m_Inspector.Repaint();
		}

		protected void BipedPose()
		{
			AvatarBipedMapper.BipedPose(base.gameObject, this.m_Bones);
			AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
			this.m_Inspector.Repaint();
		}

		protected void MakePoseValid()
		{
			AvatarSetupTool.MakePoseValid(this.m_Bones);
			AvatarSetupTool.TransferPoseToDescription(base.serializedObject, base.root);
			this.m_Inspector.Repaint();
		}

		protected void PerformAutoMapping()
		{
			this.AutoMapping();
			this.ValidateMapping();
			SceneView.RepaintAll();
		}

		protected void PerformBipedMapping()
		{
			this.BipedMapping();
			this.ValidateMapping();
			SceneView.RepaintAll();
		}

		protected void AutoMapping()
		{
			Dictionary<int, Transform> dictionary = AvatarAutoMapper.MapBones(base.gameObject.transform, base.modelBones);
			foreach (KeyValuePair<int, Transform> current in dictionary)
			{
				AvatarSetupTool.BoneWrapper boneWrapper = this.m_Bones[current.Key];
				boneWrapper.bone = current.Value;
				boneWrapper.Serialize(base.serializedObject);
			}
		}

		protected void BipedMapping()
		{
			Dictionary<int, Transform> dictionary = AvatarBipedMapper.MapBones(base.gameObject.transform);
			foreach (KeyValuePair<int, Transform> current in dictionary)
			{
				AvatarSetupTool.BoneWrapper boneWrapper = this.m_Bones[current.Key];
				boneWrapper.bone = current.Value;
				boneWrapper.Serialize(base.serializedObject);
			}
		}

		protected void ClearMapping()
		{
			if (base.serializedObject != null)
			{
				Undo.RegisterCompleteObjectUndo(this, "Clear Mapping");
				AvatarSetupTool.ClearHumanBoneArray(base.serializedObject);
				this.ResetBones();
				this.ValidateMapping();
				SceneView.RepaintAll();
			}
		}

		protected Vector4 QuaternionToVector4(Quaternion rot)
		{
			return new Vector4(rot.x, rot.y, rot.z, rot.w);
		}

		protected Quaternion Vector4ToQuaternion(Vector4 rot)
		{
			return new Quaternion(rot.x, rot.y, rot.z, rot.w);
		}

		protected bool IsAnyBodyPartActive()
		{
			for (int i = 1; i < this.m_BodyPartToggle.Length; i++)
			{
				if (this.m_BodyPartToggle[i])
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateSelectedBone()
		{
			int num = AvatarMappingEditor.s_SelectedBoneIndex;
			if (AvatarMappingEditor.s_SelectedBoneIndex < 0 || AvatarMappingEditor.s_SelectedBoneIndex >= this.m_Bones.Length || this.m_Bones[AvatarMappingEditor.s_SelectedBoneIndex].bone != Selection.activeTransform)
			{
				AvatarMappingEditor.s_SelectedBoneIndex = -1;
				if (Selection.activeTransform != null)
				{
					for (int i = 0; i < this.m_Bones.Length; i++)
					{
						if (this.m_Bones[i].bone == Selection.activeTransform)
						{
							AvatarMappingEditor.s_SelectedBoneIndex = i;
							break;
						}
					}
				}
			}
			if (AvatarMappingEditor.s_SelectedBoneIndex != num)
			{
				List<int> viewsThatContainBone = AvatarControl.GetViewsThatContainBone(AvatarMappingEditor.s_SelectedBoneIndex);
				if (viewsThatContainBone.Count > 0 && !viewsThatContainBone.Contains(this.m_BodyView))
				{
					this.m_BodyView = viewsThatContainBone[0];
				}
			}
		}

		protected void DisplayFoldout()
		{
			Dictionary<Transform, bool> modelBones = base.modelBones;
			EditorGUIUtility.SetIconSize(Vector2.one * 16f);
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUI.color = Color.grey;
			GUILayout.Label(AvatarMappingEditor.styles.dotFrameDotted.image, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUI.color = Color.white;
			GUILayout.Label("Optional Bone", new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			EditorGUILayout.EndHorizontal();
			for (int i = 1; i < this.m_BodyPartToggle.Length; i++)
			{
				if (this.m_BodyPartToggle[i])
				{
					if (AvatarMappingEditor.s_DirtySelection && !this.m_BodyPartFoldout[i])
					{
						for (int j = 0; j < this.m_BodyPartHumanBone[i].Length; j++)
						{
							int num = this.m_BodyPartHumanBone[i][j];
							if (AvatarMappingEditor.s_SelectedBoneIndex == num)
							{
								this.m_BodyPartFoldout[i] = true;
							}
						}
					}
					this.m_BodyPartFoldout[i] = GUILayout.Toggle(this.m_BodyPartFoldout[i], AvatarMappingEditor.styles.BodyPartMapping[i], EditorStyles.foldout, new GUILayoutOption[0]);
					EditorGUI.indentLevel++;
					if (this.m_BodyPartFoldout[i])
					{
						for (int k = 0; k < this.m_BodyPartHumanBone[i].Length; k++)
						{
							int num2 = this.m_BodyPartHumanBone[i][k];
							if (num2 != -1)
							{
								AvatarSetupTool.BoneWrapper boneWrapper = this.m_Bones[num2];
								string text = boneWrapper.humanBoneName;
								if (i == 5 || i == 6 || i == 8)
								{
									text = text.Replace("Right", string.Empty);
								}
								if (i == 3 || i == 4 || i == 7)
								{
									text = text.Replace("Left", string.Empty);
								}
								text = ObjectNames.NicifyVariableName(text);
								Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
								Rect selectRect = controlRect;
								selectRect.width -= 15f;
								boneWrapper.HandleClickSelection(selectRect, num2);
								boneWrapper.BoneDotGUI(new Rect(controlRect.x + EditorGUI.indent, controlRect.y - 1f, 19f, 19f), num2, false, false, base.serializedObject, this);
								controlRect.xMin += 19f;
								Transform transform = EditorGUI.ObjectField(controlRect, new GUIContent(text), boneWrapper.bone, typeof(Transform), true) as Transform;
								if (transform != boneWrapper.bone)
								{
									Undo.RegisterCompleteObjectUndo(this, "Avatar mapping modified");
									boneWrapper.bone = transform;
									boneWrapper.Serialize(base.serializedObject);
									if (transform != null && !modelBones.ContainsKey(transform))
									{
										modelBones[transform] = true;
									}
								}
								if (!string.IsNullOrEmpty(boneWrapper.error))
								{
									GUILayout.BeginHorizontal(new GUILayoutOption[0]);
									GUILayout.Space(EditorGUI.indent + 19f + 4f);
									GUILayout.Label(boneWrapper.error, AvatarMappingEditor.s_Styles.errorLabel, new GUILayoutOption[0]);
									GUILayout.EndHorizontal();
								}
							}
						}
					}
					EditorGUI.indentLevel--;
				}
			}
			AvatarMappingEditor.s_DirtySelection = false;
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}

		private bool TransformChanged(Transform tr)
		{
			SerializedProperty serializedProperty = AvatarSetupTool.FindSkeletonBone(base.serializedObject, tr, false, false);
			if (serializedProperty != null)
			{
				SerializedProperty serializedProperty2 = serializedProperty.FindPropertyRelative(AvatarSetupTool.sPosition);
				if (serializedProperty2 != null && serializedProperty2.vector3Value != tr.localPosition)
				{
					return true;
				}
				SerializedProperty serializedProperty3 = serializedProperty.FindPropertyRelative(AvatarSetupTool.sRotation);
				if (serializedProperty3 != null && serializedProperty3.quaternionValue != tr.localRotation)
				{
					return true;
				}
				SerializedProperty serializedProperty4 = serializedProperty.FindPropertyRelative(AvatarSetupTool.sScale);
				if (serializedProperty4 != null && serializedProperty4.vector3Value != tr.localScale)
				{
					return true;
				}
			}
			return false;
		}

		protected BoneState GetBoneState(int i, out string error)
		{
			error = string.Empty;
			AvatarSetupTool.BoneWrapper bone = this.m_Bones[i];
			if (bone.bone == null)
			{
				return BoneState.None;
			}
			AvatarSetupTool.BoneWrapper boneWrapper = this.m_Bones[AvatarSetupTool.GetFirstHumanBoneAncestor(this.m_Bones, i)];
			if (i == 0 && bone.bone.parent == null)
			{
				error = bone.messageName + " cannot be the root transform";
				return BoneState.InvalidHierarchy;
			}
			if (boneWrapper.bone != null && !bone.bone.IsChildOf(boneWrapper.bone))
			{
				error = bone.messageName + " is not a child of " + boneWrapper.messageName + ".";
				return BoneState.InvalidHierarchy;
			}
			if (i != 23 && boneWrapper.bone != null && boneWrapper.bone != bone.bone && (bone.bone.position - boneWrapper.bone.position).sqrMagnitude < Mathf.Epsilon)
			{
				error = bone.messageName + " has bone length of zero.";
				return BoneState.BoneLenghtIsZero;
			}
			IEnumerable<AvatarSetupTool.BoneWrapper> source = from f in this.m_Bones
			where f.bone == bone.bone
			select f;
			if (source.Count<AvatarSetupTool.BoneWrapper>() > 1)
			{
				error = bone.messageName + " is also assigned to ";
				bool flag = true;
				for (int j = 0; j < this.m_Bones.Length; j++)
				{
					if (i != j && this.m_Bones[i].bone == this.m_Bones[j].bone)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							error += ", ";
						}
						error += ObjectNames.NicifyVariableName(this.m_Bones[j].humanBoneName);
					}
				}
				error += ".";
				return BoneState.Duplicate;
			}
			return BoneState.Valid;
		}

		protected AvatarControl.BodyPartColor IsValidBodyPart(BodyPart bodyPart)
		{
			AvatarControl.BodyPartColor bodyPartColor = AvatarControl.BodyPartColor.Off;
			bool flag = false;
			if (bodyPart != BodyPart.LeftFingers && bodyPart != BodyPart.RightFingers)
			{
				for (int i = 0; i < this.m_BodyPartHumanBone[(int)bodyPart].Length; i++)
				{
					if (this.m_BodyPartHumanBone[(int)bodyPart][i] != -1)
					{
						BoneState state = this.m_Bones[this.m_BodyPartHumanBone[(int)bodyPart][i]].state;
						flag |= (state == BoneState.Valid);
						if (HumanTrait.RequiredBone(this.m_BodyPartHumanBone[(int)bodyPart][i]) && state == BoneState.None)
						{
							return AvatarControl.BodyPartColor.Red;
						}
						if (state != BoneState.Valid && state != BoneState.None)
						{
							return AvatarControl.BodyPartColor.Red;
						}
					}
				}
			}
			else
			{
				bool flag2 = true;
				int num = 3;
				for (int j = 0; j < this.m_BodyPartHumanBone[(int)bodyPart].Length / num; j++)
				{
					bool flag3 = false;
					int num2 = j * num;
					for (int k = num - 1; k >= 0; k--)
					{
						bool flag4 = this.m_Bones[this.m_BodyPartHumanBone[(int)bodyPart][num2 + k]].state == BoneState.Valid;
						flag2 &= flag4;
						if (flag3)
						{
							if (!flag4)
							{
								return (AvatarControl.BodyPartColor)10;
							}
						}
						else
						{
							flag |= (flag3 = (!flag3 && flag4));
						}
					}
				}
				bodyPartColor = ((!flag2) ? AvatarControl.BodyPartColor.IKRed : AvatarControl.BodyPartColor.IKGreen);
			}
			if (!flag)
			{
				return AvatarControl.BodyPartColor.IKRed;
			}
			return AvatarControl.BodyPartColor.Green | bodyPartColor;
		}

		private HumanTemplate OpenHumanTemplate()
		{
			string text = "Assets/";
			string text2 = EditorUtility.OpenFilePanel("Open Human Template", text, "ht");
			if (text2 == string.Empty)
			{
				return null;
			}
			string projectRelativePath = FileUtil.GetProjectRelativePath(text2);
			HumanTemplate humanTemplate = AssetDatabase.LoadMainAssetAtPath(projectRelativePath) as HumanTemplate;
			if (humanTemplate == null && EditorUtility.DisplayDialog("Human Template not found in project", "Import asset '" + text2 + "' into project", "Yes", "No"))
			{
				string text3 = text + FileUtil.GetLastPathNameComponent(text2);
				text3 = AssetDatabase.GenerateUniqueAssetPath(text3);
				FileUtil.CopyFileOrDirectory(text2, text3);
				AssetDatabase.Refresh();
				humanTemplate = (AssetDatabase.LoadMainAssetAtPath(text3) as HumanTemplate);
				if (humanTemplate == null)
				{
					Debug.Log(string.Concat(new string[]
					{
						"Failed importing file '",
						text2,
						"' to '",
						text3,
						"'"
					}));
				}
			}
			return humanTemplate;
		}

		public static bool MatchName(string transformName, string boneName)
		{
			string text = ":";
			char[] separator = text.ToCharArray();
			string[] array = transformName.Split(separator);
			string[] array2 = boneName.Split(separator);
			return transformName == boneName || (array.Length > 1 && array[1] == boneName) || (array2.Length > 1 && transformName == array2[1]) || (array.Length > 1 && array2.Length > 1 && array[1] == array2[1]);
		}

		protected void ApplyTemplate()
		{
			Undo.RegisterCompleteObjectUndo(this, "Apply Template");
			HumanTemplate humanTemplate = this.OpenHumanTemplate();
			if (humanTemplate == null)
			{
				return;
			}
			for (int i = 0; i < this.m_Bones.Length; i++)
			{
				string boneName = humanTemplate.Find(this.m_Bones[i].humanBoneName);
				if (boneName.Length > 0)
				{
					Transform bone = base.modelBones.Keys.FirstOrDefault((Transform f) => AvatarMappingEditor.MatchName(f.name, boneName));
					this.m_Bones[i].bone = bone;
				}
				else
				{
					this.m_Bones[i].bone = null;
				}
				this.m_Bones[i].Serialize(base.serializedObject);
			}
			this.ValidateMapping();
			SceneView.RepaintAll();
		}

		private void SaveHumanTemplate()
		{
			string message = string.Format("Create a new human template", new object[0]);
			string text = EditorUtility.SaveFilePanelInProject("Create New Human Template", "New Human Template", "ht", message);
			if (text == string.Empty)
			{
				return;
			}
			HumanTemplate humanTemplate = new HumanTemplate();
			humanTemplate.ClearTemplate();
			for (int i = 0; i < this.m_Bones.Length; i++)
			{
				if (this.m_Bones[i].bone != null)
				{
					humanTemplate.Insert(this.m_Bones[i].humanBoneName, this.m_Bones[i].bone.name);
				}
			}
			AssetDatabase.CreateAsset(humanTemplate, text);
		}

		public override void OnSceneGUI()
		{
			if (AvatarMappingEditor.s_Styles == null)
			{
				return;
			}
			AvatarSkeletonDrawer.DrawSkeleton(base.root, base.modelBones, this.m_Bones);
			if (GUIUtility.hotControl == 0)
			{
				this.TransferPoseIfChanged();
			}
		}
	}
}
