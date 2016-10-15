using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class AvatarMuscleEditor : AvatarSubEditor
	{
		private class Styles
		{
			public GUIContent[] muscleBodyGroup = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Body"),
				EditorGUIUtility.TextContent("Head"),
				EditorGUIUtility.TextContent("Left Arm"),
				EditorGUIUtility.TextContent("Left Fingers"),
				EditorGUIUtility.TextContent("Right Arm"),
				EditorGUIUtility.TextContent("Right Fingers"),
				EditorGUIUtility.TextContent("Left Leg"),
				EditorGUIUtility.TextContent("Right Leg")
			};

			public GUIContent[] muscleTypeGroup = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Open Close"),
				EditorGUIUtility.TextContent("Left Right"),
				EditorGUIUtility.TextContent("Roll Left Right"),
				EditorGUIUtility.TextContent("In Out"),
				EditorGUIUtility.TextContent("Roll In Out"),
				EditorGUIUtility.TextContent("Finger Open Close"),
				EditorGUIUtility.TextContent("Finger In Out")
			};

			public GUIContent armTwist = EditorGUIUtility.TextContent("Upper Arm Twist");

			public GUIContent foreArmTwist = EditorGUIUtility.TextContent("Lower Arm Twist");

			public GUIContent upperLegTwist = EditorGUIUtility.TextContent("Upper Leg Twist");

			public GUIContent legTwist = EditorGUIUtility.TextContent("Lower Leg Twist");

			public GUIContent armStretch = EditorGUIUtility.TextContent("Arm Stretch");

			public GUIContent legStretch = EditorGUIUtility.TextContent("Leg Stretch");

			public GUIContent feetSpacing = EditorGUIUtility.TextContent("Feet Spacing");

			public GUIContent hasTranslationDoF = EditorGUIUtility.TextContent("Translation DoF");

			public GUIStyle box = new GUIStyle("OL box noexpand");

			public GUIStyle title = new GUIStyle("OL TITLE");

			public GUIStyle toolbar = "TE Toolbar";

			public GUIStyle toolbarDropDown = "TE ToolbarDropDown";

			public GUIContent muscle = EditorGUIUtility.TextContent("Muscles");

			public GUIContent resetMuscle = EditorGUIUtility.TextContent("Reset");

			public Styles()
			{
				this.box.padding = new RectOffset(0, 0, 4, 4);
			}
		}

		private const string sMinX = "m_Limit.m_Min.x";

		private const string sMinY = "m_Limit.m_Min.y";

		private const string sMinZ = "m_Limit.m_Min.z";

		private const string sMaxX = "m_Limit.m_Max.x";

		private const string sMaxY = "m_Limit.m_Max.y";

		private const string sMaxZ = "m_Limit.m_Max.z";

		private const string sModified = "m_Limit.m_Modified";

		private const string sArmTwist = "m_HumanDescription.m_ArmTwist";

		private const string sForeArmTwist = "m_HumanDescription.m_ForeArmTwist";

		private const string sUpperLegTwist = "m_HumanDescription.m_UpperLegTwist";

		private const string sLegTwist = "m_HumanDescription.m_LegTwist";

		private const string sArmStretch = "m_HumanDescription.m_ArmStretch";

		private const string sLegStretch = "m_HumanDescription.m_LegStretch";

		private const string sFeetSpacing = "m_HumanDescription.m_FeetSpacing";

		private const string sHasTranslationDoF = "m_HumanDescription.m_HasTranslationDoF";

		private const float sMuscleMin = -180f;

		private const float sMuscleMax = 180f;

		private const float kPreviewWidth = 80f;

		private const float kNumberWidth = 38f;

		private const float kLineHeight = 16f;

		private static AvatarMuscleEditor.Styles s_Styles;

		protected int[][] m_Muscles = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5
			},
			new int[]
			{
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15,
				16,
				17
			},
			new int[]
			{
				34,
				35,
				36,
				37,
				38,
				39,
				40,
				41,
				42
			},
			new int[]
			{
				52,
				53,
				54,
				55,
				56,
				57,
				58,
				59,
				60,
				61,
				62,
				63,
				64,
				65,
				66,
				67,
				68,
				69,
				70,
				71
			},
			new int[]
			{
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50,
				51
			},
			new int[]
			{
				72,
				73,
				74,
				75,
				76,
				77,
				78,
				79,
				80,
				81,
				82,
				83,
				84,
				85,
				86,
				87,
				88,
				89,
				90,
				91
			},
			new int[]
			{
				18,
				19,
				20,
				21,
				22,
				23,
				24,
				25
			},
			new int[]
			{
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33
			}
		};

		protected int[][] m_MasterMuscle = new int[][]
		{
			new int[]
			{
				0,
				3,
				6,
				9,
				18,
				21,
				23,
				26,
				29,
				31,
				34,
				36,
				39,
				41,
				43,
				45,
				48,
				50
			},
			new int[]
			{
				1,
				4,
				7,
				10
			},
			new int[]
			{
				2,
				5,
				8,
				11
			},
			new int[]
			{
				19,
				24,
				27,
				32,
				35,
				37,
				42,
				44,
				46,
				51
			},
			new int[]
			{
				20,
				22,
				28,
				30,
				38,
				40,
				47,
				49
			},
			new int[]
			{
				52,
				54,
				55,
				56,
				58,
				59,
				60,
				62,
				63,
				64,
				66,
				67,
				68,
				70,
				71,
				72,
				74,
				75,
				76,
				78,
				79,
				80,
				82,
				83,
				84,
				86,
				87,
				88,
				90,
				91
			},
			new int[]
			{
				53,
				57,
				61,
				65,
				69,
				73,
				77,
				81,
				85,
				89
			}
		};

		private bool[] m_MuscleBodyGroupToggle;

		private bool[] m_MuscleToggle;

		private int m_FocusedMuscle;

		[SerializeField]
		private float[] m_MuscleValue;

		[SerializeField]
		private float[] m_MuscleMasterValue;

		[SerializeField]
		protected float m_ArmTwistFactor;

		[SerializeField]
		protected float m_ForeArmTwistFactor;

		[SerializeField]
		protected float m_UpperLegTwistFactor;

		[SerializeField]
		protected float m_LegTwistFactor;

		[SerializeField]
		protected float m_ArmStretchFactor;

		[SerializeField]
		protected float m_LegStretchFactor;

		[SerializeField]
		protected float m_FeetSpacingFactor;

		[SerializeField]
		protected bool m_HasTranslationDoF;

		private string[] m_MuscleName;

		private int m_MuscleCount;

		private SerializedProperty[] m_MuscleMin;

		private SerializedProperty[] m_MuscleMax;

		[SerializeField]
		private float[] m_MuscleMinEdit;

		[SerializeField]
		private float[] m_MuscleMaxEdit;

		private SerializedProperty[] m_Modified;

		private SerializedProperty m_ArmTwistProperty;

		private SerializedProperty m_ForeArmTwistProperty;

		private SerializedProperty m_UpperLegTwistProperty;

		private SerializedProperty m_LegTwistProperty;

		private SerializedProperty m_ArmStretchProperty;

		private SerializedProperty m_LegStretchProperty;

		private SerializedProperty m_FeetSpacingProperty;

		private SerializedProperty m_HasTranslationDoFProperty;

		protected AvatarSetupTool.BoneWrapper[] m_Bones;

		private static AvatarMuscleEditor.Styles styles
		{
			get
			{
				if (AvatarMuscleEditor.s_Styles == null)
				{
					AvatarMuscleEditor.s_Styles = new AvatarMuscleEditor.Styles();
				}
				return AvatarMuscleEditor.s_Styles;
			}
		}

		private static Rect GetSettingsRect(Rect wholeWidthRect)
		{
			wholeWidthRect.xMin += 83f;
			wholeWidthRect.width -= 4f;
			return wholeWidthRect;
		}

		private static Rect GetSettingsRect()
		{
			return AvatarMuscleEditor.GetSettingsRect(GUILayoutUtility.GetRect(10f, 16f));
		}

		private static Rect GetPreviewRect(Rect wholeWidthRect)
		{
			wholeWidthRect.width = 71f;
			wholeWidthRect.x += 5f;
			wholeWidthRect.height = 16f;
			return wholeWidthRect;
		}

		private void HeaderGUI(string h1, string h2)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(h1, AvatarMuscleEditor.styles.title, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			GUILayout.Label(h2, AvatarMuscleEditor.styles.title, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			GUILayout.EndHorizontal();
		}

		private static float PreviewSlider(Rect position, float val)
		{
			val = GUI.HorizontalSlider(AvatarMuscleEditor.GetPreviewRect(position), val, -1f, 1f);
			if (val > -0.1f && val < 0.1f)
			{
				val = 0f;
			}
			return val;
		}

		internal void ResetValuesFromProperties()
		{
			this.m_ArmTwistFactor = this.m_ArmTwistProperty.floatValue;
			this.m_ForeArmTwistFactor = this.m_ForeArmTwistProperty.floatValue;
			this.m_UpperLegTwistFactor = this.m_UpperLegTwistProperty.floatValue;
			this.m_LegTwistFactor = this.m_LegTwistProperty.floatValue;
			this.m_ArmStretchFactor = this.m_ArmStretchProperty.floatValue;
			this.m_LegStretchFactor = this.m_LegStretchProperty.floatValue;
			this.m_FeetSpacingFactor = this.m_FeetSpacingProperty.floatValue;
			this.m_HasTranslationDoF = this.m_HasTranslationDoFProperty.boolValue;
			for (int i = 0; i < this.m_Bones.Length; i++)
			{
				if (this.m_Modified[i] != null)
				{
					bool boolValue = this.m_Modified[i].boolValue;
					int num = HumanTrait.MuscleFromBone(i, 0);
					int num2 = HumanTrait.MuscleFromBone(i, 1);
					int num3 = HumanTrait.MuscleFromBone(i, 2);
					if (num != -1)
					{
						this.m_MuscleMinEdit[num] = ((!boolValue) ? HumanTrait.GetMuscleDefaultMin(num) : this.m_MuscleMin[num].floatValue);
						this.m_MuscleMaxEdit[num] = ((!boolValue) ? HumanTrait.GetMuscleDefaultMax(num) : this.m_MuscleMax[num].floatValue);
					}
					if (num2 != -1)
					{
						this.m_MuscleMinEdit[num2] = ((!boolValue) ? HumanTrait.GetMuscleDefaultMin(num2) : this.m_MuscleMin[num2].floatValue);
						this.m_MuscleMaxEdit[num2] = ((!boolValue) ? HumanTrait.GetMuscleDefaultMax(num2) : this.m_MuscleMax[num2].floatValue);
					}
					if (num3 != -1)
					{
						this.m_MuscleMinEdit[num3] = ((!boolValue) ? HumanTrait.GetMuscleDefaultMin(num3) : this.m_MuscleMin[num3].floatValue);
						this.m_MuscleMaxEdit[num3] = ((!boolValue) ? HumanTrait.GetMuscleDefaultMax(num3) : this.m_MuscleMax[num3].floatValue);
					}
				}
			}
		}

		internal void InitializeProperties()
		{
			this.m_ArmTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_ArmTwist");
			this.m_ForeArmTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_ForeArmTwist");
			this.m_UpperLegTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_UpperLegTwist");
			this.m_LegTwistProperty = base.serializedObject.FindProperty("m_HumanDescription.m_LegTwist");
			this.m_ArmStretchProperty = base.serializedObject.FindProperty("m_HumanDescription.m_ArmStretch");
			this.m_LegStretchProperty = base.serializedObject.FindProperty("m_HumanDescription.m_LegStretch");
			this.m_FeetSpacingProperty = base.serializedObject.FindProperty("m_HumanDescription.m_FeetSpacing");
			this.m_HasTranslationDoFProperty = base.serializedObject.FindProperty("m_HumanDescription.m_HasTranslationDoF");
			for (int i = 0; i < this.m_Bones.Length; i++)
			{
				SerializedProperty serializedProperty = this.m_Bones[i].GetSerializedProperty(base.serializedObject, false);
				if (serializedProperty != null)
				{
					this.m_Modified[i] = serializedProperty.FindPropertyRelative("m_Limit.m_Modified");
					int num = HumanTrait.MuscleFromBone(i, 0);
					int num2 = HumanTrait.MuscleFromBone(i, 1);
					int num3 = HumanTrait.MuscleFromBone(i, 2);
					if (num != -1)
					{
						this.m_MuscleMin[num] = serializedProperty.FindPropertyRelative("m_Limit.m_Min.x");
						this.m_MuscleMax[num] = serializedProperty.FindPropertyRelative("m_Limit.m_Max.x");
					}
					if (num2 != -1)
					{
						this.m_MuscleMin[num2] = serializedProperty.FindPropertyRelative("m_Limit.m_Min.y");
						this.m_MuscleMax[num2] = serializedProperty.FindPropertyRelative("m_Limit.m_Max.y");
					}
					if (num3 != -1)
					{
						this.m_MuscleMin[num3] = serializedProperty.FindPropertyRelative("m_Limit.m_Min.z");
						this.m_MuscleMax[num3] = serializedProperty.FindPropertyRelative("m_Limit.m_Max.z");
					}
				}
			}
		}

		internal void Initialize()
		{
			if (this.m_Bones == null)
			{
				this.m_Bones = AvatarSetupTool.GetHumanBones(base.serializedObject, base.modelBones);
			}
			this.m_FocusedMuscle = -1;
			this.m_MuscleBodyGroupToggle = new bool[this.m_Muscles.Length];
			for (int i = 0; i < this.m_Muscles.Length; i++)
			{
				this.m_MuscleBodyGroupToggle[i] = false;
			}
			this.m_MuscleName = HumanTrait.MuscleName;
			for (int j = 0; j < this.m_MuscleName.Length; j++)
			{
				if (this.m_MuscleName[j].StartsWith("Right"))
				{
					this.m_MuscleName[j] = this.m_MuscleName[j].Substring(5).Trim();
				}
				if (this.m_MuscleName[j].StartsWith("Left"))
				{
					this.m_MuscleName[j] = this.m_MuscleName[j].Substring(4).Trim();
				}
			}
			this.m_MuscleCount = HumanTrait.MuscleCount;
			this.m_MuscleToggle = new bool[this.m_MuscleCount];
			this.m_MuscleValue = new float[this.m_MuscleCount];
			this.m_MuscleMin = new SerializedProperty[this.m_MuscleCount];
			this.m_MuscleMax = new SerializedProperty[this.m_MuscleCount];
			this.m_MuscleMinEdit = new float[this.m_MuscleCount];
			this.m_MuscleMaxEdit = new float[this.m_MuscleCount];
			for (int k = 0; k < this.m_MuscleCount; k++)
			{
				this.m_MuscleToggle[k] = false;
				this.m_MuscleValue[k] = 0f;
				this.m_MuscleMin[k] = null;
				this.m_MuscleMin[k] = null;
			}
			this.m_Modified = new SerializedProperty[this.m_Bones.Length];
			for (int l = 0; l < this.m_Bones.Length; l++)
			{
				this.m_Modified[l] = null;
			}
			this.InitializeProperties();
			this.ResetValuesFromProperties();
			this.m_MuscleMasterValue = new float[this.m_MasterMuscle.Length];
			for (int m = 0; m < this.m_MasterMuscle.Length; m++)
			{
				this.m_MuscleMasterValue[m] = 0f;
			}
		}

		public override void Enable(AvatarEditor inspector)
		{
			base.Enable(inspector);
			this.Initialize();
			this.WritePose();
		}

		public override void OnInspectorGUI()
		{
			if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
			{
				this.WritePose();
			}
			using (new EditorGUI.DisabledScope(!base.avatarAsset.isHuman))
			{
				EditorGUIUtility.labelWidth = 110f;
				EditorGUIUtility.fieldWidth = 40f;
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				this.MuscleGroupGUI();
				EditorGUILayout.Space();
				this.MuscleGUI();
				EditorGUILayout.Space();
				this.PropertiesGUI();
				GUILayout.EndVertical();
				GUILayout.Space(1f);
				GUILayout.EndHorizontal();
				this.DisplayMuscleButtons();
				base.ApplyRevertGUI();
			}
		}

		protected void DisplayMuscleButtons()
		{
			GUILayout.BeginHorizontal(string.Empty, AvatarMuscleEditor.styles.toolbar, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Rect rect = GUILayoutUtility.GetRect(AvatarMuscleEditor.styles.muscle, AvatarMuscleEditor.styles.toolbarDropDown);
			if (GUI.Button(rect, AvatarMuscleEditor.styles.muscle, AvatarMuscleEditor.styles.toolbarDropDown))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(AvatarMuscleEditor.styles.resetMuscle, false, new GenericMenu.MenuFunction(this.ResetMuscleToDefault));
				genericMenu.DropDown(rect);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		protected override void ResetValues()
		{
			base.serializedObject.Update();
			this.ResetValuesFromProperties();
		}

		protected void ResetMuscleToDefault()
		{
			Avatar avatar = null;
			if (base.gameObject != null)
			{
				Animator animator = base.gameObject.GetComponent(typeof(Animator)) as Animator;
				avatar = animator.avatar;
			}
			for (int i = 0; i < this.m_MuscleCount; i++)
			{
				float muscleDefaultMin = HumanTrait.GetMuscleDefaultMin(i);
				float muscleDefaultMax = HumanTrait.GetMuscleDefaultMax(i);
				if (this.m_MuscleMin[i] != null && this.m_MuscleMax[i] != null)
				{
					this.m_MuscleMin[i].floatValue = (this.m_MuscleMinEdit[i] = muscleDefaultMin);
					this.m_MuscleMax[i].floatValue = (this.m_MuscleMaxEdit[i] = muscleDefaultMax);
				}
				int num = HumanTrait.BoneFromMuscle(i);
				if (this.m_Modified[num] != null && num != -1)
				{
					this.m_Modified[num].boolValue = false;
				}
				if (avatar != null)
				{
					avatar.SetMuscleMinMax(i, muscleDefaultMin, muscleDefaultMax);
				}
			}
			this.WritePose();
		}

		protected void UpdateAvatarParameter(HumanParameter parameterId, float value)
		{
			if (base.gameObject != null)
			{
				Animator animator = base.gameObject.GetComponent(typeof(Animator)) as Animator;
				Avatar avatar = animator.avatar;
				avatar.SetParameter((int)parameterId, value);
			}
		}

		protected bool UpdateMuscle(int muscleId, float min, float max)
		{
			Undo.RegisterCompleteObjectUndo(this, "Updated muscle range");
			this.m_MuscleMin[muscleId].floatValue = min;
			this.m_MuscleMax[muscleId].floatValue = max;
			int num = HumanTrait.BoneFromMuscle(muscleId);
			if (num != -1)
			{
				this.m_Modified[num].boolValue = true;
			}
			this.m_FocusedMuscle = muscleId;
			if (base.gameObject != null)
			{
				Animator animator = base.gameObject.GetComponent(typeof(Animator)) as Animator;
				Avatar avatar = animator.avatar;
				avatar.SetMuscleMinMax(muscleId, min, max);
			}
			SceneView.RepaintAll();
			return base.gameObject != null;
		}

		protected void MuscleGroupGUI()
		{
			bool flag = false;
			this.HeaderGUI("Preview", "Muscle Group Preview");
			GUILayout.BeginVertical(AvatarMuscleEditor.styles.box, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect(10f, 16f);
			Rect settingsRect = AvatarMuscleEditor.GetSettingsRect(rect);
			Rect previewRect = AvatarMuscleEditor.GetPreviewRect(rect);
			if (GUI.Button(previewRect, "Reset All", EditorStyles.miniButton))
			{
				for (int i = 0; i < this.m_MuscleMasterValue.Length; i++)
				{
					this.m_MuscleMasterValue[i] = 0f;
				}
				for (int j = 0; j < this.m_MuscleValue.Length; j++)
				{
					this.m_MuscleValue[j] = 0f;
				}
				flag = true;
			}
			GUI.Label(settingsRect, "Reset All Preview Values", EditorStyles.label);
			for (int k = 0; k < this.m_MasterMuscle.Length; k++)
			{
				Rect rect2 = GUILayoutUtility.GetRect(10f, 16f);
				Rect settingsRect2 = AvatarMuscleEditor.GetSettingsRect(rect2);
				float num = this.m_MuscleMasterValue[k];
				this.m_MuscleMasterValue[k] = AvatarMuscleEditor.PreviewSlider(rect2, this.m_MuscleMasterValue[k]);
				if (this.m_MuscleMasterValue[k] != num)
				{
					Undo.RegisterCompleteObjectUndo(this, "Muscle preview");
					for (int l = 0; l < this.m_MasterMuscle[k].Length; l++)
					{
						if (this.m_MasterMuscle[k][l] != -1)
						{
							this.m_MuscleValue[this.m_MasterMuscle[k][l]] = this.m_MuscleMasterValue[k];
						}
					}
				}
				flag |= (this.m_MuscleMasterValue[k] != num && base.gameObject != null);
				GUI.Label(settingsRect2, AvatarMuscleEditor.styles.muscleTypeGroup[k], EditorStyles.label);
			}
			GUILayout.EndVertical();
			if (flag)
			{
				this.WritePose();
			}
		}

		protected void MuscleGUI()
		{
			bool flag = false;
			this.HeaderGUI("Preview", "Per-Muscle Settings");
			GUILayout.BeginVertical(AvatarMuscleEditor.styles.box, new GUILayoutOption[0]);
			for (int i = 0; i < this.m_MuscleBodyGroupToggle.Length; i++)
			{
				Rect rect = GUILayoutUtility.GetRect(10f, 16f);
				Rect settingsRect = AvatarMuscleEditor.GetSettingsRect(rect);
				this.m_MuscleBodyGroupToggle[i] = GUI.Toggle(settingsRect, this.m_MuscleBodyGroupToggle[i], AvatarMuscleEditor.styles.muscleBodyGroup[i], EditorStyles.foldout);
				if (this.m_MuscleBodyGroupToggle[i])
				{
					for (int j = 0; j < this.m_Muscles[i].Length; j++)
					{
						int num = this.m_Muscles[i][j];
						if (num != -1 && this.m_MuscleMin[num] != null && this.m_MuscleMax[num] != null)
						{
							bool flag2 = this.m_MuscleToggle[num];
							rect = GUILayoutUtility.GetRect(10f, (!flag2) ? 16f : 32f);
							settingsRect = AvatarMuscleEditor.GetSettingsRect(rect);
							settingsRect.xMin += 15f;
							settingsRect.height = 16f;
							this.m_MuscleToggle[num] = GUI.Toggle(settingsRect, this.m_MuscleToggle[num], this.m_MuscleName[num], EditorStyles.foldout);
							float num2 = AvatarMuscleEditor.PreviewSlider(rect, this.m_MuscleValue[num]);
							if (this.m_MuscleValue[num] != num2)
							{
								Undo.RegisterCompleteObjectUndo(this, "Muscle preview");
								this.m_FocusedMuscle = num;
								this.m_MuscleValue[num] = num2;
								flag |= (base.gameObject != null);
							}
							if (flag2)
							{
								bool flag3 = false;
								settingsRect.xMin += 15f;
								settingsRect.y += 16f;
								Rect position = settingsRect;
								if (settingsRect.width > 160f)
								{
									Rect position2 = settingsRect;
									position2.width = 38f;
									EditorGUI.BeginChangeCheck();
									this.m_MuscleMinEdit[num] = EditorGUI.FloatField(position2, this.m_MuscleMinEdit[num]);
									flag3 |= EditorGUI.EndChangeCheck();
									position2.x = settingsRect.xMax - 38f;
									EditorGUI.BeginChangeCheck();
									this.m_MuscleMaxEdit[num] = EditorGUI.FloatField(position2, this.m_MuscleMaxEdit[num]);
									flag3 |= EditorGUI.EndChangeCheck();
									position.xMin += 43f;
									position.xMax -= 43f;
								}
								EditorGUI.BeginChangeCheck();
								EditorGUI.MinMaxSlider(position, ref this.m_MuscleMinEdit[num], ref this.m_MuscleMaxEdit[num], -180f, 180f);
								flag3 |= EditorGUI.EndChangeCheck();
								if (flag3)
								{
									this.m_MuscleMinEdit[num] = Mathf.Clamp(this.m_MuscleMinEdit[num], -180f, 0f);
									this.m_MuscleMaxEdit[num] = Mathf.Clamp(this.m_MuscleMaxEdit[num], 0f, 180f);
									flag |= this.UpdateMuscle(num, this.m_MuscleMinEdit[num], this.m_MuscleMaxEdit[num]);
								}
							}
						}
					}
				}
			}
			GUILayout.EndVertical();
			if (flag)
			{
				this.WritePose();
			}
		}

		protected void PropertiesGUI()
		{
			bool flag = false;
			this.HeaderGUI(string.Empty, "Additional Settings");
			GUILayout.BeginVertical(AvatarMuscleEditor.styles.box, new GUILayoutOption[0]);
			this.m_ArmTwistFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.armTwist, this.m_ArmTwistFactor, 0f, 1f);
			if (this.m_ArmTwistProperty.floatValue != this.m_ArmTwistFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Upper arm twist");
				this.m_ArmTwistProperty.floatValue = this.m_ArmTwistFactor;
				this.UpdateAvatarParameter(HumanParameter.UpperArmTwist, this.m_ArmTwistFactor);
				flag = true;
			}
			this.m_ForeArmTwistFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.foreArmTwist, this.m_ForeArmTwistFactor, 0f, 1f);
			if (this.m_ForeArmTwistProperty.floatValue != this.m_ForeArmTwistFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Lower arm twist");
				this.m_ForeArmTwistProperty.floatValue = this.m_ForeArmTwistFactor;
				this.UpdateAvatarParameter(HumanParameter.LowerArmTwist, this.m_ForeArmTwistFactor);
				flag = true;
			}
			this.m_UpperLegTwistFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.upperLegTwist, this.m_UpperLegTwistFactor, 0f, 1f);
			if (this.m_UpperLegTwistProperty.floatValue != this.m_UpperLegTwistFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Upper leg twist");
				this.m_UpperLegTwistProperty.floatValue = this.m_UpperLegTwistFactor;
				this.UpdateAvatarParameter(HumanParameter.UpperLegTwist, this.m_UpperLegTwistFactor);
				flag = true;
			}
			this.m_LegTwistFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.legTwist, this.m_LegTwistFactor, 0f, 1f);
			if (this.m_LegTwistProperty.floatValue != this.m_LegTwistFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Lower leg twist");
				this.m_LegTwistProperty.floatValue = this.m_LegTwistFactor;
				this.UpdateAvatarParameter(HumanParameter.LowerLegTwist, this.m_LegTwistFactor);
				flag = true;
			}
			this.m_ArmStretchFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.armStretch, this.m_ArmStretchFactor, 0f, 1f);
			if (this.m_ArmStretchProperty.floatValue != this.m_ArmStretchFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Arm stretch");
				this.m_ArmStretchProperty.floatValue = this.m_ArmStretchFactor;
				this.UpdateAvatarParameter(HumanParameter.ArmStretch, this.m_ArmStretchFactor);
				flag = true;
			}
			this.m_LegStretchFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.legStretch, this.m_LegStretchFactor, 0f, 1f);
			if (this.m_LegStretchProperty.floatValue != this.m_LegStretchFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Leg stretch");
				this.m_LegStretchProperty.floatValue = this.m_LegStretchFactor;
				this.UpdateAvatarParameter(HumanParameter.LegStretch, this.m_LegStretchFactor);
				flag = true;
			}
			this.m_FeetSpacingFactor = EditorGUI.Slider(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.feetSpacing, this.m_FeetSpacingFactor, 0f, 1f);
			if (this.m_FeetSpacingProperty.floatValue != this.m_FeetSpacingFactor)
			{
				Undo.RegisterCompleteObjectUndo(this, "Feet spacing");
				this.m_FeetSpacingProperty.floatValue = this.m_FeetSpacingFactor;
				this.UpdateAvatarParameter(HumanParameter.FeetSpacing, this.m_FeetSpacingFactor);
				flag = true;
			}
			this.m_HasTranslationDoF = EditorGUI.Toggle(AvatarMuscleEditor.GetSettingsRect(), AvatarMuscleEditor.styles.hasTranslationDoF, this.m_HasTranslationDoF);
			if (this.m_HasTranslationDoFProperty.boolValue != this.m_HasTranslationDoF)
			{
				Undo.RegisterCompleteObjectUndo(this, "Translation DoF");
				this.m_HasTranslationDoFProperty.boolValue = this.m_HasTranslationDoF;
			}
			GUILayout.EndVertical();
			if (flag)
			{
				this.WritePose();
			}
		}

		protected void WritePose()
		{
			if (base.gameObject)
			{
				Animator animator = base.gameObject.GetComponent(typeof(Animator)) as Animator;
				if (animator != null)
				{
					Avatar avatar = animator.avatar;
					if (avatar != null && avatar.isValid && avatar.isHuman)
					{
						AvatarUtility.SetHumanPose(animator, this.m_MuscleValue);
						SceneView.RepaintAll();
					}
				}
			}
		}

		public void DrawMuscleHandle(Transform t, int humanId)
		{
			Animator animator = base.gameObject.GetComponent(typeof(Animator)) as Animator;
			Avatar avatar = animator.avatar;
			int num = HumanTrait.MuscleFromBone(humanId, 0);
			int num2 = HumanTrait.MuscleFromBone(humanId, 1);
			int num3 = HumanTrait.MuscleFromBone(humanId, 2);
			float axisLength = avatar.GetAxisLength(humanId);
			Quaternion quaternion = avatar.GetPreRotation(humanId);
			Quaternion quaternion2 = avatar.GetPostRotation(humanId);
			quaternion = t.parent.rotation * quaternion;
			quaternion2 = t.rotation * quaternion2;
			Color b = new Color(1f, 1f, 1f, 0.5f);
			Quaternion zYRoll = avatar.GetZYRoll(humanId, Vector3.zero);
			Vector3 limitSign = avatar.GetLimitSign(humanId);
			Vector3 vector = quaternion2 * Vector3.right;
			Vector3 p = t.position + vector * axisLength;
			Handles.color = Color.white;
			Handles.DrawLine(t.position, p);
			if (num != -1)
			{
				Quaternion zYPostQ = avatar.GetZYPostQ(humanId, t.parent.rotation, t.rotation);
				float num4 = this.m_MuscleMinEdit[num];
				float num5 = this.m_MuscleMaxEdit[num];
				vector = quaternion2 * Vector3.right;
				Vector3 vector2 = zYPostQ * Vector3.forward;
				Handles.color = Color.black;
				Vector3 vector3 = t.position + vector * axisLength * 0.75f;
				vector = quaternion2 * Vector3.right * limitSign.x;
				Quaternion rotation = Quaternion.AngleAxis(num4, vector);
				vector2 = rotation * vector2;
				Handles.color = Color.yellow;
				Handles.color = Handles.xAxisColor * b;
				Handles.DrawSolidArc(vector3, vector, vector2, num5 - num4, axisLength * 0.25f);
				vector2 = quaternion2 * Vector3.forward;
				Handles.color = Handles.centerColor;
				Handles.DrawLine(vector3, vector3 + vector2 * axisLength * 0.25f);
			}
			if (num2 != -1)
			{
				float num6 = this.m_MuscleMinEdit[num2];
				float num7 = this.m_MuscleMaxEdit[num2];
				vector = quaternion * Vector3.up * limitSign.y;
				Vector3 vector2 = quaternion * zYRoll * Vector3.right;
				Handles.color = Color.black;
				Quaternion rotation2 = Quaternion.AngleAxis(num6, vector);
				vector2 = rotation2 * vector2;
				Handles.color = Color.yellow;
				Handles.color = Handles.yAxisColor * b;
				Handles.DrawSolidArc(t.position, vector, vector2, num7 - num6, axisLength * 0.25f);
			}
			if (num3 != -1)
			{
				float num8 = this.m_MuscleMinEdit[num3];
				float num9 = this.m_MuscleMaxEdit[num3];
				vector = quaternion * Vector3.forward * limitSign.z;
				Vector3 vector2 = quaternion * zYRoll * Vector3.right;
				Handles.color = Color.black;
				Quaternion rotation3 = Quaternion.AngleAxis(num8, vector);
				vector2 = rotation3 * vector2;
				Handles.color = Color.yellow;
				Handles.color = Handles.zAxisColor * b;
				Handles.DrawSolidArc(t.position, vector, vector2, num9 - num8, axisLength * 0.25f);
			}
		}

		public override void OnSceneGUI()
		{
			AvatarSkeletonDrawer.DrawSkeleton(base.root, base.modelBones);
			if (base.gameObject == null)
			{
				return;
			}
			Animator x = base.gameObject.GetComponent(typeof(Animator)) as Animator;
			if (this.m_FocusedMuscle == -1 || x == null)
			{
				return;
			}
			int num = HumanTrait.BoneFromMuscle(this.m_FocusedMuscle);
			if (num != -1)
			{
				this.DrawMuscleHandle(this.m_Bones[num].bone, num);
			}
		}
	}
}
